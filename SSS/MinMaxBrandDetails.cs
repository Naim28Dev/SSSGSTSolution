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
    public partial class MinMaxBrandDetails : Form
    {
        DataBaseAccess dba;
        public MinMaxBrandDetails()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MinMaxBrandDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void GetRecord()
        {
            try
            {
                string strSubQuery = "";
                if (rdoMin.Checked)
                    strSubQuery += " Where NetQty<MinStock ";
                else if (rdoMaxStock.Checked)
                    strSubQuery += " Where NetQty>MaxStock ";

                string strQuery = " Select * from (Select BrandName,ISNULL(MinStock,0)MinStock,ISNULL(MaxStock,0)MaxStock,SUM(Qty)NetQty from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,Qty from StockMaster SM Inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN') UNION ALL Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,-Qty from StockMaster SM Inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('PURCHASERETURN','SALES') )Stock CROSS APPLY (Select MinStock,MaxStock from BrandMaster BM WHere BM.BrandName=Stock.BrandName and (MinStock>0 OR MaxStock>0))BM Group by BrandName,MinStock,MaxStock)_Stock " + strSubQuery+" Order by BrandName  ";
                DataTable dt = dba.GetDataTable(strQuery);
                dgrdDetails.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    int _index = 0;
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_index].Cells["sno"].Value = (_index + 1);
                        dgrdDetails.Rows[_index].Cells["brandName"].Value = row["BrandName"];
                        dgrdDetails.Rows[_index].Cells["minStock"].Value = dba.ConvertObjectToDouble(row["MinStock"]);
                        dgrdDetails.Rows[_index].Cells["maxStock"].Value = dba.ConvertObjectToDouble(row["MaxStock"]) ;
                        dgrdDetails.Rows[_index].Cells["currentStock"].Value = dba.ConvertObjectToDouble(row["NetQty"]);
                        _index++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            GetRecord();
        }

        private void MinMaxBrandDetails_Load(object sender, EventArgs e)
        {
            GetRecord();
        }
    }
}
