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
    public partial class LayoutMaster : Form
    {
        DataBaseAccess dba;
        string sbrPrint = "";
        public LayoutMaster()
        {
            InitializeComponent();

            dba = new DataBaseAccess();
            GetLayoutStatus();
        }
        private void GetLayoutStatus()
        {
            object objValue = DataBaseAccess.ExecuteMyScalar("select Layout from PrintLayoutMaster");
            sbrPrint = Convert.ToString(objValue);

            if (sbrPrint.Contains("T"))
            {
                rdThermalPage.Checked = true;
                rdFullPage.Checked = rdQuarterPage.Checked = rdHalfPage.Checked = false;
                pnlThermal.Visible = true;
                pnlFullPage.Visible = pnlQuarter.Visible = pnlHalfPage.Visible = false;
            }
            else
            {
                chkThermalT5.Visible = chkThermal3.Visible = chkThermalT4.Visible = chkThermal1.Visible = lblT3.Visible = lblT5.Visible = lblT4.Visible = lblT1.Visible = false;
                picBoxT5.Visible = picBoxT3.Visible = picBoxT4.Visible = picBoxT1.Visible = false;
            }



            if (sbrPrint == "F")
            {
                rdFullPage.Checked = true;
                rdHalfPage.Checked = rdQuarterPage.Checked = rdThermalPage.Checked = false;
                pnlFullPage.Visible = true;
                pnlHalfPage.Visible = pnlQuarter.Visible = pnlThermal.Visible = false;
                chkFullPage.Checked = true;
                chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = chkThermalT5.Checked = false;
            }
            else if (sbrPrint == "H")
            {
                rdHalfPage.Checked = true;
                rdFullPage.Checked = rdQuarterPage.Checked = rdThermalPage.Checked = false;
                pnlHalfPage.Visible = true;
                pnlFullPage.Visible = pnlQuarter.Visible = pnlThermal.Visible = false;
                chkHalfPage.Checked = true;
                chkFullPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = chkThermalT5.Checked = false;
            }
            else if (sbrPrint == "Q")
            {
                rdQuarterPage.Checked = true;
                rdFullPage.Checked = rdHalfPage.Checked = rdThermalPage.Checked = false;
                pnlQuarter.Visible = true;
                pnlFullPage.Visible = pnlHalfPage.Visible = pnlThermal.Visible = false;
                chkQuarter.Checked = true;
                chkFullPage.Checked = chkHalfPage.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = chkThermalT5.Checked = false;
            }
            else if (sbrPrint == "T1")
            {
                chkThermal1.Checked = true;
                chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal3.Checked = chkThermalT4.Checked = chkThermalT5.Checked = false;
                chkThermal3.Visible = chkThermalT4.Visible = chkThermalT5.Visible = lblT3.Visible = lblT4.Visible = lblT5.Visible = false;
                picBoxT3.Visible = picBoxT4.Visible = picBoxT5.Visible = false;
            }
            //else if (sbrPrint =="T2")
            //{
            //    chkThermal1.Checked = true;
            //    chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal2.Checked = chkThermalT3.Checked = false;
            //}
            else if (sbrPrint == "T3")
            {
                chkThermal3.Checked = true;
                chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermalT4.Checked = chkThermalT5.Checked = false;
                chkThermal1.Visible = chkThermalT4.Visible = chkThermalT5.Visible = lblT1.Visible = lblT4.Visible = lblT5.Visible = false;
                picBoxT1.Visible = picBoxT4.Visible = picBoxT5.Visible = false;
            }
            else if (sbrPrint == "T4")
            {
                chkThermalT4.Checked = true;
                chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT5.Checked = false;
                chkThermal3.Visible = chkThermal1.Visible = chkThermalT5.Visible = lblT3.Visible = lblT1.Visible = lblT5.Visible = false;
                picBoxT3.Visible = picBoxT1.Visible = picBoxT5.Visible = false;
            }
            else if (sbrPrint == "T5")
            {
                chkThermalT5.Checked = true;
                chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
                chkThermal3.Visible = chkThermalT4.Visible = chkThermal1.Visible = lblT3.Visible = lblT4.Visible = lblT1.Visible = false;
                picBoxT3.Visible = picBoxT4.Visible = picBoxT1.Visible = false;
            }
            if (MainPage.strLoginName.Contains("SUPERADMIN"))
            {
                // chkThermalT5.Checked = chkFullPage.Checked = chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
                chkThermalT5.Visible = chkThermal3.Visible = chkThermalT4.Visible = chkThermal1.Visible = lblT3.Visible = lblT5.Visible = lblT4.Visible = lblT1.Visible = true;
                picBoxT5.Visible = picBoxT3.Visible = picBoxT4.Visible = picBoxT1.Visible = true;
            }
        }

        private void LayoutMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LayoutMaster_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private DataTable CreateDataTable()
        {
            DataTable myDataTable = new DataTable();
            try
            {
                myDataTable.Columns.Add("HeaderImage", typeof(byte[]));
                myDataTable.Columns.Add("HeaderLogo", typeof(byte[]));
                myDataTable.Columns.Add("CompanyName", typeof(String));
                myDataTable.Columns.Add("UserName", typeof(String));


                DataRow row = myDataTable.NewRow();
                row["CompanyName"] = MainPage.strCompanyName;
                row["HeaderImage"] = MainPage._headerImage;
                row["HeaderLogo"] = MainPage._brandLogo;
                row["UserName"] = MainPage.strLoginName;


                myDataTable.Rows.Add(row);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myDataTable;
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox2.Enabled = false;

            {
                Reporting.CryFullPage objReport = new Reporting.CryFullPage();
                Reporting.ShowReport objShow = new Reporting.ShowReport("Full PAGE PRINT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();
                objReport.Close();
                objReport.Dispose();
            }
            pictureBox2.Enabled = true;
        }



        private void chkFullPage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFullPage.Checked)
            {
                chkHalfPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            string SBR = "", strQuery = "";
            if (chkFullPage.Checked)
                SBR = "F";
            else if (chkHalfPage.Checked)
                SBR = "H";
            else if (chkQuarter.Checked)
                SBR = "Q";
            else if (chkThermal1.Checked)
                SBR = "T1";
            //else if (chkThermal1.Checked)
            //    SBR = "T2";
            else if (chkThermal3.Checked)
                SBR = "T3";
            else if (chkThermalT4.Checked)
                SBR = "T4";
            else if (chkThermalT5.Checked)
                SBR = "T5";
            else
                SBR = "F";


            if (SBR != "")
            {
                strQuery = "if not exists(select * from PrintLayoutMaster) begin insert into PrintLayoutMaster values('SaleBook_Retail','" + SBR + "','" + MainPage.strLoginName + "') end else begin Update PrintLayoutMaster set Layout='" + SBR + "', UpdatedBy='" + MainPage.strLoginName + "' end ";

                //strQuery = "Update PrintLayoutMaster set Layout='"+SBR+"', UpdatedBy='"+MainPage.strLoginName+"'";
                int Result = dba.ExecuteMyQuery(strQuery);
                if (Result > 0)
                {
                    MainPage.strPrintLayout = SBR;
                    MessageBox.Show("Thank you !! Layout updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                    MessageBox.Show("Sorry !! Unable to update right now", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void rdFullPage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdFullPage.Checked)
            {
                rdHalfPage.Checked = rdQuarterPage.Checked = rdThermalPage.Checked = false;
                pnlFullPage.Visible = true;
                pnlHalfPage.Visible = pnlQuarter.Visible = pnlThermal.Visible = false;
            }
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            pictureBox12.Enabled = true;
            DataTable dt = CreateDataTable();
            if (dt.Rows.Count > 0)
            {
                Reporting.CryHalfPage objReport = new Reporting.CryHalfPage();
                objReport.SetDataSource(dt);
                Reporting.ShowReport objShow = new Reporting.ShowReport("HALF PAGE PRINT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();

                objReport.Close();
                objReport.Dispose();
            }
            else
                MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            pictureBox12.Enabled = true;
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            pictureBox11.Enabled = true;
            DataTable dt = CreateDataTable();
            if (dt.Rows.Count > 0)
            {
                Reporting.CryQuarterPage objReport = new Reporting.CryQuarterPage();
                objReport.SetDataSource(dt);
                Reporting.ShowReport objShow = new Reporting.ShowReport("QUARTER PAGE PRINT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();

                objReport.Close();
                objReport.Dispose();
            }
            else
                MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            pictureBox11.Enabled = true;
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            picBoxT1.Enabled = true;
            DataTable dt = CreateDataTable();
            if (dt.Rows.Count > 0)
            {
                Reporting.CryThermalPage objReport = new Reporting.CryThermalPage();
                objReport.SetDataSource(dt);
                Reporting.ShowReport objShow = new Reporting.ShowReport("THERMAL PAGE PRINT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();

                objReport.Close();
                objReport.Dispose();
            }
            else
                MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            picBoxT1.Enabled = true;
        }

        private void rdHalfPage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdHalfPage.Checked)
            {
                rdFullPage.Checked = rdQuarterPage.Checked = rdThermalPage.Checked = false;
                pnlFullPage.Visible = pnlQuarter.Visible = pnlThermal.Visible = false;
                pnlHalfPage.Visible = true;
            }
        }

        private void rdQuarterPage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdQuarterPage.Checked)
            {
                rdFullPage.Checked = rdHalfPage.Checked = rdThermalPage.Checked = false;
                pnlQuarter.Visible = true;
                pnlFullPage.Visible = pnlHalfPage.Visible = pnlThermal.Visible = false;
            }
        }

        private void rdThermalPage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdThermalPage.Checked)
            {
                rdFullPage.Checked = rdQuarterPage.Checked = rdHalfPage.Checked = false;
                pnlThermal.Visible = true;
                pnlFullPage.Visible = pnlQuarter.Visible = pnlHalfPage.Visible = false;
            }
        }

        private void chkHalfPage_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkHalfPage.Checked)
            {
                chkFullPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = false;
            }
        }

        private void chkQuarter_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkQuarter.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkThermal1.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
            }
        }

        private void chkThermal_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkThermal1.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkQuarter.Checked = chkThermalT5.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
            }
        }

        private void chkThermal1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkThermal1.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkQuarter.Checked = chkThermalT5.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
            }
        }

        private void chkThermal2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkThermal3.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermalT5.Checked = chkThermalT4.Checked = false;
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            //    pictureBox8.Enabled = true;
            //    DataTable dt = CreateDataTable();
            //    if (dt.Rows.Count > 0)
            //    {
            //        Reporting.CryThermal1 objReport = new Reporting.CryThermal1();
            //        objReport.SetDataSource(dt);
            //        Reporting.ShowReport objShow = new Reporting.ShowReport("THERMAL PAGE PRINT PREVIEW");
            //        objShow.myPreview.ReportSource = objReport;
            //        objShow.ShowDialog();

            //        objReport.Close();
            //        objReport.Dispose();
            //    }
            //    else
            //        MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    pictureBox8.Enabled = true;
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            picBoxT3.Enabled = true;
            DataTable dt = CreateDataTable();
            if (dt.Rows.Count > 0)
            {
                Reporting.CryThermal2rpt objReport = new Reporting.CryThermal2rpt();
                objReport.SetDataSource(dt);
                Reporting.ShowReport objShow = new Reporting.ShowReport("THERMAL PAGE PRINT PREVIEW");
                objShow.myPreview.ReportSource = objReport;
                objShow.ShowDialog();

                objReport.Close();
                objReport.Dispose();
            }
            else
                MessageBox.Show("Sorry ! Please select record !! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            picBoxT3.Enabled = true;
        }

        private void chkThermalT4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkThermalT4.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermalT5.Checked = chkThermal3.Checked = false;
            }
        }

        private void chkThermalT4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkThermalT5.Checked)
            {
                chkHalfPage.Checked = chkFullPage.Checked = chkQuarter.Checked = chkThermal1.Checked = chkThermal3.Checked = chkThermalT4.Checked = false;
            }
        }
    }
}
