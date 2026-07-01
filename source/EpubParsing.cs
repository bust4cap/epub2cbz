using epub2cbz.Properties;
using ExCSS;
using SixLabors.ImageSharp;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace epub2cbz
{
    internal class EpubParsing
    {
        public static string ResolveRootPath(string rootFile, string relativeFile)
        {
            string decodedPath = WebUtility.UrlDecode(relativeFile);

            Uri baseUri = new(new Uri("dummy://root/"), rootFile);
            Uri resolvedUri = new(baseUri, decodedPath);

            return resolvedUri.AbsolutePath.TrimStart('/');
        }

        public static string GetOpfFile(Dictionary<string, ZipArchiveEntry> entryMap)
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

        public static XDocument GetOpfDocument(Dictionary<string, ZipArchiveEntry> entryMap,
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

        public static List<BookInfo.EpubPage> ParseOpfPagesXml(Dictionary<string, ZipArchiveEntry> entryMap,
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

                        (width, height) = ImageProcessing.GetImageDimensions(streamDimensions);

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

                            (width, height) = ImageProcessing.GetImageDimensions(streamDimensions);

                            bool isDoublePage = false;

                            ///
                            if (width >= (height * wideImageRatio)) isDoublePage = true;
                            ///

                            bookFull.Add(new()
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
                    bookFull.Add(new()
                    {
                        Page = dicPagesIdsSpread[i].Pages.Split('#')[0],
                        Spread = dicPagesIdsSpread[i].Spread ?? string.Empty,
                        Blank = true
                    });
                }
            }

            return bookFull;
        }

        public static List<BookInfo.EpubPagesIdsSpread> ParseSpineXml(XDocument opfDoc,
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

        public static List<BookInfo.EpubChapter> ParseEpubToc(Dictionary<string, ZipArchiveEntry> entryMap,
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
                UserInterface.AppendColoredText(string.Format(Resources.NcxOrNavNotInOpf, epubFile) + Environment.NewLine, System.Drawing.Color.Blue);
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

                    if (!entryMap.TryGetValue(altTocPath, out var altTocEntry))
                    {
                        i++;

                        if (bookFull.Count - 1 < number + i)
                        {
                            break;
                        }

                        continue;
                    }

                    using StreamReader reader = new(altTocEntry.Open(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
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
                    UserInterface.AppendColoredText(ex.Message + Environment.NewLine, System.Drawing.Color.Red);
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

        public static List<BookInfo.EpubChapter> ParseAlternativeToc(Dictionary<string, ZipArchiveEntry> entryMap,
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

        public static void ParseAlternativeCover(Dictionary<string, ZipArchiveEntry> entryMap,
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
                Program.imageExtensions.Any(ext => coverPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                string filename = ResolveRootPath(opfPath, coverPath);

                if (!entryMap.TryGetValue(filename, out var bookEntry)) { }

                if (!string.IsNullOrEmpty(filename)
                    && filename != bookFull[0].Image
                    && bookEntry is not null)
                {
#if DEBUG
                    UserInterface.AppendColoredText($"DEBUG: '{Path.GetFileNameWithoutExtension(epubFile)}' - Alternative Cover" + Environment.NewLine, System.Drawing.Color.DarkOrange);
#endif
                    using var streamDimensions = bookEntry.Open();

                    int width = 0;
                    int height = 0;

                    (width, height) = ImageProcessing.GetImageDimensions(streamDimensions);

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
                    UserInterface.AppendColoredText($"HTML DETECTED: '{epubFile}'" + Environment.NewLine, System.Drawing.Color.Purple);
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
            else if (Program.imageExtensions.Any(ext => actualFilename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
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
    }
}
