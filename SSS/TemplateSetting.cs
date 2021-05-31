using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel;

namespace SSS
{
    public partial class TemplateSetting : Form
    {
        DataBaseAccess dba;
        public TemplateSetting()
        {
            InitializeComponent();
            dba = new DataBaseAccess();          
        }

        private void SalesTemplate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }     

        private void txtSheetNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.KeyHandlerPoint(sender, e, 0);
            }
        }

   
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    OpenFileDialog _browser = new OpenFileDialog();
                    _browser.Filter = "Excel Files (*.xls,*.xlsx,*.csv)|*.xls;*.xlsx;*.csv|Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv";
                    _browser.ShowDialog();
                    if (_browser.FileName != "")
                        txtFilePath.Text = _browser.FileName;

                }
            }
            catch
            {
            }
        }

        private void txtSheetNo_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSheetNo.Text == "1")
                    txtSheetNo.Text = "";
            }
        }

        private void txtSheetNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSheetNo.Text == "")
                    txtSheetNo.Text = "1";
            }
        }

        private void txtHeaderRow_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtHeaderRow.Text == "1")
                    txtHeaderRow.Text = "";
            }
        }

        private void txtHeaderRow_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtHeaderRow.Text == "")
                    txtHeaderRow.Text = "1";
            }
        }      

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    DataSet ds = GetDataFromExcel();
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        BindColumnHeader(dt);
                       // DeleteAlreadyAddedColumn();
                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindColumnHeader(DataTable _dt)
        {
            lstExcel.Items.Clear();
            string[] strCellName = dba.ExcelCellName;
            int _index = 0;
            foreach (DataColumn _column in _dt.Columns)
            {
                lstExcel.Items.Add(_column.ColumnName + "~" + strCellName[_index]);
                _index++;
            }
        }

        //private void BindDBColumnHeader()
        //{
        //    DataTable _dt = dba.GetDataTable("Select * from ImportDBColumns WHere DBColumnName not in (Select ICD.DBColumnName from ImportColumnDetails ICD) and BillType='" + txtTemplateName.Text + "' ");
        //    lstColumn.DataSource = _dt;
        //    lstColumn.DisplayMember = "DBColumnName";
        //}


        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                string strExcelColumn = "", strDBColumn = "";

                if (lstExcel.SelectedIndex >= 0)
                    strExcelColumn = Convert.ToString(lstExcel.SelectedItem);//["Column_Name"]);
                if (lstColumn.SelectedIndex >= 0)
                    strDBColumn = Convert.ToString(lstColumn.SelectedItem); //Convert.ToString((lstColumn.SelectedItem as DataRowView)["DBColumnName"]); //
                if (strExcelColumn != "" && strDBColumn != "")
                {
                    MapColumnToRight(strExcelColumn, strDBColumn);
                    lstExcel.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("Sorry ! Please select in both column to map !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
        }

        private void MapColumnToRight(string strExcel, string strDBColumn)
        {
            int _index = dgrdName.Rows.Count;
            dgrdName.Rows.Add();
            string[] strColumn = strExcel.Split('~');

            dgrdName.Rows[_index].Cells["srNo"].Value = (_index + 1);
            dgrdName.Rows[_index].Cells["excelColumn"].Value = strColumn[0];
            if (strColumn.Length > 1)
                dgrdName.Rows[_index].Cells["columnType"].Value = strColumn[1];
            dgrdName.Rows[_index].Cells["systemColumn"].Value = strDBColumn;

            lstColumn.Items.RemoveAt(lstColumn.SelectedIndex);

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdName.Rows.Count > 0)
                {
                    DataGridViewRow row = dgrdName.Rows[dgrdName.Rows.Count - 1];
                    if (dgrdName.SelectedRows.Count > 0)
                    {
                        row = dgrdName.SelectedRows[0];
                    }

                    string strColumnName = Convert.ToString(row.Cells["systemColumn"].Value);
                    lstColumn.Items.Add(strColumnName);
                    dgrdName.Rows.Remove(row);
                    RearrangeSerialNo();
                }
            }
            catch
            {
            }
        }

        private void RearrangeSerialNo()
        {
            int index = 1;
            foreach (DataGridViewRow row in dgrdName.Rows)
            {
                row.Cells["srNo"].Value = index;
                index++;
            }
        }

        private void EnableAllControl()
        {
            txtTemplateName.ReadOnly = true; 
            txtSheetNo.ReadOnly = txtHeaderRow.ReadOnly = false;
        }

        private void DisableAllControl()
        {
            txtTemplateName.ReadOnly = false;
            txtSheetNo.ReadOnly = txtHeaderRow.ReadOnly = true;
        }

        private void ClearAllText()
        {
            txtTemplateName.Text =  "";
            txtHeaderRow.Text = txtSheetNo.Text = "1";            
            dgrdName.Rows.Clear();
        }

        private DataSet GetDataFromExcel()
        {
            DataSet ds = null;
            try
            {
                if (txtFilePath.Text != "")
                {
                    if (txtFilePath.Text.Contains(".CSV"))
                    {
                        ds = DataBaseAccess.GetDataTabletFromCSVFile(txtFilePath.Text); //ConvertCSVtoDataSet(txtFilePath.Text);
                    }
                    else if (txtFilePath.Text.Contains(".XLS"))
                    {
                        FileStream stream = new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read);
                        IExcelDataReader excelReader = null;

                        if (txtFilePath.Text.Contains(".XLSX"))
                        {
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        excelReader.IsFirstRowAsColumnNames = true;
                        ds = excelReader.AsDataSet();
                    }
                }
            }
            catch
            {
            }
            return ds;
        }

        public DataSet ConvertCSVtoDataSet(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath, Encoding.GetEncoding("iso-8859-1"));         
            string strAll = sr.ReadToEnd(), strSplitData = "\n\"";
            if (!strAll.Contains(strSplitData))
                strSplitData = "\r\n";
            string[] strAllData = strAll.Split(new string[] { strSplitData }, StringSplitOptions.None);

            DataTable dt = new DataTable();
            if (strAllData.Length > 0)
            {
                string[] headers = strAllData[0].Split(',');

                foreach (string header in headers)
                {
                    if (!dt.Columns.Contains(header))
                        dt.Columns.Add(header);
                }

                 string strData = "";
                 bool _bStatus = false;
                foreach (string strValue in strAllData)
                {
                    if (_bStatus)
                    {
                        string[] rows = SplitString(strValue);
                        try
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                if (i < dt.Columns.Count)
                                {
                                    strData = Convert.ToString(rows[i]);
                                    dr[i] = strData;// rows[i];
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                        catch
                        {
                        }
                    }
                    else
                        _bStatus = true;
                }
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        private string[] SplitString(string strData)
        {
            System.Text.RegularExpressions.MatchCollection matches = new System.Text.RegularExpressions.Regex("((?<=\")[^\"]*(?=\"(,|$)+)|(?<=,|^)[^,\"]*(?=,|$))").Matches(strData);
            List<string> list = new List<string>();
            foreach (var match in matches)
            {
                list.Add(match.ToString());
            }
            return list.ToArray();
        }

        private bool ValidateControl()
        {
            if (txtTemplateName.Text == "")
            {
                MessageBox.Show("Sorry ! Template Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTemplateName.Focus();
                return false;
            }
            if (dgrdName.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Please map atleast one column ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lstExcel.Focus();
                return false;
            }
            return true;
        }


        private bool CheckAvailability()
        {
            try
            {
                if (txtTemplateName.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DataTable MyTable = dba.GetDataTable("Select Template_Name from OL_BillTemplateDetails Where Template_Type='SALES' and Template_Name ='" + txtTemplateName.Text + "'");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Template Name : " + txtTemplateName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTemplateName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTemplateName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        DataTable MyTable = dba.GetDataTable("Select Template_Name from [OL_BillTemplateDetails] Where Template_Type='SALES' and Template_Name ='" + txtTemplateName.Text + "' and ID !=" + lblID.Text + "");
                        if (MyTable.Rows.Count > 0)
                        {
                            lblMsg.Text = "Sorry ! Template Name : " + txtTemplateName.Text + " already exist ! ";
                            lblMsg.ForeColor = Color.Red;
                            txtTemplateName.Focus();
                            return false;
                        }
                        else
                        {
                            lblMsg.Text = txtTemplateName.Text + " is available ! Please proceed";
                            lblMsg.ForeColor = Color.DarkGreen;
                            return true;
                        }
                    }
                }
                else
                {
                    lblMsg.Text = "Sorry ! Template Name can't be blank ! ";
                    lblMsg.ForeColor = Color.Red;
                    txtTemplateName.Focus();
                    return false;
                }
            }
            catch { }
            return false;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add")
            {
                if (btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                }
                btnAdd.Text = "&Save";
                btnEdit.Text = "&Edit";
                EnableAllControl();
                ClearAllText();
                txtTemplateName.Focus();
            }
            else
            {
                btnAdd.Enabled = false;

                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (ValidateControl())
                        SaveRecord();
                }
                btnAdd.Enabled = true;
            }
        }

        private void SaveRecord()
        {
            try
            {
                string strQuery = "";
                foreach (DataGridViewRow row in dgrdName.Rows)
                {
                    strQuery += " if not exists (Select [BillType] from [dbo].[ImportColumnDetails] Where [BillType]='" + txtTemplateName.Text + "' and [DBColumnName]='" + row.Cells["systemColumn"].Value + "') begin INSERT INTO [dbo].[ImportColumnDetails]([BillType],[BillCode],[BillNo],[DBColumnName],[TemplateColumnName],[Data_Type],[ReqColumn],[CheckMaster],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                            + " ('" + txtTemplateName.Text + "','',0,'" + row.Cells["systemColumn"].Value + "','" + row.Cells["excelColumn"].Value + "','" + row.Cells["columnType"].Value + "','" + row.Cells["reqStatus"].Value + "','" +  row.Cells["chkExists"].Value + "','" + MainPage.strLoginName + "','',1,0) end ";
                }

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thanks ! Record save successfully.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    ClearRecordAfterSave();
                    btnAdd.Text = "&Add";
                    GetRecordFromTemplateName();                    
                }
                else
                    MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }
        }

        private void ClearRecordAfterSave()
        {
            DisableAllControl();
            txtFilePath.Text =lblMsg.Text= "";
            lstExcel.DataSource = new DataTable();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnAdd.Text = "&Add";
                    btnEdit.Text = "&Update";
                    EnableAllControl();
                    txtTemplateName.Focus();
                }
                else if (ValidateControl())
                {
                    btnEdit.Enabled = false;
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateRecord();
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record updated successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                        }
                        else
                            MessageBox.Show("Sorry ! Record not updated, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    btnEdit.Enabled = true;
                }
            }
            catch
            {
            }
        }

        private int UpdateRecord()
        {
            string strQuery = " Delete from ImportColumnDetails Where BillType='" + txtTemplateName.Text + "' ";

            foreach (DataGridViewRow row in dgrdName.Rows)
            {
                strQuery += " if not exists (Select [BillType] from [dbo].[ImportColumnDetails] Where [BillType]='" + txtTemplateName.Text + "' and [DBColumnName]='" + row.Cells["systemColumn"].Value + "') begin INSERT INTO [dbo].[ImportColumnDetails]([BillType],[BillCode],[BillNo],[DBColumnName],[TemplateColumnName],[Data_Type],[ReqColumn],[CheckMaster],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                             + " ('" + txtTemplateName.Text + "','',0,'" + row.Cells["systemColumn"].Value + "','" + row.Cells["excelColumn"].Value + "','" + row.Cells["columnType"].Value + "','" + row.Cells["reqStatus"].Value + "','" + row.Cells["chkExists"].Value + "','" + MainPage.strLoginName + "','',1,0) end ";
            }

            int count = dba.ExecuteMyQuery(strQuery);
            return count;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtTemplateName.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Delete from ImportColumnDetails Where BillType='"+txtTemplateName.Text+"' ";

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            ClearAllText();
                        }
                        else
                            MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtTemplateName.Text = "";
            txtTemplateName.Focus();
        }

        private void txtTemplateName_KeyDown(object sender, KeyEventArgs e)
         {
            try
            {
                
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TEMPLATENAME", "SEARCH TEMPLATE NAME",e.KeyCode);
                        objSearch.ShowDialog();
                        txtTemplateName.Text = objSearch.strSelectedData;
                        GetRecordFromTemplateName();
                    
                    }
                    else
                    {
                        e.Handled = true;
                    }               
            }
            catch { }
        }

        private void GetRecordFromTemplateName()
        {
            dgrdName.Rows.Clear();
            if (txtTemplateName.Text != "")
            {
                string strQuery = "",strTemplate= txtTemplateName.Text;
                strQuery = "Select * from ImportColumnDetails Where BillType='" + strTemplate + "' "
                         + "Select * from ImportDBColumns Where DBColumnName not in (Select ICD.DBColumnName from ImportColumnDetails ICD Where BillType='" + txtTemplateName.Text + "') and BillType='" + txtTemplateName.Text + "'  ";
                            
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    BindDataWithGrid(ds.Tables[0]);
                    BindSystemColumns(ds.Tables[1]);
                }
            }
            else
                ClearAllText();
        }

        private void BindSystemColumns(DataTable dt)
        {
            lstColumn.Items.Clear();
            foreach (DataRow row in dt.Rows)
                lstColumn.Items.Add(row["DBColumnName"]);
        }


        private void BindDataWithGrid(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                dgrdName.Rows.Add(dt.Rows.Count);
                int _index = 0;
                foreach (DataRow row in dt.Rows)
                {
                    dgrdName.Rows[_index].Cells["srNo"].Value = _index + 1;
                    dgrdName.Rows[_index].Cells["excelColumn"].Value = row["TemplateColumnName"];
                    dgrdName.Rows[_index].Cells["systemColumn"].Value = row["DBColumnName"];                   
                    dgrdName.Rows[_index].Cells["columnType"].Value = row["Data_Type"];
                    dgrdName.Rows[_index].Cells["reqStatus"].Value =Convert.ToBoolean(row["ReqColumn"]);
                    dgrdName.Rows[_index].Cells["chkExists"].Value = Convert.ToBoolean(row["CheckMaster"]);
                    _index++;

                }
            }
        }

        private void DeleteAlreadyAddedColumn()
        {
            string strName = "";
            int _index = 0;
            foreach (DataGridViewRow row in dgrdName.Rows)
            {
                strName = Convert.ToString(row.Cells["systemColumn"].Value);
                _index= lstColumn.Items.IndexOf(strName);
                lstColumn.Items.RemoveAt(_index);
            }
        }

        private void dgrdName_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex <4)
                e.Cancel = true;
        }
    }
}
