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
    public partial class MergingParty : Form
    {
        DataBaseAccess dba;
        public MergingParty()
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("SALESPARTY");
                    if (strData != "")
                        txtFSalesParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFSalesParty.Text = strData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("SALESPARTY");
                    if (strData != "")
                        txtSSalesParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtSSalesParty.Text = strData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("SALESPARTY");
                    if (strData != "")
                        txtFinalSalesParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFinalSalesParty.Text = strData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                    if (strData != "")
                        txtFPurchaseParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFPurchaseParty.Text = strData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                    if (strData != "")
                        txtSPurchaseParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtSPurchaseParty.Text = strData;
                    }
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
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                    if (strData != "")
                        txtFinalPurchaseParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFinalPurchaseParty.Text = strData;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtFOtherParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("OTHERPARTY");
                    if (strData != "")
                        txtFOtherParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("OTHERPARTY", "SEARCH OTHER PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFOtherParty.Text = strData;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtSOtherParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("OTHERPARTY");
                    if (strData != "")
                        txtSOtherParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("OTHERPARTY", "SEARCH OTHER PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtSOtherParty.Text = strData;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtFinalOtherParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("OTHERPARTY");
                    if (strData != "")
                        txtFinalOtherParty.Text = strData;
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("OTHERPARTY", "SEARCH OTHER PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData;
                        if (strData != "")
                            txtFinalOtherParty.Text = strData;
                    }
                }
            }
            catch
            {
            }
        }

        private bool ValidateSalesParty()
        {
            if (txtFSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! First Sundry Debtors is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFSalesParty.Focus();
                return false;
            }
            if (txtSSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Second Sundry Debtors is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSSalesParty.Focus();
                return false;
            }
            if (txtFinalSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Final Sundry Debtors is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalSalesParty.Focus();
                return false;
            }
           
            return true;
        }

        private bool ValidatePurchaseParty()
        {
            if (txtFPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! First Sundry Creditor is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFPurchaseParty.Focus();
                return false;
            }
            if (txtSPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! Second Sundry Creditor is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSPurchaseParty.Focus();
                return false;
            }
            if (txtFinalPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! Final Sundry Creditor is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalPurchaseParty.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateOtherParty()
        {
            if (txtFOtherParty.Text == "")
            {
                MessageBox.Show("Sorry ! First Other party is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFOtherParty.Focus();
                return false;
            }
            if (txtSOtherParty.Text == "")
            {
                MessageBox.Show("Sorry ! Second Other party is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSOtherParty.Focus();
                return false;
            }
            if (txtFinalOtherParty.Text == "")
            {
                MessageBox.Show("Sorry ! Final Other party is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalOtherParty.Focus();
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
                if (ValidateSalesParty())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these Parties in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeSalesParty();
                }
            }
            catch
            {
            }
            btnSMerge.Enabled = true ;
            btnSMerge.Text = "&Merge Sales Parties";
        }

        private void MergeSalesParty()
        {
            double dAmt = 0;
            //string strQuery = "", strStatus = "DEBIT", strOldParty = "";
            //dAmt = dba.GetCombinedOpeningBalance(txtFSalesParty.Text, txtSSalesParty.Text, txtFinalSalesParty.Text);
            //if (dAmt < 0)
            //{
            //    dAmt = Math.Abs(dAmt);
            //    strStatus = "CREDIT";
            //}
            // strOldParty = " '" + txtFSalesParty.Text + "','" + txtSSalesParty.Text + "' ";
            string strSaleParty = "", strFirstName = "", strSecondName = "";
            string[] strFullName = txtFinalSalesParty.Text.Split(' ');
            if (strFullName.Length > 1)
            {
                strSaleParty = strFullName[0].Trim();
                strFullName = txtFSalesParty.Text.Split(' ');
                strFirstName = strFullName[0].Trim();
                strFullName = txtSSalesParty.Text.Split(' ');
                strSecondName = strFullName[0].Trim();
                //strOldParty = " '" + strFirstName + "','" + strSecondName + "'";

                //strQuery = " Update BalanceAmount set AccountID='" + strSaleParty + "',UpdateStatus=1 where  AccountID in (" + strOldParty + ") and AccountStatus != 'OPENING'"
                //             + " Update BalanceAmount set AccountStatusID='" + strSaleParty + "',UpdateStatus=1 where  AccountStatusID in (" + strOldParty + ")"
                //             + "  Update JournalAccount set DebitPartyID='" + strSaleParty + "',UpdateStatus=1 where  DebitPartyID in (" + strOldParty + ")"
                //             + "  Update JournalAccount set CreditPartyID='" + strSaleParty + "',UpdateStatus=1 where  CreditPartyID in (" + strOldParty + ")"
                //             + "  Update OrderBooking set SalePartyID='" + strSaleParty + "',UpdateStatus=1 where  SalePartyID in (" + strOldParty + ")"
                //             + "  Update GoodsReceive set SalePartyID='" + strSaleParty + "',UpdateStatus=1 where  SalePartyID in (" + strOldParty + ")"
                //             + "  Update SalesRecord set SalePartyID='" + strSaleParty + "',UpdateStatus=1 where  SalePartyID in (" + strOldParty + ")"
                //             + "  Update PurchaseRecord set SalePartyID='" + strSaleParty + "',UpdateStatus=1 where  SalePartyID in (" + strOldParty + ")"
                //             + "  Update GoodsReturned set SalePartyID='" + strSaleParty + "',UpdateStatus=1 where  SalePartyID in (" + strOldParty + ")"
                //             //+ "  Update ForwardingRecord set Buyer='" + strSaleParty + "',UpdateStatus=1 where  Buyer in (" + strOldParty + ")"
                //             + "  Update SupplierMaster set OpeningBal='" + dAmt + "' , Status='" + strStatus + "',UpdateStatus=1 where (AreaCode+Cast(AccountNo as varchar)) = '" + strSaleParty + "'"
                //             + "  Update SupplierMaster set HasteSale='" + strSaleParty + "' ,UpdateStatus=1 where  HasteSale in (" + strOldParty + ")"
                //             + "  Update BalanceAmount set Amount ='" + dAmt + "' ,Description='FORWARDED', Status='" + strStatus + "',UpdateStatus=1 where PartyName= '" + strSaleParty + "' and AccountStatus='OPENING'"
                //             + "  Update BiltyDetail set PartyName ='" + strSaleParty + "' ,UpdateStatus=1 where  dbo.GetFullName(PartyName) in (" + strOldParty + ") "
                //             + "  Delete from BalanceAmount Where AccountID in (" + strOldParty + ") and AccountStatus='OPENING' and  AccountID !='" + strSaleParty + "'"
                //             + "  Delete from SupplierMaster where (AreaCode+Cast(AccountNo as varchar)) in (" + strOldParty + ") and (AreaCode+Cast(AccountNo as varchar)) !='" + strSaleParty + "'";

                int count = dba.MergePartyName(strFirstName, strSecondName, strSaleParty, "SUNDRY DEBTORS", true);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Party Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtFSalesParty.Text = txtSSalesParty.Text = txtFinalSalesParty.Text = "";
                }
                else
                    MessageBox.Show("Sorry ! An Error occured in merging party name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MergePurchaseParty()
        {
            //double dAmt = 0;
            //string strQuery = "", strStatus = "DEBIT", strOldParty = "";
            //dAmt = dba.GetCombinedOpeningBalance(txtFPurchaseParty.Text, txtSPurchaseParty.Text, txtFinalPurchaseParty.Text);
            //if (dAmt < 0)
            //{
            //    dAmt = Math.Abs(dAmt);
            //    strStatus = "CREDIT";
            //}
            //     strOldParty = " '" + txtFPurchaseParty.Text + "','" + txtSPurchaseParty.Text + "' ";
            string strParty = "", strFirstName = "", strSecondName = "";
            string[] strFullName = txtFinalPurchaseParty.Text.Split(' ');
            if (strFullName.Length > 1)
            {
                strParty = strFullName[0].Trim();
                strFullName = txtFPurchaseParty.Text.Split(' ');
                strFirstName = strFullName[0].Trim();
                strFullName = txtSPurchaseParty.Text.Split(' ');
                strSecondName = strFullName[0].Trim();
               // strOldParty = " '" + strFirstName + "','" + strSecondName + "'";


                int count = dba.MergePartyName(strFirstName, strSecondName, strParty, "SUNDRY CREDITOR", true);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Party Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtFPurchaseParty.Text = txtSPurchaseParty.Text = txtFinalPurchaseParty.Text = "";
                }
                else
                    MessageBox.Show("Sorry ! An Error occured in merging party name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MergeOtherParty()
        {
            //double dAmt = 0;
            //string strQuery = "", strStatus = "DEBIT", strOldParty = "";
            //dAmt = dba.GetCombinedOpeningBalance(txtFOtherParty.Text, txtSOtherParty.Text, txtFinalOtherParty.Text);
            //if (dAmt < 0)
            //{
            //    dAmt = Math.Abs(dAmt);
            //    strStatus = "CREDIT";
            //}
         //  strOldParty = " '" + txtFOtherParty.Text + "','" + txtSOtherParty.Text + "' ";
            string strParty = "",strFirstName="",strSecondName="";
            string[] strFullName = txtFinalOtherParty.Text.Split(' ');
            if (strFullName.Length > 1)
            {
                strParty = strFullName[0].Trim();
                strFullName = txtFOtherParty.Text.Split(' ');
                strFirstName = strFullName[0].Trim();
                strFullName = txtSOtherParty.Text.Split(' ');
                strSecondName = strFullName[0].Trim();
                //strOldParty = " '" + strFirstName + "','" + strSecondName + "'";


                //strQuery = " Update BalanceAmount set AccountID='" + strParty + "',UpdateStatus=1 where  AccountID in (" + strOldParty + ") and AccountStatus != 'OPENING'"
                //             + " Update BalanceAmount set AccountStatusID='" + strParty + "',UpdateStatus=1 where  AccountStatusID in (" + strOldParty + ")"
                //             + "  Update JournalAccount set DebitPartyID='" + strParty + "',UpdateStatus=1 where  DebitPartyID in (" + strOldParty + ")"
                //             + "  Update JournalAccount set CreditPartyID='" + strParty + "',UpdateStatus=1 where  CreditPartyID in (" + strOldParty + ")"
                //             + "  Update SupplierMaster set OpeningBal='" + dAmt + "' , Status='" + strStatus + "',UpdateStatus=1 where (AreaCode+Cast(AccountNo as varchar)) = '" + strParty + "'"
                //             + "  Update BalanceAmount set Amount ='" + dAmt + "' ,Description='FORWARDED', Status='" + strStatus + "',UpdateStatus=1 where AccountID= '" + strParty + "' and AccountStatus='OPENING'"
                //             + "  Delete from BalanceAmount Where AccountID in (" + strOldParty + ") and AccountStatus='OPENING' and  AccountID !='" + strParty + "'"
                //             + "  Delete from SupplierMaster where (AreaCode+Cast(AccountNo as varchar)) in (" + strOldParty + ") and (AreaCode+Cast(AccountNo as varchar)) !='" + strParty + "'";

                int count = dba.MergePartyName(strFirstName, strSecondName, strParty, "OTHER PARTY", true);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Party Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    txtFOtherParty.Text = txtSOtherParty.Text = txtFinalOtherParty.Text = "";
                }
                else
                    MessageBox.Show("Sorry ! An Error occured in merging party name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
               
        private void btnPMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnPMerge.Text = "Please wait ..";
                btnPMerge.Enabled = false;
                if (ValidatePurchaseParty())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these Parties in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergePurchaseParty();
                }
            }
            catch
            {
            }
            btnPMerge.Enabled = true;
            btnPMerge.Text = "&Merge Purchase Parties";
        }

        private void btnOMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnOMerge.Text = "Please wait ..";
                btnOMerge.Enabled = false;
                if (ValidateOtherParty())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these Parties in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeOtherParty();
                }
            }
            catch
            {
            }
            btnOMerge.Enabled = true;
            btnOMerge.Text = "&Merge Other Parties";
        }

        private void btnSCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
