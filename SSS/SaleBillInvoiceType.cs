using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class SaleBillInvoiceType : Form
    {
        DataBaseAccess dba;
        public int _originalCopy = 0, _supplierCopy = 0, _transportCopy = 0;
        public bool _oLetterHead = false, _sLetterHead = false, _tLetterHead = false;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaleBillInvoiceType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
                else if (e.KeyCode == Keys.Enter)
                    SendKeys.Send("{TAB}");
            }
            catch { }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                _originalCopy = dba.ConvertObjectToInt(txtOriginal.Text);
                _supplierCopy = dba.ConvertObjectToInt(txtSupplier.Text);
                _transportCopy = dba.ConvertObjectToInt(txtTransport.Text);
                _oLetterHead = chkOLetterHead.Checked;
                _sLetterHead = chkSLetterHead.Checked;
                _tLetterHead = chkTLetterHead.Checked;
                this.Close();
            }
            catch { }
        }

        private void txtSupplier_Enter(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if(txt!=null)
                {
                    if (txt.Text == "0")
                        txt.Clear();
                }
            }
            catch { }
        }

        private void txtSupplier_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                if (txt != null)
                {
                    if (txt.Text == "")
                        txt.Text = "0";
                }
            }
            catch { }
        }

        public SaleBillInvoiceType()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void txtSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }


    }
}
