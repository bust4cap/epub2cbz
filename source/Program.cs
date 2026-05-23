using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using epub2cbz.Properties;
using ExCSS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace epub2cbz
{
    public static class VersionDate
    {
        public static string GetVersionDateYear { get; } = "2026";
        public static string GetVersionDateMonth { get; } = "05";
        public static string GetVersionDateDay { get; } = "19";
        public static int GetVersionNumber { get; } = 1;
    }

    static class Program
    {
        private static MainForm? _mainForm;

        private static int numberEpubs = 0;
        private static int numberCurrentEpub = 0;
        public static CancellationTokenSource cts = new();

        private static readonly HashSet<string> imageExtensions = [".jpeg", ".jpg", ".png", ".webp", ".gif"];
        private static readonly ConcurrentDictionary<string, bool> _processedCbzFiles = new(StringComparer.InvariantCultureIgnoreCase);
        private static readonly char[] invalidPathFileChars = [.. Path.GetInvalidPathChars(), .. Path.GetInvalidFileNameChars()];

        private static string _exportFileExtension = ".cbz";

        private static void ProgressBarStep()
        {
            _mainForm?.BeginInvoke(_mainForm.toolStripProgressBar.PerformStep);
        }

        public static void ClearAndFocusConsole()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.outputBoxConsole.Clear();
                _mainForm.outputBoxConsole.Focus();
            });
        }

        public static void AppendColoredText(string text,
            System.Drawing.Color color)
        {
            _mainForm?.BeginInvoke(() =>
            {
                _mainForm.outputBoxConsole.SelectionStart = _mainForm.outputBoxConsole.TextLength;
                _mainForm.outputBoxConsole.SelectionLength = 0;
                _mainForm.outputBoxConsole.SelectionColor = color;
                _mainForm.outputBoxConsole.AppendText(text);
                _mainForm.outputBoxConsole.SelectionColor = _mainForm.outputBoxConsole.ForeColor;
            });
        }

        private static CompressionLevel GetCompressionLevel()
        {
            int level = PopupSettings.CheckboxStates.DropDownCompressionLevelState;
            if (level == 0) return CompressionLevel.NoCompression;
            else if (level == 1) return CompressionLevel.Fastest;
            else if (level == 2) return CompressionLevel.Optimal;
            else return CompressionLevel.SmallestSize;
        }

        private static readonly Dictionary<string, IImageEncoder> ImageSharpFormatToEncoding = new()
        {
            { ".png", new PngEncoder() },
            { ".jpg", new JpegEncoder() { Quality = 90 } },
            { ".jpeg", new JpegEncoder() { Quality = 90 }},
            { ".gif", new GifEncoder() },
            { ".webp", new WebpEncoder() { Quality = 90 } },
        };

        public static readonly Dictionary<string, (int width, int height)> DeviceResolutionKindle = new()
        {
            { "Kindle 1/2", (600, 670) },
            { "Kindle DX/DXG", (824, 1000) },
            { "Kindle 5-10/Keyboard/Touch", (600, 800) },
            { "Kindle Paperwhite 1/2", (758, 1024) },
            { "Kindle 11/Voyage/Oasis", (1072, 1448) },
            { "Kindle Paperwhite 3/4", (1072, 1448) },
            { "Kindle Oasis 2/3", (1264, 1680) },
            { "Kindle Paperwhite 12/Colorsoft 12", (1272, 1696) },
            { "Kindle Paperwhite 5/Signature Edition", (1236, 1648) },
            { "Kindle Scribe", (1860, 2480) },
        };

        public static readonly Dictionary<string, (int width, int height)> DeviceResolutionKobo = new()
        {
            { "Kobo Mini/Touch", (600, 800) },
            { "Kobo Glo", (768, 1024) },
            { "Kobo Glo HD/Clara HD/Clara 2E/Clara Colour", (1072, 1448) },
            { "Kobo Aura/Nia", (758, 1024) },
            { "Kobo Aura HD", (1080, 1440) },
            { "Kobo Aura H2O", (1080, 1430) },
            { "Kobo Elipsa/Aura ONE", (1404, 1872) },
            { "Kobo Libra H2O/Libra 2/Libra Colour", (1264, 1680) },
            { "Kobo Forma/Sage", (1440, 1920) },
        };

        enum Fail
        {
            Blank,
            Cbz,
            Split
        }

        private sealed class CountingStream(Stream baseStream) : Stream
        {
            private readonly Stream _baseStream = baseStream;
            public long BytesWritten { get; private set; }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _baseStream.Write(buffer, offset, count);
                BytesWritten += count;
            }

            public override void Write(ReadOnlySpan<byte> buffer)
            {
                _baseStream.Write(buffer);
                BytesWritten += buffer.Length;
            }

            public override void Flush() => _baseStream.Flush();
            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => BytesWritten;
            public override long Position { get => BytesWritten; set => throw new NotSupportedException(); }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
        }

        public static string ResolveRootPath(string rootFile, string relativeFile)
        {
            string decodedPath = WebUtility.UrlDecode(relativeFile);

            Uri baseUri = new(new Uri("dummy://root/"), rootFile);
            Uri resolvedUri = new(baseUri, decodedPath);

            return resolvedUri.AbsolutePath.TrimStart('/');
        }

        private static void IntegrateChapters(List<BookInfo.EpubPage> bookFull,
            List<BookInfo.EpubChapter> chapters)
        {
            var chapterMap = chapters.ToDictionary(c => c.Page, c => c.Title.Trim());

            for (int i = 0; i < bookFull.Count; i++)
            {
                if (chapterMap.TryGetValue(Path.GetFileName(bookFull[i].Page), out var title))
                {
                    bookFull[i] = bookFull[i] with
                    {
                        Bookmark = WebUtility.HtmlDecode(title)
                    };
                }
                else if (i == 0)
                {
                    bookFull[i] = bookFull[i] with
                    {
                        Bookmark = "Cover"
                    };
                }
            }
        }

        private static bool CompareImages(Dictionary<string, ZipArchiveEntry> entryMap,
            string firstImage,
            string secondImage,
            string epubFile)
        {
            if (string.IsNullOrEmpty(secondImage)) return false;

            try
            {
                ZipArchiveEntry? firstCoverEntry = entryMap.GetValueOrDefault(firstImage);
                ZipArchiveEntry? secondCoverEntry = entryMap.GetValueOrDefault(secondImage);

                if (firstCoverEntry is null || secondCoverEntry is null) return false;

                using var firstCoverStream = firstCoverEntry.Open();
                using var secondCoverStream = secondCoverEntry.Open();


                using Image<Rgba32> image1 = SixLabors.ImageSharp.Image.Load<Rgba32>(firstCoverStream);
                using Image<Rgba32> image2 = SixLabors.ImageSharp.Image.Load<Rgba32>(secondCoverStream);

                var dhashAlgorithm = new DifferenceHash();
                ulong dimageHash1 = dhashAlgorithm.Hash(image1);
                ulong dimageHash2 = dhashAlgorithm.Hash(image2);
                double dpercentageImageSimilarity = CompareHash.Similarity(dimageHash1, dimageHash2);

                if (dpercentageImageSimilarity >= 97.5)
                {
#if DEBUG
                    AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Duplicate Cover Similarity: "
                        + dpercentageImageSimilarity + Environment.NewLine,
                        System.Drawing.Color.LightGreen);
#endif
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool IsDifferentColor(Rgba32 pixel, Rgba32 compareColor, byte tolerance)
        {
            return Math.Abs(pixel.R - compareColor.R) > tolerance
                   || Math.Abs(pixel.G - compareColor.G) > tolerance
                   || Math.Abs(pixel.B - compareColor.B) > tolerance;
        }

        private static Rgba32 CalculatePixelAverage(Rgba32 firstPixel, Rgba32 secondPixel)
        {
            byte averageRed = (byte)((firstPixel.R + secondPixel.R) / 2);
            byte averageGreen = (byte)((firstPixel.G + secondPixel.G) / 2);
            byte averageBlue = (byte)((firstPixel.B + secondPixel.B) / 2);

            byte averageAlpha = 255;

            Rgba32 averageColor = new(averageRed, averageGreen, averageBlue, averageAlpha);

            return averageColor;
        }

        private static bool ApplyCropping(Image<Rgba32> originalImage)
        {
            using Image<Rgba32> image = originalImage.Clone();

            // Determine border color from middle pixel
            Rgba32 topLeft = image[0, 0];
            Rgba32 topRight = image[image.Width - 1, 0];
            Rgba32 bottomLeft = image[0, image.Height - 1];
            Rgba32 bottomRight = image[image.Width - 1, image.Height - 1];

            Rgba32 borderColorTop = CalculatePixelAverage(topLeft, topRight);
            Rgba32 borderColorBottom = CalculatePixelAverage(bottomLeft, bottomRight);
            Rgba32 borderColorLeft = CalculatePixelAverage(topLeft, bottomLeft);
            Rgba32 borderColorRight = CalculatePixelAverage(topRight, bottomRight);

            Rgba32 black = new(0, 0, 0, 255);
            Rgba32 white = new(255, 255, 255, 255);

            byte colorTolerance = PopupSettings.CheckboxStates.TextBoxCropColorToleranceValue; // 0-255 Standard: 15
            int pixelPadding = PopupSettings.CheckboxStates.TextBoxCropPaddingValue; // 0-XXX Standard: 5
            double deviationTolerance = 0.01 * PopupSettings.CheckboxStates.TextBoxCropDeviationToleranceValue; // 0-100 Standard: 1

            if (pixelPadding > Math.Min(image.Width, image.Height)) pixelPadding = 5;

            int top = 0;
            int bottom = image.Height - 1;
            int left = 0;
            int right = image.Width - 1;

            int horizontalPixelThreshold = (int)(image.Width * deviationTolerance);
            int verticalPixelThreshold = (int)(image.Height * deviationTolerance);

            image.ProcessPixelRows(accessor =>
            {
                // Find Top
                if (!IsDifferentColor(borderColorTop, white, colorTolerance)
                    || !IsDifferentColor(borderColorTop, black, colorTolerance))
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        int nonBorderPixels = 0;
                        Span<Rgba32> row = accessor.GetRowSpan(y);
                        foreach (Rgba32 pixel in row)
                        {
                            if (IsDifferentColor(pixel, borderColorTop, colorTolerance))
                            {
                                nonBorderPixels++;

                                if (horizontalPixelThreshold == 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (nonBorderPixels > horizontalPixelThreshold)
                        {
                            top = y;
                            break;
                        }
                    }
                }

                // Find Bottom
                if (!IsDifferentColor(borderColorBottom, white, colorTolerance)
                    || !IsDifferentColor(borderColorBottom, black, colorTolerance))
                {
                    for (int y = accessor.Height - 1; y >= top; y--)
                    {
                        int nonBorderPixels = 0;
                        Span<Rgba32> row = accessor.GetRowSpan(y);
                        foreach (Rgba32 pixel in row)
                        {
                            if (IsDifferentColor(pixel, borderColorBottom, colorTolerance))
                            {
                                nonBorderPixels++;

                                if (horizontalPixelThreshold == 0)
                                {
                                    break;
                                }
                            }
                        }
                        if (nonBorderPixels > horizontalPixelThreshold)
                        {
                            bottom = y;
                            break;
                        }
                    }
                }
            });

            // Find Left
            if (!IsDifferentColor(borderColorLeft, white, colorTolerance)
                || !IsDifferentColor(borderColorLeft, black, colorTolerance))
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int nonBorderPixels = 0;
                    for (int y = top; y <= bottom; y++)
                    {
                        if (IsDifferentColor(image[x, y], borderColorLeft, colorTolerance))
                        {
                            nonBorderPixels++;

                            if (verticalPixelThreshold == 0)
                            {
                                break;
                            }
                        }
                    }
                    if (nonBorderPixels > verticalPixelThreshold)
                    {
                        left = x;
                        break;
                    }
                }
            }

            // Find Right
            if (!IsDifferentColor(borderColorRight, white, colorTolerance)
                || !IsDifferentColor(borderColorRight, black, colorTolerance))
            {
                for (int x = image.Width - 1; x >= left; x--)
                {
                    int nonBorderPixels = 0;
                    for (int y = top; y <= bottom; y++)
                    {
                        if (IsDifferentColor(image[x, y], borderColorRight, colorTolerance))
                        {
                            nonBorderPixels++;

                            if (verticalPixelThreshold == 0)
                            {
                                break;
                            }
                        }
                    }
                    if (nonBorderPixels > verticalPixelThreshold)
                    {
                        right = x;
                        break;
                    }
                }
            }

            int paddedTop = Math.Max(0, top - pixelPadding);
            int paddedBottom = Math.Min(image.Height - 1, bottom + pixelPadding);
            int paddedLeft = Math.Max(0, left - pixelPadding);
            int paddedRight = Math.Min(image.Width - 1, right + pixelPadding);

            int cropWidth = paddedRight - paddedLeft + 1;
            int cropHeight = paddedBottom - paddedTop + 1;

            if (cropWidth <= 0 ||
                cropHeight <= 0 ||
                (cropWidth == image.Width && cropHeight == image.Height))
            {
                return false;
            }

            originalImage.Mutate(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(paddedLeft, paddedTop, cropWidth, cropHeight)));

            return true;
        }

        private static (int, int) CalculateScaling(int originalWidth, int originalHeight)
        {
            double widthRatio = (double)PopupSettings.CheckboxStates.TextBoxResizeWidthValue / originalWidth;
            double heightRatio = (double)PopupSettings.CheckboxStates.TextBoxResizeHeightValue / originalHeight;

            double scaleFactor = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)Math.Round(originalWidth * scaleFactor, MidpointRounding.AwayFromZero);
            int newHeight = (int)Math.Round(originalHeight * scaleFactor, MidpointRounding.AwayFromZero);

            return (newWidth, newHeight);
        }

        private static bool ApplyResizing(SixLabors.ImageSharp.Image image)
        {
            int maxWidth = PopupSettings.CheckboxStates.TextBoxResizeWidthValue;
            int maxHeight = PopupSettings.CheckboxStates.TextBoxResizeHeightValue;

            if (image.Width == maxWidth && image.Height == maxHeight) return false;

            ResizeOptions options = new()
            {
                Size = new SixLabors.ImageSharp.Size(maxWidth, maxHeight),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Bicubic
            };

            image.Mutate(x => x.Resize(options));

            return true;
        }

        private static bool? CheckDuplicateCover(List<BookInfo.EpubChapter> chapters,
            List<BookInfo.EpubPage> bookFull,
            Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            string epubFilename,
            bool? correctSpread,
            string epubFile)
        {
            if (chapters.Count > 0
                && chapters[0].Page == Path.GetFileName(bookFull[1].Page)
                && (chapters[0].Title.Contains("Cover")
                || chapters[0].Title == "カバー"
                || chapters[0].Title == "表紙"))
            {
                correctSpread = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
            }
            else
            {
                XNamespace opf = "http://www.idpf.org/2007/opf";

                var item = opfDoc.Descendants(opf + "guide").Descendants(opf + "reference").FirstOrDefault(i => (string?)i.Attribute("type") == "cover");
                if (item is not null)
                {
                    string coverPath = (string)item.Attribute("href")!;

                    if (Path.GetFileName(coverPath.Split('#')[0]) == Path.GetFileName(bookFull[1].Page))
                    {
                        correctSpread = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                    }
                    else if (CompareImages(entryMap, bookFull[0].Image, bookFull[1].Image, epubFile))
                    {
                        correctSpread = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                    }
                }
#if DEBUG
                else if (Path.GetFileName(bookFull[0].Image) == Path.GetFileName(bookFull[1].Image))
                {
                    AppendColoredText($"DEBUG: '{epubFilename}' - Image 0 == Image 1" + Environment.NewLine, System.Drawing.Color.HotPink);

                    correctSpread = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                }
#endif
                else if (CompareImages(entryMap, bookFull[0].Image, bookFull[1].Image, epubFile))
                {
                    correctSpread = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                }
            }

            return correctSpread;
        }

        private static bool CheckEPUB(string epubFile)
        {
            using FileStream fs = new(epubFile, FileMode.Open, FileAccess.Read);
            if (fs.Length < 4) return false;

            Span<byte> buffer = stackalloc byte[4];
            fs.ReadExactly(buffer);

            return BinaryPrimitives.ReadUInt32BigEndian(buffer) == 0x504B0304;  // EPUB
        }

        private static bool CheckDRMProtection(Dictionary<string, ZipArchiveEntry> entryMap,
            string filename)
        {
            if (string.IsNullOrEmpty(filename) ||
                !entryMap.TryGetValue(filename, out var fileEntry))
            {
                return true;
            }


            if (filename.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase) ||
                filename.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                filename.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))      // Walter Isaacson - Steve Jobs
            {
                using StreamReader reader = new(fileEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                string fileContent = reader.ReadToEnd();
                if (fileContent.Contains("html", StringComparison.OrdinalIgnoreCase)) return false;
            }
            else if (imageExtensions.Any(ext => filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                if (fileEntry.Length < 4) return false;

                using Stream fs = fileEntry.Open();

                Span<byte> buffer = stackalloc byte[4];
                fs.ReadExactly(buffer);

                uint hexValue = BinaryPrimitives.ReadUInt32BigEndian(buffer);

                if (hexValue == 0xFFD8FFE0 ||  // JPEG
                    hexValue == 0xFFD8FFE1 ||  // JPEG
                    hexValue == 0x89504E47 ||  // PNG
                    hexValue == 0x52494646 ||  // WEBP
                    hexValue == 0x47494638)    // GIF
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsImageBlankWhite(Dictionary<string, ZipArchiveEntry> entryMap,
            string bookImage)
        {
            try
            {
                ZipArchiveEntry? bookEntry = entryMap.GetValueOrDefault(bookImage);

                if (bookEntry == null) return false;

                using Stream imageStream = bookEntry.Open();
                using Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageStream);

                bool isBlank = true;

                image.ProcessPixelRows(accessor =>
                {
                    // Ignore 2 outermost pixels
                    for (int y = 2; y < (accessor.Height - 2); y++)
                    {
                        Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                        for (int x = 2; x < (pixelRow.Length - 2); x++)
                        {
                            ref Rgba32 pixel = ref pixelRow[x];
                            if (pixel.A < 250 || pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                            {
                                isBlank = false;
                                return;
                            }
                        }
                    }
                });

                return isBlank;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool? CheckPageSpread(string readingDirection,
            List<BookInfo.EpubPage> bookFull)
        {
            if (bookFull[1].Doublepage == true) return true;
            if (bookFull.Count < 3) return null;

            int pageSpreadCounter = 0;
            bool foundSpreadInfo = false;
            string firstPage;
            string secondPage;

            if (readingDirection == "YesAndRightToLeft") // right to left
            {
                firstPage = "page-spread-right";
                secondPage = "page-spread-left";
            }
            else // left to right
            {
                firstPage = "page-spread-left";
                secondPage = "page-spread-right";
            }

            for (int i = 1; i < bookFull.Count; i++)
            {
                if (bookFull[i].Doublepage == true)
                {
                    if ((i + pageSpreadCounter) % 2 != 0) return true;
                    pageSpreadCounter++;
                    continue;
                }

                if (string.IsNullOrEmpty(bookFull[i].Spread))
                {
                    continue;
                }

                foundSpreadInfo = true;

                if (bookFull[i].Spread.Contains(firstPage, StringComparison.OrdinalIgnoreCase) && (i + pageSpreadCounter) % 2 != 0
                    || bookFull[i].Spread.Contains(secondPage, StringComparison.OrdinalIgnoreCase) && (i + pageSpreadCounter) % 2 == 0)
                {
                    return true; // Spread seems to be correct
                }
                else if (bookFull[i].Spread.Contains(firstPage, StringComparison.OrdinalIgnoreCase) && (i + pageSpreadCounter) % 2 == 0
                    || bookFull[i].Spread.Contains(secondPage, StringComparison.OrdinalIgnoreCase) && (i + pageSpreadCounter) % 2 != 0)
                {
                    return false; // blank page needed
                }
            }

            return foundSpreadInfo ? false : null;
        }

        private static void FixPageAlignmentPost(List<BookInfo.EpubPage> bookFull,
            string readingDirection)
        {
            bool isRtl = readingDirection == "YesAndRightToLeft";
            bool expectedSideIsLeft = !isRtl;

            List<BookInfo.EpubPage> alignedPages = new(bookFull.Count)
            {
                bookFull[0]
            };

            for (int i = 1; i < bookFull.Count; i++)
            {
                var current = bookFull[i];
                bool insertBlank = false;
                bool reqLeft = false;
                bool reqRight = false;

                if (!string.IsNullOrEmpty(current.Spread))
                {
                    if (current.Spread.Contains("left", StringComparison.OrdinalIgnoreCase)) reqLeft = true;
                    else if (current.Spread.Contains("right", StringComparison.OrdinalIgnoreCase)) reqRight = true;
                }

                bool requiresLeft = reqLeft || (current.Doublepage && !isRtl);  // ltr doublepage needs to start on the left
                bool requiresRight = reqRight || (current.Doublepage && isRtl); // rtl doublepage needs to start on the right

                if ((requiresLeft && !expectedSideIsLeft)       // page should be on the left but is currently on the right
                    || (requiresRight && expectedSideIsLeft))   // page should be on the right but is currently on the left
                {
                    insertBlank = true;
                }

                if (insertBlank)
                {
                    alignedPages.Add(new()
                    {
                        Page = "blank",
                        Spread = expectedSideIsLeft ? "page-spread-left" : "page-spread-right"
                    });

                    expectedSideIsLeft = !expectedSideIsLeft;
                }

                alignedPages.Add(current);

                if (current.Doublepage) expectedSideIsLeft = !isRtl;
                else expectedSideIsLeft = !expectedSideIsLeft;
            }

            if (alignedPages.Count > bookFull.Count)
            {
                bookFull.Clear();
                bookFull.AddRange(alignedPages);
            }
        }

        private static List<BookInfo.EpubPage> ParseOpfPagesXml(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            string opfPath,
            XDocument opfDoc,
            List<BookInfo.EpubPagesIdsSpread> dicPagesIdsSpread)
        {
            List<BookInfo.EpubPage> bookFull = [];
            (string cssPath, var stylesheet) = GetCssFile(opfPath, opfDoc, entryMap);
            const double wideImageRatio = 1.125; // Images have to be at least 12.5% wider than tall to be considered "wide"

            for (int i = 0; i < dicPagesIdsSpread.Count; i++)
            {
                string? imagePath = FindImagePathInFile(entryMap, epubFile, dicPagesIdsSpread[i].Pages.Split('#')[0]);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    if (entryMap.TryGetValue(imagePath, out var bookEntry)
                        && bookEntry is not null)
                    {
                        // Handle wide images first
                        using var streamDimensions = bookEntry.Open();

                        int width = 0;
                        int height = 0;

                        (width, height) = GetImageDimensions(streamDimensions);

                        bool isDoublePage = width >= (height * wideImageRatio);

                        bookFull.Add(new()
                        {
                            Page = dicPagesIdsSpread[i].Pages.Split('#')[0],
                            Image = imagePath,
                            Spread = dicPagesIdsSpread[i].Spread ?? string.Empty,
                            Doublepage = isDoublePage,
                            Height = height,
                            Width = width,
                            Size = bookEntry.Length
                        });
                        continue;
                    }
                }
                //  If image paths are only found in a css file (e.g. The Hobbit)
                else if (dicPagesIdsSpread.Count > i && !string.IsNullOrEmpty(cssPath))
                {
                    string cssImage = FindImagePathInCss(entryMap, stylesheet, dicPagesIdsSpread[i].Pages.Split('#')[0]);

                    if (!string.IsNullOrEmpty(cssImage))
                    {
                        cssImage = ResolveRootPath(cssPath, cssImage);

                        if (!entryMap.TryGetValue(cssImage, out var bookEntry)) { }

                        if (!string.IsNullOrEmpty(cssImage)
                            && bookEntry is not null)
                        {
                            // Handle wide images first
                            using var streamDimensions = bookEntry.Open();

                            int width = 0;
                            int height = 0;

                            (width, height) = GetImageDimensions(streamDimensions);

                            bool isDoublePage = false;

                            ///
                            if (width >= (height * wideImageRatio)) isDoublePage = true;
                            ///

                            bookFull.Add(new BookInfo.EpubPage()
                            {
                                Page = dicPagesIdsSpread[i].Pages.Split('#')[0],
                                Image = cssImage,
                                Spread = dicPagesIdsSpread[i].Spread ?? string.Empty,
                                Doublepage = isDoublePage,
                                Height = height,
                                Width = width,
                                Size = bookEntry.Length
                            });
                        }
                    }
                }

                //  Add blank page if image source is not linked
                if (!bookFull.Any(b => b.Page == dicPagesIdsSpread[i].Pages))
                {
                    bookFull.Add(new BookInfo.EpubPage()
                    {
                        Page = dicPagesIdsSpread[i].Pages.Split('#')[0],
                        Spread = dicPagesIdsSpread[i].Spread ?? string.Empty
                    });
                }
            }

            return bookFull;
        }

        private static XDocument GetOpfDocument(Dictionary<string, ZipArchiveEntry> entryMap,
            string opfPath)
        {
            ZipArchiveEntry fileEntry = entryMap.GetValueOrDefault(opfPath)!;
            using StreamReader reader = new(fileEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string opfContent = reader.ReadToEnd();

            // Replace null characters
            opfContent = opfContent.Replace("\0", string.Empty).Replace("\x01", string.Empty);
            opfContent = opfContent.Replace("&amp;", "&").Replace("&", "&amp;");

            XDocument opfDoc = XDocument.Parse(opfContent);

            return opfDoc;
        }

        private static List<BookInfo.EpubPagesIdsSpread> ParseSpineXml(XDocument opfDoc,
            string opfPath)
        {
            Dictionary<string, string?> pages = [];
            List<BookInfo.EpubPagesIdsSpread> dicPagesIdsSpread = [];

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var opfMetadata = opfDoc.Descendants(opf + "spine").Descendants(opf + "itemref");
            if (opfMetadata is not null)
            {
                foreach (XElement e in opfMetadata)
                {
                    pages.Add(e.Attribute("idref")!.Value, e.Attribute("properties")?.Value);
                }
            }

            foreach (KeyValuePair<string, string?> page in pages)
            {
                var opfManifest = opfDoc.Descendants(opf + "manifest").Descendants(opf + "item").FirstOrDefault(i => (string?)i.Attribute("id") == page.Key);
                if (opfManifest is not null)
                {
                    string? opfHref = (string?)opfManifest.Attribute("href");
                    if (!string.IsNullOrEmpty(opfHref)) opfHref = ResolveRootPath(opfPath, opfHref);

                    dicPagesIdsSpread.Add(new()
                    {
                        Pages = opfHref ?? string.Empty,
                        Ids = page.Key,
                        Spread = page.Value ?? string.Empty
                    });
                }
            }

            return dicPagesIdsSpread;
        }

        private static bool CheckISBN10(string identifier)
        {
            int checksum = 0;
            char chrChecksum;
            for (int i = 0; i < 9; i++)
            {
                try
                {
                    checksum += int.Parse(identifier[i].ToString()) * (10 - i);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            checksum = 11 - (checksum % 11);
            if (checksum == 11) chrChecksum = char.Parse("0");
            else if (checksum == 10) chrChecksum = char.Parse("X");
            else chrChecksum = char.Parse(checksum.ToString());

            if (chrChecksum == identifier[9]) return true;
            else return false;
        }

        private static char CalculateISBN13Checksum(string identifier)
        {
            identifier = "978" + identifier;
            int checksum = 0;
            char chrChecksum;
            for (int i = 0; i < 12; i++)
            {
                checksum += int.Parse(identifier[i].ToString()) * (i % 2 == 0 ? 1 : 3);
            }
            checksum = 10 - (checksum % 10);
            if (checksum == 10) chrChecksum = char.Parse("0");
            else chrChecksum = char.Parse(checksum.ToString());

            return chrChecksum;
        }

        private static string ReturnMetadataISBNCalculated(string source)
        {
            source = source.ToLower().Replace("urn:isbn:", string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty).Trim();
            if (source.Length == 13
                && (source.StartsWith("978") || source.StartsWith("979")))
            {
                source = "ISBN: " + source;

                return source;
            }
            // check if its a valid isbn 10, if it is, convert to isbn 13, if it isnt, discard
            else if (source.Length == 10
                && CheckISBN10(source))
            {
                char checksum = CalculateISBN13Checksum(source[..9]);
                source = "ISBN: 978" + source[..9] + checksum;

                return source;
            }
            return string.Empty;
        }

        private static string ReturnMetadataISBN(XElement xmlMetadata,
            XNamespace opf,
            XNamespace dc)
        {
            // replace comment brackets first
            xmlMetadata = XElement.Parse(xmlMetadata.ToString().Replace("<!--", string.Empty).Replace("-->", string.Empty));

            string? asin = xmlMetadata.Descendants(opf + "meta").FirstOrDefault(i => (string?)i.Attribute("name") == "ASIN")?.Attribute("content")?.Value;
            string? identifier = xmlMetadata.Descendants(dc + "identifier").FirstOrDefault()?.Value;
            string? asinIdentifier = xmlMetadata.Descendants(dc + "identifier")
                .FirstOrDefault(i => (string?)i.Attribute(opf + "scheme") == "MOBI-ASIN")?.Value;

            if (!string.IsNullOrEmpty(asin)
                && asin.Length == 10)
            {
                asin = "ASIN: " + asin;
                return asin.Trim();
            }
            else if (!string.IsNullOrEmpty(asinIdentifier)
                && asinIdentifier.Length == 10)
            {
                asinIdentifier = "ASIN: " + asinIdentifier;
                return asinIdentifier.Trim();
            }
            else if (!string.IsNullOrEmpty(identifier)
                && identifier.StartsWith("urn:asin:", StringComparison.OrdinalIgnoreCase)
                && identifier.Length == 19)
            {
                identifier = "ASIN: " + identifier[9..];
                return identifier.Trim();
            }

            if (!string.IsNullOrEmpty(identifier)
                && !identifier.StartsWith("urn:uuid:", StringComparison.OrdinalIgnoreCase)
                && !identifier.StartsWith("calibre:", StringComparison.OrdinalIgnoreCase))
            {
                return ReturnMetadataISBNCalculated(identifier);
            }

            string? source = xmlMetadata.Descendants(dc + "source").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(source) && source.StartsWith("urn:isbn:", StringComparison.OrdinalIgnoreCase))
            {
                return ReturnMetadataISBNCalculated(source);
            }
            
            return string.Empty;
        }

        private static string ReturnMetadataContributors(string role,
            XElement xmlMetadata,
            XNamespace opf,
            XNamespace dc)
        {
            List<string?> dcRoles = [];
            List<string?> dcRoleList = [.. xmlMetadata.Descendants(opf + "meta")
                .Where(a => a.Attribute("property")?.Value == "role" && a.Attribute("scheme")?.Value == "marc:relators")
                .Where(a => a?.Value.ToLower() == role)
                .Select(a => a.Attribute("refines")?.Value.Replace("#", string.Empty))];
            foreach (string? dcRole in dcRoleList)
            {
                dcRoles.AddRange([.. xmlMetadata.Descendants(dc + "contributor")
                .Where(a => a.Attribute("id")?.Value == dcRole)
                .Select(a => a?.Value)]);
            }
            return string.Join(" & ", dcRoles.Select(a => a?.Trim()));
        }

        private static (Dictionary<string, string?>, string readDir) ParseMetadataXml(XDocument xmlDoc)
        {
            Dictionary<string, string?> metadata = [];

            XNamespace opf = "http://www.idpf.org/2007/opf";
            XNamespace dc = "http://purl.org/dc/elements/1.1/";

            XElement? xmlMetadata = xmlDoc.Descendants(opf + "metadata").FirstOrDefault();
            if (xmlMetadata is not null && MainForm.FormElements.CheckboxComicInfoState)
            {
                if (PopupSettings.CheckboxStates.CheckboxTranslatorState) metadata["Translators"] = ReturnMetadataContributors("trl", xmlMetadata, opf, dc);
                if (PopupSettings.CheckboxStates.CheckboxProducerState) metadata["Producers"] = ReturnMetadataContributors("pro", xmlMetadata, opf, dc);

                if (PopupSettings.CheckboxStates.CheckboxIsbnAsinState) metadata["ISBN"] = ReturnMetadataISBN(xmlMetadata, opf, dc);
#if DEBUG
                metadata["Booktype"] = xmlMetadata.Descendants(opf + "meta").FirstOrDefault(i => (string?)i.Attribute("name") == "book-type")?.Attribute("content")?.Value;
                metadata["Rights"] = xmlMetadata.Descendants(dc + "rights").FirstOrDefault()?.Value;
#endif
                if (PopupSettings.CheckboxStates.CheckboxTitleState
                    || PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                {
                    metadata["Title"] = xmlMetadata.Descendants(dc + "title").FirstOrDefault()?.Value.Trim();
                }

                if (PopupSettings.CheckboxStates.CheckboxSeriesState)
                {
                    metadata["Series"] = xmlMetadata.Descendants(opf + "meta").FirstOrDefault(i => (string?)i.Attribute("name") == "calibre:series")?.Attribute("content")?.Value.Trim();
                }

                if (PopupSettings.CheckboxStates.CheckboxVolumeState)
                {
                    metadata["SeriesIndex"] = xmlMetadata.Descendants(opf + "meta").FirstOrDefault(i => (string?)i.Attribute("name") == "calibre:series_index")?.Attribute("content")?.Value.Trim();
                }

                if (PopupSettings.CheckboxStates.CheckboxAuthorState)
                {
                    List<string> dcAuthors = [.. xmlMetadata.Descendants(dc + "creator")
                        .Select(element => element.Value)
                        .Distinct()];
                    metadata["Authors"] = string.Join(" & ", dcAuthors.Select(a => a.Trim()));
                }

                if (PopupSettings.CheckboxStates.CheckboxLanguageState)
                {
                    List<string> dcLanguages = [.. xmlMetadata.Descendants(dc + "language").Select(element => element.Value.Trim())];
                    dcLanguages = [.. dcLanguages.Select(language => language.Length > 2 ? language[..2] : language)];
                    if (dcLanguages.Count > 0)
                    {
                        metadata["Language"] = dcLanguages.Contains("en") ? dcLanguages[dcLanguages.IndexOf("en")] :
                                     dcLanguages.Contains("ja") ? dcLanguages[dcLanguages.IndexOf("ja")] :
                                     dcLanguages[0];
                    }
                    else metadata["Language"] = null;
                }

                if (PopupSettings.CheckboxStates.CheckboxPublisherState) metadata["Publisher"] = xmlMetadata.Descendants(dc + "publisher").FirstOrDefault()?.Value.Trim();

                if (PopupSettings.CheckboxStates.CheckboxDateState)
                {
                    string? dcDate = xmlMetadata.Descendants(dc + "date").FirstOrDefault()?.Value;
                    if (dcDate?.Length > 10) dcDate = dcDate[..10];
                    metadata["Date"] = dcDate;
                }

                if (PopupSettings.CheckboxStates.CheckboxDescriptionState)
                {
                    string? dcDescription = xmlMetadata.Descendants(dc + "description").FirstOrDefault()?.Value;
                    if (!string.IsNullOrEmpty(dcDescription))
                    {
                        string match = @"\s{2,}";
                        dcDescription = Regex.Replace(dcDescription, match, "\n").Trim();
                    }
                    metadata["Description"] = dcDescription;
                }
            }

            string readingDirection;
            XElement? xmlReadingDirection = xmlDoc.Descendants(opf + "spine").FirstOrDefault();

            if (xmlReadingDirection is not null
                && xmlReadingDirection.Attribute("page-progression-direction")?.Value == "rtl")
            {
                readingDirection = "YesAndRightToLeft";
            }
            else readingDirection = "No";

            return (metadata, readingDirection);
        }

        public static (int width, int height) GetImageDimensions(Stream zipEntryStream)
        {
            try
            {
                ImageInfo image = SixLabors.ImageSharp.Image.Identify(zipEntryStream);
                return (image.Width, image.Height);
            }
			catch (Exception)
			{
                return (0, 0);
			}
        }

        private static List<BookInfo.EpubChapter> GetTocFile(Dictionary<string, ZipArchiveEntry> entryMap,
            List<BookInfo.EpubChapter> newChapters,
            List<BookInfo.EpubPage> bookFull,
            int number)
        {
            List<BookInfo.EpubChapter> newToc = [];
            List<string> toc = [];

            int i = 0;
            // check for 6 pages after and including the initial TOC file
            while (i < 6)
            {
                try
                {
                    string altTocPath = bookFull[number + i].Page;

                    if (!entryMap.TryGetValue(altTocPath, out var altTocEntry)) { }

                    using StreamReader reader = new(altTocEntry!.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                    string altTocContent = reader.ReadToEnd();

                    XDocument altTocDoc = XDocument.Parse(altTocContent);
                    XNamespace altToc = "http://www.w3.org/1999/xhtml";

                    toc = [.. altTocDoc.Descendants(altToc + "body")
                             .Descendants(altToc + "a")
                             .Attributes("href")
                             .Select(attr => attr.Value)];

                    foreach (var entry in toc)
                    {
                        foreach (var book in bookFull)
                        {
                            if (Path.GetFileName(book.Page) == Path.GetFileName(entry.Split('#')[0]))
                            {
                                newChapters.Add(new()
                                {
                                    Title = $"Page {bookFull.IndexOf(book) + 1}",
                                    Page = Path.GetFileName(entry.Split('#')[0])
                                });
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    AppendColoredText(ex.Message + Environment.NewLine, System.Drawing.Color.Red);
#endif
                    break;
                }

                i++;

                if (bookFull.Count - 1 < number + i)
                {
                    break;
                }
            }

            HashSet<string> seen = [];

            foreach (var newChapter in newChapters)
            {
                string fileName = newChapter.Page;

                if (seen.Contains(fileName)) continue;

                seen.Add(fileName);
                newToc.Add(newChapter);
            }

            return newToc;
        }

        private static List<BookInfo.EpubChapter> ParseAlternativeToc(Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            List<BookInfo.EpubChapter> chapters,
            List<BookInfo.EpubPage> bookFull,
            string opfPath)
        {
            List<BookInfo.EpubChapter> newToc = [];
            string altTocFile = string.Empty;

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "guide").Descendants(opf + "reference").FirstOrDefault(i => (string?)i.Attribute("type") == "toc");
            if (item is not null) altTocFile = (string)item.Attribute("href")!;

            if (!string.IsNullOrEmpty(altTocFile))
            {
                altTocFile = ResolveRootPath(opfPath, altTocFile.Split('#')[0]);
                foreach (var book in bookFull)
                {
                    int index = bookFull.IndexOf(book);

                    if (book.Page == altTocFile)
                    {
                        newToc = GetTocFile(entryMap, chapters, bookFull, index);
                        break;
                    }
                }
            }
            else
            {
                newToc = chapters;
            }

            return newToc;
        }

        private static void ParseAlternativeCover(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            XDocument opfDoc,
            List<BookInfo.EpubPage> bookFull,
            string opfPath)
        {
            string coverPath = string.Empty;

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "metadata")
                .Descendants(opf + "meta")
                .FirstOrDefault(i => (string?)i.Attribute("name") == "cover");
            if (item is not null) coverPath = (string)item.Attribute("content")!;

            var coverId = opfDoc.Descendants(opf + "manifest")
                .Descendants(opf + "item")
                .FirstOrDefault(i => (string?)i.Attribute("id") == coverPath);
            if (coverId is not null) coverPath = (string)coverId.Attribute("href")!;

            if (!string.IsNullOrEmpty(coverPath) &&
                imageExtensions.Any(ext => coverPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                string filename = ResolveRootPath(opfPath, coverPath);

                if (!entryMap.TryGetValue(filename, out var bookEntry)) { }

                if (!string.IsNullOrEmpty(filename)
                    && filename != bookFull[0].Image
                    && bookEntry is not null)
                {
#if DEBUG
                    AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Alternative Cover" + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif
                    using var streamDimensions = bookEntry.Open();

                    int width = 0;
                    int height = 0;

                    (width, height) = GetImageDimensions(streamDimensions);

                    bookFull.Insert(0, new()
                    {
                        Page = "Cover",
                        Image = filename,
                        Height = height,
                        Width = width,
                        Size = bookEntry.Length
                    });
                }
            }
        }

        private static Image<Rgba32> CreateBlankImage(int dimensionX,
            int dimensionY)
        {
            var image = new Image<Rgba32>(dimensionX, dimensionY, SixLabors.ImageSharp.Color.White);

            return image;
        }
        
        private static void FillBlankImageResolutions(int width,
            int height,
            List<BookInfo.EpubPage> bookFull)
        {
            for (int i = 0; i < bookFull.Count; i++)
            {
                if (bookFull[i].Page == "blank" || (bookFull[i].Image == string.Empty && bookFull[i].Height == 0 && bookFull[i].Width == 0))
                {
                    bookFull[i] = bookFull[i] with
                    {
                        Height = height,
                        Width = width
                    };
                }
            }
        }

        private static (int, int) GetSinglePageResolution(Dictionary<string, ZipArchiveEntry> entryMap,
            List<BookInfo.EpubPage> bookFull)
        {
            int dimensionX = 0;
            int dimensionY = 0;

            for (int i = 0; i < bookFull.Count; i++)
            {
                if (!string.IsNullOrEmpty(bookFull[i].Image) && i > 0)
                {
                    ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(bookFull[i].Image)!;
                    using var stream = bookEntry.Open();
                    if (imageExtensions.Any(ext => bookFull[i].Image.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        (dimensionX, dimensionY) = GetImageDimensions(stream);

                        if (dimensionY > dimensionX) break;
                    }
                }
                /// If all images are wide, create dimension for a single blank page
                else if (i == bookFull.Count - 1) dimensionX /= 2;
            }

            if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
            {
                (int resizedSingleWidth, int resizedSingleHeight) = CalculateScaling(dimensionX, dimensionY);

                if (resizedSingleHeight > 0
                    && resizedSingleWidth > 0)
                {
                    dimensionX = resizedSingleWidth;
                    dimensionY = resizedSingleHeight;
                }
            }

            return (dimensionX, dimensionY);
        }

        private static void ExtractImageStreamsSimple(Dictionary<string, ZipArchiveEntry> entryKeysNew,
            string targetCbz)
        {
            bool hasDuplicates = entryKeysNew.Keys
                .Select(Path.GetFileName)
                .GroupBy(filename => filename, StringComparer.InvariantCultureIgnoreCase)
                .Any(group => group.Count() > 1);

            using ZipArchive destinationArchive = ZipFile.Open(targetCbz, ZipArchiveMode.Create);

            foreach (KeyValuePair<string, ZipArchiveEntry> page in entryKeysNew)
            {
                var compressionLevel = CompressionLevel.NoCompression;

                ZipArchiveEntry destinationEntry = null!;

                // If there are duplicate filenames, extract them with their original folder
                if (hasDuplicates) destinationEntry = destinationArchive.CreateEntry(page.Key, compressionLevel);

                // If there are no duplicate filenames, extract them all to the root folder
                else destinationEntry = destinationArchive.CreateEntry(Path.GetFileName(page.Key), compressionLevel);

                using Stream sourceStream = page.Value.Open();
                using Stream destinationStream = destinationEntry.Open();
                sourceStream.CopyTo(destinationStream);
            }
        }

        private static void ExtractImageStreams(Dictionary<string, ZipArchiveEntry> entryMap,
            string targetCbz,
            List<BookInfo.EpubPage> bookFull,
            string readingDirection)
        {
            if (File.Exists(targetCbz)
                || !_processedCbzFiles.TryAdd(targetCbz, true))
            {
                throw new Exception(Fail.Cbz.ToString());
            }

            using ZipArchive destinationArchive = ZipFile.Open(targetCbz, ZipArchiveMode.Create);

            (int singleWidth, int singleHeight) = GetSinglePageResolution(entryMap, bookFull);

            bool doSplit = PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState;
            int numberWideImages = doSplit ? bookFull.Count(page => page.Doublepage == true) : 0;

            string currentChapterFolder = string.Empty;
            int totalChapters = bookFull.Count(page => !string.IsNullOrEmpty(page.Bookmark));
            int currentChapterIndex = 0;

            var compressionLevel = GetCompressionLevel();
            bool doCrop = PopupSettings.CheckboxStates.CheckboxCropImagesState;
            bool doResize = PopupSettings.CheckboxStates.CheckboxResizeImagesState
                            && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                            && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0;

            int padLength = (bookFull.Count + numberWideImages - 1).ToString().Length;
            byte[]? cachedBlankImageData = null;

            for (int i = 0; i < bookFull.Count; i++)
            {
                string prefix = (i == 0) ? "cover_" : "p_";

                if (PopupSettings.CheckboxStates.CheckboxChapterFoldersState && totalChapters > 1 && !string.IsNullOrEmpty(bookFull[i].Bookmark))
                {
                    string valueBookmark = bookFull[i].Bookmark;
                    foreach (char c in invalidPathFileChars) valueBookmark = valueBookmark.Replace(c, '_');
                    while (valueBookmark.Contains("__")) valueBookmark = valueBookmark.Replace("__", "_");

                    currentChapterFolder = $"{currentChapterIndex.ToString().PadLeft((totalChapters - 1).ToString().Length, '0')} - {valueBookmark}/";
                    currentChapterIndex++;
                }

                string baseFileNameFirst = $"{prefix}{i.ToString().PadLeft(padLength, '0')}{Path.GetExtension(bookFull[i].Image)}";
                string fullEntryPathFirst = $"{currentChapterFolder}{baseFileNameFirst}";

                if (!string.IsNullOrEmpty(bookFull[i].Image) &&
                    imageExtensions.Any(ext => bookFull[i].Image.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(bookFull[i].Image)!;
                    bool isDoublePage = doSplit && i > 0 && bookFull[i].Doublepage == true;
                    string extension = Path.GetExtension(bookFull[i].Image).ToLowerInvariant();

                    if (isDoublePage)
                    {
                        string baseFileNameSecond = $"{prefix}{(i + 1).ToString().PadLeft(padLength, '0')}{extension}";
                        string fullEntryPathSecond = $"{currentChapterFolder}{baseFileNameSecond}";

                        try
                        {
                            using Stream sourceStream = bookEntry.Open();
                            using Image<Rgba32> imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(sourceStream);

                            if (doCrop) ApplyCropping(imageToProcess);

                            int halfWidth = imageToProcess.Width / 2;
                            int imageHeight = imageToProcess.Height;
                            IImageEncoder encoder = ImageSharpFormatToEncoding[extension];

                            using Image<Rgba32> leftImage = imageToProcess.Clone(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, halfWidth, imageHeight)));
                            if (doResize) ApplyResizing(leftImage);

                            using Image<Rgba32> rightImage = imageToProcess.Clone(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(halfWidth, 0, imageToProcess.Width - halfWidth, imageHeight)));
                            if (doResize) ApplyResizing(rightImage);

                            Image<Rgba32> firstImage = readingDirection == "YesAndRightToLeft" ? rightImage : leftImage;
                            Image<Rgba32> secondImage = readingDirection == "YesAndRightToLeft" ? leftImage : rightImage;

                            using (Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open())
                            using (CountingStream countingStreamFirst = new(destinationStream))
                            {
                                firstImage.Save(countingStreamFirst, encoder);
                                bookFull[i] = bookFull[i] with
                                {
                                    Height = firstImage.Height,
                                    Width = firstImage.Width,
                                    Size = countingStreamFirst.BytesWritten
                                };
                            }

                            using (Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathSecond, compressionLevel).Open())
                            using (CountingStream countingStreamSecond = new(destinationStream))
                            {
                                secondImage.Save(countingStreamSecond, encoder);
                                bookFull.Insert(i + 1, new()
                                {
                                    Page = "second spread page",
                                    Height = secondImage.Height,
                                    Width = secondImage.Width,
                                    Size = countingStreamSecond.BytesWritten
                                });
                            }
                            i++;
                        }
                        catch (Exception)
                        {
                            throw new Exception(Fail.Split.ToString());
                        }
                    }
                    else
                    {
                        if (doCrop || doResize)
                        {
                            bool cropped, resized;

                            using (Stream sourceStream = bookEntry.Open())
                            using (Image<Rgba32> imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(sourceStream))
                            {
                                cropped = doCrop && ApplyCropping(imageToProcess);
                                resized = doResize && ApplyResizing(imageToProcess);

                                if (cropped || resized)
                                {
                                    IImageEncoder encoder = ImageSharpFormatToEncoding[extension];

                                    using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                                    using CountingStream countingStream = new(destinationStream);

                                    imageToProcess.Save(countingStream, encoder);

                                    bookFull[i] = bookFull[i] with
                                    {
                                        Height = imageToProcess.Height,
                                        Width = imageToProcess.Width,
                                        Size = countingStream.BytesWritten
                                    };
                                }
                            }

                            if (!cropped && !resized)
                            {
                                using Stream originalSourceStream = bookEntry.Open();
                                using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                                originalSourceStream.CopyTo(destinationStream);

                                bookFull[i] = bookFull[i] with { Size = bookEntry.Length };
                            }
                        }
                        else
                        {
                            using Stream sourceStream = bookEntry.Open();
                            using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                            sourceStream.CopyTo(destinationStream);

                            bookFull[i] = bookFull[i] with { Size = bookEntry.Length };
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (cachedBlankImageData == null)
                        {
                            using var blankImage = CreateBlankImage(singleWidth, singleHeight);
                            using var memoryStream = new MemoryStream();
                            blankImage.SaveAsPng(memoryStream);
                            cachedBlankImageData = memoryStream.ToArray();
                        }

                        bookFull[i] = bookFull[i] with
                        {
                            Height = singleHeight,
                            Width = singleWidth,
                            Size = cachedBlankImageData.Length
                        };

                        using Stream sourceStream = new MemoryStream(cachedBlankImageData);
                        using Stream destinationStream = destinationArchive.CreateEntry($"{currentChapterFolder}{prefix}{i.ToString().PadLeft(padLength, '0')}.png", compressionLevel).Open();
                        sourceStream.CopyTo(destinationStream);
                    }
                    catch (Exception)
                    {
                        throw new Exception(Fail.Blank.ToString());
                    }
                }
            }
        }

        private static List<BookInfo.EpubChapter> ParseEpubToc(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            XDocument opfDoc,
            string opfPath)
        {
            List<BookInfo.EpubChapter> chapters = [];
            List<string> navPaths = GetNcxFile(opfDoc, opfPath);

            if (navPaths.Count > 0)
            {
                List<BookInfo.EpubChapter> xhtmlChapters = [];
                List<BookInfo.EpubChapter> ncxChapters = [];

                foreach (string navPath in navPaths)
                {
                    ZipArchiveEntry tocEntry = entryMap.GetValueOrDefault(navPath)!;
                    using StreamReader reader = new(tocEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                    string tocContent = reader.ReadToEnd();

#pragma warning disable SYSLIB1045 // "GeneratedRegexAttribute"
                    tocContent = Regex.Replace(tocContent, @"&(?!([a-zA-Z]+|#\d+|#x[a-zA-Z0-9]+);)", "&amp;");
#pragma warning restore SYSLIB1045

                    XDocument tocDoc = XDocument.Parse(tocContent);
                    XNamespace ops = "http://www.idpf.org/2007/ops";
                    XNamespace xhtml = "http://www.w3.org/1999/xhtml";
                    XNamespace ncx = "http://www.daisy.org/z3986/2005/ncx/";

                    if (navPath.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase))
                    {
                        var opfMetadata = tocDoc.Descendants(xhtml + "nav")
                            .FirstOrDefault(i => (string?)i.Attribute(ops + "type") == "toc")?
                            .Descendants(xhtml + "a")
                            .Select(a => new { Href = a.Attribute("href")!.Value, Name = a.Value })
                            .ToLookup(item => item.Href, item => item.Name);
                        if (opfMetadata is not null)
                        {
                            foreach (var e in opfMetadata)
                            {
                                string title = string.Join(" - ", e);
                                var page = e.Key.ToString().Split('#')[0] ?? string.Empty;

                                page = ResolveRootPath(navPath, page);

                                xhtmlChapters.Add(new()
                                {
                                    Title = title,
                                    Page = Path.GetFileName(page)
                                });
                            }
                        }
                    }
                    else if (navPath.EndsWith(".ncx", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var navPoint in tocDoc.Descendants(ncx + "navPoint"))
                        {
                            string title = navPoint.Descendants(ncx + "text").FirstOrDefault()?.Value.Trim() ?? string.Empty;
                            var page = navPoint.Descendants(ncx + "content").FirstOrDefault()?.Attribute("src")?.Value.Split('#')[0] ?? string.Empty;

                            page = ResolveRootPath(navPath, page);

                            ncxChapters.Add(new()
                            {
                                Title = title,
                                Page = Path.GetFileName(page)
                            });
                        }
                    }
                }

                if (xhtmlChapters.Count >= ncxChapters.Count) chapters = xhtmlChapters;
                else chapters = ncxChapters;
            }
            else
            {
                AppendColoredText(string.Format(Resources.NcxOrNavNotInOpf, epubFile) + Environment.NewLine, System.Drawing.Color.Blue);
            }

            for (int i = chapters.Count - 2; i >= 0; i--)
            {
                if (chapters[i].Page == chapters[i + 1].Page)
                {
                    chapters[i] = chapters[i] with
                    {
                        Title = chapters[i].Title + " - " + chapters[i + 1].Title
                    };
                    chapters.RemoveAt(i + 1);
                }
            }

            return chapters;
        }

        private static bool TryExtractUrl(string? value, out string path)
        {
            path = string.Empty;
            if (!string.IsNullOrWhiteSpace(value) &&
                value.StartsWith("url(", StringComparison.OrdinalIgnoreCase) &&
                value.EndsWith(")", StringComparison.OrdinalIgnoreCase))
            {
                path = value[4..^1].Trim('\'', '\"');
                return true;
            }
            return false;
        }

        private static string FindImagePathInCss(Dictionary<string, ZipArchiveEntry> entryMap,
            Stylesheet? stylesheet,
            string xhtmlPage)
        {
            if (stylesheet is null) return string.Empty;

            if (string.IsNullOrEmpty(xhtmlPage)) return string.Empty;

            if (!xhtmlPage.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase) &&
                !xhtmlPage.EndsWith(".html", StringComparison.OrdinalIgnoreCase) &&
                !xhtmlPage.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (!entryMap.TryGetValue(xhtmlPage, out var xhtmlSource))
            {
                return string.Empty;
            }

            using StreamReader xhtmlReader = new(xhtmlSource.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string xhtmlFileContent = xhtmlReader.ReadToEnd();

            XDocument fileDoc;
            try
            {
                fileDoc = XDocument.Parse(xhtmlFileContent);
            }
            catch
            {
                return string.Empty;
            }

            XNamespace ns = fileDoc.Root!.Name.Namespace;

            var divInfo = fileDoc.Descendants(ns + "body").Descendants(ns + "div").FirstOrDefault();
            var divId = divInfo?.Attribute("id");
            var divClass = divInfo?.Attribute("class");

            if (divId is not null && !string.IsNullOrWhiteSpace(divId.Value))
            {
                string selector = "#" + divId.Value;
                var rule = stylesheet.StyleRules.FirstOrDefault(r => r.SelectorText == selector);

                if (TryExtractUrl(rule?.Style?.BackgroundImage, out var extractedPath))
                {
                    return extractedPath;
                }
            }

            if (divClass is not null && !string.IsNullOrWhiteSpace(divClass.Value))
            {
                string[] classNames = divClass.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var className in classNames)
                {
                    var rule = stylesheet.StyleRules.FirstOrDefault(r => r.SelectorText == $"div.{className}");
                    if (TryExtractUrl(rule?.Style?.BackgroundImage, out var extractedPathDiv))
                    {
                        return extractedPathDiv;
                    }

                    rule = stylesheet.StyleRules.FirstOrDefault(r => r.SelectorText == $".{className}");
                    if (TryExtractUrl(rule?.Style?.BackgroundImage, out var extractedPath))
                    {
                        return extractedPath;
                    }
                }
            }

            return string.Empty;
        }

        private static string? FindImagePathInFile(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            string actualFilename)
        {
            string imagePath = string.Empty;

            if (string.IsNullOrEmpty(actualFilename)) return string.Empty;

            if (actualFilename.EndsWith(".xhtml", StringComparison.OrdinalIgnoreCase) ||
                actualFilename.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                actualFilename.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                if (!entryMap.TryGetValue(actualFilename, out var fileEntry))
                {
                    return null;
                }

                using StreamReader reader = new(fileEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                string fileContent = reader.ReadToEnd();

#if DEBUG
                /// Likely the last page of samples
                if (fileContent.StartsWith("<HTML>"))
                {
                    AppendColoredText($"HTML DETECTED: '{epubFile}'" + Environment.NewLine, System.Drawing.Color.Purple);
                    return null;
                }
#endif

                XDocument fileDoc;
                try
                {
                    fileDoc = XDocument.Parse(fileContent);
                }
                catch
                {
                    return null;
                }

                XNamespace svg = "http://www.w3.org/2000/svg";
                XNamespace xlink = "http://www.w3.org/1999/xlink";

                XNamespace ns = fileDoc.Root!.Name.Namespace;

                string itemSrc = string.Empty;

                var itemSrcList = fileDoc.Descendants(ns + "body").Descendants(ns + "img").Attributes("src").ToList();
                if (itemSrcList.Count > 1)
                {
                    if (CheckXAtrributeListUniformity(itemSrcList, out string uniformValue))
                    {
                        itemSrc = ResolveRootPath(actualFilename, uniformValue);
                    }
                    else
                    {
                        var largestImage = itemSrcList
                            .Select(attr => ResolveRootPath(actualFilename, attr.Value))
                            .Where(entryMap.ContainsKey)
                            .Select(path => (Path: path, Size: entryMap[path].Length))
                            .MaxBy(img => img.Size);
                        if (largestImage.Path is null)
                        {
                            return null;
                        }

                        itemSrc = largestImage.Path;
                    }
                }
                else if (itemSrcList.Count == 1)
                {
                    itemSrc = ResolveRootPath(actualFilename, itemSrcList[0].Value);
                }

                if (string.IsNullOrEmpty(itemSrc))
                {
                    var itemXlink = fileDoc.Descendants(ns + "body").Descendants(svg + "image").FirstOrDefault();
                    if (itemXlink is not null)
                    {
                        imagePath = ResolveRootPath(actualFilename, (string)itemXlink.Attribute(xlink + "href")!);
                    }
                }
                else imagePath = itemSrc;
            }
            else if (imageExtensions.Any(ext => actualFilename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                imagePath = actualFilename;
            }

            return imagePath;
        }

        private static bool CheckXAtrributeListUniformity(List<XAttribute> list, out string value)
        {
            value = string.Empty;

            ReadOnlySpan<XAttribute> span = CollectionsMarshal.AsSpan(list);
            string firstValue = span[0].Value;

            for (int i = 1; i < span.Length; i++)
            {
                if (span[i].Value != firstValue)
                {
                    return false;
                }
            }

            value = firstValue;
            return true;
        }

        private static (string seriesName, string volumeNumber, string isVolumeOrChapter) GetVolumeAndChapterNumber(string epubFilename)
        {
            int vIndex = epubFilename.LastIndexOf("v", StringComparison.OrdinalIgnoreCase);
            if (vIndex > 0
                && vIndex + 1 < epubFilename.Length
                && epubFilename[vIndex - 1] == ' ')
            {
                if (int.TryParse(epubFilename[(vIndex + 1)..], out int volumeNumber))
                {
                    return (epubFilename[..vIndex].TrimEnd(), volumeNumber.ToString(), "v");
                }
            }

            int cIndex = epubFilename.LastIndexOf("c", StringComparison.OrdinalIgnoreCase);
            if (cIndex > 0
                && cIndex + 1 < epubFilename.Length
                && epubFilename[cIndex - 1] == ' ')
            {
                string chapterNumberPart = epubFilename[(cIndex + 1)..];
                string[] parts = chapterNumberPart.Split('.');

                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int mainChapter) && int.TryParse(parts[1], out int subChapter))
                    {
                        string formattedChapterNumber = $"{mainChapter}.{subChapter}"; 
                        
                        return (epubFilename[..cIndex].TrimEnd(), formattedChapterNumber, "c");
                    }
                }
                else if (parts.Length == 1)
                {
                    if (int.TryParse(parts[0], out int mainChapter))
                    {
                        return (epubFilename[..cIndex].TrimEnd(), mainChapter.ToString(), "c");
                    }
                }
            }

            return (epubFilename.TrimEnd(), string.Empty, string.Empty);
        }

        private static void WriteComicInfoXml(string targetCbz,
            string epubFilename,
            string readingDirection,
            List<BookInfo.EpubPage> bookFull,
            Dictionary<string, string?> metadata)
        {
            (string seriesName, string volumeNumber, string isVolumeOrChapter) = GetVolumeAndChapterNumber(Path.GetFileName(epubFilename));

            string comicInfo = "ComicInfo.xml";

            var compressionLevel = GetCompressionLevel();

            Encoding utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = utf8WithoutBom
            };

            using ZipArchive archive = MainForm.FormElements.CheckboxExtractImagesState
                ? ZipFile.Open(targetCbz, ZipArchiveMode.Update)
                : ZipFile.Open(targetCbz, ZipArchiveMode.Create);
            ZipArchiveEntry entry = archive.CreateEntry(comicInfo, compressionLevel);

            using Stream entryStream = entry.Open();
            using XmlWriter xmlWriter = XmlWriter.Create(entryStream, settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("ComicInfo");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

            if (PopupSettings.CheckboxStates.CheckboxTitleState
                && metadata.TryGetValue("Title", out string? titleValue)
                && !string.IsNullOrEmpty(titleValue))
            {
                xmlWriter.WriteElementString("Title", titleValue);
            }
            if (PopupSettings.CheckboxStates.CheckboxSeriesState)
            {
                if (PopupSettings.CheckboxStates.CheckboxReplaceSeriesState
                    && !string.IsNullOrWhiteSpace(PopupSettings.CheckboxStates.TextboxReplaceSeriesState))
                {
                    xmlWriter.WriteElementString("Series", PopupSettings.CheckboxStates.TextboxReplaceSeriesState.Trim());
                }
                else if (metadata.TryGetValue("Series", out string? seriesValue)
                    && !string.IsNullOrEmpty(seriesValue))
                {
                    xmlWriter.WriteElementString("Series", seriesValue);
                }
                else
                {
                    if (seriesName.EndsWith('_')) seriesName = seriesName[..^1] + "?";

                    xmlWriter.WriteElementString("Series", seriesName
                        .Replace(" 1_2 ", " 1/2 ")
                        .Replace("_ ", ": "));
                }
            }
            if (PopupSettings.CheckboxStates.CheckboxVolumeState)
            {
                if (metadata.TryGetValue("SeriesIndex", out string? seriesIndexValue)
                    && !string.IsNullOrEmpty(seriesIndexValue))
                {
                    xmlWriter.WriteElementString("Volume", seriesIndexValue);
                }
                else if (!string.IsNullOrEmpty(volumeNumber))
                {
                    if (isVolumeOrChapter == "v") xmlWriter.WriteElementString("Volume", volumeNumber);
                    else if (isVolumeOrChapter == "c") xmlWriter.WriteElementString("Number", volumeNumber);
                }
            }
            if (PopupSettings.CheckboxStates.CheckboxDescriptionState
                && metadata.TryGetValue("Description", out string? descriptionValue)
                && !string.IsNullOrEmpty(descriptionValue))
            {
                xmlWriter.WriteElementString("Summary", WebUtility.HtmlDecode(descriptionValue)
                    .Replace("<div>", string.Empty).Replace("</div>", string.Empty)
                    .Replace("<p>", string.Empty).Replace("</p>", string.Empty)
                    .Replace("<br>", "\n")
                    .Replace("\u00A0", " ")); // non breaking space (Mail)
            }
            xmlWriter.WriteElementString("Notes", "Created using: epub2cbz");
            if (PopupSettings.CheckboxStates.CheckboxDateState
                && metadata.TryGetValue("Date", out string? dateValue)
                && !string.IsNullOrEmpty(dateValue))
            {
                DateTime? dateParsed = new();
                try
                {
                    dateParsed = DateTime.ParseExact(dateValue, "yyyy-MM-dd", null);
                }
                catch
                {
                    try
                    {
                        dateParsed = DateTime.ParseExact(dateValue, "yyyy-dd-MM", null);
                    }
                    catch
                    {
                        dateParsed = null;
                    }
                }
                finally
                {
                    if (dateParsed.HasValue)
                    {
                        DateTime dt = dateParsed.Value;
                        xmlWriter.WriteElementString("Year", dt.Year.ToString());
                        xmlWriter.WriteElementString("Month", dt.Month.ToString());
                        xmlWriter.WriteElementString("Day", dt.Day.ToString());
                    }
                }
            }
            if (PopupSettings.CheckboxStates.CheckboxAuthorState
                && metadata.TryGetValue("Authors", out string? authorsValue)
                && !string.IsNullOrEmpty(authorsValue))
            {
                xmlWriter.WriteElementString("Writer", WebUtility.HtmlDecode(authorsValue));
            }
            if (PopupSettings.CheckboxStates.CheckboxProducerState
                && metadata.TryGetValue("Producers", out string? producersValue)
                && !string.IsNullOrEmpty(producersValue))
            {
                xmlWriter.WriteElementString("Editor", producersValue);
            }
            if (PopupSettings.CheckboxStates.CheckboxTranslatorState
                && metadata.TryGetValue("Translators", out string? translatorsValue)
                && !string.IsNullOrEmpty(translatorsValue))
            {
                xmlWriter.WriteElementString("Translator", translatorsValue);
            }
            if (PopupSettings.CheckboxStates.CheckboxPublisherState
                && metadata.TryGetValue("Publisher", out string? publisherValue)
                && !string.IsNullOrEmpty(publisherValue))
            {
                xmlWriter.WriteElementString("Publisher", WebUtility.HtmlDecode(publisherValue));
            }
            if (PopupSettings.CheckboxStates.CheckboxPageCountState)
            {
                xmlWriter.WriteElementString("PageCount", bookFull.Count.ToString());
            }
            if (PopupSettings.CheckboxStates.CheckboxLanguageState
                && metadata.TryGetValue("Language", out string? languageValue)
                && !string.IsNullOrEmpty(languageValue))
            {
                xmlWriter.WriteElementString("LanguageISO", languageValue.ToLower());
            }
            if (PopupSettings.CheckboxStates.CheckboxReadingDirectionState)
            {
                xmlWriter.WriteElementString("Manga", readingDirection);
            }

            if (PopupSettings.CheckboxStates.CheckboxIsbnAsinState
                && metadata.TryGetValue("ISBN", out string? isbnValue)
                && !string.IsNullOrEmpty(isbnValue))
            {
                xmlWriter.WriteElementString("GTIN", metadata["ISBN"]);
            }

            if (PopupSettings.CheckboxStates.CheckboxChaptersState
                || PopupSettings.CheckboxStates.CheckboxImageSizeState
                || PopupSettings.CheckboxStates.CheckboxFileSizeState)
            {
                xmlWriter.WriteStartElement("Pages");

                for (int i = 0; i < bookFull.Count; i++)
                {
                    xmlWriter.WriteStartElement("Page");

                    xmlWriter.WriteAttributeString("Image", i.ToString());

                    if (i == 0)
                    {
                        xmlWriter.WriteAttributeString("Type", "FrontCover");
                    }
                    if (bookFull[i].Doublepage == true
                        && (!PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState
                        || !MainForm.FormElements.CheckboxExtractImagesState))
                    {
                        xmlWriter.WriteAttributeString("DoublePage", "True");
                    }
                    if (PopupSettings.CheckboxStates.CheckboxFileSizeState)
                    {
                        xmlWriter.WriteAttributeString("ImageSize", bookFull[i].Size.ToString());
                    }
                    if (PopupSettings.CheckboxStates.CheckboxChaptersState)
                    {
                        if (PopupSettings.CheckboxStates.CheckboxOffsetChaptersState)
                        {
                            if (i <= 1)
                            {
                                if (!string.IsNullOrEmpty(bookFull[i].Bookmark))
                                {
                                    xmlWriter.WriteAttributeString("Bookmark", bookFull[i].Bookmark);
                                }
                            }
                            else if (!string.IsNullOrEmpty(bookFull[i - 1].Bookmark))
                            {
                                xmlWriter.WriteAttributeString("Bookmark", bookFull[i - 1].Bookmark);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(bookFull[i].Bookmark))
                            {
                                xmlWriter.WriteAttributeString("Bookmark", bookFull[i].Bookmark);
                            }
                        }
                    }
                    if (PopupSettings.CheckboxStates.CheckboxImageSizeState)
                    {
                        xmlWriter.WriteAttributeString("ImageWidth", bookFull[i].Width.ToString());
                        xmlWriter.WriteAttributeString("ImageHeight", bookFull[i].Height.ToString());
                    }

                    xmlWriter.WriteEndElement(); // Page
                }

                xmlWriter.WriteEndElement(); // Pages
            }

            xmlWriter.WriteEndElement(); // ComicInfo

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }

        private static string GetOpfFile(Dictionary<string, ZipArchiveEntry> entryMap)
        {
            const string containerPath = "META-INF/container.xml";

            ZipArchiveEntry xmlEntry = entryMap.GetValueOrDefault(containerPath) ?? throw new Exception(Resources.ContainerXMLNotFound);
            using StreamReader reader = new(xmlEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string xmlContent = reader.ReadToEnd();

            XDocument xmlDoc = XDocument.Parse(xmlContent);
            XNamespace xmlns = "urn:oasis:names:tc:opendocument:xmlns:container";

            var item = xmlDoc.Descendants(xmlns + "rootfiles").Descendants(xmlns + "rootfile").FirstOrDefault();
            string opfFile = string.Empty;

            if (item is not null) opfFile = item.Attribute("full-path")?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(opfFile)) return opfFile;
            else throw new Exception(Resources.OPFFileNotFound);
        }

        private static (string, Stylesheet?) GetCssFile(string opfPath,
            XDocument opfDoc,
            Dictionary<string, ZipArchiveEntry> entryMap)
        {
            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "manifest").Descendants(opf + "item").FirstOrDefault(i => (string)i.Attribute("media-type")! == "text/css");
            if (item is null) return (string.Empty, null);

            string cssPath = ResolveRootPath(opfPath, (string)item.Attribute("href")!);

            if (!entryMap.TryGetValue(cssPath, out var cssSource))
            {
                return (string.Empty, null);
            }

            using StreamReader reader = new(cssSource.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string fileContent = reader.ReadToEnd();

            var parser = new StylesheetParser();
            var stylesheet = parser.Parse(fileContent);

            return (cssPath, stylesheet);
        }

        private static List<string> GetNcxFile(XDocument opfDoc,
            string opfPath)
        {
            XNamespace opf = "http://www.idpf.org/2007/opf";

            var navigationItems = opfDoc.Descendants(opf + "manifest")
                                .Descendants(opf + "item")
                                .Where(i =>
                                    (string)i.Attribute("media-type")! == "application/x-dtbncx+xml" ||
                                    (string)i.Attribute("properties")! == "nav"
                                )
                                .Select(item => (string)item.Attribute("href")!)
                                .Distinct();

            List<string> navPaths = [.. navigationItems.Select(item => ResolveRootPath(opfPath, item))];

            return navPaths;
        }

        private static bool? RemoveDuplicateCover(Dictionary<string, ZipArchiveEntry> entryMap,
            List<BookInfo.EpubPage> bookFull,
            string epubFilename,
            bool? correctSpread)
        {
            bool wasWide = false;

            if (PopupSettings.CheckboxStates.CheckboxHigherResolutionCover
                && bookFull[0].Height > 0
                && bookFull[1].Height > 0)
            {
                if (bookFull[0].Height >= bookFull[1].Height)
                {
                    if (bookFull[1].Doublepage == true) wasWide = true;
                    bookFull.RemoveAt(1);
                }
                else bookFull.RemoveAt(0);
            }
            else
            {
                if (bookFull[1].Doublepage == true) wasWide = true;
                bookFull.RemoveAt(1);
            }

#if DEBUG
            string debugMessage = string.Empty;
#endif

            if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                if (correctSpread == true
                    && wasWide == false
                    && bookFull[1].Doublepage == false)
                {
                    bool isBlank = false;

                    if (!string.IsNullOrEmpty(bookFull[1].Image)
                        && PopupSettings.CheckboxStates.CheckboxBlankImageState)
                    {
                        isBlank = IsImageBlankWhite(entryMap, bookFull[1].Image);
                    }
                    else if (bookFull[1].Image == string.Empty
                        && PopupSettings.CheckboxStates.CheckboxBlankImageState)
                    {
                        isBlank = true;
                    }

                    // add blank image to keep correct page spread
                    if (!isBlank)
                    {
                        bookFull.Insert(1, new()
                        {
                            Page = "blank",
                        });

#if DEBUG
                        debugMessage += " & Blank Inserted";
#endif
                    }
                    // remove blank image to keep correct page spread
                    else
                    {
                        bookFull.RemoveAt(1);

#if DEBUG
                        debugMessage += " & Removed Double Blank";
#endif
                    }
                }
                else if (correctSpread == false) correctSpread = true;
            }

#if DEBUG
            AppendColoredText($"DEBUG: '{epubFilename}' - Removed Duplicate Cover" + debugMessage + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif

            return correctSpread;
        }

        private static void InsertBlankPage (Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFilename,
            List<BookInfo.EpubPage> bookFull)
        {
            bool isBlank = false;

            if (!string.IsNullOrEmpty(bookFull[1].Image)
                && PopupSettings.CheckboxStates.CheckboxBlankImageState)
            {
                isBlank = IsImageBlankWhite(entryMap, bookFull[1].Image);
            }
            else if (bookFull[1].Image == string.Empty
                && PopupSettings.CheckboxStates.CheckboxBlankImageState)
            {
                isBlank = true;
            }

            // add blank image to keep correct page spread
            if (!isBlank)
            {
                bookFull.Insert(1, new()
                {
                    Page = "blank",
                });
            }
            // remove blank image to keep correct page spread
            else
            {
                bookFull.RemoveAt(1);

#if DEBUG
                AppendColoredText($"DEBUG: '{epubFilename}' - Removed Double Blank" + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif
            }
        }

        private static void ProcessEpub(string epubFile,
            CancellationToken token)
        {
            string rootDir = string.Empty;
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                rootDir = MainForm.FolderNameClass.InputFolderName!;
            }
            else if (!string.IsNullOrEmpty(MainForm.FolderNameClass.OutputFolderName))
            {
                rootDir = MainForm.FolderNameClass.OutputFolderName;
            }
            else
            {
                rootDir = Path.GetDirectoryName(epubFile)!;
            }

            string epubFilename = Path.GetFileNameWithoutExtension(epubFile);
            string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);


            /// Check if file is actually an EPUB
            /// 

            if (!CheckEPUB(epubFile))
            {
                Interlocked.Increment(ref numberCurrentEpub);
                AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.NotAnEPUB, Path.GetFileName(epubFile)) + Environment.NewLine, System.Drawing.Color.Red);
                ProgressBarStep();

                return;
            }

            ///
            /// 

            using ZipArchive epub = ZipFile.OpenRead(epubFile);
            Dictionary<string, ZipArchiveEntry> entryMap = epub.Entries
                .ToDictionary(e => e.FullName, e => e, StringComparer.InvariantCultureIgnoreCase);

            string opfPath = string.Empty;
            try
            {
                opfPath = GetOpfFile(entryMap);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref numberCurrentEpub);
                AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(ex.Message, Path.GetFileName(epubFile)) + Environment.NewLine, System.Drawing.Color.Red);
                ProgressBarStep();

                return;
            }

            if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                if (File.Exists(targetCbz)
                    || !_processedCbzFiles.TryAdd(targetCbz, true))
                {
                    Interlocked.Increment(ref numberCurrentEpub);
                    AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                        + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                    ProgressBarStep();
                    return;
                }

                Dictionary<string, ZipArchiveEntry> entryKeysNew = [];
                HashSet<string> imageExtensionsSimple = [".jpeg", ".jpg", ".png", ".webp", ".svg", ".gif"];
                foreach (KeyValuePair<string, ZipArchiveEntry> pair in entryMap)
                {
                    if (imageExtensionsSimple.Any(suffix => pair.Key.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                    {
                        entryKeysNew.Add(pair.Key, pair.Value);
                    }
                }

                ExtractImageStreamsSimple(entryKeysNew, targetCbz);

                Interlocked.Increment(ref numberCurrentEpub);
                AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.Processed, epubFilename) + Environment.NewLine, System.Drawing.Color.Green);
                ProgressBarStep();

                entryMap.Clear();
                return;
            }

            XDocument opfDoc = GetOpfDocument(entryMap, opfPath);

            bool barnesAndNobleBook = BarnesAndNoble.IsBarnesAndNobleBook(opfDoc);
            XDocument replicaMapDoc = new();

            List<BookInfo.EpubPagesIdsSpread> pages = [];
            if (barnesAndNobleBook)
            {
                (replicaMapDoc, string replicaMapPath) = BarnesAndNoble.GetReplicaMap(entryMap, opfDoc, opfPath);
                pages = BarnesAndNoble.ParseReplicaMapPages(replicaMapDoc, replicaMapPath);
            }
            else
            {
                pages = ParseSpineXml(opfDoc, opfPath);
            }

            /// Try to check if Epub is still DRM protected
            /// 

            if (PopupSettings.CheckboxStates.CheckboxDRMProtectionState && CheckDRMProtection(entryMap, pages[0].Pages.Split('#')[0]))
            {
                Interlocked.Increment(ref numberCurrentEpub);
                AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.IsDRMProtected, Path.GetFileName(epubFile)) + Environment.NewLine, System.Drawing.Color.Red);
                ProgressBarStep();

                entryMap.Clear();
                return;
            }

            ///
            ///

            (Dictionary<string, string?> metadata, string readingDirection) = ParseMetadataXml(opfDoc);

            if (PopupSettings.CheckboxStates.CheckboxMetadataTitleState
                && metadata.TryGetValue("Title", out string? titleValue)
                && !string.IsNullOrEmpty(titleValue))
            {
                foreach (char c in invalidPathFileChars)
                {
                    titleValue = titleValue.Replace(c, '_');
                }
                while (titleValue.Contains("__"))
                {
                    titleValue = titleValue.Replace("__", "_");
                }
                //
                targetCbz = Path.Combine(rootDir, titleValue + _exportFileExtension);
            }

            if (File.Exists(targetCbz))
            {
                Interlocked.Increment(ref numberCurrentEpub);
                AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                ProgressBarStep();
                return;
            }

            List<BookInfo.EpubPage> bookFull = [];
            if (barnesAndNobleBook)
            {
                bookFull = BarnesAndNoble.ParseReplicaMapPagesXml(entryMap, pages);
                BarnesAndNoble.ParseCover(entryMap, epubFile, opfDoc, bookFull, opfPath);
            }
            else
            {
                bookFull = ParseOpfPagesXml(entryMap, epubFile, opfPath, opfDoc, pages);
                ParseAlternativeCover(entryMap, epubFile, opfDoc, bookFull, opfPath);
            }

            if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                FixPageAlignmentPost(bookFull, readingDirection);
            }

            List<BookInfo.EpubChapter> chapters = [];
            if (barnesAndNobleBook)
            {
                chapters = BarnesAndNoble.ParseToc(bookFull, replicaMapDoc);
            }
            else
            {
                chapters = ParseEpubToc(entryMap, epubFile, opfDoc, opfPath);
                chapters = ParseAlternativeToc(entryMap, opfDoc, chapters, bookFull, opfPath);
            }

            bool? correctSpread = null;
            if (barnesAndNobleBook)
            {
                correctSpread = true;
            }
            else
            {
                correctSpread = CheckPageSpread(readingDirection, bookFull);
            }

