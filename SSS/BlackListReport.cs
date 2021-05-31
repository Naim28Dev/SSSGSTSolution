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
    public partial class BlackListReport : Form
    {
        public BlackListReport()
        {
            InitializeComponent();
            GetAllData();
        }

        private void BlackListReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void GetAllData()
        {
            try
            {
                dgrdBlackList.Rows.Clear();
                dgrdTransactionLock.Rows.Clear();
                DataTable table = DataBaseAccess.GetDataTableRecord("Select (AreaCode+Cast(AccountNo as varchar)+' '+Name) Name,UPPER(GroupName)GroupName,TransactionLock,BlackList,BlackListReason,UpdatedBy from SupplierMaster Where TransactionLock=1 OR BlackList=1  order By Name");
                DataRow[] rows = table.Select(String.Format(" BlackList=1 "));
                if (rows.Length > 0)
                {
                    dgrdBlackList.Rows.Add(rows.Length);
                    int rowIndex = 0;
                    foreach (DataRow row in rows)
                    {
                        dgrdBlackList.Rows[rowIndex].Cells["sno"].Value = (rowIndex + 1) + ".";
                        dgrdBlackList.Rows[rowIndex].Cells["partyName"].Value = row["Name"];
                        dgrdBlackList.Rows[rowIndex].Cells["groupName"].Value = row["GroupName"];
                        dgrdBlackList.Rows[rowIndex].Cells["blackReason"].Value = row["BlackListReason"];
                        dgrdBlackList.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                        rowIndex++;
                    }
                }

                DataRow[] trows = table.Select(String.Format(" TransactionLock=1 "));
                if (trows.Length > 0)
                {
                    dgrdTransactionLock.Rows.Add(trows.Length);
                    int rowIndex = 0;
                    foreach (DataRow row in trows)
                    {
                        dgrdTransactionLock.Rows[rowIndex].Cells["tSno"].Value = (rowIndex+1) + ".";
                        dgrdTransactionLock.Rows[rowIndex].Cells["tPartyName"].Value = row["Name"];
                        dgrdTransactionLock.Rows[rowIndex].Cells["tGroupName"].Value = row["GroupName"];
                        dgrdTransactionLock.Rows[rowIndex].Cells["tUpdatedBy"].Value = row["UpdatedBy"];
                        rowIndex++;
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdBlackList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}
