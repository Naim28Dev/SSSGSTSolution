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
    public partial class Month_Lock : Form
    {
        DataBaseAccess dba;
        public Month_Lock()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            GetMonthData();
        }

        private void GetMonthData()
        {
            try
            {
                dgrdMonth.Rows.Clear();
                DataTable dt = dba.GetDataTable("Select * from MonthLockDetails Order by ID asc");
                if (dt.Rows.Count > 0)
                {
                    dgrdMonth.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                    foreach(DataRow row in dt.Rows)
                    {
                        dgrdMonth.Rows[_rowIndex].Cells["monthName"].Value = row["MonthName"];
                        dgrdMonth.Rows[_rowIndex].Cells["status"].Value = row["Status"];
                        if (Convert.ToString(row["Status"]) == "LOCK")
                            dgrdMonth.Rows[_rowIndex].Cells["chkStatus"].Value = true;
                        else
                            dgrdMonth.Rows[_rowIndex].Cells["chkStatus"].Value = false;
                        _rowIndex++;
                    }
                }
            }
            catch { }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainPage.strUserRole == "ADMIN" || MainPage.strUserRole == "SUPERADMIN")
                {
                    dgrdMonth.EndEdit();
                    DialogResult dr = MessageBox.Show("Are you sure want to lock/unlock month ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {

                        UpdateRecords();
                    }
                }
            }
            catch { }
        }

        private void UpdateRecords()
        {
            string strQuery = "";
            foreach(DataGridViewRow row in dgrdMonth.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
                    strQuery += " Update MonthLockDetails Set Status='LOCK',Date=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) Where Status='UNLOCK' and MonthName='" + row.Cells["monthName"].Value + "' ";
                else
                    strQuery += " Update MonthLockDetails Set Status='UNLOCK',Date=DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) Where Status='LOCK' and MonthName='" + row.Cells["monthName"].Value + "' ";
            }

            if(strQuery !="")
            {
                int _count = dba.ExecuteMyQuery(strQuery);
                if(_count>0)
                {
                    MessageBox.Show("Status changed successfully  ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    GetMonthData();
                }
                else
                    MessageBox.Show("Sorry Unable to update records ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Month_Lock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdMonth_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
                e.Cancel = true;
        }
    }
}
