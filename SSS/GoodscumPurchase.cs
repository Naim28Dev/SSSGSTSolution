using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace SSS
{
    public partial class GoodscumPurchase : Form
    {
        DataBaseAccess dba;
        DataTable dtItemName;
        string strFullOrderNo = "", strLastSerialNo = "", strOldPartyName = "", _strPcsType = "", _strCustomerName = "", _strSubPartyName = "", _strSupplierName = "", _strPDFFilePath = "", _strBillType = "";
        double dOldNetAmt = 0, dTotalAmount = 0, _dCancelQty_ByUser = 0, _dFinalAmt = 0;
        public bool saleStatus = false, updateStatus = false, newStatus = false;
        public GoodscumPurchase()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public GoodscumPurchase(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            newStatus = bStatus;
        }

        public GoodscumPurchase(string strCode, string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);
        }

        public GoodscumPurchase(string strCode, string strSNo, bool sStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            saleStatus = sStatus;
            BindRecordWithControl(strSNo);
            EnableAllControls();
        }

        private void GoodsReciept_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlRelatedParty.Visible)
                    pnlRelatedParty.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
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
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
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
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                    else if (e.Control && e.Shift && e.KeyCode == Keys.D)
                    {
                        if (btnAdd.Enabled && btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            btnAdd.Text = "&Save";
                            txtPurchaseInvoiceNo.Clear();
                            SetSerialNo();
                            EnableAllControls();
                            EnableForAdding();
                            txtSalesParty.Focus();
                        }
                    }
                }
            }
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = " Select GReceiveCode,(Select ISNULL(MAX(ReceiptNo),0) from GoodsReceive Where PurchaseStatus=1 and ReceiptCode=GReceiveCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["GReceiveCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }


                }
            }
            catch
            {
            }
        }

        private void SetColumnWidth()
        {
            if (txtBillCode.Text.Contains("SRT") || txtBillCode.Text.Contains("SSO"))
            {
                dgrdDetails.Columns["itemName"].Width = 120;
                dgrdDetails.Columns["designName"].Width = 105;
                dgrdDetails.Columns["cut"].Visible = dgrdDetails.Columns["mtr"].Visible = dgrdDetails.Columns["fold"].Visible = true;
                lblTotalMTR.Enabled = true;
            }
            else
            {
                dgrdDetails.Columns["itemName"].Width = 200;
                dgrdDetails.Columns["designName"].Width = 190;
                dgrdDetails.Columns["cut"].Visible = dgrdDetails.Columns["mtr"].Visible = dgrdDetails.Columns["fold"].Visible = false;
                lblTotalMTR.Enabled = false;
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ReceiptNo),'') from GoodsReceive Where PurchaseStatus=1 and ReceiptCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ReceiptNo),'') from GoodsReceive Where PurchaseStatus=1 and ReceiptCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(ReceiptNo),'') from GoodsReceive Where PurchaseStatus=1 and ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(ReceiptNo),'') from GoodsReceive Where PurchaseStatus=1 and ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo<" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                if (strSerialNo != "")
                {
                    DisableAllControls();
                    ClearAllText();
                    txtReason.Clear();
                    pnlDeletionConfirmation.Visible = false;
                    btnAdd.TabStop = true;

                    string strQuery = " Select TOP 1 *,(CONVERT(varchar,InvoiceDate,103)) IDate,(CONVERT(varchar,ReceivingDate,103)) RDate,(CONVERT(varchar,OrderDate,103)) ODate,dbo.GetFullName(SalePartyID) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') HParty,ISNULL((PurchasePartyID+' '+SM.Name),'PERSONAL') PParty,(SM.NormalDhara)NDhara,(SM.SNDhara)SDhara,(SM.CFormApply)PremiumDhara,CheckedBy,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,ReceivingDate))) LockType,ISNULL(OtherBillStatus,0) OtherBStatus,SM.GSTNo,SaleBillNo from GoodsReceive GR OUTER APPLY ((Select TOP 1 ISNULL((CASE WHEN CheckStatus=1 then CheckedBy else '' end),'')CheckedBy,SaleBillNo from PurchaseRecord PR Where PR.GRSNo=(ReceiptCode+' '+CAST(ReceiptNo as varchar))))PR OUTER APPLY (Select TOP 1 Name,NormalDhara,SNDhara,CFormApply,GSTNo from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=GR.PurchasePartyID) SM Where ReceiptCode='" + txtBillCode.Text + "' And ReceiptNo=" + strSerialNo + " and PurchaseStatus=1 "
                                    // + " Select (OB.OrderCode+' '+CAST(OB.SerialNo as varchar)) ID,(CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)FullOrder,(CASE When OB.PurchasePartyID='' then OB.Personal else dbo.GetFullName(OB.PurchasePartyID) end) PParty,OB.Items,OB.Pieces,CAST((CAST(OB.Quantity as Money)-OB.AdjustedQty-ISNULL(OB.CancelQty,0)) as Numeric(18,0)) Quantity,OB.Amount,(Convert(varchar,OB.Date,103))Date,OB.Remark,OB.SchemeName from OrderBooking OB inner Join GoodsReceive GR on OB.SalePartyID=GR.SalePartyID and OB.SubPartyID=GR.SubPartyID Where OB.Status='PENDING' and GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + strSerialNo
                                    + "  Select (OB.OrderCode+' '+CAST(OB.SerialNo as varchar)) ID,(CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)FullOrder,(CASE When OB.PurchasePartyID='' then OB.Personal else dbo.GetFullName(OB.PurchasePartyID) end) PParty,OB.Items,OB.Pieces,CAST((CAST(OB.Quantity as Money)-OB.AdjustedQty-ISNULL(OB.CancelQty,0)) as Numeric(18,0)) Quantity,OB.Amount,(Convert(varchar,OB.Date,103))Date,OB.Remark,OB.SchemeName,(SalePartyID+' '+Name)SParty,SubPartyID,OrderCategory from OrderBooking OB OUTER APPLY (Select Name,SM.Other as SSSName from SupplierMaster SM Where (AreaCode+AccountNo)=OB.SalePartyID) SM OUTER APPLY (Select  SM1.Other as SName from GoodsReceive GR inner join SupplierMaster SM1 on GR.SalePartyID=SM1.AreaCode+SM1.AccountNo and GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + strSerialNo + ")SM2 WHere OB.Status='PENDING' and SSSName=SName "
                                    + " Select TransactionLock,GroupII,BlackList,Category,TINNumber from SupplierMaster SM inner Join GoodsReceive GR on (SM.AreaCode+CAST(SM.AccountNo as varchar))=GR.SalePartyID Where SM.GroupName !='SUB PARTY' and GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + strSerialNo
                                    + " Select *,(CASE WHEN RATE is NULL then ROUND((Amount/Quantity),2) else Rate end)NRate from dbo.[GoodsReceiveDetails] Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + strSerialNo + " Order by ID asc "
                                    + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            //pnlTax.Visible = true;
                            BindGoodsReceiveDetails(_dt, ds.Tables[3]);
                            BindDataWithControlUsingDataTable(_dt);
                            BindPendingOrderWithGrid(ds.Tables[1]);
                            BindGSTDetailsWithControl(ds.Tables[4]);
                            DataTable dt = ds.Tables[2];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                                    txtSalesParty.BackColor = Color.IndianRed;
                                else
                                    txtSalesParty.BackColor = Color.White;
                                if (Convert.ToString(dt.Rows[0]["Category"]) == "CASH PARTY" || Convert.ToString(dt.Rows[0]["TINNumber"]) == "CASH PARTY")
                                    pnlCash.Visible = true;
                                else
                                    pnlCash.Visible = false;
                            }
                        }
                    }
                    EditOption();

                }
            }
            catch
            {
            }
        }

        private void BindDataWithControlUsingDataTable(DataTable dt)
        {
            DisableAllControls();
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                _strPDFFilePath = lblID.Text = "";
                txtBillCode.Text = Convert.ToString(dr["ReceiptCode"]);
                txtBillNo.Text = Convert.ToString(dr["ReceiptNo"]);
                txtDate.Text = Convert.ToString(dr["RDate"]);
                txtSalesParty.Text = Convert.ToString(dr["SParty"]);
                txtSubParty.Text = Convert.ToString(dr["HParty"]);
                txtPurchaseParty.Text = strOldPartyName = Convert.ToString(dr["PParty"]);

                txtBox.Text = Convert.ToString(dr["Box"]);
                txtRemark.Text = Convert.ToString(dr["Remark"]);
                strFullOrderNo = txtOrderNo.Text = Convert.ToString(dr["OrderNo"]);
                txtOrderDate.Text = Convert.ToString(dr["ODate"]);
                txtSaleBillNo.Text = Convert.ToString(dr["SaleBillNo"]);

                string strPStatus = Convert.ToString(dr["PackingStatus"]).ToUpper();
                if (strPStatus == "DIRECT")
                    rdoDirect.Checked = true;
                else if (strPStatus == "PACKED")
                    rdoPacked.Checked = true;
                else if (strPStatus == "CAMEOFFICE")
                    rdoCameOffice.Checked = true;
                else
                    rdoSummary.Checked = true;

                lblQty.Text = Convert.ToString(dr["Quantity"]);
                txtPcsType.Text = Convert.ToString(dr["Pieces"]);
                txtPurchaseInvoiceNo.Text = Convert.ToString(dr["InvoiceNo"]);
                txtPurchaseDate.Text = Convert.ToString(dr["IDate"]);
                txtPurchaseType.Text = Convert.ToString(dr["PurchaseType"]);
                //txtReverseCharge.Text = Convert.ToString(dr["ReverseCharge"]);
                txtNoOfCase.Text = Convert.ToString(dr["NoOfCase"]);
                txtSignAmt.Text = Convert.ToString(dr["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(dr["OtherAmount"]);
                txtDisPer.Text = Convert.ToString(dr["DisPer"]);
                txtDiscountAmt.Text = Convert.ToString(dr["DisAmount"]);
                txtTaxPer.Text = Convert.ToString(dr["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(dr["TaxAmount"]);
                txtPcsAmt.Text = dba.ConvertObjectToDouble(dr["PcsRateAmt"]).ToString("N2", MainPage.indianCurancy);
                txtTaxFree.Text = Convert.ToString(dr["Tax"]);
                txtPackingAmt.Text = Convert.ToString(dr["Packing"]);
                txtFreight.Text = Convert.ToString(dr["Freight"]);
                txtROSign.Text = Convert.ToString(dr["RoundOffSign"]);
                txtRoundOff.Text = Convert.ToString(dr["RoundOffAmt"]);

                if (txtROSign.Text == "")
                    txtROSign.Text = "+";
                if (txtRoundOff.Text == "")
                    txtRoundOff.Text = "0.00";

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(dr["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                lblGrossAmt.Text = dba.ConvertObjectToDouble(dr["GrossAmount"]).ToString("N2", MainPage.indianCurancy);// Convert.ToString(dr["GrossAmount"]);
                dOldNetAmt = dba.ConvertObjectToDouble(dr["NetAmount"]);
                lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);
                dTotalAmount = dba.ConvertObjectToDouble(dr["Amount"]);

                txtNormalDhara.Text = Convert.ToString(dr["NDhara"]);
                txtSuperNetDhara.Text = Convert.ToString(dr["SDhara"]);
                txtPremiumDhara.Text = Convert.ToString(dr["PremiumDhara"]);
                txtGSTNo.Text = Convert.ToString(dr["GSTNo"]);

                double dSPer = dba.ConvertObjectToDouble(dr["SpecialDscPer"]);
                txtSpeDiscPer.Text = dSPer.ToString("0.####");
                txtSpecialDiscAmt.Text = Convert.ToString(dr["SpecialDscAmt"]);

                if (Convert.ToString(dr["Dhara"]) == "NORMAL")
                    rdoNormalDhara.Checked = true;
                else if (Convert.ToString(dr["Dhara"]) == "SUPER")
                    rdoSuperNet.Checked = true;
                else
                    rdoPremium.Checked = true;

                if (txtPurchaseParty.Text == "")
                    txtPurchaseParty.Text = "PERSONAL";
                if (txtSubParty.Text == "")
                    txtSubParty.Text = "SELF";

                if (Convert.ToBoolean(dr["OtherBStatus"]))
                    btnPrint.Enabled = false;
                else
                    btnPrint.Enabled = true;

                string strCreatedBy = Convert.ToString(dr["CreatedBy"]), strUpdatedBy = Convert.ToString(dr["UpdatedBy"]), strCheckedBy = Convert.ToString(dr["CheckedBy"]), strPrintedBy = Convert.ToString(dr["PrintedBy"]);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += ", Updated By : " + strUpdatedBy;

                if (strCheckedBy != "")
                {
                    lblCreatedBy.Text += ", Checked By : " + strCheckedBy;
                    btnChecked.Text = "Status : Checked";
                    btnChecked.BackColor = Color.DarkGreen;
                }
                else
                {
                    btnChecked.Text = "Status : Un-Checked";
                    btnChecked.BackColor = Color.FromArgb(185, 30, 12);
                }
                if (strPrintedBy != "")
                    lblCreatedBy.Text += ", Printed By : " + strPrintedBy;

                if (txtSpeDiscPer.Text == "")
                    txtSpeDiscPer.Text = txtSpecialDiscAmt.Text = "0.00";
                chkTCSAmt.Checked = false;
                txtTCSPer.Text = txtTCSAmt.Text = "0.00";
                if (dt.Columns.Contains("TCSPer"))
                {
                    double dTCSPer = dba.ConvertObjectToDouble(dr["TCSPer"]), dTCSAmt = dba.ConvertObjectToDouble(dr["TCSAmt"]);
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

                EnableAndDisableBySale(dr["SaleBill"]);


                if (Convert.ToString(dr["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    // else
                    // btnEdit.Enabled = btnDelete.Enabled = true;
                }

                //    EnableAllControls();
                //CalculateTotalAmt();                
            }
        }

        private void BindGoodsReceiveDetails(DataTable _dtMain, DataTable _dtDetails)
        {
            double dMtr = 0, dTMtr = 0, dFold = 0;
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dMtr = dba.ConvertObjectToDouble(row["MTR"]);
                    dFold = dba.ConvertObjectToDouble(row["Fold"]);
                    dgrdDetails.Rows[_index].Cells["sno"].Value = (_index + 1) + ".";
                    dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    if (Convert.ToString(row["DesignName"]) != "")
                        dgrdDetails.Rows[_index].Cells["designName"].Value = row["DesignName"];
                    else
                        dgrdDetails.Rows[_index].Cells["designName"].Value = row["ItemName"];
                    dgrdDetails.Rows[_index].Cells["gQty"].Value = row["Quantity"];
                    dgrdDetails.Rows[_index].Cells["gAmount"].Value = row["Amount"];
                    dgrdDetails.Rows[_index].Cells["cut"].Value = row["Cut"];
                    dgrdDetails.Rows[_index].Cells["mtr"].Value = dMtr.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[_index].Cells["fold"].Value = dFold;
                    dgrdDetails.Rows[_index].Cells["gRate"].Value = row["NRate"];

                    if (Convert.ToString(row["GRate"]) != "")
                        dgrdDetails.Rows[_index].Cells["rate"].Value = row["GRate"];
                    else
                        dgrdDetails.Rows[_index].Cells["rate"].Value = row["NRate"];

                    if (dMtr != 0 && dFold != 0 && dFold != 100)
                        dMtr = (dMtr * dFold / 100);
                    dTMtr += dMtr;

                    _index++;
                }
            }
            else
            {
                dgrdDetails.Rows.Add();
                DataRow row = _dtMain.Rows[0];
                dgrdDetails.Rows[0].Cells["sno"].Value = "1.";
                dgrdDetails.Rows[0].Cells["itemName"].Value = row["Item"];
                dgrdDetails.Rows[0].Cells["gQty"].Value = row["Quantity"];
                dgrdDetails.Rows[0].Cells["gAmount"].Value = row["Amount"];
                dgrdDetails.Rows[0].Cells["gPacking"].Value = row["Packing"];
                dgrdDetails.Rows[0].Cells["gFreight"].Value = row["Freight"];
                dgrdDetails.Rows[0].Cells["gTax"].Value = row["Tax"];
            }
            lblTotalMTR.Text = dTMtr.ToString("N2", MainPage.indianCurancy);
        }

        private void EnableAllControls()
        {
            txtNoOfCase.ReadOnly = txtPurchaseDate.ReadOnly = txtPurchaseInvoiceNo.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtDate.ReadOnly = txtBox.ReadOnly = txtRemark.ReadOnly = txtSpeDiscPer.ReadOnly = txtSpecialDiscAmt.ReadOnly = txtPcsAmt.ReadOnly = txtTaxFree.ReadOnly = txtPackingAmt.ReadOnly = txtFreight.ReadOnly =txtTaxPer.ReadOnly= false;
            grpDiscountStatus.Enabled = btnOrderClear.Enabled = chkTCSAmt.Enabled = true;

            if (btnEdit.Text == "&Update" && !btnAdd.Enabled && !btnSearch.Enabled)
                grpPackingStatus.Enabled = false;
            else
                grpPackingStatus.Enabled = true;
        }

        private void DisableAllControls()
        {
            txtTCSPer.ReadOnly =  txtNoOfCase.ReadOnly = txtPurchaseDate.ReadOnly = txtPurchaseInvoiceNo.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = txtDate.ReadOnly = txtBox.ReadOnly = txtRemark.ReadOnly = txtSpeDiscPer.ReadOnly = txtSpecialDiscAmt.ReadOnly = txtPcsAmt.ReadOnly = txtTaxFree.ReadOnly = txtPackingAmt.ReadOnly = txtFreight.ReadOnly = txtTaxPer.ReadOnly = true;
            txtBillNo.ReadOnly = grpPackingStatus.Enabled = grpDiscountStatus.Enabled = btnOrderClear.Enabled = chkTCSAmt.Enabled = false;
        }

        private void EnableAndDisableBySale(object objValue)
        {
            if (Convert.ToString(objValue).ToUpper() == "CLEAR" || saleStatus)
            {
                txtSalesParty.Enabled = txtSubParty.Enabled = btnDelete.Enabled = false;
                if (saleStatus)
                {
                    txtPurchaseParty.Enabled = grpDiscountStatus.Enabled = true;
                    dgrdDetails.ReadOnly = grpPackingStatus.Enabled = false;
                    btnAdd.Enabled = btnSearch.Enabled = false;
                    btnEdit.Text = "&Update";
                    txtDate.Focus();
                }
                else
                {
                    txtPurchaseParty.Enabled = grpPackingStatus.Enabled = grpDiscountStatus.Enabled = false;// txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
                    dgrdDetails.ReadOnly = true;
                }
            }
            else
            {
                txtSalesParty.Enabled = txtSubParty.Enabled = txtPurchaseParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = btnDelete.Enabled = true;// dgrdDetails.Enabled = true;// txtAmount.Enabled = txtPacking.Enabled = txtTax.Enabled = txtFreight.Enabled =
                dgrdDetails.ReadOnly = false;
            }
        }

        private void EnableForAdding()
        {
            txtSalesParty.Enabled = txtSubParty.Enabled = btnDelete.Enabled = txtPurchaseParty.Enabled = grpPackingStatus.Enabled = grpDiscountStatus.Enabled = true;// dgrdDetails.Enabled= true;
            dgrdDetails.ReadOnly = false;
        }

        private void ClearAllText()
        {
            txtSaleBillNo.Text = txtSalesParty.Text = txtSubParty.Text = txtPurchaseParty.Text = txtBox.Text = txtOrderNo.Text = txtOrderDate.Text = lblID.Text = lblCreatedBy.Text = txtRemark.Text = "";
            txtRoundOff.Text = lblTaxableAmt.Text = txtNoOfCase.Text = txtPurchaseType.Text = txtPurchaseInvoiceNo.Text = txtPcsType.Text = strOldPartyName = txtGSTNo.Text = "";
            txtNormalDhara.Text = txtSuperNetDhara.Text = txtPremiumDhara.Text = txtBox.Text = lblQty.Text = lblTotalMTR.Text = "0";
            txtTaxPer.Text = "18.00";
            txtROSign.Text = "+";
            // txtReverseCharge.Text = "NOT APPLICABLE";
            txtPcsType.Text = "LOOSE";
            _strPDFFilePath = _strPcsType = _strCustomerName = _strSubPartyName = _strSupplierName = "";

            txtPDFFileName.Text = txtPurchaseDate.Text = lblCreatedBy.Text = "";
            txtTCSAmt.Text = txtOtherAmt.Text = txtDisPer.Text = txtDiscountAmt.Text = txtTaxPer.Text = txtTaxAmt.Text = lblGrossAmt.Text = lblNetAmt.Text = txtSpecialDiscAmt.Text = txtSpeDiscPer.Text = txtPcsAmt.Text = txtTaxFree.Text = txtPackingAmt.Text = txtFreight.Text = "0.00";
            txtSignAmt.Text = "-";
            txtSalesParty.BackColor = Color.White;
            dgrdPending.Rows.Clear();
            dgrdRelatedParty.Rows.Clear();

            chkSendSMS.Checked = chkTCSAmt.Checked = false;
            rdoPacked.Checked = rdoNormalDhara.Checked = true;
            pnlDeletionConfirmation.Visible = false;
            pnlCash.Visible = false;

            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;
            dOldNetAmt = dTotalAmount = _dCancelQty_ByUser = 0;
            btnChecked.Text = "Status : Un-Checked";
            btnChecked.BackColor = Color.FromArgb(185, 30, 12);
            dgrdDetails.ReadOnly = false;
            dgrdDetails.Rows[0].Cells["sno"].Value = "1.";
            btnPrint.Enabled = true;
            txtTCSPer.Text = MainPage.dTCSPer.ToString("0.000");

            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy"); // txtPurchaseDate.Text = 
        }

        //private void SetSerialNo()
        //{
        //    try
        //    {
        //        if (txtBillCode.Text != "")
        //        {
        //            DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(ReceiptNo)+1,1)SNo,(Select ISNULL(Max(GoodsReceiveNo)+1,1) from MaxSerialNo)ReceiptNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='PURCHASE' and TaxIncluded=0) TaxName  from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' ");
        //            if (table.Rows.Count > 0)
        //            {
        //                int receiptNo = Convert.ToInt32(table.Rows[0]["SNo"]), maxReceiptNo = Convert.ToInt32(table.Rows[0]["ReceiptNo"]);
        //                if (receiptNo > maxReceiptNo)
        //                    txtBillNo.Text = Convert.ToString(receiptNo);
        //                else
        //                    txtBillNo.Text = Convert.ToString(maxReceiptNo);

        //                txtPurchaseType.Text = Convert.ToString(table.Rows[0]["TaxName"]);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string[] strReport = { "Exception occurred in Set Goods Receipt No in Goods Received", ex.Message };
        //        dba.CreateErrorReports(strReport);
        //    }
        //}

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(ReceiptNo)+1,1)SNo,(Select ISNULL(Max(PurchaseBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from PurchaseBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='LOCAL' and SaleType='PURCHASE' and TaxIncluded=0) TaxName  from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "')Purchase ");
                    if (table.Rows.Count > 0)
                    {
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                        txtPurchaseType.Text = Convert.ToString(table.Rows[0]["TaxName"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in set sale bill No in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        
        private void txtReceiptNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Update" && txtOrderNo.Text.Trim() != "")
                {
                    MessageBox.Show("Sorry ! Please remove Order no after that you can change sale party name !!", "Order No remove", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillCode.Focus();
                }
                else if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("SALESPARTY");
                        if (strData != "")
                        {
                            dgrdPending.Rows.Clear();
                            txtSalesParty.Text = strData;
                            txtSubParty.Text = "SELF";
                            GetPendingOrderAndOther();
                        }
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
                            {
                                dgrdPending.Rows.Clear();
                                txtSalesParty.Text = strData;
                                txtSubParty.Text = "SELF";
                                GetPendingOrderAndOther();
                                GetRelatedpartyDetails();

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

        private void GetRelatedpartyDetails()
        {
            pnlRelatedParty.Visible = false;
            dgrdRelatedParty.Rows.Clear();

            if (txtSalesParty.Text != "")
            {
                DataTable dt = dba.GetRelatedPartyDetails(txtSalesParty.Text);
                if (dt.Rows.Count > 0)
                {
                    dgrdRelatedParty.Rows.Add(dt.Rows.Count);
                    int _index = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdRelatedParty.Rows[_index].Cells["rSno"].Value = (_index + 1) + ".";
                        dgrdRelatedParty.Rows[_index].Cells["relatedParty"].Value = row["Name"];
                        _index++;
                    }
                }
            }
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
        }

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtSalesParty.Text != "")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SUBPARTY", txtSalesParty.Text, "SEARCH SUB PARTY", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtSubParty.Text = objSearch.strSelectedData;
                            GetPendingOrderAndOther();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void ClearFewRecordOnPartyChange()
        {
            txtSalesParty.BackColor = Color.White;
            pnlCash.Visible = false;
            dgrdPending.Rows.Clear();
            if (txtOrderNo.Text != "")
            {
                lblNetAmt.Text = txtDisPer.Text = txtDiscountAmt.Text = txtOtherAmt.Text = lblGrossAmt.Text = "0.00";//txtAmount.Text = txtPacking.Text = txtFreight.Text = txtTax.Text =
                txtPurchaseParty.Text = txtOrderNo.Text = txtOrderDate.Text = lblID.Text = "";//txtItemName.Text = txtPiecesType.Text =
                CalculateTotalAmt();
            }
        }

        private void GetPendingOrderAndOther()
        {
            try
            {
                ClearFewRecordOnPartyChange();
                if (txtSalesParty.Text != "" && txtSubParty.Text != "")
                {
                    bool tStatus = true;
                    string strSaleParty = "", strSubParty = "";
                    string[] strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strSaleParty = strFullName[0].Trim();
                    strFullName = txtSubParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                        strSubParty = strFullName[0].Trim();

                    if (strSaleParty != "" && strSubParty != "")
                    {
                        string strQuery = " Select TransactionLock,GroupII,BlackList,Category,TINNumber,ISNULL(STransactionLock,'False')STransactionLock,ISNULL(SBlackList,'False')SBlackList,UPPER(SM.Other1) as OrangeZone,ISNULL(SOrangeZone,'FALSE')SOrangeZone from SupplierMaster SM OUTER APPLY (Select SM1.TransactionLock as STransactionLock,SM1.BlackList as SBlackList,UPPER(SM1.Other1) as SOrangeZone from SupplierMaster SM1 Where GroupName='SUB PARTY' and (SM1.AreaCode+SM1.AccountNo)='" + strSubParty + "')SM1 Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' "
                                        + " Select (OrderCode+' '+CAST(SerialNo as varchar)) ID,(CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)FullOrder,(CASE When PurchasePartyID='' then Personal else dbo.GetFullName(PurchasePartyID) end) PParty,Items,Pieces,CAST((CAST(Quantity as Money)-AdjustedQty-CancelQty) as Numeric(18,0)) as Quantity,Amount,(Convert(varchar,Date,103))Date,Remark,SchemeName,(SalePartyID+' '+Name)SParty,SubPartyID,OrderCategory from OrderBooking OB Cross APPLY (Select Name,Other as SSSName from SupplierMaster SM Where SM.AreaCode+SM.AccountNo=SalePartyID) SM OUTER APPLY (Select SM1.Other as SName from SupplierMaster SM1 Where AreaCode+AccountNo='" + strSaleParty + "')SM2 Where Status='PENDING' and SSSName=SName Order by OB.Date asc ";
                        //+ " Select (OrderCode+' '+CAST(SerialNo as varchar)) ID,(CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)FullOrder,(CASE When P_Party='' then Personal else dbo.GetFullName(PurchasePartyID) end) PParty,Items,Pieces,CAST((CAST(Quantity as Money)-AdjustedQty-CancelQty) as Numeric(18,0)) as Quantity,Amount,(Convert(varchar,Date,103))Date,Remark,SchemeName from OrderBooking OB Where Status='PENDING' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' Order by OB.Date asc ";

                        DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSalesParty.Clear();
                                    txtSubParty.Clear();
                                    txtSalesParty.Focus();
                                    tStatus = false;
                                }
                                if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                                {
                                    //txtSalesParty.BackColor = Color.IndianRed;
                                    MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSalesParty.Clear();
                                    txtSubParty.Clear();
                                    txtSalesParty.Focus();
                                    tStatus = false;
                                }
                                if (Convert.ToString(dt.Rows[0]["OrangeZone"]) == "TRUE")
                                {
                                    //txtSalesParty.BackColor = Color.IndianRed;
                                    MessageBox.Show("This Account is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtSalesParty.Clear();
                                    txtSubParty.Clear();
                                    txtSalesParty.Focus();
                                    tStatus = false;
                                }
                                if (txtSubParty.Text != "SELF")
                                {
                                    if (Convert.ToBoolean(dt.Rows[0]["STransactionLock"]))
                                    {
                                        MessageBox.Show("Transaction has been locked on this Sub Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        txtSubParty.Text = "SELF";
                                        txtSubParty.Focus();
                                        tStatus = false;
                                    }
                                    if (Convert.ToBoolean(dt.Rows[0]["SBlackList"]))
                                    {
                                        //txtSalesParty.BackColor = Color.IndianRed;
                                        MessageBox.Show("This Sub Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        txtSubParty.Text = "SELF";
                                        txtSubParty.Focus();
                                        tStatus = false;
                                    }
                                    if (Convert.ToString(dt.Rows[0]["SOrangeZone"]) == "TRUE")
                                    {
                                        //txtSalesParty.BackColor = Color.IndianRed;
                                        MessageBox.Show("This Account is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        txtSubParty.Text = "SELF";
                                        txtSubParty.Focus();
                                        tStatus = false;
                                    }
                                }
                                if (Convert.ToString(dt.Rows[0]["Category"]) == "CASH PARTY" || Convert.ToString(dt.Rows[0]["TINNumber"]) == "CASH PARTY")
                                    pnlCash.Visible = true;
                                else
                                    pnlCash.Visible = false;
                            }
                            if (tStatus)
                                BindPendingOrderWithGrid(ds.Tables[1]);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void BindPendingOrderWithGrid(DataTable dt)
        {
            int rowIndex = 0;
            dgrdPending.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                dgrdPending.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdPending.Rows[rowIndex].Cells["chk"].Value = false;
                    dgrdPending.Rows[rowIndex].Cells["id"].Value = row["ID"];
                    dgrdPending.Rows[rowIndex].Cells["date"].Value = row["Date"];
                    dgrdPending.Rows[rowIndex].Cells["order"].Value = row["FullOrder"];
                    dgrdPending.Rows[rowIndex].Cells["party"].Value = row["PParty"];
                    dgrdPending.Rows[rowIndex].Cells["item"].Value = row["Items"];
                    dgrdPending.Rows[rowIndex].Cells["qty"].Value = row["Quantity"];
                    dgrdPending.Rows[rowIndex].Cells["amount"].Value = Convert.ToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                    dgrdPending.Rows[rowIndex].Cells["pcsType"].Value = row["Pieces"];
                    dgrdPending.Rows[rowIndex].Cells["oRemark"].Value = row["Remark"];
                    dgrdPending.Rows[rowIndex].Cells["oSalesParty"].Value = row["SParty"];
                    dgrdPending.Rows[rowIndex].Cells["oSubParty"].Value = row["SubPartyID"];
                    dgrdPending.Rows[rowIndex].Cells["oCategory"].Value = row["OrderCategory"];

                    if (Convert.ToString(row["SchemeName"]) == "TOUR JAN-2019")
                        dgrdPending.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Plum;
                    else if (Convert.ToString(row["SchemeName"]) != "")
                        dgrdPending.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightSalmon;
                    if (Convert.ToString(row["Remark"]).Contains("HOLD"))
                        dgrdPending.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;

                    if (Convert.ToString(row["OrderCategory"]) == "**")
                        dgrdPending.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
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
                    dgrdTax.Rows[rowIndex].Cells["taxType"].Value = row["TaxType"];

                    rowIndex++;
                }
            }
        }

        private void dgrdPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 0)
                    {
                        bool chkCheckStatus = Convert.ToBoolean(dgrdPending.CurrentCell.Value);
                        foreach (DataGridViewRow row in dgrdPending.Rows)
                            row.Cells["chk"].Value = false;

                        dgrdPending.CurrentCell.Value = chkCheckStatus;
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        ShowOrderBookingPage();
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowOrderBookingPage()
        {
            string strOrderNo = Convert.ToString(dgrdPending.CurrentRow.Cells["id"].Value);
            string[] strOrder = strOrderNo.Split(' ');
            if (strOrder.Length > 1)
            {
                if (strOrder[0] != "" && strOrder[1] != "")
                {
                    ShowOrderDetails(strOrder[0], strOrder[1]);
                }
            }
        }

        private void ShowOrderDetails(string strSerialCode, string strSerialNo)
        {
            try
            {
                OrderBooking objOrderBooking = new OrderBooking(strSerialCode, strSerialNo);
                objOrderBooking.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objOrderBooking.ShowInTaskbar = true;
                objOrderBooking.Show();
            }
            catch { }
        }

        private void btnPAdd_Click(object sender, EventArgs e)
        {
            try
            {
                bool aStatus = true;
                if (btnEdit.Text == "&Update" && strFullOrderNo != "" && lblID.Text == "")
                {
                    DialogResult result = MessageBox.Show("An Order No is already adjusted with this goods receipt !! Are you still want to adjust new order no ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.No)
                        aStatus = false;
                }
                if (aStatus)
                {
                    foreach (DataGridViewRow row in dgrdPending.Rows)
                    {
                        if (Convert.ToBoolean(row.Cells["chk"].Value))
                        {
                            BindGRPendingData(row);
                            row.Visible = false;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void ReverseEntry()
        {
            if (lblID.Text != "")
            {
                _strPcsType = _strCustomerName = _strSubPartyName = _strSupplierName = "";

                foreach (DataGridViewRow row in dgrdPending.Rows)
                {
                    if (lblID.Text == Convert.ToString(row.Cells["order"].Value))
                    {
                        row.Visible = true;
                        lblID.Text = "";
                        break;
                    }
                }
            }
        }

        private bool BindGRPendingData(DataGridViewRow row)
        {
            if (row != null)
            {
                string _strOrderNo = Convert.ToString(row.Cells["order"].Value);
                if (CheckPartyAdjustmentWithOrder(_strOrderNo))
                {
                    ReverseEntry();
                    lblID.Text = txtOrderNo.Text = _strOrderNo;
                    txtOrderDate.Text = Convert.ToString(row.Cells["Date"].Value);
                    _strSupplierName = Convert.ToString(row.Cells["party"].Value);
                    _strPcsType = Convert.ToString(row.Cells["pcsType"].Value);
                    _strCustomerName = Convert.ToString(row.Cells["oSalesParty"].Value);
                    _strSubPartyName = Convert.ToString(row.Cells["oSubParty"].Value);

                    CalculateSpecialDiscountAmt();
                }
            }
            return true;
        }

        private bool CheckGrossAmt()
        {
            double dAmt = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                dAmt += dba.ConvertObjectToDouble(row.Cells["gAmount"].Value) + dba.ConvertObjectToDouble(row.Cells["gPacking"].Value) + dba.ConvertObjectToDouble(row.Cells["gFreight"].Value);
                if (dAmt > 0)
                    break;
            }

            if (dAmt > 0 && dba.ConvertObjectToDouble(lblGrossAmt.Text) == 0)
            {
                MessageBox.Show("Sorry ! Gross Amt can't be blank !!", "Gross Amt required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.Focus();
                return false;
            }

            if (dba.ConvertObjectToDouble(txtOtherAmt.Text) > 5)
            {
                MessageBox.Show("Sorry ! Other Amt can't be more than 5.00 !!", "Other amt exceed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                {
                    txtOtherAmt.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Receipt code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Receipt no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors can't be blank !!", "Sundry Debtors required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (txtSubParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sub party can't be blank !!", "Sub party required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! SUNDRY CREDITOR can't be blank !!", "SUNDRY CREDITOR required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }
            if (txtPurchaseInvoiceNo.Text == "")
            {
                MessageBox.Show("Sorry ! Purchase Invoice no can't be blank !!", "SUNDRY CREDITOR required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseInvoiceNo.Focus();
                return false;
            }
            if (txtPurchaseDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid purchase date  !!", "Purchase date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseDate.Focus();
                return false;
            }
            if (MainPage._bTaxStatus)
            {
                if (txtPurchaseType.Text == "")
                {
                    MessageBox.Show("Sorry ! Purchase Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseType.Focus();
                    return false;
                }
                if (dba.ConvertObjectToDouble(txtTaxAmt.Text) == 0)
                {
                    MessageBox.Show("Sorry ! Tax Amt can't be zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTaxPer.Focus();
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }

            if (!rdoSummary.Checked && txtOrderNo.Text == "" && btnAdd.Text == "&Save")//&& !rdoPacked.Checked 
            {
                ShowOrderBookingDetails();
                if (!MainPage.strUserRole.Contains("ADMIN") && txtOrderNo.Text == "")
                    return false;
            }

            if (_strPcsType != "" && txtPcsType.Text != _strPcsType)
            {
                MessageBox.Show("Sorry ! Pcs type in Order and Pcs type in Purchase both are different.", "Pcs Type mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPcsType.Focus();
                return false;
            }
            if (_strCustomerName != "" && txtSalesParty.Text != _strCustomerName)
            {
                MessageBox.Show("Sorry ! Sundry Debtors in Order and Sundry Debtors in Purchase both are different.", "Sundry Debtors Name mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (_strSubPartyName != "" && !txtSubParty.Text.Contains(_strSubPartyName))
            {
                MessageBox.Show("Sorry ! Sub Party in Order and Sub Party in Purchase both are different.", "Sub Party mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }
            //if (_strSupplierName != "" && !txtPurchaseParty.Text.Contains(_strSupplierName))
            //{
            //    DialogResult result= MessageBox.Show("Sorry ! SUNDRY CREDITOR in Order and SUNDRY CREDITOR in Purchase both are different.\nAre you want to continue ? ", "SUNDRY CREDITOR mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    if (DialogResult.Yes != result)
            //    {
            //        txtPurchaseParty.Focus();
            //        return false;
            //    }
            //}
            if (txtPcsType.Text == "PETI" && (txtNoOfCase.Text == "" || txtNoOfCase.Text == "0"))
            {
                MessageBox.Show("Sorry ! No of case can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNoOfCase.Focus();
                return false;
            }

            if (txtPurchaseParty.Text != "PERSONAL")
            {
                if ((rdoNormalDhara.Checked && txtNormalDhara.Text == "") || (rdoSuperNet.Checked && txtSuperNetDhara.Text == "") || (rdoPremium.Checked && txtPremiumDhara.Text == ""))
                {
                    MessageBox.Show("Sorry ! Normal/Super/Premium dhara of this party can't be blank !!", "Dhara required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseParty.Focus();
                    return false;
                }
            }
            bool _bGrossAmt = CheckGrossAmt();
            if (!_bGrossAmt)
                return false;

            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;

            if (txtDate.Text.Length == 10 && txtPurchaseDate.Text.Length == 10)
            {
                DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text), _invoiceDate = dba.ConvertDateInExactFormat(txtPurchaseDate.Text);
                if (_invoiceDate > _date)
                {
                    MessageBox.Show("Sorry ! Invoice date can't be greater than receipt date !!", "Invoice date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseDate.Focus();
                    return false;
                }
                else if (_invoiceDate < _date.AddDays(-20) && !MainPage.strUserRole.Contains("ADMIN"))
                {
                    MessageBox.Show("Sorry ! More than 20 days back invoice not allowed !!", "Invoice date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseDate.Focus();
                    return false;
                }
                if (txtOrderDate.Text.Length == 10 && btnAdd.Text == "&Save")
                {
                    DateTime _orderDate = dba.ConvertDateInExactFormat(txtOrderDate.Text);
                    if (_orderDate < _date.AddDays(-30))
                    {
                        MessageBox.Show("Sorry ! This order is more then 30 days old, Please confirm with respective Marketer/Manager. !!", "Order is too old", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //return false;
                    }
                }
            }

            double dQty = 0, dAmt = 0;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(row.Cells["itemName"].Value), strDesign = Convert.ToString(row.Cells["designName"].Value);
                dQty = dba.ConvertObjectToDouble(row.Cells["gQty"].Value);
                dAmt = dba.ConvertObjectToDouble(row.Cells["gAmount"].Value);
                if (dQty == 0 && dAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter Item Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["itemName"];
                        dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                        dgrdDetails.Focus();
                        return false;
                    }
                    if (strDesign == "")
                    {
                        MessageBox.Show("Sorry ! Design name can't be blank", "Enter design name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["designName"];
                        dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                        dgrdDetails.Focus();
                        return false;
                    }

                    if (strDesign == "PACKING" || strDesign == "FREIGHT" || strDesign == "TAX")
                        row.Cells["designName"].Value = strDesign + " CHARGES";

                    //if (dQty == 0)
                    //{
                    //    MessageBox.Show("Sorry ! Quantity can't be blank", "Enter qty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.CurrentCell = row.Cells["gQty"];
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}
                    if (dAmt == 0 && txtPurchaseParty.Text != "PERSONAL")
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Amt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["gAmount"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add();
                dgrdDetails.Rows[0].Cells["sno"].Value = "1.";
                MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                dgrdDetails.Focus();
                return false;
            }

            if (btnEdit.Text == "&Update" && !btnPrint.Enabled && MainPage.strUserRole != "SUPERADMIN")
            {
                MessageBox.Show("Sorry ! This bill is linked with other bill plz remove from there !!", "Linked with other bill", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return ValidateOtherValidation(false);
        }

        private bool ValidateFromPreviousBill(bool _bStatus)
        {
            string strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "";
            string[] strFullName = txtSalesParty.Text.Split(' ');
            if (strFullName.Length > 1)
                strSalePartyID = strFullName[0].Trim();

            strFullName = txtSubParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strSubPartyID = strFullName[0].Trim();

            if (txtPurchaseParty.Text != "PERSONAL")
            {
                strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 0)
                    strPurchasePartyID = strFullName[0].Trim();
            }

            //double dAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);

            string strQuery = "Select ReceiptNo from GoodsReceive Where ReceiptNo!=" + txtBillNo.Text + " and PurchasePartyID='" + strPurchasePartyID + "' and LTRIM(RTRIM(InvoiceNo)) Like('" + txtPurchaseInvoiceNo.Text.Trim() + "') ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (Convert.ToString(objValue) != "")
            {
                MessageBox.Show("Sorry ! This detail might be saved in receipt no : " + objValue + " ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //{
                //    GoodscumPurchase objGoodsReciept = new GoodscumPurchase(txtBillCode.Text, Convert.ToString(objValue));
                //    objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                //    objGoodsReciept.ShowDialog();
                //}
                if (_bStatus)
                    txtPurchaseInvoiceNo.Focus();
                return false;
            }
            else
                return true;
        }

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            string strOrderQuery = "", strQuery = "";
            if (txtOrderNo.Text != "" && txtBillCode.Text.Contains("MTB"))
            {
                double dQty = Convert.ToDouble(lblQty.Text);
                strOrderQuery = " Select SUM((CAST(Quantity as Money)+(AdjustedQty-(ISNULL(_OBP.Qty,0)+CancelQty+" + dQty + ")))) Qty from OrderBooking OB OUTER APPLY (Select (CAST(Quantity as Money))Qty from GoodsReceive Where OrderNo=LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))) _OBP Where Status='PENDING' and ((CAST(Quantity as Money)+(AdjustedQty-(ISNULL(_OBP.Qty,0)+CancelQty+" + dQty + ")))*100)/CAST(Quantity as Money)<=10 and LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))='" + txtOrderNo.Text + "' ";
            }
            else
                strOrderQuery = "0";

            strQuery = "Select TransactionLock,State as SStateName,(" + strOrderQuery + ") OQty, (Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtPurchaseType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select TOP 1 UPPER(Tick) from BalanceAmount Where AccountStatus='PURCHASE A/C' and Description Like(CS.PBillCode+' " + txtBillNo.Text + "')),'FALSE') TickStatus,CS.PBillCode,Other as SSSName,(Select Top 1 SM.Other as SSSOName from SupplierMaster SM Where GroupName='SUNDRY CREDITOR' and (AreaCode+AccountNo+' '+Name)='" + _strSupplierName + "')SSSOName from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "' ";
            DataTable dt = dba.GetDataTable(strQuery);
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtPurchaseParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (_strSupplierName != "" && Convert.ToString(dt.Rows[0]["SSSName"]) != Convert.ToString(dt.Rows[0]["SSSOName"]))
                {
                    MessageBox.Show("Sorry ! Both Supplier names doesn't match, Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (txtBillCode.Text.Contains("MTB"))
                {
                    double dOQty = dba.ConvertObjectToDouble(dt.Rows[0]["OQty"]);
                    if (dOQty > 0)
                    {
                        DialogResult _DResult = MessageBox.Show(dOQty + " qty still pending in this Order,\nAre you want to cancel " + dOQty + " qty from order ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (_DResult == DialogResult.Yes)
                            _dCancelQty_ByUser = dOQty;
                    }
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

                    if (txtSaleBillNo.Text != "" && MainPage.strOnlineDataBaseName != "")
                    {
                        bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtSaleBillNo.Text);
                        if (!netStatus)
                        {
                            //MessageBox.Show("Sorry ! This Sale bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from (Select ISNULL(Max(BillNo)+1,1)BillNo from PurchaseBook where BillCode='" + txtBillCode.Text + "' UNION ALL Select ISNULL(Max(ReceiptNo)+1,1)BillNo from GoodsReceive where ReceiptCode='" + txtBillCode.Text + "')_Sales "));
                            //  MessageBox.Show("Sorry ! This Receipt No is already Exist ! you are Late,  Receipt Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Text = strBillNo;
                            // chkStatus = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Receipt No is already in used please Choose Different Receipt No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Receipt No can't be blank  ..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }

        private string GetPackingStatus()
        {
            if (rdoDirect.Checked)
                return "DIRECT";
            else if (rdoPacked.Checked)
                return "PACKED";
            else if (rdoCameOffice.Checked)
                return "CAMEOFFICE";
            else
                return "SUMMARY";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                _dCancelQty_ByUser = 0;
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                    }
                    btnEdit.Text = "&Edit";

                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();
                    EnableForAdding();
                    txtSalesParty.Focus();
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;

                }
                else if (ValidateFromPreviousBill(true) && ValidateControls() && CheckBillNoAndSuggest())
                {
                    // DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    // if (result == DialogResult.Yes)
                    if (FinalConfirmation())
                    {
                        SaveRecord();
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strODate = "NULL", strNCode = "", strDate = "", strPurchaseDate = "";
                DateTime rDate = dba.ConvertDateInExactFormat(txtDate.Text), oDate = rDate;
                strDate = rDate.ToString("MM/dd/yyyy hh:mm:ss");
                strPurchaseDate = dba.ConvertDateInExactFormat(txtPurchaseDate.Text).ToString("MM/dd/yyyy hh:mm:ss");

                if (txtOrderNo.Text != "" && txtOrderDate.Text.Length == 10)
                {
                    oDate = dba.ConvertDateInExactFormat(txtOrderDate.Text);
                    strODate = "'" + oDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strPerosnal = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "", strTaxAccountID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                if (txtPurchaseParty.Text == "PERSONAL")
                    strPerosnal = "PERSONAL";
                else
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                    }
                }
                if (txtBox.Text == "")
                    txtBox.Text = "0";
                double dGrossRate = 0, dNetRate = 0, dAmt = 0, dTAmt = 0, dQty = 0, dTQty = 0, dCut = 0, dMtr = 0, dFold = 0, dPacking = 0, dTPackingAmt = 0, dFreightAmt = 0, dTFreightAmt = 0, dTaxAmt = 0, dTTaxAmt = 0, dPcsAmt = dba.ConvertObjectToDouble(txtPcsAmt.Text), dTcsPer = 0, dTCSAmt = 0;
                double dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text), dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisPer = dba.ConvertObjectToDouble(txtDisPer.Text), dDisAmt = dba.ConvertObjectToDouble(txtDiscountAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text), dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text), dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmt.Text), dSpclDiscPer = dba.ConvertObjectToDouble(txtSpeDiscPer.Text), dSpclDiscAmt = dba.ConvertObjectToDouble(txtSpecialDiscAmt.Text), dNoOfCase = dba.ConvertObjectToDouble(txtNoOfCase.Text);
                string strQuery = "", strInnerQuery = "", strPcsType = txtPcsType.Text, strItemName = "", strDhara = "NORMAL", strHSNCode = "", strDesignName = "", strArticleName = "";
                if (rdoSuperNet.Checked)
                    strDhara = "SUPER";
                else if (rdoPremium.Checked)
                    strDhara = "PREMIUM";

                dTTaxAmt = dba.ConvertObjectToDouble(txtTaxFree.Text);
                dTPackingAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text);
                dTFreightAmt = dba.ConvertObjectToDouble(txtFreight.Text);
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);
                }
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dGrossRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                    dNetRate = dba.ConvertObjectToDouble(rows.Cells["gRate"].Value);
                    dTQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    dCut = dba.ConvertObjectToDouble(rows.Cells["cut"].Value);
                    dMtr = dba.ConvertObjectToDouble(rows.Cells["mtr"].Value);
                    dFold = dba.ConvertObjectToDouble(rows.Cells["fold"].Value);

                    strArticleName = Convert.ToString(rows.Cells["itemName"].Value);
                    strDesignName = Convert.ToString(rows.Cells["designName"].Value).Trim();

                    //dTPackingAmt += dPacking = dba.ConvertObjectToDouble(rows.Cells["gPacking"].Value);
                    //dTFreightAmt += dFreightAmt = dba.ConvertObjectToDouble(rows.Cells["gFreight"].Value);
                    //dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(rows.Cells["gTax"].Value);

                    if (strItemName == "")
                    {
                        dPacking = dTPackingAmt;
                        dFreightAmt = dTFreightAmt;
                        dTaxAmt = dTTaxAmt;
                    }
                    else
                        dTaxAmt = dPacking = dFreightAmt = 0;

                    if (strItemName.Length < 150)
                    {
                        if (strItemName != "")
                            strItemName += ",";
                        strItemName += strArticleName;
                    }

                    if (strArticleName != "")
                    {
                        strHSNCode = strArticleName.Substring(strArticleName.Length - 5, 5).Replace(":", "").Trim();
                        strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                    }
                    else
                        strHSNCode = "";

                    strInnerQuery += " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus],[Rate],[GRate],[DesignName],[Cut],[MTR],[Fold]) VALUES "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strArticleName + "','" + strPcsType + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxAmt + " ,1,0," + dNetRate + "," + dGrossRate + ",'" + strDesignName + "'," + dCut + "," + dMtr + "," + dFold + ")  ";

                    if (_strPDFFilePath != "" && strHSNCode != "")
                    {
                        strInnerQuery += " if not exists(Select ItemName from[dbo].[ItemMapping]  Where ItemName = '" + strArticleName + "' and DesignName = '" + strDesignName + "' and UpdatedBy = '" + strHSNCode + "' ) begin "
                                      + " INSERT INTO [dbo].[ItemMapping] ([ItemName],[DesignName],[Date],[CreatedBy],[UpdatedBy]) Values ('" + strArticleName + "','" + strDesignName + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "','" + strHSNCode + "') end";
                    }
                }

                strQuery += "if not exists (Select ReceiptCode from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " UNION ALL Select BillCode from PurchaseBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[GoodsReceive] ([ReceiptCode],[ReceiptNo],[OrderNo],[OrderDate],[SalesParty],[SubSalesParty],[PurchaseParty],[ReceivingDate],[Pieces],[Quantity],[Amount],[Freight],[Tax],[Packing],[Item],[Personal],[SaleBill],[PackingStatus],[CreatedBy],[PrintedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[Box],[Remark],[SalePartyID],[SubPartyID],[PurchasePartyID],[InvoiceNo],[InvoiceDate],[PurchaseType],[ReverseCharge],[Dhara],[GrossAmount],[OtherSign],[OtherAmount],[DisPer],[DisAmount],[TaxPer],[TaxAmount],[NetAmount],[PurchaseStatus],[SpecialDscPer],[SpecialDscAmt],[PcsRateAmt],[NoOfCase],[TCSPer],[TCSAmt],[TaxableAmt],[RoundOffSign],[RoundOffAmt]) Values "
                               + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtOrderNo.Text + "'," + strODate + ",'" + strSaleParty + "','" + strSubParty + "','" + strPurchaseParty + "','" + rDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strPcsType + "','" + dTQty + "'," + dTAmt + ",'" + dTFreightAmt + "', '" + dTTaxAmt + "','" + dTPackingAmt + "','" + strItemName + "','" + strPerosnal + "','PENDING',"
                               + " '" + GetPackingStatus() + "','" + MainPage.strLoginName + "','','',1,0," + txtBox.Text + ",'" + txtRemark.Text + "','" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "','" + txtPurchaseInvoiceNo.Text.Trim() + "','" + strPurchaseDate + "','" + txtPurchaseType.Text + "','NOT APPLICABLE','" + strDhara + "'," + dGrossAmt + ",'" + txtSignAmt.Text + "'," + dOtherAmt + "," + dDisPer + "," + dDisAmt + "," + dTaxPer + "," + dPTaxAmt + "," + dNetAmt + ",1," + dSpclDiscPer + "," + dSpclDiscAmt + "," + dPcsAmt + "," + dNoOfCase + "," + dTcsPer + "," + dTCSAmt + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ")  "
                               + strInnerQuery;


                //Purchase Entry

                strQuery += " Declare @TCSAccount nvarchar(250),@Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ,@DisStatus nvarchar(20),@DisPer float; "
                         + " Select TOP 1 @BillCode = PBillCode from CompanySetting  Select @DisPer = ((" + dDisPer + " * -1) + (CASE WHEN (Category = 'CASH PURCHASE' OR TINNumber = 'CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (AreaCode Like('SRT%') OR AreaCode Like('CCK%') OR Category = 'CLOTH PURCHASE') then 1 else 0 end)) from SupplierMaster Where (AreaCode + AccountNo) = '" + strPurchasePartyID + "' "
                         + " Set @DisStatus = '+'; if (@DisPer < 0) begin Set @DisStatus = '-'; end Set @DisPer = ABS(@DisPer); ";


                strQuery += " if not exists (Select * from [PurchaseRecord] Where [BillCode]=@BillCode and [BillNo]=" + txtBillNo.Text + " ) begin "
                              + " INSERT INTO [dbo].[PurchaseRecord] ([BillCode],[BillNo],[GRSNo],[DueDays],[SupplierName],[SaleBillNo],[SalesParty],[Pieces],[Item],[Discount],[DiscountStatus],[Amount],[Freight],[Tax],[Packing],[FreightDiscount],[TaxDiscount],[PackingDiscount],[NetDiscount],[Remark],[OtherPer],[Others],[GrossAmt],[NetAmt],[BillDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[PurchasePartyID],[TaxLedger],[TaxAmount],[TaxPer],[ReverseCharge],[Dhara],[InvoiceNo],[InvoiceDate],[CheckStatus],[CheckedBy],[PurchaseSource],[SpecialDscPer],[SpecialDscAmt],[PcsRateAmt],[TCSPer],[TCSAmt],[TaxableAmt],[RoundOffSign],[RoundOffAmt]) VALUES "
                              + " (@BillCode," + txtBillNo.Text + ",'" + txtBillCode.Text + " " + txtBillNo.Text + "','30','" + strPurchaseParty + "','','" + strSaleParty + "','" + dTQty + "','" + strItemName + "',@DisPer,@DisStatus,'" + dTAmt + "'," + dTFreightAmt + "," + dTTaxAmt + "," + dTPackingAmt + ","
                              + " '0','0','0'," + dDisAmt + ",'" + txtRemark.Text + "','0','" + txtSignAmt.Text + txtOtherAmt.Text + "','" + lblGrossAmt.Text + "','" + lblNetAmt.Text + "','" + strDate + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strPurchasePartyID + "','" + txtPurchaseType.Text + "'," + dPTaxAmt + "," + dTaxPer + ",'NOT APPLICABLE','" + strDhara + "','" + txtPurchaseInvoiceNo.Text.Trim() + "','" + strPurchaseDate + "',1,'" + MainPage.strLoginName + "','DIRECT'," + dSpclDiscPer + "," + dSpclDiscAmt + "," + dPcsAmt + "," + dTcsPer + "," + dTCSAmt + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + ") "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT',@BillCode+' " + txtBillNo.Text + "','" + lblNetAmt.Text + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "') end ";


                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end ";


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

                    strQuery += " end ";
                }
                //Purchase End

                if (dTCSAmt > 0)
                {
                    strQuery += " Select Top 1  @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }

                if (txtOrderNo.Text != "")
                {
                    string[] strOrderNo = txtOrderNo.Text.Split(' ');
                    if (strOrderNo.Length > 2)
                        strNCode = strOrderNo[2];

                    if (txtPcsType.Text == "PETI")
                        dTQty = dNoOfCase;
                    else if (txtPcsType.Text == "LOOSE" && !txtBillCode.Text.Contains("MTB"))
                        strQuery += " Update OB Set CancelQty=CancelQty+((CAST(Quantity as Money)-(AdjustedQty +CancelQty+" + (dTQty + _dCancelQty_ByUser) + "))) from OrderBooking OB Where ((CAST(Quantity as Money)-(AdjustedQty +CancelQty+" + (dTQty + _dCancelQty_ByUser) + ")))>0 and OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";

                    strQuery += " Update OrderBooking Set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + (dTQty + _dCancelQty_ByUser) + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dTQty + "),CancelQty=(CancelQty+" + _dCancelQty_ByUser + "), UpdateStatus=1 where OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                }

                string strEditStatus = "CREATION";
                if (_strPDFFilePath != "")
                    strEditStatus = "IMPORTED";
                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('GOODSPURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'" + strEditStatus + "') ";

                strQuery += " end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    CopyPDFFileInGSTFolder();

                    btnAdd.Text = "&Add";
                    MessageBox.Show("Thank You ! Record saved successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    AskForPrint(false);
                    ShowSaleBillDetails();

                    if (chkSendSMS.Checked)
                        SendSMSToParty();

                    ClearAllText();
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                _dCancelQty_ByUser = 0;
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;

                        BindLastRecord();
                        btnAdd.Text = "&Add";
                    }
                    if (btnEdit.Enabled)
                    {
                        btnEdit.Text = "&Update";
                        EnableAllControls();
                        txtBillNo.ReadOnly = true;
                        btnAdd.TabStop = false;
                        chkSendSMS.Checked = false;
                        txtDate.Focus();
                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;

                    if (ValidateControls() && ValidatePurchaseAndSaleStatus())
                    {
                        txtBillNo.Focus();
                        DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                }
            }
            catch
            {
            }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord()
        {
            try
            {
                string strODate = "NULL", strNCode = "", strDate = "", strPurchaseDate = "";
                DateTime rDate = dba.ConvertDateInExactFormat(txtDate.Text), oDate = rDate;
                strDate = rDate.ToString("MM/dd/yyyy hh:mm:ss");
                strPurchaseDate = dba.ConvertDateInExactFormat(txtPurchaseDate.Text).ToString("MM/dd/yyyy hh:mm:ss");

                if (txtOrderNo.Text != "" && txtOrderDate.Text.Length == 10)
                {
                    oDate = dba.ConvertDateInExactFormat(txtOrderDate.Text);
                    strODate = "'" + oDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string strSaleParty = "", strSubParty = "", strPurchaseParty = "", strPerosnal = "", strSalePartyID = "", strSubPartyID = "", strPurchasePartyID = "", strTaxAccountID = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSaleParty = txtSalesParty.Text.Replace(strSalePartyID + " ", "");
                }
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strSubPartyID = strFullName[0].Trim();
                    strSubParty = txtSubParty.Text.Replace(strSubPartyID + " ", "");
                }
                if (txtPurchaseParty.Text == "PERSONAL")
                    strPerosnal = "PERSONAL";
                else
                {
                    strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                    }
                }
                if (txtBox.Text == "")
                    txtBox.Text = "0";

                string strQuery = "", strPcsType = txtPcsType.Text, strItemName = "", strDhara = "NORMAL";
                strQuery = " if exists (Select [ReceiptCode] from GoodsReceive Where [ReceiptCode]='" + txtBillCode.Text + "' and [ReceiptNo]=" + txtBillNo.Text + " ) begin Delete from GoodsReceiveDetails Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ";

                double dGrossRate = 0, dNetRate = 0, dTcsPer = 0, dTCSAmt = 0, dAmt = 0, dTAmt = 0, dQty = 0, dTQty = 0, dPacking = 0, dTPackingAmt = 0, dFreightAmt = 0, dTFreightAmt = 0, dTaxAmt = 0, dTTaxAmt = 0, dpcsAmt = dba.ConvertObjectToDouble(txtPcsAmt.Text), dCut = 0, dMtr = 0, dFold = 0;
                double dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text), dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisPer = dba.ConvertObjectToDouble(txtDisPer.Text), dDisAmt = dba.ConvertObjectToDouble(txtDiscountAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text), dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text), dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmt.Text), dSpclDiscPer = dba.ConvertObjectToDouble(txtSpeDiscPer.Text), dSpclDiscAmt = dba.ConvertObjectToDouble(txtSpecialDiscAmt.Text), dNoOfCase = dba.ConvertObjectToDouble(txtNoOfCase.Text);

                if (rdoSuperNet.Checked)
                    strDhara = "SUPER";
                else if (rdoPremium.Checked)
                    strDhara = "PREMIUM";

                dTTaxAmt = dba.ConvertObjectToDouble(txtTaxFree.Text);
                dTPackingAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text);
                dTFreightAmt = dba.ConvertObjectToDouble(txtFreight.Text);
                dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                dTCSAmt = dba.ConvertObjectToDouble(txtTCSAmt.Text);

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dGrossRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                    dNetRate = dba.ConvertObjectToDouble(rows.Cells["gRate"].Value);
                    dTQty += dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);

                    dCut = dba.ConvertObjectToDouble(rows.Cells["cut"].Value);
                    dMtr = dba.ConvertObjectToDouble(rows.Cells["mtr"].Value);
                    dFold = dba.ConvertObjectToDouble(rows.Cells["fold"].Value);

                    if (strItemName == "")
                    {
                        dPacking = dTPackingAmt;
                        dFreightAmt = dTFreightAmt;
                        dTaxAmt = dTTaxAmt;
                    }
                    else
                        dTaxAmt = dPacking = dFreightAmt = 0;

                    if (strItemName.Length < 200)
                    {
                        if (strItemName != "")
                            strItemName += ",";
                        strItemName += Convert.ToString(rows.Cells["itemName"].Value);
                    }

                    strQuery += " INSERT INTO [dbo].[GoodsReceiveDetails]([ReceiptCode],[ReceiptNo],[ItemName],[PcsType],[Quantity],[Amount],[PackingAmt],[FreightAmt],[TaxAmt],[InsertStatus],[UpdateStatus],[Rate],[GRate],[DesignName],[Cut],[MTR],[Fold]) VALUES "
                             + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + rows.Cells["itemName"].Value + "','" + strPcsType + "'," + dQty + "," + dAmt + "," + dPacking + "," + dFreightAmt + " ," + dTaxAmt + " ,1,0," + dNetRate + "," + dGrossRate + ",'" + Convert.ToString(rows.Cells["designName"].Value).Trim() + "'," + dCut + "," + dMtr + "," + dFold + ")  ";// end ";
                }

                if (oDate > Convert.ToDateTime("09/13/2019") && !txtBillCode.Text.Contains("MTB"))
                    strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)- GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-GR.Qty),UpdateStatus=1 from OrderBooking OB Cross Apply (Select ReceiptCode,ReceiptNo,GR.OrderNo,(CASE WHEN (Pieces='PETI' and GR.OrderDate>'09/13/2019') Then GR.NoofCase else CAST(GR.Quantity as Money) end) Qty  from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo) GR Where GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + txtBillNo.Text + " and GR.OrderNo!='' ";
                else
                    strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(CancelQty,0)- GR.Qty))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-GR.Qty),UpdateStatus=1 from OrderBooking OB Cross Apply (Select ReceiptCode,ReceiptNo,GR.OrderNo,CAST(GR.Quantity as Money) Qty  from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo) GR Where GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + txtBillNo.Text + " and GR.OrderNo!='' ";

                strQuery += " UPDATE [dbo].[GoodsReceive] SET [OrderNo]='" + txtOrderNo.Text + "',[OrderDate]=" + strODate + ",[SalesParty]='" + strSaleParty + "',[SubSalesParty]='" + strSubParty + "',[PurchaseParty]='" + strPurchaseParty + "',[ReceivingDate]='" + strDate + "',[Pieces]='" + strPcsType + "',[Quantity]='" + dTQty + "', [Amount]=" + dTAmt + ", [InvoiceNo]='" + txtPurchaseInvoiceNo.Text + "',[InvoiceDate]='" + strPurchaseDate + "',[PurchaseType]='" + txtPurchaseType.Text + "',[ReverseCharge]='NOT APPLICABLE',[Dhara]='" + strDhara + "',[GrossAmount]=" + dGrossAmt + ",[OtherSign]='" + txtSignAmt.Text + "',[OtherAmount]=" + dOtherAmt + ",[SpecialDscPer]=" + dSpclDiscPer + ",[SpecialDscAmt]=" + dSpclDiscAmt + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ","
                          + " [DisPer]=" + dDisPer + ",[DisAmount]=" + dDisAmt + ",[TaxPer]=" + dTaxPer + ",[TaxAmount]=" + dPTaxAmt + ",[NetAmount]=" + dNetAmt + ",[Freight]='" + dTFreightAmt + "',[Tax]='" + dTTaxAmt + "',[Packing]='" + dTPackingAmt + "',[Item]='" + strItemName + "',[Personal]='" + strPerosnal + "',[PackingStatus]='" + GetPackingStatus() + "',[Box]=" + txtBox.Text + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Remark]='" + txtRemark.Text + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "',[PcsRateAmt]=" + dpcsAmt + ",[NoOfCase]=" + dNoOfCase + ",[TCSPer]=" + dTcsPer + ",[TCSAmt]=" + dTCSAmt + " Where [ReceiptCode]='" + txtBillCode.Text + "' and [ReceiptNo]=" + txtBillNo.Text + " ";

                if (strFullOrderNo != "" || (txtOrderNo.Text != strFullOrderNo))
                {
                    //strQuery += " Update OrderBooking Set Status='PENDING' Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end) in (Select OrderNo from GoodsReceive Where OrderNo!='' and ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + ") ";
                    if (txtOrderNo.Text != "")
                    {
                        string[] strOrderNo = txtOrderNo.Text.Split(' ');
                        if (strOrderNo.Length > 2)
                            strNCode = strOrderNo[2];
                        // if (oDate > Convert.ToDateTime("09/13/2019"))
                        {
                            if (txtPcsType.Text == "PETI")//&& dNoOfCase > 0
                                dTQty = dNoOfCase;
                            else if (txtPcsType.Text == "LOOSE" && !txtBillCode.Text.Contains("MTB"))
                                strQuery += " Update OB Set CancelQty=CancelQty+((CAST(Quantity as Money)-(AdjustedQty +CancelQty+" + (dTQty + _dCancelQty_ByUser) + "))) from OrderBooking OB Where ((CAST(Quantity as Money)-(AdjustedQty +CancelQty+" + (dTQty + _dCancelQty_ByUser) + ")))>0 and OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                        }
                        strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + (dTQty + _dCancelQty_ByUser) + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dTQty + "),CancelQty=(CancelQty+" + _dCancelQty_ByUser + "), UpdateStatus=1 where OrderNo=" + strOrderNo[1] + " and OrderCode='" + strOrderNo[0] + "' and NumberCode='" + strNCode + "' ";
                    }
                }

                //Purchase Entry

                strQuery += " Declare @TCSAccount nvarchar(250),@Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ,@DisStatus nvarchar(20),@DisPer float; "
                         + " Select TOP 1 @BillCode = PBillCode from CompanySetting  Select @DisPer = ((" + dDisPer + " * -1) + (CASE WHEN (Category = 'CASH PURCHASE' OR TINNumber='CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (Category = 'CLOTH PURCHASE' OR '" + txtBillCode.Text + "' Like('%SRT%') OR '" + txtBillCode.Text + "' Like('%CCK%')) then 1 else 0 end)) from SupplierMaster Where (AreaCode + AccountNo) = '" + strPurchasePartyID + "' "
                         + " Set @DisStatus = '+'; if (@DisPer < 0) begin Set @DisStatus = '-'; end Set @DisPer = ABS(@DisPer); ";

                strQuery += " if exists (Select [BillCode] from [PurchaseRecord] Where [BillCode]=@BillCode and [BillNo]=" + txtBillNo.Text + " ) begin "
                        + " UPDATE [dbo].[PurchaseRecord] SET [SupplierName]='" + strPurchaseParty + "',[SalesParty]='" + strSaleParty + "',[Pieces]=" + dTQty + ",[Item]='" + strItemName + "',"
                        + " [Discount]=@DisPer,[DiscountStatus]=@DisStatus,[Amount]='" + dTAmt + "',[Freight]='" + dTFreightAmt + "',[Tax]='" + dTTaxAmt + "',[Packing]='" + dTPackingAmt + "',"
                        + " [NetDiscount]='" + dDisAmt + "',[Remark]='" + txtRemark.Text + "',[Others]='" + txtSignAmt.Text + txtOtherAmt.Text + "',[GrossAmt]=" + dGrossAmt + ",[ReverseCharge]='NOT APPLICABLE',[InvoiceNo]='" + txtPurchaseInvoiceNo.Text + "',[InvoiceDate]='" + strPurchaseDate + "',[Dhara]='" + strDhara + "',[SpecialDscPer]=" + dSpclDiscPer + ",[SpecialDscAmt]=" + dSpclDiscAmt + ",[PcsRateAmt]=" + dpcsAmt + ","
                        + " [NetAmt]='" + dNetAmt + "',[BillDate]='" + strDate + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SalePartyID]='" + strSalePartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "',[TaxLedger]='" + txtPurchaseType.Text + "',[TaxAmount]=" + dPTaxAmt + ",[TaxPer]=" + dTaxPer + ",[TCSPer]=" + dTcsPer + ",[TCSAmt]=" + dTCSAmt + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + " Where [BillCode]=@BillCode AND [BillNo]=" + txtBillNo.Text + " AND [GRSNo]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                        + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strPurchaseParty + "',[Amount]='" + dNetAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strPurchasePartyID + "' Where [AccountStatus]='PURCHASE A/C' AND [Description]=@BillCode+' " + txtBillNo.Text + "' "
                        + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]=@BillCode+' " + txtBillNo.Text + "'  "
                        + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += "  Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                                 + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                 + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                 + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                 + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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

                strQuery += " end ";

                if (dTCSAmt > 0)
                {
                    strQuery += "Select @TCSAccount=(AreaCode+AccountNo) from SupplierMaster Where GroupName='SHORT-TERM LOANS AND ADVANCES' and Category='TCS RECEIVABLES' "
                             + " INSERT INTO[dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES   "
                             + " ('" + strDate + "',@TCSAccount,'DUTIES & TAXES','DEBIT',@BillCode+' " + txtBillNo.Text + "'," + dTCSAmt + ",'DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@TCSAccount) ";
                }


                // end Purchase Entry
                //object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                           + "('GOODSPURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery += " end ";
                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    //if (!Convert.ToBoolean(objValue))
                    //{
                    //    strQuery = strQuery.Replace("Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;", "");
                    //    DataBaseAccess.CreateDeleteQuery(strQuery);
                    //}

                    MessageBox.Show("Thank You ! Record updated successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    if (chkSendSMS.Checked)
                        SendSMSToParty();
                    btnEdit.Text = "&Edit";
                    updateStatus = true;
                    if (saleStatus)
                        this.Close();
                    else
                    {
                        AskForPrint(true);
                        ClearAllText();
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Updating Record in Goods Received", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnOrderClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOrderNo.Text != "" && (btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                {
                    btnOrderClear.Enabled = false;
                    if (CheckPartyAdjustmentWithOrder(txtOrderNo.Text))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to clear this order no ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            ClearOrderFilledData();
                        }
                    }
                }
            }
            catch
            {
            }
            btnOrderClear.Enabled = true;
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

        private void txtBox_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0")
                        txt.Text = "";
                }
            }
        }

        private void txtBox_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0";
                }
            }
        }

        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                            txtPurchaseParty.Text = strData;
                    }
                    else
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
                            string strData = objSearch.strSelectedData, strGSTNo = "";
                            if (strData != "")
                            {
                                bool _blackListed = false, _bOrangeList = false;
                                if (dba.CheckTransactionLockWithBlackList_OrangeList(strData, ref _blackListed, ref _bOrangeList))
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
                                else if (_bOrangeList)
                                {
                                    MessageBox.Show("This Account is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    txtPurchaseParty.Text = "";
                                    txtGSTNo.Text = "";
                                }
                                else
                                {
                                    txtPurchaseParty.Text = strData;
                                    txtGSTNo.Text = strGSTNo;
                                    GetPartyDhara();
                                }
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

        private void txtPiecesType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {

                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;

                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (txtOrderNo.Text == "")
                        {
                            SearchData objSearch = new SearchData("PIECESTYPE", "SEARCH PIECES TYPE", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                                txtPcsType.Text = strData;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Please remove the order which is associated with this purchase. After than you can change the Pcs Type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch
            {
            }
        }


        private void ClearOrderFilledData()
        {
            if (lblID.Text != "0" && lblID.Text != "")
            {
                ReverseEntry();

            }
            txtOrderNo.Text = txtOrderDate.Text = "";
        }

        private void CheckAvailability()
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (txtBillNo.Text != "")
                    {
                        object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(GoodsReceiveNo),0) from MaxSerialNo");
                        int maxBillNo = Convert.ToInt32(objMax);
                        if (maxBillNo < Convert.ToInt32(txtBillNo.Text))
                        {
                            int check = dba.CheckGoodsReceiptAvailability(txtBillCode.Text, txtBillNo.Text);
                            if (check < 1)
                            {
                                lblMsg.Text = txtBillNo.Text + "  Receipt No is Available ........";
                                lblMsg.ForeColor = Color.White;
                                lblMsg.Visible = true;
                            }
                            else
                            {
                                lblMsg.Text = txtBillNo.Text + " Receipt No is already exist ! ";
                                lblMsg.ForeColor = Color.White;
                                lblMsg.Visible = true;
                                //txtBillNo.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Receipt No is already in used please Choose Different Receipt No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Focus();

                        }
                    }
                    else
                    {
                        lblMsg.Text = "Please Choose Receipt Number .......";
                        lblMsg.ForeColor = Color.White;
                        lblMsg.Visible = true;
                        txtBillNo.Focus();

                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability in Goods Receive ", ex.Message };
                dba.CreateErrorReports(strReport);
                txtBillNo.Focus();
            }
        }
        private void txtReceiptNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        CheckAvailability();
                    }
                    else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
                else
                {
                    txtBillNo.Focus();
                }
            }
            catch
            {
            }
        }

        private void GoodsReciept_Load(object sender, EventArgs e)
        {
            try
            {
                EditOption();
                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtBillNo.Focus();
                }
                else
                {
                    //byte[] b = Convert.FromBase64String(MainPage.__strLoginName);
                    //string str = System.Text.ASCIIEncoding.ASCII.GetString(b);
                    if (MainPage.mymainObject.bFullEditControl && MainPage.mymainObject.bPurchaseEdit && MainPage.mymainObject.bAccountMasterEdit) //|| MainPage.strLoginName == str
                        btnReset.Enabled = true;
                }
            }
            catch
            {
            }

        }

        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView)
                {
                    if (!MainPage.mymainObject.bPurchaseAdd)
                        btnAdd.Enabled = btnRefresh.Enabled= btnImport.Enabled = false;
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = btnChecked.Enabled = false;
                    if (!MainPage.mymainObject.bPurchaseView)
                        txtBillNo.Focus();
                    SetColumnWidth();
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    this.Close();
                }

            }
            catch
            {
            }
        }

        private void AskForPrint(bool _bStatus)
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
                                int count = SavePrintedByName(_bStatus);
                                if (count > 0)
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

        private int SavePrintedByName(bool _bStatus)
        {
            try
            {
                string strQuery = "", strPrintedBy = "",strOrderCategory="";
                int _count = 0;
                if (_bStatus)
                {
                    DataTable dt = dba.GetDataTable("Select PrintedBy,(Select TOP 1 OrderCategory FROM OrderBooking Where RTRIM((OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode))='" + txtOrderNo.Text + "')OrderCategory from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");
                    if (dt.Rows.Count > 0)
                    {
                        strPrintedBy = Convert.ToString(dt.Rows[0]["PrintedBy"]);
                        strOrderCategory = Convert.ToString(dt.Rows[0]["OrderCategory"]);
                        if (strOrderCategory.Contains("**"))
                        {
                            MessageBox.Show("Sorry ! This order is in ** category, That's why unable to print the slip.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("ADMIN"))
                                return 0;
                        }
                    }
                }
                else
                {
                    if (txtOrderNo.Text != "")
                    {
                        object obj = DataBaseAccess.ExecuteMyScalar("Select TOP 1 OrderCategory FROM OrderBooking Where RTRIM((OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode))='" + txtOrderNo.Text + "' ");
                        strOrderCategory = Convert.ToString(obj);
                        if (strOrderCategory.Contains("**"))
                        {
                            MessageBox.Show("Sorry ! This order is in ** category, That's why unable to print the slip.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            if (!MainPage.strUserRole.Contains("ADMIN"))
                                return 0;
                        }
                    }
                }

                if (strPrintedBy == "" || MainPage.strUserRole.Contains("ADMIN") || MainPage.mymainObject.bFullEditControl)
                {
                    strQuery += " Update GoodsReceive Set PrintedBy='" + MainPage.strLoginName + "',[OtherBillReq]=0 Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " "
                             +" INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                             + "('GOODSPURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'PRINTED') ";

                    _count = dba.ExecuteMyQuery(strQuery);
                }
                else
                {
                    MessageBox.Show("Sorry ! Purchase slip already being printed by : " + strPrintedBy + ",\nPlease contact to concern deparment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return _count;
            }
            catch { }
            return 0;
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
                myDataTable.Columns.Add("TCSPer", typeof(String));
                myDataTable.Columns.Add("TCSAmt", typeof(String));
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("BrandLogo", typeof(byte[]));
                myDataTable.Columns.Add("SignatureImage", typeof(byte[]));

                DataRow row = myDataTable.NewRow();
                double dMtr = dba.ConvertObjectToDouble(lblTotalMTR.Text);

                if (MainPage.strSoftwareType == "AGENT")
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                else
                    row["CompanyName"] = "";

                if (!btnPrint.Enabled)
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text + "/D";
                else
                    row["SerialNo"] = txtBillCode.Text + " " + txtBillNo.Text;

                row["InvoiceNo"] = txtPurchaseInvoiceNo.Text; ;
                row["InvoiceDate"] = txtPurchaseDate.Text;
                row["BarValue"] = "*" + txtBillCode.Text + txtBillNo.Text + "*";

                row["SupplierHead"] = "SUPPLIER";
                row["PParty"] = txtPurchaseParty.Text;

                row["SParty"] = txtSalesParty.Text;
                row["SubParty"] = txtSubParty.Text;
                row["Date"] = txtDate.Text;

                if (dba.ConvertObjectToDouble(txtBox.Text) > 0)
                    row["Qty"] = lblQty.Text + "  Pcs (" + txtBox.Text + " Box(s))";
                else
                    row["Qty"] = lblQty.Text + " Pcs";
                if (dMtr > 0)
                    row["Qty"] = row["Qty"] + "/" + dMtr.ToString("N2", MainPage.indianCurancy) + " Mtr";
                row["Amount"] = lblGrossAmt.Text;
                row["Tax"] = txtTaxAmt.Text;
                row["Freight"] = txtDiscountAmt.Text;
                row["Packing"] = txtOtherAmt.Text;
                row["NetAmount"] = lblNetAmt.Text;
                row["Remark"] = txtRemark.Text;
                if (chkTCSAmt.Checked)
                {
                    row["TCSPer"] = "TCS (" + txtTCSPer.Text + "%)";
                    row["TCSAmt"] = txtTCSAmt.Text;
                }
                else
                {
                    row["TCSPer"] = "TCS Amt";
                    row["TCSAmt"] = "0.00";
                }
                row["UserName"] = MainPage.strLoginName + " ,  Date & Time : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                row["HeaderImage"] = MainPage._headerImage;
                row["BrandLogo"] = MainPage._brandLogo;
                row["SignatureImage"] = MainPage._signatureImage;

                myDataTable.Rows.Add(row);
            }
            catch
            {
            }
            return myDataTable;
        }

        private void CalculateSpclPerFromSpclAmount()
        {
            double dTAmt = 0, dQty = 0, dSpclAmt = dba.ConvertObjectToDouble(txtSpecialDiscAmt.Text), dSpclPer = 0;

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                if (dQty == 0)
                    dQty = dba.ConvertObjectToDouble(rows.Cells["mtr"].Value);

                dTAmt += dQty * dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
            }

            if (dTAmt != 0 && dSpclAmt != 0)
            {
                dSpclPer = (dSpclAmt * 100) / dTAmt;
            }
            txtSpeDiscPer.Text = dSpclPer.ToString("0.####");

            CalculateSpecialDiscountAmt();
        }

        private void CalculateSpecialDiscountAmt()
        {
            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                CalculateAllDetails(3, rows);
                if (Convert.ToString(rows.Cells["itemName"].Value) == "")
                    rows.DefaultCellStyle.BackColor = Color.Tomato;
            }

            CalculateTotalAmt();
        }

        private void CalculateTotalQty()
        {
            double dQty = 0;
            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                dQty += dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);

            lblQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
        }

        private void CalculateTotalAmt()
        {
            try
            {
                double dMtr = 0, dTMtr = 0, dTaxableAmt = 0, dRoundOff = 0, dAmt = 0, dFold = 0, dTAmt = 0, dTPackingAmt = 0, dTFreightAmt = 0, dTTaxAmt = 0, dQty = 0;
                int _rowIndex = 1;

                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    rows.Cells["sno"].Value = _rowIndex + ".";
                    dQty += dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);
                    dMtr = dba.ConvertObjectToDouble(rows.Cells["mtr"].Value);
                    dFold = dba.ConvertObjectToDouble(rows.Cells["fold"].Value);
                    if (dMtr > 0 && dFold > 0)
                        dMtr = ((dMtr * dFold) / 100);
                    dTMtr += dMtr;
                    dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    _rowIndex++;
                }

                dTotalAmount = dTAmt;

                dTPackingAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text);
                dTFreightAmt = dba.ConvertObjectToDouble(txtFreight.Text);
                dTTaxAmt = dba.ConvertObjectToDouble(txtTaxFree.Text);

                lblTotalMTR.Text = dTMtr.ToString("N2", MainPage.indianCurancy);
                lblQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                lblGrossAmt.Text = (dTAmt + dTPackingAmt + dTFreightAmt + dTTaxAmt).ToString("N2", MainPage.indianCurancy);

                //else
                //    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

                CalculateNetAmount();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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
                        int count = SavePrintedByName(true);
                        if (count > 0)
                        {
                            Reporting.PSNoReport objReport = new Reporting.PSNoReport();
                            objReport.SetDataSource(dt);

                            if (MainPage._PrintWithDialog)
                                dba.PrintWithDialog(objReport);
                            else
                                objReport.PrintToPrinter(1, false, 0, 0);
                            // objReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                            objReport.Close();
                            objReport.Dispose();

                            btnPrint.Enabled = true;
                        }
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
                    Reporting.ShowReport objShow = new Reporting.ShowReport("Goods Receipt Slip");
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidatePurchaseAndSaleStatus()
        {
            string strQuery = "", strSubQuery = "'PENDING'";

            if (!saleStatus)
                strSubQuery = " ISNULL((Select UPPER(SaleBill) from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + "),'PENDING') ";

            strQuery = "Select ISNULL(Count(*),0) PStatus, " + strSubQuery + " SStatus from PurchaseRecord Where PurchaseSource!='DIRECT' and GRSNO='" + txtBillCode.Text + " " + txtBillNo.Text + "' ";
            DataTable dt = dba.GetDataTable(strQuery);
            if (dt.Rows.Count > 0)
            {
                if (dba.ConvertObjectToDouble(dt.Rows[0]["PStatus"]) > 0 && txtPurchaseParty.Text != "PERSONAL")
                {
                    MessageBox.Show("Sorry ! Purchase bill has been made of this serial no, Please remove purchase bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (Convert.ToString(dt.Rows[0]["SStatus"]) == "CLEAR")
                {
                    MessageBox.Show("Sorry ! Sale bill has been made of this serial no, Please update from sales bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }
            else
                return false;

            return true;
        }

        private void SendSMSToParty()
        {
            try
            {
                string strMessage = "", strMobileNo = "", strOldMobileNo = "";
                if (btnEdit.Text == "&Update" && txtPurchaseParty.Text != strOldPartyName && strOldPartyName != "")
                {
                    DataTable dt = dba.GetDataTable("Select MobileNo,(Select MobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strOldPartyName + "') OldMobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "'");
                    if (dt.Rows.Count > 0)
                    {
                        strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                        strOldMobileNo = Convert.ToString(dt.Rows[0]["OldMobileNo"]);
                    }
                }
                else
                {
                    object objMobile = DataBaseAccess.ExecuteMyScalar("Select MobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "'");
                    strMobileNo = Convert.ToString(objMobile);
                }
                if (strMobileNo != "" && txtPurchaseParty.Text != "PERSONAL")
                {
                    if (btnAdd.Text == "&Save")
                        strMessage = "M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblNetAmt.Text + " with Receipt no : " + txtBillCode.Text + " " + txtBillNo.Text + ", on the date : " + txtDate.Text + ".";
                    else
                    {
                        if (txtSalesParty.Text != strOldPartyName)
                            strMessage = "M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblNetAmt.Text + " with receipt no : " + txtBillCode.Text + " " + txtBillNo.Text + ", on the date : " + txtDate.Text + ".";
                        else
                            strMessage = "Updation in Receipt : " + txtBillCode.Text + " " + txtBillNo.Text + ", M/S " + txtPurchaseParty.Text + ", We have received your item of amount : " + lblNetAmt.Text + ", on the date : " + txtDate.Text + ".";
                    }
                }
                SendSMS objSMS = new SendSMS();
                if (strMessage != "" && strMobileNo != "")
                    objSMS.SendSingleSMS(strMessage, strMobileNo);
                if (strOldMobileNo != "" && strOldPartyName != "PERSONAL")
                {
                    strMessage = "Sorry ! Wrong  entry on your account on Dt : " + txtDate.Text + ", Please avoid it.";
                    objSMS.SendSingleSMS(strMessage, strOldMobileNo);
                }
            }
            catch
            {
            }
        }

        private void lnkOrderNo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (txtOrderNo.Text != "")
                {
                    string strQuery = "Select OrderCode,SerialNo from OrderBooking Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end)='" + txtOrderNo.Text + "' ";
                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        string strOrderCode = Convert.ToString(dt.Rows[0]["OrderCode"]), strSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                        if (strOrderCode != "" && strSerialNo != "")
                        {
                            OrderBooking objOrder = new OrderBooking(strOrderCode, strSerialNo);
                            objOrder.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                            objOrder.ShowDialog();
                            objOrder.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";

                }
            }
        }

        private void txtQuantity_Enter(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0" || txt.Text == "0.00" || txt.Text == "0.0000")
                        txt.Clear();
                }
            }
        }

        private void dgrdItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                        //if (Index < dgrdDetails.RowCount - 1)
                        //    CurrentRow = Index - 1;
                        //else
                        CurrentRow = Index;

                        if (IndexColmn < dgrdDetails.ColumnCount - 6)
                        {
                            if (Index == dgrdDetails.RowCount - 1)
                                IndexColmn += 1;
                            if (CurrentRow >= 0)
                            {
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 6)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 6)
                                    IndexColmn++;
                                if (!dgrdDetails.Columns[IndexColmn].Visible && IndexColmn < dgrdDetails.ColumnCount - 6)
                                    IndexColmn++;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value) != "" && Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["gAmount"].Value) != "")
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[CurrentRow + 1].Cells["sno"].Value = (CurrentRow + 2) + ".";
                                dgrdDetails.Rows[CurrentRow + 1].Cells["itemName"].Value = dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value;
                                dgrdDetails.Rows[CurrentRow + 1].Cells["designName"].Value = dgrdDetails.Rows[CurrentRow].Cells["designName"].Value;
                                dgrdDetails.Rows[CurrentRow + 1].Cells["cut"].Value = dgrdDetails.Rows[CurrentRow].Cells["cut"].Value;
                                dgrdDetails.Rows[CurrentRow + 1].Cells["mtr"].Value = dgrdDetails.Rows[CurrentRow].Cells["mtr"].Value;
                                dgrdDetails.Rows[CurrentRow + 1].Cells["fold"].Value = dgrdDetails.Rows[CurrentRow].Cells["fold"].Value;

                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow + 1].Cells["itemName"];
                            }
                            else
                            {
                                txtPackingAmt.Focus();
                                //if (btnAdd.Text == "&Save")
                                //    btnAdd.Focus();
                                //else
                                //    btnEdit.Focus();
                            }
                        }
                        if (IndexColmn == 2)
                        {
                            dgrdDetails.CurrentCell.Value = Convert.ToString(dgrdDetails.CurrentCell.Value).Trim() + " ";
                            e.Handled = true;
                            dgrdDetails.BeginEdit(false);
                        }
                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        Index = dgrdDetails.CurrentCell.RowIndex;
                        if (btnAdd.Text == "&Save")
                        {
                            dgrdDetails.Rows.RemoveAt(Index);
                            CalculateTotalAmt();
                        }
                        else if (btnEdit.Text == "&Update")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                dgrdDetails.Rows.RemoveAt(Index);
                                CalculateTotalAmt();
                            }
                        }
                        if (dgrdDetails.Rows.Count == 0)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["sno"].Value = dgrdDetails.Rows.Count + ".";
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                        }
                    }
                    else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
                    {
                        int _rowCount = dgrdDetails.Rows.Count;
                        dgrdDetails.Rows.Add(1);

                        dgrdDetails.Rows[_rowCount].Cells["sno"].Value = (_rowCount + 1) + ".";
                        dgrdDetails.Rows[_rowCount].Cells["itemName"].Value = dgrdDetails.CurrentRow.Cells["itemName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["designName"].Value = dgrdDetails.CurrentRow.Cells["designName"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["gQty"].Value = dgrdDetails.CurrentRow.Cells["gQty"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["cut"].Value = dgrdDetails.CurrentRow.Cells["cut"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["mtr"].Value = dgrdDetails.CurrentRow.Cells["mtr"].Value;
                        dgrdDetails.Rows[_rowCount].Cells["fold"].Value = dgrdDetails.CurrentRow.Cells["fold"].Value;

                        dgrdDetails.CurrentCell = dgrdDetails.Rows[_rowCount].Cells["rate"];
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch
            {
            }
        }

        private void ArrangeSerialNo()
        {
            int serialNo = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells["sno"].Value = serialNo;
                serialNo++;
            }
        }

        private void dgrdItem_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 1)
                    {
                        SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            if (Convert.ToString(dgrdDetails.CurrentRow.Cells["designName"].Value) == "" || (btnAdd.Text == "&Save" && (dgrdDetails.Rows.Count - 1) == e.RowIndex && _strPDFFilePath == ""))
                            {
                                string[] strItem = objSearch.strSelectedData.Split(':');
                                dgrdDetails.CurrentRow.Cells["designName"].Value = strItem[0].Trim();
                            }
                            CalculateNetAmount();

                            if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor == Color.Tomato)
                                dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                        }
                        e.Cancel = true;
                    }
                    if (e.ColumnIndex == 8)
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void txtPurchaseDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (MainPage.strUserRole.Contains("SUPERADMIN"))
                    // dba.GetStringFromDateForCompany(txtPurchaseDate);
                    dba.GetDateInExactFormat(sender, true, false, false);
                else
                    // dba.GetStringFromDate(txtPurchaseDate);
                    dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void txtPurchaseType_KeyDown(object sender, KeyEventArgs e)
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
                        txtPurchaseType.Text = objSearch.strSelectedData;
                        CalculateNetAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSignAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char pressedKey = e.KeyChar;
                if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                    e.Handled = false;
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void dgrdItem_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            int cIndex = dgrdDetails.CurrentCell.ColumnIndex;
            if (cIndex == 2)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.CharacterCasing = CharacterCasing.Upper;
                txtBox.KeyPress += new KeyPressEventHandler(txtRemark_KeyPress);
            }
            else if (cIndex > 2)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dgrdItem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 2)
                {
                    dgrdDetails.CurrentCell.Value = dba.ConvertObjectToDoubleWithSign(dgrdDetails.CurrentCell.EditedFormattedValue);
                    double dQty = 0, dRate = 0, dAmt = 0, dSplDis = 0, dNetRate = 0;
                    if (e.ColumnIndex < 10 && e.ColumnIndex != 8 && e.ColumnIndex != 4)
                    {
                        CalculateAllDetails(e.ColumnIndex, dgrdDetails.CurrentRow);
                    }
                    CalculateSpecialDiscountAmt();
                }
                else if (e.ColumnIndex == 2)
                {
                    if (Convert.ToString(dgrdDetails.CurrentCell.Value) == "")
                        dgrdDetails.CurrentCell.Value = dgrdDetails.CurrentRow.Cells["itemName"].Value;
                    else
                        dgrdDetails.CurrentCell.Value = Convert.ToString(dgrdDetails.CurrentCell.Value).Trim();

                }
            }
            catch
            {
            }
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void rdoNormalDhara_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoNormalDhara.Checked)
            {
                txtDisPer.Text = txtNormalDhara.Text;
                CalculateNetAmount();
            }
        }

        private void rdoSuperNet_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoSuperNet.Checked)
            {
                txtDisPer.Text = txtSuperNetDhara.Text;
                CalculateNetAmount();
            }
        }

        private void txtSignAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSignAmt.Text == "")
                    txtSignAmt.Text = "-";
                if (txtOtherAmt.Text == "")
                    txtOtherAmt.Text = "0.00";
                CalculateNetAmount();
            }
        }

        private void txtReverseCharge_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("REVERSECHARGES", "SEARCH REVERSE CHARGES", e.KeyCode);
                        objSearch.ShowDialog();
                        //txtReverseCharge.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTaxPer_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode != Keys.Escape)
            //{
            //         if (!pnlTax.Visible)
            //            pnlTax.Visible = true;
            //        else
            //            pnlTax.Visible = false;
            //    }
            //}
        }

        private void txtOtherAmt_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            //{
            //    if (btnAdd.Text == "&Save")
            //        btnAdd.Focus();
            //    else if (btnEdit.Text == "&Update")
            //        btnEdit.Focus();
            //}
        }

        private void btnChecked_Click(object sender, EventArgs e)
        {
            btnChecked.Enabled = false;
            ChangeCheckStatus();
            btnChecked.Enabled = true;
        }

        private void GoodscumPurchase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void dgrdDetails_Enter(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
            }
            catch { }
        }

        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    dgrdDetails.Focus();
            //    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
            //}
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "" && ValidateOtherValidation(true))
                {
                    if (btnAdd.Text != "&Save" && ValidatePurchaseAndSaleStatus() && dba.ValidateBackDateEntry(txtDate.Text))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strQuery = "";
                            if (strFullOrderNo != "")
                            {
                                //  strQuery += " Update OrderBooking Set Status='PENDING' Where (CASE When NumberCode!='' then (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode) else OrderCode+' '+CAST(OrderNo as varchar) end) in (Select OrderNo from GoodsReceive Where OrderNo!='' and ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + ") ";

                                strQuery += " Update OB Set OB.Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- ISNULL(GR.Qty,0)))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty-ISNULL(GR.Qty,0)),UpdateStatus=1  from OrderBooking OB Cross APPLY (Select ReceiptCode,ReceiptNo, (CASE WHEN GR.Pieces='PETI' and GR.NoOfCase>0 and GR.OrderDate>'09/13/2019' then GR.NoOfCase else CAST(GR.Quantity as money) end) as Qty from GoodsReceive GR Where LTRIM(RTRIM((OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)))=GR.OrderNo and GR.OrderNo!='')GR Where GR.ReceiptCode='" + txtBillCode.Text + "' and GR.ReceiptNo=" + txtBillNo.Text + "  ";
                            }

                            strQuery += " Delete from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text
                                     + " Delete from GoodsReceiveDetails Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('PURCHASE A/C','DUTIES & TAXES') and Description in (Select (BillCode+' '+CAST(BillNo as varchar)) from PurchaseRecord Where GRSNO='" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from PurchaseRecord Where GRSNO='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";


                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from GoodsReceive Where  ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
                                    DataBaseAccess.CreateDeleteQuery(strQuery);
                                txtReason.Text = "";
                                DeleteImortedPDFFileInGSTFolder();

                                pnlDeletionConfirmation.Visible = false;
                                MessageBox.Show("Thank You ! Record deleted successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindNextRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
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

        private bool CheckPartyAdjustmentWithOrder(string _strOrderNo)
        {
            if (txtOrderNo.Text != "" && btnEdit.Text == "&Update")
            {
                string strQuery = " Select SchemeName from OrderBooking OB Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+NumberCode)='" + _strOrderNo + "' ";
                object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
                if (Convert.ToString(objValue) != "")
                {
                    string _strBillNo = txtBillCode.Text + " " + txtBillNo.Text;
                    bool _bStatus = DataBaseAccess.CheckPartyAdjustedAmount(_strBillNo);

                    if (!_bStatus)
                    {
                        MessageBox.Show("Sorry ! Supplier account has been adjusted, Please unadjust supplier's account.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
            }
            return true;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
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
                        SearchData objSearch = new SearchData("GOODSRCODE", "SEARCH GOODS RECEIPT CODE", e.KeyCode);
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

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void txtSpeDiscPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtSpeDiscPer.Text == "")
                        txtSpeDiscPer.Text = "0.00";
                    CalculateSpecialDiscountAmt();
                }
            }
            catch
            {
            }
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("GOODSPURCHASE", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void rdoSummary_CheckedChanged(object sender, EventArgs e)
        {
            //GetTaxAmount(0, 0);
            CalculateNetAmount();
            txtTaxAmt.ReadOnly = !rdoSummary.Checked;
        }

        private void txtTaxAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtTaxAmt.Text == "")
                    txtTaxAmt.Text = "0.00";
                CalculateNetAmount();

            }
        }

        private void txtPurchaseInvoiceNo_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPurchaseInvoiceNo.Text != "")
                    ValidateFromPreviousBill(true);
            }
        }

        private void txtSpecialDiscAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtSpecialDiscAmt.Text == "")
                    txtSpecialDiscAmt.Text = "0.00";
                CalculateSpclPerFromSpclAmount();
            }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnEdit.Text == "&Update")
                e.Handled = true;
            else
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtSpeDiscPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.KeyHandlerPoint(sender, e, 4);
            else
                e.Handled = true;
        }

        private void txtPcsAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.KeyHandlerPoint(sender, e, 2);
            }
            else
                e.Handled = true;
        }

        private void txtPcsAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPcsAmt.Text == "")
                    txtPcsAmt.Text = "0.00";
                CalculateSpecialDiscountAmt();
            }
        }

        private void txtTaxFree_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtTaxFree.Text == "")
                    txtTaxFree.Text = "0.00";
                CalculateTotalAmt();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                //if (MainPage.strUserRole.Contains("ADMIN"))
                //{
                DialogResult result = MessageBox.Show("Are you sure you want to reset ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string strQuery = "";
                    if (btnPrint.Enabled)
                        strQuery = "Update GoodsReceive Set OtherBillStatus=1,[OtherBillReq]=0 Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ";
                    else
                        strQuery = "Update GoodsReceive Set OtherBillStatus=0,[OtherBillReq]=1 Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ";

                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        btnPrint.Enabled = !btnPrint.Enabled;
                        MessageBox.Show("Thank you ! Bill reset successfully!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to reset  right now !!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                //}
            }
            catch { }
        }

        private void ChangeCheckStatus()
        {
            try
            {
                if (btnAdd.Text != "&Save")
                {
                    if (btnChecked.Text == "Status : Un-Checked")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to checked this bill ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = "Declare @BillCode nvarchar(50) ; Select TOP 1 @BillCode=PBillCode from CompanySetting "
                                          + " UPDATE PurchaseRecord Set CheckStatus = 1, CheckedBy = '" + MainPage.strLoginName + "' WHere BillCode = @BillCode and BillNo =" + txtBillNo.Text
                                          + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                          + "('GOODSPURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CHECKED') ";


                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thanks ! Status changed successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnChecked.Text = "Status : Checked";
                                btnChecked.BackColor = Color.DarkGreen;
                            }
                        }
                    }
                    else if (btnChecked.Text == "Status : Checked")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to un-checked this bill ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = "Declare @BillCode nvarchar(50) ; Select TOP 1 @BillCode=PBillCode from CompanySetting "
                                            + " UPDATE PurchaseRecord Set CheckStatus = 0, CheckedBy = '' WHere BillCode = @BillCode and BillNo =" + txtBillNo.Text
                                            + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                            + "('GOODSPURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UNCHECKED') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thanks ! Status changed successfully !!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnChecked.Text = "Status : Un-Checked";
                                btnChecked.BackColor = Color.FromArgb(185, 30, 12);
                            }

                        }
                    }

                }
            }
            catch { }
        }

        private void GetPartyDhara()
        {
            try
            {
                string strPurchasePartyID, strQuery = "";
                if (txtPurchaseParty.Text != "PERSONAL")
                {
                    string[] strFullName = txtPurchaseParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strQuery = "Select SM.NormalDhara,SM.SNDhara as SUPERDhara,SM.CFormApply as PremiumDhara,GSTNo,(Select Top 1 PurchaseType from GoodsReceive Where PurchasePartyID=(SM.AreaCode+CAST(SM.AccountNo as varchar)) Order by ReceiptNo Desc)PurchaseType from SupplierMaster SM  Where  (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strPurchasePartyID + "' ";
                        DataTable dt = dba.GetDataTable(strQuery);
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtNormalDhara.Text = txtDisPer.Text = Convert.ToString(row["NormalDhara"]);
                            txtSuperNetDhara.Text = Convert.ToString(row["SUPERDhara"]);
                            txtPremiumDhara.Text = Convert.ToString(row["PremiumDhara"]);
                            txtGSTNo.Text = Convert.ToString(row["GSTNo"]);
                            if (Convert.ToString(row["PurchaseType"]) != "")
                                txtPurchaseType.Text = Convert.ToString(row["PurchaseType"]);
                            rdoNormalDhara.Checked = true;
                        }
                        else
                            txtNormalDhara.Text = txtSuperNetDhara.Text = txtDisPer.Text = txtGSTNo.Text = "";

                        CalculateNetAmount();
                    }
                    else
                        txtNormalDhara.Text = txtSuperNetDhara.Text = txtDisPer.Text = txtGSTNo.Text = "";
                }
                else
                {
                    txtNormalDhara.Text = txtSuperNetDhara.Text = txtDisPer.Text = "0";
                    txtGSTNo.Text = "";
                }
            }
            catch
            { txtNormalDhara.Text = txtSuperNetDhara.Text = txtDisPer.Text = txtGSTNo.Text = ""; }
        }

        private void txtOrderNo_DoubleClick(object sender, EventArgs e)
        {

        }

        private void txtGSTNo_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
        }

        private void txtTaxPer_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
        }

        private void dgrdPending_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int colIndex = dgrdPending.CurrentCell.ColumnIndex, rowIndex = dgrdPending.CurrentRow.Index;
                if (colIndex >= 0 && rowIndex >= 0)
                {
                    if (e.KeyCode == Keys.Space)
                    {
                        if (dgrdPending.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                        {
                            dgrdPending.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            dgrdPending.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                        }

                    }
                }
            }
            catch { }
        }

        private void rdoPremium_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoPremium.Checked)
            {
                txtDisPer.Text = txtPremiumDhara.Text;
                CalculateNetAmount();
            }
        }

        private void txtSaleBillNo_DoubleClick(object sender, EventArgs e)
        {

        }

        private void ShowSaleBook(string strCode, string strBillNo)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                dba.ShowSaleBookPrint(strCode, strBillNo, false, false);
            }
            else
            {
                SaleBook objSale = new SaleBook(strCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.Show();
            }
        }

        private void txtSaleBillNo_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSaleBillNo.Text != "")
                {
                    string[] strNumber = txtSaleBillNo.Text.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                }
                else if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Enabled || btnEdit.Enabled)
                        ShowSaleBillDetails();
                }
            }
            catch { }
        }

        private void txtGSTNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                            txtPurchaseParty.Text = strData;
                    }
                    else
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
                            string strData = objSearch.strSelectedData, strGSTNo = "";
                            if (strData != "")
                            {
                                bool _blackListed = false;
                                if (dba.CheckTransactionLockWithBlackList(strData, ref _blackListed))
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
                                    txtPurchaseParty.Text = strData;
                                    txtGSTNo.Text = strGSTNo;
                                    GetPartyDhara();
                                }
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

        private void txtPackingAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPackingAmt.Text == "")
                    txtPackingAmt.Text = "0.00";
                CalculateTotalAmt();
            }
        }

        private void txtFreight_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtFreight.Text == "")
                    txtFreight.Text = "0.00";
                CalculateTotalAmt();
            }
        }

        private void dgrdRelatedParty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strParty = Convert.ToString(dgrdRelatedParty.CurrentCell.Value), strOldParty = txtSalesParty.Text;
                    if (strParty != "")
                    {
                        txtSalesParty.Text = strParty;
                        dgrdRelatedParty.CurrentCell.Value = strOldParty;
                    }

                    dgrdPending.Rows.Clear();
                    txtSubParty.Text = "SELF";
                    GetPendingOrderAndOther();
                    GetRelatedpartyDetails();

                    txtSalesParty.Focus();
                }
                // GetRelatedpartyDetails();
            }
            catch { }
        }

        private void txtSalesParty_Leave(object sender, EventArgs e)
        {
            pnlRelatedParty.Visible = false;
        }

        private void txtSalesParty_Enter(object sender, EventArgs e)
        {
            if (dgrdRelatedParty.Rows.Count > 0)
                pnlRelatedParty.Visible = true;
            else
                pnlRelatedParty.Visible = false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
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
            }
            catch
            {
            }
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

        private void txtOrderNo_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtOrderNo.Text != "")
                {
                    string strSerialCode = "", strSerialNo = "";
                    dba.GetOrderSerialCodeAndSerialNo(txtOrderNo.Text, ref strSerialCode, ref strSerialNo);
                    if (strSerialCode != "" && strSerialNo != "")
                        ShowOrderBookingDetails_Update(strSerialCode, strSerialNo);
                }
                else
                    ShowOrderBookingDetails();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void CalculateNetAmount()
        {
            double dSplDisPer = 0, dSplDiscount = 0, dDisPer = 0, dDiscount = 0, dOtherPerAmt = 0, dRoundOff = 0, dOtherAmt = 0, dFreight = 0, dPacking = 0, dTax = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dTaxAmt = 0, dFinalAmt = 0, dTcsPer = 0, dTCSAmt = 0, dTaxableAmt = 0;
            try
            {
                dSplDisPer = dba.ConvertObjectToDouble(txtSpeDiscPer.Text);
                dDisPer = dba.ConvertObjectToDouble(txtDisPer.Text);

                dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);

                if (dTotalAmount != 0 && dSplDisPer > 0)
                    dSplDiscount = ((dTotalAmount * 100 / (100 - dSplDisPer)) * dSplDisPer) / 100;
                // dTotalAmount -= dSplDiscount;

                dDiscount = (dTotalAmount * dDisPer) / 100;

                dTOAmt = dOtherPerAmt + dOtherAmt + dFreight + dPacking + dTax;
                dFinalAmt = dGrossAmt - dDiscount + dTOAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, ref dTaxableAmt);

                _dFinalAmt = dNetAmt = (dGrossAmt - dDiscount + dOtherPerAmt + dOtherAmt + dFreight + dPacking + dTax + dTaxAmt);
                if (chkTCSAmt.Checked)
                {
                    dTcsPer = dba.ConvertObjectToDouble(txtTCSPer.Text);
                    dTCSAmt = (dNetAmt * dTcsPer) / 100.00;
                    dNetAmt += dTCSAmt;
                }

                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0")); //Math.Round(dNetAmt, 0);
                dRoundOff = dNNetAmt - dNetAmt;

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

                txtSpecialDiscAmt.Text = dSplDiscount.ToString("N2", MainPage.indianCurancy);
                txtDiscountAmt.Text = Math.Abs(dDiscount).ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtTCSAmt.Text = dTCSAmt.ToString("N2", MainPage.indianCurancy);

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch// (Exception ex)
            {
                //  string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                // dba.CreateErrorReports(strReport);
            }
        }

        private double GetTaxAmount(double dFinalAmt, double dOtherAmt, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer = 0;
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (rdoSummary.Checked)
                {
                    dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                }
                else
                {
                    if (MainPage._bTaxStatus && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
                    {
                        DataTable _dt = dba.GetSaleTypeDetails(txtPurchaseType.Text, "PURCHASE");
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
                                double dDisStatus = 0;

                                strGRSNo = txtBillCode.Text + " " + txtBillNo.Text;
                                dDisStatus = dba.ConvertObjectToDouble(txtDisPer.Text);

                                double dRate = 0, dPacking = 0, dQty = 0, dAmt = 0;
                                dPacking += dba.ConvertObjectToDouble(txtPackingAmt.Text) + dba.ConvertObjectToDouble(txtFreight.Text);

                                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                                {
                                    dRate = dba.ConvertObjectToDouble(rows.Cells["gRate"].Value);
                                    dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                                    dQty = dba.ConvertObjectToDouble(rows.Cells["gQty"].Value);

                                    if (dRate > 0)
                                    {
                                        if (strQuery != "")
                                            strQuery += " UNION ALL ";

                                        strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0  ";
                                    }
                                }

                                dPacking += dOtherAmt;
                                if (dPacking != 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,"+dTaxPer+" as TaxRate ";
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
                                        dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);

                                        dTaxAmt = dTTaxAmt;
                                        if (dPacking == 0)
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

            if (!rdoSummary.Checked)
            {
                txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                txtTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);
            }

            if (_strTaxType == "INCLUDED")
                dTaxAmt = 0;
            return dTaxAmt;
        }

        //private void BindTaxDetails(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt, ref double dTaxableAmt)
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
        //                dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);                      
        //                dTTaxAmt += Convert.ToDouble(dTaxAmt.ToString("0.00")); 

        //                if (dTaxRate > dMaxRate)
        //                    dMaxRate = dTaxRate;

        //                dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
        //                dgrdTax.Rows[_index].Cells["taxName"].Value = strIGST;
        //                dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;

        //                if (strRegion == "LOCAL")
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N4", MainPage.indianCurancy);
        //                    _index++;
        //                    dgrdTax.Rows[_index].Cells["taxName"].Value = strSGST;
        //                    dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N4", MainPage.indianCurancy);
        //                }
        //                else
        //                {
        //                    dgrdTax.Rows[_index].Cells["taxRate"].Value = dTaxRate.ToString("N2", MainPage.indianCurancy);
        //                    dgrdTax.Rows[_index].Cells["taxAmt"].Value = dTaxAmt.ToString("N4", MainPage.indianCurancy);
        //                }

        //                _index++;
        //            }
        //        }
        //    }
        //    catch { }
        //}

        private void ShowSaleBillDetails()
        {
            try
            {
                if ((!rdoSummary.Checked && !rdoPacked.Checked) || (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit"))
                {
                    DialogResult _result = MessageBox.Show("Are you want to generate sale bill right now ?", "Sale Bill Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (_result == DialogResult.Yes)
                    {
                        SaleBook _sale = new SSS.SaleBook(true);
                        _sale._strPSalesParty = txtSalesParty.Text;
                        _sale._strPSubParty = txtSubParty.Text;
                        _sale._strPackingType = GetPackingStatus();
                        _sale.strNewAddedGRSNO = txtBillCode.Text + " " + txtBillNo.Text;
                        _sale.MdiParent = MainPage.mymainObject;
                        _sale.Show();

                    }
                }
            }
            catch { }
        }

        private void CalculateAllDetails(int _ColIndex, DataGridViewRow row)
        {
            try
            {
                if (_ColIndex >= 0 && _ColIndex != 4 && dgrdDetails.CurrentRow.Index >= 0)
                {
                    double dPcsAmt = dba.ConvertObjectToDouble(txtPcsAmt.Text), dTotalMtr = dba.ConvertObjectToDouble(lblTotalMTR.Text), dTotalQty = dba.ConvertObjectToDouble(lblQty.Text), dPerPcsRate = 0;
                    if (dTotalMtr > 0)
                        dTotalQty = dTotalMtr;
                    if (dPcsAmt != 0 && dTotalQty != 0)
                        dPerPcsRate = dPcsAmt / dTotalQty;


                    double dQty = 0, dMtr = 0, dFold = 0, dNetAmt = 0, dRate = 0, dNetRate = 0, dRoundOff = 0, dAmt = 0, dSplDis = 0; //dCut = 0, dMtr = 0,
                                                                                                                                      //DataGridViewRow row = dgrdDetails.CurrentRow;
                    dQty = dba.ConvertObjectToDouble(row.Cells["gQty"].Value);
                    //dCut = dba.ConvertObjectToDouble(row.Cells["cut"].Value);

                    dFold = dba.ConvertObjectToDouble(row.Cells["fold"].Value);
                    dRate = dba.ConvertObjectToDouble(row.Cells["rate"].Value);
                    dAmt = dba.ConvertObjectToDouble(row.Cells["gAmount"].Value);
                    dSplDis = dba.ConvertObjectToDouble(txtSpeDiscPer.Text);

                    dMtr = dba.ConvertObjectToDouble(row.Cells["mtr"].Value);
                    if (dMtr > 0 && dFold != 0)
                    {
                        if (dMtr > 0)
                            dQty = ((dMtr * dFold) / 100.00);
                    }

                    if (dFold == 0)
                        dFold = 100;

                    if (dQty == 0)
                        dQty = 1;

                    if (_ColIndex > 2 && _ColIndex < 8)
                    {
                        dNetRate = (dRate * (100 - dSplDis) / 100.00);//* dFold) / 100.00
                        dNetRate -= dPerPcsRate;

                        dAmt = dQty * Math.Round(dNetRate, 2);

                        row.Cells["gAmount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                    }
                    else if (_ColIndex == 9)
                    {
                        dNetRate = dAmt / dQty;
                        //dNetRate = ((dRate * (100 - dSplDis) / 100)) * dFold / 100;
                        dRate = ((dNetRate * 100.0) / (100.00 - dSplDis)); // / dFold) * 100.00
                        dRate += dPerPcsRate;
                        row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    }

                    row.Cells["gRate"].Value = dNetRate.ToString("N2", MainPage.indianCurancy);
                }
            }
            catch { }
        }

        private void PasteClipboard(DataGridView myDataGridView)
        {
            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            {
                if (myDataGridView.RowCount > 0)
                    myDataGridView.Rows.Clear();

                if (myDataGridView.ColumnCount > 0)
                    myDataGridView.Columns.Clear();

                bool columnsAdded = false;
                string[] pastedRows = System.Text.RegularExpressions.Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
                foreach (string pastedRow in pastedRows)
                {
                    string[] pastedRowCells = pastedRow.Split(new char[] { '\t' });

                    //if (!columnsAdded)
                    //{
                    //    for (int i = 0; i < pastedRowCells.Length; i++)
                    //        myDataGridView.Columns.Add("col" + i, pastedRowCells[i]);

                    //    columnsAdded = true;
                    //    continue;
                    //}

                    myDataGridView.Rows.Add();
                    int myRowIndex = myDataGridView.Rows.Count - 1;

                    using (DataGridViewRow myDataGridViewRow = myDataGridView.Rows[myRowIndex])
                    {
                        myDataGridViewRow.Cells[0].Value = myDataGridView.Rows.Count;
                        for (int i = 1; i < pastedRowCells.Length; i++)
                            myDataGridViewRow.Cells[i].Value = pastedRowCells[i];
                    }
                }
            }
        }

        private void ShowOrderBookingDetails()
        {
            try
            {
                if (!rdoSummary.Checked)
                {
                    DialogResult _result = MessageBox.Show("Are you want to generate order right now ?", "Order Booking Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (_result == DialogResult.Yes)
                    {
                        OrderBooking _order = new SSS.OrderBooking(true);
                        _order._strPSalesParty = txtSalesParty.Text;
                        _order._strPSubParty = txtSubParty.Text;
                        _order._strPackingType = txtPcsType.Text;
                        _order._strPPurchaseParty = txtPurchaseParty.Text;
                        if (txtPcsType.Text == "LOOSE")
                            _order._strQty = lblQty.Text;
                        else
                            _order._strQty = txtNoOfCase.Text;

                        _order._strAmount = lblNetAmt.Text;

                        if (dgrdDetails.Rows.Count > 0)
                            _order._strItemName = Convert.ToString(dgrdDetails.Rows[0].Cells["itemName"].Value);
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                            _order._strGRSNO = txtBillCode.Text + " " + txtBillNo.Text;

                        _order.FormBorderStyle = FormBorderStyle.FixedDialog;
                        _order.ShowDialog();

                        string _strOrder = _order.strAddedOrderDetails;
                        if (_strOrder != "")
                        {
                            string[] str = _strOrder.Split('|');
                            txtOrderNo.Text = str[0];
                            _strCustomerName = str[2];
                            _strSubPartyName = str[3];
                            txtOrderDate.Text = str[4];
                            _strSupplierName = str[5];
                            _strPcsType = str[6];
                        }
                    }
                }
            }
            catch { }
        }

        private void ShowOrderBookingDetails_Update(string strSerialCode, string strSerialNo)
        {
            try
            {
                if (!rdoSummary.Checked)
                {

                    OrderBooking _order = new SSS.OrderBooking(strSerialCode, strSerialNo);
                    _order.FormBorderStyle = FormBorderStyle.FixedDialog;
                    _order._strOrderNo_Update = txtOrderNo.Text;
                    _order.updateStatus = true;
                    _order.ShowDialog();

                    string _strOrder = _order.strAddedOrderDetails;
                    if (_strOrder != "")
                    {
                        string[] str = _strOrder.Split('|');
                        txtOrderNo.Text = str[0];
                        _strCustomerName = str[2];
                        _strSubPartyName = str[3];
                        txtOrderDate.Text = str[4];
                        _strSupplierName = str[5];
                        _strPcsType = str[6];
                    }
                }
            }
            catch { }

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
                            _itemIndex = 20;
                        else if ((txtPurchaseParty.Text.Contains("MISHU ENTERPRISES")) && i == 2)
                            _itemIndex = 19;
                        else if (txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS"))
                            _itemIndex = 10;
                        else if (txtPurchaseParty.Text.Contains("SURAJ ACTIVEGEAR") && i == 2)
                            _itemIndex -= 2;
                    }

                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP") || txtPurchaseParty.Text.Contains("BONNYS NX"))
                        _bEndStatus = SetItemDetailsByCustomize_Branches(_itemIndex, text, ref _bLongLable);
                    else if (txtPurchaseParty.Text.Contains("LUCKY JACKET")|| txtPurchaseParty.Text.Contains("KC GARMENTS") || txtPurchaseParty.Text.Contains("JANAK GARMENTEX") || txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") || txtPurchaseParty.Text.Contains("NIKUNJ TRADING") || txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") || txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS") || txtPurchaseParty.Text.Contains("DONARGOLD GARMENTS") || txtPurchaseParty.Text.Contains("W STAN GARMENTS") || txtPurchaseParty.Text.Contains("GEX GARMENTS") || txtPurchaseParty.Text.Contains("MOTI FASHIONS") || txtPurchaseParty.Text.Contains("TANEJA FASHION") || txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") || txtPurchaseParty.Text.Contains("VIPIN COLLECTION") || txtPurchaseParty.Text.Contains("JOLLY FASHIONS") || txtPurchaseParty.Text.Contains("CHANCELLOR INDUSTRIES") || txtPurchaseParty.Text.Contains("MAA PADMAVATI APPARELS") || txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("KHANDELWAL TRADERS") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("CLASSIN APPARELS") || txtPurchaseParty.Text.Contains("MAUZ FASHIONS") || txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("AASHI COLLECTION") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION LLP") || txtPurchaseParty.Text.Contains("S.R CREATION") || txtPurchaseParty.Text.Contains("SHUBHI GARMENTS") || txtPurchaseParty.Text.Contains("WORLD CHOICE") || txtPurchaseParty.Text.Contains("MIKEY FASHION") || txtPurchaseParty.Text.Contains("WORLD SAHAB") || txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") || txtPurchaseParty.Text.Contains("ARPIT FASHION") || txtPurchaseParty.Text.Contains("FCB GARMENT") || txtPurchaseParty.Text.Contains("FCB GARMENT") || txtGSTNo.Text.Contains("07EHOPK4815E1Z8") || txtGSTNo.Text.Contains("07BQWPK0733R2ZZ") || txtGSTNo.Text.Contains("07AXGPG4663A1ZT"))
                        _bEndStatus = SetItemDetailsByCustomize_Delhi(_itemIndex, text, ref _bLongLable);
                    else if (_strBillType == "BUSY" || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS") || txtGSTNo.Text.Contains("07AAHPJ9380P1ZX") || txtPurchaseParty.Text.Contains("SHRI KRISHNA GARMENTS") || txtGSTNo.Text.Contains("07BEMPM7164N1ZQ") || txtGSTNo.Text.Contains("07AUCPJ3982A1ZW"))
                        _bEndStatus = SetItemDetailsLineByBusy(_itemIndex, text);
                    else if (bByteData)
                        _bEndStatus = SetItemDetailsLineByLine(_itemIndex, text);
                    else
                        _bEndStatus = SetItemDetails(_itemIndex, text, ref _bLongLable);

                    if (_bEndStatus)
                        break;
                }
                txtSalesParty.Focus();
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            if (txtPcsAmt.Text != "0.00")
                CalculateTotalQty();
            CalculateSpecialDiscountAmt();

            if (txtPurchaseInvoiceNo.Text != "")
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
                    if (strText.ToUpper().Contains("ROUND"))
                        strText = _lines[_index + 6];
                    if (txtPurchaseParty.Text.Contains("SPARKY"))
                    {
                        string[] str = _lines[_index].Trim().Split(' ');
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(str[str.Length-1]).ToString("0.00");
                       // chkTCSAmt.Checked = true;
                    }
                    else
                        txtPackingAmt.Text = dba.ConvertObjectToDouble(strText).ToString("0.00");                
                        
                    break;
                }
                if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("SPARKY") )
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
                            txtFreight.Text = _data[0];
                        }
                        else
                            txtFreight.Text = _data[_data.Length - 1];
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
                else if (!strText.Contains("Less") && !strText.Contains("TCS") && !strText.Replace(" ", "").ToUpper().Contains("CGST") && !strText.Replace(" ", "").ToUpper().Contains("SGST") && !strText.Replace(" ", "").ToUpper().Contains("IGST") && !strText.ToUpper().Contains("DISCOUNT") && !strText.ToUpper().Contains("JURISDICTION") && !strText.ToUpper().Contains("COMPUTER"))
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
                    else if (txtPurchaseParty.Text.Contains("SAMAKSH  ENTERPRISES") || txtPurchaseParty.Text.Contains("TAANI INDUSTRIES PVT LTD") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES"))
                    {
                        if (_data.Length > 3)
                        {
                            string strDescription = "";
                            string[] _strText = _lines[_index - 1].Trim().Split(' ');

                            if (_data[0] == _lineIndex.ToString() || _strText[0] == _lineIndex.ToString())
                            {
                                strDescription = _lines[_index + 1].Trim();
                                string[] __str = strDescription.Split(' ');
                                if (__str.Length > 0)
                                {
                                    strHSNCode = __str[__str.Length - 1];
                                    strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                }
                                if(strHSNCode=="")
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
                                if (strItem != "" && strDescription != "" && !strDescription.Contains(",") && !strDescription.Contains(strHSNCode) && !strDescription.Contains(".00") && !strDescription.ToLower().Contains("continue") && strDescription.Length > 2)
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
                    else if (txtPurchaseParty.Text.Contains("SPARKY"))
                    {
                        if (_data.Length > 2)
                        {                           
                            if (_data[0] == _lineIndex.ToString())
                            {                               
                              string[] _strText = _lines[_index].Trim().Split(' ');

                                foreach (string str in _strText)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str.Trim();
                                }

                                strHSNCode = _lines[_index + 1].Trim();
                                _data = _lines[_index+2].Trim().Split(' ');
                                strQty = _data[0].Trim();
                                if (_data.Length>3)
                                    strRate = _data[2].Trim();

                                _index+=2;
                            }
                        }
                    }
                    else if (_data.Length > 7 || ((txtPurchaseParty.Text.Contains("SOMANI") || txtPurchaseParty.Text.Contains("BATRA EXCLUSIVE") || txtPurchaseParty.Text.Contains("G.D. CREATION") || txtPurchaseParty.Text.Contains(" P.R. ENTERPRISES")) && _data.Length > 6))
                    {
                        int _length = _data.Length;
                        _bLongLable = true;
                        if (txtPurchaseParty.Text.Contains("SPARKY"))
                        {
                            int _addIndex = 0;
                            if (_length > 13)
                                _addIndex = 6;
                            strHSNCode = _data[_length - (_addIndex + 7)];
                            strQty = _data[_length - (_addIndex + 6)];
                            strRate = _data[_length - (_addIndex + 4)];

                            for (int i = 0; i < _length - (_addIndex + 7); i++)
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
                               
                            if (txtGSTNo.Text.Contains("07AFGPA3656P1Z6") && _data[0] != _lineIndex.ToString())
                            {
                                strItem = _data[0];
                                strHSNCode = _data[_data.Length-1];
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if (strHSNCode.Length > 2)
                                {
                                    strQty = _data[_data.Length - 3];
                                    strRate = _data[_data.Length - 4];
                                }
                                else
                                {
                                    strQty = _data[_data.Length - 2];
                                    strRate = _data[_data.Length - 3];
                                    strHSNCode= _lines[_index+1].Trim();
                                }
                            }
                           else
                            {
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
                        dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                        dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                        dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                        if (txtPurchaseParty.Text.Contains("NATIONAL GARMENTS"))
                        {
                            dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = "SHIRT : " + strItem;
                            if (strHSNCode.Contains("6205"))
                                strItem = "SHIRT:6205";
                            else
                                CheckItemNameExistence(ref strItem, ref strHSNCode);
                        }
                        else
                            CheckItemNameExistence(ref strItem, ref strHSNCode);

                        dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
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

            if (txtGSTNo.Text.Contains("07AAUPS2828Q1ZM"))
            {
                rateIndex = 5;
                qtyIndex = 6;
                hsnCodeIndex = 8;
                itemIndex = 9;
                _lineGap = 6;
            }
            else
            {
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
                        txtFreight.Text = _data[0];
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
                        if (!strItem.ToUpper().Contains("AMOUNT") && !strItem.ToUpper().Contains("ROUND"))
                        {
                            dgrdDetails.Rows.Add();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["sno"].Value = _lineIndex + ".";
                            dgrdDetails.Rows[_lineIndex - 1].Cells["designName"].Value = strItem.ToUpper();
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gQty"].Value = strQty;
                            dgrdDetails.Rows[_lineIndex - 1].Cells["gRate"].Value = dgrdDetails.Rows[_lineIndex - 1].Cells["rate"].Value = strRate;
                            CheckItemNameExistence(ref strItem, ref strHSNCode);
                            dgrdDetails.Rows[_lineIndex - 1].Cells["itemName"].Value = strItem;
                        }
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

        private void txtTCSAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtTCSAmt.Text == "")
                    txtTCSAmt.Text = "0.00";
                CalculateNetAmount();
            }
        }

        private void chkTCSAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                CalculateNetAmount();
                if (MainPage.strUserRole.Contains("SUPERADMIN"))
                    txtTCSPer.ReadOnly = false;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && txtOrderNo.Text=="")|| (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit"))
                    btnRefresh.Enabled = false;
                DialogResult result = MessageBox.Show("Are you sure you want to download order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int count = dba.DownloadOrderDetails();
                    if (count > 0)
                    {
                        MessageBox.Show("Thank you ! Order downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                       
                    }
                    else
                        MessageBox.Show("Sorry ! No order found right now ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
            btnRefresh.Enabled = true;
        }

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtTaxPer.Text == "")
                        txtTaxPer.Text = "18.00";
                    double dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);
                    if (dTaxPer != 3 && dTaxPer != 5 && dTaxPer != 12 && dTaxPer != 18 && dTaxPer != 28)
                        txtTaxPer.Text = "18.00";
                    CalculateNetAmount();
                }
            }
            catch { }
        }

        private void txtTCSPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.KeyHandlerPoint(sender, e, 4);
        }

        private void txtTCSPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtTCSAmt.Text == "")
                    txtTCSAmt.Text = "0.00";
                CalculateNetAmount();
            }
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
                        txtFreight.Text = _data[_data.Length - 1];
                    }
                    else
                    {
                        strText = _lines[_index + 1];
                        _data = strText.Split(' ');
                        if (_data.Length > 0 && _data.Length < 10)
                            txtFreight.Text = _data[0];
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
                        txtFreight.Text = _data[_data.Length - 1];
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
                        if (__str.Length > 2 || txtPurchaseParty.Text.Contains("DOLLCY GARMENTS") || txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                        {
                            strItem = strQty = strRate = "";
                            int index = 0;
                            strItem = __str[0].Replace(_lineIndex + ".", "").Replace(". ", " ").Trim();
                            if(strItem=="")
                            {
                                strText = _lines[_index + 1].Trim();
                                strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                __str = strText.Split('|');
                                strItem = __str[0].Replace(_lineIndex + ".", "").Replace(". ", " ").Trim();
                            }
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
                            else if (txtGSTNo.Text.Contains("07BMHPK1455M1ZS") && __str.Length > 3)
                            {
                                strItem = __str[0].Replace(_lineIndex + ".", "").Replace(". ", " ").Trim();
                                int hsnIndex = 0;
                                if (__str.Length == 5)
                                    hsnIndex = 1;
                                string[] _strHSNCode = __str[hsnIndex].Split(' ');
                                strHSNCode = _strHSNCode[_strHSNCode.Length - 1];
                                if (__str.Length > 3)
                                {
                                    string[] _strQty = __str[hsnIndex+1].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strQty = _strQty[0];
                                    _strQty = __str[hsnIndex+2].Trim().Split(' ');
                                    if (_strQty.Length > 0)
                                        strRate = _strQty[0];
                                }


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
                                if (__str.Length > 1)
                                    strHSNCode = __str[1].Trim();
                                else
                                {
                                    string[] ___str = __str[0].Split(' ');
                                    strHSNCode = ___str[___str.Length - 1];
                                }
                                if (strHSNCode.Length == 8 && (strHSNCode.Contains("000") || strHSNCode.Contains("990")))
                                    strHSNCode = strHSNCode.Substring(0, 4);
                                string __strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9]", "");
                                if ((__strHSNCode.Length != 4 || strHSNCode.Length != 4) && strHSNCode != "63" && strHSNCode != "62" && __str.Length > 2)
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
                                    else if (strHSNCode == "" && strHSNCode.Length != 4 && strHSNCode.Length != 8)
                                    {
                                        if (!strHSNCode.Contains(".00"))
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
                                        if (strHSNCode.Length != 4 || strHSNCode.Contains(".00"))
                                        {
                                            string[] _strHSN = strItem.Split(' ');
                                            strHSNCode = _strHSN[_strHSN.Length - 1];
                                        }
                                        else
                                            index++;
                                    }
                                }
                                string strDescription = "";
                                int _qtyRateIndex = 0;
                                if (__str.Length < 2)
                                {
                                    strText = _lines[_index + 1].Trim();
                                    strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    __str = strText.Split('|');
                                    if (__str.Length < 2)
                                    {
                                        strText = _lines[_index + 2].Trim();
                                        strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                        __str = strText.Split('|');
                                    }
                                    index--;

                                }
                                if (_qtyIndex > 0)
                                    strDescription = __str[index + _qtyIndex + 1].Trim();

                                if (txtPurchaseParty.Text.Contains("TAANI INDUSTRIES"))
                                    index++;
                                if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                                {
                                    if (__str.Length == 2 && strQty == "")
                                    {
                                        strText = _lines[_index + 1].Trim();
                                        strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                        __str = strText.Split('|');
                                        if (__str.Length == 1)
                                        {
                                            strText = _lines[_index + 2].Trim();
                                            strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                            __str = strText.Split('|');
                                            _index++;
                                        }
                                        index--;
                                    }
                                    if (_qtyRateIndex == 0)
                                        _qtyRateIndex++;
                                }

                                string[] _strQty = __str[index + _qtyIndex + 2].Trim().Split(' ');
                                if (_strQty.Length > 0)
                                    strQty = _strQty[0];
                                if (_strQty.Length == 1 && !txtPurchaseParty.Text.Contains("TAANI INDUSTRIES"))
                                    _qtyRateIndex = 1;
                                if (__str.Length < 5 && txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                                {
                                    strText = _lines[_index + _rateIndex + 1].Trim();
                                    strText = strText.Replace("|", ":").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("  ", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|").Replace("||", "|");
                                    __str = strText.Split('|');
                                    if (__str.Length > 0)
                                        strRate = __str[0].Trim();
                                }
                                else
                                {

                                    string _strData = __str[index + _qtyIndex + _rateIndex + _qtyRateIndex + 3];
                                    if (_strData.Contains("%"))
                                    {
                                        _strData = __str[index + _qtyIndex + _rateIndex + 3];
                                    }

                                    string[] _strRate = _strData.Trim().Split(' ');
                                    if (_strRate.Length > 0)
                                        strRate = _strRate[0];
                                    if (strRate == "0.00" && __str.Length > 6)
                                        strRate = __str[5].Trim();
                                }
                                if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS") && strItem.ToUpper().Contains("FREIGHT CHARGE"))
                                    strItem = "";
                                if (_lines.Length > _index + 5 && !txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
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
                        txtFreight.Text = _data[0];

                    break;
                }
                else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    return true;
                else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS"))
                {
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtFreight.Text = _data[_data.Length - 1];
                    }
                }
                else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                {
                    string[] _data = strText.Split(' ');
                    if (_data.Length > 1)
                        txtFreight.Text = _data[_data.Length - 1];
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
                            txtFreight.Text = _data[0];
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
            for (; _index < _lines.Length; _index++)
            {
                if (txtPurchaseParty.Text.Contains("KC GARMENTS") || _index < _lines.Length - 1)
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
                            txtFreight.Text = _data[0];

                        break;
                    }
                    else if ((strText.ToUpper().Contains("OTHER CHARGES :")) && txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION"))
                    {
                        strText = _lines[_index - 1].Trim();
                        double _dFreight = dba.ConvertObjectToDouble(strText);
                        if (_dFreight > 0)
                            txtFreight.Text = _dFreight.ToString("0.00");

                        break;
                    }
                    else if ((strText.ToUpper().Contains("TOTAL") || strText.ToUpper().Contains("ROUND") || strText.ToUpper().Contains("R / O")) && !strText.ToUpper().Contains(" TOTAL :") && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("JANAK GARMENTEX") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES") && !txtPurchaseParty.Text.Contains("LADLEE WESTERN OUTFITS") && !txtPurchaseParty.Text.Contains("MOTI FASHIONS") && !txtPurchaseParty.Text.Contains("TANEJA FASHION") && !txtPurchaseParty.Text.Contains("KHANDELWAL BROTHERS") && !txtPurchaseParty.Text.Contains("VIPIN COLLECTION") && !txtPurchaseParty.Text.Contains("J.D. FASHION WEAR") && !txtPurchaseParty.Text.Contains("HARDIK TEXTILE") && !txtPurchaseParty.Text.Contains("SONY CREATION") && !txtPurchaseParty.Text.Contains("MAUZ FASHIONS") && !txtPurchaseParty.Text.Contains("CLASSIN APPARELS") && !txtPurchaseParty.Text.Contains("HELLO BROTHER FASHION") && !txtPurchaseParty.Text.Contains("MISHU ENTERPRISES") && !txtPurchaseParty.Text.Contains("ARPIT FASHION") && !txtPurchaseParty.Text.Contains("AASHI COLLECTION") && !txtPurchaseParty.Text.Contains("WORLD CHOICE") && !txtGSTNo.Text.Contains("07AXGPG4663A1ZT"))
                        return true;
                    else if ((strText.ToUpper().Contains("FREIGHT") || strText.ToUpper().Contains("FORWARDING")) && !txtPurchaseParty.Text.Contains("KC GARMENTS") && !txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                    {
                        if (txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                        {
                            string[] _data = strText.Trim().Split(' ');
                            if (_data.Length > 2)
                                txtFreight.Text = _data[0];
                            if (txtFreight.Text == "(+)")
                                txtFreight.Text = "";
                        }
                        else if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                        {
                            string[] _data = _lines[_index + 1].Trim().Split(' ');
                            if (_data.Length > 0)
                            {
                                txtFreight.Text = _data[_data.Length - 1].Trim();
                                if (txtFreight.Text == "")
                                {
                                    _data = _lines[_index].Trim().Split(' ');
                                    if (_data.Length > 0)
                                        txtFreight.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1].Trim()).ToString("0.00");
                                }
                            }
                        }
                        else
                        {
                            string[] _data = strText.Split(' ');
                            if (_data.Length > 1)
                                txtFreight.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                        }
                    }
                    else if (strText == "CGST" && (txtGSTNo.Text.Contains("07AXGPG4663A1ZT")) && _index<_lines.Length-3)
                    {
                        strText = _lines[_index +3].Trim();
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 0)
                            txtFreight.Text = dba.ConvertObjectToDouble(_data[_data.Length - 1]).ToString("0.00");
                        break;
                    }
                    else if (strText.ToUpper().Contains("OTHER CHARGE") && txtPurchaseParty.Text.Contains("MOTI FASHIONS"))
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtFreight.Text = _data[_data.Length - 1];
                    }
                    else if (strText.ToUpper().Contains("PC. DISCOUNT") && txtPurchaseParty.Text.Contains("NAVRANG ENTERPRISES"))
                    {
                        string[] _data = strText.Split(' ');
                        if (_data.Length > 1)
                            txtPcsAmt.Text = _data[0];
                    }
                    else if (strText.ToUpper().Contains("DISCOUNT PER. PC.") && txtPurchaseParty.Text.Contains("TANEJA FASHION"))
                    {
                        string _strText = strText.Replace("Amount", "").Replace("Chargeable", "").Replace("(", "").Replace(")", "").Replace("In Words", "").Replace(":", "").Trim();
                        string[] _data = _strText.Split(' ');
                        if (_data.Length > 1)
                            txtPcsAmt.Text = _data[0];
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
                                txtFreight.Text = _data[0];
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
                            //if (txtFreight.Text != "0.00")
                            //{
                            strText = _lines[_index + 1].Trim();
                            string[] _data = strText.Split(' ');
                            if (_data.Length > 0)
                            {
                                txtFreight.Text = (dba.ConvertObjectToDouble(_data[0]) + dba.ConvertObjectToDouble(txtFreight.Text)).ToString("0.00");
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
                                int __index = str.Length - 2;
                                for (; __index < str.Length; __index++)
                                {
                                    if (strItem != "")
                                        strItem += " ";
                                    strItem += str[__index];
                                }
                            }
                        }
                        else if ((txtGSTNo.Text.Contains("07AXGPG4663A1ZT")) && str.Length > 10)
                        {
                            if (str[0] == _lineIndex.ToString())
                            {
                                strHSNCode = str[2].Trim();
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                if (strHSNCode.Length == 4 && str[3].Contains("."))
                                {
                                    strItem = str[1];
                                    strQty = str[3];
                                    strRate = str[4];
                                }
                                else
                                {
                                    strHSNCode = str[3].Trim();
                                    strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");

                                    strItem = str[1] + " " + str[2];
                                    strQty = str[4];
                                    strRate = str[5];
                                }
                            }
                        }
                        else if ((txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("N.D. FASHION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NEELKANTH ENTERPRISES") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtGSTNo.Text.Contains("07EHOPK4815E1Z8")) && str.Length > 10)
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
                                if (str.Length > 1)
                                    strHSNCode = str[1].Trim();
                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                if (strHSNCode == "")
                                    strHSNCode = str[0].Trim();
                                else
                                    strItem = str[0].Trim();
                                if (strItem != "")
                                {
                                    for (int __index = 1; __index < str.Length; __index++)
                                    {
                                        if (strItem != "")
                                            strItem += " ";
                                        strItem += str[__index];
                                    }
                                }
                                if (strItem != "" && strDescription != "")
                                    strItem += " " + strDescription;
                                strItem = strItem.Trim();
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
                                if (str.Length > 11)
                                {
                                    strItem = str[1].Trim();
                                    strQty = str[2].Trim();
                                    strRate = str[3].Trim();
                                    strHSNCode = str[str.Length - 1].Trim();
                                }
                                else
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
                        else if (txtPurchaseParty.Text.Contains("AASHI COLLECTION"))
                        {
                            if (str[0] == _lineIndex.ToString())
                            {
                                if (str.Length > 4)
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
                                else
                                {
                                    strItem = _lines[_index + 1].Trim();
                                    strRate = _lines[_index + 7].Trim();
                                    str = _lines[_index + 8].Trim().Split(' ');
                                    strQty = str[0];
                                    strHSNCode = _lines[_index + 11].Trim();
                                    _index += 10;
                                }
                            }
                        }
                        else if ((txtPurchaseParty.Text.Contains("S.R CREATION") || txtGSTNo.Text.Contains("07BQWPK0733R2ZZ")) && str.Length > 4)
                        {
                            if (str[str.Length - 5] == _lineIndex.ToString())
                            {
                                if (txtGSTNo.Text.Contains("07BQWPK0733R2ZZ"))
                                {
                                    strRate = str[str.Length - 2].Trim();
                                    strQty = str[0].Trim();
                                    strHSNCode = str[2].Trim();
                                    // strItem = str[str.Length - 4].Trim();
                                    for (int _i = str.Length - 4; _i < str.Length - 2; _i++)
                                    {
                                        strItem += str[_i].Trim() + " ";
                                    }
                                    strItem = strItem.Trim();

                                }
                                else
                                {
                                    strQty = str[str.Length - 2].Trim();
                                    strHSNCode = str[str.Length - 3].Trim();
                                    strItem = str[str.Length - 4].Trim();

                                    str = _lines[_index + 1].Trim().Split(' ');
                                    strRate = str[0].Trim();
                                }
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
                        else if (txtPurchaseParty.Text.Contains("FCB GARMENT") && str.Length > 4)
                        {
                            if (str.Length > 5)
                            {
                                if (str[str.Length - 6] == _lineIndex.ToString())
                                {
                                    strQty = str[0].Trim();
                                    strHSNCode = str[3].Trim();
                                    strRate = str[str.Length - 3].Trim();
                                    for (int _i = 6; _i < str.Length - 7; _i++)
                                    {
                                        strItem += str[_i].Trim() + " ";
                                    }
                                    strItem = strItem.Trim();
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

                                    if (_data.Length < 15)
                                    {
                                        strQty = _data[_data.Length - 1].Trim();

                                        strItem = strText.Replace(_lineIndex + " ", "").Replace(strQty, "");
                                        _data = _lines[_index + 1].Trim().Split(' ');
                                        if (_data.Length > 2)
                                            strRate = _data[1].Trim();
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
                                        strQty = _data[_data.Length - 7].Trim();
                                        strRate = _data[_data.Length - 5].Trim();

                                        for (int _intIndex = 1; _intIndex < _data.Length - 8; _intIndex++)
                                        {
                                            strItem += " " + _data[_intIndex];
                                        }
                                        strItem = strItem.Trim();
                                        if (_index < _lines.Length - 1)
                                        {
                                            strText = _lines[_index + 1].Trim();
                                            strHSNCode = strText;
                                        }
                                        _index -= 2;
                                    }
                                }
                                else
                                {
                                    strText = _lines[_index - 1].Trim();
                                    string[] _data = strText.Split(' ');
                                    if (_data.Length > 0)
                                        strHSNCode = _data[_data.Length - 1].Trim();
                                    if (_index < _lines.Length - 1)
                                    {
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
                                }
                                _index += 2;
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
                                    if (txtPurchaseParty.Text.Contains("VARDHMAN GARMENTS"))
                                    {
                                        if (strItem.ToUpper().Contains("FREIGHT"))
                                            strItem = strHSNCode = "";
                                    }
                                    else
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
                                            strHSNCode = "";
                                            string _strData = __str[0];
                                            _data = _strData.Split(' ');
                                            if (_data.Length > 1)
                                                strHSNCode = _data[_data.Length - 1];
                                            if (strHSNCode.Length == 4)
                                                strHSNCode = System.Text.RegularExpressions.Regex.Replace(strHSNCode, "[^0-9.]", "");
                                            if (strHSNCode.Length != 4)
                                            {
                                                strHSNCode = __str[1];
                                            }
                                            else
                                                strQty = __str[__str.Length - 1].Trim();

                                            strItem = __str[0].Replace(_lineIndex + ".", "").Replace(strHSNCode, "").Trim();

                                        }
                                        else
                                            strItem = strText.Replace(_lineIndex + ".", "").Trim();

                                        strText = _lines[_index + 1].Trim();
                                        _data = strText.Replace("  ", " ").Split(' ');
                                        if (strHSNCode != "")
                                        {
                                            if (_data.Length > 4)
                                            {
                                                if (_data[2].Contains(".00"))
                                                {
                                                    strQty = _data[2].Trim();
                                                    strRate = _data[3].Trim();
                                                }
                                                else if (_data[3].Contains(".00"))
                                                {
                                                    strQty = _data[3].Trim();
                                                    strRate = _data[4].Trim();
                                                }
                                                else
                                                    strRate = _data[_data.Length - 3].Trim();
                                            }
                                        }
                                        if (strQty == "" && strRate == "")
                                        {
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
                                                {
                                                    if (_data[2].Contains(".00"))
                                                    {
                                                        strQty = _data[2].Trim();
                                                        strRate = _data[3].Trim();
                                                    }
                                                    else if (_data[3].Contains(".00"))
                                                    {
                                                        strQty = _data[3].Trim();
                                                        strRate = _data[4].Trim();
                                                    }
                                                    else
                                                        strRate = _data[_data.Length - 3].Trim();
                                                }
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
                                        }
                                        //_index += 4;
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
                if (txtGSTNo.Text=="" && strText.Length==15 && !strText.Contains("AAYCS8982Q"))
                {
                    bool chk = System.Text.RegularExpressions.Regex.IsMatch(strText, @"\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}");
                    if(chk)
                    {
                        SetSupplierDetailsWithGSTNo(strText.Trim());
                    }
                }
                else if ((strText.Contains("GSTIN  :") || strText.Contains("GSTIN/UIN") || strText.Contains("GSTIN. :") || strText.Contains("GSTIN: ") || strText.Contains("GSTIN :")) && !strText.Contains("AAYCS8982Q"))
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
                            if(strGSTNO.Length>9)
                            SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                        }
                        if (txtGSTNo.Text == "")
                        {
                            _strText = _lines[_index];
                            strGST = _strText.Trim().Split(' ');
                            _strText = strGST[0];
                            if (_strText.Length < 10)
                                _strText = strGST[strGST.Length - 1];
                            SetSupplierDetailsWithGSTNo(_strText.Trim());
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
                        if(strGSTNO.Length>10)
                        SetSupplierDetailsWithGSTNo(strGSTNO.Trim());
                    }
                    if (txtGSTNo.Text == "")
                    {
                        string _strText = _lines[_index + 1];
                        strGST = _strText.Trim().Split(' ');
                        string strGSTNO = strGST[0].Trim();
                        if (strGSTNO.Length > 10)
                            SetSupplierDetailsWithGSTNo(strGSTNO);

                        _strText = _lines[_index + 2];
                        strGST = _strText.Trim().Split(' ');
                        strGSTNO = strGST[0].Trim();
                        if (strGSTNO.Length > 10)
                            SetSupplierDetailsWithGSTNo(strGSTNO);
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
                else if (strText.ToUpper().Contains("CUSTOMER NO.") && txtPurchaseDate.Text.Length != 10)
                {
                    string _strText = _lines[_index + 3];
                    string[] strDate = _strText.Trim().Split(' ');
                    if (strDate.Length > 0)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate[0]);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO/ DATE"))
                {
                    if (txtPurchaseParty.Text.Contains("FULLTOSS"))
                    {
                        txtPurchaseInvoiceNo.Text = _lines[_index + 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, _lines[_index + 2]);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.    :"))
                {
                    if (txtPurchaseInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtPurchaseInvoiceNo.Text = strInvoiceNo[4].Trim();
                            string strInv = Regex.Replace(txtPurchaseInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtPurchaseInvoiceNo.Text += strInvoiceNo[5].Trim();
                        }
                        if (txtPurchaseInvoiceNo.Text == "")
                        {
                            _strText = strText.Replace("                     ", " ");
                            strInvoiceNo = _strText.Trim().Split(' ');
                            if (strInvoiceNo.Length > 1)
                                txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO.     :"))
                {
                    if (txtPurchaseInvoiceNo.Text == "")
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                        string[] strInvoiceNo = _strText.Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                        {
                            txtPurchaseInvoiceNo.Text = strInvoiceNo[0].Trim();
                            string strInv = Regex.Replace(txtPurchaseInvoiceNo.Text, "[^0-9]", "");
                            if (strInv == "" && strInvoiceNo.Length > 4)
                                txtPurchaseInvoiceNo.Text += strInvoiceNo[5].Trim();
                            if (_strText.ToUpper().Contains("DATED"))
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, strInvoiceNo[strInvoiceNo.Length - 2]);
                                txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO. :"))
                {
                    string _strText = strText.Replace("  ", " ");
                    string[] strInvoiceNo = _strText.Trim().Split(' ');

                    if (txtPurchaseInvoiceNo.Text == "")
                        txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();

                    if (txtPurchaseInvoiceNo.Text.Trim() == ":")
                        txtPurchaseInvoiceNo.Text = "";
                    if (txtPurchaseInvoiceNo.Text == "" && (txtPurchaseParty.Text.Contains("HARDIK TEXTILE") || txtPurchaseParty.Text.Contains("SONY CREATION")))
                    {
                        strInvoiceNo = _lines[_index + 1].Trim().Split(' ');
                        if (strInvoiceNo.Length > 1)
                            txtPurchaseInvoiceNo.Text = strInvoiceNo[0].Trim();
                    }
                    else if (txtPurchaseInvoiceNo.Text.Contains("CREDIT") && strInvoiceNo.Length > 3)
                        txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 2].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE NO."))
                {
                    if (txtPurchaseInvoiceNo.Text == "")
                    {
                        if (_lines[_index + 1].ToUpper().Contains(": CUSTOMER NAME"))
                        {
                            txtPurchaseInvoiceNo.Text = _lines[_index - 2];
                            if (!txtPurchaseInvoiceNo.Text.Contains("KC"))
                                txtPurchaseInvoiceNo.Text = _lines[_index - 3];
                        }
                        else
                        {
                            txtPurchaseInvoiceNo.Text = _lines[_index + 1].Replace("Credit", "").Trim();
                            if (txtPurchaseInvoiceNo.Text == ":")
                                txtPurchaseInvoiceNo.Text = "";
                            if(txtPurchaseInvoiceNo.Text.ToUpper().Contains("DATE OF INVOICE"))
                            {
                                txtPurchaseInvoiceNo.Text = _lines[_index + 2].Trim();
                                string __strdate= _lines[_index + 3].Trim();
                                if (__strdate.Contains("20"))
                                {
                                    DateTime _iDate = DateTime.Now;
                                    ConvertDateTime(ref _iDate, __strdate);
                                    txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                                }
                            }
                            if ((txtPurchaseInvoiceNo.Text.Contains("2020") || txtPurchaseInvoiceNo.Text.Contains("2021")) && txtPurchaseInvoiceNo.Text.Trim().Length < 11)
                            {
                                DateTime _iDate = DateTime.Now;
                                ConvertDateTime(ref _iDate, txtPurchaseInvoiceNo.Text.Trim());
                                txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                                txtPurchaseInvoiceNo.Text = "";
                            }
                            else
                            {
                                string _strText = strText.Replace("  ", " ");
                                string[] strGST = _strText.Split(' ');
                                if ((txtPurchaseParty.Text.Contains("LADLEE WESTERN") || txtPurchaseParty.Text.Contains("M. BEST CREATION") || txtPurchaseParty.Text.Contains("I.D. CREATION") || txtPurchaseParty.Text.Contains("ARPIT FASHION")) && strGST.Length > 3)
                                    txtPurchaseInvoiceNo.Text = strGST[3];
                                else if (txtPurchaseParty.Text.Contains("RIDDHI SIDDHI GARMENTS") || txtPurchaseParty.Text.Contains("SIMRAN APPARELS"))
                                {
                                    strGST = _lines[_index + 1].Trim().Split(' ');
                                    if(strGST.Length>1)
                                    txtPurchaseInvoiceNo.Text = strGST[1];
                                    else
                                    {
                                        strGST = _lines[_index +2].Trim().Split(' ');
                                        if (strGST.Length > 1)
                                            txtPurchaseInvoiceNo.Text = strGST[0];
                                    }
                                }
                                else if (txtPurchaseParty.Text.Contains("PUNEET READYMADE") || txtPurchaseParty.Text.Contains("THAKUR COLLECTION") || txtPurchaseParty.Text.Contains("NILAMBRI FASHION OPC PVT LTD") || txtPurchaseParty.Text.Contains("SANSKAR TRADING") || txtPurchaseParty.Text.Contains("AMAN CREATION") || txtPurchaseParty.Text.Contains("R.S. TRADERS") || txtPurchaseParty.Text.Contains("KIRAN FABRICS") || txtPurchaseParty.Text.Contains("P.P INTERNATIONAL") || txtPurchaseParty.Text.Contains("WORLD SAHAB") || txtGSTNo.Text.Contains("07EHOPK4815E1Z8") || txtGSTNo.Text.Contains("07BQWPK0733R2ZZ")) 
                                {
                                    txtPurchaseInvoiceNo.Text = strGST[0];
                                }
                                if (_strText.ToUpper().Contains("DATED"))
                                {
                                    txtPurchaseInvoiceNo.Text = "";
                                    if (strGST.Length > 1 && txtPurchaseInvoiceNo.Text == "")
                                        txtPurchaseInvoiceNo.Text = strGST[2];

                                    DateTime _iDate = DateTime.Now;
                                    ConvertDateTime(ref _iDate, strGST[strGST.Length - 1].Trim());
                                    txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                                }
                                else
                                {
                                    if (strGST.Length > 1 && txtPurchaseInvoiceNo.Text == "")
                                        txtPurchaseInvoiceNo.Text = strGST[strGST.Length - 1];
                                    if (txtPurchaseInvoiceNo.Text.Contains("BILL"))
                                        txtPurchaseInvoiceNo.Text = _lines[_index + 2];
                                    if (txtPurchaseInvoiceNo.Text.Contains("DATE"))
                                        txtPurchaseInvoiceNo.Clear();
                                }
                            }
                        }
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO  :") || strText.ToUpper().Contains("INVOICE NO :"))
                {
                    string[] strGST = strText.Split(' ');
                    if (strGST.Length > 1 && txtPurchaseDate.Text.Length != 10)
                    {
                        txtPurchaseInvoiceNo.Text = strGST[strGST.Length - 1];
                        DateTime _iDate = DateTime.Now;
                        strGST = _lines[_index + 1].Split(' ');
                        string strDate = strGST[0];
                        if (strDate.Length < 9)
                            strDate = strGST[strGST.Length - 1];
                        ConvertDateTime(ref _iDate, strDate);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                        if (txtPurchaseInvoiceNo.Text == ":")
                            txtPurchaseInvoiceNo.Text = "";
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NO:             DT."))
                {
                    string _strText = strText.Replace("  ", " ").Replace("  ", " ").Trim();
                    string[] strInvoiceDate = _strText.Trim().Split(' ');
                    if (txtPurchaseDate.Text.Length != 10)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                    _strText = _lines[_index + 1].Trim();
                    string[] strInvoiceNo = _strText.Trim().Split(' ');
                    if (txtPurchaseInvoiceNo.Text == "")
                        txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    _bBonny = true;
                }
                else if (strText.ToUpper().Contains("INVOICE NO"))
                {
                    if (strText.Contains(": "))
                    {
                        string[] strInvoiceNo = strText.Replace(": ", "").Trim().Split(' ');
                        txtPurchaseInvoiceNo.Text = strInvoiceNo[0];
                    }
                    else if (txtPurchaseInvoiceNo.Text == "")
                        txtPurchaseInvoiceNo.Text = _lines[_index + 1].Replace(": ", "");
                }
                else if (strText.ToUpper().Contains("SUPPLIER'S REF.") && txtPurchaseParty.Text.Contains("SAM TRADERS"))
                {
                    if (txtPurchaseInvoiceNo.Text.Contains("DELIVERY NOTE") || txtPurchaseInvoiceNo.Text == "")
                        txtPurchaseInvoiceNo.Text = _lines[_index + 1];
                }
                else if (strText.ToUpper().Contains("SARAOGI SUPER SALES PVT. LTD") && txtGSTNo.Text.Contains("07AADCJ2544A1Z3"))
                {
                    if (txtPurchaseInvoiceNo.Text == "")
                        txtPurchaseInvoiceNo.Text = _lines[_index - 1].Trim();
                }
                else if (strText.ToUpper().Contains("DATE OF INVOICE :"))
                {
                    string[] strInvoiceDate = strText.Replace("                    ", " ").Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[4].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[3].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains(": INVOICE DATE"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[0].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE NUMBER   :"))
                {
                    string[] strInvoiceNo = strText.Trim().Split(' ');
                    if (strInvoiceNo.Length > 1)
                        txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                }
                else if (strText.ToUpper().Contains("INVOICE DATE         :"))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE DATE "))
                {
                    string[] strInvoiceDate = strText.Trim().Split(' ');
                    if (strInvoiceDate.Length > 1)
                    {
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strInvoiceDate[2].Trim());
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Trim() == "INVOICE" && _index > 0)
                {
                    if (txtPurchaseInvoiceNo.Text == "ISSUE DATE :")
                        txtPurchaseInvoiceNo.Text = "";

                    if (txtPurchaseInvoiceNo.Text == "" && txtPurchaseDate.Text.Length != 10)
                    {
                        string[] strGST = _lines[_index + 1].Split(' ');
                        txtPurchaseInvoiceNo.Text = strGST[0];

                        strGST = _lines[_index + 2].Split(' ');
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strGST[0]);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("INVOICE") && !strText.ToUpper().Contains("TAX INVOICE"))
                {
                    if (txtPurchaseParty.Text.Contains("DADU TEXTILES LLP"))
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');

                        if (txtPurchaseInvoiceNo.Text == "")
                            txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    }
                    else if (txtPurchaseParty.Text.Contains("GEX GARMENTS"))
                    {
                        string _strText = strText.Replace("  ", " ");
                        string[] strInvoiceNo = _strText.Trim().Split(' ');

                        if (txtPurchaseInvoiceNo.Text == "")
                            txtPurchaseInvoiceNo.Text = strInvoiceNo[strInvoiceNo.Length - 1].Trim();
                    }
                    else if(strText.Trim().ToUpper()=="DATE OF INVOICE")
                    {
                        string[] str = _lines[_index + 1].Split(' ');
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, str[0]);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATED") && !strText.ToUpper().Contains("LR DATED"))
                {
                    if (_lines[_index + 1] != "" && !_lines[_index + 1].Contains("Delivery Note"))
                    {
                        if (txtPurchaseDate.Text.Length != 10)
                        {
                            DateTime _iDate = DateTime.Now;
                            string[] strInvoiceDate = strText.Trim().Split(' ');
                            if (strInvoiceDate[strInvoiceDate.Length - 1].Length > 6)
                                ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                            else
                                ConvertDateTime(ref _iDate, _lines[_index + 1]);
                            txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("DATE.   :"))
                {
                    if (txtPurchaseDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        DateTime _iDate = DateTime.Now;
                        string[] strInvoiceDate = _strText.Trim().Split(' ');
                        if (strInvoiceDate.Length > 2)
                            ConvertDateTime(ref _iDate, strInvoiceDate[2]);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if (strText.ToUpper().Contains("DATE :"))
                {
                    if (txtPurchaseDate.Text.Length != 10)
                    {
                        string _strText = strText.Replace("  ", " ").Replace("  ", " ");
                        if (!_strText.Contains("DATE"))
                        {
                            DateTime _iDate = DateTime.Now;
                            string[] strInvoiceDate = _strText.Trim().Split(' ');
                            if (strInvoiceDate.Length > 2)
                                ConvertDateTime(ref _iDate, strInvoiceDate[strInvoiceDate.Length - 1]);
                            txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                        }
                    }
                }
                else if (strText.ToUpper().Contains("DATE"))
                {
                    if (txtPurchaseDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index + 1].Replace(":", "").Trim();
                        if (strDate.Length > 7 && (strDate.Contains("2019") || strDate.Contains("2020") || strDate.Contains("2021")))
                        {
                            DateTime _iDate = DateTime.Now;
                            ConvertDateTime(ref _iDate, strDate);
                            txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
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
                        txtPurchaseInvoiceNo.Text = _lines[_index + 1];
                        _itemIndex = _index + 20;
                        break;
                    }
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("JOLLY FASHIONS"))
                {
                    txtPurchaseInvoiceNo.Text = _lines[_index - 1];
                    _itemIndex = _index + 20;
                    break;
                }
                else if (strText.ToUpper().Contains("SL") && txtPurchaseParty.Text.Contains("J.D. FASHION WEAR"))
                {
                    if (txtPurchaseDate.Text.Length != 10)
                    {
                        string strDate = _lines[_index - 4];
                        DateTime _iDate = DateTime.Now;
                        ConvertDateTime(ref _iDate, strDate);
                        txtPurchaseDate.Text = _iDate.ToString("dd/MM/yyyy");
                    }
                }
                else if ((strText.ToUpper().Contains("ME/") || strText.ToUpper().Contains("HLLP")) && (strText.ToUpper().Contains("19-20") || strText.ToUpper().Contains("20-21")))
                {
                    txtPurchaseInvoiceNo.Text = strText.Trim();
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
                    _itemIndex = _index + 1;
                    if (txtPurchaseParty.Text.Contains("AGARWAL COLLECTION"))
                        _itemIndex--;
                    break;
                }
                else if (strText.ToUpper().Contains("DESCRIPTION"))
                {
                    _itemIndex = _index + 1;
                    if (txtPurchaseParty.Text.Contains("S.R CREATION"))
                        _itemIndex--;
                    break;
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
                    GetPartyDhara();
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
                    GetPartyDhara();
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

        private bool CheckItemNameExistence(ref string strItemName, ref string strHSNCode)
        {
            btnImport.Enabled = false;
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
                btnImport.Enabled = true;
            }
            catch { }
            return true;
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

                for (int i = 0; i < strMainItemName.Length; i++)
                {
                    if (i == 0)
                        strItemQuery += " and DesignName Like('" + strMainItemName[i] + "%') ";
                    else
                        strItemQuery += " and DesignName Like('%" + strMainItemName[i] + "%') ";
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
                
                if (strItemQuery != "" || (strItemName != "" && strMainItemName == ""))
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

        /// <OLD summary>
        /// strItemName, @"[\d-]", string.Empty).Trim();

        //        string[] strPartItem = strReplacedItemName.Split(',');
        //                if (strPartItem.Length == 1)
        //                    strPartItem = strReplacedItemName.Split('-');
        //                if (strPartItem.Length == 1)
        //                    strPartItem = strReplacedItemName.Split(' ');

        //                if (strPartItem.Length > 1)
        //                {
        //                    strFirstItemName = strPartItem[0];
        //                    if (strFirstItemName.Length == 1)
        //                        strFirstItemName = strPartItem[1];
        //                    if (strFirstItemName.Length< 2 && strPartItem.Length> 2)
        //                        strFirstItemName = strPartItem[2];

        //                    strSecondItemName = strPartItem[1];
        //                    if (strSecondItemName.Length == 1 && strPartItem.Length > 2)
        //                        strSecondItemName = strPartItem[2];
        //                    if (strSecondItemName.Length< 2 && strPartItem.Length> 3)
        //                        strSecondItemName = strPartItem[3];

        //                    strItemsQuery = " UNION ALL Select ItemName,5 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('" + strFirstItemName.Replace("/", "").Trim() + "%') and ItemName Like('%" + strHSNCode + "')  UNION ALL Select ItemName,5 SerialNo from Items Where SubGroupName='PURCHASE' and ItemName Like('" + strSecondItemName.Replace("/", "").Trim() + "%') and ItemName Like('%" + strHSNCode + "') ";
        //                }
        //                if (strReplacedItemName.Length > 2)
        //                {
        //                    strReplaceItemQuery = " UNION ALL Select ItemName,1 SerialNo from ItemMapping Where DesignName Like('%" + strReplacedItemName + "%') and UpdatedBy Like('%" + strHSNCode + "') UNION ALL Select ItemName,6 SerialNo from Items Where ItemName Like('%" + strReplacedItemName + "%') and ItemName Like('%" + strHSNCode + "')   ";
        //                }

        //string strQuery = " Select TOP 1 * from ( Select ItemName,0 SerialNo from ItemMapping Where DesignName Like('" + strMainItemName + "') and UpdatedBy Like('" + strHSNCode + "') UNION ALL Select ItemName,3 SerialNo from ItemMapping Where DesignName Like('%" + strItemName + "%') and DesignName Like('%" + strHSNCode + "%') and UpdatedBy Like('" + strHSNCode + "') UNION ALL "
        //                + " Select ItemName,4 SerialNo from Items Where SubGroupName='PURCHASE' and  ItemName Like('%" + strItemName + "%') and ItemName Like('%" + strHSNCode + "') " + strItemsQuery + "  " + strReplaceItemQuery + " UNION ALL Select ItemName,0 SerialNo from Items Where SubGroupName='PURCHASE' and  ((ItemName Like('" + strItemName.Replace("T SHIRT", "T-SHIRT").Replace("T. SHIRT", "T-SHIRT") + "%')) and ItemName Like('%" + strHSNCode + "%'))  UNION ALL Select ItemName,1 SerialNo from Items Where SubGroupName='PURCHASE' and  ((ItemName Like('%" + strItemName + "%') OR ItemName Like('%" + strItemName.Replace("T SHIRT", "T-SHIRT") + "%')) and ItemName Like('%" + strHSNCode + "%'))  "
        //                + " )_Sale Order By SerialNo ";

        //object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
        //                return Convert.ToString(objValue);
        /// </summary>
        /// <param name="strItem"></param>
        /// <returns></returns>

        private string GetHSNCodeFromItem(string strItem)
        {
            string strQuery = "";
            strQuery = " Select _IGM.HSNCode from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName Where _IM.ItemName='" + strItem + "' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            return Convert.ToString(objValue);
        }

        private void ClearAllTextForPDF()
        {
            _strBillType = txtPurchaseParty.Text = txtNoOfCase.Text = txtGSTNo.Text = "";
            lblQty.Text = lblTotalMTR.Text = "0";
            _strPDFFilePath = _strSubPartyName = _strSupplierName = "";
            txtPurchaseInvoiceNo.Text = txtPurchaseDate.Text = lblCreatedBy.Text = "";
            txtOtherAmt.Text = txtDisPer.Text = txtDiscountAmt.Text = txtTaxPer.Text = txtTaxAmt.Text = lblGrossAmt.Text = lblNetAmt.Text = txtSpecialDiscAmt.Text = txtSpeDiscPer.Text = txtPcsAmt.Text = txtTaxFree.Text = txtPackingAmt.Text = txtFreight.Text = "0.00";
            txtSignAmt.Text = "-";
            txtPcsType.Text = "PETI";
            rdoDirect.Checked = true;
            txtSalesParty.BackColor = Color.White;
            dgrdPending.Rows.Clear();
            dgrdRelatedParty.Rows.Clear();

            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;
            dOldNetAmt = dTotalAmount = _dCancelQty_ByUser = 0;

        }

        private bool FinalConfirmation()
        {
            try
            {
                UserAgreement _objAgree = new UserAgreement();
                _objAgree.txtSalesParty.Text = txtSalesParty.Text;
                _objAgree.txtSubParty.Text = txtSubParty.Text;
                _objAgree.txtSupplierGSTNo.Text = txtGSTNo.Text;
                _objAgree.txtPurchaseParty.Text = txtPurchaseParty.Text;
                _objAgree.txtPurchaseInvoiceNo.Text = txtPurchaseInvoiceNo.Text;
                _objAgree.txtInvoiceDate.Text = "Date : " + txtPurchaseDate.Text;
                _objAgree.txtNetAmt.Text = lblNetAmt.Text;
                _objAgree.txtTotalQty.Text = lblQty.Text;

                double dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);

                if (txtPurchaseType.Text.Contains("L/"))
                    _objAgree.txtCGSTAmt.Text = _objAgree.txtSGSTAmt.Text = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                else
                    _objAgree.txtIGSTAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                this.Hide();
                _objAgree.ShowDialog();
                this.Show();
                return _objAgree._bConfirmation;
            }
            catch { }
            return false;
        }
    }
}


