using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;
using CrystalDecisions.CrystalReports.Engine;

namespace SSS
{
    public partial class SaleBook : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strOldPartyName = "", strAmendedQuery = "", strAllAttachedBillNo = "";
        public string _strPSalesParty = "", _strPSubParty = "", _strPackingType = "", strNewAddedGRSNO = "", strOldLRNumber = "";

        double dOldNetAmt = 0, dGreenTaxAmount = 0, dForwardingCharges = 0, dExtraCharges = 0, _dPerCasePackingAmt = 0;
        SendSMS objSMS;
        bool newStatus = false;
        DataTable _dtPendingStock = null;
        public SaleBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            GetStartupData(true);
        }

        public SaleBook(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            GetStartupData(false);
            newStatus = bStatus;
        }

        public SaleBook(string strCode, string strBillNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            txtBillCode.Text = strCode;
            txtBillNo.Text = strBillNo;
            BindRecordWithControl(strBillNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select SBillCode,(Select ISNULL(MAX(BillNo),0) from SalesRecord Where BillCode=SBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtBillCode.Text = Convert.ToString(dt.Rows[0]["SBillCode"]);
                    strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                }
                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
            }
            catch
            {
            }
        }

        private void SaleBook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (pnlShowSaleBill.Visible)
                    pnlShowSaleBill.Visible = false;
                else if (pnlPendingStock.Visible)
                    pnlPendingStock.Visible = false;
                else if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else if (btnClose.Enabled)
                {
                    this.Close();
                }
            }
            else if (e.KeyCode == Keys.F8)
            {
                if (this.MdiParent != MainPage.mymainObject && newStatus && (MainPage.mymainObject.bReportSummary))
                {
                    ShowReportSummary objshowAllReport = new ShowReportSummary(txtSalesParty.Text);
                    objshowAllReport.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objshowAllReport.TopLevel = true;
                    objshowAllReport.Show();
                }
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (e.Modifiers != Keys.Shift)
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
                    }
                }
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesRecord Where BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesRecord Where BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            if (txtBillNo.Text != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
                else
                    BindLastRecord();
            }
            else
                ClearAllText();
        }

        private void BindPreviousRecord()
        {
            if (txtBillNo.Text != "")
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindRecordWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private void BindRecordWithControl(string strSerialNo)
        {
            try
            {
                DisableAllControls();
                btnAdd.TabStop = true;
                ClearAllText();
                if (txtBillCode.Text != "" && strSerialNo != "")
                {
                    string strQuery = "";
                    strQuery += " Select SR.*,dbo.GetFullName(SalePartyID) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') HParty,CONVERT(varchar,BillDate,103)BDate,CONVERT(varchar,ISNULL(PackingDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)PDate,CONVERT(varchar,ISNULL(LrDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)LDate,Round((CASE WHEN CAST(GrossAmt as money)!=0 then CAST(((CAST(OtherPer as Money)*100)/CAST(GrossAmt as Money)) as float) else '0' end),2) OtherPercentage, " //,CONVERT(varchar,DATEADD(dd,Cast(DueDays as int),BillDate),103) DDate
                             + " (Select StandardLogin from CompanySetting Where CompanyName='" + MainPage.strLoginName + "') LoginType,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,BillDate))) LockType,ISNULL(TR.GreenTaxAmt,0)GTaxAmt,ISNULL(TR.ForwardingCharges,0)ForwardingCharges,ISNULL(TR.ExtraCharges,0)ExtraCharges,ISNULL(PAgent,'DIRECT') PAgent from SalesRecord  SR OUTER APPLY (Select Top 1 (Description_1+' '+Name)PAgent from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SR.Description_1)SM OUTER APPLY(Select TP.GreenTaxAmt,TP.ForwardingCharges,TP.ExtraCharges from Transport TP Where TP.TransportName=Transport)TR Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                             + " Select *,ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') PParty,ISNULL(PurchaseStatus,0)PStatus from SalesEntry Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " Union All "
                             + " SELECT [ID],'' [BillCode],0 [BillNo], [SalesFrom],SerialNo as [GRSNo],ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') [SupplierName],[Pieces],Item [Items],'0' [Discount],'' [DiscountStatus],'' [SNDhara],'0' [Amount],'0' [Packing],'0' [Freight],'0' [Tax],'0' [TotalAmt],'' [PBill],'0' [RemPcs],'' [BillDate],'' [PurchaseBill],'' [Personal],[InsertStatus],[UpdateStatus],[PurchasePartyID],0 as PurchaseStatus,ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') PParty,0 as PStatus FROM [dbo].[GoodsReturned] WHere AdjustedSaleBillNumber='" + txtBillCode.Text + " " + strSerialNo + "' "
                             + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt != null)
                        {
                            BindSalesRecordDetails(dt);
                        }
                        dt.Clear();
                        dt = ds.Tables[1];
                        if (dt != null)
                        {
                            BindSalesEntryDetails(dt);
                        }
                        BindGSTDetailsWithControl(ds.Tables[2]);
                        // CalculateGridAmount();
                        GetPendingOnlyGRRecord();
                    }
                }
            }
            catch
            {
                btnEdit.Enabled = false;
            }
        }

        private void BindSalesRecordDetails(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtBillNo.Text = Convert.ToString(row["BillNo"]);
                txtDate.Text = Convert.ToString(row["BDate"]);
                strOldPartyName = txtSalesParty.Text = Convert.ToString(row["SParty"]);
                txtSubParty.Text = Convert.ToString(row["HParty"]);
                txtTransport.Text = Convert.ToString(row["Transport"]);
                txtPvtMarka.Text = Convert.ToString(row["Marka"]);
                txtBStation.Text = Convert.ToString(row["Station"]);
                txtDueDays.Text = Convert.ToString(row["DueDays"]);
                //txtDueDate.Text = Convert.ToString(row["DDate"]);
                //txtReference.Text = Convert.ToString(row["ReferenceName"]);

                strOldLRNumber = txtLRNumber.Text = Convert.ToString(row["LrNumber"]);
                if (strOldLRNumber != "")
                    txtLRDate.Text = Convert.ToString(row["LDate"]);

                txtPackingDate.Text = Convert.ToString(row["PDate"]);
                txtPackerName.Text = Convert.ToString(row["PackerName"]);
                txtCartonType.Text = Convert.ToString(row["CartoneType"]);
                txtCartonSize.Text = Convert.ToString(row["CartoneSize"]);
                txtRemarks.Text = Convert.ToString(row["Remark"]);
                txtPacking.Text = Convert.ToString(row["OtherPacking"]);
                txtPostage.Text = Convert.ToString(row["Postage"]);
                lblNetAddLs.Text = Convert.ToString(row["NetAddLs"]);
                lblTotalPcs.Text = Convert.ToString(row["TotalPcs"]);
                lblGrossAmt.Text = Convert.ToString(row["GrossAmt"]);
                lblFinalAmt.Text = Convert.ToString(row["FinalAmt"]);
                lblNetAmt.Text = Convert.ToString(row["NetAmt"]);
                txtOtherPerText.Text = Convert.ToString(row["OtherPerText"]);
                txtOtherText.Text = Convert.ToString(row["OtherText"]);
                txtOtherPer.Text = Convert.ToString(row["OtherPercentage"]);
                txtTaxLedger.Text = Convert.ToString(row["SalesType"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                txtTimeOfSupply.Text = Convert.ToString(row["TimeOfSupply"]);
                txtWayBillNo.Text = Convert.ToString(row["WayBillNo"]);
                txtVehicleNo.Text = Convert.ToString(row["VehicleNo"]);
                txtGreenTax.Text = Convert.ToString(row["GreenTaxAmt"]);
                txtServiceAmt.Text = Convert.ToString(row["ServiceAmount"]);
                txtNoofCases.Text = Convert.ToString(row["OtherField"]);
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                lblRoundOffSign.Text = Convert.ToString(row["RoundOffSign"]);
                lblRoundOffAmt.Text = Convert.ToString(row["RoundOffAmt"]);

                txtAttachedBill.Text = strAllAttachedBillNo = Convert.ToString(row["AttachedBill"]);
                txtBillStatus.Text = Convert.ToString(row["BillStatus"]);
                txtDescription.Text = Convert.ToString(row["Description"]);
                txtPackedBillNo.Text = Convert.ToString(row["PackedBillNo"]);
                txtPackingType.Text = Convert.ToString(row["Description_2"]);
                txtPetiAgent.Text = Convert.ToString(row["PAgent"]);
                EnableAllControlsAfterLock(row["LoginType"]);

                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;
                string strPStatus = Convert.ToString(row["GoodsType"]).ToUpper();
                if (strPStatus == "DIRECT")
                    rdoDirect.Checked = true;
                else if (strPStatus == "PACKED")
                    rdoPacked.Checked = true;
                else
                    rdoCameOffice.Checked = true;

                if (dt.Columns.Contains("IRNNO"))
                    txtIRNNo.Text = Convert.ToString(row["IRNNo"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                if (dt.Columns.Contains("WayBillDate"))
                {
                    txtWayBIllDate.Text = Convert.ToString(row["WayBillDate"]);
                }
                else if (CreateCompany.strNewAddedQuery != "")
                    dba.ExecuteMyQuery(CreateCompany.strNewAddedQuery);

                grpPacking.Enabled = !rdoDirect.Checked;
                dOldNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
                dGreenTaxAmount = dba.ConvertObjectToDouble(row["GTaxAmt"]);
                dForwardingCharges = dba.ConvertObjectToDouble(row["ForwardingCharges"]);
                dExtraCharges = dba.ConvertObjectToDouble(row["ExtraCharges"]);
                double __dPackingAmt = dba.ConvertObjectToDouble(row["OtherPacking"]), __dNoOfCase = dba.ConvertObjectToDouble(row["OtherField"]);

                if (__dPackingAmt != 0 && __dNoOfCase != 0)
                    _dPerCasePackingAmt = (__dPackingAmt / __dNoOfCase);
                else
                    _dPerCasePackingAmt = 0;

                double dOtherPerAmt = dba.ConvertObjectToDouble(row["OtherPer"]), dOtherAmt = dba.ConvertObjectToDouble(row["Others"]);
                if (dOtherPerAmt >= 0)
                {
                    txtSignPer.Text = "+";
                    txtOtherPerAmt.Text = dOtherPerAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    txtSignPer.Text = "-";
                    txtOtherPerAmt.Text = Math.Abs(dOtherPerAmt).ToString("N2", MainPage.indianCurancy);
                }
                if (dOtherAmt >= 0)
                {
                    txtSign.Text = "+";
                    txtOtherAmt.Text = dOtherAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    txtSign.Text = "-";
                    txtOtherAmt.Text = Math.Abs(dOtherAmt).ToString("N2", MainPage.indianCurancy);
                }

                if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }

                if (lblRoundOffSign.Text == "")
                {
                    lblRoundOffSign.Text = "+";
                    lblRoundOffAmt.Text = "0.00";
                }
            }
            //else
            //{
            //    ClearAllText();
            //}
        }

        private void BindSalesEntryDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            if (dt.Rows.Count > 0)
            {
                int rowIndex = 0;
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["chk"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["gid"].Value = row["ID"];
                    dgrdDetails.Rows[rowIndex].Cells["serialNo"].Value = row["GRSNo"];
                    dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = row["PParty"];
                    dgrdDetails.Rows[rowIndex].Cells["pcs"].Value = row["Pieces"];
                    dgrdDetails.Rows[rowIndex].Cells["item"].Value = row["Items"];
                    dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = row["Discount"];
                    dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = row["DiscountStatus"];
                    dgrdDetails.Rows[rowIndex].Cells["sndhara"].Value = row["SNDhara"];
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = row["Amount"];
                    dgrdDetails.Rows[rowIndex].Cells["packing"].Value = row["Packing"];
                    dgrdDetails.Rows[rowIndex].Cells["freight"].Value = row["Freight"];
                    dgrdDetails.Rows[rowIndex].Cells["tax"].Value = row["Tax"];
                    dgrdDetails.Rows[rowIndex].Cells["total"].Value = row["TotalAmt"];
                    dgrdDetails.Rows[rowIndex].Cells["rempcs"].Value = row["RemPcs"];
                    dgrdDetails.Rows[rowIndex].Cells["pBill"].Value = row["PurchaseBill"];
                    dgrdDetails.Rows[rowIndex].Cells["remark"].Value = row["SalesFrom"];
                    dgrdDetails.Rows[rowIndex].Cells["purchaseStatus"].Value = row["PStatus"];
                    rowIndex++;
                }
            }

            if (dgrdDetails.Rows.Count == 0)
                txtSalesParty.Enabled = txtSubParty.Enabled = rdoDirect.Enabled = rdoPacked.Enabled = grpPacking.Enabled = rdoCameOffice.Enabled = true;

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
                //  pnlTax.Visible = true;
            }
            //  else
            //   pnlTax.Visible = false;
        }

        private void EnableAllControls()
        {
            txtIRNNo.ReadOnly = txtDate.ReadOnly = txtPvtMarka.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtPackingDate.ReadOnly = txtSignPer.ReadOnly = txtOtherPerText.ReadOnly = txtOtherPer.ReadOnly = txtSign.ReadOnly = txtOtherText.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly = txtRemarks.ReadOnly = txtGreenTax.ReadOnly = txtVehicleNo.ReadOnly = txtWayBillNo.ReadOnly = txtWayBIllDate.ReadOnly = txtTimeOfSupply.ReadOnly = txtNoofCases.ReadOnly = txtPostage.ReadOnly = false;
            btnPAdd.Enabled = btnRemove.Enabled = chkSendSMS.Enabled = chkEmail.Enabled =chkCourier.Enabled= true;
            if (btnEdit.Text == "&Update" && !MainPage.strUserRole.Contains("ADMIN"))
            {
                txtDate.ReadOnly = true;
                DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                if (MainPage.currentDate < _date.AddDays(3) && MainPage.mymainObject.bFullEditControl)
                    txtDate.ReadOnly = false;
            }
        }

        private void DisableAllControls()
        {
            txtIRNNo.ReadOnly = txtDate.ReadOnly = txtPvtMarka.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtPackingDate.ReadOnly = txtSignPer.ReadOnly = txtOtherPerText.ReadOnly = txtOtherPer.ReadOnly = txtSign.ReadOnly = txtOtherText.ReadOnly = txtOtherAmt.ReadOnly = txtPacking.ReadOnly = txtRemarks.ReadOnly = txtGreenTax.ReadOnly = txtVehicleNo.ReadOnly = txtWayBillNo.ReadOnly = txtWayBIllDate.ReadOnly = txtTimeOfSupply.ReadOnly = txtNoofCases.ReadOnly = txtTaxPer.ReadOnly = txtPostage.ReadOnly = txtPackedBillNo.ReadOnly = true;
            rdoPacked.Enabled = rdoDirect.Enabled = rdoCameOffice.Enabled = btnPAdd.Enabled = btnRemove.Enabled = chkSendSMS.Enabled = chkEmail.Enabled = chkCourier.Enabled = false;
        }

        private void EnableAllControlsAfterLock(object objValue)
        {
            if (Convert.ToString(objValue) == "NO")
                txtSignPer.Enabled = txtOtherPerText.Enabled = txtOtherPer.Enabled = txtSign.Enabled = txtOtherText.Enabled = txtOtherAmt.Enabled = txtPacking.Enabled = btnPAdd.Enabled = btnRemove.Enabled = txtGreenTax.Enabled = txtTaxPer.Enabled = dgrdDetails.Enabled = false;
        }

        private void ClearAllText()
        {
            txtIRNNo.Text = txtPackingDate.Text = strOldLRNumber = strOldPartyName = txtAttachedBill.Text = txtPackedBillNo.Text = txtDescription.Text = txtSalesParty.Text = txtSubParty.Text = txtTransport.Text = txtPvtMarka.Text = txtBStation.Text = txtLRNumber.Text = txtPackerName.Text = txtCartonSize.Text = txtCartonType.Text = txtOtherPerText.Text = txtOtherText.Text = txtRemarks.Text = txtTaxLedger.Text = txtVehicleNo.Text = txtWayBillNo.Text = txtWayBIllDate.Text = txtTimeOfSupply.Text = txtNoofCases.Text = "";
            txtSignPer.Text = txtSign.Text = lblRoundOffSign.Text = "+";
         
            lblTaxableAmt.Text = lblRoundOffAmt.Text = txtOtherPer.Text = txtOtherAmt.Text = txtPacking.Text = txtPostage.Text = lblGrossAmt.Text = lblFinalAmt.Text = lblNetAmt.Text = txtTaxAmt.Text = txtGreenTax.Text = txtServiceAmt.Text =  "0.00";
            lblAmount.Text = lblFreight.Text = lblTotalAmt.Text = lblNetAddLs.Text = lblNetAmt.Text = lblPacking.Text = lblTax.Text = "0.00";
            lblCreatedBy.Text = strAllAttachedBillNo = lblMsg.Text = txtPetiAgent.Text = txtPackingType.Text = "";
            dOldNetAmt = 0;
            txtTaxPer.Text = "18.00";
            txtDescription.Text = "";

            txtOtherText.Text = "EXTRA";
            txtBillStatus.Text = "BILLED";
            txtNoofCases.Text = "";
            txtSalesParty.BackColor = Color.White;
            dgrdPending.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdTax.Rows.Clear();
            dgrdPendingStock.Rows.Clear();
            dgrdShowSaleBill.Rows.Clear();
            rdoPacked.Checked = true;
            txtReason.Clear();
            chkCourier.Checked = false;
            pnlDeletionConfirmation.Visible = pnlPendingStock.Visible = pnlShowSaleBill.Visible = false;
            chkPAll.Checked = chkEmail.Checked = chkSendSMS.Checked = pnlTax.Visible = false;
            if (MainPage.currentDate > MainPage.startFinDate && MainPage.currentDate <= MainPage.endFinDate)
                txtDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    string strQuery = "", strSubQuery = "(Select Top 1 TaxName from SaleTypeMaster Where Region='INTERSTATE' and SaleType='SALES' and TaxIncluded=0)";
                    if (strNewAddedGRSNO != "")
                        strSubQuery = " (Select Top 1 TaxName from SaleTypeMaster STM OUTER APPLY (Select _STM.TaxIncluded as _TaxIncluded from GoodsReceive GR inner join SaleTypeMaster _STM  ON GR.PurchaseType=_STM.TaxName and _STM.SaleType='PURCHASE' Where (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar)) in ('" + strNewAddedGRSNO + "')) _STM Where Region='INTERSTATE' and SaleType='SALES' and TaxIncluded=_TaxIncluded ) ";
                    strQuery = "Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo," + strSubQuery + " as TaxName  from SalesRecord Where BillCode='" + txtBillCode.Text + "')Sales ";

                    DataTable table = DataBaseAccess.GetDataTableRecord(strQuery);
                    if (table.Rows.Count > 0)
                    {
                        //double billNo = dba.ConvertObjectToDouble(table.Rows[0][0]), maxBillNo = dba.ConvertObjectToDouble(table.Rows[0][1]),dSerialNo=Convert(;
                        //if (billNo > maxBillNo)
                        //    txtBillNo.Text = Convert.ToString(billNo);
                        //else
                        txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                        txtTaxLedger.Text = Convert.ToString(table.Rows[0]["TaxName"]);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in set sale bill No in Sale book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtOtherPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && dgrdDetails.Rows.Count == 0)
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("SALESPARTY");
                        if (strData != "")
                        {
                            txtSalesParty.Text = strData;
                            txtSubParty.Text = "SELF";
                            GetPendingGRRecord();
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
                            if (objSearch.strSelectedData != "")
                            {
                                txtSalesParty.Text = objSearch.strSelectedData;
                                txtSubParty.Text = "SELF";
                                GetPendingGRRecord();
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

        private void txtSubParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && dgrdDetails.Rows.Count == 0 && txtSalesParty.Text != "")
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
                            GetPendingGRRecord();
                        }
                    }
                }
                e.Handled = true;
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
                    string strTransport = txtTransport.Text;
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH TRANSPORT NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTransport.Text = objSearch.strSelectedData;
                        if (strTransport != txtTransport.Text)
                        {
                            dGreenTaxAmount = 0;
                            GetGreenTaxFromTransport();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetGreenTaxFromTransport()
        {
            if (txtTransport.Text != "")
            {
                string strQuery = " Select ISNULL(GreenTaxAmt,0) as GreenTaxAmt,ISNULL(ForwardingCharges,0) as ForwardingCharges,ISNULL(ExtraCharges,0) as ExtraCharges from Transport Where TransportName='" + txtTransport.Text + "' ";
                DataTable _dt = dba.GetDataTable(strQuery);
                if (_dt.Rows.Count > 0)
                {
                    dGreenTaxAmount = dba.ConvertObjectToDouble(_dt.Rows[0]["GreenTaxAmt"]);
                    dForwardingCharges = dba.ConvertObjectToDouble(_dt.Rows[0]["ForwardingCharges"]);
                    dExtraCharges = dba.ConvertObjectToDouble(_dt.Rows[0]["ExtraCharges"]);
                }
            }
            SetGreenTaxAmt();
        }

        private void txtBStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("STATIONNAME", "SEARCH BOOKING STATION NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtBStation.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtPackerName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PACKERNAME", "SEARCH PACKER NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPackerName.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCartonType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CARTONTYPE", "SEARCH CARTON TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtCartonType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtCartonSize_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("CARTONSIZE", "SEARCH CARTON SIZE", e.KeyCode);
                        objSearch.ShowDialog();
                        string strSize = objSearch.strSelectedData;
                        if (strSize != "")
                        {
                            string[] strAllData = strSize.Split('|');
                            if (strAllData.Length > 1)
                            {
                                txtCartonSize.Text = strAllData[0];
                                if (rdoPacked.Checked)
                                {
                                    txtPackingType.Text = strAllData[1];
                                    _dPerCasePackingAmt = dba.ConvertObjectToDouble(strAllData[2]);
                                    SetPackingAmt();
                                }
                                else
                                    txtPackingType.Text = "";
                            }
                            else
                            {
                                txtCartonSize.Text = "";
                                if (rdoPacked.Checked)
                                    txtPacking.Text = MainPage.dPackingAmount.ToString("0");
                            }
                        }
                        else
                        {
                            txtCartonSize.Text = "";
                            if (rdoPacked.Checked)
                                txtPacking.Text = MainPage.dPackingAmount.ToString("0");
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SetPackingAmt()
        {
            try
            {
                double dCase = dba.ConvertObjectToDouble(txtNoofCases.Text);
                double dPackingAmt = (dCase * _dPerCasePackingAmt);
                txtPacking.Text = dPackingAmt.ToString("0");
            }
            catch { }
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
                //SetDueDays();
            }
        }

        private void txtPackingDate_Leave(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                dba.GetDateInExactFormat(sender, false, false, false);
        }

        private DataSet GetPendingRecordDataSet()
        {
            DataSet ds = null;
            string strSaleParty = "", strSubParty = "";
            string[] strFullName = txtSalesParty.Text.Split(' ');
            if (strFullName.Length > 1)
                strSaleParty = strFullName[0].Trim();
            strFullName = txtSubParty.Text.Split(' ');
            if (strFullName.Length > 0)
                strSubParty = strFullName[0].Trim();

            if (strSaleParty != "" && strSubParty != "")
            {
                string strQuery = "", strNewGRSNoQuery = "";
                if (strNewAddedGRSNO != "")
                    strNewGRSNoQuery = " and (ReceiptCode+' '+CAST(ReceiptNo as varchar)) not in ('" + strNewAddedGRSNO + "') ";

                strQuery = " Select RCode,CONVERT(varchar,Date,103) RDate,PParty,Item,Pieces,Quantity,Amount,PurchaseStatus from (Select (ReceiptCode+' '+CAST(ReceiptNo as varchar)) RCode,ReceivingDate as Date,(CASE When PurchaseParty!='' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Item,Pieces,Quantity,Amount,ISNULL(PurchaseStatus,0) as PurchaseStatus from GoodsReceive Where SaleBill='PENDING' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' and PackingStatus='" + GetPackingStatus() + "' " + strNewGRSNoQuery + "  "
                              + " Union ALL Select SerialNo as RCode,Date,(CASE When PartyName !='PERSONAL' then dbo.GetFullName(PurchasePartyID) else PartyName end) PParty,Item,SalesFrom as Pieces,Pieces as Quantity,0 as Amount,0 as PurchaseStatus from GoodsReturned Where Status='PENDING' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "') GoodsReceive Order by Date ";

                if (strSubParty == "SELF")
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation,(Select TP.GreenTaxAmt from Transport TP Where TP.TransportName=Transport) GTaxAmt from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";
                else
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation,(Select TP.GreenTaxAmt from Transport TP Where TP.TransportName=Transport) GTaxAmt from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSubParty + "' ";

                strQuery += " Select TransactionLock,GroupII,BlackList,Reference,UPPER(Other1) as OrangeZone,(CASE When DueDays!='' then DueDays else (Select TOP 1 GraceDays from CompanySetting) end) DueDays,(CASE When Postage!='' then Postage else (Select TOP 1 Postage from CompanySetting) end) Postage,(CASE WHEN FourthTransport='False' then FourthTransport else 'True' end) as PostageStatus,Category,TINNumber from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";

                ds = DataBaseAccess.GetDataSetRecord(strQuery);

            }
            return ds;
        }

        private void GetPendingGRRecord()
        {
            dgrdPending.Rows.Clear();
            txtTransport.Text = txtPvtMarka.Text = txtBStation.Text = txtAttachedBill.Text = "";
            bool tStatus = true;
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                DataSet ds = GetPendingRecordDataSet();
                if (ds != null)
                {
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[2];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            txtDueDays.Text = Convert.ToString(dt.Rows[0]["DueDays"]);
                            txtPostage.Text = Convert.ToString(dt.Rows[0]["Postage"]);
                            if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                tStatus = false;
                            }
                            else if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                            {
                                //txtSalesParty.BackColor = Color.IndianRed;
                                MessageBox.Show("This Account is in blacklist ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                tStatus = false;
                            }
                            else if (Convert.ToString(dt.Rows[0]["OrangeZone"]) == "TRUE")
                            {
                                //txtSalesParty.BackColor = Color.IndianRed;
                                MessageBox.Show("This Account is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                tStatus = false;
                            }
                            // else
                            //   txtSalesParty.BackColor = Color.White;
                            if (Convert.ToBoolean(dt.Rows[0]["PostageStatus"]))
                                pnlNOCourier.Visible = false;
                            else
                            {
                                pnlNOCourier.Visible = true;
                                txtPostage.Text = "0.00";
                            }

                            if (Convert.ToString(dt.Rows[0]["Category"]) == "CASH PARTY" || Convert.ToString(dt.Rows[0]["TINNumber"]) == "CASH PARTY")
                                pnlCash.Visible = true;
                            else
                                pnlCash.Visible = false;
                            //SetDueDays();
                        }

                        if (tStatus)
                        {
                            dt.Clear();
                            dt = ds.Tables[0];

                            if (dt.Rows.Count > 0)
                            {
                                int rowIndex = 0;
                                dgrdPending.Rows.Add(dt.Rows.Count);
                                foreach (DataRow row in dt.Rows)
                                {
                                    dgrdPending.Rows[rowIndex].Cells["chkItem"].Value = false;
                                    dgrdPending.Rows[rowIndex].Cells["pGRSNo"].Value = row["RCode"];
                                    dgrdPending.Rows[rowIndex].Cells["pSupplier"].Value = row["PParty"];
                                    dgrdPending.Rows[rowIndex].Cells["pStatus"].Value = row["PurchaseStatus"];
                                    rowIndex++;
                                }
                            }
                            dt.Clear();
                            dt = ds.Tables[1];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                if (row != null)
                                {
                                    //txtTransport.Text = Convert.ToString(row["Transport"]);
                                    txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                                    // txtBStation.Text = Convert.ToString(row["BookingStation"]);
                                    dGreenTaxAmount = dba.ConvertObjectToDouble(row["GTaxAmt"]);
                                    SetGreenTaxAmt();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetGreenTaxAmt()
        {
            try
            {
                double dGTaxAmt = 0, dForwarding = 0, dPackingAmt = 0;
                if (!rdoDirect.Checked)
                {
                    double dCase = dba.ConvertObjectToDouble(txtNoofCases.Text);
                    dGTaxAmt = (dCase * dGreenTaxAmount);
                    if (rdoCameOffice.Checked)
                        dForwarding = (dCase * dForwardingCharges);
                    else
                    {
                        dForwarding = (dCase * dExtraCharges);
                        dPackingAmt = (dCase * _dPerCasePackingAmt);
                    }
                }

                if (txtTransport.Text.Contains("JAI HANUMAN") && txtBStation.Text.Contains("GONDA"))
                    dGTaxAmt = 0;
                txtGreenTax.Text = dGTaxAmt.ToString("0.00");
                txtOtherAmt.Text = dForwarding.ToString("0.00");
                txtPacking.Text = dPackingAmt.ToString("0");
            }
            catch { }
        }

        private void GetPendingOnlyGRRecord()
        {
            dgrdPending.Rows.Clear();
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                DataSet ds = GetPendingRecordDataSet();
                if (ds != null)
                {
                    if (ds.Tables.Count > 1)
                    {
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
                            if (Convert.ToBoolean(dt.Rows[0]["PostageStatus"]))
                                pnlNOCourier.Visible = false;
                            else
                                pnlNOCourier.Visible = true;
                        }

                        dt.Clear();
                        dt = ds.Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            int rowIndex = 0;
                            dgrdPending.Rows.Add(dt.Rows.Count);
                            foreach (DataRow row in dt.Rows)
                            {
                                dgrdPending.Rows[rowIndex].Cells["chkItem"].Value = false;
                                dgrdPending.Rows[rowIndex].Cells["pGRSNo"].Value = row["RCode"];
                                dgrdPending.Rows[rowIndex].Cells["pSupplier"].Value = row["PParty"];
                                dgrdPending.Rows[rowIndex].Cells["pStatus"].Value = row["PurchaseStatus"];
                                rowIndex++;
                            }
                        }
                    }
                }
            }
        }

        //private void SetDueDays()
        //{
        //    try
        //    {
        //        if (txtDueDays.Text != "")
        //        {
        //            double dDays = dba.ConvertObjectToDouble(txtDueDays.Text);
        //            DateTime sDate = dba.ConvertDateInExactFormat(txtDate.Text);
        //            sDate = sDate.AddDays(dDays);
        //            txtDueDate.Text = sDate.ToString("dd/MM/yyyy");
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private string GetPackingStatus()
        {
            if (rdoDirect.Checked)
                return "DIRECT";
            else if (rdoPacked.Checked)
                return "PACKED";
            else
                return "CAMEOFFICE";
        }

        private void rdoPacked_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoPacked.Checked)
            {
                GetPendingGRRecord();
                txtPackingDate.Text = "";
                SetGreenTaxAmt();
            }
            grpPacking.Enabled = !rdoDirect.Checked;
        }

        private void rdoDirect_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoDirect.Checked)
            {
                GetPendingGRRecord();
                SetGreenTaxAmt();
            }
            grpPacking.Enabled = !rdoDirect.Checked;
        }

        private void btnPAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    string strRemSNo = "", strSNo = "", strGRSNo = "";
                    dgrdPending.EndEdit();
                    dgrdDetails.Focus();
                    for (int rowIndex = 0; rowIndex < dgrdPending.Rows.Count; rowIndex++)
                    {
                        DataGridViewRow row = dgrdPending.Rows[rowIndex];

                        if (Convert.ToBoolean(row.Cells["chkItem"].Value))
                        {
                            strGRSNo = Convert.ToString(row.Cells["pGRSNo"].Value);
                            if (strGRSNo.Contains("RM "))
                            {
                                if (strRemSNo == "")
                                    strRemSNo = "'" + strGRSNo + "'";
                                else
                                    strRemSNo += ",'" + strGRSNo + "'";
                            }
                            else
                            {
                                if (strSNo == "")
                                    strSNo = "'" + strGRSNo + "'";
                                else
                                    strSNo += ",'" + strGRSNo + "'";
                            }
                            dgrdPending.Rows.RemoveAt(rowIndex);
                            rowIndex--;
                        }
                    }
                    AddGoodsReceiveRecordInGrid(strSNo, strRemSNo);
                }
                if (dgrdDetails.Rows.Count > 0)
                    txtSalesParty.Enabled = txtSubParty.Enabled = rdoDirect.Enabled = rdoPacked.Enabled = rdoCameOffice.Enabled = false;
            }
            catch
            {
            }
        }

        private void AddGoodsReceiveRecordInGrid(string strSNo, string strRemSNo)
        {
            if (strSNo != "" || strRemSNo != "")
            {
                if (txtSalesParty.Text != "" && txtSubParty.Text != "")
                {
                    string strSaleParty = "", strSubParty = "";
                    string[] strFullName = txtSalesParty.Text.Split(' ');
                    if (strFullName.Length > 1)
                        strSaleParty = strFullName[0].Trim();
                    strFullName = txtSubParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                        strSubParty = strFullName[0].Trim();
                    if (strSNo == "")
                        strSNo = "'0'";
                    if (strSaleParty != "" && strSubParty != "")
                    {
                        string strQuery = "";
                        strQuery = " Select (ReceiptCode+' '+CAST(ReceiptNo as varchar)) RCode,(CASE When PurchaseParty!='' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Item,Quantity,Amount,Packing,Freight,Tax,ROUND((Amount+CAST(Packing as Money)+CAST(Freight as Money)+CAST(Tax as Money)),2) TotalAmt,"
                                 + " ISNULL(SM.NormalDhara,0) Dhara,GR.Remark,(CASE WHEN SM.TINNumber='CASH PURCHASE' then SM.TINNumber else SM.Category end) SCategory,Dhara PDhara,(CASE WHEN GR.PurchasePartyID='DL5255' and OB.SchemeName Like('%TOUR JAN%') then 0 else ((GR.DisPer*-1)+(CASE WHEN (SM.Category='CASH PURCHASE' OR SM.TINNumber='CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (SM.Category='CLOTH PURCHASE' OR ReceiptCode Like('%SRT%') OR ReceiptCode Like('%CCK%')) then 1 else 0 end)) end) Dis,ISNULL(PurchaseStatus,0) PurchaseStatus,OB.Transport as TransportName,OB.MRemark,OB.SchemeName,OB.Station as StationName,GR.NoOfCase from GoodsReceive GR inner join SupplierMaster SM on (AreaCode+ CAST(AccountNo as varchar))=GR.PurchasePartyID OUTER APPLY (Select Top 1 Transport,MRemark,OB.SchemeName,OB.Station from OrderBooking OB  Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo) OB Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (" + strSNo + ") and SaleBill='PENDING' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' and PackingStatus='" + GetPackingStatus() + "'";

                        if (strRemSNo != "")
                            strQuery += " Union All Select SerialNo as RCode,(CASE When PartyName !='PERSONAL' then dbo.GetFullName(PurchasePartyID) else PartyName end) PParty,Item,Pieces as Quantity,0 as Amount,'0' Packing,'0' Freight,'0' Tax,0 TotalAmt,'0' as Dhara,SalesFrom as Remark,'' as SCategory,'' PDhara,0 as SDis, 0 PurchaseStatus,'' as TransportName,'' as MRemark,'' as SchemeName,'' as StationName,0 as NoOfCase from GoodsReturned Where Status='PENDING' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' and SerialNo in (" + strRemSNo + ") ";

                        DataTable dt = dba.GetDataTable(strQuery);
                        BindGoodsReceiveDataWithGrid(dt);
                    }
                }
            }
        }

        private void BindGoodsReceiveDataWithGrid(DataTable dt)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    int rowIndex = dgrdDetails.Rows.Count;
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    string strGRSNo = "", strDhara = "", strPParty = "", strTransport = "", strTransportName = "", strRemark = "", strStationName = "";
                    bool _pStatus = false;
                    double dDisPer = 0, dPetiCount = 0;
                    strTransportName = txtTransport.Text;
                    foreach (DataRow row in dt.Rows)
                    {
                        strGRSNo = Convert.ToString(row["RCode"]);
                        strDhara = Convert.ToString(row["Dhara"]);
                        strPParty = Convert.ToString(row["PParty"]);
                        _pStatus = Convert.ToBoolean(row["PurchaseStatus"]);
                        strTransport = Convert.ToString(row["TransportName"]);
                        dPetiCount += dba.ConvertObjectToDouble(row["NoOfCase"]);
                        if (Convert.ToString(row["StationName"]) != "")
                            strStationName = Convert.ToString(row["StationName"]);

                        strRemark = Convert.ToString(row["MRemark"]);
                        if (strTransportName == "")
                            strTransportName = strTransport;
                        else if (strTransport != "" && strTransportName != strTransport && strTransportName != "")
                        {
                            MessageBox.Show("Warning ! Transport in Order Form and Transport in Master Both are different, Please be sure about transport name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            strTransportName = strTransport;
                        }

                        dgrdDetails.Rows[rowIndex].Cells["gid"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["chk"].Value = false;
                        dgrdDetails.Rows[rowIndex].Cells["serialNo"].Value = strGRSNo;// row["RCode"];
                        dgrdDetails.Rows[rowIndex].Cells["partyName"].Value = strPParty;// row["PParty"];
                        dgrdDetails.Rows[rowIndex].Cells["pcs"].Value = row["Quantity"];
                        dgrdDetails.Rows[rowIndex].Cells["item"].Value = row["Item"];
                        dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["packing"].Value = row["Packing"];
                        dgrdDetails.Rows[rowIndex].Cells["freight"].Value = row["Freight"];
                        dgrdDetails.Rows[rowIndex].Cells["tax"].Value = row["Tax"];
                        dgrdDetails.Rows[rowIndex].Cells["total"].Value = dba.ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["rempcs"].Value = "0";
                        dgrdDetails.Rows[rowIndex].Cells["pBill"].Value = "";
                        dgrdDetails.Rows[rowIndex].Cells["remark"].Value = row["Remark"];
                        dgrdDetails.Rows[rowIndex].Cells["purchaseStatus"].Value = _pStatus;


                        if (_pStatus)
                        {
                            dDisPer = dba.ConvertObjectToDouble(row["Dis"]);
                            dgrdDetails.Rows[rowIndex].Cells["sndhara"].Value = row["PDhara"];
                            dgrdDetails.Rows[rowIndex].Cells["pBill"].Value = "CLEAR";
                            if (dDisPer >= 0)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = "+";
                                dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = dDisPer;
                            }
                            else
                            {
                                dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = "-";
                                dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = Math.Abs(dDisPer);

                            }
                        }
                        else if (!strGRSNo.Contains("RM ") && strPParty != "PERSONAL")
                        {
                            SetDiscountDetails(strDhara, "NORMAL", rowIndex, Convert.ToString(row["SCategory"]));
                        }
                        else
                        {
                            dgrdDetails.Rows[rowIndex].Cells["sndhara"].Value = "NORMAL";
                            dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = "+";
                            dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = 0;
                        }

                        if (strRemark != "" && !txtRemarks.Text.Contains(strRemark))
                        {
                            if (txtRemarks.Text == "")
                                txtRemarks.Text = strRemark;
                            else
                                txtRemarks.Text += " " + strRemark;
                        }
                        rowIndex++;
                    }

                    if (strTransportName != "")
                    {
                        if (txtTransport.Text != strTransportName)
                        {
                            txtTransport.Text = strTransportName;
                            dGreenTaxAmount = 0;
                        }
                    }
                    txtBStation.Text = strStationName;
                    txtNoofCases.Text = (dba.ConvertObjectToDouble(txtNoofCases.Text) + dPetiCount).ToString();

                    GetGreenTaxFromTransport();
                    CalculateGridAmount();
                }
            }
            catch
            {
            }
        }

        private void SetDiscountDetails(string strDhara, string strDType, int rowIndex, string strCategory)
        {
            double _dPer = 3;
            if (strDhara != "")
            {
                if (strCategory.ToUpper() == "CASH PURCHASE")
                    _dPer = 5;
                if (txtBillCode.Text.Contains("SRT") || strCategory.ToUpper() == "CLOTH PURCHASE")
                    _dPer--;

                double dDhara = dba.ConvertObjectToDouble(strDhara);
                dgrdDetails.Rows[rowIndex].Cells["sndhara"].Value = strDType;
                dDhara = (dDhara * -1) + _dPer;
                if (dDhara >= 0)
                {
                    dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = "+";
                    dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = dDhara;
                }
                else
                {
                    dgrdDetails.Rows[rowIndex].Cells["DiscountStatus"].Value = "-";
                    dgrdDetails.Rows[rowIndex].Cells["Disc"].Value = Math.Abs(dDhara);
                }
                if (!MainPage.mymainObject.bSaleEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                    btnEdit.Enabled = btnDelete.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please Enter Normal Dhara in Party Master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = false;
            }
        }

        private void GetDharaDetails(DataGridViewRow row)
        {
            string strParty = Convert.ToString(row.Cells["partyName"].Value), strDType = Convert.ToString(row.Cells["sndhara"].Value), strDhara = "", strQuery = "";
            if (strDType == "NORMAL")
                strQuery = " Select NormalDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";
            else if (strDType == "SUPER")
                strQuery = " Select SNDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";
            else
                strQuery = " Select CFormApply as PremiumDhara,Category from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + strParty + "' ";

            DataTable _dt = dba.GetDataTable(strQuery);
            if (_dt.Rows.Count > 0)
            {

                strDhara = Convert.ToString(_dt.Rows[0][0]);
                if (strDhara != "")
                {//CASH PURCHASE
                    SetDiscountDetails(strDhara, strDType, row.Index, Convert.ToString(_dt.Rows[0][1]));
                    CalculateGridAmount();
                }
                else
                {
                    MessageBox.Show("Please enter Super Net Dhara in party master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please enter Super Net Dhara in party master ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = false;
            }
        }

        private void CalulateGridDataAmtOnBind()
        {
            try
            {
                double dPcs = 0, dAmt = 0, dTAmt = 0, dPacking = 0, dFreight = 0, dTax = 0, dTotalAmt = 0, dPer = 0, dDisAmt = 0, dPDhara = 0, dFDhara = 0, dTDhara = 0, dFTPAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dPcs += (dba.ConvertObjectToDouble(row.Cells["pcs"].Value) - dba.ConvertObjectToDouble(row.Cells["rempcs"].Value));
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    dPacking += dba.ConvertObjectToDouble(row.Cells["packing"].Value);
                    dFreight += dba.ConvertObjectToDouble(row.Cells["freight"].Value);
                    dTax += dba.ConvertObjectToDouble(row.Cells["tax"].Value);
                    dTotalAmt += dba.ConvertObjectToDouble(row.Cells["total"].Value);
                    dPer = dba.ConvertObjectToDouble(row.Cells["DiscountStatus"].Value + "" + row.Cells["Disc"].Value);
                    dDisAmt += (dAmt * dPer) / 100;
                }
                if (MainPage.dPackingDhara != 0 && dPacking != 0)
                    dPDhara = (dPacking * MainPage.dPackingDhara) / 100;
                if (MainPage.dFreightDhara != 0 && dFreight != 0)
                    dFDhara = (dFreight * MainPage.dFreightDhara) / 100;
                if (MainPage.dTaxDhara != 0 && dTax != 0)
                    dTDhara = (dTax * MainPage.dTaxDhara) / 100;

                // lblPAmount.Text = dPDhara.ToString();
                //lblFAmount.Text = dFDhara.ToString();
                // lblTAmount.Text = dTDhara.ToString();
                dFTPAmt = dPDhara + dFDhara + dTDhara;
                //lblFTPAmount.Text = dFTPAmt.ToString();
                dDisAmt += dFTPAmt;

                lblTotalPcs.Text = dPcs.ToString("N0", MainPage.indianCurancy);
                lblNetAddLs.Text = dDisAmt.ToString("N2", MainPage.indianCurancy);
                lblAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblPacking.Text = dPacking.ToString("N0", MainPage.indianCurancy);
                lblFreight.Text = dFreight.ToString("N0", MainPage.indianCurancy);
                lblTax.Text = dTax.ToString("N0", MainPage.indianCurancy);
                lblTotalAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }

        private void CalculateGridAmount()
        {
            try
            {
                double dPcs = 0, dAmt = 0, dTAmt = 0, dPacking = 0, dFreight = 0, dTax = 0, dTotalAmt = 0, dPer = 0, dDisAmt = 0, dPDhara = 0, dFDhara = 0, dTDhara = 0, dFTPAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dPcs += (dba.ConvertObjectToDouble(row.Cells["pcs"].Value) - dba.ConvertObjectToDouble(row.Cells["rempcs"].Value));
                    dTAmt += dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    dPacking += dba.ConvertObjectToDouble(row.Cells["packing"].Value);
                    dFreight += dba.ConvertObjectToDouble(row.Cells["freight"].Value);
                    dTax += dba.ConvertObjectToDouble(row.Cells["tax"].Value);
                    dTotalAmt += dba.ConvertObjectToDouble(row.Cells["total"].Value);
                    dPer = dba.ConvertObjectToDouble(row.Cells["DiscountStatus"].Value + "" + row.Cells["Disc"].Value);
                    dDisAmt += (dAmt * dPer) / 100;
                }
                if (MainPage.dPackingDhara != 0 && dPacking != 0)
                    dPDhara = (dPacking * MainPage.dPackingDhara) / 100;
                if (MainPage.dFreightDhara != 0 && dFreight != 0)
                    dFDhara = (dFreight * MainPage.dFreightDhara) / 100;
                if (MainPage.dTaxDhara != 0 && dTax != 0)
                    dTDhara = (dTax * MainPage.dTaxDhara) / 100;

                //lblPAmount.Text = dPDhara.ToString();
                // lblFAmount.Text = dFDhara.ToString();
                // lblTAmount.Text = dTDhara.ToString();
                dFTPAmt = dPDhara + dFDhara + dTDhara;
                // lblFTPAmount.Text = dFTPAmt.ToString();
                dDisAmt += dFTPAmt;

                lblTotalPcs.Text = dPcs.ToString("N0", MainPage.indianCurancy);
                lblNetAddLs.Text = dDisAmt.ToString("N2", MainPage.indianCurancy);
                lblAmount.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
                lblPacking.Text = dPacking.ToString("N0", MainPage.indianCurancy);
                lblFreight.Text = dFreight.ToString("N0", MainPage.indianCurancy);
                lblTax.Text = dTax.ToString("N0", MainPage.indianCurancy);
                lblTotalAmt.Text = lblGrossAmt.Text = dTotalAmt.ToString("N2", MainPage.indianCurancy);
                lblFinalAmt.Text = (dTotalAmt + dDisAmt).ToString("N2", MainPage.indianCurancy);
                CallOtherPerLeave();
                //CalculateAllAmount();
            }
            catch
            {
            }
        }

        private void CalculateAllAmount()
        {
            string strTaxType = "";
            if (txtNoofCases.Focused)
                SetGreenTaxAmt();

            double dFinalAmt = 0, dOtherPerAmt = 0, dOtherAmt = 0, dPacking = 0, dPostage = 0, dNetAmt = 0, dNetOtherAmount = 0, dTaxAmount = 0, dGreenTax = 0, dServiceAmt = 0, dNetItemTax = 0, dNetAddLess = 0, dTaxableAmt = 0;
            dNetAddLess = dba.ConvertObjectToDouble(lblNetAddLs.Text);
            dFinalAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text) + dNetAddLess;
            dOtherPerAmt = dba.ConvertObjectToDouble(txtSignPer.Text + txtOtherPerAmt.Text);
            dOtherAmt = dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmt.Text);
            dPacking = dba.ConvertObjectToDouble(txtPacking.Text);
            dPostage = dba.ConvertObjectToDouble(txtPostage.Text);
            dGreenTax = dba.ConvertObjectToDouble(txtGreenTax.Text);

            dNetOtherAmount = dOtherPerAmt + dOtherAmt + dPacking + dPostage + dGreenTax;
            dTaxAmount = GetTaxAmount(dNetOtherAmount, dFinalAmt, ref strTaxType, ref dTaxableAmt);
            dServiceAmt = dba.ConvertObjectToDouble(txtServiceAmt.Text);

            dNetAmt = dFinalAmt + dNetOtherAmount + dTaxAmount + dServiceAmt;
            double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));
            double dDiff = dNNetAmt - dNetAmt;

            if (strTaxType != "INCLUDED")
            {
                if (dServiceAmt != 0)
                    dNetItemTax = Math.Round(((dServiceAmt * 100) / MainPage.dTaxDhara), 2);
                dFinalAmt += dNetItemTax;
            }
            else { dFinalAmt -= dNetAddLess; }

            if (dDiff >= 0)
            {
                lblRoundOffSign.Text = "+";
                lblRoundOffAmt.Text = dDiff.ToString("0.00");
            }
            else
            {
                lblRoundOffSign.Text = "-";
                lblRoundOffAmt.Text = Math.Abs(dDiff).ToString("0.00");
            }
            if (dTaxableAmt > 0)
                lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
            else
                lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);

            lblFinalAmt.Text = dFinalAmt.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);

        }

        private void txtSign2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void txtSign1_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    TextBox txt = sender as TextBox;
                    if (txt != null)
                    {
                        if (txt.Text == "")
                            txt.Text = "+";
                        CalculateAllAmount();
                    }
                }
            }
            catch
            {
            }
        }

        private void txtOtherPer_Enter(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    TextBox txt = sender as TextBox;
                    if (txt != null)
                    {
                        if (txt.Text == "0.00")
                            txt.Text = "";
                    }
                }
            }
            catch
            {
            }
        }

        private void txtOtherPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOtherPer.Text == "")
                        txtOtherPer.Text = "0.00";
                    CallOtherPerLeave();
                }
            }
            catch
            {
            }
        }

        private void CallOtherPerLeave()
        {
            double dAmt = dba.ConvertObjectToDouble(txtOtherPer.Text), dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);
            dAmt = (dAmt * dGrossAmt) / 100;
            txtOtherPerAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }

        private void txtOtherAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOtherAmt.Text == "")
                        txtOtherAmt.Text = "0.00";
                    CalculateAllAmount();
                }
            }
            catch
            {
            }
        }

        private void txtPacking_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtPacking.Text == "")
                        txtPacking.Text = "0.00";
                    CalculateAllAmount();
                }
            }
            catch
            {
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                bool dStatus = true;
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        dStatus = false;
                        bool iStatus = true;
                        if (ValidationForDeletion(ref iStatus))
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want to remove this goods received no. ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                                dStatus = true;
                        }
                    }
                    if (dStatus)
                    {
                        string strRemSNo = "", strSNo = "", strGRSNo = "";
                        for (int rowIndex = 0; rowIndex < dgrdDetails.Rows.Count; rowIndex++)
                        {
                            DataGridViewRow row = dgrdDetails.Rows[rowIndex];
                            if (Convert.ToBoolean(row.Cells["chk"].Value))
                            {
                                if (Convert.ToString(row.Cells["gid"].Value) == "")
                                {
                                    BackToPendingGrid(row, rowIndex);
                                    rowIndex--;
                                }
                                else if (Convert.ToString(row.Cells["pBill"].Value) == "PENDING" || Convert.ToBoolean(row.Cells["purchaseStatus"].Value))
                                {
                                    strGRSNo = Convert.ToString(row.Cells["serialNo"].Value);
                                    //BackToPendingGrid(row, rowIndex);
                                    //rowIndex--;
                                    if (strGRSNo.Contains("RM "))
                                    {
                                        if (strRemSNo == "")
                                            strRemSNo = "'" + strGRSNo + "'";
                                        else
                                            strRemSNo += ",'" + strGRSNo + "'";
                                    }
                                    else
                                    {
                                        if (strSNo == "")
                                            strSNo = "'" + strGRSNo + "'";
                                        else
                                            strSNo += ",'" + strGRSNo + "'";
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! Purchase has been made of Serial No : " + row.Cells["serialNo"].Value, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }

                        if (strSNo != "" || strRemSNo != "")
                            DeleteAfterSuccessfulRemoval(strSNo, strRemSNo);
                        if (dgrdDetails.Rows.Count == 0)
                            txtSalesParty.Enabled = txtSubParty.Enabled = rdoDirect.Enabled = rdoPacked.Enabled = grpPacking.Enabled = rdoCameOffice.Enabled = true;

                        CalculateGridAmount();
                    }
                }
            }
            catch
            {
            }
        }

        private void BackToPendingGrid(DataGridViewRow row, int rIndex)
        {
            int rowIndex = dgrdPending.Rows.Count;
            dgrdPending.Rows.Add();
            dgrdPending.Rows[rowIndex].Cells["chkItem"].Value = false;
            dgrdPending.Rows[rowIndex].Cells["pGRSNo"].Value = row.Cells["serialNo"].Value;
            dgrdPending.Rows[rowIndex].Cells["pSupplier"].Value = row.Cells["partyName"].Value;
            dgrdPending.Rows[rowIndex].Cells["pStatus"].Value = row.Cells["purchaseStatus"].Value;
            dgrdDetails.Rows.RemoveAt(rIndex);

            if (!rdoPacked.Checked)
            {
                double dNoCase = dba.ConvertObjectToDouble(txtNoofCases.Text);
                if (dNoCase > 0)
                    txtNoofCases.Text = (dNoCase - 1).ToString("0");
            }
        }

        private void DeleteAfterSuccessfulRemoval(string strGRSNo, string strRemSNo)
        {
            if (DeleteSalesEntry(strGRSNo, strRemSNo))
            {
                if (strGRSNo != "")
                {
                    if (strRemSNo != "")
                        strGRSNo += "," + strRemSNo;
                }
                else
                    strGRSNo = strRemSNo;
                strGRSNo = strGRSNo.Replace("'", "");
                string strSNo = "";
                for (int rowIndex = 0; rowIndex < dgrdDetails.Rows.Count; rowIndex++)
                {
                    DataGridViewRow row = dgrdDetails.Rows[rowIndex];
                    if (Convert.ToBoolean(row.Cells["chk"].Value))
                    {
                        strSNo = Convert.ToString(row.Cells["serialNo"].Value);
                        if (strGRSNo.Contains(strSNo))
                        {
                            BackToPendingGrid(row, rowIndex);
                            rowIndex--;
                        }
                    }

                }

                if (!rdoPacked.Checked)
                {
                    double dNoCase = dba.ConvertObjectToDouble(txtNoofCases.Text);
                    if (dNoCase > 0)
                        txtNoofCases.Text = (dNoCase - 1).ToString("0");
                }
            }
        }

        private bool DeleteSalesEntry(string strGRSNo, string strRemSNo)
        {
            try
            {
                bool iStatus = false;
                if (ValidationForSingleRowDeletion(ref iStatus, strGRSNo))
                {
                    string strQuery = "";
                    if (strGRSNo != "")
                    {
                        strQuery += " Update GoodsReceive set SaleBill='PENDING',UpdateStatus=1  Where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (" + strGRSNo + ")  "
                                 + "  Update PurchaseRecord set SaleBillNo='' Where GRSNo  in (" + strGRSNo + ") "
                                 + " Delete from GoodsReturned where SalesBill='" + txtBillCode.Text + " " + txtBillNo.Text + "' and SerialNo in (Select ('RM '+GRSNo) GRSNo from SalesEntry Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and GRSNo in (" + strGRSNo + ")) "
                                 + " Delete from SalesEntry Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and GRSNo in (" + strGRSNo + ")  "
                                 + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                 + " ('SALESENTRY','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strGRSNo.Replace("'", "") + " No deleted from Sales Entry, with Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                    }
                    if (strRemSNo != "")
                        strQuery += " Update GoodsReturned set Status='PENDING',AdjustedSaleBillNumber='',UpdateStatus=1 where SerialNo in  (" + strRemSNo + ") and  AdjustedSaleBillNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' ";

                    int count = dba.ExecuteMyQuery(strQuery);

                    if (count > 0)
                    {
                        if (!iStatus)
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                        btnAdd.Enabled = btnClose.Enabled = MainPage.mymainObject.ControlBox = MainPage.mymainObject.mStrip.Enabled = false;
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }



        private bool ValidationForSingleRowDeletion(ref bool iStatus, string strGRSNo)
        {
            try
            {
                if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(2) > MainPage.currentDate))
                {
                    string strQuery = "Select TransactionLock,ISNULL((Select InsertStatus from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select UPPER(Tick) from BalanceAmount Where AccountStatus='SALES A/C' and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "'),'FALSE') TickStatus,ISNULL((Select AdjustedSaleBillNumber from GoodsReturned Where SalesBill='" + txtBillCode.Text + " " + txtBillNo.Text + "' and SerialNo in (Select ('RM '+GRSNo) GRSNo from SalesEntry Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and GRSNo in (" + strGRSNo + ")) and AdjustedSaleBillNumber!='' and Status='CLEAR'),'') AdjustSNo from SupplierMaster Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ";

                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                        {
                            MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! You can't delete this bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);
                        if (!iStatus && MainPage.strOnlineDataBaseName != "")
                        {
                            bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtBillCode.Text + " " + txtBillNo.Text);
                            if (!netStatus)
                            {
                                MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
                        {
                            MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (Convert.ToString(dt.Rows[0]["AdjustSNo"]) != "")
                        {
                            MessageBox.Show("Sorry ! The remaining pcs of this bill has been adjusted in sale bill no : " + dt.Rows[0]["AdjustSNo"] + ", Please remove from there ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have sufficient permission to romove the record ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool ValidateAllControls(bool _cStatus)
        {
            if (_cStatus)
                CalculateAllAmount();
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! please insert bill in company setting, Bill code can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill No can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Date is not valid, Please enter valid date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sundry Debtors name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if (txtSubParty.Text == "")
            {
                MessageBox.Show("Sorry ! Sub Party name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubParty.Focus();
                return false;
            }
            if (txtLRNumber.Text != "" && txtLRDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! LR Date can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLRDate.Focus();
                return false;
            }
            if (MainPage._bTaxStatus)
            {
                if (txtTaxLedger.Text == "")
                {
                    MessageBox.Show("Sorry ! Tax Ledger can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTaxLedger.Focus();
                    return false;
                }
                if (dba.ConvertObjectToDouble(txtTaxAmt.Text)==0)
                {
                    MessageBox.Show("Sorry ! Tax Amt can't be zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTaxPer.Focus();
                    if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                        return false;
                }
            }
            if (pnlNOCourier.Visible && dba.ConvertObjectToDouble(txtPostage.Text) > 0)
            {
                MessageBox.Show("Sorry ! Postage is not applicable in this account", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPostage.Focus();
                if (!MainPage.strUserRole.Contains("ADMIN"))
                    return false;
            }

            if (btnAdd.Text == "&Save" && !MainPage.strUserRole.Contains("ADMIN"))
                chkEmail.Checked = true;

            bool _bStatus = dba.ValidateForwardDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;

            if (txtBillStatus.Text != "STOCK")
            {
                if (txtBStation.Text == "")
                {
                    MessageBox.Show("Sorry ! Booking station can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBStation.Focus();
                    return false;
                }
            }

            if (rdoPacked.Checked)
            {
                if (txtBillStatus.Text != "STOCK" && txtPackedBillNo.Text == "" && !MainPage.strUserRole.Contains("ADMIN"))
                {
                    if (txtPackerName.Text == "")
                    {
                        MessageBox.Show("Sorry ! Packer name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPackerName.Focus();
                        return false;
                    }
                    if (txtCartonType.Text == "")
                    {
                        MessageBox.Show("Sorry ! Carton Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCartonType.Focus();
                        return false;
                    }
                    if (txtCartonSize.Text == "")
                    {
                        MessageBox.Show("Sorry ! Carton size can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCartonSize.Focus();
                        return false;
                    }
                    if (txtPackingDate.Text.Length != 10)
                    {
                        MessageBox.Show("Sorry ! Packing date is not valid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPackingDate.Focus();
                        return false;
                    }
                    if (txtBillCode.Text.Contains("DLS"))
                    {
                        if (txtPetiAgent.Text == "")
                        {
                            MessageBox.Show("Sorry ! Peti agent can't be blank.", "Peti Agent required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPetiAgent.Focus();
                            return false;
                        }
                        if (txtPackingType.Text == "")
                        {
                            MessageBox.Show("Sorry ! Packing type can't be blank.", "Packing type required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtPackingType.Focus();
                            return false;
                        }
                    }
                    if (dba.ConvertObjectToDouble(txtPacking.Text) == 0)
                    {
                        MessageBox.Show("Packing amount can't zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPacking.Focus();
                        //return false;
                    }
                    //if (dba.ConvertObjectToDouble(txtPostage.Text) == 0)
                    //{
                    //    MessageBox.Show("Postage amount can't zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    txtPostage.Focus();
                    //    return false;
                    //}
                }
            }
            else if (rdoCameOffice.Checked && txtBillCode.Text.Contains("DLS"))
            {
                if (txtPackingDate.Text.Length != 10)
                {
                    MessageBox.Show("Sorry ! Packing date is not valid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPackingDate.Focus();
                    return false;
                }
                if (txtPetiAgent.Text == "")
                {
                    MessageBox.Show("Sorry ! Peti agent can't be blank.", "Peti Agent required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPetiAgent.Focus();
                    return false;
                }
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Atleast one entry is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdPending.Focus();
                return false;
            }
            double dAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
            if (dAmt == 0)
            {
                MessageBox.Show("Sorry ! Net amt can't be zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }

            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text).AddDays(1);

            string _strGRSNO = "", strCheckQuery = GetSalePendingGRSNO(ref _strGRSNO), strSaleBillGRSNo = GetSalBillGRSNO();
            if (btnEdit.Text == "&Update")
            {
                string strQuery = "Select TransactionLock,UPPER(Other1) as OrangeZone,State as SStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtTaxLedger.Text + "') Region,(Select TOP 1 StateName from CompanyDetails) CStateName,ISNULL((Select InsertStatus from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select UPPER(Tick) from BalanceAmount Where AccountStatus='SALES A/C' and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "'),'FALSE') TickStatus,(Select BDate from (Select ISNULL(MAX(BillDate),'') BDate,(Select CONVERT(varchar,BillDate,103) from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ")OldDate from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + ")_Sale Where BDate<='" + _date.ToString("MM/dd/yyyy") + "' )_Date," + strCheckQuery + " as SaleStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays,(Select TOP 1 PType from (Select Distinct STM.TaxIncluded as PType,(Select TaxIncluded from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtTaxLedger.Text + "') SType from PurchaseRecord PR inner join SaleTypeMaster STM on PR.TaxLedger=STM.TaxName and SaleType='PURCHASE'  WHere GRSNo in (" + _strGRSNO + ") )_PType Where PType!=SType)_SalesTypeCheck,(Select GSTNo from Transport Where TransportName='" + txtTransport.Text + "') TransportGSTNo,(Select CONVERT(nvarchar,_Date,103)_Date from (Select MAX(ReceivingDate) _Date from GoodsReceive Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_GR Where _Date> '" + _date.ToString("MM/dd/yyyy") + "') ExceedDate,(Select Count(*) from (Select Distinct ISNULL(OB.SchemeName,'') _Scheme from GoodsReceive GR Left Join OrderBooking OB on (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_Order)SchemeCount,(Select Count(*) from (Select Distinct ISNULL(OB.OfferName,'') _Scheme from GoodsReceive GR Left Join OrderBooking OB on (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_Order) OfferCount from SupplierMaster Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "'  ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["_Date"]) == "" && txtBillNo.Text!="1")
                    {
                        MessageBox.Show("Date can't be below from previous bill ! Please enter correct date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDate.Focus();
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["SaleStatus"]) != "")
                    {
                        MessageBox.Show("Sorry ! Goods Receipt no : " + Convert.ToString(dt.Rows[0]["SaleStatus"]) + " has been already sold ! Please Select Different GRSNo ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                    {
                        MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["OrangeZone"]) == "TRUE")
                    {
                        MessageBox.Show("This Account : " + txtSalesParty.Text + " is in orange list ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (Convert.ToString(dt.Rows[0]["_SalesTypeCheck"]) != "")
                    {
                        MessageBox.Show("Sorry ! Sales Type and Purchase Type doesn't match ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTaxLedger.Focus();
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["TransportGSTNo"]) == "" && txtTransport.Text != "" && !txtTransport.Text.Contains("BY HAND"))
                    {
                        MessageBox.Show("Sorry ! Transport is not valid, Please enter correct Transport ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTransport.Focus();
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["ExceedDate"]) != "")
                    {
                        MessageBox.Show("Sorry ! Purchase Date(" + dt.Rows[0]["ExceedDate"] + ") can't be greater than Sale Date  ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDate.Focus();
                        return false;
                    }
                    //if (dba.ConvertObjectToDouble(dt.Rows[0]["SchemeCount"]) > 1)
                    //{
                    //    MessageBox.Show("Sorry ! You can't add more than one scheme in a single sale bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}
                    if (dba.ConvertObjectToDouble(dt.Rows[0]["OfferCount"]) > 1)
                    {
                        MessageBox.Show("Sorry ! You can't add more than one offer in a single sale bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.Focus();
                        return false;
                    }

                    if (strOldPartyName != txtSalesParty.Text || dOldNetAmt != dAmt)
                    {
                        chkEmail.Checked = true;
                        if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(1) > MainPage.currentDate))
                        {
                            if (!Convert.ToBoolean(dt.Rows[0]["InsertStatus"]) && MainPage.strOnlineDataBaseName != "")
                            {
                                bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtBillCode.Text + " " + txtBillNo.Text);
                                if (!netStatus)
                                {
                                    MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                            else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
                            {
                                MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }

                            if (dba.ConvertObjectToDouble(dt.Rows[0]["BillDays"]) > 40)
                            {
                                DialogResult result = MessageBox.Show("Are you want to amend this bill for GSTR-1 if GSTR-1 has been filed ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    strAmendedQuery = " if not exists (Select [OBillCode] from [dbo].[AmendmentDetails] Where [OBillCode]='" + txtBillCode.Text + "' and [OBillNo]=" + txtBillNo.Text + " ) begin INSERT INTO [dbo].[AmendmentDetails]([BillType],[Date],[OBillCode],[OBillNo],[ODate],[ORBillCode],[ORBillNo],[ORDate],[Columnof1],[Columnof2],[Columnof3],[Columnof4],[Columnof5],[CreatedBy],[InsertStatus],[UpdateStatus]) Select 'SALES' as BillType,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) as [Date],BillCode,BillNo,BillDate,'',0,NULL,(Select TOP 1 GSTNo from SupplierMaster SM Where (AreaCode+AccountNo)=SalePartyID) as GSTNo,'','','','','" + MainPage.strLoginName + "',1,0 from SalesRecord Where BillNo=" + txtBillNo.Text + " and BillCode='" + txtBillCode.Text + "'  end ";
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
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
                else
                {
                    MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                string strQuery = "Select State as SStateName, (Select TOP 1 Region from SaleTypeMaster Where SaleType = 'SALES' and TaxName = '" + txtTaxLedger.Text + "') Region,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select BDate from (Select ISNULL(MAX(BillDate),DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BDate from SalesRecord WHere BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + ")_Sale Where BDate<'" + _date + "') BDate," + strCheckQuery + " as SaleStatus,(Select TOP 1 PType from (Select Distinct STM.TaxIncluded as PType,(Select TaxIncluded from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtTaxLedger.Text + "') SType from PurchaseRecord PR inner join SaleTypeMaster STM on PR.TaxLedger=STM.TaxName and SaleType='PURCHASE'  WHere GRSNo in (" + _strGRSNO + ") )_PType Where PType!=SType)_SalesTypeCheck,(Select GSTNo from Transport Where TransportName='" + txtTransport.Text + "') TransportGSTNo,(Select CONVERT(nvarchar,_Date,103)_Date from (Select MAX(ReceivingDate) _Date from GoodsReceive Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_GR Where _Date> '" + _date.ToString("MM/dd/yyyy") + "') ExceedDate,(Select Count(*) from (Select Distinct ISNULL(OB.SchemeName,'') _Scheme from GoodsReceive GR Left Join OrderBooking OB on (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_Order)SchemeCount,(Select Count(*) from (Select Distinct ISNULL(OB.OfferName,'') _Scheme from GoodsReceive GR Left Join OrderBooking OB on (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo Where (ReceiptCode+' '+CAST(ReceiptNo as nvarchar)) in (" + strSaleBillGRSNo + "))_Order) OfferCount from SupplierMaster Where GroupName != 'SUB PARTY' and(AreaCode + CAST(AccountNo as varchar) + ' ' + Name) = '" + txtSalesParty.Text + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {

                    if (Convert.ToString(dt.Rows[0]["BDate"]) == "" && txtBillNo.Text != "1")
                    {
                        MessageBox.Show("Date can't be below from previous bill ! Please enter correct date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDate.Focus();
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["SaleStatus"]) != "")
                    {
                        MessageBox.Show("Sorry ! Goods Receipt no : " + Convert.ToString(dt.Rows[0]["SaleStatus"]) + " has been already sold ! Please Select Different GRSNo ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    string strRegion = Convert.ToString(dt.Rows[0]["Region"]), strCStateName = Convert.ToString(dt.Rows[0]["CStateName"]).ToUpper(), strSStateName = Convert.ToString(dt.Rows[0]["SStateName"]).ToUpper();
                    if (strRegion == "LOCAL" && strSStateName != strCStateName)
                    {
                        MessageBox.Show("You are entering a central transaction for a party belonging to same state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //if (result == DialogResult.Yes)
                        //    return true;
                        //else
                        return false;
                    }
                    if (strRegion == "INTERSTATE" && strSStateName == strCStateName)
                    {
                        MessageBox.Show("You are entering a local  transaction for a party belonging to other  state.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //if (result == DialogResult.Yes)
                        //    return true;
                        //else
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["_SalesTypeCheck"]) != "")
                    {
                        MessageBox.Show("Sorry ! Sales Type and Purchase Type doesn't match ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTaxLedger.Focus();
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["TransportGSTNo"]) == "" && txtTransport.Text != "" && !txtTransport.Text.Contains("BY HAND"))
                    {
                        MessageBox.Show("Sorry ! Transport is not valid, Please enter correct Transport ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTransport.Focus();
                        return false;
                    }
                    if (Convert.ToString(dt.Rows[0]["ExceedDate"]) != "")
                    {
                        MessageBox.Show("Sorry ! Purchase Date(" + dt.Rows[0]["ExceedDate"] + ") can't be greater than Sale Date  ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDate.Focus();
                        return false;
                    }
                    //if (dba.ConvertObjectToDouble(dt.Rows[0]["SchemeCount"]) > 1)
                    //{
                    //    MessageBox.Show("Sorry ! You can't add more than one scheme in a single sale bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    dgrdDetails.Focus();
                    //    return false;
                    //}
                    if (dba.ConvertObjectToDouble(dt.Rows[0]["OfferCount"]) > 1)
                    {
                        MessageBox.Show("Sorry ! You can't add more than one offer in a single sale bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }

            if (btnAdd.Text == "&Save" || (dOldNetAmt != dAmt || strOldPartyName != txtSalesParty.Text))
                return ValidateAmountLimit(dAmt);
            else
                return true;
        }

        private bool CheckBillNoAndSuggest()
        {
            bool chkStatus = true;
            try
            {
                if (txtBillNo.Text != "")
                {
                    object objMax = DataBaseAccess.ExecuteMyScalar("Select ISNULL(Max(SaleBillNo),1) from MaxSerialNo");
                    int maxBillNo = Convert.ToInt32(objMax);
                    if (maxBillNo <= Convert.ToInt32(txtBillNo.Text))
                    {
                        int check = dba.CheckSaleBillAvailability(txtBillCode.Text, txtBillNo.Text);
                        if (check > 0)
                        {
                            string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from (Select ISNULL(Max(BillNo)+1,1)BillNo from SalesRecord where BillCode='" + txtBillCode.Text + "' UNION ALL Select ISNULL(Max(BillNo)+1,1)BillNo from SalesBook where BillCode='" + txtBillCode.Text + "')_Sales "));
                            MessageBox.Show("Sorry ! Bill No is already taken !! Your Bill Number  : " + strBillNo, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Text = strBillNo;
                            chkStatus = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("This Bill No is already in used please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtBillNo.Focus();
                        chkStatus = false;
                    }
                }
                else
                {
                    MessageBox.Show("Bill No can't be blank  ..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBillNo.Focus();
                    chkStatus = false;
                }
            }
            catch
            {
            }
            return chkStatus;
        }

        private string GetSalBillGRSNO()
        {
            string strGRSNo = "";
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (strGRSNo != "")
                        strGRSNo += ",";
                    strGRSNo += "'" + row.Cells["serialNo"].Value + "'";
                }
            }
            catch { }
            return strGRSNo;
        }

        private string GetSalePendingGRSNO(ref string _strGRSNO)
        {
            string strGRSNo = "";
            try
            {
                double dID = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dID = dba.ConvertObjectToDouble(row.Cells["gid"].Value);

                    if (dID == 0)
                    {
                        if (strGRSNo != "")
                            strGRSNo += ",";
                        strGRSNo += "'" + row.Cells["serialNo"].Value + "'";
                    }

                    if (_strGRSNO != "")
                        _strGRSNO += ",";
                    _strGRSNO += "'" + row.Cells["serialNo"].Value + "'";
                }

                if (strGRSNo != "")
                    strGRSNo = " (Select Top 1 ReceiptNo from GoodsReceive Where SaleBill='CLEAR' and (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (" + strGRSNo + ") UNION ALL Select Top 1 BillNo from SalesEntry Where BillNo!=" + txtBillNo.Text + " and GRSNo in (" + _strGRSNO + ") ) ";
                else
                    strGRSNo = " (Select '' Where ''!='') ";
            }
            catch { strGRSNo = " (Select '' Where ''!='') "; }
            return strGRSNo;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text != "&Update")
                {
                    if (btnAdd.Text == "&Add")
                    {
                        if (btnEdit.Text == "&Update")
                            btnEdit.Text = "&Edit";
                        ClearAllText();
                        btnAdd.Text = "&Save";
                        SetSerialNo();
                        EnableAllControls();
                        txtSalesParty.Enabled = txtSubParty.Enabled = rdoPacked.Enabled = rdoDirect.Enabled = rdoCameOffice.Enabled = chkEmail.Checked = chkSendSMS.Checked = true;
                        txtDate.Focus();
                        if (!MainPage.mymainObject.bSaleEdit)
                            btnEdit.Enabled = btnDelete.Enabled = false;
                        else
                            btnEdit.Enabled = btnDelete.Enabled = true;
                    }
                    else
                    {
                        if (ValidateAllControls(true) && CheckBillNoAndSuggest())
                        {
                            if (btnAdd.Enabled)
                            {
                                btnAdd.Enabled = false;
                                DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    SaveRecord();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strLRDate = "NULL", strPDate = "NULL", strNetBalanceSaveQuery = "", strPetiAgent = "DIRECT";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                strLRDate = "'" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strGRSNoQuery = "", _str = "";
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

                if (txtPetiAgent.Text != "" && txtPetiAgent.Text != "DIRECT")
                {
                    strFullName = txtPetiAgent.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPetiAgent = strFullName[0].Trim();
                    }
                }

                double dAmt = Convert.ToDouble(lblNetAmt.Text);

                strGRSNoQuery = GetSalePendingGRSNO(ref _str);

                string strQuery = " if not exists (" + strGRSNoQuery + ") begin if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + "  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ") begin  "
                                      + " INSERT INTO [dbo].[SalesRecord] ([BillCode],[BillNo],[BillDate],[SalesParty],[SubParty],[Transport],[Station],[Marka],[GoodsType],[DueDays],[PackerName],[PackingDate],[CartoneType],[CartoneSize],[NetAddLs],[LrNumber],[LrDate],[Parcel],"
                                      + " [Remark],[OtherPerText],[OtherPer],[OtherText],[Others],[OtherPacking],[Postage],[TotalPcs],[GrossAmt],[FinalAmt],[NetAmt],[ForwardingChallan],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[SalesType],[TaxAmount],[ServiceAmount],[GreenTaxAmt],[WayBillNo],[VehicleNo],[TimeOfSupply],[OtherField],[TaxPer],[AttachedBill],[BillStatus],[Description],[PackedBillNo],[WayBillDate],[Description_1],[Description_2],[RoundOffSign],[RoundOffAmt],[TaxableAmt],[IRNNO]) VALUES "
                                      + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strSaleParty + "','" + strSubParty + "','" + txtTransport.Text + "','" + txtBStation.Text + "','" + txtPvtMarka.Text + "','" + GetPackingStatus() + "', "
                                      + " '" + txtDueDays.Text + "','" + txtPackerName.Text + "'," + strPDate + ",'" + txtCartonType.Text + "','" + txtCartonSize.Text + "','" + lblNetAddLs.Text + "','" + txtLRNumber.Text + "'," + strLRDate + ",'0','" + txtRemarks.Text + "','" + txtOtherPerText.Text + "', "
                                      + " '" + txtSignPer.Text + txtOtherPerAmt.Text + "','" + txtOtherText.Text + "', '" + txtSign.Text + txtOtherAmt.Text + "','" + txtPacking.Text + "','" + txtPostage.Text + "','" + lblTotalPcs.Text + "','" + lblGrossAmt.Text + "','" + lblFinalAmt.Text + "','" + lblNetAmt.Text + "','PENDING','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + txtTaxLedger.Text + "'," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + "," + dba.ConvertObjectToDouble(txtServiceAmt.Text) + "," + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",'" + txtWayBillNo.Text + "','" + txtVehicleNo.Text + "','" + txtTimeOfSupply.Text + "','" + txtNoofCases.Text + "'," + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",'" + txtAttachedBill.Text + "','" + txtBillStatus.Text + "','" + txtDescription.Text + "','" + txtPackedBillNo.Text + "','" + txtWayBIllDate.Text + "','" + strPetiAgent + "','" + txtPackingType.Text + "','" + lblRoundOffSign.Text + "'," + lblRoundOffAmt.Text + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",'" + txtIRNNo.Text + "') ";

                strNetBalanceSaveQuery += " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                      + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','DR','" + lblFinalAmt.Text + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "') ";

                strQuery += strNetBalanceSaveQuery;

                if (txtTaxLedger.Text != "" && MainPage._bTaxStatus)
                {
                    double dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                    if (dTaxAmt > 0)
                    {
                        strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ;"
                                 + " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtTaxLedger.Text + "'; "
                                 + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                 + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                 + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                 + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                    }
                }

                string strGRSNo = "", strSupplier = "", strPurchaseStatus = "", strPersonalStatus = "", strPurchasePartyID = "", strTaxAccountID = "";
                double dRemPcs = 0;
                bool _pStatus = false;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strGRSNo = Convert.ToString(row.Cells["serialNo"].Value);
                    strSupplier = Convert.ToString(row.Cells["partyName"].Value);
                    dRemPcs = dba.ConvertObjectToDouble(row.Cells["rempcs"].Value);
                    _pStatus = Convert.ToBoolean(row.Cells["purchaseStatus"].Value);

                    strPurchasePartyID = "";
                    if (strSupplier != "PERSONAL")
                    {
                        strFullName = strSupplier.Split(' ');
                        if (strFullName.Length > 0)
                        {
                            strPurchasePartyID = strFullName[0].Trim();
                            strSupplier = strSupplier.Replace(strPurchasePartyID + " ", "");
                        }
                        strPersonalStatus = "NO";
                        strPurchaseStatus = "PENDING";
                    }
                    else
                    {
                        strPersonalStatus = "YES";
                        strPurchaseStatus = "CLEAR";
                    }
                    if (_pStatus)
                        strPurchaseStatus = "CLEAR";

                    if (strGRSNo.Contains("RM "))
                    {
                        strQuery += " Update GoodsReturned Set Status='CLEAR',AdjustedSaleBillNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' ,UpdateStatus=1  Where  SerialNo='" + strGRSNo + "' ";
                    }
                    else
                    {
                        strQuery += " INSERT INTO [dbo].[SalesEntry] ([BillCode],[BillNo],[SalesFrom],[GRSNo],[SupplierName],[Pieces],[Items],[Discount],[DiscountStatus],[SNDhara],[Amount],[Packing],[Freight],[Tax],[TotalAmt],[PBill],[RemPcs],[BillDate],[PurchaseBill],[Personal],[InsertStatus],[UpdateStatus],[PurchasePartyID],[PurchaseStatus]) VALUES  "
                                      + " ('" + txtBillCode.Text + "','" + txtBillNo.Text + "','" + row.Cells["remark"].Value + "','" + strGRSNo + "','" + strSupplier + "','" + row.Cells["pcs"].Value + "','" + row.Cells["item"].Value + "','" + row.Cells["Disc"].Value + "','" + row.Cells["DiscountStatus"].Value + "','" + row.Cells["sndhara"].Value + "','" + row.Cells["amount"].Value + "','" + row.Cells["packing"].Value + "','" + row.Cells["freight"].Value + "',"
                                      + " '" + row.Cells["tax"].Value + "','" + row.Cells["total"].Value + "','','" + row.Cells["rempcs"].Value + "','" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strPurchaseStatus + "','" + strPersonalStatus + "',1,0,'" + strPurchasePartyID + "','" + _pStatus + "') "
                                      + " Update GoodsReceive set SaleBill='CLEAR',UpdateStatus=1 where (ReceiptCode+' '+CAST(ReceiptNo as varchar))='" + strGRSNo + "' "
                                      + " Update PurchaseRecord set SaleBillNo='" + txtBillCode.Text + " " + txtBillNo.Text + "',[Discount]='" + row.Cells["Disc"].Value + "',[DiscountStatus]='" + row.Cells["DiscountStatus"].Value + "' where GRSNo='" + strGRSNo + "' ";

                    }

                    if (dRemPcs > 0)
                    {
                        strQuery += " INSERT INTO [dbo].[GoodsReturned] ([SalesParty],[SubParty],[SalesBill],[SalesFrom],[SerialNo],[PartyName],[Pieces],[Item],[Status],[AdjustedSaleBillNumber],[Date],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID]) VALUES "
                                      + " ('" + strSaleParty + "','" + strSubParty + "','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + row.Cells["remark"].Value + "','RM " + strGRSNo + "','" + strSupplier + "','" + row.Cells["rempcs"].Value + "','" + row.Cells["item"].Value + "','PENDING','','" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "') ";
                    }
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
                             + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                string strNetQuery = "";
                if (txtAttachedBill.Text != "")
                {
                    strNetQuery = " Update [dbo].[SalesRecord] Set [Description_1]='" + strPetiAgent + "',[Description_2]='" + txtPackingType.Text + "',[Transport]='" + txtTransport.Text + "',[Station]='" + txtBStation.Text + "',[CartoneType]='" + txtCartonType.Text + "', [BillStatus]='" + txtBillStatus.Text + "',[PackedBillNo]='" + txtBillCode.Text + " " + txtBillNo.Text + "', [LrNumber]='" + txtLRNumber.Text + "',[LrDate]=" + strLRDate + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo] in (" + txtAttachedBill.Text + ")  ";
                }

                if (chkCourier.Checked && !pnlNOCourier.Visible)
                    strQuery += dba.SaveCourierDetails(txtBillCode.Text, txtBillNo.Text, strSalePartyID, strSaleParty, txtBStation.Text);

                strQuery += strNetQuery + " end end";

                strNetQuery += " if not exists (Select AccountID from BalanceAmount Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C') begin " + strNetBalanceSaveQuery + " end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record saved successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    // if (strNetQuery != "")
                    DataBaseAccess.CreateDeleteQuery(strNetQuery);

                    string strMobileNo = "", strPath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                    SendSMSToParty(strMobileNo);

                    if (chkEmail.Checked)
                        NotificationClass.SetNotification("SALES", strSalePartyID, dAmt, txtBillCode.Text + " " + txtBillNo.Text);

                    AskForPrint(strPath);

                    if (newStatus)
                    {
                        btnAdd.Text = "&Add";
                        strNewAddedGRSNO = "";
                        ClearAllText();
                        BindRecordWithControl(txtBillNo.Text);
                        this.Close();
                    }
                    else
                    {
                        btnAdd.Text = "&Add";
                        strNewAddedGRSNO = "";
                        ClearAllText();
                        BindRecordWithControl(txtBillNo.Text);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! An error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Sales Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save")
                {
                    if (btnEdit.Text == "&Edit")
                    {
                        //if (btnAdd.Text == "&Save")
                        //{
                        //    BindLastRecord();
                        //    btnAdd.Text = "&Add";
                        //}

                        if (btnEdit.Enabled)
                        {
                            btnEdit.Text = "&Update";
                            EnableAllControls();
                            chkEmail.Checked = chkSendSMS.Checked = true;
                            chkCourier.Checked = false;
                            if (dgrdDetails.Rows.Count > 0)
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells[2];
                            txtBillNo.ReadOnly = true;
                            btnAdd.TabStop = false;
                            txtDate.Focus();
                        }
                        else
                            return;

                    }
                    else
                    {
                        strAmendedQuery = "";
                        if (ValidateAllControls(true))
                        {
                            string strBiltyPath = "";                           
                            if (strOldLRNumber != txtLRNumber.Text && !txtLRNumber.Text.Contains("PKD") && !txtLRNumber.Text.Contains("HAND") && !txtLRNumber.Text.Contains("BUS") && !txtLRNumber.Text.Contains("MISS") && txtLRNumber.Text != "")
                            {
                                strBiltyPath = DataBaseAccess.GetBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
                                if (strBiltyPath == "" && !MainPage.strUserRole.Contains("SUPERADMIN"))
                                    return;
                                chkEmail.Checked = false;
                                chkCourier.Checked = true;
                            }
                            if (btnEdit.Enabled)
                            {
                                btnEdit.Enabled = false;
                                DialogResult result = MessageBox.Show("Are you sure want to update record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    UpdateRecord(strBiltyPath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnEdit.Enabled = true;
        }

        private void UpdateRecord(string strBiltyPath)
        {
            try
            {
                int count = UpdateRecordAndReturnInt();
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    string strMobileNo = "", strFilePath = "";
                    SendEmailToSalesParty(false, ref strMobileNo, ref strFilePath);
                    if (strBiltyPath != "")
                        SendEmailBiltyToSalesParty(false, ref strMobileNo, ref strBiltyPath);
                    SendSMSToParty(strMobileNo);

                    AskForPrint(strFilePath);

                    btnEdit.Text = "&Edit";
                    BindRecordWithControl(txtBillNo.Text);
                    btnClose.Enabled = MainPage.mymainObject.ControlBox = MainPage.mymainObject.mStrip.Enabled = true;
                    txtBillNo.ReadOnly = false;
                    btnAdd.Enabled = MainPage.mymainObject.bSaleAdd;
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Update Record in Sales Book ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private int UpdateRecordAndReturnInt()
        {
            string strLRDate = "NULL", strPDate = "NULL", strBillingStatus = txtBillStatus.Text, strPetiAgent = "DIRECT";
            DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

            strLRDate = "'" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";

            if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
            if (txtPackingDate.Text.Length == 10)
                strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

            string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strGRSNoQuery = "", strTaxAccountID = "", _str = "";
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
            if (txtPetiAgent.Text != "" && txtPetiAgent.Text != "DIRECT")
            {
                strFullName = txtPetiAgent.Text.Split(' ');
                if (strFullName.Length > 0)
                {
                    strPetiAgent = strFullName[0].Trim();
                }
            }

            if (txtWayBillNo.Text != "" && txtBillStatus.Text == "BILLED")
                strBillingStatus = "SHIPPED";

            double dAmt = Convert.ToDouble(lblNetAmt.Text);
            strGRSNoQuery = GetSalePendingGRSNO(ref _str);

            string strQuery = " if not exists (" + strGRSNoQuery + ") begin " + strAmendedQuery + " UPDATE [dbo].[SalesRecord] SET [BillDate]='" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',[SalesParty]='" + strSaleParty + "',[SubParty]='" + strSubParty + "',[Transport]='" + txtTransport.Text + "',[Station]='" + txtBStation.Text + "',[Marka]='" + txtPvtMarka.Text + "',"
                                    + " [GoodsType]='" + GetPackingStatus() + "',[DueDays]='" + txtDueDays.Text + "',[PackerName]='" + txtPackerName.Text + "',[PackingDate]=" + strPDate + ",[CartoneType]='" + txtCartonType.Text + "',[CartoneSize]='" + txtCartonSize.Text + "',[NetAddLs]='" + lblNetAddLs.Text + "',"
                                    + " [LrNumber]='" + txtLRNumber.Text + "',[LrDate]=" + strLRDate + ",[Remark]='" + txtRemarks.Text + "',[OtherPerText]='" + txtOtherPerText.Text + "',[OtherPer]='" + txtSignPer.Text + txtOtherPerAmt.Text + "',[OtherText]='" + txtOtherText.Text + "',[Others]='" + txtSign.Text + txtOtherAmt.Text + "',[ServiceAmount]=" + dba.ConvertObjectToDouble(txtServiceAmt.Text) + ",[GreenTaxAmt]=" + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",[WayBillNo]='" + txtWayBillNo.Text + "',[VehicleNo]='" + txtVehicleNo.Text + "',[TimeOfSupply]='" + txtTimeOfSupply.Text + "',[OtherField]='" + txtNoofCases.Text + "',"
                                    + " [OtherPacking]='" + txtPacking.Text + "',[Postage]='" + txtPostage.Text + "',[TotalPcs]='" + lblTotalPcs.Text + "',[GrossAmt]='" + lblGrossAmt.Text + "',[FinalAmt]='" + lblFinalAmt.Text + "',[NetAmt]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SalesType]='" + txtTaxLedger.Text + "',[TaxAmount]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[TaxPer]= " + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[AttachedBill]='" + txtAttachedBill.Text + "',[BillStatus]='" + strBillingStatus + "',[Description]='" + txtDescription.Text + "',[WayBillDate]='" + txtWayBIllDate.Text + "',[Description_1]='" + strPetiAgent + "',[Description_2]='" + txtPackingType.Text + "',[RoundOffSign]='" + lblRoundOffSign.Text + "',[RoundOffAmt]=" + lblRoundOffAmt.Text + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[IRNNO]='" + txtIRNNo.Text + "' WHERE [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                    + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',[PartyName]='" + strSaleParty + "',[Amount]='" + lblNetAmt.Text + "',[FinalAmount]='" + lblFinalAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strSalePartyID + "' Where [AccountStatus]='SALES A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                                    + " Delete from[dbo].[BalanceAmount] Where[AccountStatus]='DUTIES & TAXES' AND[Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                                    + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";

            if (txtTaxLedger.Text != "" && MainPage._bTaxStatus)
            {
                double dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);

                if (dTaxAmt > 0)
                {
                    strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ;"
                             + " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtTaxLedger.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
                }
            }

            string strGRSNo = "", strSupplier = "", strPurchaseStatus = "", strPersonalStatus = "", strID = "", strPurchasePartyID = "";
            double dRemPcs = 0;
            bool _pStatus = false;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strID = Convert.ToString(row.Cells["gid"].Value);
                strGRSNo = Convert.ToString(row.Cells["serialNo"].Value);
                strSupplier = Convert.ToString(row.Cells["partyName"].Value);
                dRemPcs = dba.ConvertObjectToDouble(row.Cells["rempcs"].Value);
                _pStatus = Convert.ToBoolean(row.Cells["purchaseStatus"].Value);
                strPurchasePartyID = "";
                if (strSupplier != "PERSONAL")
                {
                    strFullName = strSupplier.Split(' ');
                    if (strFullName.Length > 0)
                    {
                        strPurchasePartyID = strFullName[0].Trim();
                        strSupplier = strSupplier.Replace(strPurchasePartyID + " ", "");
                    }
                    strPersonalStatus = "NO";
                    strPurchaseStatus = "PENDING";
                }
                else
                {
                    strPersonalStatus = "YES";
                    strPurchaseStatus = "CLEAR";
                }

                if (_pStatus)
                    strPurchaseStatus = "CLEAR";

                if (strID == "")
                {
                    if (strGRSNo.Contains("RM "))
                    {
                        strQuery += " Update GoodsReturned Set Status='CLEAR',AdjustedSaleBillNumber='" + txtBillCode.Text + " " + txtBillNo.Text + "' ,UpdateStatus=1  Where  SerialNo='" + strGRSNo + "' ";
                    }
                    else
                    {
                        strQuery += " INSERT INTO [dbo].[SalesEntry] ([BillCode],[BillNo],[SalesFrom],[GRSNo],[SupplierName],[Pieces],[Items],[Discount],[DiscountStatus],[SNDhara],[Amount],[Packing],[Freight],[Tax],[TotalAmt],[PBill],[RemPcs],[BillDate],[PurchaseBill],[Personal],[InsertStatus],[UpdateStatus],[PurchasePartyID],[PurchaseStatus]) VALUES  "
                                      + " ('" + txtBillCode.Text + "','" + txtBillNo.Text + "','" + row.Cells["remark"].Value + "','" + strGRSNo + "','" + strSupplier + "','" + row.Cells["pcs"].Value + "','" + row.Cells["item"].Value + "','" + row.Cells["Disc"].Value + "','" + row.Cells["DiscountStatus"].Value + "','" + row.Cells["sndhara"].Value + "','" + row.Cells["amount"].Value + "','" + row.Cells["packing"].Value + "','" + row.Cells["freight"].Value + "',"
                                      + " '" + row.Cells["tax"].Value + "','" + row.Cells["total"].Value + "','','" + row.Cells["rempcs"].Value + "','" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "','" + strPurchaseStatus + "','" + strPersonalStatus + "',1,0,'" + strPurchasePartyID + "','" + _pStatus + "') "
                                      + " Update GoodsReceive set SaleBill='CLEAR',UpdateStatus=1 where (ReceiptCode+' '+CAST(ReceiptNo as varchar))='" + strGRSNo + "' "
                                      + " Update PurchaseRecord set SaleBillNo='" + txtBillCode.Text + " " + txtBillNo.Text + "',[Discount]='" + row.Cells["Disc"].Value + "',[DiscountStatus]='" + row.Cells["DiscountStatus"].Value + "' where GRSNo='" + strGRSNo + "' ";
                    }

                    if (dRemPcs > 0)
                    {
                        strQuery += " INSERT INTO [dbo].[GoodsReturned] ([SalesParty],[SubParty],[SalesBill],[SalesFrom],[SerialNo],[PartyName],[Pieces],[Item],[Status],[AdjustedSaleBillNumber],[Date],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID]) VALUES "
                                      + " ('" + strSaleParty + "','" + strSubParty + "','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + row.Cells["remark"].Value + "','RM " + strGRSNo + "','" + strSupplier + "','" + row.Cells["rempcs"].Value + "','" + row.Cells["item"].Value + "','PENDING','','" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "') ";
                    }
                }
                else if (!strGRSNo.Contains("RM "))
                {
                    if (strSupplier == "PERSONAL")
                        strPersonalStatus = ",[PurchaseBill]='CLEAR',[Personal]='YES' ";
                    else
                        strPersonalStatus = ",[PurchaseBill]='PENDING',[Personal]='NO' ";

                    strQuery += " UPDATE [dbo].[SalesEntry]  SET [SupplierName]='" + strSupplier + "',[Pieces]='" + row.Cells["pcs"].Value + "',[Items]='" + row.Cells["item"].Value + "',[Discount]='" + row.Cells["Disc"].Value + "',[DiscountStatus]='" + row.Cells["DiscountStatus"].Value + "', "
                                  + " [SNDhara]='" + row.Cells["sndhara"].Value + "',[Amount]='" + row.Cells["amount"].Value + "',[Packing]='" + row.Cells["packing"].Value + "',[Freight]='" + row.Cells["freight"].Value + "',[Tax]='" + row.Cells["tax"].Value + "',[TotalAmt]='" + row.Cells["total"].Value + "',"
                                  + " [RemPcs]='" + row.Cells["rempcs"].Value + "',[UpdateStatus]=1 " + strPersonalStatus + ",[SalesFrom] ='" + row.Cells["remark"].Value + "',[PurchasePartyID]='" + strPurchasePartyID + "',PurchaseStatus='" + _pStatus + "',[BillDate]='" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "' WHERE [BillCode]='" + txtBillCode.Text + "' AND [BillNo]=" + txtBillNo.Text + " AND [GRSNo]='" + strGRSNo + "' "
                                  + " UPDATE SE Set PurchaseBill='CLEAR' from SalesEntry SE inner join PurchaseRecord PR ON SE.GRSNO=PR.GRSNO Where SE.GRSNO='" + strGRSNo + "' and PurchaseBill='PENDING' ";

                    if (dRemPcs > 0)
                    {
                        strQuery += "  if not exists (Select * from [dbo].[GoodsReturned] Where [SalesBill]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [SerialNo]='RM " + strGRSNo + "' ) begin "
                                  + " INSERT INTO [dbo].[GoodsReturned] ([SalesParty],[SubParty],[SalesBill],[SalesFrom],[SerialNo],[PartyName],[Pieces],[Item],[Status],[AdjustedSaleBillNumber],[Date],[InsertStatus],[UpdateStatus],[SalePartyID],[SubPartyID],[PurchasePartyID]) VALUES "
                                  + " ('" + strSaleParty + "','" + strSubParty + "','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + row.Cells["remark"].Value + "','RM " + strGRSNo + "','" + strSupplier + "','" + row.Cells["rempcs"].Value + "','" + row.Cells["item"].Value + "','PENDING','','" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',1,0,'" + strSalePartyID + "','" + strSubPartyID + "','" + strPurchasePartyID + "') end "
                                  + "  else begin Update [dbo].[GoodsReturned] SET [SalesParty]='" + strSaleParty + "',[SubParty]='" + strSubParty + "',[PartyName]='" + strSupplier + "',[Pieces]='" + row.Cells["rempcs"].Value + "',[Item]='" + row.Cells["item"].Value + "',[Date]='" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',[UpdateStatus]=1,[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "' WHERE [SalesBill]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [SerialNo]='RM " + strGRSNo + "' end ";
                    }
                    else
                    {
                        strQuery += " if exists (Select * from [dbo].[GoodsReturned] Where [SalesBill]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [SerialNo]='RM " + strGRSNo + "' ) begin "
                                    + " Delete from [dbo].[GoodsReturned] Where [SalesBill]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [SerialNo]='RM " + strGRSNo + "' end ";
                    }
                }
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
                         + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
            }

            strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";
            string strNetQuery = "";

            //if (strAllAttachedBillNo != txtAttachedBill.Text)
            //{
            if (strAllAttachedBillNo != "")
                strNetQuery += " Update [dbo].[SalesRecord] Set [Description_1]='',[Description_2]='',[BillStatus]='BILLED',[PackedBillNo]='', [LrNumber]='',[LrDate]=" + strLRDate + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo] in (" + strAllAttachedBillNo + ")  ";
            if (txtAttachedBill.Text != "")
                strNetQuery += " Update [dbo].[SalesRecord] Set [Description_1]='" + strPetiAgent + "',[Description_2]='" + txtPackingType.Text + "',[Transport]='" + txtTransport.Text + "',[Station]='" + txtBStation.Text + "',[CartoneType]='" + txtCartonType.Text + "',[BillStatus]='" + txtBillStatus.Text + "',[PackedBillNo]='" + txtBillCode.Text + " " + txtBillNo.Text + "', [LrNumber]='" + txtLRNumber.Text + "',[LrDate]=" + strLRDate + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo] in (" + txtAttachedBill.Text + ")  ";
            //}
            //else if(txtAttachedBill.Text!="")
            //    strNetQuery += " Update [dbo].[SalesRecord] Set [BillStatus]='" + txtBillStatus.Text + "',[PackedBillNo]='" + txtBillCode.Text + " " + txtBillNo.Text + "', [LrNumber]='" + txtLRNumber.Text + "',[LrDate]=" + strLRDate + " Where [BillCode]='" + txtBillCode.Text + "' and [BillNo] in (" + txtAttachedBill.Text + ")  ";

            if (chkCourier.Checked && !pnlNOCourier.Visible)
                strQuery += dba.SaveCourierDetails(txtBillCode.Text, txtBillNo.Text, strSalePartyID, strSaleParty, txtBStation.Text);

            strQuery += strNetQuery + " end ";

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0 && strNetQuery != "")
            {
                DataBaseAccess.CreateDeleteQuery(strNetQuery);

                if (chkEmail.Checked)
                    NotificationClass.SetNotification("UPDATESALES", strSalePartyID, dAmt, txtBillCode.Text + " " + txtBillNo.Text);
            }

            return count;
        }    
        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save")
                {
                    pnlDeletionConfirmation.Visible = true;
                    txtReason.Focus();
                }
            }
            catch
            {
            }
        }

        private bool ValidationForDeletion(ref bool iStatus)
        {
            try
            {
                if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                {
                    string strQuery = "Select TransactionLock,ISNULL((Select TOP 1 InsertStatus from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,ISNULL((Select Top 1 UPPER(Tick) from BalanceAmount Where AccountStatus='SALES A/C' and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "'),'FALSE') TickStatus,ISNULL((Select AdjustedSaleBillNumber from GoodsReturned Where SalesBill='" + txtBillCode.Text + " " + txtBillNo.Text + "' and AdjustedSaleBillNumber!='' and Status='CLEAR'),'') AdjustSNo,(Select ISNULL(Count(*),0) from SalesEntry Where PurchaseStatus=0 and PurchaseBill='CLEAR' and SupplierName!='PERSONAL' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ") PurchaseCount from SupplierMaster Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ";

                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);

                        if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                        {
                            MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! You can't delete this bill ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dba.ConvertObjectToDouble(dt.Rows[0]["PurchaseCount"]) > 0)
                        {
                            MessageBox.Show("Sorry ! Purchase has been made of this bill, please delete purchase bill ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                        if (!iStatus && MainPage.strOnlineDataBaseName != "")
                        {
                            bool netStatus = DataBaseAccess.CheckPartyAdjustedAmount(txtBillCode.Text + " " + txtBillNo.Text);
                            if (!netStatus)
                            {
                                MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        else if (Convert.ToString(dt.Rows[0]["TickStatus"]) == "TRUE")
                        {
                            MessageBox.Show("Sorry ! This bill has been adjusted, Please unadjust this bill after that you can change ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        if (Convert.ToString(dt.Rows[0]["AdjustSNo"]) != "")
                        {
                            MessageBox.Show("Sorry ! The remaining pcs of this bill has been adjusted in sale bill no : " + dt.Rows[0]["AdjustSNo"] + ", Please remove from there ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        MessageBox.Show("Sorry ! Atleast one entry is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdPending.Focus();
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have sufficient permission to change Amount/Party name ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    {
                        if (e.ColumnIndex == 1 || e.ColumnIndex == 8 || e.ColumnIndex == 14)
                        {
                            string strSerialNo = "", strPStatus = "";
                            strSerialNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["serialNo"].Value);
                            strPStatus = Convert.ToString(dgrdDetails.CurrentRow.Cells["pBill"].Value);
                            if (strPStatus.ToUpper() != "CLEAR")
                            {
                                if (e.ColumnIndex == 8)
                                {
                                    if (!strSerialNo.Contains("RM ") && !Convert.ToBoolean(dgrdDetails.CurrentRow.Cells["purchaseStatus"].Value))
                                    {
                                        SearchData objSearch = new SearchData("DHARA", "SEARCH DHARA TYPE", Keys.Space);
                                        objSearch.ShowDialog();
                                        if (objSearch.strSelectedData != "")
                                        {
                                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                            GetDharaDetails(dgrdDetails.CurrentRow);
                                        }
                                    }
                                    e.Cancel = true;
                                }
                                else if (e.ColumnIndex == 14)
                                {
                                    if (strSerialNo.Contains("RM "))
                                        e.Cancel = true;
                                }
                            }
                            else
                            {
                                if (e.ColumnIndex != 1 || !Convert.ToBoolean(dgrdDetails.CurrentRow.Cells["purchaseStatus"].Value))
                                {
                                    MessageBox.Show("Sorry ! This serial no has been purchased, Please delete purchase after that you can change it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    e.Cancel = true;
                                }
                            }
                        }
                        else
                            e.Cancel = true;
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 14)
                {
                    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    {
                        double dPcs = dba.ConvertObjectToDouble(dgrdDetails.CurrentRow.Cells["pcs"].Value), dRemPcs = dba.ConvertObjectToDouble(dgrdDetails.CurrentRow.Cells["rempcs"].Value);
                        if (dRemPcs < dPcs)
                        {
                            CalculateGridAmount();
                        }
                        else
                        {
                            dgrdDetails.CurrentCell.Value = 0;
                            MessageBox.Show("Sorry ! Remaining pcs can't be greater than original pcs !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (dgrdDetails.CurrentCell.ColumnIndex == 14)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdDetails.CurrentCell.ColumnIndex == 14)
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 2)
                    {
                        string strGRSNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (!strGRSNo.Contains("RM "))
                        {
                            string[] strSerial = strGRSNo.Split(' ');
                            if (strSerial.Length > 1)
                            {
                                if (Control.ModifierKeys == Keys.Control)
                                {
                                    DataBaseAccess.ShowPDFFiles(strSerial[0], strSerial[1]);
                                }
                                else if (btnEdit.Text == "&Update")
                                {
                                    if (Convert.ToBoolean(dgrdDetails.CurrentRow.Cells["purchaseStatus"].Value))
                                    {
                                        GoodscumPurchase objGoods = new GoodscumPurchase(strSerial[0], strSerial[1], true);
                                        objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                        objGoods.ShowInTaskbar = true;
                                        objGoods.ShowDialog();
                                        if (objGoods.updateStatus)
                                            UpdateGridWithUpdatedValue(strSerial[0], strSerial[1], dgrdDetails.CurrentRow);
                                    }
                                    else if (Convert.ToString(dgrdDetails.CurrentRow.Cells["pBill"].Value).ToUpper() == "PENDING" || Convert.ToString(dgrdDetails.CurrentRow.Cells["partyName"].Value).ToUpper() == "PERSONAL")
                                    {
                                        GoodsReceipt objGoods = new GoodsReceipt(strSerial[0], strSerial[1], true);
                                        objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                        objGoods.ShowInTaskbar = true;
                                        objGoods.ShowDialog();
                                        if (objGoods.updateStatus)
                                            UpdateGridWithUpdatedValue(strSerial[0], strSerial[1], dgrdDetails.CurrentRow);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sorry ! This serial no has been purchased, Please delete purchase after that you can change it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    if (btnAdd.Text == "&Save")
                                    {
                                        if (Convert.ToBoolean(dgrdDetails.CurrentRow.Cells["purchaseStatus"].Value))
                                        {
                                            GoodscumPurchase objGoods = new GoodscumPurchase(strSerial[0], strSerial[1], true);
                                            objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                            objGoods.ShowInTaskbar = true;
                                            objGoods.ShowDialog();
                                            if (objGoods.updateStatus)
                                                UpdateGridWithUpdatedValue(strSerial[0], strSerial[1], dgrdDetails.CurrentRow);
                                        }
                                        else
                                        {
                                            GoodsReceipt objGoods = new GoodsReceipt(strSerial[0], strSerial[1], true);
                                            objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                            objGoods.ShowInTaskbar = true;
                                            objGoods.ShowDialog();
                                            if (objGoods.updateStatus)
                                                UpdateGridWithUpdatedValue(strSerial[0], strSerial[1], dgrdDetails.CurrentRow);
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToBoolean(dgrdDetails.CurrentRow.Cells["purchaseStatus"].Value))
                                        {
                                            GoodscumPurchase objGoods = new GoodscumPurchase(strSerial[0], strSerial[1], false);
                                            objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                            objGoods.ShowInTaskbar = true;
                                            objGoods.ShowDialog();
                                        }
                                        else
                                        {
                                            GoodsReceipt objGoods = new GoodsReceipt(strSerial[0], strSerial[1], false);
                                            objGoods.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                            objGoods.ShowInTaskbar = true;
                                            objGoods.ShowDialog();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (e.ColumnIndex == 3)
                    {
                        string strPartyName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strPartyName != "")
                        {
                            SupplierMaster objSupplier = new SupplierMaster(strPartyName);
                            objSupplier.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                            objSupplier.ShowInTaskbar = true;
                            objSupplier.Show();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateGridWithUpdatedValue(string strCode, string strNo, DataGridViewRow row)
        {
            string strQuery = " Select (ReceiptCode+' '+CAST(ReceiptNo as varchar)) RCode,(CASE When PurchaseParty!='' then dbo.GetFullName(PurchasePartyID) else 'PERSONAL' end) PParty,Item,Quantity,Amount,Packing,Freight,Tax,(Amount+CAST(Packing as Money)+CAST(Freight as Money)+CAST(Tax as Money)) TotalAmt,"
                            + " ISNULL(SM.NormalDhara,0) Dhara,GR.Remark,ISNULL(SM.Category,'') SCategory,Dhara PDhara,(CASE WHEN GR.PurchasePartyID='DL5255' and OB.SchemeName Like('%TOUR JAN%') then 0 else ((GR.DisPer*-1)+(CASE WHEN (SM.Category = 'CASH PURCHASE' OR SM.TINNumber = 'CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (SM.Category='CLOTH PURCHASE' OR ReceiptCode Like('%SRT%') OR ReceiptCode Like('%CCK%')) then 1 else 0 end)) end) Dis,ISNULL(PurchaseStatus,0)PurchaseStatus  from GoodsReceive GR inner join SupplierMaster SM on (AreaCode+ CAST(AccountNo as varchar))=GR.PurchasePartyID OUTER APPLY (Select Top 1 SchemeName from OrderBooking OB  Where (CASE When OB.NumberCode!='' then (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode) else OB.OrderCode+' '+CAST(OB.OrderNo as varchar) end)=GR.OrderNo) OB Where ReceiptCode='" + strCode + "' AND ReceiptNo=" + strNo;

            DataTable dt = dba.GetDataTable(strQuery);
            BindUpdatedGoodsReceiveDataWithGrid(dt, row);
        }

        private void BindUpdatedGoodsReceiveDataWithGrid(DataTable dt, DataGridViewRow dRow)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    string strGRSNo = "", strDhara = "", strPParty = "";
                    bool _pStatus = false;
                    double dDisPer = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        strGRSNo = Convert.ToString(row["RCode"]);
                        strDhara = Convert.ToString(row["Dhara"]);
                        strPParty = Convert.ToString(row["PParty"]);
                        _pStatus = Convert.ToBoolean(row["PurchaseStatus"]);

                        dRow.Cells["serialNo"].Value = strGRSNo;// row["RCode"];
                        dRow.Cells["partyName"].Value = strPParty;// row["PParty"];
                        dRow.Cells["pcs"].Value = row["Quantity"];
                        dRow.Cells["item"].Value = row["Item"];
                        dRow.Cells["amount"].Value = dba.ConvertObjectToDouble(row["Amount"]).ToString("N2", MainPage.indianCurancy);
                        dRow.Cells["packing"].Value = row["Packing"];
                        dRow.Cells["freight"].Value = row["Freight"];
                        dRow.Cells["tax"].Value = row["Tax"];
                        dRow.Cells["total"].Value = dba.ConvertObjectToDouble(row["TotalAmt"]).ToString("N2", MainPage.indianCurancy);
                        dRow.Cells["rempcs"].Value = "0";
                        dRow.Cells["remark"].Value = row["Remark"];
                        dRow.Cells["purchaseStatus"].Value = _pStatus;

                        if (_pStatus)
                        {
                            dDisPer = dba.ConvertObjectToDouble(row["Dis"]);
                            dRow.Cells["sndhara"].Value = row["PDhara"];
                            dRow.Cells["pBill"].Value = "CLEAR";
                            if (dDisPer >= 0)
                            {
                                dRow.Cells["DiscountStatus"].Value = "+";
                                dRow.Cells["Disc"].Value = dDisPer;
                            }
                            else
                            {
                                dRow.Cells["DiscountStatus"].Value = "-";
                                dRow.Cells["Disc"].Value = Math.Abs(dDisPer);

                            }
                        }
                        if (strGRSNo.Contains("RM ") || strPParty == "PERSONAL")
                        {
                            dRow.Cells["sndhara"].Value = "NORMAL";
                            dRow.Cells["DiscountStatus"].Value = "+";
                            dRow.Cells["Disc"].Value = 0;
                        }
                    }

                    CalculateGridAmount();
                    if (ValidateAllControls(false) && btnEdit.Text == "&Update")
                        UpdateRecordAndReturnInt();
                }
                else
                {
                    MessageBox.Show("Sorry ! An error occured, Please try again !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            strNewAddedGRSNO = "";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
        }

        private void SendSMSToParty(string strMobileNo)
        {
            try
            {
                if (chkSendSMS.Checked)
                {
                    if (strMobileNo == "")
                        strMobileNo = Convert.ToString(dba.GetPartyMobileNo(txtSalesParty.Text));

                    string strBalance = ".", strName = dba.GetSafePartyName(txtSalesParty.Text);
                    if (strMobileNo != "")
                    {
                        if (MainPage.strSendBalanceInSMS == "YES")
                        {
                            double dAmt = dba.GetPartyAmountFromQuery(txtSalesParty.Text);
                            if (dAmt > 0)
                                strBalance = " BAL : " + dAmt.ToString("0") + " Dr";
                            else if (dAmt < 0)
                                strBalance = " BAL : " + Math.Abs(dAmt).ToString("0") + " Cr";
                            else
                                strBalance = " BAL : 0";
                        }

                        string strMessage = "", strSubMsg = "";
                        if (txtTransport.Text != "")
                            strSubMsg = " thru : " + txtTransport.Text;
                        if (txtLRNumber.Text != "")
                            strSubMsg += ", LR No.  " + txtLRNumber.Text + " (" + txtLRDate.Text + ")";
                        if (txtRemarks.Text != "")
                            strSubMsg += ", Note : " + txtRemarks.Text;

                        if (btnAdd.Text == "&Save")
                            strMessage = "M/s " + strName + ", Sale bill created with bill no : " + txtBillCode.Text + " " + txtBillNo.Text + ", DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalPcs.Text + " SUPP. DETAILS :- " + GetPPartyName() + strSubMsg + strBalance;
                        else
                            strMessage = "M/s " + strName + ", Sale bill no : " + txtBillCode.Text + " " + txtBillNo.Text + " updated with dated : " + txtDate.Text + ", AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalPcs.Text + " SUPP. DETAILS :- " + GetPPartyName() + strSubMsg + strBalance;

                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetPPartyName()
        {
            string strParty = "";
            //if (dgrdDetails.Rows.Count == 1)
            //{
            //    strParty = Convert.ToString(dgrdDetails.Rows[0].Cells["party"].Value);
            //}
            int _index = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strParty += " (" + _index + ") " + dba.GetSafePartyName(Convert.ToString(row.Cells["partyName"].Value)) + " " + row.Cells["pcs"].Value;
                _index++;
            }
            strParty += ".";
            return strParty;
        }


        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    double dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
                    if (btnAdd.Text == "&Save" || (btnEdit.Text == "&Update" && (dOldNetAmt != dNetAmt || strOldPartyName != txtSalesParty.Text)) || _bStatus)
                    {
                        string strPath = SetSignatureInBill(false, false, true), strEmailID = "", strWhatsAppNo = "";
                        if (strPath != "")
                        {
                            strFilePath = strPath;
                            string[] strParty = txtSalesParty.Text.Split(' ');
                            if (strParty.Length > 1)
                            {
                                string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "' and GroupName='SUNDRY DEBTORS'  ";
                                DataTable _dt = dba.GetDataTable(strQuery);
                                if (_dt.Rows.Count > 0)
                                {
                                    strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                                    strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                                    strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                                    if (strEmailID != "")
                                    {
                                        SendMail(strEmailID, strPath, 0);
                                    }
                                    else if (_bStatus)
                                        MessageBox.Show("Sorry ! Please enter mail id in party master !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                    if (strWhatsAppNo != "")
                                    {
                                        SendWhatsappMessage(strWhatsAppNo, strPath);
                                    }
                                }
                                else
                                {
                                    DialogResult _updateResult = MessageBox.Show("Sorry ! Email/Whatsapp didn't sent, Please retry !! ", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                                    if (_updateResult == DialogResult.Retry)
                                        SendEmailToSalesParty(_bStatus, ref strMobileNo, ref strFilePath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SendWhatsappMessage(string strMobileNo, string strPath)
        {
            string strMsgType = "", _strFileName = txtBillCode.Text.Replace("21-22/", "").Replace("22-23/", "").Replace("19-20/", "").Replace("20-21/", "") + "_" + txtBillNo.Text + ".pdf", strBranchCode = txtBillCode.Text, strWhastappMessage = "";
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
            string strMType = "";
            if (btnEdit.Text == "&Update")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                strMsgType = "sale_bill_update";
                strMType = "invoice_update";
            }
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    dba.DeleteSaleBillFile(strPath, strBranchCode);

                strMsgType = "sale_bill";
                strMType = "invoice_generation";
            }


            bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            if (!_bStatus)
            {
                DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (_updateResult == DialogResult.Retry)
                    _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            }

            if (_bStatus)
            {

                strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + lblNetAmt.Text + "\",";
                if (strMobileNo != "")
                {
                    string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                    if (strResult != "")
                        MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            else
            {
                strWhastappMessage = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + lblNetAmt.Text + "\"}";
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strWhastappMessage, "", "");
            }
        }

        private void SendEmailBiltyToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (strFilePath != "")
                {

                    string strEmailID = "", strWhatsAppNo = "";

                    string[] strParty = txtSalesParty.Text.Split(' ');
                    if (strParty.Length > 1)
                    {
                        string strQuery = " Select EmailID,MobileNo,WhatsappNo from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo) SOD Where (SM.AreaCode+CAST(SM.AccountNo as varchar))='" + strParty[0] + "' and GroupName='SUNDRY DEBTORS'  ";
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            strEmailID = Convert.ToString(_dt.Rows[0]["EmailID"]);
                            strMobileNo = Convert.ToString(_dt.Rows[0]["MobileNo"]);
                            strWhatsAppNo = Convert.ToString(_dt.Rows[0]["WhatsappNo"]);

                            if (strEmailID != "")
                                SendMail(strEmailID, strFilePath, 0);

                            if (strWhatsAppNo != "")
                            {
                                SendBiltyWhatsappMessage(strWhatsAppNo, strFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void SendBiltyWhatsappMessage(string strMobileNo, string strPath)
        {
            string _strFileName = "Bilty_" + txtBillCode.Text.Replace("18 -19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strBranchCode = txtBillCode.Text;
            string strFilePath = "http://pdffiles.ssspltd.com/SALEBILL/" + strBranchCode + "/" + _strFileName, strName = dba.GetSafePartyName(txtSalesParty.Text);
            string strWhastappMessage = "", strMsgType = "", strTextMsg = "", strMType = "";

            if (btnEdit.Text == "&Update")
            {
                dba.DeleteSaleBillFile(strPath, strBranchCode);
                strMsgType = "bilty_update";
                strMType = "bilty_copy";
                strTextMsg = "{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + strFilePath + "\"}";
                strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + txtLRNumber.Text + "\",\"variable4\": \"" + txtLRDate.Text + "\",";
            }
            else
            {
                strMsgType = "sale_bill_update";
                strMType = "invoice_generation";
                strWhastappMessage = "\"variable1\": \"" + strName + "\",\"variable2\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\",\"variable3\": \"" + lblNetAmt.Text + "\",";
                strTextMsg = "{\"default\": \"" + strName + "\" },{\"default\": \"" + txtBillCode.Text + " " + txtBillNo.Text + "\" },{\"default\": \"" + lblNetAmt.Text + "\"}";
            }

            bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            if (_bStatus)
            {
                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strTextMsg, "", "");
        }

        //private void UploadPDFFile()
        //{
        //    string strPath = SetSignatureInBill(false, false);
        //    string _strFileName = txtBillCode.Text.Replace("18-19/", "") + txtBillNo.Text  + ".pdf"; //+ "_" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()

        //    bool _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName);
        //    if (_bStatus)
        //    {
        //        string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. Mobile No ! ", "Mobile No", "", 400, 300);
        //        if (strValue != "" && strValue != "0")
        //        {

        //            string strURL = "http://whatsapp-demo-ums.appspot.com/whatsapp/send?WhatsAppTo=91" + strValue + "&WhatsAppMsg=Sale Bill Generated&WhatsAppPDFUrl=http://pdffiles.ssspltd.com/SALEBILL/" + _strFileName;

        //            string strResult = DataBaseAccess.apicall(strURL);
        //            if (strResult != "")
        //                MessageBox.Show("Thank you ! Bill Send Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //            else
        //                MessageBox.Show("Sorry ! Unable to send bill ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        private void SendEmailForDeletion(string strFileName)
        {
            try
            {
                SendMail(MainPage.strSenderEmailID, strFileName, 1);
            }
            catch
            {
            }
        }

        private void SendMail(string strEmail, string strpath, int billStatus)
        {
            try
            {
                //MailAddress mailFrom = new MailAddress(MainPage.strSenderEmailID);
                //MailAddress mailTo = new MailAddress(strEmail);
                // MailMessage message = new MailMessage(mailFrom, mailTo);

                string strMessage = "", strSub = "";
                if (billStatus == 0)
                {
                    if (btnAdd.Text == "&Save" || (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit"))
                    {
                        strMessage = "A/c : " + txtSalesParty.Text + " , we have generated your sale bill <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b> and attached with this mail, please find it.";
                    }
                    else
                    {
                        strMessage = "Update ! A/c : " + txtSalesParty.Text + ", we have update your sale bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and bilty scan copy attached with this mail, please find it.";
                    }

                    if (btnAdd.Text == "&Save")
                        strSub = "Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " created.";
                    else
                        strSub = "Alert ! Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " updated.";
                }
                else
                {
                    strMessage = " Alert ! Sale bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + "</b> is Deleted by : " + MainPage.strLoginName + "  and  the deleted Sale bill is attached with this mail. ";
                    strSub = "Alert ! Sale bill no :  " + txtBillCode.Text + " " + txtBillNo.Text + " deleted by : " + MainPage.strLoginName;
                }

                bool bStatus = DataBaseAccess.SendEmail(strEmail, strSub, strMessage, strpath, "", "SALE BILL", true);
                if (billStatus == 0 && bStatus)
                {
                    MessageBox.Show("Thank you ! Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
        }

        //private string CreateSaleBillPDFFile()
        //{
        //    string strFileName = "";
        //    try
        //    {
        //        string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
        //        strFileName = strPath + "\\" + txtBillNo.Text + ".pdf";
        //        if (File.Exists(strFileName))
        //            File.Delete(strFileName);
        //        Directory.CreateDirectory(strPath);
        //        if (txtTaxLedger.Text != "")
        //        {
        //            GSTPrintAndPreview(false, strFileName,true);
        //        }
        //        else
        //        {
        //            Reporting.SalesReport report = new Reporting.SalesReport();
        //            DataTable dt = CreateDataTable();
        //            report.SetDataSource(dt);
        //            report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);
        //        }
        //    }
        //    catch
        //    {
        //        strFileName = "";
        //    }
        //    return strFileName;
        //}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetOtherAmountText()
        {
            string strOtherText = txtOtherPerText.Text;
            if (txtOtherText.Text != "")
            {
                if (strOtherText == "")
                    strOtherText = txtOtherText.Text;
                else
                    strOtherText += " & " + txtOtherText.Text;
            }
            if (strOtherText == "")
                strOtherText = "Other Amount";
            strOtherText += " : ";
            return strOtherText;
        }

        private DataTable CreateDataTable()
        {

            DataTable myDataTable = new DataTable();
            try
            {
                string[] strPartyDetail = dba.GetPartyAddress(txtSalesParty.Text);

                ChangeCurrencyToWord currency = new ChangeCurrencyToWord();
                string strNumeric = currency.changeCurrencyToWords(Convert.ToDouble(lblNetAmt.Text).ToString("0"));

                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("SalesParty", typeof(String));
                myDataTable.Columns.Add("Address", typeof(String));
                myDataTable.Columns.Add("BillNo", typeof(String));
                myDataTable.Columns.Add("BillDate", typeof(String));
                myDataTable.Columns.Add("LrNo", typeof(String));
                myDataTable.Columns.Add("LrDate", typeof(String));
                myDataTable.Columns.Add("Transport", typeof(String));
                myDataTable.Columns.Add("Station", typeof(String));
                myDataTable.Columns.Add("DueDays", typeof(String));
                myDataTable.Columns.Add("Marka", typeof(String));
                myDataTable.Columns.Add("Haste", typeof(String));
                myDataTable.Columns.Add("DueDate", typeof(String));
                myDataTable.Columns.Add("GRSNo", typeof(String));
                myDataTable.Columns.Add("Party", typeof(String));
                myDataTable.Columns.Add("Pieces", typeof(String));
                myDataTable.Columns.Add("Discount", typeof(String));
                myDataTable.Columns.Add("DisStatus", typeof(String));
                myDataTable.Columns.Add("Amount", typeof(String));
                myDataTable.Columns.Add("RPcs", typeof(String));
                myDataTable.Columns.Add("Remark", typeof(String));
                myDataTable.Columns.Add("NetDiscount", typeof(String));
                myDataTable.Columns.Add("Packing", typeof(String));
                myDataTable.Columns.Add("Postage", typeof(String));
                myDataTable.Columns.Add("PackPostage", typeof(String));
                myDataTable.Columns.Add("GrossAmount", typeof(String));
                myDataTable.Columns.Add("NetAmount", typeof(String));
                myDataTable.Columns.Add("AmountInWord", typeof(String));
                myDataTable.Columns.Add("PPacking", typeof(String));
                myDataTable.Columns.Add("Freight", typeof(String));
                myDataTable.Columns.Add("Tax", typeof(String));
                myDataTable.Columns.Add("PostOffice", typeof(String));
                myDataTable.Columns.Add("PhoneNo", typeof(String));
                myDataTable.Columns.Add("TotalPcs", typeof(String));
                myDataTable.Columns.Add("OtherText", typeof(String));
                myDataTable.Columns.Add("OtherAmount", typeof(String));
                myDataTable.Columns.Add("Items", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));

                double dGPacking = 0, dGFreight = 0, dGTax = 0, dPacking = 0, dPostage = 0, dOtherAmt = 0, dOtherPerAmt = 0, dNetOtherAmt = 0;
                dPacking = dba.ConvertObjectToDouble(txtPacking.Text);
                dPostage = dba.ConvertObjectToDouble(txtPostage.Text);
                dOtherAmt = dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmt.Text);
                dOtherPerAmt = dba.ConvertObjectToDouble(txtSignPer.Text + txtOtherPerAmt.Text);
                dNetOtherAmt = dPacking + dPostage + dOtherAmt + dOtherPerAmt;
                string strOtherText = "";
                if (dOtherAmt != 0 || dOtherPerAmt != 0)
                {
                    strOtherText = GetOtherAmountText();
                }
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    DataRow row = myDataTable.NewRow();
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    row["SalesParty"] = txtSalesParty.Text;
                    row["Address"] = strPartyDetail[0];
                    row["BillNo"] = txtBillCode.Text + " " + txtBillNo.Text;
                    row["BillDate"] = "Date : " + txtDate.Text;
                    row["LrNo"] = txtLRNumber.Text;
                    if (txtLRNumber.Text != "")
                        row["LrDate"] = txtLRDate.Text;
                    row["Transport"] = txtTransport.Text;
                    row["Station"] = txtBStation.Text;
                    row["DueDays"] = txtDueDays.Text;
                    row["Marka"] = txtPvtMarka.Text;
                    row["Haste"] = txtSubParty.Text;
                    //  row["DueDate"] = txtDueDate.Text;
                    row["Remark"] = txtRemarks.Text;


                    row["GRSNo"] = dr.Cells["serialno"].Value;
                    row["Party"] = dr.Cells["partyName"].Value;
                    row["Pieces"] = dr.Cells["pcs"].Value;
                    row["Discount"] = dr.Cells["Disc"].Value;
                    row["DisStatus"] = dr.Cells["DiscountStatus"].Value;
                    row["Amount"] = dba.ConvertObjectToDouble(dr.Cells["amount"].Value).ToString("N2", MainPage.indianCurancy);
                    row["Items"] = dr.Cells["item"].Value;
                    row["RPcs"] = dr.Cells["rempcs"].Value;
                    if (Convert.ToString(dr.Cells["remark"].Value) != "")
                        row["Party"] += " (" + dr.Cells["remark"].Value + ")";

                    row["PostOffice"] = strPartyDetail[1] + "  " + strPartyDetail[2];
                    if (strPartyDetail[3] != "")
                        row["PhoneNo"] = strPartyDetail[3] + "  ,  " + strPartyDetail[4];
                    else
                        row["PhoneNo"] = strPartyDetail[4];

                    if (lblNetAddLs.Text.Contains("-"))
                        row["NetDiscount"] = lblNetAddLs.Text;
                    else
                        row["NetDiscount"] = "+" + lblNetAddLs.Text;

                    if (dNetOtherAmt > 0)
                        row["PackPostage"] = "+" + (dNetOtherAmt.ToString("N2", MainPage.indianCurancy));
                    else
                        row["PackPostage"] = dNetOtherAmt.ToString("N2", MainPage.indianCurancy);
                    row["TotalPcs"] = lblTotalPcs.Text;

                    if ((dOtherAmt + dOtherPerAmt) > 0)
                    {
                        row["OtherText"] = strOtherText;
                        row["OtherAmount"] = dOtherAmt + dOtherPerAmt;
                    }
                    row["Packing"] = txtPacking.Text;
                    row["Postage"] = txtPostage.Text;
                    row["GrossAmount"] = lblGrossAmt.Text;
                    row["NetAmount"] = lblNetAmt.Text;
                    row["AmountInWord"] = strNumeric;
                    row["UserName"] = MainPage.strLoginName + " ,  Date : " + MainPage.strCurrentDate + " " + DateTime.Now.ToString("hh:mm tt");

                    myDataTable.Rows.Add(row);

                    dGPacking = dba.ConvertObjectToDouble(dr.Cells["Packing"].Value);
                    dGFreight = dba.ConvertObjectToDouble(dr.Cells["Freight"].Value);
                    dGTax = dba.ConvertObjectToDouble(dr.Cells["Tax"].Value);
                    if (dGPacking > 0)
                        SetOtherDataWithPreviousRow("PACKING", MainPage.dPackingDhara, dGPacking, ref myDataTable);
                    if (dGFreight > 0)
                        SetOtherDataWithPreviousRow("FREIGHT", MainPage.dFreightDhara, dGFreight, ref myDataTable);
                    if (dGTax > 0)
                        SetOtherDataWithPreviousRow("TAX", MainPage.dTaxDhara, dGTax, ref myDataTable);
                }
            }
            catch
            {
            }
            return myDataTable;
        }

        private void SetOtherDataWithPreviousRow(string strParty, double dPer, double dAmt, ref DataTable dt)
        {
            DataRow oldRow = dt.Rows[dt.Rows.Count - 1];
            DataRow row = dt.NewRow();
            for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
            {
                row[colIndex] = oldRow[colIndex];
            }
            row["GRSNo"] = "    ''";
            row["Party"] = strParty;
            row["Discount"] = dPer;
            row["DisStatus"] = "+";
            row["Amount"] = dAmt.ToString("N2", MainPage.indianCurancy);
            row["Items"] = row["Pieces"] = "";
            dt.Rows.Add(row);
        }

        private void AskForPrint(string _strFilePath)
        {
            try
            {
                if (txtTaxLedger.Text != "")
                {
                    GSTPrintAndPreview(true, "", false, true);
                }
                else
                {

                    string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill", strFileName = strPath + "\\" + txtBillNo.Text + ".pdf";
                    if (!Directory.Exists(strPath))
                        Directory.CreateDirectory(strPath);
                    FileInfo fileInfo = new FileInfo(strFileName);
                    try
                    {
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch
                    {
                    }

                    Reporting.SalesReport report = new Reporting.SalesReport();
                    DataTable dt = CreateDataTable();
                    report.SetDataSource(dt);

                    report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                    DialogResult result = MessageBox.Show("ARE YOU WANT TO PRINT SALE BILL ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                            if (strValue != "" && strValue != "0")
                            {
                                int nCopy = Int32.Parse(strValue);
                                report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                                report.PrintToPrinter(nCopy, false, 0, 1);
                            }
                        }
                        catch
                        {
                        }
                    }
                    report.Close();
                    report.Dispose();
                }
            }
            catch
            {
            }
        }

        private void chkPAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgrdPending.Rows)
                row.Cells["chkItem"].Value = chkPAll.Checked;
        }

        private string SetSignatureInBill(bool _bPStatus, bool _createPDF, bool _dscVerified)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string strFileName = "", strPath = "";
            try
            {
                string strNewPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill";
                strFileName = strNewPath + "\\" + txtBillNo.Text + ".pdf";
                if (File.Exists(strFileName))
                    File.Delete(strFileName);
                Directory.CreateDirectory(strNewPath);

                if (_createPDF)
                {
                    SaveFileDialog _browser = new SaveFileDialog();
                    _browser.Filter = "PDF Files (*.pdf)|*.pdf;";
                    _browser.FileName = txtBillNo.Text + ".pdf";
                    _browser.ShowDialog();

                    if (_browser.FileName != "")
                        strPath = _browser.FileName;
                }
                else
                {
                    string _strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\SalesBill\\" + txtBillNo.Text, _strFileName = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                    _strPath += "\\" + _strFileName;

                    strPath = _strPath + "\\" + txtBillNo.Text + ".pdf";
                    if (File.Exists(strPath))
                        File.Delete(strPath);
                    Directory.CreateDirectory(_strPath);
                }

                if (strPath != "")
                {
                    bool _bstatus = GSTPrintAndPreview(false, strFileName, true, _dscVerified);
                    if (_bstatus)
                    {
                        if (!_dscVerified && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";

                            PDFSigner _objSigner = new PDFSigner();
                            bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                            if (!_bFileStatus)
                                strPath = "";
                            if (_bPStatus && _bFileStatus)
                                System.Diagnostics.Process.Start(strPath);
                        }
                        else
                        {
                            File.Copy(strFileName, strPath);
                            if (_bPStatus)
                                System.Diagnostics.Process.Start(strPath);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                strPath = "";
                MessageBox.Show("Error " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return strPath;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (txtTaxLedger.Text != "")
                    {
                        //                       if (MainPage.strUserRole.Contains("ADMIN") || MainPage.strLoginName.Contains("BHARTI"))
                        AskForPrint("");
                        //                        else
                        //                           SetSignatureInBill(true, false, false);
                    }
                    else
                    {
                        Reporting.SalesReport objReport = new SSS.Reporting.SalesReport();
                        DataTable dt = CreateDataTable();
                        if (dt.Rows.Count > 0)
                        {
                            objReport.SetDataSource(dt);
                            objReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            objReport.PrintToPrinter(1, false, 0, 1);
                            objReport.Close();
                            objReport.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (txtTaxLedger.Text != "")
                    {
                        GSTPrintAndPreview(false, "", false, true);
                    }
                    else
                    {
                        Reporting.SalesReport objReport = new SSS.Reporting.SalesReport();
                        DataTable dt = CreateDataTable();
                        if (dt.Rows.Count > 0)
                        {
                            objReport.SetDataSource(dt);

                            Reporting.ShowReport objShow = new Reporting.ShowReport("SALES REPORT PREVIEW");
                            objShow.myPreview.ReportSource = objReport;
                            objShow.ShowDialog();
                            objReport.Close();
                            objReport.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false, __bPrintStatus = _pstatus,_bDiffState=false;
            if (btnAdd.Text == "&Save")
                __bPrintStatus = false;
            DataTable dt = dba.CreateOnlineSaleBookDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, __bPrintStatus,ref _bDiffState);
            bool status = SelectReportAndPrint(_bIGST, dt, _dtGST, _dtSalesAmt, strPath, __bPrintStatus,_bDiffState);
            return status;
        }
        private bool SelectReportAndPrint(bool _bIGST, DataTable dt, DataTable _dtGST, DataTable _dtSalesAmt, string strPath, bool _pstatus,bool _bDiffState)
        {
            ReportClass rpt = new ReportClass();
            if (dt.Rows.Count > 0)
            {
                if (!_bIGST)
                {
                    if (txtBillCode.Text.Contains("SRT"))
                        rpt = new Reporting.SaleBook_Cloth_Report_CSGST_DSC();
                    else if (txtBillCode.Text.Contains("SSO"))
                        rpt = new Reporting.SaleBook_Cloth_Report_CSGST();
                    else 
                    {
                        if (_bDiffState)
                            rpt = new Reporting.SaleBookReport_CSGST_DSC_Ship_From();
                        else
                            rpt = new Reporting.SaleBookReport_CSGST_DSC();
                    }
                }
                else
                {
                    if (txtBillCode.Text.Contains("SRT"))
                        rpt = new Reporting.SaleBook_Cloth_Report_IGST_DSC();
                    else if (txtBillCode.Text.Contains("SSO"))
                        rpt = new Reporting.SaleBook_Cloth_Report_IGST();
                    else
                    {
                        if (_bDiffState)
                            rpt = new Reporting.SaleBookReport_IGST_DSC_Ship_From();
                        else
                            rpt = new Reporting.SaleBookReport_IGST_DSC();
                    }
                }
            }
            rpt.SetDataSource(dt);
            rpt.Subreports[0].SetDataSource(_dtGST);
            rpt.Subreports[1].SetDataSource(_dtSalesAmt);
            bool status = false;
            if (rpt != null)
            {
                status = PrintMultiCopy(rpt, dt, strPath, _pstatus);
            }
            return status;
        }
        private bool PrintMultiCopy(ReportClass rpt, DataTable dt, string strPath, bool _pstatus)
        {
            rpt.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            if (strPath != "")
            {
                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                rpt.Close();
                rpt.Dispose();
                return true;
            }
            else
            {
                if (_pstatus)
                {
                    SaleBillInvoiceType objInvoice = new SaleBillInvoiceType();
                    objInvoice.ShowDialog();
                    if (objInvoice._originalCopy > 0)
                    {
                        SetSubTitleInDataTable(ref dt, "Original Copy", objInvoice._oLetterHead);
                        rpt.SetDataSource(dt);
                        rpt.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                    }
                    if (objInvoice._transportCopy > 0)
                    {
                        SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                        rpt.SetDataSource(dt);
                        rpt.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                    }
                    if (objInvoice._supplierCopy > 0)
                    {
                        SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                        rpt.SetDataSource(dt);
                        rpt.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                    }
                }
                else
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                    objReport.myPreview.ReportSource = rpt;
                    objReport.myPreview.ShowExportButton = true;
                    objReport.myPreview.ShowPrintButton = false;
                    objReport.ShowDialog();
                }
                rpt.Close();
                rpt.Dispose();
            }
            return false;
        }
        private bool checkCompanyAndSupplierState()
        {
            string SMName = Convert.ToString(dgrdDetails.Rows[0].Cells[3].Value);

            string Query = "DECLARE @CS Varchar(500),@SMS Varchar(500)"
                        + " SELECT @CS = StateName FROM CompanyDetails"
                        + " SELECT @SMS = [State] FROM SupplierMaster WHERE AreaCode + AccountNo +' '+ Name = '" + SMName + "'"
                        + " IF(@CS = @SMS) SELECT 1 as status else SELECT 0 as status";

            int status = (int)DataBaseAccess.ExecuteMyScalar(Query);
            return (status == 0);
        }

        private void SetSubTitleInDataTable(ref DataTable dt, string strSubTitle, bool _bLetterHead)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (_bLetterHead)
                    row["HeaderImage"] = row["BrandLogo"] = null;
                else
                {
                    row["HeaderImage"] = MainPage._headerImage;
                    row["BrandLogo"] = MainPage._brandLogo;
                }
                row["SubTitle"] = strSubTitle;
            }
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;

                DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //if (MainPage.strLoginName == "A")
                    //    UploadPDFFile();
                    //else
                    //{
                    string strFileName = CreatePDFFile();
                    if (strFileName != "")
                        MessageBox.Show("Thank you ! PDF generated on " + strFileName, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //}
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private string CreatePDFFile()
        {
            string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill", strFileName = strPath + "\\" + txtBillNo.Text + ".pdf";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);


            if (txtTaxLedger.Text != "")
            {
                strFileName = SetSignatureInBill(false, true, false);
            }
            else
            {
                FileInfo fileInfo = new FileInfo(strFileName);
                try
                {
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                catch
                {
                }

                Reporting.SalesReport report = new Reporting.SalesReport();
                DataTable dt = CreateDataTable();
                report.SetDataSource(dt);
                report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strFileName);

                report.Close();
                report.Dispose();
            }
            return strFileName;
        }

        private void txtBillNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
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

        private void SaleBook_Load(object sender, EventArgs e)
        {
            try
            {
                EditOption();
                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtSalesParty.Text = _strPSalesParty;
                    txtSubParty.Text = _strPSubParty;

                    if (_strPackingType == "DIRECT")
                        rdoDirect.Checked = true;
                    else if (_strPackingType == "PACKED")
                        rdoPacked.Checked = true;
                    else
                        rdoCameOffice.Checked = true;

                    GetPendingGRRecord();
                    AddGoodsReceiveRecordInGrid("'" + strNewAddedGRSNO + "'", "");

                    if (dgrdDetails.Rows.Count == 0)
                        txtSalesParty.Enabled = txtSubParty.Enabled = rdoDirect.Enabled = rdoPacked.Enabled = grpPacking.Enabled = rdoCameOffice.Enabled = true;
                    else
                        txtSalesParty.Enabled = txtSubParty.Enabled = rdoDirect.Enabled = rdoPacked.Enabled = rdoCameOffice.Enabled = false;
                    txtTransport.Focus();
                    btnEdit.Enabled = btnDelete.Enabled = false;
                }

            }
            catch
            {
            }
        }

        private bool ValidateAmountLimit(double dNetAmt)
        {
            object objLimit = DataBaseAccess.ExecuteMyScalar("Select AmountLimit from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dba.ConvertObjectToDouble(objLimit) > 0)
            {
                string strQuery = "";
                if (btnEdit.Text == "&Update")
                {
                    if (strOldPartyName == txtSalesParty.Text)
                        dNetAmt -= dOldNetAmt;
                    //strQuery = " +(Select ISNULL(SUM(CAST(Amount as Money)),0) Amt from BalanceAmount Where AccountStatus='SALES A/C' and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "'  ) ";
                }
                double dAmt = dba.CheckAmountLimitValidation(txtSalesParty.Text, strQuery);
                if (dAmt != -1)
                {
                    dAmt -= dNetAmt;
                    if (dAmt < 0 && MainPage.mymainObject.bCreditLimitmanagement)
                    {
                        MessageBox.Show("Sorry ! Amount limit has been exceeded, Please extend amount limit of : " + Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
                else if (MainPage.strUserRole.Contains("SUPERADMIN"))
                    return true;
                else
                {
                    MessageBox.Show("Unable to check balance amount from internet, please connect internet !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
                return false;
            return true;
        }

        private void txtRemarks_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
            //else
            //    e.Handled = true;
        }

        private void txtDate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                        txtSalesParty.Focus();
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
                txtTaxLedger.Enabled = txtTaxAmt.Enabled = MainPage._bTaxStatus;

                if (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView)
                {
                    if (!MainPage.mymainObject.bSaleAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = btnWayBillNo.Enabled = false;
                    if (!MainPage.mymainObject.bSaleView)
                        txtBillNo.Focus();
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

        private void txtTaxLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALESTYPE", "SEARCH SALES TYPE", e.KeyCode);
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

        private double GetTaxAmount(double dOtherAmt, double _dFinalAmt, ref string strTaxType, ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dServiceAmt = 0;
            try
            {
                if (MainPage._bTaxStatus && txtTaxLedger.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = dba.GetSaleTypeDetails(txtTaxLedger.Text, "SALES");
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string strTaxationType = Convert.ToString(row["TaxationType"]), _strTaxType = "EXCLUDED", strGRSNO = "";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                                _strTaxType = "INCLUDED";
                            strTaxType = _strTaxType;

                            string strQuery = "", strSubQuery = "", strGRSNo = "", strSSQuery = "", strFQuery = "", strTaxQuery = "";
                            double dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);

                            if (dOtherAmt != 0 && MainPage.startFinDate >= Convert.ToDateTime("04/01/2021"))
                                dTaxPer = 18;
                            strFQuery = "Declare @TaxRate float; ";
                            // strQuery += " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) Amount,(TaxRate) TaxRate from (Select HSNCode,SUM(Amount) Amount,SUM(Quantity) Qty,TaxRate from (  ";
                            strQuery += " Select SUM(Amount)TaxableAmt,SUM(TaxAmt) Amt,SUM(RTaxAmt)TaxAmt,(TaxRate) TaxRate from (Select HSNCode,SUM(Amount) Amount,ROUND(SUM((Amount*TaxRate)/100.00),4)TaxAmt,ROUND(CAST(((SUM(Amount)*TaxRate)/100.00) as money),2)RTaxAmt,SUM(Quantity) Qty,TaxRate from ( ";

                            double dDisStatus = 0;

                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                strGRSNo = Convert.ToString(rows.Cells["serialNo"].Value);
                                dDisStatus = dba.ConvertObjectToDouble(rows.Cells["DiscountStatus"].Value + "" + rows.Cells["Disc"].Value);

                                strGRSNO += "'" + rows.Cells["serialNo"].Value + "'";

                                strSubQuery += "Select (GM.Other+ ' : '+GM.HSNCode) as HSNCode,GRD.Quantity,ROUND(((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100.00)/(100.00+TaxRate)) else Amount end))*(100.00+" + dDisStatus + "))/100.00),4)Amount,GM.TaxRate from GoodsReceiveDetails GRD Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where GRD.ItemName=_IM.ItemName ) as GM Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.Amount>0  Union All "
                                            + " Select '' as HSNCode,0 as Quantity,ROUND((((((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((Amount * 100.00) / (100.00 + GM.TaxRate)) else Amount end))*(100 + " + dDisStatus + ") / 100.00))*GM.TaxRate)/ 100.00)*CS.TaxDhara)/ 100.00)),4) Amount,@TaxRate asTaxRate from GoodsReceiveDetails GRD OUTER APPLY(Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100.00)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where GRD.ItemName=_IM.ItemName ) as GM Where(GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and Amount > 0  Union All "
                                            + " Select '' as HSNCode,0 as Quantity,ROUND(((((GRD.PackingAmt + GRD.FreightAmt)* (CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then ((100) / (100 + @TaxRate)) else 1 end)) + ((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara) / 100.00)),4) Amount,@TaxRate as TaxRate from GoodsReceiveDetails GRD Outer Apply(Select TOP 1 FreightDhara from CompanySetting) CS Where(GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and(GRD.PackingAmt + GRD.FreightAmt) > 0  Union All "
                                            + " Select '' as HSNCode,0 as Quantity,ROUND((((GRD.TaxAmt) * (CS.TaxDhara) / 100.00) * (CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((100) / (100 + @TaxRate)) else 1 end)),4) Amount,@TaxRate as TaxRate from GoodsReceiveDetails GRD Outer Apply(Select TOP 1 TaxDhara from CompanySetting) CS Where(GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and(GRD.TaxAmt) > 0  Union All ";

                                if (strSSQuery != "")
                                {
                                    strSSQuery += " Union ALL ";
                                    strTaxQuery += " Union ALL ";
                                }

                                strSSQuery += " Select (((((Amount*(100+" + dDisStatus + ")/100.00)*TaxRate/100.00)*TaxDhara)/100.00)) Amount,0 PackingAmt from (Select SUM(CASE WHEN '" + _strTaxType + "'='INCLUDED' then ROUND(((GRD.Amount*100.00)/(100.00+GM.TaxRate)),4) else GRD.Amount end) Amount,GRD.ItemName,GM.TaxRate,SUM(GRD.Quantity) Quantity from GoodsReceiveDetails GRD Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where GRD.ItemName=_IM.ItemName ) as GM  Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.Amount>0 Group by GRD.ItemName,GRD.Amount,GRD.Quantity,GRD.MTR,GM.TaxRate)_Sales  OUTER APPLY (Select TOP 1 TaxDhara from CompanySetting) CS " //Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/Quantity)>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/Quantity)<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _Sales.ItemName=_IM.ItemName ) as GM  
                                           + " Union ALL Select 0 Amount , (CASE WHEN '" + _strTaxType + "'='INCLUDED' then ROUND(((((GRD.PackingAmt + GRD.FreightAmt) * CS.FreightDhara) / 100.00)),4) else 0 end) PackingAmt from GoodsReceiveDetails GRD Outer Apply(Select TOP 1 FreightDhara from CompanySetting) CS  Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and (GRD.PackingAmt+GRD.FreightAmt)>0  ";
                                strTaxQuery += " Select GM.TaxRate from GoodsReceiveDetails GRD Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Rate*100)/(100+TaxRate)) else GRD.Rate end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end)))<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where GRD.ItemName=_IM.ItemName ) as GM Where (GRD.ReceiptCode+' '+ CAST(GRD.ReceiptNo as nvarchar)) in ('" + strGRSNo + "') ";
                            }

                            if (dTaxPer == 18)
                                strFQuery += " SET @TaxRate=" + dTaxPer;
                            else
                                strFQuery += " Select  @TaxRate=ISNULL(MAX(TaxRate),0) from (" + strTaxQuery + ")_Tax ";

                            strSubQuery += "  SELECT TOP 1 '' as HSNCode,0 as Quantity," + dOtherAmt + " as Amount,@TaxRate as TaxRate)_Sales  Group by HSNCode,TaxRate )_Sales Where Amount!=0 and TaxRate>0 Group by TaxRate ";
                            strQuery += strSubQuery;


                            strSSQuery = " Select (SUM(Amount)+(CASE When '" + _strTaxType + "'='INCLUDED' then ROUND((((" + dOtherAmt + "+SUM(Amount+PackingAmt))*@TaxRate)/100.00),2) else 0 end)) Amt  from ( " + strSSQuery + ") _Sales ";

                            DataSet _ds = DataBaseAccess.GetDataSetRecord(strFQuery + strQuery + strSSQuery);
                            if (_ds.Tables.Count > 0)
                            {
                                dServiceAmt = DataBaseAccess.ConvertObjectToDoubleStatic(_ds.Tables[1].Rows[0][0]);
                                double dMaxRate = 0, dTTaxAmt = 0;
                                // BindTaxDetails(_ds.Tables[0], row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                dba.BindTaxDetails(dgrdTax, _ds.Tables[0], row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);

                                dTaxAmt = dTTaxAmt;
                                dTaxPer = dMaxRate;
                            }

                            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                            txtServiceAmt.Text = dServiceAmt.ToString("N2", MainPage.indianCurancy);
                            txtTaxPer.Text = dTaxPer.ToString("0.00");

                            if (_strTaxType == "INCLUDED")
                                dTaxAmt = 0;
                        }
                        else if (strTaxationType == "VOUCHERWISE" || strTaxationType == "REVERSECHARGE")
                        {
                            double _dTaxPer = dba.ConvertObjectToDouble(row["TaxRate"]);
                            if (_dTaxPer > 0)
                            {
                                dTaxAmt = ((_dFinalAmt + dOtherAmt) * _dTaxPer) / 100;
                            }

                            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                            txtTaxPer.Text = _dTaxPer.ToString("0.00");
                            dTaxableAmt = (_dFinalAmt + dOtherAmt);
                            txtServiceAmt.Text = "0.00";
                        }
                        else
                            txtTaxAmt.Text = txtServiceAmt.Text = txtTaxPer.Text = "0.00";
                    }
                    else
                        txtTaxAmt.Text = txtServiceAmt.Text = txtTaxPer.Text = "0.00";
                }
                else
                    txtTaxAmt.Text = txtServiceAmt.Text = txtTaxPer.Text = "0.00";

                if (!newStatus)
                    btnAdd.Enabled = btnEdit.Enabled = true;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                {
                    if (!MainPage.mymainObject.bSaleAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bSaleEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                txtTaxAmt.Text = txtServiceAmt.Text = txtTaxPer.Text = "0.00";
                MessageBox.Show("Sorry ! " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAdd.Enabled = btnEdit.Enabled = false;
            }
            return dTaxAmt;
        }


        private void txtGreenTax_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtGreenTax.Text == "")
                        txtGreenTax.Text = "0";
                    CalculateAllAmount();
                }
            }
            catch
            {
            }
        }

        private void dgrdPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                ShowGoodsReceivePage();
        }

        private void ShowGoodsReceivePage()
        {
            try
            {
                string strAllGRSNo = Convert.ToString(dgrdPending.CurrentRow.Cells["pGRSNo"].Value);

                string[] strGRSNo = strAllGRSNo.Split(' ');
                if (strGRSNo.Length > 1)
                {
                    if (strGRSNo[0] != "" && strGRSNo[1] != "")
                    {
                        bool _bPStatus = Convert.ToBoolean(dgrdPending.CurrentRow.Cells["pStatus"].Value);
                        if (_bPStatus)
                        {

                            GoodscumPurchase objGoodsReciept = new GoodscumPurchase(strGRSNo[0], strGRSNo[1]);
                            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objGoodsReciept.Show();
                        }
                        else
                        {
                            GoodsReceipt objGoodsReciept = new GoodsReceipt(strGRSNo[0], strGRSNo[1]);
                            objGoodsReciept.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objGoodsReciept.Show();
                        }
                    }
                }
            }
            catch { }
        }

        private void txtTaxPer_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtTaxPer.Text == "")
                        txtGreenTax.Text = "0";
                    CalculateAllAmount();
                }
            }
            catch
            {
            }
        }

        private void SaleBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (btnClose.Enabled)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            e.Cancel = true;
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to close please update sale bill first. ", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }
            }
            catch { }
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                if (txtSalesParty.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillCode.Text != "" && txtBillNo.Text != "")
                    {
                        if (rdoDirect.Checked)
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to send email & whatsapp to supplier ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strBillNos = "'" + txtBillCode.Text + txtBillNo.Text + "'";
                                int _count = dba.SendEmailIDAndWhatsappNumberToSupplier(strBillNos);
                                if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                                else
                                    MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                result = MessageBox.Show("Are you sure want to send email to customer ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    string strMobileNo = "", strPath = "";
                                    SendEmailToSalesParty(true, ref strMobileNo, ref strPath);
                                }
                            }
                        }
                        else
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to send email to customer ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strMobileNo = "", strPath = "";
                                SendEmailToSalesParty(true, ref strMobileNo, ref strPath);
                            }
                        }
                    }
                }
            }
            catch { }
            btnSendEmail.Enabled = true;
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtReason.Text != "")
                {
                    bool iStatus = true;
                    if ((ValidationForDeletion(ref iStatus) && dba.ValidateBackDateEntry(txtDate.Text)) || MainPage.strUserRole == "SUPERADMIN")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strFileName = SetSignatureInBill(false, false, true);

                            string strQuery = "";
                            if (strAllAttachedBillNo != "")
                                strQuery += " Update [dbo].[SalesRecord] Set [PackedBillNo]=''  Where [BillCode]='" + txtBillCode.Text + "' and [BillNo] in (" + strAllAttachedBillNo + ")  ";

                            strQuery += " Update GoodsReceive set SaleBill='PENDING',UpdateStatus=1 where (ReceiptCode+' '+CAST(ReceiptNo as varchar)) in (Select GRSNo from SalesEntry Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ") "
                                          + " Update PurchaseRecord set SaleBillNo='' where GRSNo in (Select GRSNo from SalesEntry Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + ") "
                                          + " Delete from BalanceAmount Where AccountStatus in ('SALES A/C','DUTIES & TAXES') and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                                          + " Delete from BiltyDetail  Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                          + " Delete from SalesEntry Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                          + " Delete from SalesRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                          + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                          + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                          + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", with Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (iStatus)
                                    strQuery = " Delete from BalanceAmount Where AccountStatus in ('SALES A/C','DUTIES & TAXES') and Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' ";

                                DataBaseAccess.CreateDeleteQuery(strQuery);

                                txtReason.Text = "";
                                SendEmailForDeletion(strFileName);
                                txtBillNo.ReadOnly = pnlDeletionConfirmation.Visible = false;
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
            catch { }
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
                        SearchData objSearch = new SearchData("SALECODE", "SEARCH SALE BILL CODE", e.KeyCode);
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

        private void lblCreatedBy_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALES", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtBillNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnEdit.Text == "&Update")
                e.Handled = true;
            else
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void rdoCameOffice_CheckedChanged(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && rdoCameOffice.Checked)
            {
                GetPendingGRRecord();
                SetGreenTaxAmt();
            }
            grpPacking.Enabled = !rdoDirect.Checked;
        }

        private void txtPostage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtPostage_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtPostage.Text == "")
                        txtPostage.Text = "0";
                    CalculateAllAmount();
                }
            }
            catch
            {
            }
        }

        private void btnBillCancel_Click(object sender, EventArgs e)
        {
            pnlPendingStock.Visible = false;
        }

        private void dgrdItem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                {
                    string strBillNo = Convert.ToString(dgrdPendingStock.CurrentCell.Value), strBillType = Convert.ToString(dgrdPendingStock.CurrentRow.Cells["billType"].Value);
                    string[] strNumber = strBillNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1], strBillType);
                    }
                }
            }
            catch { }
        }

        private void chkPSAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdPendingStock.Rows)
                {
                    row.Cells["chkPBill"].Value = chkPSAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdPendingStock_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                e.Cancel = true;
            }
        }

        private void dgrdPendingStock_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dgrdPendingStock.CurrentCell = dgrdPendingStock.CurrentRow.Cells[dgrdPendingStock.CurrentCell.ColumnIndex + 1];
                    AddSelectedStockBill();
                }
            }
            catch
            {
            }
        }

        private void AddSelectedStockBill()
        {
            try
            {
                string strSBillNo = "", strBill = "";
                foreach (DataGridViewRow row in dgrdPendingStock.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkPBill"].Value))
                    {
                        strBill = Convert.ToString(row.Cells["pendingBillNo"].Value);
                        string[] str = strBill.Split(' ');
                        if (str.Length > 1)
                        {
                            if (strSBillNo != "")
                                strSBillNo += ",";
                            strSBillNo += str[1];
                            row.Cells["chkPBill"].Value = false;
                        }
                    }
                }
                txtAttachedBill.Text = strSBillNo;
                txtAttachedBill.Focus();
                pnlPendingStock.Visible = false;
            }
            catch
            {
            }
        }

        private void BindPendingStock()
        {
            try
            {
                dgrdPendingStock.Rows.Clear();
                string strSaleParty = "", strSubParty = "", strQuery = "";
                string[] strFullName = txtSalesParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strSaleParty = strFullName[0].Trim();
                strFullName = txtSubParty.Text.Split(' ');
                if (strFullName.Length > 0)
                    strSubParty = strFullName[0].Trim();

                string strPackingStatus = GetPackingStatus();

                if (strSaleParty != "" && strSubParty != "")
                {
                    strQuery += " Select * from ( "
                             + " Select (BillCode+' '+ CAST(BillNo as varchar)) BillNo,CONVERT(varchar,BillDate,103) Date,'NORMAL' as BillType,BillNo as _BillNo from SalesRecord SR Where BillStatus in ('BILLED','STOCK') and ISNULL(PackedBillNo,'')='' and ISNULL(AttachedBill,'')='' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' and SR.BillNo!=" + txtBillNo.Text + " and [GoodsType]='" + strPackingStatus + "' UNION ALL "
                             + " Select (BillCode+' '+ CAST(BillNo as varchar)) BillNo,CONVERT(varchar,Date,103) Date,'RETAIL' BillType,BillNo as _BillNo from SalesBook Where ISNULL(PackedBillNo,'')='' and ISNULL(AttachedBill,'')='' and SalePartyID='" + strSaleParty + "' and SubPartyID='" + strSubParty + "' )_Sale Order by _BillNo desc ";
                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        int rowIndex = 0;
                        dgrdPendingStock.Rows.Add(dt.Rows.Count);
                        chkPSAll.Checked = false;

                        foreach (DataRow row in dt.Rows)
                        {

                            dgrdPendingStock.Rows[rowIndex].Cells["chkPBill"].Value = false;
                            dgrdPendingStock.Rows[rowIndex].Cells["pendingBillNo"].Value = row["BillNo"];
                            dgrdPendingStock.Rows[rowIndex].Cells["pDate"].Value = row["Date"];
                            dgrdPendingStock.Rows[rowIndex].Cells["billType"].Value = row["BillType"];
                            rowIndex++;
                        }
                    }
                }
            }
            catch { }
        }

        private void txtAttachedBill_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                {
                    if (!pnlPendingStock.Visible)
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            pnlPendingStock.Visible = true;
                            BindPendingStock();
                            SetAttachedBillToGrid();

                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void BindAttachedBillShowBill()
        {
            dgrdShowSaleBill.Rows.Clear();
            string[] strAllBIll = txtAttachedBill.Text.Split(',');
            if (strAllBIll.Length > 0)
            {
                dgrdShowSaleBill.Rows.Add(strAllBIll.Length);
                int _index = 0;
                foreach (string str in strAllBIll)
                {
                    dgrdShowSaleBill.Rows[_index].Cells["sSaleBillNo"].Value = txtBillCode.Text + " " + str;
                    _index++;
                }
            }
        }

        private void SetAttachedBillToGrid()
        {
            try
            {
                string strAttachBill = txtAttachedBill.Text;
                if (strAttachBill != "")
                {
                    string[] strAllItem = strAttachBill.Split(',');
                    foreach (string strID in strAllItem)
                    {
                        string strBillNo = strID.Trim();
                        if (strBillNo != "")
                        {
                            DataRow[] fileterrow = _dtPendingStock.Select(String.Format("BillNo Like ('" + strBillNo + "') "));
                            if (fileterrow.Length > 0 && dgrdPendingStock.Rows.Count > 0)
                            {
                                int index = _dtPendingStock.Rows.IndexOf(fileterrow[0]);
                                dgrdPendingStock.Rows[index].Cells["chkPBill"].Value = true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void txtBillStatus_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("SALEBILLSTATUS", "SEARCH BILL STATUS", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtBillStatus.Text = objSearch.strSelectedData;
                            lblMsg.Text = "";
                            if (txtBillStatus.Text == "STOCK")
                            {
                                txtPacking.Text = txtPostage.Text = "0.00";
                                CalculateAllAmount();
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

        private void txtPackedBillNo_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPackedBillNo.Text != "")
                {
                    string[] strNumber = txtPackedBillNo.Text.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                }
            }
            catch { }
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            pnlShowSaleBill.Visible = false;
        }

        private void dgrdShowSaleBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    string strBillNo = Convert.ToString(dgrdShowSaleBill.CurrentCell.Value);
                    string[] strNumber = strBillNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ShowSaleBook(strNumber[0], strNumber[1]);
                    }
                }
            }
            catch { }
        }

        private void ShowSaleBook(string strBillCode, string strBillNo)
        {
            SaleBook objSale = new SaleBook(strBillCode, strBillNo);
            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            objSale.ShowInTaskbar = true;
            objSale.TopLevel = true;
            objSale.Show();
        }

        private void ShowSaleBook(string strBillCode, string strBillNo, string strBillType)
        {
            if (strBillType == "RETAIL")
            {
                SaleBook_Trading objSale = new SaleBook_Trading(strBillCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.TopLevel = true;
                objSale.Show();
            }
            else
            {
                SaleBook objSale = new SaleBook(strBillCode, strBillNo);
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.ShowInTaskbar = true;
                objSale.TopLevel = true;
                objSale.Show();
            }
        }

        private void txtAttachedBill_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (txtAttachedBill.Text != "")
                {
                    if (!txtAttachedBill.Text.Contains(","))
                    {
                        ShowSaleBook(txtBillCode.Text, txtAttachedBill.Text);
                    }
                    else
                    {
                        BindAttachedBillShowBill();
                        if (dgrdShowSaleBill.Rows.Count > 0)
                            pnlShowSaleBill.Visible = true;
                        else
                            pnlShowSaleBill.Visible = false;
                    }
                }
            }
            catch { }
        }

        private void txtTransport_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenTransportMaster(txtTransport.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
        }

        private void txtPackerName_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPackerName.Text);
        }

        private void btnWayBillNo_Click(object sender, EventArgs e)
        {
            btnWayBillNo.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "" && !txtTransport.Text.Contains("BY HAND"))
                {
                    if (txtTransport.Text != "")
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                        {
                            DialogResult result = MessageBox.Show("Are you sure you want generate JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";

                                var _success = dba.GenerateEWayBillJSON(strBillNo);
                                if (_success)
                                {
                                    DialogResult _result = MessageBox.Show("Are you want to open eway bill site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (_result == DialogResult.Yes)
                                        System.Diagnostics.Process.Start("https://ewaybillgst.gov.in/BillGeneration/BulkUploadEwayBill.aspx");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Transport Name can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTransport.Focus();
                    }
                }
            }
            catch { }
            btnWayBillNo.Enabled = true;
        }

        private void btnPrintWayBill_Click(object sender, EventArgs e)
        {
            btnPrintWayBill.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        if (txtWayBillNo.Text != "" && txtWayBIllDate.Text != "")
                        {
                            if (txtWayBIllDate.Text.Length == 19)
                            {
                                DataTable _dt = dba.CreateWayBillDataTable(txtBillCode.Text, txtBillNo.Text);
                                if (_dt.Rows.Count > 0)
                                {
                                    Reporting.WayBillReport objReport = new Reporting.WayBillReport();
                                    objReport.SetDataSource(_dt);

                                    Reporting.ShowReport objShow = new Reporting.ShowReport("WAY BILL PREVIEW");
                                    objShow.myPreview.ReportSource = objReport;
                                    objShow.myPreview.ShowPrintButton = true;
                                    objShow.myPreview.ShowExportButton = true;
                                    objShow.ShowDialog();

                                    objReport.Close();
                                    objReport.Dispose();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Please enter valid way bill date (dd/MM/yyyy hh:mm tt).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtWayBIllDate.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Way bill no. and Way bill date can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtWayBillNo.Focus();
                        }
                    }
                }
            }
            catch { }
            btnPrintWayBill.Enabled = true;
        }

        private void txtLRNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                {
                    if (e.KeyChar != Convert.ToChar(13))
                    {
                        if (txtBillStatus.Text == "BILLED")
                        {
                            lblMsg.Text = "Oops !! Please change bill status billed to shipped.";
                            lblMsg.ForeColor = Color.Red;
                            e.Handled = true;
                        }
                        else
                        {
                            lblMsg.Text = "";
                            dba.ValidateSpace(sender, e);
                        }
                    }
                }
            }
            catch { }
        }

        private void txtWayBIllDate_Leave(object sender, EventArgs e)
        {
            if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
            {
                if (txtWayBIllDate.Text != "")
                {
                    if (txtWayBIllDate.Text.Length != 19)
                    {
                        MessageBox.Show("Sorry ! Please enter valid way bill date (dd/MM/yyyy hh:mm tt).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtWayBIllDate.Focus();
                    }
                }
            }
        }

        private void txtNoofCases_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                SetGreenTaxAmt();
        }

        private void txtOtherAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (!txtOtherAmt.Text.Contains("."))
                    {
                        if (txtOtherAmt.Text.Length > 3)
                            e.Handled = true;
                    }
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
        }

        private void txtTaxAmt_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int colIndex = dgrdDetails.CurrentCell.ColumnIndex, rowIndex = dgrdDetails.CurrentRow.Index;
                if (colIndex >= 0 && rowIndex >= 0)
                {
                    if (e.KeyCode == Keys.Space)
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
            }
            catch { }
        }

        private void btnEInvoice_Click(object sender, EventArgs e)
        {
            btnEInvoice.Enabled = false;
            try
            {
                GenerateEInvoice(false);
            }
            catch { }
            btnEInvoice.Enabled = true;
        }

        private void btnEinvoiceEWayBIll_Click(object sender, EventArgs e)
        {
            btnEinvoiceEWayBIll.Enabled = false;
            try
            {
                GenerateEInvoice(true);
            }
            catch { }
            btnEinvoiceEWayBIll.Enabled = true;
        }

        private void GenerateEInvoice(bool _bStatus)
        {
            if (txtBillCode.Text != "" && txtBillNo.Text != "")
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want E-Invoice JSON ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        object obj = DataBaseAccess.ExecuteMyScalar("Select GSTNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtSalesParty.Text + "' and GSTNo!=''");
                        if (Convert.ToString(obj) != "")
                        {
                            string strBillNo = "'" + txtBillCode.Text + " " + txtBillNo.Text + "'";
                            var _success = dba.GenerateEInvoiceJSON_SaleBook(_bStatus, strBillNo);
                            if (_success)
                            {
                                DialogResult _result = MessageBox.Show("Are you want to open e-invoice site ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (_result == DialogResult.Yes)
                                    System.Diagnostics.Process.Start("https://einvoice1.gst.gov.in/Invoice/BulkUpload");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! E-Invoice is allowed only for B2B customer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void pnlNOCourier_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnlNOCourier.Width == 55)
                    pnlNOCourier.Size = new Size(110, 110);
                else
                    pnlNOCourier.Size = new Size(55, 55);
            }
            catch { }
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

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.ValidateSpace(sender, e);
        }

        private void txtPetiAgent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PETIAGENT", "SEARCH PETI AGENT", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPetiAgent.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnShowBilty_Click(object sender, EventArgs e)
        {
            try
            {
                btnShowBilty.Enabled = false;

                if (txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    if (txtPackedBillNo.Text != "")
                    {
                        string[] str = txtPackedBillNo.Text.Split(' ');
                        if (str.Length > 1)
                            DataBaseAccess.ShowBiltyPDFFiles(txtBillCode.Text, str[1]);
                        else
                            DataBaseAccess.ShowBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
                    }
                    else
                        DataBaseAccess.ShowBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
                }
            }
            catch
            {
            }
            btnShowBilty.Enabled = true;
        }

        private void txtPetiType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("PETITYPE", "SEARCH PETI TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtPackingType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnForwardingNote_Click(object sender, EventArgs e)
        {
            btnForwardingNote.Enabled = false;
            try
            {
                if (txtBillCode.Text != "" && txtBillNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        DataTable _dt = CreateDataTable_FormwardingNote();
                        if (_dt.Rows.Count > 0)
                        {
                            Reporting.Forwarding_Note objReport = new Reporting.Forwarding_Note();
                            objReport.SetDataSource(_dt);
                            //objReport.PrintToPrinter(1, false, 0, 0);

                            Reporting.ShowReport objShow = new Reporting.ShowReport("FORWARDING NOTE PREVIEW");
                            objShow.myPreview.ReportSource = objReport;
                            objShow.myPreview.ShowPrintButton = true;
                            objShow.myPreview.ShowExportButton = true;
                            objShow.ShowDialog();

                            objReport.Close();
                            objReport.Dispose();
                        }
                    }
                }
            }
            catch { }
            btnForwardingNote.Enabled = true;
        }

        private void btnAttachBill_Click(object sender, EventArgs e)
        {
            AddSelectedStockBill();
        }

        private void btnAttachDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                txtAttachedBill.Clear();
        }

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("SALES", txtBillCode.Text, txtBillNo.Text);
                    objEdit.ShowDialog();
                }
            }
            catch { }
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
        //                dTaxableAmt += dba.ConvertObjectToDouble(row["TaxableAmt"]);
        //                dTaxAmt = dba.ConvertObjectToDouble(row["Amount"]);

        //                dTTaxAmt += Convert.ToDouble(dTaxAmt.ToString("0.00"));
        //                if (dTaxRate > dMaxRate)
        //                    dMaxRate = dTaxRate;

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

        private DataTable CreateDataTable_FormwardingNote()
        {
            DataTable _dt = new DataTable();
            try
            {
                _dt.Columns.Add("HeaderName", typeof(String));
                _dt.Columns.Add("Date", typeof(String));
                _dt.Columns.Add("CompanyName", typeof(String));
                _dt.Columns.Add("CompanyGSTNo", typeof(String));
                _dt.Columns.Add("PartyName", typeof(String));
                _dt.Columns.Add("PartyGSTNo", typeof(String));
                _dt.Columns.Add("TransportName", typeof(String));
                _dt.Columns.Add("BookingStation", typeof(String));
                _dt.Columns.Add("PvtMarka", typeof(String));
                _dt.Columns.Add("CaseNo", typeof(String));
                _dt.Columns.Add("InvoiceValue", typeof(String));
                _dt.Columns.Add("Quantity", typeof(String));
                _dt.Columns.Add("Remark", typeof(String));

                string strQuery = "Select GSTNo,(Select TOP 1 GSTNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtSalesParty.Text + "') _GSTIN,(Select TOP 1 GSTNo from SupplierMaster Where (AreaCode+AccountNo+' '+Name)='" + txtSubParty.Text + "') _SGSTIN from CompanyDetails Where Other='" + MainPage.strCompanyName + "' ";
                DataTable table = dba.GetDataTable(strQuery);
                string strPartyName = txtSalesParty.Text, strGSTNo = "";


                if (table.Rows.Count > 0)
                {
                    DataRow row = _dt.NewRow();
                    DataRow dr = table.Rows[0];

                    if (txtSubParty.Text != "SELF")
                    {
                        strPartyName = txtSubParty.Text;
                        strGSTNo = Convert.ToString(dr["_SGSTIN"]);
                    }

                    if (strGSTNo == "")
                        strGSTNo = Convert.ToString(dr["_GSTIN"]);

                    row["HeaderName"] = "FORWARDING NOTE";
                    row["Date"] = txtDate.Text;
                    row["CompanyName"] = MainPage.strPrintComapanyName;
                    row["CompanyGSTNo"] = dr["GSTNo"];
                    row["PartyName"] = strPartyName;
                    row["PartyGSTNo"] = strGSTNo;
                    row["TransportName"] = txtTransport.Text;
                    row["BookingStation"] = txtBStation.Text;
                    row["PvtMarka"] = txtPvtMarka.Text;
                    row["CaseNo"] = txtBillNo.Text + " X " + txtNoofCases.Text;
                    row["InvoiceValue"] = lblNetAmt.Text;
                    row["Quantity"] = lblTotalPcs.Text;
                    _dt.Rows.Add(row);
                }
            }
            catch { }
            return _dt;
        }

    }
}
