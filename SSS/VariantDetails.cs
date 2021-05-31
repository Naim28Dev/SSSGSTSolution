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
    public partial class VariantDetails : Form
    {
        DataBaseAccess dba;
        string strCatID = "";
        public VariantDetails()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindCategoryDetails();
        }

        private void BindCategoryDetails()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select  * from CategoryDetails ");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    strCatID = Convert.ToString(row["ID"]);
                    txtCategory1.Text = Convert.ToString(row["CategoryName1"]);
                    txtCategory2.Text = Convert.ToString(row["CategoryName2"]);
                    txtCategory3.Text = Convert.ToString(row["CategoryName3"]);
                    txtCategory4.Text = Convert.ToString(row["CategoryName4"]);
                    txtCategory5.Text = Convert.ToString(row["CategoryName5"]);

                    if (txtCategory1.Text != "")
                        chkCategory1.Checked = true;
                    if (txtCategory2.Text != "")
                        chkCategory2.Checked = true;
                    if (txtCategory3.Text != "")
                        chkCategory3.Checked = true;
                    if (txtCategory4.Text != "")
                        chkCategory4.Checked = true;
                    if (txtCategory5.Text != "")
                        chkCategory5.Checked = true;

                }
            }
            catch
            {
            }
        }

        private void CategoryDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void chkCategory1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCategory1.Checked)
            {
                txtCategory1.ReadOnly = false;
                lblCat1.Visible = true;
            }
            else
            {
                txtCategory1.ReadOnly = true;
                lblCat1.Visible = false;
            }
        }

        private void chkCategory2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCategory2.Checked)
            {
                txtCategory2.ReadOnly = false;
                lblCat2.Visible = true;
            }
            else
            {
                txtCategory2.ReadOnly = true;
                lblCat2.Visible = false;
            }
        }

        private void chkCategory3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCategory3.Checked)
            {
                txtCategory3.ReadOnly = false;
                lblCat3.Visible = true;
            }
            else
            {
                txtCategory3.ReadOnly = true;
                lblCat3.Visible = false;
            }
        }

        private void chkCategory4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCategory4.Checked)
            {
                txtCategory4.ReadOnly = false;
                lblCat4.Visible = true;
            }
            else
            {
                txtCategory4.ReadOnly = true;
                lblCat4.Visible = false;
            }
        }

        private void chkCategory5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCategory5.Checked)
            {
                txtCategory5.ReadOnly = false;
                lblCat5.Visible = true;
            }
            else
            {
                txtCategory5.ReadOnly = true;
                lblCat5.Visible = false;
            }
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

        private bool ValidateControls()
        {
            if (chkCategory1.Checked && txtCategory1.Text == "")
            {
                MessageBox.Show("Sorry ! Please fill variant 1 !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory1.Focus();
                return false;
            }
            if (chkCategory2.Checked && txtCategory2.Text == "")
            {
                MessageBox.Show("Sorry ! Please fill variant 2 !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory2.Focus();
                return false;
            }
            if (chkCategory3.Checked && txtCategory3.Text == "")
            {
                MessageBox.Show("Sorry ! Please fill variant 3 !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory3.Focus();
                return false;
            }
            if (chkCategory4.Checked && txtCategory4.Text == "")
            {
                MessageBox.Show("Sorry ! Please fill variant 4 !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory4.Focus();
                return false;
            }
            if (chkCategory5.Checked && txtCategory5.Text == "")
            {
                MessageBox.Show("Sorry ! Please fill variant 5 !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategory5.Focus();
                return false;
            }

            if (!chkCategory1.Checked)
                txtCategory1.Clear();
            if (!chkCategory2.Checked)
                txtCategory2.Clear();
            if (!chkCategory3.Checked)
                txtCategory3.Clear();
            if (!chkCategory4.Checked)
                txtCategory4.Clear();
            if (!chkCategory5.Checked)
                txtCategory5.Clear();
            return true;
        }

        private void SaveRecord()
        {
            string strQuery = "", strID = "";
            if (strCatID != "")
            {
                strQuery = " UPDATE [dbo].[CategoryDetails] SET [CategoryName1]='" + txtCategory1.Text + "',[CategoryName2]='" + txtCategory2.Text + "', "
                               + " [CategoryName3]='" + txtCategory3.Text + "',[CategoryName4]='" + txtCategory4.Text + "',[CategoryName5]='" + txtCategory5.Text + "' Where ID=" + strCatID + " ";
            }
            else
            {
                strQuery = "INSERT INTO [dbo].[CategoryDetails]([CategoryName1],[CategoryName2],[CategoryName3],[CategoryName4],[CategoryName5]) OUTPUT INSERTED.ID VALUES"
                                + " ('" + txtCategory1.Text + "','" + txtCategory2.Text + "','" + txtCategory3.Text + "','" + txtCategory4.Text + "','" + txtCategory5.Text + "') ";
            }

            if (strCatID == "")
            {
                object objID = DataBaseAccess.ExecuteMyScalar(strQuery);
                strID = Convert.ToString(objID);
            }
            else
            {
                int count = dba.ExecuteMyQuery(strQuery);
                strID = count.ToString();
            }
            if (strID != "" && strID != "0")
            {
                MessageBox.Show("Thank you ! Record saved successfully ! ", "Successfull", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                if (strCatID == "")
                    strCatID = strID;
                SetCategoryDetails();
            }
            else
                MessageBox.Show("Sorry ! Unable to save record ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetCategoryDetails()
        {
            MainPage.StrCategory1 = MainPage.StrCategory2 = MainPage.StrCategory3 = MainPage.StrCategory4 = MainPage.StrCategory5 = "";
            if (chkCategory1.Checked)
                MainPage.StrCategory1 = txtCategory1.Text;
            if (chkCategory2.Checked)
                MainPage.StrCategory2 = txtCategory2.Text;
            if (chkCategory3.Checked)
                MainPage.StrCategory3 = txtCategory3.Text;
            if (chkCategory4.Checked)
                MainPage.StrCategory4 = txtCategory4.Text;
            if (chkCategory5.Checked)
                MainPage.StrCategory5 = txtCategory5.Text;

            DataBaseAccess.SetCategoryData();
            SetVariantInAllOpenPage();
        }

        private void SetVariantInAllOpenPage()
        {
            try {
                foreach (Form childForm in MainPage.mymainObject.MdiChildren)
                {
                    if (childForm.Name.Contains("PurchaseBook_Retail"))
                    {
                        PurchaseBook_Retail_Merge objP = (PurchaseBook_Retail_Merge)childForm;
                        DataGridView dgrd = objP.dgrdDetails;
                        if (dgrd != null)
                            SetCategoryInGrid(dgrd);

                    }
                    else if (childForm.Name == "SaleBook_Retail")
                    {
                        SaleBook_Retail objP = (SaleBook_Retail)childForm;
                        DataGridView dgrd = objP.dgrdDetails;
                        if (dgrd != null)
                            SetCategoryInGrid(dgrd);

                    }
                }
            }
            catch { }
        }

        private void SetCategoryInGrid(DataGridView dgrdDetails)
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant2"].Visible = false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["variant3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["variant4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["variant5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant5"].Visible = false;
            }
            catch
            {
            }
        }
    }
}
