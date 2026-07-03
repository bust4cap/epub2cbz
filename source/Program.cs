using epub2cbz.Properties;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace epub2cbz
{
    public static class VersionDate
    {
        public static string GetVersionDateYear { get; } = "2026";
        public static string GetVersionDateMonth { get; } = "07";
        public static string GetVersionDateDay { get; } = "03";
        public static int GetVersionNumber { get; } = 1;
    }

    static class Program
    {
        private static MainForm? _mainForm;

        public static int numberEpubs = 0;
        public static int numberCurrentEpub = 0;
        public static CancellationTokenSource cts = new();

        public static readonly HashSet<string> imageExtensions = [".jpeg", ".jpg", ".png", ".webp", ".gif"];
        public static readonly ConcurrentDictionary<string, bool> _processedCbzFiles = new(StringComparer.InvariantCultureIgnoreCase);
        public static readonly char[] invalidPathFileChars = [.. Path.GetInvalidPathChars(), .. Path.GetInvalidFileNameChars()];

        private static string _exportFileExtension = ".cbz";

        public static CompressionLevel GetCompressionLevel()
        {
            int level = PopupSettings.CheckboxStates.DropDownCompressionLevelState;
            if (level == 0) return CompressionLevel.NoCompression;
            else if (level == 1) return CompressionLevel.Fastest;
            else if (level == 2) return CompressionLevel.Optimal;
            else return CompressionLevel.SmallestSize;
        }

        private static void ProcessEpub(string epubFile,
            CancellationToken token)
        {
            int currentProgress = 0;

            string rootDir = string.Empty;
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                rootDir = MainForm.FolderNameClass.InputFolderName!;
            }
            else if (!string.IsNullOrEmpty(MainForm.FolderNameClass.OutputFolderName))
            {
                rootDir = MainForm.FolderNameClass.OutputFolderName;
            }
            else
            {
                rootDir = Path.GetDirectoryName(epubFile)!;
            }

            string epubFilename = Path.GetFileNameWithoutExtension(epubFile);
            string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);


            /// Check if file is actually an EPUB
            /// 

            if (!EpubDetection.CheckEPUB(epubFile))
            {
                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.NotAnEPUB, Path.GetFileName(epubFile)) + Environment.NewLine, Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                return;
            }

            ///
            /// 

            using ZipArchive epub = ZipFile.OpenRead(epubFile);
            Dictionary<string, ZipArchiveEntry> entryMap = epub.Entries
                .ToDictionary(e => e.FullName, e => e, StringComparer.InvariantCultureIgnoreCase);

            string opfPath = string.Empty;
            try
            {
                opfPath = EpubParsing.GetOpfFile(entryMap);
            }
            catch (Exception ex)
            {
                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(ex.Message, Path.GetFileName(epubFile)) + Environment.NewLine, Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                entryMap.Clear();
                return;
            }

            if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                if (File.Exists(targetCbz)
                    || !_processedCbzFiles.TryAdd(targetCbz, true))
                {
                    currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                    UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                        + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, Color.Red);
                    UserInterface.ProgressBarStep(currentProgress);

                    entryMap.Clear();
                    return;
                }

                string simpleTempTarget = Path.Combine(rootDir, Guid.NewGuid().ToString() + ".tmp");
                try
                {
                    SimpleExtraction.Extract(entryMap, simpleTempTarget);

                    if (File.Exists(simpleTempTarget))
                    {
                        File.Move(simpleTempTarget, targetCbz, overwrite: true);
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(simpleTempTarget)) File.Delete(simpleTempTarget);

                    currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                    UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                        + ex.Message + Environment.NewLine, Color.Red);
                    UserInterface.ProgressBarStep(currentProgress);

                    entryMap.Clear();
                    return;
                }

                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.Processed, epubFilename) + Environment.NewLine, Color.Green);
                UserInterface.ProgressBarStep(currentProgress);

                entryMap.Clear();
                return;
            }

            XDocument opfDoc = EpubParsing.GetOpfDocument(entryMap, opfPath);

