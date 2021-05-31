using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class Active_InactiveCustomers : Form
    {
        DataBaseAccess dba;
        public Active_InactiveCustomers()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
           
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Active_InactiveCustomers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = "", strSubQuery = "";
                if (rdoActive.Checked)
                    strSubQuery = " and SAmt>0 ";
                else if (rdoInactive.Checked)
                    strSubQuery = " and SAmt=0 ";
                strQuery = "Select * from (Select (AreaCode+AccountNo+' '+Name)CustomerName,Station,MobileNo,(ISNULL((Select SUM(Amt)SAmt from (  Select SUM(CAST(NetAmt as money)) Amt from SalesRecord SR Where BillDate>(DATEADD(dd,-90,Getdate())) and SR.SalePartyID = (AreaCode + AccountNo)UNION ALL Select SUM(NetAmt) Amt from SalesBook SR Where Date>(DATEADD(dd,-90,Getdate())) and SR.SalePartyID = (AreaCode + AccountNo))Sales),0)) SAmt from SupplierMaster SM Where GroupName = 'SUNDRY DEBTORS' UNION ALL Select (SalePartyID) as CustomerName, (Station) as Station, MobileNo, SUM(NetAmt)SAmt from SalesBook WHere Date>(DATEADD(dd,-90,Getdate())) and SalePartyID not in (Select(AreaCode + AccountNo) from SupplierMaster WHere GroupName = 'SUNDRY DEBTORS') "
                         + " Group by MobileNo,SalePartyID,Station)_Sales WHere CustomerName != ''  " + strSubQuery + "  Order by SAmt desc ";

                //strQuery = "Select * from (Select (AreaCode+AccountNo+' '+Name)CustomerName,Station,MobileNo,SUM(ISNULL(SAmt,0)) SAmt from SupplierMaster SM OUTER APPLY (Select SUM(Amt)SAmt from ( "
                //         + " Select SUM(CAST(NetAmt as money)) Amt from SalesRecord SR Where SR.SalePartyID = AreaCode + AccountNo UNION ALL "
                //         + " Select SUM(NetAmt) Amt from SalesBook SR Where SR.SalePartyID = AreaCode + AccountNo)Sales)_Sales Where GroupName = 'SUNDRY DEBTORS' Group by (AreaCode + AccountNo + ' ' + Name), Station, MobileNo "
                //         + " UNION ALL Select SalePartyID as CustomerName, Station, MobileNo, SUM(NetAmt)SAmt from SalesBook WHere SalePartyID not in (Select(AreaCode + AccountNo) from SupplierMaster WHere GroupName = 'SUNDRY DEBTORS') Group by SalePartyID,Station,MobileNo "
                //         + " )_Sales WHere CustomerName != '' " + strSubQuery + " Order by SAmt desc ";

                DataTable dt = dba.GetDataTable(strQuery);
                BindDataWithGrid(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            int _index = 0, activeCount = 0, inactiveCount = 0;
            dgrdDetails.Rows.Clear();
            double dSAmt = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dSAmt = dba.ConvertObjectToDouble(row["SAmt"]);
                    dgrdDetails.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                    dgrdDetails.Rows[_index].Cells["customerName"].Value = row["CustomerName"];
                    dgrdDetails.Rows[_index].Cells["station"].Value = row["Station"];
                    dgrdDetails.Rows[_index].Cells["mobileNo"].Value = row["MobileNo"];
                    dgrdDetails.Rows[_index].Cells["saleAmt"].Value = dSAmt;
                    if (dSAmt > 0)
                    {
                        activeCount++;
                        dgrdDetails.Rows[_index].Cells["status"].Value = "ACTIVE";
                        dgrdDetails.Rows[_index].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        inactiveCount++;
                        dgrdDetails.Rows[_index].Cells["status"].Value = "INACTIVE";
                        dgrdDetails.Rows[_index].DefaultCellStyle.BackColor = Color.Tomato;
                    }
                    _index++;
                }
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            GetDataFromDB();
        }

        private void Active_InactiveCustomers_Load(object sender, EventArgs e)
        {
            GetDataFromDB();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex==1 && e.RowIndex>=0)
                {
                    string strPartyName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    DataBaseAccess.OpenPartyMaster(strPartyName);
                }
            }
            catch { }
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
