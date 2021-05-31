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
    public partial class PartyName : Form
    {
        DataBaseAccess dba;        
        string strParty = "";
        CashBook cashObj;
        DataTable table = null;
       
        public PartyName()
        {
            InitializeComponent();
            dba = new DataBaseAccess();          
            BindCollectionwithText();
        }

        public PartyName(DataTable myTable)
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            table = myTable;
            BindCollectionwithText();
        }    

        

        private void BindCollectionwithText()
        {
            AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
         //   DataTable partyTable = dba.GetDataTable("Select * from SupplierMaster where GroupName not in ('SUB PARTY','CASH A/C') order by Name");
            try
            {
                if (txtName.Text == "")
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        namesCollection.Add(dr[0].ToString());
                    }
                }
                namesCollection.Add("ADD NEW COST TYPE");
                txtName.AutoCompleteCustomSource = namesCollection;
            }
            catch
            {
            }
        }
      
        public string CostName
        {
            get
            {
                return strParty;
            }
        }    

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode==Keys.Tab)
            {
                if (txtName.Text == "ADD NEW COST TYPE")
                {
                    this.Hide();
                    CostMaster objCostMaster = new CostMaster(1);
                    objCostMaster.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    objCostMaster.ShowDialog();
                    if (objCostMaster.CostName != "")
                    {
                        strParty = objCostMaster.CostName;                       
                        this.Close();
                    }
                    this.Show();
                }
                else
                {
                    int count = CheckPartyAvailability(txtName.Text);
                    if (count > 0)
                    {
                        strParty = txtName.Text;
                        // cashObj.SetDataGridViewValue(txtName.Text);
                        this.Close();
                    }
                }
            }
        }

        private void txtName_TextChanged_1(object sender, EventArgs e)
        {
            //GetPartyBalance(txtName.Text);
            //lblBalance.Visible = true;
        }

        private void PartyName1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27)
            {
                    this.Close();
                    strParty = "";
            }
        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsWhiteSpace(e.KeyChar) && txtName.Text.Length < 1)
            {
                e.Handled = true;
            }
        }

        public int CheckPartyAvailability(string strParty)
        {
            int count = 0;
            try
            {
                if (table != null)
                {
                    DataRow[] rows = table.Select(String.Format("CostType='" + strParty + "'"));
                    count = rows.Length;
                }
            }
            catch
            {
            }
            return count;
        }

    }
}
