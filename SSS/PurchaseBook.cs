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
    public partial class PurchaseBook : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "";
        double dOldNetAmt = 0;
        bool newStatus = false;
        public PurchaseBook()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData(true);
        }

        public PurchaseBook(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData(false);
            newStatus = bStatus;          
        }

        public PurchaseBook(string strCode,string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);
        }

        private void GetStartupData(bool bStatus)
        {
            try
            {
                string strQuery = " Select PBillCode,GReceiveCode,FreightDhara,TaxDhara,(Select ISNULL(MAX(BillNo),0) from PurchaseRecord Where BillCode=PBillCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    txtBillCode.Text = Convert.ToString(dt.Rows[0]["PBillCode"]);
                    txtGRSCode.Text = Convert.ToString(dt.Rows[0]["GReceiveCode"]);

                    txtFreightPer.Text = txtPackingPer.Text = Convert.ToString(dt.Rows[0]["FreightDhara"]);
                    txtTaxPer.Text = Convert.ToString(dt.Rows[0]["TaxDhara"]);
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

        private void PurchaseBook_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.Enter)
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
                }
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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
                if (txtBillCode.Text != "" && strSerialNo != "")
                {
                    string strQuery = "";
                    strQuery += " Select *,dbo.GetFullName(SalePartyID) SParty,dbo.GetFullName(PurchasePartyID) PParty,Convert(varchar,BillDate,103)BDate,CONVERT(varchar,DATEADD(dd,Cast(PR.DueDays as int),BillDate),103) DDate,Round((CASE WHEN CAST(GrossAmt as money)!=0 then CAST(((CAST(OtherPer as Money)*100)/CAST(GrossAmt as Money)) as float) else '0' end),2) OtherPercentage,Round((CASE WHEN CAST(Amount as money)!=0 then CAST(((CAST(NetDiscount as Money)*100)/CAST(Amount as Money)) as float) else '0' end),2) NetDisPer,Round((CASE WHEN CAST(Freight as money)!=0 then CAST(((CAST(FreightDiscount as Money)*100)/CAST(Freight as Money)) as float) else '0' end),2) FrieghtDisPer "
                             + " ,Round((CASE WHEN CAST(Tax as money)!=0 then CAST(((CAST(TaxDiscount as Money)*100)/CAST(Tax as Money)) as float) else '0' end),2) TaxDisPer,Round((CASE WHEN CAST(Packing as money)!=0 then CAST(((CAST(PackingDiscount as Money)*100)/CAST(Packing as Money)) as float) else '0' end),2) PackingDisPer,ISNULL(SM.NormalDhara,0) NormalDhara,ISNULL(SM.SNDhara,0) SUPERDhara,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,BillDate))) LockType,InvoiceNo,Convert(varchar,InvoiceDate,103) InvDate from PurchaseRecord PR inner join SupplierMaster SM on (AreaCode+ CAST(AccountNo as varchar))=PR.PurchasePartyID  Where BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + ""
                             + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='PURCHASE' and BillCode='" + txtGRSCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        BindHeaderFooterRecord(ds.Tables[0]);
                        BindGSTDetailsWithControl(ds.Tables[1]);
                        EditOption();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Data in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void BindHeaderFooterRecord(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                txtBillNo.Text = Convert.ToString(row["BillNo"]);
                txtDate.Text = Convert.ToString(row["BDate"]);
                string strGRSNo = Convert.ToString(row["GRSNo"]);
                string[] strFullGRSNo = strGRSNo.Split(' ');
                if (strFullGRSNo.Length > 1)
                {
                    txtGRSCode.Text = strFullGRSNo[0];
                    txtGRSNo.Text = strFullGRSNo[1];
                }
                                
                txtPurchaseInvoiceNo.Text = Convert.ToString(row["InvoiceNo"]);
                txtInvoiceDate.Text = Convert.ToString(row["InvDate"]);
                txtDueDays.Text = Convert.ToString(row["DueDays"]);
                txtDueDate.Text = Convert.ToString(row["DDate"]);
                txtSupplierName.Text = Convert.ToString(row["PParty"]);
                txtNormalDhara.Text = Convert.ToString(row["NormalDhara"]);
                txtSuperNetDhara.Text = Convert.ToString(row["SUPERDhara"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtDis.Text = Convert.ToString(row["NetDisPer"]);
                txtDiscountAmt.Text = Convert.ToString(row["NetDiscount"]);
                lblGAmount.Text = Convert.ToString(row["GrossAmt"]);
                lblNAmount.Text = Convert.ToString(row["NetAmt"]);
                dOldNetAmt = dba.ConvertObjectToDouble(lblNAmount.Text);

                double dOtherPerAmt = 0, dOtherAmt = 0, dFrieght = 0, dTax = 0, dPacking = 0;
                dOtherPerAmt = dba.ConvertObjectToDouble(row["OtherPer"]);
                dOtherAmt = dba.ConvertObjectToDouble(row["Others"]);
                dFrieght = dba.ConvertObjectToDouble(row["FreightDiscount"]);
                dTax = dba.ConvertObjectToDouble(row["TaxDiscount"]);
                dPacking = dba.ConvertObjectToDouble(row["PackingDiscount"]);
                txtFreightPer.Text = txtPackingPer.Text = MainPage.dFreightDhara.ToString("0.00");
                txtTaxPer.Text = MainPage.dTaxDhara.ToString("0.00");
                txtFSign.Text = txtPSign.Text = txtTSign.Text = "-";
                if (dOtherPerAmt >= 0)
                {
                    txtSignPer.Text = "+";
                    txtOtherPer.Text = Convert.ToString(row["OtherPercentage"]);
                    txtOtherPerAmt.Text = dOtherPerAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    txtSignPer.Text = "-";
                    txtOtherPer.Text = Math.Abs(Convert.ToDouble(row["OtherPercentage"])).ToString("N2", MainPage.indianCurancy);
                    txtOtherPerAmt.Text = Math.Abs(dOtherPerAmt).ToString("N2", MainPage.indianCurancy);
                }
                if (dOtherAmt >= 0)
                {
                    txtSignAmt.Text = "+";
                    txtOtherAmt.Text = dOtherAmt.ToString("N2", MainPage.indianCurancy);
                }
                else
                {
                    txtSignAmt.Text = "-";
                    txtOtherAmt.Text = Math.Abs(dOtherAmt).ToString("N2", MainPage.indianCurancy);
                }
                if (dFrieght > 0)
                {
                    txtFSign.Text = "+";
                    txtFreightPer.Text = Convert.ToString(row["FrieghtDisPer"]);
                    lblFreight.Text = dFrieght.ToString("N2", MainPage.indianCurancy);
                }
                else if (dFrieght < 0)
                {
                    txtFreightPer.Text = Math.Abs(Convert.ToDouble(row["FrieghtDisPer"])).ToString("N2", MainPage.indianCurancy);
                    lblFreight.Text = Math.Abs(dFrieght).ToString("N2", MainPage.indianCurancy);
                }

                if (dTax > 0)
                {
                    txtTSign.Text = "+";
                    txtTaxPer.Text = Convert.ToString(row["FrieghtDisPer"]);
                    lblTax.Text = dTax.ToString("N2", MainPage.indianCurancy);
                }
                else if (dTax < 0)
                {
                    txtTaxPer.Text = Math.Abs(Convert.ToDouble(row["FrieghtDisPer"])).ToString("N2", MainPage.indianCurancy);
                    lblTax.Text = Math.Abs(dTax).ToString("N2", MainPage.indianCurancy);
                }
                if (dPacking > 0)
                {
                    txtPSign.Text = "+";
                    txtPackingPer.Text = Convert.ToString(row["FrieghtDisPer"]);
                    lblPacking.Text = dPacking.ToString("N2", MainPage.indianCurancy);
                }
                else if (dPacking < 0)
                {
                    txtPackingPer.Text = Math.Abs(Convert.ToDouble(row["FrieghtDisPer"])).ToString("N2", MainPage.indianCurancy);
                    lblPacking.Text = Math.Abs(dPacking).ToString("N2", MainPage.indianCurancy);
                }
                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);
                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;

               

                if (dt.Columns.Contains("TaxLedger"))
                {
                    txtTaxLedger.Text = Convert.ToString(row["TaxLedger"]);
                    txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                    txtGSTTaxPer.Text = Convert.ToString(row["TaxPer"]);
                }

                if (Convert.ToString(row["LockType"]) == "LOCK" && MainPage.strUserRole != "SUPERADMIN" && MainPage.strUserRole != "ADMIN")
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }

                if (Convert.ToString(row["PurchaseSource"]) == "DIRECT")
                    btnDelete.Enabled = btnEdit.Enabled = false;
               


                BindDataWithGrid(row);
            }
            else
                ClearAllText();
        }

        private void BindDataWithGrid(DataRow row)
        {
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["id"].Value = row["ID"];
            dgrdDetails.Rows[0].Cells["billNo"].Value = row["SaleBillNo"];
            dgrdDetails.Rows[0].Cells["partyName"].Value = row["SParty"];
            dgrdDetails.Rows[0].Cells["pcs"].Value = row["Pieces"];
            dgrdDetails.Rows[0].Cells["item"].Value = row["Item"];
            dgrdDetails.Rows[0].Cells["Disc"].Value = row["Discount"];
            dgrdDetails.Rows[0].Cells["DiscountStatus"].Value = row["DiscountStatus"];
            dgrdDetails.Rows[0].Cells["snDhara"].Value = row["Dhara"];
            dgrdDetails.Rows[0].Cells["amount"].Value = row["Amount"];
            dgrdDetails.Rows[0].Cells["freight"].Value = row["Freight"];
            dgrdDetails.Rows[0].Cells["tax"].Value = row["Tax"];
            dgrdDetails.Rows[0].Cells["packing"].Value = row["Packing"];
            dgrdDetails.Rows[0].Cells["total"].Value = row["GrossAmt"];

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
                pnlTax.Visible = true;
            }
            else
                pnlTax.Visible = false;
        }


        private void EnableAllControls()
        {
           txtDate.ReadOnly=  txtSignPer.ReadOnly = txtSignAmt.ReadOnly =  txtOtherPer.ReadOnly  = txtOtherAmt.ReadOnly = txtRemark.ReadOnly = txtGSTTaxPer.ReadOnly= txtPurchaseInvoiceNo.ReadOnly = txtInvoiceDate.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtDate.ReadOnly = txtSignPer.ReadOnly = txtSignAmt.ReadOnly = txtOtherPer.ReadOnly = txtOtherAmt.ReadOnly = txtRemark.ReadOnly =txtPurchaseInvoiceNo.ReadOnly=txtInvoiceDate.ReadOnly= true;
            txtFSign.ReadOnly = txtTSign.ReadOnly = txtPSign.ReadOnly = txtFreightPer.ReadOnly = txtTaxPer.ReadOnly = txtPackingPer.ReadOnly = txtGSTTaxPer.ReadOnly= true;
        }

        private void ClearAllText()
        {
            txtPurchaseInvoiceNo.Text= lblCreatedBy.Text = txtBillNo.Text = txtGRSNo.Text = txtSupplierName.Text = txtNormalDhara.Text = txtSuperNetDhara.Text = txtDueDate.Text = txtDueDays.Text = txtRemark.Text = txtTaxLedger.Text = txtReverseCharge.Text = "";
            txtSignPer.Text = txtSignAmt.Text = txtFSign.Text = txtTSign.Text = txtPSign.Text = "-";
            txtOtherPer.Text = txtOtherPerAmt.Text = txtOtherAmt.Text = txtDis.Text = txtDiscountAmt.Text = lblGAmount.Text = lblNAmount.Text = txtTaxAmt.Text = txtGSTTaxPer.Text = "0.00";
            chkFreight.Checked = chkTax.Checked = chkPacking.Checked =pnlTax.Visible= false;
           
            txtFreightPer.Text = txtPackingPer.Text = MainPage.dFreightDhara.ToString("0.00");
            txtTaxPer.Text = MainPage.dTaxDhara.ToString("0.00");
            dgrdDetails.Rows.Clear();
            dgrdTax.Rows.Clear();
            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text = txtInvoiceDate.Text= DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtInvoiceDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void ClearFewText()
        {
            txtBillNo.Text =txtSupplierName.Text = txtNormalDhara.Text = txtSuperNetDhara.Text = txtDueDate.Text = txtDueDays.Text = "";
            txtSignPer.Text = txtSignAmt.Text = txtFSign.Text = txtTSign.Text = txtPSign.Text = "-";
            txtOtherPer.Text = txtOtherPerAmt.Text = txtOtherAmt.Text = txtDis.Text = txtDiscountAmt.Text = lblGAmount.Text = lblNAmount.Text =txtTaxAmt.Text= txtGSTTaxPer.Text=  "0.00";
            chkFreight.Checked = chkTax.Checked = chkPacking.Checked = false;
            dgrdDetails.Rows.Clear();
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo)+1,1)SNo  from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' and BillNo!='' ");
                    if (objValue != null)
                    {
                        txtBillNo.Text = Convert.ToString(objValue);
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in set  bill No in Purchase book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtGRSNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GRSNOFORPURCHASE", txtGRSCode.Text, "SEARCH GOODS RECEIVE NO", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtGRSNo.Text = objSearch.strSelectedData;
                            GetRecordFromSales();
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetRecordFromSales()
        {
            try
            {
                ClearFewText();
                if (txtGRSCode.Text != "" && txtGRSNo.Text != "")
                {
                    string strQuery = " Select SE.*,ISNULL(dbo.GetFullName(PurchasePartyID),'PERSONAL') PParty,(Select TOP 1 dbo.GetFullName(SR.SalePartyID) from SalesRecord SR Where SR.BillCode=SE.BillCode and SR.BillNo=SE.BillNo) SalesParty,(Select CONVERT(varchar,ReceivingDate,103)  from GoodsReceive Where (ReceiptCode+' '+CAST(ReceiptNo as varchar))=SE.GRSNo) RDate,((CASE When (SM.Category = 'CASH PURCHASE' OR SM.TINNumber = 'CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (SM.Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)-(CAST((DiscountStatus+Discount) as Money))) DisPer,((((CASE When (SM.Category='CASH PURCHASE'  OR SM.TINNumber = 'CASH PURCHASE') then 5 else 3 end)-(CASE WHEN (SM.Category = 'CLOTH PURCHASE' OR BillCode Like('%SRT%') OR BillCode Like('%CCK%')) then 1 else 0 end)-(CAST((DiscountStatus+Discount) as Money)))*Cast(Amount as Money))/100) DisPerAmt,"
                                    + " SM.NormalDhara,SM.SNDhara as SUPERDhara,(CASE When SM.DueDays!='' then SM.DueDays else (Select TOP 1 GraceDays from CompanySetting) end) DueDays,(Select UPPER(Category) from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar))=PurchasePartyID) SCategory from SalesEntry SE inner join SupplierMaster SM on  (SM.AreaCode+CAST(SM.AccountNo as varchar))=SE.PurchasePartyID Where Personal='NO' and GRSNo='" + txtGRSCode.Text + " " + txtGRSNo.Text + "' ";

                    DataTable dt = dba.GetDataTable(strQuery);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        dgrdDetails.Rows.Add(1);
                        txtBillNo.Text = txtGRSNo.Text;
                        txtSupplierName.Text = Convert.ToString(dr["PParty"]);
                        txtDate.Text = Convert.ToString(dr["RDate"]);
                        dgrdDetails.Rows[0].Cells["id"].Value = dr["ID"];
                        dgrdDetails.Rows[0].Cells["billNo"].Value = dr["BillCode"] + " " + dr["BillNo"];
                        dgrdDetails.Rows[0].Cells["partyName"].Value = dr["SalesParty"];
                        dgrdDetails.Rows[0].Cells["pcs"].Value = dr["Pieces"];
                        dgrdDetails.Rows[0].Cells["item"].Value = dr["Items"];
                        dgrdDetails.Rows[0].Cells["Disc"].Value = dr["Discount"];
                        dgrdDetails.Rows[0].Cells["DiscountStatus"].Value = dr["DiscountStatus"];
                        dgrdDetails.Rows[0].Cells["snDhara"].Value = dr["SNDhara"];
                        dgrdDetails.Rows[0].Cells["amount"].Value = dr["Amount"];
                        dgrdDetails.Rows[0].Cells["freight"].Value = dr["Freight"];
                        dgrdDetails.Rows[0].Cells["tax"].Value = dr["Tax"];
                        dgrdDetails.Rows[0].Cells["packing"].Value = dr["Packing"];
                        dgrdDetails.Rows[0].Cells["total"].Value = lblGAmount.Text = Convert.ToString(dr["TotalAmt"]);
                        txtDis.Text = Convert.ToDouble(dr["DisPer"]).ToString("N2", MainPage.indianCurancy);
                        txtDiscountAmt.Text = Convert.ToDouble(dr["DisPerAmt"]).ToString("N2",MainPage.indianCurancy);
                        txtNormalDhara.Text = Convert.ToString(dr["NormalDhara"]);
                        txtSuperNetDhara.Text = Convert.ToString(dr["SUPERDhara"]);
                        txtDueDays.Text = Convert.ToString(dr["DueDays"]);
                        if (txtDis.Text == "")
                            txtDis.Text = txtDiscountAmt.Text = "0.00";                      
                        if (txtDueDays.Text != "" && txtDate.Text.Length==10)
                        {
                            int days = Convert.ToInt32(txtDueDays.Text);
                            txtDueDate.Text = dba.ConvertDateInExactFormat(txtDate.Text).AddDays(days).ToString("dd/MM/yyyy");
                            btnAdd.Enabled = true;
                            CalculateNetAmount();
                        }
                        else
                        {
                            MessageBox.Show("Sorry ! Due days & bill date can't be blank !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            btnAdd.Enabled = false;
                        }
                    }                 
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred on Getting Record From Sale in Purchase Entry", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private bool ValidateDhara()
        {
            if (Convert.ToString(dgrdDetails.Rows[0].Cells["snDhara"].Value) == "NORMAL")
            {
                if (txtNormalDhara.Text != "")
                {
                    if (Convert.ToDouble(txtNormalDhara.Text) != Convert.ToDouble(txtDis.Text))
                    {
                        MessageBox.Show("Sorry ! Saled dhara and current normal dhara doesn't matched !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnAdd.Enabled = btnEdit.Enabled = false;
                        return false;
                    }
                    else
                    {
                        btnAdd.Enabled = btnEdit.Enabled = true;
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Saled dhara and current normal dhara doesn't matched !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled =btnEdit.Enabled= false;
                    return false;
                }
            }
            else
            {
                if (txtSuperNetDhara.Text != "")
                {
                    if (Convert.ToDouble(txtSuperNetDhara.Text) != Convert.ToDouble(txtDis.Text))
                    {
                        MessageBox.Show("Sorry ! Saled dhara and current super net dhara doesn't matched !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnAdd.Enabled = btnEdit.Enabled = false;
                        return false;
                    }
                    else
                    {
                        btnAdd.Enabled = btnEdit.Enabled = true;
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Saled dhara and current normal dhara doesn't matched !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnAdd.Enabled = btnEdit.Enabled = false;
                    return false;
                }
            }
        }

        private void txtSign1_KeyPress(object sender, KeyPressEventArgs e)
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
                            txt.Text = "-";
                        CalculateNetAmount();
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

        private void txtOtherPer_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
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
            double dAmt = dba.ConvertObjectToDouble(txtOtherPer.Text), dGrossAmt = dba.ConvertObjectToDouble(lblGAmount.Text);
            dAmt = (dAmt * dGrossAmt) / 100;
            txtOtherPerAmt.Text = dAmt.ToString("N2", MainPage.indianCurancy);
            CalculateNetAmount();
        }

        private void txtOtherAmt_Leave(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (txtOtherAmt.Text == "")
                        txtOtherAmt.Text = "0.00";
                    CalculateNetAmount();
                }
            }
            catch
            {
            }
        }

        private void chkFreight_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (chkFreight.Checked && txtFreightPer.Text != "" && dgrdDetails.Rows.Count > 0)
                    {
                        double dFreight = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["freight"].Value), dPer = Convert.ToDouble(txtFreightPer.Text);
                        if (dFreight != 0 && dPer != 0)
                            lblFreight.Text = ((dFreight * dPer) / 100).ToString("N2", MainPage.indianCurancy);
                        else
                            lblFreight.Text = "0.00";
                    }
                    else
                    {
                        lblFreight.Text = "0.00";
                    }
                    CalculateNetAmount();
                }
            }
            catch
            {
            }
        }

        private void chkTax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (chkTax.Checked && txtTaxPer.Text != "" && dgrdDetails.Rows.Count > 0)
                    {
                        double dTax = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["tax"].Value), dPer = Convert.ToDouble(txtTaxPer.Text);
                        if (dTax != 0 && dPer != 0)
                            lblTax.Text = ((dTax * dPer) / 100).ToString("N2", MainPage.indianCurancy);
                        else
                            lblTax.Text = "0.00";
                    }
                    else
                    {
                        lblTax.Text = "0.00";
                    }
                    CalculateNetAmount();
                }
            }
            catch
            {
            }
        }

        private void chkPacking_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (chkPacking.Checked && txtPackingPer.Text != "" && dgrdDetails.Rows.Count > 0)
                    {
                        double dPacking = dba.ConvertObjectToDouble(dgrdDetails.Rows[0].Cells["packing"].Value), dPer = Convert.ToDouble(txtPackingPer.Text);
                        if (dPacking != 0 && dPer != 0)
                            lblPacking.Text = ((dPacking * dPer) / 100).ToString("N2", MainPage.indianCurancy);
                        else
                            lblPacking.Text = "0.00";
                    }
                    else
                    {
                        lblPacking.Text = "0.00";
                    }
                    CalculateNetAmount();
                }
            }
            catch
            {
            }
        }

        private bool ValidateControls()
        {
            CalculateNetAmount();
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
            bool _bStatus = dba.ValidateBackDateEntry(txtDate.Text);
            if (!_bStatus)
                return false;
            if (MainPage.strUserRole != "ADMIN")
            {
                if (txtPurchaseInvoiceNo.Text == "")
                {
                    MessageBox.Show("Sorry ! Purchase Invoice No can't be blank", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPurchaseInvoiceNo.Focus();
                    return false;
                }
                if (txtInvoiceDate.Text.Length != 10)
                {
                    MessageBox.Show("Sorry ! Invoice Date is not valid, Please enter valid date", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInvoiceDate.Focus();
                    return false;
                }
            }
            if (txtBillNo.Text != txtGRSNo.Text)
            {
                MessageBox.Show("Sorry ! Bill No and GRSNo should be same !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGRSNo.Focus();
                return false;
            }
            if (txtSupplierName.Text == "")
            {
                MessageBox.Show("Sorry ! Supplier name can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSupplierName.Focus();
                return false;
            }
            if (txtDueDays.Text == "")
            {
                MessageBox.Show("Sorry ! Due days can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDueDays.Focus();
                return false;
            }
            if (txtNormalDhara.Text == "" && txtSuperNetDhara.Text == "")
            {
                MessageBox.Show("Sorry ! Both dhara can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNormalDhara.Focus();
                return false;
            }
            if (dba.ConvertObjectToDouble(lblNAmount.Text) == 0)
            {
                MessageBox.Show("Sorry ! Net amt can't be zero.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGRSNo.Focus();
                return false;
            }
            if (dgrdDetails.Rows.Count == 0)
            {
                MessageBox.Show("Sorry ! Atleast one entry is required.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGRSNo.Focus();
                return false;
            }
            if (btnAdd.Text == "&Save")
            {
                DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='SALES' and TaxName='" + txtTaxLedger.Text + "') Region,(Select ISNULL((Select (CASE WHEN STM.TaxIncluded= ST.TaxIncluded THEN 'OK' else 'DENY' end) from SaleTypeMaster ST Where ST.SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "'),'DENY') PTaxIncluded from SalesEntry SE INNER join SalesRecord SR on SE.BillCode=SR.BillCode and SE.BillNo=SR.BillNo inner join SaleTypeMaster STM on SR.SalesType=STM.TaxName and STM.SaleType='SALES' WHere SE.GRSNo='" + txtGRSCode.Text + " " + txtGRSNo.Text + "') IncludeStatus from SupplierMaster Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSupplierName.Text + "' ");

                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSupplierName.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
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

                if (Convert.ToString(dt.Rows[0]["IncludeStatus"]) == "DENY")
                {
                    MessageBox.Show("Sorry Sale type and purchase type doesn't match in tax inclusion!\nPlease enter correct purchase type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (btnEdit.Text == "&Update")
            {
                bool iStatus = false, bStatus = false;
                bStatus= ValidateOtherValidation(ref iStatus);
                if (!bStatus)
                    return iStatus;
            }
            return ValidateDhara();
        }

        private bool ValidateOtherValidation(ref bool iStatus)
        {
            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtTaxLedger.Text + "') Region,ISNULL((Select InsertStatus from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),0) InsertStatus,'FALSE' TickStatus,(Select ISNULL((Select (CASE WHEN STM.TaxIncluded= ST.TaxIncluded THEN 'OK' else 'DENY' end) from SaleTypeMaster ST Where ST.SaleType='PURCHASE' and TaxName='"+txtTaxLedger.Text+"'),'DENY') PTaxIncluded from SalesEntry SE INNER join SalesRecord SR on SE.BillCode=SR.BillCode and SE.BillNo=SR.BillNo inner join SaleTypeMaster STM on SR.SalesType=STM.TaxName and STM.SaleType='SALES' WHere SE.GRSNo='"+txtGRSCode.Text+" "+txtGRSNo.Text+"') IncludeStatus from SupplierMaster Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtSupplierName.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtSupplierName.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                iStatus = Convert.ToBoolean(dt.Rows[0]["InsertStatus"]);

                if (dOldNetAmt != Convert.ToDouble(lblNAmount.Text))
                {                   
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
                if (Convert.ToString(dt.Rows[0]["IncludeStatus"]) == "DENY")
                {
                    MessageBox.Show("Sorry Sale type and purchase type doesn't match in tax inclusion!\nPlease enter correct purchase type ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Sorry ! No record found for validation ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void CalculateNetAmount()
        {
            double dDiscount = 0, dOtherPerAmt = 0, dOtherAmt = 0, dFreight = 0, dPacking = 0, dTax = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dTaxAmt = 0,dFinalAmt=0;
            try
            {
                dDiscount = dba.ConvertObjectToDouble(txtDiscountAmt.Text);
                dOtherPerAmt = dba.ConvertObjectToDouble(txtSignPer.Text + txtOtherPerAmt.Text);
                dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGAmount.Text);
                if (chkFreight.Checked)
                    dFreight = dba.ConvertObjectToDouble(lblFreight.Text);
                if (chkPacking.Checked)
                    dPacking = dba.ConvertObjectToDouble(lblPacking.Text);
                if (chkTax.Checked)
                    dTax = dba.ConvertObjectToDouble(lblTax.Text);

                dTOAmt = dOtherPerAmt + dOtherAmt + dFreight + dPacking + dTax;
                dFinalAmt = dGrossAmt - dDiscount + dTOAmt;

                dTaxAmt = GetTaxAmount(dFinalAmt,dTOAmt);

                dNetAmt = dGrossAmt - dDiscount + dOtherPerAmt + dOtherAmt + dFreight + dPacking + dTax + dTaxAmt;
                lblNAmount.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);
            }
            catch// (Exception ex)
            {
              //  string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
               // dba.CreateErrorReports(strReport);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to clear entered data ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                      
                        btnEdit.Text = "&Edit";
                    }
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();                  
                     btnDelete.Enabled = btnEdit.Enabled = true;
                    EnableAllControls();
                    txtDate.Focus();
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else if (ValidateControls())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Adding in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnAdd.Enabled = true;
        }

        private void SaveRecord()
        {
            try
            {
                string strQuery = "", strDate = "", strInvDate = "", strPurchaseParty = "", strSalesParty = "", strSalePartyID  = "", strPurchasePartyID = "", strTaxAccountID="";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text), invDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text); ;
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                strInvDate = invDate.ToString("MM/dd/yyyy hh:mm:ss");
                DataGridViewRow row = dgrdDetails.Rows[0];

                string[] strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }
                strFullName = Convert.ToString(row.Cells["partyName"].Value).Split(' ');
                if (strFullName.Length > 0)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSalesParty =Convert.ToString(row.Cells["partyName"].Value).Replace(strPurchasePartyID + " ", "");
                }
                double dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);

                strQuery = " if not exists (Select * from [PurchaseRecord] Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ) begin "
                              + " INSERT INTO [dbo].[PurchaseRecord] ([BillCode],[BillNo],[GRSNo],[DueDays],[SupplierName],[SaleBillNo],[SalesParty],[Pieces],[Item],[Discount],[DiscountStatus],[Amount],[Freight],[Tax],[Packing],[FreightDiscount],[TaxDiscount],[PackingDiscount],[NetDiscount],[Remark],[OtherPer],[Others],[GrossAmt],[NetAmt],[BillDate],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SalePartyID],[PurchasePartyID],[TaxLedger],[TaxAmount],[TaxPer],[ReverseCharge],[Dhara],[InvoiceNo],[InvoiceDate],[CheckStatus],[CheckedBy],[PurchaseSource]) VALUES "
                              + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtGRSCode.Text + " " + txtGRSNo.Text + "','" + txtDueDays.Text + "','" + strPurchaseParty + "','" + row.Cells["billNo"].Value + "','" + strSalesParty + "','" + row.Cells["pcs"].Value + "','" + row.Cells["item"].Value + "','" + row.Cells["Disc"].Value + "','" + row.Cells["DiscountStatus"].Value + "','" + row.Cells["amount"].Value + "','" + row.Cells["freight"].Value + "','" + row.Cells["tax"].Value + "','" + row.Cells["packing"].Value + "',"
                              + " '" + lblFreight.Text + "','" + lblTax.Text + "','" + lblPacking.Text + "','" + txtDiscountAmt.Text + "','" + txtRemark.Text + "','" + txtSignPer.Text + txtOtherPerAmt.Text + "','" + txtSignAmt.Text + txtOtherAmt.Text + "','" + lblGAmount.Text + "','" + lblNAmount.Text + "','" + strDate + "','" + MainPage.strLoginName + "','',1,0,'" + strSalePartyID + "','" + strPurchasePartyID + "','"+txtTaxLedger.Text+"',"+dTaxAmt+","+dba.ConvertObjectToDouble(txtGSTTaxPer.Text)+",'"+txtReverseCharge.Text+"' ,'"+row.Cells["snDhara"].Value+"','"+txtPurchaseInvoiceNo.Text+"','"+ strInvDate+"',1,'','SALES') "
                              + " UPDATE [SalesEntry] Set [PurchaseBill]='CLEAR', [UpdateStatus]=1 where [GRSNo]='" + txtGRSCode.Text + " " + txtGRSNo.Text + "' "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "','" + strPurchaseParty + "','PURCHASE A/C','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNAmount.Text + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "')  ";

                if (dTaxAmt > 0 && txtTaxLedger.Text!="")
                {                   
                        strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ;"
                                 + " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                                 + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                 + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                 + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                 + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";                   
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
                                   + " ('PURCHASE','" + txtGRSCode.Text + "'," + txtGRSNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNAmount.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";
                
                strQuery += " end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    ClearAllText();
                    BindLastRecord();
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Saving Record in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
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
                      
                        BindLastRecord();
                        btnAdd.Text = "&Add";
                    }
                    if (btnEdit.Enabled)
                    {
                        btnEdit.Text = "&Update";
                        EnableAllControls();
                        txtDate.Focus();
                    }
                    else
                        return;
                }
                else
                {
                    btnEdit.Enabled = false;
                    if (ValidateControls())
                    {
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
                CalculateNetAmount();
                string strQuery = "", strDate = "", strInvDate = "", strPurchaseParty = "", strSalesParty = "", strSalePartyID = "", strPurchasePartyID = "", strTaxAccountID = "";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                if (txtInvoiceDate.Text.Length==10)
                {
                    DateTime invDate = dba.ConvertDateInExactFormat(txtInvoiceDate.Text);
                    strInvDate = "'"+invDate.ToString("MM/dd/yyyy hh:mm:ss")+"'";
                }
                else
                    strInvDate = "NULL";
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");

                DataGridViewRow row = dgrdDetails.Rows[0];

                string[] strFullName = txtSupplierName.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtSupplierName.Text.Replace(strPurchasePartyID + " ", "");
                }
                strFullName = Convert.ToString(row.Cells["partyName"].Value).Split(' ');
                if (strFullName.Length > 0)
                {
                    strSalePartyID = strFullName[0].Trim();
                    strSalesParty = Convert.ToString(row.Cells["partyName"].Value).Replace(strPurchasePartyID + " ", "");
                }

                double dTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);

                strQuery = "  UPDATE [dbo].[PurchaseRecord] SET [DueDays]='" + txtDueDays.Text + "',[SupplierName]='" + strPurchaseParty + "',[SaleBillNo]='" + row.Cells["billNo"].Value + "',[SalesParty]='" + strSalesParty + "',[Pieces]='" + row.Cells["pcs"].Value + "',[Item]='" + row.Cells["item"].Value + "',"
                         + " [Discount]='" + row.Cells["Disc"].Value + "',[DiscountStatus]='" + row.Cells["DiscountStatus"].Value + "',[Amount]='" + row.Cells["amount"].Value + "',[Freight]='" + row.Cells["freight"].Value + "',[Tax]='" + row.Cells["tax"].Value + "',[Packing]='" + row.Cells["packing"].Value + "',[FreightDiscount]='" + lblFreight.Text + "',"
                         + " [TaxDiscount]='" + lblTax.Text + "',[PackingDiscount]='" + lblPacking.Text + "',[NetDiscount]='" + txtDiscountAmt.Text + "',[Remark]='" + txtRemark.Text + "',[OtherPer]='" + txtSignPer.Text + txtOtherPerAmt.Text + "',[Others]='" + txtSignAmt.Text + txtOtherAmt.Text + "',[GrossAmt]='" + lblGAmount.Text + "',[ReverseCharge]='" + txtReverseCharge.Text + "',[InvoiceNo]='" + txtPurchaseInvoiceNo.Text + "',[InvoiceDate]=" + strInvDate + ", "
                         + " [NetAmt]='" + lblNAmount.Text + "',[BillDate]='" + strDate + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[SalePartyID]='" + strSalePartyID + "',[PurchasePartyID]='" + strPurchasePartyID + "',[TaxLedger]='" + txtTaxLedger.Text + "',[TaxAmount]=" + dTaxAmt + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtGSTTaxPer.Text) + " Where [BillCode]='" + txtBillCode.Text + "' AND [BillNo]=" + txtBillNo.Text + " AND [GRSNo]='" + txtGRSCode.Text + " " + txtGRSNo.Text + "' "
                         + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strPurchaseParty + "',[Amount]='" + lblNAmount.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strPurchasePartyID + "' Where [AccountStatus]='PURCHASE A/C' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                         + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtGRSCode.Text + "' and [BillNo]=" + txtGRSNo.Text + " ";

                if (dTaxAmt > 0 && txtTaxLedger.Text != "")
                {
                    strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250) ;"
                                 + " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtTaxLedger.Text + "'; "
                                 + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IremoGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                                 + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                                 + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                                 + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                                 + " ('" + bDate.ToString("MM/dd/yyyy hh:mm:ss") + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                                   + " ('PURCHASE','" + txtGRSCode.Text + "'," + txtGRSNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                        + "('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNAmount.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    BindRecordWithControl(txtBillNo.Text);
                }
                else
                {
                    MessageBox.Show("Sorry ! An Error occurred , Try After some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Updating Record in Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
            txtBillNo.Focus();
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 1)
                        ShowSaleBook();
                    else if (e.ColumnIndex == 2)
                        ShowPartyMaster();
                }
            }
            catch
            {
            }
        }

        private void ShowSaleBook()
        {
            string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);
            string[] strNumber = strInvoiceNo.Split(' ');
            if (strNumber.Length > 1)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    dba.ShowSaleBookPrint(strNumber[0], strNumber[1],false, false);
                }
                else
                {
                    SaleBook objSale = new SaleBook(strNumber[0], strNumber[1]);
                    objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objSale.ShowInTaskbar = true;
                    objSale.Show();
                }
            }
        }

        private void ShowPartyMaster()
        {
            string strPartyName = Convert.ToString(dgrdDetails.CurrentCell.Value);
            if (strPartyName != "")
            {
                SupplierMaster objSupplier = new SupplierMaster(strPartyName);
                objSupplier.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                objSupplier.Show();
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode==Keys.Enter && dgrdDetails.CurrentRow.Index >= 0)
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex == 1)
                        ShowSaleBook();
                    else if (dgrdDetails.CurrentCell.ColumnIndex == 2)
                        ShowPartyMaster();
                }
            }
            catch
            {
            }
        }

        private void PurchaseBook_Load(object sender, EventArgs e)
        {
            try
            {
                if (newStatus)
                {
                    btnAdd.PerformClick();
                    txtGRSNo.Focus();
                }
                EditOption();
            }
            catch
            {
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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


        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bPurchaseAdd || MainPage.mymainObject.bPurchaseEdit || MainPage.mymainObject.bPurchaseView)
                {
                    if (!MainPage.mymainObject.bPurchaseAdd)
                        btnAdd.Visible = false;
                    if (!MainPage.mymainObject.bPurchaseEdit)
                        btnEdit.Visible = btnDelete.Visible = false;
                    if (!MainPage.mymainObject.bPurchaseView)
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
                        SearchData objSearch = new SearchData("PURCHASETYPE", "SEARCH PURCHASE TYPE", e.KeyCode);
                        objSearch.ShowDialog();
                        txtTaxLedger.Text = objSearch.strSelectedData;
                        CalculateNetAmount();
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private double GetTaxAmount(double dFinalAmt,double dOtherAmt)
        {
            double dTaxAmt = 0, dServiceAmt = 0, dTaxPer=0;
            string _strTaxType = "";
            try
            {
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

                            dTaxPer = dba.ConvertObjectToDouble(txtGSTTaxPer.Text);

                            string strQuery = "", strSubQuery = "", strGRSNo = "";
                            double dDisStatus = 0;

                            strGRSNo = txtGRSCode.Text + " " + txtGRSNo.Text;
                            dDisStatus = dba.ConvertObjectToDouble(txtDis.Text);

                            strQuery += " Declare @TaxRate float; ";
                            if (dTaxPer == 18)
                                strQuery += " SET @TaxRate = " + dTaxPer;
                            else
                                strQuery += " Select @TaxRate=MAX(GM.TaxRate) from GoodsReceiveDetails GRD inner join GoodsReceive GR on GRD.ReceiptCode=GR.ReceiptCode and GRD.ReceiptNo=GR.ReceiptNo  Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Amount*100)/(100+TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end))/GRD.Quantity)>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((GRD.Amount*100)/(100+TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end))/GRD.Quantity)<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where GRD.ItemName=_IM.ItemName ) as GM Where (GRD.ReceiptCode+' '+ CAST(GRD.ReceiptNo as nvarchar)) in ('" + strGRSNo + "') ";

                            strQuery += "Select SUM(ROUND(((Amount*TaxRate)/100),2)) as Amt,(TaxRate) TaxRate from ( "
                                        + " Select HSNCode,SUM(Amount) Amount,SUM(Quantity) Qty,TaxRate from (  ";

                            strSubQuery += " Select(GM.Other + ' : ' + GM.HSNCode) as HSNCode,GRD.Quantity,ROUND(((((CASE WHEN '"+ _strTaxType+"' = 'INCLUDED' then((Amount * 100) / (100 + TaxRate)) else Amount end))*(100 - " + dDisStatus + "))/ 100),2)Amount,GM.TaxRate from GoodsReceiveDetails GRD Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '"+ _strTaxType+ "' = 'INCLUDED' then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end))/ GRD.Quantity)> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType+ "' = 'INCLUDED' then((GRD.Amount * 100) / (100 + TaxRate)) else GRD.Amount end)))* (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 - " + dDisStatus + ") / 100.00) else 1.00 end))/ GRD.Quantity)< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where GRD.ItemName = _IM.ItemName ) as GM Where(GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.Amount > 0   Union All "
                                        + " Select '' as HSNCode,0 as Quantity,ROUND(((GRD.PackingAmt + GRD.FreightAmt) * (CASE WHEN '"+ _strTaxType+"' = 'INCLUDED' then((100) / (100 + @TaxRate)) else 1 end)),2) Amount,@TaxRate as TaxRate from GoodsReceiveDetails GRD Where(GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and(GRD.PackingAmt + GRD.FreightAmt) > 0  Union All "
                                        + " SELECT  '' as HSNCode,0 as Quantity," + dOtherAmt + " as Amount,@TaxRate as TaxRate  )_Sales Group by HSNCode, TaxRate)_Sales Where TaxRate>0 Group by TaxRate ";

                            //strSubQuery += " Select (GM.Other+ ' : '+GM.HSNCode) as HSNCode,GRD.Quantity,(GRD.Amount-((GRD.Amount*(" + dDisStatus + "))/100))Amount,GM.TaxRate from GoodsReceiveDetails GRD  left join Items _IM on GRD.ItemName=_IM.ItemName Outer APPLY (SELECT TOP 1 GM1.GroupName,GM1.HSNCode, GM1.TaxRate,GM1.Other from ItemGroupMaster GM1 Where GM1.GroupName=_IM.GroupName and (GM1.AmtRange>=((GRD.Amount-((GRD.Amount*(" + dDisStatus + "))/100))/GRD.Quantity) OR GM1.AmtRange=0) Order by GM1.AmtRange asc) as GM Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.Amount>0   Union All   "
                            //                      + " Select (GM.Other+ ' : '+GM.HSNCode) as HSNCode,0 as Quantity,GRD.PackingAmt as Amount,@TaxRate as TaxRate from  GoodsReceiveDetails GRD Outer Apply (Select TOP 1 FreightDhara from CompanySetting) CS OUTER APPLY (SELECT TOP 1 GM1.GroupName,GM1.HSNCode, GM1.TaxRate,GM1.Other from ItemGroupMaster GM1 Where GM1.Other='SAC' and (GM1.GroupName Like('%PACKING%'))  and (GM1.AmtRange>=GRD.PackingAmt OR GM1.AmtRange=0) Order by GM1.AmtRange asc) as GM Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.PackingAmt>0  Union All   "
                            //                      + " Select (GM.Other+ ' : '+GM.HSNCode) as HSNCode,0 as Quantity,GRD.FreightAmt as Amount,@TaxRate as TaxRate from GoodsReceiveDetails GRD Outer Apply (Select TOP 1 FreightDhara from CompanySetting) CS OUTER APPLY (SELECT TOP 1 GM1.GroupName,GM1.HSNCode, GM1.TaxRate,GM1.Other from ItemGroupMaster GM1 Where GM1.Other='SAC' and (GM1.GroupName Like('%FREIGHT%'))  and (GM1.AmtRange>=GRD.FreightAmt OR GM1.AmtRange=0) Order by GM1.AmtRange asc) as GM Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.FreightAmt>0  Union All  "
                            //                      + " Select (GM.Other+ ' : '+GM.HSNCode) as HSNCode,0 as Quantity,GRD.TaxAmt as Amount,@TaxRate as TaxRate from GoodsReceiveDetails GRD Outer Apply (Select TOP 1 TaxDhara from CompanySetting) CS OUTER APPLY (SELECT TOP 1 GM1.GroupName,GM1.HSNCode, GM1.TaxRate,GM1.Other from ItemGroupMaster GM1 Where GM1.Other='SAC' and (GM1.GroupName Like('%TAX%'))  and (GM1.AmtRange>=GRD.TaxAmt OR GM1.AmtRange=0) Order by GM1.AmtRange asc) as GM Where (GRD.ReceiptCode+' '+CAST(GRD.ReceiptNo as varchar)) in ('" + strGRSNo + "') and GRD.TaxAmt>0  Union All   ";

                            //strSubQuery += " SELECT TOP 1 (GM.Other+ ' : '+GM.HSNCode) as HSNCode,0 as Quantity," + dOtherAmt + " as Amount,@TaxRate as TaxRate from ItemGroupMaster GM Where (GM.GroupName Like('%SHIPPING%') OR GM.GroupName Like('%SERVICE%'))  and (GM.AmtRange>=" + dOtherAmt + " OR GM.AmtRange=0) Order by GM.AmtRange asc  )_Sales  Group by HSNCode,TaxRate)_Sales)_Sales ";

                            strQuery += strSubQuery;

                            DataTable dt = dba.GetDataTable(strQuery);
                            if (dt.Rows.Count > 0)
                            {
                                //dTaxAmt = dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]);
                                //dTaxPer = dba.ConvertObjectToDouble(dt.Rows[0]["TaxRate"]);
                                double dMaxRate = 0, dTTaxAmt = 0;
                                BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt);

                                dTaxAmt = dTTaxAmt;// dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]);
                                dTaxPer = dMaxRate;// dba.ConvertObjectToDouble(dt.Rows[0]["TaxRate"]);
                                pnlTax.Visible = true;
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
                        }
                        else
                            txtTaxAmt.Text =  txtTaxPer.Text = "0.00";
                    }
                }

            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Purchase Book", ex.Message };
                dba.CreateErrorReports(strReport);
            }

            txtTaxAmt.Text = dTaxAmt.ToString("N2", MainPage.indianCurancy);
            txtGSTTaxPer.Text = dTaxPer.ToString("N2", MainPage.indianCurancy);

            if (_strTaxType == "INCLUDED")
                dTaxAmt = 0;
            return dTaxAmt;
        }

        private void txtGRSCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("GOODSRCODE", "SEARCH GOODS RECEIVE CODE", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtGRSCode.Text = objSearch.strSelectedData;
                            ClearAllText();
                        }
                    }
                }
                e.Handled = true;
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

        private void txtGSTTaxPer_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                if (txtTaxLedger.Text != "")
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
                        if (objSearch.strSelectedData != "")
                        {
                            txtReverseCharge.Text = objSearch.strSelectedData;
                        }
                    }
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlTax.Visible = false;
        }

        private void BindTaxDetails(DataTable _dt, DataRow _row, ref double dMaxRate, ref double dTTaxAmt)
        {
            try
            {
                dgrdTax.Rows.Clear();
                if (_dt.Rows.Count > 0)
                {
                    dgrdTax.Rows.Add(_dt.Rows.Count);
                    int _index = 0;
                    string strRegion = Convert.ToString(_row["Region"]), strIGST = Convert.ToString(_row["IGSTName"]), strSGST = Convert.ToString(_row["SGSTName"]); ;
                    if (strRegion == "LOCAL")
                        dgrdTax.Rows.Add(_dt.Rows.Count);
                    double dTaxRate = 0, dTaxAmt = 0;

                    foreach (DataRow row in _dt.Rows)
                    {
                        dTaxRate = dba.ConvertObjectToDouble(row["TaxRate"]);
                        dTTaxAmt += dTaxAmt = dba.ConvertObjectToDouble(row["Amt"]);
                        if (dTaxRate > dMaxRate)
                            dMaxRate = dTaxRate;

                        dgrdTax.Rows[_index].Cells["taxName"].Value = strIGST;
                        dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;

                        if (strRegion == "LOCAL")
                        {
                            dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
                            dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                            _index++;
                            dgrdTax.Rows[_index].Cells["taxName"].Value = strSGST;
                            dgrdTax.Rows[_index].Cells["taxType"].Value = strRegion;
                            dgrdTax.Rows[_index].Cells["taxRate"].Value = (dTaxRate / 2).ToString("N2", MainPage.indianCurancy);
                            dgrdTax.Rows[_index].Cells["taxAmt"].Value = (dTaxAmt / 2).ToString("N2", MainPage.indianCurancy);
                        }
                        else
                        {
                            dgrdTax.Rows[_index].Cells["taxRate"].Value = dTaxRate.ToString("N2", MainPage.indianCurancy);
                            dgrdTax.Rows[_index].Cells["taxAmt"].Value = dTaxAmt.ToString("N2", MainPage.indianCurancy);
                        }

                        _index++;
                    }
                }
            }
            catch { }
        }

        private void txtInvoiceDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.GetDateInExactFormat(sender, true, true, true);
            }
        }

        private void txtRemark_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                dba.ValidateSpace(sender, e);
            }
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

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save")
                {
                    if (txtReason.Text != "")
                    {
                        bool iStatus = true;
                        if (ValidateOtherValidation(ref iStatus))
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                            {
                                string strQuery = "";
                                strQuery += " Delete from PurchaseRecord Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "  "
                                         + " Update SalesEntry Set PurchaseBill='PENDING',UpdateStatus=1 Where GRSNo='" + txtGRSCode.Text + " " + txtGRSNo.Text + "' "
                                         + " Delete from BalanceAmount Where  Description='" + txtBillCode.Text + " " + txtBillNo.Text + "' and AccountStatus in ('PURCHASE A/C','DUTIES & TAXES')  "
                                         + " Delete from [dbo].[GSTDetails] Where [BillType]='PURCHASE' and [BillCode]='" + txtGRSCode.Text + "' and [BillNo]=" + txtGRSNo.Text + " "
                                         + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                         + " ('PURCHASE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text+ ", With Amt : "+ lblNAmount.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                                int count = dba.ExecuteMyQuery(strQuery);
                                if (count > 0)
                                {
                                    if (!iStatus)
                                        DataBaseAccess.CreateDeleteQuery(strQuery);
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
            }
            catch
            {
            }
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
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

        private void txtReason_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }
    }
}
