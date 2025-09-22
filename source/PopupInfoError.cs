using epub2cbz_gui.Properties;
using System.Media;

namespace epub2cbz_gui
{
    public partial class PopupInfoError : Form
    {
        public PopupInfoError()
        {
            InitializeComponent();
        }

        public void ShowInfoError(string message, string title, string icon = "error")
        {
            this.Text = title;
            labelMessage.Text = message;

            if (icon == "info")
            {
                buttonCancel.Visible = true;
                buttonCancel.Text = Resources.CancelButtonText;
                pictureBox.Image = Resources.info_icon;
            }
            else
            {
                pictureBox.Image= Resources.error_icon;
            }


            int xPositionPicture = this.ClientRectangle.Width / 20;
            int yPositionPicture = (buttonOK.Location.Y - pictureBox.Height) / 2;

            pictureBox.Location = new Point(xPositionPicture, yPositionPicture);

            int xPositionLabel = pictureBox.Right + (this.ClientRectangle.Width - pictureBox.Right - labelMessage.Width) / 2;
            int yPositionLabel = (buttonOK.Location.Y - labelMessage.Height) / 2;

            labelMessage.Location = new Point(xPositionLabel, yPositionLabel);


            this.ShowDialog();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PopupInfoError_FormClosing(object sender, FormClosingEventArgs e)
        {
            pictureBox.Image!.Dispose();
            pictureBox.Image = null;
        }

        private void PopupInfoError_Load(object sender, EventArgs e)
        {
            SystemSounds.Asterisk.Play();
        }
    }
}