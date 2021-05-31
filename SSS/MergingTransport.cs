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
    public partial class MergingTransport : Form
    {
        DataBaseAccess dba;
        public MergingTransport()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void MergingTransport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtFTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH FIRST TRANSPORT", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtFTransport.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH SECOND TRANSPORT", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtSTransport.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFinalTransport_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH FINAL TRANSPORT", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtFinalTransport.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH FIRST STATION", e.KeyCode);
                    objSearch.ShowDialog();
                    if(objSearch.strSelectedData!="")
                    txtFStation.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtSStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH SECOND STATION", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtSStation.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtFinalStation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATIONNAME", "SEARCH FINAL STATION", e.KeyCode);
                    objSearch.ShowDialog();
                    if (objSearch.strSelectedData != "")
                        txtFinalStation.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private bool ValidateTransportName()
        {
            if (txtFTransport.Text == "")
            {
                MessageBox.Show("Sorry ! First Transport is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFTransport.Focus();
                return false;
            }
            if (txtSTransport.Text == "")
            {
                MessageBox.Show("Sorry ! Second Transport is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSTransport.Focus();
                return false;
            }
            if (txtFinalTransport.Text == "")
            {
                MessageBox.Show("Sorry ! Final Transport is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalTransport.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateStationName()
        {
            if (txtFStation.Text == "")
            {
                MessageBox.Show("Sorry ! First Station is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFStation.Focus();
                return false;
            }
            if (txtSStation.Text == "")
            {
                MessageBox.Show("Sorry ! Second Station is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSStation.Focus();
                return false;
            }
            if (txtFinalStation.Text == "")
            {
                MessageBox.Show("Sorry ! Final Station is required ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFinalStation.Focus();
                return false;
            }

            return true;
        }

        private void btnTMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnTMerge.Text = "Please wait ..";
                btnTMerge.Enabled = false;
                if (ValidateTransportName())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these transport in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeTransportName();
                }
            }
            catch
            {
            }
            btnTMerge.Enabled = true;
            btnTMerge.Text = "&Merge Transport";
        }

        private void btnSMerge_Click(object sender, EventArgs e)
        {
            try
            {
                btnSMerge.Text = "Please wait ..";
                btnSMerge.Enabled = false;
                if (ValidateStationName())
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to merge these station in single one ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Yes)
                        MergeStationName();
                }
            }
            catch
            {
            }
            btnSMerge.Enabled = true;
            btnSMerge.Text = "&Merge Station";
        }

        private void MergeTransportName()
        {
            string strQuery = "", strOldTransport = "";
            strOldTransport = "'" + txtFTransport.Text + "','" + txtSTransport.Text + "' ";

            //strQuery += " Update BiltyDetail set TransportName ='" + txtFinalTransport.Text + "',UpdateStatus=1 where TransportName in (" + strOldTransport + ") "
            //              + " Update ForwardingRecord set Transport='" + txtFinalTransport.Text + "',UpdateStatus=1 where Transport in (" + strOldTransport + ") "
            //              + " Update OrderBooking set Transport='" + txtFinalTransport.Text + "',UpdateStatus=1 where Transport in (" + strOldTransport + ") "
            //              + " Update SalesRecord set Transport='" + txtFinalTransport.Text + "',UpdateStatus=1 where Transport in (" + strOldTransport + ") "
            //              + " Update SupplierMaster set Transport='" + txtFinalTransport.Text + "',UpdateStatus=1 where Transport in (" + strOldTransport + ") "
            //              + " Delete from Transport where TransportName in (" + strOldTransport + ") and TransportName !='" + txtFinalTransport.Text + "' ";

            //int count = dba.ExecuteMyQuery(strQuery);
            int count = dba.MergePartyName(txtFTransport.Text, txtSTransport.Text, txtFinalTransport.Text, "TRANSPORT", true);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Transport Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtFTransport.Text = txtSTransport.Text = txtFinalTransport.Text = "";
            }
            else
                MessageBox.Show("Sorry ! An Error occured in merging transport name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void MergeStationName()
        {
            string strQuery = "", strOldStation = "";
            strOldStation = "'" + txtFStation.Text + "','" + txtSStation.Text + "' ";

            //strQuery += " Update BiltyDetail set StationName ='" + txtFinalStation.Text + "',UpdateStatus=1 where StationName in (" + strOldStation + ") " 
            //              + " Update ForwardingRecord set Station='" + txtFinalStation.Text + "',UpdateStatus=1 where Station in (" + strOldStation + ") "
            //              + " Update OrderBooking set Station ='" + txtFinalStation.Text + "',UpdateStatus=1 where Station in (" + strOldStation + ") "
            //              + " Update OrderBooking set Booking ='" + txtFinalStation.Text + "',UpdateStatus=1 where Booking in (" + strOldStation + ") "
            //              + " Update SalesRecord set Station='" + txtFinalStation.Text + "',UpdateStatus=1 where Station in (" + strOldStation + ") "
            //              + " Update SupplierMaster set Station='" + txtFinalStation.Text + "',UpdateStatus=1 where Station in (" + strOldStation + ") "
            //              + " Update SupplierMaster set BookingStation='" + txtFinalStation.Text + "',UpdateStatus=1 where BookingStation in (" + strOldStation + ") "
            //              + " Delete from Station where StationName in (" + strOldStation + ") and StationName !='" + txtFinalStation + "' ";

            int count = dba.MergePartyName(txtFStation.Text, txtSStation.Text, txtFinalStation.Text, "STATION", true);          
               // int count = dba.ExecuteMyQuery(strQuery);
            if (count > 0)
            {
                MessageBox.Show("Thank You ! Station Mergered Successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                txtFStation.Text = txtSStation.Text = txtFinalStation.Text = "";
            }
            else
                MessageBox.Show("Sorry ! An Error occured in merging station name, please try after some time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
