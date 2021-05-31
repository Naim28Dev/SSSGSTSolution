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
    public partial class Incentive_Target : Form
    {
        DataBaseAccess dba;
        string strSelBrands = "", strSelItems = "", strSelBarcodes = "";
        public Incentive_Target()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            SetType();
            //SetSerialNo();
            BindLastRecord();
        }

        private void SetSerialNo()
        {
            try
            {
                DataTable dt = dba.GetDataTable("SELECT (ISNULL(MAX(BillNo),0)+1)BillNo FROM IncentiveDetails");
                if (dt.Rows.Count > 0)
                    txtBillNo.Text = Convert.ToString(dt.Rows[0]["BillNo"]);
            }
            catch (Exception ex)
            { }
        }

        private void txtFromDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 0);
        }

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            txtFromDate.Enabled = txtToDate.Enabled = chkDate.Checked;
            txtFromDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            txtToDate.Text = MainPage.endFinDate.ToString("dd/MM/yyyy");
        }

        private void chkSerial_CheckedChanged(object sender, EventArgs e)
        {
            txtFromMRP.Enabled = txtToMRP.Enabled = chkMRP.Checked;
            txtFromMRP.Text = txtToMRP.Text = "";
        }

        private void SetType()
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                GvBrand.ReadOnly = GvItem.ReadOnly = GvBarcode.ReadOnly = true;
                chkMRP.Checked = false;

                if (rdoByBrand.Checked)
                    AddNewRowInGV(GvBrand);
                else if (rdoByBarcode.Checked)
                    AddNewRowInGV(GvBarcode);
                else if (rdoByItem.Checked)
                    AddNewRowInGV(GvItem);
                else if (rdoByMRP.Checked)
                    chkMRP.Checked = true;
            }
        }
        private void AddNewRowInGV(DataGridView GV)
        {
            GV.ReadOnly = false;
            GV.Focus();
            if (GV.Rows.Count > 0)
            {
                if (Convert.ToString(GV[1, 0].Value) != "" && Convert.ToString(GV[1, 0].Value) != "Select New Here")
                {
                    GV.Rows.Insert(0);
                    GV.CurrentCell = GV.Rows[0].Cells[1];
                    GV.Rows[0].Cells[1].Value = "Select New Here";
                }
            }
            else
            {
                GV.Rows.Insert(0);
                GV.CurrentCell = GV.Rows[0].Cells[1];
                GV.Rows[0].Cells[1].Value = "Select New Here";
            }
        }
        private void rdoByBrand_CheckedChanged(object sender, EventArgs e)
        {
            SetType();
        }

        private void txtFromDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, true);
        }

        private void Incentive_Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
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
                        BindAllDataWithControl(txtBillNo.Text);
                    }
                }
            }
        }

        private void BindNextRecord()
        {
            try
            {
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from IncentiveDetails Where BillNo>" + txtBillNo.Text + " ");
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
                object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MAX(BillNo),'') from IncentiveDetails Where BillNo<" + txtBillNo.Text + " ");
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
            object objValue = DataBaseAccess.ExecuteMyScalar("Select ISNULL(MIN(BillNo),'') from IncentiveDetails ");
            string strSerialNo = Convert.ToString(objValue);
            if (strSerialNo != "" && strSerialNo != "0")
            {
                BindAllDataWithControl(strSerialNo);
            }
            else
                ClearAllText();
        }

        private bool ValidateControl()
        {
            if (rdoByBrand.Checked && strSelBrands == "")
            {
                MessageBox.Show("Sorry ! Please select any brand from list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkAllBrand.Focus();
                return false;
            }
            else if (rdoByItem.Checked && strSelItems == "")
            {
                MessageBox.Show("Sorry ! Please select any item from list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkAllItem.Focus();
                return false;
            }
            else if (rdoByBarcode.Checked && strSelBarcodes == "")
            {
                MessageBox.Show("Sorry ! Please select any barcode from list.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkAllBarcode.Focus();
                return false;
            }
            else if (rdoByMRP.Checked)
            {
                double dToMRP = dba.ConvertObjectToDouble(txtToMRP.Text);
                if (dToMRP == 0)
                {
                    MessageBox.Show("Sorry ! MRP range must be entered !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtToMRP.Focus();
                    return false;
                }
            }
            double dValue = dba.ConvertObjectToDouble(txtValue.Text);
            if (dValue == 0)
            {
                MessageBox.Show("Sorry ! Incentive can't blank  !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue.Focus();
                return false;
            }
            return true;
        }

        private void txtFromMRP_Enter(object sender, EventArgs e)
        {
            if (dba.ConvertObjectToDouble(txtFromMRP.Text) <= 0)
                txtFromMRP.Clear();
        }

        private void txtValue_Leave(object sender, EventArgs e)
        {
            if (dba.ConvertObjectToDouble(txtValue.Text) <= 0)
                txtValue.Clear();
        }

        private void BindAllDataWithControl(object objSerialNo)
        {
            try
            {
                string strQuery = "Select * ,Convert(varchar,StartDate,103)SDate,Convert(varchar,EndDate,103)EDate,Convert(varchar,Date,103)_Date  from [dbo].[IncentiveDetails] WHERE BillNo =" + objSerialNo ;
                strQuery += " select FilterName,FilterValue from IncentiveSecondary WHERE BillNo =" + objSerialNo + " Order by FilterName,FilterValue ";

                DataSet ds = dba.GetDataSet(strQuery);
                DataTable dt = ds.Tables[0];
                ClearAllText();
                DisableAllControls();
                
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    SetIncentiveOn(Convert.ToString(row["IncentiveOn"]));

                    if (Convert.ToString(row["IncentiveType"]) == "PER")
                        rdoPer.Checked = true;
                    else
                        rdoAmt.Checked = true;

                    if (Convert.ToString(row["SDate"]) != "")
                    {
                        chkDate.Checked = true;
                        txtFromDate.Text = Convert.ToString(row["SDate"]);
                        txtToDate.Text = Convert.ToString(row["EDate"]);
                    }
                    double dToMRP = dba.ConvertObjectToDouble(row["EndMRP"]);
                    if (dToMRP > 0)
                    {
                        chkMRP.Checked = true;
                        txtFromMRP.Text = Convert.ToString(row["StartMRP"]);
                        txtToMRP.Text = dToMRP.ToString("N2", MainPage.indianCurancy);
                    }
                    txtValue.Text = Convert.ToString(row["IncentivePer"]);
                    txtDate.Text = Convert.ToString(row["_Date"]);
                    txtBillCode.Text = Convert.ToString(row["BillCode"]);
                    txtBillNo.Text = Convert.ToString(row["BillNo"]);
                }

                DataTable dt2 = ds.Tables[1];
                if (rdoByBrand.Checked)
                    FillGVAndCheckItTrue(GvBrand, dt2,ref strSelBrands);
                else if (rdoByItem.Checked)
                    FillGVAndCheckItTrue(GvItem, dt2,ref strSelItems);
                else if (rdoByBarcode.Checked)
                    FillGVAndCheckItTrue(GvBarcode, dt2,ref strSelBarcodes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry ! " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FillGVAndCheckItTrue(DataGridView GV, DataTable DT,ref string selected)
        {
            try
            {
                GvBarcode.Rows.Clear();
                GvBrand.Rows.Clear();
                GvItem.Rows.Clear();
                selected = "";
                int index = 0;
                foreach (DataRow dr in DT.Rows)
                {
                    GV.Rows.Add(1);
                    GV.Rows[GV.Rows.Count - 1].Cells[0].Value = true;
                    GV.Rows[GV.Rows.Count - 1].Cells[1].Value = dr[1];
                    GV.Rows[GV.Rows.Count - 1].Cells[2].Value = index + 1;
                    selected += Convert.ToString(dr[1]);

                    index++;
                }
            }
            catch { }
        }
        private void GetAllIncentive()
        {
            try
            {
                string strQuery = "Select *,(BillCode + ' ' + Cast(BillNo as Varchar(20))) BillCodeNo,Convert(varchar,StartDate,103)SDate,Convert(varchar,EndDate,103)EDate,Convert(varchar,Date,103)_Date  from [dbo].[IncentiveDetails] Order by Date Desc";
                DataTable dt = dba.GetDataTable(strQuery);

                dgrdDetail.Rows.Clear();
                if (dt.Rows.Count > 0)
                {
                    dgrdDetail.Rows.Add(dt.Rows.Count);
                    int rowIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetail.Rows[rowIndex].Cells["BillCodeNo"].Value = row["BillCodeNo"];
                        dgrdDetail.Rows[rowIndex].Cells["incentiveOn"].Value = row["IncentiveOn"];
                        dgrdDetail.Rows[rowIndex].Cells["incentiveType"].Value = row["IncentiveType"];
                        dgrdDetail.Rows[rowIndex].Cells["incentiveValue"].Value = row["IncentivePer"];
                        dgrdDetail.Rows[rowIndex].Cells["startDate"].Value = row["SDate"];
                        dgrdDetail.Rows[rowIndex].Cells["endDate"].Value = row["EDate"];
                        dgrdDetail.Rows[rowIndex].Cells["startMRP"].Value = row["startMRP"];
                        dgrdDetail.Rows[rowIndex].Cells["endMRP"].Value = row["endMRP"];
                        dgrdDetail.Rows[rowIndex].Cells["date"].Value = row["_Date"];
                        dgrdDetail.Rows[rowIndex].Cells["createdBy"].Value = row["CreatedBy"];
                        dgrdDetail.Rows[rowIndex].Cells["branchCode"].Value = row["branchCode"];
                        dgrdDetail.Rows[rowIndex].Cells["ActionEdit"].Value = "Edit";
                        dgrdDetail.Rows[rowIndex].Cells["ActionDelete"].Value = "Delete";
                        rowIndex++;

                    }
                }
            }
            catch(Exception ex) { }
        }

        private void dgrdDetail_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 10)
                    {
                        string[] str = Convert.ToString(dgrdDetail.CurrentRow.Cells["BillCodeNo"].Value).Split(' ');
                        BindAllDataWithControl(str[1]);
                        Tabs.SelectedTab = INCPage;
                    }
                    else if (e.ColumnIndex == 11)
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            string strQuery = " Delete from IncentiveDetails Where (BillCode + ' ' + Cast(BillNo as Varchar(20))) = '" +Convert.ToString(dgrdDetail.CurrentRow.Cells["BillCodeNo"]) + "'";
                             strQuery += " Delete from IncentiveSecondary Where (BillCode + ' ' + Cast(BillNo as Varchar(20))) = '" + Convert.ToString(dgrdDetail.CurrentRow.Cells["BillCodeNo"]) + "'";
                            int _count = dba.ExecuteMyQuery(strQuery);
                            if (_count > 0)
                            {
                                MessageBox.Show("Thank you ! Record deleted successfully  !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                BindLastRecord();
                            }
                            else
                                MessageBox.Show("Sorry ! Unable to update right now, Please try again later  !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch { }
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
                    txtBillNo.ReadOnly = false;
                    ClearAllText();
                    EnableAllControls();
                    SetSerialNo();
                    txtDate.Focus();
                    btnDelete.Enabled = false;
                }
                else
                {
                    if (ValidateControl())
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to save this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRecord();
                            btnEdit.Text = "&Edit";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string GetIncentiveOn()
        {
            if (rdoByBrand.Checked)
                return "BRAND";
            else if (rdoByBarcode.Checked)
                return "BARCODE";
            else if (rdoByMRP.Checked)
                return "MRP";
            else if (rdoByItem.Checked)
                return "ITEM";
            else
                return "";
        }

        private void SaveRecord()
        {
            try
            {
                string strStartDate = "NULL", strEndDate = "NULL";
                if (chkDate.Checked && txtFromDate.Text.Length == 10 && txtToDate.Text.Length == 10)
                {
                    DateTime _date = dba.ConvertDateInExactFormat(txtFromDate.Text);
                    strStartDate = "'" + _date.ToString("MM/dd/yyyy") + "' ";
                    _date = dba.ConvertDateInExactFormat(txtToDate.Text);
                    strEndDate = "'" + _date.ToString("MM/dd/yyyy") + "' ";
                }
                double dStartMRP = 0, dEndMRP = 0, dValue = dba.ConvertObjectToDouble(txtValue.Text);
                if (chkMRP.Checked)
                {
                    dStartMRP = dba.ConvertObjectToDouble(txtFromMRP.Text);
                    dEndMRP = dba.ConvertObjectToDouble(txtToMRP.Text);
                }

                string strQuery = "", strIncOn = GetIncentiveOn(), strIncType = "PER";
                if (rdoAmt.Checked)
                    strIncType = "AMT";

                strQuery += " if exists (Select IncentiveOn from IncentiveDetails Where BillCode = '" + txtBillCode.Text + "' AND BillNo = " + txtBillNo.Text + ") begin "
                                + " UPDATE[dbo].[IncentiveDetails] SET  [IncentiveOn] ='" + strIncOn + "',[InentiveValue] = '',[IncentiveType] ='" + strIncType + "' ,[IncentivePer] =" + dValue + " ,[FilterByDate] ='" + chkDate.Checked.ToString() + "' ,[StartDate] =" + strStartDate + " ,[EndDate] =" + strEndDate + " ,[FilterByMRP] ='" + chkMRP.Checked.ToString() + "' ,[StartMRP] =" + dStartMRP + ",[EndMRP] =" + dEndMRP + " ,[Date] =DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) ,[UpdatedBy] = '" + MainPage.strLoginName + "',[UpdateStatus] = 1 WHERE BillCode = '" + txtBillCode.Text + "' AND BillNo = " + txtBillNo.Text + " end else begin  "
                                + " INSERT INTO [dbo].[IncentiveDetails] ([BillCode],[BillNo],[BranchCode],[IncentiveOn],[InentiveValue],[IncentiveType],[IncentivePer],[FilterByDate],[StartDate],[EndDate],[FilterByMRP],[StartMRP],[EndMRP],[Date],[CreatedBy],[UpdatedBy],[InsertStatus],[UpdateStatus]) VALUES "
                                + " ('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'" + MainPage.strBranchCode + "','" + strIncOn + "','','" + strIncType + "' ," + dValue + " ,'" + chkDate.Checked.ToString() + "' ," + strStartDate + "," + strEndDate + ",'" + chkMRP.Checked.ToString() + "' ," + dStartMRP + "," + dEndMRP + ",DATEADD(MINUTE,30,DATEADD(hh,5,GETUTCDATE())) ,'" + MainPage.strLoginName + "','',0,0) end ";

                strQuery += CreateSubQuery();

                int count = dba.ExecuteMyQuery(strQuery);
                if (count > 0)
                {
                    MessageBox.Show("Thank You! Record Saved Successfully .", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    btnAdd.Text = "&Add";
                    BindAllDataWithControl(txtBillNo.Text);
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
            string strSubQuery = "";

            if (GvBarcode.Rows.Count > 0 && rdoByBarcode.Checked)
            {
                foreach (DataGridViewRow ro in GvBarcode.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strSubQuery += " INSERT INTO IncentiveSecondary(BillCode,BillNo,FilterName,FilterValue,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                        + " VALUES('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'BARCODE','" + Convert.ToString(ro.Cells[1].Value) + "',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (GvItem.Rows.Count > 0 && rdoByItem.Checked)
            {
                foreach (DataGridViewRow ro in GvItem.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strSubQuery += " INSERT INTO IncentiveSecondary(BillCode,BillNo,FilterName,FilterValue,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                    + " VALUES('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'ITEM','" + Convert.ToString(ro.Cells[1].Value) + "',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }
            if (GvBrand.Rows.Count > 0 && rdoByBrand.Checked)
            {
                foreach (DataGridViewRow ro in GvBrand.Rows)
                {
                    if (Convert.ToBoolean(ro.Cells[0].Value))
                    {
                        strSubQuery += " INSERT INTO IncentiveSecondary(BillCode,BillNo,FilterName,FilterValue,InsertStatus,UpdateStatus,CreatedBy,UpdatedBy) "
                                    + " VALUES('" + txtBillCode.Text + "'," + txtBillNo.Text + ",'BRAND','" + Convert.ToString(ro.Cells[1].Value) + "',1,0,'" + MainPage.strLoginName + "','')";
                    }
                }
            }

            strSubQuery = " DELETE From IncentiveSecondary where BillCode = '" + txtBillCode.Text + "' AND BillNo = " + dba.ConvertObjectToDouble(txtBillNo.Text) + strSubQuery;

            return strSubQuery;
        }

        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void txtStartDate_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, false, false);
        }

        private void txtOfferDtFrom_Leave(object sender, EventArgs e)
        {
            dba.GetDateInExactFormat(sender, false, false, true);
        }

        private void getSelectedInGVAndClear(DataGridView GV, ref string Selected)
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
        private void checkUncheckGV(DataGridView GV, bool bchecked, ref string Selected)
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
            Selected = "";
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

        private void FillGVAndCheckItTrue(DataGridView GV, DataTable DT, string AlreadySelected)
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
                    GV.Rows[GV.Rows.Count - 1].Cells[2].Value = index + 1;

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

        private void BindGVForUpdate(DataGridView GV, DataTable DT, ref string SelectedInGV)
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
                    GV.Rows[index].Cells[2].Value = index + 1;

                    SelectedInGV += Convert.ToString(dr[0]);
                    index++;
                }
            }
            catch { }
        }

        private void EnableAllControls()
        {
            chkAllBarcode.Enabled = chkAllBrand.Enabled = chkAllItem.Enabled = btnBrandClear.Enabled = btnItemClear.Enabled = btnIncBarClear.Enabled = true;
            txtFromMRP.ReadOnly = txtToMRP.ReadOnly = txtBillCode.ReadOnly = txtBillNo.ReadOnly = txtToDate.ReadOnly = txtFromDate.ReadOnly = false;
            chkMRP.Enabled = grpBoxIncOn.Enabled = txtValue.Enabled = true;
            GvBrand.ReadOnly = GvItem.ReadOnly = GvBarcode.ReadOnly = false;
        }

        private void DisableAllControls()
        {
            chkAllBarcode.Enabled = chkAllBrand.Enabled = chkAllItem.Enabled = btnBrandClear.Enabled = btnItemClear.Enabled = btnIncBarClear.Enabled = false;
            txtFromMRP.ReadOnly = txtToMRP.ReadOnly = txtBillCode.ReadOnly = txtBillNo.ReadOnly = txtToDate.ReadOnly = txtFromDate.ReadOnly = true;
            chkMRP.Enabled = grpBoxIncOn.Enabled = txtValue.Enabled = false;
            GvBrand.ReadOnly = GvItem.ReadOnly = GvBarcode.ReadOnly = true;
        }

        private void ClearAllText()
        {
            if (DateTime.Today > MainPage.startFinDate)
                txtDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            else
                txtDate.Text = MainPage.startFinDate.ToString("dd/MM/yyyy");
            rdoPer.Checked = rdoByBrand.Checked = true;
            chkAllBarcode.Checked = chkAllBrand.Checked = chkAllItem.Checked = chkDate.Checked = chkMRP.Checked = false;
            txtValue.Text = "0.00";
            txtBarSearch.Text = txtBarSearch.Text = txtItemSearch.Text = txtFromMRP.Text = txtToMRP.Text = "";
            GvBrand.Rows.Clear();
            GvItem.Rows.Clear();
            GvBarcode.Rows.Clear();
        }

        private void BindLastRecord()
        {
            object objSerialNo = DataBaseAccess.ExecuteMyScalar("Select MAX(BillNo) from IncentiveDetails Where Billno!=0 ");
            if (Convert.ToString(objSerialNo) != "")
            {
                BindAllDataWithControl(objSerialNo);
            }
            else
            {
                ClearAllText();
            }
        }

        private void txtFromDate_Enter(object sender, EventArgs e)
        {
            dba.SelectInTextBox(sender, 0, 0);
        }

        private bool checkDuplicate(DataGridView GV, string Value)
        {
            bool status = true;
            if (GV.Rows.Count > 1)
            {
                status = true;
                int cnt = GV.Rows.Count;
                for (int i = 0; i < cnt; i++)
                {
                    if (Convert.ToString(GV[1, i].Value) == Value)
                    {
                        status = false;
                        break;
                    }
                }
            }
            if (!status)
                MessageBox.Show("Sorry ! Same Record already added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return status;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GvIncBarcode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    if (e.ColumnIndex == 1)
                    {
                        DataGridView GV = (DataGridView)sender;
                        BeginEdit(GV, e);
                    }
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex) { }
        }

        private void BeginEdit(DataGridView GV, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                SearchData objSearch = null;
                if (GV == GvBarcode)
                {
                    objSearch = new SearchData("BARCODEDETAILS", "SEARCH BARCODE NO", Keys.Space);
                }
                else if (GV == GvItem)
                {
                    objSearch = new SearchData("ITEMNAME", "SEARCH ITEM NAME", Keys.Space);
                }
                else if (GV == GvBrand)
                {
                    objSearch = new SearchData("BRANDNAME", "SEARCH BRAND NAME", Keys.Space);
                }

                objSearch.ShowDialog();
                string strValue = objSearch.strSelectedData;
                if (strValue != "")
                {
                    if (checkDuplicate(GV, strValue))
                    {
                        GV[0, e.RowIndex].Value = true;
                        GV[1, e.RowIndex].Value = strValue;

                        if (Convert.ToString(GV[1, 0].Value) != "" && Convert.ToString(GV[1, 0].Value) != "Select New Here")
                        {
                            GV.Rows.Insert(0);
                            GV.CurrentCell = GV.Rows[0].Cells[1];
                            GV.Rows[0].Cells[1].Value = "Select New Here";
                        }
                    }
                }
                e.Cancel = true;
                if (GV == GvBarcode)
                    strSelBarcodes += "," + strValue;
                else if (GV == GvItem)
                    strSelItems += "," + strValue;
                else if (GV == GvBrand)
                    strSelBrands += "," + strValue;
            }
            catch (Exception ex) { }
        }

        private void btnIncBarClear_Click(object sender, EventArgs e)
        {
            ClearGV(GvBarcode);
        }

        private void ClearGV(DataGridView GV)
        {
            try
            {
                if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
                {
                    for (int i = 0; i < GV.Rows.Count; i++)
                    {
                        if (!(Convert.ToBoolean(GV[0, i].Value)))
                        {
                            GV.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    GV.Rows.Insert(0);
                    GV.Focus();
                    GV.CurrentCell = GV.Rows[0].Cells[1];
                    GV.Rows[0].Cells[1].Value = "Select New Here";
                    GV.Columns[0].ReadOnly = false;
                }
            }
            catch (Exception ex) { }
        }

        private void GvIncBarcode_Scroll(object sender, ScrollEventArgs e)
        {
            chkAllBarcode.Visible = GvBarcode.HorizontalScrollingOffset == 0;
        }

        private void chkAllBrand_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(GvBrand, chkAllBrand.Checked, ref strSelBrands);
        }

        private void chkAllItem_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(GvItem, chkAllItem.Checked, ref strSelItems);
        }

        private void txtIncBarSearch_TextChanged(object sender, EventArgs e)
        {
        }

        private void chkIncBar_CheckedChanged(object sender, EventArgs e)
        {
            checkUncheckGV(GvBarcode, chkAllBarcode.Checked, ref strSelBarcodes);
        }

        private void txtIncItemSearch_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtIncBrandSearch_TextChanged(object sender, EventArgs e)
        {
        }

        private void FilterGV(DataGridView GV, object sender)
        {
            try
            {
                TextBox txtbox = (TextBox)sender;
                string filter = txtbox.Text.ToUpper();
                if (txtbox.TextLength > 0)
                {
                    foreach (DataGridViewRow dr in GV.Rows)
                    {
                        if (Convert.ToString(dr.Cells[1].Value).ToUpper().Contains(filter))
                        {
                            GV.CurrentCell = dr.Cells[1];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void Incentive_Target_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    e.Cancel = true;
            }
        }

        private void GvItem_Scroll(object sender, ScrollEventArgs e)
        {
            chkAllItem.Visible = GvItem.HorizontalScrollingOffset == 0;
        }

        private void GvBrand_Scroll(object sender, ScrollEventArgs e)
        {
            chkAllBrand.Visible = GvBrand.HorizontalScrollingOffset == 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnAdd.Text == "&Add" || btnEdit.Text == "&Edit")
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete record ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string strQuery = " Delete from IncentiveDetails Where BillCode = '" + txtBillCode.Text + "' + BillNo = " + txtBillNo.Text;
                        strQuery += " Delete from IncentiveSecondary Where BillCode = '" + txtBillCode.Text + "' + BillNo = " + txtBillNo.Text;
                        int _count = dba.ExecuteMyQuery(strQuery);
                        if (_count > 0)
                        {
                            MessageBox.Show("Thank you ! Record deleted successfully  !", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            BindLastRecord();
                        }
                        else
                            MessageBox.Show("Sorry ! Unable to update right now, Please try again later  !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch { }
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

        private void btnBrandClear_Click(object sender, EventArgs e)
        {
            ClearGV(GvBrand);
        }

        private void btnItemClear_Click(object sender, EventArgs e)
        {
            ClearGV(GvItem);
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
                    txtBillNo.ReadOnly = true;
                    btnDelete.Enabled = true;
                    btnEdit.Text = "&Update";
                    EnableAllControls();

                    if (rdoByBrand.Checked)
                        AddNewRowInGV(GvBrand);
                    else if (rdoByItem.Checked)
                        AddNewRowInGV(GvItem);
                    else if (rdoByBarcode.Checked)
                        AddNewRowInGV(GvBarcode);  
                }
                else
                {
                    if (ValidateControl())
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to update this record ? ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SaveRecord();
                            btnEdit.Text = "&Edit";
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

        private void txtBrandSearch_KeyUp(object sender, KeyEventArgs e)
        {
            FilterGV(GvBrand, sender);
        }

        private void txtItemSearch_KeyUp(object sender, KeyEventArgs e)
        {
            FilterGV(GvItem, sender);
        }

        private void txtBarSearch_KeyUp(object sender, KeyEventArgs e)
        {
            FilterGV(GvBarcode, sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            GetAllIncentive();
        }

        private void SetIncentiveOn(string strIncOn)
        {
            if (strIncOn == "BRAND")
                rdoByBrand.Checked = true;
            else if (strIncOn == "SUPPLIER")
                rdoBySupplier.Checked = true;
            else if (strIncOn == "BARCODE")
                rdoByBarcode.Checked = true;
            else if (strIncOn == "MRP")
                rdoByMRP.Checked = true;
            else if (strIncOn == "ITEM")
                rdoByItem.Checked = true;
        }
    }
}
