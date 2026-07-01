using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace epub2cbz
{
    internal class MetadataParsing
    {
        public static (Dictionary<string, string?>, string readDir) ParseMetadataXml(XDocument xmlDoc)
        {
            Dictionary<string, string?> metadata = [];

            XNamespace opf = "http://www.idpf.org/2007/opf";
            XNamespace dc = "http://purl.org/dc/elements/1.1/";

            XElement? xmlMetadata = xmlDoc.Root?.Element(opf + "metadata") ?? xmlDoc.Descendants(opf + "metadata").FirstOrDefault();
            if (xmlMetadata is not null && MainForm.FormElements.CheckboxComicInfoState)
            {
                if (PopupSettings.CheckboxStates.CheckboxTranslatorState) metadata["Translators"] = ReturnMetadataContributors("trl", xmlMetadata, opf, dc);
                if (PopupSettings.CheckboxStates.CheckboxProducerState) metadata["Producers"] = ReturnMetadataContributors("pro", xmlMetadata, opf, dc);

                if (PopupSettings.CheckboxStates.CheckboxIsbnAsinState) metadata["ISBN"] = ReturnMetadataISBN(xmlMetadata, opf, dc);

                string? title = null, publisher = null, date = null, description = null, series = null, seriesIndex = null, bookType = null, rights = null;
                HashSet<string> authors = [];
                List<string> languages = [];

                foreach (XElement el in xmlMetadata.Elements())
                {
                    string value = el.Value.Trim();

                    if (el.Name == dc + "title" && title is null) title = value;
                    else if (el.Name == dc + "publisher" && publisher is null) publisher = value;
                    else if (el.Name == dc + "date" && date is null) date = value;
                    else if (el.Name == dc + "description" && description is null) description = value;
                    else if (el.Name == dc + "creator" && !string.IsNullOrWhiteSpace(value)) authors.Add(value);
                    else if (el.Name == dc + "language" && value.Length > 0)
                    {
                        languages.Add(value.Length > 2 ? value[..2] : value);
                    }
                    else if (el.Name == dc + "rights" && rights is null) rights = value;
                    else if (el.Name == opf + "meta")
                    {
                        string? nameAttr = (string?)el.Attribute("name");
                        string? contentAttr = (string?)el.Attribute("content") ?? (string?)el;

                        if (nameAttr == "calibre:series") series = contentAttr?.Trim();
                        else if (nameAttr == "calibre:series_index") seriesIndex = contentAttr?.Trim();
                        else if (nameAttr == "book-type") bookType = contentAttr?.Trim();
                    }
                }
#if DEBUG
                metadata["Booktype"] = bookType;
                metadata["Rights"] = rights;
#endif
                if (PopupSettings.CheckboxStates.CheckboxTitleState || PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                    metadata["Title"] = title;

                if (PopupSettings.CheckboxStates.CheckboxSeriesState) metadata["Series"] = series;

                if (PopupSettings.CheckboxStates.CheckboxVolumeState) metadata["SeriesIndex"] = seriesIndex;

                if (PopupSettings.CheckboxStates.CheckboxAuthorState && authors.Count > 0)
                    metadata["Authors"] = string.Join(" & ", authors);

                if (PopupSettings.CheckboxStates.CheckboxLanguageState)
                {
                    if (languages.Count > 0)
                    {
                        metadata["Language"] = languages.Contains("en") ? "en" :
                                               languages.Contains("ja") ? "ja" :
                                               languages[0];
                    }
                    else metadata["Language"] = null;
                }

                if (PopupSettings.CheckboxStates.CheckboxPublisherState) metadata["Publisher"] = publisher;

                if (PopupSettings.CheckboxStates.CheckboxDateState && date is not null)
                    metadata["Date"] = date.Length > 10 ? date[..10] : date;

                if (PopupSettings.CheckboxStates.CheckboxDescriptionState && description is not null)
                {
                    string match = @"\s{2,}";
                    metadata["Description"] = Regex.Replace(description, match, "\n").Trim();
                }
            }

            string readingDirection = "No";
            XElement? xmlReadingDirection = xmlDoc.Root?.Element(opf + "spine") ?? xmlDoc.Descendants(opf + "spine").FirstOrDefault();

            if (xmlReadingDirection?.Attribute("page-progression-direction")?.Value == "rtl")
                readingDirection = "YesAndRightToLeft";

            return (metadata, readingDirection);
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

            if (chrChecksum == char.ToUpper(identifier[9])) return true;
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
            if (xmlMetadata.Nodes().Any(n => n.NodeType == XmlNodeType.Comment))
            {
                xmlMetadata = XElement.Parse(xmlMetadata.ToString().Replace("<!--", string.Empty).Replace("-->", string.Empty));
            }

            string? asin = null;
            string? identifier = null;
            string? asinIdentifier = null;
            string? source = null;

            foreach (XElement el in xmlMetadata.Elements())
            {
                if (el.Name == opf + "meta")
                {
                    if (asin is null && (string?)el.Attribute("name") == "ASIN")
                    {
                        asin = (string?)el.Attribute("content");
                    }
                }
                else if (el.Name == dc + "identifier")
                {
                    identifier ??= el.Value;

                    if (asinIdentifier is null && (string?)el.Attribute(opf + "scheme") == "MOBI-ASIN")
                    {
                        asinIdentifier = el.Value;
                    }
                }
                else if (el.Name == dc + "source" && source is null)
                {
                    source = el.Value;
                }
            }

            if (!string.IsNullOrEmpty(asin)
                && asin.Length == 10)
            {
                return ("ASIN: " + asin).Trim();
            }
            else if (!string.IsNullOrEmpty(asinIdentifier)
                && asinIdentifier.Length == 10)
            {
                return ("ASIN: " + asinIdentifier).Trim();
            }
            else if (!string.IsNullOrEmpty(identifier)
                && identifier.StartsWith("urn:asin:", StringComparison.OrdinalIgnoreCase)
                && identifier.Length == 19)
            {
                return ("ASIN: " + identifier[9..]).Trim();
            }

            if (!string.IsNullOrEmpty(identifier)
                && !identifier.StartsWith("urn:uuid:", StringComparison.OrdinalIgnoreCase)
                && !identifier.StartsWith("calibre:", StringComparison.OrdinalIgnoreCase))
            {
                return ReturnMetadataISBNCalculated(identifier);
            }

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
            List<string> targetIds = [];
            Dictionary<string, string> contributorMap = [];

            foreach (XElement el in xmlMetadata.Elements())
            {
                if (el.Name == opf + "meta")
                {
                    if ((string?)el.Attribute("property") == "role" &&
                        (string?)el.Attribute("scheme") == "marc:relators" &&
                        string.Equals(el.Value, role, StringComparison.OrdinalIgnoreCase))
                    {
                        string? refines = (string?)el.Attribute("refines");
                        if (!string.IsNullOrEmpty(refines))
                        {
                            string id = refines.StartsWith('#') ? refines[1..] : refines;
                            targetIds.Add(id);
                        }
                    }
                }
                else if (el.Name == dc + "contributor")
                {
                    string? id = (string?)el.Attribute("id");
                    if (!string.IsNullOrEmpty(id))
                    {
                        contributorMap[id] = el.Value.Trim();
                    }
                }
            }

            if (targetIds.Count == 0) return string.Empty;

            HashSet<string> matchedContributors = [];
            foreach (string id in targetIds)
            {
                if (contributorMap.TryGetValue(id, out string? name))
                {
                    matchedContributors.Add(name);
                }
            }

            return string.Join(" & ", matchedContributors);
        }
    }
}
