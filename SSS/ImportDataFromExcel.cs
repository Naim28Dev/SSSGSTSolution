using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Excel;

namespace SSS
{
    public partial class ImportDataFromExcel : Form
    {
        DataBaseAccess dba;
        public ImportDataFromExcel()
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            txtTemplate.Text = "BANK";
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
        }

        private void ImportDataFromExcel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
                else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                    SendKeys.Send("{TAB}");
            }
            catch { }
        }

        private void txtTemplate_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TEMPLATETYPE", "SEARCH TEMPLATE TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtTemplate.Text = objSearch.strSelectedData;
                    EnableDisableControl();
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("TEMPLATETYPE", "SEARCH TEMPLATE TYPE", Keys.Space);
                objSearch.ShowDialog();
                txtTemplate.Text = objSearch.strSelectedData;
                if (objSearch.strSelectedData != "")
                    txtTemplate.Text = objSearch.strSelectedData;
                txtTemplate.Focus();
                EnableDisableControl();
            }
            catch
            {
            }
        }

        private void EnableDisableControl()
        {
            if (txtTemplate.Text == "BANK")
                txtStatus.Enabled = txtDescLike.Enabled = txtDescNotLike.Enabled = txtAmount.Enabled = txtAccountHead.Enabled = txtSecondParty.Enabled = btnAccountHead.Enabled = btnSecondParty.Enabled =  true;
            else if (txtTemplate.Text == "PURCHASEBILL")
            {
                txtStatus.Enabled = txtDescLike.Enabled = txtDescNotLike.Enabled = txtAmount.Enabled = txtSecondParty.Enabled = btnSecondParty.Enabled = chkSendSMS.Enabled = false;
                txtAccountHead.Enabled = btnAccountHead.Enabled = true;
            }
            else
                txtStatus.Enabled = txtDescLike.Enabled = txtDescNotLike.Enabled = txtAmount.Enabled = txtAccountHead.Enabled = txtSecondParty.Enabled = btnAccountHead.Enabled = btnSecondParty.Enabled =  false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Excel Files (*.xls,*.xlsx)|*.xls;*.xlsx|Excel Files (*.xlsx)|*.xlsx";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                    txtFilePath.Text = _browser.FileName;
            }
            catch
            {
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            btnShow.Enabled = false;
            try
            {
                dgrdDetails.DataSource = null;
                if (txtTemplate.Text != "")
                {
                    if (txtFilePath.Text != "")
                    {
                        DataSet ds = GetDataFromExcel();
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            if (txtTemplate.Text == "BANK" && (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strCompanyName.Contains("SUPER")))
                            {
                                for (int _index = 0; _index <= 6; _index++)
                                {
                                    dt.Rows.RemoveAt(0);
                                }

                                if (dt.Columns.Count < 10)
                                    dt.Columns.Add("Column10", typeof(String));
                                if (dt.Columns.Count < 11)                               
                                    dt.Columns.Add("Column11", typeof(String));                                
                                if (dt.Columns.Count < 12)
                                    dt.Columns.Add("Column12", typeof(String));
                                if (dt.Columns.Count < 13)
                                    dt.Columns.Add("Column13", typeof(String));

                                DataRow[] rows = dt.Select("Column1>0 " + CreateQuery());
                                if (rows.Length > 0)
                                {
                                    DataTable _dt = rows.CopyToDataTable();
                                    SetSerialNo(ref _dt);
                                    dgrdDetails.DataSource = _dt;
                                }
                                if (dgrdDetails.Columns.Count > 0)
                                {
                                    dgrdDetails.Columns[0].Width = dgrdDetails.Columns[6].Width = 50;
                                    dgrdDetails.Columns[5].Width = 300;
                                    dgrdDetails.Columns[9].Width = dgrdDetails.Columns[10].Width = 250;

                                    dgrdDetails.Columns[1].Visible = dgrdDetails.Columns[3].Visible = dgrdDetails.Columns[4].Visible = dgrdDetails.Columns[8].Visible = dgrdDetails.Columns[10].Visible = dgrdDetails.Columns[11].Visible = false;

                                    dgrdDetails.Columns[0].HeaderText = "S.No";
                                    dgrdDetails.Columns[2].HeaderText = "Value Date";
                                    dgrdDetails.Columns[5].HeaderText = "Description";
                                    dgrdDetails.Columns[6].HeaderText = "Cr/Dr";
                                    dgrdDetails.Columns[7].HeaderText = "Amount";
                                    dgrdDetails.Columns[9].HeaderText = "ACCOUNT NAME";
                                    dgrdDetails.Columns[10].HeaderText = "COST CENTER ACCOUNT";
                                }
                            }
                            else if (txtTemplate.Text == "PURCHASEBILL")
                            {
                                dt.Columns.Add("SSSItemName", typeof(String)).SetOrdinal(5);
                                dgrdDetails.DataSource = dt;
                            }
                            else
                            {
                                dgrdDetails.DataSource = dt;
                            }
                        }
                    }
                    else
                    {
                        if (txtTemplate.Text == "BANK" && chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                        {
                            DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDate.Text), toDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                            DataTable _table = BankAPI.GetAccountStatement("777705000285", fromDate.ToString("dd-MM-yyyy"), toDate.ToString("dd-MM-yyyy"));
                            if (_table.Rows.Count > 0)
                                BindTableWithGrid(_table);
                            else
                                MessageBox.Show("Sorry ! No record found !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Sorry ! Please enter file path after than you can view the records !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else
                    MessageBox.Show("Sorry ! Please enter template name after that you can view the records !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnShow.Enabled = true;
        }

        private void SetSerialNo(ref DataTable dt)
        {
            int _index = 1;
            foreach (DataRow row in dt.Rows)
            {
                row["Column1"] = _index;
                _index++;
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (txtStatus.Text != "")
                strQuery += " and Column7 Like('%" + txtStatus.Text + "%') ";
            if (txtDescLike.Text != "")
                strQuery += " and Column6 Like('%" + txtDescLike.Text + "%') ";
            if (txtDescNotLike.Text != "")
                strQuery += " and Column6 Not Like('%" + txtDescNotLike.Text + "%') ";
            if (txtAmount.Text != "")
                strQuery += " and Column8>" + txtAmount.Text + " ";

            return strQuery;
        }

        private DataTable CreateTableForBank(DataTable _dt)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Column1", typeof(int));
            dt.Columns.Add("Column2", typeof(String));
            dt.Columns.Add("Column3", typeof(String));
            dt.Columns.Add("Column4", typeof(String));
            dt.Columns.Add("Column5", typeof(String));
            dt.Columns.Add("Column6", typeof(String));
            dt.Columns.Add("Column7", typeof(String));
            dt.Columns.Add("Column8", typeof(String));
            dt.Columns.Add("Column9", typeof(String));
            dt.Columns.Add("Column10", typeof(String));
            dt.Columns.Add("Column11", typeof(String));
            dt.Columns.Add("Column12", typeof(String));

            int _index = 1;
            foreach(DataRow _row in _dt.Rows)
            {
                DataRow row = dt.NewRow();
                row["Column1"] = _index;
                row["Column2"] = _row["TransactionID"];
                row["Column3"] = _row["ValueDate"];
                row["Column4"] = _row["TxnDate"];
                row["Column5"] = "-";
                row["Column6"] = _row["Remarks"];
                row["Column7"] = _row["Type"];
                row["Column8"] = _row["Amount"];
                row["Column9"] = _row["Balance"];
                dt.Rows.Add(row);

                _index++;
            }

            return dt;         
        }

        private void BindTableWithGrid(DataTable __dt)
        {
            DataTable dt= CreateTableForBank(__dt);
             DataRow[] rows = dt.Select("Column1>0 " + CreateQuery());
            if (rows.Length > 0)
            {
                DataTable _dt = rows.CopyToDataTable();
                SetSerialNo(ref _dt);
                dgrdDetails.DataSource = _dt;
            }
            else
            {
                MessageBox.Show("Sorry ! No record found !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (dgrdDetails.Columns.Count > 0)
            {
                dgrdDetails.Columns[0].Width = dgrdDetails.Columns[6].Width = 50;
                dgrdDetails.Columns[5].Width = 300;
                dgrdDetails.Columns[9].Width = dgrdDetails.Columns[10].Width = 250;

                dgrdDetails.Columns[1].Visible = dgrdDetails.Columns[3].Visible = dgrdDetails.Columns[4].Visible = dgrdDetails.Columns[8].Visible = dgrdDetails.Columns[10].Visible = dgrdDetails.Columns[11].Visible = false;

                dgrdDetails.Columns[0].HeaderText = "S.No";
                dgrdDetails.Columns[2].HeaderText = "Value Date";
                dgrdDetails.Columns[5].HeaderText = "Description";
                dgrdDetails.Columns[6].HeaderText = "Cr/Dr";
                dgrdDetails.Columns[7].HeaderText = "Amount";
                dgrdDetails.Columns[9].HeaderText = "ACCOUNT NAME";
                dgrdDetails.Columns[10].HeaderText = "COST CENTER ACCOUNT";
            }
        }

        private DataSet GetDataFromExcel()
        {
            DataSet ds = null;
            try
            {
                if (txtFilePath.Text != "")
                {
                    if (txtFilePath.Text.Contains(".XLS"))
                    {

                        FileStream stream = new FileStream(txtFilePath.Text, FileMode.Open, FileAccess.Read);
                        IExcelDataReader excelReader = null;
                        if (txtFilePath.Text.ToUpper().Contains(".XLSX"))
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        else
                            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                        if (txtTemplate.Text == "BANK")
                            excelReader.IsFirstRowAsColumnNames = false;
                        else
                            excelReader.IsFirstRowAsColumnNames = true;
                        ds = excelReader.AsDataSet();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ds;
        }

        private void txtStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAccountHead_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    string strPartyType = "BANKPARTY", strHeader = "SEARCH BANK A/C";
                    if (txtTemplate.Text == "PURCHASEBILL")
                    {
                        strPartyType = "PURCHASEPERSONALPARTY";
                        strHeader = "SEARCH SUNDRY CREDITOR";
                    }
                    SearchData objSearch = new SearchData(strPartyType, strHeader, e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtAccountHead.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtAccountHead.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtAccountHead.Text = "";
                        }
                    }
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

        private void btnAccountHead_Click(object sender, EventArgs e)
        {
            try
            {
                string strPartyType = "BANKPARTY", strHeader = "SEARCH BANK A/C";
                if (txtTemplate.Text == "PURCHASEBILL")
                {
                    strPartyType = "PURCHASEPERSONALPARTY";
                    strHeader = "SEARCH SUNDRY CREDITOR";
                }
                SearchData objSearch = new SearchData(strPartyType, strHeader, Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                {
                    txtAccountHead.Text = objSearch.strSelectedData;
                    if (dba.CheckTransactionLock(txtAccountHead.Text))
                    {
                        MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtAccountHead.Text = "";
                    }
                    txtAccountHead.Focus();
                }
            }
            catch { }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            btnImport.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    if ((txtAccountHead.Text != "" && txtSecondParty.Text != "") || txtTemplate.Text != "BANK")
                    {
                        DialogResult reuslt = MessageBox.Show("Are you sure you want to import details !!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (DialogResult.Yes == reuslt)
                        {
                            if (txtTemplate.Text == "BANK")
                            {
                                if (MainPage.mymainObject.bCashAdd)
                                {
                                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strCompanyName.Contains("SUPER"))
                                        SaveRecord_Bank();
                                    else
                                        SaveCASHRecord();
                                }

                                else
                                    MessageBox.Show("Sorry !! You don't have sufficient persmission to import bank entries !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (txtTemplate.Text == "CASH")
                            {
                                if (MainPage.mymainObject.bCashAdd)
                                    SaveCASHRecord();
                                else
                                    MessageBox.Show("Sorry !! You don't have sufficient persmission to import cash entries !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (txtTemplate.Text == "JOURNAL")
                            {
                                if (MainPage.mymainObject.bJournalAdd)
                                    SaveJournalRecord();
                                else
                                    MessageBox.Show("Sorry !! You don't have sufficient persmission to import journal entries !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if(txtTemplate.Text== "DESIGNMASTER")
                            {
                                if (MainPage.mymainObject.bAccountMasterAdd)
                                    SaveItemMaster();
                                else
                                    MessageBox.Show("Sorry !! You don't have sufficient persmission to import item master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                               
                            }
                            else if (txtTemplate.Text == "ACCOUNTMASTER")
                            {
                                if (MainPage.mymainObject.bPartyMasterAdd)
                                    SaveAccountMaster();
                                else
                                    MessageBox.Show("Sorry !! You don't have sufficient persmission to import item master !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            }                            
                            else if (txtTemplate.Text == "WAYBILL")
                                UpdateWayBillRecord();
                            else if (txtTemplate.Text == "EINVOICE")
                                UpdateEInvoiceBillRecord();
                        }
                    }
                    else
                        MessageBox.Show("Sorry !! Please enter Account Head and Other Party Name !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex){ MessageBox.Show("Sorry !! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnImport.Enabled = true;
        }

        private void SaveRecord()
        {
            string strQuery = "", strStatus = "", strDate = "", strDescription = "";
            double dAmt = 0;
            DateTime _date = DateTime.Now;
            string[] strFullParty = txtAccountHead.Text.Split(' ');

            strQuery = " Declare @SerialNo bigint, @SerialCode nvarchar(250),@AccountID nvarchar(250),@AccountName nvarchar(250); Select TOP 1 @SerialCode=BankVCode from CompanySetting ";
            string strCashAccount = "", strSecondParty = txtSecondParty.Text, strAccountID = "", strAccountStatusID = "", strNewAccountID = "", strNewAccountName = "", strCostCentreAccount="";
            double dAccountNo = 0;
            strAccountID = strFullParty[0];
            strCashAccount = txtAccountHead.Text.Replace(strAccountID + " ", "");

            strFullParty = strSecondParty.Split(' ');
            strAccountStatusID = strFullParty[0];
            strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");
            bool _bStatus = false;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToString(row.Cells[2].Value) != "")
                {
                    _bStatus = true;
                    //if (txtDateFormat.Text != "")
                    //    _date = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells[2].Value), txtDateFormat.Text);
                    //else
                    //{
                    // _date = Convert.ToDateTime(row.Cells[2].Value);                       
                    //}

                    if (ConvertDateTime(ref _date, Convert.ToString(row.Cells[2].Value)))
                    {
                        strDate = _date.ToString("MM/dd/yyyy");

                        if (Convert.ToString(row.Cells[6].Value).ToUpper() == "CR")
                            strStatus = "CREDIT";
                        else
                            strStatus = "DEBIT";
                        dAmt = dba.ConvertObjectToDouble(row.Cells[7].Value);
                        strDescription = Convert.ToString(row.Cells[5].Value).Replace("'", "").ToUpper();

                        strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode=@SerialCode ";
                        strNewAccountName = Convert.ToString(row.Cells[9].Value);

                        if (strDescription.Contains("SASUSP") && strNewAccountName=="")
                        {
                           string _strDescription = strDescription.Substring(strDescription.IndexOf("SASUSP") + 6, 7);
                            _strDescription = System.Text.RegularExpressions.Regex.Replace(_strDescription, "[^0-9]", "");

                            dAccountNo = dba.ConvertObjectToDouble(_strDescription);
                            if (dAccountNo > 0)
                            {
                                strQuery += " if exists (Select AccountNo from SupplierMaster Where GroupName!='SUB PARTY' and AccountNo='" + dAccountNo.ToString("0") + "' ) begin Select TOP 1 @AccountID=(AreaCode+AccountNo),@AccountName=Name from SupplierMaster Where GroupName!='SUB PARTY' and AccountNo='" + dAccountNo.ToString("0") + "' end else begin "
                                         + " Set @AccountID = '" + strAccountStatusID + "'; Set @AccountName = '" + strSecondParty + "' end ";
                            }
                            else
                            {
                                strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                                //   _bStatus = false;
                            }
                        }
                        else
                        {
                            
                            if (strNewAccountName.Trim() != "")
                            {
                                strFullParty = strNewAccountName.Split(' ');
                                strNewAccountID = strFullParty[0];
                                strNewAccountName = strNewAccountName.Replace(strNewAccountID + " ", "");

                                strQuery += " Set @AccountID ='" + strNewAccountID + "'; Set @AccountName='" + strNewAccountName + "';";
                            }
                            else
                                strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                        }

                        if (_bStatus && dAmt > 0)
                        {
                            strCostCentreAccount = Convert.ToString(row.Cells["Column11"].Value);
                            if (strCostCentreAccount != "")
                            {
                                string[] strFParty = strCostCentreAccount.Split(' ');
                                if (strFParty.Length > 1)
                                    strCostCentreAccount = strFParty[0];
                            }

                            if (strStatus == "CREDIT")
                            {
                                strQuery += "if not exists (Select BalanceID from BalanceAmount Where Description='" + strDescription + "' and Convert(nvarchar,Date,103)='" + _date.ToString("dd/MM/yyyy") + "' and CAST(Amount as Money)=" + dAmt.ToString("0.00") + ") begin INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "','" + strCashAccount + "',@AccountName,'DEBIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "',@AccountID,0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "',@AccountName,'" + strCashAccount + "','CREDIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,@AccountID,'" + strAccountID + "',0,'"+ strCostCentreAccount+"') "
                                         + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                         + "('BANK',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION') end ";
                            }
                            else if (strStatus == "DEBIT")
                            {
                                strQuery += "if not exists (Select BalanceID from BalanceAmount Where Description='" + strDescription + "' and Convert(nvarchar,Date,103)='" + _date.ToString("dd/MM/yyyy") + "' and CAST(Amount as Money)=" + dAmt.ToString("0.00") + ") begin INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "','" + strCashAccount + "',@AccountName,'CREDIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "',@AccountID,0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "',@AccountName,'" + strCashAccount + "','DEBIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,@AccountID,'" + strAccountID + "',0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                         + "('BANK',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION') end ";
                            }
                        }
                    }
                }
            }

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                SendSMSToParty();

                MessageBox.Show("Thank you !! Record imported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                dgrdDetails.DataSource = null;
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveRecord_Bank()
        {
            string strQuery = "", strStatus = "", strDate = "", strDescription = "";
            double dAmt = 0;
            DateTime _date = DateTime.Now;
            string[] strFullParty = txtAccountHead.Text.Split(' ');
                        
            string strCashAccount = "", strSecondParty = txtSecondParty.Text, strAccountID = "", strAccountStatusID = "", strNewAccountID = "", strNewAccountName = "", strCostCentreAccount = "",strPartyName="";
            double dAccountNo = 0;
            strAccountID = strFullParty[0];
            strCashAccount = txtAccountHead.Text.Replace(strAccountID + " ", "");

            strFullParty = strSecondParty.Split(' ');
            strAccountStatusID = strFullParty[0];
            strSecondParty = strSecondParty.Replace(strAccountStatusID + " ", "");
            bool _bStatus = false;
            int _allCount = 0;
            DataTable dtVoucher = dba.GetDataTable("Select VoucherCode,MAX(VoucherNo)VoucherNo from BalanceAmount Where VoucherCode in (Select BankVCode from CompanySetting Where CompanyName='" + MainPage.strCompanyName + "') Group by VoucherCode");
            
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (Convert.ToString(row.Cells[2].Value) != "")
                {
                    strPartyName = "";
                    strQuery = " Declare @SerialNo bigint, @SerialCode nvarchar(250),@AccountID nvarchar(250),@AccountName nvarchar(250); Select TOP 1 @SerialCode=BankVCode from CompanySetting ";
                    dAccountNo = 0;
                    _bStatus = true; 
                    if (ConvertDateTime(ref _date, Convert.ToString(row.Cells[2].Value)))
                    {
                        strDate = _date.ToString("MM/dd/yyyy");

                        if (Convert.ToString(row.Cells[6].Value).ToUpper() == "CR")
                            strStatus = "CREDIT";
                        else
                            strStatus = "DEBIT";
                        dAmt = dba.ConvertObjectToDouble(row.Cells[7].Value);
                        strDescription = Convert.ToString(row.Cells[5].Value).Replace("'", "").ToUpper();

                        strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode=@SerialCode ";
                        strPartyName = strNewAccountName = Convert.ToString(row.Cells[9].Value);

                        if (strDescription.Contains("SASUSP") && strNewAccountName == "")
                        {
                            string _strDescription = strDescription.Substring(strDescription.IndexOf("SASUSP") + 6, 7);
                            _strDescription = System.Text.RegularExpressions.Regex.Replace(_strDescription, "[^0-9]", "");

                            dAccountNo = dba.ConvertObjectToDouble(_strDescription);
                            if (dAccountNo > 0)
                            {
                                strQuery += " if exists (Select AccountNo from SupplierMaster Where GroupName='SUNDRY DEBTORS' and AccountNo='" + dAccountNo.ToString("0") + "' ) begin Select TOP 1 @AccountID=(AreaCode+AccountNo),@AccountName=Name from SupplierMaster Where GroupName='SUNDRY DEBTORS' and AccountNo='" + dAccountNo.ToString("0") + "' end else begin "
                                         + " Set @AccountID = '" + strAccountStatusID + "'; Set @AccountName = '" + strSecondParty + "' end ";
                            }
                            else
                            {
                                strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                            }
                        }
                        else if (strDescription.Contains("APIPAY") && strNewAccountName == "")
                        {
                            if (strDescription.Length > 10)
                            {
                                int _index = strDescription.IndexOf("APIPAY"), _sIndex = strDescription.IndexOf("/", (_index - 8)) + 1;
                                string strAccountNo = strDescription.Substring(_sIndex, 8);
                                strAccountNo = System.Text.RegularExpressions.Regex.Replace(strAccountNo, "[^0-9]", "");

                                dAccountNo = dba.ConvertObjectToDouble(strAccountNo);
                                if (dAccountNo > 0)
                                {
                                    strQuery += " if exists (Select AccountNo from SupplierMaster Where GroupName='SUNDRY DEBTORS' and AccountNo='" + dAccountNo.ToString("0") + "' ) begin Select TOP 1 @AccountID=(AreaCode+AccountNo),@AccountName=Name from SupplierMaster Where GroupName='SUNDRY DEBTORS' and AccountNo='" + dAccountNo.ToString("0") + "' end else begin "
                                             + " Set @AccountID = '" + strAccountStatusID + "'; Set @AccountName = '" + strSecondParty + "' end ";
                                }
                                else
                                {
                                    strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                                }
                            }
                            else
                                strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                        }
                        else
                        {
                            if (strNewAccountName.Trim() != "")
                            {
                                strFullParty = strNewAccountName.Split(' ');
                                strNewAccountID = strFullParty[0];
                                strNewAccountName = strNewAccountName.Replace(strNewAccountID + " ", "");

                                strQuery += " Set @AccountID ='" + strNewAccountID + "'; Set @AccountName='" + strNewAccountName + "';";
                            }
                            else
                                strQuery += " Set @AccountID ='" + strAccountStatusID + "'; Set @AccountName='" + strSecondParty + "';";
                        }

                        if (_bStatus && dAmt > 0)
                        {
                            strCostCentreAccount = Convert.ToString(row.Cells["Column11"].Value);
                            if (strCostCentreAccount != "")
                            {
                                string[] strFParty = strCostCentreAccount.Split(' ');
                                if (strFParty.Length > 1)
                                    strCostCentreAccount = strFParty[0];
                            }

                            if (strStatus == "CREDIT")
                            {
                                strQuery += "if not exists (Select BalanceID from BalanceAmount Where Description='" + strDescription + "' and Convert(nvarchar,Date,103)='" + _date.ToString("dd/MM/yyyy") + "' and CAST(Amount as Money)=" + dAmt.ToString("0.00") + ") begin INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "','" + strCashAccount + "',@AccountName,'DEBIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "',@AccountID,0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "',@AccountName,'" + strCashAccount + "','CREDIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,@AccountID,'" + strAccountID + "',0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                         + "('BANK',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION') end ";
                            }
                            else if (strStatus == "DEBIT")
                            {
                                strQuery += "if not exists (Select BalanceID from BalanceAmount Where Description='" + strDescription + "' and Convert(nvarchar,Date,103)='" + _date.ToString("dd/MM/yyyy") + "' and CAST(Amount as Money)=" + dAmt.ToString("0.00") + ") begin INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "','" + strCashAccount + "',@AccountName,'CREDIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "',@AccountID,0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                         + " (@SerialCode,@SerialNo,'" + strDate + "',@AccountName,'" + strCashAccount + "','DEBIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,@AccountID,'" + strAccountID + "',0,'" + strCostCentreAccount + "') "
                                         + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                         + "('BANK',@SerialCode,@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION') end ";
                            }

                            //if (strStatus == "CREDIT")
                            //    strQuery += dba.GetTCSquery_Import("@AccountID", dAmt, "@SerialCode", "@SerialNo", strDate);

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                _allCount += count;
                                strQuery = "";
                                if (chkSendSMS.Checked)
                                    SendWhatsappMessage(strPartyName, strStatus, dAccountNo, dAmt, strDescription, _date);
                            }
                        }
                    }
                }
            }

            if (_allCount > 0 && dtVoucher.Rows.Count > 0)
            {
                DataRow row = dtVoucher.Rows[0];
                string strVCode = Convert.ToString(row["VoucherCode"]);
                double dVNo = dba.ConvertObjectToDouble(row["VoucherNo"]);
                if (strVCode != "" && dVNo > 0)
                {
                    int _count = _allCount / 3;
                    dba.SaveTCSDetails(strVCode, dVNo, _count);
                }
            }

            if (_allCount > 0)
            {
                MessageBox.Show("Thank you !! " + (_allCount / 3) + " Record imported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                dgrdDetails.DataSource = null;
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ConvertDateTime(ref DateTime _date,string strDate)
        {
            try
            {
                if (txtDateFormat.Text != "")
                    _date = dba.ConvertDateInExactFormat(strDate, txtDateFormat.Text);
                else
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
                            if(strAllDate.Length>2)
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
                }
                return true;
            }
            catch(Exception ex) { MessageBox.Show("Sorry !! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return false;
        }

        private void txtSecondParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH SECOND ACCOUNT", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                    {
                        txtSecondParty.Text = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(txtSecondParty.Text))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtSecondParty.Text = "";
                        }
                    }
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

        private void btnSecondParty_Click(object sender, EventArgs e)
        {
            try
            {
                SearchData objSearch = new SearchData("ALLPARTY", "SEARCH SECOND ACCOUNT", Keys.Space);
                objSearch.ShowDialog();
                if (objSearch.strSelectedData != "")
                {
                    txtSecondParty.Text = objSearch.strSelectedData;
                    if (dba.CheckTransactionLock(txtSecondParty.Text))
                    {
                        MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtSecondParty.Text = "";
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    dgrdDetails.Rows.RemoveAt(dgrdDetails.CurrentRow.Index);
                    if (txtTemplate.Text == "BANK")
                        RearrenageSerial();
                }
            }
            catch { }
        }

        private void RearrenageSerial()
        {
            int _index = 1;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                row.Cells[0].Value = _index;
                _index++;
            }
        }


        private void CheckPartyTypeForCostCentre()
        {
            try
            {
                bool _bStatus = false;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (Convert.ToString(row.Cells["Column12"].Value) == "COST CENTRE")
                    {
                        _bStatus = true;
                        break;
                    }
                }
                dgrdDetails.Columns["Column11"].Visible = _bStatus;
            }
            catch { }
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (txtTemplate.Text == "BANK" && MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strCompanyName.Contains("SUPER"))
                {
                    if (e.ColumnIndex == 9)
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "Search Account Name", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSearchData != "")
                        {
                            if (txtAccountHead.Text != objSearch.strSelectedData)
                            {
                                string strPartyType = "", strGroupName = "";
                                dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                                if (dba.CheckTransactionLockWithPartyType(objSearch.strSelectedData, ref strPartyType, ref strGroupName))
                                {
                                    MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgrdDetails.CurrentCell.Value = "";
                                }
                                else if (strPartyType == "COST CENTRE")
                                    dgrdDetails.CurrentRow.Cells["Column12"].Value = strPartyType;
                                else
                                    dgrdDetails.CurrentRow.Cells["Column12"].Value = "";
                                CheckPartyTypeForCostCentre();
                                if (dgrdDetails.Columns["Column11"].Visible)
                                {
                                    dgrdDetails.CurrentCell = dgrdDetails.CurrentRow.Cells["Column11"];
                                    dgrdDetails.FirstDisplayedCell = dgrdDetails.CurrentCell;
                                }
                            }
                            else
                                MessageBox.Show("Sorry ! Both account name can't be same ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        e.Cancel = true;
                    }
                    else if (e.ColumnIndex == 10)
                    {
                        string strType = "ALLPARTY", strParty = Convert.ToString(dgrdDetails.CurrentRow.Cells[9].Value);
                        if (strParty.Contains("CUSTOMER"))
                            strType = "SALESPARTY";
                        else if (strParty.Contains("SUPPLIER"))
                            strType = "PURCHASEPARTY";

                        SearchData objSearch = new SearchData(strType, "Search Account Name", Keys.Space);
                        objSearch.ShowDialog();

                        dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(objSearch.strSelectedData))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdDetails.CurrentCell.Value = "";
                        }
                        e.Cancel = true;
                    }
                }
                else if (txtTemplate.Text == "CASH" || txtTemplate.Text == "BANK")
                {
                    string strColumnName = dgrdDetails.CurrentCell.OwningColumn.Name;
                    if (strColumnName != "")
                    {
                        object objValue = DataBaseAccess.ExecuteMyScalar("Select DBColumnName from ImportColumnDetails Where BillType='"+txtTemplate.Text+"' and CheckMaster=1 and TemplateColumnName='" + strColumnName + "' ");
                        if (Convert.ToString(objValue) != "")
                        {  
                            SearchData objSearch = new SearchData("ALLPARTY", "Search Account Name", Keys.Space);
                            objSearch.ShowDialog();

                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                            if (dba.CheckTransactionLock(objSearch.strSelectedData))
                            {
                                MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgrdDetails.CurrentCell.Value = "";
                            }                            
                        }
                        dgrdDetails.Columns[strColumnName].Width=180;
                    }
                    e.Cancel = true;
                }
                else if (txtTemplate.Text == "PURCHASEBILL" && e.ColumnIndex >= 0)
                {
                    if (dgrdDetails.Columns[e.ColumnIndex].Name == "SSSItemName")
                    {
                        SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                            dgrdDetails.CurrentCell.Value = objSearch.strSelectedData;
                    }
                    e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch { }
        }

        private void SendWhatsappMessage(string strPartyName,string strStatus, double dAccountNo, double dNetAmt,string strDescription,DateTime _date)
        {
            if ((strPartyName != "" || dAccountNo > 0))
            {
                string[] strCode = strPartyName.Split(' ');
               string strWhatsappNo = "", strMobileNo="", strGroupName="", strPartyID="", strMessage="", strWhastappMessage="";              
                DataTable dt = DataBaseAccess.GetDataTableRecord("Select MobileNo,UPPER(GroupName) GroupName,((ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)) PartyName,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where ((ISNULL(AreaCode,'')+ISNULL(AccountNo,'')) ='" + strCode[0] + "' OR AccountNo ='" + dAccountNo + "') ");
                if (dt.Rows.Count > 0)
                {
                    strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                    strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                    strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);

                    if (strPartyName == "")
                        strPartyName = Convert.ToString(dt.Rows[0]["PartyName"]);
                    string[] strFullName = strPartyName.Split(' ');
                    if (strFullName.Length > 1)
                        strPartyID = strFullName[0].Trim();
                    strPartyName = dba.GetSafePartyName(strPartyName);


                    if (strMobileNo.Length == 10 && dNetAmt > 0)
                    {
                        if (strStatus == "CR" || strStatus == "CREDIT")
                        {
                            strMessage = "M/S : " + strPartyName + ", We have received your amt Rs. " + dNetAmt.ToString("N2", MainPage.indianCurancy) + " THRU " + strDescription + " DT : " + _date.ToString("dd/MM/yyyy") + ".";
                            strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strDescription + "\" },{\"default\": \"" + _date.ToString("dd/MM/yyyy") + "\" }";
                        }
                        else
                        {
                            strMessage = "M/S : " + strPartyName + ", We have paid your amt Rs. " + dNetAmt.ToString("N2", MainPage.indianCurancy) + " THRU " + strDescription + " DT : " + _date.ToString("dd/MM/yyyy") + ".";
                            strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strDescription + "\" },{\"default\": \"" + _date.ToString("dd/MM/yyyy") + "\" }";
                        }

                            SendSMS objSMS = new SendSMS();
                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                    }

                    if (strStatus == "CR" || strStatus == "CREDIT")
                        NotificationClass.SetNotification("RECEIPT", strPartyID, dNetAmt, "");
                    else
                        NotificationClass.SetNotification("PAYMENT", strPartyID, dNetAmt, "");

                    if (strWhatsappNo != "")
                        WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "cash_bank", strWhastappMessage, "", "");
                    //WhatsappClass.SendWhatsAppMessage(strWhatsappNo, strMessage, "", "BULKBANK", "", "TEXT");
                }
            }
        }

        private void SendSMSToParty()
        {
            try
            {
                if (chkSendSMS.Checked && txtTemplate.Text == "BANK")
                {
                    string strMessage = "", strWhastappMessage="", strPartyID ="", strNetBalance = "", strPartyName = "", strMobileNo = "", strBankName = "", strGroupName = "", strStatus = "",strDescription = "",strWhatsappNo="";
                    double dNetAmt = 0;
                    strBankName = dba.GetSafePartyName(txtAccountHead.Text);                    
                    double dAccountNo = 0;
                    DateTime _date = DateTime.Now;
                    string[] strFullParty = txtAccountHead.Text.Split(' ');

                    bool _bStatus = true;

                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strMessage = strWhastappMessage=strNetBalance = strPartyName="";
                        _bStatus = true;
                        strStatus = Convert.ToString(row.Cells[6].Value).ToUpper();
                        _date = DateTime.Now;
                        //if (txtDateFormat.Text != "")
                        //    _date = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells[2].Value), txtDateFormat.Text);
                        //else
                        //    _date = Convert.ToDateTime(row.Cells[2].Value);

                        if (ConvertDateTime(ref _date, Convert.ToString(row.Cells[2].Value)))
                        {
                            strDescription = Convert.ToString(row.Cells[5].Value).Replace("'", "").ToUpper();

                            if (strDescription.Contains("SASUSP"))
                            {
                                strPartyName = Convert.ToString(row.Cells[9].Value);
                                if (strPartyName == "")
                                {
                                    string _strDescription = strDescription.Substring(strDescription.IndexOf("SASUSP") + 6, 7);
                                    _strDescription = System.Text.RegularExpressions.Regex.Replace(_strDescription, "[^0-9]", "");

                                    dAccountNo = dba.ConvertObjectToDouble(_strDescription);
                                   // dAccountNo = dba.ConvertObjectToDouble(strDescription.Substring(strDescription.IndexOf("SASUSP") + 6, 6));
                                    if (dAccountNo == 0)
                                        _bStatus = false;
                                }
                            }
                            else
                            {
                                strPartyName = Convert.ToString(row.Cells[9].Value);
                                if (strPartyName.Trim() == "")
                                    strPartyName = txtSecondParty.Text;
                            }

                            if (_bStatus && (strPartyName != "" || dAccountNo > 0))
                            {
                                string[] strCode = strPartyName.Split(' ');
                                strWhatsappNo = "";
                                DataTable dt = DataBaseAccess.GetDataTableRecord("Select MobileNo,UPPER(GroupName) GroupName,((ISNULL(AreaCode,'')+ISNULL(AccountNo,'')+' '+Name)) PartyName,WhatsappNo  from SupplierMaster SM OUTER APPLY (Select WaybillUserName as WhatsappNo from SupplierOtherDetails SOD Where SM.AreaCode=SOD.AreaCode and SM.AccountNo=SOD.AccountNo)SOD Where ((ISNULL(AreaCode,'')+ISNULL(AccountNo,'')) ='" + strCode[0] + "' OR AccountNo ='" + dAccountNo + "') ");
                                if (dt.Rows.Count > 0)
                                {
                                    strMobileNo = Convert.ToString(dt.Rows[0]["MobileNo"]);
                                    strGroupName = Convert.ToString(dt.Rows[0]["GroupName"]);
                                    strWhatsappNo = Convert.ToString(dt.Rows[0]["WhatsappNo"]);

                                    if (strPartyName == "")
                                        strPartyName = Convert.ToString(dt.Rows[0]["PartyName"]);
                                    string[] strFullName = strPartyName.Split(' ');
                                    if (strFullName.Length > 1)
                                        strPartyID = strFullName[0].Trim();

                                    dNetAmt = dba.ConvertObjectToDouble(row.Cells[7].Value);
                                    strPartyName = dba.GetSafePartyName(strPartyName);

                                    if (strMobileNo.Length == 10 && dNetAmt > 0)
                                    {
                                        if (strStatus == "CR")
                                        {
                                            strMessage = "M/S : " + strPartyName + ", We have received your amt Rs. " + dNetAmt + " THRU " + strDescription + " DT : " + _date.ToString("dd/MM/yyyy") + strNetBalance + ".";
                                            strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"received\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strDescription + "\" },{\"default\": \"" + _date.ToString("dd/MM/yyyy") + "\" }";
                                        }
                                        else
                                        {
                                            strMessage = "M/S : " + strPartyName + ", We have paid your amt Rs. " + dNetAmt + " THRU " + strDescription + " DT : " + _date.ToString("dd/MM/yyyy") + strNetBalance + ".";
                                            strWhastappMessage = "{\"default\": \"" + strPartyName + "\" },{\"default\": \"paid\" },{\"default\": \"" + dNetAmt.ToString("N2", MainPage.indianCurancy) + "\" },{\"default\": \"" + strDescription + "\" },{\"default\": \"" + _date.ToString("dd/MM/yyyy") + "\" }";
                                        }

                                            SendSMS objSMS = new SendSMS();
                                        objSMS.SendSingleSMS(strMessage, strMobileNo);
                                    }

                                    if (strStatus == "CR")
                                        NotificationClass.SetNotification("RECEIPT", strPartyID, dNetAmt, "");
                                    else
                                        NotificationClass.SetNotification("PAYMENT", strPartyID, dNetAmt, "");

                                    if (strWhatsappNo != "")
                                        WhatsappClass.SendWhatsappWithIMIMobile(strWhatsappNo, "cash_bank", strWhastappMessage, "", "");
                                   // WhatsappClass.SendWhatsAppMessage(strWhatsappNo, strMessage, "", "BULKBANK", "", "TEXT");
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

        private void UpdateWayBillRecord()
        {
            string strQuery = "", stBillNo = "",strWayBillNo="",strAllSaleBillNo="";
            decimal _wayBillNo=0;
            DateTime _date = DateTime.Now;
            bool bST = false;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (strAllSaleBillNo != "")
                    strAllSaleBillNo += ",";
                stBillNo = Convert.ToString(row.Cells[2].Value);
                strAllSaleBillNo += "'" + stBillNo + "'";

                strWayBillNo = Convert.ToString(row.Cells[8].Value);
                if (stBillNo != "" && strWayBillNo != "")
                {
                    _wayBillNo = Decimal.Parse(strWayBillNo, System.Globalization.NumberStyles.Float);
                    if (ConvertDateTime(ref _date, Convert.ToString(row.Cells[9].Value)))
                    {
                        if (stBillNo.Contains("ST"))
                        {
                            bST = true;
                            strQuery += " Update StockTransfer Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                 + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                 + " Select 'STOCKTRANSFER' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],TotalAmt as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],0 as [InsertStatus],0 as [UpdateStatus],'WAYBILL_UPDATED' as [EditStatus] from StockTransfer Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "')  ";
                        }
                        else
                        {
                            strQuery += " Update SalesRecord Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',[BillStatus]='SHIPPED',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update SalesBook Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType='SALES' and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                     + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'WAYBILL_UPDATED' as [EditStatus] from SalesRecord Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'WAYBILL_UPDATED' as [EditStatus] from SalesBook Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";
                        }
                    }
                }
            }

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                if(bST)
                    DataBaseAccess.CreateDeleteQuery(strQuery);
                else if (chkSendSMS.Checked)
                {
                    int _count = dba.SendEmailIDAndWhatsappNumberToSupplier(strAllSaleBillNo);
                    if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                    else
                        MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show("Thank you !! Record imported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                dgrdDetails.DataSource = null;
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateEInvoiceBillRecord()
        {
            string strQuery = "", stBillNo = "", strWayBillNo = "", strAllSaleBillNo = "",strIRNNo="",strQRCode="",strACKNo;
            decimal _wayBillNo = 0;
            DateTime _date = DateTime.Now;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                if (strAllSaleBillNo != "")
                    strAllSaleBillNo += ",";
                stBillNo = Convert.ToString(row.Cells[4].Value);
                strIRNNo = Convert.ToString(row.Cells[1].Value);
                strACKNo= Convert.ToString(row.Cells[2].Value);
                strQRCode = Convert.ToString(row.Cells[10].Value);
                strWayBillNo = Convert.ToString(row.Cells[11].Value);
                if (strWayBillNo != "" && strWayBillNo.Length<20)
                {
                    strAllSaleBillNo += "'" + stBillNo + "'";
                  
                    strWayBillNo = Convert.ToString(row.Cells[11].Value);
                    if (stBillNo != "" && strWayBillNo != "")
                    {
                        _wayBillNo = Decimal.Parse(strWayBillNo, System.Globalization.NumberStyles.Float);
                        if (ConvertDateTime(ref _date, Convert.ToString(row.Cells[3].Value)))
                        {
                            strQuery += " Update SalesRecord Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',[BillStatus]='SHIPPED',[IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update SalesBook Set WayBillNo='" + strWayBillNo + "',[WayBillDate]='" + _date.ToString("dd/MM/yyyy hh:mm tt") + "',UpdateStatus =1,[IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType='SALES' and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                                     + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                                     + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesRecord  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') UNION ALL Select 'SALESERVICE' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SaleServiceBook  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";
                        }
                    }
                }
                else
                {
                    strQuery += " Update SalesRecord Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                            + " Update SalesBook Set [IRNNo]='" + strIRNNo + "',[ACKNO]='"+strACKNo+"',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                            + " Update SaleServiceBook Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                            + " Update SaleReturn Set [IRNNo]='" + strIRNNo + "',[ACKNO]='" + strACKNo + "',[QRCode]='" + strQRCode + "',UpdateStatus =1,UpdatedBy='" + MainPage.strLoginName + "' Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                            + " Update [dbo].[GSTDetails] Set InsertStatus=1 Where BillType in ('SALES','SALESERVICE','SALERETURN','SALERETURN','DEBITNOTE') and (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') "
                            + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) "
                            + " Select 'SALES' as [BillType],[BillCode],[BillNo],DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) [Date],CAST([NetAmt] as Money) as NetAmt,'" + MainPage.strLoginName + "' as [UpdatedBy],1 as [InsertStatus],0 as [UpdateStatus],'EINVOICE_GEN' as [EditStatus] from SalesRecord  Where (BillCode+CAST(BillNo as varchar)) in ('" + stBillNo + "') ";
                }
            }

            int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                if (chkSendSMS.Checked && strAllSaleBillNo!="")
                {
                    int _count = dba.SendEmailIDAndWhatsappNumberToSupplier(strAllSaleBillNo);
                    if (_count > 0) { MessageBox.Show("Thank you !! (" + _count + ") Email and whatsapp message sent successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); }
                    else
                        MessageBox.Show("Sorry !! Unable to send email and whatsapp messages", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                MessageBox.Show("Thank you !! Record imported successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                dgrdDetails.DataSource = null;
            }
            else
            {
                MessageBox.Show("Sorry ! Unable to import record !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtDateFormat_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 1;
                if (txtTemplate.Text != "DESIGNMASTER")
                {
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        row.Cells[0].Value = _index;
                        _index++;
                    }
                }
            }
            catch { }
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private bool ValidateGridColumn(ref DataTable dt)
        {
            dt = dba.GetDataTable("Select DBColumnName,TemplateColumnName,ReqColumn,CheckMaster from ImportColumnDetails Where BillType='" + txtTemplate.Text+"' ");
            bool _status = true;
            if (dt.Rows.Count > 0)
            {
                string strColumnName="", strValue = "";
                int _index = 1;
                bool _bExistence = false;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    DataRow[] rowReq = dt.Select("ReqColumn=1");
                    DataRow[] rowExists = dt.Select("CheckMaster=1");
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                    foreach (DataRow _row in rowReq)
                    {
                        strColumnName = Convert.ToString(_row["TemplateColumnName"]);
                        if (strColumnName != "")
                        {
                            strValue = Convert.ToString(row.Cells[strColumnName].Value);
                            if (strValue == "")
                            {
                                MessageBox.Show("Sorry ! " + strColumnName + " is blank in Row Number : " + _index, "Required field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                row.DefaultCellStyle.BackColor = System.Drawing.Color.Tomato;
                                _status = false;
                            }
                        }
                    }
                    foreach (DataRow _row in rowExists)
                    {
                        strColumnName = Convert.ToString(_row["TemplateColumnName"]);
                        if (strColumnName != "")
                        {
                            strValue = Convert.ToString(row.Cells[strColumnName].Value);
                            if (strValue != "")
                            {
                                _bExistence = dba.CheckPartyExistence(strValue);
                                if (!_bExistence)
                                {
                                    MessageBox.Show("Sorry ! " + strColumnName + " is not in master list in Row Number : " + _index, "Master Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    row.DefaultCellStyle.BackColor = System.Drawing.Color.Gold;
                                    _status = false;
                                }
                            }
                        }
                    }
                    _index++;
                }
            }
            return _status;
        }

        private void SaveCASHRecord()
        {
            DataTable _dt = new DataTable();
            if (ValidateGridColumn(ref _dt))
            {
                //DialogResult _result = MessageBox.Show("Are you sure you want to import cash record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
               // if(_result==DialogResult.Yes)
                {
                    int _count=SaveCashTemplate(_dt);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved succcessfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dgrdDetails.DataSource = new DataTable();
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save record right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int SaveCashTemplate(DataTable _dt)
        {
            int count = 0;
            try
            {              

                    string strQuery = "", strSerialCode = "", strDate = "", strDebitAccount = "", strCreditAccount = "", strDescription = "", strAmount = "", strCreatedBy = "";
                    strQuery = " Declare @SerialNo bigint;";
                    DateTime _bDate = DateTime.Now;
                    string strSerialCodeColumn = "", strDateColumn = "", strDebitPartyColumn = "", strCreditPartyColumn = "", strDescColumn = "", strAmtColumn = "", strCreatedByColumn = "", strAccountID = "", strAccountStatusID = "";

                    DataRow[] _rows = _dt.Select("DBColumnName='VoucherCode'");
                    if (_rows.Length > 0)
                        strSerialCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='Date'");
                    if (_rows.Length > 0)
                        strDateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='DebitParty'");
                    if (_rows.Length > 0)
                        strDebitPartyColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='CreditParty'");
                    if (_rows.Length > 0)
                        strCreditPartyColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='Description'");
                    if (_rows.Length > 0)
                        strDescColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='Amount'");
                    if (_rows.Length > 0)
                        strAmtColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    _rows = _dt.Select("DBColumnName='CreatedBy'");
                    if (_rows.Length > 0)
                        strCreatedByColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                    int rowCount = 0;
                    foreach (DataGridViewRow row in dgrdDetails.Rows)
                    {
                        strSerialCode = strDate = strDebitAccount = strCreditAccount = strAmount = strDescription = strCreatedBy = strAccountID = strAccountStatusID = "";
                        if (strSerialCodeColumn != "")
                            strSerialCode = Convert.ToString(row.Cells[strSerialCodeColumn].Value);
                        if (strDateColumn != "")
                            strDate = Convert.ToString(row.Cells[strDateColumn].Value);
                        if (strDebitPartyColumn != "")
                            strDebitAccount = Convert.ToString(row.Cells[strDebitPartyColumn].Value);
                        if (strCreditPartyColumn != "")
                            strCreditAccount = Convert.ToString(row.Cells[strCreditPartyColumn].Value);
                        if (strDescColumn != "")
                            strDescription = Convert.ToString(row.Cells[strDescColumn].Value);
                        if (strAmtColumn != "")
                            strAmount = Convert.ToString(row.Cells[strAmtColumn].Value);
                        if (strCreatedByColumn != "")
                            strCreatedBy = Convert.ToString(row.Cells[strCreatedByColumn].Value);
                        string[] strParty = strDebitAccount.Split(' ');
                        if (strParty.Length > 1)
                        {
                            strAccountID = strParty[0];
                            strDebitAccount = strDebitAccount.Replace(strAccountID,"").Trim();
                        }
                        strParty = strCreditAccount.Split(' ');
                        if (strParty.Length > 1)
                        {
                            strAccountStatusID = strParty[0];
                            strCreditAccount = strCreditAccount.Replace(strAccountStatusID, "").Trim(); 
                        }
                        ConvertDateTime(ref _bDate, strDate);
                        double dAmt = dba.ConvertObjectToDouble(strAmount);

                        if (strAccountID != "" && strAccountStatusID != "" && dAmt > 0 && strSerialCode != "")
                        {
                            strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode='"+ strSerialCode+"' ";
                            strQuery += " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                     + " ('" + strSerialCode + "',@SerialNo,'" + _bDate.ToString("MM/dd/yyyy") + "','" + strDebitAccount + "','" + strCreditAccount + "','DEBIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountID + "','" + strAccountStatusID + "',0,'') "
                                     + " INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[RemoteCode],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[CostCentreAccountID]) VALUES "
                                     + " ('" + strSerialCode + "',@SerialNo,'" + _bDate.ToString("MM/dd/yyyy") + "','" + strCreditAccount + "','" + strDebitAccount + "','CREDIT','" + strDescription + "','" + dAmt.ToString("0.00") + "','','" + MainPage.strLoginName + "','','False',0,1,0,'" + strAccountStatusID + "','" + strAccountID + "',0,'') "
                                     + " INSERT INTO[dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                     + "('"+txtTemplate.Text+"','" + strSerialCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmt + ",'" + MainPage.strLoginName + "',1,0,'BULKCREATION')  ";
                        }

                        if (rowCount > 50)
                        {
                            strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                            count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                strQuery = "";
                                rowCount = 0;
                            }
                        }
                        rowCount++;

                    }
                    if (strQuery != "")
                    {
                        count = dba.ExecuteMyQuery(strQuery);
                    }               
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return count;
        }

        private void SaveJournalRecord()
        {
            DataTable _dt = new DataTable();
            if (ValidateGridColumn(ref _dt))
            {                                {
                    int _count = SaveJournalTemplate(_dt);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved succcessfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dgrdDetails.DataSource = new DataTable();
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save record right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int SaveJournalTemplate(DataTable _dt)
        {
            int count = 0;
            try
            {

                string strQuery = "", strSerialCode = "",strVoucherNo="",strOldVoucherNo="", strDate = "", strAccountName = "", strAmtStatus = "", strDescription = "", strAmount = "", strCreatedBy = "", strAccountID = "", strAccountStatusID = "", strGSTNature = "", strCreditPartyAccountID="",strCreditAccountID="", strCreditorPartyName = "", strCreditorAccountName = "", strRCMNature = "", strInvoiceNo = "", strInvoiceDate = "", strItemName = "", strTaxableAmt = "", strGSTPer = "", strRegion = "", strRemark = "";
                strQuery = " Declare @SerialNo bigint;";
                DateTime _bDate = DateTime.Now;
                string strSerialCodeColumn = "",strVoucherNoColumn="", strDateColumn = "", strAccountNameColumn = "", strAmtStatusColumn = "", strDescColumn = "", strAmtColumn = "", strCreatedByColumn = "", strGSTNatureColumn="", strCreditorPartyNameColumn="", strCreditorAccountNameColumn="", strRCMNatureColumn="", strInvoiceNoColumn="", strInvoiceDateColumn="", strItemNameColumn="", strTaxableAmtColumn="", strGSTPerColumn ="",strRegionColumn="", strRemarkCol="";

                DataRow[] _rows = _dt.Select("DBColumnName='VoucherCode'");
                if (_rows.Length > 0)
                    strSerialCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='VoucherNo'");
                if (_rows.Length > 0)
                    strVoucherNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Date'");
                if (_rows.Length > 0)
                    strDateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);

                _rows = _dt.Select("DBColumnName='AccountName' ");
                if (_rows.Length > 0)
                    strAccountNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='AmtStatus'");
                if (_rows.Length > 0)
                    strAmtStatusColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Description'");
                if (_rows.Length > 0)
                    strDescColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Amount'");
                if (_rows.Length > 0)
                    strAmtColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CreatedBy'");
                if (_rows.Length > 0)
                    strCreatedByColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='GSTNature'");
                if (_rows.Length > 0)
                    strGSTNatureColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CreditorPartyName'");
                if (_rows.Length > 0)
                    strCreditorPartyNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CreditorAccountName'");
                if (_rows.Length > 0)
                    strCreditorAccountNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='RCMNature'");
                if (_rows.Length > 0)
                    strRCMNatureColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='InvoiceNo'");
                if (_rows.Length > 0)
                    strInvoiceNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='InvoiceDate'");
                if (_rows.Length > 0)
                    strInvoiceDateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='ItemName'");
                if (_rows.Length > 0)
                    strItemNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='TaxableAmt'");
                if (_rows.Length > 0)
                    strTaxableAmtColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='GSTPer'");
                if (_rows.Length > 0)
                    strGSTPerColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Remark'");
                if (_rows.Length > 0)
                    strRemarkCol = Convert.ToString(_rows[0]["TemplateColumnName"]);    
                _rows = _dt.Select("DBColumnName='Region'");
                if (_rows.Length > 0)
                    strRegionColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);

                double dTotalAmt = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    strSerialCode = strVoucherNo = strDate = strAccountName = strAmtStatus = strDescription = strAmount = strCreatedBy = strAccountID = strAccountStatusID = strGSTNature = strCreditorPartyName = strCreditorAccountName = strRCMNature = strInvoiceNo = strInvoiceDate = strItemName = strTaxableAmt = strGSTPer = strRegion = strRemark = "";

                    if (strSerialCodeColumn != "")
                        strSerialCode = Convert.ToString(row.Cells[strSerialCodeColumn].Value);
                    if (strVoucherNoColumn != "")
                        strVoucherNo = Convert.ToString(row.Cells[strVoucherNoColumn].Value);
                    if (strDateColumn != "")
                        strDate = Convert.ToString(row.Cells[strDateColumn].Value);
                    if (strAccountNameColumn != "")
                        strAccountName = Convert.ToString(row.Cells[strAccountNameColumn].Value).ToUpper();
                    if (strAmtStatusColumn != "")
                        strAmtStatus = Convert.ToString(row.Cells[strAmtStatusColumn].Value).ToUpper();
                    if (strDescColumn != "")
                        strDescription = Convert.ToString(row.Cells[strDescColumn].Value).ToUpper();
                    if (strAmtColumn != "")
                        strAmount = Convert.ToString(row.Cells[strAmtColumn].Value);
                    if (strCreatedByColumn != "")
                        strCreatedBy = Convert.ToString(row.Cells[strCreatedByColumn].Value);   
                    if (strGSTNatureColumn != "")
                        strGSTNature = Convert.ToString(row.Cells[strGSTNatureColumn].Value).ToUpper();
                    if (strCreditorPartyNameColumn != "")
                        strCreditorPartyName = Convert.ToString(row.Cells[strCreditorPartyNameColumn].Value);
                    if (strCreditorAccountNameColumn != "")
                        strCreditorAccountName = Convert.ToString(row.Cells[strCreditorAccountNameColumn].Value);
                    if (strRCMNatureColumn != "")
                        strRCMNature = Convert.ToString(row.Cells[strRCMNatureColumn].Value);
                    if (strInvoiceNoColumn != "")
                        strInvoiceNo = Convert.ToString(row.Cells[strInvoiceNoColumn].Value);
                    if (strInvoiceDateColumn != "")
                        strInvoiceDate = Convert.ToString(row.Cells[strInvoiceDateColumn].Value);
                    if (strItemNameColumn != "")
                        strItemName = Convert.ToString(row.Cells[strItemNameColumn].Value);
                    if (strTaxableAmtColumn != "")
                        strTaxableAmt = Convert.ToString(row.Cells[strTaxableAmtColumn].Value);
                    if (strGSTPerColumn != "")
                        strGSTPer = Convert.ToString(row.Cells[strGSTPerColumn].Value);
                    if (strRegionColumn != "")
                        strRegion = Convert.ToString(row.Cells[strRegionColumn].Value);
                    if (strRemarkCol != "")
                        strRemark = Convert.ToString(row.Cells[strRemarkCol].Value);

                    string[] strParty = strAccountName.Split(' ');
                    if (strParty.Length > 1)
                    {
                        strAccountID = strParty[0];
                        strAccountName = strAccountName.Replace(strAccountID, "").Trim();
                    }
                 
                    ConvertDateTime(ref _bDate, strDate);
                    double dAmt = 0;// dba.ConvertObjectToDouble(strAmount);
                    dAmt = dba.ConvertObjectToDouble(strAmount);

                    if (strSerialCode != "" && strAccountID != "" && dAmt > 0)
                    {
                        if (strCreatedBy == "")
                            strCreatedBy = MainPage.strLoginName;

                        if (strVoucherNo != strOldVoucherNo)
                        {
                            if(dTotalAmt>0 && strOldVoucherNo!="")
                            {
                                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                 + "('JOURNAL','" + strSerialCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTotalAmt/2 + ",'" + strCreatedBy + "',1,0,'CREATION') ";
                            }
                            strQuery += " Select @SerialNo=(ISNULL(MAX(VoucherNo),0)+1) from BalanceAmount Where VoucherCode='" + strSerialCode + "'  ";
                            strOldVoucherNo = strVoucherNo;
                            dTotalAmt = 0;
                        }
                        dTotalAmt += dAmt;

                        if (strAccountID != "" && dAmt > 0 && strSerialCode != "")
                        {
                            strQuery += "   INSERT INTO [dbo].[BalanceAmount] ([VoucherCode],[VoucherNo],[Date],[PartyName],[AccountStatus],[Status],[Description],[Amount],[JournalID],[UserName],[UpdatedBy],[Tick],[InsertStatus],[UpdateStatus],[AccountID],[AccountStatusID],[ChequeStatus],[GSTNature],[CostCentreAccountID]) VALUES "
                                          + " ('" + strSerialCode + "',@SerialNo,'" + _bDate.ToString("MM/dd/yyyy") + "','" + strAccountName + "','JOURNAL A/C','" + strAmtStatus + "','" + strDescription + "'," + dAmt + ",'0','" + strCreatedBy + "','','False',1,0,'" + strAccountID + "','',0,'" + strGSTNature + "','')  ";
                        }

                        if (strCreditorAccountName != "" && strInvoiceNo != "" && strItemName != "" && strInvoiceDate != "" && strGSTNature== "REGISTERED EXPENSE (B2B)")
                        {
                            double dTaxableAmt = dba.ConvertObjectToDouble(strTaxableAmt), dGSTPer = dba.ConvertObjectToDouble(strGSTPer), dIGSTAmt = 0, dCGSTAmt = 0, dSGSTAmt = 0;
                            DateTime _invoiceDate = _bDate;
                            ConvertDateTime(ref _invoiceDate, strInvoiceDate);

                            strParty = strCreditorPartyName.Split(' ');
                            if (strParty.Length > 1)
                            {
                                strCreditPartyAccountID = strParty[0];
                                strCreditorPartyName = strCreditorPartyName.Replace(strCreditPartyAccountID, "").Trim();
                            }
                            strParty = strCreditorAccountName.Split(' ');
                            if (strParty.Length > 1)
                            {
                                strCreditAccountID = strParty[0];
                                strCreditorAccountName = strCreditorAccountName.Replace(strCreditAccountID, "").Trim();
                            }
                            if (strRegion == "LOCAL")
                            {
                                dCGSTAmt = dSGSTAmt = (((dTaxableAmt * dGSTPer) / 100.00) / 2);
                            }
                            else
                                dIGSTAmt = ((dTaxableAmt * dGSTPer) / 100.00);

                            strQuery += " INSERT INTO [dbo].[JournalVoucherDetails] ([VoucherCode],[VoucherNo],[PartyID],[OriginalInvoiceNo],[InvoiceDate],[DiffAmt],[GSTPer],[IGSTAmt],[CGSTAmt],[SGSTAmt],[Other],[TotalAmt],[TotalDiffAmt],[TotalTaxAmt],[AccountID],[RCMNature],[Remark],[Region],[InsertStatus]) VALUES "
                                  + " ('" + strSerialCode + "',@SerialNo,'" + strCreditPartyAccountID + "','" + strInvoiceNo + "','" + _invoiceDate.ToString("MM/dd/yyyy") + "','" + dTaxableAmt + "','" + strGSTPer + "'," + dIGSTAmt + ",'" + dCGSTAmt + "','" + dSGSTAmt + "','" + strItemName + "','" + dTaxableAmt + "','" + dTaxableAmt + "','" + dTaxableAmt + "','" + strCreditAccountID + "','','" + strRegion + "','" + strRegion + "',1) ";
                        }                                                                       
                    }
                }

                if (dTotalAmt > 0 && strOldVoucherNo != "")
                {
                    strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('JOURNAL','" + strSerialCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dTotalAmt/2 + ",'" + strCreatedBy + "',1,0,'CREATION') ";
                }

                strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                count = dba.ExecuteMyQuery(strQuery);
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return count;
        }

        private void SaveItemMaster()
        {
            DataTable _dt = new DataTable();
            if (ValidateGridColumn(ref _dt))
            {
                {
                    int _count = SaveItemMaster(_dt);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved succcessfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dgrdDetails.DataSource = new DataTable();
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save record right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private int SaveItemMaster(DataTable _dt)
        {
            int count = 0;
            try
            {
                string strDesignCodeQuery = "Select FChallanCode from CompanySetting Where  CompanyName='" + MainPage.strCompanyName + "' ",strMasterSerialCode="";
                object obj = DataBaseAccess.ExecuteMyScalar(strDesignCodeQuery);
                strMasterSerialCode = Convert.ToString(obj);

                string strQuery = "", strSerialCode = "", strBillNo="",strOldItemName="", strDate="", strItemName="", strDesignName="", strGroupName="", strCategoryName="", strBrandName="", strItemType="", strUnitName="", strBarCode="", strVariant1="", strVariant2="", strVariant3="", strVariant4="", strVariant5="", strPurchaseRate="", strSaleRate="", strOpeningQty="",  strOpeningRate="", strCreatedBy="", strDepartmentName="",strSaleMRP="",strBarCodingType="";
                strQuery = " Declare @SerialNo bigint;";
                DateTime _bDate = DateTime.Now;
                string strBillCodeColumn = "", strBillNoColumn = "", strDateColumn = "", strItemNameColumn = "", strDesignNameColumn = "", strGroupNameColumn = "", strCategoryNameColumn = "", strBrandNameColumn = "", strItemTypeColumn = "", strUnitNameColumn = "", strBarCodeColumn = "", strVariant1Column = "", strVariant2Column = "", strVariant3Column = "", strVariant4Column = "", strVariant5Column = "", strPurchaseRateColumn = "", strSaleRateColumn = "", strOpeningQtyColumn = "", strOpeningRateColumn = "", strCreatedByColumn = "", strDepartmentNameColumn="", strSaleMRPColumn = "",strBarCodingTypeColumn="";

                DataRow[] _rows = _dt.Select("DBColumnName='BillCode' ");
                if (_rows.Length > 0)
                    strBillCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='BillNo'");
                if (_rows.Length > 0)
                    strBillNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Date'");
                if (_rows.Length > 0)
                    strDateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='ItemName' ");
                if (_rows.Length > 0)
                    strItemNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DesignName'");
                if (_rows.Length > 0)
                    strDesignNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='GroupName'");
                if (_rows.Length > 0)
                    strGroupNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CategoryName'");
                if (_rows.Length > 0)
                    strCategoryNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='BrandName'");
                if (_rows.Length > 0)
                    strBrandNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='ItemType'");
                if (_rows.Length > 0)
                    strItemTypeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='UnitName'");
                if (_rows.Length > 0)
                    strUnitNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='BarCode'");
                if (_rows.Length > 0)
                    strBarCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Variant1'");
                if (_rows.Length > 0)
                    strVariant1Column = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Variant2'");
                if (_rows.Length > 0)
                    strVariant2Column = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Variant3'");
                if (_rows.Length > 0)
                    strVariant3Column = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Variant4'");
                if (_rows.Length > 0)
                    strVariant4Column = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Variant5'");
                if (_rows.Length > 0)
                    strVariant5Column = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PurchaseRate'");
                if (_rows.Length > 0)
                    strPurchaseRateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='SaleRate'");
                if (_rows.Length > 0)
                    strSaleRateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='OpeningQty'");
                if (_rows.Length > 0)
                    strOpeningQtyColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='OpeningRate'");
                if (_rows.Length > 0)
                    strOpeningRateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CreatedBy'");
                if (_rows.Length > 0)
                    strCreatedByColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DepartmentName'");
                if (_rows.Length > 0)
                    strDepartmentNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='SaleMRP'");
                if (_rows.Length > 0)
                    strSaleMRPColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);

                _rows = _dt.Select("DBColumnName='BarcodingType'");
                if (_rows.Length > 0)
                    strBarCodingTypeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);

                if(strItemNameColumn!="")
                {
                    dgrdDetails.Sort(dgrdDetails.Columns[strItemNameColumn], System.ComponentModel.ListSortDirection.Ascending);
                }

                int _rowIndex = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {  
                    if (strBillCodeColumn != "")
                        strSerialCode = Convert.ToString(row.Cells[strBillCodeColumn].Value);
                    if (strBillNoColumn != "")
                        strBillNo = Convert.ToString(row.Cells[strBillNoColumn].Value);
                    if (strDateColumn != "")
                        strDate = Convert.ToString(row.Cells[strDateColumn].Value);
                    if (strItemNameColumn != "")
                        strItemName = Convert.ToString(row.Cells[strItemNameColumn].Value).ToUpper();
                    if (strDesignNameColumn != "")
                        strDesignName = Convert.ToString(row.Cells[strDesignNameColumn].Value).ToUpper();
                    if (strGroupNameColumn != "")
                        strGroupName = Convert.ToString(row.Cells[strGroupNameColumn].Value).ToUpper();
                    if (strCategoryNameColumn != "")
                        strCategoryName = Convert.ToString(row.Cells[strCategoryNameColumn].Value);
                    if (strCreatedByColumn != "")
                        strCreatedBy = Convert.ToString(row.Cells[strCreatedByColumn].Value);
                    if (strBrandNameColumn != "")
                        strBrandName = Convert.ToString(row.Cells[strBrandNameColumn].Value).ToUpper();
                    if (strItemTypeColumn != "")
                        strItemType = Convert.ToString(row.Cells[strItemTypeColumn].Value);
                    if (strUnitNameColumn != "")
                        strUnitName = Convert.ToString(row.Cells[strUnitNameColumn].Value);
                    if (strBarCodeColumn != "")
                        strBarCode = Convert.ToString(row.Cells[strBarCodeColumn].Value);
                    if (strVariant1Column != "")
                        strVariant1 = Convert.ToString(row.Cells[strVariant1Column].Value);
                    if (strVariant2Column != "")
                        strVariant2 = Convert.ToString(row.Cells[strVariant2Column].Value);
                    if (strVariant3Column != "")
                        strVariant3 = Convert.ToString(row.Cells[strVariant3Column].Value);
                    if (strVariant4Column != "")
                        strVariant4 = Convert.ToString(row.Cells[strVariant4Column].Value);
                    if (strVariant5Column != "")
                        strVariant5 = Convert.ToString(row.Cells[strVariant5Column].Value);
                    if (strPurchaseRateColumn != "")
                        strPurchaseRate = Convert.ToString(row.Cells[strPurchaseRateColumn].Value);
                    if (strSaleRateColumn != "")
                        strSaleRate = Convert.ToString(row.Cells[strSaleRateColumn].Value);
                    if (strOpeningQtyColumn != "")
                        strOpeningQty = Convert.ToString(row.Cells[strOpeningQtyColumn].Value);
                    if (strOpeningRateColumn != "")
                        strOpeningRate = Convert.ToString(row.Cells[strOpeningRateColumn].Value);
                    if(strDepartmentNameColumn!="")
                        strDepartmentName = Convert.ToString(row.Cells[strDepartmentNameColumn].Value);
                    if (strSaleMRPColumn != "")
                        strSaleMRP = Convert.ToString(row.Cells[strSaleMRPColumn].Value);
                    if (strBarCodingTypeColumn != "")
                        strBarCodingType = Convert.ToString(row.Cells[strBarCodingTypeColumn].Value);

                    if (strBarCodingType == "")
                        strBarCodingType = MainPage.strBarCodingType;

                    if (strDate.Length > 3)
                        ConvertDateTime(ref _bDate, strDate);
                    else
                        _bDate = MainPage.currentDate;

                    double dPRate = dba.ConvertObjectToDouble(strPurchaseRate), dSRate = dba.ConvertObjectToDouble(strSaleRate), dSMRP = dba.ConvertObjectToDouble(strSaleMRP), dOQty = dba.ConvertObjectToDouble(strOpeningQty), dORate = dba.ConvertObjectToDouble(strOpeningRate);

                    if (strSerialCode == "")
                        strSerialCode = strMasterSerialCode;

                    if (strItemType != "")
                        strItemType = "PURCHASE";

                    if (strSerialCode != "" && strItemName != "")
                    {
                        if (strCreatedBy == "")
                            strCreatedBy = MainPage.strLoginName;

                        if (strItemName != strOldItemName)
                        {
                            if (strOldItemName != "")
                            {
                                strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                                          + "('DESIGNMASTER','" + strSerialCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + strCreatedBy + "',1,0,'CREATION') end ";

                                if (_rowIndex > 50)
                                {
                                    count += dba.ExecuteMyQuery(strQuery);
                                    strQuery = "Declare @SerialNo bigint;";
                                    _rowIndex = 0;
                                }
                            }
                            strQuery += " Select @SerialNo=(ISNULL(MAX(BillNo),0)+1) from [dbo].[Items] Where [BillCode]='" + strSerialCode + "' if(@SerialNo='') Set @SerialNo=1;  ";
                            strOldItemName = strItemName;


                            if (strItemName != "" && strSerialCode != "")
                            {
                                strQuery += " if not exists (Select [ItemName] from [dbo].[Items] Where (([BillCode]='" + strSerialCode + "' and [BillNo]=@SerialNo)  OR ([ItemName]='" + strItemName + "'))) begin  INSERT INTO [dbo].[Items] ([ItemName],[Date],[InsertStatus],[UpdateStatus],[GroupName],[SubGroupName],[UnitName],[BillCode],[BillNo],[BuyerDesignName],[QtyRatio],[StockUnitName],[DisStatus],[DisRemark],[Other],[CreatedBy],[UpdatedBy],[BrandName],[MakeName],[BarcodingType]) VALUES "
                                          + " ('" + strItemName + "','" + _bDate.ToString("MM/dd/yyyy") + "',1,0,'" + strGroupName + "','" + strItemType + "','" + strUnitName + "','" + strSerialCode + "',@SerialNo,'" + strDesignName + "',1,'" + strUnitName + "',0,'','" + strCategoryName + "','" + strCreatedBy + "','','" + strBrandName + "','" + strDepartmentName + "','"+ strBarCodingType + "') "
                                          + " if not exists (Select GroupName from ItemGroupMaster Where GroupName='" + strGroupName + "' ) begin "
                                          + " Insert into ItemGroupMaster ([GroupName],[CategoryName],[ParentGroup],[HSNCode],[Other],[InsertStatus],[UpdateStatus],[TaxCategoryName],[TaxRate]) Values "
                                          + " ('" + strGroupName + "','','','" + strGroupName + "','HSN',1,0,'5-12%',5)  end ";


                            }
                        }

                        if (strBarCode!="" || dPRate>0 || dSRate>0 || dOQty!=0 || dORate!=0 || dSMRP>0)
                        {
                            strQuery += " INSERT INTO [dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[PurchasePartyID],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[PurchaseRate],[Margin],[SaleRate],[Reorder],[OpeningQty],[OpeningRate],[ActiveStatus],[GodownName],[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[SaleMRP],[DesignName],[Brand]) VALUES "
                             + " (0,'" + strSerialCode + "',@SerialNo,'','" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dPRate + ",0," + dSRate + ",0," + dOQty + "," + dORate+",1,'','" + strBarCode + "','" + strCreatedBy + "','',1,0,"+dSMRP+ ",'" + strDesignName + "','" + strBrandName + "')";
                            //if (dOQty > 0)
                            {
                                strQuery += " INSERT INTO [dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName]) VALUES "
                                              + " ('OPENING','" + strSerialCode + "',@SerialNo, '" + strItemName + "','" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + dOQty + "," + dORate + " ,'','" +strCreatedBy + "','',1,0," + dORate + ",'" + _bDate.ToString("MM/dd/yyyy") + "','" + strBarCode + "','" + strBrandName + "','" + strDesignName + "') ";
                            }

                            if(strBarCode!="")
                            {                                
                                    strQuery += " INSERT INTO [dbo].[BarcodeDetails]([BillCode],[BillNo],[ParentBarCode],[BarCode],[NetQty],[SetQty],[LastPrintNo],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[InStock]) "
                                             + " SELECT '" + strSerialCode + "',@SerialNo,'" + strBarCode + "','" + strBarCode + "',"+ dOQty+","+ dOQty+",1,'" + MainPage.strLoginName + "','',1,0,1 ";                                

                            }
                        }
                    }
                    _rowIndex++;
                }

               
                if (strQuery != "")
                {
                    if (strOldItemName != "")
                    {
                        strQuery += " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                         + "('DESIGNMASTER','" + strSerialCode + "',@SerialNo,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),0,'" + strCreatedBy + "',1,0,'CREATION') end ";
                    }

                    strQuery = " SET QUERY_GOVERNOR_COST_LIMIT 0; " + strQuery;
                    count += dba.ExecuteMyQuery(strQuery);
                }
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return count;
        }

        private void SaveAccountMaster()
        {
            DataTable _dt = new DataTable();
            if (ValidateGridColumn(ref _dt))
            {
                {
                    int _count = SaveAccountMaster(_dt);
                    if (_count > 0)
                    {
                        MessageBox.Show("Thank you ! Record saved succcessfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        dgrdDetails.DataSource = new DataTable();
                    }
                    else
                        MessageBox.Show("Sorry ! Unable to save record right now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int SaveAccountMaster(DataTable _dt)
        {
            int count = 0;
            try
            {
                string strAreaCode = "", strName = "", strNickName = "", strGroupName = "", strCategory = "", strDealerType = "", strPartyType = "", strOpeningBal = "", strStatus = "", strAddress = "", strPermanentAddress = "", strDistrictName = "", strStation = "", strBookingStation = "", strState = "", strPinCode = "", strTransport = "", strNormalDhara = "", strSNDhara = "", strContactPerson = "", strMobileNo = "", strPhoneNo = "", strAccountantMobileNo = "", strWhatsappNo = "", strEmailID = "", strReference = "", strPvtMarka = "", strAmountLimit = "", strPostage = "", strAadharNo = "", strGSTNo = "", strPANNumber = "", strSaleIncentive = "", strCourierName = "", strDOB = "", strDOA = "", strSpouseName = "";

                string strAreaCodeColumn = "", strNameColumn = "", strNickNameColumn = "", strGroupNameColumn = "", strCategoryColumn = "", strDealerTypeColumn = "", strPartyTypeColumn = "", strOpeningBalColumn = "", strStatusColumn = "", strAddressColumn = "", strPermanentAddressColumn = "", strDistrictNameColumn = "", strStationColumn = "", strBookingStationColumn = "", strStateColumn = "", strPinCodeColumn = "", strTransportColumn = "", strNormalDharaColumn = "", strSNDharaColumn = "", strContactPersonColumn = "", strMobileNoColumn = "", strPhoneNoColumn = "", strAccountantMobileNoColumn = "", strWhatsappNoColumn = "", strEmailIDColumn = "", strReferenceColumn = "", strPvtMarkaColumn = "", strAmountLimitColumn = "", strPostageColumn = "", strAadharNoColumn = "", strGSTNoColumn = "", strPANNumberColumn = "", strSaleIncentiveColumn = "", strCourierNameColumn = "", strDOBColumn = "", strDOAColumn = "", strSpouseNameColumn = "";

                DataRow[] _rows = _dt.Select("DBColumnName='AreaCode' ");
                if (_rows.Length > 0)
                    strAreaCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                //_rows = _dt.Select("DBColumnName='AccountNo'");
                //if (_rows.Length > 0)
                //    strAccountNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Name'");
                if (_rows.Length > 0)
                    strNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='NickName'");
                if (_rows.Length > 0)
                    strNickName = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='GroupName' ");
                if (_rows.Length > 0)
                    strGroupNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Category'");
                if (_rows.Length > 0)
                    strCategoryColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DealerType'");
                if (_rows.Length > 0)
                    strDealerTypeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PartyType'");
                if (_rows.Length > 0)
                    strPartyTypeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='OpeningBal'");
                if (_rows.Length > 0)
                    strOpeningBalColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Status'");
                if (_rows.Length > 0)
                    strStatusColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Address'");
                if (_rows.Length > 0)
                    strAddressColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PermanentAddress'");
                if (_rows.Length > 0)
                    strPermanentAddressColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DistrictName'");
                if (_rows.Length > 0)
                    strDistrictNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Station'");
                if (_rows.Length > 0)
                    strStationColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='BookingStation'");
                if (_rows.Length > 0)
                    strBookingStationColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='State'");
                if (_rows.Length > 0)
                    strStateColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PinCode'");
                if (_rows.Length > 0)
                    strPinCodeColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Transport'");
                if (_rows.Length > 0)
                    strTransportColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='NormalDhara'");
                if (_rows.Length > 0)
                    strNormalDharaColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='SNDhara'");
                if (_rows.Length > 0)
                    strSNDharaColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='ContactPerson'");
                if (_rows.Length > 0)
                    strContactPersonColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='MobileNo'");
                if (_rows.Length > 0)
                    strMobileNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PhoneNo'");
                if (_rows.Length > 0)
                    strPhoneNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='AccountantMobileNo'");
                if (_rows.Length > 0)
                    strAccountantMobileNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='WhatsappNo'");
                if (_rows.Length > 0)
                    strWhatsappNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='EmailID'");
                if (_rows.Length > 0)
                    strEmailIDColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Reference'");
                if (_rows.Length > 0)
                    strReferenceColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PvtMarka'");
                if (_rows.Length > 0)
                    strPvtMarkaColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='AmountLimit'");
                if (_rows.Length > 0)
                    strAmountLimitColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='Postage'");
                if (_rows.Length > 0)
                    strPostageColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='AadharNo'");
                if (_rows.Length > 0)
                    strAadharNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='GSTNo'");
                if (_rows.Length > 0)
                    strGSTNoColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='PANNumber'");
                if (_rows.Length > 0)
                    strPANNumberColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='SaleIncentive'");
                if (_rows.Length > 0)
                    strSaleIncentiveColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='CourierName'");
                if (_rows.Length > 0)
                    strCourierNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DOB'");
                if (_rows.Length > 0)
                    strDOBColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='DOA'");
                if (_rows.Length > 0)
                    strDOAColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);
                _rows = _dt.Select("DBColumnName='SpouseName'");
                if (_rows.Length > 0)
                    strSpouseNameColumn = Convert.ToString(_rows[0]["TemplateColumnName"]);

                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    if (strAreaCodeColumn != "")
                        strAreaCode = Convert.ToString(row.Cells[strAreaCodeColumn].Value).ToUpper().Trim();
                    //if (strAccountNoColumn != "")
                    //    strAccountNo = Convert.ToString(row.Cells[strAccountNoColumn].Value);
                    if (strNameColumn != "")
                        strName = Convert.ToString(row.Cells[strNameColumn].Value).ToUpper().Trim();
                    if (strNickNameColumn != "")
                        strNickName = Convert.ToString(row.Cells[strNickNameColumn].Value).ToUpper().Trim();
                    if (strGroupNameColumn != "")
                        strGroupName = Convert.ToString(row.Cells[strGroupNameColumn].Value).ToUpper().Trim();
                    if (strCategoryColumn != "")
                        strCategory = Convert.ToString(row.Cells[strCategoryColumn].Value).ToUpper().Trim();
                    if (strDealerTypeColumn != "")
                        strDealerType = Convert.ToString(row.Cells[strDealerTypeColumn].Value).ToUpper().Trim();
                    if (strPartyTypeColumn != "")
                        strPartyType = Convert.ToString(row.Cells[strPartyTypeColumn].Value).ToUpper().Trim();
                    if (strOpeningBalColumn != "")
                        strOpeningBal = Convert.ToString(row.Cells[strOpeningBalColumn].Value);
                    if (strStatusColumn != "")
                        strStatus = Convert.ToString(row.Cells[strStatusColumn].Value).ToUpper().Trim();
                    if (strAddressColumn != "")
                        strAddress = Convert.ToString(row.Cells[strAddressColumn].Value).ToUpper().Trim();
                    if (strPermanentAddressColumn != "")
                        strPermanentAddress = Convert.ToString(row.Cells[strPermanentAddressColumn].Value).ToUpper().Trim();
                    if (strDistrictNameColumn != "")
                        strDistrictName = Convert.ToString(row.Cells[strDistrictNameColumn].Value).ToUpper().Trim();
                    if (strStationColumn != "")
                        strStation = Convert.ToString(row.Cells[strStationColumn].Value).ToUpper().Trim();
                    if (strBookingStationColumn != "")
                        strBookingStation = Convert.ToString(row.Cells[strBookingStationColumn].Value).ToUpper().Trim();
                    if (strStateColumn != "")
                        strState = Convert.ToString(row.Cells[strStateColumn].Value).ToUpper().Trim();
                    if (strPinCodeColumn != "")
                        strPinCode = Convert.ToString(row.Cells[strPinCodeColumn].Value).ToUpper().Trim();
                    if (strTransportColumn != "")
                        strTransport = Convert.ToString(row.Cells[strTransportColumn].Value).ToUpper().Trim();
                    if (strNormalDharaColumn != "")
                        strNormalDhara = Convert.ToString(row.Cells[strNormalDharaColumn].Value).ToUpper().Trim();
                    if (strSNDharaColumn != "")
                        strSNDhara = Convert.ToString(row.Cells[strSNDharaColumn].Value).ToUpper().Trim();
                    if (strContactPersonColumn != "")
                        strContactPerson = Convert.ToString(row.Cells[strContactPersonColumn].Value).ToUpper().Trim();
                    if (strMobileNoColumn != "")
                        strMobileNo = Convert.ToString(row.Cells[strMobileNoColumn].Value).ToUpper().Trim();
                    if (strPhoneNoColumn != "")
                        strPhoneNo = Convert.ToString(row.Cells[strPhoneNoColumn].Value).ToUpper().Trim();
                    if (strAccountantMobileNoColumn != "")
                        strAccountantMobileNo = Convert.ToString(row.Cells[strAccountantMobileNoColumn].Value).ToUpper().Trim();
                    if (strWhatsappNoColumn != "")
                        strWhatsappNo = Convert.ToString(row.Cells[strWhatsappNoColumn].Value).ToUpper().Trim();
                    if (strEmailIDColumn != "")
                        strEmailID = Convert.ToString(row.Cells[strEmailIDColumn].Value).ToUpper().Trim();
                    if (strReferenceColumn != "")
                        strReference = Convert.ToString(row.Cells[strReferenceColumn].Value).ToUpper().Trim();
                    if (strPvtMarkaColumn != "")
                        strPvtMarka = Convert.ToString(row.Cells[strPvtMarkaColumn].Value).ToUpper().Trim();
                    if (strAmountLimitColumn != "")
                        strAmountLimit = Convert.ToString(row.Cells[strAmountLimitColumn].Value).ToUpper().Trim();
                    if (strPostageColumn != "")
                        strPostage = Convert.ToString(row.Cells[strPostageColumn].Value).ToUpper().Trim();
                    if (strAadharNoColumn != "")
                        strAadharNo = Convert.ToString(row.Cells[strAadharNoColumn].Value).ToUpper().Trim();
                    if (strGSTNoColumn != "")
                        strGSTNo = Convert.ToString(row.Cells[strGSTNoColumn].Value).ToUpper().Trim();
                    if (strPANNumberColumn != "")
                        strPANNumber = Convert.ToString(row.Cells[strPANNumberColumn].Value).ToUpper().Trim();
                    if (strSaleIncentiveColumn != "")
                        strSaleIncentive = Convert.ToString(row.Cells[strSaleIncentiveColumn].Value);
                    if (strCourierNameColumn != "")
                        strCourierName = Convert.ToString(row.Cells[strCourierNameColumn].Value).ToUpper().Trim();
                    if (strDOBColumn != "")
                        strDOB = Convert.ToString(row.Cells[strDOBColumn].Value);
                    if (strDOAColumn != "")
                        strDOA = Convert.ToString(row.Cells[strDOAColumn].Value);
                    if (strSpouseNameColumn != "")
                        strSpouseName = Convert.ToString(row.Cells[strSpouseNameColumn].Value);

                    if (strAreaCode != "" && strName != "" && strGroupName != "")
                    {
                        SaveAccountMasterRecords(strAreaCode, strName, strNickName, strGroupName, strCategory, strDealerType, strPartyType, strOpeningBal, strStatus, strAddress, strPermanentAddress, strDistrictName, strStation, strBookingStation, strState, strPinCode, strTransport, strNormalDhara, strSNDhara, strContactPerson, strMobileNo, strPhoneNo, strAccountantMobileNo, strWhatsappNo, strEmailID, strReference, strPvtMarka, strAmountLimit, strPostage, strAadharNo, strGSTNo, strPANNumber, strSaleIncentive, strCourierName, strDOB, strDOA, strSpouseName);
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            return count;
        }

        private int SaveAccountMasterRecords(string strAreaCode = "", string strName = "", string strNickName = "", string strGroupName = "", string strCategory = "", string strDealerType = "", string strPartyType = "", string strOpeningBal = "", string strStatus = "", string strAddress = "", string strPermanentAddress = "", string strDistrictName = "", string strStation = "", string strBookingStation = "", string strState = "", string strPinCode = "", string strTransport = "", string strNormalDhara = "", string strSNDhara = "", string strContactPerson = "", string strMobileNo = "", string strPhoneNo = "", string strAccountantMobileNo = "", string strWhatsappNo = "", string strEmailID = "", string strReference = "", string strPvtMarka = "", string strAmountLimit = "", string strPostage = "", string strAadharNo = "", string strGSTNo = "", string strPANNumber = "", string strSaleIncentive = "", string strCourierName = "", string strDOB = "", string strDOA = "", string strSpouseName = "")
        {
            int count = 0;
            try
            {
                if (strStatus == "CR")
                    strStatus = "CREDIT";
                else if (strStatus == "DR")
                    strStatus = "DEBIT";

                double dAmtLimit = 0;
                if (strAmountLimit != "")
                    dAmtLimit = dba.ConvertObjectToDouble(strAmountLimit);
                string[] record = new string[58];

                record[0] = strName.Trim();
                record[1] = strCategory.Trim();
                record[2] = strGroupName;
                record[3] = dba.ConvertObjectToDouble(strOpeningBal).ToString();
                record[4] = strStatus;
                record[5] = "";
                record[6] = strAddress;
                record[7] = strState;
                record[8] = strPinCode;
                record[9] = strTransport;
                record[10] = strStation;
                record[11] = strBookingStation;
                record[12] = strPartyType;
                record[13] = strNormalDhara;
                record[14] = strSNDhara;
                record[15] = strContactPerson;
                record[16] = strPhoneNo;
                record[17] = strMobileNo;
                record[18] = strPvtMarka;
                record[19] = strReference;
                record[20] = strEmailID;
                record[21] = "";
                record[22] = MainPage.currentDate.ToString("MM/dd/yyyy h:mm:ss tt");
                record[23] = "";
                record[24] = dAmtLimit.ToString("0");
                record[25] = strPermanentAddress;
                record[26] = "";
                record[27] = "";
                record[28] = "True";
                record[29] = "";
                record[30] = "";
                record[31] = "0";
                record[32] = strPostage;
                record[33] = "false";
                record[34] = "false";
                record[35] = "";
                record[36] = strDealerType;
                record[37] = strAreaCode;
                record[38] = "";
                record[39] = strAadharNo;
                record[40] = strNickName;
                record[41] = strSaleIncentive;
                record[42] = strGSTNo.Trim();
                record[43] = strPANNumber;
                record[44] = "";
                record[45] = strAccountantMobileNo;
                record[47] = strCourierName;
                record[48] = strDistrictName;
                record[49] = "0";
                record[50] = "false";


                if (strGroupName == "SUNDRY DEBTORS" && (MainPage.strCompanyName.Contains("STYLO") || MainPage.strCompanyName.Contains("SARAOGI")) && MainPage.strSoftwareType == "AGENT")
                {
                    record[33] = "True";
                }
                string strAccountNo = "", strOtherQuery = GetOtherDetailsQuery(strAreaCode, strWhatsappNo, strDOA, strDOB, strSpouseName, dAmtLimit);
                if (MainPage.strOnlineDataBaseName != "")
                {
                    string strResult = dba.SaveSupplierNameInOnline(record, MainPage.strOnlineDataBaseName, ref strAccountNo, strOtherQuery);
                    if (strResult == "net")
                    {
                        MessageBox.Show("Sorry ! An error occured, Please try again later", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        count = -2;
                    }
                    else if (strResult == "error")
                        count = 0;
                    else if (strResult == "success")
                        count = 2;
                }
                else
                    count = dba.SaveSupplierMaster(record, ref strAccountNo, strOtherQuery);

                if (count > 0 & strAccountNo != "")
                {
                    if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                    {
                        string strUserRole = "CUSTOMER", strUserType = "1";
                        if (strGroupName == "SUNDRY CREDITOR")
                        {
                            strUserRole = "SUPPLIER";
                            strUserType = "2";
                        }
                        else if (strGroupName != "SUNDRY DEBTORS")
                        {
                            strUserRole = "EMPLOYEE";
                            strUserType = "3";
                        }
                        if (MainPage.strCompanyName.Contains("SARAOGI") && MainPage.strSoftwareType == "AGENT")
                        {
                            string _strName = strAreaCode + strAccountNo + " " + strName;

                            AppAPI.AddNewUserinApp(strName, strEmailID, strMobileNo, strUserType, strAreaCode + strAccountNo, strUserRole);
                            AppAPI.AddNewUserinSSSAddaApp(_strName, strEmailID, strMobileNo, strUserType, strAreaCode + strAccountNo, strUserRole, strName, strDistrictName, strState, strGSTNo, "");
                        }
                    }
                }
                else
                {
                    if (count != -2)
                        MessageBox.Show("Sorry ! An error occured, Please try again later", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Saving Record in Supplier Master ", ex.Message };
                dba.CreateErrorReports(strReport);
                count = 0;
            }
            return count;
        }

        private string GetOtherDetailsQuery(string strAreaCode = "", string strWhatsappNo = "", string _strDOA = "", string _strDOB = "", string strSpouse = "", double dAmtLimit = 0)
        {
            string strQuery = "", strDOB = "NULL", strDOA = "NULL";
            //if (_strDOA.Length==10)
            //    strDOA = "'" + dba.ConvertDateInExactFormat(_strDOA).ToString("MM/dd/yyyy") + "'";
            //if (_strDOB.Length==10)
            //    strDOB = "'" + dba.ConvertDateInExactFormat(_strDOB).ToString("MM/dd/yyyy") + "'";

            strQuery += " INSERT INTO [dbo].[SupplierOtherDetails] ([AreaCode],[AccountNo],[WaybillUserName],[WaybillPassword],[CompanyRegNo],[NameOfFirm],[OtherDetails],[NB_Manufacturing],[NB_SoleSellingAgent],[NB_Dealer],[NB_Agent],[NB_Assembler],[NB_Trader],[NC_Proprietary],[NC_Partnership],[NC_Private],[NC_Public],[Other],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[DOB],[DOA],[SpouseName],[Description]) VALUES "
                     + " ('" + strAreaCode + "',@ID,'" + strWhatsappNo + "','','','','','false','false','false','false','false','false','false','false','false','false','','" + MainPage.strLoginName + "','',1,0," + strDOB + "," + strDOA + ",'" + strSpouse + "','') "
                     + " INSERT INTO [dbo].[EditTrailDetails] ([BillType],[BillCode],[BillNo],[Date],[NetAmt],[UpdatedBy],[InsertStatus],[UpdateStatus],[EditStatus]) VALUES "
                     + "('PARTYMASTER','" + strAreaCode + "',@ID,DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE()))," + dAmtLimit + ",'" + MainPage.strLoginName + "',0,0,'CREATION') ";

            return strQuery;
        }


    }
}

