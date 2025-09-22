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
            checkBoxSettingsComicInfoImageSize = new CheckBox();
            checkBoxSettingsComicInfoTranslators = new CheckBox();
            checkBoxSettingsComicInfoProducers = new CheckBox();
            checkBoxSettingsComicInfoChapters = new CheckBox();
            checkBoxSettingsComicInfoPageCount = new CheckBox();
            checkBoxSettingsComicInfoReadingDirection = new CheckBox();
            groupBoxExperimental = new GroupBox();
            textBoxReplaceSeries = new TextBox();
            checkBoxSettingsReplaceSeries = new CheckBox();
            checkBoxSettingsExperimentalChapterFolders = new CheckBox();
            checkBoxSettingsExperimentalMetadataTitle = new CheckBox();
            checkBoxSettingsExperimentalCheckDRMProtection = new CheckBox();
            checkBoxSettingsCroppingEnable = new CheckBox();
            checkBoxSettingsExperimentalAddBlankPage = new CheckBox();
            checkBoxSettingsExperimentalCoverResolution = new CheckBox();
            checkBoxSettingsExperimentalSplitPageSpread = new CheckBox();
            checkBoxSettingsExperimentalEveryPageIsChapter = new CheckBox();
            checkBoxSettingsExperimentalSpreadInsertBlank = new CheckBox();
            checkBoxSettingsResizingEnable = new CheckBox();
            checkBoxSettingsExperimentalPageSpread = new CheckBox();
            checkBoxSettingsExperimentalRemoveDuplicateCovers = new CheckBox();
            toolTipSettings = new ToolTip(components);
            dropDownCompressionLevel = new ComboBox();
            groupBoxCompressionLevel = new GroupBox();
            groupBoxThreads = new GroupBox();
            dropDownThreads = new ComboBox();
            buttonSettingsCheckForUpdate = new Button();
            buttonSettingsResetToDefault = new Button();
            groupBoxResizing = new GroupBox();
            labelSettingsResizeKobo = new Label();
            labelSettingsResizeKindle = new Label();
            labelSettingsResizeHeight = new Label();
            labelSettingsResizeWidth = new Label();
            textBoxSettingsResizeHeight = new TextBox();
            textBoxSettingsResizeWidth = new TextBox();
            dropDownKobo = new ComboBox();
            dropDownKindle = new ComboBox();
            radioButtonSettingsCbz = new RadioButton();
            radioButtonSettingsZip = new RadioButton();
            groupBoxCropping = new GroupBox();
            labelSettingsCropDeviationTolerance = new Label();
            textBoxSettingsCropDeviationTolerance = new TextBox();
            labelSettingsCropColorTolerance = new Label();
            labelSettingsCropPadding = new Label();
            textBoxSettingsCropColorTolerance = new TextBox();
            textBoxSettingsCropPadding = new TextBox();
            groupBoxComicInfo.SuspendLayout();
            groupBoxExperimental.SuspendLayout();
            groupBoxCompressionLevel.SuspendLayout();
            groupBoxThreads.SuspendLayout();
            groupBoxResizing.SuspendLayout();
            groupBoxCropping.SuspendLayout();
            SuspendLayout();
            // 
            // buttonSettingsOK
            // 
            buttonSettingsOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSettingsOK.Location = new Point(463, 484);
            buttonSettingsOK.Margin = new Padding(2, 1, 2, 1);
            buttonSettingsOK.Name = "buttonSettingsOK";
            buttonSettingsOK.Size = new Size(81, 25);
            buttonSettingsOK.TabIndex = 200;
            buttonSettingsOK.Text = "OK";
            buttonSettingsOK.UseVisualStyleBackColor = true;
            buttonSettingsOK.Click += BtnSettingsOK_Click;
            // 
            // buttonSettingsCancel
            // 
            buttonSettingsCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSettingsCancel.Location = new Point(378, 484);
            buttonSettingsCancel.Margin = new Padding(2, 1, 2, 1);
            buttonSettingsCancel.Name = "buttonSettingsCancel";
            buttonSettingsCancel.Size = new Size(81, 25);
            buttonSettingsCancel.TabIndex = 100;
            buttonSettingsCancel.Text = "Cancel";
            buttonSettingsCancel.UseVisualStyleBackColor = true;
            buttonSettingsCancel.Click += BtnSettingsCancel_Click;
            // 
            // checkBoxSettingsComicInfoSeries
            // 
            checkBoxSettingsComicInfoSeries.AutoSize = true;
            checkBoxSettingsComicInfoSeries.Location = new Point(95, 18);
            checkBoxSettingsComicInfoSeries.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoSeries.Name = "checkBoxSettingsComicInfoSeries";
            checkBoxSettingsComicInfoSeries.Size = new Size(56, 19);
            checkBoxSettingsComicInfoSeries.TabIndex = 2;
            checkBoxSettingsComicInfoSeries.Text = "Series";
            checkBoxSettingsComicInfoSeries.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoTitle
            // 
            checkBoxSettingsComicInfoTitle.AutoSize = true;
            checkBoxSettingsComicInfoTitle.Location = new Point(13, 18);
            checkBoxSettingsComicInfoTitle.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoTitle.Name = "checkBoxSettingsComicInfoTitle";
            checkBoxSettingsComicInfoTitle.Size = new Size(48, 19);
            checkBoxSettingsComicInfoTitle.TabIndex = 1;
            checkBoxSettingsComicInfoTitle.Text = "Title";
            checkBoxSettingsComicInfoTitle.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoVolume
            // 
            checkBoxSettingsComicInfoVolume.AutoSize = true;
            checkBoxSettingsComicInfoVolume.Location = new Point(196, 18);
            checkBoxSettingsComicInfoVolume.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoVolume.Name = "checkBoxSettingsComicInfoVolume";
            checkBoxSettingsComicInfoVolume.Size = new Size(66, 19);
            checkBoxSettingsComicInfoVolume.TabIndex = 3;
            checkBoxSettingsComicInfoVolume.Text = "Volume";
            checkBoxSettingsComicInfoVolume.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoAuthors
            // 
            checkBoxSettingsComicInfoAuthors.AutoSize = true;
            checkBoxSettingsComicInfoAuthors.Location = new Point(13, 39);
            checkBoxSettingsComicInfoAuthors.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoAuthors.Name = "checkBoxSettingsComicInfoAuthors";
            checkBoxSettingsComicInfoAuthors.Size = new Size(68, 19);
            checkBoxSettingsComicInfoAuthors.TabIndex = 6;
            checkBoxSettingsComicInfoAuthors.Text = "Authors";
            checkBoxSettingsComicInfoAuthors.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoLanguage
            // 
            checkBoxSettingsComicInfoLanguage.AutoSize = true;
            checkBoxSettingsComicInfoLanguage.Location = new Point(13, 60);
            checkBoxSettingsComicInfoLanguage.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoLanguage.Name = "checkBoxSettingsComicInfoLanguage";
            checkBoxSettingsComicInfoLanguage.Size = new Size(78, 19);
            checkBoxSettingsComicInfoLanguage.TabIndex = 11;
            checkBoxSettingsComicInfoLanguage.Text = "Language";
            checkBoxSettingsComicInfoLanguage.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoPublisher
            // 
            checkBoxSettingsComicInfoPublisher.AutoSize = true;
            checkBoxSettingsComicInfoPublisher.Location = new Point(292, 39);
            checkBoxSettingsComicInfoPublisher.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoPublisher.Name = "checkBoxSettingsComicInfoPublisher";
            checkBoxSettingsComicInfoPublisher.Size = new Size(75, 19);
            checkBoxSettingsComicInfoPublisher.TabIndex = 9;
            checkBoxSettingsComicInfoPublisher.Text = "Publisher";
            checkBoxSettingsComicInfoPublisher.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoDate
            // 
            checkBoxSettingsComicInfoDate.AutoSize = true;
            checkBoxSettingsComicInfoDate.Location = new Point(400, 18);
            checkBoxSettingsComicInfoDate.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoDate.Name = "checkBoxSettingsComicInfoDate";
            checkBoxSettingsComicInfoDate.Size = new Size(50, 19);
            checkBoxSettingsComicInfoDate.TabIndex = 5;
            checkBoxSettingsComicInfoDate.Text = "Date";
            checkBoxSettingsComicInfoDate.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoDescription
            // 
            checkBoxSettingsComicInfoDescription.AutoSize = true;
            checkBoxSettingsComicInfoDescription.Location = new Point(292, 18);
            checkBoxSettingsComicInfoDescription.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoDescription.Name = "checkBoxSettingsComicInfoDescription";
            checkBoxSettingsComicInfoDescription.Size = new Size(86, 19);
            checkBoxSettingsComicInfoDescription.TabIndex = 4;
            checkBoxSettingsComicInfoDescription.Text = "Description";
            checkBoxSettingsComicInfoDescription.UseVisualStyleBackColor = false;
            // 
            // groupBoxComicInfo
            // 
            groupBoxComicInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoImageSize);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoTranslators);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoProducers);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoChapters);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoPageCount);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoReadingDirection);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoSeries);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoDescription);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoTitle);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoDate);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoVolume);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoPublisher);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoAuthors);
            groupBoxComicInfo.Controls.Add(checkBoxSettingsComicInfoLanguage);
            groupBoxComicInfo.Location = new Point(17, 17);
            groupBoxComicInfo.Margin = new Padding(2, 1, 2, 1);
            groupBoxComicInfo.Name = "groupBoxComicInfo";
            groupBoxComicInfo.Padding = new Padding(2, 1, 2, 1);
            groupBoxComicInfo.Size = new Size(527, 89);
            groupBoxComicInfo.TabIndex = 0;
            groupBoxComicInfo.TabStop = false;
            groupBoxComicInfo.Text = "Info to include in ComicInfo.xml:";
            // 
            // checkBoxSettingsComicInfoImageSize
            // 
            checkBoxSettingsComicInfoImageSize.AutoSize = true;
            checkBoxSettingsComicInfoImageSize.Location = new Point(400, 60);
            checkBoxSettingsComicInfoImageSize.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoImageSize.Name = "checkBoxSettingsComicInfoImageSize";
            checkBoxSettingsComicInfoImageSize.Size = new Size(82, 19);
            checkBoxSettingsComicInfoImageSize.TabIndex = 14;
            checkBoxSettingsComicInfoImageSize.Text = "Image Size";
            checkBoxSettingsComicInfoImageSize.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoTranslators
            // 
            checkBoxSettingsComicInfoTranslators.AutoSize = true;
            checkBoxSettingsComicInfoTranslators.Location = new Point(196, 39);
            checkBoxSettingsComicInfoTranslators.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoTranslators.Name = "checkBoxSettingsComicInfoTranslators";
            checkBoxSettingsComicInfoTranslators.Size = new Size(82, 19);
            checkBoxSettingsComicInfoTranslators.TabIndex = 8;
            checkBoxSettingsComicInfoTranslators.Text = "Translators";
            checkBoxSettingsComicInfoTranslators.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoProducers
            // 
            checkBoxSettingsComicInfoProducers.AutoSize = true;
            checkBoxSettingsComicInfoProducers.Location = new Point(95, 39);
            checkBoxSettingsComicInfoProducers.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoProducers.Name = "checkBoxSettingsComicInfoProducers";
            checkBoxSettingsComicInfoProducers.Size = new Size(79, 19);
            checkBoxSettingsComicInfoProducers.TabIndex = 7;
            checkBoxSettingsComicInfoProducers.Text = "Producers";
            checkBoxSettingsComicInfoProducers.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoChapters
            // 
            checkBoxSettingsComicInfoChapters.AutoSize = true;
            checkBoxSettingsComicInfoChapters.Location = new Point(292, 60);
            checkBoxSettingsComicInfoChapters.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoChapters.Name = "checkBoxSettingsComicInfoChapters";
            checkBoxSettingsComicInfoChapters.Size = new Size(73, 19);
            checkBoxSettingsComicInfoChapters.TabIndex = 13;
            checkBoxSettingsComicInfoChapters.Text = "Chapters";
            checkBoxSettingsComicInfoChapters.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoPageCount
            // 
            checkBoxSettingsComicInfoPageCount.AutoSize = true;
            checkBoxSettingsComicInfoPageCount.Location = new Point(400, 39);
            checkBoxSettingsComicInfoPageCount.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoPageCount.Name = "checkBoxSettingsComicInfoPageCount";
            checkBoxSettingsComicInfoPageCount.Size = new Size(88, 19);
            checkBoxSettingsComicInfoPageCount.TabIndex = 10;
            checkBoxSettingsComicInfoPageCount.Text = "Page Count";
            checkBoxSettingsComicInfoPageCount.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsComicInfoReadingDirection
            // 
            checkBoxSettingsComicInfoReadingDirection.AutoSize = true;
            checkBoxSettingsComicInfoReadingDirection.Location = new Point(95, 60);
            checkBoxSettingsComicInfoReadingDirection.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsComicInfoReadingDirection.Name = "checkBoxSettingsComicInfoReadingDirection";
            checkBoxSettingsComicInfoReadingDirection.Size = new Size(120, 19);
            checkBoxSettingsComicInfoReadingDirection.TabIndex = 12;
            checkBoxSettingsComicInfoReadingDirection.Text = "Reading Direction";
            checkBoxSettingsComicInfoReadingDirection.UseVisualStyleBackColor = false;
            // 
            // groupBoxExperimental
            // 
            groupBoxExperimental.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxExperimental.Controls.Add(textBoxReplaceSeries);
            groupBoxExperimental.Controls.Add(checkBoxSettingsReplaceSeries);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalChapterFolders);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalMetadataTitle);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalCheckDRMProtection);
            groupBoxExperimental.Controls.Add(checkBoxSettingsCroppingEnable);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalAddBlankPage);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalCoverResolution);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalSplitPageSpread);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalEveryPageIsChapter);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalSpreadInsertBlank);
            groupBoxExperimental.Controls.Add(checkBoxSettingsResizingEnable);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalPageSpread);
            groupBoxExperimental.Controls.Add(checkBoxSettingsExperimentalRemoveDuplicateCovers);
            groupBoxExperimental.Location = new Point(17, 108);
            groupBoxExperimental.Margin = new Padding(2, 1, 2, 1);
            groupBoxExperimental.Name = "groupBoxExperimental";
            groupBoxExperimental.Padding = new Padding(2, 1, 2, 1);
            groupBoxExperimental.Size = new Size(527, 169);
            groupBoxExperimental.TabIndex = 20;
            groupBoxExperimental.TabStop = false;
            groupBoxExperimental.Text = "Experimental Features:";
            // 
            // textBoxReplaceSeries
            // 
            textBoxReplaceSeries.Location = new Point(248, 144);
            textBoxReplaceSeries.Margin = new Padding(2, 1, 2, 1);
            textBoxReplaceSeries.Name = "textBoxReplaceSeries";
            textBoxReplaceSeries.Size = new Size(275, 23);
            textBoxReplaceSeries.TabIndex = 32;
            // 
            // checkBoxSettingsReplaceSeries
            // 
            checkBoxSettingsReplaceSeries.AutoSize = true;
            checkBoxSettingsReplaceSeries.Location = new Point(13, 144);
            checkBoxSettingsReplaceSeries.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsReplaceSeries.Name = "checkBoxSettingsReplaceSeries";
            checkBoxSettingsReplaceSeries.Size = new Size(135, 19);
            checkBoxSettingsReplaceSeries.TabIndex = 31;
            checkBoxSettingsReplaceSeries.Text = "Replace Series Name";
            checkBoxSettingsReplaceSeries.UseVisualStyleBackColor = false;
            checkBoxSettingsReplaceSeries.CheckedChanged += CheckBoxSettingsReplaceSeries_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalChapterFolders
            // 
            checkBoxSettingsExperimentalChapterFolders.AutoSize = true;
            checkBoxSettingsExperimentalChapterFolders.Location = new Point(271, 102);
            checkBoxSettingsExperimentalChapterFolders.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalChapterFolders.Name = "checkBoxSettingsExperimentalChapterFolders";
            checkBoxSettingsExperimentalChapterFolders.Size = new Size(192, 19);
            checkBoxSettingsExperimentalChapterFolders.TabIndex = 30;
            checkBoxSettingsExperimentalChapterFolders.Text = "Create Folders for each Chapter";
            checkBoxSettingsExperimentalChapterFolders.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalMetadataTitle
            // 
            checkBoxSettingsExperimentalMetadataTitle.AutoSize = true;
            checkBoxSettingsExperimentalMetadataTitle.Location = new Point(13, 102);
            checkBoxSettingsExperimentalMetadataTitle.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalMetadataTitle.Name = "checkBoxSettingsExperimentalMetadataTitle";
            checkBoxSettingsExperimentalMetadataTitle.Size = new Size(123, 19);
            checkBoxSettingsExperimentalMetadataTitle.TabIndex = 29;
            checkBoxSettingsExperimentalMetadataTitle.Text = "Use metadata Title";
            checkBoxSettingsExperimentalMetadataTitle.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalCheckDRMProtection
            // 
            checkBoxSettingsExperimentalCheckDRMProtection.AutoSize = true;
            checkBoxSettingsExperimentalCheckDRMProtection.Location = new Point(271, 81);
            checkBoxSettingsExperimentalCheckDRMProtection.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalCheckDRMProtection.Name = "checkBoxSettingsExperimentalCheckDRMProtection";
            checkBoxSettingsExperimentalCheckDRMProtection.Size = new Size(195, 19);
            checkBoxSettingsExperimentalCheckDRMProtection.TabIndex = 28;
            checkBoxSettingsExperimentalCheckDRMProtection.Text = "Check if files are DRM protected";
            checkBoxSettingsExperimentalCheckDRMProtection.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsCroppingEnable
            // 
            checkBoxSettingsCroppingEnable.AutoSize = true;
            checkBoxSettingsCroppingEnable.Location = new Point(271, 123);
            checkBoxSettingsCroppingEnable.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsCroppingEnable.Name = "checkBoxSettingsCroppingEnable";
            checkBoxSettingsCroppingEnable.Size = new Size(93, 19);
            checkBoxSettingsCroppingEnable.TabIndex = 0;
            checkBoxSettingsCroppingEnable.Text = "Crop Images";
            checkBoxSettingsCroppingEnable.UseVisualStyleBackColor = true;
            checkBoxSettingsCroppingEnable.CheckedChanged += CheckBoxSettingsCroppingEnable_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalAddBlankPage
            // 
            checkBoxSettingsExperimentalAddBlankPage.AutoSize = true;
            checkBoxSettingsExperimentalAddBlankPage.Location = new Point(13, 81);
            checkBoxSettingsExperimentalAddBlankPage.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalAddBlankPage.Name = "checkBoxSettingsExperimentalAddBlankPage";
            checkBoxSettingsExperimentalAddBlankPage.Size = new Size(109, 19);
            checkBoxSettingsExperimentalAddBlankPage.TabIndex = 27;
            checkBoxSettingsExperimentalAddBlankPage.Text = "Add Blank Page";
            checkBoxSettingsExperimentalAddBlankPage.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalCoverResolution
            // 
            checkBoxSettingsExperimentalCoverResolution.AutoSize = true;
            checkBoxSettingsExperimentalCoverResolution.Location = new Point(271, 39);
            checkBoxSettingsExperimentalCoverResolution.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalCoverResolution.Name = "checkBoxSettingsExperimentalCoverResolution";
            checkBoxSettingsExperimentalCoverResolution.Size = new Size(179, 19);
            checkBoxSettingsExperimentalCoverResolution.TabIndex = 24;
            checkBoxSettingsExperimentalCoverResolution.Text = "Keep higher resolution Cover";
            checkBoxSettingsExperimentalCoverResolution.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalSplitPageSpread
            // 
            checkBoxSettingsExperimentalSplitPageSpread.AutoSize = true;
            checkBoxSettingsExperimentalSplitPageSpread.Location = new Point(271, 60);
            checkBoxSettingsExperimentalSplitPageSpread.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalSplitPageSpread.Name = "checkBoxSettingsExperimentalSplitPageSpread";
            checkBoxSettingsExperimentalSplitPageSpread.Size = new Size(118, 19);
            checkBoxSettingsExperimentalSplitPageSpread.TabIndex = 26;
            checkBoxSettingsExperimentalSplitPageSpread.Text = "Split wide images";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSplitPageSpread, "Split Page Spreads that are one wide image into two single page images.");
            checkBoxSettingsExperimentalSplitPageSpread.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalEveryPageIsChapter
            // 
            checkBoxSettingsExperimentalEveryPageIsChapter.AutoSize = true;
            checkBoxSettingsExperimentalEveryPageIsChapter.Location = new Point(13, 60);
            checkBoxSettingsExperimentalEveryPageIsChapter.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalEveryPageIsChapter.Name = "checkBoxSettingsExperimentalEveryPageIsChapter";
            checkBoxSettingsExperimentalEveryPageIsChapter.Size = new Size(188, 19);
            checkBoxSettingsExperimentalEveryPageIsChapter.TabIndex = 25;
            checkBoxSettingsExperimentalEveryPageIsChapter.Text = "Fix every Page being a Chapter";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalEveryPageIsChapter, "Some books can have every page as a separate chapter. This removes all of them for a cleaner look. (Can happen with books bought on Amazon)");
            checkBoxSettingsExperimentalEveryPageIsChapter.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsExperimentalSpreadInsertBlank
            // 
            checkBoxSettingsExperimentalSpreadInsertBlank.AutoSize = true;
            checkBoxSettingsExperimentalSpreadInsertBlank.Location = new Point(271, 18);
            checkBoxSettingsExperimentalSpreadInsertBlank.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalSpreadInsertBlank.Name = "checkBoxSettingsExperimentalSpreadInsertBlank";
            checkBoxSettingsExperimentalSpreadInsertBlank.Size = new Size(133, 19);
            checkBoxSettingsExperimentalSpreadInsertBlank.TabIndex = 22;
            checkBoxSettingsExperimentalSpreadInsertBlank.Text = "Blank Page behavior";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSpreadInsertBlank, "When preserving Page Spread and a Blank Image would have to be inserted to do so, check if a Blank Image is following and if it is, delete that one instead of having 2 Blank Images back to back.");
            checkBoxSettingsExperimentalSpreadInsertBlank.UseVisualStyleBackColor = false;
            // 
            // checkBoxSettingsResizingEnable
            // 
            checkBoxSettingsResizingEnable.AutoSize = true;
            checkBoxSettingsResizingEnable.Location = new Point(13, 123);
            checkBoxSettingsResizingEnable.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsResizingEnable.Name = "checkBoxSettingsResizingEnable";
            checkBoxSettingsResizingEnable.Size = new Size(99, 19);
            checkBoxSettingsResizingEnable.TabIndex = 0;
            checkBoxSettingsResizingEnable.Text = "Resize Images";
            checkBoxSettingsResizingEnable.UseVisualStyleBackColor = false;
            checkBoxSettingsResizingEnable.CheckedChanged += CheckBoxSettingsResizingEnable_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalPageSpread
            // 
            checkBoxSettingsExperimentalPageSpread.AutoSize = true;
            checkBoxSettingsExperimentalPageSpread.Location = new Point(13, 18);
            checkBoxSettingsExperimentalPageSpread.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalPageSpread.Name = "checkBoxSettingsExperimentalPageSpread";
            checkBoxSettingsExperimentalPageSpread.Size = new Size(227, 19);
            checkBoxSettingsExperimentalPageSpread.TabIndex = 21;
            checkBoxSettingsExperimentalPageSpread.Text = "Try to preserve Page Spread alignment";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalPageSpread, "Tries to preserve correct Page Spread alignment, so a right page will stay a right page and a left page will stay a left page. Assumes Epub has set this info correctly.");
            checkBoxSettingsExperimentalPageSpread.UseVisualStyleBackColor = false;
            checkBoxSettingsExperimentalPageSpread.CheckedChanged += CheckBoxSettingsPageSpread_CheckedChanged;
            // 
            // checkBoxSettingsExperimentalRemoveDuplicateCovers
            // 
            checkBoxSettingsExperimentalRemoveDuplicateCovers.AutoSize = true;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Location = new Point(13, 39);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Margin = new Padding(2, 1, 2, 1);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Name = "checkBoxSettingsExperimentalRemoveDuplicateCovers";
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Size = new Size(160, 19);
            checkBoxSettingsExperimentalRemoveDuplicateCovers.TabIndex = 23;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Text = "Remove duplicate Covers";
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalRemoveDuplicateCovers, "Removes duplicate Cover if it is displayed as the first page again. (Can happen with books bought on Amazon)");
            checkBoxSettingsExperimentalRemoveDuplicateCovers.UseVisualStyleBackColor = false;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.CheckedChanged += CheckBoxSettingsExperimentalRemoveDuplicateCovers_CheckedChanged;
            // 
            // dropDownCompressionLevel
            // 
            dropDownCompressionLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownCompressionLevel.FormattingEnabled = true;
            dropDownCompressionLevel.Items.AddRange(new object[] { "No Compression", "Fastest", "Optimal", "Smallest Size" });
            dropDownCompressionLevel.Location = new Point(13, 20);
            dropDownCompressionLevel.Margin = new Padding(2, 1, 2, 1);
            dropDownCompressionLevel.Name = "dropDownCompressionLevel";
            dropDownCompressionLevel.Size = new Size(174, 23);
            dropDownCompressionLevel.TabIndex = 55;
            toolTipSettings.SetToolTip(dropDownCompressionLevel, "'No Compression' is fastest and recommended, as the images are already in a compressed format.");
            // 
            // groupBoxCompressionLevel
            // 
            groupBoxCompressionLevel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxCompressionLevel.Controls.Add(dropDownCompressionLevel);
            groupBoxCompressionLevel.Location = new Point(17, 424);
            groupBoxCompressionLevel.Margin = new Padding(2, 1, 2, 1);
            groupBoxCompressionLevel.Name = "groupBoxCompressionLevel";
            groupBoxCompressionLevel.Padding = new Padding(2, 1, 2, 1);
            groupBoxCompressionLevel.Size = new Size(212, 54);
            groupBoxCompressionLevel.TabIndex = 50;
            groupBoxCompressionLevel.TabStop = false;
            groupBoxCompressionLevel.Text = "CBZ Compression Level:";
            // 
            // groupBoxThreads
            // 
            groupBoxThreads.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBoxThreads.Controls.Add(dropDownThreads);
            groupBoxThreads.Location = new Point(332, 424);
            groupBoxThreads.Margin = new Padding(2, 1, 2, 1);
            groupBoxThreads.Name = "groupBoxThreads";
            groupBoxThreads.Padding = new Padding(2, 1, 2, 1);
            groupBoxThreads.Size = new Size(212, 54);
            groupBoxThreads.TabIndex = 60;
            groupBoxThreads.TabStop = false;
            groupBoxThreads.Text = "Threads";
            // 
            // dropDownThreads
            // 
            dropDownThreads.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownThreads.FormattingEnabled = true;
            dropDownThreads.Location = new Point(13, 20);
            dropDownThreads.Margin = new Padding(2, 1, 2, 1);
            dropDownThreads.Name = "dropDownThreads";
            dropDownThreads.Size = new Size(174, 23);
            dropDownThreads.TabIndex = 65;
            // 
            // buttonSettingsCheckForUpdate
            // 
            buttonSettingsCheckForUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonSettingsCheckForUpdate.Location = new Point(17, 484);
            buttonSettingsCheckForUpdate.Name = "buttonSettingsCheckForUpdate";
            buttonSettingsCheckForUpdate.Size = new Size(81, 25);
            buttonSettingsCheckForUpdate.TabIndex = 90;
            buttonSettingsCheckForUpdate.Text = "Update";
            buttonSettingsCheckForUpdate.UseVisualStyleBackColor = true;
            buttonSettingsCheckForUpdate.Click += ButtonSettingsCheckForUpdate_Click;
            // 
            // buttonSettingsResetToDefault
            // 
            buttonSettingsResetToDefault.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonSettingsResetToDefault.Location = new Point(202, 484);
            buttonSettingsResetToDefault.Name = "buttonSettingsResetToDefault";
            buttonSettingsResetToDefault.Size = new Size(93, 25);
            buttonSettingsResetToDefault.TabIndex = 202;
            buttonSettingsResetToDefault.Text = "Reset Settings";
            buttonSettingsResetToDefault.UseVisualStyleBackColor = true;
            buttonSettingsResetToDefault.Click += ButtonSettingsResetToDefault_Click;
            // 
            // groupBoxResizing
            // 
            groupBoxResizing.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxResizing.Controls.Add(labelSettingsResizeKobo);
            groupBoxResizing.Controls.Add(labelSettingsResizeKindle);
            groupBoxResizing.Controls.Add(labelSettingsResizeHeight);
            groupBoxResizing.Controls.Add(labelSettingsResizeWidth);
            groupBoxResizing.Controls.Add(textBoxSettingsResizeHeight);
            groupBoxResizing.Controls.Add(textBoxSettingsResizeWidth);
            groupBoxResizing.Controls.Add(dropDownKobo);
            groupBoxResizing.Controls.Add(dropDownKindle);
            groupBoxResizing.Location = new Point(17, 279);
            groupBoxResizing.Margin = new Padding(2, 1, 2, 1);
            groupBoxResizing.Name = "groupBoxResizing";
            groupBoxResizing.Padding = new Padding(2, 1, 2, 1);
            groupBoxResizing.Size = new Size(527, 82);
            groupBoxResizing.TabIndex = 203;
            groupBoxResizing.TabStop = false;
            groupBoxResizing.Text = "Resizing:";
            // 
            // labelSettingsResizeKobo
            // 
            labelSettingsResizeKobo.AutoSize = true;
            labelSettingsResizeKobo.Location = new Point(196, 51);
            labelSettingsResizeKobo.Name = "labelSettingsResizeKobo";
            labelSettingsResizeKobo.Size = new Size(38, 15);
            labelSettingsResizeKobo.TabIndex = 11;
            labelSettingsResizeKobo.Text = "Kobo:";
            // 
            // labelSettingsResizeKindle
            // 
            labelSettingsResizeKindle.AutoSize = true;
            labelSettingsResizeKindle.Location = new Point(196, 22);
            labelSettingsResizeKindle.Name = "labelSettingsResizeKindle";
            labelSettingsResizeKindle.Size = new Size(43, 15);
            labelSettingsResizeKindle.TabIndex = 10;
            labelSettingsResizeKindle.Text = "Kindle:";
            // 
            // labelSettingsResizeHeight
            // 
            labelSettingsResizeHeight.AutoSize = true;
            labelSettingsResizeHeight.Location = new Point(35, 51);
            labelSettingsResizeHeight.Name = "labelSettingsResizeHeight";
            labelSettingsResizeHeight.Size = new Size(46, 15);
            labelSettingsResizeHeight.TabIndex = 9;
            labelSettingsResizeHeight.Text = "Height:";
            // 
            // labelSettingsResizeWidth
            // 
            labelSettingsResizeWidth.AutoSize = true;
            labelSettingsResizeWidth.Location = new Point(35, 22);
            labelSettingsResizeWidth.Name = "labelSettingsResizeWidth";
            labelSettingsResizeWidth.Size = new Size(42, 15);
            labelSettingsResizeWidth.TabIndex = 8;
            labelSettingsResizeWidth.Text = "Width:";
            // 
            // textBoxSettingsResizeHeight
            // 
            textBoxSettingsResizeHeight.Location = new Point(88, 48);
            textBoxSettingsResizeHeight.MaxLength = 4;
            textBoxSettingsResizeHeight.Name = "textBoxSettingsResizeHeight";
            textBoxSettingsResizeHeight.Size = new Size(48, 23);
            textBoxSettingsResizeHeight.TabIndex = 7;
            textBoxSettingsResizeHeight.KeyPress += TextBoxSettingsResizeHeight_KeyPress;
            textBoxSettingsResizeHeight.Leave += TextBoxSettingsResizeHeight_Leave;
            // 
            // textBoxSettingsResizeWidth
            // 
            textBoxSettingsResizeWidth.Location = new Point(88, 19);
            textBoxSettingsResizeWidth.MaxLength = 4;
            textBoxSettingsResizeWidth.Name = "textBoxSettingsResizeWidth";
            textBoxSettingsResizeWidth.Size = new Size(48, 23);
            textBoxSettingsResizeWidth.TabIndex = 6;
            textBoxSettingsResizeWidth.KeyPress += TextBoxSettingsResizeWidth_KeyPress;
            textBoxSettingsResizeWidth.Leave += TextBoxSettingsResizeWidth_Leave;
            // 
            // dropDownKobo
            // 
            dropDownKobo.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownKobo.FormattingEnabled = true;
            dropDownKobo.Items.AddRange(new object[] { "Kobo Mini/Touch", "Kobo Glo", "Kobo Glo HD/Clara HD/Clara 2E/Clara Colour", "Kobo Aura/Nia", "Kobo Aura HD", "Kobo Aura H2O", "Kobo Elipsa/Aura ONE", "Kobo Libra H2O/Libra 2/Libra Colour", "Kobo Forma/Sage" });
            dropDownKobo.Location = new Point(246, 48);
            dropDownKobo.Name = "dropDownKobo";
            dropDownKobo.Size = new Size(275, 23);
            dropDownKobo.TabIndex = 2;
            dropDownKobo.SelectedIndexChanged += DropDownKobo_SelectedIndexChanged;
            // 
            // dropDownKindle
            // 
            dropDownKindle.DropDownStyle = ComboBoxStyle.DropDownList;
            dropDownKindle.FormattingEnabled = true;
            dropDownKindle.Items.AddRange(new object[] { "Kindle 1/2", "Kindle DX/DXG", "Kindle 5-10/Keyboard/Touch", "Kindle Paperwhite 1/2", "Kindle 11/Voyage/Oasis", "Kindle Paperwhite 3/4", "Kindle Oasis 2/3/Paperwhite 12/Colorsoft 12", "Kindle Paperwhite 5/Signature Edition", "Kindle Scribe" });
            dropDownKindle.Location = new Point(246, 19);
            dropDownKindle.Name = "dropDownKindle";
            dropDownKindle.Size = new Size(275, 23);
            dropDownKindle.TabIndex = 1;
            dropDownKindle.SelectedIndexChanged += DropDownKindle_SelectedIndexChanged;
            // 
            // radioButtonSettingsCbz
            // 
            radioButtonSettingsCbz.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            radioButtonSettingsCbz.AutoSize = true;
            radioButtonSettingsCbz.Checked = true;
            radioButtonSettingsCbz.Location = new Point(260, 430);
            radioButtonSettingsCbz.Name = "radioButtonSettingsCbz";
            radioButtonSettingsCbz.Size = new Size(47, 19);
            radioButtonSettingsCbz.TabIndex = 204;
            radioButtonSettingsCbz.TabStop = true;
            radioButtonSettingsCbz.Text = "CBZ";
            radioButtonSettingsCbz.UseVisualStyleBackColor = true;
            // 
            // radioButtonSettingsZip
            // 
            radioButtonSettingsZip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            radioButtonSettingsZip.AutoSize = true;
            radioButtonSettingsZip.Location = new Point(260, 455);
            radioButtonSettingsZip.Name = "radioButtonSettingsZip";
            radioButtonSettingsZip.Size = new Size(42, 19);
            radioButtonSettingsZip.TabIndex = 205;
            radioButtonSettingsZip.Text = "ZIP";
            radioButtonSettingsZip.UseVisualStyleBackColor = true;
            // 
            // groupBoxCropping
            // 
            groupBoxCropping.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxCropping.Controls.Add(labelSettingsCropDeviationTolerance);
            groupBoxCropping.Controls.Add(textBoxSettingsCropDeviationTolerance);
            groupBoxCropping.Controls.Add(labelSettingsCropColorTolerance);
            groupBoxCropping.Controls.Add(labelSettingsCropPadding);
            groupBoxCropping.Controls.Add(textBoxSettingsCropColorTolerance);
            groupBoxCropping.Controls.Add(textBoxSettingsCropPadding);
            groupBoxCropping.Location = new Point(17, 363);
            groupBoxCropping.Margin = new Padding(2, 1, 2, 1);
            groupBoxCropping.Name = "groupBoxCropping";
            groupBoxCropping.Padding = new Padding(2, 1, 2, 1);
            groupBoxCropping.Size = new Size(527, 56);
            groupBoxCropping.TabIndex = 206;
            groupBoxCropping.TabStop = false;
            groupBoxCropping.Text = "Cropping:";
            // 
            // labelSettingsCropDeviationTolerance
            // 
            labelSettingsCropDeviationTolerance.AutoSize = true;
            labelSettingsCropDeviationTolerance.Location = new Point(352, 24);
            labelSettingsCropDeviationTolerance.Name = "labelSettingsCropDeviationTolerance";
            labelSettingsCropDeviationTolerance.Size = new Size(90, 15);
            labelSettingsCropDeviationTolerance.TabIndex = 17;
            labelSettingsCropDeviationTolerance.Text = "Margin of Error:";
            labelSettingsCropDeviationTolerance.SizeChanged += LabelSettingsCropDeviationTolerance_SizeChanged;
            // 
            // textBoxSettingsCropDeviationTolerance
            // 
            textBoxSettingsCropDeviationTolerance.Location = new Point(448, 21);
            textBoxSettingsCropDeviationTolerance.MaxLength = 3;
            textBoxSettingsCropDeviationTolerance.Name = "textBoxSettingsCropDeviationTolerance";
            textBoxSettingsCropDeviationTolerance.Size = new Size(40, 23);
            textBoxSettingsCropDeviationTolerance.TabIndex = 16;
            textBoxSettingsCropDeviationTolerance.Leave += TextBoxSettingsCropDeviationTolerance_Leave;
            // 
            // labelSettingsCropColorTolerance
            // 
            labelSettingsCropColorTolerance.AutoSize = true;
            labelSettingsCropColorTolerance.Location = new Point(173, 24);
            labelSettingsCropColorTolerance.Name = "labelSettingsCropColorTolerance";
            labelSettingsCropColorTolerance.Size = new Size(92, 15);
            labelSettingsCropColorTolerance.TabIndex = 15;
            labelSettingsCropColorTolerance.Text = "Color Tolerance:";
            labelSettingsCropColorTolerance.SizeChanged += LabelSettingsCropColorTolerance_SizeChanged;
            // 
            // labelSettingsCropPadding
            // 
            labelSettingsCropPadding.AutoSize = true;
            labelSettingsCropPadding.Location = new Point(13, 24);
            labelSettingsCropPadding.Name = "labelSettingsCropPadding";
            labelSettingsCropPadding.Size = new Size(54, 15);
            labelSettingsCropPadding.TabIndex = 14;
            labelSettingsCropPadding.Text = "Padding:";
            labelSettingsCropPadding.SizeChanged += LabelSettingsCropPadding_SizeChanged;
            // 
            // textBoxSettingsCropColorTolerance
            // 
            textBoxSettingsCropColorTolerance.Location = new Point(271, 21);
            textBoxSettingsCropColorTolerance.MaxLength = 3;
            textBoxSettingsCropColorTolerance.Name = "textBoxSettingsCropColorTolerance";
            textBoxSettingsCropColorTolerance.Size = new Size(40, 23);
            textBoxSettingsCropColorTolerance.TabIndex = 13;
            textBoxSettingsCropColorTolerance.Leave += TextBoxSettingsCropColorTolerance_Leave;
            // 
            // textBoxSettingsCropPadding
            // 
            textBoxSettingsCropPadding.Location = new Point(73, 21);
            textBoxSettingsCropPadding.MaxLength = 3;
            textBoxSettingsCropPadding.Name = "textBoxSettingsCropPadding";
            textBoxSettingsCropPadding.Size = new Size(40, 23);
            textBoxSettingsCropPadding.TabIndex = 12;
            textBoxSettingsCropPadding.Leave += TextBoxSettingsCropPadding_Leave;
            // 
            // PopupSettings
            // 
            AcceptButton = buttonSettingsOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonSettingsCancel;
            ClientSize = new Size(561, 521);
            Controls.Add(groupBoxCropping);
            Controls.Add(radioButtonSettingsZip);
            Controls.Add(radioButtonSettingsCbz);
            Controls.Add(groupBoxResizing);
            Controls.Add(buttonSettingsResetToDefault);
            Controls.Add(buttonSettingsCheckForUpdate);
            Controls.Add(groupBoxThreads);
            Controls.Add(groupBoxCompressionLevel);
            Controls.Add(groupBoxExperimental);
            Controls.Add(groupBoxComicInfo);
            Controls.Add(buttonSettingsCancel);
            Controls.Add(buttonSettingsOK);
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
            groupBoxExperimental.ResumeLayout(false);
            groupBoxExperimental.PerformLayout();
            groupBoxCompressionLevel.ResumeLayout(false);
            groupBoxThreads.ResumeLayout(false);
            groupBoxResizing.ResumeLayout(false);
            groupBoxResizing.PerformLayout();
            groupBoxCropping.ResumeLayout(false);
            groupBoxCropping.PerformLayout();
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
    }
}