using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SSS
{
    public partial class CreditNote_Supplier : Form
    {
        DataBaseAccess dba;
        string strLastSerialNo = "", strDeletedSID = "",strPurchaseBillCode="", strOldPartyName="";
        double dOldNetAmt = 0;
        public bool saleStatus = false,updateStatus=false,newStatus=false;
        public CreditNote_Supplier()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strLastSerialNo != "0")
                BindRecordWithControl(strLastSerialNo);
        }

        public CreditNote_Supplier(bool bStatus)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            newStatus = bStatus;          
        }

        public CreditNote_Supplier(string strCode,string strSNo)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetStartupData();
            if (strCode != "")
                txtBillCode.Text = strCode;
            BindRecordWithControl(strSNo);            
        }

        public CreditNote_Supplier(string strCode, string strSNo, bool sStatus)
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
                if (pnlDeletionConfirmation.Visible)
                    pnlDeletionConfirmation.Visible = false;
                else if (pnlTax.Visible)
                    pnlTax.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused && !txtOtherAmt.Focused)
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
                        if ( btnAdd.Text == "&Add" && btnEdit.Text== "&Edit" && txtBillNo.Text != "")
                        {
                            BindRecordWithControl(txtBillNo.Text);
                        }
                    }
                }
            }
        }

        private void GetStartupData()
        {
            try
            {
                string strQuery = " Select PBillCode,CreditNoteCode,(Select ISNULL(MAX(BillNo),0) from PurchaseReturn Where EntryType='CREDITNOTE' and BillCode=CreditNoteCode)SerialNo from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "' ";
                DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        txtPBillCode.Text = strPurchaseBillCode = Convert.ToString(dt.Rows[0]["PBillCode"]);
                        txtBillCode.Text = Convert.ToString(dt.Rows[0]["CreditNoteCode"]);
                        strLastSerialNo = Convert.ToString(dt.Rows[0]["SerialNo"]);
                    }
                }
            }
            catch
            {
            }
        }

        private void BindLastRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseReturn Where EntryType='CREDITNOTE' and BillCode='" + txtBillCode.Text + "' ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseReturn Where EntryType='CREDITNOTE' and BillCode='" + txtBillCode.Text + "' ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                ClearAllText();
        }

        private void BindNextRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from PurchaseReturn Where EntryType='CREDITNOTE' and BillCode='" + txtBillCode.Text + "' and BillNo>" + txtBillNo.Text + " ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
                BindRecordWithControl(strSerialNo);
            else
                BindLastRecord();
        }

        private void BindPreviousRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from PurchaseReturn Where EntryType='CREDITNOTE' and  BillCode='" + txtBillCode.Text + "' and BillNo<" + txtBillNo.Text + " ");
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

                    string strQuery = "  Select *,Convert(varchar,Date,103)BDate,Convert(varchar,PurchaseBillDate,103)PDate,dbo.GetFullName(PurchasePartyID) PurchaseParty,(Select TOP 1 Status from MonthLockDetails Where MonthName=UPPER(DATENAME(MM,PR.Date))) LockType from PurchaseReturn PR Where EntryType='CREDITNOTE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo
                                    + " Select *,dbo.GetFullName(SalePartyID) SalesParty from PurchaseReturnDetails Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo + "  order by SID "
                                    + " Select *,ISNULL(dbo.GetFullName(GSTAccount),'') AccountName from dbo.[GSTDetails] Where BillType='CREDITNOTE' and BillCode='" + txtBillCode.Text + "' and BillNo=" + strSerialNo;

                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 1)
                    {
                        DataTable _dt = ds.Tables[0];
                        if (_dt.Rows.Count > 0)
                        {
                            dgrdDetails.Rows.Clear();
                            pnlTax.Visible = true;                           
                            BindDataWithControlUsingDataTable(_dt);
                            BindSaleReturnDetails(_dt, ds.Tables[1]);
                            BindGSTDetailsWithControl(ds.Tables[2]);                       
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
                DataRow row = dt.Rows[0];

                txtBillCode.Text = Convert.ToString(row["BillCode"]);
                txtBillNo.Text = Convert.ToString(row["BillNo"]);
                txtDate.Text = Convert.ToString(row["BDate"]);
                txtPBillCode.Text = Convert.ToString(row["PurchaseBillCode"]);
                txtPBillNo.Text = Convert.ToString(row["PurchaseBillNo"]);
                txtPDate.Text = Convert.ToString(row["PDate"]);
                strOldPartyName=txtPurchaseParty.Text = Convert.ToString(row["PurchaseParty"]);
                txtPurchaseInvoiceNo.Text = Convert.ToString(row["ReverseCharge"]);
                txtPurchaseType.Text = Convert.ToString(row["PurchaseType"]);
                txtRemark.Text = Convert.ToString(row["Remark"]);
                txtSignAmt.Text = Convert.ToString(row["OtherSign"]);
                txtOtherAmt.Text = Convert.ToString(row["OtherAmt"]);           
                txtDiscountAmt.Text = Convert.ToString(row["NetDiscount"]);           
                txtTaxPer.Text = Convert.ToString(row["TaxPer"]);
                txtTaxAmt.Text = Convert.ToString(row["TaxAmount"]);
                lblQty.Text = Convert.ToString(row["TotalQty"]);
                lblGrossAmt.Text = dba.ConvertObjectToDouble(row["GrossAmt"]).ToString("N2", MainPage.indianCurancy); 
                lblNetAmt.Text = dba.ConvertObjectToDouble(row["NetAmt"]).ToString("N2",MainPage.indianCurancy);
                txtPurchaseInvoiceNo.Text = Convert.ToString(row["ReverseCharge"]);

                if (dt.Columns.Contains("TaxableAmt"))
                    lblTaxableAmt.Text = dba.ConvertObjectToDouble(row["TaxableAmt"]).ToString("N2", MainPage.indianCurancy);
                txtRoundOffSign.Text = Convert.ToString(row["RoundOffSign"]);
                txtRoundOffAmt.Text = Convert.ToString(row["RoundOffAmt"]);

                if (txtRoundOffSign.Text == "")
                    txtRoundOffSign.Text = "+";
                if (txtRoundOffAmt.Text == "")
                    txtRoundOffAmt.Text = "0.00";

                string strCreatedBy = Convert.ToString(row["CreatedBy"]), strUpdatedBy = Convert.ToString(row["UpdatedBy"]);

                if (strCreatedBy != "")
                    lblCreatedBy.Text = "Created By : " + strCreatedBy;
                if (strUpdatedBy != "")
                    lblCreatedBy.Text += " , Updated  By : " + strUpdatedBy;


                if (Convert.ToString(row["LockType"]) == "LOCK" && !MainPage.strUserRole.Contains("ADMIN"))
                    btnEdit.Enabled = btnDelete.Enabled = false;
                else
                {
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                txtBillNo.ReadOnly = false;
            }
        }

        private void BindSaleReturnDetails(DataTable _dtMain, DataTable _dtDetails)
        {
            dgrdDetails.Rows.Clear();
            if (_dtDetails.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(_dtDetails.Rows.Count);
                int _index = 0;
                foreach (DataRow row in _dtDetails.Rows)
                {
                    dgrdDetails.Rows[_index].Cells["sno"].Value = (_index+1)+".";
                    dgrdDetails.Rows[_index].Cells["sid"].Value = row["SID"];               
                    dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    dgrdDetails.Rows[_index].Cells["description"].Value = row["Variant1"];                  
                    dgrdDetails.Rows[_index].Cells["gAmount"].Value = row["Amount"];    
                    _index++;
                }
            }          
        }

        private void EnableAllControls()
        {
            txtPurchaseInvoiceNo.ReadOnly= txtPDate.ReadOnly= txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            txtPurchaseInvoiceNo.ReadOnly = txtPDate.ReadOnly = txtDate.ReadOnly = txtRemark.ReadOnly = txtSignAmt.ReadOnly = txtOtherAmt.ReadOnly =  true;
        }

        private void ClearAllText()
        {
            strOldPartyName=txtPurchaseParty.Text = txtPurchaseInvoiceNo.Text = txtPurchaseType.Text = txtRemark.Text = strDeletedSID = lblMsg.Text = lblCreatedBy.Text =txtPurchaseInvoiceNo.Text= txtPurchaseInvoiceNo.Text=txtPBillNo.Text = "";
            txtDiscountAmt.Text = txtTaxPer.Text = txtTaxAmt.Text = txtOtherAmt.Text=  "0.00";
            lblQty.Text = lblGrossAmt.Text = lblNetAmt.Text = "0.00";         
            txtPBillCode.Text = strPurchaseBillCode;
            txtSignAmt.Text = "-";
            dgrdDetails.Rows.Clear();
            dgrdDetails.Rows.Add();
            dgrdDetails.Rows[0].Cells["sno"].Value = "1.";
            dgrdTax.Rows.Clear();
            pnlTax.Visible = false;


            if (DateTime.Today > MainPage.startFinDate && DateTime.Now <= MainPage.endFinDate)
                txtDate.Text =txtPDate.Text= DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = txtPDate.Text=MainPage.startFinDate.ToString("dd/MM/yyyy");
        }

        private void SetSerialNo()
        {
            try
            {
                if (txtBillCode.Text != "")
                {
                    DataTable table = DataBaseAccess.GetDataTableRecord("Select ISNULL(MAX(BillNo)+1,1)SNo  from [PurchaseReturn] Where BillCode='" + txtBillCode.Text + "' ");
                    if (table.Rows.Count > 0)
                    {                 
                            txtBillNo.Text = Convert.ToString(table.Rows[0]["SNo"]);                       
                    }
                }
            }
            catch 
            {              
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
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                        string strData = objRead.ReadDataFromCard("PURCHASEPARTY");
                        if (strData != "")
                        {
                            dgrdDetails.Rows.Clear();
                            txtPurchaseParty.Text = strData;

                            if (dgrdDetails.Rows.Count == 0)
                                dgrdDetails.Rows.Add();
                        }
                    }
                    else
                    {
                        char objChar = Convert.ToChar(e.KeyCode);
                        int value = e.KeyValue;
                        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                        {
                            SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH PARTY NAME", e.KeyCode);
                            objSearch.ShowDialog();
                            string strData = objSearch.strSelectedData;
                            if (strData != "")
                            {
                                if (dba.CheckTransactionLock(strData))
                                {
                                    MessageBox.Show("Transaction has been locked on this party ! Please select different supplier ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                                   
                                }
                                else
                                {
                                    dgrdDetails.Rows.Clear();
                                    txtPurchaseParty.Text = strData;                                  
                                    txtPBillNo.Clear();
                                    if (dgrdDetails.Rows.Count == 0)
                                        dgrdDetails.Rows.Add();
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

        private bool ValidateControls()
        {
            if (txtBillCode.Text == "")
            {
                MessageBox.Show("Sorry ! Bill code can't be blank !!", "Receipt code required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillCode.Focus();
                return false;
            }
            if (txtBillNo.Text == "")
            {
                MessageBox.Show("Sorry ! Bill no can't be blank !!", "Receipt no required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBillNo.Focus();
                return false;
            }           
            if (txtDate.Text.Length != 10)
            {
                MessageBox.Show("Sorry ! Please enter valid date  !!", "Date not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDate.Focus();
                return false;
            }
            if (txtPurchaseParty.Text == "")
            {
                MessageBox.Show("Sorry ! SUNDRY CREDITOR can't be blank !!", " SUNDRY CREDITOR required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseParty.Focus();
                return false;
            }          
          
            if (txtPurchaseType.Text == "" && MainPage._bTaxStatus)
            {
                MessageBox.Show("Sorry ! Purchase Type can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPurchaseType.Focus();
                return false;
            }
         
            double dAmt=0;
            string strPartyName = "", strItem = "", strDhara = "";

            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
               strItem = Convert.ToString(row.Cells["itemName"].Value);               
                dAmt = dba.ConvertObjectToDouble(row.Cells["gAmount"].Value);

                if (strItem == "" &&  dAmt == 0)
                    dgrdDetails.Rows.Remove(row);
                else
                {
                    if (strItem == "")
                    {
                        MessageBox.Show("Sorry ! Item name can't be blank", "Enter item name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["itemName"];
                        dgrdDetails.Focus();
                        return false;
                    }                   
                    if (dAmt == 0)
                    {
                        MessageBox.Show("Sorry ! Amount can't be blank", "Enter Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["gAmount"];
                        dgrdDetails.Focus();
                        return false;
                    }
                }
            }

            if (dgrdDetails.Rows.Count == 0)
            {
                dgrdDetails.Rows.Add();
                MessageBox.Show("Sorry ! Please add atleast one entry", "One Entry required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgrdDetails.Rows[0].Cells["sno"].Value = "1.";
                dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["itemName"];
                dgrdDetails.Focus();
                return false;
            }
            return ValidateOtherValidation(false) ;
        }      

        private bool ValidateOtherValidation(bool _bUpdateStatus)
        {

            DataTable dt = dba.GetDataTable("Select TransactionLock,State as SStateName,(Select TOP 1 StateName from CompanyDetails) CStateName,(Select TOP 1 Region from SaleTypeMaster Where SaleType='PURCHASE' and TaxName='" + txtPurchaseType.Text + "') Region,ISNULL((Select TOP 1 InsertStatus from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + "),1) InsertStatus,'FALSE' TickStatus  from SupplierMaster,CompanySetting CS  Where GroupName!='SUB PARTY' and (AreaCode+CAST(AccountNo as varchar)+' '+Name)='" + txtPurchaseParty.Text + "' ");
            if (dt.Rows.Count > 0)
            {
                if (Convert.ToBoolean(dt.Rows[0]["TransactionLock"]))
                {
                    MessageBox.Show("Transaction has been locked on this Account : " + txtPurchaseParty.Text + " ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (btnEdit.Text == "&Update" || _bUpdateStatus)
                {
                    if (strOldPartyName != txtPurchaseParty.Text || dOldNetAmt != Convert.ToDouble(lblNetAmt.Text) || _bUpdateStatus)
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
                    int check = dba.CheckPurchaseReturnAvailability(txtBillCode.Text, txtBillNo.Text);
                    if (check > 0)
                    {
                        string strBillNo = Convert.ToString(DataBaseAccess.ExecuteMyScalar("Select Max(BillNo)+1 from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' "));
                        MessageBox.Show("Sorry ! This Bill No is already Exist ! you are Late,  Bill Number  : " + strBillNo + "  is available ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private string GetEntryType()
        {
            return "CREDITNOTE";
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
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnEdit.Text = "&Edit";
                    }
                        
                    ClearAllText();
                    btnAdd.Text = "&Save";
                    SetSerialNo();
                    EnableAllControls();
                  
                    txtDate.Focus();
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled = false;
                    else
                        btnEdit.Enabled = btnDelete.Enabled = true;
                }
                else if (ValidateControls() && CheckBillNoAndSuggest())
                {
                    DialogResult result = MessageBox.Show("Are you sure want to save record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveRecord();
                    }
                }
            }
            catch
            {
            }
            btnAdd.Enabled = true ;
        }

        private void SaveRecord()
        {
            try
            {
                string strDate = "",strPDate="NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if(txtPDate.Text.Length==10 && txtPBillNo.Text!="")
                {
                    DateTime pDate = dba.ConvertDateInExactFormat(txtPDate.Text);
                    strPDate = "'"+pDate.ToString("MM/dd/yyyy hh:mm:ss")+"'";
                }

                string strPurchaseParty = "", strPurchasePartyID = "", strTaxAccountID="";
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }           

                double dAmt = 0,dTotalAmt = 0,dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "";

                strQuery += " if not exists (Select BillCode from [PurchaseReturn] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ) begin  "
                               + " INSERT INTO [dbo].[PurchaseReturn] ([BillCode],[BillNo],[Date],[PurchasePartyID],[EntryType],[PurchaseType],[Remark],[OtherSign],[OtherAmt],[NetDiscount],[TaxPer],[TaxAmount],[TotalQty],[GrossAmt],[NetAmt],[OtherText],[OtherValue],[CreatedBy],[UpdatedBy],[ReverseCharge],[InsertStatus],[UpdateStatus],[PurchaseBillCode],[PurchaseBillNo],[PurchaseBillDate],[RoundOffSign],[RoundOffAmt],[TaxableAmt]) VALUES "
                               + "  ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strDate + "','" + strPurchasePartyID + "','" + GetEntryType() + "','" + txtPurchaseType.Text + "','" + txtRemark.Text + "','" + txtSignAmt.Text + "'," + dba.ConvertObjectToDouble(txtOtherAmt.Text) + "," +
                               +dba.ConvertObjectToDouble(txtDiscountAmt.Text) + "," + dba.ConvertObjectToDouble(txtTaxPer.Text) + "," + dPTaxAmt + "," + dba.ConvertObjectToDouble(lblQty.Text) + "," + dba.ConvertObjectToDouble(lblGrossAmt.Text) + "," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'','','" + MainPage.strLoginName + "','','"+txtPurchaseInvoiceNo.Text+"',1,0,'"+txtPBillCode.Text+ "','" + txtPBillNo.Text + "'," + strPDate + ",'" + txtRoundOffSign.Text + "'," + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + "," + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ")  "
                               + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                               + " ('" + strDate + "','" + strPurchaseParty + "','CREDIT NOTE','CREDIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + lblNetAmt.Text + "','CR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,'" + strPurchasePartyID + "') ";


                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dTotalAmt=dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([BillCode],[BillNo],[RemoteID],[SRBillNo],[SalePartyID],[ItemName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[InsertStatus],[UpdateStatus],[Variant1]) VALUES  "
                             + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'','','" + rows.Cells["itemName"].Value + "','+',0,'NORMAL',1," + dAmt + ",0,0 ,0," + dTotalAmt + ",1,0,'" + rows.Cells["description"].Value + "') ";
                  
                }



                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";
                               
                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end end ";
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
                                   + " ('CREDITNOTE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value  + "','',1) ";// end ";
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('CREDITNOTE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'CREATION') ";
                
                strQuery += "  end";



                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You ! Record Saved Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);                               
                    btnAdd.Text = "&Add";
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
                string[] strReport = { "Exception occurred in Saving Record in Sale Return", ex.Message };
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
                        strDeletedSID = "";
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
                string strDate = "", strPDate = "NULL";
                DateTime bDate = dba.ConvertDateInExactFormat(txtDate.Text);
                strDate = bDate.ToString("MM/dd/yyyy hh:mm:ss");
                if (txtPDate.Text.Length == 10)
                {
                    DateTime pDate = dba.ConvertDateInExactFormat(txtPDate.Text);
                    strPDate = "'" + pDate.ToString("MM/dd/yyyy hh:mm:ss") + "'";
                }

                string  strPurchaseParty = "",  strPurchasePartyID = "", strTaxAccountID = "", strDeletedSIDQuery = ""; 
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                {
                    strPurchasePartyID = strFullName[0].Trim();
                    strPurchaseParty = txtPurchaseParty.Text.Replace(strPurchasePartyID + " ", "");
                }           
              

                double dAmt = 0,  dTotalAmt = 0, dPTaxAmt = dba.ConvertObjectToDouble(txtTaxAmt.Text);
                string strQuery = "",strID="";

                strQuery += "UPDATE  [dbo].[PurchaseReturn]  SET [Date]='" + strDate + "',[PurchasePartyID]='" + strPurchasePartyID + "',[PurchaseType]='" + txtPurchaseType.Text + "',[Remark]='" + txtRemark.Text + "',[OtherSign]='" + txtSignAmt.Text + "',[OtherAmt]=" + dba.ConvertObjectToDouble(txtOtherAmt.Text) + ",[ReverseCharge]='" + txtPurchaseInvoiceNo.Text + "',[TaxableAmt]=" + dba.ConvertObjectToDouble(lblTaxableAmt.Text) + ",[RoundOffSign]='" + txtRoundOffSign.Text + "',[RoundOffAmt]=" + dba.ConvertObjectToDouble(txtRoundOffAmt.Text) + ","
                         + " [NetDiscount]=" + dba.ConvertObjectToDouble(txtDiscountAmt.Text) + ",[TaxPer]=" + dba.ConvertObjectToDouble(txtTaxPer.Text) + ",[TaxAmount]=" + dPTaxAmt + ",[TotalQty]=" + dba.ConvertObjectToDouble(lblQty.Text) + ",[GrossAmt]=" + dba.ConvertObjectToDouble(lblGrossAmt.Text) + ",[NetAmt]=" + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[PurchaseBillCode]='"+txtPBillCode.Text+ "',[PurchaseBillNo]='" + txtPBillNo.Text + "',[PurchaseBillDate]=" + strPDate+ " Where [EntryType]='" + GetEntryType() + "' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                         + " UPDATE [dbo].[BalanceAmount] SET [Date]='" + strDate + "',[PartyName]='" + strPurchaseParty + "',[Amount]='" + lblNetAmt.Text + "',[UpdatedBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[AccountID]='" + strPurchasePartyID + "' Where [AccountStatus]='CREDIT NOTE' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "' "
                         + " Delete from BalanceAmount Where [AccountStatus]='DUTIES & TAXES' AND [Description]='" + txtBillCode.Text + " " + txtBillNo.Text + "'  "
                         + " Delete from [dbo].[GSTDetails] Where [BillType]='CREDITNOTE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " ";

               
                foreach (DataGridViewRow rows in dgrdDetails.Rows)
                {
                    dTotalAmt = dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                    strID = Convert.ToString(rows.Cells["sid"].Value);                 

                    if (strID == "")
                    {                       
                        strQuery += " INSERT INTO [dbo].[PurchaseReturnDetails] ([BillCode],[BillNo],[RemoteID],[SRBillNo],[SalePartyID],[ItemName],[DisStatus],[Discount],[Dhara],[Qty],[Amount],[Packing],[Freight],[TaxFree],[TotalAmt],[InsertStatus],[UpdateStatus],[Variant1]) VALUES  "
                                 + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",0,'','','" + rows.Cells["itemName"].Value + "','+',0,'NORMAL',1," + dAmt + ",0,0 ,0," + dTotalAmt + ",1,0,'" + rows.Cells["description"].Value + "') ";
                    }
                    else
                    {
                        strQuery += "Update [dbo].[PurchaseReturnDetails] SET [ItemName]='" + rows.Cells["itemName"].Value + "',[Amount]=" + dAmt + ",[TotalAmt]=" + dTotalAmt + ",[UpdateStatus]=1 Where [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " and SID=" + strID + " ";
                    }

                 
                }

                strQuery += " Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;";

                if (dPTaxAmt > 0 && txtPurchaseType.Text != "")
                {
                    strQuery += " Select @Region=Region,@IGSTName = TaxAccountIGST, @SGSTName = TaxAccountSGST from SaleTypeMaster Where SaleType='PURCHASE' and TaxName = '" + txtPurchaseType.Text + "'; "
                             + " if(@IGSTName!='' OR @SGSTName!='') begin if(@Region='LOCAL') begin  if(@IGSTName=@SGSTName) begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end else begin"
                             + " if(@IGSTName!='') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; "
                             + " INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end "
                             + " if (@SGSTName != '') begin Select @SGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@SGSTName;  INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@SGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt / 2 + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@SGSTName) end end end "
                             + " else if (@IGSTName != '') begin Select @IGSTFullName =Name from SupplierMaster Where (AreaCode+CAST(AccountNo as nvarchar))=@IGSTName; INSERT INTO [dbo].[BalanceAmount] ([Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[AmountStatus],[FinalAmount],[JournalID],[Tick],[VoucherCode],[VoucherNo],[UserName],[UpdatedBy],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID]) VALUES  "
                             + " ('" + strDate + "',@IGSTFullName,'DUTIES & TAXES','DEBIT','" + txtBillCode.Text + " " + txtBillNo.Text + "','" + dPTaxAmt + "','DR','0','0','False','',0,'" + MainPage.strLoginName + "','',0,0,0,@IGSTName) end ";
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
                             + " ('CREDITNOTE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + strTaxAccountID + "','" + rows.Cells["taxRate"].Value + "'," + dba.ConvertObjectToDouble(rows.Cells["taxAmt"].Value) + ",'" + rows.Cells["taxType"].Value + "','',1) ";// end ";
                }

                object objValue = "True";
                if (strDeletedSID != "")
                {
                    strQuery += " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and SID in (" + strDeletedSID + ") ";
                  

                    strDeletedSIDQuery += " Delete from [dbo].[PurchaseReturnDetails] WHERE BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " and RemoteID in (" + strDeletedSID + ") ";
                
                    objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(InsertStatus,1) from PurchaseReturn Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");
                }

                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('CREDITNOTE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dba.ConvertObjectToDouble(lblNetAmt.Text) + ",'" + MainPage.strLoginName + "',1,0,'UPDATION') ";


                strQuery += " end ";      

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    if (!Convert.ToBoolean(objValue))
                    {
                        strQuery = strQuery.Replace("Declare @Region nvarchar(50),@IGSTName nvarchar(250),@SGSTName nvarchar(250),@IGSTFullName nvarchar(250),@SGSTFullName nvarchar(250),@BillCode nvarchar(50) ;", "");
                        DataBaseAccess.CreateDeleteQuery(strQuery);
                    }
                    MessageBox.Show("Thank You ! Record updated Successfully !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    btnEdit.Text = "&Edit";
                    updateStatus = true;
                    if (saleStatus)
                        this.Close();
                    else
                    {
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
                string[] strReport = { "Exception occurred in Updating Record in Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text != "&Save")
            {
                pnlDeletionConfirmation.Visible = true;
                txtReason.Focus();
            }         
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnAdd.Text = "&Add";
            btnEdit.Text = "&Edit";
            txtBillNo.ReadOnly = false;
            BindLastRecord();
        }

        private void CheckAvailability()
        {
            try
            {
                if (btnAdd.Text == "&Save")
                {
                    if (txtBillNo.Text != "")
                    {                        
                            int check = dba.CheckPurchaseReturnAvailability(txtBillCode.Text, txtBillNo.Text);
                            if (check < 1)
                            {
                                lblMsg.Text = txtBillNo.Text + "  Bill No is Available ........";
                                lblMsg.ForeColor = Color.White;
                                lblMsg.Visible = true;

                            }
                            else
                            {
                                lblMsg.Text = txtBillNo.Text + " Bill No is already exist ! ";
                                lblMsg.ForeColor = Color.White;
                                lblMsg.Visible = true;
                                txtBillNo.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("This Bill No is already in used please Choose Different Bill No..", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtBillNo.Focus();

                        }                  
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Chaeck Availability in Purchase Return ", ex.Message };
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
                    else if (btnAdd.Text=="&Add" && btnEdit.Text == "&Edit")
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
            }
            catch
            {
            }
        }

        private void EditOption()
        {
            try
            {
                if (MainPage.mymainObject.bDrCrNoteAdd || MainPage.mymainObject.bDrCrNoteEdit || MainPage.mymainObject.bDrCrNoteView)
                {
                    if (!MainPage.mymainObject.bDrCrNoteAdd)
                        btnAdd.Enabled = false;
                    if (!MainPage.mymainObject.bDrCrNoteEdit)
                        btnEdit.Enabled = btnDelete.Enabled =  false;
                    if (!MainPage.mymainObject.bDrCrNoteView)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            if (cIndex == 2 ||cIndex ==3 )
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.CharacterCasing = CharacterCasing.Upper;
                if (cIndex == 3)
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int cIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (cIndex == 3)
                {
                    Char pressedKey = e.KeyChar;
                    if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                        e.Handled = false;
                    else
                    {
                        dba.KeyHandlerPoint(sender, e, 2);
                    }
                }
            }
            catch { }
        }

        private void dgrdItem_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int cIndex = e.ColumnIndex;
                if (cIndex == 3)
                {
                    CalculateTotalAmount();                    
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

        private void txtTaxPer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape)
            {
                if (!pnlTax.Visible)
                    pnlTax.Visible = true;
                else
                    pnlTax.Visible = false;
            }
        }

        private void txtOtherAmt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                if (btnAdd.Text == "&Save")
                    btnAdd.Focus();
                else if (btnEdit.Text == "&Update")
                    btnEdit.Focus();
            }
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

        private void CalculateTotalAmount()
        {
            double dAmt = 0, dTAmt = 0;

            foreach (DataGridViewRow rows in dgrdDetails.Rows)
            {              
                dTAmt += dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
            }

            txtDiscountAmt.Text = "0.00";       
            lblGrossAmt.Text = dTAmt.ToString("N2", MainPage.indianCurancy);
            CalculateNetAmount();
        }
       
        private void CalculateNetAmount()
        {
           
            double dDiscount = 0,dPackingAmt=0, dOtherAmt = 0, dRoundOffAmt = 0, dTaxableAmt = 0, dGrossAmt = 0, dNetAmt = 0, dTOAmt = 0, dTaxAmt = 0, dFinalAmt = 0;
            try
            {   
       
                dOtherAmt = dba.ConvertObjectToDouble(txtSignAmt.Text + txtOtherAmt.Text);
                dGrossAmt = dba.ConvertObjectToDouble(lblGrossAmt.Text);
                dDiscount = dba.ConvertObjectToDouble(txtDiscountAmt.Text);               

                dTOAmt = dOtherAmt + dPackingAmt ;
                dFinalAmt = dGrossAmt + dDiscount + dTOAmt;
                dTaxAmt = GetTaxAmount(dFinalAmt, dTOAmt,ref dTaxableAmt);  
                dNetAmt = dGrossAmt + dDiscount + dOtherAmt + dPackingAmt + dTaxAmt;
                double dNNetAmt = Convert.ToDouble(dNetAmt.ToString("0"));// Math.Round(dNetAmt, 0);
                dRoundOffAmt = (dNNetAmt - dNetAmt);

                if (dRoundOffAmt >= 0)
                {
                    txtRoundOffSign.Text = "+";
                    txtRoundOffAmt.Text = dRoundOffAmt.ToString("0.00");
                }
                else
                {
                    txtRoundOffSign.Text = "-";
                    txtRoundOffAmt.Text = Math.Abs(dRoundOffAmt).ToString("0.00");
                }

                lblNetAmt.Text = dNetAmt.ToString("N0", MainPage.indianCurancy);

                if (dTaxableAmt > 0)
                    lblTaxableAmt.Text = dTaxableAmt.ToString("N2", MainPage.indianCurancy);
                else
                    lblTaxableAmt.Text = dNetAmt.ToString("N2", MainPage.indianCurancy);
            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Sale Return", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }


        private double GetTaxAmount(double dFinalAmt, double dOtherAmt,ref double dTaxableAmt)
        {
            double dTaxAmt = 0,  dTaxPer = 0,dServiceAmt=0;
            string _strTaxType = "";
            try
            {
                if (MainPage._bTaxStatus && txtPurchaseType.Text != "" && dgrdDetails.Rows.Count > 0)
                {
                    dgrdTax.Rows.Clear();
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

                            string strQuery = "",  strServiceQuery="",strItemName="";
                            double dDisStatus = 0;                          

                            double dAmt = 0, dQty = 0, dPacking = 0;
                            foreach (DataGridViewRow rows in dgrdDetails.Rows)
                            {
                                dAmt = dba.ConvertObjectToDouble(rows.Cells["gAmount"].Value);
                                dQty = 1;
                                if (dAmt > 0)
                                {
                                    strItemName = Convert.ToString(rows.Cells["itemName"].Value);

                                    if (strQuery != "")
                                    {
                                        strQuery += " UNION ALL ";
                                        strServiceQuery += " UNION ALL ";
                                    }

                                    strQuery += " Select '' as ID, (GM.Other + ' : ' + GM.HSNCode) as HSNCode," + dQty + " as Quantity,ROUND((((" + dAmt + " )*(100 + " + dDisStatus + "))/ 100.00),4)Amount,GM.TaxRate from Items _IM Outer APPLY (Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + " * 100) / (100 + TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/ " + dQty + ")> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN '" + _strTaxType + "' = 'INCLUDED' then((" + dAmt + "* 100) / (100 + TaxRate)) else " + dAmt + " end)))*(CASE WHEN _TC.AmountType='NET PRICE' then ((100.00 + " + dDisStatus + ") / 100.00) else 1.00 end))/" + dQty + ")< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where _IM.ItemName='" + strItemName + "' and " + dAmt + ">0  ";
                                }
                            }

                            if (strQuery != "")
                            {
                                dPacking += dOtherAmt;
                                if (dPacking != 0)
                                {
                                    if (strQuery != "")
                                        strQuery += " UNION ALL ";
                                    strQuery += " Select '' as ID,'' as HSNCode,0 as Quantity, " + dPacking + " Amount,0 as TaxRate ";
                                }
                                if (strQuery != "")
                                {
                                    strQuery = " Select SUM(Amount)TaxableAmt,SUM(ROUND(((Amount*TaxRate)/100.00),4)) as Amt,TaxRate from ( Select HSNCode,(Amount* (CASE WHen '" + _strTaxType + "' = 'INCLUDED' then(100/(100+TaxRate)) else 1 end))Amount,Qty,TaxRate from ("
                                                   + " Select HSNCode, SUM(Amount)Amount, SUM(Quantity) Qty,(CASE WHEN HSNCode='' and SUM(Quantity)=0 and TaxRate=0 then  MAX(TaxRate) OVER(PARTITION BY ID) else TaxRate end)TaxRate  from ( " + strQuery + ")_Goods Group by ID,HSNCode,TaxRate)Goods )_Goods Where Amount!=0 and TaxRate>0 Group by TaxRate";
                                                   //+ " OUTER APPLY (Select (SUM(Amount)+(CASE WHen '" + _strTaxType + "'='INCLUDED' then ((" + dPacking + "*_Goods.MTaxRate)/100.00) else 0 end)) ServiceAmt   from ( "
                                                   //+ " Select (((((Amount)*TaxRate/100.00)*TaxDhara)/100.00)*(CASE WHen '" + _strTaxType + "'='INCLUDED' then ((100+_Goods.MTaxRate)/100.00) else 1 end)) Amount from ( " + strServiceQuery
                                                   //+ " )_Sales OUTER APPLY (Select TOP 1 TaxDhara from CompanySetting) CS Outer APPLY (Select TOP 1 _IGM.Other,_IGM.HSNCode,(CASE WHEN _TC.ChangeTaxRate=1 then (CASE WHEN _TC.GreaterORSmaller='>' then (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((_Sales.Amount*100)/(100+TaxRate)) else _Sales.Amount end))))/Quantity)>_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN ((((((CASE WHEN '" + _strTaxType + "'='INCLUDED' then ((_Sales.Amount*100)/(100+TaxRate)) else _Sales.Amount end))))/Quantity)<_TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from Items _IM left join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName and _IGM.ParentGroup='' left join TaxCategory _TC on _IGM.TaxCategoryName=_TC.CategoryName Where _Sales.ItemName=_IM.ItemName) as GM  ) _Sales) SCharge)_FinalSales ";


                                    DataTable dt = dba.GetDataTable(strQuery);
                                    if (dt.Rows.Count > 0)
                                    {
                                        double dMaxRate = 0, dTTaxAmt = 0;
                                        //BindTaxDetails(dt, row, ref dMaxRate, ref dTTaxAmt,ref dTaxableAmt);
                                        dba.BindTaxDetails(dgrdTax, dt, row, ref dMaxRate, ref dTTaxAmt, ref dTaxableAmt);
                                        dTaxAmt = dTTaxAmt;
                                        dTaxPer = dMaxRate;
                                       

                                        pnlTax.Visible = true;
                                    }
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
                            txtTaxAmt.Text = txtTaxPer.Text =  "0.00";
                    }
                }

                btnEdit.Enabled = btnAdd.Enabled = true;
                if (!MainPage.mymainObject.bDrCrNoteAdd)
                    btnAdd.Enabled = false;
                if (!MainPage.mymainObject.bDrCrNoteEdit)
                    btnEdit.Enabled = false;

            }
            catch (Exception ex)
            {
                string[] strReport = { "TAX CALCULATION : Sale Return Book", ex.Message };
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

        //private void BindTaxDetails(DataTable _dt, DataRow _row,ref double dMaxRate,ref double dTTaxAmt,ref double dTaxableAmt)
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

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                    {
                        if (e.ColumnIndex == 1)
                        {
                            SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                string[] strItem = objSearch.strSelectedData.Split('|');
                                if (strItem.Length > 0)
                                    dgrdDetails.CurrentCell.Value = strItem[0];
                            }
                            CalculateTotalAmount();
                            e.Cancel = true;
                        }
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
   
        private void txtOtherAmt_Enter_1(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "0.00")
                        txt.Clear();
                }
            }
        }

        private void txtPDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                //dba.GetStringFromDateForCompany(txtPDate);
                dba.GetDateInExactFormat(sender, true, false,false);
        }

        private void txtPBillNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((btnAdd.Text == "&Save" || btnEdit.Text == "&Update"))
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        string strQuery = "";
                        if (txtPurchaseParty.Text != "")
                        {
                            string[] strFullName = txtPurchaseParty.Text.Split(' ');
                            if (strFullName.Length > 1)
                                strQuery = " Where PurchasePartyID ='" + strFullName[0].Trim() + "'  ";

                            SearchData objSearch = new SearchData("PURCHASEBILLNOFORMPURCHASE_CREDITNOTE", strQuery, "SEARCH PURCHASE BILL NO", e.KeyCode);
                            objSearch.ShowDialog();
                            if (objSearch.strSelectedData != "")
                            {
                                string[] strData = objSearch.strSelectedData.Split('|');
                                txtPBillNo.Text = strData[0];
                                if (strData.Length > 1)
                                {
                                    txtPDate.Text = strData[1];
                                }
                            }

                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    GSTPrintAndPreview(false, "");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Purchase Return", ex.Message };
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
                    if (txtPurchaseType.Text != "")
                    {
                        GSTPrintAndPreview(true, "");
                    }
                }
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }

        private void txtOtherAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void txtPurchaseParty_DoubleClick(object sender, EventArgs e)
        {
            DataBaseAccess.OpenPartyMaster(txtPurchaseParty.Text);
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text != "&Save" && dba.ValidateBackDateEntry(txtDate.Text))
                {
                    if (txtReason.Text != "" && ValidateOtherValidation(true))
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes && txtBillCode.Text != "" && txtBillNo.Text != "")
                        {
                            string strQuery = "";

                            strQuery += " Delete from [PurchaseReturn] Where [BillCode]='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text
                                     + " Delete from [PurchaseReturnDetails] Where BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " "
                                     + " Delete from BalanceAmount Where AccountStatus in ('CREDIT NOTE','DUTIES & TAXES') and Description in ('" + txtBillCode.Text + " " + txtBillNo.Text + "') "
                                     + " Delete from [dbo].[GSTDetails] Where [BillType]='CREDITNOTE' and [BillCode]='" + txtBillCode.Text + "' and [BillNo]=" + txtBillNo.Text + " "
                                     + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                     + " ('CREDITNOTE','" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + txtReason.Text + ", With Amt : " + lblNetAmt.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            object objStatus = DataBaseAccess.ExecuteMyScalar("Select InsertStatus from PurchaseReturn Where  BillCode='" + txtBillCode.Text + "' and BillNo=" + txtBillNo.Text + " ");

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                if (!Convert.ToBoolean(objStatus))
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
                        SearchData objSearch = new SearchData("CREDITNOTECODE", "SEARCH CREDIT NOTE CODE", e.KeyCode);
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

        private void lblCreatedBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (lblCreatedBy.Text.Length > 10 && txtBillCode.Text != "" && txtBillNo.Text != "" && btnAdd.Text != "&Save")
                {
                    EditTrailDetails objEdit = new EditTrailDetails("CREDITNOTE", txtBillCode.Text, txtBillNo.Text);

                    objEdit.ShowDialog();
                }
            }
            catch { }
        }

        private void txtPBillCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtOtherAmt_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                TextBox txt = (TextBox)sender;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0.00";
                    CalculateNetAmount();
                }
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int CurrentRow = 0;
                int IndexColmn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdDetails.CurrentCell.RowIndex;
                    IndexColmn = dgrdDetails.CurrentCell.ColumnIndex;
                    if (Index < dgrdDetails.RowCount - 1)
                        CurrentRow = Index - 1;
                    else
                        CurrentRow = Index;

                    if (IndexColmn < dgrdDetails.ColumnCount - 2)
                    {
                        IndexColmn += 1;
                        if (CurrentRow >= 0)
                            dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow].Cells[IndexColmn];
                    }
                    else if (Index == dgrdDetails.RowCount - 1)
                    {
                        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                        {
                            if (Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["itemName"].Value) != "" && Convert.ToString(dgrdDetails.Rows[CurrentRow].Cells["gAmount"].Value) != "")
                            {
                                dgrdDetails.Rows.Add(1);
                                dgrdDetails.CurrentCell = dgrdDetails.Rows[CurrentRow + 1].Cells["itemName"];
                            }
                            else
                            {
                                if (btnAdd.Text == "&Save")
                                    btnAdd.Focus();
                                else
                                    btnEdit.Focus();
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    Index = dgrdDetails.CurrentCell.RowIndex;
                    if (btnAdd.Text == "&Save")
                    {
                        dgrdDetails.Rows.RemoveAt(Index);
                        CalculateTotalAmount();
                    }
                    else if (btnEdit.Text == "&Update")
                    {
                        string strID = Convert.ToString(dgrdDetails.CurrentRow.Cells["sid"].Value);
                        if (strID != "")
                        {
                            DialogResult result = MessageBox.Show("Are you sure want to delete current row ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                if (strDeletedSID != "")
                                    strDeletedSID += ",";
                                strDeletedSID += strID;
                                dgrdDetails.Rows.RemoveAt(Index);
                                CalculateTotalAmount();
                            }
                        }
                        else
                        {
                            dgrdDetails.Rows.RemoveAt(Index);
                            CalculateTotalAmount();
                        }
                    }

                }
            }
            catch
            {
            }
        }       

        private void GSTPrintAndPreview(bool _pstatus, string strPath)
        {
            DataTable _dtGST = null, _dtSalesAmt = null; ;
            bool _bIGST = false;
            string msgToShow = "";
            DataTable dt = dba.CreateDebitNoteDataTable(txtBillCode.Text, txtBillNo.Text, ref _dtGST, ref _bIGST, ref _dtSalesAmt, "CREDIT NOTE", "DEBIT NOTE");
            if (dt.Rows.Count > 0)
            {
                if (!_bIGST)
                {
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        Reporting.DCNoteReport_CGST objOL_salebill = new Reporting.DCNoteReport_CGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        msgToShow = "CREDIT NOTE REPORT PREVIEW";
                        FinallyPrint(_pstatus, objOL_salebill, strPath, msgToShow);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("CREDIT NOTE REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //objOL_salebill.Close();
                        //objOL_salebill.Dispose();
                    }
                    else
                    {
                        Reporting.DCNoteReport_CGST_Retail objOL_salebill = new Reporting.DCNoteReport_CGST_Retail();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        msgToShow = "PURCHASE RETURN REPORT PREVIEW";
                        FinallyPrint(_pstatus, objOL_salebill, strPath, msgToShow);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //    objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("PURCHASE RETURN REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //objOL_salebill.Close();
                        //objOL_salebill.Dispose();
                    }
                }
                else
                {
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        Reporting.DCNoteReport_IGST objOL_salebill = new Reporting.DCNoteReport_IGST();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        msgToShow = "CREDIT NOTE REPORT PREVIEW";
                        FinallyPrint(_pstatus, objOL_salebill, strPath, msgToShow);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("CREDIT NOTE REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //objOL_salebill.Close();
                        //objOL_salebill.Dispose();
                    }
                    else
                    {
                        Reporting.DCNoteReport_IGST_Retail objOL_salebill = new Reporting.DCNoteReport_IGST_Retail();
                        objOL_salebill.SetDataSource(dt);
                        objOL_salebill.Subreports[0].SetDataSource(_dtGST);
                        objOL_salebill.Subreports[1].SetDataSource(_dtSalesAmt);
                        msgToShow = "CREDIT NOTE REPORT PREVIEW";
                        FinallyPrint(_pstatus, objOL_salebill, strPath, msgToShow);
                        objOL_salebill.Close();
                        objOL_salebill.Dispose();
                        //    objOL_salebill.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //    if (strPath != "")
                        //    {
                        //        objOL_salebill.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
                        //    }
                        //    else
                        //    {
                        //        if (_pstatus)
                        //        {
                        //            // string strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                        //            if (strValue != "" && strValue != "0")
                        //            {
                        //                int nCopy = Int32.Parse(strValue);
                        //                objOL_salebill.PrintToPrinter(nCopy, false, 0, 0);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Reporting.ShowReport objReport = new Reporting.ShowReport("CREDIT NOTE REPORT PREVIEW");
                        //            objReport.myPreview.ReportSource = objOL_salebill;
                        //            objReport.ShowDialog();
                        //        }
                        //    }
                        //objOL_salebill.Close();
                        //objOL_salebill.Dispose();
                    }
                }
            }
        }

        private void FinallyPrint(bool _pstatus, CrystalDecisions.CrystalReports.Engine.ReportClass Report,string strPath,string msg)
        {
            Report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            if (strPath != "")
            {
                Report.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);
            }
            else
            {
                if (_pstatus)
                {
                    if (MainPage._PrintWithDialog)
                        dba.PrintWithDialog(Report);
                    else
                    {
                        string strValue = "0";
                        if (_pstatus)
                        {
                            strValue = Microsoft.VisualBasic.Interaction.InputBox("ENTER NO. OF COPIES TO PRINT  ! ", "Number of Copies", "2", 400, 300);
                            if (strValue != "" && strValue != "0")
                            {
                                int nCopy = Int32.Parse(strValue);
                                Report.PrintToPrinter(nCopy, false, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    Reporting.ShowReport objReport = new Reporting.ShowReport(msg);
                    objReport.myPreview.ReportSource = Report;
                    objReport.ShowDialog();
                }
            }
            Report.Close();
            Report.Dispose();
        }
    }
}
