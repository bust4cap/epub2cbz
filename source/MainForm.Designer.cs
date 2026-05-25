using epub2cbz.Properties;
using System.Windows.Forms;

namespace epub2cbz;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        folderBrowserDialog = new FolderBrowserDialog();
        buttonPath = new Button();
        toolTip = new ToolTip(components);
        buttonOpenSettings = new Button();
        buttonStart = new Button();
        checkBoxComicInfo = new CheckBox();
        checkBoxImages = new CheckBox();
        outputBoxConsole = new RichTextBox();
        comboBoxLanguage = new ComboBox();
        buttonFileModeFileList = new Button();
        textBoxPath = new TextBox();
        buttonPathClear = new Button();
        buttonSwitchModes = new Button();
        statusStripMain = new StatusStrip();
        toolStripStatusLabelCurrentMode = new ToolStripStatusLabel();
        toolStripProgressBar = new ToolStripProgressBar();
        statusStripMain.SuspendLayout();
        SuspendLayout();
        // 
        // buttonPath
        // 
        buttonPath.BackgroundImageLayout = ImageLayout.Center;
        buttonPath.FlatAppearance.BorderColor = Color.DimGray;
        buttonPath.FlatStyle = FlatStyle.Flat;
        buttonPath.Location = new Point(12, 38);
        buttonPath.Margin = new Padding(2, 1, 2, 1);
        buttonPath.Name = "buttonPath";
        buttonPath.Size = new Size(25, 25);
        buttonPath.TabIndex = 11;
        buttonPath.UseVisualStyleBackColor = true;
        buttonPath.Click += ButtonPath_Click;
        // 
        // buttonOpenSettings
        // 
        buttonOpenSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonOpenSettings.BackgroundImageLayout = ImageLayout.Center;
        buttonOpenSettings.FlatAppearance.BorderColor = Color.DimGray;
        buttonOpenSettings.FlatStyle = FlatStyle.Flat;
        buttonOpenSettings.Location = new Point(439, 10);
        buttonOpenSettings.Margin = new Padding(2, 1, 2, 1);
        buttonOpenSettings.Name = "buttonOpenSettings";
        buttonOpenSettings.Size = new Size(25, 25);
        buttonOpenSettings.TabIndex = 6;
        toolTip.SetToolTip(buttonOpenSettings, "Settings");
        buttonOpenSettings.UseVisualStyleBackColor = true;
        buttonOpenSettings.Click += BtnOpenSettings_Click;
        // 
        // buttonStart
        // 
        buttonStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonStart.FlatAppearance.BorderColor = Color.DimGray;
        buttonStart.FlatStyle = FlatStyle.Flat;
        buttonStart.Location = new Point(570, 10);
        buttonStart.Margin = new Padding(2, 1, 2, 1);
        buttonStart.Name = "buttonStart";
        buttonStart.Size = new Size(74, 25);
        buttonStart.TabIndex = 8;
        buttonStart.Text = Resources.StartButtonText;
        buttonStart.UseVisualStyleBackColor = true;
        buttonStart.Click += ButtonStart_Click;
        // 
        // checkBoxComicInfo
        // 
        checkBoxComicInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        checkBoxComicInfo.AutoSize = true;
        checkBoxComicInfo.Checked = true;
        checkBoxComicInfo.CheckState = CheckState.Checked;
        checkBoxComicInfo.Location = new Point(273, 12);
        checkBoxComicInfo.Margin = new Padding(2, 1, 2, 1);
        checkBoxComicInfo.Name = "checkBoxComicInfo";
        checkBoxComicInfo.Size = new Size(104, 19);
        checkBoxComicInfo.TabIndex = 5;
        checkBoxComicInfo.Text = "ComicInfo.xml";
        checkBoxComicInfo.UseVisualStyleBackColor = true;
        checkBoxComicInfo.CheckedChanged += CheckBoxComicInfo_CheckedChanged;
        // 
        // checkBoxImages
        // 
        checkBoxImages.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        checkBoxImages.AutoSize = true;
        checkBoxImages.Checked = true;
        checkBoxImages.CheckState = CheckState.Checked;
        checkBoxImages.Location = new Point(149, 12);
        checkBoxImages.Margin = new Padding(2, 1, 2, 1);
        checkBoxImages.Name = "checkBoxImages";
        checkBoxImages.Size = new Size(102, 19);
        checkBoxImages.TabIndex = 4;
        checkBoxImages.Text = Resources.ImagesCheckboxText;
        checkBoxImages.UseVisualStyleBackColor = true;
        checkBoxImages.Visible = false;
        checkBoxImages.CheckedChanged += CheckBoxImages_CheckedChanged;
        // 
        // outputBoxConsole
        // 
        outputBoxConsole.AllowDrop = true;
        outputBoxConsole.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        outputBoxConsole.BackColor = Color.Black;
        outputBoxConsole.ForeColor = Color.White;
        outputBoxConsole.Location = new Point(7, 70);
        outputBoxConsole.Margin = new Padding(2, 1, 2, 1);
        outputBoxConsole.Name = "outputBoxConsole";
        outputBoxConsole.ReadOnly = true;
        outputBoxConsole.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
        outputBoxConsole.Size = new Size(639, 267);
        outputBoxConsole.TabIndex = 0;
        outputBoxConsole.Text = "";
        outputBoxConsole.DragDrop += OutputBoxConsole_DragDrop;
        outputBoxConsole.DragEnter += OutputBoxConsole_DragEnter;
        // 
        // comboBoxLanguage
        // 
        comboBoxLanguage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        comboBoxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBoxLanguage.FlatStyle = FlatStyle.Flat;
        comboBoxLanguage.FormattingEnabled = true;
        comboBoxLanguage.Items.AddRange(new object[] { "English", "Deutsch", "日本語", "繁體中文", "简体中文" });
        comboBoxLanguage.Location = new Point(478, 12);
        comboBoxLanguage.Name = "comboBoxLanguage";
        comboBoxLanguage.RightToLeft = RightToLeft.No;
        comboBoxLanguage.Size = new Size(78, 23);
        comboBoxLanguage.TabIndex = 7;
        comboBoxLanguage.SelectedIndexChanged += ComboBoxLanguage_SelectedIndexChanged;
        // 
        // buttonFileModeFileList
        // 
        buttonFileModeFileList.FlatAppearance.BorderColor = Color.DimGray;
        buttonFileModeFileList.FlatStyle = FlatStyle.Flat;
        buttonFileModeFileList.Location = new Point(41, 10);
        buttonFileModeFileList.Margin = new Padding(2, 1, 2, 1);
        buttonFileModeFileList.Name = "buttonFileModeFileList";
        buttonFileModeFileList.Size = new Size(74, 25);
        buttonFileModeFileList.TabIndex = 2;
        buttonFileModeFileList.Text = "File List";
        buttonFileModeFileList.UseVisualStyleBackColor = true;
        buttonFileModeFileList.Visible = false;
        buttonFileModeFileList.Click += ButtonFileModeFileList_Click;
        // 
        // textBoxPath
        // 
        textBoxPath.BorderStyle = BorderStyle.FixedSingle;
        textBoxPath.Location = new Point(70, 40);
        textBoxPath.Margin = new Padding(2, 1, 2, 1);
        textBoxPath.Name = "textBoxPath";
        textBoxPath.ReadOnly = true;
        textBoxPath.Size = new Size(575, 23);
        textBoxPath.TabIndex = 13;
        textBoxPath.TextChanged += TextBoxPath_TextChanged;
        // 
        // buttonPathClear
        // 
        buttonPathClear.BackgroundImageLayout = ImageLayout.Center;
        buttonPathClear.FlatAppearance.BorderColor = Color.DimGray;
        buttonPathClear.FlatStyle = FlatStyle.Flat;
        buttonPathClear.Location = new Point(41, 38);
        buttonPathClear.Margin = new Padding(2, 1, 2, 1);
        buttonPathClear.Name = "buttonPathClear";
        buttonPathClear.Size = new Size(25, 25);
        buttonPathClear.TabIndex = 12;
        buttonPathClear.UseVisualStyleBackColor = true;
        buttonPathClear.Click += ButtonPathClear_Click;
        // 
        // buttonSwitchModes
        // 
        buttonSwitchModes.BackgroundImageLayout = ImageLayout.Center;
        buttonSwitchModes.FlatAppearance.BorderColor = Color.DimGray;
        buttonSwitchModes.FlatStyle = FlatStyle.Flat;
        buttonSwitchModes.Location = new Point(12, 10);
        buttonSwitchModes.Margin = new Padding(2, 1, 2, 1);
        buttonSwitchModes.Name = "buttonSwitchModes";
        buttonSwitchModes.Size = new Size(25, 25);
        buttonSwitchModes.TabIndex = 1;
        buttonSwitchModes.UseVisualStyleBackColor = true;
        buttonSwitchModes.Click += ButtonSwitchModes_Click;
        // 
        // statusStripMain
        // 
        statusStripMain.AllowMerge = false;
        statusStripMain.AutoSize = false;
        statusStripMain.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelCurrentMode, toolStripProgressBar });
        statusStripMain.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
        statusStripMain.Location = new Point(0, 338);
        statusStripMain.Name = "statusStripMain";
        statusStripMain.Size = new Size(656, 22);
        statusStripMain.SizingGrip = false;
        statusStripMain.TabIndex = 101;
        // 
        // toolStripStatusLabelCurrentMode
        // 
        toolStripStatusLabelCurrentMode.Name = "toolStripStatusLabelCurrentMode";
        toolStripStatusLabelCurrentMode.Size = new Size(38, 17);
        toolStripStatusLabelCurrentMode.Text = "Mode";
        // 
        // toolStripProgressBar
        // 
        toolStripProgressBar.Alignment = ToolStripItemAlignment.Right;
        toolStripProgressBar.AutoSize = false;
        toolStripProgressBar.Name = "toolStripProgressBar";
        toolStripProgressBar.Overflow = ToolStripItemOverflow.Never;
        toolStripProgressBar.Size = new Size(100, 16);
        toolStripProgressBar.Step = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(656, 360);
        Controls.Add(statusStripMain);
        Controls.Add(buttonPathClear);
        Controls.Add(comboBoxLanguage);
        Controls.Add(buttonOpenSettings);
        Controls.Add(outputBoxConsole);
        Controls.Add(checkBoxImages);
        Controls.Add(checkBoxComicInfo);
        Controls.Add(buttonStart);
        Controls.Add(buttonPath);
        Controls.Add(buttonFileModeFileList);
        Controls.Add(textBoxPath);
        Controls.Add(buttonSwitchModes);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(2, 1, 2, 1);
        MaximizeBox = false;
        MinimumSize = new Size(672, 399);
        Name = "MainForm";
        Text = "epub2cbz";
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;
        statusStripMain.ResumeLayout(false);
        statusStripMain.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
    private FolderBrowserDialog folderBrowserDialog;
    public Button buttonStart;
    public CheckBox checkBoxImages;
    public CheckBox checkBoxComicInfo;
    public RichTextBox outputBoxConsole;
    public Button buttonOpenSettings;
    public ComboBox comboBoxLanguage;
    public ToolTip toolTip;
    public Button buttonFileModeFileList;
    private TextBox textBoxPath;
    public Button buttonPathClear;
    public Button buttonPath;
    public Button buttonSwitchModes;
    private StatusStrip statusStripMain;
    private ToolStripStatusLabel toolStripStatusLabelCurrentMode;
    public ToolStripProgressBar toolStripProgressBar;
}