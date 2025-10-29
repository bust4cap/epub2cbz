using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using epub2cbz_gui.Properties;
using ExCSS;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ImageSharpDrawing = SixLabors.ImageSharp.Drawing;

namespace epub2cbz_gui
{
    public static class VersionDate
    {
        public static string GetVersionDateYear { get; } = "2025";
        public static string GetVersionDateMonth { get; } = "10";
        public static string GetVersionDateDay { get; } = "29";
        public static int GetVersionNumber { get; } = 1;
    }

    static class Program
    {
        private static int numberEpubs = 0;
        private static int numberCurrentEpub = 0;
        public static CancellationTokenSource cts = new();

        private static readonly HashSet<string> imageExtensions = [".jpeg", ".jpg", ".png", ".webp", ".gif"];
        private static readonly ConcurrentDictionary<string, bool> _processedCbzFiles = new(StringComparer.InvariantCultureIgnoreCase);
        private static readonly char[] invalidPathFileChars = [.. Path.GetInvalidPathChars(), .. Path.GetInvalidFileNameChars()];

        private static string _exportFileExtension = ".cbz";

        private static void ProgressBarStep()
        {
            MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault()!;
            mainForm.Invoke(new Action(mainForm.toolStripProgressBar.PerformStep));
        }

        public static void ClearAndFocusConsole()
        {
            MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault()!;
            mainForm.Invoke(new Action(() =>
            {
                mainForm.outputBoxConsole.Clear();
                mainForm.outputBoxConsole.Focus();
            }));
        }

