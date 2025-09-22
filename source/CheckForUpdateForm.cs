using System.Diagnostics;
using epub2cbz_gui.Properties;

namespace epub2cbz_gui
{
    public partial class CheckForUpdateForm : Form
    {
        public CheckForUpdateForm()
        {
            InitializeComponent();

            labelUpdateCurrentText.Text = PopupSettings.VersionNumbers.CurrentVersion;
            labelUpdateNewestText.Text = PopupSettings.VersionNumbers.NewestVersion;

            CheckIfUpdateIsAvailable();
        }

        private void CheckIfUpdateIsAvailable()
        {
            int current = int.Parse(labelUpdateCurrentText.Text[1..].Replace(".", string.Empty).Replace("-", string.Empty));

            if (int.TryParse(labelUpdateNewestText.Text[1..].Replace(".", string.Empty).Replace("-", string.Empty), out int newest))
            {
                if (newest > current)
                {
                    labelUpdateIsUpToDate.ForeColor = !MainForm.FormElements.DarkModeState ? Color.DarkGoldenrod : Color.Goldenrod;
                    labelUpdateIsUpToDate.Text = Resources.SettingsUpdateNewerVersionAvailable;
                }
                else
                {
                    labelUpdateIsUpToDate.ForeColor = !MainForm.FormElements.DarkModeState ? Color.DarkGreen : Color.Green;
                    labelUpdateIsUpToDate.Text = Resources.SettingsUpdateAlreadyOnNewest;
                }
            }
            else
            {
                labelUpdateIsUpToDate.ForeColor = !MainForm.FormElements.DarkModeState ? Color.DarkRed : Color.Red;
                labelUpdateIsUpToDate.Text = Resources.SettingsUpdateErrorTop + Environment.NewLine +
                        Resources.SettingsUpdateErrorBottom;
            }

        }
        private void ButtonUpdateOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabelUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
                (((LinkLabel)sender).Text)
            { UseShellExecute = true });
        }

        private void CheckForUpdateForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.SettingsUpdateWindowTitle;
            labelUpdateCurrent.Text = Resources.SettingsUpdateCurrentVersion;
            labelUpdateNewest.Text = Resources.SettingsUpdateNewestVersion;

            labelUpdateIsUpToDate.Left = this.ClientRectangle.Width / 2 - labelUpdateIsUpToDate.Width / 2;
            linkLabelUpdate.Left = this.ClientRectangle.Width / 2 - linkLabelUpdate.Width / 2;
            buttonUpdateOK.Left = this.ClientRectangle.Width / 2 - buttonUpdateOK.Width / 2;

            if (MainForm.FormElements.DarkModeState) linkLabelUpdate.LinkColor = Color.SkyBlue;
        }
    }
}
