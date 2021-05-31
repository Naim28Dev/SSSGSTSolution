using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class SaleBook_Trading : Form
    {
        DataBaseAccess dba;
        SendSMS objSMS;
        string strLastSerialNo = "", strOldPartyName="", strOldLRNumber="";
        bool qtyAdjustStatus = false;
        double dOldNetAmt = 0, dSalesPartyDiscount = 0;
        public SaleBook_Trading()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            GetStartupData(true);
        }

        public SaleBook_Trading(string strSerialCode, string strSerialNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SSS.SendSMS();
            SetCategory();
            txtBillCode.Text = strSerialCode;
            GetStartupData(false);
            BindRecordWithControl(strSerialNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select SBillCode,(Select ISNULL(MAX(BillNo),0) from SalesBook Where BillCode=SBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    if (txtBillCode.Text == "")
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["SBillCode"]);
                    strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                }
               
                if (strLastSerialNo != "" && strLastSerialNo != "0" && bStatus)
                    BindRecordWithControl(strLastSerialNo);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in GetStartupData in Sale Book", ex.Message };
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'   ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "'  and BillNo>" + txtBillNo.Text + " ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from SalesBook Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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
                chkPick.Checked = false;
                strOldPartyName = "";
                string strQuery = " Select *,(SalePartyID+' '+SName) SParty,ISNULL(dbo.GetFullName(SubPartyID),'SELF') HParty,CONVERT(varchar,Date,103)BDate,CONVERT(varchar,ISNULL(PackingDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)PDate,CONVERT(varchar,ISNULL(LrDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)LDate,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,SB.Date))) LockType,ISNULL(PAgent,'DIRECT') PAgent,NormalDhara,PostageStatus from SalesBook SB OUTER APPLY (Select Top 1 SM.Name as SName,NormalDhara,(CASE WHEN FourthTransport='False' then FourthTransport else 'True' end) as PostageStatus from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.SalePartyID)SM1  OUTER APPLY (Select Top 1 (Description_1+' '+Name)PAgent from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.Description_1)SM  Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "    Select * from SalesBookSecondary SBS Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + " order by SID "
                                + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo; 
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                DisableAllControls();
                txtReason.Text = strOldLRNumber= "";
                pnlDeletionConfirmation.Visible = false;
                txtBillNo.ReadOnly = false;
                lblCreatedBy.Text = "";
                if (ds.Tables.Count > 1)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            DataRow row = dt.Rows[0];
                            txtBillNo.Text = strSerialNo;
                            txtDate.Text = Convert.ToString(row["BDate"]);                      
                            txtSalesParty.Text = strOldPartyName= Convert.ToString(row["SParty"]);
                            txtSubParty.Text = Convert.ToString(row["HParty"]);
                            txtWayBIllDate.Text = Convert.ToString(row["WayBillDate"]);
                            txtWayBillNo.Text = Convert.ToString(row["WaybillNo"]);
                            txtNoofCases.Text = Convert.ToString(row["NoOfCase"]);
                            strOldLRNumber=txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            txtLRDate.Text = Convert.ToString(row["LDate"]);
                            txtTimeOfSupply.Text = Convert.ToString(row["LRTime"]);
                            txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                            txtPackerName.Text = Convert.ToString(row["PackerName"]);
                            txtPackingDate.Text = Convert.ToString(row["PDate"]);
                            txtCartonType.Text = Convert.ToString(row["CartonType"]);
                            txtCartonSize.Text = Convert.ToString(row["CartonSize"]);
                            txtSalesType.Text = Convert.ToString(row["SalesType"]);                          
                            txtRemark.Text = Convert.ToString(row["Remark"]);       
                            txtTransport.Text = Convert.ToString(row["TransportName"]);
                            txtBStation.Text=Convert.ToString(row["Station"]);
                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtOtherAmount.Text = Convert.ToString(row["OtherAmt"]);
                            txtDiscPer.Text = Convert.ToString(row["DisPer"]);
                            txtDiscAmt.Text = Convert.ToString(row["DisAmt"]);
                            txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                            txtTaxAmt.Text = Convert.ToString(row["TaxAmt"]);                           
                            txtROSign.Text = Convert.ToString(row["RoundOffSign"]);
                            txtRoundOff.Text = Convert.ToString(row["RoundOffAmt"]);
                            txtPostage.Text = Convert.ToString(row["PostageAmt"]);
                            txtGreenTax.Text = Convert.ToString(row["GreenTax"]);
                            txtOtherPerSign.Text=Convert.ToString(row["Description"]);
                            txtPetiType.Text = Convert.ToString(row["Description_2"]);
                            txtImportData.Text = Convert.ToString(row["Description_3"]);
                            txtPetiAgent.Text = Convert.ToString(row["PAgent"]);

                            txtSpclDisPer.Text = Convert.ToString(row["SpecialDscPer"]);
                            txtSplDisAmt.Text = Convert.ToString(row["SpecialDscAmt"]);
                            if (dt.Columns.Contains("IRNNO"))
                                txtIRNo.Text = Convert.ToString(row["IRNNO"]);

                            if (dt.Columns.Contains("TaxableAmt"))
                                lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);

                            if (txtROSign.Text == "")
                                txtROSign.Text = "+";
                            if (txtRoundOff.Text == "")
                                txtRoundOff.Text = "0.00";

                            dSalesPartyDiscount=dba.ConvertObjectToDouble(row["NormalDhara"]);
                            dOldNetAmt = Convert.ToDouble(row["NetAmt"]);
                            lblTotalQty.Text = Convert.ToDouble(row["TotalQty"]).ToString("N2", MainPage.indianCurancy);
                            lblGrossAmt.Text = Convert.ToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy);
                            lblNetAmt.Text = dOldNetAmt.ToString("N2", MainPage.indianCurancy);

                            if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                                btnEdit.Enabled = btnDelete.Enabled = false;
                            else
                            {
                                if (!MainPage.mymainObject.bSaleEdit)
                                    btnEdit.Enabled = btnDelete.Enabled = false;
                                else
                                    btnEdit.Enabled = btnDelete.Enabled = true;
                            }

                            if (Convert.ToBoolean(row["PostageStatus"]))
                                pnlNOCourier.Visible = false;
                            else
                                pnlNOCourier.Visible = true;

                            string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                            if (strCreatedBy != "")
                                lblCreatedBy.Text = "Created By : " + strCreatedBy;
                            if (strUpdatedBy != "")
                                lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

                        }
                    }

                    BindSalesBookDetails(ds.Tables[1]);
                    BindGSTDetailsWithControl(ds.Tables[2]);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void BindSalesBookDetails(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            int rowIndex = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                foreach (DataRow row in dt.Rows)
                {
                    dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["SID"];
                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];
                    dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = row["SONumber"]; 
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
                    dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
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
                //pnlTax.Visible = true;
            }
            else
                pnlTax.Visible = false;
        }

        private bool CheckSONumberInDetails()
        {
            if (dgrdDetails.Rows.Count > 0)
            {

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["soNumber"].Value) != "")
                    {
                        MessageBox.Show("Sorry ! You can't update party name because few sales order added", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (MainPage.strUserRole.Contains("ADMIN"))
                            return true;
                        else
                            return false;
                    }
                }
            }
            return true;
        }

        private void txtSalesParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        if (CheckSONumberInDetails())
                        {
                            SearchData objSearch = new SearchData("SALESPARTY", "SEARCH SUNDRY DEBTORS NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                txtSalesParty.Text = objSearch.strSelectedData;
                                string strData = objSearch.strSelectedData;
                                if (strData != "")
                                {
                                    txtSalesParty.Text = strData;
                                    txtSubParty.Text = "SELF";
                                    GetSalesPartyRecord();
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
                string strQuery = "";
               
                if (strSubParty == "SELF")
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";
                else
                    strQuery += " Select Transport,PvtMarka,Station,BookingStation from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))='" + strSubParty + "' ";

                strQuery += " Select TransactionLock,GroupII,BlackList,Reference,UPPER(Other1) as OrangeZone,(CASE When DueDays!='' then DueDays else (Select TOP 1 GraceDays from CompanySetting) end) DueDays,(CASE When Postage!='' then Postage else (Select TOP 1 Postage from CompanySetting) end) Postage,(CASE WHEN FourthTransport='False' then FourthTransport else 'True' end) as PostageStatus,Category,NormalDhara,SNDhara as SUPERDhara from SupplierMaster Where GroupName !='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar))='" + strSaleParty + "' ";

                ds = DataBaseAccess.GetDataSetRecord(strQuery);
            }
            return ds;
        }

        private void GetSalesPartyRecord()
        {
            txtTransport.Text = txtPvtMarka.Text = txtBStation.Text = "";
            bool tStatus = true;
            if (txtSalesParty.Text != "" && txtSubParty.Text != "")
            {
                DataSet ds = GetPendingRecordDataSet();
                if (ds != null)
                {
                    if (ds.Tables.Count > 1)
                    {
                        DataTable dt = ds.Tables[1];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            txtPostage.Text = Convert.ToString(dt.Rows[0]["Postage"]);
                            dSalesPartyDiscount = dba.ConvertObjectToDouble(dt.Rows[0]["NormalDhara"]);

                            if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Clear();
                                txtSubParty.Clear();
                                tStatus = false;
                            }
                            else if (Convert.ToBoolean(dt.Rows[0]["BlackList"]))
                            {
                                MessageBox.Show("This Account is in blacklist ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Text = "";
                                txtSubParty.Text = "";
                                tStatus = false;
                            }
                            else if (Convert.ToString(dt.Rows[0]["OrangeZone"])=="TRUE")
                            {
                                MessageBox.Show("This Account is in orange list ! Please select different account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtSalesParty.Text = "";
                                txtSubParty.Text = "";
                                tStatus = false;
                            }
                            //    txtSalesParty.BackColor = Color.Tomato;
                            //else
                            //    txtSalesParty.BackColor = Color.White;
                            else
                            {
                                if (Convert.ToString(dt.Rows[0]["Category"]) == "CASH PARTY")
                                    pnlCash.Visible = true;
                                else
                                    pnlCash.Visible = false;
                            }

                            if (Convert.ToBoolean(dt.Rows[0]["PostageStatus"]))
                                pnlNOCourier.Visible = false;
                            else
                            {
                                pnlNOCourier.Visible = true;
                                txtPostage.Text = "0.00";
                            }

                            //txtDiscPer.Text = Convert.ToString(dt.Rows[0]["NormalDhara"]);
                        }
                        if (tStatus)
                        {
                            dt.Clear();
                            dt = ds.Tables[0];
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DataRow row = dt.Rows[0];
                                if (row != null)
                                {
                                    txtTransport.Text = Convert.ToString(row["Transport"]);
                                    txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                                    txtBStation.Text = Convert.ToString(row["BookingStation"]);
                                }
                            }
                        }
                    }
                }
            }
        }        

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
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && MainPage.mymainObject.bSaleView)
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

        private void EnableAllControls()
        {
            txtIRNo.ReadOnly= txtSpclDisPer.ReadOnly= txtOtherPerSign.ReadOnly = txtPackingDate.ReadOnly = txtPvtMarka.ReadOnly = txtWayBillNo.ReadOnly = txtWayBIllDate.ReadOnly = txtLRNumber.ReadOnly = txtLRDate.ReadOnly = txtNoofCases.ReadOnly = txtTimeOfSupply.ReadOnly = txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtDiscPer.ReadOnly =   txtPostage.ReadOnly = txtGreenTax.ReadOnly = txtTaxPer.ReadOnly= false;
            dgrdDetails.ReadOnly =  false;
        }

        private void DisableAllControls()
        {
            txtIRNo.ReadOnly = txtSpclDisPer.ReadOnly = txtOtherPerSign.ReadOnly= txtPackingDate.ReadOnly= txtPvtMarka.ReadOnly=txtWayBillNo.ReadOnly=txtWayBIllDate.ReadOnly=txtLRNumber.ReadOnly=txtLRDate.ReadOnly=txtNoofCases.ReadOnly=txtTimeOfSupply.ReadOnly= txtDate.ReadOnly = txtRemark.ReadOnly = txtSign.ReadOnly = txtOtherAmount.ReadOnly = txtPackingAmt.ReadOnly = txtDiscPer.ReadOnly = txtTaxPer.ReadOnly = txtPostage.ReadOnly = txtGreenTax.ReadOnly = txtTaxPer.ReadOnly = true;
            dgrdDetails.ReadOnly =true;          
            lblMsg.Text =lblCreatedBy.Text= "";
        }

        private void ClearAllText()
        {
            txtImportData.Text = txtPetiType.Text = txtPetiAgent.Text = txtPackerName.Text = txtCartonSize.Text = txtCartonType.Text = lblCreatedBy.Text = txtPvtMarka.Text = txtWayBillNo.Text = txtWayBIllDate.Text = txtLRNumber.Text = txtNoofCases.Text = txtTimeOfSupply.Text = txtSalesParty.Text = txtSalesType.Text = txtSubParty.Text = txtSalesType.Text = txtRemark.Text = txtTransport.Text =txtIRNo.Text= "";
            lblTaxableAmt.Text= txtGreenTax.Text= txtSpclDisPer.Text = txtSplDisAmt.Text = txtRoundOff.Text = txtOtherAmount.Text = txtPackingAmt.Text = txtDiscAmt.Text = txtTaxAmt.Text = txtTaxPer.Text = lblTotalQty.Text = lblGrossAmt.Text = lblNetAmt.Text = "0.00";
            txtSign.Text = txtROSign.Text = txtOtherPerSign.Text = "+";
            txtTaxPer.Text = "18.00";
            //if (txtBillCode.Text.Contains("PTN"))
            //    txtDiscPer.Text = "0.10";
            dgrdTax.Rows.Clear();
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add(1);
            dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
            qtyAdjustStatus = chkPick.Checked = false;
            strOldLRNumber = "";
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = txtLRDate.Text = txtPackingDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtLRDate.Text = txtPackingDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");

        }

        private void SetSerialNo()
        {
            DataTable table = DataBaseAccess.GetDataTableRecord("Select (CASE WHEN SNo >= MAXNo AND SNo >= SerialNo THEN SNo WHEN MAXNo >= SNo AND MAXNo >= SerialNo THEN MAXNo WHEN SerialNo >= SNo AND SerialNo >= MAXNo THEN SerialNo ELSE SNo END) SerialNo,TaxName from (Select ISNULL(MAX(BillNo)+1,1)SNo,(Select ISNULL(Max(SaleBillNo)+1,1) from MaxSerialNo)MAXNo,(Select ISNULL(Max(BillNo)+1,1) from SalesBook SB Where SB.BillCode='" + txtBillCode.Text + "')SerialNo,(Select Top 1 TaxName from SaleTypeMaster Where Region='INTERSTATE' and SaleType='SALES' and TaxIncluded=0) TaxName  from SalesRecord Where BillCode='" + txtBillCode.Text + "')Sales ");
            if (table.Rows.Count > 0)
            {
                //double billNo = dba.ConvertObjectToDouble(table.Rows[0][0]), maxBillNo = dba.ConvertObjectToDouble(table.Rows[0][1]),dSerialNo=Convert(;
                //if (billNo > maxBillNo)
                //    txtBillNo.Text = Convert.ToString(billNo);
                //else

                txtBillNo.Text = Convert.ToString(table.Rows[0]["SerialNo"]);
                if (MainPage._bTaxStatus)
                    txtSalesType.Text = Convert.ToString(table.Rows[0]["TaxName"]);
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
            if (txtSalesParty.Text == "")
            {
                MessageBox.Show("Sorry ! SUNDRY DEBTORS Name can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalesParty.Focus();
                return false;
            }
            if(MainPage._bTaxStatus)
            {
                if(txtSalesType.Text=="")
                {
                    MessageBox.Show("Sorry ! Sales Type can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalesType.Focus();
                    return false;
                }
                else
                {
                    if(txtSalesType.Text.Contains("INCLUDE") && !MainPage.strUserRole.Contains("ADMIN"))
                    {
                        MessageBox.Show("Sorry ! Include sales Type is not allowed !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSalesType.Focus();
                        return false;
                    }
                }
                if (dba.ConvertObjectToDouble(txtTaxAmt.Text) == 0)
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
            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;

            CalculateAllAmount();

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {
                string strItem = Convert.ToString(rows.Cells["itemName"].Value);
                double dAmount = ConvertObjectToDouble(rows.Cells["amount"].Value);
                if (strItem == "" && dAmount == 0)
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
                        if (!strItem.Contains("BOX"))
                        {
                            MessageBox.Show("Sorry ! Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell = rows.Cells["qty"];
                            dgrdDetails.Focus();
                            return false;
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Amount  can't be blank ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell = rows.Cells["qty"];
                            dgrdDetails.Focus();
                            if (!MainPage.strUserRole.Contains("ADMIN"))
                                return false;
                        }
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

            double dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
            if (btnAdd.Text == "&Save" || (dOldNetAmt != dNetAmt || strOldPartyName != txtSalesParty.Text))
            {
                bool __bStatus=  ValidateAmountLimit(dNetAmt);
                if (!__bStatus)
                    return __bStatus;
            }  
            return ValidateStock();
        }

        private bool ValidateAmountLimit(double dNetAmt)
        {
            if (txtIRNo.Text != "")
            {
                MessageBox.Show("E-Invoice has been generated, Please cancel EInvoice and remove IRN from this bill !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                    return false;
            }

            object objLimit = DataBaseAccess.ExecuteMyScalar("Select AmountLimit from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dba.ConvertObjectToDouble(objLimit) > 0)
            {
                string strQuery = "";
                if (btnEdit.Text == "&Update")
                    strQuery = " +(Select ISNULL(SUM(CAST(NetAmt as Money)),0) Amt from SalesBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) ";

                double dAmt = dba.CheckAmountLimitValidation(txtSalesParty.Text, strQuery);
                if (dAmt != -1)
                {
                    if (strOldPartyName == txtSalesParty.Text)
                        dAmt -= dNetAmt;
                    if (dAmt < 0 && MainPage.mymainObject.bCreditLimitmanagement)
                    {
                        MessageBox.Show("Sorry ! Amount limit has been exceeded, Please extend amount limit of : " + Math.Abs(dAmt).ToString("N2", MainPage.indianCurancy) + " !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
                else if (MainPage.strUserRole.Contains("ADMIN"))
                    return true;
                else
                {
                    MessageBox.Show("Unable to check balance amount from internet, please connect internet !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }


        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update") && (Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (e.ColumnIndex < 2 || e.ColumnIndex == 11 || e.ColumnIndex == 18)
                        e.Cancel = true;
                    else if (e.ColumnIndex == 2)
                    {
                        string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";
                        bool _bChkStatus = false;
                        if (!txtBillCode.Text.Contains("PTN"))
                            _bChkStatus = true;

                        if (MainPage.strSoftwareType != "AGENT")
                            _bChkStatus = false;

                        SearchCategory objSearch = new SearchCategory("", "DESIGNNAMEWITHBARCODE", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, true, _bChkStatus);
                        objSearch.ShowDialog();
                        GetAllDesignSizeColorWithBarCode(objSearch, dgrdDetails.CurrentRow.Index);
                        e.Cancel = true;

                    }
                    else if (e.ColumnIndex == 3)
                    {
                        if (txtSalesParty.Text != "")
                        {
                            string strItemName = "", strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";
                            bool _boxStatus = false;
                            if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                            {
                                strItemName = Convert.ToString(dgrdDetails.CurrentRow.Cells["itemName"].Value);
                                strCategory1 = Convert.ToString(dgrdDetails.CurrentRow.Cells["variant1"].Value);
                                strCategory2 = Convert.ToString(dgrdDetails.CurrentRow.Cells["variant2"].Value);
                                strCategory3 = Convert.ToString(dgrdDetails.CurrentRow.Cells["variant3"].Value);
                                strCategory4 = Convert.ToString(dgrdDetails.CurrentRow.Cells["variant4"].Value);
                                strCategory5 = Convert.ToString(dgrdDetails.CurrentRow.Cells["variant5"].Value);
                                _boxStatus = true;
                            }

                            SearchCategory objSearch = new SearchCategory("", "SONUMBER", strItemName, "", "", "", "", "", txtSalesParty.Text, Keys.Space, _boxStatus);
                            objSearch.ShowDialog();
                            GetAllSONumberDesignSizeColor(objSearch, dgrdDetails.CurrentRow.Index);
                        }
                        else
                            MessageBox.Show("Sorry ! Please enter party name for SO Number !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 6 || e.ColumnIndex == 7 || e.ColumnIndex == 8 || e.ColumnIndex == 9)
                    {
                        if (!txtBillCode.Text.Contains("PTN"))
                        {
                            string strCategory1 = "", strCategory2 = "", strCategory3 = "", strCategory4 = "", strCategory5 = "";
                            bool _bChkStatus = false;
                            if (!txtBillCode.Text.Contains("PTN"))
                                _bChkStatus = true;
                            if (MainPage.strSoftwareType != "AGENT")
                                _bChkStatus = false;

                            SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", strCategory1, strCategory2, strCategory3, strCategory4, strCategory5, Keys.Space, true, _bChkStatus);
                            objSearch.ShowDialog();
                            GetAllDesignSizeColor(objSearch, dgrdDetails.CurrentRow.Index);
                        }
                        e.Cancel = true;
                    }
                    else if ((e.ColumnIndex == 13 || e.ColumnIndex == 12 || e.ColumnIndex == 14) && !MainPage.strUserRole.Contains("ADMIN") && !MainPage.strLoginName.Contains("ASHISH"))
                    {
                        if (e.ColumnIndex == 12)
                        {
                            {
                                if (MainPage.strSoftwareType == "AGENT")
                                    e.Cancel = true;
                                else if (Convert.ToString(dgrdDetails.CurrentRow.Cells["barCode"].Value) != "")
                                    e.Cancel = true;
                            }
                        }
                        else
                            e.Cancel = true;
                    }
                       
                }               
                else
                    e.Cancel = true;
            }
            catch
            {
            }
        }

        private void GetAllSONumberDesignSizeColor(SearchCategory objCategory, int rowIndex)
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
                                string[] strAllItem = strItem.Split('|');
                                if (strAllItem.Length > 1)
                                {
                                    if (firstRow)
                                        dgrdDetails.Rows.Add();
                                    else
                                        firstRow = true;
                                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                    if (strAllItem.Length > 1)
                                        dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = strAllItem[1];

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) == "")
                                    {
                                        if (strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2];
                                        if (strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3];
                                        if (strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4];
                                    }
                                    //if (strAllItem.Length > 4)
                                    //    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                                    //if (strAllItem.Length > 5)
                                    //    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                                    //if (strAllItem.Length > 6)
                                    //    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];

                                    if (strAllItem.Length > 1)
                                    {
                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                        {
                                            if (strAllItem[0].Contains(objCategory.txtSearch.Text.Trim()))
                                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                            else if (strAllItem.Length > 5)
                                                dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[5];
                                        }
                                        if ((txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                            GetSaleRate(dgrdDetails.Rows[rowIndex]);

                                        SetUnitName(strAllItem[2], rowIndex);
                                    }

                                    rowIndex++;
                                    if (strAllItem[0].Contains(objCategory.txtSearch.Text.Trim()))
                                        break;                                  
                                        
                                }
                            }
                            rowIndex--;
                        }
                        else
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                if (strAllItem.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["soNumber"].Value = strAllItem[1];

                                if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) == "")
                                {
                                    if (strAllItem.Length > 2)
                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[2];
                                    if (strAllItem.Length > 4)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[3];
                                    if (strAllItem.Length > 5)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[4];
                                }
                                //if ( strAllItem.Length > 4)
                                //    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                                //if ( strAllItem.Length > 5)
                                //    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                                //if (strAllItem.Length > 6)
                                //    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];

                                if (strAllItem.Length > 1)
                                {
                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["qty"].Value) == "")
                                    {
                                        if (strAllItem[0].Contains(objCategory.txtSearch.Text.Trim()))
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        else if (strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[5];
                                    }

                                    if ((txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                        GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                    SetUnitName(strAllItem[1], rowIndex);
                                }
                            }
                        }
                        ArrangeSerialNo();
                    }

                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex == dgrdDetails.Rows.Count - 1)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["soNumber"];
                        dgrdDetails.Focus();
                    }   
                }

                CalculateAllAmount();
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
                                string[] strAllItem = strItem.Split('|');
                                if (strItem != "ADD NEW DESIGNNAME NAME")
                                {
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;

                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[0];

                                        if (strAllItem.Length > 1)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                        if (strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                        if (strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                        if (strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                        if (strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                        if (strAllItem.Length > 6)                                          
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[6];
                                        if (strAllItem.Length > 7)
                                            dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[7];
                                        if (strAllItem.Length > 8)
                                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[8];
                                        if (strAllItem.Length > 9)
                                            dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[9];



                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "" && (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                            GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                        else
                                        {
                                            double dQty = 0, dRate = 0, dAmt = 0;
                                            if (strAllItem.Length > 6)
                                                dQty = ConvertObjectToDouble(strAllItem[6]);
                                            if (strAllItem.Length > 7)
                                                dRate = ConvertObjectToDouble(strAllItem[9]);
                                            dAmt = dQty * dRate;
                                            dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                                        }
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
                                if (strAllItem.Length > 1)
                                    dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[1];
                                if (strAllItem.Length > 2)
                                    dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[2];
                                if (strAllItem.Length > 3)
                                    dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[3];
                                if (strAllItem.Length > 4)
                                    dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[4];
                                if (strAllItem.Length > 5)
                                    dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[5];

                                if (strAllItem.Length > 6)
                                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[6];
                                if (strAllItem.Length > 7)
                                    dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[7];
                                if (strAllItem.Length > 8)
                                    dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[8];
                                if (strAllItem.Length > 9)
                                    dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[9];

                                if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                                    GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                else
                                {
                                    double dQty = 0, dRate = 0, dAmt = 0 ;
                                    if (strAllItem.Length > 6)
                                        dQty = ConvertObjectToDouble(strAllItem[6]);
                                    if (strAllItem.Length > 7)
                                        dRate = ConvertObjectToDouble(strAllItem[9]);
                                    dAmt = dQty * dRate;
                                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                }

                                SetUnitName(strAllItem[0], rowIndex);
                            }
                        }

                        ArrangeSerialNo();
                        CalculateAllAmount();

                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex==dgrdDetails.Rows.Count-1)
                        {
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["itemName"];
                            dgrdDetails.Focus();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void GetAllDesignSizeColorWithBarCode(SearchCategory objCategory, int rowIndex)
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
                            string strItem = Convert.ToString(objCategory.lbSearchBox.Items[0]);
                            //foreach (string strItem in objCategory.lbSearchBox.Items)
                            {
                                string[] strAllItem = strItem.Split('|');
                                if (strItem != "ADD NEW DESIGNNAMEWITHBARCODE NAME")
                                {
                                    if (strAllItem.Length > 0)
                                    {
                                        if (firstRow)
                                            dgrdDetails.Rows.Add();
                                        else
                                            firstRow = true;
                                        dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                        dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[1];
                                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;
                                        if (strAllItem.Length > 2)
                                            dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[2];
                                        if (strAllItem.Length > 3)
                                            dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[3];
                                        if (strAllItem.Length > 4)
                                            dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                                        if (strAllItem.Length > 5)
                                            dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                                        if (strAllItem.Length > 6)
                                            dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];

                                        if (strAllItem.Length > 7)
                                            dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[7];
                                        if (strAllItem.Length > 8)
                                            dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[8];
                                        if (strAllItem.Length > 9)
                                            dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[9];
                                        if (strAllItem.Length > 10)
                                            dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[10];
                                        
                                        if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "" && (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                            GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                        else
                                        {
                                            double dQty = 0, dRate = 0, dAmt = 0;
                                            if (strAllItem.Length > 7)
                                                dQty = ConvertObjectToDouble(strAllItem[7]);
                                            if (strAllItem.Length > 8)
                                                dRate = ConvertObjectToDouble(strAllItem[10]);
                                            dAmt = dQty * dRate;
                                            dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                                        }
                                        SetUnitName(strAllItem[1], rowIndex);

                                        rowIndex++;
                                    }
                                }
                            }
                            if (rowIndex > 0)
                                rowIndex--;
                        }
                        else
                        {
                            if (strData != "ADD NEW DESIGNNAMEWITHBARCODE NAME")
                            {
                                string[] strAllItem = strData.Split('|');
                                if (strAllItem.Length > 0)
                                {
                                    dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = strAllItem[0];
                                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = strAllItem[1];
                                    dgrdDetails.Rows[rowIndex].Cells["qty"].Value = 1;

                                    if (strAllItem.Length > 2)
                                        dgrdDetails.Rows[rowIndex].Cells["variant1"].Value = strAllItem[2];
                                    if (strAllItem.Length > 3)
                                        dgrdDetails.Rows[rowIndex].Cells["variant2"].Value = strAllItem[3];
                                    if (strAllItem.Length > 4)
                                        dgrdDetails.Rows[rowIndex].Cells["variant3"].Value = strAllItem[4];
                                    if (strAllItem.Length > 5)
                                        dgrdDetails.Rows[rowIndex].Cells["variant4"].Value = strAllItem[5];
                                    if (strAllItem.Length > 6)
                                        dgrdDetails.Rows[rowIndex].Cells["variant5"].Value = strAllItem[6];

                                    if (strAllItem.Length > 7)
                                        dgrdDetails.Rows[rowIndex].Cells["qty"].Value = strAllItem[7];
                                    if (strAllItem.Length > 8)
                                        dgrdDetails.Rows[rowIndex].Cells["mrp"].Value = strAllItem[8];
                                    if (strAllItem.Length > 9)
                                        dgrdDetails.Rows[rowIndex].Cells["disPer"].Value = strAllItem[9];
                                    if (strAllItem.Length > 10)
                                        dgrdDetails.Rows[rowIndex].Cells["rate"].Value = strAllItem[10];

                                    if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["id"].Value) == "" && (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT"))
                                        GetSaleRate(dgrdDetails.Rows[rowIndex]);
                                    else
                                    {
                                        double dQty = 0, dRate = 0, dAmt = 0;
                                        if (strAllItem.Length > 7)
                                            dQty = ConvertObjectToDouble(strAllItem[7]);
                                        if (strAllItem.Length > 8)
                                            dRate = ConvertObjectToDouble(strAllItem[10]);
                                        dAmt = dQty * dRate;
                                        dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                                    }

                                    SetUnitName(strAllItem[1], rowIndex);
                                }
                            }
                        }
                        ArrangeSerialNo();
                        CalculateAllAmount();

                       if (Convert.ToString(dgrdDetails.Rows[rowIndex].Cells["itemName"].Value) != "" && rowIndex == dgrdDetails.Rows.Count - 1)
                        {                         
                            dgrdDetails.Rows.Add(1);
                            dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                            dgrdDetails.Focus();
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

        private void GetSaleRate(DataGridViewRow row)
        {
            try
            {
                
                double dDisPer = 0, dMRP = 0,_dMRP=0, dRate = 0,dQty=1, _dQty = 0, dSpclDis = dba.ConvertObjectToDouble(txtSpclDisPer.Text),dQtyRatio=1;
                if (Convert.ToString(row.Cells["soNumber"].Value) != "")
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);

                if (row != null)
                {
                    object objDisPer = 0;
                    if (Convert.ToString(row.Cells["itemName"].Value) != "")
                    {
                        DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
                        object objValue = 0;
                        if (MainPage.strSoftwareType == "AGENT")
                            objValue = dba.GetSaleRate(row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref _dQty, ref objDisPer, _date);
                        else
                        {
                            objValue = dba.GetSaleRate_Other(row.Cells["itemName"].Value, row.Cells["variant1"].Value, row.Cells["variant2"].Value, row.Cells["variant3"].Value, row.Cells["variant4"].Value, row.Cells["variant5"].Value, ref _dQty, ref objDisPer, _date, ref dQtyRatio);
                            if (dSalesPartyDiscount > 0)
                                objDisPer = dSalesPartyDiscount;
                        }

                        dDisPer = ConvertObjectToDouble(objDisPer) * -1;
                        dMRP = ConvertObjectToDouble(objValue);
                        row.Cells["mrp"].Value = dMRP;
                    }
                }

                if (_dQty <= 0)
                    row.DefaultCellStyle.BackColor = Color.Tomato;

                if (dSpclDis != 0 && dMRP != 0)
                    _dMRP= dMRP * (100.00 -dSpclDis) / 100.00;

                if (dDisPer != 0 && _dMRP != 0)
                    dRate = _dMRP * (100.00 + dDisPer) / 100.00;
                if (dRate == 0)
                    dRate = _dMRP;

                dQtyRatio = (dQty * dQtyRatio);

                row.Cells["mrp"].Value = dMRP;
                row.Cells["disPer"].Value = dDisPer;
                row.Cells["rate"].Value = dRate;
                row.Cells["qty"].Value = dQtyRatio;

                double dAmt = 0, dDisc = ConvertObjectToDouble(row.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQtyRatio * dRate;

                row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 10)
                        CalculateAmountWithQtyRate(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 13 || e.ColumnIndex == 12)
                        CalculateAmountWithMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 14)
                        CalculateDisWithAmountMRP(dgrdDetails.Rows[e.RowIndex]);
                    else if (e.ColumnIndex == 15)
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
            rows.Cells["rate"].Value = dRate.ToString("N2",MainPage.indianCurancy);
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

                            if(Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["barCode"].Value)!="" && IndexColmn==11)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
                                dgrdDetails.Focus();
                            }
                        }
                        else if (Index == dgrdDetails.RowCount - 1)
                        {
                            string strItemName = Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value);
                          //  double dAmt = ConvertObjectToDouble(dgrdDetails.Rows[CurrentRow].Cells["amount"].Value);

                            if (strItemName != "")//&& dAmt>0
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["srNo"].Value = dgrdDetails.Rows.Count;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.RowCount - 1].Cells["barCode"];
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
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
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
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["id"].Value);//,strSONumber = Convert.ToString(dgrdDetails.CurrentRow.Cells["soNumber"].Value);
                        if (strID == "")
                        {
                            dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
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
                        int colIndex = dgrdDetails.CurrentCell.ColumnIndex;
                        if (colIndex == 3)// && colIndex != 10 && colIndex != 13 && colIndex != 15 && colIndex != 18)
                            dgrdDetails.CurrentCell.Value = "";
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
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 10 || columnIndex == 12 || columnIndex == 13 || columnIndex == 14 || columnIndex == 15)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex == 13)
                {
                    Char pressedKey = e.KeyChar;
                    if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                        e.Handled = false;
                    else
                        dba.KeyHandlerPoint(sender, e, 2);
                }
                else if (columnIndex == 10 || columnIndex == 12 || columnIndex == 14 || columnIndex == 15)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
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
                    string strQuery = " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and [SID]=" + strID + " "
                                    + " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.[SID], SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text + " and SBS.[SID]=" + strID + " ";
                    int _index = dgrdDetails.CurrentRow.Index;
                    dgrdDetails.Rows.RemoveAt(_index);
                    CalculateAllAmount();
                   // if (ValidateControls())
                    {
                        int result = UpdateRecord(strQuery);
                        if (result < 1)
                            BindRecordWithControl(txtBillNo.Text);
                        else
                        {
                            strQuery = " Delete from SalesBookSecondary Where  [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and RemoteID=" + strID + " "
                                    + " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.[SID], SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text + " and SBS.[SID]=" + strID + " ";
                            DataBaseAccess.CreateDeleteQuery(strQuery);
                            if (dgrdDetails.Rows.Count == 0)
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["barCode"];
                                dgrdDetails.Enabled = true;
                            }
                            else
                                ArrangeSerialNo();
                        }

                        dgrdDetails.ReadOnly = false;
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

        private void CalculateAmountWithQtyRate(DataGridViewRow rows)
        {
            double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dRate = ConvertObjectToDouble(rows.Cells["rate"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);
            dAmt = dQty * dRate;
            rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
            rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
            CalculateAllAmount();
        }
        private void CalculateAmountWithMRP(DataGridViewRow rows)
        {            

            double dDisPer = 0, dMRP = 0, dRate = 0;
            if (rows != null)
            {
                dMRP = ConvertObjectToDouble(rows.Cells["mrp"].Value);
                dDisPer = ConvertObjectToDouble(rows.Cells["disPer"].Value);               

                if (dDisPer != 0 && dMRP != 0)
                    dRate = dMRP * (100 + dDisPer) / 100;
                if (dRate == 0)
                    dRate = dMRP;
                dRate = Math.Round(dRate, 2);

                rows.Cells["rate"].Value = dRate;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
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

                rows.Cells["disPer"].Value = dDisPer*-1;
                double dAmt = 0, dQty = ConvertObjectToDouble(rows.Cells["qty"].Value), dDisc = ConvertObjectToDouble(rows.Cells["disc"].Value), dOCharges = ConvertObjectToDouble(rows.Cells["otherCharges"].Value);//, dNRate = ConvertObjectToDouble(row.Cells["rate"].Value)
                dAmt = dQty * dRate;

                rows.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);
                rows.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);

                CalculateAllAmount();
            }
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

                double dFinalAmt = 0, dQty = 0, dTOAmt = 0, dBasicAmt = 0, dOtherAmt = 0, dNetAmt = 0, dPackingAmt = 0, dDiscAmt = 0, dTaxAmt = 0, dPostage = 0, dGreenTaxAmt = 0, dRoundOff = 0, dServiceAmt = 0,dTaxableAmt=0;
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dQty += ConvertObjectToDouble(rows.Cells["qty"].Value);
                    dBasicAmt += ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                }

                lblGrossAmt.Text = dBasicAmt.ToString("N2", MainPage.indianCurancy);
                dPackingAmt = ConvertObjectToDouble(txtPackingAmt.Text);               
                dPostage = ConvertObjectToDouble(txtPostage.Text);
                dGreenTaxAmt = ConvertObjectToDouble(txtGreenTax.Text);

                if (txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                {
                    dOtherAmt = ConvertObjectToDouble(txtOtherAmount.Text);
                    if (txtSign.Text == "-")
                        dOtherAmt = dOtherAmt * -1;
                }

                if (!txtBillCode.Text.Contains("PTN"))
                {
                    if (MainPage.strSoftwareType == "AGENT")
                        dServiceAmt = dba.ConvertObjectToDouble(txtSign.Text + txtOtherAmount.Text);
                }

                double dDisPer = ConvertObjectToDouble(txtOtherPerSign.Text + txtDiscPer.Text), dGrossAmt = 0;

                //dDiscAmt = (dBasicAmt * dDisPer) / 100;
                dTOAmt = dOtherAmt + dPackingAmt + dPostage + dGreenTaxAmt;

              //  dDiscAmt = (dGrossAmt * dDisPer) / 100;
               // dTOAmt += dDiscAmt;

                // dTOAmt = dOtherAmt + dPackingAmt;
                dGrossAmt = dBasicAmt + dTOAmt;
                dDiscAmt = (dGrossAmt * dDisPer) / 100;
                dTOAmt += dDiscAmt;


                dFinalAmt = dGrossAmt + dDiscAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt, dDiscAmt,dServiceAmt,ref dTaxableAmt);
              

                dNetAmt = dFinalAmt + dTaxAmt + dServiceAmt;

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

                lblTotalQty.Text = dQty.ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNNetAmt.ToString("N2", MainPage.indianCurancy);
                txtDiscAmt.Text = Math.Abs(dDiscAmt).ToString("0.00");
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
                    if (txtTaxPer.Text == "")
                        txtTaxPer.Text = "0.00";                 
                    CalculateAllAmount();
                }           
        }

        private void txtPackingAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
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
                chkEmail.Checked = chkSendSMS.Checked = true;
                ClearAllText();
                SetSerialNo();
                txtDate.Focus();
            }
            else if (ValidateControls() && CheckBillNoAndSuggest() && ValidateOtherValidation(false))
            {
                DialogResult result = MessageBox.Show("Are you sure want to save record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveRecord();
                }
            }
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


        private void SaveRecord()
        {
            try
            {
                string strDate="", strLRDate = "NULL", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dAmt = ConvertObjectToDouble(lblNetAmt.Text),dTaxAmt=dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc = dba.ConvertObjectToDouble(txtDiscAmt.Text),dGrossAmt=dba.ConvertObjectToDouble(lblGrossAmt.Text),_dOtherAmt=dba.ConvertObjectToDouble(txtOtherAmount.Text),dFinalAmt=0;
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strPetiAgent = "DIRECT";
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
                   string[] _strFullName = txtPetiAgent.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strPetiAgent = _strFullName[0].Trim();
                    }
                }

                if (txtSign.Text == "-" || txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                    _dOtherAmt = 0;
                else if(_dOtherAmt>0)
                    _dOtherAmt = Math.Round(((_dOtherAmt / 3) * 100),2);
                dFinalAmt = dGrossAmt + _dOtherAmt;

                string strQuery = " if not exists(Select BillNo from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + "  UNION ALL Select BillNo from [SalesRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + ") begin "
                                + " INSERT INTO [dbo].[SalesBook] ([BillCode],[BillNo],[Date],[SalePartyID],[SubPartyID],[SalesType],[Station],[TransportName],[WaybillNo],[WayBillDate],[NoOfCase],[LRNumber],[LRDate],[LRTime],[PvtMarka],[Remark],[Description],[PackerName],[PackingDate],[CartonType],[CartonSize],[DisPer],[DisAmt],[TaxPer],[TaxAmt],[PackingAmt],[OtherSign],[OtherAmt],[PostageAmt],[GreenTax],[RoundOffSign],[RoundOffAmt],[TotalQty],[GrossAmt],[FinalAmt],[NetAmt],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[OrderNo],[AttachedBill],[PackedBillNo],[Description_1],[Description_2],[Description_3],[SpecialDscPer],[SpecialDscAmt],[TaxableAmt],[IRNNO]) VALUES  "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','"+ strSalePartyID+"','"+strSubPartyID+"','"+txtSalesType.Text+ "','" + txtBStation.Text + "','" + txtTransport.Text + "','" + txtWayBillNo.Text + "','" + txtWayBIllDate.Text + "','" + txtNoofCases.Text + "','" + txtLRNumber.Text + "',"+strLRDate+ ",'" + txtTimeOfSupply.Text + "','" + txtPvtMarka.Text + "','" + txtRemark.Text + "','"+txtOtherPerSign.Text+"','" + txtPackerName.Text + "',"+strPDate+ ",'" + txtCartonType.Text + "','" + txtCartonSize.Text + "', "
                                + " " + dba.ConvertObjectToDouble(txtDiscPer.Text) + "," + dDisc + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dba.ConvertObjectToDouble(txtTaxAmt.Text) + "," + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",'" + txtSign.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmount.Text) + "," + dba.ConvertObjectToDouble(txtPostage.Text) + "," + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",'" + txtROSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOff.Text) + "," + dba.ConvertObjectToDouble(lblTotalQty.Text) + "," + dGrossAmt + ","+dFinalAmt+"," + dAmt + ",'" + MainPage.strLoginName+ "','',1,0,'','','','"+ strPetiAgent+"','"+txtPetiType.Text+"','"+txtImportData.Text+"',"+dba.ConvertObjectToDouble(txtSpclDisPer.Text)+","+dba.ConvertObjectToDouble(txtSplDisAmt.Text)+","+ dba.ConvertObjectToDouble(lblTaxableAmt.Text) +",'"+txtIRNo.Text+"')  "
                                + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  " 
                                + " ('" + strDate + "','" + strSaleParty + "','SALES A/C','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dAmt + "','DR','"+ dFinalAmt + "','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strSalePartyID + "')  ";

                double dQty = 0, dRate = 0;
                string strSONumber = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);

                    strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                  + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'"+strSONumber+ "','" + row.Cells["barCode"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty+ "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + "," + dRate + ","
                                  + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0)";

                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode]) VALUES "
                             + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "','" + row.Cells["barCode"].Value + "') ";
                    }

                    if(strSONumber!="")
                        strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + dQty + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dQty + "), UpdateStatus=1 where (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode)='"+strSONumber+"'  ";
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";
                           
                if (dTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text+" "+ txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                                   + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt+ ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";

                if (chkCourier.Checked && !pnlNOCourier.Visible)
                    strQuery += dba.SaveCourierDetails(txtBillCode.Text, txtBillNo.Text, strSalePartyID, strSaleParty, txtBStation.Text);


                strQuery += " end ";

                if (strQuery != "")
                {
                    int count = dba.ExecuteMyQuery(strQuery);
                    if (count > 0)
                    {
                        string strMobileNo = "", strPath = "";
                        SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                        SendSMSToParty(strMobileNo);
                       
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

        //private string SaveCourierDetails(string strDate, string strPartyID, string strPartyName)
        //{
        //    string strQuery = "";
        //    if (chkCourier.Checked && !pnlNOCourier.Visible)
        //    {
        //        strQuery += "Declare @CCode varchar(50),@CSNo bigint,@CourierName varchar(250); "
        //                 + " Select @CCode = SCode, @CSNo = (ISNULL(MAX(SNo), 0) + 1), @CourierName = (Select Top 1 SM.CourierName from SupplierMaster SM Where AreaCode+AccountNo = '" + strPartyID + "') from CourierRegister Group by SCode "
        //                 + " if (@CCode is NULL) begin Select @CCode = CourierCode from CompanySetting Select Top 1 @CourierName = SM.CourierName from SupplierMaster SM Where (AreaCode+AccountNo)='" + strPartyID + "' end"
        //                 + " if not exists (Select ID from CourierRegister where [SaleBillCode]='" + txtBillCode.Text + "' and [SaleBillNo]='" + txtBillNo.Text + "') begin "
        //                 + " INSERT INTO [dbo].[CourierRegister] ([SCode],[SNo],[SerialCode],[CourierNo],[CourierName],[Date],[DocType],[SalesParty],[Station],[Remark],[UserName],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleBillCode],[SaleBillNo],[SalePartyID]) VALUES "
        //                 + " (@CCode,@CSNo,'','',@CourierName,'" + strDate + "','BILL','" + strPartyName + "','" + txtBStation.Text + "','','" + MainPage.strLoginName + "','',1,0,'" + txtBillCode.Text + "','" + txtBillNo.Text + "','" + strPartyID + "') "
        //                 + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
        //                 + " ('COURIEROUT',@CCode,@CSNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + MainPage.strLoginName + "',1,0,'CREATION') end ";
        //    }
        //    return strQuery;
        //}

        private void EditOldRecord()
        {
          
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
                    chkCourier.Checked = false;
                    if (dgrdDetails.Rows.Count == 0)
                    {
                        dgrdDetails.Rows.Add(1);
                        dgrdDetails.Rows[0].Cells["srNo"].Value = 1;
                    }
                    txtDate.Focus();
                }
                else if (ValidateControls() && ValidateOtherValidation(false))
                {
                    string strBiltyPath = "";
                    if (strOldLRNumber != txtLRNumber.Text && !txtLRNumber.Text.Contains("PKD") && !txtLRNumber.Text.Contains("HAND") && !txtLRNumber.Text.Contains("BUS") && txtLRNumber.Text != "")
                    {
                        strBiltyPath = DataBaseAccess.GetBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
                        if (strBiltyPath == "" && !MainPage.strUserRole.Contains("SUPERADMIN"))
                            return;
                        chkEmail.Checked = false;
                        chkCourier.Checked = true;
                    }
                    
                    DialogResult result = MessageBox.Show("Are you sure want to update record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = UpdateRecord("");
                        if (count > 0)
                        {
                            string strMobileNo = "", strPath = "";
                            if (strBiltyPath != "")
                                SendEmailBiltyToSalesParty(false, ref strMobileNo, ref strBiltyPath);

                            SendEmailToSalesParty(false, ref strMobileNo, ref strPath);
                            SendSMSToParty(strMobileNo);

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
                string strDate = "", strLRDate = "NULL", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);

                if (txtLRNumber.Text != "" && txtLRDate.Text.Length == 10)
                    strLRDate = "'" + dba.ConvertDateInExactFormat(txtLRDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";
                if (txtPackingDate.Text.Length == 10)
                    strPDate = "'" + dba.ConvertDateInExactFormat(txtPackingDate.Text).ToString("MM/dd/yyyy hh:mm:ss") + "'";

                strDate = bDate.ToString("MM/dd/yyyy h:mm:ss tt");

                double dAmt = ConvertObjectToDouble(lblNetAmt.Text), dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text), dDisc= dba.ConvertObjectToDouble(txtDiscAmt.Text), dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text), _dOtherAmt = dba.ConvertObjectToDouble(txtOtherAmount.Text), dFinalAmt = 0; 
                string strSaleParty = "", strSubParty = "", strSalePartyID = "", strSubPartyID = "", strTaxAccountID="", strPetiAgent = "DIRECT";
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
                    string[] _strFullName = txtPetiAgent.Text.Split(' ');
                    if (_strFullName.Length > 0)
                    {
                        strPetiAgent = _strFullName[0].Trim();
                    }
                }

                if (txtSign.Text == "-" || txtBillCode.Text.Contains("PTN") || MainPage.strSoftwareType != "AGENT")
                    _dOtherAmt = 0;
                else if (_dOtherAmt > 0)
                    _dOtherAmt = Math.Round(((_dOtherAmt / 3) * 100), 2);
                dFinalAmt = dGrossAmt + _dOtherAmt;


                string strQuery = " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where RTRIM(OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text
                                + " if exists (Select [BillCode] from [SalesBook] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin "
                                + " Update [dbo].[SalesBook] Set [Date]='" + strDate + "',[SalePartyID]='" + strSalePartyID + "',[SubPartyID]='" + strSubPartyID + "',[SalesType]='" + txtSalesType.Text + "',[Station]='" + txtBStation.Text + "',[TransportName]='" + txtTransport.Text + "',[WaybillNo]='" + txtWayBillNo.Text + "',[WayBillDate]='" + txtWayBIllDate.Text + "',[NoOfCase]='" + txtNoofCases.Text + "',[LRNumber]='" + txtLRNumber.Text + "',[LRDate]=" + strLRDate + ",[LRTime]='" + txtTimeOfSupply.Text + "',[PvtMarka]='" + txtPvtMarka.Text + "',[Remark]='" + txtRemark.Text + "',[Description]='" + txtOtherPerSign.Text + "',[PackerName]='" + txtPackerName.Text + "',[PackingDate]=" + strPDate + ",[CartonType]='" + txtCartonType.Text + "',[CartonSize]='" + txtCartonSize.Text + "',[DisPer]=" + dba.ConvertObjectToDouble(txtDiscPer.Text) + ",[DisAmt]=" + dDisc + ","
                                + " [TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmt]=" + dba.ConvertObjectToDouble(txtTaxAmt.Text) + ",[PackingAmt]=" + dba.ConvertObjectToDouble(txtPackingAmt.Text) + ",[OtherSign]='" + txtSign.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmount.Text) + ",[PostageAmt]=" + dba.ConvertObjectToDouble(txtPostage.Text) + ",[GreenTax]=" + dba.ConvertObjectToDouble(txtGreenTax.Text) + ",[RoundOffSign]='" + txtROSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOff.Text) + ",[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblTotalQty.Text) + ",[GrossAmt]=" +dGrossAmt + ",[FinalAmt]=" + dFinalAmt + ",[NetAmt]=" + dAmt + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[Description_1]='"+ strPetiAgent+"',[Description_2]='"+ txtPetiType.Text+ "' ,[Description_3]='" + txtImportData.Text + "',[SpecialDscPer]=" + dba.ConvertObjectToDouble(txtSpclDisPer.Text) + ",[SpecialDscAmt]=" + dba.ConvertObjectToDouble(txtSplDisAmt.Text) + ",[IRNNO]='"+txtIRNo.Text+"' Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strSaleParty + "',[Amount]=" + dAmt + ",[FinalAmount]='" + dFinalAmt + "',[UpdatedBy]='" + MainPage.strLoginName + "',[AccountID]='" + strSalePartyID + "' Where Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus]='SALES A/C'  "
                                + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ";
                               
                string  strID = "", strSONumber="";
                double dQty = 0, dRate = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSONumber = Convert.ToString(row.Cells["soNumber"].Value);
                    dQty = ConvertObjectToDouble(row.Cells["qty"].Value);
                    dRate = ConvertObjectToDouble(row.Cells["rate"].Value);
                    strID = Convert.ToString(row.Cells["id"].Value);
                    if (strID == "")
                    {
                        strQuery += " INSERT INTO [dbo].[SalesBookSecondary] ([BillCode],[BillNo],[RemoteID],[SONumber],[BarCode],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[MRP],[SDisPer],[Rate],[Amount],[Disc],[OCharges],[BasicAmt],[UnitName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'" + strSONumber + "','" + row.Cells["barCode"].Value + "','" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + "," + dRate + ","
                                 + " " + ConvertObjectToDouble(row.Cells["amount"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["disc"].Value) + "," + dba.ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ", " + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",'" + row.Cells["unitName"].Value + "','" + MainPage.strLoginName + "','',1,0)";
                    }
                    else
                        strQuery += " Update [dbo].[SalesBookSecondary] SET [SONumber]='" + strSONumber + "',[BarCode]='" + row.Cells["barCode"].Value + "',[ItemName]='" + row.Cells["itemName"].Value + "',[Variant1]='" + row.Cells["variant1"].Value + "',[Variant2]='" + row.Cells["variant2"].Value + "',[Variant3]='" + row.Cells["variant3"].Value + "',[Variant4]='" + row.Cells["variant4"].Value + "',[Variant5]='" + row.Cells["variant5"].Value + "',[Qty]=" + dQty + ",[MRP]=" + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",[SDisPer]=" + dba.ConvertObjectToDouble(row.Cells["disPer"].Value) + ",[Rate]=" + dRate + ",[Amount]=" + ConvertObjectToDouble(row.Cells["amount"].Value) + ",[Disc]=" + ConvertObjectToDouble(row.Cells["disc"].Value) + ", [OCharges]=" + ConvertObjectToDouble(row.Cells["otherCharges"].Value) + ",[BasicAmt]=" + ConvertObjectToDouble(row.Cells["netAmt"].Value) + ",[UnitName]='" + row.Cells["unitName"].Value + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + "  ";

                    if (strSONumber != "")
                        strQuery += " Update OrderBooking set Status=(Case When (CAST(Quantity as Money)-(AdjustedQty+ISNULL(CancelQty,0)+ " + dQty + "))>0 Then 'PENDING' else 'CLEAR' end),AdjustedQty=(AdjustedQty+" + dQty + "), UpdateStatus=1 where (OrderCode+' '+CAST(OrderNo as varchar)+' '+NumberCode)='" + strSONumber + "'  ";


                    if (MainPage._bTaxStatus || txtImportData.Text == "")
                    {
                        strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode]) VALUES "
                                 + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ", '" + row.Cells["itemName"].Value + "','" + row.Cells["variant1"].Value + "','" + row.Cells["variant2"].Value + "','" + row.Cells["variant3"].Value + "','" + row.Cells["variant4"].Value + "','" + row.Cells["variant5"].Value + "'," + dQty + "," + dRate + " ,'','" + MainPage.strLoginName + "','',1,0," + dba.ConvertObjectToDouble(row.Cells["mrp"].Value) + ",'" + strDate + "','" + row.Cells["barCode"].Value + "') ";
                    }
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ";

                if (dTaxAmt > 0 && txtSalesType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='SALES' and TaxName = '" + txtSalesType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName=Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                         + "('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";

                strQuery = strSubQuery + strQuery;

                if (chkCourier.Checked && !pnlNOCourier.Visible)
                    strQuery += dba.SaveCourierDetails(txtBillCode.Text, txtBillNo.Text, strSalePartyID, strSaleParty, txtBStation.Text);


                strQuery += " end";

                // end Purchase Entry

                //object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from GoodsReceive Where ReceiptCode='" + txtBillCode.Text + "' and ReceiptNo=" + txtBillNo.Text + " ");
                          
               
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

        private void txtSalesType_KeyDown(object sender, KeyEventArgs e)
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
                        txtSalesType.Text = objSearch.strSelectedData;
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

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    GSTPrintAndPreview(false, "", false,true);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Sales Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    SetSignatureInBill(true, false,true);
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }      



        private void PurchaseBook_Load(object sender, EventArgs e)
        {
            SetPermission();
        }

        private void SetPermission()
        {
            if (MainPage.mymainObject.bSaleAdd || MainPage.mymainObject.bSaleEdit || MainPage.mymainObject.bSaleView)
            {
                if (!MainPage.mymainObject.bSaleAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bSaleEdit)
                    btnEdit.Enabled = btnDelete.Enabled = false;
                if (!MainPage.mymainObject.bSaleView)
                    txtBillNo.Enabled = false;
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }

                
        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,double dInsuranceAmt,double dServiceAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0, dTaxPer=0, dServiceAmount =0,dOtherChargesAmt=0;//,dInsuranceAmt=0
            string _strTaxType = "";
            try
            {
                dgrdTax.Rows.Clear();
                if (MainPage._bTaxStatus && txtSalesType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    DataTable _dt = dba.GetSaleTypeDetails(txtSalesType.Text, "SALES");
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow row = _dt.Rows[0];
                        string strTaxationType = Convert.ToString(row["TaxationType"]);
                        _strTaxType = "EXCLUDED";
                        if (strTaxationType == "ITEMWISE")
                        {
                            if (Convert.ToBoolean(row["TaxIncluded"]))
                            {
                                _strTaxType = "INCLUDED";
                                dOtherAmt += dServiceAmt;
                            }

                            dTaxPer = dba.ConvertObjectToDouble(txtTaxPer.Text);
                            if (dOtherAmt != 0 && MainPage.startFinDate >= Convert.ToDateTime("04/01/2021"))
                                dTaxPer = 18;

                            //dInsuranceAmt = dba.ConvertObjectToDouble(txtDiscAmt.Text);

                            string strQuery = "", strSubQuery = "", strGRSNo = "", strSSQuery="";
                            double dDisStatus = 0;

                            strGRSNo = txtBillCode.Text + " " + txtBillNo.Text; 

                            double dRate = 0,dQty = 0, dAmt = 0,dBasicAmt=0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dRate = dba.ConvertObjectToDouble(rows.Cells["rate"].Value);
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["amount"].Value);
                                dBasicAmt = dba.ConvertObjectToDouble(rows.Cells["netAmt"].Value);
                                dQty = dba.ConvertObjectToDouble(rows.Cells["qty"].Value);

                                if (dRate > 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dBasicAmt + " )*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + ">0 ";
                                    if (!txtBillCode.Text.Contains("PTN"))
                                    {
                                        if (MainPage.strSoftwareType == "AGENT")
                                            strQuery += " UNION ALL Select '' as ID, '' as HSNCode,0 as Quantity, (((((Amount*(100+" + dDisStatus + ")/100.00)*TaxRate/100.00)*3)/100.00)) Amount,0 TaxRate from (Select (CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end) Amount,TaxRate from( Select " + dQty + " as Quantity,ROUND((((" + dAmt + ")*(100 - " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + " >0 )_Sales)_Sales ";
                                    }

                                    if (strSSQuery != "")
                                        strSSQuery += " Union ALL ";
                                    strSSQuery += " Select (((((Amount*(100+" + dDisStatus + ")/100.00)*TaxRate/100.00)*3)/100.00)) Amount from (Select (CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((Amount*100)/(100+TaxRate)) else Amount end) Amount,TaxRate from( Select " + dQty + " as Quantity,ROUND((((" + dAmt + ")*(100 - " + dDisStatus + "))/ 100.00),2)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + " * 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dRate + "* 100) / (100 + TaxRate)) else " + dRate + " end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - 0) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + rows.Cells["itemName"].Value + "' and " + dAmt + " >0 )_Sales)_Sales ";
                                }
                            }                     
                          
                            if (dInsuranceAmt != 0 && txtOtherPerSign.Text!="-")
                                dTaxPer = 18;
                           
                            if (dOtherAmt != 0)
                            {
                                if (strQuery != "")
                                    strQuery += " UNION ALL ";
                                strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dOtherAmt + " Amount,"+ dTaxPer + " as TaxRate ";
                            }

                            if (strQuery != "")
                            {
                                strQuery = " Select SUM(TaxableAmt)TaxableAmt,SUM(ROUND(Amt,4)) as Amt,SUM(ROUND(Amt,2)) as TaxAmt,TaxRate,((" + dOtherAmt + "*TaxRate)/ 100) OtherChargesAmt from (Select HSNCode,SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' and Qty>0 then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                         + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by HSNCode,TaxRate)_Sales  Group by TaxRate ";

                                strQuery += strSubQuery;
                                if (!txtBillCode.Text.Contains("PTN") && strSSQuery != "")
                                {
                                    if (MainPage.strSoftwareType == "AGENT")
                                        strSSQuery = " Select SUM(Amount)Amt from ( " + strSSQuery + ") _Sales ";
                                    else
                                        strSSQuery = "Select 0  as Amt";
                                }
                                else
                                    strSSQuery = "Select 0  as Amt";

                                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery + strSSQuery);
                                if (ds.Tables.Count > 0)
                                {
                                    DataTable dt = ds.Tables[0];
                                    double dMaxRate = 0, dTTaxAmt = 0;
                                    dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        DataRow[] _rows = dt.Select("TaxableAmt=" + dOtherAmt);
                                        if(_rows.Length>0)
                                        dOtherChargesAmt = dba.ConvertObjectToDouble(_rows[0]["Amt"]);
                                    }
                                        
                                    if (ds.Tables[1].Rows.Count > 0)
                                        dServiceAmount = DataBaseAccess.ConvertObjectToDoubleStatic(ds.Tables[1].Rows[0][0]);

                                    dTaxAmt = dTTaxAmt;
                                    if (dOtherAmt == 0)
                                        dTaxPer = dMaxRate;
                                   // pnlTax.Visible = true;
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
                           // pnlTax.Visible = true;
                        }
                        else
                            txtTaxAmt.Text = txtTaxPer.Text = "0.00";
                    }
                }
                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bSaleAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bSaleEdit)
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
            if (!txtBillCode.Text.Contains("PTN"))
            {
                if (MainPage.strSoftwareType == "AGENT")
                {
                    txtSign.Text = "+";
                    txtOtherAmount.Text = dServiceAmount.ToString("N2", MainPage.indianCurancy);
                }
            }

            if (_strTaxType == "INCLUDED")
                dTaxAmt = dOtherChargesAmt;
            return dTaxAmt;
        }


        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {
            if(_bUpdateStatus)
            {
                if (txtIRNo.Text != "")
                {
                    double dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text);
                    if (dOldNetAmt != dNetAmt)
                    {
                        MessageBox.Show("E-Invoice has been generated, Please cancel EInvoice and remove IRN from this bill !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                            return false;
                    }
                }
            }
            DateTime _date = dba.ConvertDateInExactFormat(txtDate.Text);
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtSalesType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from SaleServiceBook Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus,DATEDIFF(dd,'" + _date.ToString("MM/dd/yyyy") + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))) BillDays  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSalesParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSalesParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtSalesParty.Text || dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || _bUpdateStatus)
                    {
                        if (MainPage.mymainObject.bFullEditControl || (dba.ConvertDateInExactFormat(txtDate.Text).AddDays(3) > MainPage.currentDate))
                        {
                            bool iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);

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
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    if (ValidateOtherValidation(true))
                    {
                        if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtBillNo.Text != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {

                                string strQuery = " Update OB SET OB.AdjustedQty=(OB.AdjustedQty-SBS.Qty),Status=(Case When (CAST(OB.Quantity as Money)-(OB.AdjustedQty+ISNULL(OB.CancelQty,0)- SBS.Qty))>0 Then 'PENDING' else 'CLEAR' end) from  OrderBooking OB CROSS APPLY (Select SBS.BillCode,SBS.BillNo,SBS.Qty from SalesBookSecondary SBS Where (OB.OrderCode+' '+CAST(OB.OrderNo as varchar)+' '+OB.NumberCode)=SBS.SONumber)SBS Where SBS.[BillCode]='" + txtBillCode.Text + "' and SBS.[BillNo]=" + txtBillNo.Text
                                                + " Delete from SalesBook Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from SalesBookSecondary Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text
                                                + " Delete from [BalanceAmount]  Where [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' and [AccountStatus] in ('SALES A/C','DUTIES & TAXES')  "
                                                + " Delete from [dbo].[GSTDetails] Where [BillType]='SALES' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                                + " Delete from StockMaster Where BillType='SALES' and BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                                + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                                + " ('SALES','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

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
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
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
                                //txtPackingType.Text = strAllData[1];
                                txtPackingAmt.Text = strAllData[2];
                            }
                            else
                            {
                                txtCartonSize.Text = "";
                                txtPackingAmt.Text = MainPage.dPackingAmount.ToString("0");
                            }
                        }
                        else
                        {
                            txtCartonSize.Text = "";
                            txtPackingAmt.Text = MainPage.dPackingAmount.ToString("0");
                        }
                        CalculateAllAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SetPackingTaxAmt()
        {
            try
            {
                double dPackingAmt = 0;

                double dCase = dba.ConvertObjectToDouble(txtNoofCases.Text), _dPackingAmt = dba.ConvertObjectToDouble(txtPackingAmt.Text);
                dPackingAmt = (dCase * _dPackingAmt);

                txtPackingAmt.Text = dPackingAmt.ToString("0.00");
            }
            catch { }
        }

        private void txtPostage_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtPostage.Text == "")
                    txtPostage.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtGreenTax_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtGreenTax.Text == "")
                    txtGreenTax.Text = "0.00";
                CalculateAllAmount();
            }
        }

        private void txtLRDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, false, false);
            }
        }

        private void txtPackingDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void txtSalesParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtSalesParty.Text);
        }

        private void txtSubParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenSubPartyMaster(txtSubParty.Text);
        }

        private void txtTransport_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenTransportMaster(txtTransport.Text);
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

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void AskForPrint()
        {
            try
            {

                DialogResult _result = MessageBox.Show("Are you want to print Sale Bill ?", "Print Sale Service Bill", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (_result == DialogResult.Yes)
                    SetSignatureInBill(true, false,true);
            }
            catch
            {
            }
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

                    if (File.Exists(strPath))
                        File.Delete(strPath);
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
                    bool _bstatus = GSTPrintAndPreview(_bPStatus, strFileName,true, _dscVerified);
                    if (_bstatus)
                    {
                        if (MainPage.strSoftwareType == "AGENT")
                        {
                            if (!_dscVerified)
                            {
                                string strSignPath = MainPage.strServerPath.Replace(@"\NET", "") + "\\Signature\\sign.pfx";

                                PDFSigner _objSigner = new PDFSigner();
                                bool _bFileStatus = _objSigner.SetSign(strFileName, strPath, strSignPath);
                                if (!_bFileStatus)
                                    strPath = "";
                                if (_bPStatus && _bFileStatus)
                                    System.Diagnostics.Process.Start(strPath);

                                File.Copy(strFileName, strPath);
                                if (_bPStatus)
                                    System.Diagnostics.Process.Start(strPath);
                            }
                            else
                                File.Copy(strFileName, strPath);
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

        private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC, bool _dscVerified)
        {
           // string strValue = "0";
            //if (_pstatus)
            //{
            //    strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "1", 400, 300);
            //    if (strValue == "" || strValue == "0")
            //    {
            //        return false;
            //    }
            //}

            DataTable _dtGST = null, _dtSalesAmt = null;
            bool _bIGST = false;
            DataTable dt = dba.CreateOnlineSalesBookRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _dscVerified, _pstatus);
            if (dt.Rows.Count > 0)
            {
                if (!MainPage._bTaxStatus)
                {

                    Reporting.SaleBookRetailReport objOL_salebill = new Reporting.SaleBookRetailReport();
                    objOL_salebill.SetDataSource(dt);
                    objOL_salebill.Subreports[0].SetDataSource(_dtSalesAmt);
                   // objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                    objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    if (strPath != "")
                    {
                        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
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
                                objOL_salebill.SetDataSource(dt);
                                objOL_salebill.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                            }
                            if (objInvoice._transportCopy > 0)
                            {
                                SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                                objOL_salebill.SetDataSource(dt);
                                objOL_salebill.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                            }
                            if (objInvoice._supplierCopy > 0)
                            {
                                SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                                objOL_salebill.SetDataSource(dt);
                                objOL_salebill.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                            }
                        }
                        else
                        {
                            Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                            objReport.myPreview.ReportSource = objOL_salebill;                          
                            objReport.ShowDialog();
                        }
                    }
                }
                else
                {
                    if (!_bIGST)
                    {
                        if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
                        {
                            Reporting.SaleBookRetailReport_CSGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_DSC();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            if (strPath != "" && !_pstatus)
                            {
                                objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
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
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                                    }
                                    if (objInvoice._transportCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                                    }
                                    if (objInvoice._supplierCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //objReport.myPreview.ShowExportButton = false;
                                    //objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            Reporting.SaleBookRetailReport_CSGST objOL_salebill = new Reporting.SaleBookRetailReport_CSGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                            if (strPath != "" && !_pstatus)
                            {
                                objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
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
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                                    }
                                    if (objInvoice._transportCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                                    }
                                    if (objInvoice._supplierCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    // objReport.myPreview.ShowExportButton = false;
                                    //objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((MainPage.strCompanyName.Contains("SARAOGI") || MainPage.strCompanyName.Contains("STYLO")) && MainPage.strSoftwareType == "AGENT")
                        {
                            Reporting.SaleBookRetailReport_IGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_IGST_DSC();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                            if (strPath != "" && !_pstatus)
                            {
                                objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
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
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                                    }
                                    if (objInvoice._transportCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                                    }
                                    if (objInvoice._supplierCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //objReport.myPreview.ShowExportButton = false;
                                    // objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            Reporting.SaleBookRetailReport_IGST objOL_salebill = new Reporting.SaleBookRetailReport_IGST();
                            objOL_salebill.SetDataSource(dt);
                            objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                            objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                            objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

                            if (strPath != "" && !_pstatus)
                            {
                                objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
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
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._originalCopy, true, 0, 0);
                                    }
                                    if (objInvoice._transportCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Transporter Copy", objInvoice._tLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._transportCopy, true, 0, 0);
                                    }
                                    if (objInvoice._supplierCopy > 0)
                                    {
                                        SetSubTitleInDataTable(ref dt, "Supplier Copy", objInvoice._sLetterHead);
                                        objOL_salebill.SetDataSource(dt);
                                        objOL_salebill.PrintToPrinter(objInvoice._supplierCopy, true, 0, 0);
                                    }
                                }
                                else
                                {
                                    Reporting.ShowReport objReport = new Reporting.ShowReport("SALES BOOK REPORT PREVIEW");
                                    objReport.myPreview.ReportSource = objOL_salebill;
                                    //  objReport.myPreview.ShowExportButton = false;
                                    //  objReport.myPreview.ShowPrintButton = false;
                                    objReport.ShowDialog();
                                }
                            }
                        }
                    }
                }
            }

            return false;
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


        //private bool GSTPrintAndPreview(bool _pstatus, string strPath, bool _bDSC)
        //{
        //    string strValue = "0";
        //    if (_pstatus)
        //    {
        //        strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT ! ", "Number of Copies", "2", 400, 300);
        //        if (strValue == "" || strValue == "0")
        //        {
        //            return false;
        //        }
        //    }

        //    DataTable _dtGST = null, _dtSalesAmt = null;
        //    bool _bIGST = false;
        //    DataTable dt = dba.CreateOnlineSalesBookRetailDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, _bDSC);
        //    if (dt.Rows.Count > 0)
        //    {
        //        if (!_bIGST)
        //        {
        //            if (_bDSC && strPath != "")
        //            {
        //                Reporting.SaleBookRetailReport_CSGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_DSC();
        //                objOL_salebill.SetDataSource(dt);
        //                objOL_salebill.Subreports[0].SetDataSource(_dtGST);
        //                objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
        //                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                if (strPath != "")
        //                {
        //                    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                Reporting.SaleBookRetailReport_CSGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_CSGST_DSC();
        //                objOL_salebill.SetDataSource(dt);
        //                objOL_salebill.Subreports[0].SetDataSource(_dtGST);
        //                objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
        //                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                if (strPath != "")
        //                {
        //                    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
        //                    return true;
        //                }
        //                else
        //                {
        //                    if (_pstatus)
        //                    {
        //                        if (strValue != "" && strValue != "0")
        //                        {
        //                            int nCopy = Int32.Parse(strValue);
        //                            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES SERVICE BOOK REPORT PREVIEW");
        //                        objReport.myPreview.ReportSource = objOL_salebill;
        //                        objReport.myPreview.ShowExportButton = false;
        //                        objReport.myPreview.ShowPrintButton = false;
        //                        objReport.ShowDialog();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (_bDSC && strPath != "")
        //            {
        //                Reporting.SaleBookRetailReport_IGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_IGST_DSC();
        //                objOL_salebill.SetDataSource(dt);
        //                objOL_salebill.Subreports[0].SetDataSource(_dtGST);
        //                objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
        //                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                if (strPath != "")
        //                {
        //                    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                Reporting.SaleBookRetailReport_IGST_DSC objOL_salebill = new Reporting.SaleBookRetailReport_IGST_DSC();
        //                objOL_salebill.SetDataSource(dt);
        //                objOL_salebill.Subreports[0].SetDataSource(_dtGST);
        //                objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
        //                objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
        //                if (strPath != "")
        //                {
        //                    objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
        //                    return true;
        //                }
        //                else
        //                {
        //                    if (_pstatus)
        //                    {
        //                        // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
        //                        if (strValue != "" && strValue != "0")
        //                        {
        //                            int nCopy = Int32.Parse(strValue);
        //                            objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Reporting.ShowReport objReport = new Reporting.ShowReport("SALES SERVICE BOOK REPORT PREVIEW");
        //                        objReport.myPreview.ReportSource = objOL_salebill;
        //                        objReport.myPreview.ShowExportButton = false;
        //                        objReport.myPreview.ShowPrintButton = false;
        //                        objReport.ShowDialog();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}

        private void SendEmailToSalesParty(bool _bStatus, ref string strMobileNo, ref string strFilePath)
        {
            try
            {
                if (chkEmail.Checked || _bStatus)
                {
                    string strPath = SetSignatureInBill(false, false,true), strEmailID = "", strWhatsAppNo = "";
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
            string strMsgType = "", _strFileName = txtBillCode.Text.Replace("18-19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text, strWhastappMessage = "";
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
            string _strFileName = "Bilty_" + txtBillCode.Text.Replace("18 -19/", "").Replace("19-20/", "").Replace("20-21/", "").Replace("21-22/", "").Replace("22-23/", "") + "_" + txtBillNo.Text + ".pdf", strMessage = "", strBranchCode = txtBillCode.Text;
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
            if (!_bStatus)
            {
                DialogResult _updateResult = MessageBox.Show("Unable to send whatsapp message due to internet connectivity, Please retry !!", "Warning", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
                if (_updateResult == DialogResult.Retry)
                    _bStatus = dba.UploadSaleBillPDFFile(strPath, _strFileName, strBranchCode);
            }

            if (_bStatus)
            {
                string strResult = WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMsgType, strWhastappMessage, "", strFilePath);
                if (strResult != "")
                    MessageBox.Show("Thank you ! Whatsapp messsage sent successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                WhatsappClass.SendWhatsappWithIMIMobile(strMobileNo, strMType, strTextMsg, "", "");
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
                            strSubMsg += ", LR No. " + txtLRNumber.Text + " (" + txtLRDate.Text + ")";
                        if (txtRemark.Text != "")
                            strSubMsg += ", Note : " + txtRemark.Text;

                        if (btnAdd.Text == "&Save")
                            strMessage = "M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalQty.Text +" " + strSubMsg + strBalance;
                        else
                            strMessage = "Alert : M/s " + strName + ", B.N. :  " + txtBillCode.Text + " " + txtBillNo.Text + " DT : " + txtDate.Text + " AMT : " + lblNetAmt.Text.Replace(",", "") + " Pcs. : " + lblTotalQty.Text + " " + strSubMsg + strBalance;

                     
                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }
                }
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
                        strMessage = "Update ! A/c : " + txtSalesParty.Text + ", we have update your sale bill no : <b>" + txtBillCode.Text + " " + txtBillNo.Text + " </b>, and attached with this mail, please find it.";
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

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
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

        private void txtNoofCases_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                    SetPackingTaxAmt();
            }
            catch { }
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

        private bool ValidateStock()
        {
            if (!MainPage._bTaxStatus && txtImportData.Text != "")
                return true;
            else
            {
                bool _bStatus = false;
                DataTable _dt = GenerateDistinctItemName();
                _bStatus = CheckQtyAvalability(_dt);
                if (MainPage.startFinDate >= Convert.ToDateTime("04/01/2021"))
                    _bStatus = CheckHSNCode(_dt);

                if (!_bStatus && MainPage.strUserRole.Contains("SUPERADMIN"))
                    _bStatus = true;
                return _bStatus;
            }
        }

        private DataTable GenerateDistinctItemName()
        {
            DataTable _dt = new DataTable();
            try
            {               
                _dt.Columns.Add("ItemName", typeof(String));
                _dt.Columns.Add("Variant1", typeof(String));
                _dt.Columns.Add("Variant2", typeof(String));
                _dt.Columns.Add("Variant3", typeof(String));
                _dt.Columns.Add("Variant4", typeof(String));
                _dt.Columns.Add("Variant5", typeof(String));
                _dt.Columns.Add("MRP", typeof(String));
                _dt.Columns.Add("Qty", typeof(String));

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] _rows = _dt.Select("ItemName='" + row.Cells["itemName"].Value + "' and Variant1='" + row.Cells["variant1"].Value + "' and Variant2='" + row.Cells["variant2"].Value + "' and ISNULL(Variant3,'')='" + row.Cells["variant3"].Value + "' and ISNULL(Variant4,'')='" + row.Cells["variant4"].Value + "' and ISNULL(Variant5,'')='" + row.Cells["variant5"].Value + "'  and ISNULL(MRP,0)='" + row.Cells["mrp"].Value + "' ");
                    if (_rows.Length > 0)
                    {
                        double dOQty = dba.ConvertObjectToDouble(_rows[0]["Qty"]),dQty=dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                        _rows[0]["Qty"] = dOQty + dQty;
                    }
                    else
                    {
                        DataRow _row = _dt.NewRow();
                        _row["ItemName"] = row.Cells["itemName"].Value;
                        _row["Variant1"] = row.Cells["variant1"].Value;
                        _row["Variant2"] = row.Cells["variant2"].Value;
                        _row["Variant3"] = row.Cells["variant3"].Value;
                        _row["Variant4"] = row.Cells["variant4"].Value;
                        _row["Variant5"] = row.Cells["variant5"].Value;
                        _row["MRP"] = row.Cells["mrp"].Value;
                        _row["Qty"] = row.Cells["qty"].Value;
                        _dt.Rows.Add(_row);
                    }
                }
            }
            catch { }
            return _dt;
        }

        private bool CheckHSNCode(DataTable _dt)
        {
            DataTable table = _dt.DefaultView.ToTable(true, "ItemName");
            string strItemName = "";
            foreach(DataRow row in table.Rows)
            {
                strItemName += "'"+Convert.ToString(row["itemName"])+"',";
            }
            if (strItemName != "")
            {
                strItemName = strItemName.Substring(0,strItemName.Length - 1);
                string strQry = " SELECT distinct STUFF((SELECT distinct ',' + ISNULL(IM.ItemName,'') FROM ItemGroupMaster IGM INNER JOIN ItemS IM on IGM.GroupName = IM.GroupName WHERE ItemName IN (" + strItemName + ") AND Len(HSNCode) < 6 FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)') ,1,1,'')ItemNames";
                DataTable _dTable = dba.GetDataTable(strQry);
                if (_dTable.Rows.Count > 0)
                {
                    string ItemNames = Convert.ToString(_dTable.Rows[0]["ItemNames"]);
                    if (ItemNames != "")
                    {
                        SetWrongHSNColor(ItemNames);
                        return false;
                    }
                }
            }

            return true;
        }


        private bool SetWrongHSNColor(string strItemName)
        {
            string[] ItemNameArr = strItemName.Split(',');
            bool _bStatus = true;
            try
            {
                foreach (DataGridViewRow _row in dgrdDetails.Rows)
                {
                   string ItemName = Convert.ToString(_row.Cells["itemName"].Value);
                    if (Array.Exists(ItemNameArr, el => el == ItemName.Trim()))
                    {
                        _row.DefaultCellStyle.BackColor = Color.Gold;
                        _bStatus = false;
                    }
                }

                if(!_bStatus)
                {
                    MessageBox.Show("Sorry ! Yellow  mentioned color have less than 6 digit hsn code. Please set it atleast 6 digit hsn code", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _bStatus = false;
            }
            return _bStatus;
        }



        private bool CheckQtyAvalability(DataTable dt)
        {
            string strQuery = "",strSubQuery="",strNCQuery="";
            try
            {
                //SetGridViewBackGroundColor();
                foreach (DataRow row in dt.Rows)
                {
                    strSubQuery = "";
                    if (!txtBillCode.Text.Contains("PTN"))
                    {
                        if (MainPage.strSoftwareType == "AGENT")
                            strSubQuery = " and MRP=" + ConvertObjectToDouble(row["MRP"]) + " ";
                    }

                    if (strQuery != "")
                    {
                        strQuery += " UNION ALL ";
                        strNCQuery += " UNION ALL ";
                    }

                    strQuery += " Select ItemName,Variant1,Variant2,SUM(PQty+SQty) Qty from ( "
                         + " Select ItemName, Variant1, Variant2, SUM(Qty)PQty, 0 SQty from StockMaster Where ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " and BillType in ('OPENING', 'PURCHASE', 'SALERETURN','STOCKIN') Group by ItemName,Variant1,Variant2 UNION ALL "
                         + " Select ItemName,Variant1,Variant2,0 PQty,-SUM(Qty) SQty from StockMaster Where ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " and BillType in ('SALES','PURCHASERETURN','STOCKOUT') Group by ItemName,Variant1,Variant2 UNION ALL "
                         + " Select ItemName,Variant1,Variant2,SUM(Qty) PQty,0 SQty from StockMaster Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " Group by ItemName,Variant1,Variant2 "
                         + " UNION ALL Select '" + row["ItemName"] + "' as ItemName,'" + row["Variant1"] + "' as Variant1,'" + row["Variant2"] + "' as Variant2,0 as PQty, -" + row["Qty"] + " Qty )Stock Group by ItemName, Variant1, Variant2 ";

                    strNCQuery += " Select ItemName,Variant1,Variant2,SUM(PQty+SQty) Qty from ( "
                             + " Select ItemName, Variant1, Variant2, SUM(Qty)PQty, 0 SQty from StockMaster Where ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " and BillType in ('OPENING', 'PURCHASE', 'SALERETURN','STOCKIN') Group by ItemName,Variant1,Variant2 UNION ALL "
                             + " Select ItemName,Variant1,Variant2,0 PQty,-SUM(Qty) SQty from StockMaster Where ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " and BillType in ('SALES','PURCHASERETURN','STOCKOUT') Group by ItemName,Variant1,Variant2 UNION ALL "
                             + " Select ItemName,Variant1,Variant2,SUM(Qty) PQty,0 SQty from StockMaster Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and Variant2='" + row["Variant2"] + "' " + strSubQuery + " Group by ItemName,Variant1,Variant2 "
                             + " )Stock Group by ItemName, Variant1, Variant2 ";

                }

                if (strQuery != "")
                {
                    DataTable _dTable = null, _dtNC = null; 
                    if (MainPage.strSoftwareType == "AGENT")
                    {
                        //strQuery = " Select * from ( " + strQuery + " )StockMaster Where Qty<0 ";
                        _dTable = dba.GetDataTable(strQuery);
                    }
                    else
                    {
                        _dTable = dba.GetDataTable(strQuery);                     

                       // if (MainPage._bTaxStatus)
                            _dtNC = SearchDataOther.GetDataTable_NC(strNCQuery);
                        //else
                        //    _dtNC = SearchDataOther.GetDataTable_TC(strNCQuery);

                        if (_dtNC != null && _dtNC.Rows.Count > 0)
                            _dTable.Merge(_dtNC,true);
                    }

                    DataTable _dtStock = Generate_Stock_Table(_dTable);
                    bool _bStatus = SetOutOfStockColor(_dtStock);
                    if (!_bStatus)
                    {                       
                        lblMsg.Text = "Red color item is out of stock ! Unable to generate sale bill !!";
                        lblMsg.ForeColor = Color.Red;
                        return false;
                    }
                    else
                    {
                        lblMsg.Text = "";
                        lblMsg.ForeColor = Color.DarkGreen;
                        return true;
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }

        private DataTable Generate_Stock_Table(DataTable dt)
        {           
            DataTable dTable = dt.DefaultView.ToTable(true, "ItemName", "Variant1", "Variant2");
            dTable.Columns.Add("Quantity", typeof(String));
            object objValue = "";
            foreach (DataRow row in dTable.Rows)
            {
                objValue = dt.Compute("Sum(Qty)", "ItemName='" + row["ItemName"] + "' and Variant1='" + row["Variant1"] + "' and [Variant2]='" + row["Variant2"] + "' ");
                row["Quantity"] = objValue;
            }
            return dTable;
        }

        private bool SetOutOfStockColor(DataTable dt)
        {
            string strItemName = "", strVariant1 = "", strVariant2 = "";
            double dQty = 0;
            bool _bStatus = true;
            try
            {             

                DataRow[] rows = null;
                foreach (DataGridViewRow _row in dgrdDetails.Rows)
                {
                    strItemName = Convert.ToString(_row.Cells["itemName"].Value);
                    strVariant1 = Convert.ToString(_row.Cells["variant1"].Value);
                    strVariant2 = Convert.ToString(_row.Cells["variant2"].Value);
                    rows = dt.Select("ItemName = '" + strItemName + "' and Variant1 = '" + strVariant1 + "' and[Variant2] = '" + strVariant2 + "' ");

                    if (rows.Length > 0)
                    {
                        dQty = dba.ConvertObjectToDouble(rows[0]["Quantity"]);
                        if (dQty < 0)
                        {
                            _row.DefaultCellStyle.BackColor = Color.Tomato;
                            _bStatus = false;
                        }
                        else
                            _row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                    }
                    else
                        _row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _bStatus = false;
            }
            return _bStatus;
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
                    DataBaseAccess.ShowBiltyPDFFiles(txtBillCode.Text, txtBillNo.Text);
                }
            }
            catch
            {
            }
            btnShowBilty.Enabled = true;
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

        private void CalculateSpecialDiscount()
        {
            try
            {
                double dSpclPer = 0, dSpclAmt = 0, dMRP = 0, _dMRP = 0,dAmt =0, dDisPer=0, dRate = 0, dQty = 0, dDisc=0, dOCharges=0;
                dSpclPer = dba.ConvertObjectToDouble(txtSpclDisPer.Text);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    dRate = 0;
                    _dMRP =dMRP = dba.ConvertObjectToDouble(row.Cells["mrp"].Value);
                    dDisPer = dba.ConvertObjectToDouble(row.Cells["disPer"].Value);
                    dQty = dba.ConvertObjectToDouble(row.Cells["qty"].Value);
                    if (dSpclPer != 0 && dMRP != 0)
                    {
                        dSpclAmt += (dMRP * dSpclPer) / 100.00;
                        _dMRP = dMRP * (100.00 - dSpclPer) / 100.00;
                    }
                    else
                        _dMRP = dMRP;

                    if ((dDisPer != 0 || dSpclPer != 0) && _dMRP != 0)
                        dRate = _dMRP * (100.00 + (dDisPer)) / 100.00;
                    if (dRate == 0)
                        dRate = _dMRP;

                    dAmt = (dRate * dQty);
                    row.Cells["rate"].Value = dRate.ToString("N2", MainPage.indianCurancy);
                    row.Cells["amount"].Value = dAmt.ToString("N2", MainPage.indianCurancy);

                    dDisc = ConvertObjectToDouble(row.Cells["disc"].Value);
                    dOCharges = ConvertObjectToDouble(row.Cells["otherCharges"].Value);
                    row.Cells["netAmt"].Value = (dAmt - dDisc + dOCharges).ToString("N2", MainPage.indianCurancy);
                }

                txtSplDisAmt.Text= dSpclAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
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
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchDataOther objSearch = new SearchDataOther("_BillNo", "", "SEARCH SALE BILL NO", e.KeyCode, false);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtImportData.Text = objSearch.strSelectedData;
                            GetDataFromLocal();
                        }
                        else
                            txtImportData.Text = "";
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
            if (txtImportData.Text != "" &&  (btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
            {
                BindRecordWithControlWithImport();
            }
        }

        private void BindRecordWithControlWithImport()
        {
            try
            {
                strOldPartyName = "";
                string strQuery = " Select *,CONVERT(varchar,Date,103)BDate,CONVERT(varchar,ISNULL(PackingDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)PDate,CONVERT(varchar,ISNULL(LrDate,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))),103)LDate,ISNULL(PAgent,'DIRECT') PAgent,(CASE WHEN SalesType Like('%INCLUDE%') then (GrossAmt-TaxAmt) else GrossAmt end)GAmt from SalesBook SB  OUTER APPLY (Select Top 1 (Description_1+' '+Name)PAgent from SupplierMaster SM Where (SM.AreaCode+SM.AccountNo)=SB.Description_1)SM Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "' "
                                + " Select * from SalesBookSecondary SBS OUTER APPLY ( Select Top 1 _IS.Description as BarCode from Items _IM inner join ItemSecondary _IS on _IM.BillCode=_IS.BillCode  and _IM.BillNo=_IS.BillNo Where _IM.ItemName=SBS.ItemName and _IS.Variant1=SBS.Variant1 and _IS.Variant2=SBS.Variant2 and [ActiveStatus]=1) _IM Where (BillCode+' '+CAST(BillNo as varchar))='" + txtImportData.Text + "'  order by SID ";
                DataSet ds = SearchDataOther.GetDataSet(strQuery);
                txtReason.Text = strOldLRNumber = "";
                pnlDeletionConfirmation.Visible = false;
                lblCreatedBy.Text = "";
                DataTable dt = null;
                if (ds.Tables.Count > 1)
                {
                    dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {

                            DataRow row = dt.Rows[0];

                            txtNoofCases.Text = Convert.ToString(row["NoOfCase"]);
                            strOldLRNumber = txtLRNumber.Text = Convert.ToString(row["LRNumber"]);
                            txtLRDate.Text = Convert.ToString(row["LDate"]);
                            txtTimeOfSupply.Text = Convert.ToString(row["LRTime"]);
                            txtPvtMarka.Text = Convert.ToString(row["PvtMarka"]);
                            txtPackerName.Text = Convert.ToString(row["PackerName"]);
                            txtPackingDate.Text = Convert.ToString(row["PDate"]);
                            txtCartonType.Text = Convert.ToString(row["CartonType"]);
                            txtCartonSize.Text = Convert.ToString(row["CartonSize"]);
                            txtRemark.Text = Convert.ToString(row["Remark"]);
                            txtTransport.Text = Convert.ToString(row["TransportName"]);
                            txtBStation.Text = Convert.ToString(row["Station"]);
                            txtPackingAmt.Text = Convert.ToString(row["PackingAmt"]);
                            txtSign.Text = Convert.ToString(row["OtherSign"]);
                            txtPetiType.Text = Convert.ToString(row["Description_2"]);
                            txtPetiAgent.Text = Convert.ToString(row["PAgent"]);
                            txtOtherAmount.Text = Convert.ToString(row["GAmt"]);
                            txtSign.Text = "-";
                            //double dSpclDisPer = dba.ConvertObjectToDouble(row["SpecialDscPer"]);
                            //if (dSpclDisPer != 0)
                            //    dSpclDisPer = (100 - dSpclDisPer);
                            //txtSpclDisPer.Text = dSpclDisPer.ToString("N2", MainPage.indianCurancy);
                        }
                    }
                }

                dt = ds.Tables[1];
                dgrdDetails.Rows.Clear();
                int rowIndex = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[rowIndex].Cells["srNo"].Value = rowIndex + 1;                      
                        dgrdDetails.Rows[rowIndex].Cells["barCode"].Value = row["BarCode"];                     
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
                        dgrdDetails.Rows[rowIndex].Cells["disc"].Value = row["Disc"];
                        dgrdDetails.Rows[rowIndex].Cells["otherCharges"].Value = row["OCharges"];
                        dgrdDetails.Rows[rowIndex].Cells["netAmt"].Value = Convert.ToDouble(row["BasicAmt"]).ToString("N2", MainPage.indianCurancy);
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];

                        rowIndex++;
                    }
                }

                CalculateAllAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private void txtPetiAgent_DoubleClick(object sender, EventArgs e)
        {
            if (txtPetiAgent.Text != "" && txtPetiAgent.Text != "DIRECT")
                DataBaseAccess.OpenPartyMaster(txtPetiAgent.Text);
        }

        private void btnCreatePDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnCreatePDF.Enabled = false;
                DialogResult result = MessageBox.Show("ARE YOU SURE YOU WANT TO CREATE PDF ? ", "CONFIRMATION", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string strPath = SetSignatureInBill(false, true, true);
                    if (strPath != "")
                        MessageBox.Show("Thank you ! PDF generated on " + strPath, "PDF generated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch
            {
            }
            btnCreatePDF.Enabled = true;
        }

        private void txtOtherPerSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                e.Handled = true;
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

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 4)
                {
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        string strDesignName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                        if (strDesignName != "")
                        {
                            DesignMaster objDesign = new DesignMaster(strDesignName);
                            objDesign.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objDesign.ShowInTaskbar = true;
                            objDesign.ShowDialog();
                        }
                    }
                }
            }
            catch { }
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

                                var _success = dba.GenerateEWayBillJSON(strBillNo, "TRADING");
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
                                DataTable _dt = dba.CreateTradingWayBillDataTable(txtBillCode.Text, txtBillNo.Text);
                                if (_dt.Rows.Count > 0)
                                {
                                    Reporting.WayBillReport objReport = new Reporting.WayBillReport();
                                    objReport.SetDataSource(_dt);

                                    if (MainPage._PrintWithDialog)
                                        dba.PrintWithDialog(objReport);
                                    else
                                    {
                                        Reporting.ShowReport objShow = new Reporting.ShowReport("WAY BILL PREVIEW");
                                        objShow.myPreview.ReportSource = objReport;
                                        objShow.myPreview.ShowPrintButton = true;
                                        objShow.myPreview.ShowExportButton = true;
                                        objShow.ShowDialog();
                                    }

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

        private void txtTaxPer_DoubleClick(object sender, EventArgs e)
        {
            pnlTax.Visible = !pnlTax.Visible;
        }

        private void btnEInvoice_Click(object sender, EventArgs e)
        {
            btnEInvoice.Enabled = false;
            try
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
                                var _success = dba.GenerateEInvoiceJSON_SaleBook(true,strBillNo, "TRADING");
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
            catch { }
            btnEInvoice.Enabled = true;
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
                    row["Quantity"] = lblTotalQty.Text;
                    _dt.Rows.Add(row);
                }
            }
            catch { }
            return _dt;
        }

        private string CreatePDFFile()
        {
            string strPath = MainPage.strServerPath + "\\PDF Files\\" + MainPage.strCompanyName + "\\Sales Bill", strFileName = strPath + "\\" + txtBillNo.Text + ".pdf";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);

           
                strFileName = SetSignatureInBill(false, true, true);
          
            return strFileName;
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
                        txtPetiType.Text = objSearch.strSelectedData;
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void SetGridViewBackGroundColor()
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (row.DefaultCellStyle.BackColor == Color.Tomato)
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                }
            }
            catch { }
        }
    }
}
