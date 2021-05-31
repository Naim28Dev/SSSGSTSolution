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
    public partial class ReportSetting : Form
    {
        AccountMaster account;
        string category = "";
        DataBaseAccess dba;
        public int UpdateCounter = 0;

        public ReportSetting(string cat)
        {
            try
            {
                InitializeComponent();
                account = new AccountMaster();
                dba = new DataBaseAccess();
                category = cat;
                if (cat == "Sales")
                {
                    BindSalesDataWithGrid();
                }
                else if (cat == "Purchase")
                {
                    BindPurchaseDataWithGrid();
                }
                else if (cat == "Order")
                {
                    BindOrderDataWithGrid();
                }
                else if (cat == "OrderColumn")
                {
                    BindOrderColumnDataWithGrid();
                }
            }
            catch
            {
            }
        }

        public void BindSalesDataWithGrid()
        {
            try
            {
                DataTable dt = account.GetColumnSetting();
                BindDataWithGrid(dt);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bind DataGridview With Sale BillNo in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
      
        public void BindPurchaseDataWithGrid()
        {
            try
            {
                DataTable dt = account.GetPurchaseColumnSetting();
                BindDataWithGrid(dt);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bind DataGridview With Purchase BillNo in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public void BindOrderDataWithGrid()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from OrderFormatSetting Order by Place asc");
                BindDataWithGrid(dt);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bind DataGridview With Order in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        public void BindOrderColumnDataWithGrid()
        {
            try
            {
                DataTable dt = dba.GetDataTable("Select * from OrderColumnSetting Order by Place asc");
                BindDataWithGrid(dt);
            }
            catch
            {
            }
        }

        private void BindDataWithGrid(DataTable dt)
        {
            try
            {
                dgrdReport.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdReport.Rows.Add(dt.Rows.Count);

                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        DataRow dr = dt.Rows[i];
                        dgrdReport.Rows[i].Cells[0].Value = dr[0];
                        dgrdReport.Rows[i].Cells[1].Value = dr[1];
                        dgrdReport.Rows[i].Cells[2].Value = dr[2];
                        dgrdReport.Rows[i].Cells[3].Value = dr[3];
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Bind DataGridview With Order in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
      
        private void UpdateSalesRecord()
        {
            try
            {
                string[] record = new string[4];
                foreach (DataGridViewRow dr in dgrdReport.Rows)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        record[i] = "";
                        try
                        {
                            record[i] = dr.Cells[i].Value.ToString();
                        }
                        catch
                        {
                        }
                    }
                    int count = account.UpdateReportSetting(record);
                    if (count < 1)
                    {
                        MessageBox.Show("Sorry ! Record Not Updated");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Update Sale Record in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            
        }

        private void UpdatePurchaseRecord()
        {
            try
            {
                string[] record = new string[4];
                string strQuery = "";
                foreach (DataGridViewRow dr in dgrdReport.Rows)
                {
                    //for (int i = 0; i < 4; ++i)
                    //{
                    //    record[i] = "";
                    //    try
                    //    {
                    //        record[i] = dr.Cells[i].Value.ToString();
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}
                    strQuery += " Update PurchaseFormatSetting set Place='" + dr.Cells[3].EditedFormattedValue + "' where ColumnNo='" + dr.Cells[1].Value + "' ";
                }


                int count = dba.ExecuteMyQuery(strQuery);
                if (count < 1)
                {
                    MessageBox.Show("Sorry ! Record Not Updated");
                    return;
                }
                
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Update Purchase Record in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }

        private void UpdateOrderRecord()
        {
            try
            {
                string[] record = new string[4];
                foreach (DataGridViewRow dr in dgrdReport.Rows)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        record[i] = "";
                        try
                        {
                            record[i] = dr.Cells[i].Value.ToString();
                        }
                        catch
                        {
                        }
                    }
                    int count = account.UpdateOrderReportSetting(record);
                    if (count < 1)
                    {
                        MessageBox.Show("Sorry ! Record Not Updated");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Update Order Record in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }


        private void UpdateOrderColumnRecord()
        {
            try
            {
                string[] record = new string[4];
                foreach (DataGridViewRow dr in dgrdReport.Rows)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        record[i] = "";
                        try
                        {
                            record[i] = dr.Cells[i].Value.ToString();
                        }
                        catch
                        {
                        }
                    }
                    int count = account.UpdateOrderColumnReportSetting(record);
                    if (count < 1)
                    {
                        MessageBox.Show("Sorry ! Record Not Updated");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Update Order Column Record in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("Are you want to save Updated record.. ?", "Confirmation", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    UpdateCounter ++;
                    if (category == "Sales")
                    {
                        UpdateSalesRecord();
                    }
                    else if (category == "Purchase")
                    {
                        UpdatePurchaseRecord();
                    }
                    else if (category == "Order")
                    {
                        UpdateOrderRecord();
                    }
                    else if (category == "OrderColumn")
                    {
                        UpdateOrderColumnRecord();
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Click Event of Close Button in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }

        }

        private void ReportSetting_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 27)
                {
                    DialogResult dr = MessageBox.Show("Are you want to save Updated record.. ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        UpdateCounter++;
                        if (category == "Sales")
                        {
                            UpdateSalesRecord();
                        }
                        else if (category == "Purchase")
                        {
                            UpdatePurchaseRecord();
                        }
                        else if (category == "Order")
                        {
                            UpdateOrderRecord();
                        }
                        else if (category == "OrderColumn")
                        {
                            UpdateOrderColumnRecord();
                        }
                    }
                    this.Close();
                }
                //else if (e.KeyCode == Keys.Enter)
                //{
                //    this.GetNextControl(ActiveControl, true).Focus();
                //}
            }
            catch (Exception ex)
            {
                string[] strReport = { "Error occurred in Key Up Event of Form in Report Setting", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void dgrdReport_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                TextBox txtBox = e.Control as TextBox;
                txtBox.CharacterCasing = CharacterCasing.Upper;
              
                if (dgrdReport.CurrentCell.ColumnIndex == 3)
                {
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);                    
                }
                else
                {
                    txtBox.KeyPress -= new KeyPressEventHandler(txtBox_KeyPress);                  
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
