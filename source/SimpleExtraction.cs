using System.IO.Compression;

namespace epub2cbz
{
    internal class SimpleExtraction
    {
        public static void Extract(Dictionary<string, ZipArchiveEntry> entryMap,
            string targetCbz)
        {
            Dictionary<string, ZipArchiveEntry> entryKeysNew = [];
            HashSet<string> imageExtensionsSimple = [".jpeg", ".jpg", ".png", ".webp", ".svg", ".gif"];
            foreach (KeyValuePair<string, ZipArchiveEntry> pair in entryMap)
            {
                if (imageExtensionsSimple.Any(suffix => pair.Key.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                {
                    entryKeysNew.Add(pair.Key, pair.Value);
                }
            }

            ExtractImageStreamsSimple(entryKeysNew, targetCbz);
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
        }
    }
}
