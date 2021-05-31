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
    public partial class EditTrailDetails : Form
    {
        DataBaseAccess dba;
        public EditTrailDetails(string strType,string strCode,string strSNo)
        {
            try
            {
                InitializeComponent();
                dba = new SSS.DataBaseAccess();
                int pageWidth = MainPage.mymainObject.Width, pageHeight = MainPage.mymainObject.Height;
                if (pageWidth == 0)
                    pageWidth = 1000;
                this.Location = new Point(pageWidth - 455, 30);

                GetDataFromDB(strType, strCode, strSNo);
                if (strType == "PAYMENTREQUEST" || strType == "GOODSPURCHASE" || strType == "SALES")
                {
                    dgrdDetails.Columns["updatedBy"].Width = 85;
                    dgrdDetails.Columns["netAmt"].Width = 75;
                    dgrdDetails.Columns["editStatus"].Visible = true;
                }
                if (strType == "DESIGNMASTER")
                {
                    dgrdDetails.Columns["remark"].Visible = true;
                    dgrdDetails.Columns["netAmt"].Visible = false;
                }
            }
            catch { }
        }

        private void EditTrailDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {               
                    this.Close();
            }
        }

        private void GetDataFromDB(string strType, string strCode, string strSNo)
        {
            try
            {
                dgrdDetails.Rows.Clear();
                if (strCode != "" && strSNo != "")
                {
                    DataTable dt = dba.GetDataTable("Select *,CONVERT(varchar,Date,100)EDate,Reason from [EditTrailDetails] Where [BillType]='" + strType + "' and [BillCode]='" + strCode + "' and [BillNo]=" + strSNo + " Order by Date desc ");
                    if (dt.Rows.Count > 0)
                    {
                        dgrdDetails.Rows.Add(dt.Rows.Count);
                        int _rowIndex = 0;
                        string strEditStatus = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            strEditStatus = Convert.ToString(row["EditStatus"]);
                            dgrdDetails.Rows[_rowIndex].Cells["date"].Value = row["EDate"];
                            //if (strEditStatus == "PRINTED")
                            //    dgrdDetails.Rows[_rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"] + "(PRINT)";
                            //else
                            dgrdDetails.Rows[_rowIndex].Cells["updatedBy"].Value = row["UpdatedBy"];
                            dgrdDetails.Rows[_rowIndex].Cells["netAmt"].Value = row["NetAmt"];
                            dgrdDetails.Rows[_rowIndex].Cells["remark"].Value = row["Reason"];
                            dgrdDetails.Rows[_rowIndex].Cells["editStatus"].Value = row["EditStatus"];
                            _rowIndex++;
                        }
                    }
                    else
                        this.Close();
                }

            }
            catch { }
        }
    }
}
