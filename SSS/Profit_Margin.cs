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
    public partial class Profit_Margin : Form
    {
        DataBaseAccess dba;
        string strProfitID = "";
        public Profit_Margin()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            BindProfitMarginDetails();
        }

        private void Profit_Margin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void BindProfitMarginDetails()
        {
            try
            {
                rdoDesignMaster.Checked = true;
                DataTable dt = dba.GetDataTable("Select  * from [dbo].[ProfitMargin] Where CompanyName='" + MainPage.strCompanyName + "' ");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strProfitID = Convert.ToString(row["ID"]);
                    if(Convert.ToBoolean(row["FixedProfit"]))
                    {
                        rdoFix.Checked = true;
                        txtFixedProfit.Text = Convert.ToString(row["FixedProfitRate"]);
                    }
                    else 
                        txtFixedProfit.Text = "0";
                   
                    if (Convert.ToBoolean(row["PurchaseBill"]))
                    {
                        rdoPurchaseBill.Checked = true;
                        txtPurchaseBillwise.Text = Convert.ToString(row["PurchaseBillRate"]);
                    }
                    else
                        txtPurchaseBillwise.Text = "0";
                    if (Convert.ToBoolean(row["Itemwise"]))
                    {
                        rdoItemWise.Checked = true;
                        txtItemWise.Text = Convert.ToString(row["ItemwiseRate"]);
                    }
                    else
                        txtItemWise.Text = "0";
                    if (Convert.ToBoolean(row["BrandWise"]))
                    {
                        rdoBrandWise.Checked = true;
                        txtBrandWise.Text = Convert.ToString(row["BrandWiseRate"]);
                    }
                    else
                        txtBrandWise.Text = "0";
                }
            }
            catch
            {
            }
        }

        private void txtFixedProfit_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
            catch { }
        }

        private void txtFixedProfit_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender,e, 2);
        }

        private void txtFixedProfit_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "")
                    txtNew.Text = "0.00";
            }
            catch { }
        }

        private bool ValidateControls()
        {            

            if (!rdoFix.Checked)
                txtFixedProfit.Clear();
            if (!rdoItemWise.Checked)
                txtItemWise.Clear();
            if (!rdoPurchaseBill.Checked)
                txtPurchaseBillwise.Clear();
           
            return true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to submit data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DialogResult.Yes == result)
                    {
                        SaveRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveRecord()
        {
            double dFixRate = 0, dPurchaseBillRate = 0, dItemRate = 0,dBrandRate=0;
            int _fixRate = 0, _purchaseBill = 0, _itemRate = 0,_brandRate=0;
            string strQuery = "";
            if (rdoFix.Checked)
            {
                dFixRate = dba.ConvertObjectToDouble(txtFixedProfit.Text);
                _fixRate = 1;
            }
            else if (rdoItemWise.Checked)
            {
                dItemRate = dba.ConvertObjectToDouble(txtItemWise.Text);
                _itemRate = 1;
            }
            else if (rdoPurchaseBill.Checked)
            {
                dPurchaseBillRate = dba.ConvertObjectToDouble(txtPurchaseBillwise.Text);
                _purchaseBill = 1;
            }
            else if (rdoBrandWise.Checked)
            {
                dBrandRate = dba.ConvertObjectToDouble(txtBrandWise.Text);
                _brandRate = 1;
            }

            strQuery = " If not exists (Select CompanyName from ProfitMargin Where CompanyName='" + MainPage.strCompanyName + "') begin INSERT INTO [dbo].[ProfitMargin] ([CompanyName],[FixedProfit],[FixedProfitRate],[PurchaseBill],[PurchaseBillRate],[Itemwise],[ItemwiseRate],[BrandWise],[BrandWiseRate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) Values "
                     + " ('" + MainPage.strCompanyName + "'," + _fixRate + "," + dFixRate + "," + _purchaseBill + "," + dPurchaseBillRate + "," + _itemRate + "," + dItemRate + "," + _brandRate + "," + dBrandRate + ",'" + MainPage.strLoginName + "','',1,0) end else begin UPDATE [dbo].[ProfitMargin] SET [FixedProfit]=" + _fixRate + ",[FixedProfitRate]=" + dFixRate + ",[PurchaseBill]=" + _purchaseBill + ",[PurchaseBillRate]=" + dPurchaseBillRate + ",[Itemwise]=" + _itemRate + ",[ItemwiseRate]=" + dItemRate + ",[BrandWise]="+_brandRate+ ",[BrandWiseRate]=" + dBrandRate + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where CompanyName='" + MainPage.strCompanyName + "' end ";

            int count = dba.ExecuteMyQuery(strQuery);

            if (count > 0)
            {
                MainPage._bFixedMargin = MainPage._bPurchaseBillWiseMargin = MainPage._bItemWiseMargin = MainPage._bBrandWiseMargin =MainPage._bDesignMasterMargin= false;
                MainPage.dFixedMargin = MainPage.dPurchaseBillMargin = MainPage.dItemwiseMargin = MainPage.dBrandwiseMargin = 0;

                if (rdoFix.Checked)
                {
                    MainPage._bFixedMargin = true;
                    MainPage.dFixedMargin = dFixRate;
                }
                else if (rdoPurchaseBill.Checked)
                {
                    MainPage._bPurchaseBillWiseMargin = true;
                    MainPage.dPurchaseBillMargin = dPurchaseBillRate;
                }
                else if (rdoItemWise.Checked)
                {
                    MainPage._bItemWiseMargin = true;
                    MainPage.dItemwiseMargin = dItemRate;
                }
                else if (rdoBrandWise.Checked)
                {
                    MainPage._bBrandWiseMargin = true;
                    MainPage.dBrandwiseMargin = dBrandRate;
                }
                else
                    MainPage._bDesignMasterMargin = true;
                MessageBox.Show("Thank you ! Record saved successfully ! ", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }
            else
                MessageBox.Show("Sorry ! Unable to save record ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void rdoFix_CheckedChanged(object sender, EventArgs e)
        {
            txtFixedProfit.Enabled = rdoFix.Checked;
            if (!rdoFix.Checked)
                txtFixedProfit.Text = "0";
        }

        private void rdoItemWise_CheckedChanged(object sender, EventArgs e)
        {
            txtItemWise.Enabled = rdoItemWise.Checked;
            if (!rdoItemWise.Checked)
                txtItemWise.Text = "0";
        }

        private void rdoPurchaseBill_CheckedChanged(object sender, EventArgs e)
        {
            txtPurchaseBillwise.Enabled = rdoPurchaseBill.Checked;
            if (!rdoPurchaseBill.Checked)
                txtPurchaseBillwise.Text = "0";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rdoBrandWise_CheckedChanged(object sender, EventArgs e)
        {
            txtBrandWise.Enabled = rdoBrandWise.Checked;
            if (!rdoBrandWise.Checked)
                txtBrandWise.Text = "0";

        }
    }
}
