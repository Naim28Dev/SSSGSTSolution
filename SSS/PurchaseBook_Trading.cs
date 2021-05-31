using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace SSS
{
    public partial class PurchaseBook_Trading : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "",strOldPartyName="", _strPDFFilePath="", _strBillType="",_strSType="";
        double dOldNetAmt = 0;
        bool qtyAdjustStatus = false;
        public PurchaseBook_Trading()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            GetStartupData(true);
        }

        public PurchaseBook_Trading(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetCategory();
            txtBillCode.Text = strSerialCode;
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select PBillCode,(Select ISNULL(MAX(BillNo),0) from PurchaseBook Where BillCode=PBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtBillCode.Text = Convert.ToString(dt.Rows[0]["PBillCode"]);                 
                    strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                }              

                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void SetCategory()
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


        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'   ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'  ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "'  and BillNo>" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                {
                    BindRecordWithControl(strSerialNo);
                }
                else
                {
                    BindLastRecord();
                }
            }
            catch
            {
            }
        }

        private void BindPreviousRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
                else
                    BindFirstRecord();
            }
            catch
            {
            }
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                string strQuery = "Select *,(Convert(varchar,Date,103)) SDate,(Convert(varchar,InvoiceDate,103)) IDate,(Convert(varchar,LRDate,103)) LDate,(PurchasePartyID+' '+SM.Name) PartyName,GSTNo,State,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,PB.Date))) LockType from PurchaseBook PB OUTER APPLY(Select Name,GSTNo,State from SupplierMaster Where AreaCode+AccountNo=PurchasePartyID)SM Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  Select * from PurchaseBookSecondary Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " order by ID "
                                + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                chkPick.Checked = false;
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                DisableAllControls();
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = "";
                txtReason.Text = "";
                pnlDeletionConfirmation.Visible = false;
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtBillNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["SDate"]);
                            txtInvoiceNo.Text = Convert.ToString(row["InvoiceNo"]);
                            txtInvoiceDate.Text = Convert.ToString(row["IDate"]);
                            txtPurchaseParty.Text = strOldPartyName= Convert.ToString(row["PartyName"]);
                            txtTaxLedger.Text = Convert.ToString(row["PurchaseType"]);
                            txtImportData.Text = Convert.ToString(row["Description"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtGodown.Text = Convert.ToString(row["GodownName"]);
                            txtTransport.Text = Convert.ToString(row["TransportName"]);
                            txtLRNo.Text = Convert.ToString(row["LRNumber"]);
                            txtLRDate.Text = Convert.ToString(row["LDate"]);

                            txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                            txtStateName.Text = Convert.ToString(row["State"]);
                            txtStockStatus.Text = Convert.ToString(row["StockStatus"]);
                            txtmanufacturer.Text = Convert.ToString(row["Other1"]);

                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                            txtDiscPer.Text = Convert.ToString(row["DiscPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DiscAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);                           
                            txtROSign.Text = Convert.ToString(row["ROSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOff"]);
                            txtOtherPerSign.Text = Convert.ToString(row["Other"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);

                            lblTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            lblGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                            chkTCSAmt.Checked = false;
                            txtTCSPer.Text = txtTCSAmt.Text = "0.00";
                            if (dt.Columns.Contains("TCSPer"))
                            {
                                double dTCSPer = dba.ConvertObjectToDouble(row["TCSPer"]), dTCSAmt = dba.ConvertObjectToDouble(row["TCSAmt"]);
                                if (dTCSAmt > 0)
                                {
                                    txtTCSPer.Text = dTCSPer.ToString("0.000");
                                    txtTCSAmt.Text = dTCSAmt.ToString("N2", MainPage.indianCurancy);
                                    chkTCSAmt.Checked = true;
                                }
                                else
                                {
                                    DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                                    if (_date >= Convert.ToDateTime("10/01/2020"))
                                        txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");
                                }
                            }

                            if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                                btnEdit.Enabled = btnDelete.Enabled = false;
                            else
                            {
                                if (!MainPage.mymainObject.bPurchaseEdit)
                                    btnEdit.Enabled = btnDelete.Enabled = false;
                                else
                                    btnEdit.Enabled = btnDelete.Enabled = true;
                            }

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                            if (txtLRNo.Text == "")
                                txtLRDate.Text = MainPage.strCurrentDate;

                            if(ds.Tables.Count>1)
                            {
                                if(ds.Tables[1].Rows.Count>0)
                                {
                                    txtOrderNo.Text = Convert.ToString(ds.Tables[1].Rows[0]["PONumber"]);
                                }
                            }

                        }
                    }

                    BindPurchaseBookDetails(ds.Tables[1]);
                    BindGSTDetailsWithControl(ds.Tables[2]);


                }
            }
            catch
            {
            }
        }

        private void BindPurchaseBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();      
            int rowIndex = 0;
            qtyAdjustStatus = false;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]); 
                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]);
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Discount"];
                    dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                    dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                    rowIndex++;
                }
            }
        }

        private void BindGSTDetailsWithControl(DataTable dt)
        {
            int rowIndex = 0;
            dgrdTax.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdTax.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdTax.Rows[rowIndex].Cells["taxName"].Value = row["AccountName"];
                    dgrdTax.Rows[rowIndex].Cells["taxRate"].Value = row["TaxRate"];
                    dgrdTax.Rows[rowIndex].Cells["taxAmt"].Value = row["TaxAmount"];
                    dgrdTax.Rows[rowIndex].Cells["taxType"].Value = row["taxType"];

                    rowIndex++;
                }
                pnlTax.Visible = true;
            }
            else
                pnlTax.Visible = false;
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {

                        string strPName = txtPurchaseParty.Text;
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtPurchaseParty.Text = objSearch.strSelectedData;
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                string strGSTNo = "",strStateName="";
                                bool _blackListed = false;
                                if (dba.CheckTransactionLockWithBlackListGSTNo(strData, ref _blackListed, ref strGSTNo, ref strStateName))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please select different account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = txtStateName.Text = txtGSTNo.Text = "";                                    
                                }
                                else if (_blackListed)
                                {
                                    MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = txtStateName.Text = txtGSTNo.Text =  "";                                    
                                }
                                else
                                {
                                    txtPurchaseParty.Text = strData;
                                    txtGSTNo.Text = strGSTNo;
                                    txtStateName.Text = strStateName;
                                }
                            }
                        }

                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        //private void GetPartyDhara()
        //{
        //    try
        //    {
        //        string strPurchasePartyID, strQuery = "";
        //        if (txtPartyName.Text != "PERSONAL")
        //        {
        //            string[] strFullName = txtPartyName.Text.Split(' ');
        //            if (strFullName.Length > 1)
        //            {
        //                strPurchasePartyID = strFullName[0].Trim();
        //                strQuery = "Select SM.NormalDhara,SM.SNDhara as SUPERDhara from SupplierMaster SM  Where  (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strPurchasePartyID + "' ";
        //                DataTable dt = dba.GetDataTable(strQuery);
        //                if (dt.Rows.Count > 0)
        //                {
        //                    DataRow row = dt.Rows[0];
        //                    txtDiscPer.Text = Convert.ToString(row["NormalDhara"]);
        //                    //txtSuperNetDhara.Text = Convert.ToString(row["SUPERDhara"]);
        //                    //rdoNormalDhara.Checked = true;
        //                }
        //                else
        //                    txtDiscPer.Text = "0";
        //            }
        //            else
        //                txtDiscPer.Text = "0";
        //            CalculateAllAmount();
        //        }
        //    }
        //    catch
        //    { }
        //}

        //private void GetAgentName()
        //{
        //    DataTable dt = dba.GetDataTable("Select ContactPerson,(Select ISNULL(SM1.CommRate,0) from SupplierMaster SM1 Where SM1.Name=SM.ContactPerson) CommRate from SupplierMaster SM Where SM.Name='" + txtPartyName.Text + "' ");
        //    if (dt.Rows.Count > 0)
        //    {
        //        txtAgentName.Text = Convert.ToString(dt.Rows[0]["ContactPerson"]);
        //        txtAgentCommPer.Text = Convert.ToString(dt.Rows[0]["CommRate"]);
        //    }
        //    if (txtAgentCommPer.Text == "")
        //        txtAgentCommPer.Text = "0.00";
        //    //CalculateAgentCommission();
        //} 

        private void SaleBook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bPurchaseView)
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        BindNextRecord();
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        BindPreviousRecord();
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        BindFirstRecord();
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        BindLastRecord();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.E)
                    {
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
            }
        }
       
        
        private void txtAgentName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("AGENTNAME", "SEARCH AGENT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtAgentName.Text = objSearch.strSelectedData;
                       // CalculateAgentComm();
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        //private void CalculateAgentComm()
        //{
        //    if (txtAgentName.Text != "")
        //    {
        //        string strQuery = " Select ISNULL(CommRate,0) from SupplierMaster Where Name='" + txtAgentName.Text + "' and DealerType='AGENT'";
        //        object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
        //        txtAgentCommPer.Text = dba.ConvertObjectToDouble(objValue).ToString("0.00");
        //        CalculateAgentCommission();
        //    }
        //    else
        //    {
        //        txtAgentCommPer.Text = txtAgentCommAmt.Text = "0.00";
        //    }     
        //}
         

        private void EnableAllControls()
        {
            txtSpclDisPer.ReadOnly = txtOtherPerSign.ReadOnly = txtDate.ReadOnly = txtInvoiceDate.ReadOnly = txtInvoiceNo.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtDiscPer.ReadOnly = txtTaxPer.ReadOnly=  txtROSign.ReadOnly = txtRoundOff.ReadOnly = false;
            dgrdDetails.ReadOnly =  false;
            chkTCSAmt.Enabled = true; 
        }

        private void DisableAllControls()
        {
            txtSpclDisPer.ReadOnly = txtOtherPerSign.ReadOnly= txtDate.ReadOnly = txtInvoiceDate.ReadOnly = txtInvoiceNo.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtDiscPer.ReadOnly = txtTaxPer.ReadOnly = txtROSign.ReadOnly=txtRoundOff.ReadOnly= true;
            dgrdDetails.ReadOnly =true;  
            chkTCSAmt.Enabled = false;
            lblMsg.Text = "";
        }

        private void ClearAllText()
        {
            txtGSTNo.Text=txtStateName.Text= txtImportData.Text= txtOrderNo.Text= strOldPartyName= txtPurchaseParty.Text = txtTaxLedger.Text = txtAgentName.Text = txtTaxLedger.Text = txtRemark.Text =  txtInvoiceNo.Text = txtTransport.Text = txtGodown.Text = txtStockStatus.Text = txtLRNo.Text = txtmanufacturer.Text = "";
            txtTCSAmt.Text = txtSpclDisPer.Text = txtSplDisAmt.Text = lblTaxableAmt.Text = txtRoundOff.Text = txtOtherAmount.Text = txtPackingAmt.Text = txtDiscPer.Text = txtDiscAmt.Text = txtTaxAmt.Text =  lblTotalQty.Text = lblGrossAmt.Text = lblNetAmt.Text = "0.00";
            txtSign.Text = txtROSign.Text =  "+";
            txtOtherPerSign.Text = "-";
            if (MainPage._bTaxStatus)
                txtTaxPer.Text = "18.00";
            else
                txtTaxPer.Text = "0.00";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = chkPick.Checked =chkTCSAmt.Checked= false;
            txtStockStatus.Text = "STOCK IN";
            txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");

            dOldNetAmt = 0;
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtInvoiceDate.Text = txtLRDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtInvoiceDate.Text = txtLRDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void ClearAllTextForPDF()
        {
            _strSType = "";
            txtImportData.Text = txtOrderNo.Text = strOldPartyName = txtPurchaseParty.Text = txtAgentName.Text =txtRemark.Text = txtInvoiceNo.Text = txtTransport.Text = txtGodown.Text = txtStockStatus.Text = txtLRNo.Text = txtmanufacturer.Text = "";
            txtSpclDisPer.Text = txtSplDisAmt.Text = lblTaxableAmt.Text = txtRoundOff.Text = txtOtherAmount.Text = txtPackingAmt.Text = txtDiscPer.Text = txtDiscAmt.Text = txtTaxAmt.Text = txtTaxPer.Text = lblTotalQty.Text = lblGrossAmt.Text = lblNetAmt.Text = "0.00";
            txtSign.Text = txtROSign.Text = "+";
            txtOtherPerSign.Text = "-";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = chkPick.Checked = false;
            dOldNetAmt = 0;
            _strBillType = "";
            //if (DateTime.Today > MainPage.startFinDate)
            //    txtDate.Text = txtInvoiceDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            //else
            //    txtDate.Text = txtInvoiceDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }


        //private void SetSerialNo()
        //{
        //    object objValue = DataBaseAccess.ExecuteMyScalar("Select (ISNULL(MAX(BillNo),0)+1)SerialNo from PurchaseBook Where BillCode='"+txtBillCode.Text+"' ");
        //    txtBillNo.Text = Convert.ToString(objValue);
        //}

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(PurchaseBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from PurchaseBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='PURCHASE' and TaxIncluded=0) TaxName  from PurchaseRecord Where BillCode='" + txtBillCode.Text + "')Purchase ");
                    if (table.Rows.Count > 0)
                    {
                        //double billNo = dba.ConvertObjectToDouble(table.Rows[0][0]), maxBillNo = dba.ConvertObjectToDouble(table.Rows[0][1]),dSerialNo=Convert(;
                        //if (billNo > maxBillNo)
                        //    txtBillNo.Text = Convert.ToString(billNo);
                        //else
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                        if(MainPage._bTaxStatus && Convert.ToString(table.Rows[0]["TaxName"])!="")
                        txtTaxLedger.Text = Convert.ToString(table.Rows[0]["TaxName"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in set sale bill No in Purchase book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill No can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }          
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Date is not valid ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! Party Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }
            if (chkTCSAmt.Checked)
            {
                double dTCSAmt = ConvertObjectToDouble(txtTCSAmt.Text);
                if (dTCSAmt > 0)
                {
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select (AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES'");
                    if (Convert.ToString(objValue) == "")
                    {
                        MessageBox.Show("Sorry ! Please create TCS Account under category : 'TCS RECEIVABLES' with Group Name : SHORT-TERM LOANS AND ADVANCES'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(rows.Cells["itemName"].Value);
                double dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);
                if (strItem == "" && dAmount ==0)
                    dgrdDetails.Rows.Remove(rows);
                else
                {                  
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }
                    else if (dAmount == 0)
                    {
                        MessageBox.Show("Sorry ! Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = rows.Cells["qty"];
                        dgrdDetails.Focus();
                        return false;
                    }                   
                }
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add(1);
                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                MessageBox.Show("Sorry ! Please add atleast one entry in table ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return ValidateOtherValidation(false);
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            string strOrderQuery = "";           
                strOrderQuery = "0";

            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(" + strOrderQuery + ") OQty, (Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select TOP 1 UPPER(Tick) from BalanceAmount Where AccountStatus='PURCHASE A/C' and Description Like(CS.PBillCode+' " + txtBillNo.Text + "')),'FALSE') TickStatus,CS.PBillCode from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtPurchaseParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }               

                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || strOldPartyName != txtPurchaseParty.Text || _bUpdateStatus)
                    {
                        if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                        {
                            bool iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);
                            string strPBillCode = Convert.ToString(dt.Rows[0]["PBillCode"]);

                            if (!iStatus && MainPage.strOnlineDataBaseName != "")
                            {
                                bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(strPBillCode + " " + txtBillNo.Text);
                                if (!netStatus)
                                {
                                    // MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                            else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
                            {
                                MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }                   
                }

                if (!_bUpdateStatus)
                {
                    string strRegion = Convert.ToString(dt.Rows[0]["Region"]), strCStateName = Convert.ToString(dt.Rows[0]["CStateName"]).ToUpper(), strSStateName = Convert.ToString(dt.Rows[0]["SStateName"]).ToUpper();
                    if (strRegion != "")
                    {
                        if (strRegion == "LOCAL" && strSStateName != strCStateName)
                        {
                            MessageBox.Show("You are entering a central transaction for a party belonging to same state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //if (result == DialogResult.Yes)
                            //    return true;
                            //else
                            return false;
                        }
                        else if (strRegion == "INTERSTATE" && strSStateName == strCStateName)
                        {
                            MessageBox.Show("You are entering a local  transaction for a party belonging to other  state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //if (result == DialogResult.Yes)
                            //    return true;
                            //else
                            return false;
                        }
                    }
                }
                //if (Convert.ToString(dt.Rows[0]["IncludeStatus"]) == "DENY")
                //{
                //    MessageBox.Show("Sorry Sale type and purchase type doesn't match in tax inclusion!\nPlease enter correct purchase type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool CheckBillNoAndSuggest()
        {
            bool chkStatus = true;
            try
            {
                if (txtBillNo.Text != "")
                {
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    {
                        int check = dba.CheckGoodsReceiptAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from (Select ISNULL(Max(BillNo)+1,1)BillNo from PurchaseBook where BillCode='" + txtBillCode.Text + "' UNION ALL Select ISNULL(Max(BillNo)+1,1)BillNo from PurchaseRecord where BillCode='" + txtBillCode.Text + "')_Sales"));
                            MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Text = strBillNo;
                            chkStatus = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Bill No is already in used please Choose Different bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bill No can't be blank ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }


        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex <2 || e.ColumnIndex == 9 || e.ColumnIndex == 16)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7)
                    {
                        string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";

                        SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, true,"");
                        objSearch.ShowDialog();
                        GetAllDesignSizeColor(objSearch, dgrdDetails.CurrentRow.Index);
                        e.Cancel = true;
                    }
                    else
                    {
                        if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT" || !MainPage.mymainObject.bFullEditControl)
                        {
                            if ((e.ColumnIndex == 10 || e.ColumnIndex == 12) && !MainPage.strUserRole.Contains("ADMIN") && !MainPage.strSoftwareType.Contains("TRADING"))
                                e.Cancel = true;
                            else if (e.ColumnIndex == 11 && !MainPage.strUserRole.Contains("SUPERADMIN"))
                                e.Cancel = true;
                        }
                    }
                }
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }


        private void GetAllDesignSizeColor(SearchCategory objCategory, int rowIndex)
        {
            try
            {
                bool firstRow = false;
                if (objCategory != null)
                {
                    if (objCategory.lbSearchBox.Items.Count > 0)
                    {
                        string strData = objCategory.strSelectedData;
                        if (strData == "")
                        {
                            foreach (string strItem in objCategory.lbSearchBox.Items)
                            {
                                if (strItem != "ADD NEW DESIGNNAME NAME")
                                {
                                    string[] strAllItem = strItem.Split('|');
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                        if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                            GetPurchaseRate(dgrdDetails.Rows[rowIndex]);

                                        SetUnitName(strAllItem[0], rowIndex);

                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowIndex > 0)
                                rowIndex--;
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];
                                if (MainPage.StrCategory1 != "" && strAllItem.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                if (MainPage.StrCategory2 != "" && strAllItem.Length > 2)
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                if (MainPage.StrCategory3 != "" && strAllItem.Length > 3)
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                if (MainPage.StrCategory4 != "" && strAllItem.Length > 4)
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                if (MainPage.StrCategory5 != "" && strAllItem.Length > 5)
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "")
                                    GetPurchaseRate(dgrdDetails.Rows[rowIndex]);
                                SetUnitName(strAllItem[0], rowIndex);
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (btnAdd.Text == "&Save")
                        {
                            if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "")
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                                dgrdDetails.Focus();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void SetUnitName(string strDesignName, int rowIndex)
        {
            if (strDesignName != "")
            {
                DataTable table = dba.GetDataTable("Select ISNULL(QtyRatio,1) QtyRatio,UnitName as PurchaseUnit,StockUnitName UnitName from Items Where ItemName='" + strDesignName + "' ");
                if (table.Rows.Count > 0)
                {
                    // dgrdDetails.Rows[rowIndex].Cells["qtyRatio"].Value = table.Rows[0]["QtyRatio"];
                    //dgrdDetails.Rows[rowIndex].Cells["purchaseUnit"].Value = table.Rows[0]["PurchaseUnit"];
                    dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = table.Rows[0]["UnitName"];
                }
            }
        }

        private void GetPurchaseRate(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, _dMRP = 0, dRate=0, dSpclDis = dba.ConvertObjectToDouble(txtSpclDisPer.Text);
                if (row != null)
                {
                    object objDisPer = 0, objSaleRate=0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                        if (dDisPer != 0)
                            dDisPer = dDisPer * -1;
                    }
                }
                if (dSpclDis != 0 && dMRP != 0)
                    _dMRP = dMRP * (100.00 - dSpclDis) / 100.00;
                else
                    _dMRP = dMRP;

                dDisPer = Math.Abs(dDisPer);
                if (dDisPer != 0 && _dMRP != 0)
                    dRate = _dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = _dMRP;

                row.Cells["mrp"].Value = Math.Round(dMRP, 4);
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = Math.Round(dRate, 4);
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value), dDisc = ConvertObjectToDouble(row.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                dAmt = dQty * dRate;

                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);                
            }
            catch
            {
            }
        }

        private void GetPurchaseRate_Import(DataGridViewRow row)
        {
            try
            {
                double dDisPer = 0, dMRP = 0, _dMRP = 0, dRate = 0, dSpclDis = dba.ConvertObjectToDouble(txtSpclDisPer.Text),dOldRate=0,dOldMRP=0;
                if (row != null)
                {
                    object objDisPer = 0, objSaleRate=0;
                    dOldRate = dba.ConvertObjectToDouble(row.Cells["rate"].Value);
                    dOldMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        object objValue = dba.GetPurchaseRate(ref objDisPer, row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref objSaleRate);
                        dDisPer = ConvertObjectToDouble(objDisPer);
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                        if (dDisPer != 0)
                            dDisPer = dDisPer * -1;
                    }
                }
                if (dSpclDis != 0 && dMRP != 0)
                    _dMRP = dMRP * (100.00 - dSpclDis) / 100.00;
                else
                    _dMRP = dMRP;



                dDisPer = Math.Abs(dDisPer);
                if (dDisPer != 0 && _dMRP != 0)
                    dRate = _dMRP * (100 - dDisPer) / 100;
                if (dRate == 0)
                    dRate = _dMRP;

                if (dOldMRP != dMRP && dOldMRP > 0)
                    dMRP = dOldMRP;

                    if (dRate!=dOldRate)
                {
                    dDisPer = (100.00-((dOldRate * 100.00) / dMRP));
                    dRate = dOldRate;
                }
                row.Cells["mrp"].Value = Math.Round(dMRP, 4);
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = Math.Round(dRate, 4);
                double dAmt = 0, dQty = ConvertObjectToDouble(row.Cells["qty"].Value), dDisc = ConvertObjectToDouble(row.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                dAmt = dQty * dRate;

                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }


        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, dMRP = 0, _dMRP = 0, dAmt = 0, dDisPer = 0, dRate = 0, dQty = 0, dDisc = 0, dOCharges = 0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    _dMRP = dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (dSpclPer != 0 && dMRP != 0)
                    {
                        dSpclAmt += (dMRP * dSpclPer) / 100.00;
                        _dMRP = dMRP * (100.00 - dSpclPer) / 100.00;
                    }

                    if ((dDisPer != 0) && _dMRP != 0)
                    {
                        dDisPer = Math.Abs(dDisPer);

                        dRate = _dMRP * (100.00 - (dDisPer)) / 100.00;
                        //dRate =Math.Round(dRate,4);
                    }
                    if (dRate == 0)
                        dRate = _dMRP;

                    if (txtPurchaseParty.Text.Contains("SIYARAM"))
                        dRate = Convert.ToDouble(dRate.ToString("0"));
                    else
                        dRate = Math.Round(dRate, 4);

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate;
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    dDisc = ConvertObjectToDouble(row.Cells["disc"].Value);
                    dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                    row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
                }

                txtSplDisAmt.Text = dSpclAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 8 || e.ColumnIndex == 10 || e.ColumnIndex == 11)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 12)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 13)
                        CalculateRateWithQtyAmount(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 14 || e.ColumnIndex == 15)
                        CalculateAmountWithDiscOtherChargese(dgrdDetails.Rows[e.RowIndex]);
                   
                }
            }
            catch
            {
            }
        }
        
        private void CalculateRateWithQtyAmount(DataGridViewRow rows)
        {
            double dRate = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            if (dAmount != 0 && dQty != 0)
                dRate = dAmount / dQty ;

            rows.Cells["rate"].Value = Math.Round(dRate,4);
            rows.Cells["netAmt"].Value = (dAmount - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    int CurrentRow = 0;
                    int IndexColmn = 0;
                    int Index;
                    if (e.KeyCode == Keys.Enter)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                        if (Index < dgrdDetails.RowCount - 1)
                        {
                            CurrentRow = Index - 1;
                        }
                        else
                        {
                            CurrentRow = Index;
                        }
                        if (IndexColmn < dgrdDetails.ColumnCount - 2)
                        {
                            IndexColmn += 1;
                            if (!dgrdDetails.Columns[IndexColmn].Visible)
                                IndexColmn++;
                            if (CurrentRow >= 0)
                            {
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 1)
                                    IndexColmn++;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                            double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "" && dAmt>0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells[2];
                                dgrdDetails.Focus();
                            }
                            else
                            {
                                txtPackingAmt.Focus();
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.F1 && btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                            dgrdDetails.Enabled = true;
                        }
                        else
                        {
                            ArrangeSerialNo();
                        }
                        CalculateAllAmount();
                    }
                    else if (e.KeyCode == Keys.F1 && btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);
                        if (strID == "")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                                dgrdDetails.Enabled = true;
                            }
                            else
                            {
                                ArrangeSerialNo();
                            }
                            CalculateAllAmount();
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to delete permanently  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                DeleteOneRow(strID);
                            }
                        }
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        //int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        //if (colIndex != 1 && colIndex != 10 && colIndex != 13 && colIndex != 15 && colIndex != 18)
                        //    dgrdDetails.CurrentCell.Value = "";
                        //if (colIndex == 9 || colIndex == 14)
                        //{
                        //    CalculateAmountWithQtyRate(dgrdDetails.CurrentRow);
                        //    CalculateAllAmount();
                        //}
                    }
                }
            }
            catch { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (columnIndex == 8 || columnIndex == 10 || columnIndex ==11 || columnIndex == 12 || columnIndex == 13)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (columnIndex == 10 || columnIndex == 12)
                dba.KeyHandlerPoint(sender, e, 4);
            else if (columnIndex == 8 || columnIndex == 13)
                dba.KeyHandlerPoint(sender, e, 2);
            else if (columnIndex == 11)
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    dba.KeyHandlerPoint(sender, e, 2);
            }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells["srNo"].Value = serialNo;
                serialNo++;
            }
        }

        private void DeleteOneRow(string strID)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    string strQuery = " Delete from PurchaseBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + " ";
                    dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                    CalculateAllAmount();
                    int result = UpdateRecord(strQuery);
                    if (result < 1)
                        BindRecordWithControl(txtBillNo.Text);
                    else
                    {
                        strQuery = " Delete from PurchaseBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID=" + strID + " ";
                        DataBaseAccess.CreateDeleteQuery(strQuery);

                        dgrdDetails.ReadOnly = false;
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                            dgrdDetails.Enabled = true;
                        }
                        else
                            ArrangeSerialNo();
                    }
                }

            }
            catch
            {
            }
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void CalculateDisWithAmountMRP(DataGridViewRow rows)
        {

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);


                if (dRate != 0 && dMRP != 0)
                    dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                rows.Cells["disPer"].Value = dDisPer;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
        }

        private void CalculateDisWithAmountMRP_Current(DataGridViewRow rows)
        {

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dRate = ConvertObjectToDouble(rows.Cells["rate"].Value);


                if (dRate != 0 && dMRP != 0)
                    dDisPer = ((dMRP - dRate) / dMRP) * 100.00;

                rows.Cells["disPer"].Value = dDisPer;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            }
        }

        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dRate=0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value), dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            dDisPer = Math.Abs(dDisPer);
            if (dDisPer != 0 && dMRP != 0)
            {
                dRate = dMRP * (100.00 - dDisPer) / 100.00;
                //dRate = Math.Round(dRate, 2);
            }
            if (dRate == 0)
                dRate = dMRP;

            dAmt = dQty * dRate;
            rows.Cells["rate"].Value = Math.Round(dRate, 4); 
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

            CalculateAllAmount();
        }

        private void CalculateAmountWithDiscOtherChargese(DataGridViewRow rows)
        {
            double dAmt = ConvertObjectToDouble(rows.Cells["amount"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void CalculateAllAmount()
        {
            try
            {
                CalculateSpecialDiscount();

                double dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dTaxableAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dAgentCommAmt = 0, dRoundOff = 0;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                }

                lblGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dPackingAmt = ConvertObjectToDouble(txtPackingAmt.Text);
                dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                if (txtSign.Text == "-")
                    dOtherAmt = dOtherAmt * -1;

                double dDisPer = ConvertObjectToDouble(txtOtherPerSign.Text + txtDiscPer.Text), dGrossAmt = 0, dTcsPer = 0, dTCSAmt = 0;

                dTOAmt = dOtherAmt + dPackingAmt;
                dGrossAmt = dBasicAmt + dTOAmt;
                dDiscAmt = (dGrossAmt * dDisPer) / 100;
                dTOAmt += dDiscAmt;

                dFinalAmt = dBasicAmt + dTOAmt;
                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt,ref dTaxableAmt);

                dNetAmt = dBasicAmt + dOtherAmt + dPackingAmt + dTaxAmt + dDiscAmt;
                dNetAmt = Math.Round(dNetAmt, 2);
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = (dNetAmt * dTcsPer) / 100.00;
                    dNetAmt += dTCSAmt;
                }


                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);
                dRoundOff = (dNNetAmt - dNetAmt);

                if (dRoundOff >= 0)
                {
                    txtROSign.Text = "+";
                    txtRoundOff.Text = dRoundOff.ToString("0.00");
                }
                else
                {
                    txtROSign.Text = "-";
                    txtRoundOff.Text = Math.Abs(dRoundOff).ToString("0.00");
                }

                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
                txtTCSAmt.Text= dTCSAmt.ToString("N2", MainPage.indianCurancy);
                lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private double ConvertObjectToDouble(object objValue)
        {
            double dValue = 0;
            if (objValue != null && Convert.ToString(objValue) != "")
            {
                try
                {
                    dValue = Convert.ToDouble(objValue);
                }
                catch
                {
                }
            }
            return dValue;
        }

        private void txtPackingAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPackingAmt.Text == "")
                    txtPackingAmt.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSign.Text == "")
                    txtSign.Text = "+";
                CalculateAllAmount();
            }
        }

        private void txtOtherAmount_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtOtherAmount.Text == "")
                    txtOtherAmount.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtDiscPer_Leave(object sender, EventArgs e)
        {           
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtDiscPer.Text == "")
                        txtDiscPer.Text = "0.00";                    
                    CalculateAllAmount();
                }           
        }

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "18.00";
                    double dTaxPer = dba.ConvertObjectToDouble(txt.Text);
                    if (dTaxPer != 3 && dTaxPer != 5 && dTaxPer != 12 && dTaxPer != 18 && dTaxPer != 28)
                        txt.Text = "18.00";
                    CalculateAllAmount();
                }
            }
        }

        private void txtPackingAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add")
            {
                if (btnEdit.Text == "&Update")
                {
                    DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                }
                btnAdd.Text = "&Save";
                btnEdit.Text = "&Edit";
                EnableAllControls();
                txtBillNo.ReadOnly = false;
                ClearAllText();
                SetSerialNo();
                txtDate.Focus();
            }
            else if (ValidateControls() && CheckBillNoAndSuggest())
            {
                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
        }

        private void SaveRecord()
        {
            try
            {
                string strInvoiceDate="NULL", strLRDate = "NULL";
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (txtInvoiceDate.Text.Length == 10)
                {
                    DateTime iDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text);
                    strInvoiceDate = "'" + iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtLRDate.Text.Length == 10 && txtLRNo.Text != "")
                {
                    DateTime lDate = dba.ConvertDateInExactFormat(txtLRDate.Text);
                    strLRDate = "'" + lDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                double dAmt = Convert.ToDouble(lblNetAmt.Text),dTaxAmt=dba.ConvertObjectToDouble(txtTaxAmt.Text),dSpclDisPer=dba.ConvertObjectToDouble(txtSpclDisPer.Text), dTcsPer = 0, dTCSAmt = 0;
                string strPurchaseParty = "", strPurchasePartyID = "", strMNID="";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }
                if (txtmanufacturer.Text != "")
                {
                    strFullName = txtmanufacturer.Text.Split(' ');
                    strMNID = strFullName[0];
                }

                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                }

                string strQuery = " if not exists(Select ReceiptCode from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " UNION ALL Select BillCode from PurchaseBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ") begin "
                                        + " INSERT INTO [dbo].[PurchaseBook] ([BillCode],[BillNo],[Date],[InvoiceNo],[InvoiceDate],[PurchasePartyID],[PurchaseParty],[PurchaseType],[Remark],[Description],[Other],[PackingAmt],[OtherSign],[OtherAmt],[DiscPer],[DiscAmt],[TaxPer],[TaxAmt],[TotalQty],[GrossAmt],[NetAmt],[ROSign],[RoundOff],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SpecialDscPer],[SpecialDscAmt],[TransportName],[GodownName],[LRNumber],[LRDate],[StockStatus],[Other1],[TCSPer],[TCSAmt],[TaxableAmt]) VALUES "
                                        + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + txtInvoiceNo.Text + "'," + strInvoiceDate + ",'" + strPurchasePartyID + "','" + strPurchaseParty + "','"+txtTaxLedger.Text+"','" + txtRemark.Text + "','"+txtImportData.Text+"','" + txtOtherPerSign.Text + "'," + dba.ConvertObjectToDouble(txtPackingAmt.Text)+",'"+ txtSign.Text+"',"+dba.ConvertObjectToDouble(txtOtherAmount.Text)+","+dba.ConvertObjectToDouble(txtDiscPer.Text)+","+dba.ConvertObjectToDouble(txtDiscAmt.Text)+","+dba.ConvertObjectToDouble(txtTaxPer.Text)+","+ dTaxAmt + ","+dba.ConvertObjectToDouble(lblTotalQty.Text)+","+dba.ConvertObjectToDouble(lblGrossAmt.Text)+","+dAmt+",'"+txtROSign.Text+"',"+txtRoundOff.Text+",'"+MainPage.strLoginName+ "','',1,0," + dSpclDisPer + "," + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",'" + txtTransport.Text + "','" + txtGodown.Text + "','" + txtLRNo.Text + "'," + strLRDate + ",'" + txtStockStatus.Text + "','" + strMNID + "',"+dTcsPer+","+dTCSAmt +","+ dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ")  "
                                       + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  " 
                                       + " ('" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "')  ";

                double dQty = 0, dRate = 0, _dDisPer = 0, dMRP =0 ;
                string strSDis = "";
                if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                    strPurchaseParty = strPurchasePartyID = "";
               

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    _dDisPer = ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);

                    if (!txtBillCode.Text.Contains("PTN"))
                    {
                        if (MainPage.strSoftwareType == "AGENT")
                            strSDis =  "-"+_dDisPer;
                        else
                            strSDis = "";
                    }
                    else
                        strSDis = "";

                    strQuery += " INSERT INTO [dbo].[PurchaseBookSecondary] ([BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Discount],[OCharges],[BasicAmt],[UnitName],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PONumber]) VALUES  "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ","
                                  + " " + Convert.ToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + Convert.ToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "',0,'" + MainPage.strLoginName + "','',1,0,'" + txtOrderNo.Text + "')";
                    if(dba.ConvertObjectToDouble(txtSpclDisPer.Text)==0 && txtTaxLedger.Text!="NO STOCK")

                    if (MainPage._bTaxStatus || (dSpclDisPer == 0 && txtImportData.Text == ""))
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                                  + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + strPurchasePartyID + "','" + strPurchaseParty + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'" + strSDis + "','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "') ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@TCSAccount nvarchar(250); ";
                           
                if (dTaxAmt > 0 && txtTaxLedger.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" +txtBillCode.Text+" "+ txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }

                //GST Details
                string strTaxAccountID = "";
                foreach (DataGridViewRow rows in dgrdTax.Rows)
                {
                    strTaxAccountID = "";
                    strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strTaxAccountID = strFullName[0].Trim();
                    }

                    strQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                   + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                if (dTCSAmt > 0)
                {
                    strQuery += " Select @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";


                strQuery += " end ";

                if (strQuery != "")
                {
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        btnAdd.Text = "&Add";
                        AskForPrint();

                        BindRecordWithControl(txtBillNo.Text);                       
                    }
                    else
                        MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    dgrdDetails.ReadOnly = qtyAdjustStatus;
                    txtBillNo.ReadOnly = true;
                    if(dgrdDetails.Rows.Count==0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                    if (txtStockStatus.Text == "")
                        txtStockStatus.Text = "STOCK IN";
                    txtDate.Focus();
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateRecord("");
                        if (count > 0)
                        {
                            MessageBox.Show("Thank you ! Record saved successfully ! ", "Congratulation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            btnEdit.Text = "&Edit";
                            BindRecordWithControl(txtBillNo.Text);                            
                        }
                        else
                            MessageBox.Show("Sorry ! Record not updated, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
        }

        private int UpdateRecord(string strSubQuery)
        {
            int result = 0;
            try
            {
                string strInvoiceDate = "NULL", strLRDate="NULL", strTaxAccountID="";
                DateTime strDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (txtInvoiceDate.Text.Length == 10)
                {
                    DateTime iDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text);
                    strInvoiceDate = "'" + iDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                if (txtLRDate.Text.Length == 10 && txtLRNo.Text != "")
                {
                    DateTime lDate = dba.ConvertDateInExactFormat(txtLRDate.Text);
                    strLRDate = "'" + lDate.ToString("MM/dd/yyyy h:mm:ss tt") + "'";
                }
                double dAmt = Convert.ToDouble(lblNetAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dSpclDisPer=ConvertObjectToDouble(txtSpclDisPer.Text), dTcsPer = 0, dTCSAmt = 0;
                string strPurchaseParty = "", strPurchasePartyID = "", strMNID="";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }
                if (txtmanufacturer.Text != "")
                {
                    strFullName = txtmanufacturer.Text.Split(' ');
                    strMNID = strFullName[0];
                }

                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                }

                string strQuery = " if exists (Select [BillCode] from [PurchaseBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin UPDATE [dbo].[PurchaseBook] Set [Date]='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[InvoiceNo]='" + txtInvoiceNo.Text + "',[InvoiceDate]=" + strInvoiceDate + ",[PurchasePartyID]='" + strPurchasePartyID + "',[PurchaseParty]='" + strPurchaseParty + "',[PurchaseType]='" + txtTaxLedger.Text + "',[Remark]='" + txtRemark.Text + "',[Description]='" + txtImportData.Text + "',[Other]='" + txtOtherPerSign.Text + "',[PackingAmt]=" + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[DiscPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DiscAmt]=" + dba.ConvertObjectToDouble(txtDiscAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblTotalQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dAmt + ",[ROSign]='" + txtROSign.Text + "',[RoundOff]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + " ,[TransportName]='" + txtTransport.Text + "',[GodownName]='" + txtGodown.Text + "',[LRNumber]='" + txtLRNo.Text + "',[LRDate]=" + strLRDate + ",[StockStatus]='" + txtStockStatus.Text + "',[Other1]='" + strMNID + "',[TCSPer]=" + dTcsPer + ",[TCSAmt]=" + dTCSAmt + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "                               
                                + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "',[PartyName]='" + strPurchaseParty + "',[Amount]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strPurchasePartyID + "' Where Description='" + txtBillCode.Text+" "+ txtBillNo.Text + "' and [AccountStatus]='PURCHASE A/C'  "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from StockMaster Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";

                string  strID = "", strSDis="";
                double dQty = 0, dRate = 0, _dDisPer=0,dMRP=0;
                             
                if (txtBillCode.Text.Contains("PTN"))
                    strPurchaseParty = strPurchasePartyID = "";
                if (MainPage.strSoftwareType != "AGENT")
                    strPurchaseParty = strPurchasePartyID = "";

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    _dDisPer = ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);

                    if (!txtBillCode.Text.Contains("PTN"))
                    {
                        if (MainPage.strSoftwareType == "AGENT")
                            strSDis = "-" + _dDisPer;
                        else
                            strSDis = "";
                    }
                    else
                        strSDis = "";

                    strID = Convert.ToString(row.Cells["id"].Value);                    
                    if (strID == "")
                    {                        
                        strQuery += " INSERT INTO [dbo].[PurchaseBookSecondary] ([BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Discount],[OCharges],[BasicAmt],[UnitName],[RemoteID],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PONumber]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dMRP + "," + _dDisPer + "," + dRate + ","
                                + " " + Convert.ToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + Convert.ToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "',0,'" + MainPage.strLoginName + "','',1,0,'"+txtOrderNo.Text+"') ";
                    }
                    else
                    {
                        strQuery += " Update [dbo].[PurchaseBookSecondary] SET [ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dMRP + ",[SDisPer]=" + _dDisPer + ","
                                 + " [Rate]=" + dRate + ",[Amount]=" + Convert.ToDouble(row.Cells["amount"].Value) + ",[Discount]=" + Convert.ToDouble(row.Cells["disc"].Value) + ", [OCharges]=" + Convert.ToDouble(row.Cells["otherCharges"].Value) + ",[BasicAmt]=" + Convert.ToDouble(row.Cells["netAmt"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='"+MainPage.strLoginName+ "',[UpdateStatus]=1,[PONumber]='"+txtOrderNo.Text+"'  Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and ID="+strID+"  ";                      
                    }

                    if (MainPage._bTaxStatus || (dSpclDisPer == 0 && txtImportData.Text == ""))
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date]) VALUES "
                                + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'', '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + strPurchasePartyID + "','" + strPurchaseParty + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'" + strSDis + "','" + MainPage.strLoginName + "','',1,0," + dMRP + ",'" + strDate + "') ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ,@TCSAccount nvarchar(250);";

                if (dTaxAmt > 0 && txtTaxLedger.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end  end ";
                }

                //GST Details
             
                foreach (DataGridViewRow rows in dgrdTax.Rows)
                {
                    strTaxAccountID = "";
                    strFullName = Convert.ToString(rows.Cells["taxName"].Value).Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strTaxAccountID = strFullName[0].Trim();
                    }

                    strQuery += " INSERT INTO [dbo].[GSTDetails] ([BillType],[BillCode],[BillNo],[GSTAccount],[TaxRate],[TaxAmount],[TaxType],[HSNCode],[InsertStatus]) VALUES "
                                   + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                if (dTCSAmt > 0)
                {
                    strQuery += " Select @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                        + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                strQuery += "  Update SM Set SM.BarCode=_IM.BarCode from StockMaster SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo="+txtBillNo.Text
                    + " Update SM Set SM.BarCode=_IM.BarCode from PurchaseBookSecondary SM OUTER APPLY (Select BarCode from Items _IM inner join ItemSecondary _IS on _Im.BillCode=_IS.BillCode and _Im.BillNo=_IS.BillNo Where _Im.ItemName=SM.ItemName and _IS.Variant1=SM.Variant1 and _IS.Variant2=SM.Variant2)_IM Where SM.BillCode='" + txtBillCode.Text + "' and SM.BillNo=" + txtBillNo.Text+" end";

                // end Purchase Entry                         
               
                result = dba.ExecuteMyQuery(strQuery);
            }
            catch
            {
            }
            return result;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            BindLastRecord();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtInvoiceDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, false, false, true);
            }
        }

        private void txtPurchaseLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTaxLedger.Text = objSearch.strSelectedData;
                        CalculateAllAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPackingAmt_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txtNew = sender as TextBox;
                if (txtNew.Text == "0" || txtNew.Text == "00" || txtNew.Text == "0.00")
                    txtNew.Clear();
            }
        }
      

    
        private void txtRoundOff_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtROSign_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                CalculateAllAmount();
            }
        }

        private void txtSerialNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            {
                if (txtBillNo.Text != "")
                    BindRecordWithControl(txtBillNo.Text);
                else
                    ClearAllText();
            }
            //else if (txtSerialNo.Text != "")
            //    CheckSerialNoAvailability();
        }
        
        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
        }

        private void CreateDataTableColumn(ref DataTable dt)
        {
            dt.Columns.Add("HeaderName", typeof(String));
            dt.Columns.Add("CompanyName", typeof(String));
            dt.Columns.Add("CompanyAddress", typeof(String));
            dt.Columns.Add("CompanyEmail", typeof(String));
            dt.Columns.Add("BillNo", typeof(String));
            dt.Columns.Add("Date", typeof(String));
            dt.Columns.Add("PartyName", typeof(String));
            dt.Columns.Add("PartyAddress", typeof(String));
            dt.Columns.Add("PartyEmail", typeof(String));
            dt.Columns.Add("LedgerName", typeof(String));
            dt.Columns.Add("AgentName", typeof(String));
            dt.Columns.Add("TransportName", typeof(String));
            dt.Columns.Add("PONumber", typeof(String));
            dt.Columns.Add("PODate", typeof(String));
            dt.Columns.Add("Remark", typeof(String));
            dt.Columns.Add("SNo", typeof(String));
            dt.Columns.Add("ItemName", typeof(String));
            dt.Columns.Add("Qty", typeof(String));
            dt.Columns.Add("DQty", typeof(String));
            dt.Columns.Add("Rate", typeof(String));
            dt.Columns.Add("Unit", typeof(String));
            dt.Columns.Add("Amount", typeof(String));
            dt.Columns.Add("Disc", typeof(String));
            dt.Columns.Add("OtherCharges", typeof(String));
            dt.Columns.Add("BasicAmt", typeof(String));
            dt.Columns.Add("OtherText", typeof(String));
            dt.Columns.Add("NetAmt", typeof(String));
            dt.Columns.Add("TotalQty", typeof(String));
            dt.Columns.Add("AmountInWord", typeof(String));
            dt.Columns.Add("UserName", typeof(String));
         
        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("SerialNo", typeof(String));
                myDataTable.Columns.Add("PParty", typeof(String));
                myDataTable.Columns.Add("SParty", typeof(String));
                myDataTable.Columns.Add("Date", typeof(String));
                myDataTable.Columns.Add("Qty", typeof(String));
                myDataTable.Columns.Add("NetAmount", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));
                myDataTable.Columns.Add("SupplierHead", typeof(String));
                myDataTable.Columns.Add("SubParty", typeof(String));
                myDataTable.Columns.Add("Freight", typeof(String));
                myDataTable.Columns.Add("Tax", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("Packing", typeof(String));
                myDataTable.Columns.Add("Remark", typeof(String));
                myDataTable.Columns.Add("InvoiceNo", typeof(String));
                myDataTable.Columns.Add("InvoiceDate", typeof(String));
                myDataTable.Columns.Add("BarValue", typeof(String));

                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                DataRow row = myDataTable.NewRow();

                double dOtherAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text) + dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmount.Text);

                if (MainPage.strSoftwareType == "AGENT")
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                else
                    row["CompanyName"] = "";
                if (!btnPrint.Enabled)
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text + "/D";
                else
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text;

                row["InvoiceNo"] = txtInvoiceNo.Text;
                row["InvoiceDate"] = txtInvoiceDate.Text;
                row["BarValue"] = txtBillCode.Text + txtBillNo.Text;

                row["SupplierHead"] = "SUPPLIER";
                row["PParty"] = txtPurchaseParty.Text;

                row["SParty"] = "----";
                row["SubParty"] = "----";
                row["Date"] = txtDate.Text;
                row["Qty"] = lblTotalQty.Text + " Pcs";
                row["Amount"] = lblGrossAmt.Text;
                row["Tax"] = txtTaxAmt.Text;
                if (txtOtherPerSign.Text == "-")
                    row["Freight"] = txtDiscAmt.Text;
                if (dOtherAmt >= 0)
                    row["Packing"] = "(+)" + dOtherAmt.ToString("N2", MainPage.indianCurancy);
                else
                    row["Packing"] = "(-)" + Math.Abs(dOtherAmt).ToString("N2", MainPage.indianCurancy);

                row["NetAmount"] = lblNetAmt.Text;
                row["Remark"] = txtRemark.Text;

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;
              
                row["UserName"] = MainPage.strLoginName + " ,  Date & Time : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");
                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void AskForPrint()
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("ARE YOU WANT TO PRINT PURCHASE SLIP ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            DataTable dt = CreateDataTable();
                            if (dt.Rows.Count > 0)
                            {
                                Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                                objReport.SetDataSource(dt);
                                if (MainPage._PrintWithDialog)
                                    dba.PrintWithDialog(objReport);
                                else
                                    objReport.PrintToPrinter(1, false, 0, 1);

                                objReport.Close();
                                objReport.Dispose();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }


        //private DataTable CreateDataTable()
        //{
        //    DataTable table = new DataTable();
        //    try
        //    {
        //        CreateDataTableColumn(ref table);

        //        string strQuery = " Select CM.CmpAddress as CompanyEmail,CM.CmpCity as EmailPwd, CD.FullName,(Address+'-'+CAST(CD.PinCode as varchar))Address, ('Phone : '+CD.stdCode1+'-'+CD.PhoneNo1 +', Email : '+CD.EmailId) PhoneNo,(SM.PostalAddress+', '+SM.City)PartyAddress,(SM.State+'-'+SM.PinCode) PartyStatePIN, "
        //                               + " ((CASE SM.PhoneNo When '' then '' else ' Phone No. : '+SM.PhoneNo end)+(CASE SM.EmailID When '' then '' else ' Email Id : '+SM.EMailID end)) PartyPhoneNo,SM.TINNumber as BuyerTIN,SM.CSTNumber as BuyerCST,CD.TinNo as CompanyTIN from CompanyDetails  CD,SupplierMaster SM,CompanyMaster CM Where SM.Name='" + txtPartyName.Text + "'"
        //                               + " DECLARE @PO VARCHAR(MAX),@PODate varchar(MAX) SET @PO='' ; SET @PODate='' SELECT  @PO = @PO + (Case When CharIndex('/',PONumber)>1 then SUBSTRING(Replace(PONumber,'PO/',''),0,CharIndex('/',Replace(PONumber,'PO/',''))) else PONumber end) +',',@PODate=@PODate+ Replace(Convert(Varchar(6), PODate, 106), ' ', '-')+',' FROM PurchaseOrder Where PONumber in (Select PONumber from PurchaseBookSecondary Where PONumber!='' and SerialNo=" + txtBillNo.Text + ") "
        //                               + " Select PBS.ItemName,PBS.Category1,PBS.Category2,PBS.Category3,PBS.Category4,PBS.Category5,DM.SupplierDesignName,DM.ManfDesignName,Sum(PBS.Qty) Qty,SUM(ISNULL(PBS.AlterQty,0)) AlterQty,SUM(ISNULL(PBS.DQty,0)) DQty,PBS.Rate,(Sum(PBS.Qty+ISNULL(PBS.DQty,0))*Rate) Amount,SUM(PBS.Discount) Disc,SUM(PBS.OCharges) OtherCharges,SUM(PBS.BasicAmt)BasicAmt, PBS.UnitName,Case When LEN(@PO)>0 then (LEFT(@PO,LEN(@PO)-1)) else '' end AS PONumber, "
        //                               + " Case When LEN(@PODate)>0 then (LEFT(@PODate,LEN(@PODate)-1))else '' end  AS PODate from PurchaseBookSecondary PBS left Join DesignMaster DM on PBS.ItemName=DM.DesignName Where PBS.SerialNo=" + txtBillNo.Text + " Group By PBS.ItemName,PBS.Category1,PBS.Category2,PBS.Category3,PBS.Category4,PBS.Category5,PBS.Rate,PBS.UnitName,DM.SupplierDesignName,DM.ManfDesignName Order by ItemName,PBS.Category2,PBS.Category1";

        //        DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
        //        if (ds.Tables.Count > 1)
        //        {
        //            DataTable dt = ds.Tables[0], dtItem = ds.Tables[1];
        //            DataRow dRow = dt.Rows[0];
        //            string strCompanyName = "", strComapnyAddress = "", strCompanyEmail = "", strPartyName = "", strPartyAddress = "", strPartySP = "", strPartyEmail = "", strAgentName = "", strItemName = "";
        //            //if (MainPage.pCompanyName)
        //            strCompanyName = Convert.ToString(dRow["FullName"]);
        //            //if (MainPage.pCompanyAddress)
        //            //{
        //            strComapnyAddress = Convert.ToString(dRow["Address"]);
        //            strCompanyEmail = Convert.ToString(dRow["PhoneNo"]);
        //            //}
        //            //if (MainPage.pBuyerName)
        //            strPartyName = txtPartyName.Text;
        //            //if (MainPage.pBuyerAddress)
        //            //{
        //            strPartyAddress = Convert.ToString(dRow["PartyAddress"]);
        //            strPartySP = Convert.ToString(dRow["PartyStatePIN"]);
        //            strPartyEmail = Convert.ToString(dRow["PartyPhoneNo"]);
        //            //}
        //            //if (MainPage.pAgentName)
        //            strAgentName = txtAgentName.Text;

        //            int rowIndex = 1;
        //            foreach (DataRow rows in dtItem.Rows)
        //            {
        //                DataRow row = table.NewRow();

        //                row["HeaderName"] = "Purchase Bill";
        //                row["CompanyName"] = strCompanyName;
        //                row["CompanyAddress"] = strComapnyAddress;
        //                row["CompanyEmail"] = strCompanyEmail;
        //                row["PartyName"] = strPartyName;
        //                row["PartyAddress"] = strPartyAddress + "  " + strPartySP;
        //                row["PartyEmail"] = strPartyEmail;
        //                row["LedgerName"] = txtTaxLedger.Text;
        //                row["BillNo"] = txtBillNo.Text;
        //                row["Date"] = txtDate.Text;
        //                row["AgentName"] = txtAgentName.Text;
        //                row["TransportName"] = txtTransport.Text;
        //                row["PONumber"] = rows["PONumber"];
        //                row["PODate"] = rows["PODate"];
        //                if (txtRemark.Text != "")
        //                    row["Remark"] = "Remark : " + txtRemark.Text;

        //                    strItemName = Convert.ToString(rows["ItemName"]);

        //                //if (Convert.ToString(rows["Category2"]) != "")
        //                //    strItemName += " / " + rows["Category2"];

        //                //if (Convert.ToString(rows["Category1"]) != "")
        //                //    strItemName += " / " + rows["Category1"];
        //                row["SNo"] = rowIndex + ".";
        //                row["ItemName"] = strItemName;
        //                if (Convert.ToString(rows["AlterQty"]) == "0")
        //                    row["Qty"] = rows["Qty"];
        //                else
        //                    row["Qty"] = rows["Qty"] + "/" + rows["AlterQty"];
        //                row["DQty"] = rows["DQty"];
        //                row["Unit"] = rows["UnitName"];
        //                row["Rate"] = rows["Rate"];
        //                row["Amount"] = Convert.ToDouble(rows["Amount"]).ToString("N2", MainPage.indianCurancy);
        //                row["Disc"] = rows["Disc"];
        //                row["OtherCharges"] = rows["OtherCharges"];
        //                row["BasicAmt"] = Convert.ToDouble(rows["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
        //                row["UserName"] = MainPage.strLoginName + " ,  Date : " + DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("hh:mm tt");
        //                table.Rows.Add(row);
        //                rowIndex++;
        //            }

        //            AddOtherAmount(ref table);

        //            if (table.Rows.Count > 0)
        //            {
        //                DataRow row = table.Rows[table.Rows.Count - 1];
        //                row["TotalQty"] = lblTotalQty.Text;
        //                row["NetAmt"] = lblNetAmt.Text;
        //                double dAmt = Convert.ToDouble(lblNetAmt.Text);
        //                ChangeCurrencyToWord objCurrency = new ChangeCurrencyToWord();
        //                row["AmountInWord"] = objCurrency.changeCurrencyToWords(dAmt);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return table;
        //}

        //private void AddOtherAmount(ref DataTable table)
        //{
        //    double dPackingAmt = Convert.ToDouble(txtPackingAmt.Text), dOtherAmt = Convert.ToDouble(txtOtherAmount.Text), dTaxAmt = Convert.ToDouble(txtTaxAmt.Text), dDiscount = Convert.ToDouble(txtDiscAmt.Text), dRoundOff = Convert.ToDouble(txtRoundOff.Text);
        //    DataRow row = table.NewRow();
        //    AsignPreviousData(ref row, table);
        //    row["BasicAmt"] = "-------------------";
        //    table.Rows.Add(row);
        //    row = table.NewRow();
        //    AsignPreviousData(ref row, table);
        //    row["BasicAmt"] = lblGrossAmt.Text;
        //    table.Rows.Add(row);

        //    if (dPackingAmt > 0)
        //    {
        //        row = table.NewRow();
        //        AsignPreviousData(ref row, table);
        //        row["Amount"] = "Packing";
        //        row["Disc"] = "";
        //        row["OtherCharges"] = "";
        //        row["BasicAmt"] = "(+) " + dPackingAmt.ToString("N2", MainPage.indianCurancy);
        //        table.Rows.Add(row);
        //    }
        //    if (dOtherAmt > 0)
        //    {
        //        row = table.NewRow();
        //        AsignPreviousData(ref row, table);
        //        row["Amount"] = "Other Amount";
        //        row["Disc"] = "";
        //        row["OtherCharges"] = "";
        //        row["BasicAmt"] = "(" + txtSign.Text + ") " + dOtherAmt.ToString("N2", MainPage.indianCurancy);
        //        table.Rows.Add(row);
        //    }
        //    if (dTaxAmt > 0)
        //    {
        //        row = table.NewRow();
        //        AsignPreviousData(ref row, table);
        //        if (txtTaxLedger.Text != "")
        //            row["Amount"] = txtTaxLedger.Text;
        //        else
        //            row["Amount"] = "Tax";
        //        row["Disc"] = txtTaxPer.Text;
        //        row["OtherCharges"] = "%";
        //        row["BasicAmt"] = "(+) " + dTaxAmt.ToString("N2", MainPage.indianCurancy);
        //        table.Rows.Add(row);
        //    }
        //    //if (dAgentComm > 0)
        //    //{
        //    //    row = table.NewRow();
        //    //    AsignPreviousData(ref row, table);
        //    //    row["OtherAmount"] = "Agent Commission";
        //    //    row["Rate"] = txtAgentCommPer.Text;
        //    //    row["UOM"] = "%";
        //    //    row["Amount"] = "(+) " + dAgentComm.ToString("N2", MainPage.indianCurancy);
        //    //    table.Rows.Add(row);
        //    //}
        //    if (dDiscount > 0)
        //    {
        //        row = table.NewRow();
        //        AsignPreviousData(ref row, table);
        //        row["Amount"] = "Discount";
        //        row["Disc"] = txtDiscPer.Text;
        //        row["OtherCharges"] = "%";
        //        row["BasicAmt"] = "(-) " + dDiscount.ToString("N2", MainPage.indianCurancy);
        //        table.Rows.Add(row);
        //    }
        //    if (dRoundOff > 0)
        //    {
        //        row = table.NewRow();
        //        AsignPreviousData(ref row, table);
        //        row["Amount"] = "Round Off";
        //        row["BasicAmt"] = "(" + txtROSign.Text + ") " + dRoundOff.ToString("N2", MainPage.indianCurancy);
        //        table.Rows.Add(row);
        //    }
        //}

        //private void AsignPreviousData(ref DataRow row, DataTable dt)
        //{

        //    DataRow dr = dt.Rows[dt.Rows.Count - 1];
        //    row["HeaderName"] = dr["HeaderName"];
        //    row["CompanyName"] = dr["CompanyName"];
        //    row["CompanyAddress"] = dr["CompanyAddress"];
        //    row["CompanyEmail"] = dr["CompanyEmail"];
        //    row["PartyName"] = dr["PartyName"];
        //    row["PartyAddress"] = dr["PartyAddress"];
        //    row["PartyEmail"] = dr["PartyEmail"];
        //    row["LedgerName"] = dr["LedgerName"];
        //    row["BillNo"] = dr["BillNo"];
        //    row["Date"] = dr["Date"];
        //    row["AgentName"] = dr["AgentName"];
        //    row["TransportName"] = dr["TransportName"];
        //    row["PONumber"] = dr["PONumber"];
        //    row["PODate"] = dr["PODate"];
        //    row["Remark"] = dr["Remark"];
        //    row["UserName"] = dr["UserName"];         
        //}

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    btnPreview.Enabled = false;
                    DataTable dt = CreateDataTable();
                    Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                    objReport.SetDataSource(dt);
                    Reporting.ShowReport objShow = new Reporting.ShowReport("Purchase Receiving Slip");
                    objShow.myPreview.ReportSource = objReport;
                    objShow.myPreview.ShowPrintButton = false;
                    objShow.myPreview.ShowExportButton = false;
                    objShow.ShowDialog();
                    objReport.Close();
                    objReport.Dispose();
                }
            }
            catch
            {
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {

                    btnPrint.Enabled = false;
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
                        defS.Collate = false;
                        defS.FromPage = 0;
                        defS.ToPage = 0;
                        defS.Copies = (short)MainPage.iNCopyPurchase;

                        Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                        objReport.SetDataSource(dt);
                        if (MainPage._PrintWithDialog)
                            dba.PrintWithDialog(objReport, false, MainPage.iNCopyPurchase);
                        else
                            objReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
                        btnPrint.Enabled = true;
                        objReport.Close();
                        objReport.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Printing in Goods Receive ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }


        private void PurchaseBook_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void SetPermission()
        {
            if (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView)
            {
                if (!MainPage.mymainObject.bPurchaseAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseView)
                    txtBillNo.Enabled = false;

                btnBarCodePrint.Enabled = MainPage.mymainObject.bBarcodePrint;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtTaxLedger.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtTaxLedger.Text, "PURCHASE");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        string strTaxationType = Convert.ToString(row["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                                _strTaxType = "INCLUDED";

                            dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);

                            string strQuery = "", strSubQuery = "", strGRSNo = "";
                            double dDisStatus = 0, dSpclDiscount = ConvertObjectToDouble(txtDiscPer.Text);

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;
                          

                            double dRate = 0, dQty = 0, dAmt = 0, dBasicAmt = 0, dOAmt = 0; 
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);
                                dAmt = dRate * dQty;
                                dAmt=Math.Round(dAmt, 2);

                                dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                                dOAmt += (dBasicAmt - dAmt);                               
                                                               
                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dSpclDiscount + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dSpclDiscount + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
                                }
                            }

                            if (dOAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOAmt + " Amount,"+dTaxPer+" as TaxRate ";
                            }

                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount," + dTaxPer + " as TaxRate ";
                            }
                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate ";

                                strQuery += strSubQuery;

                                DataTable dt = dba.GetDataTable(strQuery);
                                if (dt.Rows.Count > 0)
                                {
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    // BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt ,ref dTaxableAmt);
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    dTaxAmt = dTTaxAmt;
                                    if (dOtherAmt == 0 && dOAmt == 0)
                                        dTaxPer = dMaxRate;
                                    pnlTax.Visible = true;
                                }
                            }
                        }
                        else if (strTaxationType == "VOUCHERWISE")
                        {
                            double _dTaxPer = dba.ConvertObjectToDouble(row["TaxRate"]);
                            if (_dTaxPer > 0)
                            {
                                dTaxAmt = (dFinalAmt * _dTaxPer) / 100;
                            }
                            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                            txtTaxPer.Text = _dTaxPer.ToString("0.00");
                            pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text = "0.00";
                    }
                }
                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bPurchaseAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bPurchaseEdit)
                    btnEdit.Enabled = false;

            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
                MessageBox.Show("Error ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEdit.Enabled = btnAdd.Enabled = false;
            }

            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);

            if (_strTaxType == "INCLUDED")
                dTaxAmt = 0;
            return dTaxAmt;
        }


        //private void BindTaxDetails(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt,ref double dTaxableAmt)
        //{
        //    try
        //    {
        //        dgrdTax.Rows.Clear();
        //        if (_dt.Rows.Count > 0)
        //        {
        //            dgrdTax.Rows.Add(_dt.Rows.Count);
        //            int _index = 0;
        //            string strRegion = Convert.ToString(_row["Region"]), strIGST = Convert.ToString(_row["IGSTName"]), strSGST = Convert.ToString(_row["SGSTName"]); ;
        //            if (strRegion == "LOCAL")
        //                dgrdTax.Rows.Add(_dt.Rows.Count);
        //            double dTaxRate = 0, dTaxAmt = 0;

        //            foreach (DataRow row in _dt.Rows)
        //            {
        //                dTaxRate = dba.ConvertObjectToDouble(row["TaxRate"]);
        //                dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);
        //                if (dTaxRate > dMaxRate)
        //                    dMaxRate = dTaxRate;

        //                dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
        //                dgrdTax.Rows[_index].Cells["taxName"].Value = strIGST;
        //                dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;

        //                if (strRegion == "LOCAL")
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
        //                    _index++;
        //                    dgrdTax.Rows[_index].Cells["taxName"].Value = strSGST;
        //                    dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
        //                }
        //                else
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = dTaxRate.ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = dTaxAmt.ToString("N2", MainPage.indianCurancy);
        //                }

        //                _index++;
        //            }
        //        }
        //    }
        //    catch { }
        //}


        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " Delete from PurchaseBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                            + " Delete from PurchaseBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                            + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('PURCHASE A/C','DUTIES & TAXES')  "
                                            + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                            + " Delete from StockMaster Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                DataBaseAccess.CreateDeleteQuery(strQuery);
                                MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtReason.Text = "";
                                pnlDeletionConfirmation.Visible = false;
                                BindNextRecord();
                            }
                            else
                                MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReason.Focus();
                }
            }
            catch
            {
            }
            btnFinalDelete.Enabled = true;
        }

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtInvoiceNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtPartyName_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }     

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("PURCHASE", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtOtherPerSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch { }
        }

        private void txtOtherPerSign_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOtherPerSign.Text == "")
                        txtOtherPerSign.Text = "+";
                    CalculateAllAmount();
                }
            }
            catch { }
        }

        private void txtDiscPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 3);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASECODE", "SEARCH PURCHASE BILL CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            txtBillCode.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtOrderNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("RETAILORDERNO", "SEARCH RETAIL ORDER NO", e.KeyCode);
                        objSearch.ShowDialog();
                        txtOrderNo.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSpclDisPer_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSpclDisPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSpclDisPer.Text == "")
                    txtSpclDisPer.Text = "0";
                CalculateAllAmount();
            }
        }

        private void chkPick_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtImportData.Enabled = chkPick.Checked;
                txtImportData.Clear();
            }
            else
            {
                txtImportData.Enabled = false;
                txtImportData.Clear();
            }
        }

        private void txtImportData_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!MainPage._bTaxStatus && (btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strPurchasePartyID = "";
                        if (txtPurchaseParty.Text != "")
                        {
                            string[] str = txtPurchaseParty.Text.Split(' ');
                            if (str.Length > 1)
                            {
                                strPurchasePartyID = str[0];
                                SearchDataOther objSearch = new SearchDataOther("PURCHASEBILLNO", strPurchasePartyID, "SEARCH PURCHASE BILL NO", e.KeyCode, false);
                                objSearch.ShowDialog();
                                if (objSearch.strSelectedData != "")
                                {
                                    str = objSearch.strSelectedData.Split('|');
                                    if (str.Length > 1)
                                    {
                                        txtImportData.Text = str[0];                                       
                                        GetDataFromLocal();
                                    }
                                }
                            }
                        }
                        else
                            MessageBox.Show("Sorry ! Please enter supplier name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetDataFromLocal()
        {
            if (txtImportData.Text != "" && btnAdd.Text == "&Save")
            {
                BindRecordWithControlWithImport();
            }
        }

        private void BindRecordWithControlWithImport()
        {
            try
            {
                string strQuery = " Select PBS.*,GAmt from  PurchaseBookSecondary PBS Outer APPLY (Select (CASE WHEN PurchaseType Like('%INCLUDE%') then (GrossAmt-TaxAmt) else GrossAmt end)GAmt,ISNULL(SpecialDscPer,0)SpecialDscPer from PurchaseBook PB Where PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo) PB  Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'  order by ID ";

                DataSet ds = SearchDataOther.GetDataSet(strQuery);
                DataTable dt = null;
                dgrdDetails.Rows.Clear();

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow _row = dt.Rows[0]; 
                            txtOtherAmount.Text = Convert.ToString(_row["GAmt"]);
                            txtSign.Text = "-";

                            int rowIndex = 0;

                            dgrdDetails.Rows.Add(dt.Rows.Count);
                            foreach (DataRow row in dt.Rows)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                                dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row["ItemName"];
                                dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = row["Variant1"];
                                dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = row["Variant2"];
                                dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = row["Variant3"];
                                dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = row["Variant4"];
                                dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = row["Variant5"];
                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = row["Qty"];
                                dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = row["SDisPer"];
                                dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = Convert.ToDouble(row["MRP"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["rate"].Value = Convert.ToDouble(row["Rate"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Discount"];
                                dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                                dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                                dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                                rowIndex++;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private bool ValidateFromPreviousBill(bool _bStatus)
        {
            string strPurchasePartyID = "";
            string[] strFullName = txtPurchaseParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strPurchasePartyID = strFullName[0].Trim();
            
            string strQuery = "Select BillNo from PurchaseBook  Where BillNo!=" + txtBillNo.Text + " and PurchasePartyID='" + strPurchasePartyID + "' and LTRIM(RTRIM(InvoiceNo)) Like('" + txtInvoiceNo.Text.Trim() + "') ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (Convert.ToString(objValue) != "")
            {
                MessageBox.Show("Sorry ! This detail might be saved in Bill no : " + objValue + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (_bStatus)
                    txtInvoiceNo.Focus();
                return false;
            }
            else
                return true;
        }

        private void chkImportPDF_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkImportPDF.Enabled = false;
                if (btnAdd.Text == "&Save")
                {
                    if (chkImportPDF.Checked)
                    {
                        string strFilePath = "";
                        OpenFileDialog _browser = new OpenFileDialog();
                        _browser.Filter = "PURCHASE PDF Files (*.pdf)|*.pdf;";
                        _browser.ShowDialog();
                        if (_browser.FileName != "")
                        {
                            strFilePath = _browser.FileName;
                            txtPDFFileName.Text = _browser.SafeFileName;
                            ExtractDataFromPDF(strFilePath);
                            _strPDFFilePath = strFilePath;
                        }
                    }
                    else
                    {
                        _strPDFFilePath = txtPDFFileName.Text = "";                        
                    }
                }
            }
            catch
            {
            }
            chkImportPDF.Enabled = true;
        }

        private void CopyPDFFileInGSTFolder()
        {
            try
            {
                if (_strPDFFilePath != "")
                {
                    string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Imported_PDF";
                    System.IO.Directory.CreateDirectory(strPath);

                    strPath += "\\" + txtBillNo.Text + ".pdf ";

                    if (System.IO.File.Exists(_strPDFFilePath))
                        System.IO.File.Copy(_strPDFFilePath, strPath, true);
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void DeleteImortedPDFFileInGSTFolder()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Imported_PDF";
                System.IO.Directory.CreateDirectory(strPath);
                strPath += "\\" + txtBillNo.Text + ".pdf ";
                if (System.IO.File.Exists(_strPDFFilePath))
                    System.IO.File.Delete(strPath);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnOpenFile.Enabled = false;

                if (txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    DataBaseAccess.ShowPDFFiles(txtBillCode.Text, txtBillNo.Text);
                }
            }
            catch
            {
            }
            btnOpenFile.Enabled = true;
        }

        private void ExtractDataFromPDF(string strPath)
        {
            try
            {
                ClearAllTextForPDF();
                SetSerialNo();

                PdfReader reader = new PdfReader(strPath);
                int PageNum = reader.NumberOfPages;
                string text = "";
                int _itemIndex = 0;
                dgrdDetails.Rows.Clear();
                bool _bEndStatus = false, _bLongLable = false, bByteData = false;
                for (int i = 1; i <= PageNum; i++)
                {
                    try
                    {
                        text = PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());
                    }
                    catch { text = ReadPDFData.GetTextFromPDF(strPath, i); bByteData = true; }

                    if (i == 1)
                    {
                        if (text.Trim() == "")
                        {
                            MessageBox.Show("Sorry ! Please select valid pdf file !! ", "PDF file not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                        SetBasicDetails(ref _itemIndex, text);
                    }
                    else
                    {
                        if ((txtPurchaseParty.Text.Contains("FULLTOSS") || txtPurchaseParty.Text.Contains("JAI AMBEY")) && i == 2)
                            _itemIndex -= 8;
                        else if ((txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP")) && i == 2)
                            _itemIndex -= 22;
                        else if ((txtPurchaseParty.Text.Contains("MISHU ENTERPRISES")) && i == 2)
                            _itemIndex = 19;
                        else if (txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS"))
                            _itemIndex = 10;
                    }

                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP") || txtPurchaseParty.Text.Contains("BONNYS NX"))
                        _bEndStatus = SetItemDetailsByCustomize_Branches(_itemIndex, text, ref _bLongLable);
                    else if (txtPurchaseParty.Text.Contains("SARAOGI SUPER SALES"))
                        _bEndStatus = SetItemDetailsByCustomize_Saraogi(_itemIndex, text, ref _bLongLable);
                    else if (txtPurchaseParty.Text.Contains("LUCKY JACKET") || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS") || txtPurchaseParty.Text.Contains("KC GARMENTS") || txtPurchaseParty.Text.Contains("JANAK GARMENTEX") || txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") || txtPurchaseParty.Text.Contains("NIKUNJ TRADING") || txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") || txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS") || txtPurchaseParty.Text.Contains("DONARGOLD GARMENTS") || txtPurchaseParty.Text.Contains("W STAN GARMENTS") || txtPurchaseParty.Text.Contains("GEX GARMENTS") || txtPurchaseParty.Text.Contains("MOTI FASHIONS") || txtPurchaseParty.Text.Contains("TANEJA FASHION") || txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") || txtPurchaseParty.Text.Contains("VIPIN COLLECTION") || txtPurchaseParty.Text.Contains("JOLLY FASHIONS") || txtPurchaseParty.Text.Contains("CHANCELLOR INDUSTRIES") || txtPurchaseParty.Text.Contains("MAA PADMAVATI APPARELS") || txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("CLASSIN APPARELS") || txtPurchaseParty.Text.Contains("MAUZ FASHIONS") || txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("AASHI COLLECTION") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") || txtPurchaseParty.Text.Contains("S.R CREATION") || txtPurchaseParty.Text.Contains("SHUBHI GARMENTS") || txtPurchaseParty.Text.Contains("WORLD CHOICE") || txtPurchaseParty.Text.Contains("MIKEY FASHION") || txtPurchaseParty.Text.Contains("WORLD SAHAB") || txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) //|| txtPurchaseParty.Text.Contains("N.D. FASHION")|| txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES")
                        _bEndStatus = SetItemDetailsByCustomize_Delhi(_itemIndex, text, ref _bLongLable);
                    else if (_strBillType == "BUSY" || txtPurchaseParty.Text.Contains("SHRI KRISHNA GARMENTS"))
                        _bEndStatus = SetItemDetailsLineByBusy(_itemIndex, text);
                    else if (bByteData)
                        _bEndStatus = SetItemDetailsLineByLine(_itemIndex, text);
                    else
                        _bEndStatus = SetItemDetails(_itemIndex, text, ref _bLongLable);

                    if (_bEndStatus)
                        break;
                }
                //txtSalesParty.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
          
            CalculateAllAmount();

            if (txtInvoiceNo.Text != "")
                ValidateFromPreviousBill(false);
        }


        private bool SetItemDetails(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :"))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        string __strFreight = _data[_data.Length - 1];
                        if (__strFreight.Length == 4 && (__strFreight.Contains("62") || __strFreight.Contains("63")))
                        {
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmount.Text = _data[0];
                        }
                        else
                            txtOtherAmount.Text = _data[_data.Length - 1];
                    }
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtPackingAmt.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("CHARGES"))
                {
                    strText = _lines[_index + 1].Trim();
                    if (strText != "")
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(strText).ToString("0.00");
                }
                else if (!strText.Contains("Less") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST") && !strText.ToUpper().Contains("DISCOUNT") && !strText.ToUpper().Contains("JURISDICTION") && !strText.ToUpper().Contains("COMPUTER"))
                {
                    strItem = strQty = strRate = "";
                    strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                    string[] _data = strText.Split(' ');
                    if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        if (_data.Length < 8)
                        {
                            if (_data.Length > 1)
                            {
                                strHSNCode = _data[0];
                                for (int i = 1; i < _data.Length - 4; i++)
                                {
                                    if (_data[i].Contains(".00"))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " " + _data[i];
                                        else
                                            strItem = _data[i].Trim();
                                    }
                                }

                                strText = _lines[_index + 1].Trim();
                                _data = strText.Split(' ');
                                if (_data.Length > 1)
                                {
                                    strQty = _data[0];
                                    strRate = _data[_data.Length - 1];
                                }
                                _index++;
                            }
                        }
                        else
                        {
                            strHSNCode = _data[0];
                            for (int i = 1; i < _data.Length - 4; i++)
                            {
                                if (_data[i].Contains(".00"))
                                    break;
                                else
                                {
                                    if (strItem != "")
                                        strItem += " " + _data[i];
                                    else
                                        strItem = _data[i].Trim();
                                }
                            }

                            strQty = _data[_data.Length - 6];
                            strRate = _data[_data.Length - 1];
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("TAANI INDUSTRIES PVT LTD") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                strDescription = _lines[_index + 1].Trim();

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - 2];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_data.Length - 3];
                                    strRate = _data[_data.Length - 4];
                                }
                                else
                                    strRate = _data[_data.Length - 3];
                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "" && !strDescription.Contains(",") && !strDescription.Contains(".00") && !strDescription.ToLower().Contains("continue") && strDescription.Length > 2)
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                strDescription = _lines[_index + 1].Trim();

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - 2];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_data.Length - 3];
                                    strRate = _data[_data.Length - 4];
                                }
                                else
                                    strRate = _data[_data.Length - 3];
                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "" && !strDescription.Contains(",") && !strDescription.Contains(".00") && !strDescription.ToLower().Contains("continue") && strDescription.Length > 2)
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("SNS GARMENTS"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                int _qtyIndex = 0;
                                strDescription = _lines[_index + 1].Trim();
                                if (strDescription.ToUpper().Contains("CONTINUED"))
                                    strDescription = "";

                                strHSNCode = _data[_data.Length - 1];
                                strQty = _data[_data.Length - _qtyIndex - 2];
                                if (strQty == "%")
                                    _qtyIndex = 3;
                                //strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                //if (strQty == "" || strQty == "." || strQty == "..")
                                //    _qtyIndex++;

                                strQty = _data[_data.Length - _qtyIndex - 2];
                                strRate = _data[_data.Length - _qtyIndex - 3];

                                int i = 0;
                                if (_data[0] == _lineIndex.ToString())
                                    i = 1;
                                for (; i < 3; i++)
                                {
                                    if (_data[i].Contains(",") || _data[i].Contains("."))
                                        break;
                                    else
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += _data[i].Trim();
                                    }
                                }
                                if (strItem != "" && strDescription != "")
                                    strItem += " " + strDescription;
                            }
                        }
                    }
                    else if (_data.Length > 7 || ((txtPurchaseParty.Text.Contains("SOMANI") || txtPurchaseParty.Text.Contains("BATRA EXCLUSIVE") || txtPurchaseParty.Text.Contains("G.D. CREATION") || txtPurchaseParty.Text.Contains(" P.R. ENTERPRISES")) && _data.Length > 6))
                    {
                        int _length = _data.Length;
                        _bLongLable = true;
                        if (txtPurchaseParty.Text.Contains("SPARKY"))
                        {
                            strHSNCode = _data[_length - 7];
                            strQty = _data[_length - 6];
                            strRate = _data[_length - 4];

                            for (int i = 0; i < _length - 7; i++)
                            {
                                if (strItem != "")
                                    strItem += " " + _data[i];
                                else if (i == 0)
                                    strItem = _data[i].Replace(_lineIndex.ToString(), "").Trim();
                                else
                                    strItem = _data[i].Trim();
                            }
                        }
                        else if (txtPurchaseParty.Text.Contains("JAI AMBEY"))
                        {
                            _data = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Split(' ');
                            if (_data.Length > 9)
                            {
                                _length = _data.Length;
                                strHSNCode = _data[_length - 8];
                                strQty = _data[_length - 3];
                                strRate = _data[_length - 7];

                                for (int i = 0; i < _length - 8; i++)
                                {
                                    if (strItem != "")
                                        strItem += " " + _data[i];
                                    else if (i == 0)
                                    {
                                        strItem = _data[i].Replace(_lineIndex.ToString(), "").Trim();
                                    }
                                    else
                                        strItem = _data[i].Trim();
                                }
                            }
                        }
                        else
                        {
                            strHSNCode = _data[_length - 1];
                            int qtyIndex = 0;
                            string strDescription = "";
                            if (_data[_length - 2] == "%")
                            {
                                strQty = _data[_length - 5];
                                strRate = _data[_length - 6];
                                qtyIndex = 1;
                            }
                            else
                            {
                                strQty = _data[_length - 3];
                                strRate = _data[_length - 4];
                                strQty = System.Text.RegularExpressions.Regex.Replace(strQty, "[^0-9.]", "");
                                if (strQty == "" || strQty == "." || strQty == "..")
                                {
                                    strQty = _data[_length - 4];
                                    strRate = _data[_length - 5];
                                    qtyIndex = 1;
                                }
                                strRate = System.Text.RegularExpressions.Regex.Replace(strRate, "[^0-9.]", "");
                                if (strRate == "" || strRate == "." || strRate == "..")
                                {
                                    strQty = _data[_length - 5];
                                    strRate = _data[_length - 6];
                                    qtyIndex = 2;
                                }
                            }

                            strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                            int _hsnIndex = 0;
                            if (strHSNCode == "")
                            {
                                string[] _hsnData = _lines[_index + 1].Trim().Split(' ');
                                strHSNCode = _hsnData[0];
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if (strHSNCode.Length != 4)
                                    strHSNCode = _hsnData[_hsnData.Length - 1];
                                strQty = _data[_length - 2];
                                strRate = _data[_length - 3];
                                _hsnIndex--;
                            }

                            int colIndex = 6;
                            if (strText.Contains("%"))
                                colIndex = 8;
                            if (Regex.Matches(strText, "%").Count > 1)
                                colIndex = 9;
                            string __strItem = "";
                            for (int i = 0; i < (_length - colIndex - _hsnIndex - qtyIndex); i++)
                            {
                                __strItem = _data[i];
                                if (__strItem != "" && !__strItem.Contains(".00") && !__strItem.Contains(".10") && !__strItem.Contains(".20") && !__strItem.Contains(".30") && !__strItem.Contains(".40") && !__strItem.Contains(".50") && !__strItem.Contains(".60") && !__strItem.Contains(".70") && !__strItem.Contains(".80") && !__strItem.Contains(".90"))
                                {
                                    if (strItem != "")
                                        strItem += " " + __strItem;
                                    else if (__strItem.Length > 2)
                                    {
                                        if (__strItem.Substring(0, 2).Trim() == _lineIndex.ToString() || __strItem.Substring(0, 2).Trim() == _lineIndex.ToString() + ".")
                                            __strItem = __strItem.Substring(2, __strItem.Length - 2);
                                        strItem = __strItem.Trim();
                                    }
                                }
                            }
                            if (strItem == "" && strHSNCode != "" && strRate != "")
                                strItem = _data[0];
                            if (txtPurchaseParty.Text.Contains("ENGLISH CHANNEL") && !_lines[_index + 1].ToUpper().Contains("GST"))
                                strDescription = _lines[_index + 1].Trim();
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                        }
                    }
                    else if (_data.Length > 2 && !_bLongLable)
                    {
                        if (!strText.ToUpper().Contains("BATCH :"))
                        {
                            strAmount = _data[_data.Length - 1];
                            strItem = strText.Replace(strAmount, "").Trim();
                            _data = strText.Split(' ');
                            if (_data[0] == _lineIndex.ToString() || _data[0] == _lineIndex + ".")
                                strItem = strItem.Substring(_data[0].Length);

                            if (strItem != "")
                            {
                                _index++;
                                strText = _lines[_index];
                                _data = strText.Split(' ');
                                if ((_data.Length == 6 && Regex.Matches(strText, "%").Count > 0) || Regex.Matches(strText, "%").Count > 1)
                                {
                                    strRate = _data[3];
                                    strQty = _data[4];
                                }
                                else
                                    strRate = _data[_data.Length - 1];
                                if (_lines[_index + 1].Length == 4)
                                    strHSNCode = _lines[_index + 1];
                                if (strQty == "")
                                {
                                    _index++;
                                    strText = _lines[_index];
                                    _data = strText.Split(' ');
                                    if (_data.Length > 2)
                                    {
                                        strQty = _data[0];
                                        strHSNCode = _data[_data.Length - 1];
                                    }
                                    else if (_data.Length > 0)
                                    {
                                        strQty = _data[0];
                                        _index++;
                                        strText = _lines[_index];
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                        {
                                            strHSNCode = _data[0];
                                            if (strHSNCode.Length < 4)
                                                strHSNCode = _data[_data.Length - 1];
                                            strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                            if (strHSNCode == "")
                                            {
                                                string[] _hsnData = _lines[_index + 1].Trim().Split(' ');
                                                strHSNCode = _hsnData[0];
                                                _index++;
                                            }
                                            else
                                            {
                                                string strDescription = _lines[_index + 1].Trim().ToUpper();
                                                string[] __data = strDescription.Split(' ');
                                                if (__data[0] != (_lineIndex + 1).ToString() && __data[0] != (_lineIndex + 1) + "." && !strDescription.Contains(".00") && !strDescription.Contains("CONTINUED") && !strDescription.Contains(","))
                                                {
                                                    if (strDescription != "" && strItem != "")
                                                        strItem += " " + strDescription;
                                                    _index++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (_data.Length > 1 && (_lines[_index - 1].Trim() == _lineIndex.ToString() || _data[0] == _lineIndex.ToString()))
                    {
                        strAmount = _data[_data.Length - 1];
                        strItem = strText.Replace(strAmount, "").Trim();
                        _data = strText.Split(' ');
                        if (_data[1] != "%")
                        {
                            if (_data[0] == _lineIndex.ToString() || _data[0] == _lineIndex + ".")
                                strItem = strItem.Substring(_data[0].Length);

                            _index++;
                            strText = _lines[_index];
                            _data = strText.Split(' ');

                            if ((_data.Length == 6 && Regex.Matches(strText, "%").Count > 0) || Regex.Matches(strText, "%").Count > 1)
                            {
                                strRate = _data[3];
                                strQty = _data[4];
                            }
                            else
                                strRate = _data[_data.Length - 1];
                            if (_lines[_index + 1].Length == 4)
                                strHSNCode = _lines[_index + 1];
                            if (strHSNCode == "")
                            {
                                _data = _lines[_index + 1].Trim().Split(' ');
                                strHSNCode = _data[_data.Length - 1];
                                _index++;
                            }
                            if (strQty == "")
                            {
                                _index++;
                                strText = _lines[_index];
                                _data = strText.Split(' ');
                                if (_data.Length > 2)
                                {
                                    strQty = _data[0];
                                    strHSNCode = _data[_data.Length - 1];
                                }
                                else if (_data.Length > 0)
                                {
                                    strQty = _data[0];
                                    _index++;
                                    strText = _lines[_index];
                                    _data = strText.Split(' ');
                                    if (_data.Length > 0)
                                        strHSNCode = _data[0];
                                }
                            }
                        }
                        else
                            strAmount = strItem = "";
                    }

                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();

                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["srNo"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["qty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["mrp"].Value = strRate;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["variant1"].Value = "";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["variant2"].Value = "";
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        if (strItem != "")
                        {
                            dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                            GetPurchaseRate_Import(dgrdDetails.Rows[_lineIndex - 1]);
                            SetUnitName(strItem, _lineIndex - 1);
                        }
                        else
                        {
                            dgrdDetails.Rows[_lineIndex - 1].DefaultCellStyle.BackColor = Color.Tomato;
                            CalculateDisWithAmountMRP_Current(dgrdDetails.Rows[_lineIndex - 1]);
                        }

                        _lineIndex++;
                    }
                }
            }
            return false;
        }

        private bool SetItemDetailsLineByLine(int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex - 1, _lineIndex = 1, _lineGap = 0, qtyIndex = 0, hsnCodeIndex = 0, rateIndex = 0, itemIndex = 0, _startIndex = 0;
            string strText = "", strItem = "", strHSNCode = "", strQty = "", strRate = "", strSNo = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            if (_index < 0)
                _index = 0;
            if (_index > 0 && _lines[_index - 1].Trim().ToUpper().Contains("NO. & KIND"))
                _lineGap++;

            for (; _index < _lines.Length; _index++)
            {
                string strLine = _lines[_index];
                if (strLine.Contains("Description"))
                {
                    itemIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("Quantity"))
                {
                    qtyIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("Rate"))
                {
                    rateIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Contains("HSN"))
                {
                    hsnCodeIndex = _lineGap;
                    _startIndex++;
                }
                else if (strLine.Replace("\r", "") == "No.")
                {
                    _lineGap++;
                    _index++;
                    break;
                }
                if (_startIndex > 0)
                    _lineGap++;
            }
            // _index = _itemIndex + _lineGap;
            //_lineGap--;
            for (; _index < _lines.Length - 2;)
            {
                strText = _lines[_index];
                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND")) && !strText.ToUpper().Contains(" TOTAL : "))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING"))
                {
                    strText = _lines[_index + 1];
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 0)
                        txtOtherAmount.Text = _data[0];
                    break;
                }
                else if (strText.ToUpper().Contains("ADD : PACKING CHARGE"))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        string strPacking = _data[_data.Length - 1];
                        txtPackingAmt.Text = strPacking;
                        strPacking = System.Text.RegularExpressions.Regex.Replace(strPacking.Replace("/", ""), @"[\d-]", string.Empty); ;
                        if (strPacking.Length > 2)
                        {
                            _data = _lines[_index + 1].Split(' ');
                            if (_data.Length > 0)
                                txtPackingAmt.Text = _data[_data.Length - 1].Trim();
                        }
                    }
                    break;
                }
                else if (!strText.Contains("Less") && !strText.ToUpper().Contains("GST"))
                {
                    strItem = strQty = strRate = "";
                    strItem = strText.Replace("\r", "");
                    strSNo = _lines[_index - 1].Replace("\r", "");

                    if (strItem != "" && strSNo == _lineIndex.ToString())
                    {
                        strItem = _lines[_index + itemIndex].Replace("\r", "");
                        strHSNCode = _lines[_index + hsnCodeIndex].Trim().Replace("\r", "");
                        if (strHSNCode.Length == 8 && strHSNCode.Contains("000"))
                            strHSNCode = strHSNCode.Substring(0, 4);
                        string _strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, @"[\d-]", string.Empty);


                        int __index = 0;
                        if (_strHSNCode != "" || strHSNCode.Length != 4)
                        {
                            __index--;
                            if (_lineIndex == 1)
                                _lineGap--;
                            strHSNCode = _lines[_index + hsnCodeIndex + __index].Replace("\r", "");
                        }

                        strRate = _lines[_index + rateIndex + __index].Replace("\r", "");
                        string _strRate = System.Text.RegularExpressions.Regex.Replace(strRate, "[^0-9]", "");
                        if (_strRate == "")
                        {
                            __index++;
                            strRate = _lines[_index + rateIndex + __index].Replace("\r", "");
                        }
                        strQty = _lines[_index + qtyIndex + __index];
                        // strHSNCode = _lines[_index + hsnCodeIndex+ __index].Replace("\r", "");
                        string[] _strQty = strQty.Split(' ');
                        strQty = _strQty[0];

                        strItem = strItem.Replace("'", "").Trim();

                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        _index += _lineGap - 1;
                    }
                    else
                        _index++;
                }
                else
                    _index++;
            }
            return false;
        }

        private bool SetItemDetailsLineByBusy(int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex - 1, _lineIndex = 1, _rateIndex = 0, _qtyIndex = 0;
            string strText = "", strItem = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;

            for (; _index < _lines.Length - 4;)
            {
                strText = _lines[_index].Trim();
                if (strText.Contains("MRP      RATE"))
                    _rateIndex++;
                if (strText.Contains("Art No."))
                    _qtyIndex++;

                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND")) && !strText.ToUpper().Contains(" TOTAL : ") && !strText.ToUpper().Contains("TOTALS C/O") && !strText.ToUpper().Contains("LIST PRICE DISCOUNT"))
                    return true;
                else if (strText.ToUpper().Contains("FREIGHT"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 3)
                    {
                        txtOtherAmount.Text = _data[_data.Length - 1];
                    }
                    else
                    {
                        strText = _lines[_index + 1];
                        _data = strText.Split(' ');
                        if (_data.Length > 0 && _data.Length < 10)
                            txtOtherAmount.Text = _data[0];
                    }
                    _index++;
                }
                else if (strText.ToUpper().Contains("ADD : PACKING CHARGE"))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;

                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtPackingAmt.Text = _data[_data.Length - 1];
                    _index++;
                }
                else if (strText.ToUpper().Contains("ADD  :") && !strText.ToUpper().Contains("CGST") && !strText.ToUpper().Contains("SGST") && !strText.ToUpper().Contains("IGST") && !strText.ToUpper().Contains("R/O"))
                {
                    string[] _data = strText.Replace("  ", " ").Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmount.Text = _data[_data.Length - 1];
                    _index++;
                }
                else if (!strText.Contains("Less") && !strText.ToUpper().Contains("CGST") && !strText.ToUpper().Contains("SGST") && !strText.ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    if (_lines[_index - 1].Trim() == _lineIndex + ".")
                    {
                        strItem = strText;// _lines[_index + 1].Trim();
                        strText = _lines[_index + 1].Replace("|", ":").Trim();
                        strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                        string[] __str = strText.Split('|');
                        if (__str.Length > 2)
                        {
                            strHSNCode = __str[0].Trim();
                            strQty = __str[1].Trim();
                            strRate = __str[2].Trim();

                            string[] __strQty = strQty.Split(' ');
                            strQty = __strQty[0];

                            if (__strQty.Length == 1)
                                strRate = __str[3].Trim();

                            __str = strRate.Split(' ');
                            strRate = __str[0];
                        }

                        _index += 1;

                        if (strItem != "")
                        {
                            strItem = strItem.Replace("'", "").Trim();
                            dgrdDetails.Rows.Add();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                            dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                            if (txtPurchaseParty.Text.Contains("SONKHIYA"))
                            {
                                dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = "BABA SUIT : " + strItem;
                                if (strHSNCode.Contains("6203"))
                                    strItem = "BABA SUIT : 6203";
                                else if (strHSNCode.Contains("6103"))
                                    strItem = "BABA SUIT:6103";
                                else
                                    CheckItemNameExistence(ref strItem, ref strHSNCode);
                            }
                            else
                                CheckItemNameExistence(ref strItem, ref strHSNCode);

                            dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                            _lineIndex++;
                            _index++;
                        }
                        else
                            _index++;
                    }
                    else if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + ".")
                    {
                        strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                        if (strText.Length > 5)
                        {
                            if (strText.Substring(strText.Length - 2, 2) == "|%")
                                strText = strText.Substring(0, strText.Length - 2);
                        }
                        string[] __str = strText.Split('|');
                        if (__str.Length > 2 || txtPurchaseParty.Text.Contains("DOLLCY GARMENTS"))
                        {
                            strItem = strQty = strRate = "";
                            int index = 0;
                            strItem = __str[0].Replace(_lineIndex + ".", "").Replace(". ", " ").Trim();
                            if (txtPurchaseParty.Text.Contains("DOLLCY GARMENTS") && __str.Length < 4)
                            {
                                strHSNCode = __str[1].Trim();
                                _rateIndex = 0;
                                if (__str.Length > 2)
                                {
                                    string[] _strQty = __str[2].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strQty = _strQty[0];
                                }
                                else
                                {
                                    strText = _lines[_index + 1].Trim();
                                    strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    __str = strText.Split('|');
                                    if (__str.Length > 0)
                                        strQty = __str[__str.Length - 1];
                                    _rateIndex = 1;
                                }

                                strText = _lines[_index + _rateIndex + 1].Trim();
                                strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                __str = strText.Split('|');

                                if (__str.Length > 0)
                                    strRate = __str[__str.Length - 1];
                            }
                            else if (__str.Length == 3)
                            {
                                string[] _strHSNCode = strItem.Split(' ');
                                if (_strHSNCode[_strHSNCode.Length - 1].Trim().Length == 4)
                                {
                                    strItem = strItem.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                    strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                }
                                string[] _strQty = __str[1].Trim().Split(' ');
                                if (_strQty.Length > 0)
                                    strQty = _strQty[0];
                                string[] _strRate = __str[2].Trim().Split(' ');
                                if (_strRate.Length > 0)
                                    strRate = _strRate[0];
                            }
                            else
                            {
                                strHSNCode = __str[1].Trim();
                                if (strHSNCode.Length == 8 && (strHSNCode.Contains("000") || strHSNCode.Contains("990")))
                                    strHSNCode = strHSNCode.Substring(0, 4);
                                string __strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if ((__strHSNCode.Length != 4 || strHSNCode.Length != 4) && strHSNCode != "63" && strHSNCode != "62")
                                {
                                    string[] _strHSNCode = strHSNCode.Split(' ');
                                    __strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                    string strNewHSNCode = __str[2].Trim(), ___strHSNCode = System.Text.RegularExpressions.Regex.Replace(__strHSNCode, "[^0-9]", "");
                                    strNewHSNCode = strNewHSNCode.Replace(".00", "");

                                    if (___strHSNCode.Length == 4 && __strHSNCode.Length == 4 && strNewHSNCode.Length != 4)
                                    {
                                        strItem += " " + strHSNCode.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                        strHSNCode = _strHSNCode[_strHSNCode.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strItem += " " + strHSNCode;
                                        strHSNCode = __str[2].Trim();
                                        _strHSNCode = strHSNCode.Split(' ');
                                        if (_strHSNCode.Length > 1)
                                        {
                                            strItem += " " + strHSNCode.Replace(_strHSNCode[_strHSNCode.Length - 1], "");
                                            strHSNCode = _strHSNCode[_strHSNCode.Length - 1];
                                        }
                                        if (strHSNCode.Length != 4)
                                        {
                                            strItem += " " + strHSNCode;
                                            strHSNCode = __str[3].Trim();
                                            index++;
                                        }
                                        index++;
                                    }
                                }
                                string strDescription = "";
                                int _qtyRateIndex = 0;
                                if (_qtyIndex > 0)
                                    strDescription = __str[index + _qtyIndex + 1].Trim();

                                string[] _strQty = __str[index + _qtyIndex + 2].Trim().Split(' ');
                                if (_strQty.Length > 0)
                                    strQty = _strQty[0];
                                if (_strQty.Length == 1)
                                    _qtyRateIndex = 1;
                                string[] _strRate = __str[index + _qtyIndex + _rateIndex + _qtyRateIndex + 3].Trim().Split(' ');
                                if (_strRate.Length > 0)
                                    strRate = _strRate[0];
                                if (strRate == "0.00" && __str.Length > 6)
                                    strRate = __str[5].Trim();

                                if (_lines.Length > _index + 5)
                                {
                                    string _strDescription = _lines[_index + 5].Trim();
                                    string[] _strDesc = _strDescription.Split(' ');
                                    if (_strDesc.Length == 1 && strItem != "" && !_strDesc[0].Contains(".") && !_strDesc[0].Contains(",") && _strDesc[0].Length > 2)
                                        strItem += " " + _strDesc[0];

                                }
                                if (_qtyIndex > 0 && strDescription.Length > 2 && strItem != "")
                                    strItem += " " + strDescription;

                            }
                            if (strItem != "")
                            {
                                strItem = strItem.Replace("'", "").Trim();
                                dgrdDetails.Rows.Add();
                                dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                                dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                                dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                                dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                                if (txtPurchaseParty.Text.Contains("SONKHIYA"))
                                {
                                    dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = "BABA SUIT : " + strItem;
                                    if (strHSNCode.Contains("6203"))
                                        strItem = "BABA SUIT : 6203";
                                    else if (strHSNCode.Contains("6103"))
                                        strItem = "BABA SUIT:6103";
                                    else
                                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                                }
                                else
                                    CheckItemNameExistence(ref strItem, ref strHSNCode);

                                dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                                _lineIndex++;
                                _index++;
                            }
                            else
                                _index++;
                        }
                        else
                            _index++;
                    }
                    else
                        _index++;
                }
                else
                    _index++;
            }
            return false;
        }

        private bool SetItemDetailsByCustomize_Branches(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if ((strText.ToUpper().Contains("ADD:  FREIGHT/CARTAGE")) && txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS"))
                {
                    string[] _data = strText.Trim().Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmount.Text = _data[0];

                    break;
                }
                else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    return true;
                else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS"))
                {
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtOtherAmount.Text = _data[_data.Length - 1];
                    }
                }
                else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmount.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        strText = _lines[_index + 2].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtPackingAmt.Text = _data[0];
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmount.Text = _data[0];
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                            txtPackingAmt.Text = _data[_data.Length - 1];
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    strItem = strQty = strRate = strHSNCode = "";
                    if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + "." || (str[0] == _lineIndex.ToString() && txtPurchaseParty.Text.Contains("KC GARMENTS")))
                    {
                        if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                        {
                            if ((str[0] == _lineIndex.ToString() || str[0] == _lineIndex + ".") && str.Length > 2)
                            {
                                strText = strText.Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                string[] _data = strText.Split(' ');
                                str = _data;
                                if (_data.Length > 0)
                                {
                                    strQty = _data[_data.Length - 1].Trim();
                                    strHSNCode = _data[_data.Length - 3].Trim();
                                }
                                string __strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if (__strHSNCode.Length != 4 || strHSNCode.Length != 4)
                                {
                                    strItem = strText.Replace(_lineIndex + ".", "").Trim();
                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length == 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();

                                        strText = _lines[_index + 3].Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 1)
                                            strRate = _data[0];
                                    }
                                    else
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strText = _lines[_index + 2].Trim();
                                        _data = strText.Split(' ');
                                        strQty = _data[0].Trim();

                                        strText = _lines[_index + 4].Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 1)
                                            strRate = _data[0];
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i < str.Length - 3; i++)
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += str[i];
                                    }
                                    strText = _lines[_index + 2].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 1)
                                        strRate = _data[0];

                                }
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("BONNYS NX"))
                    {
                        strText = strText.Replace("├┼┼┼", "").Trim().Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 5)
                        {
                            string sno = "";
                            strRate = _data[_data.Length - 2].Trim();
                            strQty = _data[_data.Length - 3].Trim();
                            strHSNCode = _data[_data.Length - 5].Trim();
                            if (strHSNCode.Length == 4)
                            {
                                sno = strHSNCode.Substring(0, 1);
                                strHSNCode = "6" + strHSNCode.Substring(1);
                            }
                            else if (strHSNCode.Length == 5)
                            {
                                sno = strHSNCode.Substring(0, 2);
                                strHSNCode = "6" + strHSNCode.Substring(2);
                            }

                            if (sno == _lineIndex.ToString())
                            {
                                for (int i = 0; i < _data.Length - 5; i++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += _data[i];
                                }
                            }
                        }
                    }

                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        strItem = strQty = strRate = "";
                    }
                }
            }
            return false;
        }

        private bool SetItemDetailsByCustomize_Delhi(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');
            if ((txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") || txtPurchaseParty.Text.Contains("MISHU ENTERPRISES")) && _lines.Length < 60)
                _itemIndex = 17;

            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "", strAmount = "", strHSNCode = "", strQty = "", strRate = "";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();
                if ((strText.ToUpper().Contains("ADD : PACKING CHARGE")) || ((strText.ToUpper().Contains("GRAND TOTAL") && txtPurchaseParty.Text.Contains("FULLTOSS"))))
                {
                    strText = _lines[_index + 5];
                    txtPackingAmt.Text = strText;
                    break;
                }
                else if ((strText.ToUpper().Contains("ADD:  FREIGHT/CARTAGE")) && txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS"))
                {
                    string[] _data = strText.Trim().Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmount.Text = _data[0];

                    break;
                }
                else if ((strText.ToUpper().Contains("OTHER CHARGES :")) && txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION"))
                {
                    strText = _lines[_index - 1].Trim();
                    double _dFreight = dba.ConvertObjectToDouble(strText);
                    if (_dFreight > 0)
                        txtOtherAmount.Text = _dFreight.ToString("0.00");

                    break;
                }
                else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("JANAK GARMENTEX") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") && !txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") && !txtPurchaseParty.Text.Contains("MOTI FASHIONS") && !txtPurchaseParty.Text.Contains("TANEJA FASHION") && !txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && !txtPurchaseParty.Text.Contains("VIPIN COLLECTION") && !txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && !txtPurchaseParty.Text.Contains("HARDIK TEXTILE") && !txtPurchaseParty.Text.Contains("SONY CREATION") && !txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && !txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && !txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION") && !txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && !txtPurchaseParty.Text.Contains("ARPIT FASHION"))
                    return true;
                else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                {
                    if (txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                    {
                        string[] _data = strText.Trim().Split(' ');
                        if (_data.Length > 2)
                            txtOtherAmount.Text = _data[0];
                        if (txtOtherAmount.Text == "(+)")
                            txtOtherAmount.Text = "";
                    }
                    else if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                    {
                        string[] _data = _lines[_index + 1].Trim().Split(' ');
                        if (_data.Length > 0)
                        {
                            txtOtherAmount.Text = _data[0].Trim();
                            if (txtOtherAmount.Text == "")
                            {
                                _data = _lines[_index].Trim().Split(' ');
                                if (_data.Length > 0)
                                    txtOtherAmount.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1].Trim()).ToString("0.00");
                            }
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtOtherAmount.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                    }
                }
                else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtOtherAmount.Text = _data[_data.Length - 1];
                }
                else if (strText.ToUpper().Contains("PC. DISCOUNT") && txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                {
                    //string[] _data = strText.Split(' ');
                    //if (_data.Length > 1)
                    // txtPcsAmt.Text = _data[0];
                }
                else if (strText.ToUpper().Contains("DISCOUNT PER. PC.") && txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                {
                    //string _strText = strText.Replace("Amount", "").Replace("Chargeable", "").Replace("(", "").Replace(")", "").Replace("In Words", "").Replace(":", "").Trim();
                    //string[] _data = _strText.Split(' ');
                    //if (_data.Length > 1)
                    //  txtPcsAmt.Text = _data[0];
                }
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    if (txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                    {
                        string[] _data = strText.Trim().Split(' ');
                        if (_data.Length > 2)
                            txtPackingAmt.Text = _data[0];
                        if (txtPackingAmt.Text == "(+)")
                            txtPackingAmt.Text = "";
                    }
                    else if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        strText = _lines[_index + 2].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtPackingAmt.Text = _data[0];
                            strText = _lines[_index + 1].Trim();
                            _data = strText.Split(' ');
                            txtOtherAmount.Text = _data[0];
                        }
                    }
                    else
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                            txtPackingAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE TIME :"))
                {
                    if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        //if (txtOtherAmount.Text != "0.00")
                        //{
                        strText = _lines[_index + 1].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                        {
                            txtOtherAmount.Text = (dba.ConvertObjectToDouble(_data[0]) + dba.ConvertObjectToDouble(txtOtherAmount.Text)).ToString("0.00");
                        }
                        //  }
                        return true;
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    string[] str = strText.Split(' ');
                    strItem = strQty = strRate = "";
                    if ((txtPurchaseParty.Text.Contains("JANAK GARMENTEX")))
                    {
                        if (str.Length > 10)
                        {
                            if (str[10] == _lineIndex.ToString())
                            {
                                strQty = str[9].Trim();
                                strRate = str[1].Trim();
                                strHSNCode = str[0].Trim();
                                for (int __index = 11; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            else
                            {
                                string[] _data = _lines[_index + 1].Trim().Split(' ');
                                if (_data.Length > 10)
                                {
                                    if (_data[10] == _lineIndex.ToString())
                                    {
                                        strQty = str[9].Trim();
                                        strRate = str[1].Trim();
                                        strHSNCode = str[0].Trim();
                                        for (int __index = 11; __index < str.Length; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                        }
                        else if (str.Length > 2)
                        {
                            if (str[0] == _lineIndex.ToString())
                            {
                                for (int __index = 1; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }

                                str = _lines[_index - 1].Trim().Split(' ');
                                if (str.Length > 2)
                                {
                                    strQty = str[9].Trim();
                                    strRate = str[1].Trim();
                                    strHSNCode = str[0].Trim();
                                }

                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            strRate = str[13].Trim();
                            strQty = str[14].Trim();
                            strHSNCode = str[15].Trim();
                            for (int __index = 16; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("VIPIN COLLECTION")) && str.Length > 10)
                    {
                        if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strRate = str[8].Trim();
                            strQty = str[9].Trim();
                            strHSNCode = str[10].Trim();
                            for (int __index = 11; __index < str.Length - 1; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("SHUBHI GARMENTS")) && str.Length > 10)
                    {
                        string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');
                        if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strRate = str[7].Trim();
                            strQty = str[9].Trim();
                            strHSNCode = str[10].Trim();
                            for (int __index = 11; __index < str.Length - 1; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("NIKUNJ TRADING")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 6; __index < str.Length - 4; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) && str.Length > 10)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 6; __index < str.Length - 4; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("SONY CREATION") || txtPurchaseParty.Text.Contains("HARDIK TEXTILE")) && str.Length > 7)
                    {
                        if (str[str.Length - 2] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[0].Trim();
                            strRate = str[1].Trim();
                            strHSNCode = str[3].Trim();
                            for (int __index = 9; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 17; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 11; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem == "")
                                strItem = str[str.Length - 1].Trim();
                        }
                    }
                    else if ((txtPurchaseParty.Text.Contains("WORLD SAHAB")) && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strQty = str[1].Trim();
                            strRate = str[2].Trim();
                            strHSNCode = str[4].Trim();
                            for (int __index = 13; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem == "")
                                strItem = str[str.Length - 1].Trim();
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("DONARGOLD GARMENTS") || txtPurchaseParty.Text.Contains("W STAN GARMENTS") || txtPurchaseParty.Text.Contains("GEX GARMENTS"))
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            string strDescription = "";

                            strQty = str[str.Length - 1].Trim();
                            if (str.Length > 2)
                                strDescription = str[1];
                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0].Trim();
                            str = _lines[_index + 2].Trim().Split(' ');
                            strHSNCode = str[0].Trim();
                            for (int __index = 1; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                            _index += 2;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MOTI FASHIONS") && str.Length > 6)
                    {
                        if (_lines[_index - 1].Trim() == _lineIndex.ToString())
                        {
                            strHSNCode = str[str.Length - 2].Trim();
                            strQty = str[str.Length - 4].Trim();
                            strRate = str[str.Length - 5].Trim();

                            for (int __index = 0; __index < str.Length - 6; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            _index += 1;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && str.Length > 6)
                    {
                        if (_lines[_index + 1].Trim() == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 1].Trim();
                            strRate = str[str.Length - 9].Trim();
                            strHSNCode = str[str.Length - 10].Trim();

                            for (int __index = 0; __index < str.Length - 10; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                        else if (str[str.Length - 1] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 2].Trim();
                            strRate = str[str.Length - 10].Trim();
                            strHSNCode = str[str.Length - 11].Trim();

                            for (int __index = 0; __index < str.Length - 11; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && str.Length > 6)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            int itemIndex = 2;
                            strRate = str[str.Length - 2].Trim();
                            strHSNCode = str[2].Trim();
                            string _strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                            if (_strHSNCode.Length == 4 && strHSNCode.Length == 4)
                            {
                                strQty = str[3].Trim();
                                strRate = str[4].Trim();
                            }
                            else
                            {
                                itemIndex = 3;
                                strHSNCode = str[3].Trim();
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                if (_strHSNCode.Length == 4 && strHSNCode.Length == 4)
                                {
                                    strQty = str[4].Trim();
                                    strRate = str[5].Trim();
                                }
                                else
                                {
                                    itemIndex = 4;
                                    strHSNCode = str[4].Trim();
                                    strQty = str[5].Trim();
                                    strRate = str[6].Trim();
                                }
                            }

                            for (int __index = 1; __index < itemIndex; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("TANEJA FASHION") && str.Length > 6)
                    {
                        strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        int ___index = strText.IndexOf(" " + _lineIndex + " ");
                        if (___index > 0)
                        {
                            str = strText.Trim().Split(' ');

                            string strDescription = "";

                            strQty = str[5].Trim();
                            strHSNCode = str[6].Trim();
                            strRate = str[str.Length - 1].Trim();

                            for (int __index = 7; __index < str.Length - 5; __index++)
                            {
                                if (str[__index] == _lineIndex.ToString())
                                    break;
                                else
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            if (strItem != "" && strDescription != "")
                                strItem += " " + strDescription;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("JOLLY FASHIONS") && str.Length > 1)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strHSNCode = _lines[_index + 1].Trim();
                            if (strHSNCode.Length == 4 || strHSNCode == "62" || strHSNCode == "63")
                            {
                                string[] _strText = _lines[_index + 2].Trim().Split(' ');
                                if (_strText.Length > 1)
                                {
                                    strQty = _strText[0];
                                    strRate = _strText[1];
                                }
                                for (int __index = 1; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                                if (strItem != "")
                                    _index += 4;
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("WORLD CHOICE") && str.Length > 1)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            for (int __index = 1; __index < str.Length; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            int _qtyIndex = 0;

                            string __strText = _lines[_index + 1].Trim();
                            str = __strText.Split(' ');
                            if (str.Length == 1)
                            {
                                strQty = str[0];
                                _qtyIndex++;

                                __strText = _lines[_index + _qtyIndex + 1].Trim();
                                str = __strText.Split(' ');
                                strRate = str[0];
                            }
                            else if (str.Length == 2)
                            {
                                strQty = str[0];
                                strRate = str[1];
                            }

                            __strText = _lines[_index + _qtyIndex + 4].Trim();
                            str = __strText.Split(' ');
                            if (str.Length > 2)
                                strHSNCode = str[str.Length - 2];
                            if (strItem != "")
                                _index += 5;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("CHANCELLOR INDUSTRIES") || txtPurchaseParty.Text.Contains("MAA PADMAVATI APPARELS"))
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 8];
                            strQty = str[str.Length - 10];
                            strHSNCode = str[str.Length - 11];

                            for (int __index = 1; __index < str.Length - 11; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && str.Length > 10)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 12];
                            strQty = str[str.Length - 13];
                            strHSNCode = str[str.Length - 14];

                            for (int __index = 1; __index < str.Length - 14; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && str.Length > 10)
                    {
                        if (str[str.Length - 8] == _lineIndex.ToString())
                        {
                            string strDesc = "";
                            if (strText.Contains("STD"))
                            {
                                strText = strText.Replace(" STD ", " ").Replace("  ", " ").Trim();
                                strDesc = " STD";
                            }
                            str = strText.Split(' ');

                            strRate = str[str.Length - 9];
                            strQty = str[str.Length - 10];
                            strHSNCode = str[str.Length - 2];

                            for (int __index = 1; __index < str.Length - 10; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }

                            if (strItem != "" && strDesc != "")
                                strItem += strDesc;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("AASHI COLLECTION") && str.Length > 4)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 1];
                            strHSNCode = str[str.Length - 2];
                            for (int __index = 1; __index < str.Length - 2; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0];
                            _index += 2;
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("S.R CREATION") && str.Length > 4)
                    {
                        if (str[str.Length - 5] == _lineIndex.ToString())
                        {
                            strQty = str[str.Length - 2].Trim();
                            strHSNCode = str[str.Length - 3].Trim();
                            strItem = str[str.Length - 4].Trim();

                            str = _lines[_index + 1].Trim().Split(' ');
                            strRate = str[0].Trim();
                        }
                        else if (str.Length > 5)
                        {
                            if (str[str.Length - 6] == _lineIndex.ToString())
                            {
                                strQty = str[str.Length - 3].Trim();
                                strHSNCode = str[str.Length - 4].Trim();
                                strItem = (str[str.Length - 6] + " " + str[str.Length - 5]).Trim();

                                str = _lines[_index + 1].Trim().Split(' ');
                                strRate = str[0].Trim();
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") && str.Length > 6)
                    {
                        if (str[5] == _lineIndex.ToString())
                        {
                            strQty = str[0];
                            strRate = str[10];
                            strHSNCode = str[11];

                            for (int __index = 12; __index <= str.Length - 1; __index++)
                            {
                                if (__index != 14)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            //if (str[12] != "")
                            //    strItem += " " + str[12];
                            if (str[8] != "")
                                strItem += " " + str[8];
                            if (str[9] != "")
                                strItem += " " + str[9];
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && str.Length > 6)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 2];
                            strQty = str[str.Length - 4];
                            strHSNCode = str[str.Length - 5];

                            for (int __index = 1; __index < str.Length - 5; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (txtPurchaseParty.Text.Contains("MIKEY FASHION") && str.Length > 4)
                    {
                        if (str[0] == _lineIndex.ToString())
                        {
                            strRate = str[str.Length - 1];
                            strQty = str[str.Length - 2];
                            strHSNCode = str[str.Length - 3];

                            for (int __index = 1; __index < str.Length - 3; __index++)
                            {
                                if (strItem != "")
                                    strItem += " ";
                                strItem += str[__index];
                            }
                        }
                    }
                    else if (str[0] == _lineIndex + "." || _lines[_index - 1].Trim() + str[0] == _lineIndex + "." || (str[0] == _lineIndex.ToString() && txtPurchaseParty.Text.Contains("KC GARMENTS")))
                    {

                        if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                        {
                            if (str[0] == _lineIndex.ToString() && str.Length > 2)
                            {
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 0)
                                    strQty = _data[_data.Length - 1].Trim();

                                strItem = strText.Replace(_lineIndex + " ", "").Replace(strQty, "");
                                _data = _lines[_index + 1].Trim().Split(' ');
                                if (_data.Length > 2)
                                    strRate = _data[_data.Length - 1].Trim();
                                else
                                {
                                    _data = _lines[_index + 2].Trim().Split(' ');
                                    strRate = _data[0].Trim();
                                }
                                if ((_index + 3) < _lines.Length)
                                {
                                    _data = _lines[_index - 1].Split(' ');
                                    strHSNCode = _data[_data.Length - 1].Trim();
                                }
                            }
                            else
                            {
                                strText = _lines[_index - 1].Trim();
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 0)
                                    strHSNCode = _data[_data.Length - 1].Trim();
                                strItem = _lines[_index + 1].Trim();
                                _data = _lines[_index + 2].Split(' ');
                                if (_data.Length > 3)
                                {
                                    strQty = _data[0].Trim();
                                    strRate = _data[3].Trim();
                                }
                                else
                                {
                                    strQty = _data[0];
                                    _data = _lines[_index + 3].Split(' ');
                                    if (_data.Length > 0)
                                        strRate = _data[0];
                                }
                            }
                            _index += 3;
                        }
                        else if (txtPurchaseParty.Text.Contains("LUCKY JACKET") || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                        {
                            if (str.Length < 2)
                            {
                                strItem = _lines[_index + 1].Trim();
                                strText = _lines[_index + 2].Trim();
                                string[] _data = strText.Split(' ');
                                if (_data.Length > 3)
                                {
                                    strHSNCode = _data[0].Trim();
                                    strQty = _data[_data.Length - 1].Trim();
                                }
                                strText = _lines[_index + 5].Trim();
                                _data = strText.Split(' ');
                                if (_data.Length > 3)
                                    strRate = _data[_data.Length - 1].Trim();
                                else
                                {
                                    strText = _lines[_index + 4].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 2)
                                        strRate = _data[_data.Length - 1].Trim();
                                }

                                _index += 4;
                            }
                            else
                            {
                                strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                string[] __str = strText.Split('|');

                                if (__str.Length > 2)
                                {
                                    strItem = strQty = strRate = "";
                                    int index = 0;
                                    double dRate = 0;
                                    strItem = __str[0].Replace(_lineIndex + ".", "").Trim();
                                    strHSNCode = __str[1].Trim();
                                    if (strHSNCode.Length != 4)
                                    {
                                        strItem += " " + strHSNCode;
                                        strHSNCode = __str[2].Trim();
                                        index++;
                                    }
                                    string[] _strQty = __str[index + 2].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strQty = _strQty[0];
                                    if (txtPurchaseParty.Text.Contains("LUCKY JACKET"))
                                    {
                                        if (__str.Length > 3)
                                        {
                                            _strQty = __str[3].Trim().Split(' ');
                                            strRate = _strQty[0].Trim();
                                            dRate = dba.ConvertObjectToDouble(strRate);
                                        }
                                        strText = _lines[_index + 1].Trim();
                                    }
                                    else
                                        strText = _lines[_index + 2].Trim();

                                    strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    string[] _data = strText.Split('|');
                                    if (_data.Length == 1 && dRate == 0)
                                    {
                                        string[] __strRate = _data[0].Trim().Split(' ');
                                        if (__strRate.Length > 0)
                                            strRate = __strRate[0].Trim();
                                    }
                                    else if (_data.Length > 1 && dRate == 0)
                                    {
                                        string[] __strRate = _data[1].Trim().Split(' ');
                                        if (__strRate.Length > 0)
                                            strRate = __strRate[0].Trim();
                                        if (strRate == "%" && _data.Length > 2)
                                        {
                                            __strRate = _data[2].Trim().Split(' ');
                                            if (__strRate.Length > 0)
                                                strRate = __strRate[0].Trim();
                                        }

                                    }
                                    if (strRate == "0.00")
                                    {
                                        strText = _lines[_index + 3].Trim();
                                        if (strText == "%")
                                            strText = _lines[_index + 4].Trim();
                                        _data = strText.Trim().Split(' ');
                                        strRate = _data[0];
                                        if (strRate == "%")
                                            strRate = _data[_data.Length - 1].Trim();

                                        _index += 4;
                                    }
                                }
                                else if (txtPurchaseParty.Text.Contains("LUCKY JACKET"))
                                {
                                    strText = strText.Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    string[] _data = strText.Split('|');

                                    strItem = _data[0].Replace(_lineIndex + ".", "").Trim();
                                    strHSNCode = _data[1].Trim();
                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                        strQty = _data[_data.Length - 1].Trim();
                                    strText = _lines[_index + 2].Trim();
                                    if (Regex.Matches(strText, "%").Count > 1)
                                    {
                                        _data = strText.Replace("  ", " ").Trim().Split(' ');
                                        if (_data.Length > 4)
                                            strRate = _data[_data.Length - 3].Trim();
                                        else
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strText = _lines[_index + 2].Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    _index += 3;
                                }
                                else if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                                {
                                    string[] _data = null;
                                    if (__str.Length == 2)
                                    {
                                        string _strData = __str[0];
                                        _data = _strData.Split(' ');
                                        if (_data.Length > 1)
                                            strHSNCode = _data[_data.Length - 1];
                                        strItem = __str[0].Replace(_lineIndex + ".", "").Replace(strHSNCode, "").Trim();
                                        strQty = __str[__str.Length - 1].Trim();
                                    }
                                    else
                                        strItem = strText.Replace(_lineIndex + ".", "").Trim();

                                    strText = _lines[_index + 1].Trim();
                                    _data = strText.Split(' ');
                                    if (_data.Length > 3)
                                    {
                                        strHSNCode = _data[0].Trim();
                                        strQty = _data[_data.Length - 1].Trim();
                                    }
                                    if (__str.Length == 2)
                                        strText = _lines[_index + 2].Trim();
                                    else
                                        strText = _lines[_index + 3].Trim();

                                    if (Regex.Matches(strText, "%").Count > 1)
                                    {
                                        _data = strText.Replace("  ", " ").Trim().Split(' ');
                                        if (_data.Length > 4)
                                            strRate = _data[_data.Length - 3].Trim();
                                        else
                                            strRate = _data[_data.Length - 1].Trim();
                                    }
                                    else
                                    {
                                        strText = _lines[_index + 4].Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                                        _data = strText.Split(' ');
                                        if (_data.Length > 0)
                                            strRate = _data[0].Trim();
                                        if ((strRate == "0.00" || strRate == "0" || strRate == "%") && _data.Length > 2)
                                            strRate = _data[_data.Length - 1].Trim();
                                        if (dba.ConvertObjectToDouble(strRate) < 7)
                                        {
                                            strText = _lines[_index + 3].Trim();
                                            _data = strText.Replace("  ", " ").Trim().Split(' ');
                                            strRate = _data[_data.Length - 1].Trim();
                                        }
                                    }
                                    _index += 4;
                                }
                            }
                        }
                    }


                    if (strItem != "")
                    {
                        strItem = strItem.Replace("'", "").Trim();
                        dgrdDetails.Rows.Add();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        CheckItemNameExistence(ref strItem, ref strHSNCode);
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        _lineIndex++;
                        strItem = strQty = strRate = "";
                    }
                }
            }
            return false;
        }

        private int GetIndexOfValue(string[] str,string strValue,bool _reverse)
        {
            if(_reverse)
            {
                for(int index=str.Length-1; index>=0;index--)
                {
                    if (str[index].Contains(strValue))
                        return index;
                }
            }
            else
            {
                for (int index = 0; index <= str.Length - 1; index++)
                {
                    if (str[index].Contains(strValue))
                        return index;
                }
            }
            return 0;
        }

        private bool SetItemDetailsByCustomize_Saraogi(int _itemIndex, string strData, ref bool _bLongLable)
        {
            string[] _lines = strData.Split('\n');

            int _index = _itemIndex + 1, _lineIndex = 1;
            string strText = "", strItem = "",strSize="",strColor="", strAmount = "", strHSNCode = "", strQty = "", strRate = "",strMRP="";
            _lineIndex = dgrdDetails.Rows.Count + 1;
            for (; _index < _lines.Length - 1; _index++)
            {
                strText = _lines[_index].Trim();

                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("JANAK GARMENTEX") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") && !txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") && !txtPurchaseParty.Text.Contains("MOTI FASHIONS") && !txtPurchaseParty.Text.Contains("TANEJA FASHION") && !txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && !txtPurchaseParty.Text.Contains("VIPIN COLLECTION") && !txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && !txtPurchaseParty.Text.Contains("HARDIK TEXTILE") && !txtPurchaseParty.Text.Contains("SONY CREATION") && !txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && !txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && !txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION") && !txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && !txtPurchaseParty.Text.Contains("ARPIT FASHION"))
                    return true;
                else if (strText.ToUpper().Contains("PACKING"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1 && _data[_data.Length - 1] != "-")
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                }
                else if (strText.ToUpper().Contains("POSTAGE AMT") || strText.ToUpper().Contains("OTHER CHARGES") || strText.ToUpper().Contains("INSURANCE CHARGE") || strText.ToUpper().Contains("INSURANCE CHARGE") || strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("OTHER AMT") || strText.ToUpper().Contains("TRANSPORT CHARGE") || strText.ToUpper().Contains("DISPATCHED CHARGE"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                    {
                        double dOther = dba.ConvertObjectToDouble(txtOtherAmount.Text) + dba.ConvertObjectToDouble(_data[_data.Length - 1]);
                        txtOtherAmount.Text = dOther.ToString("0.00");
                    }
                }
                else if (!strText.ToUpper().Contains("LESS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST"))
                {
                    strItem = strSize = strColor = strQty = strRate = "";
                    string[] str = strText.Split(' ');

                    if (str[str.Length - 1] == _lineIndex + "." && _strSType=="")
                    {
                        //Retails

                        string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');

                        strHSNCode = str[0];
                        if (strHSNCode.Length == 8)
                            strHSNCode = strHSNCode.Substring(0, 4);

                        int rIndex = 3;
                        if (Regex.Matches(strText, "%").Count > 2)
                            rIndex = 4;
                        strRate = str[rIndex].Trim();

                        for (int __index = rIndex + 1; __index <= str.Length - 4; __index++)
                        {
                            if (strItem != "")
                                strItem += " ";
                            strItem += str[__index];
                        }
                        strSize = str[str.Length - 3];
                        strColor = str[str.Length - 2];

                        strText = _lines[_index - 1].Trim();
                        _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                        str = _str.Split(' ');
                        if (str.Length > 3)
                        {
                            strMRP = str[2];
                            strQty = str[0];
                        }
                    }
                    else if (Regex.Matches(strText, "%").Count > 1)
                    {
                        if ((txtPurchaseParty.Text.Contains("SARAOGI")) && str.Length > 4)
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');

                            strHSNCode = str[0].Trim();

                            int perIndex = GetIndexOfValue(str, "%", true);
                            strRate = str[perIndex + 1].Trim();
                            if (strText.Contains("P.No:"))
                            {
                                int pIndex = GetIndexOfValue(str, "P.No", false);
                                for (int __index = perIndex + 2; __index < pIndex; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                            else if (_lines[_index + 1].Trim().Contains("P.No:"))
                            {
                                int pIndex = str.Length;
                                for (int __index = perIndex + 2; __index < pIndex; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }

                            strText = _lines[_index - 1].Trim();
                            _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');

                            strQty = str[str.Length - 4].Trim();
                            strMRP = str[str.Length - 2].Trim();
                            if (strItem == "")
                            {
                                for (int __index = 2; __index < str.Length - 4; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                        }
                    }
                    else if (str[str.Length - 1] == _lineIndex + ".")
                    {
                        {
                            string _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                            str = _str.Split(' ');
                            strRate = str[str.Length - 2].Trim();

                            strText = _lines[_index - 1].Trim();
                            if (!strText.Contains("%"))
                            {
                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                strHSNCode = str[0];

                                strText = _lines[_index - 3].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                if (str.Length > 4)
                                {
                                    strQty = str[str.Length - 4].Trim();
                                    strMRP = str[str.Length - 2].Trim();
                                    if (strItem == "")
                                    {
                                        for (int __index = 2; __index < str.Length - 4; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                strHSNCode = str[0];

                                strText = _lines[_index - 2].Trim();
                                _str = strText.Replace("   ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Replace("-", "").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
                                str = _str.Split(' ');
                                if (str.Length > 4)
                                {
                                    strQty = str[str.Length - 4].Trim();
                                    strMRP = str[str.Length - 2].Trim();
                                    if (strItem == "")
                                    {
                                        for (int __index = 2; __index < str.Length - 4; __index++)
                                        {
                                            if (strItem != "")
                                                strItem += " ";
                                            strItem += str[__index];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (strItem != "")
                {
                    strItem = strItem.Replace("'", "").Trim();
                    dgrdDetails.Rows.Add();
                    dgrdDetails.Rows[_lineIndex - 1].Cells["srNo"].Value = _lineIndex + ".";
                    dgrdDetails.Rows[_lineIndex - 1].Cells["qty"].Value = strQty;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["mrp"].Value = strMRP;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["variant1"].Value = strSize;
                    dgrdDetails.Rows[_lineIndex - 1].Cells["variant2"].Value = strColor;
                    CheckItemNameExistence(ref strItem, ref strHSNCode);
                    if (strItem != "")
                    {
                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        GetPurchaseRate_Import(dgrdDetails.Rows[_lineIndex - 1]);
                        SetUnitName(strItem, _lineIndex - 1);
                    }
                    else
                    {
                        dgrdDetails.Rows[_lineIndex - 1].DefaultCellStyle.BackColor = Color.Tomato;
                        CalculateDisWithAmountMRP_Current(dgrdDetails.Rows[_lineIndex - 1]);
                    }
                    _lineIndex++;
                    strItem =strSize=strColor= strQty = strRate = "";
                }
            }
            return false;
        }

        private void SetBasicDetails(ref int _itemIndex, string strData)
        {
            string[] _lines = strData.Split('\n');
            int _index = 0;
            if ((_lines[0].Contains("GSTIN  :") || _lines[0].ToUpper().Contains("ORIGINAL COPY")) && (_lines[0].Contains("               ") || _lines[1].Contains("               ")) && (_lines[0].ToUpper().Contains("TAX INVOICE") || _lines[1].ToUpper().Contains("TAX INVOICE") || _lines[2].ToUpper().Contains("TAX INVOICE")))
                _strBillType = "BUSY";
            if ((_lines[0].Contains("GSTIN") || _lines[1].ToUpper().Contains("ORIGINAL COPY")) && (_lines[1].Contains("               ") || _lines[2].Contains("               ")) && _lines[2].ToUpper().Contains("TAX INVOICE"))
                _strBillType = "BUSY";
            if (_lines[3].Contains("N.A.R. TRADING"))
                _strBillType = "BUSY";

            bool _bBonny = false;
            foreach (string strText in _lines)
            {
                if ((strText.Contains("GSTIN  :") || strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN. :") || strText.Contains("GSTIN: ") || strText.Contains("GSTIN :")) && !strText.Contains("AAYCS8982Q"))
                {
                    string _strText = strText.Replace("  ", "");
                    string[] strGST = _strText.Trim().Split(' ');
                    if (strGST.Length > 1)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        if (strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN  :"))
                            strGSTNO = strGST[1];
                        else if ((strText.Contains("GSTIN. :") || strText.Contains("GSTIN :")) && strGSTNO.Length != 15 && strGST.Length > 2)
                            strGSTNO = strGST[2];
                        if (strGSTNO.Length == 18)
                            strGSTNO = strGSTNO.Substring(0, 15);

                        if (strGSTNO.Length == 15)
                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                        else
                        {
                            strGST = strText.Replace("  ", " ").Replace("  ", " ").Trim().Split(' ');
                            if (strGST.Length > 0)
                            {
                                strGSTNO = strGST[1];
                                if (strGSTNO == ":" && strGST.Length > 2)
                                    strGSTNO = strGST[2];

                                if (strGSTNO.Length == 15)
                                    SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                            }
                        }
                        if (_index > 1)
                        {
                            if (_lines[_index - 1].ToUpper().Contains("TAX INVOICE") && _lines[0].ToUpper().Contains("INVOICE NO."))
                            {
                                txtInvoiceNo.Text = _lines[_index - 3].ToUpper().Trim();
                                txtInvoiceDate.Text = _lines[_index - 2].ToUpper().Trim();
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN :"))
                {
                    if (_index > 1)
                    {
                        if (_lines[_index - 1].ToUpper().Contains("TAX INVOICE") && _lines[0].ToUpper().Contains("INVOICE NO."))
                        {
                            string _strText = strText.Replace("  ", "");
                            string[] strGST = _strText.Trim().Split(' ');
                            if (strGST.Length > 1)
                            {
                                string strGSTNO = strGST[strGST.Length - 1];
                                if (strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN  :"))
                                    strGSTNO = strGST[1];
                                else if ((strText.Contains("GSTIN. :") || strText.Contains("GSTIN :")) && strGSTNO.Length != 15 && strGST.Length > 2)
                                    strGSTNO = strGST[2];
                                if (strGSTNO.Length == 18)
                                    strGSTNO = strGSTNO.Substring(0, 15);

                                if (strGSTNO.Length == 15)
                                    SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                                else
                                {
                                    strGST = strText.Replace("  ", " ").Replace("  ", " ").Trim().Split(' ');
                                    if (strGST.Length > 0)
                                    {
                                        strGSTNO = strGST[1];
                                        if (strGSTNO == ":" && strGST.Length > 2)
                                            strGSTNO = strGST[2];

                                        if (strGSTNO.Length == 15)
                                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                                    }
                                }
                                txtInvoiceNo.Text = _lines[_index - 3].ToUpper().Trim();
                                txtInvoiceDate.Text = _lines[_index - 2].ToUpper().Trim();
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GST NO. :"))
                {
                    if (_index > 1)
                    {
                        string _strText = _lines[_index - 1];
                        string[] strGST = _strText.Trim().Split(' ');
                        if (strGST.Length > 0)
                        {
                            string strGSTNO = strGST[0];
                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                        }
                        if (txtGSTNo.Text == "")
                        {
                            _strText = _lines[_index];
                            strGST = _strText.Trim().Split(' ');
                            SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                        }
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN.") && txtPurchaseParty.Text == "" && !strText.Contains("AAYCS8982Q"))
                {
                    string[] strGST = strText.Trim().Split('.');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN") && _strBillType == "BUSY" && txtPurchaseParty.Text == "")
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[0];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GSTIN") && !strText.Contains("AAYCS8982Q") && txtPurchaseParty.Text == "")
                {
                    string[] strGST = strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        string _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("GST NO.") && !strText.Contains("AAYCS8982Q") && txtPurchaseParty.Text == "")
                {
                    string[] strGST = strText.Trim().Split('-');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[strGST.Length - 1];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        string _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.ToUpper().Contains("VEHICLE NO.") && txtPurchaseParty.Text == "")
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Replace(": ", "").Trim().Split(' ');
                    if (strGST.Length > 0)
                    {
                        string strGSTNO = strGST[0];
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                }
                else if (strText.ToUpper().Contains("CUSTOMER NO.") && txtInvoiceDate.Text.Length != 10)
                {
                    string _strText = _lines[_index + 3];
                    string[] strDate = _strText.Trim().Split(' ');
                    if (strDate.Length > 0)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate[0]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO/ DATE"))
                {
                    if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        txtInvoiceNo.Text = _lines[_index + 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, _lines[_index + 2]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.    :"))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtInvoiceNo.Text = strInvoiceNo[4].Trim();
                            string strInv = Regex.Replace(txtInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtInvoiceNo.Text += strInvoiceNo[5].Trim();
                        }
                        if (txtInvoiceNo.Text == "")
                        {
                            _strText = strText.Replace("                     ", " ");
                            strInvoiceNo = _strText.Trim().Split(' ');
                            if (strInvoiceNo.Length > 1)
                                txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.     :"))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtInvoiceNo.Text = strInvoiceNo[0].Trim();
                            string strInv = Regex.Replace(txtInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtInvoiceNo.Text += strInvoiceNo[5].Trim();
                            if (_strText.ToUpper().Contains("DATED"))
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, strInvoiceNo[strInvoiceNo.Length - 2]);
                                txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO. :"))
                {
                    string _strText = strText.Replace("  ", " ");
                    string[] strInvoiceNo = _strText.Trim().Split(' ');

                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                    if (txtInvoiceNo.Text.Trim() == ":")
                        txtInvoiceNo.Text = "";
                    if (txtInvoiceNo.Text == "" && (txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION")))
                    {
                        strInvoiceNo = _lines[_index + 1].Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                            txtInvoiceNo.Text = strInvoiceNo[0].Trim();
                    }
                    else if (txtInvoiceNo.Text.Contains("CREDIT") && strInvoiceNo.Length > 3)
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 2].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE NO."))
                {
                    if (txtInvoiceNo.Text == "")
                    {
                        if (_lines[_index + 1].ToUpper().Contains(": CUSTOMER NAME"))
                        {
                            txtInvoiceNo.Text = _lines[_index - 2];
                            if (!txtInvoiceNo.Text.Contains("KC"))
                                txtInvoiceNo.Text = _lines[_index - 3];
                        }
                        else
                        {
                            txtInvoiceNo.Text = _lines[_index + 1].Replace("Credit", "").Trim();
                            if (txtInvoiceNo.Text == ":")
                                txtInvoiceNo.Text = "";
                            if ((txtInvoiceNo.Text.Contains("2020") || txtInvoiceNo.Text.Contains("2021")) && txtInvoiceNo.Text.Trim().Length < 11)
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, txtInvoiceNo.Text.Trim());
                                txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                                txtInvoiceNo.Text = "";
                            }
                            else
                            {
                                string _strText = strText.Replace("  ", " ");
                                string[] strGST = _strText.Split(' ');
                                if ((txtPurchaseParty.Text.Contains("LADLEE WESTERN") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) && strGST.Length > 3)
                                    txtInvoiceNo.Text = strGST[3];
                                else if (txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS"))
                                {
                                    strGST = _lines[_index + 1].Trim().Split(' ');
                                    txtInvoiceNo.Text = strGST[1];
                                }
                                else if (txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("WORLD SAHAB")) // || txtPurchaseParty.Text.Contains("N.D. FASHION") //|| txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES")
                                {
                                    txtInvoiceNo.Text = strGST[0];
                                }
                                if (_strText.ToUpper().Contains("DATED"))
                                {
                                    txtInvoiceNo.Text = "";
                                    if (strGST.Length > 1 && txtInvoiceNo.Text == "")
                                        txtInvoiceNo.Text = strGST[2];

                                    DateTime _iDate = DateTime.Now;
                                    ConvertDateTime(ref _iDate, strGST[strGST.Length - 1].Trim());
                                    txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    if (strGST.Length > 1 && txtInvoiceNo.Text == "")
                                        txtInvoiceNo.Text = strGST[strGST.Length - 1];
                                    if (txtInvoiceNo.Text.Contains("BILL"))
                                        txtInvoiceNo.Text = _lines[_index + 2];
                                    if (txtInvoiceNo.Text.Contains("LR DATE"))
                                        txtInvoiceNo.Text = "";
                                }
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO  :") || strText.ToUpper().Contains("INVOICE NO :"))
                {
                    string[] strGST = strText.Split(' ');
                    if (strGST.Length > 1 && txtInvoiceDate.Text.Length != 10)
                    {
                        txtInvoiceNo.Text = strGST[strGST.Length - 1];
                        DateTime _iDate = DateTime.Now;
                        strGST = _lines[_index + 1].Split(' ');
                        string strDate = strGST[0];
                        if (strDate.Length < 9)
                            strDate = strGST[strGST.Length - 1];
                        ConvertDateTime(ref _iDate, strDate);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        if (txtInvoiceNo.Text == ":")
                            txtInvoiceNo.Text = "";
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO:             DT."))
                {
                    string _strText = strText.Replace("  ", " ").Replace("  ", " ").Trim();
                    string[] strInvoiceDate = _strText.Trim().Split(' ');
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                    _strText = _lines[_index + 1].Trim();
                    string[] strInvoiceNo = _strText.Trim().Split(' ');
                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    _bBonny = true;
                }
                else if (strText.ToUpper().Contains("INVOICE NO"))
                {
                    if (strText.Contains(": "))
                    {
                        string[] strInvoiceNo = strText.Replace(": ", "").Trim().Split(' ');
                        txtInvoiceNo.Text = strInvoiceNo[0];
                    }
                    else if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index + 1].Replace(": ", "");
                }
                else if (strText.ToUpper().Contains("SUPPLIER'S REF.") && txtPurchaseParty.Text.Contains("SAM TRADERS"))
                {
                    if (txtInvoiceNo.Text.Contains("DELIVERY NOTE") || txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index + 1];
                }
                else if (strText.ToUpper().Contains("SARAOGI SUPER SALES PVT. LTD") && txtGSTNo.Text.Contains("07AADCJ2544A1Z3"))
                {
                    if (txtInvoiceNo.Text == "")
                        txtInvoiceNo.Text = _lines[_index - 1].Trim();
                }
                else if (strText.ToUpper().Contains("DATE OF INVOICE :"))
                {
                    string[] strInvoiceDate = strText.Replace("                    ", " ").Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[4].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 3)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[3].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains(": INVOICE DATE"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[0].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NUMBER   :"))
                {
                    string[] strInvoiceNo = strText.Trim().Split(' ');
                    if (strInvoiceNo.Length > 1)
                        txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE DATE         :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE "))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        if (strInvoiceDate[2].Length > 8)
                        {
                            DateTime _iDate = DateTime.Now;
                            ConvertDateTime(ref _iDate, strInvoiceDate[2].Trim());
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Trim() == "INVOICE" && _index > 0)
                {
                    if (txtInvoiceNo.Text == "ISSUE DATE :")
                        txtInvoiceNo.Text = "";

                    if (txtInvoiceNo.Text == "" && txtInvoiceDate.Text.Length != 10)
                    {
                        string[] strGST = _lines[_index + 1].Split(' ');
                        txtInvoiceNo.Text = strGST[0];

                        strGST = _lines[_index + 2].Split(' ');
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strGST[0]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE"))
                {
                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');

                        if (txtInvoiceNo.Text == "")
                            txtInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    }
                }
                else if (strText.ToUpper().Contains("DATED") && !strText.ToUpper().Contains("LR DATED"))
                {
                    if (_lines[_index + 1] != "" && !_lines[_index + 1].Contains("Delivery Note"))
                    {
                        if (txtInvoiceDate.Text.Length != 10)
                        {
                            DateTime _iDate = DateTime.Now;
                            string[] strInvoiceDate = strText.Trim().Split(' ');
                            if (strInvoiceDate[strInvoiceDate.Length - 1].Length > 6)
                                ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                            else
                                ConvertDateTime(ref _iDate, _lines[_index + 1]);
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("DATE.   :"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        DateTime _iDate = DateTime.Now;
                        string[] strInvoiceDate = _strText.Trim().Split(' ');
                        if (strInvoiceDate.Length > 2)
                            ConvertDateTime(ref _iDate, strInvoiceDate[2]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATE :"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        DateTime _iDate = DateTime.Now;
                        string[] strInvoiceDate = _strText.Trim().Split(' ');
                        if (strInvoiceDate.Length > 2)
                            ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATE"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index + 1].Replace(":", "").Trim();
                        if (strDate.Length > 7 && (strDate.Contains("2019") || strDate.Contains("2020") || strDate.Contains("2021")))
                        {
                            DateTime _iDate = DateTime.Now;
                            ConvertDateTime(ref _iDate, strDate);
                            txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("ORIGINAL FOR RECIPIENT"))
                {
                    string[] strGST = strText.Trim().Split(' ');
                    if (txtGSTNo.Text == "" && strGST.Length > 1)
                    {
                        string _strGST = strGST[0];
                        if (_strGST.Length == 15)
                            SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                    }
                }
                else if (strText.Replace(" ", "").Replace(" ", "").ToUpper().Trim().Contains("BILLINGADDRESSSHIPPINGADDRESSTRANSPORT"))
                {
                    string _strText = _lines[_index + 1];
                    string[] strGST = _strText.Trim().Split(' ');
                    SetSupplierDetailsWithGSTNo(strGST[0].Trim());
                }
                else if (strText.ToUpper().Contains("PLACE OF SUPPLY"))
                {
                    if (txtPurchaseParty.Text.Contains("JAI AMBEY"))
                    {
                        txtInvoiceNo.Text = _lines[_index + 1];
                        _itemIndex = _index + 20;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("JOLLY FASHIONS"))
                {
                    txtInvoiceNo.Text = _lines[_index - 1];
                    _itemIndex = _index + 20;
                    break;
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("J.D. FASHION WEAR"))
                {
                    if (txtInvoiceDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index - 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate);
                        txtInvoiceDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if ((strText.ToUpper().Contains("ME/") || strText.ToUpper().Contains("HLLP")) && (strText.ToUpper().Contains("19-20") || strText.ToUpper().Contains("20-21")))
                {
                    txtInvoiceNo.Text = strText.Trim();
                }
                else if (strText.Contains("MRP"))
                {
                    if (txtPurchaseParty.Text.Contains("SPARKY"))
                    {
                        _itemIndex = _index + 2;
                        break;
                    }
                    else if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        _itemIndex = _index + 6;
                        break;
                    }
                    else if (txtPurchaseParty.Text.Contains("KC GARMENTS"))
                    {
                        _itemIndex = _index;
                        break;
                    }
                    else if (strText.ToUpper().Contains("DESCRIPTION"))
                    {
                        _itemIndex = _index + 1;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("DESCRIPTION OF GOODS"))
                {
                    if (txtInvoiceNo.Text != "" || txtPurchaseParty.Text != "")
                    {
                        _itemIndex = _index + 1;
                        if (txtPurchaseParty.Text.Contains("AGARWAL COLLECTION"))
                            _itemIndex--;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("DESCRIPTION"))
                {
                    _itemIndex = _index + 1;
                    if (txtPurchaseParty.Text.Contains("S.R CREATION"))
                        _itemIndex--;
                    break;
                }
                else if (strText.ToUpper().Contains("H.O. ADDRESS :"))
                {
                    _itemIndex = _index;
                    break;
                }
                else if (strText.ToUpper().Contains("GRS NO."))
                {
                    _strSType = "AGENT";
                }
                _index++;
            }

            if (_bBonny && txtPurchaseParty.Text == "")
            {
                txtGSTNo.Text = "24AATFB2023M1ZE";
                txtPurchaseParty.Text = "AH5112 BONNYS NX : AHD";
                if (txtPurchaseParty.Text.Contains("BONNYS NX"))
                    _itemIndex = 20;
            }
        }

        private void SetSupplierDetailsWithGSTNo(string strGSTNO)
        {
            string strPartyName = "";
            if (strGSTNO != "")
            {
                bool _blackListed = false;
                if (dba.CheckTransactionLockWithBlackListFromGSTNo(strGSTNO, ref _blackListed, ref strPartyName))
                {
                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Text = "";
                    txtGSTNo.Text = "";
                }
                else if (_blackListed)
                {
                    MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Text = "";
                    txtGSTNo.Text = "";
                }
                else
                {
                    txtPurchaseParty.Text = strPartyName;
                    txtGSTNo.Text = strGSTNO;
                    //GetPartyDhara();
                }
            }
        }

        private void SetSupplierDetailsWithSupplierName(string strSupplierName)
        {
            if (strSupplierName != "")
            {
                bool _blackListed = false;
                if (dba.CheckTransactionLockWithBlackList(strSupplierName, ref _blackListed))
                {
                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Text = "";
                    txtGSTNo.Text = "";
                }
                else if (_blackListed)
                {
                    MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Text = "";
                    txtGSTNo.Text = "";
                }
                else
                {
                    txtPurchaseParty.Text = strSupplierName;
                   // GetPartyDhara();
                }
            }
        }

        private bool ConvertDateTime(ref DateTime _date, string strDate)
        {
            try
            {
                double dDate = dba.ConvertObjectToDouble(strDate);
                if (dDate > 0)
                    _date = DateTime.FromOADate(dDate);
                else
                {
                    try
                    {
                        char split = '/';
                        if (strDate.Contains("-"))
                            split = '-';
                        string[] strNDate = strDate.Split(' ');
                        string[] strAllDate = strNDate[0].Split(split);
                        string strMonth = strAllDate[0], strFormat = "dd/MM/yyyy";
                        if (strMonth.Length == 1)
                            strFormat = "d/M/yyyy";

                        if (dba.ConvertObjectToInt(strMonth) == MainPage.currentDate.Month)
                        {
                            strFormat = "MM/dd/yyyy";
                            if (strMonth.Length == 1)
                                strFormat = "M/d/yyyy";
                        }
                        if (strAllDate.Length > 2)
                        {
                            if (strAllDate[2].Length == 2)
                                strFormat = strFormat.Replace("yyyy", "yy");
                        }

                        if (strDate.Contains("-"))
                            strFormat = strFormat.Replace("/", "-");

                        if (strDate.Length > 10)
                        {
                            string strTimeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                            if (strDate.Contains("AM") || strDate.Contains("PM"))
                                strFormat += " " + strTimeFormat;// " hh:mm:ss tt";//
                            else
                            {
                                string[] strTime = strDate.Split(':');
                                if (strTime.Length > 2)
                                    strFormat += " HH:mm:ss";
                                else
                                    strFormat += " HH:mm";
                            }
                        }

                        _date = dba.ConvertDateInExactFormat(strDate, strFormat);
                    }
                    catch
                    {
                        _date = Convert.ToDateTime(strDate);
                    }
                }
                return true;
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }

        private void txtLRNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtLRDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtLRDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, false, false, true);
            }
        }

        private void txtGodown_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("MATERIALCENTER", "SEARCH GODOWN NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtGodown.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtStockStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASESTATUS", "STOCK STATUS", e.KeyCode);
                        objSearch.ShowDialog();
                        txtStockStatus.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtmanufacturer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH MANUFACTURER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtmanufacturer.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void txtTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransport.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void chkTCSAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                CalculateAllAmount();
            }
        }

        private bool CheckItemNameExistence(ref string strItemName, ref string strHSNCode)
        {
          
            try
            {
                if (strItemName != "")
                {
                    strItemName = CheckItemName(strItemName, strHSNCode);
                    if (strItemName != "")
                    {
                        if (strHSNCode == "")
                            strHSNCode = GetHSNCodeFromItem(strItemName);
                    }
                    else if (strHSNCode == "")
                    {
                        strHSNCode = GetHSNCodeFromItem(strItemName);
                    }
                }
            }
            catch { }
            return true;
        }

        private void txtGSTNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strSearch = "PURCHASEPARTYWITHGSTNO";
                        if (MainPage.strUserRole.Contains("ADMIN"))
                            strSearch = "PURCHASEPERSONALPARTY";

                        SearchData objSearch = new SearchData(strSearch, "SEARCH SUNDRY CREDITOR", e.KeyCode);
                        objSearch.ShowDialog();
                        string strData = objSearch.strSelectedData, strGSTNo = "", strStateName = "";
                        if (strData != "")
                        {
                            bool _blackListed = false;
                            if (dba.CheckTransactionLockWithBlackListGSTNo(strData, ref _blackListed, ref strGSTNo, ref strStateName))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPurchaseParty.Text = txtStateName.Text = txtGSTNo.Text = "";
                            }
                            else if (_blackListed)
                            {
                                MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtPurchaseParty.Text = txtStateName.Text = txtGSTNo.Text = "";
                            }
                            else
                            {
                                txtPurchaseParty.Text = strData;
                                txtGSTNo.Text = strGSTNo;
                                txtStateName.Text = strStateName;
                                //GetPartyDhara();
                            }
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private string GetHSNCodeFromItem(string strItem)
        {
            string strQuery = "";
            strQuery = " Select _IGM.HSNCode from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where _IM.ItemName='" + strItem + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        private string CheckItemName(string strItemName, string strHSNCode)
        {
            try
            {
                if (strHSNCode.Length > 4)
                    strHSNCode = strHSNCode.Substring(0, 4);
                string strMainItemName = strItemName, strItemQuery = "", strFirstItemQuery = "", strSecondItemQuery = "", strThirdQuery = "", strFirstItemName = "", strSecondItemName = "", strThirdItemName = "", strSubQuery = "";
                string[] strItem = strItemName.Split(' ');
                strMainItemName = strMainItemName.Replace(" ", "").Replace(".", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace("/", "").Replace("@", "");
                strMainItemName = System.Text.RegularExpressions.Regex.Replace(strMainItemName, @"[\d-]", string.Empty);
                if (strItem.Length > 1)
                {
                    strFirstItemName = strItem[0];
                    //strFirstItemName= System.Text.RegularExpressions.Regex.Replace(strFirstItemName, @"[\d-]", string.Empty);
                    if (strFirstItemName.Length > 2)
                    {
                        for (int i = 0; i < strFirstItemName.Length; i++)
                        {
                            if (i == 0)
                                strFirstItemQuery += " and DesignName Like('" + strFirstItemName[i] + "%') ";
                            else
                                strFirstItemQuery += " and DesignName Like('%" + strFirstItemName[i] + "%') ";
                        }
                    }
                    strSecondItemName = strItem[1];
                    strSecondItemName = System.Text.RegularExpressions.Regex.Replace(strSecondItemName, @"[\d-]", string.Empty);
                    if (strSecondItemName.Length > 2)
                    {
                        for (int i = 0; i < strSecondItemName.Length; i++)
                        {
                            if (i == 0)
                                strSecondItemQuery += " and DesignName Like('" + strSecondItemName[i] + "%') ";
                            else
                                strSecondItemQuery += " and DesignName Like('%" + strSecondItemName[i] + "%') ";
                        }
                    }
                    if (strItem.Length > 2)
                    {
                        strThirdItemName = strItem[2];
                        strThirdItemName = System.Text.RegularExpressions.Regex.Replace(strThirdItemName, @"[\d-]", string.Empty);
                        if (strThirdItemName.Length > 2)
                        {
                            for (int i = 0; i < strThirdItemName.Length - 1; i++)
                            {
                                if (i == 0)
                                    strThirdQuery += " and DesignName Like('" + strThirdItemName[i] + "%') ";
                                else
                                    strThirdQuery += " and DesignName Like('%" + strThirdItemName[i] + "%') ";
                            }
                        }
                    }
                }
                if (strMainItemName.Length > 1)
                {
                    for (int i = 0; i < strMainItemName.Length; i++)
                    {
                        if (i == 0)
                            strItemQuery += " and DesignName Like('" + strMainItemName[i] + "%') ";
                        else
                            strItemQuery += " and DesignName Like('%" + strMainItemName[i] + "%') ";
                    }
                }

                if (strFirstItemQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,2 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strFirstItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,3 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strFirstItemQuery;
                }
                if (strSecondItemQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,4 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strSecondItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,5 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strSecondItemQuery;
                }
                if (strThirdQuery != "")
                {
                    strSubQuery += " UNION ALL Select Distinct ItemName,6 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strThirdQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                 + " Select Distinct ItemName,7 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strThirdQuery;
                }

                if (strItemQuery != "")
                {
                    string strQuery = " Select Top 1 * from ( "
                                    + " Select Distinct ItemName,-1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName ='" + strItemName + "' UNION ALL "
                                    + " Select Distinct ItemName,0 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName Like('" + strItemName + "%') UNION ALL "
                                    + " Select Distinct ItemName,1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') and DesignName Like('%" + strItemName + "%') UNION ALL "
                                    + " Select Distinct ItemName,0 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('%" + strHSNCode + "%') " + strItemQuery.Replace("DesignName", "ItemName") + " UNION ALL "
                                    + " Select Distinct ItemName,1 SerialNo from ItemMapping Where UpdatedBy Like('" + strHSNCode + "') " + strItemQuery + strSubQuery
                                    + " )_Sale Order By SerialNo ";

                    object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                    return Convert.ToString(objValue);
                }
                else
                    return "";
            }
            catch { }
            return "";
        }
    }
}
