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
    public partial class NewCategory : Form
    {
        DataBaseAccess dba;
        public string strName = "";
        public NewCategory()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void NewCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 27)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Key Down Event Forms  in New Category", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtName.Text != "")
                {
                    dba = new DataBaseAccess();
                    string[] record = new string[4];
                    record[0] = txtName.Text;
                    record[1] = txtDiscountCr.Text;
                    record[2] = txtDiscountDr.Text;
                    int count=dba.SaveCategory(record);
                    if (count > 0)
                    {
                        strName = txtName.Text;
                        MessageBox.Show("Thank you ! Category Saved successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Category Name can't be Blank");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Submit Button  in New Category", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtDiscountCr_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
