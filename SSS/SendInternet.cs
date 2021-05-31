using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace SSS
{
    public partial class SendInternet : Form
    {
        ForwardingData fd;
        DataBaseAccess dba;
        int roundCount = 0;
        bool _bStatus = true;
        public SendInternet()
        {
            try
            {
                InitializeComponent();
                fd = new ForwardingData();
                dba = new DataBaseAccess();
                GetRemoteCompanyUpdatedDate();
                forwardingTabs.TabPages[1].Hide();
                btnSend.Focus();
            }
            catch
            {
            }
        }

        public SendInternet(int status)
        {
            try
            {
                InitializeComponent();
                fd = new ForwardingData();
                dba = new DataBaseAccess();
                _bStatus = false;
            }
            catch
            {
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TransferRecord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

       
        private void CreateBackupFile()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\" + MainPage.strCompanyName + " Backup\\" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

                Directory.CreateDirectory(strPath);
                dba.CreateBackupWithCommand(strPath);
                //dba.UploadFile();

                MessageBox.Show("Thanks ! Backup Send on FTP Server Successfully ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                SendMail();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Backup not Created " + ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendMail()
        {
            try
            {
                string strMessage = "We have send our backup file on FTP Server of the Company :  " + MainPage.strCompanyName + "   ! Please Check it out. Thanks Regard ..." + MainPage.strLoginName;
                bool bStatus = DataBaseAccess.SendEmail("saraogi285@gmail.com", "Backup Sending Report", strMessage, "", "", "BACKUP SENT",false);
                if (bStatus)
                    MessageBox.Show("Mail Sent Successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch
            {
            }
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetRemoteCompanyUpdatedDate()
        {
            string strCompanyName = "";
            try
            {              
                object objValue = DataBaseAccess.ExecuteMyScalar(" Select * from master.dbo.sysdatabases where Name='CompanyInformation' ");
                if (Convert.ToString(objValue) != "")                
                {
                    DataTable table = dba.GetLastUpdatedDate();
                    if (table.Rows.Count > 0)
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
                                string strFileName = fi.Name;
                                foreach (DataRow row in table.Rows)
                                {
                                    if (Convert.ToString(row["CompanyName"]) == "A" + strFileName)
                                    {
                                        string strFilePath = strPath + "\\" + strFileName + "\\" + strFileName + ".syber";
                                        StreamReader sr=new StreamReader(strFilePath);

                                        strCompanyName += sr.ReadLine() + "   :  Last Updated - " + Convert.ToDateTime(row["UpdatedDate"]).ToString("dd/MM/yyyy h:mm:ss tt") + " && Updated By :  " + Convert.ToString(row["UpdatedBy"])+"\n";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            lblUpdatedTime.Text = strCompanyName;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (MainPage.strOnlineDataBaseName != "" && MainPage.strLiveDataBaseIP!="")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Send Data to Internet Server ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    btnSend.Text = "Please wait .....";
                    btnSend.Enabled = false;
                    SendData();
                }
                btnSend.Enabled = true;
                btnSend.Text = "&Send Data To Internet";
            }
            else
                MessageBox.Show("Sorry ! Online database is not configured in company master !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public int SendData()
        {
            int count = 0;
            try
            {
                if (MainPage.strOnlineDataBaseName != "" && MainPage.strLiveDataBaseIP != "" && MainPage.strLoginName != "")
                {
                    MainPage.OpenConnection();
                    SqlCommand cmd = new SqlCommand("ALTER DATABASE " + MainPage.strDataBaseFile + " SET Single_User WITH Rollback Immediate ", MainPage.con);
                    int result = cmd.ExecuteNonQuery();
                  
                    string strDeleteQuery = "", strInsertedQuery = "";
                    strDeleteQuery = DataBaseAccess.ReadDeleteQuery();
                    strInsertedQuery = DataBaseAccess.GetAllInsertedRecord();

                    if (strInsertedQuery != "")
                    {
                        strDeleteQuery += strInsertedQuery;
                    }
                    if (strDeleteQuery.Length > 15)
                    {
                        strDeleteQuery = " Declare @Region varchar(50),@IGSTName varchar(250), @SGSTName varchar(250), @IGSTFullName varchar(250), @SGSTFullName varchar(250), @_FinalAmt  numeric(18,2), @_PackingAmt numeric(18,2)=0 ,@TCSAccount varchar(250),@CashName varchar(250),@CardName varchar(250); "
                                       + strDeleteQuery;

                        count = DataBaseAccess.ExecuteQueryOnNet(strDeleteQuery, MainPage.strOnlineDataBaseName);
                        if (count > 0)
                        {
                            DataBaseAccess.DeleteRemoteQuery();
                            DataBaseAccess.SetMultiUserDataBase();
                            MessageBox.Show("Thank you ! Data sent to internet successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry No record found for sending  ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_bStatus)
                    MessageBox.Show("Warning : " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                DataBaseAccess.SetMultiUserDataBase();
            }
            return count;
        }
    }
}
