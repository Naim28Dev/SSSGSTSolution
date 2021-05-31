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
    public partial class LastIntDiscountDetails : Form
    {
        DataBaseAccess dba;
        public LastIntDiscountDetails(DataTable _dt)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();
                int pageWidth = MainPage.mymainObject.Width, pageHeight = MainPage.mymainObject.Height;
                if (pageWidth == 0)
                    pageWidth = 1000;
                this.Location = new Point(pageWidth - 455, 30);

                BindDataWithTable(_dt);
            }
            catch { }
        }

        private void EditTrailDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void BindDataWithTable(DataTable dt)
        {
            try
            {
                dgrdDetails.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    int _rowIndex = 0;
                 
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[_rowIndex].Cells["date"].Value = row["_Date"];
                        dgrdDetails.Rows[_rowIndex].Cells["voucherNo"].Value = row["VoucherNo"];
                        dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = row["NetAmt"];
                        dgrdDetails.Rows[_rowIndex].Cells["status"].Value = row["Status"];
                        dgrdDetails.Rows[_rowIndex].Cells["billType"].Value = row["BillType"];
                        _rowIndex++;
                    }
                }
                else
                    this.Close();
            }
            catch { }
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1)
                    ShowDetails();
            }
            catch { }
        }

        private void ShowDetails()
        {

            string strAccount = Convert.ToString(dgrdDetails.CurrentRow.Cells["billType"].Value).ToUpper(), strVoucherNo = Convert.ToString(dgrdDetails.CurrentRow.Cells["voucherNo"].Value).ToUpper();

            if (strAccount != "" && strVoucherNo != "")
            {
                string[] strVoucher = strVoucherNo.Trim().Split(' ');
                if (strVoucher.Length > 0)
                {
                    if (strAccount == "SALESERVICE")
                    {
                        if (strVoucher.Length > 1)
                        {
                            SaleServiceBook objSale = new SaleServiceBook(strVoucher[0].Trim(), strVoucher[1].Trim());
                            objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                            objSale.ShowInTaskbar = true;
                            objSale.Show();
                        }
                    }
                    else
                    {
                        if (strVoucher.Length > 1)
                        {
                            JournalEntry_New objJournal = new JournalEntry_New(strVoucher[0].Trim(), strVoucher[1].Trim());
                            objJournal.FormBorderStyle = FormBorderStyle.FixedSingle;
                            objJournal.ShowInTaskbar = true;
                            objJournal.Show();
                        }
                    }
                }
            }
        }
    }
}