        public static void AppendColoredText(string text,
            System.Drawing.Color color)
        {
            MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault()!;
            mainForm.Invoke(new Action(() =>
            {
                mainForm.outputBoxConsole.SelectionStart = mainForm.outputBoxConsole.TextLength;
                mainForm.outputBoxConsole.SelectionLength = 0;
                mainForm.outputBoxConsole.SelectionColor = color;
                mainForm.outputBoxConsole.AppendText(text);
                mainForm.outputBoxConsole.SelectionColor = mainForm.outputBoxConsole.ForeColor;
            }));
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
            { "Kindle Oasis 2/3/Paperwhite 12/Colorsoft 12", (1264, 1680) },
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

        private static List<Dictionary<string, string>> IntegrateChapters(List<Dictionary<string, string>> bookFull,
            List<Dictionary<string, string>> chapters)
        {
            for (int i = 0; i < bookFull.Count; i++)
            {
                string bookmark = string.Empty;

                foreach (var chapter in chapters)
                {
                    string chapterPage = Path.GetFileName(chapter["page"].Split('#').FirstOrDefault()) ?? string.Empty;

                    if (chapterPage == Path.GetFileName(bookFull[i]["page"].Split('#')[0]))
                    {
                        bookmark = chapter["title"].Trim();
                        break;
                    }
                }

                if (i == 0
                    && string.IsNullOrEmpty(bookmark))
                {
                    bookmark = "Cover";
                }
                if (!string.IsNullOrEmpty(bookmark))
                {
                    bookFull[i].Add("bookmark", WebUtility.HtmlDecode(bookmark));
                }
            }

            return bookFull;
        }

        private static bool CompareImages(Dictionary<string, ZipArchiveEntry> entryMap,
            string firstImage,
            string secondImage,
            string epubFile)
        {
            if (string.IsNullOrEmpty(secondImage)) return false;

            try
            {
                ZipArchiveEntry firstCoverEntry = entryMap.GetValueOrDefault(firstImage)!;
                ZipArchiveEntry secondCoverEntry = entryMap.GetValueOrDefault(secondImage)!;
                using var firstCoverStream = firstCoverEntry.Open();
                using var secondCoverStream = secondCoverEntry.Open();


                using Image<Rgba32> image1 = SixLabors.ImageSharp.Image.Load<Rgba32>(firstCoverStream);
                using Image<Rgba32> image2 = SixLabors.ImageSharp.Image.Load<Rgba32>(secondCoverStream);

                var dhashAlgorithm = new DifferenceHash();
                ulong dimageHash1 = dhashAlgorithm.Hash(image1);
                ulong dimageHash2 = dhashAlgorithm.Hash(image2);
                double dpercentageImageSimilarity = CompareHash.Similarity(dimageHash1, dimageHash2);

                if (dpercentageImageSimilarity >= 95)
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

        private static MemoryStream? CalculateCroppingBorder(Stream imageStream, out int cropWidth, out int cropHeight)
        {
            cropWidth = 0;
            cropHeight = 0;

            using Image<Rgba32> originalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(imageStream);
            if (originalImage == null) return null;

            using Image<Rgba32> image = originalImage.Clone();

            // Determine border color from middle pixel
            Rgba32 topLeft = image[0, 0];
            Rgba32 topRight = image[image.Width -1, 0];
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

            cropWidth = paddedRight - paddedLeft + 1;
            cropHeight = paddedBottom - paddedTop + 1;

            if (cropWidth <= 0 ||
                cropHeight <= 0 ||
                (cropWidth == image.Width && cropHeight == image.Height))
            {
                return null;
            }

            SixLabors.ImageSharp.Rectangle cropRectangle = new(paddedLeft, paddedTop, cropWidth, cropHeight);
            originalImage.Mutate(ctx => ctx.Crop(cropRectangle));

            var outputStream = new MemoryStream();
            originalImage.Save(outputStream, originalImage.Metadata.DecodedImageFormat!);
            outputStream.Position = 0;

            return outputStream;
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

        private static MemoryStream ResizeImage(Stream imageStream)
        {
            using SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imageStream);

            int maxWidth = PopupSettings.CheckboxStates.TextBoxResizeWidthValue;
            int maxHeight = PopupSettings.CheckboxStates.TextBoxResizeHeightValue;

            ResizeOptions options = new()
            {
                Size = new SixLabors.ImageSharp.Size(maxWidth, maxHeight),
                Mode = ResizeMode.Max
            };

            if (image.Height < maxHeight
                && image.Width < maxWidth)
            {
                options.Sampler = KnownResamplers.Lanczos3;
            }
            else
            {
                options.Sampler = KnownResamplers.Bicubic;
            }

            image.Mutate(x => x.Resize(options));

            var outputStream = new MemoryStream();
            image.Save(outputStream, image.Metadata.DecodedImageFormat!);
            outputStream.Position = 0;

            return outputStream;
        }

        private static (List<Dictionary<string, string>> bookFull, bool? correctSpread) CheckDuplicateCover(List<Dictionary<string, string>> chapters,
            List<Dictionary<string, string>> bookFull,
            Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            string epubFilename,
            bool? correctSpread,
            string epubFile)
        {
            if (chapters.Count > 0 &&
                    Path.GetFileName(chapters[0]["page"].Split('#')[0]) == Path.GetFileName(bookFull[1]["page"]) &&
                    (chapters[0]["title"].Contains("Cover")
                    || chapters[0]["title"] == "カバー"
                    || chapters[0]["title"] == "表紙"))
            {
                (bookFull, correctSpread) = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
            }
            else
            {
                XNamespace opf = "http://www.idpf.org/2007/opf";

                var item = opfDoc.Descendants(opf + "guide").Descendants(opf + "reference").FirstOrDefault(i => (string?)i.Attribute("type") == "cover");
                if (item != null)
                {
                    string coverPath = (string)item.Attribute("href")!;

                    if (Path.GetFileName(coverPath.Split('#')[0]) == Path.GetFileName(bookFull[1]["page"]))
                    {
                        (bookFull, correctSpread) = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                    }
                    else if (CompareImages(entryMap, bookFull[0]["image"], bookFull[1]["image"], epubFile))
                    {
                        (bookFull, correctSpread) = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                    }
                }
#if DEBUG
                else if (Path.GetFileName(bookFull[0]["image"]) == Path.GetFileName(bookFull[1]["image"]))
                {
                    AppendColoredText($"DEBUG: '{epubFilename}' - Image 0 == Image 1" + Environment.NewLine, System.Drawing.Color.HotPink);

                    (bookFull, correctSpread) = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                }
#endif
                else if (CompareImages(entryMap, bookFull[0]["image"], bookFull[1]["image"], epubFile))
                {
                    (bookFull, correctSpread) = RemoveDuplicateCover(entryMap, bookFull, epubFilename, correctSpread);
                }
            }

            return (bookFull, correctSpread);
        }

        private static bool CheckEPUB(string epubFile)
        {
            byte[] buffer = new byte[4];

            using FileStream fs = new(epubFile, FileMode.Open, FileAccess.Read);

            if (fs.Length < 4)
            {
                return false;
            }

            fs.ReadExactly(buffer, 0, 4);
            string hexValues = string.Empty;

            foreach (var element in buffer)
            {
                hexValues += element.ToString("X2");
            }
            if (hexValues != "504B0304")    // EPUB
            {
                return false;
            }

            return true;
        }

        private static bool CheckDRMProtection(Dictionary<string, ZipArchiveEntry> entryMap,
            List<Dictionary<string, string>> pages)
        {
            bool isDRMProtected = true;

            string filename = pages[0]["pages"].Split('#')[0];
            filename = RemoveStartingDots(filename);

            string actualFilename = entryMap
                    .Where(map => map.Key.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
                    .Select(map => map.Key)
                    .FirstOrDefault(string.Empty);

            if (!string.IsNullOrEmpty(actualFilename))
            {
                if (actualFilename.EndsWith(".xhtml", StringComparison.InvariantCultureIgnoreCase) ||
                    actualFilename.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) ||
                    actualFilename.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))      // Walter Isaacson - Steve Jobs
                {
                    ZipArchiveEntry fileEntry = entryMap
                        .Where(map => map.Key.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Value)
                        .FirstOrDefault()!;

                    using StreamReader reader = new(fileEntry.Open());
                    string fileContent = reader.ReadToEnd();
                    fileContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(fileContent));
                    if (fileContent.Contains("html", StringComparison.InvariantCultureIgnoreCase)) isDRMProtected = false;
                }
                else if (imageExtensions.Any(ext => actualFilename.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
                {
                    byte[] buffer = new byte[4];

                    ZipArchiveEntry fileEntry = entryMap
                        .Where(map => map.Key.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Value)
                        .FirstOrDefault()!;

                    using Stream fileStream = fileEntry.Open();

                    fileStream.ReadExactly(buffer, 0, 4);
                    string hexValues = string.Empty;

                    foreach (var element in buffer)
                    {
                        hexValues += element.ToString("X2");
                    }
                    if (hexValues == "FFD8FFE0" ||  // JPEG
                        hexValues == "FFD8FFE1" ||  // JPEG
                        hexValues == "89504E47" ||  // PNG
                        hexValues == "52494646" ||  // WEBP
                        hexValues == "47494638")    // GIF
                    {
                        isDRMProtected = false;
                    }
                }
            }

            return isDRMProtected;
        }

        private static (MemoryStream, MemoryStream, int, int) SplitImageSharp(ZipArchiveEntry bookEntry,
            string fileExtension)
        {
            using var sourceStream = bookEntry.Open();

            Image<Rgba32> imageToProcess;
            int imageWidth;
            int imageHeight;

            if (PopupSettings.CheckboxStates.CheckboxCropImagesState)
            {
                using MemoryStream bufferedSourceStream = new();
                sourceStream.CopyTo(bufferedSourceStream);
                bufferedSourceStream.Position = 0;

                using MemoryStream? croppedSourceStream = CalculateCroppingBorder(bufferedSourceStream, out int croppedWidth, out int croppedHeight);
                if (croppedSourceStream != null)
                {
                    imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(croppedSourceStream);
                    imageWidth = croppedWidth;
                    imageHeight = croppedHeight;
                }
                else
                {
                    bufferedSourceStream.Position = 0;
                    imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(bufferedSourceStream);
                    imageWidth = imageToProcess.Width;
                    imageHeight = imageToProcess.Height;
                }
            }
            else
            {
                imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(sourceStream);
                imageWidth = imageToProcess.Width;
                imageHeight = imageToProcess.Height;
            }

            int halfWidth = imageWidth / 2;

            IImageEncoder encoder = ImageSharpFormatToEncoding[fileExtension];

            // Process left half
            using var outputImageLeft = imageToProcess.Clone(context => context.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, halfWidth, imageHeight)));
            var encodedDataLeft = new MemoryStream();
            outputImageLeft.Save(encodedDataLeft, encoder);
            encodedDataLeft.Position = 0; // Reset stream position to the beginning

            // Process right half
            using var outputImageRight = imageToProcess.Clone(context => context.Crop(new SixLabors.ImageSharp.Rectangle(halfWidth, 0, imageWidth - halfWidth, imageHeight)));
            var encodedDataRight = new MemoryStream();
            outputImageRight.Save(encodedDataRight, encoder);
            encodedDataRight.Position = 0; // Reset stream position to the beginning

            imageToProcess.Dispose();

            return (encodedDataLeft, encodedDataRight, halfWidth, imageHeight);
        }

        private static bool IsImageBlankWhite(Dictionary<string, ZipArchiveEntry> entryMap,
            string bookImage)
        {
            try
            {
                ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(bookImage)!;

                if (bookEntry != null)
                {
                    using Stream imageStream = bookEntry.Open();
                    using Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageStream);

                    // Ignore 2 outermost pixels
                    for (int y = 2; y < (image.Height - 2); y++)
                    {
                        for (int x = 2; x < (image.Width - 2); x++)
                        {
                            Rgba32 pixel = image[x, y];
                            if (pixel.A < 250 || pixel.R < 250 || pixel.G < 250 || pixel.B < 250) return false; // Image isn't blank
                        }
                    }
                    return true; // Image is blank
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool? CheckPageSpread(string readingDirection,
            List<Dictionary<string, string>> bookFull)
        {
            if (bookFull[1]["doublepage"] == "true") return true;
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
                if (bookFull[i]["doublepage"] == "true")
                {
                    if ((i + pageSpreadCounter) % 2 != 0) return true;
                    pageSpreadCounter++;
                    continue;
                }

                if (string.IsNullOrEmpty(bookFull[i]["spread"]))
                {
                    continue;
                }

                foundSpreadInfo = true;

                if (bookFull[i]["spread"].Contains(firstPage, StringComparison.InvariantCultureIgnoreCase) && (i + pageSpreadCounter) % 2 != 0
                    || bookFull[i]["spread"].Contains(secondPage, StringComparison.InvariantCultureIgnoreCase) && (i + pageSpreadCounter) % 2 == 0)
                {
                    return true; // Spread seems to be correct
                }
                else if (bookFull[i]["spread"].Contains(firstPage, StringComparison.InvariantCultureIgnoreCase) && (i + pageSpreadCounter) % 2 == 0
                    || bookFull[i]["spread"].Contains(secondPage, StringComparison.InvariantCultureIgnoreCase) && (i + pageSpreadCounter) % 2 != 0)
                {
                    return false; // blank page needed
                }
            }

            return foundSpreadInfo ? false : null;
        }

        private static List<Dictionary<string, string>> FixPageAlignmentPost(List<Dictionary<string, string>> bookFull,
            string readingDirection)
        {
            for (int i = 1; i < bookFull.Count; i++)
            {
                //  Add blank page before double page if page alignment is incorrect
                if (bookFull[i]["doublepage"] == "true")
                {
                    if (readingDirection == "No")
                    {
                        // For ltr books - if current page is wide and last page was a single left page
                        if (i > 1
                            && bookFull[i - 1]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i - 1]["doublepage"] == "false")
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-right",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["spread"] == string.Empty
                            && bookFull[i - 1]["doublepage"] == "false"
                            && (bookFull[i - 2]["doublepage"] == "true" || (i - 2) == 0))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-right",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                    }
                    else if (readingDirection == "YesAndRightToLeft")
                    {
                        // For rtl books - if current page is wide and last page was a single right page
                        if (i > 1
                            && bookFull[i - 1]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i - 1]["doublepage"] == "false")
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-left",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["spread"] == string.Empty
                            && bookFull[i - 1]["doublepage"] == "false"
                            && (bookFull[i - 2]["doublepage"] == "true" || (i - 2) == 0))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-left",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                    }
                }
                //  Add blank page for single pages and after double pages if page alignment is incorrect
                else
                {
                    if (readingDirection == "No")
                    {
                        if (i > 1
                            && bookFull[i - 1]["doublepage"] == "true"
                            && bookFull[i]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-left",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["doublepage"] == "false"
                            && bookFull[i - 1]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-right",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-left",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                    }
                    else if (readingDirection == "YesAndRightToLeft")
                    {
                        if (i > 1
                            && bookFull[i - 1]["doublepage"] == "true"
                            && bookFull[i]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-right",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i]["spread"].Contains("left", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-right",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                        else if (i > 1
                            && bookFull[i - 1]["doublepage"] == "false"
                            && bookFull[i - 1]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase)
                            && bookFull[i]["spread"].Contains("right", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bookFull.Insert(i, new Dictionary<string, string>()
                            {
                                ["page"] = "blank",
                                ["image"] = string.Empty,
                                ["spread"] = "page-spread-left",
                                ["doublepage"] = "false",
                                ["height"] = string.Empty,
                                ["width"] = string.Empty
                            });

                            i++;
                        }
                    }
                }
            }

            return bookFull;
        }

        private static List<Dictionary<string, string>> ParseOpfPagesXml(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            string opfPath,
            XDocument opfDoc,
            List<Dictionary<string, string>> dicPagesIdsSpread,
            Dictionary<string, string?> metadata)
        {
            List<Dictionary<string, string>> bookFull = [];
            bool coverFound = false;
            string cssPath = GetCssFile(opfPath, opfDoc);
            const double wideImageRatio = 1.125; // Images have to be at least 12.5% wider than tall to be considered "wide"

            for (int i = 0; i < dicPagesIdsSpread.Count; i++)
            {
                string? imagePath = FindImagePathInFile(entryMap, epubFile, RemoveStartingDots(dicPagesIdsSpread[i]["pages"].Split('#')[0]), metadata);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    imagePath = entryMap
                        .Where(map => map.Key.Contains(RemoveStartingDots(imagePath), StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Key)
                        .FirstOrDefault(string.Empty);

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        // Handle wide images first
                        ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(imagePath)!;
                        using var streamDimensions = bookEntry.Open();

                        int width = 0;
                        int height = 0;

                        (width, height) = GetImageDimensions(streamDimensions);

                        bool isDoublePage = false;

                        ///
                        if (width >= (height * wideImageRatio)) isDoublePage = true;
                        ///

                        bookFull.Add(new Dictionary<string, string>()
                        {
                            ["page"] = dicPagesIdsSpread[i]["pages"].Split('#')[0],
                            ["image"] = imagePath,
                            ["spread"] = dicPagesIdsSpread[i]["spread"] ?? string.Empty,
                            ["doublepage"] = isDoublePage.ToString().ToLower(),
                            ["height"] = height.ToString(),
                            ["width"] = width.ToString()
                        });
                        coverFound = true;
                        continue;
                    }
                }
                //  If image paths are only found in a css file (e.g. The Hobbit)
                else if (dicPagesIdsSpread.Count > i && !coverFound)
                {
                    string cssImage = FindImagePathInCss(entryMap, cssPath, dicPagesIdsSpread[i]["ids"]);
                    if (!string.IsNullOrEmpty(cssImage))
                    {
                        cssImage = entryMap
                            .Where(map => map.Key.Contains(RemoveStartingDots(cssImage), StringComparison.InvariantCultureIgnoreCase))
                            .Select(map => map.Key)
                            .FirstOrDefault(string.Empty);

                        if (!string.IsNullOrEmpty(cssImage))
                        {
                            // Handle wide images first
                            ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(cssImage)!;
                            using var streamDimensions = bookEntry.Open();

                            int width = 0;
                            int height = 0;

                            (width, height) = GetImageDimensions(streamDimensions);

                            bool isDoublePage = false;

                            ///
                            if (width >= (height * wideImageRatio)) isDoublePage = true;
                            ///

                            bookFull.Add(new Dictionary<string, string>()
                            {
                                ["page"] = dicPagesIdsSpread[i]["pages"].Split('#')[0],
                                ["image"] = cssImage,
                                ["spread"] = dicPagesIdsSpread[i]["spread"] ?? string.Empty,
                                ["doublepage"] = isDoublePage.ToString().ToLower(),
                                ["height"] = height.ToString(),
                                ["width"] = width.ToString()
                            });
                        }
                    }
                }

                //  Add blank page if image source is not linked
                if (!bookFull.Any(b => b["page"] == dicPagesIdsSpread[i]["pages"]))
                {
                    bookFull.Add(new Dictionary<string, string>()
                    {
                        ["page"] = dicPagesIdsSpread[i]["pages"].Split('#')[0],
                        ["image"] = string.Empty,
                        ["spread"] = dicPagesIdsSpread[i]["spread"] ?? string.Empty,
                        ["doublepage"] = "false",
                        ["height"] = string.Empty,
                        ["width"] = string.Empty
                    });
                }
            }

            return bookFull;
        }

        private static XDocument GetOpfDocument(Dictionary<string, ZipArchiveEntry> entryMap,
            string opfPath)
        {
            ZipArchiveEntry fileEntry = entryMap.GetValueOrDefault(opfPath)!;
            using StreamReader reader = new(fileEntry.Open());
            string opfContent = reader.ReadToEnd();
            opfContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(opfContent));

            // Replace null characters
            opfContent = opfContent.Replace("\0", string.Empty).Replace("\x01", string.Empty);
            opfContent = opfContent.Replace("&amp;", "&").Replace("&", "&amp;");

            Dictionary<string, string?> pages = [];
            List<Dictionary<string, string>> dicPagesIdsSpread = [];

            XDocument opfDoc = XDocument.Parse(opfContent);

            return opfDoc;
        }

