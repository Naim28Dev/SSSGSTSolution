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
    public partial class SMSReportRegister : Form
    {
        DataTable table = null;
        DataBaseAccess dba;
        SendSMS objSMS;
        string strOldQuery = "";

        public SMSReportRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            objSMS = new SendSMS();
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");

            if (!MainPage.strUserRole.Contains("ADMIN"))
                btnDelete.Enabled = false;
        }             

        private void GetAdvanceSearchedRecord()
        {
            try
            {
                string strQuery = "Select * from SMSReport Where ID!=0 ";
                string query = CreateQuery();
                if (query != "")
                {
                    strQuery = strQuery + " " + query;
                }
                strQuery += " Order By Date Desc ";
                strOldQuery = strQuery;
                table = dba.GetDataTable(strQuery);
                BindDataWithGrid();                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Advance searched Record in Show SMS Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                
                if (txtPartyName.Text != "")
                {
                    strQuery = " and CAST(MobileNo as varchar) in (Select MobileNo from SupplierMaster Where (AreaCode+CAST(AccountNo as varchar)+' '+Name) ='" + txtPartyName.Text + "') ";
                }
              
                if (chkDate.Checked)
                {
                    DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1) ;
                    strQuery += " and (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + eDate.ToString("MM/dd/yyyy") + "') ";
                }

                if (txtMobileNo.Text != "")
                {
                    strQuery += " and  Cast(MobileNo as varchar) Like('%" + txtMobileNo.Text + "%')  ";
                }

                if (txtMessage.Text != "")
                {
                    strQuery += " and  TextMessage Like('%" + txtMessage.Text + "%') ";
                }

                if (GetStatus() != "")
                {
                    strQuery += " and  Status ='" + GetStatus() + "'  ";
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Creating Query in SMS Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            return strQuery;
        }

        private string GetStatus()
        {
            string strStatus = "";
            if (rdoSent.Checked)            
                strStatus = "SENT";            
            else if (rdoFailed.Checked)            
                strStatus = "FAILED";            
            return strStatus;
        }

        private void BindDataWithGrid()
        {
            try
            {                
                int rowIndex = 0,j=0,k=0;
                dgrdSMS.Rows.Clear();
                if (table.Rows.Count > 0)
                {
                    dgrdSMS.Rows.Add(table.Rows.Count);
                    foreach (DataRow dr in table.Rows)
                    {
                        string strMessage = Convert.ToString(dr["TextMessage"]).ToUpper();
                        dgrdSMS.Rows[rowIndex].Cells["chkStatus"].Value = false;
                        dgrdSMS.Rows[rowIndex].Cells["date"].Value = dr["Date"];
                        if(strMessage.Contains("SALE BILL"))                        
                            dgrdSMS.Rows[rowIndex].Cells["senderID"].Value = "SALES";                        
                        else if (strMessage.Contains("YOUR AMOUNT"))                        
                            dgrdSMS.Rows[rowIndex].Cells["senderID"].Value = "CASH";                       
                        else if (strMessage.Contains("COURIER FROM"))                        
                            dgrdSMS.Rows[rowIndex].Cells["senderID"].Value = "COURIER";                        
                        else                        
                            dgrdSMS.Rows[rowIndex].Cells["senderID"].Value = "OTHER";
                        
                        dgrdSMS.Rows[rowIndex].Cells["mobileNo"].Value = dr["MobileNo"];
                        dgrdSMS.Rows[rowIndex].Cells["message"].Value = strMessage;
                        dgrdSMS.Rows[rowIndex].Cells["smsStatus"].Value = dr["Status"];
                        dgrdSMS.Rows[rowIndex].Cells["sendedBy"].Value = dr["SendBy"];
                        dgrdSMS.Rows[rowIndex].Cells["smsResendedBy"].Value = dr["UpdatedBy"];
                        dgrdSMS.Rows[rowIndex].Cells["smsID"].Value = dr["ID"];

                        if (Convert.ToString(dr["Status"]) == "SENT")
                        {
                            j++;
                        }
                        else
                        {
                            k++;
                        }
                        rowIndex++;
                    }
                }
                lblAllSMS.Text = table.Rows.Count.ToString("N0",MainPage.indianCurancy);
                lblSentSMS.Text = j.ToString("N0", MainPage.indianCurancy);
                lblFailedSMS.Text = k.ToString("N0", MainPage.indianCurancy);             
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Binding Data with Gridview in Show SMS Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            picPleasewait.Visible = true;
            btnGo.Enabled = false;
            if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                GetAdvanceSearchedRecord();
            btnGo.Enabled = true;
            picPleasewait.Visible = false;
        }

        private void ResendSMS()
        {
            try
            {
                if (dgrdSMS.Rows.Count > 0)
                {
                    DialogResult dResult = MessageBox.Show("Are you sure you want to send SMS  ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dResult == DialogResult.Yes)
                    {
                        bool sendingStatus = false;
                        foreach (DataGridViewRow row in dgrdSMS.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells[0].Value))
                            {
                                string strID = Convert.ToString(row.Cells["smsID"].Value), strMobileNo = Convert.ToString(row.Cells["mobileNo"].Value), strMessage = Convert.ToString(row.Cells["message"].Value);
                                if (strID != "" && strMobileNo != "" && strMessage != "")
                                {
                                    string strResult = objSMS.SendSingleSMS(strMessage, strMobileNo, strID);
                                    if (strResult.Contains("success"))
                                    {
                                        sendingStatus = true;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sorry ! Please Try Again  !  On this Mobile No : " + strMobileNo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                        if (sendingStatus)
                        {
                            MessageBox.Show("Message Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            table = dba.GetDataTable(strOldQuery);
                            BindDataWithGrid();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnResend_Click(object sender, EventArgs e)
        {
            btnResend.Enabled = false;
            ResendSMS();
            btnResend.Enabled = true;
        }

        private void dgrdSMS_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                e.Cancel = true;
            }
        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SMSReportRegister_KeyDown(object sender, KeyEventArgs e)
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

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdSMS.Rows)
                {
                    row.Cells[0].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdSMS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                int rowIndex = dgrdSMS.CurrentRow.Index;
                if (rowIndex >= 0)
                {
                    if (dgrdSMS.Rows[rowIndex].DefaultCellStyle.BackColor.Name != "LightGray")
                    {
                        dgrdSMS.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dgrdSMS.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdSMS.Rows.Count > 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure want to Delete these messages...? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strID = "";
                        foreach (DataGridViewRow row in dgrdSMS.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["chkStatus"].Value))
                            {
                                if (Convert.ToString(row.Cells["smsStatus"].Value) == "FAILED")
                                {
                                    if (strID == "")
                                    {
                                        strID = row.Cells["smsID"].Value + "";
                                    }
                                    else
                                    {
                                        strID += "," + row.Cells["smsID"].Value;
                                    }
                                }
                            }
                        }

                        if (strID != "")
                        {
                            string strQuery = "  Delete from SMSReport Where ID in (" + strID + ") and Status='FAILED' ";
                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                MessageBox.Show("Thank you ! SMS Deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                GetAdvanceSearchedRecord();
                            }
                            else
                            {
                                MessageBox.Show("Sorry ! Unable to delete SMS ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
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

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtPartyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ALLPARTY", "SEARCH PARTY NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPartyName.Text = objSearch.strSelectedData;

                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            try
            {
                SendUnicodeSMS objUnicode = new SendUnicodeSMS();
                objUnicode.MdiParent = MainPage.mymainObject;
                objUnicode.Show();
            }
            catch
            {
            }
        }

        private void SMSReportRegister_Load(object sender, EventArgs e)
        {
            dba.EnableCopyOnClipBoard(dgrdSMS);
        }
    }
}
