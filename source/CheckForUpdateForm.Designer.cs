namespace epub2cbz_gui
{
    partial class CheckForUpdateForm
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
            buttonUpdateOK = new Button();
            labelUpdateNewest = new Label();
            labelUpdateCurrent = new Label();
            labelUpdateCurrentText = new Label();
            labelUpdateNewestText = new Label();
            linkLabelUpdate = new LinkLabel();
            labelUpdateIsUpToDate = new Label();
            SuspendLayout();
            // 
            // buttonUpdateOK
            // 
            buttonUpdateOK.Location = new Point(152, 185);
            buttonUpdateOK.Name = "buttonUpdateOK";
            buttonUpdateOK.Size = new Size(81, 25);
            buttonUpdateOK.TabIndex = 0;
            buttonUpdateOK.Text = "OK";
            buttonUpdateOK.UseVisualStyleBackColor = true;
            buttonUpdateOK.Click += ButtonUpdateOK_Click;
            // 
            // labelUpdateNewest
            // 
            labelUpdateNewest.AutoSize = true;
            labelUpdateNewest.Location = new Point(99, 29);
            labelUpdateNewest.Name = "labelUpdateNewest";
            labelUpdateNewest.Size = new Size(90, 15);
            labelUpdateNewest.TabIndex = 1;
            labelUpdateNewest.Text = "Newest Version:";
            // 
            // labelUpdateCurrent
            // 
            labelUpdateCurrent.AutoSize = true;
            labelUpdateCurrent.Location = new Point(99, 54);
            labelUpdateCurrent.Name = "labelUpdateCurrent";
            labelUpdateCurrent.Size = new Size(91, 15);
            labelUpdateCurrent.TabIndex = 2;
            labelUpdateCurrent.Text = "Current Version:";
            // 
            // labelUpdateCurrentText
            // 
            labelUpdateCurrentText.AutoSize = true;
            labelUpdateCurrentText.Location = new Point(207, 54);
            labelUpdateCurrentText.Name = "labelUpdateCurrentText";
            labelUpdateCurrentText.Size = new Size(78, 15);
            labelUpdateCurrentText.TabIndex = 4;
            labelUpdateCurrentText.Text = "v2024.01.01-1";
            // 
            // labelUpdateNewestText
            // 
            labelUpdateNewestText.AutoSize = true;
            labelUpdateNewestText.Location = new Point(207, 29);
            labelUpdateNewestText.Name = "labelUpdateNewestText";
            labelUpdateNewestText.Size = new Size(78, 15);
            labelUpdateNewestText.TabIndex = 3;
            labelUpdateNewestText.Text = "v2024.01.01-1";
            // 
            // linkLabelUpdate
            // 
            linkLabelUpdate.AutoSize = true;
            linkLabelUpdate.Location = new Point(69, 143);
            linkLabelUpdate.Name = "linkLabelUpdate";
            linkLabelUpdate.Size = new Size(240, 15);
            linkLabelUpdate.TabIndex = 5;
            linkLabelUpdate.TabStop = true;
            linkLabelUpdate.Text = "https://github.com/bust4cap/epub2cbz";
            linkLabelUpdate.LinkClicked += LinkLabelUpdate_LinkClicked;
            // 
            // labelUpdateIsUpToDate
            // 
            labelUpdateIsUpToDate.Location = new Point(24, 82);
            labelUpdateIsUpToDate.Name = "labelUpdateIsUpToDate";
            labelUpdateIsUpToDate.Size = new Size(343, 50);
            labelUpdateIsUpToDate.TabIndex = 6;
            labelUpdateIsUpToDate.Text = "Update Available or Up To Date";
            labelUpdateIsUpToDate.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CheckForUpdateForm
            // 
            AcceptButton = buttonUpdateOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(392, 220);
            Controls.Add(labelUpdateIsUpToDate);
            Controls.Add(linkLabelUpdate);
            Controls.Add(labelUpdateCurrentText);
            Controls.Add(labelUpdateNewestText);
            Controls.Add(labelUpdateCurrent);
            Controls.Add(labelUpdateNewest);
            Controls.Add(buttonUpdateOK);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CheckForUpdateForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Update";
            Load += CheckForUpdateForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonUpdateOK;
        private Label labelUpdateNewest;
        private Label labelUpdateCurrent;
        private Label labelUpdateCurrentText;
        private Label labelUpdateNewestText;
        private LinkLabel linkLabelUpdate;
        private Label labelUpdateIsUpToDate;
    }
}