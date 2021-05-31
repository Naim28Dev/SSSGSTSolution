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
    public partial class FormDashboard : Form
    {
        DataBaseAccess dba;
        public FormDashboard()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void BindCustomer(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                Lbl_Active_Cust_Count.Text = Convert.ToString(dt.Rows[0]["ActiveCount"]);
                Lbl_Inactive_Cust_Count.Text = Convert.ToString(dt.Rows[0]["InactiveCount"]);
            }
        }

        private void BindStockInHand(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                Lbl_Stock_Qty.Text = dba.ConvertObjectToDouble(dt.Rows[0]["Qty"]).ToString("N0", MainPage.indianCurancy);
                Lbl_Stock_Amt.Text = dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindBirthDay(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToString(row["BType"]) == "DOB")
                        lbl_Birthday.Text = Convert.ToString(dt.Rows[0]["DCount"]);
                    else
                        Lbl_Anniversary_Main.Text = Convert.ToString(dt.Rows[1]["DCount"]);
                }
            }
        }

        private void BindSundryDrCrBalance(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["GroupName"]).ToUpper() == "SUNDRY DEBTORS")
                    Lbl_Sundry_Debtors.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
                else if (Convert.ToString(row["GroupName"]).ToUpper() == "SUNDRY CREDITOR")
                    Lbl_Sundry_Creditor.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindDeletedandEdited(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                Lbl_Edited_Count.Text = Convert.ToString(dt.Rows[0]["EditCount"]);
                Lbl_Removel_Count.Text = Convert.ToString(dt.Rows[0]["DeleteCount"]);
            }
        }

        private void BindPerDayProfitAndExpense(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["BType"]).ToUpper() == "PROFIT")
                    Lbl_Gross_Profit.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
                else if (Convert.ToString(row["BType"]).ToUpper() == "EXPENSE")
                    Lbl_Expenses.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }
        private void BindDailyAndWeeklySale(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                Lbl_Total_Sales_Daily_Count.Text = dba.ConvertObjectToDouble(dt.Rows[0]["DailyAmt"]).ToString("N0", MainPage.indianCurancy);
                Lbl_Total_Sales_Weekly_Count.Text = dba.ConvertObjectToDouble(dt.Rows[0]["WeeklySale"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindDepartmentWiseSale(DataTable dt)
        {
            double dAmt = 0;
            foreach (DataRow row in dt.Rows)
            {
                dAmt = dba.ConvertObjectToDouble(row["Amount"]);

                if (Convert.ToString(row["DepartmentName"]).ToUpper() == "MENS")
                    Lbl_Mens_Count.Text = dAmt.ToString("N0", MainPage.indianCurancy);
                else if (Convert.ToString(row["DepartmentName"]).ToUpper() == "WOMENS")
                    Lbl_Women_Count.Text = dAmt.ToString("N0", MainPage.indianCurancy);
                else if (Convert.ToString(row["DepartmentName"]).ToUpper() == "KIDS")
                    Lbl_Kids_Count.Text = dAmt.ToString("N0", MainPage.indianCurancy);
                else if (Convert.ToString(row["DepartmentName"]).ToUpper() == "ACCESSORIES")
                    Lbl_Accessoriws_Count.Text = dAmt.ToString("N0", MainPage.indianCurancy); 
            }
        }

        private void BindTopBrandDetails(DataTable dt)
        {
            //if(dt.Rows.Count>0)
            {
                Lbl__Brand_Hight_Profit.Text = "--";
                Lbl_Fastest_Brand.Text = "--";
            }
        }

        private void BindBankBalance(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                double dBalance = dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]);
                if (dBalance > 0)
                    Lbl_Net_Balance_Bank.Text = Math.Abs(dBalance).ToString("N0", MainPage.indianCurancy) + " Dr.";
                else
                    Lbl_Net_Balance_Bank.Text = Math.Abs(dBalance).ToString("N0", MainPage.indianCurancy) + " Cr.";
            }
        }

        private void BindSuspencesBalance(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                Lbl_Suspense_Amt.Text = dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindUnclearBalance(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["Status"]).ToUpper() == "CREDIT")
                    Lbl_Unclear_Payment.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
                else
                    Lbl_Unclear_Receipt.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindDailyBankReceiptPayment(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["Status"]).ToUpper() == "CREDIT")
                    Lbl_Total_Payment.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
                else
                    Lbl_Total_Reciept_Bank.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }


        private void BindDailyCashReceiptPayment(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["Status"]).ToUpper() == "CREDIT")
                    Lbl_Cash_Payment.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
                else
                    Lbl_Cash_Receipt.Text = dba.ConvertObjectToDouble(row["Amt"]).ToString("N0", MainPage.indianCurancy);
            }
        }

        private void BindCashBalance(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                double dBalance = dba.ConvertObjectToDouble(dt.Rows[0]["Amt"]);
                if (dBalance > 0)
                    Lbl_Cash_Balance.Text = Math.Abs(dBalance).ToString("N0", MainPage.indianCurancy)+" Dr.";
                else
                    Lbl_Cash_Balance.Text = Math.Abs(dBalance).ToString("N0", MainPage.indianCurancy) + " Cr.";
            }
        }

        private void BindChart(DataTable dt)
        {
            string strMonthName = "";
            double dSaleAmt = 0, dPurchaseAmt = 0, dProfit = 0;
            foreach (DataRow row in dt.Rows)
            {
                strMonthName = Convert.ToString(row["_MonthName"]);
                dSaleAmt = NetDBAccess.ConvertObjectToDouble(row["SaleAmt"]);
                dPurchaseAmt = NetDBAccess.ConvertObjectToDouble(row["PurchaseAmt"]);
                dProfit = NetDBAccess.ConvertObjectToDouble(row["Profit"]);

                chart1.Series["Sales"].Points.AddXY(strMonthName, dSaleAmt);
                chart1.Series["Purchase"].Points.AddXY(strMonthName, dPurchaseAmt);
                chart1.Series["Profit"].Points.AddXY(strMonthName, dProfit);
            }
        }
        private void FormDashboard_Load(object sender, EventArgs e)
        {
            initalizeFormControls();
            AdjustElement();
            GetDataFromDB();
        }
        private void AdjustElement()
        {
            Lbl_Deactivate.Left = (Pnl_Top_First_Main.Width / 2) + 70/5;
            Lbl_Deactivate_Icon.Left = (Pnl_Top_First_Main.Width / 2) + 90;
            Lbl_Inactive_Cust_Count.Left = (Pnl_Top_First_Main.Width / 2) + 60/5;
            Lbl_Maximim.Left = (Pnl_Top_Third_Main.Width / 2) + 100/5;
            Lbl_Max_Stock.Left = (Pnl_Top_Third_Main.Width / 2) + 110/5;

            Lbl_Anniversary.Left = (Pnl_Top_Fourth_Main.Width / 2) + 50/5;
            Lbl_Anniversary_Main.Left = (Pnl_Top_Fourth_Main.Width / 2) + 70/5;
            Lbl_Womens.Left = (Pnl_Department_Wise_Sale.Width / 2) + 20/5;

            Lbl_Fastest.Left = (Pnl_Profit_Wise_Sale.Width / 2) + 70/5;
            Lbl_SlowMoving.Left = Lbl_Fastest.Left;
            Lbl_Fastest_Brand.Left = (Pnl_Profit_Wise_Sale.Width / 2) + 20;
            Lbl_Slowest_Brand.Left = Lbl_Fastest_Brand.Left;
            Lbl_expense.Left = (Pnl_Top_Total_Expence.Width / 2) + 70/5;
            Lbl_Expenses.Left = (Pnl_Top_Total_Expence.Width / 2) + 70/5;
            Lbl_TotalPayment_Bank.Left = (Pnl_Side_Top.Width / 2) + 10;
            Lbl_Total_Payment.Left = (Pnl_Side_Top.Width / 2) + 15;
            Lbl_Total_Payment_Cash.Left = (Pnl_Second_Side_Main.Width / 2) + 40/5;
            Lbl_Cash_Payment.Left = (Pnl_Second_Side_Main.Width / 2) + 40/5;
            // Lbl_Unclear_Receipt_Header.Top = Pnl_Side_Top.Bottom - 110;///
            Lbl_UnclearPayment_Header.Left = (Pnl_Side_Top.Width / 2) + 5;
            Lbl_Unclear_Payment.Left = (Pnl_Side_Top.Width / 2) + 15;
            //Lbl_Unclear_Receipt.Top = Lbl_Unclear_Receipt_Header.Bottom +5;
            Lbl_Net_Balance.Left= Pnl_Side_Top.Width/2+10;
            Lbl_Net_Balance_Bank.Left = Pnl_Side_Top.Width/2-10;

            int wd = Pnl_Department_Wise_Sale.Width / 4;

            Lbl_Womens.Left = Lbl_Women_Count.Left  = Lbl_Men.Left + wd;
            Lbl_Kids.Left = Lbl_Kids_Count.Left = Lbl_Womens.Left + wd;
            Lbl_Accessories.Left = Lbl_Accessoriws_Count.Left = Lbl_Kids.Left + wd -5;
            
            Lbl_Sundry_Bebtors.Left = Lbl_Sundry_Creditor_Header.Right + 70/5;
            Lbl_Sundry_Debtors.Left = Lbl_Sundry_Creditor_Header.Right + 60/5;
            Lbl_Edited.Left = Lbl_Removel.Right + 70;
            Lbl_Edited_Count.Left = Lbl_Removel.Right + 70;

            Lbl_Amount_Pending.Left = Lbl_Quantity.Right + 80;
            Lbl_Stock_Amt.Left = Lbl_Quantity.Right + 70;

            Lbl_Total_Sales_Weekly.Left = Lbl_Daily_Icon.Right + 150/5;
            Lbl_Icon_Weekly.Left = Lbl_Total_Sales_Weekly.Right;
            Lbl_Total_Sales_Weekly_Count.Left = Lbl_Daily_Icon.Right + 130/5;
        }

        private void initalizeFormControls()
        {
            MainPage mp = MainPage.mymainObject;
            this.Width = mp.Width - 198;
            this.Height = mp.Height - 73;
            pnl_Top.Width = this.Width;
            double subwidth = this.Width / 5;
            Pnl_Top_Left_First.Left = 0;
            Pnl_Top_Left_First.Width = Convert.ToInt32(subwidth);
            Pnl_Top_First_Main.Left = Pnl_Top_Left_First.Left + 15;
            Pnl_Top_First_Main.Width = Pnl_Top_Left_First.Width - 30;
            Pnl_Top_Left_Second.Left = Pnl_Top_Left_First.Right;
            Pnl_Top_Left_Second.Width = Convert.ToInt32(subwidth);
            Pnl_Top_Second_Main.Width = Pnl_Top_Left_Second.Width - 20;
            Pnl_Top_Left_Third.Left = Pnl_Top_Left_Second.Right + 5;
            Pnl_Top_Left_Third.Width = Convert.ToInt32(subwidth) - 10;
            Pnl_Top_Third_Main.Width = Pnl_Top_Left_Third.Width - 20;
            Pnl_Top_Left_Fourth.Left = Pnl_Top_Left_Third.Right + 5;
            Pnl_Top_Left_Fourth.Width = Convert.ToInt32(subwidth) - 5;
            Pnl_Top_Fourth_Main.Width = Pnl_Top_Left_Fourth.Width - 20;
            Pnl_Balances.Left = Pnl_Top_Left_Fourth.Right;
            Pnl_Balances.Width = Convert.ToInt32(subwidth);
            Pnl_Bal_Main.Width = Pnl_Balances.Width - 20;
            Pnl_Top_Second.Width = this.Width;
            Pnl_Second_Top_Left_First.Left = 0;
            Pnl_Second_Top_Left_First.Width = Convert.ToInt32(subwidth)-30;
            Pnl_Total_Sale.Left = Pnl_Top_Second.Left + 15;
            Pnl_Total_Sale.Width = Pnl_Second_Top_Left_First.Width - 25;
            Pnl_Second_Top_Left_Second.Left = Pnl_Second_Top_Left_First.Right;
            Pnl_Second_Top_Left_Second.Width = Convert.ToInt32(subwidth)+50;
            Pnl_Department_Wise_Sale.Width = Pnl_Second_Top_Left_Second.Width - 20;
            Pnl_Second_Top_Left_Third.Left = Pnl_Second_Top_Left_Second.Right;
            Pnl_Second_Top_Left_Third.Width = Convert.ToInt32(subwidth) - 20;
            Pnl_Profit_Wise_Sale.Width = Pnl_Second_Top_Left_Third.Width - 20;
            Pnl_Second_Top_Left_Fourth.Left = Pnl_Second_Top_Left_Third.Right + 5;
            Pnl_Second_Top_Left_Fourth.Width = Convert.ToInt32(subwidth) - 15;
            Pnl_Top_Total_Expence.Width = Pnl_Second_Top_Left_Fourth.Width - 20;
            Pnl_Bills.Left = Pnl_Second_Top_Left_Fourth.Right;
            Pnl_Bills.Width = Convert.ToInt32(subwidth);
            Pnl_Bills_Main.Width = Pnl_Balances.Width - 10;

            Pnl_Right_Side.Width = Convert.ToInt32(subwidth+25);
            Pnl_Side_Top_Main.Top = -2;
            Pnl_Side_Top_Main.Height = (Pnl_Right_Side.Height / 2) + 20;
            Pnl_Side_Top_Main.Width = Pnl_Right_Side.Width;
            Pnl_Side_Top.Width = Pnl_Side_Top_Main.Width - 25;
            Pnl_Side_Top.Left = Pnl_Side_Top_Main.Left + 10;
            Pnl_Side_Top.Height = Pnl_Side_Top_Main.Height - 5;
            Pnl_Side_Second_Top_Main.Top = Pnl_Side_Top_Main.Bottom + 5;
            Pnl_Side_Second_Top_Main.Height = (Pnl_Right_Side.Height / 2) - 5;
            Pnl_Side_Second_Top_Main.Width = Pnl_Right_Side.Width - 10;
            Pnl_Second_Side_Main.Width = Pnl_Side_Top_Main.Width - 25;
            Pnl_Second_Side_Main.Left = Pnl_Side_Second_Top_Main.Left + 10;
            Pnl_Second_Side_Main.Height = Pnl_Side_Second_Top_Main.Height - 10;

            Pnl_Chart.Width = (this.Width - Pnl_Right_Side.Width);
        }
        private void FormDashboard_Resize(object sender, EventArgs e)
        {
            initalizeFormControls();
            AdjustElement();
        }

        private void fillChart()
        {
            //AddXY value in chart1 in series named as Salary  
            chart1.Series["Sales"].Points.AddXY("Jan", "10");
            chart1.Series["Sales"].Points.AddXY("Feb", "20");
            chart1.Series["Sales"].Points.AddXY("Mar", "30");
            chart1.Series["Sales"].Points.AddXY("Apr", "40");
            chart1.Series["Sales"].Points.AddXY("May", "55");
            chart1.Series["Sales"].Points.AddXY("Jun", "60");

            chart1.Series["Purchase"].Points.AddXY("Jan", "8");
            chart1.Series["Purchase"].Points.AddXY("Feb", "16");
            chart1.Series["Purchase"].Points.AddXY("Mar", "24");
            chart1.Series["Purchase"].Points.AddXY("Apr", "32");
            chart1.Series["Purchase"].Points.AddXY("May", "40");
            chart1.Series["Sales"].Points.AddXY("Jun", "48");

            chart1.Series["Profit"].Points.AddXY("Jan", "5");
            chart1.Series["Profit"].Points.AddXY("Feb", "10");
            chart1.Series["Profit"].Points.AddXY("Mar", "25");
            chart1.Series["Profit"].Points.AddXY("Apr", "20");
            chart1.Series["Profit"].Points.AddXY("May", "25");
            chart1.Series["Sales"].Points.AddXY("Jun", "30");

            //chart title  
            chart1.Titles.Add("Salary Chart");
        }

        private void FormDashboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private string CreateQuery()
        {
            DateTime _date = MainPage.currentDate;
            string strQuery = "";

            strQuery += "Select SUM((CASE WHEN SAmt>0 then 1 else 0 end))ActiveCount,SUM((CASE WHEN SAmt=0 then 1 else 0 end))InactiveCount from (Select (AreaCode+AccountNo+' '+Name)CustomerName,Station,MobileNo,(ISNULL((Select SUM(Amt)SAmt from (  Select SUM(CAST(NetAmt as money)) Amt from SalesRecord SR Where BillDate>(DATEADD(dd,-90,Getdate())) and SR.SalePartyID = (AreaCode + AccountNo)UNION ALL Select SUM(NetAmt) Amt from SalesBook SR Where Date>(DATEADD(dd,-90,Getdate())) and SR.SalePartyID = (AreaCode + AccountNo))Sales),0)) SAmt from SupplierMaster SM Where GroupName = 'SUNDRY DEBTORS' UNION ALL Select (SalePartyID) as CustomerName, (Station) as Station, MobileNo, SUM(NetAmt)SAmt from SalesBook WHere Date>(DATEADD(dd,-90,Getdate())) and SalePartyID not in (Select(AreaCode + AccountNo) from SupplierMaster WHere GroupName = 'SUNDRY DEBTORS') Group by MobileNo,SalePartyID,Station)_Sales WHere CustomerName != ''  "
                    //  + " --Stock Qty "
                    // + " Select SUM(InQty-SQty)Qty,SUM((InQty-SQty)*Rate)Amt from ( Select _IM.Other as Category,_IM.GroupName,MakeName,SM.BrandName,SM.BarCode,SM.ItemName, SM.Variant1, SM.Variant2,INQty,ISNULL(OutQty,0)SQty,ISNULL((SELECT dbo.GetAvgRate(SM.BrandName,SM.BarCode,SM.ItemName,SM.Variant1,SM.Variant2)),0)Rate,PurchasePartyID,SMaster.Name as Name from (  Select SM.BrandName,ISNULL(SM.BarCode,'')BarCode,SM.ItemName, SM.Variant1, SM.Variant2, SUM(SM.Qty)INQty from StockMaster SM  Where BillType in ('OPENING','PURCHASE','STOCKIN','SALERETURN')  Group by SM.BrandName,ISNULL(SM.BarCode,''),SM.ItemName, SM.Variant1, SM.Variant2   )SM left join (Select _SM.BrandName,ISNULL(_SM.BarCode,'')BarCode,_SM.ItemName, _SM.Variant1,_SM.Variant2,SUM(_SM.Qty)OutQty from StockMaster _SM  Where _SM.BillType in ('SALES','PURCHASERETURN','STOCKOUT')  Group by _SM.BrandName,ISNULL(_SM.BarCode,''),_SM.ItemName,_SM.Variant1,_SM.Variant2  )_SM on SM.BrandName=_SM.BrandName and SM.BarCode=_SM.BarCode and SM.ItemName=_SM.ItemName and SM.Variant1=_SM.Variant1 and SM.Variant2=_SM.Variant2    left join (Select _IM.Other,MakeName,GroupName,ItemName,ROW_NUMBER() OVER (PARTITION BY ItemName Order by ItemName) RINumber from  Items _IM)_IM on SM.ItemName=_IM.ItemName and RINumber=1   left join (SELECT PurchasePartyID,BarCode,BrandName,ItemName,Variant1,Variant2,ROW_NUMBER() OVER (PARTITION BY BarCode,BrandName,ItemName,Variant1,Variant2 ORDER BY Date DESC) AS RNumber FROM PurchaseBook PB inner join PurchaseBookSecondary PBS on PB.BillCode=PBS.BillCode and PB.BillNo=PBS.BillNo)PBS on SM.BrandName=PBS.BrandName and SM.BarCode=PBS.BarCode and SM.ItemName=PBS.ItemName and SM.Variant1=PBS.Variant1 and SM.Variant2=PBS.Variant2 and RNumber=1  left join SupplierMaster SMaster on SMaster.AreaCode+SMaster.AccountnO=PurchasePartyID  Where (InQty>0 OR ISNULL(OutQty,0)>0) )Stock "
                    + "  Declare @StockAsPer varchar(50)=(select Top 1 StockAsPer from CompanySetting ),@ID1 int,@ID2 Int if (@StockasPer = 'AvgRate') Begin set @ID1 = 2 set @ID2 = 0 End else begin set @ID1 = 0 set @ID2 = 1 end select SUM(AvlQty)Qty,SUM(Amt)Amt From (Select(SUM(InQty) - SUM(SQty))AvlQty, ((SUM(InQty) - SUM(SQty)) * Rate)Amt from( Select _IM.Other as Category, _IM.GroupName, MakeName, SM.BrandName, SM.BarCode, SM.ItemName, SM.Variant1, SM.Variant2, INQty, ISNULL(OutQty, 0)SQty, (CASE WHEN ISNULL(Purc.AvgRate, 0) > 0 then ISNULL(Purc.AvgRate, 0) else ISNULL(SM.Rate, 0)end)Rate, PurchasePartyID, SMaster.Name as Name from( Select SM.BrandName, ISNULL(SM.BarCode, '')BarCode, SM.ItemName, SM.Variant1, SM.Variant2, SUM(SM.Qty)INQty, 0 Rate from StockMaster SM  Where BillType in ('OPENING', 'PURCHASE', 'STOCKIN', 'SALERETURN')  Group by SM.BrandName, ISNULL(SM.BarCode, ''), SM.ItemName, SM.Variant1, SM.Variant2 UNION ALL SELECT BrandName, ISNULL(BarCode, '')BarCode, ItemName, Variant1, Variant2, 0 INQty, AvgRate FROM( SELECT BrandName, ISNULL(BarCode, '')BarCode, ItemName, Variant1, Variant2, SUM(Qty)TSQty, (SUM(SAmt) / SUM(Qty))AvgRate FROM( Select SO.BrandName, ISNULL(SO.BarCode, '')BarCode, SO.ItemName, SO.Variant1, SO.Variant2, Qty, (Qty) * (Rate)SAmt from StockMaster SO Where SO.BillType in ('SALES', 'PURCHASERETURN', 'STOCKOUT') and SO.Qty <> 0)SOLD Group by SOLD.BrandName, ISNULL(SOLD.BarCode, ''), SOLD.ItemName, SOLD.Variant1, SOLD.Variant2 )OUTS WHERE (Select COUNT(*) from StockMaster ISM Where BillType in ('OPENING', 'PURCHASE', 'STOCKIN', 'SALERETURN') AND ISM.BrandName = OUTS.BrandName  and ISNULL(ISM.BarCode, '')= ISNULL(OUTS.BarCode, '') and ISM.ItemName = OUTS.ItemName and ISM.Variant1 = OUTS.Variant1 and ISM.Variant2 = OUTS.Variant2  ) = 0  )SM LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY BarCode, BrandName, ItemName, Variant1, Variant2 ORDER BY ID ASC) AS RNo, BrandName, BarCode, ItemName, Variant1, Variant2, SUM(PQty)TPQty, CAST((SUM(PAmt) / SUM(PQty)) as Numeric(18, 4))AvgRate FROM ( SELECT @ID1 ID, IM.BrandName, Description BarCode, IM.ItemName, Variant1, Variant2, 1 PQty, IMS.PurchaseRate PAmt FROM ItemSecondary IMS LEFT JOIN Items IM ON IMS.BillCode = IM.BillCode AND IMS.BillNo = IM.BillNo WHERE IMS.PurchaseRate != 0 UNION ALL SELECT @ID2 ID, BrandName, BarCode, ItemName, Variant1, Variant2, (CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE' WHERE BillType IN('OPENING', 'PURCHASE', 'STOCKIN') UNION ALL SELECT @ID2+1 ID ,BrandName,BarCode,ItemName,Variant1,Variant2,(CASE WHEN Qty = 0 then 1 else Qty end) PQty,(CASE WHEN Qty = 0 then 1 else Qty end)*(CASE WHEN ST.TaxIncluded = 1 then(SELECT NetRate FROM GetTaxRate(SM.ItemName, SM.MRP, SM.Rate)) else isnull(SM.Rate, 0) end )PAmt FROM StockMaster SM LEFT JOIN PurchaseBook PB on Sm.BillNo = PB.BillNo AND SM.BillCode = PB.BillCode LEFT JOIN SaleTypeMaster ST on PB.PurchaseType = ST.TaxName AND ST.SaleType = 'PURCHASE'   WHERE BillType IN('SALERETURN') )Ratt GROUP BY BrandName, BarCode, ItemName, Variant1, Variant2, ID )Purc on SM.BrandName = Purc.BrandName and ISNULL(SM.BarCode, '') = ISNULL(Purc.BarCode, '') and SM.ItemName = Purc.ItemName and SM.Variant1 = Purc.Variant1 and SM.Variant2 = Purc.Variant2 AND RNo = 1 left join (Select _SM.BrandName, ISNULL(_SM.BarCode, '')BarCode, _SM.ItemName, _SM.Variant1, _SM.Variant2, SUM(_SM.Qty)OutQty from StockMaster _SM  Where _SM.BillType in ('SALES', 'PURCHASERETURN', 'STOCKOUT')  Group by _SM.BrandName,ISNULL(_SM.BarCode, ''),_SM.ItemName,_SM.Variant1,_SM.Variant2  )_SM on SM.BrandName = _SM.BrandName and ISNULL(SM.BarCode, '')= ISNULL(_SM.BarCode, '') and SM.ItemName = _SM.ItemName and SM.Variant1 = _SM.Variant1 and SM.Variant2 = _SM.Variant2  left join (Select _IM.Other, MakeName, GroupName, ItemName, ROW_NUMBER() OVER(PARTITION BY ItemName Order by ItemName) RINumber from Items _IM)_IM on SM.ItemName = _IM.ItemName and RINumber = 1   left join (SELECT PurchasePartyID, BarCode, BrandName, ItemName, Variant1, Variant2, ROW_NUMBER() OVER(PARTITION BY BarCode, BrandName, ItemName, Variant1, Variant2 ORDER BY Date DESC) AS RNumber FROM PurchaseBook PB inner join PurchaseBookSecondary PBS on PB.BillCode = PBS.BillCode and PB.BillNo = PBS.BillNo)PBS on SM.BrandName = PBS.BrandName and SM.BarCode = PBS.BarCode and SM.ItemName = PBS.ItemName and SM.Variant1 = PBS.Variant1 and SM.Variant2 = PBS.Variant2 and RNumber = 1  left join SupplierMaster SMaster on SMaster.AreaCode + SMaster.AccountnO = PurchasePartyID  Where(InQty != 0 OR ISNULL(OutQty, 0) != 0) )Stock Group by Rate)_Stock"

                     //+ " --Birthday "
                     + " Select 'DOB' as BTYpe, COUNT(*) as DCount from SupplierMaster SM CROSS APPLY(Select WaybillUserName as WhatsappNo, SpouseName, (CASE WHEN DOB = '1900-01-01 00:00:00.000' then NULL else DOB end) DOB,(CASE WHEN DOA = '1900-01-01 00:00:00.000' then NULL else DOA end) DOA,(CASE WHEN DOB<> '1900-01-01 00:00:00.000' then CONVERT(Date,'' + CAST(DATEPART(MM, DOB) as varchar) + '/' + CAST(DATEPART(dd, DOB) AS varchar) + '/2020', 0) else NULL end) CDOB,(CASE WHEN DOA<> '1900-01-01 00:00:00.000' then CONVERT(Date,'' + CAST(DATEPART(MM, DOA) as varchar) + '/' + CAST(DATEPART(dd, DOA) AS varchar) + '/2020', 0) else NULL end) CDOA,CONVERT(Date, '' + CAST(DATEPART(MM, DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE()))) as varchar) + '/' + CAST(DATEPART(dd, DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE()))) AS varchar) + '/2020', 0) as CDate from SupplierOtherDetails SOD Where SM.AreaCode = SOD.AreaCode and SM.AccountNo = SOD.AccountNo) SOD Where TransactionLock = 0 and(ISNULL(SOD.DOB, '') != '' OR ISNULL(SOD.DOA, '') != '') and DOB is NOT NULL and (CASE WHEN DATEDIFF(dd, CDate, CDOB) < 0 then DATEDIFF(dd, CDate, CDOB) + 365 else DATEDIFF(dd, CDate, CDOB) end)< 7 and(CASE WHEN DATEDIFF(dd, CDate, CDOB) < 0 then DATEDIFF(dd, CDate, CDOB) + 365 else DATEDIFF(dd, CDate, CDOB) end) >= 0 and CDOB<= '" + _date + "' UNION ALL "
                     + " Select 'DOA' as BType, COUNT(*) as DCount from SupplierMaster SM CROSS APPLY(Select WaybillUserName as WhatsappNo, SpouseName, (CASE WHEN DOB = '1900-01-01 00:00:00.000' then NULL else DOB end) DOB,(CASE WHEN DOA = '1900-01-01 00:00:00.000' then NULL else DOA end) DOA,(CASE WHEN DOB<> '1900-01-01 00:00:00.000' then CONVERT(Date,'' + CAST(DATEPART(MM, DOB) as varchar) + '/' + CAST(DATEPART(dd, DOB) AS varchar) + '/2020', 0) else NULL end) CDOB,(CASE WHEN DOA<> '1900-01-01 00:00:00.000' then CONVERT(Date,'' + CAST(DATEPART(MM, DOA) as varchar) + '/' + CAST(DATEPART(dd, DOA) AS varchar) + '/2020', 0) else NULL end) CDOA,CONVERT(Date, '' + CAST(DATEPART(MM, DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE()))) as varchar) + '/' + CAST(DATEPART(dd, DATEADD(MINUTE, 30, DATEADD(hh, 5, GETUTCDATE()))) AS varchar) + '/2020', 0) as CDate from SupplierOtherDetails SOD Where SM.AreaCode = SOD.AreaCode and SM.AccountNo = SOD.AccountNo) SOD Where TransactionLock = 0 and(ISNULL(SOD.DOB, '') != '' OR ISNULL(SOD.DOA, '') != '') and DOB is NOT NULL and (CASE WHEN DATEDIFF(dd, CDate, CDOA) < 0 then DATEDIFF(dd, CDate, CDOA) + 365 else DATEDIFF(dd, CDate, CDOA) end)< 7 and(CASE WHEN DATEDIFF(dd, CDate, CDOA) < 0 then DATEDIFF(dd, CDate, CDOA) + 365 else DATEDIFF(dd, CDate, CDOA) end) >= 0 and CDOA<= '" + _date + "' "
                     //+ " -- Sundry Debtors "
                     + " Select GroupName, ABS(SUM(Amount))Amt from(Select GroupName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and GroupName in ('SUNDRY DEBTORS', 'SUNDRY CREDITOR')  Group by GroupName UNION ALL Select GroupName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and GroupName in ('SUNDRY DEBTORS', 'SUNDRY CREDITOR')  Group by GroupName )Sales Group by GroupName "
                    
                     //+ " --Edit / Delete Detail "
                    + " SELECT DeleteCount = (Select COUNT(*) from RemovalReason RS Where Cast([Date] as Date) = '"+ _date.ToString("yyyy-MM-dd")+ "' AND BillType in ('SALES','PURCHASE','SALERETURN','PURCHASERETURN','SALESERVICE'))"
                    + " ,EditCount = (Select COUNT(*) from EditTrailDetails ED Where Cast([Date] as Date) = '" + _date.ToString("yyyy-MM-dd") + "' AND BillType in ('SALES','PURCHASE','SALERETURN','PURCHASERETURN','SALESERVICE') and EditStatus = 'UPDATION')"

                     //+ " --Per day Profit / Expense "
                     + " Select 'EXPENSE' BType,SUM(Amount)Amt from(Select  SUM(CAST(Amount as Money)) Amount from BalanceAmount BA CROSS APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and GroupName in ('DIRECT EXPENSE A/C', 'INDIRECT EXPENSE A/C', 'COST OF MATERIAL TRADED', 'EMPLOYEE BENEFIT EXPENSE', 'OTHER EXPENSES', 'SELLING & DISTRIBUTION EXPENSES') and Convert(varchar, Date,103)= '" + _date.ToString("dd/MM/yyyy") + "'  UNION ALL  Select - SUM(CAST(Amount as Money)) Amount from BalanceAmount BA CROSS APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and GroupName in ('DIRECT EXPENSE A/C','INDIRECT EXPENSE A/C','COST OF MATERIAL TRADED','EMPLOYEE BENEFIT EXPENSE','OTHER EXPENSES','SELLING & DISTRIBUTION EXPENSES') and Convert(varchar, Date,103)= '" + _date.ToString("dd/MM/yyyy") + "' )Sales UNION ALL "
                     + " Select 'PROFIT' as BType,ISNULL(SUM(NetAmt),0) Amt from(Select BrandName,SUM(SAmt-PAmt)NetAmt from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,(SBS.Amount) SAmt,(Select Top 1 (Purchase.Rate*SBS.Qty) as PAmt from (Select Top 1 0 ID, SM.Rate from StockMaster SM WHere BillType in ('PURCHASE') and Qty!=0 and SM.ItemName=SBS.ItemName and SM.BarCode=SBS.BarCode and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 Order by SM.Date desc UNION ALL Select Top 1 1 ID,SM.Rate from StockMaster SM WHere BillType in ('OPENING','SALERETURN','STOCKIN') and Qty!=0 and SM.ItemName=SBS.ItemName and SM.BarCode=SBS.BarCode and SM.Variant1=SBS.Variant1 and SM.Variant2=SBS.Variant2 Order by SM.Date desc )Purchase Order by ID)PAmt from SalesBook SB inner join SalesBookSecondary SBS on SB.BIllCOde=SBS.BillCOde and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName WHere Convert(varchar, SB.Date,103)='" + _date.ToString("dd/MM/yyyy") + "' ) Sales Group by BrandName UNION ALL Select BrandName,SUM(SAmt-PAmt)NetAmt from (Select ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName  WHere Convert(varchar, SE.BillDate,103)='" + _date.ToString("dd/MM/yyyy") + "')Sale Group by BrandName)_Sale "
                     //+ " Select 'PROFIT' as BType,ISNULL(SUM(NetAmt),0) Amt from(Select BrandName,(Amt-ISNULL(PAmt,0))NetAmt from ( Select BrandName,SUM(SAmt)Amt from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,(SBS.Amount) SAmt from SalesBook SB inner join SalesBookSecondary SBS on SB.BIllCOde=SBS.BillCOde and SB.BillNo=SBS.BillNo inner join Items _Im on SBS.ItemName=_IM.ItemName WHere Convert(varchar, SB.Date,103)='" + _date.ToString("dd/MM/yyyy") + "') Sales Group by BrandName)_Sales OUTER APPLY (Select SUM(PAMt)PAmt from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Qty*Rate) PAmt from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN') and Qty!=0)_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!=''  )Purchase  UNION ALL Select BrandName,SUM(SAmt-PAmt)NetAmt from ( Select ISNULL(_IM.BrandName,'')BrandName,(GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt,(GRD.Amount - ((GRD.Amount *GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode+' '+CAST(GR.ReceiptNo as varchar))=SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode=GRD.ReceiptCode and GR.ReceiptNo=GRD.ReceiptNo inner join Items _Im on GRD.ItemName=_IM.ItemName  WHere Convert(varchar, SE.BillDate,103)='" + _date.ToString("dd/MM/yyyy") + "')Sale Group by BrandName )_Sale  "
                     //+ " --Department wise sale "

                     + " Select DepartmentName,SUM(_Amt)Amount from (Select DepartmentName,SUM(CAST((CASE WHEN TaxIncluded=1 then((Amount* 100) / (100 + TaxRate)) else Amount end) as Numeric(18,2)))_Amt from (  Select ISNULL(MakeName, '') as DepartmentName, (SBS.Amount)Amount,TaxIncluded,(Select TOP 1 ((CASE WHEN _TC.ChangeTaxRate = 1 then (CASE WHEN _TC.GreaterORSmaller = '>' then(CASE WHEN((((((CASE WHEN TaxIncluded = 1 then((SBS.MRP * 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))> _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) else (CASE WHEN((((((CASE WHEN TaxIncluded= 1 then((SBS.MRP* 100) / (100 + TaxRate)) else SBS.MRP end))) * (CASE WHEN _TC.AmountType='NET PRICE' then ((100.00+SBS.SDisPer-SpecialDscPer) / 100.00) else 1.00 end)))< _TC.ChangeAmt) then _TC.TaxChangeRateIGST else _TC.TaxRateIGST end) end) else _TC.TaxRateIGST end)) TaxRate from Items _Im inner join ItemGroupMaster _IGM on _IM.GroupName=_IGM.GroupName  left join TaxCategory _TC on _IGM.TaxCategoryName = _TC.CategoryName Where _IGM.ParentGroup = '' and _IM.ItemName=SBS.ItemName)TaxRate from salesbook SB left join salesbooksecondary SBS on sb.billcode=SBS.billcode and SB.billno=SBS.billno left join SaleTypeMaster STM on STM.TaxName=SB.SalesType and STM.SaleType='SALES'  left join Items _Im on SBS.ItemName=_Im.ItemName)_Sales Where DepartmentName!='' Group by DepartmentName UNION ALL   Select ISNULL(MakeName, '') as DepartmentName, SUM((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)))_Amt from SalesEntry SE inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) inner join Items _Im on GRD.ItemName = _IM.ItemName Group by ISNULL(MakeName, ''))Sale Group by DepartmentName Order by DepartmentName "
                     //+ " Select DepartmentName,SUM(_Amt)Amount from ( "
                     //+ " Select ISNULL(MakeName, '') as DepartmentName, SUM(SBS.Amount)_Amt from SalesBookSecondary SBS inner join Items _Im on SBS.ItemName = _IM.ItemName Group by ISNULL(MakeName, '') UNION ALL "
                     //+ " Select ISNULL(MakeName, '') as DepartmentName, SUM((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)))_Amt from SalesEntry SE inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) inner join Items _Im on GRD.ItemName = _IM.ItemName Group by ISNULL(MakeName, '') "
                     //+ " )Sale Group by DepartmentName Order by DepartmentName "
                     //+ " --DailySale / Weekly sale "
                  
                     + " Select SUM(ISNULL(Amt, 0)) DailyAmt,(Select SUM(ISNULL(NAmt,0))WAmt from (Select SUM(CAST(NetAmt as money)-CAST((ISNULL(RoundOffSign,'+')+(CAST(ISNULL(RoundOffAmt,0) as varchar))) as Money)-TaxAmount)NAmt from SalesRecord Where BillDate>'" + _date.AddDays(-7).ToString("MM/dd/yyyy") + "' UNION ALL Select SUM(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt)NAmt from SalesBook Where Date>'" + _date.AddDays(-7).ToString("MM/dd/yyyy") + "' )Sales)WeeklySale From( Select SUM(CAST(NetAmt as money)-CAST((ISNULL(RoundOffSign,'+')+(CAST(ISNULL(RoundOffAmt,0) as varchar))) as Money)-TaxAmount)Amt from SalesRecord Where Convert(varchar, BillDate,103)='" + _date.ToString("dd/MM/yyyy") + "' UNION ALL Select SUM(NetAmt-CAST((RoundOffSign+(CAST(RoundOffAmt as varchar))) as Money)-TaxAmt)Amt from SalesBook Where Convert(varchar, Date,103)='" + _date.ToString("dd/MM/yyyy") +"' )Balance  "
                     //+ " --Bank "
                     + " Select GroupName, SUM(Amount)Amt from(Select GroupName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and GroupName in ('BANK A/C')and(CASE WHEN(Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) = 1 Group by GroupName UNION ALL Select GroupName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and GroupName in ('BANK A/C') and(CASE WHEN(Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then 1 else ChequeStatus end) = 1 Group by GroupName )Sales Group by GroupName "
                     + " Select SUM(CAST(Amount as money))Amt, Status from BalanceAmount BA CROSS APPLY(SELECT GroupName from SupplierMaster SM Where GroupName = 'BANK A/C' and(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where VoucherCode != '' and(Description Like('%CHQ%') OR Description Like('%CHEQUE%')) and ChequeStatus = 0  Group by Status "
                     + " Select SUM(CAST(Amount as money))Amt, Status from BalanceAmount BA CROSS APPLY(SELECT GroupName from SupplierMaster SM Where GroupName = 'BANK A/C' and(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where VoucherCode != '' and Convert(varchar, (CASE WHEN(Description Not  Like('%CHQ%') AND Description Not Like('%CHEQUE%')) then Date else CHqDate end), 103)= '" + _date.ToString("dd/MM/yyyy") + "'  Group by Status "
                     // " -- SUSPENCES A/C --"
                     + " Select GroupName,SUM(Amount)Amt from(Select GroupName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and GroupName in ('SUSPENCES A/C')  and BA.VoucherCode In (SELECT BankVCode From CompanySetting) Group by GroupName UNION ALL Select GroupName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and GroupName in ('SUSPENCES A/C')  and BA.VoucherCode In (SELECT BankVCode From CompanySetting)   Group by GroupName )Sales Group by GroupName "
                     //+ " --CASH "
                     + " Select GroupName,SUM(Amount)Amt from(Select GroupName, SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'DEBIT' and GroupName in ('CASH A/C')  Group by GroupName UNION ALL Select GroupName, -SUM(CAST(Amount as Money)) Amount from BalanceAmount BA OUTER APPLY(SELECT GroupName from SupplierMaster SM Where(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Status = 'CREDIT' and GroupName in ('CASH A/C')  Group by GroupName )Sales Group by GroupName "
                     + " Select SUM(CAST(Amount as money))Amt, Status from BalanceAmount BA CROSS APPLY(SELECT GroupName from SupplierMaster SM Where GroupName = 'CASH A/C' and(SM.AreaCode + SM.AccountNo) = BA.AccountID) SM Where Convert(varchar, Date, 103) = '" + _date.ToString("dd/MM/yyyy") + "' Group by Status "
                    //+ " --Sales Garph "
                    //+ " Select _MonthName, (CASE WHEN _Month < 4 then(_Month + 12) else _Month end)_Month,SUM(ISNULL(SaleAmt, 0)) SaleAmt,SUM(ISNULL(PurchaseAmt, 0)) PurchaseAmt,SUM((SaleAmt - PurchaseAmt)) Profit From( "
                    //+ " Select Convert(char(3), BA.Date, 0)_MonthName, DATEPART(MONTH, BA.Date) AS _Month, ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) SaleAmt,0 as PurchaseAmt from BalanceAmount BA  Where BA.Status = 'DEBIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C')  Union All "
                    //+ " Select Convert(char(3), BA.Date, 0)_MonthName,DATEPART(MONTH, BA.Date) AS _Month,0 as SaleAmt,ISNULL((CAST(BA.Amount as Money) - ISNULL((SELECT SUM(CAST(BA1.Amount as Money))  from BalanceAmount BA1 Where BA1.Description = BA.Description and BA1.AccountStatus = 'DUTIES & TAXES'), 0)),0) PurchaseAmt from BalanceAmount BA Where BA.Status = 'CREDIT' and BA.AccountStatus in ('SALES A/C','SALE SERVICE','PURCHASE A/C') )Balance "
                    //+ " Group by _Month, _MonthName Order by (CASE WHEN _Month < 4 then(_Month + 12) else _Month end) "

                    // "--- Sales Graph New Query   ---- "
                    + " SELECT _MonthName, _Month, SUM(Profit)Profit, SUM(SaleAmt) SaleAmt, SUM(PurchaseAmt) PurchaseAmt FROM"
                    + " (SELECT Sales._MonthName, Sales._Month, Profit, SaleAmt, PurchaseAmt FROM"
                    + " (Select _MonthName, (CASE WHEN _Month < 4 then(_Month + 12) else _Month end)_Month"
                    + " , SUM(SAmt - PAmt)Profit, Sum(SAmt)SaleAmt, 0 PurchaseAmt  from"
                    + " (Select Convert(char(3), SB.Date, 0)_MonthName, DATEPART(MONTH, SB.Date) AS _Month, (SBS.Amount)SAmt, ("
                    + " Select Top 1(Purchase.Rate * SBS.Qty) as PAmt from(Select Top 1 0 ID, SM.Rate from StockMaster SM"
                    + " WHere BillType in ('PURCHASE') and Qty != 0 and SM.ItemName = SBS.ItemName and SM.BarCode = SBS.BarCode and"
                    + " SM.Variant1 = SBS.Variant1 and SM.Variant2 = SBS.Variant2 UNION ALL Select Top 1 1 ID, SM.Rate from StockMaster SM"
                    + " WHere BillType in ('OPENING', 'SALERETURN', 'STOCKIN') and Qty != 0 and SM.ItemName = SBS.ItemName and SM.BarCode = SBS.BarCode"
                    + " and SM.Variant1 = SBS.Variant1 and SM.Variant2 = SBS.Variant2)Purchase"
                    + " Order by ID)PAmt from SalesBook SB inner join SalesBookSecondary SBS on SB.BIllCOde = SBS.BillCOde and SB.BillNo = SBS.BillNo inner join Items _Im on SBS.ItemName = _IM.ItemName)Sale Group by _Month, _MonthName"
                    + " UNION ALL  Select _MonthName,(CASE WHEN _Month < 4 then(_Month + 12) else _Month end)_Month,SUM(SAmt - PAmt)Profit,Sum(SAmt)SaleAmt,0 PurchaseAmt"
                    + " from(Select Convert(char(3), SE.BillDate, 0)_MonthName, DATEPART(MONTH, SE.BillDate) AS _Month, (GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100))SAmt, (GRD.Amount - ((GRD.Amount * GR.DisPer) / 100)) PAmt from SalesEntry SE inner join GoodsReceive GR on (GR.ReceiptCode + ' ' + CAST(GR.ReceiptNo as varchar)) = SE.GRSNo inner join GoodsReceiveDetails GRD on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo inner join Items _Im on GRD.ItemName = _IM.ItemName)Sale Group by _Month, _MonthName ) Sales"
                    + " UNION ALL"
                    + " SELECT  _MonthName, (CASE WHEN _Month < 4 then(_Month + 12) else _Month end)_Month, 0 Profit,0 SaleAmt,SUM(Amount) as PurchaseAmt from(SELECT Convert(char(3), PB.Date, 0)_MonthName, DATEPART(MONTH, PB.Date) AS _Month, Amount FROM PurchaseBookSecondary PBS INNER JOIN PurchaseBook PB on PB.BillCode = PBS.BillCode AND PB.BillNo = PBS.BillNo) as PurchaseRecs Group By _Month, _MonthName) FINALDATA"
                    + " GROUP BY _Month, _MonthName Order by (CASE WHEN _Month < 4 then(_Month + 12) else _Month end) "


                     + " Select * from (Select BrandName,ISNULL(MinStock,0)MinStock,ISNULL(MaxStock,0)MaxStock,SUM(Qty)NetQty from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,Qty from StockMaster SM Inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN','STOCKIN') UNION ALL Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,-Qty from StockMaster SM Inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('PURCHASERETURN','SALES') )Stock CROSS APPLY (Select MinStock,MaxStock from BrandMaster BM WHere BM.BrandName=Stock.BrandName and (MinStock>0 OR MaxStock>0))BM Group by BrandName,MinStock,MaxStock)_Stock Order by BrandName  ";
         
            //Sale Ratio and Brand wise profit
            // + " Select * from (Select BrandName,(CASE WHEN _Qty!=0 then ((SQTy*100)/_Qty) else 0 end) as SaleRatio from (Select (CASE When ISNULL(PBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(PBS.BrandName,'') end) BrandName,ISNULL(PBS.BarCode,'')as BarCode,PBS.ItemName,PBS.Variant1,PBS.Variant2, SUM(Qty)_Qty from PurchaseBookSecondary PBS inner join Items _Im on PBS.ItemName=_Im.ItemName Group by ISNULL(PBS.BarCode,''),PBS.ItemName,PBS.Variant1,PBS.Variant2,(CASE When ISNULL(PBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(PBS.BrandName,'') end))PBS OUTER APPLY(Select SUM(SBS.Qty)SQty from SalesBookSecondary SBS WHere PBS.ItemName=SBS.ItemName and PBS.Variant1=SBS.Variant1 and PBS.Variant2=SBS.Variant2 and ISNULL(PBS.BarCode,'')=ISNULL(SBS.BarCode,'')) SBS )_SaleRatio Order by SaleRatio desc "
            // + " Select BrandName,(Amt-ISNULL(PAmt,0))NetAmt from (Select BrandName,SUM(SAmt)Amt from (Select (CASE When ISNULL(SBS.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SBS.BrandName,'') end) BrandName,(SBS.Amount) SAmt from SalesBookSecondary SBS inner join Items _Im on SBS.ItemName=_IM.ItemName) Sales Group by BrandName)_Sales OUTER APPLY (Select SUM(PAMt)PAmt from (Select (CASE When ISNULL(SM.BrandName,'')='' then ISNULL(_IM.BrandName,'') else ISNULL(SM.BrandName,'') end) BrandName,(Qty*Rate) PAmt from StockMaster SM inner join Items _IM on SM.ItemName=_IM.ItemName WHere BillType in ('OPENING','PURCHASE','SALERETURN') and Qty!=0)_Purchase Where _Purchase.BrandName=_Sales.BrandName and _Purchase.BrandName!='' )Purchase Where BrandName!=''  Order by (Amt-ISNULL(PAmt,0)) ";

            //+ " --Brand Wise "
            //+ " Select BrandName,SUM(_Amt)Amount from ( "
            //+ " Select ISNULL(BrandName, '') as BrandName, SUM(SBS.Amount)_Amt from SalesBookSecondary SBS Group by ISNULL(BrandName, '') UNION ALL "
            //+ " Select ISNULL(BrandName, '') as BrandName, SUM((GRD.Amount + ((GRD.Amount * (SE.DiscountStatus + SE.Discount)) / 100)))_Amt from SalesEntry SE inner join GoodsReceiveDetails GRD on SE.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) inner join Items _Im on GRD.ItemName = _IM.ItemName Group by ISNULL(BrandName, '') "
            //+ " )Sale Group by BrandName Order by BrandName";

            return strQuery;
        }

        private void GetDataFromDB()
        {
            try
            {
                string strQuery = CreateQuery();
                if (strQuery != "")
                {
                    DataSet ds = DataBaseAccess.GetDataSetRecord(strQuery);
                    if (ds.Tables.Count > 0)
                    {
                        BindCustomer(ds.Tables[0]);
                        BindStockInHand(ds.Tables[1]);
                        BindBirthDay(ds.Tables[2]);
                        BindSundryDrCrBalance(ds.Tables[3]);
                        BindDeletedandEdited(ds.Tables[4]);
                        BindPerDayProfitAndExpense(ds.Tables[5]);
                        BindDepartmentWiseSale(ds.Tables[6]);
                        BindDailyAndWeeklySale(ds.Tables[7]);
                        BindBankBalance(ds.Tables[8]);
                        BindUnclearBalance(ds.Tables[9]);
                        BindDailyBankReceiptPayment(ds.Tables[10]);
                        BindSuspencesBalance(ds.Tables[11]);
                        BindCashBalance(ds.Tables[12]);
                        BindDailyCashReceiptPayment(ds.Tables[13]);
                        BindChart(ds.Tables[14]);
                        BindMinMaxStock(ds.Tables[15]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BindMinMaxStock(DataTable dt)
        {
            try
            {
                DataRow[] row = dt.Select(" NetQty<MinStock");
                Lbl_Min_Stock.Text = row.Length.ToString();


                row = dt.Select(" NetQty>MaxStock");
                Lbl_Max_Stock.Text = row.Length.ToString();

            }
            catch { }
        }

        private void ShowPartyMasterDetail(bool _bStatus)
        {
            try
            {
                Active_InactiveCustomers obj = new SSS.Active_InactiveCustomers();
             
                if (_bStatus)
                    obj.rdoActive.Checked = true;
                else
                    obj.rdoInactive.Checked = true;
                
                obj.FormBorderStyle = FormBorderStyle.Fixed3D;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void lbl_Birthday_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                BirthdayDetails obj = new BirthdayDetails(true);
                obj.FormBorderStyle = FormBorderStyle.FixedDialog;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch { }
        }

        private void Lbl_Active_Cust_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowPartyMasterDetail(true);
        }

        private void Lbl_Inactive_Cust_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowPartyMasterDetail(false);
        }

        private void Lbl_Removel_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string strBillType = "SALES','PURCHASE','SALERETURN','PURCHASERETURN','SALESERVICE";
                RemovalReason obj = new RemovalReason(true, strBillType);
                //obj.chkDate.Checked = true;
                //obj.txtFromDate.Text = obj.txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                //obj.btnGo.PerformClick();
                obj.ShowInTaskbar = true;
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.Show();
            }
            catch { }
        }

        private void Lbl_Edited_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string strBillType = "SALES','PURCHASE','SALERETURN','PURCHASERETURN','SALESERVICE";
                EditLogReport obj = new EditLogReport(strBillType);
               // obj.chkDate.Checked = true;
               // obj.txtBillType.Text = "SALES";
               // obj.txtFromDate.Text = obj.txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                //obj.btnGo.PerformClick();
                obj.ShowInTaskbar = true;
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.Show();
            }
            catch { }
        }

        private void Lbl_Sundry_Creditor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDebtorCreditorAccount("SUNDRY CREDITOR");
        }

        private void ShowDebtorCreditorAccount(string strGroupName)
        {
            try
            {
                DebitorsCreditorsAccount obj = new DebitorsCreditorsAccount();
                obj.txtGroupName.Text = strGroupName;
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                //  obj.btnGo.PerformClick();
                obj.Show();
            }
            catch (Exception ex)
            { }
        }

        private void Lbl_Sundry_Debtors_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDebtorCreditorAccount("SUNDRY DEBTORS");
        }

        private void Lbl_Net_Balance_Bank_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowBankDetails();
        }

        private void ShowTodayBankDetails(string strStatus)
        {
            try
            {
                DayBookRegister obj = new SSS.DayBookRegister(MainPage.currentDate, MainPage.currentDate, "BANK A/C", strStatus);              
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowBankDetails()
        {
            try
            {               
                PrintMultiLedger obj = new PrintMultiLedger("BANK A/C");
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ShowBankDetailsGroup()
        {
            try
            {

                PrintMultiLedger obj = new PrintMultiLedger("BANK A/C");
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }       

        private void ShowCashAccountDetailsGroup()
        {
            try
            {
                PrintMultiLedger obj = new PrintMultiLedger("CASH A/C");
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowTodayCashAccountDetails(string strStatus)
        {
            try
            {
                DayBookRegister obj = new SSS.DayBookRegister(MainPage.currentDate, MainPage.currentDate, "CASH A/C", strStatus);
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Cash_Receipt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowCashAccountDetailsGroup();
        }

        private void Lbl_Mens_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDepartmentWiseSale("MENS");
        }

        private void ShowDepartmentWiseSale(string strType)
        {
            try
            {
                CustomSaleRegister objSale = new CustomSaleRegister();            
                objSale.ShowInTaskbar = true;
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.txtDepartment.Text = strType;
                objSale.chkDepartment.Checked =objSale._bSearchStatus=objSale.chkTaxableAmt.Checked= true;
                objSale.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Women_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDepartmentWiseSale("WOMENS");
        }

        private void Lbl_Kids_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDepartmentWiseSale("KIDS");
        }

        private void Lbl_Accessoriws_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowDepartmentWiseSale("ACCESSORIES");
        }

        private void Lbl_Stock_Qty_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowStockRegister();
        }

        private void ShowStockRegister()
        {
            try
            {
                StockRegister objStockRegister = new StockRegister();
                 objStockRegister.bShowRecord = true;               
                objStockRegister.ShowInTaskbar = true;
                objStockRegister.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objStockRegister.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Total_Sales_Daily_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                CustomSaleRegister objSale = new CustomSaleRegister();              
                objSale.ShowInTaskbar = true;
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.chkTaxableAmt.Checked = true;
                objSale.chkDate.Checked = true;
                objSale.txtFromDate.Text = objSale.txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                objSale._bSearchStatus = true;
                objSale.Show();
            }
            catch { }
        }

        private void Lbl__Brand_Profit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                BrandwiseProfit objBrandwiseProfit = new BrandwiseProfit("HIGH");
                objBrandwiseProfit.ShowInTaskbar = true;
                objBrandwiseProfit.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objBrandwiseProfit.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Min_Stock_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowMinMaxStock(true);
        }

        private void ShowMinMaxStock(bool _bStatus)
        {
            try
            {
                MinMaxBrandDetails objMinMaxBrandDetails = new MinMaxBrandDetails();
                if (_bStatus)
                    objMinMaxBrandDetails.rdoMin.Checked = true;
                else
                    objMinMaxBrandDetails.rdoMaxStock.Checked = true;

                objMinMaxBrandDetails.ShowInTaskbar = true;
                objMinMaxBrandDetails.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objMinMaxBrandDetails.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Max_Stock_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowMinMaxStock(false);
        }

        private void Lbl_Fastest_Brand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FastMovingBrand objFastMovingBrand = new FastMovingBrand("FAST");
                objFastMovingBrand.ShowInTaskbar = true;
                objFastMovingBrand.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objFastMovingBrand.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Cash_Balance_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowCashAccountDetailsGroup();
        }

        private void Lbl_Net_Balance_Bank_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowBankDetailsGroup();
        }

        private void Lbl_Total_Sales_Weekly_Count_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {               
                CustomSaleRegister objSale = new CustomSaleRegister();
                objSale.ShowInTaskbar = true;
                objSale.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objSale.chkTaxableAmt.Checked = true;
                objSale.chkDate.Checked = true;
                objSale.txtFromDate.Text = MainPage.currentDate.AddDays(-7).ToString("dd/MM/yyyy");
                objSale.txtToDate.Text = MainPage.currentDate.ToString("dd/MM/yyyy");
                objSale._bSearchStatus = true;
                objSale.Show();
            }
            catch { }
        }

        private void Lbl_Suspense_Amt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                PrintMultiLedger obj = new PrintMultiLedger("SUSPENCES A/C");
                obj.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                obj.ShowInTaskbar = true;
                obj.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Total_Reciept_Bank_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTodayBankDetails("DEBIT");
        }

        private void Lbl_Total_Payment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTodayBankDetails("CREDIT");
        }

        private void Lbl_Cash_Receipt_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTodayCashAccountDetails("DEBIT");
        }

        private void Lbl_Cash_Payment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTodayCashAccountDetails("CREDIT");
        }

        private void Lbl__Brand_Low_Profit_Click(object sender, EventArgs e)
        {
            try
            {
                BrandwiseProfit objBrandwiseProfit = new BrandwiseProfit("LOW");
                objBrandwiseProfit.ShowInTaskbar = true;
                objBrandwiseProfit.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objBrandwiseProfit.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Lbl_Slowest_Brand_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FastMovingBrand objFastMovingBrand = new FastMovingBrand("SLOW");
                objFastMovingBrand.ShowInTaskbar = true;
                objFastMovingBrand.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                objFastMovingBrand.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
