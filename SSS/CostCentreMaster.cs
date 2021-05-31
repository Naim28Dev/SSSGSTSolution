using System;
using System.Windows.Forms;

namespace SSS
{
    public partial class CostCentreMaster : Form
    {
        DataBaseAccess dba;
        //DataTable table;
        ChangeCurrencyToWord currency;
        
        public CostCentreMaster()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            currency = new ChangeCurrencyToWord();
           // GetCostCentreName();
            if (dgrdCashBook.Rows.Count == 0)
            {
                dgrdCashBook.Rows.Add(1);
            }
        }

        private void dgrdCashBook_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                TextBox txtCost = e.Control as TextBox;
                txtCost.CharacterCasing = CharacterCasing.Upper;
                if (dgrdCashBook.CurrentCell.ColumnIndex == 1)
                {
                    txtCost.KeyPress += new KeyPressEventHandler(txtDesc_KeyPress);
                }
                else if (dgrdCashBook.CurrentCell.ColumnIndex == 2)
                {
                    txtCost.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
                    txtCost.TextChanged += new EventHandler(txtBox_TextChanged);
                }
            }
            catch
            {
            }
        }

        private void txtDesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdCashBook.CurrentCell.ColumnIndex == 1)
            {
                TextBox txtBox = sender as TextBox;
                if (Char.IsWhiteSpace(e.KeyChar) && txtBox.Text.Length < 1)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgrdCashBook.CurrentCell.ColumnIndex == 2)
            {
                Char pressedKey = e.KeyChar;
                if (Char.IsLetter(pressedKey) || Char.IsSeparator(pressedKey) || Char.IsPunctuation(pressedKey) || Char.IsSymbol(pressedKey))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgrdCashBook.CurrentCell.ColumnIndex == 2)
                {
                    TextBox txtAmount = sender as TextBox;
                    if (txtAmount.Text != "")
                    {
                        lblCurrentAmount.Text = currency.changeCurrencyToWords(txtAmount.Text);
                    }
                    else
                    {
                        lblCurrentAmount.Text = "";
                    }
                }
            }
            catch
            {
            }
        }

        private void dgrdCashBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int currentRow = 0;
                int indexColumn = 0;
                int Index;
                if (e.KeyCode == Keys.Enter)
                {
                    Index = dgrdCashBook.CurrentCell.RowIndex;
                    indexColumn = dgrdCashBook.CurrentCell.ColumnIndex;
                    if (Index < dgrdCashBook.RowCount - 1)
                        currentRow = Index - 1;
                    else
                        currentRow = Index;
                    if (indexColumn < dgrdCashBook.ColumnCount - 1)
                    {
                        indexColumn += 1;
                        if (currentRow >= 0)
                            dgrdCashBook.CurrentCell = dgrdCashBook.Rows[currentRow].Cells[indexColumn];
                    }
                    else if (Index == dgrdCashBook.RowCount - 1)
                    {
                        string party = Convert.ToString(dgrdCashBook.Rows[currentRow].Cells[0].Value), strAmount = Convert.ToString(dgrdCashBook.Rows[currentRow].Cells[2].Value);
                        if (party != "" && strAmount != "")
                        {
                            dgrdCashBook.Rows.Add(1);
                            dgrdCashBook.CurrentCell = dgrdCashBook.Rows[dgrdCashBook.RowCount - 1].Cells[0];
                        }
                        else
                        {
                            btnOk.Focus();
                        }
                    }
                    else
                    {
                        dgrdCashBook.CurrentCell = dgrdCashBook.Rows[dgrdCashBook.RowCount - 1].Cells[0];
                    }
                }
                else if (e.KeyCode == Keys.F1)
                {
                    dgrdCashBook.Rows.RemoveAt(dgrdCashBook.CurrentRow.Index);
                    if (dgrdCashBook.Rows.Count == 0)
                    {
                        dgrdCashBook.Rows.Add(1);
                    }
                    CalculateTotalAmount();
                }
            }
            catch (Exception ex)
            {
                string[] strReport = { "Exception occurred on Gridview key Down in Cost Centre ", ex.Message };
                dba.CreateErrorReports(strReport);
            }
        }

        private void CalculateTotalAmount()
        {
            try
            {
                double dAmount = 0;
                foreach (DataGridViewRow row in dgrdCashBook.Rows)
                {
                    dAmount += dba.ConvertObjectToDouble(row.Cells["Amt"].Value);
                }

                lblBal.Text = dAmount.ToString("N2", MainPage.indianCurancy);
            }
            catch
            {
            }
        }       

        private void CostCentreMaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (ValidateControls())
                {
                    double dNetAmt = dba.ConvertObjectToDouble(lblNetAmt.Text), dAmt = dba.ConvertObjectToDouble(lblBal.Text);
                    if (dAmt != dNetAmt)
                    {
                        MessageBox.Show("Amount is not Adjusted! Please Adjust All Amount : " + lblNetAmt.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
            }
            catch
            {
            }
        }

        private void CostCentreMaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                this.Close();
            }
        }

        private void dgrdCashBook_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                CalculateTotalAmount();
            }
        }

        private void dgrdCashBook_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0)
                {
                    //PartyName objParty = new PartyName(table);
                    //objParty.ShowDialog();
                    //string strName = objParty.CostName;
                    //if (strName != "")
                    //{
                    //    dgrdCashBook.CurrentRow.Cells[0].Value = strName;
                    //}

                    SearchData objSearch = new SearchData("ALLPARTY", "Search Account Name", Keys.Space);
                    objSearch.ShowDialog();
                    if (objSearch.strSearchData != "")
                    {
                        dgrdCashBook.CurrentCell.Value = objSearch.strSelectedData;
                        if (dba.CheckTransactionLock(objSearch.strSelectedData))
                        {
                            MessageBox.Show("Transaction has been locked on this Account ! Please Select Different Account ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgrdCashBook.CurrentCell.Value = "";
                        }
                    }
                    e.Cancel = true;
                }
            }
            catch
            {
            }
        }

        private bool ValidateControls()
        {
            bool vStatus = true;
            try
            {
                foreach (DataGridViewRow row in dgrdCashBook.Rows)
                {
                    if (Convert.ToString(row.Cells[0].Value) == "")
                    {
                        dgrdCashBook.Rows.Remove(row);
                    }
                    else if (dba.ConvertObjectToDouble(row.Cells[2].Value) == 0)
                    {
                        MessageBox.Show("Amount can't be zero  ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return vStatus;
                    }
                }
                if (dgrdCashBook.Rows.Count == 0)
                {
                    MessageBox.Show("Atleast one entry is required  ! ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgrdCashBook.Rows.Add(1);
                    return vStatus;
                }
            }
            catch
            {
            }
            return vStatus;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        //private void GetCostCentreName()
        //{
        //    table = dba.GetDataTable("Select Distinct CostType from CostMaster");
        //}

        //private void BindCostWithTextBox(TextBox txtBox)
        //{
        //    try
        //    {
        //        AutoCompleteStringCollection objAuto = new AutoCompleteStringCollection();
        //        foreach (DataRow row in table.Rows)
        //        {
        //            objAuto.Add(Convert.ToString(row[0]));
        //        }
        //        txtBox.AutoCompleteMode = AutoCompleteMode.Suggest;
        //        txtBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //        txtBox.AutoCompleteCustomSource = objAuto;
        //    }
        //    catch
        //    {
        //    }
        //}

    }
}