#if DEBUG
            if (correctSpread == null)
            {
                AppendColoredText($"DEBUG: '{epubFilename}' - No Page Spread Information" + Environment.NewLine, System.Drawing.Color.DarkOrange);
            }
#endif

            if (chapters.Count >= (bookFull.Count - 1) && PopupSettings.CheckboxStates.CheckboxEveryPageIsChapterState) // if all pages are chapters (minus the Cover)
            {
                chapters = [];
            }

            if (PopupSettings.CheckboxStates.CheckboxDuplicateCoverState)
            {
                correctSpread = CheckDuplicateCover(chapters, bookFull, entryMap, opfDoc, epubFilename, correctSpread, epubFile);
            }

            if (correctSpread == false && PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                InsertBlankPage(entryMap, epubFilename, bookFull);
            }

            if (PopupSettings.CheckboxStates.CheckboxInsertAdditionalBlankImageState)
            {
                bookFull.Insert(1, new()
                {
                    Page = "blank",
                });
            }

            if (PopupSettings.CheckboxStates.CheckboxRemoveFirstPageState)
            {
                if (string.IsNullOrEmpty(bookFull[1].Image)
                    || IsImageBlankWhite(entryMap, bookFull[1].Image))
                {
                    bookFull.RemoveAt(1);
                }
            }

            IntegrateChapters(bookFull, chapters);

            ///
            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
            ///

            if (MainForm.FormElements.CheckboxExtractImagesState)
            {
                try
                {
                    ExtractImageStreams(entryMap, targetCbz, bookFull, readingDirection);
                }
                catch (Exception ex)
                {
                    if (Enum.GetNames<Fail>().Any(name => name == ex.Message))
                    {
                        Interlocked.Increment(ref numberCurrentEpub);

                        if (ex.Message == Fail.Cbz.ToString())
                        {
                            AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                                + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                        }
                        else if (ex.Message == Fail.Blank.ToString())
                        {
                            AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                                + Resources.BlankImageError + string.Format(Resources.NotAManga, epubFilename) + Environment.NewLine, System.Drawing.Color.Red);

                            if (File.Exists(targetCbz)) File.Delete(targetCbz);
                        }
                        else if (ex.Message == Fail.Split.ToString())
                        {
                            AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                                + Resources.SplitImageError + $" '{epubFilename}'" + Environment.NewLine, System.Drawing.Color.Red);

                            if (File.Exists(targetCbz)) File.Delete(targetCbz);
                        }

                        ProgressBarStep();

                        entryMap.Clear();
                        return;
                    }
                }

                ///
                if (token.IsCancellationRequested)
                {
                    if (File.Exists(targetCbz)) File.Delete(targetCbz);

                    token.ThrowIfCancellationRequested();
                }
                ///
            }

            if (MainForm.FormElements.CheckboxComicInfoState)
            {
                if (!MainForm.FormElements.CheckboxExtractImagesState
                    && (File.Exists(targetCbz)
                    || !_processedCbzFiles.TryAdd(targetCbz, true)))
                {
                    Interlocked.Increment(ref numberCurrentEpub);
                    AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                        + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                    ProgressBarStep();

                    entryMap.Clear();
                    return;
                }

                if (PopupSettings.CheckboxStates.CheckboxImageSizeState)
                {
                    (int width, int height) = GetSinglePageResolution(entryMap, bookFull);

                    FillBlankImageResolutions(width, height, bookFull);
                }

                //WriteComicInfoXml(targetCbz, epubFilename, readingDirection, bookFull, metadata);
            }

            Interlocked.Increment(ref numberCurrentEpub);
            AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                + string.Format(Resources.Processed, epubFilename) + Environment.NewLine, System.Drawing.Color.Green);
            ProgressBarStep();

            bookFull.Clear();
            entryMap.Clear();
            return;
        }

        private static void DisableControls()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.checkBoxComicInfo.Enabled = false;
                _mainForm.checkBoxImages.Enabled = false;
                _mainForm.buttonPath.Enabled = false;
                _mainForm.buttonPathClear.Enabled = false;
                _mainForm.buttonSwitchModes.Enabled = false;
                _mainForm.buttonFileModeFileList.Enabled = false;
                _mainForm.buttonStart.Text = Resources.AbortButtonText;
                _mainForm.comboBoxLanguage.Enabled = false;
                _mainForm.buttonOpenSettings.Enabled = false;
            });
        }

        private static void EnableControls()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.checkBoxComicInfo.Enabled = true;
                _mainForm.checkBoxImages.Enabled = true;
                _mainForm.buttonPath.Enabled = true;
                _mainForm.buttonPathClear.Enabled = true;
                _mainForm.buttonSwitchModes.Enabled = true;
                _mainForm.buttonFileModeFileList.Enabled = true;
                _mainForm.buttonStart.Enabled = true;
                _mainForm.buttonStart.Text = Resources.StartButtonText;
                _mainForm.comboBoxLanguage.Enabled = true;
                _mainForm.buttonOpenSettings.Enabled = true;
            });
        }

        private static void HandleCompletion(TimeSpan ts,
            bool wasAborted)
        {
            AppendColoredText(Environment.NewLine + string.Format(Resources.Timer, Math.Floor(ts.TotalMinutes), ts.Seconds, ts.Milliseconds), System.Drawing.Color.White);

            if (wasAborted)
            {
                AppendColoredText(Environment.NewLine + Environment.NewLine + Resources.AbortedMessage, System.Drawing.Color.Red);
            }

            EnableControls();

            _processedCbzFiles.Clear();

            cts.Dispose();
            cts = new();

            if (PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                _mainForm?.Invoke(_mainForm.fileListForm.fileListDataTable.Clear);
                MainForm.FileNameClass.FileNames.Clear();
            }
        }

        public static async Task ProgramStart()
        {
            string inputFolderName = MainForm.FolderNameClass.InputFolderName ?? string.Empty;
            string outputFolderName = MainForm.FolderNameClass.OutputFolderName ?? string.Empty;

            if (!MainForm.FormElements.CheckboxExtractImagesState
                && !MainForm.FormElements.CheckboxComicInfoState
                && !PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoCheckBoxChecked, Resources.ErrorMessageBox);

                return;
            }

            Stopwatch stopwatch = new();
            stopwatch.Start();

            string rootDir = string.Empty;
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                rootDir = inputFolderName ?? string.Empty;
            }
            else if (MainForm.FileNameClass.FileNames.Count < 1)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoEpubsFoundMessageBox, Resources.ErrorMessageBox);

                EnableControls();
                stopwatch.Stop();
                return;
            }

            _mainForm?.Invoke(() =>
            {
                _mainForm.outputBoxConsole.Text = string.Empty;
                _mainForm.toolStripProgressBar.Value = 0;
            });

            DisableControls();
            numberCurrentEpub = 0;
            bool wasAborted = false;

            AppendColoredText(Resources.ProcessingInProgress + Environment.NewLine, System.Drawing.Color.White);
            AppendColoredText(Environment.NewLine, System.Drawing.Color.White);

            if (string.IsNullOrEmpty(rootDir) && !PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoPathMessageBox, Resources.ErrorMessageBox);

                EnableControls();
                stopwatch.Stop();
                return;
            }

            List<string> epubPaths = [];

            if (PopupSettings.CheckboxStates.RadioButtonZipState
                && !PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                _exportFileExtension = ".zip";
            }
            else
            {
                _exportFileExtension = ".cbz";
            }

            try
            {
                epubPaths = await Task.Run(() =>
                {
                    List<string> foundEpubPaths = [];

                    if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
                    {
                        foreach (var epubPath in Directory.EnumerateFileSystemEntries(rootDir, "*.epub", SearchOption.AllDirectories))
                        {
                            if (cts.IsCancellationRequested)
                            {
                                throw new OperationCanceledException(cts.Token);
                            }

                            if (File.Exists(epubPath))
                            {
                                if (!PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                                {
                                    string epubFilename = Path.GetFileNameWithoutExtension(epubPath);
                                    string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);

                                    if (File.Exists(targetCbz))
                                    {
                                        AppendColoredText(string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                                        continue;
                                    }
                                }

                                foundEpubPaths.Add(epubPath);
                            }
                        }
                    }
                    else
                    {
                        foreach (var epubPath in MainForm.FileNameClass.FileNames)
                        {
                            if (cts.IsCancellationRequested)
                            {
                                throw new OperationCanceledException(cts.Token);
                            }

                            if (File.Exists(epubPath))
                            {
                                if (!PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                                {
                                    if (string.IsNullOrEmpty(outputFolderName))
                                    {
                                        rootDir = Path.GetDirectoryName(epubPath)!;
                                    }
                                    else
                                    {
                                        rootDir = outputFolderName;
                                    }

                                    string epubFilename = Path.GetFileNameWithoutExtension(epubPath);
                                    string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);

                                    if (File.Exists(targetCbz))
                                    {
                                        AppendColoredText(string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                                        continue;
                                    }
                                }

                                foundEpubPaths.Add(epubPath);
                            }
                        }
                    }
                    return foundEpubPaths;
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                wasAborted = true;
                stopwatch.Stop();
                HandleCompletion(stopwatch.Elapsed, wasAborted);
                return;
            }
            catch (Exception ex)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError($"{ex.Message}", Resources.ErrorMessageBox);

                EnableControls();
                stopwatch.Stop();
                return;
            }

            if (epubPaths.Count <= 0)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoEpubsFoundMessageBox, Resources.ErrorMessageBox);

                EnableControls();
                stopwatch.Stop();
                return;
            }


            numberEpubs = epubPaths.Count;

            _mainForm?.Invoke(() => _mainForm.toolStripProgressBar.Maximum = numberEpubs);

            using PopupSettings popup = new();
            int? nullableDegree = popup.dropDownThreads.Items[PopupSettings.CheckboxStates.DropDownParallelismDegreeState] as int?;

            if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState) nullableDegree = Environment.ProcessorCount - 1;

            int maxDegreeOfParallelism = nullableDegree ?? (Environment.ProcessorCount - 1);
            maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);

            try
            {
                await Parallel.ForEachAsync(epubPaths,
                    new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism, CancellationToken = cts.Token },
                    async (epubPath, token) =>
                    {
                        ProcessEpub(epubPath, cts.Token);
                    });
            }
            catch (OperationCanceledException)
            {
                wasAborted = true;
            }
            finally
            {
                stopwatch.Stop();
                HandleCompletion(stopwatch.Elapsed, wasAborted);
            }
        }

        private static void HandleArguments(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg.ToLower())
                {
                    case "--simple":
                    case "-s":
                        PopupSettings.CheckboxStates.CheckboxSimpleExtractionState = true;
                        break;
                    case "--dark":
                    case "-d":
#pragma warning disable WFO5001 // needs to be here until dotnet 10
                        if (!SystemInformation.HighContrast) Application.SetColorMode(SystemColorMode.Dark);
#pragma warning restore WFO5001
                        break;
                }
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            HandleArguments(args);

            ApplicationConfiguration.Initialize();

            _mainForm = new MainForm();
            Application.Run(_mainForm);
        }
    }
}