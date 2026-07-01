using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace epub2cbz
{
    internal class EpubDetection
    {
        public static bool CheckEPUB(string epubFile)
        {
            using FileStream fs = new(epubFile, FileMode.Open, FileAccess.Read);
            if (fs.Length < 4) return false;

            Span<byte> buffer = stackalloc byte[4];
            fs.ReadExactly(buffer);

            return BinaryPrimitives.ReadUInt32BigEndian(buffer) == 0x504B0304;  // EPUB
        }

        public static bool CheckDRMProtection(Dictionary<string, ZipArchiveEntry> entryMap,
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
            else if (Program.imageExtensions.Any(ext => filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                if (fileEntry.Length < 12) return true;

                using Stream fs = fileEntry.Open();

                Span<byte> buffer = stackalloc byte[12];
                fs.ReadExactly(buffer);

                uint hexValue = BinaryPrimitives.ReadUInt32BigEndian(buffer[..4]);

                bool isJpeg = (hexValue & 0xFFFFFF00) == 0xFFD8FF00;
                bool isPng = hexValue == 0x89504E47;
                bool isGif = hexValue == 0x47494638;

                bool isWebp = false;
                if (hexValue == 0x52494646)
                {
                    uint webpHeader = BinaryPrimitives.ReadUInt32BigEndian(buffer[8..]);
                    isWebp = webpHeader == 0x57454250;
                }

                if (isJpeg || isPng || isGif || isWebp)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsFixedLayoutEpub(Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument xmlDoc,
            string epubFilename)
        {
            if (IsOpenMangaFormat(xmlDoc)) return true;

            XElement? xmlMetadata = xmlDoc.Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "metadata");

            if (xmlMetadata is null) return false;

            if (IsEpub2(xmlDoc))
            {
                return CheckEpub2FixedLayout(xmlMetadata, entryMap, epubFilename);
            }
            else
            {
                return CheckEpub3FixedLayout(xmlMetadata, epubFilename);
            }
        }

        private static bool CheckEpub3FixedLayout(XElement xmlMetadata,
            string? epubFilename)
        {
            foreach (var meta in xmlMetadata.Elements().Where(e => e.Name.LocalName == "meta"))
            {
                string? name = (string?)meta.Attribute("name");
                string? content = (string?)meta.Attribute("content");
                string? property = (string?)meta.Attribute("property");
                string innerText = meta.Value.Trim();

                if (name == "book-type" && content == "comic") return true;
                if (name == "fixed-layout" && content == "true") return true;
                if (property == "rendition:layout")
                {
                    if (innerText == "pre-paginated" || innerText == "fixed") return true;
                }
            }
#if DEBUG
            UserInterface.AppendColoredText($"No fixed layout info found in EPUB 3 - {epubFilename}" + Environment.NewLine, Color.DeepPink);
#endif
            return false;
        }

        private static bool CheckEpub2FixedLayout(XElement xmlMetadata,
            Dictionary<string, ZipArchiveEntry> entryMap,
            string? epubFilename)
        {
            foreach (var meta in xmlMetadata.Elements().Where(e => e.Name.LocalName == "meta"))
            {
                string? name = (string?)meta.Attribute("name");
                string? content = (string?)meta.Attribute("content");

                if (name == "BNContentKind" && content == "ComicsDRP") return true;
            }

            string[] targetPaths = [
                "META-INF/com.kobobooks.display-options.xml",
                "META-INF/com.apple.ibooks.display-options.xml"
            ];

            bool foundFixedLayout = false;
            foreach (string path in targetPaths)
            {
                if (entryMap.TryGetValue(path, out var entry))
                {
                    using var stream = entry.Open();
                    try
                    {
                        XDocument sidecarDoc = XDocument.Load(stream);

                        foundFixedLayout = sidecarDoc.Descendants()
                            .Any(el => el.Name.LocalName == "option"
                                       && (string?)el.Attribute("name") == "fixed-layout"
                                       && el.Value.Trim() == "true");
                    }
                    catch (XmlException)
                    {

                    }

                    if (foundFixedLayout) break;
                }
            }
#if DEBUG
            if (!foundFixedLayout) UserInterface.AppendColoredText($"No fixed layout info found in EPUB 2 - {epubFilename}" + Environment.NewLine, Color.DeepPink);
#endif
            return foundFixedLayout;
        }

        private static bool IsEpub2(XDocument xmlDoc)
        {
            var root = xmlDoc.Root;

            if (root is null || root.Name.LocalName != "package") return false;

            string? version = (string?)root.Attribute("version");
            return version == "2.0";
        }

        private static bool IsOpenMangaFormat(XDocument xmlDoc)
        {
            var root = xmlDoc.Root;

            if (root is null || root.Name.LocalName != "package") return false;

            string? prefix = (string?)root.Attribute("prefix");
            return prefix?.Contains("openmangaformat.org", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
