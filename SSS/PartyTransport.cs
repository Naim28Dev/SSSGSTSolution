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
    public partial class PartyTransport : Form
    {
        DataBaseAccess dba;
       public  string strFirstTransport = "", strSecondTransport = "", strThirdTransport = "";

        public PartyTransport()
        {
            InitializeComponent();
        }

        private void PartyTransport_KeyDown(object sender, KeyEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            strFirstTransport = txtTransportI.Text;
            strSecondTransport = txtTransportII.Text;
          //  strThirdTransport = txtTransportIII.Text;
            this.Close();
        }    

        private void txtTransportI_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH FIRST TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransportI.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtTransportII_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH SECOND TRANSPORT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtTransportII.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        //private void txtTransportIII_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        char objChar = Convert.ToChar(e.KeyCode);
        //        int value = e.KeyValue;
        //        if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //        {
        //            SearchData objSearch = new SearchData("TRANSPORTNAME", "SEARCH THIRD TRANSPORT NAME", e.KeyCode);
        //            objSearch.ShowDialog();
        //            txtTransportIII.Text = objSearch.strSelectedData;
        //        }
        //        else
        //            e.Handled = true;
        //    }
        //    catch
        //    {
        //    }
        //}
    }
}
