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
    public partial class MergingItems : Form
    {
        DataBaseAccess dba;
        public MergingItems()
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

        private string GetGroupSearchQuery()
        {
            string strQuery = "";
            if (txtFGroupName.Text != "")
                strQuery += " Where TaxCategoryName in (Select TaxCategoryName from ItemGroupMaster Where GroupName='"+ txtFGroupName.Text+"') ";
            else if (txtSGroupName.Text != "")
                strQuery += " Where TaxCategoryName in (Select TaxCategoryName from ItemGroupMaster Where GroupName='" + txtSGroupName.Text + "') ";
            else if (txtFinalGroupName.Text != "")
                strQuery += " Where TaxCategoryName in (Select TaxCategoryName from ItemGroupMaster Where GroupName='" + txtFinalGroupName.Text + "') ";

            return strQuery;
        }

        private string GetItemSearchQuery()
        {
            string strQuery = "";
            if (txtFGroupName.Text != "")
                strQuery += "  Where GroupName in (Select GroupName from Items Where ItemName='" + txtFGroupName.Text + "') ";
            else if (txtSGroupName.Text != "")
                strQuery += "  Where GroupName in (Select GroupName from Items Where ItemName='" + txtSGroupName.Text + "') ";
            else if (txtFinalGroupName.Text != "")
                strQuery += "  Where GroupName in (Select GroupName from Items Where ItemName='" + txtFinalGroupName.Text + "') ";

            return strQuery;
        }

        private void txtFSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    string strQuery = GetGroupSearchQuery();
                    SearchData objSearch = new SearchData("GROUPNAMEFORMERGE", strQuery, "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtFGroupName.Text = strData;
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
                    string strQuery = GetGroupSearchQuery();
                    SearchData objSearch = new SearchData("GROUPNAMEFORMERGE", strQuery, "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtSGroupName.Text = strData;
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
                    string strQuery = GetGroupSearchQuery();
                    SearchData objSearch = new SearchData("GROUPNAMEFORMERGE", strQuery, "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtFinalGroupName.Text = strData;
                }
            }
            catch
            {
            }
        }

        private void txtFPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    string strQuery = GetItemSearchQuery();
                    SearchData objSearch = new SearchData("ITEMNAMEFORMERGE", strQuery, "SEARCH GROUP NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtFItemName.Text = strData;
                }
            }
            catch
            {
            }
        }

        private void txtSPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    string strQuery = GetItemSearchQuery();
                    SearchData objSearch = new SearchData("ITEMNAMEFORMERGE", strQuery, "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                        txtSItemName.Text = strData;
                }
            }
            catch
            {
            }
        }

        private void txtFinalPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {               
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                    string strQuery = GetItemSearchQuery();
                    SearchData objSearch = new SearchData("ITEMNAMEFORMERGE", strQuery, "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFinalItemName.Text = strData;
                    }                
            }
            catch
            {
            }
        }        

        private bool ValidateGroupName()
        {
            if (txtFGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! First Group Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFGroupName.Focus();
                return false;
            }
            if (txtSGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! Second Group Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSGroupName.Focus();
                return false;
            }
            if (txtFinalGroupName.Text == "")
            {
                MessageBox.Show("Sorry ! Final Group Name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalGroupName.Focus();
                return false;
            }
           
            return true;
        }

        private bool ValidateItemName()
        {
            if (txtFItemName.Text == "")
            {
                MessageBox.Show("Sorry ! First Item name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFItemName.Focus();
                return false;
            }
            if (txtSItemName.Text == "")
            {
                MessageBox.Show("Sorry ! Second Item name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSItemName.Focus();
                return false;
            }
            if (txtFinalItemName.Text == "")
            {
                MessageBox.Show("Sorry ! Final Item name is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalItemName.Focus();
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
                if (ValidateGroupName())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these Parties in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeGroupName();
                }
            }
            catch
            {
            }
            btnSMerge.Enabled = true ;
            btnSMerge.Text = "&Merge Item Group Name";
        }

        private void MergeGroupName()
        {
           
            int count = dba.MergeItemGroupName(txtFGroupName.Text, txtSGroupName.Text, txtFinalGroupName.Text, "GROUPNAME", true);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Group Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtFGroupName.Text = txtSGroupName.Text = txtFinalGroupName.Text = "";
            }
            else
                MessageBox.Show("Sorry ! An Error occured in merging group name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }

        private void MergeItemName()
        {
            int count = dba.MergeItemGroupName(txtFItemName.Text, txtSItemName.Text, txtFinalItemName.Text, "ITEMNAME", true);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Item Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtFItemName.Text = txtSItemName.Text = txtFinalItemName.Text = "";
            }
            else
                MessageBox.Show("Sorry ! An Error occured in merging item name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnPMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnPMerge.Text = "Please wait ..";
                btnPMerge.Enabled = false;
                if (ValidateItemName())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these items in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeItemName();
                }
            }
            catch
            {
            }
            btnPMerge.Enabled = true;
            btnPMerge.Text = "&Merge Item Name";
        }
        
        private void btnSCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
