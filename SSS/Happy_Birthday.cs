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
    public partial class Happy_Birthday : Form
    {
        public Happy_Birthday(string strPartyName)
        {
            InitializeComponent();
            this.Text = "!! DEAR " + strPartyName+" !!";
        }

        private void Happy_Birthday_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

    }
}
