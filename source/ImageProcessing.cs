using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO.Compression;

namespace epub2cbz
{
    internal class ImageProcessing
    {
        public static bool CompareImages(Dictionary<string, ZipArchiveEntry> entryMap,
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
                    UserInterface.AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Duplicate Cover Similarity: "
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

        public static bool ApplyCropping(Image<Rgba32> originalImage)
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

        public static bool ApplyResizing(SixLabors.ImageSharp.Image image)
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

        public static bool IsImageBlankWhite(Dictionary<string, ZipArchiveEntry> entryMap,
            string bookImage)
        {
            try
            {
                ZipArchiveEntry? bookEntry = entryMap.GetValueOrDefault(bookImage);

                if (bookEntry is null) return false;

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

        public static Image<Rgba32> CreateBlankImage(int dimensionX,
            int dimensionY)
        {
            var image = new Image<Rgba32>(dimensionX, dimensionY, SixLabors.ImageSharp.Color.White);

            return image;
        }

        public static (int, int) GetSinglePageResolution(List<BookInfo.EpubPage> bookFull)
        {
            int dimensionX = 0;
            int dimensionY = 0;

            for (int i = 0; i < bookFull.Count; i++)
            {
                if (!string.IsNullOrEmpty(bookFull[i].Image) && i > 0)
                {
                    if (Program.imageExtensions.Any(ext => bookFull[i].Image.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        dimensionX = bookFull[i].Width;
                        dimensionY = bookFull[i].Height;

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
    }
}
