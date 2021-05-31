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
    public partial class EmailRegister : Form
    {
        DataBaseAccess dba;
        public EmailRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void EmailRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
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
        private void btnGO_Click(object sender, EventArgs e)
        {
            try
            {
                btnGO.Enabled = false;
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                    MessageBox.Show("Sorry ! Please enter date range or uncheck on date ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 else
                    GetAllData();
            }
            catch
            {
            }
            btnGO.Enabled = true;
        }

        private void GetAllData()
        {
            string strQuery = " Select *,Convert(varchar,Date,103)BDate,(CASE WHen Status=0 then  'FAILED' else 'SENT' end) BStatus from EmailDetails Where EmailID!='' ";

            strQuery += CreateQuery() + " Order by Date desc ";
            DataTable dt = dba.GetDataTable(strQuery);
            BindRecordWithGrid(dt);
        }

        private string CreateQuery()
        {
            string strQuery = "";
            if (chkDate.Checked)
            {
                DateTime fromDate = dba.ConvertDateInExactFormat(txtFromDate.Text), toDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                strQuery += " and Date>='" + fromDate + "' and Date<'" + toDate.AddDays(1) + "' ";
            }

            if (txtEmailID.Text != "")
                strQuery += " and EmailID Like ('%" + txtEmailID.Text + "%') ";

            if (txtMessage.Text != "")
                strQuery += " and (MessageBody Like('%" + txtMessage.Text + "%') OR Subject Like('%" + txtMessage.Text + "%') OR EmailType Like('%" + txtMessage.Text + "%')) ";
        
            if (rdoFailed.Checked)
                strQuery += " and Status=0 ";
            else if (rdoSent.Checked)
                strQuery += " and Status=1 ";

            return strQuery;
        }

        private void BindRecordWithGrid(DataTable dt)
        {
            dgrdDetails.Rows.Clear();
            chkAll.Checked = false;
            lblFailedEmails.Text = lblNetEmails.Text = lblSentEmails.Text = "0";
            double dSent = 0, dFailed = 0;
            string strMessageBody = "", strSubject = "";
            if (dt.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(dt.Rows.Count);
                int rowIndex = 0;
                foreach (DataRow row in dt.Rows)
                {
                    strMessageBody = Convert.ToString(row["MessageBody"]);
                    strSubject = Convert.ToString(row["Subject"]);
                    strMessageBody = strMessageBody.Replace("|", "'");
                    strSubject = strSubject.Replace("|", "'");
                    dgrdDetails.Rows[rowIndex].Cells["chkTick"].Value = false;
                    dgrdDetails.Rows[rowIndex].Cells["sNo"].Value = (rowIndex + 1) + ".";
                    dgrdDetails.Rows[rowIndex].Cells["date"].Value = row["BDate"];
                    dgrdDetails.Rows[rowIndex].Cells["emailid"].Value = row["EmailID"];
                    dgrdDetails.Rows[rowIndex].Cells["subject"].Value = strSubject;
                    dgrdDetails.Rows[rowIndex].Cells["status"].Value = row["BStatus"];
                    dgrdDetails.Rows[rowIndex].Cells["emailType"].Value = row["EmailType"];
                    dgrdDetails.Rows[rowIndex].Cells["message"].Value = strMessageBody;
                    dgrdDetails.Rows[rowIndex].Cells["filePath"].Value = row["FilePath"];
                    dgrdDetails.Rows[rowIndex].Cells["createdBy"].Value = row["createdBy"];
                    dgrdDetails.Rows[rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                    dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                    if (Convert.ToString(row["BStatus"]) == "FAILED")
                    {
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                        dFailed++;
                    }
                    else
                        dSent++;
                    rowIndex++;
                }
            }
            lblFailedEmails.Text = dFailed.ToString("N0", MainPage.indianCurancy);
            lblSentEmails.Text = dSent.ToString("N0", MainPage.indianCurancy);
            lblNetEmails.Text = (dSent + dFailed).ToString("N0", MainPage.indianCurancy);
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["chkTick"].Value = chkAll.Checked;
            }
            catch
            {
            }
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Enabled = false;
            try
            {
                SendingEmailPage objSend = new SendingEmailPage();
                objSend.Show();
            }
            catch
            {
            }
            btnSendEmail.Enabled = true;
        }

        private void btnResend_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    btnResend.Enabled = false;
                    DialogResult result = MessageBox.Show("Are you sure want to resend these selected emails ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int count = 0;
                        foreach (DataGridViewRow row in dgrdDetails.Rows)
                        {
                            if (Convert.ToBoolean(row.Cells["chkTick"].Value))
                                count += SendEmails(row);
                        }
                        if (count > 0)
                        {
                            MessageBox.Show("Mail sent successfully !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            ResetGridAfterSent();
                        }
                        else
                            MessageBox.Show("Sorry ! Unable to send email right now !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
            }
            btnResend.Enabled = true;
        }

        private void ResetGridAfterSent()
        {
            chkAll.Checked = false;
            foreach (DataGridViewRow row in dgrdDetails.Rows)
                row.Cells["chkTick"].Value = false;
        }

        private int SendEmails(DataGridViewRow row)
        {
            int count = 0;
            string strID, strEmailID, strSubject, strMessage, strFileName, strEmailType = "";
            strID = Convert.ToString(row.Cells["id"].Value);
            strEmailID = Convert.ToString(row.Cells["emailid"].Value);
            strSubject = Convert.ToString(row.Cells["subject"].Value);
            strMessage = Convert.ToString(row.Cells["message"].Value);
            strFileName = Convert.ToString(row.Cells["filePath"].Value);
            strEmailType = Convert.ToString(row.Cells["emailType"].Value);

            bool bStatus = DataBaseAccess.SendEmail(strEmailID, strSubject, strMessage, strFileName, strID, strEmailType,true);
            if (bStatus)
                count++;
            return count;
        }

        private void EmailRegister_Load(object sender, EventArgs e)
        {
            try
            {
                dba.EnableCopyOnClipBoard(dgrdDetails);
                if (!MainPage.mymainObject.bSMSReport)
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
