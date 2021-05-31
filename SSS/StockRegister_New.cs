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
    public partial class StockRegister_New : Form
    {
        DataBaseAccess dba;
        public StockRegister_New()
        {
            InitializeComponent();
            if (MainPage.strCompanyName.Contains("PTN"))
                chkOrderConsider.Checked = true;
            dba = new DataBaseAccess();
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            SetCategory();
        }

        public StockRegister_New(DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            chkDate.Checked = true;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = eDate.ToString("dd/MM/yyyy");
            SetCategory();
            btnGO.Enabled = false;
            GetDataFromDataBase();
            btnGO.Enabled = true;
        }

        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    dgrdDetails.Columns["category1"].HeaderText = MainPage.StrCategory1;
                    dgrdDetails.Columns["category1"].Visible = true;
                    lblCategory1.Text = MainPage.StrCategory1 + " : ";
                }
                else
                {
                    dgrdDetails.Columns["category1"].Visible = false;
                    lblCategory1.Enabled = txtCategory1.Enabled = false;
                }

                if (MainPage.StrCategory2 != "")
                {
                    dgrdDetails.Columns["category2"].HeaderText = MainPage.StrCategory2;
                    dgrdDetails.Columns["category2"].Visible = true;
                    lblCategory2.Text = MainPage.StrCategory2 + " : ";
                }
                else
                {
                    dgrdDetails.Columns["category2"].Visible = false;
                    lblCategory2.Enabled = txtCategory2.Enabled = false;
                }

                if (MainPage.StrCategory3 != "")
                {
                    dgrdDetails.Columns["category3"].HeaderText = MainPage.StrCategory3;
                    dgrdDetails.Columns["category3"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category3"].Visible = false;

                if (MainPage.StrCategory4 != "")
                {
                    dgrdDetails.Columns["category4"].HeaderText = MainPage.StrCategory4;
                    dgrdDetails.Columns["category4"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category4"].Visible = false;

                if (MainPage.StrCategory5 != "")
                {
                    dgrdDetails.Columns["category5"].HeaderText = MainPage.StrCategory5;
                    dgrdDetails.Columns["category5"].Visible = true;
                }
                else
                    dgrdDetails.Columns["category5"].Visible = false;                
            }
            catch
            {
            }
        }

        private void StockRegister_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }       

        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {                    
                     SearchCategory objSearch = new SearchCategory("", "DESIGNNAME", "", txtCategory1.Text, txtCategory2.Text, "", "", "", e.KeyCode,false,"");
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
                    SearchCategory objSearch = new SearchCategory("1", MainPage.StrCategory1, "", "", "", "", "", "", e.KeyCode,false,"");
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
                    SearchCategory objSearch = new SearchCategory("2", MainPage.StrCategory2, "", "", "", "", "", "", e.KeyCode,false,"");
                    objSearch.ShowDialog();
                    txtCategory2.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }

        private string CreateQuery(ref string strSubQuery,ref string strPurchaseQuery)
        {
            string strQuery = "";

            if (txtGroupName.Text != "")
                strQuery += " and ItemName in (Select ItemName from Items Where GroupName in ('" + txtGroupName.Text + "')) ";
            if (txtItemCategory.Text != "")
                strQuery += " and ItemName in (Select ItemName from Items Where Other in ('" + txtItemCategory.Text + "')) ";


            if (txtItemName.Text != "")
            {
                strQuery += " and ItemName='" + txtItemName.Text + "' ";
                strPurchaseQuery = " and GRD.ItemName='" + txtItemName.Text + "' ";
            }
            if (txtCategory1.Text != "")
                strQuery += " and Variant1='" + txtCategory1.Text + "' ";
            if (txtCategory2.Text != "")
                strQuery += " and Variant2='" + txtCategory2.Text + "' ";

            if (txtBranchCode.Text != "")
            {
                strQuery += " and BillCode Like('%" + txtBranchCode.Text + "%') ";
                strPurchaseQuery += " and GR.ReceiptCode Like('%" + txtBranchCode.Text + "%') ";
            }


            if (rdoInStock.Checked)
                strSubQuery = " and (InQty-OutQty)>0 ";
            else if (rdoOutStock.Checked)
                strSubQuery = " and (InQty-OutQty)<0 ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strQuery += " and SM.Date>='" + sDate.ToString("MM/dd/yyyy") + "' and SM.Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
                strPurchaseQuery += " and GR.ReceivingDate>='" + sDate.ToString("MM/dd/yyyy") + "' and GR.ReceivingDate<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }

            return strQuery;
        }

        private void GetDataFromDataBase()
        {
            try
            {
                string strSQuery = "", strPurchaseQuery = "", strSubQuery = CreateQuery(ref strSQuery, ref strPurchaseQuery), strQuery = "", strCategoryQuery = "GroupName", strSummaryQuery = "", strSummaryGroupBy = "", strInnerQuery = "", strOrderQtyQuery = "0" ;

                if (chkOrderConsider.Checked)
                    strOrderQtyQuery = " (Select SUM(CAST(_OB.Quantity as Money)-(_OB.AdjustedQty+ISNULL(_OB.CancelQty,0)))OrderQty from OrderBooking _OB Where CAST(_OB.Quantity as Money)-(_OB.AdjustedQty+ISNULL(_OB.CancelQty,0))>0 and _OB.Items=NewStock.ItemName and _OB.Variant1=NewStock.Variant1 and _OB.Variant2=NewStock.Variant2 and _OB.Status='PENDING' and _OB.OrderType='RETAILORDER') ";

                strCategoryQuery = "AreaCode,Other,GroupName,ItemName,Variant1,Variant2,Variant3,Variant4,Variant5";
                if (rdoDetail.Checked)
                {
                    dgrdDetails.Columns["itemName"].HeaderText = "Item Name";
                    strSummaryGroupBy = " Order BY " + strCategoryQuery;
                }
                else
                {
                    dgrdDetails.Columns["itemName"].HeaderText = "Group Name";
                    strSummaryQuery = " Select StockType,AreaCode,GroupName,SUM(InQty)InQty,SUM(IAmount)IAmount,SUM(OutQty)OutQty,DecimalPoint,SUM(ISNULL(OrderQty,0))OrderQty,QtyRatio from(";
                    strSummaryGroupBy = " )Stock Group by StockType,AreaCode,GroupName,DecimalPoint,QtyRatio Order BY AreaCode,GroupName,StockType";
                }

                strQuery += strSummaryQuery
                         + " Select StockType," + strCategoryQuery + ", InQty,IAmount, OutQty,'' as GodownName,UnitName,(Select DecimalPoint from UnitMaster _UM Where _UM.UnitName = NewStock.UnitName) DecimalPoint,_Rate,"+ strOrderQtyQuery+ " OrderQty,QtyRatio from (";
                if (rdoSTAll.Checked || rdoRetailStock.Checked)
                {
                    strInnerQuery += " Select StockType," + strCategoryQuery + ",SUM(IQty) InQty,SUM(OQty) OutQty,(SUM(IQty*ISNULL(_Rate,SRate)))IAmount,UnitName,ISNULL(_Rate,SRate) as _Rate,QtyRatio from ( "
                     + " Select StockType," + strCategoryQuery + ",SUM(IQty) IQty,SUM(OQty) OQty,UnitName,MAX(DiscPer) as DiscPer,0 as MRP,QtyRatio from ( "
                     + " Select 'RETAIL' as StockType," + strCategoryQuery + ", SUM(Qty) IQty,0 as OQty,(ISNULL(GodownName,'MAIN GODOWN')) as GodownName,UnitName,MRP,DiscPer,QtyRatio from StockMaster SM OUTER APPLY (Select AreaCode from (Select Distinct AreaCode from SupplierMaster)__SM Where BillCode Like('%'+AreaCode+'%'))_SM OUTER APPLY (Select Top 1 DiscPer from PurchaseBook PB Where PB.BillCode=SM.BillCode and PB.BillNo=SM.BillNo) PB OUTER APPLY (Select GroupName,UnitName,Other,QtyRatio from Items _IM Where _IM.ItemName=SM.ItemName) IM Where Qty>0 and BillType in ('PURCHASE')  " + strSubQuery + " Group By " + strCategoryQuery + ",GodownName,UnitName,DiscPer,MRP,QtyRatio  Union All "
                     + " Select 'RETAIL' as StockType," + strCategoryQuery + ", SUM(Qty) IQty,0 as OQty,(ISNULL(GodownName,'MAIN GODOWN')) as GodownName,UnitName,MRP,CAST(GodownName as float) as DiscPer,QtyRatio from StockMaster SM OUTER APPLY (Select AreaCode from (Select Distinct AreaCode from SupplierMaster)__SM Where BillCode Like('%'+AreaCode+'%'))_SM OUTER APPLY (Select GroupName,UnitName,Other,QtyRatio from Items _IM Where _IM.ItemName=SM.ItemName) IM Where Qty>0 and BillType in ('OPENING')  " + strSubQuery + " Group By " + strCategoryQuery + ",GodownName,UnitName,MRP,QtyRatio  Union All "
                     + " Select 'RETAIL' as StockType," + strCategoryQuery + ", SUM(Qty) IQty,0 as OQty,(ISNULL(GodownName,'MAIN GODOWN')) as GodownName,UnitName, MRP,0 as DiscPer,QtyRatio from StockMaster SM OUTER APPLY (Select AreaCode from (Select Distinct AreaCode from SupplierMaster)__SM Where BillCode Like('%'+AreaCode+'%'))_SM OUTER APPLY (Select GroupName,UnitName,Other,QtyRatio from Items _IM Where _IM.ItemName=SM.ItemName) IM Where BillType in ('SALERETURN')  " + strSubQuery + " Group By " + strCategoryQuery + ",GodownName,UnitName,MRP,QtyRatio  Union All "
                     + " Select 'RETAIL' as StockType," + strCategoryQuery + ",0 as IQty,SUM(Qty) OQty,(ISNULL(GodownName,'MAIN GODOWN')) as GodownName,UnitName, MRP,0 as DiscPer,QtyRatio from StockMaster SM OUTER APPLY (Select AreaCode from (Select Distinct AreaCode from SupplierMaster)__SM Where BillCode Like('%'+AreaCode+'%'))_SM OUTER APPLY (Select GroupName,UnitName,Other,QtyRatio from Items _IM Where _IM.ItemName=SM.ItemName) IM Where BillType in ('SALES','PURCHASERETURN') " + strSubQuery + " Group By " + strCategoryQuery + ",GodownName,UnitName,MRP,QtyRatio "
                     + " ) Stock Group By StockType," + strCategoryQuery + ",UnitName,MRP,QtyRatio "
                     + " ) Stock OUTER APPLY (Select (_ISS.PurchaseRate)_Rate from Items _IM OUTER APPLY (Select TOP 1 ((100.00-_ICM.DisPer)*.01) NDisPer from ItemCategoryMaster _ICM Where _IM.Other=_ICM.CategoryName and MRP>=FromRange and MRP<ToRange and MRP>0)_ICM OUTER APPLY (Select _IS.Variant1,_IS.Variant2,((_IS.PurchaseRate*ISNULL(_ICM.NDisPer,1))*((100.00-ISNULL(Stock.DiscPer,0))*0.01))PurchaseRate from ItemSecondary _IS Where _Im.BillCode=_IS.BillCode and _IM.BillNo=_IS.BillNo) _ISS  Where _IM.ItemName=Stock.ItemName and _ISS.Variant1=Stock.Variant1 and _ISS.Variant2=Stock.Variant2)_ICM  OUTER APPLY (Select TOP 1 SRate from (Select 0 as ID, Max(SM.Rate) SRate from StockMaster SM Where  SM.BilLType in ('OPENING','PURCHASE') and SM.ItemName=Stock.ItemName and SM.Variant1=Stock.Variant1 and SM.Variant2=Stock.Variant2 and SM.Variant3=Stock.Variant3 and SM.MRP=Stock.MRP and _Rate IS NULL UNION ALL Select 1 as ID, Max(SM.Rate) SRate from StockMaster SM Where  SM.BilLType in ('SALERETURN') and SM.ItemName=Stock.ItemName and SM.Variant1=Stock.Variant1 and SM.Variant2=Stock.Variant2 and SM.Variant3=Stock.Variant3 and SM.MRP=Stock.MRP and _Rate IS NULL)___Stock  Where SRate is not NULL Order by ID asc)_ST Group By " + strCategoryQuery + ",UnitName,_Rate,SRate,StockType,QtyRatio ";
                }

                if (rdoSTAll.Checked || rdoLooseStock.Checked || rdoInTransite.Checked)
                {
                    if (strInnerQuery != "")
                        strInnerQuery += " UNION ALL ";

                    strInnerQuery += " Select StockType,'' as Other,AreaCode,GroupName,ItemName,'' as Variant1,'' as Variant2,'' as Variant3,'' as Variant4,''as Variant5,SUM(Qty) InQty,0 OutQty,(SUM(Rate*Qty))IAmount,_IM.UnitName,Rate as _Rate,_IM.QtyRatio from ( ";
                    if (rdoSTAll.Checked || rdoLooseStock.Checked)
                        strInnerQuery += " Select 'LOOSE' StockType,'' as Other,GR.ReceiptCode as AreaCode,GroupName, ItemName, Rate, SUM(GRD.Quantity)Qty from GoodsReceive GR inner join GoodsReceiveDetails GRD on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo OUTER APPLY(Select _IM.GroupName from Items _IM Where _IM.ItemName = GRD.ItemName Group by _IM.GroupName) _IM Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN PurchaseType Like('%INCLUDE%') then((Rate * 100) / (100 + TaxRate)) else Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - Rate) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN PurchaseType Like('%INCLUDE%') then((Rate * 100) / (100 + TaxRate)) else Rate end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - DisPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Where GR.SaleBill = 'PENDING' and PackingStatus = 'PACKED' " + strPurchaseQuery + " Group by GR.ReceiptCode,ItemName,Rate,GroupName ";

                    if (rdoSTAll.Checked || rdoInTransite.Checked)
                    {
                        if (rdoSTAll.Checked || rdoLooseStock.Checked)
                            strInnerQuery += " UNION ALL ";
                        strInnerQuery += " Select BillStatus StockType,'' as Other, GR.ReceiptCode as AreaCode,GroupName, ItemName, Rate, SUM(GRD.Quantity)Qty from GoodsReceive GR inner join GoodsReceiveDetails GRD on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo OUTER APPLY(Select _IM.GroupName from Items _IM Where _IM.ItemName = GRD.ItemName Group by _IM.GroupName) _IM Outer APPLY(Select TOP 1 _IGM.Other, _IGM.HSNCode, (CASE WHEN _TC.ChangeTaxRate = 1 then(CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN PurchaseType Like('%INCLUDE%') then((Rate * 100) / (100 + TaxRate)) else Rate end))) * (CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - Rate) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN PurchaseType Like('%INCLUDE%') then((Rate * 100) / (100 + TaxRate)) else Rate end))) *(CASE WHEN _TC.AmountType = 'NET PRICE' then((100.00 - DisPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end) TaxRate from ItemGroupMaster _IGM  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IM.GroupName = _IGM.GroupName and _IGM.ParentGroup = '' ) as GM Cross APPLY (Select BillStatus from SalesRecord SR inner join SalesEntry SE on SR.BillCode = SE.BillCode and SR.BillNo = SE.BillNo Where SR.LrNumber = '' and GRSNo = (GR.ReceiptCode + ' ' + CAST(GR.ReceiptNo as varchar))) SR Where GR.SaleBill = 'CLEAR' " + strPurchaseQuery + " Group by GR.ReceiptCode,GroupName,ItemName,Rate,BillStatus ";
                    }
                    strInnerQuery += " ) _Sales Outer APPLY(Select UnitName,QtyRatio from Items _Im Where _Im.ItemName = _Sales.ItemName) _IM Group by StockType, AreaCode, GroupName, ItemName, UnitName, Rate,QtyRatio ";
                }

                strQuery += strInnerQuery+ " ) NewStock Where (InQty!=0 OR OutQty!=0) " + strSQuery + strSummaryGroupBy;
                              
                DataTable table = dba.GetDataTable(strQuery);
                BindDataTable(table);
            }
            catch
            {
            }
        }

        private void BindDataTable(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            //if (rdoSummary.Checked)
            //{
            //    dgrdDetails.Columns["category1"].Visible = dgrdDetails.Columns["category2"].Visible = dgrdDetails.Columns["category3"].Visible = dgrdDetails.Columns["category4"].Visible = dgrdDetails.Columns["category5"].Visible = false;
            //}
            //else
            //{
            //    SetCategory();
            //}

            string strColumnName = "GroupName";
            if (rdoDetail.Checked)
                strColumnName = "ItemName";


            lblInQty.Text = lblOutQty.Text = lblNetQty.Text = "0.00";
            if (table.Rows.Count > 0)
            {
                dgrdDetails.Rows.Add(table.Rows.Count);
                int rowIndex = 0;
                double dIQty = 0, dOQty = 0, dInQty = 0, dOutQty = 0, dIAmt = 0, dOAmt = 0, dInAmount = 0, dOutAmount=0,dNetRate=0,dNetQty=0,dNetAmt=0,dNetAmount=0,dOrderQty=0,dQtyRatio=0,dStockQty=0, dTStockQty = 0;
                string strDec="";

                foreach (DataRow row in table.Rows)
                {
                    dQtyRatio = dba.ConvertObjectToDouble(row["QtyRatio"]);

                    dInQty += dIQty = dba.ConvertObjectToDouble(row["InQty"]);
                    dOutQty += dOQty = dba.ConvertObjectToDouble(row["OutQty"]);
                    dInAmount += dIAmt = dba.ConvertObjectToDouble(row["IAmount"]);
                    if (rdoDetail.Checked)
                        dNetRate = dba.ConvertObjectToDouble(row["_Rate"]);
                    else if (dIQty != 0 && dIAmt != 0)
                        dNetRate = dIAmt / dIQty;
                                      
                    if (chkOrderConsider.Checked)
                        dOrderQty = dba.ConvertObjectToDouble(row["OrderQty"]);
                    else
                        dOrderQty = 0;

                    dNetQty = dIQty - (dOQty+dOrderQty);
                    dTStockQty += dStockQty = (dNetQty * dQtyRatio);
                    dNetAmount += dNetAmt = dNetRate * dNetQty;

                    strDec = Convert.ToString(row["DecimalPoint"]);
                    dgrdDetails.Rows[rowIndex].Cells["sNo"].Value = rowIndex + 1+".";
                    dgrdDetails.Rows[rowIndex].Cells["itemName"].Value = row[strColumnName];
                    dgrdDetails.Rows[rowIndex].Cells["areaCode"].Value = row["AreaCode"];
                    dgrdDetails.Rows[rowIndex].Cells["stockType"].Value = row["StockType"];
                    if (rdoDetail.Checked)
                    {
                        dgrdDetails.Rows[rowIndex].Cells["category1"].Value = row["Variant1"];
                        dgrdDetails.Rows[rowIndex].Cells["category2"].Value = row["Variant2"];
                        dgrdDetails.Rows[rowIndex].Cells["unitName"].Value = row["UnitName"];
                        dgrdDetails.Rows[rowIndex].Cells["godownName"].Value = row["GodownName"];
                        dgrdDetails.Rows[rowIndex].Cells["category"].Value = row["Other"];
                    }                  

                    dgrdDetails.Rows[rowIndex].Cells["inQty"].Value = dIQty.ToString("N" + strDec, MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["outQty"].Value = dOQty.ToString("N" + strDec, MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["iAmount"].Value = dIAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["oAmount"].Value = dOAmt.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netQty"].Value = dNetQty.ToString("N" + strDec, MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netRate"].Value = dNetRate.ToString("N2", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["netAmount"].Value = dNetAmt.ToString("N0", MainPage.indianCurancy);
                    dgrdDetails.Rows[rowIndex].Cells["orderQty"].Value = dOrderQty.ToString("N" + strDec, MainPage.indianCurancy);                  
                    dgrdDetails.Rows[rowIndex].Cells["stockQty"].Value = dStockQty.ToString("N" + strDec, MainPage.indianCurancy);
                    
                    if (dNetQty < 0)
                        dgrdDetails.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Tomato;
                   
                    //dNetAmount += dNetAmt = Math.Abs(dNetAmt);
                    rowIndex++;
                }

                lblInQty.Text = dInQty.ToString("N2", MainPage.indianCurancy);
                lblOutQty.Text = dOutQty.ToString("N2", MainPage.indianCurancy);
                lblIAmount.Text = dInAmount.ToString("N2", MainPage.indianCurancy);
                lblOutAmt.Text = dOutAmount.ToString("N2", MainPage.indianCurancy);
                lblNetQty.Text = (dInQty - dOutQty).ToString("N2", MainPage.indianCurancy);
                lblNetAmt.Text = dNetAmount.ToString("N0", MainPage.indianCurancy);
                lblStockQty.Text= dTStockQty.ToString("N2", MainPage.indianCurancy);

                if ((dInQty - dOutQty) < 0)
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkRed;
                else
                    lblNetQty.ForeColor = lblNetAmt.ForeColor = Color.DarkGreen;
            }
        }  

        private void btnGO_Click(object sender, EventArgs e)
        {
            btnGO.Enabled = false;
            GetDataFromDataBase();
            btnGO.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = false;
                if (dgrdDetails.Rows.Count > 0)
                {
                    DataTable dt = CreateDataTable();
                    if (dt.Rows.Count > 0)
                    {
                        Reporting.StockRegisterReport objReport = new Reporting.StockRegisterReport();
                        objReport.SetDataSource(dt);
                        string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        strPath += "\\Stock_Details.pdf";
                        System.IO.FileInfo objFile = new System.IO.FileInfo(strPath);
                        if (objFile.Exists)
                            objFile.Delete();
                        strPath = strPath.Replace('/', '_');
                        objReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, strPath);

                        MessageBox.Show("Thank you ! File has been saved on Desktop with the name of Stock_Details", "Record Exported", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
            catch
            {
            }
            btnExport.Enabled = true ;
        }

        private DataTable CreateDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("CompanyName", typeof(String));
            table.Columns.Add("HeaderName", typeof(String));
            table.Columns.Add("SNo", typeof(String));
            table.Columns.Add("ItemName", typeof(String));
            table.Columns.Add("InQty", typeof(String));
            table.Columns.Add("InAmt", typeof(String));
            table.Columns.Add("OutQty", typeof(String));
            table.Columns.Add("OutAmt", typeof(String));
            table.Columns.Add("NetQty", typeof(Double));
            table.Columns.Add("NetRate", typeof(String));
            table.Columns.Add("NetAmt", typeof(String));
            table.Columns.Add("Unit", typeof(String));
            table.Columns.Add("TotalInQty", typeof(String));
            table.Columns.Add("TotalInAmt", typeof(String));
            table.Columns.Add("TotalOutQty", typeof(String));
            table.Columns.Add("TotalOutAmt", typeof(String));
            table.Columns.Add("TotalNetQty", typeof(String));
            table.Columns.Add("TotalNetAmt", typeof(String));

            int rowIndex=1;
            string strItem="", strCategory1 = "", strCategory2 = "";
            foreach (DataGridViewRow row in dgrdDetails.Rows)
            {
                strItem = Convert.ToString(row.Cells["itemName"].Value);
                strCategory1 = Convert.ToString(row.Cells["category1"].Value);
                strCategory2 = Convert.ToString(row.Cells["category2"].Value);
                if (strCategory2 != "")
                    strItem += " / " + strCategory2;
                if (strCategory1 != "")
                    strItem += " / " + strCategory1;
               
                DataRow dRow = table.NewRow();
                dRow["CompanyName"] = MainPage.strPrintComapanyName;
                dRow["HeaderName"] = "Sock Register";
                dRow["SNo"] = rowIndex + ".";
                dRow["ItemName"] = strItem + " " + row.Cells["category3"].Value;
                dRow["InQty"] = row.Cells["inQty"].Value;
                dRow["OutQty"] = row.Cells["outQty"].Value;
                dRow["InAmt"] = row.Cells["iAmount"].Value;
                dRow["OutAmt"] = row.Cells["oAmount"].Value;
                dRow["NetQty"] = row.Cells["netQty"].Value;
                dRow["NetRate"] = row.Cells["netRate"].Value;
                dRow["NetAmt"] = row.Cells["netAmount"].Value;
                dRow["Unit"] = row.Cells["unitName"].Value;
                table.Rows.Add(dRow);
                rowIndex++;
            }

            if (table.Rows.Count > 0)
            {
                rowIndex = table.Rows.Count - 1;
                table.Rows[rowIndex]["TotalInQty"] = lblInQty.Text;
                table.Rows[rowIndex]["TotalInAmt"] = lblIAmount.Text;
                table.Rows[rowIndex]["TotalOutQty"] = lblOutQty.Text;
                table.Rows[rowIndex]["TotalOutAmt"] = lblOutAmt.Text;
                table.Rows[rowIndex]["TotalNetQty"] = lblNetQty.Text;
                table.Rows[rowIndex]["TotalNetQty"] = lblNetAmt.Text;
            }

            return table;
        }

        private void dgrdDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 2 && e.RowIndex>=0)
                {
                    MonthlyStockRegister objMonthly = new MonthlyStockRegister(dgrdDetails.Rows[e.RowIndex], rdoDetail.Checked);
                    objMonthly.MdiParent = MainPage.mymainObject;
                    objMonthly.Show();
                }
            }
            catch
            {
            }
        }

        private void dgrdDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode==Keys.Enter)
                {
                    int rowIndex = dgrdDetails.CurrentRow.Index;
                    if (rowIndex >= 0)
                    {
                        MonthlyStockRegister objMonthly = new MonthlyStockRegister(dgrdDetails.Rows[rowIndex], rdoDetail.Checked);
                        objMonthly.MdiParent = MainPage.mymainObject;
                        objMonthly.Show();
                    }
                }
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
            dba.GetStringFromDateForReporting(txtFromDate);
        }

        private void txtToDate_Leave(object sender, EventArgs e)
        {
            dba.GetStringFromDateForReporting(txtToDate);
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
    }
}
