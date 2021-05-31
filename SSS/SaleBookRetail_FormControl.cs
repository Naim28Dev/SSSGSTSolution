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
    public partial class SaleBookRetail_FormControl : Form
    {
        DataBaseAccess dba;
        public SaleBookRetail_FormControl()
        {
            InitializeComponent();
            dba = new DataBaseAccess();          
            BindIndexingRecord();
            BindMendatoryRecord();
        }

        private void BindIndexingRecord()
        {
            string StrQuery = "Select * from RetailSaleBook_FormControl order by IndexNo asc,ColumnName Asc";

            DataSet ds = DataBaseAccess.GetDataSetRecord(StrQuery);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                dgrdDetails.Rows.Clear();
                int rowIndex = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdDetails.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdDetails.Rows[rowIndex].Cells["id"].Value = row["ID"];
                        dgrdDetails.Rows[rowIndex].Cells["ColumnName"].Value = row["Columnname"];
                        dgrdDetails.Rows[rowIndex].Cells["index"].Value = row["IndexNo"];

                        rowIndex++;
                    }
                }

                if (dgrdDetails.Rows.Count > 0)
                    dgrdDetails.CurrentCell = dgrdDetails.Rows[dgrdDetails.Rows.Count - 1].Cells["index"];
            }
        }

        private void BindMendatoryRecord()
        {
            string StrQuery = "select ID,MendatoryStatus,ColumnName from RetailSaleBook_FormControl where MendatoryFields='Mendatory' order by MendatoryStatus asc,ColumnName Asc";

            DataSet ds = DataBaseAccess.GetDataSetRecord(StrQuery);
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                dgrdMendatoryDetail.Rows.Clear();
                int rowIndex = 0;
                if (dt.Rows.Count > 0)
                {
                    dgrdMendatoryDetail.Rows.Add(dt.Rows.Count);
                    foreach (DataRow row in dt.Rows)
                    {
                        dgrdMendatoryDetail.Rows[rowIndex].Cells["sno"].Value = row["ID"];
                        dgrdMendatoryDetail.Rows[rowIndex].Cells["MendColumn"].Value = row["Columnname"];
                        dgrdMendatoryDetail.Rows[rowIndex].Cells["chkMend"].Value = row["MendatoryStatus"];

                        rowIndex++;
                    }
                }
                if (dgrdMendatoryDetail.Rows.Count > 0)
                    dgrdMendatoryDetail.CurrentCell = dgrdMendatoryDetail.Rows[dgrdMendatoryDetail.Rows.Count - 1].Cells["chkMend"];
            }
        }

        private void SaleBookRetail_FormControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgrdDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                    e.Cancel = true;
            }
            catch
            { }
        }

        private void dgrdDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex > 1)
                {
                    TextBox txtBox = (TextBox)e.Control;
                    txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                }
            }
            catch
            { }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int columnIndex = dgrdDetails.CurrentCell.ColumnIndex;
                if (columnIndex > 1)
                {
                    dba.KeyHandlerPoint(sender, e, 2);
                }
            }
            catch { }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                string StrQuery = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    StrQuery += "Update RetailSalebook_formcontrol set IndexNo ='" + row.Cells["index"].Value + "' where ID='" + row.Cells["id"].Value + "' and ColumnName='" + row.Cells["ColumnName"].Value + "'";
                }
                StrQuery += " ";
                result = dba.ExecuteMyQuery(StrQuery);
                if (result > 0)
                {
                    MessageBox.Show("Thank you ! Form Indexing saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    BindIndexingRecord();                   
                }
                else
                    MessageBox.Show("Sorry ! Record not saved. Please try again.");
            }
            catch (Exception ex)
            { }
        }

        private void btnMendatory_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                string StrQuery = "update RetailSaleBook_FormControl set MendatoryStatus='False' where MendatoryFields='Mendatory' ";
                foreach (DataGridViewRow row in dgrdMendatoryDetail.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["chkMend"].Value) == true)
                    {
                        StrQuery += "Update RetailSaleBook_FormControl set MendatoryStatus='" + Convert.ToBoolean(row.Cells["chkMend"].Value) + "' where ID='" + row.Cells["sno"].Value + "' and ColumnName='" + row.Cells["MendColumn"].Value + "' and MendatoryFields='Mendatory' ";
                    }
                }
                StrQuery += " ";
                result = dba.ExecuteMyQuery(StrQuery);
                if (result > 0)
                {
                    MessageBox.Show("Thank you ! Mendatory Fields Saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    BindMendatoryRecord();                    
                }
                else
                    MessageBox.Show("Sorry ! Record not saved. Please try again.");
            }
            catch (Exception ex)
            { }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            int indexer = 0;
            foreach (DataGridViewRow row in dgrdMendatoryDetail.Rows)
            {
                dgrdMendatoryDetail.Rows[indexer].Cells["chkMend"].Value = chkAll.Checked;
                indexer++;
            }
        }

        private void dgrdMendatoryDetail_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 2)
                    e.Cancel = true;
            }
            catch
            { }
        }

        private void btnResetIndexing_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                string StrQuery = "";
                foreach (DataGridViewRow row in dgrdDetails.Rows)
                {
                    StrQuery += "Update RetailSalebook_formcontrol set IndexNo =0";
                }
                StrQuery += " ";
                result = dba.ExecuteMyQuery(StrQuery);
                if (result > 0)
                {
                    MessageBox.Show("Thank you ! Form Indexing Reset successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    BindIndexingRecord();                   
                }
                else
                    MessageBox.Show("Sorry ! Record not saved. Please try again.");
            }
            catch (Exception ex)
            { }
        }

        private void btnResetmendatory_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                string StrQuery = "update RetailSaleBook_FormControl set MendatoryStatus='False' where MendatoryFields='Mendatory' ";
                result = dba.ExecuteMyQuery(StrQuery);
                if (result > 0)
                {
                    MessageBox.Show("Thank you ! Mendatories Reset successfully","Success",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
                    BindMendatoryRecord();                    
                }
                else
                    MessageBox.Show("Sorry ! Record not saved. Please try again.");
            }
            catch (Exception ex)
            { }
        }
    }
}
