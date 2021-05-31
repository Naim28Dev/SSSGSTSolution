using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace SSS
{
    public partial class PinCodeDistanceRegister : Form
    {
        DataBaseAccess dba;
        DataTable objTable;
        public PinCodeDistanceRegister()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
        }

        private void CustomPetiDetails_Load(object sender, EventArgs e)
        {
            ClearAllFields();
            GetAllRecord();
        }
        private void ClearAllFields()
        {
            txtPinCode.Text = "";
            txtDistance.Text = "";
        }
        private void GetAllRecord(string filterPin = "", string filterDis = "")
       {
            try
            {
                string strQuery = "", whereCon = "";
                if (filterPin == "")
                    whereCon = "";
                else
                    whereCon = " (FromPinCode like '%" + filterPin + "%' OR ToPinCode like '%" + filterPin + "%') ";

                if (filterDis == "")
                    whereCon += "";
                else if (whereCon != "" && filterDis != "")
                    whereCon += " AND Distance >= " + filterDis;
                else if (whereCon == "")
                    whereCon += " Distance >= " + filterDis;

                if (whereCon != "")
                    whereCon = " WHERE " + whereCon;
                else
                    whereCon = "";

                strQuery = "SELECT ID,FromPinCode,ToPinCode,Distance,Convert(varchar,[Date],103) as EntryDate FROM PinCodeDistance " + whereCon + " ORDER BY [Date]";

                objTable = DataBaseAccess.GetDataTableRecord(strQuery);
                grdPinCodeD.Rows.Clear();
                if (objTable.Rows.Count > 0)
                {
                    grdPinCodeD.Rows.Add(objTable.Rows.Count);
                    int index = 0;
                    foreach (DataRow row in objTable.Rows)
                    {
                        grdPinCodeD.Rows[index].Cells[0].Value = row["ID"];
                        grdPinCodeD.Rows[index].Cells[1].Value = index + 1;
                        grdPinCodeD.Rows[index].Cells[2].Value = row["FromPinCode"];
                        grdPinCodeD.Rows[index].Cells[3].Value = row["ToPinCode"];
                        grdPinCodeD.Rows[index].Cells[4].Value = row["Distance"];
                        grdPinCodeD.Rows[index].Cells[5].Value = row["EntryDate"];
                        index++;
                    }
                }
            }
            catch
            {
            }
        }

        private void grdCPD_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 6)
            {
                try
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to update distance", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {                       
                        int dis = Convert.ToInt32(grdPinCodeD.Rows[e.RowIndex].Cells[4].Value);
                        int ID = Convert.ToInt32(grdPinCodeD.Rows[e.RowIndex].Cells[0].Value);
                        if (dis <= 4000)
                        {
                            string strQuery = "if exists (SELECT ID FROM PinCodeDistance where ID = " + ID + ") begin";
                            strQuery += " Update PinCodeDistance Set Distance = " + dis
                                + " ,[Date]='" + MainPage.currentDate.ToString("MM/dd/yyyy")
                                + "' where ID = " + ID
                                + " end ";

                            int _count;
                            if (strQuery != "")
                            {
                                _count = dba.ExecuteMyQuery(strQuery);
                            }
                            // ClearAllFields();
                            GetAllRecord(txtPinCode.Text, txtDistance.Text);
                            MessageBox.Show("Thanks you ! Record updated successfully.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("Distance can't be greater than 4000 .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Somthing went wrong, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }


            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 7)
            {
                try
                {
                    DialogResult dr = MessageBox.Show("Are you sure want to Delete Record", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        string strQuery = " if exists (SELECT [ID] FROM PinCodeDistance Where ID='" + Convert.ToString(grdPinCodeD.Rows[e.RowIndex].Cells[0].Value) + "' ) begin ";

                        strQuery += " delete from PinCodeDistance where ID=" + Convert.ToString(grdPinCodeD.Rows[e.RowIndex].Cells[0].Value) + " end";

                        int count = dba.ExecuteMyQuery(strQuery);
                        ClearAllFields();
                        GetAllRecord();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Somthing went wrong, please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PinCodeDistanceRegister_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 27)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !grdPinCodeD.Focused)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred in Form's Key Down in Pin Code Distance Register ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void txtDistance_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.KeyHandlerPoint(sender, e, 2);
        }

        private void grdPinCodeD_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 4)
                e.Cancel = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetAllRecord(txtPinCode.Text, txtDistance.Text);
        }

        private void grdPinCodeD_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (grdPinCodeD.CurrentCell.ColumnIndex == 4)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.CharacterCasing = CharacterCasing.Upper;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                    //txtBox.TextChanged += new EventHandler(txtBox_TextChanged);
                }
            }
            catch
            {
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdPinCodeD.CurrentCell.ColumnIndex == 4)
                dba.KeyHandlerPoint(sender, e, 0);
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            //if (grdPinCodeD.CurrentCell.ColumnIndex == 4)
            //{
            //    TextBox txt = (TextBox)sender;
            //    if (txt.Text != "")
            //        lblCurrentAmount.Text = objCurrency.changeCurrencyToWords(dba.ConvertObjectToDouble(txt.Text));
            //    else
            //        lblCurrentAmount.Text = "";
            //}
        }
        private void applyeFilter()
        {
            DataRow[] row = null;
            
            if (txtDistance.Text != "" && txtPinCode.Text == "")
            {
                row = objTable.Select("Distance >= " + txtDistance.Text);
            }
            else if (txtPinCode.Text != "" && txtDistance.Text == "")
            {
                row = objTable.Select("FromPinCode Like '%" + txtPinCode.Text + "%' OR ToPinCode Like '%" + txtPinCode.Text + "%'");
            }
            else if (txtDistance.Text != "" && txtPinCode.Text != "")
            {
                row = objTable.Select("Distance >= " + txtDistance.Text + " AND (FromPinCode Like '%" + txtPinCode.Text + "%' OR ToPinCode Like '%" + txtPinCode.Text + "%')");
            }
            grdPinCodeD.Rows.Clear();
            if (row != null && row.Length > 0)
            {
                if (objTable.Rows.Count > 0)
                {
                    grdPinCodeD.Rows.Add(row.Length);
                    BindRecords(row);
                }
            }
        }

        private void BindRecords(DataRow[] Rows)
        {
            int index = 0;
            foreach (DataRow ro in Rows)
            {
                grdPinCodeD.Rows[index].Cells[0].Value = ro["ID"];
                grdPinCodeD.Rows[index].Cells[1].Value = index + 1;
                grdPinCodeD.Rows[index].Cells[2].Value = ro["FromPinCode"];
                grdPinCodeD.Rows[index].Cells[3].Value = ro["ToPinCode"];
                grdPinCodeD.Rows[index].Cells[4].Value = ro["Distance"];
                grdPinCodeD.Rows[index].Cells[5].Value = ro["EntryDate"];
                index++;
            }
        }

        private void txtPinCode_TextChanged(object sender, EventArgs e)
        {
            if (txtPinCode.Text != "")
                applyeFilter();
            else
                GetAllRecord(txtPinCode.Text, txtDistance.Text);
        }

        private void txtDistance_TextChanged(object sender, EventArgs e)
        {
            if (txtDistance.Text != "")
                applyeFilter();
            else
                GetAllRecord(txtPinCode.Text,txtDistance.Text);
        }
    }
}
