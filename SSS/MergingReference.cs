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
    public partial class MergingReference : Form
    {
        DataBaseAccess dba;
        public MergingReference()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void MergingParty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }
        
        private void txtFSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REFERENCENAME", "SEARCH REFERENCE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtFReferenceName.Text = strData;
                }
            }
            catch
            {
            }
        }

        private void txtSSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REFERENCENAME", "SEARCH REFERENCE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtSReferenceName.Text = strData;
                }
            }
            catch
            {
            }
        }

        private void txtFinalSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("REFERENCENAME", "SEARCH REFERENCE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtFinalReferenceName.Text = strData;
                }
            }
            catch
            {
            }
        }        

        private bool ValidateReferenceName()
        {
            if (txtFReferenceName.Text == "")
            {
                MessageBox.Show("Sorry ! First Reference Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFReferenceName.Focus();
                return false;
            }
            if (txtSReferenceName.Text == "")
            {
                MessageBox.Show("Sorry ! Second Reference Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSReferenceName.Focus();
                return false;
            }
            if (txtFinalReferenceName.Text == "")
            {
                MessageBox.Show("Sorry ! Final Reference Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalReferenceName.Focus();
                return false;
            }
           
            return true;
        }
        
        
        private void btnSMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnSMerge.Text = "Please wait ..";
                btnSMerge.Enabled = false;
                if (ValidateReferenceName())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these reference in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeReferenceName();
                }
            }
            catch
            {
            }
            btnSMerge.Enabled = true ;
            btnSMerge.Text = "&Merge Reference Name";
        }

        private void MergeReferenceName()
        {
           
            int count = dba.MergeReferenceName(txtFReferenceName.Text.Trim(), txtSReferenceName.Text.Trim(), txtFinalReferenceName.Text.Trim(), true);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Reference Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtFReferenceName.Text = txtSReferenceName.Text = txtFinalReferenceName.Text = "";
            }
            else
                MessageBox.Show("Sorry ! An Error occured in merging group name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }       

        
        private void btnSCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}