#if DEBUG
            if (!EpubDetection.IsFixedLayoutEpub(entryMap, opfDoc, epubFilename))
            {
                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.NotAManga, epubFilename) + Environment.NewLine, Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                entryMap.Clear();
                return;
            }
#endif

            bool barnesAndNobleBook = BarnesAndNoble.IsBarnesAndNobleBook(opfDoc);
            XDocument replicaMapDoc = new();

            List<BookInfo.EpubPagesIdsSpread> pages = [];
            if (barnesAndNobleBook)
            {
                (replicaMapDoc, string replicaMapPath) = BarnesAndNoble.GetReplicaMap(entryMap, opfDoc, opfPath);
                pages = BarnesAndNoble.ParseReplicaMapPages(replicaMapDoc, replicaMapPath);
            }
            else
            {
                pages = EpubParsing.ParseSpineXml(opfDoc, opfPath);
            }

            /// Try to check if Epub is still DRM protected
            /// 

            if (PopupSettings.CheckboxStates.CheckboxDRMProtectionState && EpubDetection.CheckDRMProtection(entryMap, pages[0].Pages.Split('#')[0]))
            {
                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.IsDRMProtected, Path.GetFileName(epubFile)) + Environment.NewLine, Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                entryMap.Clear();
                return;
            }

            ///
            ///

            (Dictionary<string, string?> metadata, string readingDirection) = MetadataParsing.ParseMetadataXml(opfDoc);

            if (PopupSettings.CheckboxStates.CheckboxMetadataTitleState
                && metadata.TryGetValue("Title", out string? titleValue)
                && !string.IsNullOrEmpty(titleValue))
            {
                foreach (char c in invalidPathFileChars)
                {
                    titleValue = titleValue.Replace(c, '_');
                }
                while (titleValue.Contains("__"))
                {
                    titleValue = titleValue.Replace("__", "_");
                }
                //
                targetCbz = Path.Combine(rootDir, titleValue + _exportFileExtension);
            }

            if (File.Exists(targetCbz))
            {
                currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                    + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, Color.Red);
                UserInterface.ProgressBarStep(currentProgress);

                entryMap.Clear();
                return;
            }

            List<BookInfo.EpubPage> bookFull = [];
            if (barnesAndNobleBook)
            {
                bookFull = BarnesAndNoble.ParseReplicaMapPagesXml(entryMap, pages);

                if (PopupSettings.CheckboxStates.CheckboxAddAlternativeCoverState)
                {
                    BarnesAndNoble.ParseCover(entryMap, epubFile, opfDoc, bookFull, opfPath);
                }
            }
            else
            {
                bookFull = EpubParsing.ParseOpfPagesXml(entryMap, epubFile, opfPath, opfDoc, pages);

                if (PopupSettings.CheckboxStates.CheckboxAddAlternativeCoverState)
                {
                    EpubParsing.ParseAlternativeCover(entryMap, epubFile, opfDoc, bookFull, opfPath);
                }
            }

            List<BookInfo.EpubChapter> chapters = [];
            if (barnesAndNobleBook)
            {
                chapters = BarnesAndNoble.ParseToc(replicaMapDoc);
            }
            else
            {
                chapters = EpubParsing.ParseEpubToc(entryMap, epubFile, opfDoc, opfPath);
                chapters = EpubParsing.ParseAlternativeToc(entryMap, opfDoc, chapters, bookFull, opfPath);
            }

            if (chapters.Count >= (bookFull.Count - 1) && PopupSettings.CheckboxStates.CheckboxEveryPageIsChapterState) // if all pages are chapters (minus the Cover)
            {
                chapters = [];
            }

            bool removedDuplicateCover = false;
            if (PopupSettings.CheckboxStates.CheckboxDuplicateCoverState)
            {
                removedDuplicateCover = PageAlignment.CheckDuplicateCover(chapters, bookFull, entryMap, opfDoc, epubFilename, epubFile);
            }

#if DEBUG
            if (!barnesAndNobleBook)
            {
                PageAlignment.CheckPageSpread(epubFilename, bookFull);
            }
