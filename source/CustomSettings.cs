using System.Xml.Linq;

namespace epub2cbz_gui
{
    internal class CustomSettings
    {
        private static readonly string configName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // AppData/Local
            "epub2cbz-gui",
            "epub2cbz-gui.cfg");

        public static class SettingStates
        {
            public static bool CheckboxComicInfoState { get; set; } = true;
            public static bool CheckboxExtractImagesState { get; set; } = true;

            public static string InputFolderName { get; set; } = string.Empty;
            public static string OutputFolderName { get; set; } = string.Empty;

            public static string WindowLocation { get; set; } = string.Empty;
        }

        public static void SaveSettings()
        {
            XDocument config = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("epub2cbz-gui_config",
                    new XElement("setting",
                        new XAttribute("name", "CheckboxSeriesState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxSeriesState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxTitleState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxTitleState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxVolumeState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxVolumeState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxPageCountState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxPageCountState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxAuthorState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxAuthorState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxProducerState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxProducerState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxTranslatorState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxTranslatorState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxLanguageState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxLanguageState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxPublisherState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxPublisherState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxDateState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxDateState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxDescriptionState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxDescriptionState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxReadingDirectionState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxReadingDirectionState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxIsbnAsinState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxIsbnAsinState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxChaptersState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxChaptersState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxImageSizeState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxImageSizeState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxDuplicateCoverState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxDuplicateCoverState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxEveryPageIsChapterState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxEveryPageIsChapterState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxPageSpreadState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxPageSpreadState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxSplitPageSpreadState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxBlankImageState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxBlankImageState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxHigherResolutionCover"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxHigherResolutionCover.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxInsertAdditionalBlankImageState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxInsertAdditionalBlankImageState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxRemoveFirstPageState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxRemoveFirstPageState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxMetadataTitleState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxMetadataTitleState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxChapterFoldersState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxChapterFoldersState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxResizeImagesState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxResizeImagesState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "TextBoxResizeWidthValue"),
                        new XElement("value", PopupSettings.CheckboxStates.TextBoxResizeWidthValue.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "TextBoxResizeHeightValue"),
                        new XElement("value", PopupSettings.CheckboxStates.TextBoxResizeHeightValue.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxCropImagesState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxCropImagesState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "TextBoxCropPaddingValue"),
                        new XElement("value", PopupSettings.CheckboxStates.TextBoxCropPaddingValue.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "TextBoxCropColorToleranceValue"),
                        new XElement("value", PopupSettings.CheckboxStates.TextBoxCropColorToleranceValue.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "TextBoxCropDeviationToleranceValue"),
                        new XElement("value", PopupSettings.CheckboxStates.TextBoxCropDeviationToleranceValue.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "RadioButtonZipState"),
                        new XElement("value", PopupSettings.CheckboxStates.RadioButtonZipState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "DropDownCompressionLevelState"),
                        new XElement("value", PopupSettings.CheckboxStates.DropDownCompressionLevelState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "DropDownParallelismDegreeState"),
                        new XElement("value", PopupSettings.CheckboxStates.DropDownParallelismDegreeState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxComicInfoState"),
                        new XElement("value", MainForm.FormElements.CheckboxComicInfoState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxExtractImagesState"),
                        new XElement("value", MainForm.FormElements.CheckboxExtractImagesState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "FolderName"),
                        new XElement("value", MainForm.FolderNameClass.InputFolderName ?? string.Empty)),
                    new XElement("setting",
                        new XAttribute("name", "OutputFolderName"),
                        new XElement("value", MainForm.FolderNameClass.OutputFolderName ?? string.Empty)),
                    new XElement("setting",
                        new XAttribute("name", "WindowLocation"),
                        new XElement("value", SettingStates.WindowLocation)),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxDRMProtectionState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxDRMProtectionState.ToString())),
                    new XElement("setting",
                        new XAttribute("name", "CheckboxFileModeState"),
                        new XElement("value", PopupSettings.CheckboxStates.CheckboxFileModeState.ToString()))
                )
            );

