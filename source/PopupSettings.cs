using epub2cbz_gui.Properties;

namespace epub2cbz_gui
{
    public partial class PopupSettings : Form
    {
        public static class VersionNumbers
        {
            public static string NewestVersion { get; set; } = string.Empty;
            public static string CurrentVersion { get; set; } = string.Empty;
        }

        public static class CheckboxStates
        {
            public static bool CheckboxSeriesState { get; set; } = true;
            public static bool CheckboxTitleState { get; set; } = true;
            public static bool CheckboxVolumeState { get; set; } = true;
            public static bool CheckboxPageCountState { get; set; } = true;
            public static bool CheckboxAuthorState { get; set; } = true;
            public static bool CheckboxProducerState { get; set; } = true;
            public static bool CheckboxTranslatorState { get; set; } = true;
            public static bool CheckboxLanguageState { get; set; } = true;
            public static bool CheckboxPublisherState { get; set; } = true;
            public static bool CheckboxDateState { get; set; } = true;
            public static bool CheckboxDescriptionState { get; set; } = true;
            public static bool CheckboxReadingDirectionState { get; set; } = true;
            public static bool CheckboxIsbnAsinState { get; set; } = true;
            public static bool CheckboxChaptersState { get; set; } = true;
            public static bool CheckboxImageSizeState { get; set; } = true;

            public static bool CheckboxDuplicateCoverState { get; set; } = true;
            public static bool CheckboxEveryPageIsChapterState { get; set; } = true;
            public static bool CheckboxPageSpreadState { get; set; } = true;
            public static bool CheckboxSplitPageSpreadState { get; set; } = true;
            public static bool CheckboxBlankImageState { get; set; } = true;
            public static bool CheckboxHigherResolutionCover { get; set; } = true;
            public static bool CheckboxInsertAdditionalBlankImageState { get; set; } = false;
            public static bool CheckboxMetadataTitleState { get; set; } = false;
            public static bool CheckboxChapterFoldersState { get; set; } = false;

            public static bool CheckboxReplaceSeriesState { get; set; } = false;
            public static string? TextboxReplaceSeriesState { get; set; } = string.Empty;

            public static bool RadioButtonZipState { get; set; } = false;

            public static int DropDownCompressionLevelState { get; set; } = 0;
            public static int DropDownParallelismDegreeState { get; set; } = 0;

            public static bool CheckboxDRMProtectionState { get; set; } = true;

            public static bool ButtonResetSettingsState { get; set; } = false;



            public static bool CheckboxSimpleExtractionState { get; set; } = false;
            public static bool CheckboxFileModeState { get; set; } = false;



            public static bool CheckboxResizeImagesState { get; set; } = false;
            public static int TextBoxResizeWidthValue { get; set; } = 0;
            public static int TextBoxResizeHeightValue { get; set; } = 0;

            public static bool CheckboxCropImagesState { get; set; } = false;
            public static int TextBoxCropDeviationToleranceValue { get; set; } = 1;
            public static int TextBoxCropPaddingValue { get; set; } = 5;
            public static byte TextBoxCropColorToleranceValue { get; set; } = 15;
        }

        public PopupSettings()
        {
            InitializeComponent();

            EnableCheckboxes();
        }

