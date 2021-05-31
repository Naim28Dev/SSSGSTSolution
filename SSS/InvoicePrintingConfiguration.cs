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
    public partial class InvoicePrintingConfiguration : Form
    {
        DataBaseAccess dba;
        public InvoicePrintingConfiguration()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetSavedRecord();
        }

        private void InvoicePrintingConfiguration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter && !txtSaleDeclaration.Focused && !txtSaleRtnDeclaration.Focused && !txtSaleServDeclaration.Focused && !txtPurchaseRtnDeclaration.Focused)
                SendKeys.Send("{TAB}");
        }

        private void GetSavedRecord()
        {
            try
            {
                //if (MainPage.StrCategory1 != "")
                //{
                //    chkCategory1.Text = MainPage.StrCategory1;
                //    chkCategory1.Enabled = true;
                //}
                //if (MainPage.StrCategory2 != "")
                //{
                //    chkCategory2.Text = MainPage.StrCategory2;
                //    chkCategory2.Enabled = true;
                //}
                //if (MainPage.StrCategory3 != "")
                //{
                //    chkCategory3.Text = MainPage.StrCategory3;
                //    chkCategory3.Enabled = true;
                //}
                //if (MainPage.StrCategory4 != "")
                //{
                //    chkCategory4.Text = MainPage.StrCategory4;
                //    chkCategory4.Enabled = true;
                //}
                //if (MainPage.StrCategory5 != "")
                //{
                //    chkCategory5.Text = MainPage.StrCategory5;
                //    chkCategory5.Enabled = true;
                //}

                DataTable dt = dba.GetDataTable("Select * from PrintingConfig");
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtTitleofDoc.Text = Convert.ToString(row["TitleOfDocument"]);
                    txtSubTitle.Text = Convert.ToString(row["SubTitle"]);
                    txtJurisdiction.Text = Convert.ToString(row["Jurisdiction"]);
                    txtGeneratedBy.Text = Convert.ToString(row["GeneratedBy"]);
                    txtSaleDeclaration.Text = Convert.ToString(row["Declaration"]);
                    txtSaleRtnDeclaration.Text = Convert.ToString(row["SaleRtnDeclaration"]);
                    txtSaleServDeclaration.Text = Convert.ToString(row["SaleServDeclaration"]);
                    txtPurchaseRtnDeclaration.Text = Convert.ToString(row["PurchRtnDeclaration"]);
                    txtTermOFDel.Text = Convert.ToString(row["TermsofDelivery"]);
                    txtNoofCopy.Text = Convert.ToString(row["NoOfCopy"]);

                    txtNCopyPurchase.Text = Convert.ToString(row["NCopyPurchase"]);
                    txtNCopySaleRtn.Text = Convert.ToString(row["NCopySaleRtn"]);
                    txtNCopyPurRtn.Text = Convert.ToString(row["NCopyPurRtn"]);
                    txtNCopyCash.Text = Convert.ToString(row["NCopyCash"]);
                    txtNCopyBank.Text = Convert.ToString(row["NCopyBank"]);
                    txtNCopyJournal.Text = Convert.ToString(row["NCopyJournal"]);
                    txtNCopySServ.Text = Convert.ToString(row["NCopySServ"]);
                    txtNCopyStockTrans.Text = Convert.ToString(row["NCopyStockTrans"]);

                    chkCompanyName.Checked = Convert.ToBoolean(row["CompanyName"]);
                    chkCompanyAddress.Checked = Convert.ToBoolean(row["CompanyAddress"]);
                    chkBuyerName.Checked = Convert.ToBoolean(row["BuyerName"]);
                    chkBuyerAddress.Checked = Convert.ToBoolean(row["BuyerAddress"]);
                    chkComTaxRegNo.Checked = Convert.ToBoolean(row["CompTaxRegNo"]);
                    chkBuyerTaxRegNo.Checked = Convert.ToBoolean(row["BuyerTaxRegNo"]);
                    chkOrderDetails.Checked = Convert.ToBoolean(row["OrderDetails"]);
                    chkSuppDesign.Checked = Convert.ToBoolean(row["SupplierDesign"]);
                    chkBarcode.Checked = Convert.ToBoolean(row["ManfDesign"]);
                    chkQty.Checked = Convert.ToBoolean(row["Qty"]);
                    chkRate.Checked = Convert.ToBoolean(row["Rate"]);
                    chkAmount.Checked = Convert.ToBoolean(row["Amount"]);
                    chkAgentName.Checked = Convert.ToBoolean(row["AgentName"]);
                    chkTaxPer.Checked = Convert.ToBoolean(row["Category1"]);
                    chkCategory2.Checked = Convert.ToBoolean(row["Category2"]);
                    chkCategory3.Checked = Convert.ToBoolean(row["Category3"]);
                    chkCategory4.Checked = Convert.ToBoolean(row["Category4"]);
                    chkCategory5.Checked = Convert.ToBoolean(row["Category5"]);             
                }
                
            }
            catch(Exception Ex)
            {
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to save changes ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecords();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveRecords()
        {
            string strQuery = " if not exists(Select * from PrintingConfig) begin  INSERT [dbo].[PrintingConfig] ([TitleOfDocument], [SubTitle], [Jurisdiction], [GeneratedBy], [Declaration],[SaleRtnDeclaration],[SaleServDeclaration],[PurchRtnDeclaration], [CompanyName], [CompanyAddress], [BuyerName], [BuyerAddress], [CompTaxRegNo], [BuyerTaxRegNo], [OrderDetails], [SupplierDesign], [ManfDesign], [Qty], [Rate], [Amount], [AgentName], [Category1], [Category2], [Category3], [Category4], [Category5], [Other], [OtherBit],[TermsofDelivery],[NoOfCopy],[NCopyPurchase],[NCopySaleRtn],[NCopyPurRtn],[NCopyCash],[NCopyBank],[NCopyJournal],[NCopySServ],[NCopyStockTrans])  "
                                + " VALUES ('" + txtTitleofDoc.Text + "','" + txtSubTitle.Text + "','" + txtJurisdiction.Text + "','" + txtGeneratedBy.Text + "','" + txtSaleDeclaration.Text + "','" + txtSaleRtnDeclaration .Text + "','" + txtSaleServDeclaration.Text + "','" + txtPurchaseRtnDeclaration.Text + "','" + chkCompanyName.Checked + "','" + chkCompanyAddress.Checked + "','" + chkBuyerName.Checked + "','" + chkBuyerAddress.Checked + "','" + chkComTaxRegNo.Checked + "','" + chkBuyerTaxRegNo.Checked + "','" + chkOrderDetails.Checked + "',"
                                + " '" + chkSuppDesign.Checked + "','" + chkBarcode.Checked + "','" + chkQty.Checked + "','" + chkRate.Checked + "','" + chkAmount.Checked + "','" + chkAgentName.Checked + "','" + chkTaxPer.Checked + "','" + chkCategory2.Checked + "','" + chkCategory3.Checked + "','" + chkCategory4.Checked + "','" + chkCategory5.Checked + "','',0,'"+txtTermOFDel.Text+"','"+txtNoofCopy.Text+ "','" + txtNCopyPurchase.Text + "','" + txtNCopySaleRtn.Text + "','" + txtNCopyPurRtn.Text + "','" + txtNCopyCash.Text + "','" + txtNCopyBank.Text + "','" + txtNCopyJournal.Text + "','" + txtNCopySServ.Text + "','" + txtNCopyStockTrans.Text + "') end else begin "
                                + " UPDATE [PrintingConfig] SET [TitleOfDocument]='" + txtTitleofDoc.Text + "', [SubTitle]='" + txtSubTitle.Text + "', [Jurisdiction]='" + txtJurisdiction.Text + "', [GeneratedBy]='" + txtGeneratedBy.Text + "', [Declaration]='" + txtSaleDeclaration.Text + "', [SaleRtnDeclaration]='" + txtSaleRtnDeclaration.Text + "', [SaleServDeclaration]='" + txtSaleServDeclaration.Text + "', [PurchRtnDeclaration]='" + txtPurchaseRtnDeclaration.Text + "', [CompanyName]='" + chkCompanyName.Checked + "', [CompanyAddress]='" + chkCompanyAddress.Checked + "', [BuyerName]='" + chkBuyerName.Checked + "',[BuyerAddress]='" + chkBuyerAddress.Checked + "', [CompTaxRegNo]='" + chkComTaxRegNo.Checked + "', [BuyerTaxRegNo]='" + chkBuyerTaxRegNo.Checked + "',"
                                + " [OrderDetails]='" + chkOrderDetails.Checked + "', [SupplierDesign]='" + chkSuppDesign.Checked + "', [ManfDesign]='" + chkBarcode.Checked + "', [Qty]='" + chkQty.Checked + "', [Rate]='" + chkRate.Checked + "', [Amount]='" + chkAmount.Checked + "', [AgentName]='" + chkAgentName.Checked + "', [Category1]='" + chkTaxPer.Checked + "', [Category2]='" + chkCategory2.Checked + "', [Category3]='" + chkCategory3.Checked + "', [Category4]='" + chkCategory4.Checked + "', [Category5]='" + chkCategory5.Checked + "',[TermsofDelivery]='" + txtTermOFDel.Text + "',[NoOfCopy]='" + txtNoofCopy.Text 
                                + "',[NCopyPurchase]='" + txtNCopyPurchase.Text + "',[NCopySaleRtn]='" + txtNCopySaleRtn.Text + "',[NCopyPurRtn]='" + txtNCopyPurRtn.Text + "',[NCopyCash]='" + txtNCopyCash.Text + "',[NCopyBank]='" + txtNCopyBank.Text + "',[NCopyJournal]='" + txtNCopyJournal.Text + "',[NCopySServ]='" + txtNCopySServ.Text + "',[NCopyStockTrans]='" + txtNCopyStockTrans.Text + "' end ";

        int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                SetUpdatedData();
                MessageBox.Show("Thank You ! Record  successfully saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                MessageBox.Show("Sorry ! Please try again later !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SetUpdatedData()
        {
            MainPage.strTitleofDocument = txtTitleofDoc.Text;
            MainPage.strSubTitle = txtSubTitle.Text;
            MainPage.strJurisdiction = txtJurisdiction.Text;
            MainPage.strGeneratedBy = txtGeneratedBy.Text;
            MainPage.strDeclaration = txtSaleDeclaration.Text;
            MainPage.strSaleRtnDeclaration = txtSaleRtnDeclaration.Text;
            MainPage.strSaleServDeclaration = txtSaleServDeclaration.Text;
            MainPage.strPurchaseRtnDeclaration = txtPurchaseRtnDeclaration.Text;
            MainPage.strTermsofDelivery = txtTermOFDel.Text;
            MainPage.strNoofCopy = txtNoofCopy.Text;

            MainPage.iNCopyPurchase = dba.ConvertObjectToInt(txtNCopyPurchase.Text);
            MainPage.iNCopySaleRtn = dba.ConvertObjectToInt(txtNCopySaleRtn.Text);
            MainPage.iNCopyPurRtn = dba.ConvertObjectToInt(txtNCopyPurRtn.Text);
            MainPage.iNCopyCash = dba.ConvertObjectToInt(txtNCopyCash.Text);
            MainPage.iNCopyBank = dba.ConvertObjectToInt(txtNCopyBank.Text);
            MainPage.iNCopyJournal = dba.ConvertObjectToInt(txtNCopyJournal.Text);
            MainPage.iNCopySServ = dba.ConvertObjectToInt(txtNCopySServ.Text);
            MainPage.iNCopyStockTrans = dba.ConvertObjectToInt(txtNCopyStockTrans.Text);

            MainPage.pCompanyName = chkCompanyName.Checked;
            MainPage.pCompanyAddress = chkCompanyAddress.Checked;
            MainPage.pBuyerName = chkBuyerName.Checked;
            MainPage.pBuyerAddress = chkBuyerAddress.Checked;
            MainPage.pCompTaxRegNo = chkComTaxRegNo.Checked;
            MainPage.pBuyerTaxRegNo = chkBuyerTaxRegNo.Checked;
            MainPage.pOrderDetails = chkOrderDetails.Checked;
            MainPage.pSuppDesign = chkSuppDesign.Checked;
            MainPage.pManfDesign = chkBarcode.Checked;
            MainPage.pQty = chkQty.Checked;
            MainPage.pRate = chkRate.Checked;
            MainPage.pAmount = chkAmount.Checked;
            MainPage.pAgentName = chkAgentName.Checked;
            MainPage.pTaxPer = chkTaxPer.Checked;
            MainPage.pCategory2 = chkCategory2.Checked;
            MainPage.pCategory3 = chkCategory3.Checked;
            MainPage.pCategory4 = chkCategory4.Checked;
            MainPage.pCategory5 = chkCategory5.Checked;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNoofCopy_Leave(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (dba.ConvertObjectToDouble(txt.Text) <= 0)
                txt.Text = "1";
        }

        private void txtNoofCopy_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
    }
}
