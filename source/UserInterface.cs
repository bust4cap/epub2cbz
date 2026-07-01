using epub2cbz.Properties;

namespace epub2cbz
{
    internal class UserInterface
    {
        private static MainForm? _mainForm;

        public static void Initialize(MainForm form)
        {
            _mainForm = form;
        }

        public static void ProgressBarStep(int currentProgress)
        {
            _mainForm?.BeginInvoke(() =>
            {
                int targetValue = Math.Clamp(currentProgress, 0, Program.numberEpubs);

                if (targetValue > _mainForm.toolStripProgressBar.Value)
                {
                    _mainForm.toolStripProgressBar.Value = targetValue;
                }
            });
        }

        public static void ClearAndFocusConsole()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.outputBoxConsole.Clear();
                _mainForm.outputBoxConsole.Focus();
            });
        }

        public static void AppendColoredText(string text,
            Color color)
        {
            _mainForm?.BeginInvoke(() =>
            {
                var box = _mainForm.outputBoxConsole;

                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;

                box.ScrollToCaret();
            });
        }

        public static void DisableControls()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.checkBoxComicInfo.Enabled = false;
                _mainForm.checkBoxImages.Enabled = false;
                _mainForm.buttonPath.Enabled = false;
                _mainForm.buttonPathClear.Enabled = false;
                _mainForm.buttonSwitchModes.Enabled = false;
                _mainForm.buttonFileModeFileList.Enabled = false;
                _mainForm.buttonStart.Text = Resources.AbortButtonText;
                _mainForm.comboBoxLanguage.Enabled = false;
                _mainForm.buttonOpenSettings.Enabled = false;
            });
        }

        public static void EnableControls()
        {
            _mainForm?.Invoke(() =>
            {
                _mainForm.checkBoxComicInfo.Enabled = true;
                _mainForm.checkBoxImages.Enabled = true;
                _mainForm.buttonPath.Enabled = true;
                _mainForm.buttonPathClear.Enabled = true;
                _mainForm.buttonSwitchModes.Enabled = true;
                _mainForm.buttonFileModeFileList.Enabled = true;
                _mainForm.buttonStart.Enabled = true;
                _mainForm.buttonStart.Text = Resources.StartButtonText;
                _mainForm.comboBoxLanguage.Enabled = true;
                _mainForm.buttonOpenSettings.Enabled = true;
            });
        }
    }
}