        private void BtnSettingsOK_Click(object sender, EventArgs e)
        {
            CheckboxStates.CheckboxSeriesState = checkBoxSettingsComicInfoSeries.Checked;
            CheckboxStates.CheckboxTitleState = checkBoxSettingsComicInfoTitle.Checked;
            CheckboxStates.CheckboxVolumeState = checkBoxSettingsComicInfoVolume.Checked;
            CheckboxStates.CheckboxPageCountState = checkBoxSettingsComicInfoPageCount.Checked;
            CheckboxStates.CheckboxAuthorState = checkBoxSettingsComicInfoAuthors.Checked;
            CheckboxStates.CheckboxProducerState = checkBoxSettingsComicInfoProducers.Checked;
            CheckboxStates.CheckboxTranslatorState = checkBoxSettingsComicInfoTranslators.Checked;
            CheckboxStates.CheckboxLanguageState = checkBoxSettingsComicInfoLanguage.Checked;
            CheckboxStates.CheckboxPublisherState = checkBoxSettingsComicInfoPublisher.Checked;
            CheckboxStates.CheckboxDateState = checkBoxSettingsComicInfoDate.Checked;
            CheckboxStates.CheckboxDescriptionState = checkBoxSettingsComicInfoDescription.Checked;
            CheckboxStates.CheckboxReadingDirectionState = checkBoxSettingsComicInfoReadingDirection.Checked;
            CheckboxStates.CheckboxIsbnAsinState = checkBoxSettingsComicInfoIsbnAsin.Checked;
            CheckboxStates.CheckboxChaptersState = checkBoxSettingsComicInfoChapters.Checked;
            CheckboxStates.CheckboxImageSizeState = checkBoxSettingsComicInfoImageSize.Checked;

            CheckboxStates.CheckboxDuplicateCoverState = checkBoxSettingsExperimentalRemoveDuplicateCovers.Checked;
            CheckboxStates.CheckboxEveryPageIsChapterState = checkBoxSettingsExperimentalEveryPageIsChapter.Checked;
            CheckboxStates.CheckboxPageSpreadState = checkBoxSettingsExperimentalPageSpread.Checked;
            CheckboxStates.CheckboxSplitPageSpreadState = checkBoxSettingsExperimentalSplitPageSpread.Checked;
            CheckboxStates.CheckboxBlankImageState = checkBoxSettingsExperimentalSpreadInsertBlank.Checked;
            CheckboxStates.CheckboxHigherResolutionCover = checkBoxSettingsExperimentalCoverResolution.Checked;
            CheckboxStates.CheckboxInsertAdditionalBlankImageState = checkBoxSettingsExperimentalAddBlankPage.Checked;
            CheckboxStates.CheckboxMetadataTitleState = checkBoxSettingsExperimentalMetadataTitle.Checked;
            CheckboxStates.CheckboxChapterFoldersState = checkBoxSettingsExperimentalChapterFolders.Checked;

            CheckboxStates.CheckboxReplaceSeriesState = checkBoxSettingsReplaceSeries.Checked;
            CheckboxStates.TextboxReplaceSeriesState = textBoxReplaceSeries.Text;

            CheckboxStates.RadioButtonZipState = radioButtonSettingsZip.Checked;

            CheckboxStates.CheckboxDRMProtectionState = checkBoxSettingsExperimentalCheckDRMProtection.Checked;

            CheckboxStates.DropDownCompressionLevelState = dropDownCompressionLevel.SelectedIndex;
            CheckboxStates.DropDownParallelismDegreeState = dropDownThreads.SelectedIndex;

            CheckboxStates.CheckboxResizeImagesState = checkBoxSettingsResizingEnable.Checked;
            if (int.TryParse(textBoxSettingsResizeWidth.Text, out int parsedWidth)) CheckboxStates.TextBoxResizeWidthValue = parsedWidth;
            else CheckboxStates.TextBoxResizeWidthValue = 0;
            if (int.TryParse(textBoxSettingsResizeHeight.Text, out int parsedHeight)) CheckboxStates.TextBoxResizeHeightValue = parsedHeight;
            else CheckboxStates.TextBoxResizeHeightValue = 0;

            CheckboxStates.CheckboxCropImagesState = checkBoxSettingsCroppingEnable.Checked;
            if (int.TryParse(textBoxSettingsCropPadding.Text, out int padding)) CheckboxStates.TextBoxCropPaddingValue = padding;
            if (byte.TryParse(textBoxSettingsCropColorTolerance.Text, out byte colorTolerance)) CheckboxStates.TextBoxCropColorToleranceValue = colorTolerance;
            if (int.TryParse(textBoxSettingsCropDeviationTolerance.Text, out int deviationTolerance)) CheckboxStates.TextBoxCropDeviationToleranceValue = deviationTolerance;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnSettingsCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CheckBoxSettingsPageSpread_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSettingsExperimentalSpreadInsertBlank.Enabled = checkBoxSettingsExperimentalPageSpread.Checked;
        }

        private void CheckBoxSettingsExperimentalRemoveDuplicateCovers_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSettingsExperimentalCoverResolution.Enabled = checkBoxSettingsExperimentalRemoveDuplicateCovers.Checked;
        }

