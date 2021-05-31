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
    public partial class Discount_Offer : Form
    {
        DataBaseAccess dba;
        string strOldIncentiveOn = "", strOldIncentiveValue = "", strOldBranchCode = "";
        int getSectionOpened = 0;
        DataTable MainOfferData = new DataTable();
        string strSelDepos = null, strSelBrands = null, strSelCategory = null, strSelItems = null, strSelBarcodes = null;
        string strSelGetDepos = null, strSelGetBrands = null, strSelGetCategory = null, strSelGetItems = null, strSelGetBarcodes = null;
        public Discount_Offer()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetSerialNo();
            BindLastRecord();
        }

        private void SetSerialNo()
        {
            try
            {
                DataTable dt = dba.GetDataTable("SELECT (ISNULL(MAX(OFFER_NO),0)+1)OfferNo FROM OFFER_MASTER");
                if (dt.Rows.Count > 0)
                    txtOfferNo.Text = Convert.ToString(dt.Rows[0]["OfferNo"]);
            }
            catch (Exception ex)
            { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Discount_Offer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                if (pnlGet.Visible)
                    pnlGet.Visible = false;
                else
                    this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            else
            {
                if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                {
                    if (e.KeyCode == Keys.PageUp)
                    {
                        BindNextRecord();
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        BindPreviousRecord();
                    }
                    else if (e.KeyCode == Keys.Home)
                    {
                        BindFirstRecord();
                    }
                    else if (e.KeyCode == Keys.End)
                    {
                        BindLastRecord();
                    }
                    else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.E)
                    {
                        BindAllDataWithControl(txtOfferNo.Text);
                    }
                }
            }
        }

        private void BindNextRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(Offer_No),'') from Offer_Master Where Offer_No>" + txtOfferNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                {
                    BindAllDataWithControl(strSerialNo);
                }
                else
                {
                    BindLastRecord();
                }
            }
            catch
            {
            }
        }

        private void BindPreviousRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(Offer_No),'') from Offer_Master Where Offer_No<" + txtOfferNo.Text + " ");
                string strSerialNo = Convert.ToString(objValue);
                if (strSerialNo != "" && strSerialNo != "0")
                    BindAllDataWithControl(strSerialNo);
                else
                    BindFirstRecord();
            }
            catch
            {
            }
        }

        private void BindFirstRecord()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(Offer_No),'') from Offer_Master ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDataWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }
      
        private void txtDate_Leave(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                dba.GetDateInExactFormat(sender, true, true, true);
        }

        private void chkArrivalDate_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtArvlDtFrom.ReadOnly = txtArvlDtTo.ReadOnly = !chkArrivalDate.Checked;
                txtArvlDtFrom.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtArvlDtTo.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, false, false);
        }

        private void chkOfferValidDate_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtOfferDtFrom.ReadOnly = txtOfferDtTo.ReadOnly = !chkOfferValidDate.Checked;
                txtOfferDtFrom.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
                txtOfferDtTo.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            }
        }

        private void txtOfferDtFrom_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void chkAmt_CheckedChanged(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                txtAmtFrom.ReadOnly = txtAmtTo.ReadOnly = !rdoAmt.Checked;
                double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text);
                if (rdoAmt.Checked)
                    txtQty.Enabled = txtFreeQty.Enabled = false;
                else
                    txtQty.Enabled = txtFreeQty.Enabled = true;
            }
        }

        private void txtAmtFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtAmtTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtFreeQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add")
                {
                    if (btnEdit.Text == "&Update")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Add ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnEdit.Text = "&Edit";
                    }
                    btnAdd.Text = "&Save";
                    txtOfferNo.ReadOnly = false;
                    ClearAllText();
                    PopulateListBoxs(true);
                    EnableAllControls();
                    SetSerialNo();
                    txtDate.Focus();
                    btnDelete.Enabled = false;
                }
                else
                {
                    double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text);
                    if (ValidateOfferControl())
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to save this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRecord();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private bool ValidateOfferControl()
        {
            if (txtOfferName.Text == "" || txtOfferName.Text == null)
            {
                MessageBox.Show("Sorry ! Offer name can not be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOfferName.Focus();
                return false;
            }
            if (txtOfferDesc.Text == "" || txtOfferDesc.Text == null)
            {
                MessageBox.Show("Sorry ! Offer description can not be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOfferDesc.Focus();
                return false;
            }
            if ((txtOfferDesc.Text != "" || txtOfferDesc.Text != null) && (txtOfferName.Text == "" || txtOfferName.Text == null))
            {
                if (txtOfferDesc.Text == txtOfferDesc.Text)
                {
                    MessageBox.Show("Offer name and description can not be same.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOfferDesc.Focus();
                    return false;
                }
            }
            return true;
        }
        private void PopulateListBoxs(bool Add=false)
        {
            try
            {
                string strQuery = "SELECT isnull(IM.MakeName,'') Department,isnull(IM.Other,'') Category,Stock.BrandName Brand, Stock.ItemName Item,BCD.BarCode BarCode "
                                + " , Color = (CASE WHEN IM.MakeName = 'WOMENS' then 'Pink' "
                                + "  WHEN IM.MakeName = 'MENS' then 'Green'"
                                + " WHEN IM.MakeName = 'KIDS' then 'Yellow'"
                                + "  WHEN IM.MakeName = 'ACCESSORIES' then 'Gray' else 'Red' end) FROM (  "
                                + " SELECT SUM(QTY)BalQty, ItemName, BrandName, BarCode FROM( "
                                + " SELECT QTY, isnull(ItemName, '')ItemName, isnull(BrandName, '')BrandName, isnull(BarCode, '')BarCode FROM StockMaster SM "
                                + " WHERE BillType IN('STOCKIN', 'OPENING', 'PURCHASE', 'SALERETURN') "
                                + " UNION ALL "
                                + " SELECT - QTY, isnull(ItemName, '')ItemName, isnull(BrandName, '')BrandName, isnull(BarCode, '')BarCode FROM StockMaster SM "
                                + " WHERE BillType IN('STOCKOUT', 'SALES', 'PURCHASERETURN') "
                                + " )dr  GROUP BY ItemName, BrandName, BarCode HAVING SUM(Qty) > 0 "
                                + " )Stock INNER JOIN Items IM on IM.ItemName = Stock.ItemName "
                                + " LEFT JOIN BarcodeDetails BCD On Stock.BarCode = BCD.ParentBarCode  WHERE isnull(BCD.Instock,0) = 1";

                DataSet DS = dba.GetDataSet(strQuery);

                //string strBUYQuery = " Select(STUFF((SELECT ',' + FiletrValue FROM OfferDetails OD WHERE FilterType = 'BUY' AND OD.OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " AND FilterName = 'DEPARTMENT' FOR XML PATH('')), 1, 1, '')) DEPARTMENT "
                //                    + " ,(STUFF((SELECT ',' + FiletrValue FROM OfferDetails OD WHERE FilterType = 'BUY' AND OD.OfferNo =  " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " AND FilterName = 'BRAND' FOR XML PATH('')), 1, 1, '')) BRAND "
                //                    + " ,(STUFF((SELECT ',' + FiletrValue FROM OfferDetails OD WHERE FilterType = 'BUY' AND OD.OfferNo =  " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " AND FilterName = 'CATEGORY' FOR XML PATH('')), 1, 1, '')) CATEGORY "
                //                    + " ,(STUFF((SELECT ',' + FiletrValue FROM OfferDetails OD WHERE FilterType = 'BUY' AND OD.OfferNo =  " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " AND FilterName = 'ITEM' FOR XML PATH('')), 1, 1, '')) ITEM "
                //                    + " ,(STUFF((SELECT ',' + FiletrValue FROM OfferDetails OD WHERE FilterType = 'BUY' AND OD.OfferNo =  " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " AND FilterName = 'BARCODE' FOR XML PATH('')), 1, 1, '')) BARCODE "

                //DataTable BUYDt = dba.GetDataTable(strQuery);
                //if (BUYDt.Columns.Count > 0)
                //{
                //    strSelDepos = Convert.ToString(BUYDt.Rows[0]["DEPARTMENT"]);
                //    strSelBrands = Convert.ToString(BUYDt.Rows[0]["BRAND"]);
                //    strSelCategory = Convert.ToString(BUYDt.Rows[0]["CATEGORY"]);
                //    strSelItems = Convert.ToString(BUYDt.Rows[0]["ITEM"]);
                //    strSelBarcodes = Convert.ToString(BUYDt.Rows[0]["BARCODE"]);
                //}

                //FillGVAndCheckItTrue(dgrdDepartment, BUYDS.Tables[0], ref strSelDepos);
                //FillGVAndCheckItTrue(dgrdBrand, BUYDS.Tables[1], ref strSelBrands);
                //FillGVAndCheckItTrue(dgrdCategory, BUYDS.Tables[2], ref strSelCategory);
                //FillGVAndCheckItTrue(dgrdItems, BUYDS.Tables[3], ref strSelItems);
                //FillGVAndCheckItTrue(dgrdBarcode, BUYDS.Tables[4], ref strSelBarcodes);

                if (DS.Tables.Count > 0)
                    MainOfferData = DS.Tables[0];

                DataTable DT = MainOfferData.DefaultView.ToTable(true, "Department");
                BindGV( dgrdDepartment, DT);
                BindGV( getDgrdDepartment, DT);
                if (!Add)
                {
                    DT = MainOfferData.DefaultView.ToTable(true, "Brand");
                    BindGV(dgrdBrand, DT);
                    BindGV(getDgrdBrand, DT);
                    DT = MainOfferData.DefaultView.ToTable(true, "Category");
                    BindGV( dgrdCategory, DT);
                    BindGV(getDgrdCategory, DT);
                    DT = MainOfferData.DefaultView.ToTable(true, "Item");
                    BindGV(dgrdItems, DT);
                    BindGV(getDgrdItem, DT);
                    DT = MainOfferData.DefaultView.ToTable(true, "Barcode");
                    BindGV(dgrdBarcode, DT);
                    BindGV(getDgrdBarcode, DT);
                }
                if (getDgrdBarcode.Rows.Count == 0)
                {
                    getDgrdBarcode.Rows.Insert(0);
                    getDgrdBarcode.Focus();
                    getDgrdBarcode.CurrentCell = getDgrdBarcode.Rows[0].Cells[1];
                    getDgrdBarcode.Columns[0].ReadOnly = false;
                }
                if (dgrdBarcode.Rows.Count == 0)
                {
                    dgrdBarcode.Rows.Insert(0);
                    dgrdBarcode.Focus();
                    dgrdBarcode.CurrentCell = getDgrdBarcode.Rows[0].Cells[1];
                    dgrdBarcode.Columns[0].ReadOnly = false;
                }
            }
            catch (Exception ex) { }
        }
        //private DataTable getDTWithColor(DataTable DT,string clm)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("Name", typeof(string));
        //    dt.Columns.Add("Color", typeof(string));

        //    DataView DV = DT.DefaultView;
        //    DV.Sort = clm + " asc";
        //    DT = DV.ToTable(true, clm, "Color");

        //    string lastName=null;

        //    foreach (DataRow dr in DT.Rows)
        //    {
        //        if(lastName != Convert.ToString(dr[clm]))
        //        {
        //            dt.Rows.Add(1);
        //            dt.Rows[dt.Rows.Count - 1][0] = Convert.ToString(dr[clm]);
        //            dt.Rows[dt.Rows.Count - 1][1] = Convert.ToString(dr["Color"]);

        //            lastName = Convert.ToString(dr[0]);
        //        }
        //    }
        //    return dt;
        //}

        private void PopulateListBoxsForUpdate()
        {
            try
            {
                string strQuery = " SELECT DISTINCT FiletrValue Department FROM OfferDetails WHERE FilterType = 'BUY' AND FilterName = 'DEPARTMENT' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Brand FROM OfferDetails WHERE FilterType = 'BUY' AND FilterName = 'BRAND' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Category FROM OfferDetails WHERE FilterType = 'BUY' AND FilterName = 'CATEGORY' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Item FROM OfferDetails WHERE FilterType = 'BUY' AND FilterName = 'ITEM' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Barcode FROM OfferDetails WHERE FilterType = 'BUY' AND FilterName = 'BARCODE' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";

                DataSet DS = dba.GetDataSet(strQuery);

                if (DS.Tables.Count > 0)
                    CheckMarkGrid(dgrdDepartment, DS.Tables[0], ref strSelDepos);
                if (DS.Tables.Count > 1)
                    BindGVForUpdate(dgrdBrand, DS.Tables[1],ref strSelBrands);
                if (DS.Tables.Count > 2)
                    BindGVForUpdate(dgrdCategory, DS.Tables[2], ref strSelCategory);
                if (DS.Tables.Count > 3)
                    BindGVForUpdate(dgrdItems, DS.Tables[3], ref strSelItems);
                if (DS.Tables.Count > 4)
                    BindGVForUpdate(dgrdBarcode, DS.Tables[4], ref strSelBarcodes);
            }
            catch (Exception ex) { }
        }
        private void getSelectedInGVAndClear(DataGridView GV,ref string Selected)
        {
            Selected = null;
            try
            { 
                for (int i = 0; i < GV.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(GV[0, i].EditedFormattedValue)))
                        Selected += "'" + Convert.ToString(GV[1, i].Value) + "',";
                }
                GV.Rows.Clear();

                if (Selected != null && Selected.Last() == ',')
                    Selected = Selected.Substring(0, Selected.Length - 1);
            }
            catch { }
        }
        private void checkUncheckGV(DataGridView GV,bool bchecked,ref string Selected)
        {
            Selected = null;
            try
            {
                for (int i = 0; i < GV.Rows.Count; i++)
                {
                    if (bchecked)
                    {
                        GV[0, i].Value = true;
                        Selected += "'" + Convert.ToString(GV[1, i].Value) + "',";
                    }
                    else
                    {
                        Selected = null;
                        GV[0, i].Value = false;
                    }
                }
                if (Selected != null && Selected.Last() == ',')
                    Selected = Selected.Substring(0, Selected.Length - 1);
            }
            catch { }
        }
        private void getSelectedInGV(DataGridView GV, ref string Selected)
        {
            Selected = null;
            try
            {
                for (int i = 0; i < GV.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(GV[0, i].EditedFormattedValue)))
                        Selected += "'" + Convert.ToString(GV[1, i].Value) + "',";
                }

                if (Selected != null && Selected.Last() == ',')
                    Selected = Selected.Substring(0, Selected.Length - 1);
            }
            catch { }
        }
        private Color getColor(string strName,string Value)
        {
            if (strName.Contains("Department"))
                strName = "Department";
            else if (strName.Contains("Brand"))
                strName = "Brand";
            else if (strName.Contains("Category"))
                strName = "Category";
            else if (strName.Contains("Item"))
                strName = "Item";
            else if (strName.Contains("Barcode"))
                strName = "Barcode";

            string depo = "";
            DataTable dt = new DataTable();
            DataRow[] ValidRows = MainOfferData.Select(strName + " = '" + Value + "'");

            Color c = Color.White;
            if (ValidRows.Length > 0)
            {
                dt = ValidRows.CopyToDataTable();
                DataView DV = dt.DefaultView;
                DataTable dt2 = new DataTable();
                dt2 = DV.ToTable(true, "Department");
                if (dt2.Rows.Count > 1 && strName != "Department")
                {
                    c = Color.SandyBrown;
                    return c;
                }
                depo = Convert.ToString(dt.Rows[0]["Department"]);
            }
            if (depo == "ACCESSORIES")
                c = Color.FromArgb(187, 187, 187);
            if (depo == "WOMENS")
                c = Color.FromArgb(255, 141, 161);
            if (depo == "MENS")
                c = Color.FromArgb(102, 148, 255);
            if (depo == "KIDS")
                c = Color.FromArgb(255, 255, 118);
            if (depo == "")
                c = Color.FromArgb(149, 192, 192);
            return c;
        }
        //private void BindGVWithColor(string strName,DataGridView GV, DataTable DT)
        //{
        //    try
        //    {
        //        GV.Rows.Clear();
        //        GV.Rows.Add(DT.Rows.Count);
        //        int index = 0;
        //        foreach (DataRow dr in DT.Rows)
        //        {
        //            Color clr = getColor(strName, Convert.ToString(dr[0]));
        //            GV.Rows[index].Cells[1].Value = dr[0];
        //            GV.Rows[index].Cells[0].Style.BackColor = clr;
        //            GV.Rows[index].Cells[1].Style.BackColor = clr;
        //            index++;
        //        }
        //    }
        //    catch { }
        //}
        private void BindGV(DataGridView GV, DataTable DT)
        {
            try
            {
                GV.Rows.Clear();
                GV.Rows.Add(DT.Rows.Count);
                int index = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    GV.Rows[index].Cells[1].Value = dr[0];
                    GV.Rows[index].Cells[2].Value = index + 1;

                    Color clr = getColor(GV.Name, Convert.ToString(dr[0]));
                    GV.Rows[index].Cells[0].Style.BackColor = clr;
                    GV.Rows[index].Cells[1].Style.BackColor = clr;
                    index++;
                }
            }
            catch { }
        }
        private void FillGVAndCheckItTrue(DataGridView GV, DataTable DT,string AlreadySelected)
        {
            try
            {
                string[] arr = { };
                if (AlreadySelected != null)
                {
                    StringBuilder sb = new StringBuilder(AlreadySelected);
                    sb.Replace("'", "");
                    AlreadySelected = sb.ToString();
                    arr = AlreadySelected.Split(',');
                }
                int index = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    GV.Rows.Add(1);
                    GV.Rows[GV.Rows.Count - 1].Cells[1].Value = dr[0];
                    GV.Rows[GV.Rows.Count - 1].Cells[2].Value = index+1;

                    Color clr = getColor(GV.Name, Convert.ToString(dr[0]));
                    GV.Rows[GV.Rows.Count - 1].Cells[0].Style.BackColor = clr;
                    GV.Rows[GV.Rows.Count - 1].Cells[1].Style.BackColor = clr;

                    int pos = -1;
                    if (arr.Length > 0)
                        pos = Array.IndexOf(arr, Convert.ToString(dr[0]));
                    if (pos >= 0)
                        GV.Rows[GV.Rows.Count - 1].Cells[0].Value = true;

                    index++;
                }
            }
            catch { }
        }
       
        private void BindGVForUpdate(DataGridView GV, DataTable DT,ref string SelectedInGV)
        {
            try
            {
                GV.Rows.Clear();
                GV.Rows.Add(DT.Rows.Count);
                int index = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    GV.Rows[index].Cells[0].Value = true;
                    GV.Rows[index].Cells[1].Value = dr[0];
                    GV.Rows[index].Cells[2].Value = index+1;

                    Color clr = getColor(GV.Name, Convert.ToString(dr[0]));
                    GV.Rows[index].Cells[0].Style.BackColor = clr;
                    GV.Rows[index].Cells[1].Style.BackColor = clr;

                    SelectedInGV += Convert.ToString(dr[0]);
                    index++;
                }
            }
            catch { }
        }
        private void CheckMarkGrid(DataGridView GV, DataTable DT,ref string SelectedInGV)
        {
            try
            {
                foreach (DataRow dr in DT.Rows)
                {
                    foreach (DataGridViewRow dgr in GV.Rows)
                    {
                        if (Convert.ToString(dgr.Cells[1].Value) == Convert.ToString(dr[0]))
                        {
                            dgr.Cells[0].Value = true;
                            SelectedInGV += Convert.ToString(dr[0]);
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveRecord()
        {
            try
            {
                string strDescType = "", strIsAddOn = "";
                if (!chkArrivalDate.Checked)
                {
                    txtArvlDtFrom.Text = txtArvlDtTo.Text = "";
                }
                if (!chkOfferValidDate.Checked)
                {
                    txtOfferDtFrom.Text = txtOfferDtTo.Text = "";
                }

                if (rdoAmt.Checked)
                    strDescType = "AMT";
                else
                    strDescType = "PER";

                if (chkAddOn.Checked)
                    strIsAddOn = "1";
                else
                    strIsAddOn = "0";
                string strDate = "Null", strArvFDate = "Null", strArvTDate = "Null", strOffFromDate = "Null", OfferToDate = "Null";

                if (txtArvlDtFrom.Text.Trim().Length == 10)
                    strArvFDate = "'" + dba.ConvertDateInExactFormat(txtArvlDtFrom.Text).ToString("MM-dd-yyyy") + "'";
                if (txtArvlDtTo.Text.Trim().Length == 10)
                    strArvTDate = "'" + dba.ConvertDateInExactFormat(txtArvlDtTo.Text).ToString("MM-dd-yyyy") + "'";
                if (txtOfferDtFrom.Text.Trim().Length == 10)
                    strOffFromDate = "'" + dba.ConvertDateInExactFormat(txtOfferDtFrom.Text).ToString("MM-dd-yyyy") + "'";
                if (txtOfferDtTo.Text.Trim().Length == 10)
                    OfferToDate = "'" + dba.ConvertDateInExactFormat(txtOfferDtTo.Text).ToString("MM-dd-yyyy") + "'";
                if (txtDate.Text.Trim().Length == 10)
                    strDate = "'" + dba.ConvertDateInExactFormat(txtDate.Text).ToString("MM-dd-yyyy") + "'";
                double dFreePer = 0;
                if (txtFreePer.Text == "")
                    dFreePer = 100;
                else
                    dFreePer = dba.ConvertObjectToDouble(txtFreePer.Text);
                double CoupanVCount = dba.ConvertObjectToDouble(txtCoupanValidCount.Text);

                string strQuery = " IF(Not Exists (SELECT * FROM Offer_Master WHERE OFFER_NAME = '"+txtOfferName.Text+"' OR (Offer_Code = '" + txtOfferCODE.Text + "' AND OFFER_NO = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + "))) begin "
                                + " IF((SELECT COUNT(ID) From OFFER_MASTER WHERE OFFER_NAME = '" + txtOfferName.Text + "'  AND OFFER_NO != " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ) = 0 ) BEGIN "
                                + " IF((SELECT COUNT(ID) FROM Offer_Master WHERE Offer_Name != '" + txtOfferName.Text + "' AND REWARD_COUPON = '" + txtOfferName.Text + "') <= 1) BEGIN "
                                + " INSERT INTO [OFFER_MASTER] ([OFFER_CODE],[OFFER_NO],[OFFER_NAME],[IsCoupan],[OFFER_DESC],[DATE],[ITEM_ARRIVAL_DATE_FROM],[ITEM_ARRIVAL_DATE_TO],[OFFER_VALID_FROM],[OFFER_VALID_TILL],[DISC_TYPE],[Discount],[AMOUNT_FROM],[AMOUNT_TO],[QUANTITY],[FREE_QTY],[FREE_PER],[FREE_MIN_AMT],[REWARD_COUPON],[CoupanValidCount],[OfferAmount],[IsAddOn],[InsertStatus],[UpdateStatus],[CreatedBy],[UpdatedBy]) "
                                + " VALUES ('OFFER','" + txtOfferNo.Text + "','" + txtOfferName.Text + "','" + Convert.ToString(chkIsCoupan.Checked) + "','" + txtOfferDesc.Text + "'," + strDate + "," + strArvFDate + "," + strArvTDate + "," + strOffFromDate + "," + OfferToDate + ",'" + strDescType + "','" + dba.ConvertObjectToDouble(txtDiscount.Text) + "','" + dba.ConvertObjectToDouble(txtAmtFrom.Text) + "','" + dba.ConvertObjectToDouble(txtAmtTo.Text) + "','" + txtQty.Text + "','" + txtFreeQty.Text + "'," + dFreePer + "," + dba.ConvertObjectToDouble(txtGetMinAmt.Text) + ",'" + txtRewardCoupon.Text + "'," + CoupanVCount + "," + dba.ConvertObjectToDouble(txtOfferAmt.Text) + "," + strIsAddOn + ",1,0,'" + MainPage.strLoginName + "','')"
                                + "";

                strQuery += CreateSubQuery();
                strQuery += CreateGetSubQuery();
                strQuery += " END ELSE BEGIN SELECT -1 Return END END ELSE BEGIN SELECT -2 END end ";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count == -1)
                {
                    MessageBox.Show("Sorry! Offer name is already exists as a coupan code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOfferName.Focus();
                }
                else if (count == -2)
                {
                    MessageBox.Show("Sorry! Offer name is already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOfferName.Focus();
                }
                else if (count > 0)
                {
                    MessageBox.Show("Thank You! Record Saved Successfully .", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    BindLastRecord();
                }
                else
                {
                    MessageBox.Show("Sorry ! Record not saved...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private string CreateSubQuery()
        {
            string strSubQuery = "", strDepartment = "", strBrand = "", strCategory = "", strItem = "", strBarcode = "";

            if (dgrdDepartment.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in dgrdDepartment.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strDepartment += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','DEPARTMENT','" + Convert.ToString(ro.Cells[1].Value) + "','BUY',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (dgrdBrand.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in dgrdBrand.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','BRAND','" + Convert.ToString(ro.Cells[1].Value) + "','BUY',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (dgrdCategory.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in dgrdCategory.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','CATEGORY','" + Convert.ToString(ro.Cells[1].Value) + "','BUY',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (dgrdItems.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in dgrdItems.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','ITEM','" + Convert.ToString(ro.Cells[1].Value) + "','BUY',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (dgrdBarcode.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in dgrdBarcode.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','BARCODE','" + Convert.ToString(ro.Cells[1].Value) + "','BUY',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }

            strSubQuery = " DELETE From OfferDetails where FilterType = 'BUY' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text);
            strSubQuery += strDepartment + strBrand + strCategory + strItem + strBarcode;
            return strSubQuery;
        }
        private void EnableAllControls()
        {
            //txtItemFilter.ReadOnly = txtBarcodeFilter.ReadOnly = txtGetItemFilter.ReadOnly = txtGetBarcodeFilter.ReadOnly = false;
            chkBarcodesAll.Enabled = chkDeparmentAll.Enabled = chkItemsAll.Enabled = chkCategoryAll.Enabled = chkBrandsAll.Enabled = true;
            chkBarcodeAll_get.Enabled = chkDepoAll_get.Enabled = chkItemAll_get.Enabled = chkCategoryAll_get.Enabled = chkBrandAll_get.Enabled = true;
            dgrdDepartment.ReadOnly = dgrdBrand.ReadOnly = dgrdItems.ReadOnly = dgrdBarcode.ReadOnly = dgrdCategory.ReadOnly = false;
            getDgrdDepartment.ReadOnly = getDgrdBrand.ReadOnly = getDgrdItem.ReadOnly = getDgrdBarcode.ReadOnly = getDgrdCategory.ReadOnly = false;

            dgrdDepartment.Columns[1].ReadOnly = true;
            dgrdBrand.Columns[1].ReadOnly = true;
            dgrdItems.Columns[1].ReadOnly = true;
            dgrdCategory.Columns[1].ReadOnly = true;
            getDgrdDepartment.Columns[1].ReadOnly = true;
            getDgrdBrand.Columns[1].ReadOnly = true;
            getDgrdItem.Columns[1].ReadOnly = true;
            getDgrdCategory.Columns[1].ReadOnly = true;

            dgrdBarcode.Columns[0].ReadOnly = false;
            dgrdBarcode.Columns[1].ReadOnly = false;

            getDgrdBarcode.Columns[0].ReadOnly = false;
            getDgrdBarcode.Columns[1].ReadOnly = false;
            txtArvlDtTo.ReadOnly = txtArvlDtFrom.ReadOnly = txtCoupanValidCount.ReadOnly = txtOfferAmt.ReadOnly = txtOfferDesc.ReadOnly = txtAmtFrom.ReadOnly = txtAmtTo.ReadOnly = txtQty.ReadOnly = txtFreeQty.ReadOnly = txtRewardCoupon.ReadOnly = txtDiscount.ReadOnly = txtOfferName.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            //txtItemFilter.ReadOnly = txtBarcodeFilter.ReadOnly = txtGetItemFilter.ReadOnly = txtGetBarcodeFilter.ReadOnly = true;
            chkBarcodesAll.Enabled = chkDeparmentAll.Enabled = chkItemsAll.Enabled = chkCategoryAll.Enabled = chkBrandsAll.Enabled = false;
            chkBarcodeAll_get.Enabled = chkDepoAll_get.Enabled = chkItemAll_get.Enabled = chkCategoryAll_get.Enabled = chkBrandAll_get.Enabled = false;
            dgrdDepartment.ReadOnly = dgrdBrand.ReadOnly = dgrdItems.ReadOnly = dgrdBarcode.ReadOnly = dgrdCategory.ReadOnly = true;
            getDgrdDepartment.ReadOnly = getDgrdBrand.ReadOnly = getDgrdItem.ReadOnly = getDgrdBarcode.ReadOnly = getDgrdCategory.ReadOnly = true;
            txtArvlDtTo.ReadOnly = txtArvlDtFrom.ReadOnly = txtCoupanValidCount.ReadOnly = txtOfferAmt.ReadOnly = txtOfferDesc.ReadOnly = txtAmtFrom.ReadOnly = txtAmtTo.ReadOnly = txtQty.ReadOnly = txtFreeQty.ReadOnly = txtRewardCoupon.ReadOnly = txtDiscount.ReadOnly = txtOfferName.ReadOnly = true;
        }

        private void ClearAllText()
        {
            chkBarcodesAll.Checked = chkDeparmentAll.Checked = chkItemsAll.Checked = chkCategoryAll.Checked = chkBrandsAll.Checked = false;
            chkBarcodeAll_get.Checked = chkDepoAll_get.Checked = chkItemAll_get.Checked = chkCategoryAll_get.Checked = chkBrandAll_get.Checked = false;

            dgrdDepartment.Rows.Clear();
            dgrdBarcode.Rows.Clear();
            dgrdCategory.Rows.Clear();
            dgrdItems.Rows.Clear();
            dgrdBrand.Rows.Clear();
            getDgrdDepartment.Rows.Clear();
            getDgrdBrand.Rows.Clear();
            getDgrdCategory.Rows.Clear();
            dgrdItems.Rows.Clear();
            getDgrdBarcode.Rows.Clear();

            clearChecked();
            clearAllGet();
            rdoPercent.Checked = chkAddOn.Checked = true;
            chkArrivalDate.Checked = chkOfferValidDate.Checked = rdoAmt.Checked = chkIsCoupan.Checked = false;
            txtFreePer.Text = "100";
            txtGetMinAmt.Text = txtCoupanValidCount.Text = "0";
            txtOfferDesc.Text = "";
            txtArvlDtFrom.Text = txtOfferDtFrom.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtArvlDtTo.Text = txtOfferDtTo.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            txtGetDepoFilter.Text = txtGetBrandFilter.Text = txtGetCategoryFilter.Text = txtDepoFilter.Text = txtBrandFilter.Text = txtCategoryFilter.Text= txtItemFilter.Text = txtBarcodeFilter.Text = txtGetItemFilter.Text = txtGetBarcodeFilter.Text = txtAmtFrom.Text = txtAmtTo.Text = txtQty.Text = txtFreeQty.Text = txtRewardCoupon.Text = txtDiscount.Text = txtOfferName.Text = "";
            if (DateTime.Today > MainPage.startFinDate)
            {
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            }
            else
            {
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            }
            txtRewardCoupon.Enabled = txtCoupanValidCount.Enabled = chkIsCoupan.Checked = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnEdit.Text == "&Edit")
                {
                    if (btnAdd.Text == "&Save")
                    {
                        DialogResult result = MessageBox.Show("Are you sure to Edit ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result != DialogResult.Yes)
                            return;
                        btnAdd.Text = "&Add";
                        BindLastRecord();
                    }
                    txtOfferNo.ReadOnly = true;
                    btnDelete.Enabled = true;
                    btnEdit.Text = "&Update";
                    EnableAllControls();
                    //PopulateListBoxs();
                    PopulateListBoxsForUpdate();
                }
                else
                {
                    double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text);
                    //if ((dDiscAmt > 0 && chkAmt.Checked) || (dDiscAmt > 0 && Convert.ToInt64(txtQty.Text) > 0))
                    if (txtOfferName.Text != "" && txtOfferName.Text != null)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to update this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            UpdateRecord();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sorry ! Record not updated...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
            }
        }

        private void UpdateRecord()
        {
            try
            {
                string strDescType = "", strIsAddOn = "";
                if (!chkArrivalDate.Checked)
                {
                    txtArvlDtFrom.Text = txtArvlDtTo.Text = "";
                }
                if (!chkOfferValidDate.Checked)
                {
                    txtOfferDtFrom.Text = txtOfferDtTo.Text = "";
                }

                if (rdoAmt.Checked)
                    strDescType = "AMT";
                else
                    strDescType = "PER";

                if (chkAddOn.Checked)
                    strIsAddOn = "1";
                else
                    strIsAddOn = "0";

                string strArvFDate = "Null", strArvTDate = "Null", strOffFromDate = "Null", OfferToDate = "Null";

                if (txtArvlDtFrom.Text.Trim().Length == 10)
                    strArvFDate = "'" + dba.ConvertDateInExactFormat(txtArvlDtFrom.Text).ToString("MM-dd-yyyy hh:mm:ss") + "'";
                if (txtArvlDtTo.Text.Trim().Length == 10)
                    strArvTDate = "'" + dba.ConvertDateInExactFormat(txtArvlDtTo.Text).ToString("MM-dd-yyyy hh:mm:ss") + "'";
                if (txtOfferDtFrom.Text.Trim().Length == 10)
                    strOffFromDate = "'" + dba.ConvertDateInExactFormat(txtOfferDtFrom.Text).ToString("MM-dd-yyyy hh:mm:ss") + "'";
                if (txtOfferDtTo.Text.Trim().Length == 10)
                    OfferToDate = "'" + dba.ConvertDateInExactFormat(txtOfferDtTo.Text).ToString("MM-dd-yyyy hh:mm:ss") + "'";

                double dFreePer = 0;
                if (txtFreePer.Text == "")
                    dFreePer = 100;
                else
                    dFreePer = dba.ConvertObjectToDouble(txtFreePer.Text);

                double CoupanVCount = dba.ConvertObjectToDouble(txtCoupanValidCount.Text);

                string strQuery = "if exists (select * from OFFER_MASTER where Offer_Code = '" + txtOfferCODE.Text + "' AND OFFER_NO = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + ") begin "
                                + " IF((SELECT COUNT(ID) From OFFER_MASTER WHERE OFFER_NAME = '"+txtOfferName.Text+ "'  AND OFFER_NO != "+dba.ConvertObjectToDouble(txtOfferNo.Text) +" ) = 0 ) BEGIN "
                                + " if((SELECT COUNT(ID) FROM Offer_Master WHERE OFFER_NAME != '" + txtOfferName.Text + "' AND REWARD_COUPON = '" + txtOfferName.Text + "') <= 1) BEGIN "
                                + " UPDATE [OFFER_MASTER] SET [OFFER_NAME]='" + txtOfferName.Text + "', [IsCoupan]='" + Convert.ToString(chkIsCoupan.Checked) + "', [OFFER_DESC] ='" + txtOfferDesc.Text + "', [ITEM_ARRIVAL_DATE_FROM] = " + strArvFDate + ",[ITEM_ARRIVAL_DATE_TO] = " + strArvTDate + ",[OFFER_VALID_FROM] = " + strOffFromDate + ",[OFFER_VALID_TILL] = " + OfferToDate + ",[DISC_TYPE] = '" + strDescType + "',[Discount] = " + dba.ConvertObjectToDouble(txtDiscount.Text) + " ,[AMOUNT_FROM] = " + dba.ConvertObjectToDouble(txtAmtFrom.Text) + " ,[AMOUNT_TO] = " + dba.ConvertObjectToDouble(txtAmtTo.Text)
                                + ", [OfferAmount] = " + dba.ConvertObjectToDouble(txtOfferAmt.Text) + ",[IsAddOn] = " + strIsAddOn + ", [FREE_PER] = " + dFreePer + ", [FREE_MIN_AMT] = " + dba.ConvertObjectToDouble(txtGetMinAmt.Text) + ", [QUANTITY] = '" + txtQty.Text + "' ,[FREE_QTY] = '" + txtFreeQty.Text + "' ,[REWARD_COUPON] = '" + txtRewardCoupon.Text + "',[CoupanValidCount]=" + CoupanVCount + ",[UpdateStatus] = '1' ,[UpdatedBy]= '" + MainPage.strLoginName + "' WHERE Offer_Code = '" + txtOfferCODE.Text + "' AND Offer_No = " + dba.ConvertObjectToDouble(txtOfferNo.Text);

                strQuery += CreateSubQuery();
                strQuery += CreateGetSubQuery();
                strQuery += " End Else BEGIN SELECT -1 RETURN END END ELSE BEGIN SELECT -2 END end";

                int count = dba.ExecuteMyQuery(strQuery);
                if (count == -1)
                {
                    MessageBox.Show("Sorry! Offer name is already exists as a coupan code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOfferName.Focus();
                }
                else if (count == -2)
                {
                    MessageBox.Show("Sorry! Offer name is already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOfferName.Focus();
                }
                if (count > 0)
                {
                    MessageBox.Show("Thank You! Record Updated Successfully .", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnEdit.Text = "&Edit";
                    BindAllDataWithControl(txtOfferNo.Text);
                }
            }
            catch (Exception ex)
            { }
        }

        private void BindLastRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MAX(Offer_No) from OFFER_MASTER Where Offer_No!=0 ");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void BindAllDataWithControl(object objSerialNo)
        {
            try
            {
                double dAmtFrom = 0, dAmtTo = 0;
                string Qry = "Select *,CONVERT(varchar,DATE,103)cDate,CONVERT(varchar,ITEM_ARRIVAL_DATE_FROM,103)ArrivalDateFrom,CONVERT(varchar,ITEM_ARRIVAL_DATE_TO,103)ArrivalDateTo,CONVERT(varchar,OFFER_VALID_FROM,103)OfferVallidFrom,CONVERT(varchar,OFFER_VALID_TILL,103)OfferValidTill from Offer_Master where Offer_No='" + objSerialNo + "'";
                DataTable dt = DataBaseAccess.GetDataTableRecord(Qry);
                DisableAllControls();
                btnDelete.Enabled = true;
                rdoPercent.Checked = true;
                txtOfferNo.ReadOnly = false;
                if (dt.Rows.Count > 0)
                {
                    getSectionOpened = 0;
                    pnlGet.Visible = false;
                    DataRow dr = dt.Rows[0];

                    txtOfferNo.Text = Convert.ToString(dr["Offer_no"]);
                    txtOfferName.Text = Convert.ToString(dr["OFFER_Name"]);
                    txtOfferDesc.Text = Convert.ToString(dr["OFFER_DESC"]);
                    txtOfferAmt.Text = Convert.ToString(dr["OfferAmount"]);
                    txtCoupanValidCount.Text = Convert.ToString(dr["CoupanValidCount"]);
                    if (Convert.ToString(dr["IsAddOn"]) == "")
                        chkAddOn.Checked = false;
                    else
                        chkAddOn.Checked = Convert.ToBoolean(dr["IsAddOn"]);
                    txtDate.Text = Convert.ToString(dr["cDate"]);
                    txtArvlDtFrom.Text = Convert.ToString(dr["ArrivalDateFrom"]).Contains("1900") ? "" : Convert.ToString(dr["ArrivalDateFrom"]);
                    txtArvlDtTo.Text = Convert.ToString(dr["ArrivalDateTo"]).Contains("1900") ? "" : Convert.ToString(dr["ArrivalDateTo"]);
                    txtOfferDtFrom.Text = Convert.ToString(dr["OfferVallidFrom"]).Contains("1900") ? "" : Convert.ToString(dr["OfferVallidFrom"]);
                    txtOfferDtTo.Text = Convert.ToString(dr["OfferValidTill"]).Contains("1900") ? "" : Convert.ToString(dr["OfferValidTill"]);
                    chkOfferValidDate.Checked = true;
                    string strDiscType = Convert.ToString(dr["Disc_Type"]);
                    if (strDiscType == "AMT")
                        rdoAmt.Checked = true;
                    else
                        rdoPercent.Checked = true;
                    txtDiscount.Text = Convert.ToString(dr["Discount"]);
                    txtAmtFrom.Text = Convert.ToString(dr["Amount_From"]);
                    txtAmtTo.Text = Convert.ToString(dr["Amount_To"]);
                    txtQty.Text = Convert.ToString(dr["Quantity"]);
                    txtFreeQty.Text = Convert.ToString(dr["Free_Qty"]);
                    txtFreePer.Text = Convert.ToString(dr["FREE_PER"]);
                    txtGetMinAmt.Text = Convert.ToString(dr["FREE_MIN_AMT"]);
                    txtRewardCoupon.Text = Convert.ToString(dr["Reward_coupon"]);

                    dAmtFrom = dba.ConvertObjectToDouble(dr["Amount_From"]);
                    dAmtTo = dba.ConvertObjectToDouble(dr["Amount_To"]);

                    if(dr["IsCoupan"].ToString() != "")
                    chkIsCoupan.Checked = Convert.ToBoolean(dr["IsCoupan"]);

                    txtRewardCoupon.Enabled = !chkIsCoupan.Checked;
                    txtCoupanValidCount.Enabled = chkIsCoupan.Checked;

                    //if (dAmtFrom > 0 || dAmtTo > 0)
                    //    rdoAmt.Checked = true;

                    //double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text), dQty = dba.ConvertObjectToDouble(txtQty);
                    //if (dDiscAmt > 0 && rdoAmt.Checked)
                    //    txtQty.Enabled = txtFreeQty.Enabled = false;
                    //else if (dDiscAmt > 0 && dQty > 0)
                    //    txtFreeQty.Enabled = rdoAmt.Enabled = false;
                    //if (rdoAmt.Checked)
                    //    txtQty.Enabled = txtFreeQty.Enabled = false;
                    //else
                    //    txtQty.Enabled = txtFreeQty.Enabled = rdoAmt.Enabled = true;
                    PopulateListBoxs();
                    PopulateListBoxsForUpdate();
                }
                else
                {
                    PopulateListBoxs();
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Text = "&Add";
                btnEdit.Text = "&Edit";
                BindLastRecord();
            }
            catch { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            pnlDeletionConfirmation.Visible = true;
            txtReason.Focus();
        }

        private void btnDeletionClose_Click(object sender, EventArgs e)
        {
            txtReason.Text = "";
            pnlDeletionConfirmation.Visible = false;
        }

        private void btnFinalDelete_Click(object sender, EventArgs e)
        {
            try
            {
                btnFinalDelete.Enabled = false;
                if (txtReason.Text != "")
                {

                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit" && txtOfferNo.Text != "")
                    {
                        DialogResult result = MessageBox.Show("Are you sure want to delete record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {

                            string strQuery = " Delete from Offer_Master Where [Offer_No]=" + txtOfferNo.Text
                                            + " DELETE From OfferDetails where [OfferNo] = " + dba.ConvertObjectToDouble(txtOfferNo.Text)
                                            + " INSERT INTO [dbo].[RemovalReason] ([BillType],[BillCode],[BillNo],[Remark],[Date],[DeletedBy]) VALUES "
                                            + " ('OFFER & DISCOUNT','OFFER'," + txtOfferNo.Text + ",'" + txtReason.Text + "',DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())),'" + MainPage.strLoginName + "') ";

                            int count = dba.ExecuteMyQuery(strQuery);
                            if (count > 0)
                            {
                                DataBaseAccess.CreateDeleteQuery(strQuery);

                                MessageBox.Show("Thank you ! Record deleted successfully ! ", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                txtReason.Text = "";
                                pnlDeletionConfirmation.Visible = false;
                                BindLastRecord();
                            }
                            else
                                MessageBox.Show("Sorry ! Record not deleted, Please try after some time !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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

        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //{
            //    double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text), dQty = dba.ConvertObjectToDouble(txtQty);
            //    if (dDiscAmt > 0 && rdoAmt.Checked)
            //        txtQty.Enabled = txtFreeQty.Enabled = false;
            //    else if (dDiscAmt > 0 && dQty > 0)
            //        txtFreeQty.Enabled = rdoAmt.Enabled = false;
            //    else
            //        txtQty.Enabled = txtFreeQty.Enabled = rdoAmt.Enabled = true;
            //}
        }

        private void txtQty_Leave(object sender, EventArgs e)
        {
            //if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //{
            //    double dDiscAmt = dba.ConvertObjectToDouble(txtDiscount.Text), dQty = dba.ConvertObjectToDouble(txtQty.Text);
            //    if (dDiscAmt > 0 && dQty > 0)
            //        txtFreeQty.Enabled = rdoAmt.Enabled = false;
            //    else
            //        txtQty.Enabled = txtFreeQty.Enabled = rdoAmt.Enabled = true;
            //}
        }

        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void clearChecked()
        {
            try
            {
                if (dgrdDepartment.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgrdDepartment.Rows)
                    {
                        row.Cells["chkDepartment"].Value = false;
                    }
                }
                if (dgrdBrand.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgrdBrand.Rows)
                    {
                        row.Cells["chkBrand"].Value = false;
                    }
                }
                if (dgrdCategory.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgrdCategory.Rows)
                    {
                        row.Cells["chkCategory"].Value = false;
                    }
                }
                if (dgrdItems.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgrdItems.Rows)
                    {
                        row.Cells["chkItem"].Value = false;
                    }
                }
                if (dgrdBarcode.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgrdBarcode.Rows)
                    {
                        row.Cells["chkBarcode"].Value = false;
                    }
                }
            }
            catch { }
        }

        private void chkDeparmentAll_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(dgrdDepartment, chkDeparmentAll.Checked,ref strSelDepos);
            getSelectedInGVAndClear(dgrdBrand, ref strSelBrands);
            getSelectedInGVAndClear(dgrdCategory, ref strSelCategory);
            getSelectedInGVAndClear(dgrdItems, ref strSelItems);
            getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
            FilterSubGVs(dgrdDepartment.Name);
            chkBrandsAll.Checked = chkCategoryAll.Checked = chkItemsAll.Checked = chkBarcodesAll.Checked = false;
        }

        private void chkBrandsAll_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(dgrdBrand, chkBrandsAll.Checked, ref strSelBrands);
            getSelectedInGVAndClear(dgrdCategory, ref strSelCategory);
            getSelectedInGVAndClear(dgrdItems, ref strSelItems);
            getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
            FilterSubGVs(dgrdBrand.Name);
            chkCategoryAll.Checked = chkItemsAll.Checked = chkBarcodesAll.Checked = false;
        }

        private void chkCategoryAll_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(dgrdCategory, chkCategoryAll.Checked, ref strSelCategory);
            getSelectedInGVAndClear(dgrdItems, ref strSelItems);
            getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
            FilterSubGVs(dgrdCategory.Name);
            chkItemsAll.Checked = chkBarcodesAll.Checked = false;
        }

        private void chkItemsAll_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(dgrdItems, chkItemsAll.Checked, ref strSelItems);
            getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
            FilterSubGVs(dgrdItems.Name);
            chkBarcodesAll.Checked = false;
        }

        private void chkBarcodesAll_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(dgrdBarcode, chkBarcodesAll.Checked, ref strSelBarcodes);
            FilterSubGVs(dgrdBarcode.Name);
        }

        #region Get

        private string CreateGetSubQuery()
        {
            string strSubQuery = "", strDepartment = "", strBrand = "", strCategory = "", strItem = "", strBarcode = "";

            if (getDgrdDepartment.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in getDgrdDepartment.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strDepartment += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','DEPARTMENT','" + Convert.ToString(ro.Cells[1].Value) + "','GET',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (getDgrdBrand.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in getDgrdBrand.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','BRAND','" + Convert.ToString(ro.Cells[1].Value) + "','GET',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (getDgrdCategory.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in getDgrdCategory.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','CATEGORY','" + Convert.ToString(ro.Cells[1].Value) + "','GET',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (getDgrdItem.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in getDgrdItem.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','ITEM','" + Convert.ToString(ro.Cells[1].Value) + "','GET',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (getDgrdBarcode.Rows.Count > 0)
            {
                foreach (DataGridViewRow ro in getDgrdBarcode.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strBrand += " INSERT INTO OfferDetails(OfferCode,OfferNo,FilterName,FiletrValue,FilterType,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtOfferCODE.Text + "','" + txtOfferNo.Text + "','BARCODE','" + Convert.ToString(ro.Cells[1].Value) + "','GET',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }

            strSubQuery = " DELETE From OfferDetails where FilterType = 'GET' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text);
            strSubQuery += strDepartment + strBrand + strCategory + strItem + strBarcode;
            return strSubQuery;
        }

        private void clearAllGet()
        {
            try
            {
                if (getDgrdDepartment.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in getDgrdDepartment.Rows)
                    {
                        row.Cells["chkGetDepo"].Value = false;
                    }
                }
                if (getDgrdBrand.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in getDgrdBrand.Rows)
                    {
                        row.Cells["chkGetBrand"].Value = false;
                    }
                }
                if (getDgrdCategory.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in getDgrdCategory.Rows)
                    {
                        row.Cells["chkGetCategory"].Value = false;
                    }
                }
                if (getDgrdItem.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in getDgrdItem.Rows)
                    {
                        row.Cells["chkGetItem"].Value = false;
                    }
                }
                if (getDgrdBarcode.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in getDgrdBarcode.Rows)
                    {
                        row.Cells["chkGetBarcode"].Value = false;
                    }
                }
            }
            catch { }
        }

        private void PopulateGetListBoxsForUpdate()
        {
            try
            {
                string strQuery = " SELECT DISTINCT FiletrValue Department FROM OfferDetails WHERE FilterType = 'GET' AND FilterName = 'DEPARTMENT' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Brand FROM OfferDetails WHERE FilterType = 'GET' AND FilterName = 'BRAND' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Category FROM OfferDetails WHERE FilterType = 'GET' AND FilterName = 'CATEGORY' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Item FROM OfferDetails WHERE FilterType = 'GET' AND FilterName = 'ITEM' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";
                strQuery += " SELECT DISTINCT FiletrValue Barcode FROM OfferDetails WHERE FilterType = 'GET' AND FilterName = 'BARCODE' AND OfferCode = '" + txtOfferCODE.Text + "' AND OfferNo = " + dba.ConvertObjectToDouble(txtOfferNo.Text) + " ORDER BY FiletrValue ";

                DataSet DS = dba.GetDataSet(strQuery);
                if (DS.Tables.Count > 0)
                    CheckMarkGrid(getDgrdDepartment, DS.Tables[0], ref strSelDepos);
                if (DS.Tables.Count > 1)
                    BindGVForUpdate(getDgrdBrand, DS.Tables[1], ref strSelGetBrands);
                if (DS.Tables.Count > 2)
                    BindGVForUpdate(getDgrdCategory, DS.Tables[2], ref strSelGetCategory);
                if (DS.Tables.Count > 3)
                    BindGVForUpdate(getDgrdItem, DS.Tables[3], ref strSelGetItems);
                if (DS.Tables.Count > 4)
                    BindGVForUpdate(getDgrdBarcode, DS.Tables[4], ref strSelGetBarcodes);
            }
            catch (Exception ex) { }
        }

        private void chkDepoAll_get_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(getDgrdDepartment, chkDepoAll_get.Checked, ref strSelGetDepos);
            getSelectedInGVAndClear(getDgrdBrand, ref strSelGetBrands);
            getSelectedInGVAndClear(getDgrdCategory, ref strSelGetCategory);
            getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
            getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
            FilterSubGVs(getDgrdDepartment.Name);
            chkBrandAll_get.Checked = chkCategoryAll_get.Checked = chkItemAll_get.Checked = chkBarcodeAll_get.Checked = false;
        }
        private void chkBrandAll_get_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(getDgrdBrand, chkBrandAll_get.Checked, ref strSelGetBrands);
            getSelectedInGVAndClear(getDgrdCategory, ref strSelGetCategory);
            getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
            getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
            FilterSubGVs(getDgrdBrand.Name);
            chkCategoryAll_get.Checked = chkItemAll_get.Checked = chkBarcodeAll_get.Checked = false;
        }
        private void chkCategoryAll_get_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(getDgrdCategory, chkCategoryAll_get.Checked, ref strSelGetCategory);
            getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
            getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
            FilterSubGVs(getDgrdCategory.Name);
            chkItemAll_get.Checked = chkBarcodeAll_get.Checked = false;
        }
        private void chkItemAll_get_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(getDgrdItem, chkItemAll_get.Checked, ref strSelGetItems);
            getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
            FilterSubGVs(getDgrdItem.Name);
            chkBarcodeAll_get.Checked = false;
        }
        private void chkBarcodeAll_get_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(getDgrdBarcode, chkBarcodeAll_get.Checked, ref strSelGetBarcodes);
            FilterSubGVs(getDgrdBarcode.Name);
        }
        #endregion Get
        private void btnShowGet_Click(object sender, EventArgs e)
        {
            if (getSectionOpened == 0)
            {
                chkDepoAll_get.Checked = chkBrandAll_get.Checked = chkCategoryAll_get.Checked = chkItemAll_get.Checked = chkBarcodeAll_get.Checked = false;
                PopulateGetListBoxsForUpdate();
            }

            pnlGet.Visible = !pnlGet.Visible;
            getSectionOpened++;
        }

        private void btnCloseGet_Click(object sender, EventArgs e)
        {
            pnlGet.Visible = false;
        }
        private void btnClearBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    for (int i = 0; i < dgrdBarcode.Rows.Count; i++)
                    {
                        if (!(Convert.ToBoolean(dgrdBarcode[0, i].Value)))
                        {
                            dgrdBarcode.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    dgrdBarcode.Rows.Insert(0);
                   // dgrdBarcode.Rows.Add();
                    dgrdBarcode.Focus();
                    dgrdBarcode.CurrentCell = dgrdBarcode.Rows[0].Cells["Barcode"];
                    dgrdBarcode.Columns["chkBarcode"].ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void btnClearGetBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    for (int i = 0; i < getDgrdBarcode.Rows.Count; i++)
                    {
                        if (!(Convert.ToBoolean(getDgrdBarcode[0, i].Value)))
                        {
                            getDgrdBarcode.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    getDgrdBarcode.Rows.Insert(0);
                   // getDgrdBarcode.Rows.Add();
                    getDgrdBarcode.Focus();
                    getDgrdBarcode.CurrentCell = getDgrdBarcode.Rows[0].Cells["getBarcode"];
                    getDgrdBarcode.Columns["chkGetBarcode"].ReadOnly = false;
                }
            }
            catch (Exception ex) { }
        }
        private void dgrdBarcode_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //    {
            //        if (e.ColumnIndex == 1)
            //        {
            //            string strValue = Convert.ToString(dgrdBarcode.CurrentCell.Value);
            //            if (strValue != "")
            //            {
            //                if (checkDuplicate("BUY", strValue))
            //                {
            //                    if (Convert.ToString(dgrdBarcode[1, dgrdBarcode.Rows.Count - 1].Value)!="")
            //                    {
            //                        dgrdBarcode.Rows.Add();
            //                        dgrdBarcode.Rows[e.RowIndex].Cells["chkBarcode"].Value = true;
            //                        dgrdBarcode.CurrentCell = dgrdBarcode.Rows[e.RowIndex + 1].Cells["Barcode"];
            //                    }
            //                }
            //                else
            //                {
            //                    dgrdBarcode.CurrentCell.Value = "";
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex) { }
        }
        private bool checkDuplicate(string Mode, string Value)
        {
            bool status = true;
            if (Mode == "GET" && (getDgrdBarcode.Rows.Count > 1))
            {
                status = true;
                int cnt = getDgrdBarcode.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    if (Convert.ToString(getDgrdBarcode[1, i].Value) == Value)
                    {
                        status = false;
                        break;
                    }
                }
            }
            else if (Mode == "BUY" && (dgrdBarcode.Rows.Count > 1))
            {
                status = true;
                int cnt = dgrdBarcode.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    if (Convert.ToString(dgrdBarcode[1, i].Value) == Value)
                    {
                        status = false;
                        break;
                    }
                }
            }
            if (!status)
            MessageBox.Show("Sorry ! Same barcode already added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return status;
        }
        private void getDgrdBarcode_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //try
            //{
            //    if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            //    {
            //        if (e.ColumnIndex == 1)
            //        {
            //            string strValue = Convert.ToString(getDgrdBarcode.CurrentCell.Value);
            //            if (strValue != "")
            //            {
            //                if (checkDuplicate("GET", strValue))
            //                {
            //                    if (Convert.ToString(getDgrdBarcode[1, getDgrdBarcode.Rows.Count - 1].Value) != "")
            //                    {
            //                        getDgrdBarcode.Rows.Add();
            //                        getDgrdBarcode.Rows[e.RowIndex].Cells["chkGetBarcode"].Value = true;
            //                        getDgrdBarcode.CurrentCell = getDgrdBarcode.Rows[e.RowIndex + 1].Cells["getBarcode"];
            //                    }
            //                }
            //                else
            //                {
            //                    getDgrdBarcode.CurrentCell.Value = "";
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex) { }
        }

        private void txtOfferNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtOfferNo_Leave(object sender, EventArgs e)
        {
            try
            {
                if (txtOfferNo.Text != "")
                {
                    if (btnAdd.Text == "&Add" && btnEdit.Text == "&Edit")
                    {
                        BindAllDataWithControl(txtOfferNo.Text);
                    }
                }
                else
                {
                    txtOfferNo.Focus();
                }
            }
            catch
            {
            }
        }
        private void dgrdBarcode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1)
                    {
                        SearchCategory_Custom objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_RETAIL", "", "", "", "", "", "", "", Keys.Space, false, false, "BarCode");
                        objSearch.ShowDialog();
                        string strValue = objSearch.strSelectedData;
                        string[] strAllItem = strValue.Split('|');
                        string strBarcode = strAllItem[0].Trim();
                        if (strBarcode != "")
                        {
                            if (checkDuplicate("BUY", strBarcode))
                            {
                                dgrdBarcode[0, e.RowIndex].Value = true;
                                dgrdBarcode[1, e.RowIndex].Value = strBarcode;

                                if (Convert.ToString(dgrdBarcode[1, dgrdBarcode.Rows.Count - 1].Value) != "")
                                {
                                    dgrdBarcode.Rows.Insert(0);
                                    dgrdBarcode.CurrentCell = dgrdBarcode.Rows[0].Cells[1];
                                }
                            }
                        }
                        e.Cancel = true;
                    }
                    //else if(e.ColumnIndex == 0)
                    //{
                    //    if(Convert.ToString(dgrdBarcode[1, e.RowIndex].Value) == "")
                    //    {
                    //        e.Cancel = true;
                    //    }
                    //}
                }
            }

            catch { }
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtOfferAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtCoupanValidCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsSymbol(pressedKey))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void dgrdDepartment_Scroll(object sender, ScrollEventArgs e)
        {
            int h = dgrdDepartment.HorizontalScrollingOffset;
            if (h == 0)
                chkDeparmentAll.Visible = true;
            else
                chkDeparmentAll.Visible = false;
        }

        private void dgrdBrand_Scroll(object sender, ScrollEventArgs e)
        {
            int h = dgrdBrand.HorizontalScrollingOffset;
            if (h == 0)
                chkBrandsAll.Visible = true;
            else
                chkBrandsAll.Visible = false;
        }

        private void dgrdCategory_Scroll(object sender, ScrollEventArgs e)
        {
            int h = dgrdCategory.HorizontalScrollingOffset;
            if (h == 0)
                chkCategoryAll.Visible = true;
            else
                chkCategoryAll.Visible = false;
        }

        private void dgrdItems_Scroll(object sender, ScrollEventArgs e)
        {
            int h = dgrdItems.HorizontalScrollingOffset;
            if (h == 0)
                chkItemsAll.Visible = true;
            else
                chkItemsAll.Visible = false;
        }

        private void dgrdBarcode_Scroll(object sender, ScrollEventArgs e)
        {
            int h = dgrdBarcode.HorizontalScrollingOffset;
            if (h == 0)
                chkBarcodesAll.Visible = true;
            else
                chkBarcodesAll.Visible = false;
        }

        private void getDgrdDepartment_Scroll(object sender, ScrollEventArgs e)
        {
            int h = getDgrdDepartment.HorizontalScrollingOffset;
            if (h == 0)
                chkDepoAll_get.Visible = true;
            else
                chkDepoAll_get.Visible = false;
        }
        private DataTable PrepareData(int lvl,string Wanted, string find1, string strSelected1
                                                    , string find2 = null, string strSelected2 = null
                                                    , string find3 = null, string strSelected3 = null
                                                    , string find4 = null, string strSelected4 = null)
        {
            DataTable dt = new DataTable();
            try
            {
                string qry = "";
                if(lvl >= 1)
                    qry = find1 + " IN (" + strSelected1 + ")";
                if (lvl >= 2)
                    qry += " AND " + find2 + " IN (" + strSelected2 + ")";
                if (lvl >= 3)
                    qry += " AND " + find3 + " IN (" + strSelected3 + ")";
                if (lvl >= 4)
                    qry += " AND " + find4 + " IN (" + strSelected4 + ")";

                if (qry != "")
                {
                    DataRow[] ValidRows = MainOfferData.Select(qry);
                    if (ValidRows.Length > 0)
                    {
                        dt = ValidRows.CopyToDataTable();
                        DataView DV = dt.DefaultView;
                        DV.Sort = Wanted + " asc";
                        dt = DV.ToTable(true, Wanted);
                    }
                }
            }
            catch { }
            return dt;
        }
        private void FilterSubGVs(string clickedGVName,string strFilter = "")
        {
            try
            {
                DataTable dtBrand = new DataTable();
                DataTable dtCategory = new DataTable();
                DataTable dtItem = new DataTable();
                DataTable dtBarcode = new DataTable();

                switch (clickedGVName)
                {
                    case "dgrdDepartment":
                        dtBrand = PrepareData(1,"Brand","Department", strSelDepos);
                        dtCategory = PrepareData(2,"Category", "Department", strSelDepos, "Brand", strSelBrands);
                        dtItem = PrepareData(3,"Item", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory);
                        dtBarcode = PrepareData(4,"Barcode", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory, "Item", strSelItems);
                        
                        FillGVAndCheckItTrue(dgrdBrand, dtBrand, strSelBrands);
                        FillGVAndCheckItTrue(dgrdCategory, dtCategory, strSelCategory);
                        FillGVAndCheckItTrue(dgrdItems, dtItem, strSelItems);
                        FillGVAndCheckItTrue(dgrdBarcode, dtBarcode, strSelBarcodes);
                        break;
                    case "dgrdBrand":
                        dtCategory = PrepareData(2,"Category", "Department", strSelDepos, "Brand", strSelBrands);
                        dtItem = PrepareData(3,"Item", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory);
                        dtBarcode = PrepareData(4,"Barcode", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory, "Item", strSelItems);

                        FillGVAndCheckItTrue(dgrdCategory, dtCategory, strSelCategory);
                        FillGVAndCheckItTrue(dgrdItems, dtItem, strSelItems);
                        FillGVAndCheckItTrue(dgrdBarcode, dtBarcode,  strSelBarcodes);
                        break;
                    case "dgrdCategory":
                        dtItem = PrepareData(3,"Item", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory);
                        dtBarcode = PrepareData(4,"Barcode", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory, "Item", strSelItems);

                        FillGVAndCheckItTrue(dgrdItems, dtItem, strSelItems);
                        FillGVAndCheckItTrue(dgrdBarcode, dtBarcode, strSelBarcodes);
                        break;
                    case "dgrdItems":
                        dtBarcode = PrepareData(4,"Barcode", "Department", strSelDepos, "Brand", strSelBrands, "Category", strSelCategory, "Item", strSelItems);

                        FillGVAndCheckItTrue(dgrdBarcode, dtBarcode, strSelBarcodes);
                        break;
                    case "dgrdBarcode":
                        
                        break;
                    // GETS
                    case "getDgrdDepartment":
                        dtBrand = PrepareData(1,"Brand", "Department", strSelGetDepos);
                        dtCategory = PrepareData(2,"Category", "Department", strSelGetDepos, "Brand", strSelGetBrands);
                        dtItem = PrepareData(3,"Item", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory);
                        dtBarcode = PrepareData(4,"Barcode", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory, "Item", strSelGetItems);

                        FillGVAndCheckItTrue(getDgrdBrand, dtBrand, strSelGetBrands);
                        FillGVAndCheckItTrue(getDgrdCategory, dtCategory, strSelGetCategory);
                        FillGVAndCheckItTrue(getDgrdItem, dtItem, strSelGetItems);
                        FillGVAndCheckItTrue(getDgrdBarcode, dtBarcode, strSelGetBarcodes);
                        break;
                    case "getDgrdBrand":
                        dtCategory = PrepareData(2,"Category", "Department", strSelGetDepos, "Brand", strSelGetBrands);
                        dtItem = PrepareData(3, "Item", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory);
                        dtBarcode = PrepareData(4, "Barcode", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory, "Item", strSelGetItems);

                        FillGVAndCheckItTrue(getDgrdCategory, dtCategory, strSelGetCategory);
                        FillGVAndCheckItTrue(getDgrdItem, dtItem, strSelGetItems);
                        FillGVAndCheckItTrue(getDgrdBarcode, dtBarcode, strSelGetBarcodes);
                        break;
                    case "getDgrdCategory":
                        dtItem = PrepareData(3,"Item", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory);
                        dtBarcode = PrepareData(4, "Barcode", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory, "Item", strSelGetItems);

                        FillGVAndCheckItTrue(getDgrdItem, dtItem, strSelGetItems);
                        FillGVAndCheckItTrue(getDgrdBarcode, dtBarcode, strSelGetBarcodes);
                        break;
                    case "getDgrdItem":
                        dtBarcode = PrepareData(4, "Barcode", "Department", strSelGetDepos, "Brand", strSelGetBrands, "Category", strSelGetCategory, "Item", strSelGetItems);

                        FillGVAndCheckItTrue(getDgrdBarcode, dtBarcode, strSelGetBarcodes);
                        break;
                    case "getDgrdBarcode":
                        
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void FilterGV(DataGridView GV,object sender)
        {
            try
            {
                TextBox txtbox = (TextBox)sender;
                string filter = txtbox.Text.ToUpper();
                int fsno = 0;
                if (txtbox.TextLength > 0)
                {
                    foreach (DataGridViewRow dr in GV.Rows)
                    {
                        if (Convert.ToString(dr.Cells[1].Value).ToUpper().StartsWith(filter))
                        {
                            //GV.FirstDisplayedCell = dr.Cells[0];

                            // fsno = Convert.ToInt32(dr.Cells[2].Value);
                            //dr.Cells[2].Value = 1;
                           // GV.Focus();
                            GV.CurrentCell = dr.Cells[1];
                            break;
                        }
                    }
                    //GV.Rows[0].Cells[2].Value = fsno;
                    //GV.Sort(GV.Columns[2], ListSortDirection.Ascending);
                }
            }
            catch (Exception ex) { }
        }
        private void txtItemFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(dgrdItems, sender);
        }

        private void txtIBarcodeFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(dgrdBarcode, sender);
        }

        private void txtGetItemFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(getDgrdItem, sender);
        }

        private void txtGetBarcodeFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(getDgrdItem, sender);
        }

        private void btnTaxClose_Click(object sender, EventArgs e)
        {
            pnlGet.Visible = false;
        }

        private void dgrdDepartment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 0)
                    {
                        DataGridView GV = (DataGridView)sender;
                        GV.Rows[e.RowIndex].Cells[0].ReadOnly = true;
                        switch (GV.Name)
                        {
                            case "dgrdDepartment":
                                getSelectedInGV(dgrdDepartment,ref strSelDepos);
                                getSelectedInGVAndClear(dgrdBrand, ref strSelBrands);
                                getSelectedInGVAndClear(dgrdCategory, ref strSelCategory);
                                getSelectedInGVAndClear(dgrdItems, ref strSelItems);
                                getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
                                chkBrandsAll.CheckState = chkCategoryAll.CheckState = chkItemsAll.CheckState = chkBarcodesAll.CheckState = CheckState.Unchecked;
                                break;
                            case "dgrdBrand":
                                getSelectedInGV(dgrdBrand, ref strSelBrands);
                                getSelectedInGVAndClear(dgrdCategory, ref strSelCategory);
                                getSelectedInGVAndClear(dgrdItems, ref strSelItems);
                                getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
                                chkCategoryAll.CheckState = chkItemsAll.CheckState = chkBarcodesAll.CheckState = CheckState.Unchecked;
                                break;
                            case "dgrdCategory":
                                getSelectedInGV(dgrdCategory, ref strSelCategory);
                                getSelectedInGVAndClear(dgrdItems, ref strSelItems);
                                getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
                                chkItemsAll.CheckState = chkBarcodesAll.CheckState = CheckState.Unchecked;
                                break;
                            case "dgrdItems":
                                getSelectedInGV(dgrdItems, ref strSelItems);
                                getSelectedInGVAndClear(dgrdBarcode, ref strSelBarcodes);
                                chkBarcodesAll.CheckState = CheckState.Unchecked;
                                break;
                            case "dgrdBarcode":
                                getSelectedInGV(dgrdBarcode, ref strSelBarcodes);
                                break;
                            // GETS
                            case "getDgrdDepartment":
                                getSelectedInGV(getDgrdDepartment,ref strSelGetDepos);
                                getSelectedInGVAndClear(getDgrdBrand, ref strSelGetBrands);
                                getSelectedInGVAndClear(getDgrdCategory, ref strSelGetCategory);
                                getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
                                getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
                                chkBrandAll_get.CheckState = chkCategoryAll_get.CheckState = chkItemAll_get.CheckState = chkBarcodeAll_get.CheckState = CheckState.Unchecked;
                                break;
                            case "getDgrdBrand":
                                getSelectedInGV(getDgrdBrand, ref strSelGetBrands);
                                getSelectedInGVAndClear(getDgrdCategory, ref strSelGetCategory);
                                getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
                                getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
                                chkCategoryAll_get.CheckState = chkItemAll_get.CheckState = chkBarcodeAll_get.CheckState = CheckState.Unchecked;
                                break;
                            case "getDgrdCategory":
                                getSelectedInGV(getDgrdCategory, ref strSelGetCategory);
                                getSelectedInGVAndClear(getDgrdItem, ref strSelGetItems);
                                getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
                                chkItemAll_get.CheckState = chkBarcodeAll_get.CheckState = CheckState.Unchecked;
                                break;
                            case "getDgrdItem":
                                getSelectedInGV(getDgrdItem, ref strSelGetItems);
                                getSelectedInGVAndClear(getDgrdBarcode, ref strSelGetBarcodes);
                                chkBarcodeAll_get.CheckState = CheckState.Unchecked;
                                break;
                            case "getDgrdBarcode":
                                getSelectedInGV(getDgrdBarcode, ref strSelGetBarcodes);
                                break;
                        }
                        FilterSubGVs(GV.Name);
                        GV.Rows[e.RowIndex].Cells[0].ReadOnly = false;
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void btnClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetOfferData()
        {
            try
            {
                string strQuery = " Declare @CDate smallDateTime SET @CDate = DateAdd(MINUTE,30,Dateadd(HOUR,5,GETUTCDATE())) "
                                + " SELECT OM.OFFER_CODE Offer_Code,OM.OFFER_NO Offer_No, OM.OFFER_NAME Offer_Name,OM.OFFER_DESC Offer_Desc"
                                + " ,(CASE WHEN OM.OFFER_VALID_TILL IS not null AND DateAdd(Day, 1, OFFER_VALID_TILL) < @CDate then 'Expired'"
                                + " WHEN OM.OFFER_VALID_FROM IS not null AND OM.OFFER_VALID_FROM > @CDate then 'Upcoming'"
                                + " else 'Currenty ON' end) Offer_Status"
                                + ", Convert(Varchar(10), OM.OFFER_VALID_FROM ,103) Valid_Fr_Date"
                                + ", Convert(Varchar(10), OM.OFFER_VALID_TILL ,103) Valid_To_Date"
                                + ", OM.QUANTITY Purchase_Qty"
                                + ", OM.DISC_TYPE Disc_Type, OM.Discount Disc_Amt, (Case when OM.IsAddOn=1 then 'Yes' else 'No' end) IsAddOn"
                                + ", Convert(Varchar(10), OM.ITEM_ARRIVAL_DATE_FROM,103) Item_Arrival_Fr_Date"
                                + ", Convert(Varchar(10), OM.ITEM_ARRIVAL_DATE_TO ,103) Item_Arrival_To_Date"
                                + ", OM.REWARD_COUPON Reward_Coupan"
                                + ", OM.FREE_QTY Free_Qty"
                                + ", OM.AMOUNT_FROM Min_Purch_Amt, OM.AMOUNT_TO Max_Purch_Amt"
                                + ", OM.OfferAmount Fix_Offer_Amt, OM.FREE_PER Free_Per, OM.FREE_MIN_AMT Min_Pay_Amt"
                                + " FROM  OFFER_MASTER OM";
                DataTable DT = dba.GetDataTable(strQuery);
                BindDataWithGrid(DT);
            }
            catch (Exception ex)
            { }
        }

        private void BindDataWithGrid(DataTable table)
        {
            try
            {
                dgrdOfferDetails.DataSource = null;
                if (table != null)
                {
                    if (table.Rows.Count > 0)
                    {
                        DataView dataView = new DataView(table);
                        dataView.Sort = "Offer_Status Asc";
                        dgrdOfferDetails.DataSource = dataView;
                        SetColumnStyle();
                        SetRowStyle();
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Bind Data with GrdiView in Offers.", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }
        private void SetRowStyle()
        {
            int rowscount = dgrdOfferDetails.Rows.Count;

            for (int i = 0; i < rowscount; i++)
            {
                if (Convert.ToString(dgrdOfferDetails.Rows[i].Cells[4].Value) == "Expired")
                {
                    dgrdOfferDetails.Rows[i].Cells[4].Style.BackColor = Color.FromArgb(255,118,118);
                }
                if (Convert.ToString(dgrdOfferDetails.Rows[i].Cells[4].Value) == "Upcoming")
                {
                    dgrdOfferDetails.Rows[i].Cells[4].Style.BackColor = Color.FromArgb(0, 255, 128);
                }
                if (Convert.ToString(dgrdOfferDetails.Rows[i].Cells[4].Value) == "Currenty ON")
                {
                    dgrdOfferDetails.Rows[i].Cells[4].Style.BackColor = Color.LightGreen;
                }
                dgrdOfferDetails.Rows[i].Cells[0].Style.Font = new Font("Arial", 8.5F, System.Drawing.FontStyle.Underline);
                dgrdOfferDetails.Rows[i].Cells[1].Style.Font = new Font("Arial", 8.5F, System.Drawing.FontStyle.Underline);
                dgrdOfferDetails.Rows[i].Cells[0].Style.ForeColor = Color.FromArgb(64,64,0);
                dgrdOfferDetails.Rows[i].Cells[1].Style.ForeColor = Color.FromArgb(64,64,0);
            }
        }

        private void SetColumnStyle()
        {
            for (int i = 0; i < dgrdOfferDetails.Columns.Count; i++)
            {
                try
                {
                    DataGridViewCellStyle cellStyle = dgrdOfferDetails.Columns[i].DefaultCellStyle;
                    DataGridViewColumn _column = dgrdOfferDetails.Columns[i];

                    string strAlign = "LEFT",clmName = "";
                    int _width = 100;
                    _column.Width = _width;
                    clmName = _column.Name.ToUpper();

                    _column.SortMode = DataGridViewColumnSortMode.Automatic;
                    if (clmName == "S_NO")
                    {
                        strAlign = "MIDDLE";
                        _width = 25;
                    }
                    if (clmName.Contains("DATE"))
                        _width = 90;
                    if (clmName.Contains("ARRIVAL"))
                        _width = 130;
                    if (clmName.Contains("COUPAN"))
                        _width = 110;
                    if (clmName.Contains("QTY") || clmName.Contains("AMT") || clmName.Contains("PER"))
                    {
                        strAlign = "RIGHT";
                        cellStyle.Format = "N2";
                    }
                    if (clmName.Contains("OFFER_NO") || clmName.Contains("ADDON"))
                    {
                        strAlign = "MIDDLE";
                        _width = 70;
                    }

                    if (strAlign == "LEFT")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (strAlign == "MIDDLE")
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else
                        cellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgrdOfferDetails.Columns[i].HeaderText = (dgrdOfferDetails.Columns[i].HeaderText).Replace("_", " ");
                    dgrdOfferDetails.Columns[i].HeaderCell.Style.Font = new Font("Arial", 8.5F, System.Drawing.FontStyle.Bold);
                    dgrdOfferDetails.Columns[i].Width = _width;
                    dgrdOfferDetails.Columns[i].DefaultCellStyle = cellStyle;
                    if(i>0)
                    dgrdOfferDetails.Columns[i].ReadOnly = true;
                }
                catch (Exception ex) { }
            }
        }

        private void dgrdOfferDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    string Header = dgrdOfferDetails.Columns[e.ColumnIndex].HeaderText;
                    if (Header == "Offer No" || Header == "Offer Code")
                    {
                        object strOfferNo = dgrdOfferDetails.Rows[e.RowIndex].Cells["Offer_No"].Value;
                        // object strOfferCode = dgrdOfferDetails.Rows[e.RowIndex].Cells["OfferCode"].Value;
                        ClearAllText();
                        BindAllDataWithControl(strOfferNo);
                        discuntTabs.SelectedTab = offerPanel;
                    }
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Click Event of Grid view While Showing Offer Record", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            GetOfferData();
        }

        private void txtOfferDesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtGetDepoFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(getDgrdDepartment, sender);
        }

        private void txtGetBrandFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(getDgrdBrand, sender);
        }

        private void txtGetCategoryFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(getDgrdCategory, sender);
        }

        private void txtDepoFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(dgrdDepartment, sender);
        }

        private void txtBrandFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(dgrdBrand, sender);
        }

        private void txtCategoryFilter_TextChanged(object sender, EventArgs e)
        {
            FilterGV(dgrdCategory, sender);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                dba.ExportToExcel(dgrdOfferDetails,"Offer_Details", "Offer Report");
            }
            catch (Exception ex) { MessageBox.Show("Sorry ! Error Occured that is " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            btnExport.Enabled = true;
        }

        private void getDgrdBrand_Scroll(object sender, ScrollEventArgs e)
        {
            int h = getDgrdBrand.HorizontalScrollingOffset;
            if (h == 0)
                chkBrandAll_get.Visible = true;
            else
                chkBrandAll_get.Visible = false;
        }

        private void getDgrdCategory_Scroll(object sender, ScrollEventArgs e)
        {
            int h = getDgrdCategory.HorizontalScrollingOffset;
            if (h == 0)
                chkCategoryAll_get.Visible = true;
            else
                chkCategoryAll_get.Visible = false;
        }

        private void getDgrdItem_Scroll(object sender, ScrollEventArgs e)
        {
            int h = getDgrdItem.HorizontalScrollingOffset;
            if (h == 0)
                chkItemAll_get.Visible = true;
            else
                chkItemAll_get.Visible = false;
        }

        private void getDgrdBarcode_Scroll(object sender, ScrollEventArgs e)
        {
            int h = getDgrdBarcode.HorizontalScrollingOffset;
            if (h == 0)
                chkBarcodeAll_get.Visible = true;
            else
                chkBarcodeAll_get.Visible = false;
        }

        private void getDgrdBarcode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1)
                    {
                        SearchCategory_Custom objSearch = new SearchCategory_Custom("", "DESIGNNAMEWITHBARCODE_RETAIL", "", "", "", "", "", "", "", Keys.Space, false, false, "BarCode");
                        objSearch.ShowDialog();
                        string strValue = objSearch.strSelectedData;
                        string[] strAllItem = strValue.Split('|');
                        string strBarcode = strAllItem[0].Trim();
                        if (strBarcode != "")
                        {
                            if (checkDuplicate("GET", strBarcode))
                            {
                                getDgrdBarcode[0, e.RowIndex].Value = true;
                                getDgrdBarcode[1, e.RowIndex].Value = strBarcode;

                                if (Convert.ToString(getDgrdBarcode[1, getDgrdBarcode.Rows.Count - 1].Value) != "")
                                {
                                    getDgrdBarcode.Rows.Insert(0);
                                    getDgrdBarcode.CurrentCell = getDgrdBarcode.Rows[0].Cells[1];
                                }
                            }
                        }
                        e.Cancel = true;
                    }
                    //else if (e.ColumnIndex == 0)
                    //{
                    //    if (Convert.ToString(getDgrdBarcode[2, e.RowIndex].Value) == "")
                    //    {
                    //        e.Cancel = true;
                    //    }
                    //}
                }
            }
            catch (Exception ex) { }
        }

        private void Discount_Offer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void btnGetSectionHide_Click(object sender, EventArgs e)
        {
            pnlGet.Visible = false;
        }

        private void chkIsCoupan_CheckedChanged(object sender, EventArgs e)
        {
            if(chkIsCoupan.Checked)
                lblOfferName.Text = "Coupan Code :";
            else
                lblOfferName.Text = "Offer Name :";

            txtRewardCoupon.Text = "";
            txtCoupanValidCount.Text = "";
            txtRewardCoupon.Enabled = !chkIsCoupan.Checked;
            txtCoupanValidCount.Enabled = chkIsCoupan.Checked;
        }
    }
}
