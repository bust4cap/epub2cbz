namespace epub2cbz_gui
{
    partial class PopupSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            buttonSettingsOK = new Button();
            buttonSettingsCancel = new Button();
            checkBoxSettingsComicInfoSeries = new CheckBox();
            checkBoxSettingsComicInfoTitle = new CheckBox();
            checkBoxSettingsComicInfoVolume = new CheckBox();
            checkBoxSettingsComicInfoAuthors = new CheckBox();
            checkBoxSettingsComicInfoLanguage = new CheckBox();
            checkBoxSettingsComicInfoPublisher = new CheckBox();
            checkBoxSettingsComicInfoDate = new CheckBox();
            checkBoxSettingsComicInfoDescription = new CheckBox();
            groupBoxComicInfo = new GroupBox();
            tableLayoutPanelComicInfo = new TableLayoutPanel();
            checkBoxSettingsComicInfoReadingDirection = new CheckBox();
            checkBoxSettingsComicInfoTranslators = new CheckBox();
            checkBoxSettingsComicInfoImageSize = new CheckBox();
            checkBoxSettingsComicInfoIsbnAsin = new CheckBox();
            checkBoxSettingsComicInfoProducers = new CheckBox();
            checkBoxSettingsComicInfoChapters = new CheckBox();
            checkBoxSettingsComicInfoPageCount = new CheckBox();
            groupBoxExperimental = new GroupBox();
            tableLayoutPanelExperimental = new TableLayoutPanel();
            checkBoxSettingsExperimentalCheckDRMProtection = new CheckBox();
            checkBoxSettingsExperimentalSpreadInsertBlank = new CheckBox();
            checkBoxSettingsExperimentalCoverResolution = new CheckBox();
            checkBoxSettingsExperimentalChapterFolders = new CheckBox();
            checkBoxSettingsExperimentalSplitPageSpread = new CheckBox();
            textBoxReplaceSeries = new TextBox();
            checkBoxSettingsExperimentalPageSpread = new CheckBox();
            checkBoxSettingsCroppingEnable = new CheckBox();
            checkBoxSettingsReplaceSeries = new CheckBox();
            checkBoxSettingsExperimentalRemoveDuplicateCovers = new CheckBox();
            checkBoxSettingsExperimentalEveryPageIsChapter = new CheckBox();
            checkBoxSettingsExperimentalMetadataTitle = new CheckBox();
            checkBoxSettingsExperimentalAddBlankPage = new CheckBox();
            checkBoxSettingsResizingEnable = new CheckBox();
            toolTipSettings = new ToolTip(components);
            dropDownCompressionLevel = new ComboBox();
            groupBoxCompressionLevel = new GroupBox();
            groupBoxThreads = new GroupBox();
            dropDownThreads = new ComboBox();
            buttonSettingsCheckForUpdate = new Button();
            buttonSettingsResetToDefault = new Button();
            groupBoxResizing = new GroupBox();
            tableLayoutPanelResizing = new TableLayoutPanel();
            flowLayoutPanelResizingTopLeft = new FlowLayoutPanel();
            textBoxSettingsResizeWidth = new TextBox();
            labelSettingsResizeWidth = new Label();
            flowLayoutPanelResizingTopRight = new FlowLayoutPanel();
            dropDownKindle = new ComboBox();
            labelSettingsResizeKindle = new Label();
            flowLayoutPanelResizingBottomRight = new FlowLayoutPanel();
            dropDownKobo = new ComboBox();
            labelSettingsResizeKobo = new Label();
            flowLayoutPanelResizingBottomLeft = new FlowLayoutPanel();
            textBoxSettingsResizeHeight = new TextBox();
            labelSettingsResizeHeight = new Label();
            radioButtonSettingsCbz = new RadioButton();
            radioButtonSettingsZip = new RadioButton();
            groupBoxCropping = new GroupBox();
            tableLayoutPanelCropping = new TableLayoutPanel();
            flowLayoutPanelCroppingLeft = new FlowLayoutPanel();
            labelSettingsCropPadding = new Label();
            textBoxSettingsCropPadding = new TextBox();
            flowLayoutPanelCroppingCenter = new FlowLayoutPanel();
            labelSettingsCropColorTolerance = new Label();
            textBoxSettingsCropColorTolerance = new TextBox();
            flowLayoutPanelCroppingRight = new FlowLayoutPanel();
            labelSettingsCropDeviationTolerance = new Label();
            textBoxSettingsCropDeviationTolerance = new TextBox();
            tableLayoutPanelSettings = new TableLayoutPanel();
            tableLayoutPanelSettingsCenter = new TableLayoutPanel();
            panelSettingsCbzZip = new Panel();
            tableLayoutPanelSettingsBottom = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBoxComicInfo.SuspendLayout();
            tableLayoutPanelComicInfo.SuspendLayout();
            groupBoxExperimental.SuspendLayout();
            tableLayoutPanelExperimental.SuspendLayout();
            groupBoxCompressionLevel.SuspendLayout();
            groupBoxThreads.SuspendLayout();
            groupBoxResizing.SuspendLayout();
            tableLayoutPanelResizing.SuspendLayout();
            flowLayoutPanelResizingTopLeft.SuspendLayout();
            flowLayoutPanelResizingTopRight.SuspendLayout();
            flowLayoutPanelResizingBottomRight.SuspendLayout();
            flowLayoutPanelResizingBottomLeft.SuspendLayout();
            groupBoxCropping.SuspendLayout();
            tableLayoutPanelCropping.SuspendLayout();
            flowLayoutPanelCroppingLeft.SuspendLayout();
            flowLayoutPanelCroppingCenter.SuspendLayout();
            flowLayoutPanelCroppingRight.SuspendLayout();
            tableLayoutPanelSettings.SuspendLayout();
            tableLayoutPanelSettingsCenter.SuspendLayout();
            panelSettingsCbzZip.SuspendLayout();
            tableLayoutPanelSettingsBottom.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonSettingsOK
            // 
            buttonSettingsOK.Anchor = AnchorStyles.None;
            buttonSettingsOK.Location = new Point(474, 3);
            buttonSettingsOK.Margin = new Padding(2, 1, 2, 1);
            buttonSettingsOK.Name = "buttonSettingsOK";
            buttonSettingsOK.Size = new Size(77, 25);
            buttonSettingsOK.TabIndex = 202;
            buttonSettingsOK.Text = "OK";
            buttonSettingsOK.UseVisualStyleBackColor = true;
            buttonSettingsOK.Click += BtnSettingsOK_Click;
            // 
            // buttonSettingsCancel
            // 
            buttonSettingsCancel.Anchor = AnchorStyles.None;
            buttonSettingsCancel.Location = new Point(395, 3);
            buttonSettingsCancel.Margin = new Padding(2, 1, 2, 1);
            buttonSettingsCancel.Name = "buttonSettingsCancel";
            buttonSettingsCancel.Size = new Size(75, 25);
            buttonSettingsCancel.TabIndex = 201;
            buttonSettingsCancel.Text = "Cancel";
            buttonSettingsCancel.UseVisualStyleBackColor = true;
            buttonSettingsCancel.Click += BtnSettingsCancel_Click;
            // 
            // checkBoxSettingsComicInfoSeries
            // 
            checkBoxSettingsComicInfoSeries.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoSeries.AutoSize = true;
            checkBoxSettingsComicInfoSeries.Location = new Point(138, 4);
            checkBoxSettingsComicInfoSeries.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoSeries.Name = "checkBoxSettingsComicInfoSeries";
            checkBoxSettingsComicInfoSeries.Size = new Size(56, 19);
            checkBoxSettingsComicInfoSeries.TabIndex = 2;
            checkBoxSettingsComicInfoSeries.Text = "Series";
            checkBoxSettingsComicInfoSeries.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoTitle
            // 
            checkBoxSettingsComicInfoTitle.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoTitle.AutoSize = true;
            checkBoxSettingsComicInfoTitle.Location = new Point(5, 4);
            checkBoxSettingsComicInfoTitle.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoTitle.Name = "checkBoxSettingsComicInfoTitle";
            checkBoxSettingsComicInfoTitle.Size = new Size(48, 19);
            checkBoxSettingsComicInfoTitle.TabIndex = 1;
            checkBoxSettingsComicInfoTitle.Text = "Title";
            checkBoxSettingsComicInfoTitle.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoVolume
            // 
            checkBoxSettingsComicInfoVolume.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoVolume.AutoSize = true;
            checkBoxSettingsComicInfoVolume.Location = new Point(271, 4);
            checkBoxSettingsComicInfoVolume.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoVolume.Name = "checkBoxSettingsComicInfoVolume";
            checkBoxSettingsComicInfoVolume.Size = new Size(66, 19);
            checkBoxSettingsComicInfoVolume.TabIndex = 3;
            checkBoxSettingsComicInfoVolume.Text = "Volume";
            checkBoxSettingsComicInfoVolume.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoAuthors
            // 
            checkBoxSettingsComicInfoAuthors.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoAuthors.AutoSize = true;
            checkBoxSettingsComicInfoAuthors.Location = new Point(138, 25);
            checkBoxSettingsComicInfoAuthors.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoAuthors.Name = "checkBoxSettingsComicInfoAuthors";
            checkBoxSettingsComicInfoAuthors.Size = new Size(68, 19);
            checkBoxSettingsComicInfoAuthors.TabIndex = 6;
            checkBoxSettingsComicInfoAuthors.Text = "Authors";
            checkBoxSettingsComicInfoAuthors.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoLanguage
            // 
            checkBoxSettingsComicInfoLanguage.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoLanguage.AutoSize = true;
            checkBoxSettingsComicInfoLanguage.Location = new Point(271, 46);
            checkBoxSettingsComicInfoLanguage.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoLanguage.Name = "checkBoxSettingsComicInfoLanguage";
            checkBoxSettingsComicInfoLanguage.Size = new Size(78, 19);
            checkBoxSettingsComicInfoLanguage.TabIndex = 11;
            checkBoxSettingsComicInfoLanguage.Text = "Language";
            checkBoxSettingsComicInfoLanguage.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoPublisher
            // 
            checkBoxSettingsComicInfoPublisher.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoPublisher.AutoSize = true;
            checkBoxSettingsComicInfoPublisher.Location = new Point(5, 46);
            checkBoxSettingsComicInfoPublisher.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoPublisher.Name = "checkBoxSettingsComicInfoPublisher";
            checkBoxSettingsComicInfoPublisher.Size = new Size(75, 19);
            checkBoxSettingsComicInfoPublisher.TabIndex = 9;
            checkBoxSettingsComicInfoPublisher.Text = "Publisher";
            checkBoxSettingsComicInfoPublisher.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoDate
            // 
            checkBoxSettingsComicInfoDate.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoDate.AutoSize = true;
            checkBoxSettingsComicInfoDate.Location = new Point(5, 25);
            checkBoxSettingsComicInfoDate.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoDate.Name = "checkBoxSettingsComicInfoDate";
            checkBoxSettingsComicInfoDate.Size = new Size(50, 19);
            checkBoxSettingsComicInfoDate.TabIndex = 5;
            checkBoxSettingsComicInfoDate.Text = "Date";
            checkBoxSettingsComicInfoDate.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoDescription
            // 
            checkBoxSettingsComicInfoDescription.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoDescription.AutoSize = true;
            checkBoxSettingsComicInfoDescription.Location = new Point(404, 4);
            checkBoxSettingsComicInfoDescription.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoDescription.Name = "checkBoxSettingsComicInfoDescription";
            checkBoxSettingsComicInfoDescription.Size = new Size(86, 19);
            checkBoxSettingsComicInfoDescription.TabIndex = 4;
            checkBoxSettingsComicInfoDescription.Text = "Description";
            checkBoxSettingsComicInfoDescription.UseVisualStyleBackColor = false;
            // 
            // groupBoxComicInfo
            // 
            groupBoxComicInfo.AutoSize = true;
            groupBoxComicInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxComicInfo.Controls.Add(tableLayoutPanelComicInfo);
            groupBoxComicInfo.Dock = DockStyle.Fill;
            groupBoxComicInfo.Location = new Point(3, 3);
            groupBoxComicInfo.Name = "groupBoxComicInfo";
            groupBoxComicInfo.Size = new Size(547, 112);
            groupBoxComicInfo.TabIndex = 0;
            groupBoxComicInfo.TabStop = false;
            groupBoxComicInfo.Text = "Info to include in ComicInfo.xml:";
            // 
            // tableLayoutPanelComicInfo
            // 
            tableLayoutPanelComicInfo.AutoSize = true;
            tableLayoutPanelComicInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelComicInfo.ColumnCount = 4;
            tableLayoutPanelComicInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoReadingDirection, 3, 2);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoTranslators, 3, 1);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoImageSize, 2, 3);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoIsbnAsin, 0, 3);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoDescription, 3, 0);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoTitle, 0, 0);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoProducers, 2, 1);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoDate, 0, 1);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoLanguage, 2, 2);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoChapters, 1, 3);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoPublisher, 0, 2);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoVolume, 2, 0);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoPageCount, 1, 2);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoSeries, 1, 0);
            tableLayoutPanelComicInfo.Controls.Add(checkBoxSettingsComicInfoAuthors, 1, 1);
            tableLayoutPanelComicInfo.Dock = DockStyle.Fill;
            tableLayoutPanelComicInfo.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelComicInfo.Location = new Point(3, 19);
            tableLayoutPanelComicInfo.Name = "tableLayoutPanelComicInfo";
            tableLayoutPanelComicInfo.Padding = new Padding(3);
            tableLayoutPanelComicInfo.RowCount = 4;
            tableLayoutPanelComicInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanelComicInfo.Size = new Size(541, 90);
            tableLayoutPanelComicInfo.TabIndex = 207;
            // 
            // checkBoxSettingsComicInfoReadingDirection
            // 
            checkBoxSettingsComicInfoReadingDirection.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoReadingDirection.AutoSize = true;
            checkBoxSettingsComicInfoReadingDirection.Location = new Point(404, 46);
            checkBoxSettingsComicInfoReadingDirection.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoReadingDirection.Name = "checkBoxSettingsComicInfoReadingDirection";
            checkBoxSettingsComicInfoReadingDirection.Size = new Size(120, 19);
            checkBoxSettingsComicInfoReadingDirection.TabIndex = 12;
            checkBoxSettingsComicInfoReadingDirection.Text = "Reading Direction";
            checkBoxSettingsComicInfoReadingDirection.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoTranslators
            // 
            checkBoxSettingsComicInfoTranslators.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoTranslators.AutoSize = true;
            checkBoxSettingsComicInfoTranslators.Location = new Point(404, 25);
            checkBoxSettingsComicInfoTranslators.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoTranslators.Name = "checkBoxSettingsComicInfoTranslators";
            checkBoxSettingsComicInfoTranslators.Size = new Size(82, 19);
            checkBoxSettingsComicInfoTranslators.TabIndex = 8;
            checkBoxSettingsComicInfoTranslators.Text = "Translators";
            checkBoxSettingsComicInfoTranslators.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoImageSize
            // 
            checkBoxSettingsComicInfoImageSize.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoImageSize.AutoSize = true;
            checkBoxSettingsComicInfoImageSize.Location = new Point(271, 67);
            checkBoxSettingsComicInfoImageSize.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoImageSize.Name = "checkBoxSettingsComicInfoImageSize";
            checkBoxSettingsComicInfoImageSize.Size = new Size(82, 19);
            checkBoxSettingsComicInfoImageSize.TabIndex = 15;
            checkBoxSettingsComicInfoImageSize.Text = "Image Size";
            checkBoxSettingsComicInfoImageSize.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoIsbnAsin
            // 
            checkBoxSettingsComicInfoIsbnAsin.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoIsbnAsin.AutoSize = true;
            checkBoxSettingsComicInfoIsbnAsin.Location = new Point(5, 67);
            checkBoxSettingsComicInfoIsbnAsin.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoIsbnAsin.Name = "checkBoxSettingsComicInfoIsbnAsin";
            checkBoxSettingsComicInfoIsbnAsin.Size = new Size(82, 19);
            checkBoxSettingsComicInfoIsbnAsin.TabIndex = 13;
            checkBoxSettingsComicInfoIsbnAsin.Text = "ISBN/ASIN";
            checkBoxSettingsComicInfoIsbnAsin.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoProducers
            // 
            checkBoxSettingsComicInfoProducers.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoProducers.AutoSize = true;
            checkBoxSettingsComicInfoProducers.Location = new Point(271, 25);
            checkBoxSettingsComicInfoProducers.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoProducers.Name = "checkBoxSettingsComicInfoProducers";
            checkBoxSettingsComicInfoProducers.Size = new Size(79, 19);
            checkBoxSettingsComicInfoProducers.TabIndex = 7;
            checkBoxSettingsComicInfoProducers.Text = "Producers";
            checkBoxSettingsComicInfoProducers.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoChapters
            // 
            checkBoxSettingsComicInfoChapters.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoChapters.AutoSize = true;
            checkBoxSettingsComicInfoChapters.Location = new Point(138, 67);
            checkBoxSettingsComicInfoChapters.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoChapters.Name = "checkBoxSettingsComicInfoChapters";
            checkBoxSettingsComicInfoChapters.Size = new Size(73, 19);
            checkBoxSettingsComicInfoChapters.TabIndex = 14;
            checkBoxSettingsComicInfoChapters.Text = "Chapters";
            checkBoxSettingsComicInfoChapters.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoPageCount
            // 
            checkBoxSettingsComicInfoPageCount.Anchor = AnchorStyles.Left;
            checkBoxSettingsComicInfoPageCount.AutoSize = true;
            checkBoxSettingsComicInfoPageCount.Location = new Point(138, 46);
            checkBoxSettingsComicInfoPageCount.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoPageCount.Name = "checkBoxSettingsComicInfoPageCount";
            checkBoxSettingsComicInfoPageCount.Size = new Size(88, 19);
            checkBoxSettingsComicInfoPageCount.TabIndex = 10;
            checkBoxSettingsComicInfoPageCount.Text = "Page Count";
            checkBoxSettingsComicInfoPageCount.UseVisualStyleBackColor = false;
            // 
            // groupBoxExperimental
            // 
            groupBoxExperimental.AutoSize = true;
            groupBoxExperimental.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxExperimental.Controls.Add(tableLayoutPanelExperimental);
            groupBoxExperimental.Dock = DockStyle.Fill;
            groupBoxExperimental.Location = new Point(3, 121);
            groupBoxExperimental.Name = "groupBoxExperimental";
            groupBoxExperimental.Size = new Size(547, 203);
            groupBoxExperimental.TabIndex = 20;
            groupBoxExperimental.TabStop = false;
            groupBoxExperimental.Text = "Experimental Features:";
            // 
            // tableLayoutPanelExperimental
            // 
            tableLayoutPanelExperimental.AutoSize = true;
            tableLayoutPanelExperimental.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelExperimental.ColumnCount = 2;
            tableLayoutPanelExperimental.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelExperimental.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalCheckDRMProtection, 1, 3);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalSpreadInsertBlank, 1, 0);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalCoverResolution, 1, 1);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalChapterFolders, 1, 4);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalSplitPageSpread, 1, 2);
            tableLayoutPanelExperimental.Controls.Add(textBoxReplaceSeries, 1, 6);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalPageSpread, 0, 0);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsCroppingEnable, 1, 5);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsReplaceSeries, 0, 6);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalRemoveDuplicateCovers, 0, 1);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalEveryPageIsChapter, 0, 2);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalMetadataTitle, 0, 4);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsExperimentalAddBlankPage, 0, 3);
            tableLayoutPanelExperimental.Controls.Add(checkBoxSettingsResizingEnable, 0, 5);
            tableLayoutPanelExperimental.Dock = DockStyle.Fill;
            tableLayoutPanelExperimental.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelExperimental.Location = new Point(3, 19);
            tableLayoutPanelExperimental.Name = "tableLayoutPanelExperimental";
            tableLayoutPanelExperimental.Padding = new Padding(3);
            tableLayoutPanelExperimental.RowCount = 7;
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857132F));
            tableLayoutPanelExperimental.Size = new Size(541, 181);
            tableLayoutPanelExperimental.TabIndex = 207;
            // 
            // checkBoxSettingsExperimentalCheckDRMProtection
            // 
            checkBoxSettingsExperimentalCheckDRMProtection.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalCheckDRMProtection.AutoSize = true;
            checkBoxSettingsExperimentalCheckDRMProtection.Location = new Point(272, 81);
            checkBoxSettingsExperimentalCheckDRMProtection.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalCheckDRMProtection.Name = "checkBoxSettingsExperimentalCheckDRMProtection";
            checkBoxSettingsExperimentalCheckDRMProtection.Size = new Size(195, 19);
            checkBoxSettingsExperimentalCheckDRMProtection.TabIndex = 28;
            checkBoxSettingsExperimentalCheckDRMProtection.Text = "Check if files are DRM protected";
            checkBoxSettingsExperimentalCheckDRMProtection.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalSpreadInsertBlank
            // 
            checkBoxSettingsExperimentalSpreadInsertBlank.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalSpreadInsertBlank.AutoSize = true;
            checkBoxSettingsExperimentalSpreadInsertBlank.Location = new Point(272, 6);
            checkBoxSettingsExperimentalSpreadInsertBlank.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalSpreadInsertBlank.Name = "checkBoxSettingsExperimentalSpreadInsertBlank";
            checkBoxSettingsExperimentalSpreadInsertBlank.Size = new Size(133, 19);
            checkBoxSettingsExperimentalSpreadInsertBlank.TabIndex = 22;
            checkBoxSettingsExperimentalSpreadInsertBlank.Text = "Blank Page behavior";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSpreadInsertBlank, "When preserving Page Spread and a Blank Image would have to be inserted to do so, check if a Blank Image is following and if it is, delete that one instead of having 2 Blank Images back to back.");
            checkBoxSettingsExperimentalSpreadInsertBlank.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalCoverResolution
            // 
            checkBoxSettingsExperimentalCoverResolution.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalCoverResolution.AutoSize = true;
            checkBoxSettingsExperimentalCoverResolution.Location = new Point(272, 31);
            checkBoxSettingsExperimentalCoverResolution.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalCoverResolution.Name = "checkBoxSettingsExperimentalCoverResolution";
            checkBoxSettingsExperimentalCoverResolution.Size = new Size(179, 19);
            checkBoxSettingsExperimentalCoverResolution.TabIndex = 24;
            checkBoxSettingsExperimentalCoverResolution.Text = "Keep higher resolution Cover";
            checkBoxSettingsExperimentalCoverResolution.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalChapterFolders
            // 
            checkBoxSettingsExperimentalChapterFolders.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalChapterFolders.AutoSize = true;
            checkBoxSettingsExperimentalChapterFolders.Location = new Point(272, 106);
            checkBoxSettingsExperimentalChapterFolders.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalChapterFolders.Name = "checkBoxSettingsExperimentalChapterFolders";
            checkBoxSettingsExperimentalChapterFolders.Size = new Size(192, 19);
            checkBoxSettingsExperimentalChapterFolders.TabIndex = 30;
            checkBoxSettingsExperimentalChapterFolders.Text = "Create Folders for each Chapter";
            checkBoxSettingsExperimentalChapterFolders.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalSplitPageSpread
            // 
            checkBoxSettingsExperimentalSplitPageSpread.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalSplitPageSpread.AutoSize = true;
            checkBoxSettingsExperimentalSplitPageSpread.Location = new Point(272, 56);
            checkBoxSettingsExperimentalSplitPageSpread.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalSplitPageSpread.Name = "checkBoxSettingsExperimentalSplitPageSpread";
            checkBoxSettingsExperimentalSplitPageSpread.Size = new Size(118, 19);
            checkBoxSettingsExperimentalSplitPageSpread.TabIndex = 26;
            checkBoxSettingsExperimentalSplitPageSpread.Text = "Split wide images";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSplitPageSpread, "Split Page Spreads that are one wide image into two single page images.");
            checkBoxSettingsExperimentalSplitPageSpread.UseVisualStyleBackColor = false;
            // 
            // textBoxReplaceSeries
            // 
            textBoxReplaceSeries.Anchor = AnchorStyles.Left;
            textBoxReplaceSeries.Location = new Point(272, 154);
            textBoxReplaceSeries.Margin = new Padding(2, 1, 2, 1);
            textBoxReplaceSeries.Name = "textBoxReplaceSeries";
            textBoxReplaceSeries.Size = new Size(255, 23);
            textBoxReplaceSeries.TabIndex = 34;
            // 
            // checkBoxSettingsExperimentalPageSpread
            // 
            checkBoxSettingsExperimentalPageSpread.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalPageSpread.AutoSize = true;
            checkBoxSettingsExperimentalPageSpread.Location = new Point(5, 6);
            checkBoxSettingsExperimentalPageSpread.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalPageSpread.Name = "checkBoxSettingsExperimentalPageSpread";
            checkBoxSettingsExperimentalPageSpread.Size = new Size(227, 19);
            checkBoxSettingsExperimentalPageSpread.TabIndex = 21;
            checkBoxSettingsExperimentalPageSpread.Text = "Try to preserve Page Spread alignment";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalPageSpread, "Tries to preserve correct Page Spread alignment, so a right page will stay a right page and a left page will stay a left page. Assumes Epub has set this info correctly.");
            checkBoxSettingsExperimentalPageSpread.UseVisualStyleBackColor = false;
            checkBoxSettingsExperimentalPageSpread.CheckedChanged += CheckBoxSettingsPageSpread_CheckedChanged;
            // 
            // checkBoxSettingsCroppingEnable
            // 
            checkBoxSettingsCroppingEnable.Anchor = AnchorStyles.Left;
            checkBoxSettingsCroppingEnable.AutoSize = true;
            checkBoxSettingsCroppingEnable.Location = new Point(272, 131);
            checkBoxSettingsCroppingEnable.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsCroppingEnable.Name = "checkBoxSettingsCroppingEnable";
            checkBoxSettingsCroppingEnable.Size = new Size(93, 19);
            checkBoxSettingsCroppingEnable.TabIndex = 32;
            checkBoxSettingsCroppingEnable.Text = "Crop Images";
            checkBoxSettingsCroppingEnable.UseVisualStyleBackColor = true;
            checkBoxSettingsCroppingEnable.CheckedChanged += CheckBoxSettingsCroppingEnable_CheckedChanged;
            // 
            // checkBoxSettingsReplaceSeries
            // 
            checkBoxSettingsReplaceSeries.Anchor = AnchorStyles.Left;
            checkBoxSettingsReplaceSeries.AutoSize = true;
            checkBoxSettingsReplaceSeries.Location = new Point(5, 156);
            checkBoxSettingsReplaceSeries.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsReplaceSeries.Name = "checkBoxSettingsReplaceSeries";
            checkBoxSettingsReplaceSeries.Size = new Size(135, 19);
            checkBoxSettingsReplaceSeries.TabIndex = 33;
            checkBoxSettingsReplaceSeries.Text = "Replace Series Name";
            checkBoxSettingsReplaceSeries.UseVisualStyleBackColor = false;
            checkBoxSettingsReplaceSeries.CheckedChanged += CheckBoxSettingsReplaceSeries_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalRemoveDuplicateCovers
            // 
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.AutoSize = true;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Location = new Point(5, 31);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Name = "checkBoxSettingsExperimentalRemoveDuplicateCovers";
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Size = new Size(160, 19);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.TabIndex = 23;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Text = "Remove duplicate Covers";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalRemoveDuplicateCovers, "Removes duplicate Cover if it is displayed as the first page again. (Can happen with books bought on Amazon)");
            checkBoxSettingsExperimentalRemoveDuplicateCovers.UseVisualStyleBackColor = false;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.CheckedChanged += CheckBoxSettingsExperimentalRemoveDuplicateCovers_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalEveryPageIsChapter
            // 
            checkBoxSettingsExperimentalEveryPageIsChapter.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalEveryPageIsChapter.AutoSize = true;
            checkBoxSettingsExperimentalEveryPageIsChapter.Location = new Point(5, 56);
            checkBoxSettingsExperimentalEveryPageIsChapter.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalEveryPageIsChapter.Name = "checkBoxSettingsExperimentalEveryPageIsChapter";
            checkBoxSettingsExperimentalEveryPageIsChapter.Size = new Size(188, 19);
            checkBoxSettingsExperimentalEveryPageIsChapter.TabIndex = 25;
            checkBoxSettingsExperimentalEveryPageIsChapter.Text = "Fix every Page being a Chapter";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalEveryPageIsChapter, "Some books can have every page as a separate chapter. This removes all of them for a cleaner look. (Can happen with books bought on Amazon)");
            checkBoxSettingsExperimentalEveryPageIsChapter.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalMetadataTitle
            // 
            checkBoxSettingsExperimentalMetadataTitle.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalMetadataTitle.AutoSize = true;
            checkBoxSettingsExperimentalMetadataTitle.Location = new Point(5, 106);
            checkBoxSettingsExperimentalMetadataTitle.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalMetadataTitle.Name = "checkBoxSettingsExperimentalMetadataTitle";
            checkBoxSettingsExperimentalMetadataTitle.Size = new Size(123, 19);
            checkBoxSettingsExperimentalMetadataTitle.TabIndex = 29;
            checkBoxSettingsExperimentalMetadataTitle.Text = "Use metadata Title";
            checkBoxSettingsExperimentalMetadataTitle.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalAddBlankPage
            // 
            checkBoxSettingsExperimentalAddBlankPage.Anchor = AnchorStyles.Left;
            checkBoxSettingsExperimentalAddBlankPage.AutoSize = true;
            checkBoxSettingsExperimentalAddBlankPage.Location = new Point(5, 81);
            checkBoxSettingsExperimentalAddBlankPage.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalAddBlankPage.Name = "checkBoxSettingsExperimentalAddBlankPage";
            checkBoxSettingsExperimentalAddBlankPage.Size = new Size(109, 19);
            checkBoxSettingsExperimentalAddBlankPage.TabIndex = 27;
            checkBoxSettingsExperimentalAddBlankPage.Text = "Add Blank Page";
            checkBoxSettingsExperimentalAddBlankPage.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsResizingEnable
            // 
            checkBoxSettingsResizingEnable.Anchor = AnchorStyles.Left;
            checkBoxSettingsResizingEnable.AutoSize = true;
            checkBoxSettingsResizingEnable.Location = new Point(5, 131);
            checkBoxSettingsResizingEnable.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsResizingEnable.Name = "checkBoxSettingsResizingEnable";
            checkBoxSettingsResizingEnable.Size = new Size(99, 19);
            checkBoxSettingsResizingEnable.TabIndex = 31;
            checkBoxSettingsResizingEnable.Text = "Resize Images";
            checkBoxSettingsResizingEnable.UseVisualStyleBackColor = false;
            checkBoxSettingsResizingEnable.CheckedChanged += CheckBoxSettingsResizingEnable_CheckedChanged;
            // 
            // dropDownCompressionLevel
            // 
            dropDownCompressionLevel.Dock = DockStyle.Fill;
            dropDownCompressionLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownCompressionLevel.FormattingEnabled = true;
            dropDownCompressionLevel.Items.AddRange(new object[] { "No Compression", "Fastest", "Optimal", "Smallest Size" });
            dropDownCompressionLevel.Location = new Point(3, 19);
            dropDownCompressionLevel.Name = "dropDownCompressionLevel";
            dropDownCompressionLevel.Size = new Size(211, 23);
            dropDownCompressionLevel.TabIndex = 81;
            toolTipSettings.SetToolTip(dropDownCompressionLevel, "'No Compression' is fastest and recommended, as the images are already in a compressed format.");
            // 
            // groupBoxCompressionLevel
            // 
            groupBoxCompressionLevel.AutoSize = true;
            groupBoxCompressionLevel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxCompressionLevel.Controls.Add(dropDownCompressionLevel);
            groupBoxCompressionLevel.Dock = DockStyle.Fill;
            groupBoxCompressionLevel.Location = new Point(2, 1);
            groupBoxCompressionLevel.Margin = new Padding(2, 1, 2, 1);
            groupBoxCompressionLevel.Name = "groupBoxCompressionLevel";
            groupBoxCompressionLevel.Size = new Size(217, 54);
            groupBoxCompressionLevel.TabIndex = 50;
            groupBoxCompressionLevel.TabStop = false;
            groupBoxCompressionLevel.Text = "CBZ Compression Level:";
            // 
            // groupBoxThreads
            // 
            groupBoxThreads.AutoSize = true;
            groupBoxThreads.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxThreads.Controls.Add(dropDownThreads);
            groupBoxThreads.Dock = DockStyle.Fill;
            groupBoxThreads.Location = new Point(333, 1);
            groupBoxThreads.Margin = new Padding(2, 1, 2, 1);
            groupBoxThreads.Name = "groupBoxThreads";
            groupBoxThreads.Size = new Size(218, 54);
            groupBoxThreads.TabIndex = 60;
            groupBoxThreads.TabStop = false;
            groupBoxThreads.Text = "Threads";
            // 
            // dropDownThreads
            // 
            dropDownThreads.Dock = DockStyle.Fill;
            dropDownThreads.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownThreads.FormattingEnabled = true;
            dropDownThreads.Location = new Point(3, 19);
            dropDownThreads.Name = "dropDownThreads";
            dropDownThreads.Size = new Size(212, 23);
            dropDownThreads.TabIndex = 111;
            // 
            // buttonSettingsCheckForUpdate
            // 
            buttonSettingsCheckForUpdate.Anchor = AnchorStyles.None;
            buttonSettingsCheckForUpdate.Location = new Point(3, 3);
            buttonSettingsCheckForUpdate.Name = "buttonSettingsCheckForUpdate";
            buttonSettingsCheckForUpdate.Size = new Size(81, 25);
            buttonSettingsCheckForUpdate.TabIndex = 121;
            buttonSettingsCheckForUpdate.Text = "Update";
            buttonSettingsCheckForUpdate.UseVisualStyleBackColor = true;
            buttonSettingsCheckForUpdate.Click += ButtonSettingsCheckForUpdate_Click;
            // 
            // buttonSettingsResetToDefault
            // 
            buttonSettingsResetToDefault.Anchor = AnchorStyles.None;
            buttonSettingsResetToDefault.Location = new Point(193, 3);
            buttonSettingsResetToDefault.Name = "buttonSettingsResetToDefault";
            buttonSettingsResetToDefault.Size = new Size(93, 25);
            buttonSettingsResetToDefault.TabIndex = 151;
            buttonSettingsResetToDefault.Text = "Reset Settings";
            buttonSettingsResetToDefault.UseVisualStyleBackColor = true;
            buttonSettingsResetToDefault.Click += ButtonSettingsResetToDefault_Click;
            // 
            // groupBoxResizing
            // 
            groupBoxResizing.AutoSize = true;
            groupBoxResizing.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxResizing.Controls.Add(tableLayoutPanelResizing);
            groupBoxResizing.Dock = DockStyle.Fill;
            groupBoxResizing.Location = new Point(3, 330);
            groupBoxResizing.Name = "groupBoxResizing";
            groupBoxResizing.Size = new Size(547, 98);
            groupBoxResizing.TabIndex = 203;
            groupBoxResizing.TabStop = false;
            groupBoxResizing.Text = "Resizing:";
            // 
            // tableLayoutPanelResizing
            // 
            tableLayoutPanelResizing.AutoSize = true;
            tableLayoutPanelResizing.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelResizing.ColumnCount = 2;
            tableLayoutPanelResizing.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27F));
            tableLayoutPanelResizing.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 73F));
            tableLayoutPanelResizing.Controls.Add(flowLayoutPanelResizingTopLeft, 0, 0);
            tableLayoutPanelResizing.Controls.Add(flowLayoutPanelResizingTopRight, 1, 0);
            tableLayoutPanelResizing.Controls.Add(flowLayoutPanelResizingBottomRight, 1, 1);
            tableLayoutPanelResizing.Controls.Add(flowLayoutPanelResizingBottomLeft, 0, 1);
            tableLayoutPanelResizing.Dock = DockStyle.Fill;
            tableLayoutPanelResizing.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelResizing.Location = new Point(3, 19);
            tableLayoutPanelResizing.Name = "tableLayoutPanelResizing";
            tableLayoutPanelResizing.Padding = new Padding(3);
            tableLayoutPanelResizing.RowCount = 2;
            tableLayoutPanelResizing.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelResizing.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanelResizing.Size = new Size(541, 76);
            tableLayoutPanelResizing.TabIndex = 207;
            // 
            // flowLayoutPanelResizingTopLeft
            // 
            flowLayoutPanelResizingTopLeft.AutoSize = true;
            flowLayoutPanelResizingTopLeft.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelResizingTopLeft.Controls.Add(textBoxSettingsResizeWidth);
            flowLayoutPanelResizingTopLeft.Controls.Add(labelSettingsResizeWidth);
            flowLayoutPanelResizingTopLeft.Dock = DockStyle.Fill;
            flowLayoutPanelResizingTopLeft.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelResizingTopLeft.Location = new Point(6, 6);
            flowLayoutPanelResizingTopLeft.Name = "flowLayoutPanelResizingTopLeft";
            flowLayoutPanelResizingTopLeft.Size = new Size(138, 29);
            flowLayoutPanelResizingTopLeft.TabIndex = 0;
            flowLayoutPanelResizingTopLeft.WrapContents = false;
            // 
            // textBoxSettingsResizeWidth
            // 
            textBoxSettingsResizeWidth.Anchor = AnchorStyles.None;
            textBoxSettingsResizeWidth.Location = new Point(87, 3);
            textBoxSettingsResizeWidth.MaxLength = 4;
            textBoxSettingsResizeWidth.Name = "textBoxSettingsResizeWidth";
            textBoxSettingsResizeWidth.Size = new Size(48, 23);
            textBoxSettingsResizeWidth.TabIndex = 51;
            textBoxSettingsResizeWidth.KeyPress += TextBoxSettingsResizeWidth_KeyPress;
            textBoxSettingsResizeWidth.Leave += TextBoxSettingsResizeWidth_Leave;
            // 
            // labelSettingsResizeWidth
            // 
            labelSettingsResizeWidth.Anchor = AnchorStyles.None;
            labelSettingsResizeWidth.AutoSize = true;
            labelSettingsResizeWidth.Location = new Point(39, 7);
            labelSettingsResizeWidth.Name = "labelSettingsResizeWidth";
            labelSettingsResizeWidth.Size = new Size(42, 15);
            labelSettingsResizeWidth.TabIndex = 8;
            labelSettingsResizeWidth.Text = "Width:";
            // 
            // flowLayoutPanelResizingTopRight
            // 
            flowLayoutPanelResizingTopRight.AutoSize = true;
            flowLayoutPanelResizingTopRight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelResizingTopRight.Controls.Add(dropDownKindle);
            flowLayoutPanelResizingTopRight.Controls.Add(labelSettingsResizeKindle);
            flowLayoutPanelResizingTopRight.Dock = DockStyle.Fill;
            flowLayoutPanelResizingTopRight.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelResizingTopRight.Location = new Point(150, 6);
            flowLayoutPanelResizingTopRight.Name = "flowLayoutPanelResizingTopRight";
            flowLayoutPanelResizingTopRight.Size = new Size(385, 29);
            flowLayoutPanelResizingTopRight.TabIndex = 1;
            flowLayoutPanelResizingTopRight.WrapContents = false;
            // 
            // dropDownKindle
            // 
            dropDownKindle.Anchor = AnchorStyles.None;
            dropDownKindle.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownKindle.FormattingEnabled = true;
            dropDownKindle.Items.AddRange(new object[] { "Kindle 1/2", "Kindle DX/DXG", "Kindle 5-10/Keyboard/Touch", "Kindle Paperwhite 1/2", "Kindle 11/Voyage/Oasis", "Kindle Paperwhite 3/4", "Kindle Oasis 2/3/Paperwhite 12/Colorsoft 12", "Kindle Paperwhite 5/Signature Edition", "Kindle Scribe" });
            dropDownKindle.Location = new Point(107, 3);
            dropDownKindle.Name = "dropDownKindle";
            dropDownKindle.Size = new Size(275, 23);
            dropDownKindle.TabIndex = 53;
            dropDownKindle.SelectedIndexChanged += DropDownKindle_SelectedIndexChanged;
            // 
            // labelSettingsResizeKindle
            // 
            labelSettingsResizeKindle.Anchor = AnchorStyles.None;
            labelSettingsResizeKindle.AutoSize = true;
            labelSettingsResizeKindle.Location = new Point(58, 7);
            labelSettingsResizeKindle.Name = "labelSettingsResizeKindle";
            labelSettingsResizeKindle.Size = new Size(43, 15);
            labelSettingsResizeKindle.TabIndex = 10;
            labelSettingsResizeKindle.Text = "Kindle:";
            // 
            // flowLayoutPanelResizingBottomRight
            // 
            flowLayoutPanelResizingBottomRight.AutoSize = true;
            flowLayoutPanelResizingBottomRight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelResizingBottomRight.Controls.Add(dropDownKobo);
            flowLayoutPanelResizingBottomRight.Controls.Add(labelSettingsResizeKobo);
            flowLayoutPanelResizingBottomRight.Dock = DockStyle.Fill;
            flowLayoutPanelResizingBottomRight.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelResizingBottomRight.Location = new Point(150, 41);
            flowLayoutPanelResizingBottomRight.Name = "flowLayoutPanelResizingBottomRight";
            flowLayoutPanelResizingBottomRight.Size = new Size(385, 29);
            flowLayoutPanelResizingBottomRight.TabIndex = 2;
            flowLayoutPanelResizingBottomRight.WrapContents = false;
            // 
            // dropDownKobo
            // 
            dropDownKobo.Anchor = AnchorStyles.None;
            dropDownKobo.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownKobo.FormattingEnabled = true;
            dropDownKobo.Items.AddRange(new object[] { "Kobo Mini/Touch", "Kobo Glo", "Kobo Glo HD/Clara HD/Clara 2E/Clara Colour", "Kobo Aura/Nia", "Kobo Aura HD", "Kobo Aura H2O", "Kobo Elipsa/Aura ONE", "Kobo Libra H2O/Libra 2/Libra Colour", "Kobo Forma/Sage" });
            dropDownKobo.Location = new Point(107, 3);
            dropDownKobo.Name = "dropDownKobo";
            dropDownKobo.Size = new Size(275, 23);
            dropDownKobo.TabIndex = 54;
            dropDownKobo.SelectedIndexChanged += DropDownKobo_SelectedIndexChanged;
            // 
            // labelSettingsResizeKobo
            // 
            labelSettingsResizeKobo.Anchor = AnchorStyles.None;
            labelSettingsResizeKobo.AutoSize = true;
            labelSettingsResizeKobo.Location = new Point(63, 7);
            labelSettingsResizeKobo.Name = "labelSettingsResizeKobo";
            labelSettingsResizeKobo.Size = new Size(38, 15);
            labelSettingsResizeKobo.TabIndex = 11;
            labelSettingsResizeKobo.Text = "Kobo:";
            // 
            // flowLayoutPanelResizingBottomLeft
            // 
            flowLayoutPanelResizingBottomLeft.AutoSize = true;
            flowLayoutPanelResizingBottomLeft.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelResizingBottomLeft.Controls.Add(textBoxSettingsResizeHeight);
            flowLayoutPanelResizingBottomLeft.Controls.Add(labelSettingsResizeHeight);
            flowLayoutPanelResizingBottomLeft.Dock = DockStyle.Fill;
            flowLayoutPanelResizingBottomLeft.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelResizingBottomLeft.Location = new Point(6, 41);
            flowLayoutPanelResizingBottomLeft.Name = "flowLayoutPanelResizingBottomLeft";
            flowLayoutPanelResizingBottomLeft.Size = new Size(138, 29);
            flowLayoutPanelResizingBottomLeft.TabIndex = 3;
            flowLayoutPanelResizingBottomLeft.WrapContents = false;
            // 
            // textBoxSettingsResizeHeight
            // 
            textBoxSettingsResizeHeight.Anchor = AnchorStyles.None;
            textBoxSettingsResizeHeight.Location = new Point(87, 3);
            textBoxSettingsResizeHeight.MaxLength = 4;
            textBoxSettingsResizeHeight.Name = "textBoxSettingsResizeHeight";
            textBoxSettingsResizeHeight.Size = new Size(48, 23);
            textBoxSettingsResizeHeight.TabIndex = 52;
            textBoxSettingsResizeHeight.KeyPress += TextBoxSettingsResizeHeight_KeyPress;
            textBoxSettingsResizeHeight.Leave += TextBoxSettingsResizeHeight_Leave;
            // 
            // labelSettingsResizeHeight
            // 
            labelSettingsResizeHeight.Anchor = AnchorStyles.None;
            labelSettingsResizeHeight.AutoSize = true;
            labelSettingsResizeHeight.Location = new Point(35, 7);
            labelSettingsResizeHeight.Name = "labelSettingsResizeHeight";
            labelSettingsResizeHeight.Size = new Size(46, 15);
            labelSettingsResizeHeight.TabIndex = 9;
            labelSettingsResizeHeight.Text = "Height:";
            // 
            // radioButtonSettingsCbz
            // 
            radioButtonSettingsCbz.AutoSize = true;
            radioButtonSettingsCbz.Checked = true;
            radioButtonSettingsCbz.Location = new Point(29, 3);
            radioButtonSettingsCbz.Name = "radioButtonSettingsCbz";
            radioButtonSettingsCbz.Size = new Size(47, 19);
            radioButtonSettingsCbz.TabIndex = 101;
            radioButtonSettingsCbz.TabStop = true;
            radioButtonSettingsCbz.Text = "CBZ";
            radioButtonSettingsCbz.UseVisualStyleBackColor = true;
            // 
            // radioButtonSettingsZip
            // 
            radioButtonSettingsZip.AutoSize = true;
            radioButtonSettingsZip.Location = new Point(29, 28);
            radioButtonSettingsZip.Name = "radioButtonSettingsZip";
            radioButtonSettingsZip.Size = new Size(42, 19);
            radioButtonSettingsZip.TabIndex = 102;
            radioButtonSettingsZip.Text = "ZIP";
            radioButtonSettingsZip.UseVisualStyleBackColor = true;
            // 
            // groupBoxCropping
            // 
            groupBoxCropping.AutoSize = true;
            groupBoxCropping.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBoxCropping.Controls.Add(tableLayoutPanelCropping);
            groupBoxCropping.Dock = DockStyle.Fill;
            groupBoxCropping.Location = new Point(3, 434);
            groupBoxCropping.Name = "groupBoxCropping";
            groupBoxCropping.Size = new Size(547, 63);
            groupBoxCropping.TabIndex = 206;
            groupBoxCropping.TabStop = false;
            groupBoxCropping.Text = "Cropping:";
            // 
            // tableLayoutPanelCropping
            // 
            tableLayoutPanelCropping.AutoSize = true;
            tableLayoutPanelCropping.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelCropping.ColumnCount = 3;
            tableLayoutPanelCropping.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33481F));
            tableLayoutPanelCropping.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33037F));
            tableLayoutPanelCropping.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33482F));
            tableLayoutPanelCropping.Controls.Add(flowLayoutPanelCroppingLeft, 0, 0);
            tableLayoutPanelCropping.Controls.Add(flowLayoutPanelCroppingCenter, 1, 0);
            tableLayoutPanelCropping.Controls.Add(flowLayoutPanelCroppingRight, 2, 0);
            tableLayoutPanelCropping.Dock = DockStyle.Fill;
            tableLayoutPanelCropping.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelCropping.Location = new Point(3, 19);
            tableLayoutPanelCropping.Name = "tableLayoutPanelCropping";
            tableLayoutPanelCropping.Padding = new Padding(3);
            tableLayoutPanelCropping.RowCount = 1;
            tableLayoutPanelCropping.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelCropping.Size = new Size(541, 41);
            tableLayoutPanelCropping.TabIndex = 74;
            // 
            // flowLayoutPanelCroppingLeft
            // 
            flowLayoutPanelCroppingLeft.AutoSize = true;
            flowLayoutPanelCroppingLeft.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelCroppingLeft.Controls.Add(labelSettingsCropPadding);
            flowLayoutPanelCroppingLeft.Controls.Add(textBoxSettingsCropPadding);
            flowLayoutPanelCroppingLeft.Dock = DockStyle.Fill;
            flowLayoutPanelCroppingLeft.Location = new Point(6, 6);
            flowLayoutPanelCroppingLeft.Name = "flowLayoutPanelCroppingLeft";
            flowLayoutPanelCroppingLeft.Size = new Size(172, 29);
            flowLayoutPanelCroppingLeft.TabIndex = 0;
            flowLayoutPanelCroppingLeft.WrapContents = false;
            // 
            // labelSettingsCropPadding
            // 
            labelSettingsCropPadding.Anchor = AnchorStyles.None;
            labelSettingsCropPadding.AutoSize = true;
            labelSettingsCropPadding.Location = new Point(3, 7);
            labelSettingsCropPadding.Name = "labelSettingsCropPadding";
            labelSettingsCropPadding.Size = new Size(54, 15);
            labelSettingsCropPadding.TabIndex = 14;
            labelSettingsCropPadding.Text = "Padding:";
            // 
            // textBoxSettingsCropPadding
            // 
            textBoxSettingsCropPadding.Anchor = AnchorStyles.None;
            textBoxSettingsCropPadding.Location = new Point(63, 3);
            textBoxSettingsCropPadding.MaxLength = 3;
            textBoxSettingsCropPadding.Name = "textBoxSettingsCropPadding";
            textBoxSettingsCropPadding.Size = new Size(34, 23);
            textBoxSettingsCropPadding.TabIndex = 71;
            textBoxSettingsCropPadding.Leave += TextBoxSettingsCropPadding_Leave;
            // 
            // flowLayoutPanelCroppingCenter
            // 
            flowLayoutPanelCroppingCenter.AutoSize = true;
            flowLayoutPanelCroppingCenter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelCroppingCenter.Controls.Add(labelSettingsCropColorTolerance);
            flowLayoutPanelCroppingCenter.Controls.Add(textBoxSettingsCropColorTolerance);
            flowLayoutPanelCroppingCenter.Dock = DockStyle.Fill;
            flowLayoutPanelCroppingCenter.Location = new Point(184, 6);
            flowLayoutPanelCroppingCenter.Name = "flowLayoutPanelCroppingCenter";
            flowLayoutPanelCroppingCenter.Size = new Size(172, 29);
            flowLayoutPanelCroppingCenter.TabIndex = 1;
            flowLayoutPanelCroppingCenter.WrapContents = false;
            // 
            // labelSettingsCropColorTolerance
            // 
            labelSettingsCropColorTolerance.Anchor = AnchorStyles.None;
            labelSettingsCropColorTolerance.AutoSize = true;
            labelSettingsCropColorTolerance.Location = new Point(3, 7);
            labelSettingsCropColorTolerance.Name = "labelSettingsCropColorTolerance";
            labelSettingsCropColorTolerance.Size = new Size(92, 15);
            labelSettingsCropColorTolerance.TabIndex = 15;
            labelSettingsCropColorTolerance.Text = "Color Tolerance:";
            // 
            // textBoxSettingsCropColorTolerance
            // 
            textBoxSettingsCropColorTolerance.Anchor = AnchorStyles.None;
            textBoxSettingsCropColorTolerance.Location = new Point(101, 3);
            textBoxSettingsCropColorTolerance.MaxLength = 3;
            textBoxSettingsCropColorTolerance.Name = "textBoxSettingsCropColorTolerance";
            textBoxSettingsCropColorTolerance.Size = new Size(34, 23);
            textBoxSettingsCropColorTolerance.TabIndex = 72;
            textBoxSettingsCropColorTolerance.Leave += TextBoxSettingsCropColorTolerance_Leave;
            // 
            // flowLayoutPanelCroppingRight
            // 
            flowLayoutPanelCroppingRight.AutoSize = true;
            flowLayoutPanelCroppingRight.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanelCroppingRight.Controls.Add(labelSettingsCropDeviationTolerance);
            flowLayoutPanelCroppingRight.Controls.Add(textBoxSettingsCropDeviationTolerance);
            flowLayoutPanelCroppingRight.Dock = DockStyle.Fill;
            flowLayoutPanelCroppingRight.Location = new Point(362, 6);
            flowLayoutPanelCroppingRight.Name = "flowLayoutPanelCroppingRight";
            flowLayoutPanelCroppingRight.Size = new Size(173, 29);
            flowLayoutPanelCroppingRight.TabIndex = 2;
            flowLayoutPanelCroppingRight.WrapContents = false;
            // 
            // labelSettingsCropDeviationTolerance
            // 
            labelSettingsCropDeviationTolerance.Anchor = AnchorStyles.None;
            labelSettingsCropDeviationTolerance.AutoSize = true;
            labelSettingsCropDeviationTolerance.Location = new Point(3, 7);
            labelSettingsCropDeviationTolerance.Name = "labelSettingsCropDeviationTolerance";
            labelSettingsCropDeviationTolerance.Size = new Size(90, 15);
            labelSettingsCropDeviationTolerance.TabIndex = 17;
            labelSettingsCropDeviationTolerance.Text = "Margin of Error:";
            // 
            // textBoxSettingsCropDeviationTolerance
            // 
            textBoxSettingsCropDeviationTolerance.Anchor = AnchorStyles.None;
            textBoxSettingsCropDeviationTolerance.Location = new Point(99, 3);
            textBoxSettingsCropDeviationTolerance.MaxLength = 3;
            textBoxSettingsCropDeviationTolerance.Name = "textBoxSettingsCropDeviationTolerance";
            textBoxSettingsCropDeviationTolerance.Size = new Size(35, 23);
            textBoxSettingsCropDeviationTolerance.TabIndex = 73;
            textBoxSettingsCropDeviationTolerance.Leave += TextBoxSettingsCropDeviationTolerance_Leave;
            // 
            // tableLayoutPanelSettings
            // 
            tableLayoutPanelSettings.AutoSize = true;
            tableLayoutPanelSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelSettings.ColumnCount = 1;
            tableLayoutPanelSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSettings.Controls.Add(tableLayoutPanelSettingsCenter, 0, 1);
            tableLayoutPanelSettings.Controls.Add(tableLayoutPanelSettingsBottom, 0, 2);
            tableLayoutPanelSettings.Controls.Add(tableLayoutPanel1, 0, 0);
            tableLayoutPanelSettings.Dock = DockStyle.Fill;
            tableLayoutPanelSettings.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelSettings.Location = new Point(0, 0);
            tableLayoutPanelSettings.Name = "tableLayoutPanelSettings";
            tableLayoutPanelSettings.Padding = new Padding(3);
            tableLayoutPanelSettings.RowCount = 3;
            tableLayoutPanelSettings.RowStyles.Add(new RowStyle());
            tableLayoutPanelSettings.RowStyles.Add(new RowStyle());
            tableLayoutPanelSettings.RowStyles.Add(new RowStyle());
            tableLayoutPanelSettings.Size = new Size(565, 610);
            tableLayoutPanelSettings.TabIndex = 207;
            // 
            // tableLayoutPanelSettingsCenter
            // 
            tableLayoutPanelSettingsCenter.AutoSize = true;
            tableLayoutPanelSettingsCenter.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelSettingsCenter.ColumnCount = 3;
            tableLayoutPanelSettingsCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanelSettingsCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanelSettingsCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanelSettingsCenter.Controls.Add(panelSettingsCbzZip, 1, 0);
            tableLayoutPanelSettingsCenter.Controls.Add(groupBoxCompressionLevel, 0, 0);
            tableLayoutPanelSettingsCenter.Controls.Add(groupBoxThreads, 2, 0);
            tableLayoutPanelSettingsCenter.Dock = DockStyle.Fill;
            tableLayoutPanelSettingsCenter.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelSettingsCenter.Location = new Point(6, 512);
            tableLayoutPanelSettingsCenter.Name = "tableLayoutPanelSettingsCenter";
            tableLayoutPanelSettingsCenter.RowCount = 1;
            tableLayoutPanelSettingsCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSettingsCenter.Size = new Size(553, 56);
            tableLayoutPanelSettingsCenter.TabIndex = 1;
            // 
            // panelSettingsCbzZip
            // 
            panelSettingsCbzZip.AutoSize = true;
            panelSettingsCbzZip.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelSettingsCbzZip.Controls.Add(radioButtonSettingsZip);
            panelSettingsCbzZip.Controls.Add(radioButtonSettingsCbz);
            panelSettingsCbzZip.Dock = DockStyle.Fill;
            panelSettingsCbzZip.Location = new Point(224, 3);
            panelSettingsCbzZip.Name = "panelSettingsCbzZip";
            panelSettingsCbzZip.Size = new Size(104, 50);
            panelSettingsCbzZip.TabIndex = 208;
            // 
            // tableLayoutPanelSettingsBottom
            // 
            tableLayoutPanelSettingsBottom.AutoSize = true;
            tableLayoutPanelSettingsBottom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanelSettingsBottom.ColumnCount = 4;
            tableLayoutPanelSettingsBottom.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelSettingsBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelSettingsBottom.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelSettingsBottom.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanelSettingsBottom.Controls.Add(buttonSettingsCheckForUpdate, 0, 0);
            tableLayoutPanelSettingsBottom.Controls.Add(buttonSettingsOK, 3, 0);
            tableLayoutPanelSettingsBottom.Controls.Add(buttonSettingsCancel, 2, 0);
            tableLayoutPanelSettingsBottom.Controls.Add(buttonSettingsResetToDefault, 1, 0);
            tableLayoutPanelSettingsBottom.Dock = DockStyle.Fill;
            tableLayoutPanelSettingsBottom.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanelSettingsBottom.Location = new Point(6, 574);
            tableLayoutPanelSettingsBottom.Name = "tableLayoutPanelSettingsBottom";
            tableLayoutPanelSettingsBottom.RowCount = 1;
            tableLayoutPanelSettingsBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelSettingsBottom.Size = new Size(553, 31);
            tableLayoutPanelSettingsBottom.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(groupBoxCropping, 0, 3);
            tableLayoutPanel1.Controls.Add(groupBoxResizing, 0, 2);
            tableLayoutPanel1.Controls.Add(groupBoxExperimental, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBoxComicInfo, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(6, 6);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(553, 500);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // PopupSettings
            // 
            AcceptButton = buttonSettingsOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            CancelButton = buttonSettingsCancel;
            ClientSize = new Size(565, 610);
            Controls.Add(tableLayoutPanelSettings);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PopupSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            Load += PopupSettings_Load;
            groupBoxComicInfo.ResumeLayout(false);
            groupBoxComicInfo.PerformLayout();
            tableLayoutPanelComicInfo.ResumeLayout(false);
            tableLayoutPanelComicInfo.PerformLayout();
            groupBoxExperimental.ResumeLayout(false);
            groupBoxExperimental.PerformLayout();
            tableLayoutPanelExperimental.ResumeLayout(false);
            tableLayoutPanelExperimental.PerformLayout();
            groupBoxCompressionLevel.ResumeLayout(false);
            groupBoxThreads.ResumeLayout(false);
            groupBoxResizing.ResumeLayout(false);
            groupBoxResizing.PerformLayout();
            tableLayoutPanelResizing.ResumeLayout(false);
            tableLayoutPanelResizing.PerformLayout();
            flowLayoutPanelResizingTopLeft.ResumeLayout(false);
            flowLayoutPanelResizingTopLeft.PerformLayout();
            flowLayoutPanelResizingTopRight.ResumeLayout(false);
            flowLayoutPanelResizingTopRight.PerformLayout();
            flowLayoutPanelResizingBottomRight.ResumeLayout(false);
            flowLayoutPanelResizingBottomRight.PerformLayout();
            flowLayoutPanelResizingBottomLeft.ResumeLayout(false);
            flowLayoutPanelResizingBottomLeft.PerformLayout();
            groupBoxCropping.ResumeLayout(false);
            groupBoxCropping.PerformLayout();
            tableLayoutPanelCropping.ResumeLayout(false);
            tableLayoutPanelCropping.PerformLayout();
            flowLayoutPanelCroppingLeft.ResumeLayout(false);
            flowLayoutPanelCroppingLeft.PerformLayout();
            flowLayoutPanelCroppingCenter.ResumeLayout(false);
            flowLayoutPanelCroppingCenter.PerformLayout();
            flowLayoutPanelCroppingRight.ResumeLayout(false);
            flowLayoutPanelCroppingRight.PerformLayout();
            tableLayoutPanelSettings.ResumeLayout(false);
            tableLayoutPanelSettings.PerformLayout();
            tableLayoutPanelSettingsCenter.ResumeLayout(false);
            tableLayoutPanelSettingsCenter.PerformLayout();
            panelSettingsCbzZip.ResumeLayout(false);
            panelSettingsCbzZip.PerformLayout();
            tableLayoutPanelSettingsBottom.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonSettingsOK;
        public Button buttonSettingsCancel;
        public CheckBox checkBoxSettingsComicInfoTitle;
        public CheckBox checkBoxSettingsComicInfoVolume;
        public CheckBox checkBoxSettingsComicInfoAuthors;
        public CheckBox checkBoxSettingsComicInfoLanguage;
        public CheckBox checkBoxSettingsComicInfoPublisher;
        public CheckBox checkBoxSettingsComicInfoDate;
        public CheckBox checkBoxSettingsComicInfoDescription;
        public GroupBox groupBoxComicInfo;
        public GroupBox groupBoxExperimental;
        public CheckBox checkBoxSettingsExperimentalRemoveDuplicateCovers;
        public CheckBox checkBoxSettingsExperimentalPageSpread;
        public ToolTip toolTipSettings;
        public CheckBox checkBoxSettingsExperimentalSpreadInsertBlank;
        public CheckBox checkBoxSettingsComicInfoReadingDirection;
        public CheckBox checkBoxSettingsComicInfoSeries;
        private CheckBox checkBoxSettingsExperimentalEveryPageIsChapter;
        private CheckBox checkBoxSettingsExperimentalSplitPageSpread;
        private GroupBox groupBoxCompressionLevel;
        private CheckBox checkBoxSettingsComicInfoPageCount;
        private CheckBox checkBoxSettingsComicInfoChapters;
        private GroupBox groupBoxThreads;
        public ComboBox dropDownThreads;
        private Button buttonSettingsCheckForUpdate;
        private CheckBox checkBoxSettingsExperimentalCoverResolution;
        private CheckBox checkBoxSettingsComicInfoTranslators;
        private CheckBox checkBoxSettingsComicInfoProducers;
        private CheckBox checkBoxSettingsComicInfoImageSize;
        private CheckBox checkBoxSettingsExperimentalAddBlankPage;
        private Button buttonSettingsResetToDefault;
        private CheckBox checkBoxSettingsExperimentalCheckDRMProtection;
        public ComboBox dropDownCompressionLevel;
        private GroupBox groupBoxResizing;
        private CheckBox checkBoxSettingsResizingEnable;
        public ComboBox dropDownKindle;
        public ComboBox dropDownKobo;
        private Label labelSettingsResizeHeight;
        private Label labelSettingsResizeWidth;
        private TextBox textBoxSettingsResizeHeight;
        private TextBox textBoxSettingsResizeWidth;
        private Label labelSettingsResizeKobo;
        private Label labelSettingsResizeKindle;
        private CheckBox checkBoxSettingsExperimentalMetadataTitle;
        private CheckBox checkBoxSettingsExperimentalChapterFolders;
        private RadioButton radioButtonSettingsCbz;
        private RadioButton radioButtonSettingsZip;
        private GroupBox groupBoxCropping;
        private Label labelSettingsCropColorTolerance;
        private CheckBox checkBoxSettingsCroppingEnable;
        private Label labelSettingsCropPadding;
        private TextBox textBoxSettingsCropColorTolerance;
        private TextBox textBoxSettingsCropPadding;
        private Label labelSettingsCropDeviationTolerance;
        private TextBox textBoxSettingsCropDeviationTolerance;
        private TextBox textBoxReplaceSeries;
        private CheckBox checkBoxSettingsReplaceSeries;
        public CheckBox checkBoxSettingsComicInfoIsbnAsin;
        private TableLayoutPanel tableLayoutPanelCropping;
        private FlowLayoutPanel flowLayoutPanelCroppingLeft;
        private FlowLayoutPanel flowLayoutPanelCroppingCenter;
        private FlowLayoutPanel flowLayoutPanelCroppingRight;
        private TableLayoutPanel tableLayoutPanelResizing;
        private FlowLayoutPanel flowLayoutPanelResizingTopLeft;
        private FlowLayoutPanel flowLayoutPanelResizingTopRight;
        private FlowLayoutPanel flowLayoutPanelResizingBottomRight;
        private FlowLayoutPanel flowLayoutPanelResizingBottomLeft;
        private TableLayoutPanel tableLayoutPanelComicInfo;
        private TableLayoutPanel tableLayoutPanelExperimental;
        private TableLayoutPanel tableLayoutPanelSettings;
        private TableLayoutPanel tableLayoutPanelSettingsCenter;
        private TableLayoutPanel tableLayoutPanelSettingsBottom;
        private Panel panelSettingsCbzZip;
        private TableLayoutPanel tableLayoutPanel1;
    }
}