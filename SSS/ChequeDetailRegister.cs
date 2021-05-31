using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class ChequeDetailRegister : Form
    {
        DataBaseAccess dba;
        public ChequeDetailRegister()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtDepositeDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtFromDate.Text =txtFromDD.Text= MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text =txtToDDate.Text= MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        public ChequeDetailRegister(string strCreditedAccount,string strChqType)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();
                txtDepositeDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                txtFromDate.Text = txtFromDD.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtToDate.Text = txtToDDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

                txtPartyName.Text = strCreditedAccount;
                if (strChqType == "SECURITY")
                    rdoChqTypeSecurity.Checked = true;
                else if (strChqType == "PDC")
                    rdoChqTypePDC.Checked = true;
                else
                    rdoChqTypeAll.Checked = true;
                GetAllData();
            }
            catch { }
        }


        private void ChequeDetailRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtBankAccount_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBankAccount.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void btnName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("BANKPARTY", "SEARCH BANK A/C", Keys.Space);
                objSearch.ShowDialog();
                txtBankAccount.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH ACCOUNT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void btnPartyName_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH ACCOUNT NAME", Keys.Space);
                objSearch.ShowDialog();
                txtPartyName.Text = objSearch.strSelectedData;
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkDepositeDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDD.Enabled = txtToDDate.Enabled = chkDepositeDate.Checked;
            txtFromDD.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false,true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtVoucherCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("CHEQUEBOOKVOUCHERCODE", "SEARCH VOUCHER CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtVoucherCode.Text = objSearch.strSelectedData;
                }

                e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                GetAllData();
            }
            catch
            {
            }
            btnGO.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (chkDate.Checked)
            {
                DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDate.Text), toDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strQuery += " and Date>='" + fromDate + "' and Date<'" + toDate.AddDays(1) + "' ";
            }
            if (chkDepositeDate.Checked)
            {
                DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDD.Text), toDate = dba.ConvertDateInExactFormat(txtToDDate.Text);
                strQuery += " and DepositeDate>='" + fromDate + "' and DepositeDate<'" + toDate.AddDays(1) + "' ";
            }

            string[] strFullName;
            if (txtBankAccount.Text != "")
            {
                strFullName = txtBankAccount.Text.Split(' ');
                if (strFullName.Length > 1)
                    strQuery += " and (DebitAccountID='" + strFullName[0].Trim() + "' OR CreditAccountID='" + strFullName[0].Trim() + "') ";
            }
            if (txtPartyName.Text != "")
            {
                strFullName = txtPartyName.Text.Split(' ');
                if (strFullName.Length > 1)
                    strQuery += " and (DebitAccountID='" + strFullName[0].Trim() + "' OR CreditAccountID='" + strFullName[0].Trim() + "') ";
            }

            if(txtChqNo.Text!="")
                strQuery += " and [Description] Like('%"+txtChqNo.Text+"%')  ";

            if (rdoChqTypeSecurity.Checked)
                strQuery += " and [ChequeType]='SECURITY' ";
            else if (rdoChqTypePDC.Checked)
                strQuery += " and [ChequeType]='PDC' ";

            if (rdoPending.Checked)
                strQuery += " and [Status]='PENDING' ";
            else if (rdoClear.Checked)
                strQuery += " and [Status]='CLEAR' ";
            else if (rdoOverDue.Checked)
                strQuery += " and ([Status]='PENDING' and [DepositeDate]>'"+MainPage.currentDate.ToString("MM/dd/yyyy")+"' ) ";

            return strQuery;
        }

        private void GetAllData()
        {
            if (rdoChqTypePDC.Checked || txtChqNo.Text.Length > 3 || MainPage.mymainObject.bSecurityChequePermission)
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (chkDepositeDate.Checked && (txtFromDD.Text.Length != 10 || txtToDDate.Text.Length != 10))
                    MessageBox.Show("Sorry ! Please enter deposite date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    string strQuery = "", strSubQuery = CreateQuery();
                    strQuery = " Select *,Convert(varchar,Date,103)BDate,Convert(varchar,DepositeDate,103)DDate,(DebitAccountID+' '+DName) as DebitParty,(CreditAccountID+' '+CName) as CreditParty from ChequeDetails CD OUTER APPLY (Select Name DName from SupplierMaster Where AreaCode+AccountNo=DebitAccountID)SM OUTER APPLY (Select Name CName from SupplierMaster Where AreaCode+AccountNo=CreditAccountID)SM1 Where BillCode!='' " + strSubQuery + " Order by Date,BillNo desc ";

                    DataTable dt = dba.GetDataTable(strQuery);
                    BindRecordWithGrid(dt);
                }
            }
            else
            {
                MessageBox.Show("Sorry ! You don't have sufficient permissions to access security cheque details !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BindRecordWithGrid(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            chkAll.Checked = false;
            lblTotalAmt.Text = "0";
            double dAmt = 0, dTotalAmt = 0;
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                string strStatus = "",strDDate="";
                foreach (DataRow row in dt.Rows)
                {
                    dTotalAmt += dAmt = dba.ConvertObjectToDouble(row["Amount"]);
                    strStatus = Convert.ToString(row["Status"]);
                    strDDate = Convert.ToString(row["DDate"]);

                    dgrdDetails.Rows[rowIndex].Cells["chkTick"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["voucherno"].Value = row["BillCode"] + " " + row["BillNo"];
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                    dgrdDetails.Rows[rowIndex].Cells["debitAccount"].Value = row["DebitParty"];
                    dgrdDetails.Rows[rowIndex].Cells["creditAccount"].Value = row["CreditParty"];
                    dgrdDetails.Rows[rowIndex].Cells["chqType"].Value = row["ChequeType"];
                    if (Convert.ToString(row["DepositeDate"]) != "" && !strDDate.Contains("1900"))
                        dgrdDetails.Rows[rowIndex].Cells["depositeDate"].Value = strDDate;
                    dgrdDetails.Rows[rowIndex].Cells["description"].Value = row["Description"];
                    dgrdDetails.Rows[rowIndex].Cells["amount"].Value = dAmt;
                    dgrdDetails.Rows[rowIndex].Cells["Status"].Value = strStatus;
                    dgrdDetails.Rows[rowIndex].Cells["bankName"].Value = row["BankName"];
                    dgrdDetails.Rows[rowIndex].Cells["branchName"].Value = row["BranchName"];
                    dgrdDetails.Rows[rowIndex].Cells["firmName"].Value = row["FirmName"];
                    dgrdDetails.Rows[rowIndex].Cells["chequeNo"].Value = row["ChequeNo"];

                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];

                    if (strStatus == "DEPOSITED")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gold;
                    else if (strStatus == "CLEAR")
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (strDDate.Length==10 && !strDDate.Contains("1900"))
                    {
                        if (strStatus == "PENDING" && MainPage.currentDate > dba.ConvertDateInExactFormat(strDDate))
                        {
                            dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                            dgrdDetails.Rows[rowIndex].Cells["Status"].Value = "OVERDUE";
                        }
                    }

                    rowIndex++;
                }
            }
            lblTotalAmt.Text = dTotalAmt.ToString("N0", MainPage.indianCurancy);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeposite_Click(object sender, EventArgs e)
        {
            btnDeposite.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    DialogResult reuslt = MessageBox.Show("Are you sure you want to deposite these selected cheques !!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (DialogResult.Yes == reuslt)
                    {
                        SaveRecord();
                    }                     
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry !! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnDeposite.Enabled = true;
        }
              
        private void SaveRecord()
        {
            string strQuery = "", strDate = "", strDebitParty = "", strCreditParty = "", strDebitID = "", strCreditID = "", strDescription = "", strID = "", strChqNo = "" ;
            double dAmt = 0;
            DateTime _date = dba.ConvertDateInExactFormat(txtDepositeDate.Text);
            strDate = _date.ToString("MM/dd/yyyy");

            strQuery = " Declare @SerialNo bigint, @SerialCode nvarchar(250),@AccountID nvarchar(250),@AccountName nvarchar(250); Select TOP 1 @SerialCode=BankVCode from CompanySetting ";
            string[] strFullParty;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToBoolean(row.Cells["chkTick"].Value))
                {
                    strDebitParty = Convert.ToString(row.Cells["debitAccount"].Value);
                    strCreditParty = Convert.ToString(row.Cells["creditAccount"].Value);
                    strDescription = Convert.ToString(row.Cells["description"].Value);
                    strChqNo = Convert.ToString(row.Cells["chequeNo"].Value);
                    
                    strID = Convert.ToString(row.Cells["id"].Value);
                    strFullParty = strDebitParty.Split(' ');
                    strDebitID = strFullParty[0];
                    strDebitParty = strDebitParty.Replace(strDebitID + " ", "");

                    strFullParty = strCreditParty.Split(' ');
                    strCreditID = strFullParty[0];
                    strCreditParty = strCreditParty.Replace(strCreditID + " ", "");

                    dAmt = dba.ConvertObjectToDouble(row.Cells["amount"].Value);
                    if (dAmt > 0)
                    {
                        strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode=@SerialCode ";


                        strQuery += "if not exists (Select BalanceID from BalanceAmount Where Description='" + strDescription + "' and Convert(nvarchar,Date,103)='" + _date.ToString("dd/MM/yyyy") + "' and CAST(Amount as Money)=" + dAmt.ToString("0.00") + " and [AccountID]='"+strDebitID+ "' and [AccountStatusID]='" + strCreditID + "') begin INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus]) VALUES "
                                 + " (@SerialCode,@SerialNo,'" + strDate + "','" + strDebitParty + "','" + strCreditParty + "','DEBIT','" + (strChqNo + " " + strDescription).Trim() + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strDebitID + "','" + strCreditID + "',0) "
                                 + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus]) VALUES "
                                 + " (@SerialCode,@SerialNo,'" + strDate + "','" + strCreditParty + "','" + strDebitParty + "','CREDIT','" + (strChqNo+" "+strDescription).Trim() + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strCreditID + "','" + strDebitID + "',0) "
                                 + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + "('BANK',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION')  "
                                 + " Update [ChequeDetails] Set Status='DEPOSITED',UpdateStatus=1 Where (BillCode+' '+CAST(BillNo as varchar))='"+ row.Cells["voucherno"].Value+"' and ID="+strID+" end ";
                    }
                    else
                    {
                        strQuery = "";
                        MessageBox.Show("Sorry ! please enter amount in voucher no : " + row.Cells["voucherno"].Value + ", after that you can deposite cheque !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgrdDetails.CurrentCell = row.Cells["voucherno"];
                        dgrdDetails.Focus();
                        break;
                    }

                }
            }
            int count = 0;
            if (strQuery != "")
            {
                count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank you !! Cheque deposited successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    dgrdDetails.DataSource = null;
                }
                else
                {
                    MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void txtDepositeDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void ChequeDetailRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
            if (!MainPage.mymainObject.bCashAdd)
                btnDeposite.Enabled = false;

            if (!MainPage.mymainObject.bCashView)
            {
                MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void txtChqNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 && e.RowIndex >= 0)
                {
                    string strInvoiceNo = Convert.ToString(dgrdDetails.CurrentCell.Value);

                    string[] strNumber = strInvoiceNo.Split(' ');
                    if (strNumber.Length > 1)
                    {
                        ChequeDetails objSale = new ChequeDetails(strNumber[0], strNumber[1]);
                        objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                        objSale.ShowInTaskbar = true;
                        objSale.Show();
                    }
                }
            }
            catch { }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnExport.Enabled = false;


                        Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                        object misValue = System.Reflection.Missing.Value;
                        Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
                        Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;

                        //Create Excel Sheets
                        xlSheets = ExcelApp.Sheets;
                        xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
                                       Type.Missing, Type.Missing, Type.Missing);
                   



                    int _skipColumn = 0;
                        string strHeader = "";
                        for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                        {
                            strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                            if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                            {
                                _skipColumn++;
                                //j++;
                                continue;
                            }

                            ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                            ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                        }
                        _skipColumn = 0;
                        // Storing Each row and column value to excel sheet
                        for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                        {
                            for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                            {
                                if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                                {
                                    _skipColumn++;
                                    //l++;
                                continue;
                                }
                                if (l < dgrdDetails.Columns.Count)
                                    ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                            }
                            _skipColumn = 0;
                        }
                        ExcelApp.Columns.AutoFit();


                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = "Cheque_Detail_Register";
                        saveFileDialog.DefaultExt = ".xls";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                            MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                        }
                        else
                            MessageBox.Show("Export Cancled...");

                        ((Microsoft.Office.Interop.Excel.Worksheet)ExcelApp.ActiveWorkbook.Sheets[ExcelApp.ActiveWorkbook.Sheets.Count]).Delete();
                        //xlWorkbook.Close(true, misValue, misValue);
                        //ExcelApp.Quit();
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);




                    }
                    else
                        MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void txtFromDD_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDepositeDate.Checked, false, true);
        }
    }
  
}
