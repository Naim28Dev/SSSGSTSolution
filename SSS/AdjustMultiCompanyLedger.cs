using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace SSS
{
    public partial class AdjustMultiCompanyLedger : Form
    {
        DataBaseAccess dba;       
        ArrayList FolderName;
        DateTime fStartDate = MainPage.startFinDate, fEndDate = MainPage.endFinDate, sStartDate = MainPage.startFinDate, sEndDate = MainPage.endFinDate;
        public AdjustMultiCompanyLedger()
        {

            try
            {
                InitializeComponent();
                dba = new DataBaseAccess();             
                FolderName = new ArrayList();
                GetFolderName();
            }
            catch
            {
            }
        }

       
        private void GetFolderName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";
             
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    string[] Folder;
                    Folder = Directory.GetDirectories(strPath);
                    foreach (string folderName in Folder)
                    {
                        FileInfo fi = new FileInfo(folderName);
                        FolderName.Add(fi.Name);
                    }
                    //FolderName.Sort();
                }
                if (FolderName.Count > 0)
                {
                    GetCompanyNameFromFile();
                }
                else
                {
                    MessageBox.Show("Sorry ! Company not found  ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnFGo.Enabled = false;
                    btnSGo.Enabled = false;
                }
               
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in  Multiple Company ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void GetCompanyNameFromFile()
        {
            try
            {              
                string strPath = "";
             
                foreach (string strName in FolderName)
                {
                    //string strName = Convert.ToString(sName);
                    strPath = MainPage.strServerPath + "\\Data\\" + strName + "\\" + strName + ".syber";                   
                    StreamReader sr = new StreamReader(strPath);
                    string strCompany = sr.ReadLine();
                    sr.Close();
                    comboFCompanyName.Items.Add(strCompany);
                    comboSCompany.Items.Add(strCompany);
                }

                if (comboFCompanyName.Items.Count > 1)
                {
                    comboFCompanyName.SelectedIndex = comboFCompanyName.Items.Count-2;
                }
                if (comboSCompany.Items.Count > 1)
                {
                    comboSCompany.SelectedIndex = comboSCompany.Items.Count-1;
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Company Name from File in Multi Ledger Merging ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdjustMultiCompanyLedger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void GetFirstLedgerFromDataBase()
        {          
            try
            {
                string strCode = Convert.ToString(FolderName[comboFCompanyName.SelectedIndex]);
                DataTable dt = new DataTable();
                DateTime sDate = fStartDate, eDate = fEndDate.AddDays(1);
                if (chkSDate.Checked && txtFFromDate.Text.Length==10 && txtFToDate.Text.Length == 10)
                {
                    sDate = dba.ConvertDateInExactFormat(txtFFromDate.Text);// DateTime.Parse(dtpSStartDate.Value.ToString("MM/dd/yyyy"));
                    eDate = dba.ConvertDateInExactFormat(txtFToDate.Text).AddDays(1);// DateTime.Parse(dtpSEndDate.Value.ToString("MM/dd/yyyy")).AddDays(1);
                }

                if (txtFParty.Text != "" && strCode != "")
                {
                      string[] strFullName = txtFParty.Text.Split(' ');
                      if (strFullName.Length > 0)
                      {
                          AddFirstOpening(sDate, "A" + strCode);
                          dt = dba.GetLedgerAccountFromPrevious(strFullName[0], sDate, eDate, "A" + strCode);
                          BindDataWithFirstGrid(dt);
                      }
                }

            }
            catch
            {
            }
        }

        private void GetSecondLedgerFromDataBase()
        {            
            try
            {
                string strCode =Convert.ToString( FolderName[comboSCompany.SelectedIndex]);
                DataTable dt = new DataTable();

                DateTime sDate = sStartDate, eDate = sEndDate.AddDays(1);
                if (chkSDate.Checked)
                {
                    sDate = dba.ConvertDateInExactFormat(txtSFromDate.Text);// DateTime.Parse(dtpSStartDate.Value.ToString("MM/dd/yyyy"));
                    eDate = dba.ConvertDateInExactFormat(txtSToDate.Text).AddDays(1);// DateTime.Parse(dtpSEndDate.Value.ToString("MM/dd/yyyy")).AddDays(1);
                }
                if (txtSParty.Text != "" && strCode != "")
                {
                      string[] strFullName = txtFParty.Text.Split(' ');
                      if (strFullName.Length > 0)
                      {
                          //AddSecondOpening(sDate, "A" + strCode);
                          dt = dba.GetLedgerAccountFromPrevious(strFullName[0], sDate, eDate, "A" + strCode);
                          BindDataWithSecondGrid(dt);
                      }
                }
            }
            catch
            {
            }
        }

        private void btnFGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnFGo.Enabled = false;
                dgrdFLedger.Rows.Clear();
                chkFAll.Checked = false;
                GetFirstLedgerFromDataBase();
                comboSCompany.Focus();
            }
            catch
            {
            }
            finally
            {
                btnFGo.Enabled = true;
            }
        }

        private void btnSGo_Click(object sender, EventArgs e)
        {
            try
            {
                btnSGo.Enabled = false;
                dgrdSLedger.Rows.Clear();
                chkSAll.Checked = false;
                GetSecondLedgerFromDataBase();
            }
            catch
            {
            }
            finally
            {
                btnSGo.Enabled = true;
            }
        }

        private void BindDataWithFirstGrid(DataTable dt)
        {
            try
            {
                int j = dgrdFLedger.Rows.Count;
                if (dt.Rows.Count > 0)
                {
                    dgrdFLedger.Rows.Add(dt.Rows.Count);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        double fAmount = 0;
                        DataRow dr = dt.Rows[i];
                        dgrdFLedger.Rows[j].Cells[0].Value = dr[0];
                        dgrdFLedger.Rows[j].Cells[1].Value = false;
                        dgrdFLedger.Rows[j].Cells[2].Value = Convert.ToDateTime(dr[1].ToString());//.ToString("dd/MM/yyyy");
                        dgrdFLedger.Rows[j].Cells[3].Value = dr["NAccountStatus"];
                        dgrdFLedger.Rows[j].Cells[4].Value = dr[5];
                        dgrdFLedger.Rows[j].Cells["hide"].Value = Convert.ToBoolean(dr["Tick"]);

                        if (Convert.ToBoolean(dr["Tick"].ToString()))
                        {
                            dgrdFLedger.Rows[j].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(137)))), ((int)(((byte)(73)))));
                        }
                        
                            fAmount = dba.ConvertObjectToDouble(dr[6]);                       

                        if ( Convert.ToString(dr[4]).ToLower() == "debit")
                        {
                            dgrdFLedger.Rows[j].Cells[5].Value = fAmount;
                            dgrdFLedger.Rows[j].Cells[6].Value = "";
                        }
                        else if (Convert.ToString(dr[4]).ToLower() == "credit")
                        {
                            dgrdFLedger.Rows[j].Cells[5].Value = "";
                            dgrdFLedger.Rows[j].Cells[6].Value = fAmount;
                        }
                        dgrdFLedger.Rows[j].Cells[7].Value = "00.00";
                        j++;

                    }

                    CalculateFirstBalance();
                }
            }
            catch
            {
            }
        }

        private void BindDataWithSecondGrid(DataTable dt)
        {
            try
            {
                int j = dgrdSLedger.Rows.Count;
                if (dt.Rows.Count > 0)
                {
                    dgrdSLedger.Rows.Add(dt.Rows.Count);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        double fAmount = 0;
                        DataRow dr = dt.Rows[i];
                        dgrdSLedger.Rows[j].Cells[0].Value = dr[0];
                        dgrdSLedger.Rows[j].Cells[1].Value = false;
                        dgrdSLedger.Rows[j].Cells[2].Value = Convert.ToDateTime(dr[1].ToString());//.ToString("dd/MM/yyyy");
                        dgrdSLedger.Rows[j].Cells[3].Value = dr["NAccountStatus"];
                        dgrdSLedger.Rows[j].Cells[4].Value = dr[5];
                        dgrdSLedger.Rows[j].Cells[8].Value = Convert.ToBoolean(dr["Tick"]);

                        if (Convert.ToBoolean(dr["Tick"]))
                        {
                            dgrdSLedger.Rows[j].DefaultCellStyle.BackColor = Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(137)))), ((int)(((byte)(73)))));
                        }

                        fAmount = dba.ConvertObjectToDouble(dr[6]);

                        if (Convert.ToString(dr[4]).ToLower() == "debit")
                        {
                            dgrdSLedger.Rows[j].Cells[5].Value = fAmount;
                            dgrdSLedger.Rows[j].Cells[6].Value = "";
                        }
                        else if (Convert.ToString(dr[4]).ToLower() == "credit")
                        {
                            dgrdSLedger.Rows[j].Cells[5].Value = "";
                            dgrdSLedger.Rows[j].Cells[6].Value = fAmount;
                        }
                        dgrdSLedger.Rows[j].Cells[7].Value = "00.00";
                        j++;

                    }

                    CalculateSecondBalance();
                }
            }
            catch
            {
            }
        }

        #region Add Opening Account

        private void AddFirstOpening(DateTime date, string strDataBase)
        {
            try
            {
                string[] strFullName = txtFParty.Text.Split(' ');
                if (strFullName.Length > 0)
                {

                    double dOpeningAmt = dba.GetOpeningAccountFromPrevious(strFullName[0], date, strDataBase);

                    if (dOpeningAmt != 0)
                    {

                        dgrdFLedger.Rows.Add(1);

                        dgrdFLedger.Rows[0].Cells[1].Value = 0;
                        dgrdFLedger.Rows[0].Cells[1].Value = (Boolean)false;
                        dgrdFLedger.Rows[0].Cells[2].Value = date;
                        dgrdFLedger.Rows[0].Cells[3].Value = "OPENING";
                        dgrdFLedger.Rows[0].Cells[4].Value = "";
                        dgrdFLedger.Rows[0].Cells["hide"].Value = false;


                        if (dOpeningAmt < 0)
                        {
                            dgrdFLedger.Rows[0].Cells[5].Value = "";
                            dgrdFLedger.Rows[0].Cells[6].Value = Math.Abs(dOpeningAmt).ToString("N2", MainPage.indianCurancy);
                        }
                        else if (dOpeningAmt > 0)
                        {
                            dgrdFLedger.Rows[0].Cells[5].Value = dOpeningAmt.ToString("N2", MainPage.indianCurancy);
                            dgrdFLedger.Rows[0].Cells[6].Value = "";
                        }
                        dgrdFLedger.Rows[0].Cells[7].Value = Math.Abs(dOpeningAmt).ToString("N2", MainPage.indianCurancy);

                    }       
                }
            }
            catch
            {
            }
        }

        #endregion

        public void CalculateFirstBalance()
        {
            try
            {
                dgrdFLedger.Sort(dgrdFLedger.Columns[2], ListSortDirection.Ascending);
                dgrdFLedger.Columns[2].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                int i = 0;

                if (dgrdFLedger.Rows.Count > 0)
                {
                    string strBalance = "";
                    foreach (DataGridViewRow dr in dgrdFLedger.Rows)
                    {
                        double fAmtDr = 0, fAmtCr = 0;

                            fAmtDr = ConvertObjectToDouble(dr.Cells[5].Value);                     
                            fAmtCr = ConvertObjectToDouble(dr.Cells[6].Value);

                            if (i == 0)
                            {
                                if (fAmtDr > 0)
                                {
                                    dr.Cells[7].Value = fAmtDr.ToString("N2", MainPage.indianCurancy) + " Dr";

                                    //strBalance = fAmtDr.ToString() + " Dr";
                                }
                                else if (fAmtCr > 0)
                                {
                                    dr.Cells[7].Value = fAmtCr.ToString("N2", MainPage.indianCurancy) + " Cr";
                                    //strBalance = fAmtCr.ToString() + " Cr";
                                }
                                else
                                {
                                    dr.Cells[7].Value = "00.00";
                                }
                                i++;
                            }
                            else
                            {
                                double fBal = 0;
                                try
                                {
                                    string amount = strBalance.Substring(0, strBalance.Length - 2);
                                    if (amount != "")
                                    {
                                        fBal = ConvertObjectToDouble(amount);
                                    }
                                }
                                catch
                                {
                                }

                                if (fAmtDr > 0)
                                {
                                    if (strBalance.Contains("Dr"))
                                    {
                                        dr.Cells[7].Value = (fAmtDr + fBal).ToString("N2", MainPage.indianCurancy) + " Dr";

                                    }
                                    else if (strBalance.Contains("Cr"))
                                    {
                                        double amt = fBal - fAmtDr;
                                        if (amt > 0)
                                        {
                                            dr.Cells[7].Value = amt.ToString("N2", MainPage.indianCurancy) + " Cr";
                                        }
                                        else if (amt < 0)
                                        {
                                            dr.Cells[7].Value = Math.Abs(amt).ToString("N2", MainPage.indianCurancy) + " Dr";
                                        }
                                        else if (amt == 0)
                                        {
                                            dr.Cells[7].Value = "00.00";
                                        }
                                    }
                                    else
                                    {
                                        dr.Cells[7].Value = fAmtDr.ToString("N2", MainPage.indianCurancy) + " Dr";
                                    }
                                    // strBalance = dr.Cells[7].Value.ToString();

                                }
                                else if (fAmtCr > 0)
                                {
                                    if (strBalance.Contains("Cr"))
                                    {
                                        dr.Cells[7].Value = (fAmtCr + fBal).ToString("N2", MainPage.indianCurancy) + " Cr";

                                    }
                                    else if (strBalance.Contains("Dr"))
                                    {
                                        double amt = fBal - fAmtCr;
                                        if (amt > 0)
                                        {
                                            dr.Cells[7].Value = amt.ToString("N2", MainPage.indianCurancy) + " Dr";
                                        }
                                        else if (amt < 0)
                                        {
                                            dr.Cells[7].Value = Math.Abs(amt).ToString("N2", MainPage.indianCurancy) + " Cr";
                                        }
                                        else if (amt == 0)
                                        {
                                            dr.Cells[7].Value = "00.00";
                                        }
                                    }
                                    else
                                    {
                                        dr.Cells[7].Value = fAmtCr.ToString("N2", MainPage.indianCurancy) + " Cr";
                                    }


                                }
                                else
                                {
                                    dr.Cells[7].Value = strBalance;
                                }

                            }
                        strBalance = dr.Cells[7].Value.ToString();
                    }
                }
            }
            catch
            {
              
            }
        }

        public void CalculateSecondBalance()
        {
            try
            {
                dgrdSLedger.Sort(dgrdSLedger.Columns[2], ListSortDirection.Ascending);
                dgrdSLedger.Columns[2].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                int i = 0;

                if (dgrdSLedger.Rows.Count > 0)
                {
                    string strBalance = "";
                    foreach (DataGridViewRow dr in dgrdSLedger.Rows)
                    {
                        double fAmtDr = 0, fAmtCr = 0;

                        fAmtDr = ConvertObjectToDouble(dr.Cells[5].Value);
                        fAmtCr = ConvertObjectToDouble(dr.Cells[6].Value);

                        if (i == 0)
                        {
                            if (fAmtDr > 0)
                            {
                                dr.Cells[7].Value = fAmtDr.ToString("N2", MainPage.indianCurancy) + " Dr";

                                //strBalance = fAmtDr.ToString() + " Dr";
                            }
                            else if (fAmtCr > 0)
                            {
                                dr.Cells[7].Value = fAmtCr.ToString("N2", MainPage.indianCurancy) + " Cr";
                                //strBalance = fAmtCr.ToString() + " Cr";
                            }
                            else
                            {
                                dr.Cells[7].Value = "00.00";
                            }
                            i++;
                        }
                        else
                        {
                            double fBal = 0;
                            try
                            {
                                string amount = strBalance.Substring(0, strBalance.Length - 2);
                                if (amount != "")
                                {
                                    fBal = ConvertObjectToDouble(amount);
                                }
                            }
                            catch
                            {
                            }

                            if (fAmtDr > 0)
                            {
                                if (strBalance.Contains("Dr"))
                                {
                                    dr.Cells[7].Value = (fAmtDr + fBal).ToString("N2", MainPage.indianCurancy) + " Dr";

                                }
                                else if (strBalance.Contains("Cr"))
                                {
                                    double amt = fBal - fAmtDr;
                                    if (amt > 0)
                                    {
                                        dr.Cells[7].Value = amt.ToString("N2", MainPage.indianCurancy) + " Cr";
                                    }
                                    else if (amt < 0)
                                    {
                                        dr.Cells[7].Value = Math.Abs(amt).ToString("N2", MainPage.indianCurancy) + " Dr";
                                    }
                                    else if (amt == 0)
                                    {
                                        dr.Cells[7].Value = "00.00";
                                    }
                                }
                                else
                                {
                                    dr.Cells[7].Value = fAmtDr.ToString("N2", MainPage.indianCurancy) + " Dr";
                                }
                                // strBalance = dr.Cells[7].Value.ToString();

                            }
                            else if (fAmtCr > 0)
                            {
                                if (strBalance.Contains("Cr"))
                                {
                                    dr.Cells[7].Value = (fAmtCr + fBal).ToString("N2", MainPage.indianCurancy) + " Cr";

                                }
                                else if (strBalance.Contains("Dr"))
                                {
                                    double amt = fBal - fAmtCr;
                                    if (amt > 0)
                                    {
                                        dr.Cells[7].Value = amt.ToString("N2", MainPage.indianCurancy) + " Dr";
                                    }
                                    else if (amt < 0)
                                    {
                                        dr.Cells[7].Value = Math.Abs(amt).ToString("N2", MainPage.indianCurancy) + " Cr";
                                    }
                                    else if (amt == 0)
                                    {
                                        dr.Cells[7].Value = "00.00";
                                    }
                                }
                                else
                                {
                                    dr.Cells[7].Value = fAmtCr.ToString("N2", MainPage.indianCurancy) + " Cr";
                                }


                            }
                            else
                            {
                                dr.Cells[7].Value = strBalance;
                            }

                        }
                        strBalance = dr.Cells[7].Value.ToString();
                    }
                }
            }
            catch
            {
            }
        }

        private double GetFirstGridViewAmount()
        {
            double dDebit = 0, dCredit = 0;
            try
            {
                foreach (DataGridViewRow dr in dgrdFLedger.Rows)
                {
                    bool status = false;
                    try
                    {
                        status = Convert.ToBoolean(dr.Cells["hide"].Value);
                    }
                    catch
                    {                       
                    }
                    if (status)
                    {
                        dDebit += ConvertObjectToDouble(dr.Cells["debit"].Value);
                        dCredit += ConvertObjectToDouble(dr.Cells["credit"].Value);
                    }
                }
               
            }
            catch
            {
            }
            dDebit = Math.Round(dDebit, 2);
            dCredit = Math.Round(dCredit, 2);
            dDebit = Math.Round((dDebit - dCredit), 2);
            return dDebit;
        }

        private double GetSecondGridViewAmount()
        {
            double dDebit = 0, dCredit = 0;
            try
            {
                foreach (DataGridViewRow dr in dgrdSLedger.Rows)
                {
                    bool status = false;
                    try
                    {
                        status = Convert.ToBoolean(dr.Cells[8].Value);
                    }
                    catch
                    {
                    }
                    if (status)
                    {
                        dDebit += ConvertObjectToDouble(dr.Cells[5].Value);
                        dCredit += ConvertObjectToDouble(dr.Cells[6].Value);
                    }
                }

            }
            catch
            {
            }
            dDebit = Math.Round(dDebit, 2);
            dCredit = Math.Round(dCredit, 2);

            dDebit = Math.Round((dDebit - dCredit), 2);
            return dDebit;
        }

        private double ConvertObjectToDouble(object objAmt)
        {
            double dAmount = 0;
            try
            {
                dAmount = Convert.ToDouble(objAmt);
            }
            catch
            {
            }
            return dAmount;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            double dFirstAmount = GetFirstGridViewAmount(),dSecondAmount=GetSecondGridViewAmount();
          
            double dTotalAmount=dFirstAmount + dSecondAmount;
            if (dTotalAmount == 0)
            {
                 DialogResult dr = MessageBox.Show("Are you sure want to Adjust  "+ Math.Abs(dFirstAmount).ToString("N2", MainPage.indianCurancy) +"  ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                 if (dr == DialogResult.Yes)
                 {
                     UpdateAdjustedEntries();
                     AdjustFirstCompanyAmount();
                     dgrdFLedger.Rows.Clear();
                     dgrdSLedger.Rows.Clear();
                     GetFirstLedgerFromDataBase();
                     GetSecondLedgerFromDataBase();
                 }
            }
            else
            {
                MessageBox.Show("First Compnay Amount  and Second Company Amount does not match, Amount Difference is : " + dTotalAmount.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void AdjustFirstCompanyAmount()
        {
            try
            {
                string strCode = Convert.ToString(FolderName[comboFCompanyName.SelectedIndex]);

                if (dgrdFLedger.Rows.Count > 0)
                {
                     string[] strFullName = txtFParty.Text.Split(' ');
                     if (strFullName.Length > 0)
                     {

                         string strQuery = "";
                         DateTime strDate = dba.ConvertDateInExactFormat(txtFFromDate.Text);// DateTime.Parse(dtpFStartdate.Value.ToString("MM/dd/yyyy"));

                         foreach (DataGridViewRow dr in dgrdFLedger.Rows)
                         {
                             Boolean chk = Convert.ToBoolean(dr.Cells[8].Value);
                             string strID = Convert.ToString(dr.Cells[0].Value), strOpening = Convert.ToString(dr.Cells[3].Value);
                             if (chk)
                             {
                                 if (strOpening == "OPENING")
                                 {
                                     strQuery += dba.UpdateOpeningBalanceAmountTickFromPreviousDataBase(strFullName[0], strDate, "True");

                                 }
                                 else if (strID != "0")
                                 {
                                     strQuery += dba.UpdateBalanceAmountTickFromPreviousDataBase(strID, "True");
                                 }
                             }
                         }
                         AdjustSecondCompanyAmount(strQuery, "A" + strCode);
                     }
                }
            }
            catch
            {
            }
        }

        private void AdjustSecondCompanyAmount(string strFirstQuery,string strFirstDataBase)
        {
            try
            {
                int count = 0;
                string strCode = Convert.ToString(FolderName[comboSCompany.SelectedIndex]);

                if (dgrdSLedger.Rows.Count > 0)
                {
                      string[] strFullName = txtFParty.Text.Split(' ');
                      if (strFullName.Length > 0)
                      {
                          DateTime strDate = dba.ConvertDateInExactFormat(txtSFromDate.Text);// DateTime.Parse(dtpSStartDate.Value.ToString("MM/dd/yyyy"));
                          string strQuery = "";
                          foreach (DataGridViewRow dr in dgrdSLedger.Rows)
                          {
                              Boolean chk = Convert.ToBoolean(dr.Cells[8].Value);
                              string strID = Convert.ToString(dr.Cells[0].Value), strOpening = Convert.ToString(dr.Cells[3].Value);
                              if (chk)
                              {
                                  if (strOpening == "OPENING")
                                  {
                                      strQuery += dba.UpdateOpeningBalanceAmountTickFromPreviousDataBase(strFullName[0], strDate, "True");
                                  }
                                  else if (strID != "0")
                                  {
                                      strQuery += dba.UpdateBalanceAmountTickFromPreviousDataBase(strID, "True");
                                  }
                              }
                          }
                          if (strFirstQuery != "" && strQuery != "")
                          {
                              count = dba.ExecutingQueryOfOtherDatabase(strFirstQuery, strFirstDataBase, strQuery, "A" + strCode);
                          }
                          if (count > 0)
                          {
                              MessageBox.Show("Thank You Entry Adjusted  Successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                          }
                          else
                          {
                              MessageBox.Show("Sorry ! Entry Can't be updated please try again later", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                          }
                      }
                }

            }
            catch
            {
            }
        }

        private void UpdateAdjustedEntries()
        {
            try
            {
                int count = 0;
                if (dgrdFLedger.Rows.Count > 0 && dgrdSLedger.Rows.Count > 0)
                {
                    DateTime strDate = dba.ConvertDateInExactFormat(txtFFromDate.Text);

                    string strNumber = dba.GetMultiCompanyAdjustedID();
                    string[] strFullName = txtFParty.Text.Split(' ');
                    if (strFullName.Length > 0)
                    {

                        //First Grid
                        string strFCode = Convert.ToString(FolderName[comboFCompanyName.SelectedIndex]);
                        string strQuery = "";
                        foreach (DataGridViewRow dr in dgrdFLedger.Rows)
                        {
                            string strOpening = Convert.ToString(dr.Cells[3].Value), strID = Convert.ToString(dr.Cells[0].Value);
                            Boolean chk = Convert.ToBoolean(dr.Cells["hide"].Value);

                            if (chk)
                            {
                                if (strOpening == "OPENING")
                                {
                                    count = dba.UpdateAdjustedOpeningBalanceAmountInMultiCompany(strFullName[0], strDate, strNumber, "A" + strFCode);
                                }
                                else
                                {
                                    // count += dba.UpdateAdjustedAmountInMultiCompany(strID, strNumber, "A" + strFCode);
                                    strQuery += dba.GetAdjustedAmountInMultiCompanyQuery(strID, strNumber, "A" + strFCode);
                                }
                            }
                        }

                        if(strQuery != "")                        
                            count += dba.ExecuteMyQuery(strQuery);

                        strQuery = "";
                        //Second Grid
                        DateTime strSDate = dba.ConvertDateInExactFormat(txtSFromDate.Text);
                        string strSCode = Convert.ToString(FolderName[comboSCompany.SelectedIndex]);

                        foreach (DataGridViewRow dr in dgrdSLedger.Rows)
                        {
                            string strOpening = Convert.ToString(dr.Cells[3].Value), strID = Convert.ToString(dr.Cells[0].Value);
                            Boolean chk = Convert.ToBoolean(dr.Cells["hide2"].Value);

                            if (chk)
                            {
                                if (strOpening == "OPENING")
                                {
                                    count = dba.UpdateAdjustedOpeningBalanceAmountInMultiCompany(strFullName[0], strSDate, strNumber, "A" + strSCode);
                                }
                                else
                                {
                                    //count += dba.UpdateAdjustedAmountInMultiCompany(strID, strNumber, "A" + strSCode);
                                    strQuery += dba.GetAdjustedAmountInMultiCompanyQuery(strID, strNumber, "A" + strSCode);
                                }
                            }
                        }

                        if (strQuery != "")
                            count += dba.ExecuteMyQuery(strQuery);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgrdFLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdFLedger.CurrentRow.Index;
                    if (dgrdFLedger.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdFLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdFLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch
            {
            }
        }

        private void dgrdSLedger_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    int rowIndex = dgrdSLedger.CurrentRow.Index;
                    if (dgrdSLedger.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdSLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdSLedger.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }

                }
            }
            catch
            {
            }
        }

        private void comboFCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {         
                string strCode = Convert.ToString(FolderName[comboFCompanyName.SelectedIndex]).Trim();
                DateTime[] date = dba.GetFinancialDateTime("A" + strCode,strCode);
                
                fStartDate = date[0];
                 txtFFromDate.Text = fStartDate.ToString("dd/MM/yyyy");
                 fEndDate = date[1];
                 txtFToDate.Text = fEndDate.ToString("dd/MM/yyyy");
                
                dgrdFLedger.Rows.Clear();
            }
            catch
            {
            }
        }

        private void comboSCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strCode = Convert.ToString(FolderName[comboSCompany.SelectedIndex]);
                DateTime[] date = dba.GetFinancialDateTime("A" + strCode,strCode);
                sStartDate = date[0];
                txtSFromDate.Text = sStartDate.ToString("dd/MM/yyyy");
                sEndDate = date[1];
                txtSToDate.Text = sEndDate.ToString("dd/MM/yyyy");
                dgrdSLedger.Rows.Clear();
            }
            catch
            {
            }
        }

        private void txtFParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    ReadWriteDataOnCard objRead = new ReadWriteDataOnCard();
                    string strData = objRead.ReadDataFromCard("ALLPARTY");
                    if (strData != "")
                    {
                        txtFParty.Text = txtSParty.Text = strData;
                        dgrdFLedger.Rows.Clear();
                        dgrdSLedger.Rows.Clear();
                    }
                }
                else
                {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        if (objSearch.strSelectedData != "")
                        {
                            txtFParty.Text = txtSParty.Text = objSearch.strSelectedData;
                            dgrdFLedger.Rows.Clear();
                            dgrdSLedger.Rows.Clear();
                        }
                    }
                    else
                        e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtFFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtFFromDate_Leave(object sender, EventArgs e)
        {
            MainPage.multiQSDate = fStartDate;
            MainPage.multiQEDate = fEndDate;
            dba.GetDateInExactFormat(sender, chkFDate.Checked, false, false, true);
        }

        private void txtSFromDate_Leave(object sender, EventArgs e)
        {
            MainPage.multiQSDate = sStartDate;
            MainPage.multiQEDate = sEndDate;
            dba.GetDateInExactFormat(sender, chkSDate.Checked, false, false, true);
        }

        private void dgrdFLedger_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkFAll.Visible = false;
                    else
                        chkFAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void dgrdSLedger_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkSAll.Visible = false;
                    else
                        chkSAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void chkFAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdFLedger.Rows)
                {
                    row.Cells["hide"].Value = chkFAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void chkSAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdSLedger.Rows)
                {
                    row.Cells["hide2"].Value = chkSAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void chkFDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFFromDate.Enabled = txtFToDate.Enabled = chkFDate.Checked;
            txtFFromDate.Text = fStartDate.ToString("dd/MM/yyyy");
            txtFToDate.Text = fEndDate.ToString("dd/MM/yyyy");
        }

        private void chkSDate_CheckedChanged(object sender, EventArgs e)
        {
            txtSFromDate.Enabled = txtSToDate.Enabled = chkSDate.Checked;
            txtSFromDate.Text = sStartDate.ToString("dd/MM/yyyy");
            txtSToDate.Text = sEndDate.ToString("dd/MM/yyyy");
        }        

    }
}