            Directory.CreateDirectory(Path.GetDirectoryName(configName)!);

            config.Save(configName);
        }

        public static void LoadSettings()
        {
            if (File.Exists(configName))
            {
                Dictionary<string, string> loadedSettings = [];
                XDocument config;

                try
                {
                    config = XDocument.Load(configName);
                }
                catch
                {
                    return;
                }

                XElement? settingsElement = config.Descendants("epub2cbz-gui_config").FirstOrDefault();

                if (settingsElement != null)
                {
                    foreach (XElement settingElement in settingsElement.Elements("setting"))
                    {
                        XAttribute? nameAttribute = settingElement.Attribute("name");
                        XElement? valueElement = settingElement.Element("value");

                        if (nameAttribute != null && valueElement != null)
                        {
                            loadedSettings[nameAttribute.Value] = valueElement.Value;
                        }
                    }
                }

                if (loadedSettings.Count > 0)
                {
                    if (loadedSettings.TryGetValue("CheckboxSeriesState", out string? valueCheckboxSeriesState)
                        && bool.TryParse(valueCheckboxSeriesState, out bool parsedValueCheckboxSeriesState))
                    {
                        PopupSettings.CheckboxStates.CheckboxSeriesState = parsedValueCheckboxSeriesState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxTitleState", out string? valueCheckboxTitleState)
                        && bool.TryParse(valueCheckboxTitleState, out bool parsedValueCheckboxTitleState))
                    {
                        PopupSettings.CheckboxStates.CheckboxTitleState = parsedValueCheckboxTitleState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxVolumeState", out string? valueCheckboxVolumeState)
                        && bool.TryParse(valueCheckboxVolumeState, out bool parsedValueCheckboxVolumeState))
                    {
                        PopupSettings.CheckboxStates.CheckboxVolumeState = parsedValueCheckboxVolumeState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxPageCountState", out string? valueCheckboxPageCountState)
                        && bool.TryParse(valueCheckboxPageCountState, out bool parsedValueCheckboxPageCountState))
                    {
                        PopupSettings.CheckboxStates.CheckboxPageCountState = parsedValueCheckboxPageCountState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxAuthorState", out string? valueCheckboxAuthorState)
                        && bool.TryParse(valueCheckboxAuthorState, out bool parsedValueCheckboxAuthorState))
                    {
                        PopupSettings.CheckboxStates.CheckboxAuthorState = parsedValueCheckboxAuthorState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxProducerState", out string? valueCheckboxProducerState)
                        && bool.TryParse(valueCheckboxProducerState, out bool parsedValueCheckboxProducerState))
                    {
                        PopupSettings.CheckboxStates.CheckboxProducerState = parsedValueCheckboxProducerState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxTranslatorState", out string? valueCheckboxTranslatorState)
                        && bool.TryParse(valueCheckboxTranslatorState, out bool parsedValueCheckboxTranslatorState))
                    {
                        PopupSettings.CheckboxStates.CheckboxTranslatorState = parsedValueCheckboxTranslatorState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxLanguageState", out string? valueCheckboxLanguageState)
                        && bool.TryParse(valueCheckboxLanguageState, out bool parsedValueCheckboxLanguageState))
                    {
                        PopupSettings.CheckboxStates.CheckboxLanguageState = parsedValueCheckboxLanguageState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxPublisherState", out string? valueCheckboxPublisherState)
                        && bool.TryParse(valueCheckboxPublisherState, out bool parsedValueCheckboxPublisherState))
                    {
                        PopupSettings.CheckboxStates.CheckboxPublisherState = parsedValueCheckboxPublisherState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxDateState", out string? valueCheckboxDateState)
                        && bool.TryParse(valueCheckboxDateState, out bool parsedValueCheckboxDateState))
                    {
                        PopupSettings.CheckboxStates.CheckboxDateState = parsedValueCheckboxDateState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxDescriptionState", out string? valueCheckboxDescriptionState)
                        && bool.TryParse(valueCheckboxDescriptionState, out bool parsedValueCheckboxDescriptionState))
                    {
                        PopupSettings.CheckboxStates.CheckboxDescriptionState = parsedValueCheckboxDescriptionState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxReadingDirectionState", out string? valueCheckboxReadingDirectionState)
                        && bool.TryParse(valueCheckboxReadingDirectionState, out bool parsedValueCheckboxReadingDirectionState))
                    {
                        PopupSettings.CheckboxStates.CheckboxReadingDirectionState = parsedValueCheckboxReadingDirectionState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxIsbnAsinState", out string? valueCheckboxIsbnAsinState)
                        && bool.TryParse(valueCheckboxIsbnAsinState, out bool parsedValueCheckboxIsbnAsinState))
                    {
                        PopupSettings.CheckboxStates.CheckboxIsbnAsinState = parsedValueCheckboxIsbnAsinState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxChaptersState", out string? valueCheckboxChaptersState)
                        && bool.TryParse(valueCheckboxChaptersState, out bool parsedValueCheckboxChaptersState))
                    {
                        PopupSettings.CheckboxStates.CheckboxChaptersState = parsedValueCheckboxChaptersState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxImageSizeState", out string? valueCheckboxImageSizeState)
                        && bool.TryParse(valueCheckboxImageSizeState, out bool parsedValueCheckboxImageSizeState))
                    {
                        PopupSettings.CheckboxStates.CheckboxImageSizeState = parsedValueCheckboxImageSizeState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxDuplicateCoverState", out string? valueCheckboxDuplicateCoverState)
                        && bool.TryParse(valueCheckboxDuplicateCoverState, out bool parsedValueCheckboxDuplicateCoverState))
                    {
                        PopupSettings.CheckboxStates.CheckboxDuplicateCoverState = parsedValueCheckboxDuplicateCoverState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxEveryPageIsChapterState", out string? valueCheckboxEveryPageIsChapterState)
                        && bool.TryParse(valueCheckboxEveryPageIsChapterState, out bool parsedValueCheckboxEveryPageIsChapterState))
                    {
                        PopupSettings.CheckboxStates.CheckboxEveryPageIsChapterState = parsedValueCheckboxEveryPageIsChapterState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxPageSpreadState", out string? valueCheckboxPageSpreadState)
                        && bool.TryParse(valueCheckboxPageSpreadState, out bool parsedValueCheckboxPageSpreadState))
                    {
                        PopupSettings.CheckboxStates.CheckboxPageSpreadState = parsedValueCheckboxPageSpreadState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxSplitPageSpreadState", out string? valueCheckboxSplitPageSpreadState)
                        && bool.TryParse(valueCheckboxSplitPageSpreadState, out bool parsedValueCheckboxSplitPageSpreadState))
                    {
                        PopupSettings.CheckboxStates.CheckboxSplitPageSpreadState = parsedValueCheckboxSplitPageSpreadState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxBlankImageState", out string? valueCheckboxBlankImageState)
                        && bool.TryParse(valueCheckboxBlankImageState, out bool parsedValueCheckboxBlankImageState))
                    {
                        PopupSettings.CheckboxStates.CheckboxBlankImageState = parsedValueCheckboxBlankImageState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxHigherResolutionCover", out string? valueCheckboxHigherResolutionCover)
                        && bool.TryParse(valueCheckboxHigherResolutionCover, out bool parsedValueCheckboxHigherResolutionCover))
                    {
                        PopupSettings.CheckboxStates.CheckboxHigherResolutionCover = parsedValueCheckboxHigherResolutionCover;
                    }

                    if (loadedSettings.TryGetValue("CheckboxInsertAdditionalBlankImageState", out string? valueCheckboxInsertAdditionalBlankImageState)
                        && bool.TryParse(valueCheckboxInsertAdditionalBlankImageState, out bool parsedValueCheckboxInsertAdditionalBlankImageState))
                    {
                        PopupSettings.CheckboxStates.CheckboxInsertAdditionalBlankImageState = parsedValueCheckboxInsertAdditionalBlankImageState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxRemoveFirstPageState", out string? valueCheckboxRemoveFirstPageState)
                        && bool.TryParse(valueCheckboxRemoveFirstPageState, out bool parsedValueCheckboxRemoveFirstPageState))
                    {
                        PopupSettings.CheckboxStates.CheckboxRemoveFirstPageState = parsedValueCheckboxRemoveFirstPageState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxMetadataTitleState", out string? valueCheckboxMetadataTitleState)
                        && bool.TryParse(valueCheckboxMetadataTitleState, out bool parsedValueCheckboxMetadataTitleState))
                    {
                        PopupSettings.CheckboxStates.CheckboxMetadataTitleState = parsedValueCheckboxMetadataTitleState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxChapterFoldersState", out string? valueCheckboxChapterFoldersState)
                        && bool.TryParse(valueCheckboxChapterFoldersState, out bool parsedValueCheckboxChapterFoldersState))
                    {
                        PopupSettings.CheckboxStates.CheckboxChapterFoldersState = parsedValueCheckboxChapterFoldersState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxResizeImagesState", out string? valueCheckboxResizeImagesState)
                        && bool.TryParse(valueCheckboxResizeImagesState, out bool parsedValueCheckboxResizeImagesState))
                    {
                        PopupSettings.CheckboxStates.CheckboxResizeImagesState = parsedValueCheckboxResizeImagesState;
                    }

                    if (loadedSettings.TryGetValue("TextBoxResizeWidthValue", out string? valueTextBoxResizeWidthValue)
                        && int.TryParse(valueTextBoxResizeWidthValue, out int parsedValueTextBoxResizeWidthValue))
                    {
                        PopupSettings.CheckboxStates.TextBoxResizeWidthValue = parsedValueTextBoxResizeWidthValue;
                    }

                    if (loadedSettings.TryGetValue("TextBoxResizeHeightValue", out string? valueTextBoxResizeHeightValue)
                        && int.TryParse(valueTextBoxResizeHeightValue, out int parsedValueTextBoxResizeHeightValue))
                    {
                        PopupSettings.CheckboxStates.TextBoxResizeHeightValue = parsedValueTextBoxResizeHeightValue;
                    }

                    if (loadedSettings.TryGetValue("CheckboxCropImagesState", out string? valueCheckboxCropImagesState)
                        && bool.TryParse(valueCheckboxCropImagesState, out bool parsedValueCheckboxCropImagesState))
                    {
                        PopupSettings.CheckboxStates.CheckboxCropImagesState = parsedValueCheckboxCropImagesState;
                    }

                    if (loadedSettings.TryGetValue("TextBoxCropPaddingValue", out string? valueTextBoxCropPaddingValue)
                        && int.TryParse(valueTextBoxCropPaddingValue, out int parsedValueTextBoxCropPaddingValue))
                    {
                        PopupSettings.CheckboxStates.TextBoxCropPaddingValue = parsedValueTextBoxCropPaddingValue;
                    }

                    if (loadedSettings.TryGetValue("TextBoxCropColorToleranceValue", out string? valueTextBoxCropToleranceValue)
                        && byte.TryParse(valueTextBoxCropToleranceValue, out byte parsedValueTextBoxCropToleranceValue))
                    {
                        PopupSettings.CheckboxStates.TextBoxCropColorToleranceValue = parsedValueTextBoxCropToleranceValue;
                    }

                    if (loadedSettings.TryGetValue("TextBoxCropDeviationToleranceValue", out string? valueTextBoxCropDeviationToleranceValue)
                        && int.TryParse(valueTextBoxCropDeviationToleranceValue, out int parsedValueTextBoxCropDeviationToleranceValue))
                    {
                        PopupSettings.CheckboxStates.TextBoxCropDeviationToleranceValue = parsedValueTextBoxCropDeviationToleranceValue;
                    }

                    if (loadedSettings.TryGetValue("RadioButtonZipState", out string? valueRadioButtonZipState)
                        && bool.TryParse(valueRadioButtonZipState, out bool parsedValueRadioButtonZipState))
                    {
                        PopupSettings.CheckboxStates.RadioButtonZipState = parsedValueRadioButtonZipState;
                    }

                    if (loadedSettings.TryGetValue("DropDownCompressionLevelState", out string? valueDropDownCompressionLevelState)
                        && int.TryParse(valueDropDownCompressionLevelState, out int parsedValueDropDownCompressionLevelState))
                    {
                        PopupSettings popupSettings = new();
                        if (parsedValueDropDownCompressionLevelState >= 0
                            && parsedValueDropDownCompressionLevelState < popupSettings.dropDownCompressionLevel.Items.Count)
                        {
                            PopupSettings.CheckboxStates.DropDownCompressionLevelState = parsedValueDropDownCompressionLevelState;
                        }
                        else
                        {
                            PopupSettings.CheckboxStates.DropDownCompressionLevelState = 0;
                        }
                    }

                    if (loadedSettings.TryGetValue("DropDownParallelismDegreeState", out string? valueDropDownParallelismDegreeState)
                        && int.TryParse(valueDropDownParallelismDegreeState, out int parsedValueDropDownParallelismDegreeState))
                    {
                        PopupSettings popupSettings = new();
                        if (parsedValueDropDownParallelismDegreeState >= 0
                            && parsedValueDropDownParallelismDegreeState < popupSettings.dropDownThreads.Items.Count)
                        {
                            PopupSettings.CheckboxStates.DropDownParallelismDegreeState = parsedValueDropDownParallelismDegreeState;
                        }
                        else
                        {
                            PopupSettings.CheckboxStates.DropDownParallelismDegreeState = 0;
                        }
                    }

                    if (loadedSettings.TryGetValue("CheckboxComicInfoState", out string? valueCheckboxComicInfoState)
                        && bool.TryParse(valueCheckboxComicInfoState, out bool parsedValueCheckboxComicInfoState))
                    {
                        SettingStates.CheckboxComicInfoState = parsedValueCheckboxComicInfoState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxExtractImagesState", out string? valueCheckboxExtractImagesState)
                        && bool.TryParse(valueCheckboxExtractImagesState, out bool parsedValueCheckboxExtractImagesState))
                    {
                        SettingStates.CheckboxExtractImagesState = parsedValueCheckboxExtractImagesState;
                    }

                    if (string.IsNullOrEmpty(SettingStates.InputFolderName)
                        && loadedSettings.TryGetValue("FolderName", out string? valueFolderName))
                    {
                        SettingStates.InputFolderName = valueFolderName;
                    }

                    if (string.IsNullOrEmpty(SettingStates.OutputFolderName)
                        && loadedSettings.TryGetValue("OutputFolderName", out string? valueOutputFolderName))
                    {
                        SettingStates.OutputFolderName = valueOutputFolderName;
                    }

                    if (loadedSettings.TryGetValue("WindowLocation", out string? valueWindowLocation))
                    {
                        SettingStates.WindowLocation = valueWindowLocation;
                    }

                    if (loadedSettings.TryGetValue("CheckboxDRMProtectionState", out string? valueCheckboxDRMProtectionState)
                        && bool.TryParse(valueCheckboxDRMProtectionState, out bool parsedValueCheckboxDRMProtectionState))
                    {
                        PopupSettings.CheckboxStates.CheckboxDRMProtectionState = parsedValueCheckboxDRMProtectionState;
                    }

                    if (loadedSettings.TryGetValue("CheckboxFileModeState", out string? valueCheckboxFileModeState)
                        && bool.TryParse(valueCheckboxFileModeState, out bool parsedValueCheckboxFileModeState))
                    {
                        PopupSettings.CheckboxStates.CheckboxFileModeState = parsedValueCheckboxFileModeState;
                    }
                }
            }
        }
    }
}
