using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSS
{
    public partial class EditLogReport : Form
    {
        DataBaseAccess dba;
        string strBillType = "";
        public EditLogReport()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
        }
        public EditLogReport(string strBType)
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            strBillType = strBType;
            SearchRecord();
        }

        private void RemovalReason_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }


        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
                txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = txtToDate.Text = MainPage.strCurrentDate;
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtBillType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BILLTYPE", "SEARCH BILL TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillType.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            SearchRecord();
            btnGo.Enabled = true;
        }

        private void SearchRecord()
        {
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length!=10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show("Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    chkDate.Focus();
                }
                else
                {
                    dgrdDetails.Rows.Clear();
                    GetRecordFromDB();
                }
            }
            catch
            {
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (txtBillType.Text != "")
                strQuery = " and BillType in ('"+txtBillType.Text+"') ";

            if (strBillType != "")
                strQuery = " and BillType in ('" + strBillType + "') ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime startDate = dba.ConvertDateInExactFormat(txtFromDate.Text), endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                strQuery += " and Date>='" + startDate.ToString("MM/dd/yyyy") + "' and Date <'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' ";
            }

            if (txtNetAmt.Text != "")
            {
                double dNetAMt = dba.ConvertObjectToDouble(txtNetAmt.Text);
                strQuery += " and NetAmt ="+ dNetAMt+" ";
            }
            return strQuery;
        }

        private void GetRecordFromDB()
        {
            string strQuery = "", strSubQuery = CreateQuery();
            strQuery = " Select Distinct Date, BillType,BillCode,BillNo,NetAmt,UpdatedBy,EditStatus from EditTrailDetails Where EditStatus='UPDATION' and BillType!='' " + strSubQuery + " Order by Date desc ";
            DataTable dt = dba.GetDataTable(strQuery);
            BindDataWithGrid(dt);
        }

        private void BindDataWithGrid(DataTable dt)
        {
            try
            {
                if(dt.Rows.Count>0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach(DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                        dgrdDetails.Rows[_rowIndex].Cells["Date"].Value = row["Date"];
                        dgrdDetails.Rows[_rowIndex].Cells["billtype"].Value = row["Billtype"];
                        dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = dba.ConvertObjectToDouble(row["netAmt"]).ToString("N2",MainPage.indianCurancy);
                        dgrdDetails.Rows[_rowIndex].Cells["billCode"].Value = row["BillCode"];
                        dgrdDetails.Rows[_rowIndex].Cells["billNo"].Value = row["BillNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["userName"].Value = row["UpdatedBy"];
                        dgrdDetails.Rows[_rowIndex].Cells["editStatus"].Value = row["editStatus"];
                        _rowIndex++;
                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdDetails.CurrentRow.Index;
                    if (dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
            catch { }
        }

        private void EditLogReport_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdDetails);           
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["sno"].Value = _index;
                    _index++;
                }
            }
            catch { }
        }
    }
}
