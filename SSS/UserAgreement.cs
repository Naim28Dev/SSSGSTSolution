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
    public partial class UserAgreement : Form
    {
        protected internal bool _bConfirmation = false;
        public UserAgreement()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _bConfirmation = false;
            this.Close();
        }

        private void UserAgreement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _bConfirmation = false;
                this.Close();
            }
        }

        private void btnConfirmation_Click(object sender, EventArgs e)
        {
            btnConfirmation.Enabled = false;
            _bConfirmation = true;
            this.Close();
        }
    }
}
