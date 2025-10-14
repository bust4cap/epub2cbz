using epub2cbz_gui.Properties;
using System.Globalization;
using System.Resources;

namespace epub2cbz_gui;

public partial class MainForm : Form
{
    public readonly FileModeListForm fileListForm;

    private readonly string strFileMode = "File Mode";
    private readonly string strFolderMode = "Folder Mode";

    public static class FormElements
    {
        public static bool CheckboxMangaListState { get; set; } = false;
        public static bool CheckboxComicInfoState { get; set; } = true;
        public static bool CheckboxExtractImagesState { get; set; } = true;

        public static bool DarkModeState { get; set; } = false;
    }

    public static class FolderNameClass
    {
        public static string? InputFolderName { get; set; } = string.Empty;
        public static string? OutputFolderName { get; set; } = string.Empty;
    }

    public static class FileNameClass
    {
        public static HashSet<string> FileNames { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);
    }

    private readonly Dictionary<int, string> languageCodeMap = new()
    {
        { 0, "en-US" },
        { 1, "de-DE" },
        { 2, "ja-JP" },
        { 3, "zh-Hant" },
        { 4, "zh-Hans" }
    };

    private void HandleAddingEpubs(string[]? files)
    {
        if (files!.Length > 0)
        {
            outputBoxConsole.Clear();
            outputBoxConsole.Focus();
            string stringFilesAdded = string.Empty;
            int filesAdded = 0;

            foreach (string file in files)
            {
                if (File.Exists(file)
                    && Path.GetExtension(file).Equals(".epub", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (FileNameClass.FileNames.Add(file))
                    {
                        fileListForm.AddItemsToList(file);

                        stringFilesAdded += Path.GetFileName(file) + Environment.NewLine;
                        filesAdded++;
                    }
                }
                else if (Directory.Exists(file))
                {
                    foreach (var epubPath in Directory.EnumerateFileSystemEntries(file, "*.epub", SearchOption.TopDirectoryOnly))
                    {
                        if (File.Exists(epubPath)
                            && FileNameClass.FileNames.Add(epubPath))
                        {
                            fileListForm.AddItemsToList(epubPath);

                            stringFilesAdded += Path.GetFileName(epubPath) + Environment.NewLine;
                            filesAdded++;
                        }
                    }
                }
            }

            int fileCount = FileNameClass.FileNames.Count;

            if (filesAdded > 0)
            {
                Program.AppendColoredText(Resources.FileModeAddedFilename, Color.White);
                Program.AppendColoredText(Environment.NewLine, Color.White);
                Program.AppendColoredText(stringFilesAdded, Color.White);

                Program.AppendColoredText(Environment.NewLine + string.Format(Resources.FileModeFilesAdded, filesAdded) + Environment.NewLine, Color.Green);
                Program.AppendColoredText(string.Format(Resources.FileModeTotalFiles, fileCount) + Environment.NewLine, Color.Green);

                outputBoxConsole.Focus();
            }
            else
            {
                Program.AppendColoredText(Resources.FileModeNoFilesAdded + Environment.NewLine, Color.Yellow);
                Program.AppendColoredText(Environment.NewLine, Color.White);
                Program.AppendColoredText(string.Format(Resources.FileModeTotalFiles, fileCount) + Environment.NewLine, Color.Green);

                outputBoxConsole.Focus();
            }
        }
    }

    private void LoadCustomWindowSettings()
    {
        checkBoxComicInfo.Checked = CustomSettings.SettingStates.CheckboxComicInfoState;
        checkBoxImages.Checked = CustomSettings.SettingStates.CheckboxExtractImagesState;

        string valueInputFolderName = CustomSettings.SettingStates.InputFolderName;

        if (!string.IsNullOrEmpty(valueInputFolderName)
            && Directory.Exists(valueInputFolderName))
        {
            FolderNameClass.InputFolderName = valueInputFolderName;

            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                textBoxPath.Text = FolderNameClass.InputFolderName;
            }
        }

        string valueOutputFolderName = CustomSettings.SettingStates.OutputFolderName;

        if (!string.IsNullOrEmpty(valueOutputFolderName)
            && Directory.Exists(valueOutputFolderName))
        {
            FolderNameClass.OutputFolderName = valueOutputFolderName;

            if (PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                textBoxPath.Text = FolderNameClass.OutputFolderName;
            }
        }


        string windowLocation = CustomSettings.SettingStates.WindowLocation;
        string[] coordinates = windowLocation.Split(',', StringSplitOptions.TrimEntries);

        // Check if settings exist
        if (coordinates.Length == 2 && int.TryParse(coordinates[0], out int x) && int.TryParse(coordinates[1], out int y) && (x != 0 || y != 0))
        {
            Point savedLocation = new(x, y);
            Rectangle workingArea = Screen.PrimaryScreen!.WorkingArea; // Default to primary screen

            // Check if the saved location is within any of the current screens
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(savedLocation))
                {
                    workingArea = screen.WorkingArea;
                    break;
                }
            }