#endif

            if (PopupSettings.CheckboxStates.CheckboxPageSpreadState)
            {
                PageAlignment.FixPageAlignmentPost(bookFull, readingDirection);

                if (removedDuplicateCover && PopupSettings.CheckboxStates.CheckboxBlankImageState)
                {
                    PageAlignment.BlankPageBehavior(bookFull, entryMap, epubFilename);
                }
            }

            if (PopupSettings.CheckboxStates.CheckboxInsertAdditionalBlankImageState)
            {
                bookFull.Insert(1, new()
                {
                    Page = "blank",
                    Blank = true
                });
            }

            if (PopupSettings.CheckboxStates.CheckboxRemoveFirstPageState)
            {
                if (string.IsNullOrEmpty(bookFull[1].Image)
                    || bookFull[1].Blank
                    || ImageProcessing.IsImageBlankWhite(entryMap, bookFull[1].Image))
                {
                    bookFull.RemoveAt(1);
                }
            }

            PageAlignment.IntegrateChapters(bookFull, chapters);

            ///
            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
            ///

            string tempTarget = Path.Combine(rootDir, Guid.NewGuid().ToString() + ".tmp");

            if (MainForm.FormElements.CheckboxExtractImagesState)
            {
                try
                {
                    Extraction.ExtractImageStreams(epubFilename, entryMap, targetCbz, tempTarget, bookFull, readingDirection);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Fail")
                    {
                        entryMap.Clear();
                        return;
                    }

                    if (File.Exists(tempTarget)) File.Delete(tempTarget);
                }

                ///
                if (token.IsCancellationRequested)
                {
                    if (File.Exists(tempTarget)) File.Delete(tempTarget);

                    token.ThrowIfCancellationRequested();
                }
                ///
            }

            if (MainForm.FormElements.CheckboxComicInfoState)
            {
                if (!MainForm.FormElements.CheckboxExtractImagesState
                    && (File.Exists(targetCbz)
                    || !_processedCbzFiles.TryAdd(targetCbz, true)))
                {
                    currentProgress = Interlocked.Increment(ref numberCurrentEpub);
                    UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                        + string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, Color.Red);
                    UserInterface.ProgressBarStep(currentProgress);

                    entryMap.Clear();
                    return;
                }

                ComicInfo.WriteComicInfoXml(tempTarget, epubFilename, readingDirection, bookFull, metadata);
            }

            if (File.Exists(tempTarget))
            {
                File.Move(tempTarget, targetCbz, overwrite: true);
            }

            currentProgress = Interlocked.Increment(ref numberCurrentEpub);
            UserInterface.AppendColoredText($"({currentProgress.ToString().PadLeft(numberEpubs.ToString().Length, '0')}/{numberEpubs}) - "
                + string.Format(Resources.Processed, epubFilename) + Environment.NewLine, Color.Green);
            UserInterface.ProgressBarStep(currentProgress);

            bookFull.Clear();
            entryMap.Clear();
            return;
        }

        private static void HandleCompletion(TimeSpan ts,
            bool wasAborted)
        {
            UserInterface.AppendColoredText(Environment.NewLine + string.Format(Resources.Timer, Math.Floor(ts.TotalMinutes), ts.Seconds, ts.Milliseconds) + Environment.NewLine, Color.White);

            if (wasAborted)
            {
                UserInterface.AppendColoredText(Environment.NewLine + Resources.AbortedMessage + Environment.NewLine, Color.Red);
            }

            UserInterface.EnableControls();

            _processedCbzFiles.Clear();

            cts.Dispose();
            cts = new();

            if (PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                _mainForm?.Invoke(_mainForm.fileListForm.fileListDataTable.Clear);
                MainForm.FileNameClass.FileNames.Clear();
            }
        }

        public static async Task ProgramStart()
        {
            string inputFolderName = MainForm.FolderNameClass.InputFolderName ?? string.Empty;
            string outputFolderName = MainForm.FolderNameClass.OutputFolderName ?? string.Empty;

            if (!MainForm.FormElements.CheckboxExtractImagesState
                && !MainForm.FormElements.CheckboxComicInfoState
                && !PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                var customOkButton = new TaskDialogButton(Resources.OkButtonText);

                var page = new TaskDialogPage()
                {
                    Caption = Resources.ErrorMessageBox,
                    Heading = Resources.ErrorMessageBox,
                    Text = Resources.NoCheckBoxChecked,
                    Icon = TaskDialogIcon.Error,
                    Buttons = { customOkButton },
                    AllowCancel = true
                };
                TaskDialog.ShowDialog(_mainForm!, page, TaskDialogStartupLocation.CenterOwner);

                return;
            }

            Stopwatch stopwatch = new();
            stopwatch.Start();

            string rootDir = string.Empty;
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                rootDir = inputFolderName ?? string.Empty;
            }
            else if (MainForm.FileNameClass.FileNames.Count < 1)
            {
                var customOkButton = new TaskDialogButton(Resources.OkButtonText);

                var page = new TaskDialogPage()
                {
                    Caption = Resources.ErrorMessageBox,
                    Heading = Resources.ErrorMessageBox,
                    Text = Resources.NoEpubsFoundMessageBox,
                    Icon = TaskDialogIcon.Error,
                    Buttons = { customOkButton },
                    AllowCancel = true
                };
                TaskDialog.ShowDialog(_mainForm!, page, TaskDialogStartupLocation.CenterOwner);

                UserInterface.EnableControls();
                stopwatch.Stop();
                return;
            }

            _mainForm?.Invoke(() =>
            {
                _mainForm.outputBoxConsole.Text = string.Empty;
                _mainForm.toolStripProgressBar.Value = 0;
            });

            UserInterface.DisableControls();
            numberCurrentEpub = 0;
            bool wasAborted = false;

            UserInterface.AppendColoredText(Resources.ProcessingInProgress + Environment.NewLine, Color.White);
            UserInterface.AppendColoredText(Environment.NewLine, Color.White);

            if (string.IsNullOrEmpty(rootDir) && !PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                var customOkButton = new TaskDialogButton(Resources.OkButtonText);

                var page = new TaskDialogPage()
                {
                    Caption = Resources.ErrorMessageBox,
                    Heading = Resources.ErrorMessageBox,
                    Text = Resources.NoPathMessageBox,
                    Icon = TaskDialogIcon.Error,
                    Buttons = { customOkButton },
                    AllowCancel = true
                };
                TaskDialog.ShowDialog(_mainForm!, page, TaskDialogStartupLocation.CenterOwner);

                UserInterface.EnableControls();
                stopwatch.Stop();
                return;
            }

            List<string> epubPaths = [];

            if (PopupSettings.CheckboxStates.RadioButtonZipState
                && !PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
            {
                _exportFileExtension = ".zip";
            }
            else
            {
                _exportFileExtension = ".cbz";
            }

            try
            {
                epubPaths = await Task.Run(() =>
                {
                    List<string> foundEpubPaths = [];

                    if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
                    {
                        foreach (var epubPath in Directory.EnumerateFileSystemEntries(rootDir, "*.epub", SearchOption.AllDirectories))
                        {
                            if (cts.IsCancellationRequested)
                            {
                                throw new OperationCanceledException(cts.Token);
                            }

                            if (File.Exists(epubPath))
                            {
                                if (!PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                                {
                                    string epubFilename = Path.GetFileNameWithoutExtension(epubPath);
                                    string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);

                                    if (File.Exists(targetCbz))
                                    {
                                        UserInterface.AppendColoredText(string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, Color.Red);
                                        continue;
                                    }
                                }

                                foundEpubPaths.Add(epubPath);
                            }
                        }
                    }
                    else
                    {
                        foreach (var epubPath in MainForm.FileNameClass.FileNames)
                        {
                            if (cts.IsCancellationRequested)
                            {
                                throw new OperationCanceledException(cts.Token);
                            }

                            if (File.Exists(epubPath))
                            {
                                if (!PopupSettings.CheckboxStates.CheckboxMetadataTitleState)
                                {
                                    if (string.IsNullOrEmpty(outputFolderName))
                                    {
                                        rootDir = Path.GetDirectoryName(epubPath)!;
                                    }
                                    else
                                    {
                                        rootDir = outputFolderName;
                                    }

                                    string epubFilename = Path.GetFileNameWithoutExtension(epubPath);
                                    string targetCbz = Path.Combine(rootDir, epubFilename + _exportFileExtension);

                                    if (File.Exists(targetCbz))
                                    {
                                        UserInterface.AppendColoredText(string.Format(Resources.CbzAlreadyExists, Path.GetFileName(targetCbz)) + Environment.NewLine, Color.Red);
                                        continue;
                                    }
                                }

                                foundEpubPaths.Add(epubPath);
                            }
                        }
                    }
                    return foundEpubPaths;
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                wasAborted = true;
                stopwatch.Stop();
                HandleCompletion(stopwatch.Elapsed, wasAborted);
                return;
            }
            catch (Exception ex)
            {
                var customOkButton = new TaskDialogButton(Resources.OkButtonText);

                var page = new TaskDialogPage()
                {
                    Caption = Resources.ErrorMessageBox,
                    Heading = Resources.ErrorMessageBox,
                    Text = $"{ex.Message}",
                    Icon = TaskDialogIcon.Error,
                    Buttons = { customOkButton },
                    AllowCancel = true
                };
                TaskDialog.ShowDialog(_mainForm!, page, TaskDialogStartupLocation.CenterOwner);

                UserInterface.EnableControls();
                stopwatch.Stop();
                return;
            }

            if (epubPaths.Count <= 0)
            {
                var customOkButton = new TaskDialogButton(Resources.OkButtonText);

                var page = new TaskDialogPage()
                {
                    Caption = Resources.ErrorMessageBox,
                    Heading = Resources.ErrorMessageBox,
                    Text = Resources.NoEpubsFoundMessageBox,
                    Icon = TaskDialogIcon.Error,
                    Buttons = { customOkButton },
                    AllowCancel = true
                };
                TaskDialog.ShowDialog(_mainForm!, page, TaskDialogStartupLocation.CenterOwner);

                UserInterface.EnableControls();
                stopwatch.Stop();
                return;
            }


            numberEpubs = epubPaths.Count;

            _mainForm?.Invoke(() => _mainForm.toolStripProgressBar.Maximum = numberEpubs);

            using PopupSettings popup = new();
            int? nullableDegree = popup.dropDownThreads.Items[PopupSettings.CheckboxStates.DropDownParallelismDegreeState] as int?;

            if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState) nullableDegree = Environment.ProcessorCount - 1;

            int maxDegreeOfParallelism = nullableDegree ?? (Environment.ProcessorCount - 1);
            maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);

            try
            {
                await Parallel.ForEachAsync(epubPaths,
                    new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism, CancellationToken = cts.Token },
                    async (epubPath, token) =>
                    {
                        ProcessEpub(epubPath, cts.Token);
                    });
            }
            catch (OperationCanceledException)
            {
                wasAborted = true;
            }
            finally
            {
                stopwatch.Stop();
                HandleCompletion(stopwatch.Elapsed, wasAborted);
            }
        }

        public static SystemColorMode GetEffectiveColorMode()
        {
            if (Application.ColorMode == SystemColorMode.System)
            {
                return Application.SystemColorMode;
            }

            return Application.ColorMode;
        }

        private static void HandleArguments(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case "--simple" or "-s":
                        PopupSettings.CheckboxStates.CheckboxSimpleExtractionState = true;
                        break;
                    case "--light" or "-l":
                        Application.SetColorMode(SystemColorMode.Classic);
                        break;
                    case "--dark" or "-d":
                        Application.SetColorMode(SystemColorMode.Dark);
                        break;
                }
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            Application.SetColorMode(SystemColorMode.System);
            HandleArguments(args);

            _mainForm = new MainForm();
            UserInterface.Initialize(_mainForm);

            Application.Run(_mainForm);
        }
    }
}