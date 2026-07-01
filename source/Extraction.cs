using epub2cbz.Properties;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;

namespace epub2cbz
{
    internal class Extraction
    {
        private static readonly Dictionary<string, IImageEncoder> ImageSharpFormatToEncoding = new()
        {
            { ".png", new PngEncoder() },
            { ".jpg", new JpegEncoder() { Quality = 90 } },
            { ".jpeg", new JpegEncoder() { Quality = 90 }},
            { ".gif", new GifEncoder() },
            { ".webp", new WebpEncoder() { Quality = 90 } },
        };

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

        public static void ExtractImageStreams(string epubFilename,
            Dictionary<string, ZipArchiveEntry> entryMap,
            string targetCbz,
            string tempTarget,
            List<BookInfo.EpubPage> bookFull,
            string readingDirection)
        {
            if (File.Exists(targetCbz)
                || !Program._processedCbzFiles.TryAdd(targetCbz, true))
            {
                int currentProgress = Interlocked.Increment(ref Program.numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(Program.numberEpubs.ToString().Length, '0')}/{Program.numberEpubs}) - "
                    + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, System.Drawing.Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                throw new Exception("Fail");
            }

            using ZipArchive destinationArchive = ZipFile.Open(tempTarget, ZipArchiveMode.Create);

            (int singleWidth, int singleHeight) = ImageProcessing.GetSinglePageResolution(bookFull);

            bool doSplit = PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState;
            int numberWideImages = doSplit ? bookFull.Count(page => page.Doublepage == true) : 0;

            string currentChapterFolder = string.Empty;
            int totalChapters = bookFull.Count(page => !string.IsNullOrEmpty(page.Bookmark));
            int currentChapterIndex = 0;

            var compressionLevel = Program.GetCompressionLevel();
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
                    foreach (char c in Program.invalidPathFileChars) valueBookmark = valueBookmark.Replace(c, '_');
                    while (valueBookmark.Contains("__")) valueBookmark = valueBookmark.Replace("__", "_");

                    currentChapterFolder = $"{currentChapterIndex.ToString().PadLeft((totalChapters - 1).ToString().Length, '0')} - {valueBookmark}/";
                    currentChapterIndex++;
                }

                string baseFileNameFirst = $"{prefix}{i.ToString().PadLeft(padLength, '0')}{Path.GetExtension(bookFull[i].Image)}";
                string fullEntryPathFirst = $"{currentChapterFolder}{baseFileNameFirst}";

                if (!bookFull[i].Blank
                    && !string.IsNullOrEmpty(bookFull[i].Image)
                    && Program.imageExtensions.Any(ext => bookFull[i].Image.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
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

                            if (doCrop) ImageProcessing.ApplyCropping(imageToProcess);

                            int halfWidth = imageToProcess.Width / 2;
                            int imageHeight = imageToProcess.Height;
                            IImageEncoder encoder = ImageSharpFormatToEncoding[extension];

                            using Image<Rgba32> leftImage = imageToProcess.Clone(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(0, 0, halfWidth, imageHeight)));
                            if (doResize) ImageProcessing.ApplyResizing(leftImage);

                            imageToProcess.Mutate(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(halfWidth, 0, imageToProcess.Width - halfWidth, imageHeight)));
                            if (doResize) ImageProcessing.ApplyResizing(imageToProcess);

                            Image<Rgba32> firstImage = readingDirection == "YesAndRightToLeft" ? imageToProcess : leftImage;
                            Image<Rgba32> secondImage = readingDirection == "YesAndRightToLeft" ? leftImage : imageToProcess;

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
                            int currentProgress = Interlocked.Increment(ref Program.numberCurrentEpub);
                            UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(Program.numberEpubs.ToString().Length, '0')}/{Program.numberEpubs}) - "
                                + Resources.SplitImageError + $" '{epubFilename}'" + Environment.NewLine, System.Drawing.Color.Red);
                            UserInterface.ProgressBarStep(currentProgress);

                            if (File.Exists(tempTarget)) File.Delete(tempTarget);

                            throw new Exception("Fail");
                        }
                    }
                    else
                    {
                        if (doCrop || doResize)
                        {
                            bool cropped, resized;

                            int maxWidth = PopupSettings.CheckboxStates.TextBoxResizeWidthValue;
                            int maxHeight = PopupSettings.CheckboxStates.TextBoxResizeHeightValue;

                            if (!doCrop && bookFull[i].Width == maxWidth && bookFull[i].Height == maxHeight)
                            {
                                using Stream sourceStream = bookEntry.Open();
                                using Stream destinationStream = destinationArchive.CreateEntry(fullEntryPathFirst, compressionLevel).Open();
                                sourceStream.CopyTo(destinationStream);

                                bookFull[i] = bookFull[i] with { Size = bookEntry.Length };
                                continue;
                            }

                            using (Stream sourceStream = bookEntry.Open())
                            using (Image<Rgba32> imageToProcess = SixLabors.ImageSharp.Image.Load<Rgba32>(sourceStream))
                            {
                                cropped = doCrop && ImageProcessing.ApplyCropping(imageToProcess);
                                resized = doResize && ImageProcessing.ApplyResizing(imageToProcess);

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
                        if (cachedBlankImageData is null)
                        {
                            using var blankImage = ImageProcessing.CreateBlankImage(singleWidth, singleHeight);
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
                        int currentProgress = Interlocked.Increment(ref Program.numberCurrentEpub);
                        UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(Program.numberEpubs.ToString().Length, '0')}/{Program.numberEpubs}) - "
                            + Resources.BlankImageError + string.Format(Resources.NotAManga, epubFilename) + Environment.NewLine, System.Drawing.Color.Red);
                        UserInterface.ProgressBarStep(currentProgress);

                        if (File.Exists(tempTarget)) File.Delete(tempTarget);

                        throw new Exception("Fail");
                    }
                }
            }
        }
    }
}
