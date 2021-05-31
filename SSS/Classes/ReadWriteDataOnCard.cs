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
    class ReadWriteDataOnCard
    {
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

        public ReadWriteDataOnCard()
        {
            cbPort.Items.Add("USB1");
            cbPort.SelectedIndex = 0;
            Connect();
        }
        
        public bool WriteDataOnCard(string strData)
        {
            bool wStatus = false, cStatus = true ;
            try
            {                
                g_Sec = Convert.ToByte(1);// Convert.ToByte(txtSector.Text);
                Blck2 = Convert.ToByte(1);//Convert.ToByte(txtBlock.Text)
                string strCardID= SelectCard();
                if (strCardID != "")
                {
                    string strSaveData = ReadBalance();
                    if (strSaveData != "")
                    {
                        DialogResult result = MessageBox.Show("This card have some other  party information , Are you want to replace it ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                            cStatus = true;
                        else
                            cStatus = false;
                    }
                    if (cStatus)
                    {
                      // string strEncript = Convert.ToBase64String(Encoding.Unicode.GetBytes(strData));

                        WriteValue = strData;
                        wStatus = WriteBalance();
                    }
                }
            }
            catch (Exception exp)
            { }
            return wStatus;
        }

        public string ReadDataFromCard(string strCode)
        {
            string strData = "";
            try
            {               
                g_Sec = Convert.ToByte(1);// Convert.ToByte(txtSector.Text);
                Blck2 = Convert.ToByte(1);//Convert.ToByte(txtBlock.Text)
                string strCardName= SelectCard();
                if (strCardName != "")
                {
                    strData = ReadBalance();
                    if (strData == "0")
                        strData = "";
                    if (strData != "" && strCode!="")
                        strData = DataBaseAccess.GetFullPartyName(strData.Trim(), strCode);
                }
                else
                    MessageBox.Show("Sorry ! Card not found !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception) { }
            return strData;
        }

        #region Select
        private string  SelectCard()
        {
            //Variable Declarations
            byte[] ResultSN = new byte[11];
            byte ResultTag = 0x00;
            byte[] TagType = new byte[51];
            int ctr = 0;
            string SN = "",strCardID="";

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
                        if (ResultSN.Length == (ctr + 1))
                            break;
                    }

                }
                if (strCardID == "")
                {
                    strCardID = SN.Trim();
                }
            }
            return strCardID;
        }
        #endregion

        #region Write Value
        private bool WriteBalance()
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
            {
                MessageBox.Show("[X] " + ACR120U.GetErrMsg(g_retCode));
                return false;
            }
            else
                return true;

            #endregion
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
                    CardBalance = "";
            }
            return CardBalance;
        }
        #endregion


        #region For Connect
        public void Connect()
        {
            cbPort.Items.Add("USB1");
            cbPort.SelectedIndex = 0;

            int ctr = 0;
            byte[] FirmwareVer = new byte[31];
            byte[] FirmwareVer1 = new byte[20];
            byte infolen = 0x00;
            string FirmStr;
            ACR120U.tReaderStatus ReaderStat = new ACR120U.tReaderStatus();

            if (g_isConnected)
            {
                //MessageBox.Show("Device is already connected.");
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

        #region DataTableRows
        public void GridTable()
        {
            dtmiss.Columns.Add("SlNo");
            dtmiss.Columns.Add("UID");
            dtmiss.Columns.Add("Value");
        }
        #endregion
    }
}
