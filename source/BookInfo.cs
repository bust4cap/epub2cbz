namespace epub2cbz
{
    internal class BookInfo
    {
        public record EpubPage(
            string Page = "",
            string Image = "",
            string Spread = "",
            bool Doublepage = false,
            int Width = 0,
            int Height = 0,
            long Size = 0,
            string Bookmark = "",
            bool Blank = false
        );

        public record EpubChapter(
            string Title = "",
            string Page = ""
        );

        public record EpubPagesIdsSpread(
            string Pages = "",
            string Ids = "",
            string Spread = ""
        );
    }
}
