using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class DesignRegister : Form
    {
        DataBaseAccess dba;
        DataTable dtReport = null;
        public DesignRegister()
        {
            dba = new DataBaseAccess();
            InitializeComponent();
            SetCategory();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                {
                    MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (chkSNo.Checked && (txtFromSNo.Text == "" || txtToSNo.Text == ""))
                {
                    MessageBox.Show(" Sorry ! Please fill Bill No or uncheck Bill No box ! ", "Bill No. Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    btnSearch.Enabled = false;
                    SearchQueryData();
                    btnSearch.Enabled = true;
                }
            }
            catch (Exception ex)
            { }
        }

        private void SearchQueryData()
        {
            string strQuery = "";
            try
            {
                strQuery = "select (BillCode+' '+cast(BillNo as varchar)) BillNum,* from (select I.*,ITS.PurchasePartyID,ITS.Variant1,ITS.Variant2,ITS.Variant3,ITS.Variant4,ITS.Variant5,ITS.PurchaseRate,ITS.Margin,ITS.SaleRate,ITS.Reorder,ITS.OpeningQty,ITS.OpeningRate,ITS.GodownName,ITS.Description from Items I left join ItemSecondary ITS on I.BillCode = ITS.BillCode and I.BillNo = ITS.BillNo)_Item where BillNo !=0"; 

                string strSubQuery = CreateQuery();
                if (strQuery != "")
                {
                    strQuery += strSubQuery;
                }
                strQuery += "  Order by BillNo,Date";
                dtReport = dba.GetDataTable(strQuery);
                BindGridViewData(dtReport);
                //CalculateTotal();
            }
            catch (Exception ex)
            { }
        }

        private void BindGridViewData(DataTable dtRecord)
        {
            try
            {
                dgrdDetails.Rows.Clear();
                if (dtRecord.Rows.Count > 0)
                {
                    int i = 0;
                    dgrdDetails.Rows.Add(dtRecord.Rows.Count);
                    foreach (DataRow row in dtRecord.Rows)
                    {
                        dgrdDetails.Rows[i].Cells["id"].Value = row["ID"];
                        dgrdDetails.Rows[i].Cells["billNo"].Value = row["BillNum"];
                        dgrdDetails.Rows[i].Cells["date"].Value = Convert.ToDateTime(row["Date"]).ToString("dd/MM/yyyy");
                        dgrdDetails.Rows[i].Cells["itemName"].Value = Convert.ToString(row["ItemName"]);
                        dgrdDetails.Rows[i].Cells["variant1"].Value = Convert.ToString(row["Variant1"]);
                        dgrdDetails.Rows[i].Cells["variant2"].Value = Convert.ToString(row["Variant2"]);
                        dgrdDetails.Rows[i].Cells["variant3"].Value = Convert.ToString(row["Variant3"]);
                        dgrdDetails.Rows[i].Cells["variant4"].Value = Convert.ToString(row["Variant4"]);
                        dgrdDetails.Rows[i].Cells["variant5"].Value = Convert.ToString(row["Variant5"]);
                        dgrdDetails.Rows[i].Cells["qtyRatio"].Value = Convert.ToString(row["QtyRatio"]);
                        dgrdDetails.Rows[i].Cells["purchaseRate"].Value = Convert.ToString(row["PurchaseRate"]);
                        dgrdDetails.Rows[i].Cells["SaleRate"].Value = Convert.ToString(row["SaleRate"]);
                        dgrdDetails.Rows[i].Cells["margin"].Value = Convert.ToString(row["Margin"]);
                        dgrdDetails.Rows[i].Cells["reOrder"].Value = Convert.ToString(row["Reorder"]);
                        dgrdDetails.Rows[i].Cells["openingQty"].Value = Convert.ToString(row["OpeningQty"]);
                        dgrdDetails.Rows[i].Cells["openingRate"].Value = Convert.ToString(row["OpeningRate"]);
                        dgrdDetails.Rows[i].Cells["godownName"].Value = Convert.ToString(row["GodownName"]);
                        dgrdDetails.Rows[i].Cells["BarCode"].Value = Convert.ToString(row["Description"]);
                        dgrdDetails.Rows[i].Cells["category"].Value = Convert.ToString(row["Other"]);
                        dgrdDetails.Rows[i].Cells["groupName"].Value = Convert.ToString(row["GroupName"]);
                        dgrdDetails.Rows[i].Cells["subGroupName"].Value = Convert.ToString(row["SubGroupname"]);
                        dgrdDetails.Rows[i].Cells["buyerDesign"].Value = Convert.ToString(row["BuyerDesignName"]);
                        dgrdDetails.Rows[i].Cells["brandName"].Value = Convert.ToString(row["BrandName"]);
                        dgrdDetails.Rows[i].Cells["makeName"].Value = Convert.ToString(row["MakeName"]);
                        dgrdDetails.Rows[i].Cells["createdBy"].Value = Convert.ToString(row["CreatedBy"]);
                        dgrdDetails.Rows[i].Cells["updatedBy"].Value = Convert.ToString(row["UpdatedBy"]);


                        i++;
                    }
                }
            }
            catch (Exception EX)
            { }
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["variant1"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant1"].Visible = false;

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["variant2"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant2"].Visible = false;

                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["variant3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["variant3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["variant4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["variant4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["variant5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["variant5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["variant5"].Visible = false;
            }
            catch
            {
            }
        }

        private string CreateQuery()
        {
            string strQuery = "";
            try
            {
                //searching qury using Date Wise..
                if (chkDate.Checked && txtFromDate.Text.Length > 9 && txtToDate.Text.Length > 9)
                {
                    DateTime strDate, endDate;
                    strDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    endDate = dba.ConvertDateInExactFormat(txtToDate.Text).AddDays(1);
                    strQuery = " and (Date >='" + strDate.ToString("MM/dd/yyyy h:mm:ss tt") + "' and Date<'" + endDate.ToString("MM/dd/yyyy h:mm:ss tt") + "')  ";

                }

                if (chkSNo.Checked && txtFromSNo.Text != "" && txtToSNo.Text != "")
                    strQuery = " and (BillNo >=" + txtFromSNo.Text + " and BillNo<=" + txtToSNo.Text + ")  ";
                
                if (txtBillCode.Text != "")
                {
                    strQuery += " and BillCode='" + txtBillCode.Text + "' ";
                }

                if (txtGroupName.Text != "")
                {
                    strQuery += " and GroupName='" + txtGroupName.Text + "' ";
                }

                if (txtSubGrpName.Text != "")
                {
                    strQuery += " and SubGroupName='" + txtSubGrpName.Text + "' ";
                }

                if (txtItem.Text != "")
                {
                    strQuery += " and ItemName='" + txtItem.Text + "' ";
                }

                if (txtCategory.Text != "")
                {
                    strQuery += " and Other='" + txtCategory.Text + "' ";
                }

                if (txtBrand.Text != "")
                {
                    strQuery += " and BrandName='" + txtBrand.Text + "' ";
                }

                if (txtDepartment.Text != "")
                {
                    strQuery += " and MakeName='" + txtDepartment.Text + "' ";
                }

            }
            catch { }

            return strQuery;
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                    char objChar = Convert.ToChar(e.KeyCode);
                    int value = e.KeyValue;
                    if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                    {
                        SearchData objSearch = new SearchData("ITEMGROUPNAME", "GROUP NAME", e.KeyCode);
                        objSearch.ShowDialog();
                        txtGroupName.Text = objSearch.strSelectedData;
                    }
                    else
                    {
                        e.Handled = true;
                    }
            }
            catch
            {
            }
        }

        private void txtSubGrpName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DESIGNTYPE", "SELECT DESIGN TYPE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSubGrpName.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SELECT ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItem.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtBrand_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDNAME", "SELECT BRAND NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBrand.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DEPARTMENTNAME", "SEARCH DEPARTMENT NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtDepartment.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void txtDesign_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMCATEGORYNAME", "SEARCH ITEM CATEGORY", e.KeyCode);
                    objSearch.ShowDialog();
                    txtCategory.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void DesignRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {

                    Microsoft.Office.Interop.Excel.Application ExcelApp = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook xlWorkbook = ExcelApp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    object misValue = System.Reflection.Missing.Value;
                    Microsoft.Office.Interop.Excel.Sheets xlSheets = null;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = null;

                    //Create Excel Sheets
                    xlSheets = ExcelApp.Sheets;
                    xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(xlSheets[1],
                                   Type.Missing, Type.Missing, Type.Missing);

                    int _skipColumn = 0;
                    string strHeader = "";
                    for (int j = 1; j < dgrdDetails.Columns.Count + 1; j++)
                    {
                        strHeader = dgrdDetails.Columns[j - 1].HeaderText;
                        if (strHeader == "" || !dgrdDetails.Columns[j - 1].Visible)
                        {
                            _skipColumn++;
                            j++;
                        }

                        ExcelApp.Cells[1, j - _skipColumn] = dgrdDetails.Columns[j - 1].HeaderText;
                        ExcelApp.Cells[1, j - _skipColumn].Font.Bold = true;

                    }
                    _skipColumn = 0;
                    // Storing Each row and column value to excel sheet
                    for (int k = 0; k < dgrdDetails.Rows.Count; k++)
                    {
                        for (int l = 0; l < dgrdDetails.Columns.Count; l++)
                        {
                            if (dgrdDetails.Columns[l].HeaderText == "" || !dgrdDetails.Columns[l].Visible)
                            {
                                _skipColumn++;
                                l++;
                            }
                            if (l < dgrdDetails.Columns.Count)
                                ExcelApp.Cells[k + 2, l - _skipColumn + 1] = dgrdDetails.Rows[k].Cells[l].Value.ToString();
                        }
                        _skipColumn = 0;
                    }
                    ExcelApp.Columns.AutoFit();


                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Design_Register";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);


                }
                else
                    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
            btnExport.Enabled = true;
        }

        private void chkSNo_CheckedChanged(object sender, EventArgs e)
        {
            txtFromSNo.ReadOnly = txtToSNo.ReadOnly = !chkSNo.Checked;
            txtFromSNo.Text = txtToSNo.Text = "";
        }

        private void txtFromSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtToSNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBillCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("DESIGNCODE", "SELECT BILL CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBillCode.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && (e.ColumnIndex == 1 || e.ColumnIndex == 3))
                    ShowDesignMaster(Convert.ToString(dgrdDetails.CurrentRow.Cells["billNo"].Value));
            }
            catch { }
        }

        private void ShowDesignMaster(string strBillNo)
        {
            string[] str = strBillNo.Split(' ');
            if(str.Length>1)
            {
                if (MainPage.bArticlewiseOpening)
                {
                    ItemMaster objItemMaster = new ItemMaster(str[0], str[1], false);
                    objItemMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objItemMaster.ShowInTaskbar = true;
                    objItemMaster.Show();
                }
                else
                {
                    DesignMaster objDesign = new SSS.DesignMaster(str[0], str[1], false);
                    objDesign.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objDesign.ShowInTaskbar = true;
                    objDesign.Show();
                }
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dgrdDetails.CurrentRow.Index >= 0 && (dgrdDetails.CurrentCell.ColumnIndex == 1 || dgrdDetails.CurrentCell.ColumnIndex == 3))
                        ShowDesignMaster(Convert.ToString(dgrdDetails.CurrentRow.Cells["billNo"].Value));
                }
            }
            catch { }
        }

        private void DesignRegister_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
        }
    }
}
