using System.IO.Compression;
using System.Net;
using System.Xml.Linq;

namespace epub2cbz
{
    internal class PageAlignment
    {
        public static void IntegrateChapters(List<BookInfo.EpubPage> bookFull,
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

        public static bool CheckDuplicateCover(List<BookInfo.EpubChapter> chapters,
            List<BookInfo.EpubPage> bookFull,
            Dictionary<string, ZipArchiveEntry> entryMap,
            XDocument opfDoc,
            string epubFilename,
            string epubFile)
        {
            if (chapters.Count > 0
                && chapters[0].Page == Path.GetFileName(bookFull[1].Page)
                && (chapters[0].Title.Contains("Cover")
                || chapters[0].Title == "カバー"
                || chapters[0].Title == "表紙"))
            {
                RemoveDuplicateCover(bookFull, epubFilename);
                return true;
            }
            else if (Path.GetFileName(bookFull[0].Image) == Path.GetFileName(bookFull[1].Image))
            {
#if DEBUG
                UserInterface.AppendColoredText($"DEBUG: '{epubFilename}' - Image 0 == Image 1" + Environment.NewLine, Color.HotPink);
#endif
                RemoveDuplicateCover(bookFull, epubFilename);
                return true;
            }
            else if (ImageProcessing.CompareImages(entryMap, bookFull[0].Image, bookFull[1].Image, epubFile))
            {
                RemoveDuplicateCover(bookFull, epubFilename);
                return true;
            }
            else
            {
                XNamespace opf = "http://www.idpf.org/2007/opf";

                var item = opfDoc.Descendants(opf + "guide").Descendants(opf + "reference").FirstOrDefault(i => (string?)i.Attribute("type") == "cover");
                if (item is not null)
                {
                    string? coverPath = (string?)item.Attribute("href");

                    if (!string.IsNullOrEmpty(coverPath)
                        && Path.GetFileName(coverPath.Split('#')[0]) == Path.GetFileName(bookFull[1].Page))
                    {
                        RemoveDuplicateCover(bookFull, epubFilename);
                        return true;
                    }
                }
            }

            return false;
        }

        private static void RemoveDuplicateCover(List<BookInfo.EpubPage> bookFull,
            string epubFilename)
        {
            if (PopupSettings.CheckboxStates.CheckboxHigherResolutionCover
                && bookFull[0].Height > 0
                && bookFull[1].Height > 0)
            {
                if (bookFull[0].Height >= bookFull[1].Height)
                {
                    if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
                    {
                        bookFull[1] = bookFull[1] with
                        {
                            Blank = true
                        };
                    }
                    else bookFull.RemoveAt(1);
                }
                else
                {
                    bookFull.RemoveAt(0);
                    if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
                    {
                        bookFull.Insert(1, new()
                        {
                            Page = "blank",
                            Blank = true
                        });
                    }
                }
            }
            else
            {
                if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
                {
                    bookFull[1] = bookFull[1] with
                    {
                        Blank = true
                    };
                }
                else bookFull.RemoveAt(1);
            }

#if DEBUG
            UserInterface.AppendColoredText($"DEBUG: '{epubFilename}' - Removed Duplicate Cover" + Environment.NewLine, Color.DarkOrange);
#endif
        }

        public static void CheckPageSpread(string epubFilename,
            List<BookInfo.EpubPage> bookFull)
        {
            string centerFound = string.Empty;

            for (int i = 0; i < bookFull.Count; i++)
            {
                if (string.IsNullOrEmpty(bookFull[i].Spread))
                {
                    continue;
                }

                if (bookFull[i].Spread.Contains("left", StringComparison.OrdinalIgnoreCase)
                    || bookFull[i].Spread.Contains("right", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (bookFull[i].Spread.Contains("center", StringComparison.OrdinalIgnoreCase)
                    && string.IsNullOrEmpty(centerFound))
                {
                    centerFound = " & only Center found";
                }
            }
#if DEBUG
            UserInterface.AppendColoredText($"DEBUG: '{epubFilename}' - No Page Spread Information" + centerFound + Environment.NewLine, Color.DarkOrange);
#endif
        }

        public static void FixPageAlignmentPost(List<BookInfo.EpubPage> bookFull,
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

        public static void BlankPageBehavior(List<BookInfo.EpubPage> bookFull,
            Dictionary<string, ZipArchiveEntry> entryMap,
            string epubFilename)
        {
            bool isBlank = false;

            if (!string.IsNullOrEmpty(bookFull[2].Image)
                && !bookFull[2].Blank)
            {
                isBlank = ImageProcessing.IsImageBlankWhite(entryMap, bookFull[2].Image);
            }
            else if (bookFull[2].Image == string.Empty
                || bookFull[2].Blank)
            {
                isBlank = true;
            }


            // remove both blank images
            if (isBlank)
            {
                bookFull.RemoveAt(2);
                bookFull.RemoveAt(1);

#if DEBUG
                UserInterface.AppendColoredText($"DEBUG: '{epubFilename}' - Removed Double Blank" + Environment.NewLine, Color.DarkOrange);
#endif
            }
        }
    }
}
