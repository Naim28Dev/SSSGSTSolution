using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class ShowStockSlabwise : Form
    {
        DataBaseAccess dba;
        public ShowStockSlabwise()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            // BindColumnSettingData();
            btnSelectCompany.Enabled = true;
            GetMultiQuarterName();
            txtDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
        }

        private void PartyBalanceDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (panelCompany.Visible)
                    panelCompany.Visible = false;
                else if (panalCOlumnSetting.Visible)
                    panalCOlumnSetting.Visible = false;
                else
                    this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
              

        private void txtDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }
        private void txtDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, false, true);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            btnGo.Enabled = false;
            try
            {
                if (txtDaysSlab.Text != "")
                {
                    GetDataFromDataBase();
                    chkAll.Checked = true;                   
                }
                else
                {
                    MessageBox.Show("Sorry ! Days slab can't be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! "+ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnGo.Enabled = true;
            panelCompany.Visible = false;
        }

        private void CreateColumn(int _slab)
        {
            if (_slab > 0)
            {
                dgrdDetails.Columns["fAmt"].HeaderText = 0 + " - " + _slab;
                dgrdDetails.Columns["sAmt"].HeaderText = (_slab + 1) + " - " + (_slab * 2);
                dgrdDetails.Columns["tAmt"].HeaderText = ((_slab * 2) + 1) + " - " + (_slab * 3);
                dgrdDetails.Columns["frAmt"].HeaderText = ((_slab * 3) + 1) + " > ";
            }
        }

        private void CreateQuery(ref string strInDateQuery, ref string strOutDateQuery)
        {
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strInDateQuery = " and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }

            if (chkOutDate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtOFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
                eDate = eDate.AddDays(1);
                strOutDateQuery = " and Date>='" + sDate.ToString("MM/dd/yyyy") + "' and Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }
        }

        public string CreateQuery(int _slab)
        {
            string strQuery = "", strSubQuery = "", strDate = "",strPPartyQuery="", strInDateQuery="" , strOutDateQuery="";

            CreateQuery(ref strInDateQuery,ref strOutDateQuery);

            if (txtGroupName.Text != "")
                strSubQuery += " and SM.GroupName='" + txtGroupName.Text + "' ";
            if (txtItemName.Text != "")
                strSubQuery += " and BA.ItemName='" + txtItemName.Text + "' ";
            if (txtItemCategory.Text != "")
                strSubQuery += " and SM.Category in ('" + txtItemCategory.Text + "') ";
            if (txtDepartment.Text != "")
                strSubQuery += " and SM.Department in ('" + txtDepartment.Text + "') ";
            if (txtCategory1.Text != "")
                strSubQuery += " and BA.Variant1='" + txtCategory1.Text + "' ";
            if (txtCategory2.Text != "")
                strSubQuery += " and BA.Variant2='" + txtCategory2.Text + "' ";

            if (txtBranchCode.Text != "")
            {
                strSubQuery += " and BA.BillCode Like('%" + txtBranchCode.Text + "%') ";
            }
            if (txtBrandName.Text != "")
                strSubQuery += " and BA.BrandName='" + txtBrandName.Text + "' ";

            if (txtPurchaseParty.Text != "")
            {
                string[] strFullName = txtPurchaseParty.Text.Split(' ');
                if (strFullName.Length > 1)
                    strPPartyQuery = " CROSS APPLY (Select Top 1 PurchasePartyID from PurchaseBook PB inner join PurchaseBookSecondary PBS on PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo Where PBS.ItemName=BA.ItemName and PBS.Variant1=BA.Variant1 and PBS.Variant2=BA.Variant2 and ISNULL(PBS.BarCode,'')=ISNULL(BA.BarCode,'') and PurchasePartyID='" + strFullName[0] + "') PR ";
            }

            DateTime _date = DateTime.Now;

            if (txtDate.Text.Length == 10)
                _date = dba.ConvertDateInExactFormat(txtDate.Text);
            strDate = _date.ToString("MM/dd/yyyy");
            
            strQuery += "Declare @_Date datetime; "
                           + " Set @_Date='" + strDate + "'; "
                           + " Select _Balance.*,ISNULL(_Rate,SRate) as _Rate from (Select GroupName,DepartMent,Category, BrandName,ItemName,Variant1,Variant2,SUM(FRAmt) FRAmt, SUM(SAmt) SAmt,SUM(TAmt) TAmt,SUM(FAmt) FAmt,SUM(RAmt) as RAmt,SUM(BAmt) BAmt,MAX(DiscPer) as DiscPer,MAX(MRP)MRP from (   "
                           + " Select ItemName, Variant1, Variant2,GroupName,DepartMent, Category, BrandName, SUM((CASE WHEN  _Days < " + _slab + " then Qty else 0 end)) FAmt,SUM((CASE WHEN _Days < (" + _slab+" * 2) and _Days >= ("+ _slab+ ") then(Qty) else 0 end)) SAmt,SUM((CASE WHEN _Days < (" + _slab + " * 3) and _Days >= (" + _slab + " * 2) then(Qty) else 0 end)) TAmt,SUM((CASE WHEN _Days >= (" + _slab + " * 3) then(Qty) else 0 end)) FRAmt,0 as RAmt,SUM(Qty) BAmt,MAX(DiscPer) as DiscPer,MAX(MRP)MRP from ( "
                           + " Select BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SM.DepartMent, SM.Category, BrandName, SUM(BA.Qty) Qty, DATEDIFF(dd, BA.Date, @_Date) _Days, MRP, DiscPer from StockMaster BA  Cross APPLY(Select GroupName,SM.Other as Category,SM.MakeName DepartMent from Items SM Where BA.ItemName = (SM.ItemName))SM OUTER APPLY(Select Top 1 DiscPer from PurchaseBook PB Where PB.BillCode = BA.BillCode and PB.BillNo = BA.BillNo) PB " + strPPartyQuery+" WHere BA.Qty > 0 and BA.BillType in ('PURCHASE') " + strInDateQuery + strSubQuery + " Group by BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SM.DepartMent, SM.Category, BrandName, BA.Date, MRP, DiscPer  UNION ALL "
                           + " Select BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SM.DepartMent, SM.Category, BrandName, SUM(BA.[Qty]) Qty, DATEDIFF(dd, BA.Date, @_Date) _Days, MRP, CAST(GodownName as float) as DiscPer  from StockMaster BA  Cross APPLY(Select GroupName,SM.Other as Category,SM.MakeName DepartMent from Items SM Where BA.ItemName = (SM.ItemName))SM " + strPPartyQuery + " WHere BA.Qty > 0 and BA.BillType in ('OPENING') " + strInDateQuery + strSubQuery + " Group by BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SM.DepartMent,SM.Category, BrandName, BA.Date, MRP, GodownName  UNION ALL "
                           + " Select BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SM.DepartMent, SM.Category, BrandName, SUM(BA.Qty) Qty, DATEDIFF(dd, BA.Date, @_Date) _Days, 0 as MRP, 0 as DiscPer from StockMaster BA  Cross APPLY(Select GroupName,SM.Other as Category,SM.MakeName DepartMent from Items SM Where BA.ItemName = (SM.ItemName))SM " + strPPartyQuery + " WHere BA.Qty > 0 and BA.BillType in ('SALERETURN','STOCKIN') " + strInDateQuery + strSubQuery + " Group by BA.ItemName,BA.Variant1,BA.Variant2,SM.GroupName,SM.DepartMent,SM.Category, BrandName,BA.Date "
                           + " )_Balance Group by GroupName, DepartMent, ItemName, Variant1, Variant2, Category, BrandName"
                           + " UNION ALL  Select BA.ItemName,BA.Variant1,BA.Variant2,SM.GroupName,SM.DepartMent,SM.Category , BrandName, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,-SUM(CAST(BA.Qty as Money)) RAmt,-SUM(CAST(BA.Qty as Money)) BAmt,0 as MRP,0 as DiscPer  from StockMaster BA  Cross APPLY(Select GroupName,SM.Other as Category,SM.MakeName DepartMent from Items SM Where BA.ItemName = (SM.ItemName))SM " + strPPartyQuery + " WHere BA.BillType in ('SALES','PURCHASERETURN','STOCKOUT')  " + strOutDateQuery + strSubQuery + "  Group by BA.ItemName,BA.Variant1,BA.Variant2,SM.GroupName,SM.DepartMent,SM.Category, BrandName "
                           + " )_Balance Group by GroupName, DepartMent, ItemName, Variant1, Variant2, Category, BrandName"
                           + " )_Balance OUTER APPLY(Select (_ISS.PurchaseRate)_Rate from Items _IM OUTER APPLY(Select TOP 1((100.00 - _ICM.DisPer) * .01) NDisPer from ItemCategoryMaster _ICM Where Category = _ICM.CategoryName and MRP >= FromRange and MRP < ToRange and MRP > 0)_ICM OUTER APPLY(Select _IS.Variant1, _IS.Variant2, ((_IS.PurchaseRate * ISNULL(_ICM.NDisPer, 1)) * ((100.00 - _Balance.DiscPer) * 0.01))PurchaseRate from ItemSecondary _IS Where _Im.BillCode = _IS.BillCode and _IM.BillNo = _IS.BillNo) _ISS  Where _IM.ItemName = _Balance.ItemName and _ISS.Variant1 = _Balance.Variant1 and _ISS.Variant2 = _Balance.Variant2)_ICM  OUTER APPLY (Select Top 1 Max(SM.Rate) SRate from StockMaster SM Where SM.ItemName=_Balance.ItemName and SM.Variant1=_Balance.Variant1 and SM.Variant2=_Balance.Variant2 and _Rate IS NULL)_ST Where ItemName != ''  Order by GroupName,Category,ItemName,Variant1,Variant2 ";
                           //+ " Select *,(CASE WHEN (FRAmt+SAmt+TAmt+FAmt)>0.00 then ROUND((Amt/(FRAmt+SAmt+TAmt+FAmt)),2) else 0.00 end)Rate from (Select GroupName,ItemName,Variant1,Variant2,SUM(Amt)Amt, SUM(FRAmt) FRAmt, SUM(SAmt) SAmt,SUM(TAmt) TAmt,SUM(FAmt) FAmt,SUM(RAmt) as RAmt,SUM(BAmt) BAmt from ( "
                           //+ " Select GroupName, ItemName, Variant1, Variant2,SUM(Amt)Amt, SUM((CASE WHEN  _Days < 30 then Qty else 0 end)) FRAmt,SUM((CASE WHEN _Days < (30 * 2) and _Days >= (30)then(Qty) else 0 end)) SAmt,SUM((CASE WHEN _Days < (30 * 3) and _Days >= (30 * 2) then(Qty) else 0 end)) TAmt,SUM((CASE WHEN _Days >= (30 * 3) then(Qty) else 0 end)) FAmt,0 as RAmt,SUM(Qty) BAmt from ( "
                           //+ " Select BA.ItemName, BA.Variant1, BA.Variant2, SM.GroupName,SUM(Qty*Rate)Amt, SUM(BA.Qty) Qty, DATEDIFF(dd, BA.Date, @_Date) _Days from StockMaster BA "+ strPPartyQuery+" Cross APPLY (Select GroupName from Items SM Where BA.ItemName = (SM.ItemName))SM WHere BA.Qty>0 and BA.BillType in ('OPENING', 'PURCHASE', 'SALERETURN') " + strSubQuery+"   Group by BA.ItemName,BA.Variant1,BA.Variant2,SM.GroupName,BA.Date,Rate "
                           //+ " )_Balance Group by GroupName, ItemName, Variant1, Variant2   UNION ALL "
                           //+ " Select SM.GroupName, BA.ItemName,BA.Variant1,BA.Variant2,0 as Amt, 0 FRAmt, 0 SAmt,0 TAmt,0 as FAmt,-SUM(CAST(BA.Qty as Money)) RAmt,-SUM(CAST(BA.Qty as Money)) BAmt from StockMaster BA " + strPPartyQuery + " Cross APPLY (Select GroupName from Items SM Where BA.ItemName = (SM.ItemName))SM WHere BA.BillType in ('SALES','PURCHASERETURN') " + strSubQuery+" Group by BA.ItemName,BA.Variant1,BA.Variant2,SM.GroupName "
                           //+ " )_Balance Group by GroupName, ItemName, Variant1, Variant2 )_Balance Where ItemName != ''  Order by GroupName,ItemName,Variant1,Variant2 ";

            return strQuery;
        }


        //private void GetDataFromDataBase()
        //{
        //    double dSlab = dba.ConvertObjectToDouble(txtDaysSlab.Text);
        //    int _slab = Convert.ToInt32(dSlab);
        //    string strQuery = CreateQuery(_slab);
        //    DataTable _dt = dba.GetDataTable(strQuery);

        //    CreateColumn(_slab);
        //    BindDataWithControl(_dt, _slab);
        //}

        private void GetDataFromDataBase()
        {
            double dSlab = dba.ConvertObjectToDouble(txtDaysSlab.Text);
            int _slab = Convert.ToInt32(dSlab);
            string strQuery = CreateQuery(_slab), strCompanyCode = "";
            DataTable _dt = null, table = null;

            int rowCount = 0;
            foreach (DataGridViewRow row in dgrdCompany.Rows)
            {
                if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                {
                    strCompanyCode = Convert.ToString(row.Cells["code"].Value);
                    if (strCompanyCode != "")
                    {
                        DataTable dt = null;
                        if (rowCount == 0)
                            table = dba.GetMultiQuarterDataTable(strQuery, strCompanyCode);
                        else
                        {
                            dt = dba.GetMultiQuarterDataTable(strQuery.Replace("SUM(BA.[Qty])", "0"), strCompanyCode);
                            if (table == null)
                                table = dt;
                            else if (dt != null)
                                table.Merge(dt, true);
                        }
                        if (table.Rows.Count > 0)
                            rowCount++;
                    }
                }
            }
            CreateColumn(_slab);
            BindDataWithControl(table, _slab);
        }

        private void BindDataWithControl(DataTable _dt, int _slab)
        {
            dgrdDetails.Rows.Clear();
            dgrdDetails.ScrollBars = ScrollBars.Both;
            double dAmt = 0, dBAmt = 0, dFAmt = 0, dRAmt = 0, dSAmt = 0, dTAmt = 0, dFRAmt = 0, dTBAmt = 0, dTRAmt = 0, dTFAmt = 0, dTSAmt = 0, dTTAmt = 0, dTFRAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dRate = 0, dFinalAmt = 0, dFinalTotalAmt = 0;
            double __dNetQty = 0;
            if (txtNetQty.Text != "")
                __dNetQty = dba.ConvertObjectToDouble(txtNetQty.Text);
            if (_dt.Rows.Count > 0)
            {

                DataTable _dtNewTable = _dt.DefaultView.ToTable(true, "ItemName", "BrandName", "GroupName", "variant1", "variant2", "Category", "Department");
                DataView _dv = _dtNewTable.DefaultView;
                _dv.Sort = "ItemName,BrandName,variant1,variant2,Category,Department";
                _dtNewTable = _dv.ToTable();

                int _index = 0;
                foreach (DataRow row in _dtNewTable.Rows)
                {
                    dBAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(BAmt)", "ItemName='" + row["ItemName"] + "'  and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));

                    if (txtNetQty.Text == "" || (dBAmt > __dNetQty || (-1 * dBAmt) > __dNetQty))
                    {
                        dgrdDetails.Rows.Add(1);
                        dTBAmt += dBAmt;
                        dTFAmt += dFAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(FAmt)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));
                        dTSAmt += dSAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(SAmt)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));
                        dTTAmt += dTAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(TAmt)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));
                        dTFRAmt += dFRAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(FRAmt)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));
                        dTRAmt += dRAmt = dba.ConvertObjectToDouble(_dt.Compute("SUM(RAmt)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' "));

                        dRate = dba.ConvertObjectToDouble(_dt.Compute("MAX(_Rate)", "ItemName='" + row["ItemName"] + "' and variant1='" + row["variant1"] + "' and variant2='" + row["variant2"] + "' ")); //dba.ConvertObjectToDouble(row["Rate"]);

                        dgrdDetails.Rows[_index].Cells["chkStatus"].Value = true;
                        dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1) + ".";
                        dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                        dgrdDetails.Rows[_index].Cells["brandName"].Value = row["BrandName"];
                        dgrdDetails.Rows[_index].Cells["groupName"].Value = row["GroupName"];
                        dgrdDetails.Rows[_index].Cells["variant1"].Value = row["variant1"];
                        dgrdDetails.Rows[_index].Cells["variant2"].Value = row["variant2"];
                        dgrdDetails.Rows[_index].Cells["category"].Value = row["Category"];
                        dgrdDetails.Rows[_index].Cells["department"].Value = row["Department"];

                        dAmt = dFRAmt + dRAmt;
                        if ((dAmt > 0 && dFRAmt > 0) || (dAmt < 0 && dFRAmt < 0))
                        {
                            dgrdDetails.Rows[_index].Cells["frAmt"].Value = dAmt;
                            dAmt = 0;
                        }
                        else
                        {
                            dgrdDetails.Rows[_index].Cells["frAmt"].Value = 0.00;
                        }
                        dAmt += dTAmt;

                        if ((dAmt > 0 && dTAmt > 0) || (dAmt < 0 && dTAmt < 0))
                        {
                            dgrdDetails.Rows[_index].Cells["tAmt"].Value = dAmt;
                            dAmt = 0;
                        }
                        else
                        {
                            dgrdDetails.Rows[_index].Cells["tAmt"].Value = 0.00;
                        }
                        dAmt += dSAmt;

                        if ((dAmt > 0 && dSAmt > 0) || (dAmt < 0 && dSAmt < 0))
                        {
                            dgrdDetails.Rows[_index].Cells["sAmt"].Value = dAmt;
                            dAmt = 0;
                        }
                        else
                        {
                            dgrdDetails.Rows[_index].Cells["sAmt"].Value = 0.00;
                        }
                        dAmt += dFAmt;

                        dgrdDetails.Rows[_index].Cells["fAmt"].Value = dAmt;

                        dFinalTotalAmt += dFinalAmt = (dBAmt * dRate);
                        dgrdDetails.Rows[_index].Cells["netBalance"].Value = dBAmt;
                        dgrdDetails.Rows[_index].Cells["netAmt"].Value = dFinalAmt;

                        if (dBAmt >= 0)
                        {
                            dDebitAmt += dBAmt;
                        }
                        else
                        {
                            dCreditAmt += Math.Abs(dBAmt);
                        }

                        _index++;
                    }
                }
            }

            dAmt = dDebitAmt - dCreditAmt;
            if (dAmt >= 0)
                lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkGreen;
            else
                lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkRed;

            lblNetQty.Text = dAmt.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dFinalTotalAmt.ToString("N2", MainPage.indianCurancy);


            lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
            lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
        }


        //private void BindDataWithControl(DataTable _dt, int _slab)
        //{
        //    dgrdDetails.Rows.Clear();
        //    double dAmt = 0, dBAmt = 0, dFAmt = 0, dRAmt = 0, dSAmt = 0, dTAmt = 0, dFRAmt = 0, dTBAmt = 0, dTRAmt = 0, dTFAmt = 0, dTSAmt = 0, dTTAmt = 0, dTFRAmt = 0, dDebitAmt = 0, dCreditAmt = 0, dRate = 0, dFinalAmt = 0, dFinalTotalAmt = 0;
        //    if (_dt.Rows.Count > 0)
        //    {
        //        dgrdDetails.Rows.Add(_dt.Rows.Count);

                    //        int _index = 0;
                    //        foreach (DataRow row in _dt.Rows)
                    //        {
                    //            dTBAmt += dBAmt = dba.ConvertObjectToDouble(row["BAmt"]);
                    //            dTFAmt += dFAmt = dba.ConvertObjectToDouble(row["FAmt"]);
                    //            dTSAmt += dSAmt = dba.ConvertObjectToDouble(row["SAmt"]);
                    //            dTTAmt += dTAmt = dba.ConvertObjectToDouble(row["TAmt"]);
                    //            dTFRAmt += dFRAmt = dba.ConvertObjectToDouble(row["FRAmt"]);
                    //            dTRAmt += dRAmt = dba.ConvertObjectToDouble(row["RAmt"]);
                    //            dRate = dba.ConvertObjectToDouble(row["Rate"]);

                    //            dgrdDetails.Rows[_index].Cells["chkStatus"].Value = true;
                    //            dgrdDetails.Rows[_index].Cells["sNo"].Value = (_index + 1) + ".";
                    //            dgrdDetails.Rows[_index].Cells["itemName"].Value = row["ItemName"];
                    //            dgrdDetails.Rows[_index].Cells["groupName"].Value = row["GroupName"];
                    //            dgrdDetails.Rows[_index].Cells["variant1"].Value = row["variant1"];
                    //            dgrdDetails.Rows[_index].Cells["variant2"].Value = row["variant2"];

                    //            dAmt = dFAmt + dRAmt;
                    //            if ((dAmt >= 0 && dFAmt >= 0) || (dAmt < 0 && dFAmt < 0))
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["frAmt"].Value = dAmt;
                    //                dAmt = 0;
                    //            }
                    //            else
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["frAmt"].Value = 0;
                    //            }
                    //            dAmt += dTAmt;

                    //            if ((dAmt >= 0 && dTAmt > 0) || (dAmt < 0 && dTAmt < 0))
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["tAmt"].Value = dAmt;
                    //                dAmt = 0;
                    //            }
                    //            else
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["tAmt"].Value = 0;
                    //            }
                    //            dAmt += dSAmt;

                    //            if ((dAmt >= 0 && dSAmt > 0) || (dAmt < 0 && dSAmt < 0))
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["sAmt"].Value = dAmt;

                    //                dAmt = 0;
                    //            }
                    //            else
                    //            {
                    //                dgrdDetails.Rows[_index].Cells["sAmt"].Value = 0;
                    //            }
                    //            dAmt += dFRAmt;

                    //            dgrdDetails.Rows[_index].Cells["fAmt"].Value = dAmt;//.ToString("N2", MainPage.indianCurancy)

                    //            dFinalTotalAmt += dFinalAmt = (dBAmt * dRate);
                    //            dgrdDetails.Rows[_index].Cells["netBalance"].Value = dBAmt;
                    //            dgrdDetails.Rows[_index].Cells["netAmt"].Value = dFinalAmt;

                    //            if (dBAmt >= 0)
                    //            {
                    //                dDebitAmt += dBAmt;
                    //            }
                    //            else
                    //            {
                    //                dCreditAmt += Math.Abs(dBAmt);
                    //            }

                    //            _index++;
                    //        }
                    //    }

                    //    dAmt = dDebitAmt - dCreditAmt;
                    //    if (dAmt >= 0)
                    //        lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkGreen;
                    //    else
                    //        lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkRed;

                    //    lblNetQty.Text = dAmt.ToString("N2", MainPage.indianCurancy);
                    //    lblNetAmt.Text = dFinalTotalAmt.ToString("N2", MainPage.indianCurancy);


                    //    lblDebit.Text = dDebitAmt.ToString("N2", MainPage.indianCurancy);
                    //    lblCredit.Text = dCreditAmt.ToString("N2", MainPage.indianCurancy);
                    //}

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (dgrdDetails.CurrentRow.DefaultCellStyle.BackColor.Name != "LightGray")
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    else
                        dgrdDetails.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                }
                else if (e.KeyCode == Keys.Enter && dgrdDetails.CurrentRow.Index >= 0 && dgrdDetails.CurrentCell.ColumnIndex == 1)
                {
                    string strName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                    if (strName != "")
                    {
                        ShowPartyLedger(strName);
                    }
                }
            }
            catch
            {
            }
        }

        private void ShowPartyLedger(string strName)
        {
            LedgerAccount _obj = new LedgerAccount(strName);
            _obj.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            _obj.ShowInTaskbar = true;
            _obj.ShowDialog();
        }


        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            

            return table;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                Reporting.ShowReport objShowReport = new SSS.Reporting.ShowReport("Show Party Record");
                btnPreview.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.PartyBalanceSlabReport objReport = new Reporting.PartyBalanceSlabReport();
                objReport.SetDataSource(dt);
                objShowReport.myPreview.ReportSource = objReport;
                objShowReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btnPreview.Enabled = true;
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    row.Cells["chkStatus"].Value = chkAll.Checked;
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    if (e.NewValue > 0)
                        chkAll.Visible = false;
                    else
                        chkAll.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint.Enabled = false;
                DataTable dt = CreateDataTable();
                Reporting.PartyBalanceSlabReport objReport = new Reporting.PartyBalanceSlabReport();
                objReport.SetDataSource(dt);
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objReport);
                else
                    objReport.PrintToPrinter(1, false, 0, 0);
            }
            catch
            {
            }
            btnPrint.Enabled = true;
        }              

        private void BindColumnSettingData()
        {
            try
            {
                string[] strHeader = { "Party Name", "Group Name", "First Slab", "Second Slab", "Third Slab", "Fourth Slab", "Net Balance", "Category Name" };
                string[] strName = { "partyName", "groupName", "fAmt", "sAmt", "tAmt", "frAmt", "netBalance", "categoryName" };
                int _rowIndex = 0;
                dgrdColumnSetting.Rows.Clear();
                dgrdColumnSetting.Rows.Add(strHeader.Length);
                foreach (string strData in strHeader)
                {
                    dgrdColumnSetting.Rows[_rowIndex].Cells["columnName"].Value = strData;
                    dgrdColumnSetting.Rows[_rowIndex].Cells["colName"].Value = strName[_rowIndex];
                    dgrdColumnSetting.Rows[_rowIndex].Cells["colIndex"].Value = _rowIndex + 1;
                    _rowIndex++;
                }
            }
            catch { }
        }

        private void dgrdColumnSetting_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgrdColumnSetting.CurrentCell.ColumnIndex == 1)
            {
                TextBox txtBox = (TextBox)e.Control;
                txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);               
            }
        }


        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (dgrdColumnSetting.CurrentCell.ColumnIndex == 1)
                    dba.KeyHandlerPoint(sender, e, 0);
            }
            catch { }
        }

        private void RearrangeColumn()
        {
            try
            {
                int _index = 0, dIndex = 1;
                string strColumn = "";
                foreach (DataGridViewRow row in dgrdColumnSetting.Rows)
                {
                    _index = dba.ConvertObjectToInt(row.Cells["colIndex"].Value);
                    strColumn = Convert.ToString(row.Cells["colName"].Value);
                    if (_index == 0)
                    {
                        dgrdDetails.Columns[strColumn].Visible = false;
                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].Visible = false;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].Visible = false;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].Visible = false;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].Visible = false;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].Visible = false;
                    }
                    else
                    {
                        dgrdDetails.Columns[strColumn].Visible = true;
                        dgrdDetails.Columns[strColumn].DisplayIndex = dIndex;
                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].DisplayIndex = ++dIndex;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].DisplayIndex = ++dIndex;

                        if (strColumn == "fAmt")
                            dgrdDetails.Columns["fStatus"].Visible = true;
                        else if (strColumn == "sAmt")
                            dgrdDetails.Columns["sStatus"].Visible = true;
                        else if (strColumn == "tAmt")
                            dgrdDetails.Columns["tStatus"].Visible = true;
                        else if (strColumn == "frAmt")
                            dgrdDetails.Columns["frStatus"].Visible = true;
                        else if (strColumn == "netBalance")
                            dgrdDetails.Columns["netStatus"].Visible = true;

                        dIndex++;
                    }
                }
            }
            catch { }
        }

        private void dgrdColumnSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==1)
            {
                RearrangeColumn();
            }
        }

        private void btnColumnSetting_Click(object sender, EventArgs e)
        {
            panalCOlumnSetting.Visible = true;
        }

        private void btnSClose_Click(object sender, EventArgs e)
        {
            panalCOlumnSetting.Visible = false;
        }
        
        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 0)
                e.Cancel = true;
        }

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", txtCategory1.Text, txtCategory2.Text, "", "", "", e.KeyCode, false,"");
                    objSearch.ShowDialog();
                    txtItemName.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void txtCategory1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory("1", MainPage.StrCategory1, "", "", "", "", "", "", e.KeyCode, false,"");
                    objSearch.ShowDialog();
                    txtCategory1.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void txtCategory2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory("2", MainPage.StrCategory2, "", "", "", "", "", "", e.KeyCode, false,"");
                    objSearch.ShowDialog();
                    txtCategory2.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkDate.Checked, false,false,true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void txtPurchaseParty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtPurchaseParty.Text = objSearch.strSelectedData;                    
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void GetMultiQuarterName()
        {
            try
            {
                string strPath = MainPage.strServerPath + "\\Data";
                dgrdCompany.Rows.Clear();
                DirectoryInfo folder = new DirectoryInfo(strPath);
                if (folder.Exists)
                {
                    int rowIndex = 0;
                    string[] sFolder = Directory.GetDirectories(strPath);
                    DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                    foreach (string folderName in sFolder)
                    {
                        string[] strFile = Directory.GetFiles(folderName, "*.syber");
                        if (strFile.Length > 0)
                        {
                            FileInfo objFile = new FileInfo(folderName);
                            DataTable dt = dba.GetMultiCompanyNameAndFinDate(objFile.Name);
                            if (dt.Rows.Count > 0)
                            {
                                dgrdCompany.Rows.Add();
                                sDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["SDate"]));
                                eDate = dba.ConvertDateInExactFormat(Convert.ToString(dt.Rows[0]["EDate"]));
                                dgrdCompany.Rows[rowIndex].Cells["companyCheck"].Value = (Boolean)false;
                                dgrdCompany.Rows[rowIndex].Cells["code"].Value = dt.Rows[0]["CCode"];
                                dgrdCompany.Rows[rowIndex].Cells["companyName"].Value = dt.Rows[0]["CompanyName"];
                                dgrdCompany.Rows[rowIndex].Cells["sTextDate"].Value = dt.Rows[0]["SDate"];
                                dgrdCompany.Rows[rowIndex].Cells["eTextDate"].Value = dt.Rows[0]["EDate"];
                                dgrdCompany.Rows[rowIndex].Cells["startDate"].Value = sDate;
                                dgrdCompany.Rows[rowIndex].Cells["endDate"].Value = eDate;

                                if (MainPage.multiQSDate > sDate)
                                    MainPage.multiQSDate = sDate;
                                if (MainPage.multiQEDate < eDate)
                                    MainPage.multiQEDate = eDate;
                                rowIndex++;
                            }
                        }
                    }
                }

                if(dgrdCompany.Rows.Count>0)
                    dgrdCompany.Rows[dgrdCompany.Rows.Count-1].Cells["companyCheck"].Value = true;

                dgrdCompany.Sort(dgrdCompany.Columns["startDate"], ListSortDirection.Ascending);
                txtFromDate.Text = txtOFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = txtOToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");

                MainPage.con.Close();
                MainPage.OpenConnection();
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Getting Folder Name in Stock Ageing ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void btnSelectCompany_Click(object sender, EventArgs e)
        {
            if (!panelCompany.Visible)
            {
                panelCompany.Visible = true;
                dgrdCompany.Focus();
            }
            else
            {
                panelCompany.Visible = false;
            }
        }

        private void dgrdCompany_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                GetSelectedQuarterDate();
            }
        }

        private void GetSelectedQuarterDate()
        {
            try
            {
                DateTime sDate = DateTime.Today, eDate = DateTime.Today;
                MainPage.multiQSDate = DateTime.Today;
                MainPage.multiQEDate = DateTime.Today;
                int rowCount = 0;
                foreach (DataGridViewRow row in dgrdCompany.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["companyCheck"].EditedFormattedValue))
                    {
                        sDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["sTextDate"].Value));
                        eDate = dba.ConvertDateInExactFormat(Convert.ToString(row.Cells["eTextDate"].Value));
                        if (rowCount == 0)
                        {
                            MainPage.multiQSDate = sDate;
                            MainPage.multiQEDate = eDate;
                        }
                        else
                        {
                            if (MainPage.multiQSDate > sDate)
                                MainPage.multiQSDate = sDate;
                            if (MainPage.multiQEDate < eDate)
                                MainPage.multiQEDate = eDate;
                        }
                        rowCount++;
                    }
                }
                txtFromDate.Text = txtOFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
                txtToDate.Text = txtOToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
            }
            catch
            {
            }
        }

        private void dgrdCompany_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
                e.Cancel = true;
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
                    saveFileDialog.FileName = "Show_Stock_SlabWise";
                    saveFileDialog.DefaultExt = ".xls";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        xlWorkbook.SaveAs(saveFileDialog.FileName, NewExcel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, NewExcel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    xlWorkbook.Close(true, misValue, misValue);
                    ExcelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlSheets);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);

                    MessageBox.Show("Thank you ! Excel exported successfully.","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);


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

        private void ShowStockSlabwise_Load(object sender, EventArgs e)
        {
            btnExport.Enabled = MainPage.mymainObject.bExport;
            dba.EnableCopyOnClipBoard(dgrdDetails);
        }

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkOutDate.Checked, false, false, true);
        }

        private void txtItemCategory_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMCATEGORYNAME", "SEARCH ITEM CATEGORY", e.KeyCode);
                    objSearch.ShowDialog();
                    txtItemCategory.Text = objSearch.strSelectedData;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void txtBranchCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANCHCODE", "", "SEARCH BRANCH CODE", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBranchCode.Text = objSearch.strSelectedData;
                }
                else
                    e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtBrandName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBrandName.Text = objSearch.strSelectedData;
                }
                e.Handled = true;
            }
            catch
            {
            }
        }

        private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("ITEMGROUPNAME", "SEARCH GROUP NAME", e.KeyCode);
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

        private void txtDepartment_KeyDown(object sender, KeyEventArgs e)
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

        private void chkOutDate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.Enabled = txtOToDate.Enabled = chkOutDate.Checked;
            txtOFromDate.Text = MainPage.multiQSDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.multiQEDate.ToString("dd/MM/yyyy");
        }
    }
}