        private static List<Dictionary<string, string>> ParseSpineXml(XDocument opfDoc)
        {
            Dictionary<string, string?> pages = [];
            List<Dictionary<string, string>> dicPagesIdsSpread = [];

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var opfMetadata = opfDoc.Descendants(opf + "spine").Descendants(opf + "itemref");
            if (opfMetadata != null)
            {
                foreach (XElement e in opfMetadata)
                {
                    pages.Add(e.Attribute("idref")!.Value, e.Attribute("properties")?.Value);
                }
            }

            foreach (KeyValuePair<string, string?> page in pages)
            {
                var opfManifest = opfDoc.Descendants(opf + "manifest").Descendants(opf + "item").FirstOrDefault(i => (string?)i.Attribute("id") == page.Key);
                if (opfManifest != null)
                {
                    string? opfHref = (string?)opfManifest.Attribute("href");
                    dicPagesIdsSpread.Add(new Dictionary<string, string>()
                    {
                        ["pages"] = opfHref ?? string.Empty,
                        ["ids"] = page.Key,
                        ["spread"] = page.Value ?? string.Empty
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
                && identifier.StartsWith("urn:asin:", StringComparison.InvariantCultureIgnoreCase)
                && identifier.Length == 19)
            {
                identifier = "ASIN: " + identifier[9..];
                return identifier.Trim();
            }

            if (!string.IsNullOrEmpty(identifier)
                && !identifier.StartsWith("urn:uuid:", StringComparison.InvariantCultureIgnoreCase)
                && !identifier.StartsWith("calibre:", StringComparison.InvariantCultureIgnoreCase))
            {
                return ReturnMetadataISBNCalculated(identifier);
            }

            string? source = xmlMetadata.Descendants(dc + "source").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(source) && source.StartsWith("urn:isbn:", StringComparison.InvariantCultureIgnoreCase))
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
            if (xmlMetadata != null && MainForm.FormElements.CheckboxComicInfoState)
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

            if (xmlReadingDirection != null
                && xmlReadingDirection.Attribute("page-progression-direction")?.Value == "rtl")
            {
                readingDirection = "YesAndRightToLeft";
            }
            else readingDirection = "No";

            return (metadata, readingDirection);
        }

        private static (int width, int height) GetImageDimensions(Stream zipEntryStream)
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

        private static List<Dictionary<string, string>> GetTocFile(Dictionary<string, ZipArchiveEntry> entryMap,
            List<Dictionary<string, string>> newChapters,
            List<Dictionary<string, string>> bookFull,
            int number)
        {
            List<Dictionary<string, string>> newToc = [];
            List<string> toc = [];

            int i = 0;
            // check for 6 pages after and including the initial TOC file
            while (i < 6)
            {
                try
                {
                    string altTocPath = RemoveStartingDots(bookFull[number + i]["page"]);

                    ZipArchiveEntry altTocEntry = entryMap
                        .Where(map => map.Key.Contains(altTocPath!, StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Value)
                        .FirstOrDefault()!;
                    using StreamReader reader = new(altTocEntry.Open());
                    string altTocContent = reader.ReadToEnd();
                    altTocContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(altTocContent));

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
                            if (Path.GetFileName(book["page"].Split('#')[0]) == Path.GetFileName(entry.Split('#')[0]))
                            {
                                newChapters.Add(new Dictionary<string, string>()
                                {
                                    ["title"] = $"Page {bookFull.IndexOf(book) + 1}",
                                    ["page"] = RemoveStartingDots(entry.Split('#')[0]),
                                    ["image"] = string.Empty
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
                string fileName = Path.GetFileName(newChapter["page"].Split('#')[0]);

                if (seen.Contains(fileName)) continue;

                seen.Add(fileName);
                newToc.Add(newChapter);
            }

            return newToc;
        }

        private static List<Dictionary<string, string>> ParseAlternativeToc(Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            List<Dictionary<string, string>> chapters,
            List<Dictionary<string, string>> bookFull)
        {
            List<Dictionary<string, string>> newToc = [];
            string altTocFile = string.Empty;

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "guide").Descendants(opf + "reference").FirstOrDefault(i => (string?)i.Attribute("type") == "toc");
            if (item != null) altTocFile = (string)item.Attribute("href")!;

            if (!string.IsNullOrEmpty(altTocFile))
            {
                foreach (var book in bookFull)
                {
                    int index = bookFull.IndexOf(book);

                    if (Path.GetFileName(book["page"]) == Path.GetFileName(altTocFile.Split('#')[0]))
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

        private static List<Dictionary<string, string>> ParseAlternativeCover(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            XDocument opfDoc,
            List<Dictionary<string, string>> bookFull)
        {
            string coverPath = string.Empty;

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "metadata")
                .Descendants(opf + "meta")
                .FirstOrDefault(i => (string?)i.Attribute("name") == "cover");
            if (item != null) coverPath = (string)item.Attribute("content")!;

            var coverId = opfDoc.Descendants(opf + "manifest")
                .Descendants(opf + "item")
                .FirstOrDefault(i => (string?)i.Attribute("id") == coverPath);
            if (coverId != null) coverPath = (string)coverId.Attribute("href")!;

            if (!string.IsNullOrEmpty(coverPath) &&
                imageExtensions.Any(ext => coverPath.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
            {
                coverPath = RemoveStartingDots(coverPath);

                string filename = entryMap
                        .Where(map => map.Key.Contains(RemoveStartingDots(coverPath), StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Key)
                        .FirstOrDefault(string.Empty);

                if (!string.IsNullOrEmpty(filename) && filename != bookFull[0]["image"])
                {
#if DEBUG
                    AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Alternative Cover" + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif
                    ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(filename)!;
                    using var streamDimensions = bookEntry.Open();

                    int width = 0;
                    int height = 0;

                    (width, height) = GetImageDimensions(streamDimensions);

                    bookFull.Insert(0, new Dictionary<string, string>()
                    {
                        ["page"] = "Cover",
                        ["image"] = filename,
                        ["spread"] = string.Empty,
                        ["doublepage"] = "false",
                        ["height"] = height.ToString(),
                        ["width"] = width.ToString()
                    });
                }
            }

            return bookFull;
        }

        private static Image<Rgba32> CreateBlankImage(int dimensionX,
            int dimensionY)
        {
            var image = new Image<Rgba32>(dimensionX, dimensionY, SixLabors.ImageSharp.Color.White);
            var black = SixLabors.ImageSharp.Color.ParseHex("040404");
            var gray = SixLabors.ImageSharp.Color.ParseHex("E6E6E6");

            var borderOuter = new ImageSharpDrawing.Polygon(new SixLabors.ImageSharp.PointF[]
            {
            new(0, 0),
            new(dimensionX - 1, 0),
            new(dimensionX - 1, dimensionY - 1),
            new(0, dimensionY - 1)
            });

            var borderInner = new ImageSharpDrawing.Polygon(new SixLabors.ImageSharp.PointF[]
            {
            new(1, 1),
            new(dimensionX - 2, 1),
            new(dimensionX - 2, dimensionY - 2),
            new(1, dimensionY - 2)
            });

            var drawingOptions = new DrawingOptions()
            {
                GraphicsOptions = new GraphicsOptions()
                {
                    Antialias = false
                }
            };

            image.Mutate(x => x.Draw(drawingOptions, black, 1, borderOuter));
            image.Mutate(x => x.Draw(drawingOptions, gray, 1, borderInner));

            return image;
        }
        
        private static List<Dictionary<string, string>> FillBlankImageResolutions(int width,
            int height,
            List<Dictionary<string, string>> bookFull)
        {
            foreach (var book in bookFull)
            {
                if (book["page"] == "blank" || (book["image"] == string.Empty && book["height"] == string.Empty && book["width"] == string.Empty))
                {
                    book["height"] = height.ToString();
                    book["width"] = width.ToString();
                }
            }

            return bookFull;
        }

        private static (int, int) GetSinglePageResolution(Dictionary<string, ZipArchiveEntry> entryMap,
            List<Dictionary<string, string>> bookFull)
        {
            int dimensionX = 0;
            int dimensionY = 0;

            for (int i = 0; i < bookFull.Count; i++)
            {
                if (!string.IsNullOrEmpty(bookFull[i]["image"]) && i > 0)
                {
                    ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(bookFull[i]["image"])!;
                    using var stream = bookEntry.Open();
                    if (imageExtensions.Any(ext => bookFull[i]["image"].EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var (width, height) = GetImageDimensions(stream);
                        dimensionX = width;
                        dimensionY = height;

                        if (height > width) break;
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

            return;
        }

        private static List<Dictionary<string, string>> ExtractImageStreams(Dictionary<string, ZipArchiveEntry> entryMap,
            string targetCbz,
            List<Dictionary<string, string>> bookFull,
            string readingDirection)
        {
            if (File.Exists(targetCbz)
                || !_processedCbzFiles.TryAdd(targetCbz, true))
            {
                throw new Exception(Fail.Cbz.ToString());
            }

            using ZipArchive destinationArchive = ZipFile.Open(targetCbz, ZipArchiveMode.Create);

            (int singleWidth, int singleHeight) = GetSinglePageResolution(entryMap, bookFull);

            // Handle wide images first
            int numberWideImages = 0;
            if (PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState)
            {
                numberWideImages = bookFull.Count(page => page.GetValueOrDefault("doublepage") == "true");
            }

            string currentChapterFolder = string.Empty;
            int totalChapters = bookFull.Count(page => page.ContainsKey("bookmark"));
            int currentChapterIndex = 0;

            for (int i = 0; i < bookFull.Count; i++)
            {
                string prefix = string.Empty;
                if (i == 0) prefix = "cover_";
                else prefix = "p_";

                var compressionLevel = GetCompressionLevel();

                if (PopupSettings.CheckboxStates.CheckboxChapterFoldersState
                    && totalChapters > 1
                    && bookFull[i].TryGetValue("bookmark", out string? valueBookmark))
                {
                    foreach (char c in invalidPathFileChars)
                    {
                        valueBookmark = valueBookmark.Replace(c, '_');
                    }
                    while (valueBookmark.Contains("__"))
                    {
                        valueBookmark = valueBookmark.Replace("__", "_");
                    }
                    valueBookmark = valueBookmark.Replace("Ä", "A").Replace("ä", "a")
                        .Replace("Ö", "O").Replace("ö", "o")
                        .Replace("Ü", "U").Replace("ü", "u")
                        .Replace("ẞ", "SS").Replace("ß", "ss");


                    valueBookmark = $"{currentChapterIndex.ToString().PadLeft((totalChapters - 1).ToString().Length, '0')} - {valueBookmark}";
                    currentChapterFolder = valueBookmark;

                    currentChapterIndex++;
                }

                string baseFileNameFirst = prefix + i.ToString().PadLeft((bookFull.Count + numberWideImages - 1).ToString().Length, '0') + Path.GetExtension(bookFull[i]["image"]);
                string fullEntryPathFirst = Path.Combine(currentChapterFolder, baseFileNameFirst);
                string baseFileNameSecond = prefix + (i + 1).ToString().PadLeft((bookFull.Count + numberWideImages - 1).ToString().Length, '0') + Path.GetExtension(bookFull[i]["image"]);
                string fullEntryPathSecond = Path.Combine(currentChapterFolder, baseFileNameSecond);

                if (!string.IsNullOrEmpty(bookFull[i]["image"]) &&
                    imageExtensions.Any(ext => bookFull[i]["image"].EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(bookFull[i]["image"])!;

                    if (numberWideImages > 0)
                    {
                        if (i > 0 && bookFull[i]["doublepage"] == "true")
                        {
                            MemoryStream encodedDataLeft = new();
                            MemoryStream encodedDataRight = new();
                            int width = 0;
                            int height = 0;

                            try
                            {
                                (encodedDataLeft, encodedDataRight, width, height) = SplitImageSharp(bookEntry, Path.GetExtension(bookFull[i]["image"]));

                                if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                    && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                    && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                                {
                                    encodedDataLeft = ResizeImage(encodedDataLeft);
                                    encodedDataRight = ResizeImage(encodedDataRight);

                                    (int resizedWidth, int resizedHeight) = CalculateScaling(width, height);
                                    
                                    if (resizedHeight > 0
                                        && resizedWidth > 0)
                                    {
                                        width = resizedWidth;
                                        height = resizedHeight;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                throw new Exception(Fail.Split.ToString());
                            }

                            if (readingDirection == "YesAndRightToLeft")
                            {
                                using (Stream sourceStreamRight = encodedDataRight)
                                {
                                    using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                                    sourceStreamRight.CopyTo(destinationStream);
                                }
                                using (Stream sourceStreamLeft = encodedDataLeft)
                                {
                                    using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathSecond, compressionLevel).Open();
                                    sourceStreamLeft.CopyTo(destinationStream);
                                }
                            }
                            else
                            {
                                using (Stream sourceStreamLeft = encodedDataLeft)
                                {
                                    using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                                    sourceStreamLeft.CopyTo(destinationStream);
                                }
                                using (Stream sourceStreamRight = encodedDataRight)
                                {
                                    using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathSecond, compressionLevel).Open();
                                    sourceStreamRight.CopyTo(destinationStream);
                                }
                            }

                            string wideSingleHeight = height.ToString();
                            string wideSingleWidth = width.ToString();

                            bookFull[i]["height"] = wideSingleHeight;
                            bookFull[i]["width"] = wideSingleWidth;

                            bookFull.Insert(i + 1, new Dictionary<string, string>()
                            {
                                ["page"] = "second spread page",
                                ["image"] = string.Empty,
                                ["spread"] = string.Empty,
                                ["doublepage"] = "false",
                                ["height"] = wideSingleHeight,
                                ["width"] = wideSingleWidth
                            });
                            i++;
                        }
                        else
                        {
                            ZipArchiveEntry destinationEntry = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel);
                            using Stream sourceStream = bookEntry.Open();
                            using Stream destinationStream = destinationEntry.Open();

                            if (PopupSettings.CheckboxStates.CheckboxCropImagesState)
                            {
                                using MemoryStream bufferedSourceStream = new();
                                sourceStream.CopyTo(bufferedSourceStream);
                                bufferedSourceStream.Position = 0;

                                using MemoryStream? croppedSourceStream = CalculateCroppingBorder(bufferedSourceStream, out int croppedWidth, out int croppedHeight);
                                if (croppedSourceStream != null)
                                {
                                    bookFull[i]["height"] = croppedHeight.ToString();
                                    bookFull[i]["width"] = croppedWidth.ToString();

                                    if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                        && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                        && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                                    {
                                        using Stream resizedSourceStream = ResizeImage(croppedSourceStream);
                                        resizedSourceStream.CopyTo(destinationStream);

                                        if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                                        if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                                        (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                                        if (resizedHeight > 0
                                            && resizedWidth > 0)
                                        {
                                            bookFull[i]["height"] = resizedHeight.ToString();
                                            bookFull[i]["width"] = resizedWidth.ToString();
                                        }
                                    }
                                    else croppedSourceStream.CopyTo(destinationStream);
                                }
                                else
                                {
                                    bufferedSourceStream.Position = 0;

                                    if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                        && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                        && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                                    {
                                        using Stream resizedSourceStream = ResizeImage(bufferedSourceStream);
                                        resizedSourceStream.CopyTo(destinationStream);

                                        if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                                        if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                                        (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                                        if (resizedHeight > 0
                                            && resizedWidth > 0)
                                        {
                                            bookFull[i]["height"] = resizedHeight.ToString();
                                            bookFull[i]["width"] = resizedWidth.ToString();
                                        }
                                    }
                                    else bufferedSourceStream.CopyTo(destinationStream);
                                }
                            }
                            else if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                            {
                                using Stream resizedSourceStream = ResizeImage(sourceStream);
                                resizedSourceStream.CopyTo(destinationStream);

                                if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                                if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                                (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                                if (resizedHeight > 0
                                    && resizedWidth > 0)
                                {
                                    bookFull[i]["height"] = resizedHeight.ToString();
                                    bookFull[i]["width"] = resizedWidth.ToString();
                                }
                            }
                            else sourceStream.CopyTo(destinationStream);
                        }
                    }
                    else
                    {
                        ZipArchiveEntry destinationEntry = destinationArchive.CreateEntry(Path.Combine(currentChapterFolder, prefix + i.ToString().PadLeft((bookFull.Count - 1).ToString().Length, '0') + Path.GetExtension(bookFull[i]["image"])), compressionLevel);
                        using Stream sourceStream = bookEntry.Open();
                        using Stream destinationStream = destinationEntry.Open();

                        if (PopupSettings.CheckboxStates.CheckboxCropImagesState)
                        {
                            using MemoryStream bufferedSourceStream = new();
                            sourceStream.CopyTo(bufferedSourceStream);
                            bufferedSourceStream.Position = 0;

                            using MemoryStream? croppedSourceStream = CalculateCroppingBorder(bufferedSourceStream, out int croppedWidth, out int croppedHeight);
                            if (croppedSourceStream != null)
                            {
                                bookFull[i]["height"] = croppedHeight.ToString();
                                bookFull[i]["width"] = croppedWidth.ToString();

                                if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                    && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                    && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                                {
                                    using Stream resizedSourceStream = ResizeImage(croppedSourceStream);
                                    resizedSourceStream.CopyTo(destinationStream);

                                    if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                                    if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                                    (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                                    if (resizedHeight > 0
                                        && resizedWidth > 0)
                                    {
                                        bookFull[i]["height"] = resizedHeight.ToString();
                                        bookFull[i]["width"] = resizedWidth.ToString();
                                    }
                                }
                                else croppedSourceStream.CopyTo(destinationStream);
                            }
                            else
                            {
                                bufferedSourceStream.Position = 0;

                                if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                                    && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                                    && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                                {
                                    using Stream resizedSourceStream = ResizeImage(bufferedSourceStream);
                                    resizedSourceStream.CopyTo(destinationStream);

                                    if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                                    if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                                    (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                                    if (resizedHeight > 0
                                        && resizedWidth > 0)
                                    {
                                        bookFull[i]["height"] = resizedHeight.ToString();
                                        bookFull[i]["width"] = resizedWidth.ToString();
                                    }
                                }
                                else bufferedSourceStream.CopyTo(destinationStream);
                            }
                        }
                        else if (PopupSettings.CheckboxStates.CheckboxResizeImagesState
                            && PopupSettings.CheckboxStates.TextBoxResizeHeightValue > 0
                            && PopupSettings.CheckboxStates.TextBoxResizeWidthValue > 0)
                        {
                            using Stream resizedSourceStream = ResizeImage(sourceStream);
                            resizedSourceStream.CopyTo(destinationStream);

                            if (!int.TryParse(bookFull[i]["height"], out int originalHeight)) { }
                            if (!int.TryParse(bookFull[i]["width"], out int originalWidth)) { }

                            (int resizedWidth, int resizedHeight) = CalculateScaling(originalWidth, originalHeight);

                            if (resizedHeight > 0
                                && resizedWidth > 0)
                            {
                                bookFull[i]["height"] = resizedHeight.ToString();
                                bookFull[i]["width"] = resizedWidth.ToString();
                            }
                        }
                        else sourceStream.CopyTo(destinationStream);
                    }
                }
                else
                {
                    try
                    {
                        bookFull[i]["height"] = singleHeight.ToString();
                        bookFull[i]["width"] = singleWidth.ToString();

                        using var blankImage = CreateBlankImage(singleWidth, singleHeight);

                        using var memoryStream = new MemoryStream();
                        blankImage.SaveAsPng(memoryStream);
                        byte[] encodedData = memoryStream.ToArray();

                        ZipArchiveEntry destinationEntry = destinationArchive.CreateEntry(Path.Combine(currentChapterFolder, prefix + i.ToString().PadLeft((bookFull.Count + numberWideImages - 1).ToString().Length, '0') + ".png"), compressionLevel);

                        using Stream sourceStream = new MemoryStream(encodedData);
                        using Stream destinationStream = destinationEntry.Open();
                        sourceStream.CopyTo(destinationStream);
                    }
                    catch (Exception)
                    {
                        throw new Exception(Fail.Blank.ToString());
                    }
                }
            }

            return bookFull;
        }

        private static List<Dictionary<string, string>> ParseEpubToc(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            XDocument opfDoc,
            string opfPath,
            Dictionary<string, string?> metadata)
        {
            List<Dictionary<string, string>> chapters = [];
            List<string> navPaths = GetNcxFile(opfDoc, opfPath);

            if (navPaths.Count > 0)
            {
                List<Dictionary<string, string>> xhtmlChapters = [];
                List<Dictionary<string, string>> ncxChapters = [];

                foreach (string navPath in navPaths)
                {
                    ZipArchiveEntry tocEntry = entryMap.GetValueOrDefault(navPath)!;
                    using StreamReader reader = new(tocEntry.Open());
                    string tocContent = reader.ReadToEnd();
                    tocContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(tocContent));

                    XDocument tocDoc = XDocument.Parse(tocContent);
                    XNamespace ops = "http://www.idpf.org/2007/ops";
                    XNamespace xhtml = "http://www.w3.org/1999/xhtml";
                    XNamespace ncx = "http://www.daisy.org/z3986/2005/ncx/";

                    if (navPath.EndsWith(".xhtml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var opfMetadata = tocDoc.Descendants(xhtml + "nav")
                            .FirstOrDefault(i => (string?)i.Attribute(ops + "type") == "toc")?
                            .Descendants(xhtml + "a")
                            .Select(a => new { Href = a.Attribute("href")!.Value, Name = a.Value })
                            .ToLookup(item => item.Href, item => item.Name);
                        if (opfMetadata != null)
                        {
                            foreach (var e in opfMetadata)
                            {
                                string title = string.Join(" - ", e);
                                var page = e.Key.ToString().Split('#')[0] ?? string.Empty;
                                string? imagePath = FindImagePathInFile(entryMap, epubFile, RemoveStartingDots(page), metadata);
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    imagePath = entryMap
                                        .Where(map => map.Key.Contains(imagePath, StringComparison.InvariantCultureIgnoreCase))
                                        .Select(map => map.Key)
                                        .FirstOrDefault(string.Empty);
                                }

                                xhtmlChapters.Add(new Dictionary<string, string>()
                                {
                                    ["title"] = title,
                                    ["page"] = page,
                                    ["image"] = imagePath ?? string.Empty
                                });
                            }
                        }
                    }
                    else if (navPath.EndsWith(".ncx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var navPoint in tocDoc.Descendants(ncx + "navPoint"))
                        {
                            string title = navPoint.Descendants(ncx + "text").FirstOrDefault()?.Value.Trim() ?? string.Empty;
                            var page = navPoint.Descendants(ncx + "content").FirstOrDefault()?.Attribute("src")?.Value.Split('#')[0] ?? string.Empty;

                            string? imagePath = FindImagePathInFile(entryMap, epubFile, RemoveStartingDots(page), metadata);
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                imagePath = entryMap
                                    .Where(map => map.Key.Contains(imagePath, StringComparison.InvariantCultureIgnoreCase))
                                    .Select(map => map.Key)
                                    .FirstOrDefault(string.Empty);
                            }
                            ncxChapters.Add(new Dictionary<string, string>()
                            {
                                ["title"] = title,
                                ["page"] = page,
                                ["image"] = imagePath ?? string.Empty
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
                if (chapters[i]["page"].Split('#')[0] == chapters[i + 1]["page"].Split('#')[0])
                {
                    chapters[i]["title"] = chapters[i]["title"] + " - " + chapters[i + 1]["title"];
                    chapters.RemoveAt(i + 1);
                }
            }

            return chapters;
        }

        private static string RemoveStartingDots(string path)
        {
            return path.StartsWith("../") ? path[3..] :
                   path.StartsWith("./") ? path[2..] :
                   path;
        }

        private static string FindImagePathInCss(Dictionary<string, ZipArchiveEntry> entryMap,
            string filename,
            string pageId)
        {
            string imagePath = string.Empty;
            filename = RemoveStartingDots(filename);

            ZipArchiveEntry fileEntry = entryMap.GetValueOrDefault(filename)!;
            using StreamReader reader = new(fileEntry.Open());
            string fileContent = reader.ReadToEnd();
            fileContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(fileContent));

            var parser = new StylesheetParser();
            var stylesheet = parser.Parse(fileContent);

            string selector = "#" + pageId;
            var rule = stylesheet.StyleRules.FirstOrDefault(r => r.SelectorText.ToString() == selector);

            if (rule != null)
            {
                imagePath = rule.Style.BackgroundImage.ToString();

                int startIndex = imagePath.IndexOf('\"') + 1;
                int endIndex = imagePath.LastIndexOf('\"');

                imagePath = imagePath[startIndex..endIndex];

                imagePath = RemoveStartingDots(imagePath);
            }

            return imagePath;
        }

        private static string? FindImagePathInFile(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            string filename,
            Dictionary<string, string?> metadata)
        {
            string imagePath = string.Empty;

            string? authors = metadata.GetValueOrDefault("Authors", string.Empty);
            string? publisher = metadata.GetValueOrDefault("Publisher", string.Empty);

            string actualFilename = entryMap
                        .Where(map => map.Key.Contains(filename, StringComparison.InvariantCultureIgnoreCase))
                        .Select(map => map.Key)
                        .FirstOrDefault(string.Empty);

            if (!string.IsNullOrEmpty(actualFilename))
            {
                if (actualFilename.EndsWith(".xhtml", StringComparison.InvariantCultureIgnoreCase) ||
                    actualFilename.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) ||
                    actualFilename.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    ZipArchiveEntry fileEntry = entryMap.GetValueOrDefault(actualFilename)!;
                    using StreamReader reader = new(fileEntry.Open());
                    string fileContent = reader.ReadToEnd();
                    fileContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(fileContent));

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
                        if (authors == "Shigeru Mizuki"
                            || publisher == "Drawn &amp; Quarterly")
                        {
                            foreach (var itemSrcListFile in itemSrcList)
                            {
                                if (!itemSrcListFile.Value.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    itemSrc = itemSrcListFile.Value;
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(itemSrc))
                        {
                            List<(string ImageSrc, int Width, int Height)> imageData = [];

                            foreach (var itemSrcListFile in itemSrcList)
                            {
                                string itemSrcFile = entryMap
                                    .Where(map => map.Key.Contains(RemoveStartingDots(itemSrcListFile.Value), StringComparison.InvariantCultureIgnoreCase))
                                    .Select(map => map.Key)
                                    .FirstOrDefault(string.Empty);
                                ZipArchiveEntry bookEntry = entryMap.GetValueOrDefault(itemSrcFile)!;
                                using var stream = bookEntry.Open();
                                (int width, int height) = GetImageDimensions(stream);

                                imageData.Add((itemSrcListFile.Value, width, height));
                            }

                            var (ImageSrc, Width, Height) = imageData
                                .OrderByDescending(d => Math.Max(d.Width, d.Height))
                                .ThenByDescending(d => Math.Min(d.Width, d.Height))
                                .FirstOrDefault();

                            itemSrc = ImageSrc;
                        }
                    }
                    else if (itemSrcList.Count == 1)
                    {
                        itemSrc = (string)itemSrcList[0].Value;
                    }

                    if (string.IsNullOrEmpty(itemSrc))
                    {
                        var itemXlink = fileDoc.Descendants(ns + "body").Descendants(svg + "image").FirstOrDefault();
                        if (itemXlink != null) imagePath = (string)itemXlink.Attribute(xlink + "href")!;
                    }
                    else imagePath = itemSrc;
                }
                else if (imageExtensions.Any(ext => actualFilename.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)))
                {
                    imagePath = actualFilename;
                }
            }

            return RemoveStartingDots(imagePath);
        }

        private static (string seriesName, string volumeNumber, string isVolumeOrChapter) GetVolumeAndChapterNumber(string epubFilename)
        {
            int vIndex = epubFilename.LastIndexOf("v", StringComparison.InvariantCultureIgnoreCase);
            if (vIndex > 0
                && vIndex + 1 < epubFilename.Length
                && epubFilename[vIndex - 1] == ' ')
            {
                if (int.TryParse(epubFilename[(vIndex + 1)..], out int volumeNumber))
                {
                    return (epubFilename[..vIndex].TrimEnd(), volumeNumber.ToString(), "v");
                }
            }

            int cIndex = epubFilename.LastIndexOf("c", StringComparison.InvariantCultureIgnoreCase);
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

        private static void WriteChaptersToXml(string targetCbz,
            string epubFilename,
            string readingDirection,
            List<Dictionary<string, string>> bookFull,
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
                    && !string.IsNullOrEmpty(PopupSettings.CheckboxStates.TextboxReplaceSeriesState!.Trim()))
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
            xmlWriter.WriteElementString("Notes", "Created using: epub2cbz-gui");
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
                || PopupSettings.CheckboxStates.CheckboxImageSizeState)
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
                    if (bookFull[i]["doublepage"] == "true"
                        && (!PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState
                        || !MainForm.FormElements.CheckboxExtractImagesState))
                    {
                        xmlWriter.WriteAttributeString("DoublePage", "True");
                    }
                    if (PopupSettings.CheckboxStates.CheckboxChaptersState)
                    {
                        if (PopupSettings.CheckboxStates.CheckboxOffsetChaptersState)
                        {
                            if (i <= 1)
                            {
                                if (bookFull[i].TryGetValue("bookmark", out string? valueBookmark))
                                {
                                    xmlWriter.WriteAttributeString("Bookmark", valueBookmark);
                                }
                            }
                            else if (bookFull[i - 1].TryGetValue("bookmark", out string? valueBookmark))
                            {
                                xmlWriter.WriteAttributeString("Bookmark", valueBookmark);
                            }
                        }
                        else
                        {
                            if (bookFull[i].TryGetValue("bookmark", out string? valueBookmark))
                            {
                                xmlWriter.WriteAttributeString("Bookmark", valueBookmark);
                            }
                        }
                    }
                    if (PopupSettings.CheckboxStates.CheckboxImageSizeState)
                    {
                        xmlWriter.WriteAttributeString("ImageWidth", bookFull[i]["width"]);
                        xmlWriter.WriteAttributeString("ImageHeight", bookFull[i]["height"]);
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
            using StreamReader reader = new(xmlEntry.Open());
            string xmlContent = reader.ReadToEnd();
            xmlContent = Encoding.UTF8.GetString(reader.CurrentEncoding.GetBytes(xmlContent));

            XDocument xmlDoc = XDocument.Parse(xmlContent);
            XNamespace xmlns = "urn:oasis:names:tc:opendocument:xmlns:container";

            var item = xmlDoc.Descendants(xmlns + "rootfiles").Descendants(xmlns + "rootfile").FirstOrDefault();
            string opfFile = string.Empty;

            if (item != null) opfFile = item.Attribute("full-path")?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(opfFile)) return opfFile;
            else throw new Exception(Resources.OPFFileNotFound);
        }

        private static string GetCssFile(string opfPath, XDocument opfDoc)
        {
            opfPath = opfPath[..(opfPath.LastIndexOf('/') + 1)];

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var item = opfDoc.Descendants(opf + "manifest").Descendants(opf + "item").FirstOrDefault(i => (string)i.Attribute("media-type")! == "text/css");
            if (item != null) opfPath += (string)item.Attribute("href")!;

            return opfPath;
        }

        private static List<string> GetNcxFile(XDocument opfDoc,
            string opfPath)
        {
            opfPath = opfPath[..(opfPath.LastIndexOf('/') + 1)];

            XNamespace opf = "http://www.idpf.org/2007/opf";

            var navigationItems = opfDoc.Descendants(opf + "manifest")
                                .Descendants(opf + "item")
                                .Where(i =>
                                    (string)i.Attribute("media-type")! == "application/x-dtbncx+xml" ||
                                    (string)i.Attribute("properties")! == "nav"
                                )
                                .Select(item => (string)item.Attribute("href")!)
                                .Distinct();

            List<string> navPaths = [.. navigationItems.Select(item => opfPath + item)];

            return navPaths;
        }

        private static (List<Dictionary<string, string>> bookFull, bool? correctSpread) RemoveDuplicateCover(Dictionary<string, ZipArchiveEntry> entryMap,
            List<Dictionary<string, string>> bookFull,
            string epubFilename,
            bool? correctSpread)
        {
            bool wasWide = false;

            if (PopupSettings.CheckboxStates.CheckboxHigherResolutionCover
                && !string.IsNullOrEmpty(bookFull[0]["height"])
                && !string.IsNullOrEmpty(bookFull[1]["height"]))
            {
                if (int.Parse(bookFull[0]["height"]) > int.Parse(bookFull[1]["height"]))
                {
                    if (bookFull[1]["doublepage"] == "true") wasWide = true;
                    bookFull.RemoveAt(1);
                }
                else bookFull.RemoveAt(0);
            }
            else
            {
                if (bookFull[1]["doublepage"] == "true") wasWide = true;
                bookFull.RemoveAt(1);
            }

#if DEBUG
            string debugMessage = string.Empty;
#endif

            if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                if (correctSpread == true
                    && wasWide == false
                    && bookFull[1]["doublepage"] == "false")
                {
                    bool isBlank = false;

                    if (!string.IsNullOrEmpty(bookFull[1]["image"])
                        && PopupSettings.CheckboxStates.CheckboxBlankImageState)
                    {
                        isBlank = IsImageBlankWhite(entryMap, bookFull[1]["image"]);
                    }
                    else if (bookFull[1]["image"] == string.Empty
                        && PopupSettings.CheckboxStates.CheckboxBlankImageState)
                    {
                        isBlank = true;
                    }

                    // add blank image to keep correct page spread
                    if (!isBlank)
                    {
                        bookFull.Insert(1, new Dictionary<string, string>()
                        {
                            ["page"] = "blank",
                            ["image"] = string.Empty,
                            ["spread"] = string.Empty,
                            ["doublepage"] = "false",
                            ["height"] = string.Empty,
                            ["width"] = string.Empty
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

            return (bookFull, correctSpread);
        }

        private static List<Dictionary<string, string>> InsertBlankPage (Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFilename,
            List<Dictionary<string, string>> bookFull)
        {
            bool isBlank = false;

            if (!string.IsNullOrEmpty(bookFull[1]["image"])
                && PopupSettings.CheckboxStates.CheckboxBlankImageState)
            {
                isBlank = IsImageBlankWhite(entryMap, bookFull[1]["image"]);
            }
            else if (bookFull[1]["image"] == string.Empty
                && PopupSettings.CheckboxStates.CheckboxBlankImageState)
            {
                isBlank = true;
            }

            // add blank image to keep correct page spread
            if (!isBlank)
            {
                bookFull.Insert(1, new Dictionary<string, string>()
                {
                    ["page"] = "blank",
                    ["image"] = string.Empty,
                    ["spread"] = string.Empty,
                    ["doublepage"] = "false",
                    ["height"] = string.Empty,
                    ["width"] = string.Empty
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

            return bookFull;
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
                    if (imageExtensionsSimple.Any(suffix => pair.Key.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase)))
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
            List<Dictionary<string, string>> pages = ParseSpineXml(opfDoc);

            /// Try to check if Epub is still DRM protected
            /// 

            if (PopupSettings.CheckboxStates.CheckboxDRMProtectionState && CheckDRMProtection(entryMap, pages))
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

            List<Dictionary<string, string>> bookFull = ParseOpfPagesXml(entryMap, epubFile, opfPath, opfDoc, pages, metadata);

            bookFull = ParseAlternativeCover(entryMap, epubFile, opfDoc, bookFull);

            if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                bookFull = FixPageAlignmentPost(bookFull, readingDirection);
            }

            List<Dictionary<string, string>> chapters = ParseEpubToc(entryMap, epubFile, opfDoc, opfPath, metadata);
            chapters = ParseAlternativeToc(entryMap, opfDoc, chapters, bookFull);
            bool? correctSpread = CheckPageSpread(readingDirection, bookFull);

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
                (bookFull, correctSpread) = CheckDuplicateCover(chapters, bookFull, entryMap, opfDoc, epubFilename, correctSpread, epubFile);
            }

            if (correctSpread == false && PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                bookFull = InsertBlankPage(entryMap, epubFilename, bookFull);
            }

            if (PopupSettings.CheckboxStates.CheckboxInsertAdditionalBlankImageState)
            {
                bookFull.Insert(1, new Dictionary<string, string>()
                {
                    ["page"] = "blank",
                    ["image"] = string.Empty,
                    ["spread"] = string.Empty,
                    ["doublepage"] = "false",
                    ["height"] = string.Empty,
                    ["width"] = string.Empty
                });
            }

            if (PopupSettings.CheckboxStates.CheckboxRemoveFirstPageState)
            {
                if (string.IsNullOrEmpty(bookFull[1]["image"])
                    || IsImageBlankWhite(entryMap, bookFull[1]["image"]))
                {
                    bookFull.RemoveAt(1);
                }
            }

            bookFull = IntegrateChapters(bookFull, chapters);

            ///
            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
            ///

            if (MainForm.FormElements.CheckboxExtractImagesState)
            {
                try
                {
                    bookFull = ExtractImageStreams(entryMap, targetCbz, bookFull, readingDirection);
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

                    bookFull = FillBlankImageResolutions(width, height, bookFull);
                }

                WriteChaptersToXml(targetCbz, epubFilename, readingDirection, bookFull, metadata);
            }

            Interlocked.Increment(ref numberCurrentEpub);
            AppendColoredText($"({numberCurrentEpub.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                + string.Format(Resources.Processed, epubFilename) + Environment.NewLine, System.Drawing.Color.Green);
            ProgressBarStep();

            bookFull.Clear();
            entryMap.Clear();
            return;
        }

        private static List<string> ReadMangaList(string filename)
        {
            List<string> mangaList = [];
            using (StreamReader reader = new(filename, Encoding.UTF8))
            {
                string? line = string.Empty;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.EndsWith(".cbz"))
                    {
                        line = string.Concat(line.AsSpan(0, line.Length - 4), ".epub");
                    }
                    mangaList.Add(line);
                }
            }
            
            return mangaList;
        }

        private static void DisableControls(MainForm mainForm)
        {
            mainForm.checkBoxComicInfo.Enabled = false;
            mainForm.checkBoxImages.Enabled = false;
            mainForm.buttonPath.Enabled = false;
            mainForm.buttonPathClear.Enabled = false;
            mainForm.buttonSwitchModes.Enabled = false;
            mainForm.buttonFileModeFileList.Enabled = false;
            mainForm.buttonStart.Text = Resources.AbortButtonText;
            mainForm.comboBoxLanguage.Enabled = false;
            mainForm.buttonOpenSettings.Enabled = false;
        }

        private static void EnableControls(MainForm mainForm)
        {
            mainForm.checkBoxComicInfo.Enabled = true;
            mainForm.checkBoxImages.Enabled = true;
            mainForm.buttonPath.Enabled = true;
            mainForm.buttonPathClear.Enabled = true;
            mainForm.buttonSwitchModes.Enabled = true;
            mainForm.buttonFileModeFileList.Enabled = true;
            mainForm.buttonStart.Enabled = true;
            mainForm.buttonStart.Text = Resources.StartButtonText;
            mainForm.comboBoxLanguage.Enabled = true;
            mainForm.buttonOpenSettings.Enabled = true;
        }

        private static void HandleCompletion(TimeSpan ts,
            bool wasAborted,
            MainForm mainForm)
        {
            AppendColoredText(Environment.NewLine + string.Format(Resources.Timer, ts.Minutes, ts.Seconds, ts.Milliseconds), System.Drawing.Color.White);

            if (wasAborted)
            {
                AppendColoredText(Environment.NewLine + Environment.NewLine + Resources.AbortedMessage, System.Drawing.Color.Red);
            }

            EnableControls(mainForm);

            _processedCbzFiles.Clear();

            cts.Dispose();
            cts = new();

            if (PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                mainForm.fileListForm.fileListDataTable.Clear();
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

            MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault()!;

            string rootDir = string.Empty;
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                rootDir = inputFolderName ?? string.Empty;
            }
            else if (MainForm.FileNameClass.FileNames.Count < 1)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoEpubsFoundMessageBox, Resources.ErrorMessageBox);

                EnableControls(mainForm);
                stopwatch.Stop();
                return;
            }

            mainForm.outputBoxConsole.Text = string.Empty;
            mainForm.toolStripProgressBar.Value = 0;

            DisableControls(mainForm);
            numberCurrentEpub = 0;
            bool wasAborted = false;

            AppendColoredText(Resources.ProcessingInProgress + Environment.NewLine, System.Drawing.Color.White);
            AppendColoredText(Environment.NewLine, System.Drawing.Color.White);

            if (string.IsNullOrEmpty(rootDir) && !PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoPathMessageBox, Resources.ErrorMessageBox);

                EnableControls(mainForm);
                stopwatch.Stop();
                return;
            }

            List<string> epubPaths = [];
            HashSet<string>? mangaList = null;

#if DEBUG
            if (MainForm.FormElements.CheckboxMangaListState && !PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                string mangalistPath = Path.Combine(rootDir, "mangalist.txt");
                if (!File.Exists(mangalistPath))
                {
                    using PopupInfoError popupInfoError = new();
                    popupInfoError.ShowInfoError(Resources.NoMangalistMessageBox, Resources.ErrorMessageBox);

                    EnableControls(mainForm);
                    stopwatch.Stop();
                    return;
                }
                mangaList = new(ReadMangaList(mangalistPath), StringComparer.InvariantCultureIgnoreCase);
            }
#endif

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

                            if (File.Exists(epubPath)
                                && (!MainForm.FormElements.CheckboxMangaListState
                                    || (mangaList != null && mangaList.Contains(Path.GetFileName(epubPath)))))
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
                HandleCompletion(stopwatch.Elapsed, wasAborted, mainForm);
                return;
            }
            catch (Exception ex)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError($"{ex.Message}", Resources.ErrorMessageBox);

                EnableControls(mainForm);
                stopwatch.Stop();
                return;
            }

            if (epubPaths.Count <= 0)
            {
                using PopupInfoError popupInfoError = new();
                popupInfoError.ShowInfoError(Resources.NoEpubsFoundMessageBox, Resources.ErrorMessageBox);

                EnableControls(mainForm);
                stopwatch.Stop();
                return;
            }


            numberEpubs = epubPaths.Count;

            mainForm.toolStripProgressBar.Maximum = numberEpubs;

            using PopupSettings popup = new();
            int? nullableDegree = popup.dropDownThreads.Items[PopupSettings.CheckboxStates.DropDownParallelismDegreeState] as int?;

            if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState) nullableDegree = Environment.ProcessorCount - 1;

            int maxDegreeOfParallelism = nullableDegree ?? (Environment.ProcessorCount - 1);
            maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);

            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        Parallel.ForEach(epubPaths,
                            new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism, CancellationToken = cts.Token },
                            (epubPath, loopState) =>
                            {
                                try
                                {
                                    ProcessEpub(epubPath, cts.Token);
                                }
                                catch (OperationCanceledException)
                                {
                                    loopState.Stop();
                                }
                            });
                    }
                    catch (OperationCanceledException)
                    {
                        wasAborted = true;
                    }

                }, cts.Token);
            }
            finally
            {
                stopwatch.Stop();
                HandleCompletion(stopwatch.Elapsed, wasAborted, mainForm);
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
            Application.Run(new MainForm());
        }
    }
}