        private async void ButtonSettingsCheckForUpdate_Click(object sender, EventArgs e)
        {
            buttonSettingsCheckForUpdate.Enabled = false;

            if (string.IsNullOrEmpty(VersionNumbers.NewestVersion) || VersionNumbers.NewestVersion == "Error")
            {
                string elementValue = string.Empty;
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                string url = @"https://raw.githubusercontent.com/bust4cap/epub2cbz/refs/heads/main/ver/latest";

                using HttpClient client = new();
                client.DefaultRequestHeaders.Add("User-Agent", "epub2cbz");
                try
                {
                    var response = await client.GetAsync(url, cts.Token);
                    response.EnsureSuccessStatusCode();
                    string content = await response.Content.ReadAsStringAsync(cts.Token);
                    elementValue = content.Trim();
                }
                catch (Exception)
                {
                    elementValue = "Error";
                }
                finally
                {
                    cts.Dispose();
                    if (!IsDisposed && IsHandleCreated)
                    {
                        Invoke(new Action(() =>
                        {
                            VersionNumbers.NewestVersion = elementValue;
                            VersionNumbers.CurrentVersion = $"v{VersionDate.GetVersionDateYear}." +
                                                            $"{VersionDate.GetVersionDateMonth}." +
                                                            $"{VersionDate.GetVersionDateDay}-" +
                                                            $"{VersionDate.GetVersionNumber}";
                            CheckForUpdateForm update = new();
                            update.ShowDialog();
                        }));
                    }
                }
            }
            else
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    Invoke(new Action(() =>
                    {
                        CheckForUpdateForm update = new();
                        update.ShowDialog();
                    }));
                }
            }

            buttonSettingsCheckForUpdate.Enabled = true;
        }

        private void ButtonSettingsResetToDefault_Click(object sender, EventArgs e)
        {
            checkBoxSettingsComicInfoSeries.Checked = true;
            checkBoxSettingsComicInfoTitle.Checked = true;
            checkBoxSettingsComicInfoVolume.Checked = true;
            checkBoxSettingsComicInfoPageCount.Checked = true;
            checkBoxSettingsComicInfoAuthors.Checked = true;
            checkBoxSettingsComicInfoProducers.Checked = true;
            checkBoxSettingsComicInfoTranslators.Checked = true;
            checkBoxSettingsComicInfoLanguage.Checked = true;
            checkBoxSettingsComicInfoPublisher.Checked = true;
            checkBoxSettingsComicInfoDate.Checked = true;
            checkBoxSettingsComicInfoDescription.Checked = true;
            checkBoxSettingsComicInfoReadingDirection.Checked = true;
            checkBoxSettingsComicInfoIsbnAsin.Checked = true;
            checkBoxSettingsComicInfoChapters.Checked = true;
            checkBoxSettingsComicInfoImageSize.Checked = true;

            checkBoxSettingsExperimentalRemoveDuplicateCovers.Checked = true;
            checkBoxSettingsExperimentalEveryPageIsChapter.Checked = true;
            checkBoxSettingsExperimentalPageSpread.Checked = true;
            checkBoxSettingsExperimentalSplitPageSpread.Checked = true;
            checkBoxSettingsExperimentalSpreadInsertBlank.Checked = true;
            checkBoxSettingsExperimentalCoverResolution.Checked = true;
            checkBoxSettingsExperimentalAddBlankPage.Checked = false;
            checkBoxSettingsExperimentalMetadataTitle.Checked = false;
            checkBoxSettingsExperimentalChapterFolders.Checked = false;
            checkBoxSettingsReplaceSeries.Checked = false;
            textBoxReplaceSeries.Text = string.Empty;

            radioButtonSettingsZip.Checked = false;
            radioButtonSettingsCbz.Checked = true;

            checkBoxSettingsResizingEnable.Checked = false;
            textBoxSettingsResizeWidth.Text = "0";
            textBoxSettingsResizeHeight.Text = "0";

            checkBoxSettingsCroppingEnable.Checked = false;
            textBoxSettingsCropPadding.Text = "5";
            textBoxSettingsCropColorTolerance.Text = "15";
            textBoxSettingsCropDeviationTolerance.Text = "1";

            checkBoxSettingsExperimentalCheckDRMProtection.Checked = true;

            dropDownCompressionLevel.SelectedIndex = 0;
            dropDownThreads.SelectedIndex = 0;

            CheckboxStates.ButtonResetSettingsState = true;
        }

        private void TextBoxSettingsResizeWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TextBoxSettingsResizeHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void DropDownKindle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.DeviceResolutionKindle.Any(kindle => dropDownKindle.SelectedItem!.ToString() == kindle.Key))
            {
                (int width, int height) = Program.DeviceResolutionKindle[dropDownKindle.SelectedItem!.ToString()!];
                textBoxSettingsResizeWidth.Text = width.ToString();
                textBoxSettingsResizeHeight.Text = height.ToString();
                toolTipSettings.SetToolTip(dropDownKindle, dropDownKindle.SelectedItem.ToString());
            }
        }

        private void DropDownKobo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.DeviceResolutionKobo.Any(kobo => dropDownKobo.SelectedItem!.ToString() == kobo.Key))
            {
                (int width, int height) = Program.DeviceResolutionKobo[dropDownKobo.SelectedItem!.ToString()!];
                textBoxSettingsResizeWidth.Text = width.ToString();
                textBoxSettingsResizeHeight.Text = height.ToString();
                toolTipSettings.SetToolTip(dropDownKobo, dropDownKobo.SelectedItem.ToString());
            }
        }

        private void TextBoxSettingsResizeWidth_Leave(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new();

            foreach (char c in textBoxSettingsResizeWidth.Text)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                int number = int.Parse(sb.ToString()); // remove leading zeros
                textBoxSettingsResizeWidth.Text = number.ToString();
            }
            else
            {
                textBoxSettingsResizeWidth.Text = string.Empty;
            }
        }

        private void TextBoxSettingsResizeHeight_Leave(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new();

            foreach (char c in textBoxSettingsResizeHeight.Text)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                int number = int.Parse(sb.ToString()); // remove leading zeros
                textBoxSettingsResizeHeight.Text = number.ToString();
            }
            else
            {
                textBoxSettingsResizeHeight.Text = string.Empty;
            }
        }

        private void TextBoxSettingsCropPadding_Leave(object sender, EventArgs e)
        {
            int tempValue = CheckboxStates.TextBoxCropPaddingValue;
            if (int.TryParse(textBoxSettingsCropPadding.Text, out int padding) && padding >= 0) textBoxSettingsCropPadding.Text = padding.ToString();
            else textBoxSettingsCropPadding.Text = tempValue.ToString();
        }

        private void TextBoxSettingsCropColorTolerance_Leave(object sender, EventArgs e)
        {
            byte tempValue = CheckboxStates.TextBoxCropColorToleranceValue;
            if (byte.TryParse(textBoxSettingsCropColorTolerance.Text, out byte tolerance)) textBoxSettingsCropColorTolerance.Text = tolerance.ToString();
            else textBoxSettingsCropColorTolerance.Text = tempValue.ToString();
        }

        private void TextBoxSettingsCropDeviationTolerance_Leave(object sender, EventArgs e)
        {
            int tempValue = CheckboxStates.TextBoxCropDeviationToleranceValue;
            if (int.TryParse(textBoxSettingsCropDeviationTolerance.Text, out int tolerance) && tolerance >= 0 && tolerance <= 100) textBoxSettingsCropDeviationTolerance.Text = tolerance.ToString();
            else textBoxSettingsCropDeviationTolerance.Text = tempValue.ToString();
        }

        private void LabelSettingsCropPadding_SizeChanged(object sender, EventArgs e)
        {
            textBoxSettingsCropPadding.Left = labelSettingsCropPadding.Left
                + labelSettingsCropPadding.Width
                + labelSettingsCropPadding.Margin.Right
                + textBoxSettingsCropPadding.Margin.Left;
        }

        private void LabelSettingsCropColorTolerance_SizeChanged(object sender, EventArgs e)
        {
            textBoxSettingsCropColorTolerance.Left = labelSettingsCropColorTolerance.Left
                + labelSettingsCropColorTolerance.Width
                + labelSettingsCropColorTolerance.Margin.Right
                + textBoxSettingsCropColorTolerance.Margin.Left;
        }

        private void LabelSettingsCropDeviationTolerance_SizeChanged(object sender, EventArgs e)
        {
            textBoxSettingsCropDeviationTolerance.Left = labelSettingsCropDeviationTolerance.Left
                + labelSettingsCropDeviationTolerance.Width
                + labelSettingsCropDeviationTolerance.Margin.Right
                + textBoxSettingsCropDeviationTolerance.Margin.Left;
        }

        private void CheckBoxSettingsResizingEnable_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxResizing.Enabled = checkBoxSettingsResizingEnable.Checked;
        }

        private void CheckBoxSettingsCroppingEnable_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxCropping.Enabled = checkBoxSettingsCroppingEnable.Checked;
        }

        private void CheckBoxSettingsReplaceSeries_CheckedChanged(object sender, EventArgs e)
        {
            textBoxReplaceSeries.Enabled = checkBoxSettingsReplaceSeries.Checked;
        }

        private void EnableCheckboxes()
        {
            groupBoxCropping.Enabled = checkBoxSettingsCroppingEnable.Checked;

            dropDownThreads.MaxDropDownItems = Environment.ProcessorCount;
            for (int i = Environment.ProcessorCount - 1; i > 0; i--)
            {
                dropDownThreads.Items.Add(i);
            }

            checkBoxSettingsComicInfoSeries.Checked = CheckboxStates.CheckboxSeriesState;
            checkBoxSettingsComicInfoTitle.Checked = CheckboxStates.CheckboxTitleState;
            checkBoxSettingsComicInfoVolume.Checked = CheckboxStates.CheckboxVolumeState;
            checkBoxSettingsComicInfoPageCount.Checked = CheckboxStates.CheckboxPageCountState;
            checkBoxSettingsComicInfoAuthors.Checked = CheckboxStates.CheckboxAuthorState;
            checkBoxSettingsComicInfoProducers.Checked = CheckboxStates.CheckboxProducerState;
            checkBoxSettingsComicInfoTranslators.Checked = CheckboxStates.CheckboxTranslatorState;
            checkBoxSettingsComicInfoLanguage.Checked = CheckboxStates.CheckboxLanguageState;
            checkBoxSettingsComicInfoPublisher.Checked = CheckboxStates.CheckboxPublisherState;
            checkBoxSettingsComicInfoDate.Checked = CheckboxStates.CheckboxDateState;
            checkBoxSettingsComicInfoDescription.Checked = CheckboxStates.CheckboxDescriptionState;
            checkBoxSettingsComicInfoReadingDirection.Checked = CheckboxStates.CheckboxReadingDirectionState;
            checkBoxSettingsComicInfoIsbnAsin.Checked = CheckboxStates.CheckboxIsbnAsinState;
            checkBoxSettingsComicInfoChapters.Checked = CheckboxStates.CheckboxChaptersState;
            checkBoxSettingsComicInfoImageSize.Checked = CheckboxStates.CheckboxImageSizeState;

            checkBoxSettingsExperimentalRemoveDuplicateCovers.Checked = CheckboxStates.CheckboxDuplicateCoverState;
            checkBoxSettingsExperimentalEveryPageIsChapter.Checked = CheckboxStates.CheckboxEveryPageIsChapterState;
            checkBoxSettingsExperimentalPageSpread.Checked = CheckboxStates.CheckboxPageSpreadState;
            checkBoxSettingsExperimentalSplitPageSpread.Checked = CheckboxStates.CheckboxSplitPageSpreadState;
            checkBoxSettingsExperimentalSpreadInsertBlank.Checked = CheckboxStates.CheckboxBlankImageState;
            checkBoxSettingsExperimentalCoverResolution.Checked = CheckboxStates.CheckboxHigherResolutionCover;
            checkBoxSettingsExperimentalAddBlankPage.Checked = CheckboxStates.CheckboxInsertAdditionalBlankImageState;
            checkBoxSettingsExperimentalMetadataTitle.Checked = CheckboxStates.CheckboxMetadataTitleState;
            checkBoxSettingsExperimentalChapterFolders.Checked = CheckboxStates.CheckboxChapterFoldersState;

            checkBoxSettingsReplaceSeries.Checked = CheckboxStates.CheckboxReplaceSeriesState;
            textBoxReplaceSeries.Text = CheckboxStates.TextboxReplaceSeriesState;
            textBoxReplaceSeries.Enabled = checkBoxSettingsReplaceSeries.Checked;

            radioButtonSettingsCbz.Checked = !CheckboxStates.RadioButtonZipState;
            radioButtonSettingsZip.Checked = CheckboxStates.RadioButtonZipState;

            checkBoxSettingsResizingEnable.Checked = CheckboxStates.CheckboxResizeImagesState;
            textBoxSettingsResizeWidth.Text = CheckboxStates.TextBoxResizeWidthValue.ToString();
            textBoxSettingsResizeHeight.Text = CheckboxStates.TextBoxResizeHeightValue.ToString();

            checkBoxSettingsCroppingEnable.Checked = CheckboxStates.CheckboxCropImagesState;
            textBoxSettingsCropPadding.Text = CheckboxStates.TextBoxCropPaddingValue.ToString();
            textBoxSettingsCropColorTolerance.Text = CheckboxStates.TextBoxCropColorToleranceValue.ToString();
            textBoxSettingsCropDeviationTolerance.Text = CheckboxStates.TextBoxCropDeviationToleranceValue.ToString();

            checkBoxSettingsExperimentalCheckDRMProtection.Checked = CheckboxStates.CheckboxDRMProtectionState;

            checkBoxSettingsExperimentalSpreadInsertBlank.Enabled = CheckboxStates.CheckboxPageSpreadState;
            checkBoxSettingsExperimentalCoverResolution.Enabled = CheckboxStates.CheckboxDuplicateCoverState;

            dropDownCompressionLevel.SelectedIndex = CheckboxStates.DropDownCompressionLevelState;
            dropDownThreads.SelectedIndex = CheckboxStates.DropDownParallelismDegreeState;

            buttonSettingsCheckForUpdate.Enabled = true;
        }

        private void PopupSettings_Load(object sender, EventArgs e)
        {
            checkBoxSettingsComicInfoSeries.Text = Resources.SettingsSeries;
            checkBoxSettingsComicInfoTitle.Text = Resources.SettingsTitle;
            checkBoxSettingsComicInfoVolume.Text = Resources.SettingsVolume;
            checkBoxSettingsComicInfoPageCount.Text = Resources.SettingsPageCount;
            checkBoxSettingsComicInfoDate.Text = Resources.SettingsDate;
            checkBoxSettingsComicInfoPublisher.Text = Resources.SettingsPublisher;
            checkBoxSettingsComicInfoLanguage.Text = Resources.SettingsLanguage;
            checkBoxSettingsComicInfoReadingDirection.Text = Resources.SettingsReadingDirection;
            checkBoxSettingsComicInfoDescription.Text = Resources.SettingsDescription;
            checkBoxSettingsComicInfoAuthors.Text = Resources.SettingsAuthor;
            checkBoxSettingsComicInfoChapters.Text = Resources.SettingsChapters;
            checkBoxSettingsComicInfoProducers.Text = Resources.SettingsProducers;
            checkBoxSettingsComicInfoTranslators.Text = Resources.SettingsTranslators;
            checkBoxSettingsComicInfoImageSize.Text = Resources.SettingsImageSize;
            groupBoxComicInfo.Text = Resources.SettingsGroupComicInfo;

            groupBoxExperimental.Text = Resources.SettingsGroupExperimental;
            checkBoxSettingsExperimentalSpreadInsertBlank.Text = Resources.SettingsExperimentalSpreadInsertBlank;
            checkBoxSettingsExperimentalSplitPageSpread.Text = Resources.SettingsExperimentalSplitPageSpread;
            checkBoxSettingsExperimentalPageSpread.Text = Resources.SettingsExperimentalPageSpread;
            checkBoxSettingsExperimentalEveryPageIsChapter.Text = Resources.SettingsExperimentalEveryPageIsChapter;
            checkBoxSettingsExperimentalRemoveDuplicateCovers.Text = Resources.SettingsExperimentalRemoveDuplicateCovers;
            checkBoxSettingsExperimentalCoverResolution.Text = Resources.SettingsExperimentalHigherResolutionCover;
            checkBoxSettingsExperimentalAddBlankPage.Text = Resources.SettingsExperimentalAddBlankPage;
            checkBoxSettingsExperimentalMetadataTitle.Text = Resources.SettingsExperimentalMetadataTitle;
            checkBoxSettingsExperimentalChapterFolders.Text = Resources.SettingsExperimentalChapterFolders;
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSpreadInsertBlank, Resources.SettingsExperimentalSpreadInsertBlankTooltip);
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalSplitPageSpread, Resources.SettingsExperimentalSplitPageSpreadTooltip);
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalPageSpread, Resources.SettingsExperimentalPageSpreadTooltip);
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalEveryPageIsChapter, Resources.SettingsExperimentalEveryPageIsChapterTooltip);
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalRemoveDuplicateCovers, Resources.SettingsExperimentalRemoveDuplicateCoversTooltip);
            toolTipSettings.SetToolTip(checkBoxSettingsExperimentalAddBlankPage, Resources.SettingsExperimentalAddBlankPageTooltip);

            checkBoxSettingsReplaceSeries.Text = Resources.ReplaceSeries;
            textBoxReplaceSeries.PlaceholderText = Resources.ReplaceSeriesPlaceholder;

            checkBoxSettingsExperimentalCheckDRMProtection.Text = Resources.SettingsExperimentalDRMProtection;

            groupBoxResizing.Text = Resources.SettingsGroupResizing;
            checkBoxSettingsResizingEnable.Text = Resources.SettingsResizingResizeImages;
            labelSettingsResizeKindle.Text = Resources.SettingsResizingKindle;
            labelSettingsResizeKobo.Text = Resources.SettingsResizingKobo;
            labelSettingsResizeWidth.Text = Resources.SettingsResizingWidth;
            labelSettingsResizeHeight.Text = Resources.SettingsResizingHeight;
            groupBoxResizing.Enabled = checkBoxSettingsResizingEnable.Checked;

            groupBoxCropping.Text = Resources.SettingsGroupCropping;
            checkBoxSettingsCroppingEnable.Text = Resources.SettingsCroppingCropImages;
            labelSettingsCropPadding.Text = Resources.SettingsCroppingPadding;
            labelSettingsCropColorTolerance.Text = Resources.SettingsCroppingColorTolerance;
            labelSettingsCropDeviationTolerance.Text = Resources.SettingsCroppingDeviationTolerance;
            toolTipSettings.SetToolTip(labelSettingsCropPadding, Resources.SettingsCroppingPaddingTooltip);
            toolTipSettings.SetToolTip(labelSettingsCropColorTolerance, Resources.SettingsCroppingColorToleranceTooltip);
            toolTipSettings.SetToolTip(labelSettingsCropDeviationTolerance, Resources.SettingsCroppingDeviationToleranceTooltip);

            groupBoxCompressionLevel.Text = Resources.SettingsLabelCompressionLevel;
            dropDownCompressionLevel.Items[0] = Resources.SettingsCompressionLevelNoCompression;
            dropDownCompressionLevel.Items[1] = Resources.SettingsCompressionLevelFastest;
            dropDownCompressionLevel.Items[2] = Resources.SettingsCompressionLevelOptimal;
            dropDownCompressionLevel.Items[3] = Resources.SettingsCompressionLevelSmallestSize;
            toolTipSettings.SetToolTip(dropDownCompressionLevel, Resources.SettingsCompressionLevelTooltip);

            groupBoxThreads.Text = Resources.SettingsLabelThreads;

            this.Text = Resources.SettingsWindowTitle;
            buttonSettingsCheckForUpdate.Text = Resources.SettingsUpdateWindowTitle;
            buttonSettingsCancel.Text = Resources.CancelButtonText;
            buttonSettingsResetToDefault.Text = Resources.ResetSettings;

            radioButtonSettingsCbz.Left = (this.ClientRectangle.Width / 2) - (radioButtonSettingsCbz.Width / 2);
            radioButtonSettingsZip.Left = (this.ClientRectangle.Width / 2) - (radioButtonSettingsCbz.Width / 2);
            buttonSettingsResetToDefault.Left = ((buttonSettingsCancel.Left + buttonSettingsCheckForUpdate.Right) / 2) - (buttonSettingsResetToDefault.Width / 2);
        }
    }
}
