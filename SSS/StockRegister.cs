using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NewExcel = Microsoft.Office.Interop.Excel;

namespace SSS
{
    public partial class StockRegister : Form
    {
        DataBaseAccess dba;
        protected internal bool bShowRecord = false;
        List<CheckBox> arrPrint = new List<CheckBox>();
        DataTable BindedDT = new DataTable();

        public StockRegister()
        {
            InitializeComponent();           
            dba = new DataBaseAccess();
            txtFromDate.Text = txtOFromDate.Text= MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = txtOToDate.Text= MainPage.endFinDate.ToString("dd/MM/yyyy");
            SetCategory();
            GetChkSetting("STOCKREGISTER");
        }

        public StockRegister(DateTime eDate)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            chkDate.Checked = true;
            txtFromDate.Text = txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
            SetCategory();
            btnGO.Enabled = false;
            if (MainPage.strSoftwareType == "RETAIL" || MainPage._bCustomPurchase)
                GetDataFromDataBase_Retail();
            else
                GetDataFromDataBase();
            btnGO.Enabled = true;
            GetChkSetting("STOCKREGISTER");
        }
        private void SetCategory()
        {
            try
            {
                if (MainPage.StrCategory1 != "")
                {
                    chkVariant1.Text = MainPage.StrCategory1;
                    txtCategory1.Enabled = true;
                }
                else
                {
                    lblCategory1.Enabled = txtCategory1.Enabled = chkVariant1.Enabled = false;
                }

                if (MainPage.StrCategory2 != "")
                {
                    chkVariant2.Text = MainPage.StrCategory2;
                    lblCategory5.Text = MainPage.StrCategory2;
                    txtCategory2.Enabled = true;
                }
                else
                {
                    lblCategory2.Enabled = txtCategory2.Enabled = chkVariant2.Enabled = false;
                }

                if (MainPage.StrCategory3 != "")
                {
                    chkVariant3.Text = MainPage.StrCategory3;
                    lblCategory3.Text = MainPage.StrCategory3 + " :";
                    txtCategory3.Enabled = true;
                }
                else
                {
                    lblCategory3.Enabled = txtCategory3.Enabled = chkVariant3.Enabled = false;
                }
                if (MainPage.StrCategory4 != "")
                {
                    chkVariant4.Text = MainPage.StrCategory4;
                    lblCategory4.Text = MainPage.StrCategory4 + " :";
                    txtCategory4.Enabled = true;
                }
                else
                {
                    lblCategory4.Enabled = txtCategory4.Enabled = chkVariant4.Enabled = false;
                }
                if (MainPage.StrCategory5 != "")
                {
                    chkVariant5.Text = MainPage.StrCategory5;
                    lblCategory5.Text = MainPage.StrCategory5 + " :";
                    txtCategory5.Enabled = true;
                }
                else
                {
                    lblCategory5.Enabled = txtCategory5.Enabled = chkVariant5.Enabled = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SetColounCategory()
        {
            try
            {
                if (chkVariant1.Checked)
                {
                    if (MainPage.StrCategory1 != "")
                    {
                        dgrdDetails.Columns["variant1"].HeaderText = MainPage.StrCategory1;
                        dgrdDetails.Columns["variant1"].Visible = true;
                        chkVariant1.Text = MainPage.StrCategory1;
                    }
                    else
                        dgrdDetails.Columns["variant1"].Visible = false;
                }

                if (chkVariant2.Checked)
                {
                    if (MainPage.StrCategory2 != "")
                    {
                        dgrdDetails.Columns["variant2"].HeaderText = MainPage.StrCategory2;
                        dgrdDetails.Columns["variant2"].Visible = true;
                        chkVariant2.Text = MainPage.StrCategory2;
                    }
                    else
                        dgrdDetails.Columns["variant2"].Visible = false;
                }

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
            catch (Exception ex)
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
            SelectVariants(sender, e, MainPage.StrCategory1);
        }

        private void txtCategory2_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory2);
        }
        private void SelectVariants(object sender, KeyEventArgs e, string VarName)
        {
            try
            {
                TextBox txt = (TextBox)sender;
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchCategory objSearch = new SearchCategory(txt.Name.Substring(txt.Name.Length - 1, 1), VarName, "", "", "", "", "", "", e.KeyCode, false, "");
                    objSearch.ShowDialog();
                    txt.Text = objSearch.strSelectedData;
                }
            }
            catch
            {
            }
        }
        private string CreateQuery(ref string strInDateQuery, ref string strOutDateQuery)
        {
            string strQuery = "";

            if (txtGroupName.Text != "")
                strQuery += " and GroupName in ('" + txtGroupName.Text + "') ";
            if (txtItemCategory.Text != "")
                strQuery += " and _IM.Other in ('" + txtItemCategory.Text + "') ";
            if (txtSupplierName.Text != "")
            {
                string[] strParty = txtSupplierName.Text.Split(' ');
                if (strParty.Length > 1)
                    strQuery += " and PB.PurchasePartyID in ('" + strParty[0] + "') ";
            }
            if (txtItemName.Text != "")           
                strQuery += " and _Stock.ItemName='" + txtItemName.Text + "' ";
            if (txtBarCode.Text != "")
                strQuery += " and _Stock.BarCode='" + txtBarCode.Text + "' ";
            if (txtCategory1.Text != "")
                strQuery += " and _Stock.Variant1='" + txtCategory1.Text + "' ";
            if (txtCategory2.Text != "")
                strQuery += " and _Stock.Variant2='" + txtCategory2.Text + "' ";
            if (txtCategory3.Text != "")
                strQuery += " and _Stock.Variant3='" + txtCategory3.Text + "' ";
            if (txtCategory4.Text != "")
                strQuery += " and _Stock.Variant4='" + txtCategory4.Text + "' ";
            if (txtCategory5.Text != "")
                strQuery += " and _Stock.Variant5='" + txtCategory5.Text + "' ";

            if (txtBranchCode.Text != "")
            {
                strQuery += " and PB.BillCode Like('%" + txtBranchCode.Text + "%') ";               
            }
            if (txtBrandName.Text != "")
                strQuery += " and _Stock.BrandName='" + txtBrandName.Text + "' ";
         

            if (rdoInStock.Checked)
                strQuery += " and (ISNULL(InQty,0)-ISNULL(SQty,0))>0 ";
            else if (rdoOutStock.Checked)
                strQuery += " and (ISNULL(InQty,0)-ISNULL(SQty,0))<0 ";

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

            return strQuery;
        }

        private void GetDataFromDataBase()
        {
            try
            {
                string strInDateQuery="", strOutDateQuery="",strSubQuery = CreateQuery(ref strInDateQuery, ref strOutDateQuery), strQuery = "",strOrderBy="";
                string strColumnQuery = "", strSupplierOuterApply = "";
                if (chkSupplier.Checked)
                {
                    strColumnQuery = "(PurchasePartyID+' '+Name)PParty";
                    strSupplierOuterApply = " Outer APPLY (Select Name from SupplierMaster SM Where (AreaCode+AccountNo)=PurchasePartyID)SM ";
                }
                if (chkCategory.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Category";
                }
                if (chkGroup.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "GroupName";
                }
                if (chkBrandName.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "BrandName";
                }
                if (chkDepartment.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Department";
                }
                if (chkItemName.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "ItemName";
                }
                if (chkVariant1.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant1";
                }
                if (chkVariant2.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant2";
                }
                if (chkVariant3.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant3";
                }
                if (chkVariant4.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant4";
                }
                if (chkVariant5.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant5";
                }
                
                if (chkRate.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Rate";
                }

                string strGroupBy = "";
                if (strColumnQuery != "")
                {
                    strGroupBy = " Group by " + strColumnQuery.Replace("PParty", "");
                    strOrderBy = " Order by " + strColumnQuery.Replace("PParty", "");
                    strColumnQuery += ",";
                }
                if (MainPage.strCompanyName.Contains("SARAOGI SUPER") && MainPage.strSoftwareType == "AGENT")
                {
                    strQuery = " Select " + strColumnQuery + "SUM(InQty) IQty, SUM(SQty) OQty,SUM(InQty*Rate) NetInAmt,SUM((InQty-SQty)*Rate) NetAmt from ( Select _IM.Other as Category,BrandName,GroupName,Department,_Stock.ItemName,_Stock.Variant1,_Stock.Variant2,_Stock.Variant3,_Stock.Variant4,_Stock.Variant5,InQty,ISNULL((Select SUM(Qty)SQty from StockMaster SM Where SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5  and BillType in ('SALES', 'PURCHASERETURN','STOCKOUT')  " + strOutDateQuery + "),0) SQty,ISNULL(MRP,ISNULL(SMRP,SSMRP))MRP,PB.BillCode,ISNULL(PBillDate,ISNULL(SDate,SSDate)) as PBillDate,PB.PurchasePartyID,ISNULL(Rate,ISNULL(SRate,SSRate)) Rate  from ( "
                            + " Select BarCode,BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(InQty)InQty from( " //, MRP
                            + " Select '' as BarCode,'' as BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('OPENING') " + strInDateQuery + " Group by ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 UNION ALL "
                            + " Select '' BarCode,'' as BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('PURCHASE') " + strInDateQuery + " Group by ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 UNION ALL "
                            + " Select '' BarCode,'' as BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('SALERETURN','STOCKIN') " + strInDateQuery + " Group by ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 "
                            + " )_Stock Group by BarCode,BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 " //, MRP
                            + " )_Stock  OUTER APPLY(Select _IM.Other,GroupName,MakeName Department from Items _IM Where _IM.ItemName = _Stock.ItemName)_IM "
                            + " " //and SM.MRP = _Stock.MRP
                            + " OUTER APPLY(Select Top 1 PB.BillCode,PB.Date as PBillDate, PB.PurchasePartyID, PBS.Rate,PBS.MRP from PurchaseBookSecondary PBS inner join PurchaseBook PB on PBS.BillCode = PB.BillCode and PBS.BillNO = PB.BillNo Where PBS.ItemName = _Stock.ItemName and PBS.Variant1 = _Stock.Variant1 and PBS.Variant2 = _Stock.Variant2  and PBS.Variant3 = _Stock.Variant3  and PBS.Variant4 = _Stock.Variant4  and PBS.Variant5 = _Stock.Variant5 Order by PB.Date desc)PB " //and _Stock.MRP = PBS.MRP
                            + " OUTER APPLY(Select Top 1 SM.Rate as SRate,SM.MRP as SMRP,SM.Date as SDate from StockMaster SM Where BillType in ('SALERETURN', 'OPENING','STOCKIN') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5 and SM.Rate!=0 and ISNULL(PB.Rate,0)=0)_STR OUTER APPLY(Select Top 1 SM.Rate as SSRate,SM.MRP as SSMRP,SM.Date as SSDate from StockMaster SM Where BillType in ('SALES','STOCKOUT') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5 and SM.Rate!=0 and ISNULL(_STR.SRate,0)=0 and ISNULL(PB.Rate,0)=0)_SSales " // and SM.MRP = _Stock.MRP 
                            + "   Where ItemName!='' " + strSubQuery + " )Stock  " + strSupplierOuterApply + " Where (InQty!=0 OR SQty!=0) " + strGroupBy + strOrderBy;

                }
                else
                {
                    strQuery = " Select " + strColumnQuery + "SUM(InQty) IQty, SUM(SQty) OQty,SUM(InQty*Rate) NetInAmt,SUM((InQty-SQty)*Rate) NetAmt from ( Select _IM.Other as Category,BrandName,GroupName,Department,_Stock.ItemName,_Stock.Variant1,_Stock.Variant2,_Stock.Variant3,_Stock.Variant4,_Stock.Variant5,InQty,ISNULL(SQty,0) SQty,ISNULL(MRP,ISNULL(SMRP,SSMRP))MRP,PB.BillCode,ISNULL(PBillDate,ISNULL(SDate,SSDate)) as PBillDate,PB.PurchasePartyID,ISNULL(Rate,ISNULL(SRate,SSRate)) Rate  from ( "
                         + " Select BarCode,BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(InQty)InQty from( " //, MRP
                         + " Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'') BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('OPENING') " + strInDateQuery + " Group by ISNULL(BarCode,''),ISNULL(BrandName,''),ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 UNION ALL "
                         + " Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'') BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('PURCHASE') " + strInDateQuery + " Group by ISNULL(BarCode,''),ISNULL(BrandName,''),ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 UNION ALL "
                         + " Select ISNULL(BarCode,'')BarCode,ISNULL(BrandName,'') BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, SUM(Qty)InQty from StockMaster SM Where BillType in ('SALERETURN') " + strInDateQuery + " Group by ISNULL(BarCode,''),ISNULL(BrandName,''),ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 "
                         + " )_Stock Group by BarCode,BrandName,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5 " //, MRP
                         + " )_Stock  OUTER APPLY(Select _IM.Other,GroupName,MakeName Department from Items _IM Where _IM.ItemName = _Stock.ItemName)_IM "
                         + " OUTER APPLY(Select SUM(Qty)SQty from StockMaster SM Where ISNULL(SM.BrandName,'')= _Stock.BrandName and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5  and BillType in ('SALES', 'PURCHASERETURN')  " + strOutDateQuery + ")_ST " //and SM.MRP = _Stock.MRP
                         + " OUTER APPLY(Select Top 1 PB.BillCode,PB.Date as PBillDate, PB.PurchasePartyID, PBS.Rate,PBS.MRP from PurchaseBookSecondary PBS inner join PurchaseBook PB on PBS.BillCode = PB.BillCode and PBS.BillNO = PB.BillNo Where ISNULL(PBS.BrandName,'') = ISNULL(_Stock.BrandName,'') and ISNULL(PBS.BarCode,'') = ISNULL(_Stock.BarCode,'') and PBS.ItemName = _Stock.ItemName and PBS.Variant1 = _Stock.Variant1 and PBS.Variant2 = _Stock.Variant2 and PBS.Variant3 = _Stock.Variant3 and PBS.Variant4 = _Stock.Variant4 and PBS.Variant5 = _Stock.Variant5 Order by PB.Date desc)PB " //and _Stock.MRP = PBS.MRP
                         + " OUTER APPLY(Select Top 1 SM.Rate as SRate,SM.MRP as SMRP,SM.Date as SDate from StockMaster SM Where BillType in ('SALERETURN', 'OPENING','STOCKIN') and ISNULL(SM.BrandName,'') = ISNULL(_Stock.BrandName,'') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5 and SM.Rate!=0 and ISNULL(PB.Rate,0)=0)_STR OUTER APPLY(Select Top 1 SM.Rate as SSRate,SM.MRP as SSMRP,SM.Date as SSDate from StockMaster SM Where BillType in ('SALES','STOCKOUT') and ISNULL(SM.BarCode,'') = ISNULL(_Stock.BarCode,'') and SM.ItemName = _Stock.ItemName and SM.Variant1 = _Stock.Variant1 and SM.Variant2 = _Stock.Variant2 and SM.Variant3 = _Stock.Variant3 and SM.Variant4 = _Stock.Variant4 and SM.Variant5 = _Stock.Variant5 and SM.Rate!=0 and ISNULL(_STR.SRate,0)=0 and ISNULL(PB.Rate,0)=0)_SSales " // and SM.MRP = _Stock.MRP 
                         + " Where (InQty!=0 OR SQty!=0) " + strSubQuery + " )Stock  " + strSupplierOuterApply + strGroupBy + strOrderBy;
                }
                DataTable table = new DataTable();
              
                if (rdoSTCurrent.Checked)
                    table = dba.GetDataTable(strQuery);
                else if (rdoOldStock.Checked)
                    table = SearchDataOther.GetDataTable_NC(strQuery);
                else
                {
                    if (MainPage._bItemMirroring)
                    {
                        table = dba.GetDatFromAllFirm(strQuery);
                        DataTable _dt = dba.GetDataTable(strQuery);
                        if (_dt.Rows.Count > 0)
                        {
                            if (table.Rows.Count > 0)
                                table.Merge(_dt);
                            else
                                table = _dt;
                        }                      
                    }
                    else
                    {
                        DataTable _dt = dba.GetDataTable(strQuery), __dt = SearchDataOther.GetDataTable_NC(strQuery);
                        if (_dt.Rows.Count > 0 && __dt.Rows.Count > 0)
                        {
                            _dt.Merge(__dt);
                            table = GetMergeDetails(_dt);
                        }
                        else if (__dt.Rows.Count > 0)
                            table = __dt;
                        else
                            table = _dt;
                    }
                }

                BindColumn(table);
                BindDataTable(table);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private string CreateQuery_Retail(ref string strInDateQuery, ref string strOutDateQuery)
        {
            string strQuery = "";

            if (txtGroupName.Text != "")
                strQuery += " and _IM.GroupName in ('" + txtGroupName.Text + "') ";
            if (txtItemCategory.Text != "")
                strQuery += " and _IM.Other in ('" + txtItemCategory.Text + "') ";
            if (txtDepartment.Text != "")
                strQuery += " and Department in ('" + txtDepartment.Text + "') ";
            if (txtSupplierName.Text != "")
            {
                string[] strParty = txtSupplierName.Text.Split(' ');
                if (strParty.Length > 1)
                    strQuery += " and PurchasePartyID in ('" + strParty[0] + "') ";
            }
            if (txtItemName.Text != "")
                strInDateQuery += " and SM.ItemName='" + txtItemName.Text + "' ";

            if (txtCategory1.Text != "")
                strInDateQuery += " and SM.Variant1='" + txtCategory1.Text + "' ";
            if (txtCategory2.Text != "")
                strInDateQuery += " and SM.Variant2='" + txtCategory2.Text + "' ";
            if (txtCategory3.Text != "")
                strInDateQuery += " and SM.Variant3='" + txtCategory3.Text + "' ";
            if (txtCategory4.Text != "")
                strInDateQuery += " and SM.Variant4='" + txtCategory4.Text + "' ";
            if (txtCategory5.Text != "")
                strInDateQuery += " and SM.Variant5='" + txtCategory5.Text + "' ";
            //if (txtBarCode.Text != "")
            //    strInDateQuery += " and SM.BarCode='" + txtBarCode.Text + "' ";

            if (txtBranchCode.Text != "")
            {
                strInDateQuery += " and SM.BillCode Like('%" + txtBranchCode.Text + "%') ";
                strOutDateQuery += " and _SM.BillCode Like('%" + txtBranchCode.Text + "%') ";
            }

            if (txtBarCode.Text != "")
                strQuery += " and SM.BarCode='" + txtBarCode.Text + "' ";

            if (txtBrandName.Text != "")
                strQuery += " and SM.BrandName='" + txtBrandName.Text + "' ";

            if (rdoInStock.Checked)
                strQuery += " and (ISNULL(InQty,0)-ISNULL(OutQty,0))>0 ";
            else if (rdoOutStock.Checked)
                strQuery += " and (ISNULL(InQty,0)-ISNULL(OutQty,0))<0 ";

            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strInDateQuery += " and SM.Date>='" + sDate.ToString("MM/dd/yyyy") + "' and SM.Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }

            if (chkOutDate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtOFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
                eDate = eDate.AddDays(1);
                strOutDateQuery += " and _SM.Date>='" + sDate.ToString("MM/dd/yyyy") + "' and _SM.Date<'" + eDate.ToString("MM/dd/yyyy") + "' ";
            }

            return strQuery;
        }

        private void GetDataFromDataBase_Retail()
        {
            try
            {
                string strInDateQuery="",strOutDateQuery="", strSubQuery = CreateQuery_Retail(ref strInDateQuery,ref strOutDateQuery), strQuery = "", strOrderBy = "";
                string strColumnQuery = "", strSupplierOuterApply = "";
                if (chkSupplier.Checked)
                {
                    strColumnQuery = "(PurchasePartyID+' '+Name)PParty";
                    strSupplierOuterApply = " Outer APPLY (Select Name from SupplierMaster SM Where (AreaCode+AccountNo)=PurchasePartyID)SM ";
                }
                if (chkCategory.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Category";
                }
                if (chkGroup.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "GroupName";
                }
                if (chkBrandName.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "BrandName";
                }
                if (chkDepartment.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Department";
                }
                if (chkItemName.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "ItemName";
                }
                if (chkVariant1.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant1";
                }
                if (chkVariant2.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant2";
                }
                if (chkVariant3.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant3";
                }
                if (chkVariant4.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant4";
                }
                if (chkVariant5.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Variant5";
                }
                if (chkRate.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Abs(Rate)  Rate";
                }
                if (chkBarcode.Checked)
                {
                    if (strColumnQuery != "") { strColumnQuery += ","; }
                    strColumnQuery += "Barcode";
                }
               
                string strGroupBy = "";
                if (strColumnQuery != "")
                {
                    strGroupBy = " Group by " + strColumnQuery.Replace("PParty", "").Replace("  Rate", "");
                    strOrderBy = " Order by " + strColumnQuery.Replace("PParty", "").Replace("  Rate", "");
                    strColumnQuery += ",";
                }
                
                string strBillType = "'OPENING','PURCHASE','STOCKIN','SALERETURN'", strBillType2= "'OPENING','PURCHASE','STOCKIN'";
                if (rdoOpening.Checked)
                {
                    strBillType = strBillType2= "'OPENING'";
                }

                strQuery = " Select " + strColumnQuery + "SUM(InQty) IQty, SUM(SQty) OQty,SUM(InQty*Abs(Rate)) NetInAmt,SUM((InQty-SQty)*Abs(Rate)) NetAmt from ( "
                        + " Select _IM.Other as Category,_IM.GroupName,Department,SM.BrandName,SM.BarCode,SM.ItemName, SM.Variant1, SM.Variant2, SM.Variant3, SM.Variant4, SM.Variant5,INQty,ISNULL(OutQty,0)SQty,(CASE WHEN ISNULL(Purc.AvgRate,0) > 0 then ISNULL(Purc.AvgRate,0) else ISNULL(SM.Rate,0)end)Rate,PurchasePartyID,SMaster.Name as Name from ( "
                        + " Select SM.BrandName,ISNULL(SM.BarCode,'')BarCode,SM.ItemName, SM.Variant1, SM.Variant2, SM.Variant3, SM.Variant4, SM.Variant5, SUM(SM.Qty)INQty,0 Rate from StockMaster SM "
                        + " Where BillType in (" + strBillType + ") " + strInDateQuery + " Group by SM.BrandName,ISNULL(SM.BarCode,''),SM.ItemName, SM.Variant1, SM.Variant2 , SM.Variant3, SM.Variant4, SM.Variant5 "
                        + " UNION ALL SELECT BrandName,ISNULL(BarCode,'')BarCode,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5, 0 INQty, AvgRate FROM(SELECT BrandName,ISNULL(BarCode,'')BarCode,ItemName, Variant1, Variant2, Variant3, Variant4, Variant5,SUM(Qty)TSQty,(SUM(SAmt)/SUM(Qty))AvgRate FROM (Select SO.BrandName, ISNULL(SO.BarCode, '')BarCode, SO.ItemName, SO.Variant1, SO.Variant2, SO.Variant3,SO.Variant4, SO.Variant5,Qty,(Qty)*(Rate)SAmt from StockMaster SO Where SO.BillType in ('SALES', 'PURCHASERETURN', 'STOCKOUT')  " + strInDateQuery.Replace("SM.", "SO.") + " )SOLD Group by SOLD.BrandName,ISNULL(SOLD.BarCode, ''),SOLD.ItemName,SOLD.Variant1,SOLD.Variant2,SOLD.Variant3,SOLD.Variant4,SOLD.Variant5 )OUTS WHERE (Select COUNT(*) from StockMaster ISM Where BillType in (" + strBillType + ") AND ISM.BrandName = OUTS.BrandName  and ISNULL(ISM.BarCode,'')= ISNULL(OUTS.BarCode,'') and ISM.ItemName = OUTS.ItemName and ISM.Variant1 = OUTS.Variant1 and ISM.Variant2 = OUTS.Variant2 and ISM.Variant3 = OUTS.Variant3 and ISM.Variant4 = OUTS.Variant4 and ISM.Variant5 = OUTS.Variant5) = 0 "
                        + " )SM "

                        + " LEFT JOIN (SELECT ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5 ORDER BY ID ASC) AS RNo, BrandName,BarCode,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,SUM(PQty)TPQty,CAST((SUM(PAmt)/SUM(PQty)) as Numeric(18,4))AvgRate FROM ("
                        + " SELECT "+ ((MainPage.strStockAsPer == "DesignMaster") ? "0" : "2") + " ID,IM.BrandName,Description BarCode, IM.ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,1 PQty,IMS.PurchaseRate PAmt FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE IMS.PurchaseRate != 0 " + strInDateQuery.Replace("SM.Variant", "IMS.Variant").Replace("SM.", "IM.")
                        + " UNION ALL SELECT " + ((MainPage.strStockAsPer == "DesignMaster") ? "1" : "0") + " ID ,BrandName,BarCode,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,(CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then (SELECT NetRate FROM GetTaxRate(SM.ItemName,SM.MRP,SM.Rate)) else isnull(SM.Rate,0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE' WHERE BillType IN (" + strBillType2 + ")" + strInDateQuery
                        + " UNION ALL SELECT " + ((MainPage.strStockAsPer == "DesignMaster") ? "2" : "1") + " ID ,BrandName,BarCode,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,(CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then (SELECT NetRate FROM GetTaxRate(SM.ItemName,SM.MRP,SM.Rate)) else isnull(SM.Rate,0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE'	WHERE BillType IN ('SALERETURN') " + strInDateQuery

                        + " )Ratt GROUP BY BrandName,BarCode,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,ID )Purc on SM.BrandName = Purc.BrandName and ISNULL(SM.BarCode,'') = ISNULL(Purc.BarCode,'') and SM.ItemName = Purc.ItemName and SM.Variant1 = Purc.Variant1 and SM.Variant2 = Purc.Variant2 AND RNo = 1 "

                        + " left join (Select _SM.BrandName,ISNULL(_SM.BarCode,'')BarCode,_SM.ItemName, _SM.Variant1,_SM.Variant2, _SM.Variant3, _SM.Variant4, _SM.Variant5,SUM(_SM.Qty)OutQty from StockMaster _SM "
                        + " Where _SM.BillType in ('SALES','PURCHASERETURN','STOCKOUT') " + strOutDateQuery + " Group by _SM.BrandName,ISNULL(_SM.BarCode,''),_SM.ItemName,_SM.Variant1,_SM.Variant2,_SM.Variant3,_SM.Variant4,_SM.Variant5 "
                        + " )_SM on SM.BrandName=_SM.BrandName and ISNULL(SM.BarCode,'')=ISNULL(_SM.BarCode,'') and SM.ItemName=_SM.ItemName and SM.Variant1=_SM.Variant1 and SM.Variant2=_SM.Variant2 and SM.Variant3=_SM.Variant3 and SM.Variant4=_SM.Variant4 and SM.Variant5=_SM.Variant5 "
                        + " left join (Select _IM.Other,MakeName Department,GroupName,ItemName,ROW_NUMBER() OVER (PARTITION BY ItemName Order by ItemName) RINumber from  Items _IM)_IM on SM.ItemName=_IM.ItemName and RINumber=1  "
                        + " left join (Select *,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5 ORDER BY Date DESC) AS RNumber from (SELECT PurchasePartyID,BarCode,BrandName,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,Date FROM PurchaseBook PB inner join PurchaseBookSecondary PBS on PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo UNION ALL  SELECT PurchasePartyID,PBS.Description as BarCode,ISNULL(PBS.Brand,BrandName)BrandName,ItemName,Variant1,Variant2, Variant3, Variant4, Variant5,Date FROM Items PB inner join ItemSecondary PBS on PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo)_PBS)PBS on SM.BrandName=PBS.BrandName and SM.BarCode=PBS.BarCode and SM.ItemName=PBS.ItemName and SM.Variant1=PBS.Variant1 and SM.Variant2=PBS.Variant2 and SM.Variant3=PBS.Variant3 and SM.Variant4=PBS.Variant4 and SM.Variant5=PBS.Variant5 and RNumber=1 "
                        + " left join SupplierMaster SMaster on SMaster.AreaCode+SMaster.AccountnO=PurchasePartyID "
                        + " Where (InQty!=0 OR ISNULL(OutQty,0)!=0) " + strSubQuery + " )Stock  " + strGroupBy + strOrderBy;

                DataTable table = new DataTable();
                if (rdoSTCurrent.Checked)
                    table = dba.GetDataTable(strQuery);
                else if (rdoOldStock.Checked)
                    table = SearchDataOther.GetDataTable_NC(strQuery);
                else
                {
                    DataTable _dt = dba.GetDataTable(strQuery), __dt = SearchDataOther.GetDataTable_NC(strQuery);// SearchDataOther.GetDataTable_NC(strQuery);
                    if (_dt.Rows.Count > 0 && __dt.Rows.Count > 0)
                    {
                        _dt.Merge(__dt);
                        table = GetMergeDetails(_dt);
                    }
                    else if (__dt.Rows.Count > 0)
                        table = __dt;
                    else
                        table = _dt;
                }

                BindColumn(table);
                BindDataTable(table);
                SetColounCategory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message);
            }
        }

        private DataTable GetMergeDetails(DataTable dt)
        {
            string strColumnName =GetColumnName(), strColumnQuery = "" ;
            string[] _strColumnName = strColumnName.Split(',');
            DataTable _dt = null;
            if (strColumnName != "")
                _dt = dt.DefaultView.ToTable(true, _strColumnName);
            else
            {
                _dt = new DataTable();               
            }
                _dt.Columns.Add("IQty", typeof(Double));
            _dt.Columns.Add("OQty", typeof(Double));
            _dt.Columns.Add("NetInAmt", typeof(Double));
            _dt.Columns.Add("NetAmt", typeof(Double));
            object objInQty = "", objOutQty = "", objNetInAmt = "", objNetAmt = "";
            if (_dt.Rows.Count > 0)
            {
                foreach (DataRow dr in _dt.Rows)
                {
                    strColumnQuery = GetColumnQuery(dr);

                    objInQty = dt.Compute("Sum(IQty)", strColumnQuery);
                    objOutQty = dt.Compute("Sum(OQty)", strColumnQuery);
                    objNetInAmt = dt.Compute("Sum(NetInAmt)", strColumnQuery);
                    objNetAmt = dt.Compute("Sum(NetAmt)", strColumnQuery);

                    dr["IQty"] = objInQty;
                    dr["OQty"] = objOutQty;
                    dr["NetInAmt"] = objNetInAmt;
                    dr["NetAmt"] = objNetAmt;
                }
            }
            else
            {
                DataRow dr = _dt.NewRow();

                objInQty = dt.Compute("Sum(IQty)", strColumnQuery);
                objOutQty = dt.Compute("Sum(OQty)", strColumnQuery);
                objNetInAmt = dt.Compute("Sum(NetInAmt)", strColumnQuery);
                objNetAmt = dt.Compute("Sum(NetAmt)", strColumnQuery);

                dr["IQty"] = objInQty;
                dr["OQty"] = objOutQty;
                dr["NetInAmt"] = objNetInAmt;
                dr["NetAmt"] = objNetAmt;
                _dt.Rows.Add(dr);
            }

            return _dt;           
        } 

        private string GetColumnQuery(DataRow dr)
        {
            string strColumnQuery = "";
            if (chkSupplier.Checked)
                strColumnQuery = "ISNULL(PParty,'')='" + Convert.ToString(dr["PParty"]) + "' ";

            if (chkCategory.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(Category,'')='" + Convert.ToString(dr["Category"]) + "' ";
            }
            if (chkGroup.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(GroupName,'')='" + Convert.ToString(dr["GroupName"]) + "' ";
            }
            if (chkBrandName.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(BrandName,'')='" + Convert.ToString(dr["BrandName"]) + "' ";
            }
            if (chkDepartment.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(Department,'')='" + Convert.ToString(dr["Department"]) + "' ";
            }
            if (chkItemName.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(ItemName,'')='" + Convert.ToString(dr["ItemName"]) + "' ";
            }
            if (chkVariant1.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(Variant1,'')='" + Convert.ToString(dr["Variant1"]) + "' ";
            }
            if (chkVariant2.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(Variant2,'')='" + Convert.ToString(dr["Variant2"]) + "' ";
            }
            if (chkRate.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += " and "; }
                strColumnQuery += "ISNULL(Rate,0)='" + Convert.ToString(dr["Rate"]) + "' ";
            }

            return strColumnQuery;
        }
              
        private string GetColumnName()
        {
            string strColumnQuery = "";
            if (chkSupplier.Checked)
                strColumnQuery = "PParty";
            if (chkCategory.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "Category";
            }
            if (chkGroup.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "GroupName";
            }
            if (chkBrandName.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "BrandName";
            }
            if (chkDepartment.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "Department";
            }
            if (chkItemName.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "ItemName";
            }
            if (chkVariant1.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "Variant1";
            }
            if (chkVariant2.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "Variant2";
            }
            if (chkRate.Checked)
            {
                if (strColumnQuery != "") { strColumnQuery += ","; }
                strColumnQuery += "Rate";
            }
            return strColumnQuery;
        }

        private void BindColumn(DataTable _dt)
        {
            BindedDT = _dt.Clone();
            BindedDT = _dt;

            dgrdDetails.Columns.Clear();

            CreateGridviewColumn(dgrdDetails, "sno", "S.No", "RIGHT", 50);
            if (chkSupplier.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "PParty", "Supplier", "LEFT", 160);
            if (chkCategory.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "Category", "Category", "LEFT", 130);
            if (chkBrandName.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "BrandName", "Brand Name", "LEFT", 130);
            if (chkGroup.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "GroupName", "GroupName", "LEFT", 130);
            if (chkDepartment.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "Department", "Department", "LEFT", 130);
            if (chkItemName.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "ItemName", "ItemName", "LEFT", 160);
            if (chkBarcode.Checked)
                CreateGridviewLinkColumn(dgrdDetails, "Barcode", "Barcode", "LEFT", 120);

            if (MainPage.StrCategory1 != "")
            {
                if (chkVariant1.Checked)
                    CreateGridviewColumn(dgrdDetails, "Variant1", MainPage.StrCategory1, "LEFT", 80);
            }
            if (MainPage.StrCategory2 != "")
            {
                if (chkVariant2.Checked)
                    CreateGridviewColumn(dgrdDetails, "Variant2", MainPage.StrCategory2, "LEFT", 80);
            }
            if (MainPage.StrCategory3 != "")
            {
                if (chkVariant3.Checked)
                    CreateGridviewColumn(dgrdDetails, "Variant3", MainPage.StrCategory3, "LEFT", 80);
            }
            if (MainPage.StrCategory4 != "")
            {
                if (chkVariant4.Checked)
                    CreateGridviewColumn(dgrdDetails, "Variant4", MainPage.StrCategory4, "LEFT", 80);
            }
            if (MainPage.StrCategory5 != "")
            {
                if (chkVariant5.Checked)
                    CreateGridviewColumn(dgrdDetails, "Variant5", MainPage.StrCategory5, "LEFT", 80);
            }

            CreateGridviewColumn(dgrdDetails, "IQty", "In Qty", "RIGHT", 120);
            CreateGridviewColumn(dgrdDetails, "OQty", "Out Qty", "RIGHT", 120);
            CreateGridviewColumn(dgrdDetails, "NetQTY", "Net Qty", "RIGHT", 120);
            if (chkRate.Checked)
                CreateGridviewColumn(dgrdDetails, "Rate", "Rate", "RIGHT", 100);
            CreateGridviewColumn(dgrdDetails,"NetAmt", "Net Amt", "RIGHT", 120);

        }

        private void CreateGridviewColumn(DataGridView dgrd, string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
                _column.Name = strColName;
                _column.HeaderText = strColHeader;
                _column.Width = _width;
                _column.SortMode = DataGridViewColumnSortMode.Automatic;
                if (strAlign == "LEFT")
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                }
                else
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
                    if (_width != 50)
                        _column.DefaultCellStyle.Format = "N2";
                }
                _column.CellTemplate = dataGridViewCell;
                dgrd.Columns.Add(_column);
            }
            catch { }
        }

        private void CreateGridviewLinkColumn(DataGridView dgrd, string strColName, string strColHeader, string strAlign, int _width)
        {
            try
            {
                DataGridViewColumn _column = new DataGridViewColumn();
                DataGridViewLinkCell dataGridViewCell = new DataGridViewLinkCell();

                _column.Name = strColName;
                _column.HeaderText = strColHeader;
                _column.Width = _width;
                _column.SortMode = DataGridViewColumnSortMode.Automatic;
                if (strAlign == "LEFT")
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Regular);
                    
                }
                else
                {
                    _column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    _column.DefaultCellStyle.Font = new Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
                    if (_width != 50)
                        _column.DefaultCellStyle.Format = "N2";
                }
                dataGridViewCell.LinkColor = Color.Black;
                _column.CellTemplate = dataGridViewCell;
                dgrd.Columns.Add(_column);
            }
            catch { }
        }

        private void BindDataTable(DataTable table)
        {
            dgrdDetails.Rows.Clear();
            if (table.Rows.Count > 0)
                dgrdDetails.Rows.Add(table.Rows.Count);

            int _rowIndex = 0;
            double dIQty = 0, dOQty = 0,dNetQty=0, dNetAmt = 0, dTIQty = 0, dTOQty = 0, dNetTQty = 0, dNetTAmt = 0,dNetInAmt=0;
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    dgrdDetails.Rows[_rowIndex].Cells["sno"].Value = (_rowIndex + 1);
                    if (chkSupplier.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["PParty"].Value = row["PParty"];
                    if (chkCategory.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Category"].Value = row["Category"];
                    if (chkBrandName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["BrandName"].Value = row["BrandName"];
                    if (chkGroup.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["GroupName"].Value = row["GroupName"];
                    if (chkDepartment.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Department"].Value = row["Department"];

                    if (chkItemName.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["ItemName"].Value = row["ItemName"];
                    if (chkBarcode.Checked && table.Columns.Contains("Barcode"))
                        dgrdDetails.Rows[_rowIndex].Cells["Barcode"].Value = row["Barcode"];
                    
                    if (MainPage.StrCategory1!="" && chkVariant1.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Variant1"].Value = row["Variant1"];
                    if (MainPage.StrCategory2 != "" && chkVariant2.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Variant2"].Value = row["Variant2"];
                    if (chkRate.Checked)
                        dgrdDetails.Rows[_rowIndex].Cells["Rate"].Value = row["Rate"];

                    dTIQty += dIQty = dba.ConvertObjectToDouble(row["IQty"]);
                    dTOQty += dOQty = dba.ConvertObjectToDouble(row["OQty"]);
                    dNetTQty += dNetQty = dIQty - dOQty;
                    dNetTAmt += dNetAmt = dba.ConvertObjectToDouble(row["NetAmt"]);
                    dNetInAmt += dba.ConvertObjectToDouble(row["NetInAmt"]);

                    dgrdDetails.Rows[_rowIndex].Cells["IQty"].Value = dIQty;
                    dgrdDetails.Rows[_rowIndex].Cells["OQty"].Value = dOQty;
                    dgrdDetails.Rows[_rowIndex].Cells["NetQTY"].Value = dNetQty;
                    dgrdDetails.Rows[_rowIndex].Cells["NetAmt"].Value = dNetAmt;
                    _rowIndex++;
                }
            }
            catch(Exception ex) { MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            lblInQty.Text = dTIQty.ToString("N2",MainPage.indianCurancy);
            lblOutQty.Text = dTOQty.ToString("N2", MainPage.indianCurancy);
            lblNetQty.Text = dNetTQty.ToString("N2", MainPage.indianCurancy);
            lblNetAmt.Text = dNetTAmt.ToString("N2", MainPage.indianCurancy);
            lblIAmount.Text = dNetInAmt.ToString("N2", MainPage.indianCurancy);
        }


        private void btnGO_Click(object sender, EventArgs e)
        {
            btnGO.Enabled = false;
            if (MainPage.strSoftwareType == "RETAIL" || MainPage._bCustomPurchase)
                GetDataFromDataBase_Retail();
            else
                GetDataFromDataBase();

            pnlSearch.Visible = false;
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
                    //DataTable dt = CreateDataTable();
                    //if (dt.Rows.Count > 0)
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
                        saveFileDialog.FileName = "Stock_Register";
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
                    //else
                    //    MessageBox.Show("Sorry ! Please select atleast one record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            
        }

        private void ShowMonthlyDetails(int _rowIndex)
        {
            DateTime _fromInDate = MainPage.startFinDate, _fromOutDate = MainPage.startFinDate, _toInDate = MainPage.endFinDate, _toOutDate = MainPage.endFinDate;
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                _fromInDate = dba.ConvertDateInExactFormat(txtFromDate.Text);
                _toInDate = dba.ConvertDateInExactFormat(txtToDate.Text);
            }
            if (chkOutDate.Checked && txtOFromDate.Text.Length == 10 && txtOToDate.Text.Length == 10)
            {
                _fromOutDate = dba.ConvertDateInExactFormat(txtOFromDate.Text);
                _toOutDate = dba.ConvertDateInExactFormat(txtOToDate.Text);
            }
            SetGridFilter(dgrdDetails.Rows[_rowIndex]);
            MonthlyStockRegister objMonthly = new MonthlyStockRegister(dgrdDetails.Rows[_rowIndex], true, _fromInDate, _toInDate, _fromOutDate, _toOutDate); //rdoDetail.Checked
            objMonthly.chkDate.Checked = chkDate.Checked;
            objMonthly.chkOutDate.Checked = chkOutDate.Checked;
            objMonthly.txtFromDate.Text = txtFromDate.Text;
            objMonthly.txtToDate.Text = txtToDate.Text;
            objMonthly.txtOFromDate.Text = txtOFromDate.Text;
            objMonthly.txtOToDate.Text = txtOToDate.Text;

            objMonthly.MdiParent = MainPage.mymainObject;
            objMonthly.Show();
        }

        private void SetGridFilter(DataGridViewRow row)
        {
            if (!chkSupplier.Checked && txtSupplierName.Text != "" && !dgrdDetails.Columns.Contains("PParty"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "PParty", "Supplier", "LEFT", 130);
                row.Cells["PParty"].Value = txtSupplierName.Text;
            }
            if (!chkCategory.Checked && txtItemCategory.Text != "" && !dgrdDetails.Columns.Contains("Category"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "Category", "Category", "LEFT", 100);
                row.Cells["Category"].Value = txtItemCategory.Text;
            }
            if (!chkBrandName.Checked && txtBrandName.Text != "" && !dgrdDetails.Columns.Contains("BrandName"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "BrandName", "Brand Name", "LEFT", 100);
                row.Cells["BrandName"].Value = txtBrandName.Text;
            }
            if (!chkGroup.Checked && txtGroupName.Text != "" && !dgrdDetails.Columns.Contains("GroupName"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "GroupName", "GroupName", "LEFT", 100);
                row.Cells["GroupName"].Value = txtGroupName.Text;
            }
            if (!chkItemName.Checked && txtItemName.Text != "" && !dgrdDetails.Columns.Contains("ItemName"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "ItemName", "ItemName", "LEFT", 140);
                row.Cells["ItemName"].Value = txtItemName.Text;
            }
            if (!chkBarcode.Checked && txtBarCode.Text != "" && !dgrdDetails.Columns.Contains("Barcode"))
            {
                CreateGridviewLinkColumn(dgrdDetails, "Barcode", "Barcode", "LEFT", 120);
                row.Cells["Barcode"].Value = txtBarCode.Text;
            }
                if (MainPage.StrCategory1 != "" && !chkVariant1.Checked && txtCategory1.Text != "" && !dgrdDetails.Columns.Contains("Variant1"))
            {
                CreateGridviewColumn(dgrdDetails, "Variant1", MainPage.StrCategory1, "LEFT", 80);
                row.Cells["Variant1"].Value = txtCategory1.Text;
            }
            if (MainPage.StrCategory2 != "" && !chkVariant2.Checked && txtCategory2.Text != "" && !dgrdDetails.Columns.Contains("Variant2"))
            {
                    CreateGridviewColumn(dgrdDetails, "Variant2", MainPage.StrCategory2, "LEFT", 80);
                row.Cells["Variant2"].Value = txtCategory2.Text;
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
                        ShowMonthlyDetails(rowIndex);
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
            dba.GetDateInExactFormat(sender, chkDate.Checked, false, true);
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

        private void txtSupplierName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("PURCHASEPARTY", "SEARCH SUNDRY CREDITOR", e.KeyCode);
                    objSearch.ShowDialog();
                    txtSupplierName.Text = objSearch.strSelectedData;                   
                }
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

        private void dgrdDetails_Sorted(object sender, EventArgs e)
        {
            try
            {
                int _index = 0;
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                    row.Cells["sno"].Value = _index++;

            }
            catch { }
        }

        private void StockRegister_Load(object sender, EventArgs e)
        {
            try
            {
                btnExport.Enabled = MainPage.mymainObject.bExport;
                if (MainPage.mymainObject.bSaleReport && MainPage.mymainObject.bPurchaseReport)
                {
                    dba.EnableCopyOnClipBoard(dgrdDetails);
                    if (MainPage._bCustomPurchase || MainPage._bTaxStatus)
                    {
                        rdoSTCurrent.Checked = true;
                        rdoOldStock.Enabled = rdoSTAll.Enabled = false;
                    }
                    if (bShowRecord)
                        btnGO.PerformClick();
                }
                else
                {
                    MessageBox.Show("Sorry ! You don't have permission !! ", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
            }
            catch { }
            
        }

        private void rdoInStock_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoInStock.Checked)
            {
                dgrdDetails.Rows.Clear();
                lblInQty.Text =  lblOutQty.Text = lblNetQty.Text = lblNetAmt.Text = lblIAmount.Text = "0.00";
            }
        }

        private void rdoOutStock_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOutStock.Checked)
            {
                dgrdDetails.Rows.Clear();
                lblInQty.Text = lblOutQty.Text = lblNetQty.Text = lblNetAmt.Text = lblIAmount.Text = "0.00";
            }
        }

        private void rdoOpening_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoOpening.Checked)
            {
                dgrdDetails.Rows.Clear();
                lblInQty.Text = lblOutQty.Text = lblNetQty.Text = lblNetAmt.Text = lblIAmount.Text = "0.00";
            }
        }

        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAll.Checked)
            {
                dgrdDetails.Rows.Clear();
                lblInQty.Text = lblOutQty.Text = lblNetQty.Text = lblNetAmt.Text = lblIAmount.Text = "0.00";
            }
        }

        private void chkOutDate_CheckedChanged(object sender, EventArgs e)
        {
            txtOFromDate.ReadOnly = txtOToDate.ReadOnly = !chkOutDate.Checked;
            txtOFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtOToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (Control ctrl in chkGroup1.Controls)
                {
                    if (ctrl is CheckBox)
                    {
                        ((CheckBox)ctrl).Checked = chkAll.Checked;
                    }
                }

            }
            catch { }
        }

        private void txtOFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, chkOutDate.Checked, false, true);
        }

        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("BARCODEDETAILS", "SEARCH BARCODE NO", e.KeyCode);
                    objSearch.ShowDialog();
                    txtBarCode.Text = objSearch.strSelectedData;
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

        private void dgrdDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex > 0 && e.RowIndex >= 0)
                {

                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        string strColumnName = dgrdDetails.Columns[e.ColumnIndex].Name;
                        if (strColumnName == "ItemName")
                        {
                            string strDesignName = Convert.ToString(dgrdDetails.CurrentCell.Value);
                            if (strDesignName != "")
                            {
                                DesignMaster objDesign = new DesignMaster(strDesignName);
                                objDesign.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                                objDesign.ShowInTaskbar = true;
                                objDesign.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        ShowMonthlyDetails(e.RowIndex);
                    }
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

        private void txtCategory3_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory3);
        }

        private void txtCategory4_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory4);
        }

        private void txtCategory5_KeyDown(object sender, KeyEventArgs e)
        {
            SelectVariants(sender, e, MainPage.StrCategory5);
        }

        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = true;
        }

        private void btnSearch2_Click(object sender, EventArgs e)
        {
            btnGO.PerformClick();
            pnlSearch.Visible = false;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            txtCategory1.Text = txtCategory2.Text = txtCategory3.Text = txtCategory4.Text = txtCategory5.Text = "";
            pnlSearch.Visible = false;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(false);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPreview.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            try
            {
                if (dgrdDetails.Rows.Count > 0)
                    PrintPreviewReport(true);
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Preview in Custom Purchase Report", ex.Message };
                dba.CreateErrorReports(strReport);
            }
            btnPrint.Enabled = true;
        }
        private void PrintPreviewReport(bool bPrint)
        {
            System.Drawing.Printing.PrinterSettings defS = new System.Drawing.Printing.PrinterSettings();
            defS.Copies = 1;
            defS.Collate = false;
            defS.FromPage = 0;
            defS.ToPage = 0;
            CrystalDecisions.CrystalReports.Engine.ReportClass objSalesManReport = null;

            if (arrPrint.Count <= 3)
                objSalesManReport = new Reporting.CustomSalesReport();
            else
                objSalesManReport = new Reporting.CustomSalesReport_LandScape();

            objSalesManReport.SetDataSource(CreatePrintDataTable());
            if (bPrint)
            {
                if (MainPage._PrintWithDialog)
                    dba.PrintWithDialog(objSalesManReport, false);
                else
                    objSalesManReport.PrintToPrinter(defS, defS.DefaultPageSettings, false);
            }
            else
            {
                Reporting.ShowReport objReport = new Reporting.ShowReport("CUSTOM PURCHASE REPORT PREVIEW");
                objReport.myPreview.ReportSource = objSalesManReport;
                objReport.ShowDialog();

            }
            objSalesManReport.Close();
            objSalesManReport.Dispose();
        }
        private DataTable GetTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("CompanyName", typeof(string));
            _dt.Columns.Add("HeaderName", typeof(string));
            _dt.Columns.Add("CustomerName", typeof(string));
            _dt.Columns.Add("FromDate", typeof(string));
            _dt.Columns.Add("ToDate", typeof(string));

            _dt.Columns.Add("lblClm1", typeof(string));
            _dt.Columns.Add("Clm1", typeof(string));
            _dt.Columns.Add("lblClm2", typeof(string));
            _dt.Columns.Add("Clm2", typeof(string));
            _dt.Columns.Add("lblClm3", typeof(string));
            _dt.Columns.Add("Clm3", typeof(string));
            _dt.Columns.Add("lblClm4", typeof(string));
            _dt.Columns.Add("Clm4", typeof(string));
            _dt.Columns.Add("lblClm5", typeof(string));
            _dt.Columns.Add("Clm5", typeof(string));
            _dt.Columns.Add("lblClm6", typeof(string));
            _dt.Columns.Add("Clm6", typeof(string));
            _dt.Columns.Add("lblClm7", typeof(string));
            _dt.Columns.Add("Clm7", typeof(string));
            _dt.Columns.Add("lblClm8", typeof(string));
            _dt.Columns.Add("Clm8", typeof(string));
            _dt.Columns.Add("lblClm9", typeof(string));
            _dt.Columns.Add("Clm9", typeof(string));
            _dt.Columns.Add("lblClm10", typeof(string));
            _dt.Columns.Add("Clm10", typeof(string));

            _dt.Columns.Add("SNo", typeof(string));
            _dt.Columns.Add("TQty", typeof(string));
            _dt.Columns.Add("TGross", typeof(string));
            _dt.Columns.Add("TTaxable", typeof(string));
            _dt.Columns.Add("TTax", typeof(string));
            _dt.Columns.Add("TNet", typeof(string));
            _dt.Columns.Add("UserName", typeof(string));
            return _dt;
        }

        private void getClmNames(int index, ref string DTClmName, ref string RptDispClm)
        {
            string chkName = "";
            if (index < arrPrint.Count)
                chkName = arrPrint[index].Name;

            //if((arrPrint.Count <= 6 && index == arrPrint.Count) || (arrPrint.Count > 6 && index == arrPrint.Count) || (arrPrint.Count > 9 && index == 9))
            //{
            //    RptDispClm = "Net Amt";
            //    DTClmName = "NetAmt";
            //    return;
            //}
            if (chkName != "")
            {
                switch (chkName)
                {
                    case "chkSupplier":
                        RptDispClm = "Supplier"; DTClmName = "PParty"; break;
                    case "chkCategory":
                        RptDispClm = "Category"; DTClmName = "Category"; break;
                    case "chkBrandName":
                        RptDispClm = "BrandName"; DTClmName = "BrandName"; break;
                    case "chkItemName":
                        RptDispClm = "ItemName"; DTClmName = "ItemName"; break;
                    case "chkGroup":
                        RptDispClm = "Group"; DTClmName = "GroupName"; break;
                    case "chkBarcode":
                        RptDispClm = "Barcode"; DTClmName = "Barcode"; break;
                    case "chkDepartment":
                        RptDispClm = "Department"; DTClmName = "Department"; break;
                    case "chkRate":
                        RptDispClm = "Rate"; DTClmName = "Rate"; break;
                    case "chkVariant1":
                        if (chkVariant1.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory1; DTClmName = "Variant1";
                        }
                        break;
                    case "chkVariant2":
                        if (chkVariant1.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory2; DTClmName = "Variant2";
                        }
                        break;
                    case "chkVariant3":
                        if (chkVariant3.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory3; DTClmName = "Variant3";
                        }
                        break;
                    case "chkVariant4":
                        if (chkVariant4.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory4; DTClmName = "Variant4";
                        }
                        break;
                    case "chkVariant5":
                        if (chkVariant5.Enabled)
                        {
                            RptDispClm = MainPage.StrCategory5; DTClmName = "Variant5";
                        }
                        break;
                }
            }
            else
            {
                RptDispClm = "";
                DTClmName = "";
            }
            if (index == arrPrint.Count )
            {
                DTClmName = "IQty"; RptDispClm = "In Qty";
            }
            if (index == arrPrint.Count + 1)
            {
                DTClmName = "OQty"; RptDispClm = "Out Qty";
            }
            if (index == arrPrint.Count + 2)
            {
                DTClmName = "NetInAmt"; RptDispClm = "Net Qty";
            }
            if (index == arrPrint.Count + 3)
            {
                DTClmName = "NetAmt"; RptDispClm = "Net Amt";
            }
        }
        private string getDateString(DataRow dr, string clm)
        {
            if (Convert.ToString(dr[clm]) != "")
            {
                if (clm.Contains("Qty") || clm.Contains("Amt"))
                    return dba.ConvertObjectToDouble(dr[clm]).ToString("N2",MainPage.indianCurancy);
                else
                    return Convert.ToString(dr[clm]);
            }
            return "";
        }
        private DataTable CreatePrintDataTable()
        {
            DataTable _dt = GetTable();
            //if (arrPrint.Count > 0)
            //{
                string DTClmName1 = "", RptDispClm1 = "";
                getClmNames(0, ref DTClmName1, ref RptDispClm1);
                string DTClmName2 = "", RptDispClm2 = "";
                getClmNames(1, ref DTClmName2, ref RptDispClm2);
                string DTClmName3 = "", RptDispClm3 = "";
                getClmNames(2, ref DTClmName3, ref RptDispClm3);
                string DTClmName4 = "", RptDispClm4 = "";
                getClmNames(3, ref DTClmName4, ref RptDispClm4);
                string DTClmName5 = "", RptDispClm5 = "";
                getClmNames(4, ref DTClmName5, ref RptDispClm5);
                string DTClmName6 = "", RptDispClm6 = "";
                getClmNames(5, ref DTClmName6, ref RptDispClm6);
                string DTClmName7 = "", RptDispClm7 = "";
                getClmNames(6, ref DTClmName7, ref RptDispClm7);
                string DTClmName8 = "", RptDispClm8 = "";
                getClmNames(7, ref DTClmName8, ref RptDispClm8);
                string DTClmName9 = "", RptDispClm9 = "";
                getClmNames(8, ref DTClmName9, ref RptDispClm9);
                string DTClmName10 = "", RptDispClm10 = "";
                getClmNames(9, ref DTClmName10, ref RptDispClm10);

                int index = 0;
                foreach (DataRow dr in BindedDT.Rows)
                {
                    DataRow _row = _dt.NewRow();
                    _row["SNo"] = index = index + 1;
                    _row["CompanyName"] = MainPage.strPrintComapanyName;

                    if (DTClmName1 != "")
                    {
                        _row["lblClm1"] = RptDispClm1;
                        _row["Clm1"] = getDateString(dr, DTClmName1);
                    }
                    if (DTClmName2 != "")
                    {
                        _row["lblClm2"] = RptDispClm2;
                        _row["Clm2"] = getDateString(dr, DTClmName2);
                    }
                    if (DTClmName3 != "")
                    {
                        _row["lblClm3"] = RptDispClm3;
                        _row["Clm3"] = getDateString(dr, DTClmName3);
                    }
                    if (DTClmName4 != "")
                    {
                        _row["lblClm4"] = RptDispClm4;
                        _row["Clm4"] = getDateString(dr, DTClmName4);
                    }
                    if (DTClmName5 != "")
                    {
                        _row["lblClm5"] = RptDispClm5;
                        _row["Clm5"] = getDateString(dr, DTClmName5);
                    }
                    if (DTClmName6 != "")
                    {
                        _row["lblClm6"] = RptDispClm6;
                        _row["Clm6"] = getDateString(dr, DTClmName6);
                    }
                    if (DTClmName7 != "")
                    {
                        _row["lblClm7"] = RptDispClm7;
                        _row["Clm7"] = getDateString(dr, DTClmName7);
                    }
                    if (DTClmName8 != "")
                    {
                        _row["lblClm8"] = RptDispClm8;
                        _row["Clm8"] = getDateString(dr, DTClmName8);
                    }
                    if (DTClmName9 != "")
                    {
                        _row["lblClm9"] = RptDispClm9;
                        _row["Clm9"] = getDateString(dr, DTClmName9);
                    }
                    if (DTClmName10 != "")
                    {
                        _row["lblClm10"] = RptDispClm10;
                        _row["Clm10"] = getDateString(dr, DTClmName10);
                    }

                    _row["TQty"] = "Net Qty : " + lblNetQty.Text;
                    _row["TGross"] = "Net Amt : " + lblNetAmt.Text;
                    _row["TTaxable"] = "In Amt : " + lblIAmount.Text; 
                    _row["TTax"] = "Out Amt : " + lblOutAmt.Text;
                    _row["TNet"] = "Net Amt : " + lblNetAmt.Text;
                    if(txtItemName.Text != "")
                        _row["CustomerName"] = "Item Name : " + txtItemName.Text;

                    _row["HeaderName"] = "Stock Report";

                    if (txtFromDate.Text.Length == 10)
                    {
                        _row["FromDate"] = txtFromDate.Text;
                        _row["ToDate"] = "  To  " + txtToDate.Text;
                    }

                    _row["UserName"] = "Printed By : " + MainPage.strLoginName;

                    _dt.Rows.Add(_row);
                }
            //}
            return _dt;
        }

        private void chkSupplier_CheckedChanged(object sender, EventArgs e)
        {
            SetColumnsIndex(sender);
        }
        private void SetColumnsIndex(object sender)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                arrPrint.Add(chk);
            else
                arrPrint.Remove(chk);
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            btnSetting.Enabled = false;
            try
            {
                DialogResult result = MessageBox.Show("Are you sure you want to update settings ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    pnlSearch.Visible = false;
                    UpdateSetting("STOCKREGISTER");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            btnSetting.Enabled = true;
        }

        private void UpdateSetting(string BillType)
        {
            string strQuery = "", clmName = "";
            int showHide = 0;
            foreach (Control ctrl in chkGroup1.Controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox chk = (CheckBox)ctrl;
                    showHide = chk.Checked ? 1 : 0;
                    clmName = chk.Name.Substring(3, chk.Name.Length - 3);
                    strQuery += " Update CustomReportSetting set ShowHide = " + showHide + ", UpdateStatus = 1, UpdatedBy = '" + MainPage.strLoginName + "' WHERE BillType = '" + BillType + "' AND ColumnName = '" + clmName + "'"
                                    + " INSERT INTO CustomReportSetting(BillType, ColumnName, ShowHide, InsertStatus, CreatedBy)"
                                    + " SELECT '" + BillType + "','" + clmName + "'," + showHide + ",1,'" + MainPage.strLoginName + "' WHERE(SELECT COUNT(*) FROM CustomReportSetting WHERE BillType = '" + BillType + "' and ColumnName = '" + clmName + "') = 0 ";
                }
                clmName = "";
            }
            
            int _count = dba.ExecuteMyQuery(strQuery);
            if (_count > 0)
            {
                MessageBox.Show("Thank you ! " + BillType + " setting updated successfully ! ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
                MessageBox.Show("Sorry ! Unable to update right now!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void GetChkSetting(string BillType)
        {
            try
            {
                string strQuery = "  Select * from CustomReportSetting WHERE BillType = '" + BillType + "' AND ShowHide > 0";
                DataTable dt = dba.GetDataTable(strQuery);
                if (dt.Rows.Count > 0)
                {
                    string clm = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        clm = Convert.ToString(dr["ColumnName"]);
                        switch (clm)
                        {
                            case "Variant2":
                                chkVariant2.Checked = true; break;
                            case "Variant1":
                                chkVariant1.Checked = true; break;
                            case "Variant3":
                                chkVariant3.Checked = true; break;
                            case "Variant4":
                                chkVariant4.Checked = true; break;
                            case "Variant5":
                                chkVariant5.Checked = true; break;
                            case "ItemName":
                                chkItemName.Checked = true; break;
                            case "Department":
                                chkDepartment.Checked = true; break;
                            case "Rate":
                                chkRate.Checked = true; break;
                            case "BrandName":
                                chkBrandName.Checked = true; break;
                            case "Barcode":
                                chkBarcode.Checked = true; break;
                            case "Supplier":
                                chkSupplier.Checked = true; break;
                            case "Category":
                                chkCategory.Checked = true; break;
                            case "Group":
                                chkGroup.Checked = true; break;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
