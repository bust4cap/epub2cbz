using epub2cbz_gui.Properties;
using System.Data;

namespace epub2cbz_gui
{
    public partial class FileModeListForm : Form
    {
        public readonly DataTable fileListDataTable = new();

        public FileModeListForm()
        {
            InitializeComponent();

            fileListDataTable.Columns.Add(dataGridViewFileModeList.Columns["ColumnFileNames"]!.Name, typeof(string));
            fileListDataTable.Columns.Add("ColumnFilePaths", typeof(string));

            dataGridViewFileModeList.Columns["ColumnFileNames"]!.DataPropertyName = "ColumnFileNames";
            dataGridViewFileModeList.AutoGenerateColumns = false;

            dataGridViewFileModeList.DataSource = fileListDataTable;
        }

        private void FileModeListForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.FileList;

            dataGridViewFileModeList.Columns["ColumnFileNames"]!.HeaderText = Resources.FileName;

            buttonFileModeClear.Text = Resources.FileModeClearButton;

            AdjustListSize();
        }

        private void AdjustListSize()
        {
            dataGridViewFileModeList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridViewFileModeList.Columns["ColumnFileNames"]!.FillWeight = 1;
        }

        public void AddItemsToList(string file)
        {
            fileListDataTable.Rows.Add(Path.GetFileName(file), file);
        }

        private void ButtonFileModeClear_Click(object sender, EventArgs e)
        {
            using PopupInfoError popupInfoError = new();
            popupInfoError.ShowInfoError(Resources.FileModeClearListMessage, Resources.ConfirmationMessageBox, "info");
            DialogResult result = popupInfoError.DialogResult;

            if (result == DialogResult.OK)
            {
                if (dataGridViewFileModeList.DataSource is DataTable myDataTable)
                {
                    myDataTable.Clear();
                }

                MainForm.FileNameClass.FileNames.Clear();

                Program.ClearAndFocusConsole();
                Program.AppendColoredText(Resources.FileModeFileListCleared + Environment.NewLine, Color.Yellow);
            }
        }

        private void ToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            for (int i = dataGridViewFileModeList.SelectedRows.Count - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGridViewFileModeList.SelectedRows[i];

                if (row.DataBoundItem is DataRowView dataRowView)
                {
                    DataRow dataRow = dataRowView.Row;

                    MainForm.FileNameClass.FileNames.Remove(dataRow["ColumnFilePaths"].ToString()!);

                    dataRow.Delete();
                }
            }
        }

        private void DataGridViewFileModeList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hit = dataGridViewFileModeList.HitTest(e.X, e.Y);

                if (hit.Type == DataGridViewHitTestType.Cell)
                {
                    if (!dataGridViewFileModeList.Rows[hit.RowIndex].Selected)
                    {
                        dataGridViewFileModeList.ClearSelection();
                        dataGridViewFileModeList.Rows[hit.RowIndex].Selected = true;
                    }

                    contextMenuStripListView.Show(dataGridViewFileModeList, e.Location);
                }
            }
        }

        private void DataGridViewFileModeList_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var dataObject = e.Row!.DataBoundItem;

            if (dataObject is DataRowView dataRowView)
            {
                DataRow dataRow = dataRowView.Row;
                MainForm.FileNameClass.FileNames.Remove(dataRow["ColumnFilePaths"].ToString()!);
            }
        }

        private void DataGridViewFileModeList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.Value != null)
            {
                DataGridViewRow currentRow = dataGridViewFileModeList.Rows[e.RowIndex];

                if (currentRow.DataBoundItem is DataRowView dataRowView)
                {
                    DataRow dataRow = dataRowView.Row;
                    dataGridViewFileModeList.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = dataRow["ColumnFilePaths"].ToString()!;
                }
            }
        }

        private void ButtonFileListOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
