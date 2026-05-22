using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace epub2cbz
{
    internal class BarnesAndNoble
    {
        public static bool IsBarnesAndNobleBook(XDocument opfDoc)
        {
            return opfDoc.Descendants("meta").Any(a => (string?)a.Attribute("name") == "BNContentKind");
        }

        public static (XDocument, string) GetReplicaMap(Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            string opfPath)
        {
            XNamespace opf = "http://www.idpf.org/2007/opf";

            string opfReplicaMap = opfDoc.Descendants(opf + "manifest")
                .Descendants(opf + "item")
                .FirstOrDefault(i => (string?)i.Attribute("id") == "replicaMap")!
                .Attribute("href")!
                .Value;

            opfReplicaMap = Program.ResolveRootPath(opfPath, opfReplicaMap);

            ZipArchiveEntry fileEntry = entryMap.GetValueOrDefault(opfReplicaMap)!;
            using StreamReader reader = new(fileEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string opfContent = reader.ReadToEnd();

            XDocument replicaMapDoc = XDocument.Parse(opfContent);

            return (replicaMapDoc, opfReplicaMap);
        }

        public static List<BookInfo.EpubPagesIdsSpread> ParseReplicaMapPages(XDocument replicaMapDoc,
            string replicaMapPath)
        {
            Dictionary<string, string?> pages = [];
            List<BookInfo.EpubPagesIdsSpread> dicPagesIdsSpread = [];

            XNamespace xmlns = replicaMapDoc.Root!.Name.Namespace;

            var opfMetadata = replicaMapDoc.Descendants(xmlns + "Pages").Descendants(xmlns + "Page");
            if (opfMetadata != null)
            {
                foreach (XElement e in opfMetadata)
                {
                    pages.Add(e.Attribute("pageNum")!.Value, e.Attribute("file")?.Value);
                }
            }

            foreach (KeyValuePair<string, string?> page in pages)
            {
                string pageFile = string.Empty;
                if (!string.IsNullOrEmpty(page.Value))
                {
                    pageFile = Program.ResolveRootPath(replicaMapPath, page.Value);
                }

                dicPagesIdsSpread.Add(new()
                {
                    Pages = pageFile ?? string.Empty,
                    Ids = page.Key
                });
            }

            return dicPagesIdsSpread;
        }

        public static List<BookInfo.EpubPage> ParseReplicaMapPagesXml(Dictionary<string, ZipArchiveEntry> entryMap,
            List<BookInfo.EpubPagesIdsSpread> dicPagesIdsSpread)
        {
            List<BookInfo.EpubPage> bookFull = [];
            const double wideImageRatio = 1.125; // Images have to be at least 12.5% wider than tall to be considered "wide"

            for (int i = 0; i < dicPagesIdsSpread.Count; i++)
            {
                string? imagePath = dicPagesIdsSpread[i].Pages;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    if (!entryMap.TryGetValue(imagePath, out var bookEntry)) { }

                    if (!string.IsNullOrEmpty(imagePath)
                        && bookEntry != null)
                    {
                        // Handle wide images first
                        using var streamDimensions = bookEntry.Open();

                        int width = 0;
                        int height = 0;

                        (width, height) = Program.GetImageDimensions(streamDimensions);

                        bool isDoublePage = false;

                        ///
                        if (width >= (height * wideImageRatio)) isDoublePage = true;
                        ///

                        bookFull.Add(new()
                        {
                            Page = dicPagesIdsSpread[i].Ids,
                            Image = imagePath,
                            Doublepage = isDoublePage,
                            Height = height,
                            Width = width,
                            Size = bookEntry.Length
                        });
                        continue;
                    }
                }
            }

            return bookFull;
        }

        public static void ParseCover(Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFile,
            XDocument opfDoc,
            List<BookInfo.EpubPage> bookFull,
            string opfPath)
        {
            XNamespace opf = "http://www.idpf.org/2007/opf";

            string opfCover = opfDoc.Descendants(opf + "manifest")
                .Descendants(opf + "item")
                .FirstOrDefault(i => (string?)i.Attribute("id") == "cover")!
                .Attribute("href")!
                .Value;

            string filename = Program.ResolveRootPath(opfPath, opfCover);

            if (!entryMap.TryGetValue(filename, out var bookEntry)) { }

            if (!string.IsNullOrEmpty(filename)
                && bookEntry != null)
            {
#if DEBUG
                Program.AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Alternative Cover" + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif
                using var streamDimensions = bookEntry.Open();

                int width = 0;
                int height = 0;

                (width, height) = Program.GetImageDimensions(streamDimensions);

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

        public static List<BookInfo.EpubChapter> ParseToc(List<BookInfo.EpubPage> bookFull,
            XDocument replicaMapDoc)
        {
            List<BookInfo.EpubChapter> chapters = [];

            XNamespace xmlns = replicaMapDoc.Root!.Name.Namespace;

            var tocItems = replicaMapDoc.Descendants(xmlns + "TOC").Descendants(xmlns + "TocEntry")
                .Select(entry => new
                {
                    Title = (string?)entry.Attribute("title"),
                    PageNumber = (string?)entry.Element(xmlns + "Page")?.Attribute("pagenum")
                })
                .ToList();

            foreach (var page in tocItems)
            {
                chapters.Add(new()
                {
                    Title = page.Title!,
                    Page = page.PageNumber!
                });
            }

            return chapters;
        }
    }
}
