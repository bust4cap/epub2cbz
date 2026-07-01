using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml;

namespace epub2cbz
{
    internal class ComicInfo
    {
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

        public static void WriteComicInfoXml(string targetCbz,
            string epubFilename,
            string readingDirection,
            List<BookInfo.EpubPage> bookFull,
            Dictionary<string, string?> metadata)
        {
            (string seriesName, string volumeNumber, string isVolumeOrChapter) = GetVolumeAndChapterNumber(Path.GetFileName(epubFilename));

            string comicInfo = "ComicInfo.xml";

            var compressionLevel = Program.GetCompressionLevel();

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
    }
}
