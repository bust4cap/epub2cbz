namespace epub2cbz_gui
{
    partial class FileModeListForm
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
            contextMenuStripListView = new ContextMenuStrip(components);
            toolStripMenuItemDelete = new ToolStripMenuItem();
            buttonFileModeClear = new Button();
            dataGridViewFileModeList = new DataGridView();
            buttonFileListOK = new Button();
            ColumnFileNames = new DataGridViewTextBoxColumn();
            contextMenuStripListView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFileModeList).BeginInit();
            SuspendLayout();
            // 
            // contextMenuStripListView
            // 
            contextMenuStripListView.Items.AddRange(new ToolStripItem[] { toolStripMenuItemDelete });
            contextMenuStripListView.Name = "contextMenuStripListView";
            contextMenuStripListView.ShowImageMargin = false;
            contextMenuStripListView.Size = new Size(83, 26);
            // 
            // toolStripMenuItemDelete
            // 
            toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            toolStripMenuItemDelete.Size = new Size(82, 22);
            toolStripMenuItemDelete.Text = "Delete";
            toolStripMenuItemDelete.Click += ToolStripMenuItemDelete_Click;
            // 
            // buttonFileModeClear
            // 
            buttonFileModeClear.Location = new Point(12, 12);
            buttonFileModeClear.Name = "buttonFileModeClear";
            buttonFileModeClear.Size = new Size(75, 23);
            buttonFileModeClear.TabIndex = 1;
            buttonFileModeClear.Text = "Clear";
            buttonFileModeClear.UseVisualStyleBackColor = true;
            buttonFileModeClear.Click += ButtonFileModeClear_Click;
            // 
            // dataGridViewFileModeList
            // 
            dataGridViewFileModeList.AllowUserToAddRows = false;
            dataGridViewFileModeList.AllowUserToResizeColumns = false;
            dataGridViewFileModeList.AllowUserToResizeRows = false;
            dataGridViewFileModeList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewFileModeList.BackgroundColor = SystemColors.Window;
            dataGridViewFileModeList.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            dataGridViewFileModeList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewFileModeList.Columns.AddRange(new DataGridViewColumn[] { ColumnFileNames });
            dataGridViewFileModeList.Location = new Point(12, 41);
            dataGridViewFileModeList.Name = "dataGridViewFileModeList";
            dataGridViewFileModeList.ReadOnly = true;
            dataGridViewFileModeList.RowHeadersVisible = false;
            dataGridViewFileModeList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewFileModeList.Size = new Size(632, 308);
            dataGridViewFileModeList.TabIndex = 2;
            dataGridViewFileModeList.CellFormatting += DataGridViewFileModeList_CellFormatting;
            dataGridViewFileModeList.UserDeletingRow += DataGridViewFileModeList_UserDeletingRow;
            dataGridViewFileModeList.MouseClick += DataGridViewFileModeList_MouseClick;
            // 
            // buttonFileListOK
            // 
            buttonFileListOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonFileListOK.Location = new Point(569, 12);
            buttonFileListOK.Name = "buttonFileListOK";
            buttonFileListOK.Size = new Size(75, 23);
            buttonFileListOK.TabIndex = 3;
            buttonFileListOK.Text = "OK";
            buttonFileListOK.UseVisualStyleBackColor = true;
            buttonFileListOK.Click += ButtonFileListOK_Click;
            // 
            // ColumnFileNames
            // 
            ColumnFileNames.DataPropertyName = "ColumnFileNames";
            ColumnFileNames.HeaderText = "File Name";
            ColumnFileNames.Name = "ColumnFileNames";
            ColumnFileNames.ReadOnly = true;
            // 
            // FileModeListForm
            // 
            AcceptButton = buttonFileListOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(656, 361);
            Controls.Add(buttonFileListOK);
            Controls.Add(dataGridViewFileModeList);
            Controls.Add(buttonFileModeClear);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(672, 400);
            Name = "FileModeListForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "File List";
            Load += FileModeListForm_Load;
            contextMenuStripListView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewFileModeList).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button buttonFileModeClear;
        private ContextMenuStrip contextMenuStripListView;
        private ToolStripMenuItem toolStripMenuItemDelete;
        private DataGridView dataGridViewFileModeList;
        private Button buttonFileListOK;
        private DataGridViewTextBoxColumn ColumnFileNames;
    }
}