using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace SSS
{
    public partial class Dashboard_Sale_Details : Form
    {
        DataBaseAccess dba;
        public Dashboard_Sale_Details()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            GetDataFromDB();
        }

        private void Dashboard_Sale_Details_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }
        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }
        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.ReadOnly = txtToDate.ReadOnly = !chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void txtMonth_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("MONTH", "SEARCH MONTH NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtMonth.Text = objSearch.strSelectedData;
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string CreateSubQuery()
        {
            string strSQuery = "";
            if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
            {
                DateTime sDate = dba.ConvertDateInExactFormat(txtFromDate.Text), eDate = dba.ConvertDateInExactFormat(txtToDate.Text);
                eDate = eDate.AddDays(1);
                strSQuery += " and  (SDate >= '" + sDate.ToString("MM/dd/yyyy") + "' and SDate <'" + eDate.ToString("MM/dd/yyyy") + "') ";
            }

            if (txtMonth.Text != "")
            {
                strSQuery += " and UPPER(DATENAME(MM,SDate))='" + txtMonth.Text + "' ";
            }

            return strSQuery;
        }

        private void GetDataFromDB()
        {
            try {
                string strQuery = "", strSubQuery = CreateSubQuery();
                strQuery += "Select BillCode,BrandName,Station,_MonthName,SizeName,Marketer,SUM(Amount)Amt from ( "
                         + " Select SBS.BillCode,Station,_MonthName,Marketer,(CASE WHEN ISNULL(SBS.BrandName, '')= '' then _IM.BrandName else ISNULL(SBS.BrandName, '') end) BrandName,ISNULL(Variant2, '') as SizeName,Amount from SalesBookSecondary SBS inner join Items _IM on SBS.ItemName = _IM.ItemName OUTER APPLY (Select Station, Convert(char(3), SB.Date, 0)_MonthName,SB.Date as SDate  from SalesBook SB Where SBS.BillCode = SB.BillCode and SBS.BillNo = SB.BillNo)SB OUTER APPLY(Select Marketer from OrderBooking OB Where SBS.SONumber = (RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode)))OB Where  SBS.BillNo>0 " + strSubQuery + " UNION ALL "
                         + " Select SBS.BillCode,Station,_MonthName,Marketer,ISNULL(_IM.BrandName, '') as BrandName,'' SizeName,(GRD.Amount + ((GRD.Amount * (SBS.DiscountStatus + SBS.Discount)) / 100)) as Amount from SalesEntry SBS inner join GoodsReceiveDetails GRD on SBS.GRSNo = (GRD.ReceiptCode + ' ' + CAST(GRD.ReceiptNo as varchar)) inner join GoodsReceive GR on GR.ReceiptCode = GRD.ReceiptCode and GR.ReceiptNo = GRD.ReceiptNo inner join  Items _IM on GRD.ItemName = _IM.ItemName  OUTER APPLY (Select Station, Convert(char(3), SB.BillDate, 0)_MonthName,SB.BillDate as SDate from SalesRecord SB Where SBS.BillCode = SB.BillCode and SBS.BillNo = SB.BillNo)SB OUTER APPLY(Select Marketer from OrderBooking OB Where GR.OrderNo = (RTRIM(OB.OrderCode + ' ' + CAST(OB.OrderNo as varchar) + ' ' + OB.NumberCode)))OB  Where  SBS.BillNo>0 " + strSubQuery + " )_Sales Group by BrandName, BillCode, Station, _MonthName, SizeName, Marketer ";

                DataTable _dt = dba.GetDataTable(strQuery);
                BindWithControl(_dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void BindWithControl(DataTable _dt)
        {
            DataTable _dTable = _dt.DefaultView.ToTable(true, "BillCode");
            _dTable.Columns.Add("Amt", typeof(double));
            
            foreach(DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "BillCode='"+row["BillCode"] +"' ");
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, branchChart, "BillCode");

            _dTable = _dt.DefaultView.ToTable(true, "BrandName");
            _dTable.Columns.Add("Amt", typeof(double));

            foreach (DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "BrandName='" + row["BrandName"] + "' ");
            }

            ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, brandChart, "BrandName");

            _dTable = _dt.DefaultView.ToTable(true, "Station");
            _dTable.Columns.Add("Amt", typeof(double));

            foreach (DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "Station='" + row["Station"] + "' ");
            }

            DataView dv = _dTable.DefaultView;
            dv.Sort = "Amt desc";

            _dTable = dv.ToTable().AsEnumerable().Take(10).CopyToDataTable();

            ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, StationChart, "Station");

            _dTable = _dt.DefaultView.ToTable(true, "_MonthName");
            _dTable.Columns.Add("Amt", typeof(double));

            foreach (DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "_MonthName='" + row["_MonthName"] + "' ");
            }

            dv = _dTable.DefaultView;
            dv.Sort = "Amt desc";

            _dTable = dv.ToTable();//.AsEnumerable().Take(10).CopyToDataTable();

            ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, monthChart, "_MonthName");

            _dTable = _dt.DefaultView.ToTable(true, "SizeName");
            _dTable.Columns.Add("Amt", typeof(double));

            foreach (DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "SizeName='" + row["SizeName"] + "' ");
            }

            ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, sizeChart, "SizeName");

            _dTable = _dt.DefaultView.ToTable(true, "Marketer");
            _dTable.Columns.Add("Amt", typeof(double));

            foreach (DataRow row in _dTable.Rows)
            {
                row["Amt"] = _dt.Compute("SUM(Amt)", "Marketer='" + row["Marketer"] + "' ");
            }

            dv = _dTable.DefaultView;
            dv.Sort = "Amt desc";
            _dTable = dv.ToTable().AsEnumerable().Take(20).CopyToDataTable();

            ds = new DataSet();
            ds.Tables.Add(_dTable);
            BindRecordWithGrid(ds, salesmanChart, "Marketer");

        }//SizeName,,

        private void BindRecordWithGrid(DataSet _ds, Chart _Chart, string strValue)
        {
            _Chart.DataSource = _ds;
            _Chart.Series["Month"].XValueMember = strValue;
            _Chart.Series["Month"].YValueMembers = "Amt";

            _Chart.ChartAreas[0].AxisX.Interval = 1;
            _Chart.Series["Month"].ChartType = SeriesChartType.Column;
            if (_Chart.Titles.Count > 0)
                _Chart.Titles.RemoveAt(0);
         
            _Chart.DataBind();
        }

        private void btnGO_Click(object sender, EventArgs e)
        {
            GetDataFromDB();
        }
    }
}
