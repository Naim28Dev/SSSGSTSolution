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
    public partial class StockAudit : Form
    {
        DataBaseAccess dba;
        DataTable BindedDT;
        SearchCategory_Custom objSearch;
        public StockAudit(string Mode = "FIRSTUSE")
        {
            InitializeComponent();
            dba = new SSS.DataBaseAccess();
            GetDataFromDB();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            try
            {
                if (MainPage.mymainObject.bShowAllRecord)
                {
                    if (chkDate.Checked && (txtFromDate.Text.Length != 10 || txtToDate.Text.Length != 10))
                        MessageBox.Show(" Sorry ! Please fill Date or uncheck Date box ! ", "Date Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        GetDataFromDB();
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have proper Authorizaton to view.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch { }
            btnGo.Enabled = true;
        }

        private string CreateQuery()
        {
            string strQuery = "", strWhere = "";
            DateTime fDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
            DateTime tDate = dba.ConvertDateInExactFormat(txtToDate.Text);

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strWhere += " AND (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (txtItemName.Text != "")
            {
                strWhere += " AND ItemName = '" + txtItemName.Text + "'";
            }
            if (txtBrandName.Text != "")
            {
                strWhere += " AND BrandName = '" + txtBrandName.Text + "'";
            }

            if (MainPage.StrCategory1 != "")
                strQuery += ",Variant1 ";
            if (MainPage.StrCategory2 != "")
                strQuery += ",Variant2 ";
            if (MainPage.StrCategory3 != "")
                strQuery += ",Variant3 ";
            if (MainPage.StrCategory4 != "")
                strQuery += ",Variant4 ";
            if (MainPage.StrCategory5 != "")
                strQuery += ",Variant5 ";

            if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                strWhere += " AND CreatedBy = '" + MainPage.strLoginName + "'";

            strQuery = "SELECT SNo=Cast(Row_Number() over(Order By Date Asc) as Int),ID,BrandName,ItemName,BarCode" + strQuery + ",Qty,Convert(varchar(12),Date,103)Date ,'"
                        + MainPage.strCompanyName + "' CompanyName,'Audit Stock Report' HeaderText,'" + MainPage.StrCategory1 + "' LblVariant1,'"
                        + MainPage.StrCategory2 + "' LblVariant2, '" + MainPage.StrCategory3 + "' LblVariant3,'"
                        + MainPage.StrCategory4 + "' LblVariant4, '" + MainPage.StrCategory5 + "' LblVariant5,CreatedBy  FROM AuditStock WHERE 1=1" + strWhere;
            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();

                if (strQuery != "")
                {
                    DataTable DT = DataBaseAccess.GetDataTableRecord(strQuery);
                    BindDataWithGrid(DT);
                    dgrdDetails.ReadOnly = false;
                }
            }
            catch
            { }
        }
        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                dgrdDetails.DataSource = null;
                if (table != null && table.Rows.Count > 0)
                {
                    DataView dataView = new DataView(table);
                    dgrdDetails.DataSource = dataView;
                    SetColumnStyle();
                    GetSum();
                    BindedDT = table.Clone();
                    BindedDT = table;
                }
                else if (txtItemName.Text == "" && txtBrandName.Text == "" && !chkDate.Checked)
                    AddNewRow();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Bind Data To Grid.", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        private void GetSum()
        {
            try
            {
                double total = 0;
                total = dgrdDetails.Rows.Cast<DataGridViewRow>()
                 .Sum(t => dba.ConvertObjectToDouble(t.Cells["Qty"].Value));

                lblTotalQty.Text = total.ToString("N2", MainPage.indianCurancy);
            }
            catch { }
        }
        private void SetColumnStyle()
        {
            for (int i = 0; i < dgrdDetails.Columns.Count; i++)
            {
                try
                {
                    DataGridViewCellStyle cellStyle = dgrdDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdDetails.Columns[i];

                    string strAlign = "LEFT", clmname = _column.Name.ToUpper();
                    int _width = 100;
                    _column.Width = _width;

                    if (clmname == "SNO")
                    {
                        strAlign = "MIDDLE";
                        _width = 50;
                    }
                    if (clmname == "DATE" || clmname.Contains("LBLVARIANT") || clmname.Contains("COMPANYNAME") || clmname.Contains("HEADERTEXT") || clmname == "ID")
                        _column.Visible = false;
                    if (clmname.Contains("VARIANT"))
                        _width = 120;
                    if (clmname == "BRANDNAME")
                        _width = 200;
                    if (clmname == "ITEMNAME")
                        _width = 285;
                    if (clmname == "BARCODE")
                        _width = 120;
                    if (clmname == "QTY")
                    {
                        _width = 80;
                        strAlign = "RIGHT";
                        cellStyle.Format = "N2";
                    }
                    if (clmname == "VARIANT1")
                        _column.HeaderText = MainPage.StrCategory1;
                    if (clmname == "VARIANT2")
                        _column.HeaderText = MainPage.StrCategory2;
                    if (clmname == "VARIANT3")
                        _column.HeaderText = MainPage.StrCategory3;
                    if (clmname == "VARIANT4")
                        _column.HeaderText = MainPage.StrCategory4;
                    if (clmname == "VARIANT5")
                        _column.HeaderText = MainPage.StrCategory5;

                    cellStyle.Font = new Font("Arial", 9F, System.Drawing.FontStyle.Regular);

                    if (strAlign == "LEFT")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (strAlign == "MIDDLE")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgrdDetails.Columns[i].DefaultCellStyle = cellStyle;
                    dgrdDetails.Columns[i].HeaderText = (dgrdDetails.Columns[i].HeaderText).Replace("_", " ");
                    dgrdDetails.Columns[i].HeaderCell.Style.Font = new Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
                    dgrdDetails.Columns[i].Width = _width;

                }
                catch { }
            }
        }

        private void SaveUpdate(double Qty, string strBarcode, string strItem, string strBrand, string strVariant1, string strVariant2, string strVariant3, string strVariant4, string strVariant5, bool bEdit)
        {
            try
            {
                string strWhere = "";
                if (MainPage.StrCategory1 != "")
                    strWhere += " AND Variant1 = '" + strVariant1 + "'";
                if (MainPage.StrCategory1 != "")
                    strWhere += " AND Variant2 = '" + strVariant2 + "'";

                string strQuery = " BEGIN TRY IF exists (SELECT * FROM AuditStock where BarCode='" + strBarcode + "' AND ItemName = '" + strItem + "' AND BrandName ='" + strBrand + "'" + strWhere + ") BEGIN"
                                + " UPDATE AuditStock SET Qty = " + (bEdit ? "" : "Qty+") + Qty + ",UpdatedBy='" + MainPage.strLoginName + "',UpdateStatus=1,InsertStatus=0,Date=GetDate() where BarCode='" + strBarcode + "' AND ItemName = '" + strItem + "' AND BrandName ='" + strBrand + "'" + strWhere
                                + " END ELSE BEGIN"
                                + " INSERT INTO AuditStock (RemoteID, BarCode, BrandName, ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, Qty, Date, CreatedBy, UpdatedBy, InsertStatus, UpdateStatus) "
                                + " SELECT 0,'" + strBarcode + "','" + strBrand + "','" + strItem + "','" + strVariant1 + "','" + strVariant2 + "','" + strVariant3 + "','" + strVariant4 + "','" + strVariant5 + "'," + Qty + ",GetDate(),'" + MainPage.strLoginName + "','',1,0 "
                                + " END "
                                + CreateQuery()
                                + " END TRY BEGIN Catch SELECT -100 as SNo END catch";

                DataTable DT = DataBaseAccess.GetDataTableRecord(strQuery);
                if (DT.Rows.Count > 0)
                {
                    if (dba.ConvertObjectToDouble(DT.Rows[0]["SNo"]) > 0)
                    {
                        BindDataWithGrid(DT);
                        dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Yellow;
                        dgrdDetails.FirstDisplayedCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells[2];
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Record not updated please retry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    dgrdDetails.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgrdDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Qty")
                {
                    DataGridViewRow dr = dgrdDetails.CurrentRow;
                    string strBarcode = "", strItem = "", strBrand = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "";
                    double Qty = 0;
                    strBarcode = Convert.ToString(dr.Cells["Barcode"].Value);
                    if (strBarcode != "")
                    {
                        strBarcode = Convert.ToString(dr.Cells["Barcode"].Value);
                        strBrand = Convert.ToString(dr.Cells["BrandName"].Value);
                        strItem = Convert.ToString(dr.Cells["ItemName"].Value);
                        if (MainPage.StrCategory1 != "")
                            strVariant1 = Convert.ToString(dr.Cells["Variant1"].Value);
                        if (MainPage.StrCategory2 != "")
                            strVariant1 = Convert.ToString(dr.Cells["Variant2"].Value);
                        if (MainPage.StrCategory3 != "")
                            strVariant1 = Convert.ToString(dr.Cells["Variant3"].Value);
                        if (MainPage.StrCategory4 != "")
                            strVariant1 = Convert.ToString(dr.Cells["Variant4"].Value);
                        if (MainPage.StrCategory5 != "")
                            strVariant1 = Convert.ToString(dr.Cells["Variant5"].Value);
                        Qty = dba.ConvertObjectToDouble(dr.Cells["Qty"].Value);

                        SaveUpdate(Qty, strBarcode, strItem, strBrand, strVariant1, strVariant2, strVariant3, strVariant4, strVariant5, true);
                    }
                }
            }
            catch (Exception ex) { }
        }
        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex > 0 && dgrdDetails.Columns[e.ColumnIndex].HeaderText != "Qty")
                {
                    string strType = "DESIGNNAMEWITHBARCODE_AUDITSTOCK", strFrom = "ItemName";
                    if (e.ColumnIndex == 1)
                        strFrom = "BarCode";
                    objSearch = new SearchCategory_Custom("", strType, "", "", "", "", "", "", "", Keys.Space, false, false, strFrom);
                    objSearch.ShowDialog();
                    string strData = objSearch.strSelectedData;
                    if (strData != "")
                    {
                        //DialogResult result = MessageBox.Show("Are you sure to Save or Update the record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (result == DialogResult.Yes)
                        {
                            string[] strAllItem = strData.Split('|');
                            if (strAllItem.Length > 0)
                            {
                                string strBarcode = strAllItem[0].Trim();
                                string strItem = "", strBrand = "", strVariant1 = "", strVariant2 = "", strVariant3 = "", strVariant4 = "", strVariant5 = "";
                                double Qty = 1;
                                string[] str = strBarcode.Split('.');
                                if (str[0] != "")
                                {
                                    strBarcode = str[0];
                                    strBrand = strAllItem[1];
                                    strItem = strAllItem[2];
                                    if (strAllItem.Length > 5)
                                        strVariant1 = strAllItem[3];
                                    if (strAllItem.Length > 6)
                                        strVariant2 = strAllItem[4];
                                    if (strAllItem.Length > 7)
                                        strVariant3 = strAllItem[5];
                                    if (strAllItem.Length > 8)
                                        strVariant4 = strAllItem[6];
                                    if (strAllItem.Length > 9)
                                        strVariant5 = strAllItem[7];

                                    SaveUpdate(Qty, strBarcode, strItem, strBrand, strVariant1, strVariant2, strVariant3, strVariant4, strVariant5, false);
                                    AddNewRow();
                                }
                            }
                            e.Cancel = true;
                        }
                        e.Cancel = true;
                    }
                    e.Cancel = true;
                }
                else if (dgrdDetails.Columns[e.ColumnIndex].HeaderText == "Qty" && Convert.ToString(dgrdDetails.CurrentRow.Cells["Barcode"].Value) != "")
                {
                    if (Convert.ToString(dgrdDetails.CurrentCell.Value) == "")
                        dgrdDetails.CurrentCell.Value = "0";
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddNewRow()
        {
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                {
                    if (Convert.ToString(dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["BarCode"].Value) != "")
                    {
                        DataView dataView = (DataView)dgrdDetails.DataSource;
                        DataRowView NewVRow = dataView.AddNew();
                        NewVRow[0] = dgrdDetails.Rows.Count;
                        dgrdDetails.DataSource = dataView;
                        dgrdDetails.ReadOnly = false;
                        dgrdDetails.Focus();
                        dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["BarCode"];
                    }
                }
                else
                {
                    string qry = "SELECT SNo=1,'' BrandName,'' ItemName,'' BarCode,'' Variant1,'' Variant2,'' Qty,'' Date,'" + MainPage.strCompanyName + "' CompanyName, 'Audit Stock Report' HeaderText, '" + MainPage.StrCategory1 + "' LblVariant1, '" + MainPage.StrCategory2 + "' LblVariant2, '" + MainPage.StrCategory3 + "' LblVariant3, '" + MainPage.StrCategory4 + "' LblVariant4, '" + MainPage.StrCategory5 + "' LblVariant5";
                    DataTable DT = dba.GetDataTable(qry);
                    BindDataWithGrid(DT);
                    dgrdDetails.ReadOnly = false;
                    dgrdDetails.Focus();
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[0].Cells["BarCode"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                PrintOrPreview(true);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Audit Stock Report ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                PrintOrPreview(false);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview  in Audit Stock Report ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }
        private string CreateUploadQuery()
        {
            string strQuery = "", strWhere = "";
            DateTime fDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
            DateTime tDate = dba.ConvertDateInExactFormat(txtToDate.Text);

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strWhere += " AND (AU.date >= '" + sDate.ToString("MM/dd/yyyy") + "' and AU.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (txtItemName.Text != "")
                strWhere += " AND AU.ItemName = '" + txtItemName.Text + "'";
            if (txtBrandName.Text != "")
                strWhere += " AND AU.BrandName = '" + txtBrandName.Text + "'";

            strQuery = " Update ItemSecondary Set OpeningQty=0 Where OpeningQty!=0 Update StockMaster Set Qty=0 Where Qty!=0 and BillType='OPENING' and ISNULL(Other1,'')!='FORWARDED' "
                     + " Update IMS set OpeningQty = AU.Qty, UpdateStatus = 1, UpdatedBy = '" + MainPage.strLoginName + "' FROM ItemSecondary IMS LEFT JOIN AuditStock AU on IMS.Description = AU.BarCode AND Ims.Variant1 = AU.Variant1 AND Ims.Variant2 = AU.Variant2 WHERE AU.Qty > 0" + strWhere;

            strQuery += " INSERT INTO[dbo].[ItemSecondary] ([RemoteID],[BillCode],[BillNo],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[OpeningQty],[ActiveStatus] ,[Description],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[PurchaseRate],[Margin],[SaleRate],[Reorder],[SaleMRP],[OpeningRate])"
                        + " SELECT 0,IM.BillCode,IM.BillNo,Variant1 ,Variant2,Variant3,Variant4,Variant5,Qty,1,BarCode,'" + MainPage.strLoginName + "','',1,0,0,0,0,0,0,0 FROM AuditStock AU LEFT JOIN Items IM ON AU.ItemName = IM.ItemName Where(SELECT COUNT(*) FROM ItemSecondary IMS WHERE IMS.Description = AU.BarCode AND Ims.Variant1 = AU.Variant1 AND Ims.Variant2 = AU.Variant2) = 0" + strWhere;

            strQuery += " Update SM SET Qty = AU.Qty, Rate = (CASE WHEN IMS.OpeningRate > 0 then IMS.OpeningRate ELSE IMS.PurchaseRate end) From StockMaster SM inner JOIN AuditStock AU ON SM.BarCode = AU.BarCode LEFT join ItemSecondary IMS ON SM.BarCode = IMS.Description WHERE SM.Billtype='OPENING' and AU.Qty != 0" + strWhere;

            strQuery += " INSERT INTO[dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName])"
                        + " SELECT 'OPENING',IMS.BillCode,IMS.BillNo,AU.ItemName,AU.Variant1,AU.Variant2,AU.Variant3,AU.Variant4,AU.Variant5,AU.Qty,(CASE WHEN IMS.OpeningRate > 0 then IMS.OpeningRate ELSE IMS.PurchaseRate end),'','','',1,0,0,GetDate(),AU.BarCode,Au.BrandName,IMS.DesignName FROM AuditStock AU LEFT join ItemSecondary IMS ON AU.BarCode = IMS.Description AND IMS.Variant1 = AU.Variant1 AND IMS.Variant2 = AU.Variant2 WHERE (SELECT COUNT(*) FROM StockMaster SM2 WHERE SM2.BilLType='OPENING' and SM2.BarCode = AU.BarCode) = 0" + strWhere;

            return strQuery;
        }
        private void btnUpload_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to upload to stock as opening ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string strQuery = CreateUploadQuery();
                    if (strQuery != "")
                    {
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thanks you!  Stock uploaded successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnUpload.Visible = false;
                        }
                        else
                            MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch { }
            btnUpload.Enabled = true;
        }
        private void PrintOrPreview(bool _print)
        {
            if (_print)
            {
                Reporting.AuditStockReport objRpt = new Reporting.AuditStockReport();
                objRpt.SetDataSource(BindedDT);
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objRpt);
                else
                {
                    objRpt.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                    objRpt.PrintToPrinter(1, false, 0, 0);
                }
            }
            else
            {
                Reporting.AuditStockReport objRpt = new Reporting.AuditStockReport();
                objRpt.SetDataSource(BindedDT);
                Reporting.ShowReport objReport = new Reporting.ShowReport("AUDIT STOCK REPORT PREVIEW");
                objReport.myPreview.ReportSource = objRpt;
                objReport.ShowDialog();
            }
        }

        private void StockAudit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && !dgrdDetails.Focused)
                SendKeys.Send("{TAB}");
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                dba.ExportToExcel(dgrdDetails, "Physical_Stock_Report", "Stock As Physical");
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExport.Enabled = true;
        }

        private void StockAudit_Load(object sender, EventArgs e)
        {
            btnUpload.Enabled = btnStockIn .Enabled= MainPage.strUserRole.Contains("SUPERADMIN") ? true : false;
        }
        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                try
                {
                    DialogResult result = MessageBox.Show("Are you sure to delete current record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = "";
                        DataGridViewRow dr = dgrdDetails.CurrentRow;
                        string ID = "";
                        ID = Convert.ToString(dr.Cells["ID"].Value);
                        if (ID != "")
                        {
                            strQuery = " BEGIN TRY IF exists (SELECT * FROM AuditStock where ID =" + ID + ") BEGIN"
                            + " DELETE FROM AuditStock where ID =" + ID + " END "
                            + CreateQuery()
                            + " END TRY BEGIN Catch SELECT -100 as SNo END catch";

                            DataTable DT = DataBaseAccess.GetDataTableRecord(strQuery);
                            if (DT.Rows.Count > 0)
                            {
                                if (dba.ConvertObjectToDouble(DT.Rows[0]["SNo"]) > 0)
                                {
                                    BindDataWithGrid(DT);
                                    dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Yellow;
                                    //dgrdDetails.FirstDisplayedCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells[2];
                                }
                                else
                                {
                                    MessageBox.Show("Sorry ! Record not deleted please retry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                dgrdDetails.DataSource = null;
                                AddNewRow();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _rowIndex = 0;
                foreach (DataGridViewRow dr in dgrdDetails.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["SNo"].Value = (_rowIndex + 1);
                    _rowIndex++;
                }
            }
            catch { }
        }

        //private void btnEdit_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgrdDetails.Rows.Count > 0)
        //        {
        //            if (Convert.ToString(dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["Barcode"].Value) == "")
        //            {
        //                dgrdDetails.Rows.RemoveAt(dgrdDetails.Rows.Count - 1);
        //            }
        //            if (btnEdit.Text == "&Edit")
        //            {
        //                btnEdit.Text = "&Update";
        //                dgrdDetails.ReadOnly = false;
        //                dgrdDetails.Columns["Qty"].ReadOnly = false;
        //                dgrdDetails.Columns["BarCode"].ReadOnly = true;
        //                dgrdDetails.Columns["BrandName"].ReadOnly = true;
        //                dgrdDetails.Columns["ItemName"].ReadOnly = true;
        //                if (MainPage.StrCategory1 != "")
        //                    dgrdDetails.Columns["Variant1"].ReadOnly = true;
        //                if (MainPage.StrCategory2 != "")
        //                    dgrdDetails.Columns["Variant2"].ReadOnly = true;
        //                if (MainPage.StrCategory3 != "")
        //                    dgrdDetails.Columns["Variant3"].ReadOnly = true;
        //                if (MainPage.StrCategory4 != "")
        //                    dgrdDetails.Columns["Variant4"].ReadOnly = true;
        //            }
        //            else
        //            {
        //                btnEdit.Text = "&Edit";
        //                dgrdDetails.ReadOnly = true;
        //            }
        //        }
        //    }
        //    catch { }
        //}

        private void txtBrandName_KeyDown(object sender, KeyEventArgs e)
        {
            //if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            //{
            char objChar = Convert.ToChar(e.KeyCode);
            int value = e.KeyValue;
            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
            {
                SearchData objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", e.KeyCode);
                objSearch.ShowDialog();
                txtBrandName.Text = objSearch.strSelectedData;
            }
            else
            {
                e.Handled = true;
            }
            //}
        }

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            //if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
            //{
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                dba.ClearTextBoxOnKeyDown(sender, e);
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch { }
            //}
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete all reocrds?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strWhere = "";
                        if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                        {
                            DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                            eDate = eDate.AddDays(1);
                            strWhere += " AND (Date >= '" + sDate.ToString("MM/dd/yyyy") + "' and Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";

                            if (!MainPage.strUserRole.Contains("SUPERADMIN"))
                                strWhere += " AND CreatedBy = '" + MainPage.strLoginName + "'";
                        }

                        string strQuery = " Delete from AuditStock Where 1=1 "
                                        + (txtItemName.Text != "" ? " AND ItemName = '" + txtItemName.Text + "'" : "")
                                        + (txtBrandName.Text != "" ? " AND BrandName = '" + txtBrandName.Text + "'" : "") + strWhere
                                        + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                        + " ('AUDITSTOCK','','','" + txtReason.Text + ", With Qty : " + lblTotalQty.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            txtReason.Text = "";
                            pnlDeletionConfirmation.Visible = false;
                            MessageBox.Show("Thank you ! Records deleted successfully ", "Record Delete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            dgrdDetails.DataSource = null;
                            AddNewRow();
                        }
                        else
                            MessageBox.Show("Sorry ! An error occurred, Please try after some time.", "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Sorry ! Please enter the Reason for deletion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtReason.Focus();
                }
            }
            catch
            {
            }
            btnFinalDelete.Enabled = true;
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = false;
        }

        private void dgrdDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == Convert.ToChar(100) || e.KeyChar == Convert.ToChar(68))
                {
                    if (dgrdDetails.CurrentCell.ColumnIndex > 11)
                    {
                        if (objSearch != null)
                            objSearch.Close();
                    }
                }
                else
                {
                    if (objSearch != null)
                    {
                        objSearch.txtSearch.Text = e.KeyChar.ToString().Trim();
                        objSearch.txtSearch.SelectionStart = 1;
                    }
                }
            }
            catch { }
        }

        private string CreateStockInQuery()
        {
            string strQuery = "", strWhere = "";
            DateTime fDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
            DateTime tDate = dba.ConvertDateInExactFormat(txtToDate.Text);

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strWhere += " AND (AU.date >= '" + sDate.ToString("MM/dd/yyyy") + "' and AU.Date <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (txtItemName.Text != "")
                strWhere += " AND AU.ItemName = '" + txtItemName.Text + "'";
            if (txtBrandName.Text != "")
                strWhere += " AND AU.BrandName = '" + txtBrandName.Text + "'";

          
            strQuery += " Update SM SET Qty = AU.Qty, Rate = (CASE WHEN IMS.OpeningRate > 0 then IMS.OpeningRate ELSE IMS.PurchaseRate end) From StockMaster SM inner JOIN AuditStock AU ON SM.BarCode = AU.BarCode LEFT join ItemSecondary IMS ON SM.BarCode = IMS.Description WHERE SM.Billtype='STOCKIN' and AU.Qty != 0" + strWhere;

            strQuery += " INSERT INTO[dbo].[StockMaster] ([BillType],[BillCode],[BillNo],[ItemName],[Variant1],[Variant2],[Variant3],[Variant4],[Variant5],[Qty],[Rate],[GodownName],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus],[MRP],[Date],[BarCode],[BrandName],[DesignName])"
                        + " SELECT 'STOCKIN',IMS.BillCode,IMS.BillNo,AU.ItemName,AU.Variant1,AU.Variant2,AU.Variant3,AU.Variant4,AU.Variant5,AU.Qty,(CASE WHEN IMS.OpeningRate > 0 then IMS.OpeningRate ELSE IMS.PurchaseRate end),'','','',1,0,0,GetDate(),AU.BarCode,Au.BrandName,IMS.DesignName FROM AuditStock AU LEFT join ItemSecondary IMS ON AU.BarCode = IMS.Description AND IMS.Variant1 = AU.Variant1 AND IMS.Variant2 = AU.Variant2 WHERE (SELECT COUNT(*) FROM StockMaster SM2 WHERE SM2.BilLType='STOCKIN' and SM2.BarCode = AU.BarCode) = 0" + strWhere;

            return strQuery;
        }

        private void btnStockIn_Click(object sender, EventArgs e)
        {
            btnStockIn.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to stock in  ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string strQuery = CreateStockInQuery();
                    if (strQuery != "")
                    {
                        int count = dba.ExecuteMyQuery(strQuery);
                        if (count > 0)
                        {
                            MessageBox.Show("Thanks you!  Stock uploaded successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnUpload.Visible = false;
                        }
                        else
                            MessageBox.Show("Sorry ! Record not saved, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch { }
            btnStockIn.Enabled = true;

        }

        //private int CheckBarCodeDuplicate(string strBarCode, int _index)
        //{
        //    //int rowIndex = -1;
        //    //bool saveStatus = false;
        //    //DataGridViewRow row = dgrdDetails.Rows
        //    //    .Cast<DataGridViewRow>()
        //    //    .Where(r => r.Cells["Barcode"].Value.ToString().Equals(strBarCode))
        //    //    .First();
        //    //    rowIndex = row.Index;
        //    //if (rowIndex >= 0)
        //    //{
        //    //    row.Cells["Qty"].Value = dba.ConvertObjectToDouble(row.Cells["Qty"].Value) + 1;
        //    //    dgrdDetails.Rows[dgrdDetails.Rows.Count - 2].Cells[0].Value = row.Cells[0].Value;
        //    //    row.Cells[0].Value = _index;

        //    //    dgrdDetails.Sort(dgrdDetails.Columns[0], ListSortDirection.Ascending);
        //    //    dgrdDetails.Rows[dgrdDetails.Rows.Count - 2].DefaultCellStyle.BackColor = Color.Yellow;

        //    //    saveStatus = SaveUpdate(dgrdDetails.Rows[dgrdDetails.Rows.Count - 2]);
        //    //    if (!saveStatus)
        //    //    {
        //    //        dgrdDetails.Rows[dgrdDetails.Rows.Count - 2].DefaultCellStyle.BackColor = Color.LightGreen;
        //    //    }
        //    //}
        //    //int _rowIndex = 0;
        //    //foreach (DataGridViewRow row in dgrdDetails.Rows)
        //    //{
        //    //    if (_rowIndex != _index)
        //    //    {
        //    //        if (Convert.ToString(row.Cells["Barcode"].Value) == strBarCode)
        //    //        {
        //    //            row.Cells["Qty"].Value = dba.ConvertObjectToDouble(row.Cells["Qty"].Value) + 1;
        //    //            return false;
        //    //        }
        //    //    }
        //    //    _rowIndex++;
        //    //}
        //    //return rowIndex;
        //}
    }
}
