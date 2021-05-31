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
    public partial class WriteOnCard : Form
    {
        DataBaseAccess dba;
        #region Variables
        public static int g_rHandle, g_retCode;
        public static bool g_isConnected = false;
        public byte g_Sec;
        public static byte[] g_pKey = new byte[6];
        ComboBox cbPort = new ComboBox();
        string TidNumber, dstrpass1, CardBalance;
        string WriteValue;
        byte Blck2;
        DataTable dtmiss = new DataTable();
        DataRow drmiss;
        #endregion
        public WriteOnCard()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void WriteOnCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
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
                    if (objSearch.strSelectedData != "")
                        txtPartyName.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void WriteOnCard_Load(object sender, EventArgs e)
        {
            try
            {
                if (!(MainPage.mymainObject.bPartyMasterEdit))
                    this.Close();
                else
                {
                    cbPort.Items.Add("USB1");
                    cbPort.SelectedIndex = 0;
                    Connect();
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private bool GetPartyCardStatus()
        {
            string strQuery = "Select CardStatus from SupplierMaster Where (AreaCode+Cast(AccountNo as varchar)+' '+Name)='"+txtPartyName.Text+"' ";
            object objValue = DataBaseAccess.ExecuteMyScalar(strQuery);
            if (objValue != null)
            {
                if (Convert.ToBoolean(objValue))
                {
                    DialogResult result = MessageBox.Show("A Card already generated to this party !! Are you want to generate again ??", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return false;
        }

        private int SetCardStatus()
        {
            int count = 0;
            string strQuery = "Update SupplierMaster Set CardStatus=1 Where (AreaCode+Cast(AccountNo as varchar)+' '+Name)='" + txtPartyName.Text + "' ";
            count = dba.ExecuteMyQuery(strQuery);
            return count;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPartyName.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to write data on card ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        btnWrite.Text = "Please wait ....";
                        btnWrite.Enabled = false;
                        if (GetPartyCardStatus())
                        {
                            int count = WriteData();
                            if (count ==0)
                            {
                                SetCardStatus();
                                MessageBox.Show("Thank you ! Record successfully write on card !! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtPartyName.Clear();
                            }
                            else
                                MessageBox.Show("Sorry ! Unable to write please try after some time !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                    MessageBox.Show("Sorry ! Party name is required for writing data on card !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
            }
            btnWrite.Enabled = true;
            btnWrite.Text="&Write On Card";
        }

        private int WriteData()
        {
            int result=0;
            string strCardName = "";
            try
            {
                g_Sec = Convert.ToByte("1");
                Blck2 = Convert.ToByte("1");
                strCardName = SelectCard();
                if (strCardName != "")
                {
                    if (GetWritedData())
                    {
                        g_Sec = Convert.ToByte("1");
                        Blck2 = Convert.ToByte("1");
                        string[] strFullName = txtPartyName.Text.Split(' ');
                        if (strFullName.Length > 1)
                        {
                            WriteValue = strFullName[0].Trim();
                            result = WriteBalance();
                        }
                    }
                }
                else
                    MessageBox.Show("Sorry ! Card not found, Please Put card on writer  !!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception) { }
            return result;
        }

        private bool GetWritedData()
        {
            g_Sec = Convert.ToByte("1");
            Blck2 = Convert.ToByte("1");           
            string strData = ReadBalance();
            if (strData != "" && strData != "0")
            {
                strData = strData.Replace("\0", "");
                DialogResult result = MessageBox.Show("This Card have already information of Party : " + strData.Trim() + ", Are you want to replace it ?? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }


        #region Write Value
        private int WriteBalance()
        {
            // write balance
            long sto = 0;
            byte vKeyType = 0x00;
            int PhysicalSector = 0;
            int ctr, tmpInt = 0;

            byte[] dout = new byte[16];
            char[] charArray1 = new char[16];

            #region Write for Sectors
            vKeyType = ACR120U.ACR120_LOGIN_KEYTYPE_A;
            PhysicalSector = Convert.ToInt16(g_Sec);
            tmpInt = Convert.ToInt16(Blck2);
            sto = 30;
            for (ctr = 0; ctr < 6; ctr++)
                g_pKey[ctr] = 0xFF;
            g_retCode = ACR120U.ACR120_Login(g_rHandle, Convert.ToByte(PhysicalSector), Convert.ToInt16(vKeyType),
                                         Convert.ToByte(sto), ref g_pKey[0]);
            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));

            tmpInt = tmpInt + Convert.ToInt16(g_Sec) * 4;
            Blck2 = Convert.ToByte(tmpInt);

            charArray1 = WriteValue.ToString().ToCharArray();

            for (ctr = 0; ctr < 16; ctr++)
            {
                if (ctr < charArray1.Length)
                    dout[ctr] = Convert.ToByte(charArray1[ctr]);
                else
                    ctr = 16;
            }

            g_retCode = ACR120U.ACR120_Write(g_rHandle, Blck2, ref dout[0]);

            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
            #endregion
            return g_retCode;
        }
        #endregion

        #region Read Value
        private string ReadBalance()
        {
            byte[] PassRead = new byte[16];
            long sto = 0;
            byte vKeyType = 0x00;
            int PhysicalSector = 0;
            int ctr, tmpInt2 = 2;

            vKeyType = ACR120U.ACR120_LOGIN_KEYTYPE_A;
            PhysicalSector = Convert.ToInt16(g_Sec);
            tmpInt2 = Convert.ToInt16(Blck2);
            sto = 30;
            for (ctr = 0; ctr < 6; ctr++)
                g_pKey[ctr] = 0xFF;
            g_retCode = ACR120U.ACR120_Login(g_rHandle, Convert.ToByte(PhysicalSector), Convert.ToInt16(vKeyType),
                                             Convert.ToByte(sto), ref g_pKey[0]);
            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
            tmpInt2 = tmpInt2 + Convert.ToInt16(g_Sec) * 4;

            Blck2 = Convert.ToByte(tmpInt2);

            g_retCode = ACR120U.ACR120_Read(g_rHandle, Blck2, ref PassRead[0]);
            if (g_retCode < 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
            else
            {
                dstrpass1 = "";
                for (ctr = 0; ctr < 16; ctr++)
                {
                    dstrpass1 = dstrpass1 + char.ToString((char)(PassRead[ctr]));
                }
                CardBalance = Convert.ToString(dstrpass1);
                if (CardBalance == "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
                    CardBalance = "0";
            }
            return CardBalance;
        }
        #endregion

        #region For Connect
        private void Connect()
        {
            int ctr = 0;
            byte[] FirmwareVer = new byte[31];
            byte[] FirmwareVer1 = new byte[20];
            byte infolen = 0x00;
            string FirmStr;
            ACR120U.tReaderStatus ReaderStat = new ACR120U.tReaderStatus();

            if (g_isConnected)
            {
             //   MessageBox.Show("Device is already connected.");
                return;
            }

            g_rHandle = ACR120U.ACR120_Open(0);
            if (g_rHandle != 0)
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_rHandle));
            else
            {
                g_isConnected = true;
                //Get the DLL version the program is using
                g_retCode = ACR120U.ACR120_RequestDLLVersion(ref infolen, ref FirmwareVer[0]);
                if (g_retCode < 0)
                    MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen) - 1; ctr++)
                        FirmStr = FirmStr + char.ToString((char)(FirmwareVer[ctr]));
                }

                g_retCode = ACR120U.ACR120_Status(g_rHandle, ref FirmwareVer1[0], ref ReaderStat);
                if (g_retCode < 0)
                    MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
                else
                {
                    FirmStr = "";
                    for (ctr = 0; ctr < Convert.ToInt16(infolen); ctr++)
                        if ((FirmwareVer1[ctr] != 0x00) && (FirmwareVer1[ctr] != 0xFF))
                            FirmStr = FirmStr + char.ToString((char)(FirmwareVer1[ctr]));
                }

            }
        }
        #endregion             

        #region Select
        private string SelectCard()
        {
            //Variable Declarations
            byte[] ResultSN = new byte[11];
            byte ResultTag = 0x00;
            byte[] TagType = new byte[51];
            int ctr = 0;
            string SN = "";

            g_retCode = ACR120U.ACR120_Select(g_rHandle, ref TagType[0], ref ResultTag, ref ResultSN[0]);
            if (g_retCode < 0)

                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));

            else
            {
                if ((TagType[0] == 4) || (TagType[0] == 5))
                {

                    SN = "";
                    for (ctr = 0; ctr < 7; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }

                }
                else
                {

                    SN = "";
                    for (ctr = 0; ctr < ResultTag; ctr++)
                    {
                        SN = SN + string.Format("{0:X2} ", ResultSN[ctr]);
                    }

                }               
            }
            return SN.Trim();
        }
        #endregion

        #region DataTableRows
        public void GridTable()
        {
            dtmiss.Columns.Add("SlNo");
            dtmiss.Columns.Add("UID");
            dtmiss.Columns.Add("Value");
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