            this.StartPosition = FormStartPosition.Manual;

            // Adjust location if it's outside the working area
            if (savedLocation.X < workingArea.Left)
            {
                savedLocation.X = workingArea.Left;
            }
            if (savedLocation.Y < workingArea.Top)
            {
                savedLocation.Y = workingArea.Top;
            }
            if (savedLocation.X > workingArea.Right - this.Width)
            {
                savedLocation.X = workingArea.Right - this.Width;
            }
            if (savedLocation.Y > workingArea.Bottom - this.Height)
            {
                savedLocation.Y = workingArea.Bottom - this.Height;
            }

            this.Location = savedLocation;
        }
        else
        {
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
        }
    }

    private void SetDarkModeIcons()
    {
        FormElements.DarkModeState = true;
        Color dark = Color.FromArgb(255, 32, 32, 32);

        buttonOpenSettings.BackgroundImage = Resources.cogwheel_light;
        buttonSwitchModes.BackgroundImage = Resources.arrows_light;

        textBoxPath.BackColor = dark;
        statusStripMain.BackColor = dark;
    }

    public MainForm()
    {
        InitializeComponent();

#pragma warning disable WFO5001 // needs to be here until dotnet 10
        if (Application.ColorMode == SystemColorMode.Dark)
        {
            SetDarkModeIcons();
        }
#pragma warning restore WFO5001

        fileListForm = new();
    }

    public void ButtonPath_Click(object sender, EventArgs e)
    {
        DialogResult result = folderBrowserDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
            {
                FolderNameClass.InputFolderName = folderBrowserDialog.SelectedPath;

                textBoxPath.Text = FolderNameClass.InputFolderName;
            }
            else
            {
                FolderNameClass.OutputFolderName = folderBrowserDialog.SelectedPath;

                textBoxPath.Text = FolderNameClass.OutputFolderName;
            }
        }
    }

    private async void ButtonStart_Click(object sender, EventArgs e)
    {
        if (buttonStart.Text == Resources.StartButtonText)
        {
            outputBoxConsole.Focus();
            await Program.ProgramStart();
            outputBoxConsole.Focus();
        }
        else
        {
            buttonStart.Enabled = false;
            Program.cts.Cancel();
            Program.AppendColoredText(Environment.NewLine + Resources.AbortingMessage + Environment.NewLine + Environment.NewLine, Color.Red);
        }
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        ///
        CustomSettings.LoadSettings();
        LoadCustomWindowSettings();
        ///

        Text = $"epub2cbz - Build: v{VersionDate.GetVersionDateYear}." +
            $"{VersionDate.GetVersionDateMonth}." +
            $"{VersionDate.GetVersionDateDay}-" +
            $"{VersionDate.GetVersionNumber}";
#if DEBUG
        Text += " | DEBUG";
#endif
        if (PopupSettings.CheckboxStates.CheckboxSimpleExtractionState)
        {
            Text += " | Simple Mode";

            checkBoxComicInfo.Visible = false;
            checkBoxImages.Visible = false;
            buttonOpenSettings.Visible = false;
        }

        if (PopupSettings.CheckboxStates.CheckboxFileModeState)
        {
#if DEBUG
            checkBoxMangalist.Visible = false;
#endif
            toolStripStatusLabelCurrentMode.Text = strFileMode;

            buttonPath.BackgroundImage = Resources.output_folder;
            buttonFileModeFileList.Visible = true;

            outputBoxConsole.Clear();
            outputBoxConsole.Focus();
            Program.AppendColoredText(Resources.FileModeNoEpubs + Environment.NewLine, Color.Yellow);
        }
        else
        {
#if DEBUG
            checkBoxMangalist.Visible = true;
#endif
            toolStripStatusLabelCurrentMode.Text = strFolderMode;

            buttonPath.BackgroundImage = Resources.input_folder;
        }

        ComboBoxDropDownWidth();
        ToolStripProgressbarWidth();

        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;

        // Get the two-letter ISO language code
        string languageCode = currentUICulture.TwoLetterISOLanguageName;
        string languageCodeName = currentUICulture.Name;

        if (languageCode == "de") comboBoxLanguage.SelectedIndex = 1;
        else if (languageCode == "ja") comboBoxLanguage.SelectedIndex = 2;
        else if (languageCode == "zh")
        {
            if (languageCodeName.Contains("Hant", StringComparison.InvariantCultureIgnoreCase)) comboBoxLanguage.SelectedIndex = 3;
            else if (languageCodeName.Contains("Hans", StringComparison.InvariantCultureIgnoreCase)) comboBoxLanguage.SelectedIndex = 4;
            else comboBoxLanguage.SelectedIndex = 0;
        }
        else comboBoxLanguage.SelectedIndex = 0;

        UpdateLanguages();
    }

    private void BtnOpenSettings_Click(object sender, EventArgs e)
    {
        PopupSettings popup = new();
        DialogResult result = popup.ShowDialog();

        if (result == DialogResult.OK)
        {
            // If the reset button has been pressed, reset MainForm Settings only when pressing OK
            if (PopupSettings.CheckboxStates.ButtonResetSettingsState)
            {
#if DEBUG
                checkBoxMangalist.Checked = false;
#endif
                checkBoxComicInfo.Checked = true;
                checkBoxImages.Checked = true;

                FolderNameClass.InputFolderName = string.Empty;
                FolderNameClass.OutputFolderName = string.Empty;

                textBoxPath.Clear();
            }
        }
        PopupSettings.CheckboxStates.ButtonResetSettingsState = false;
    }

    private void CheckBoxComicInfo_CheckedChanged(object sender, EventArgs e)
    {
        FormElements.CheckboxComicInfoState = checkBoxComicInfo.Checked;
    }

    private void CheckBoxImages_CheckedChanged(object sender, EventArgs e)
    {
        FormElements.CheckboxExtractImagesState = checkBoxImages.Checked;
    }

    private void UpdateLanguages()
    {
#if DEBUG
        toolTip.SetToolTip(checkBoxMangalist, Resources.MangalistCheckboxTooltip);
#endif
        checkBoxComicInfo.Text = Resources.ComicInfoCheckboxText;
        checkBoxImages.Text = Resources.ImagesCheckboxText;

        toolTip.SetToolTip(comboBoxLanguage, comboBoxLanguage.SelectedItem!.ToString());
        toolTip.SetToolTip(buttonPathClear, Resources.PathClearButtonText);

        if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
        {
            toolTip.SetToolTip(buttonPath, Resources.PathButtonTextInput);
            textBoxPath.PlaceholderText = Resources.PathButtonTextInput;
        }
        else
        {
            toolTip.SetToolTip(buttonPath, Resources.PathButtonTextOutput);
            textBoxPath.PlaceholderText = Resources.PathButtonTextOutput;

            UpdateFileModeConsoleText();
        }

        buttonStart.Text = Resources.StartButtonText;

        buttonFileModeFileList.Text = Resources.FileList;
        toolTip.SetToolTip(buttonSwitchModes, Resources.ButtonSwitchFileFolderMode);

        toolTip.SetToolTip(buttonOpenSettings, Resources.SettingsWindowTitle);
    }

    private void ComboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        int selectedIndex = comboBoxLanguage.SelectedIndex;

        if (languageCodeMap.TryGetValue(selectedIndex, out string? cultureCode))
        {
            CultureInfo newCulture = new(cultureCode);
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
        }

        toolTip.SetToolTip(comboBoxLanguage, comboBoxLanguage.SelectedItem!.ToString());

        UpdateLanguages();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (this.WindowState == FormWindowState.Normal)
        {
            CustomSettings.SettingStates.WindowLocation = this.Location.X.ToString() + ", " + this.Location.Y.ToString();
        }

        CustomSettings.SaveSettings();
    }

    private void OutputBoxConsole_DragEnter(object sender, DragEventArgs e)
    {
        if (PopupSettings.CheckboxStates.CheckboxFileModeState
            && e.Data!.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void OutputBoxConsole_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop))
        {
            string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];

            HandleAddingEpubs(files);
        }
    }

    private void ButtonFileModeFileList_Click(object sender, EventArgs e)
    {
        fileListForm.ShowDialog();
    }

    private void ButtonPathClear_Click(object sender, EventArgs e)
    {
        if (!PopupSettings.CheckboxStates.CheckboxFileModeState)
        {
            FolderNameClass.InputFolderName = string.Empty;
        }
        else
        {
            FolderNameClass.OutputFolderName = string.Empty;
        }

        textBoxPath.Clear();
    }

    private void TextBoxPath_TextChanged(object sender, EventArgs e)
    {
        toolTip.SetToolTip(textBoxPath, textBoxPath.Text);
    }

    private void ButtonSwitchModes_Click(object sender, EventArgs e)
    {
        PopupSettings.CheckboxStates.CheckboxFileModeState = !PopupSettings.CheckboxStates.CheckboxFileModeState;

        if (PopupSettings.CheckboxStates.CheckboxFileModeState)
        {
            toolTip.SetToolTip(buttonPath, Resources.PathButtonTextOutput);
            textBoxPath.Text = FolderNameClass.OutputFolderName;
            textBoxPath.PlaceholderText = Resources.PathButtonTextOutput;
#if DEBUG
            checkBoxMangalist.Visible = false;
#endif
            toolStripStatusLabelCurrentMode.Text = strFileMode;

            buttonFileModeFileList.Visible = true;

            buttonPath.BackgroundImage?.Dispose();
            buttonPath.BackgroundImage = Resources.output_folder;

            outputBoxConsole.Clear();
            outputBoxConsole.Focus();
            Program.AppendColoredText(Resources.FileModeNoEpubs + Environment.NewLine, Color.Yellow);
        }
        else
        {
            toolTip.SetToolTip(buttonPath, Resources.PathButtonTextInput);
            textBoxPath.Text = FolderNameClass.InputFolderName;
            textBoxPath.PlaceholderText = Resources.PathButtonTextInput;
#if DEBUG
            checkBoxMangalist.Visible = true;
#endif
            toolStripStatusLabelCurrentMode.Text = strFolderMode;

            buttonPath.BackgroundImage?.Dispose();
            buttonPath.BackgroundImage = Resources.input_folder;
            buttonFileModeFileList.Visible = false;

            outputBoxConsole.Clear();
            outputBoxConsole.Focus();
        }
    }

    private void CheckBoxMangalist_CheckedChanged(object sender, EventArgs e)
    {
        FormElements.CheckboxMangaListState = checkBoxMangalist.Checked;
    }

    private void UpdateFileModeConsoleText()
    {
        ResourceManager rm = new("epub2cbz_gui.Properties.Resources", typeof(MainForm).Assembly);

        List<CultureInfo> culturesToCheck =
        [
            new CultureInfo("en-US"),
            new CultureInfo("de-DE"),
            new CultureInfo("ja-JP"),
            new CultureInfo("zh-Hant"),
            new CultureInfo("zh-Hans"),
        ];

        foreach (CultureInfo culture in culturesToCheck)
        {
            string localizedMessage = rm.GetString("FileModeNoEpubs", culture)!;

            if (outputBoxConsole.Text.Contains(localizedMessage))
            {
                outputBoxConsole.Clear();
                Program.AppendColoredText(Resources.FileModeNoEpubs + Environment.NewLine, Color.Yellow);
            }
        }
    }

    private void ComboBoxDropDownWidth()
    {
        int width = comboBoxLanguage.DropDownWidth;

        foreach (string item in comboBoxLanguage.Items)
        {
            Size largestTextSize = TextRenderer.MeasureText(item, comboBoxLanguage.Font);
            if (largestTextSize.Width > width) width = largestTextSize.Width;
        }

        comboBoxLanguage.DropDownWidth = width;
    }

    private void ToolStripProgressbarWidth()
    {
        Size theoreticalSizeFile = TextRenderer.MeasureText(strFileMode, toolStripStatusLabelCurrentMode.Font);
        int fileWidth = theoreticalSizeFile.Width;
        Size theoreticalSizeFolder = TextRenderer.MeasureText(strFolderMode, toolStripStatusLabelCurrentMode.Font);
        int folderWidth = theoreticalSizeFolder.Width;

        int maxLabelWidth = Math.Max(fileWidth, folderWidth);

        int statusStripWidth = statusStripMain.Width - statusStripMain.Padding.Horizontal;
        toolStripProgressBar.Width = statusStripWidth - maxLabelWidth - (toolStripProgressBar.Margin.Horizontal * 2);
    }
}
