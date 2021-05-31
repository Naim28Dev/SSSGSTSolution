namespace SSS.Reporting
{
    partial class ShowReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myPreview = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblHeader = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // myPreview
            // 
            this.myPreview.ActiveViewIndex = -1;
            this.myPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.myPreview.Cursor = System.Windows.Forms.Cursors.Default;
            this.myPreview.DisplayBackgroundEdge = false;
            this.myPreview.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myPreview.Location = new System.Drawing.Point(3, 46);
            this.myPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.myPreview.Name = "myPreview";
            this.myPreview.SelectionFormula = "";
            this.myPreview.ShowCloseButton = false;
            this.myPreview.ShowCopyButton = false;
            this.myPreview.ShowGroupTreeButton = false;
            this.myPreview.ShowParameterPanelButton = false;
            this.myPreview.ShowRefreshButton = false;
            this.myPreview.ShowTextSearchButton = false;
            this.myPreview.ShowZoomButton = false;
            this.myPreview.Size = new System.Drawing.Size(899, 618);
            this.myPreview.TabIndex = 127;
            this.myPreview.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            this.myPreview.ViewTimeSelectionFormula = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblHeader);
            this.panel1.Location = new System.Drawing.Point(4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(898, 38);
            this.panel1.TabIndex = 128;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(6, 7);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(882, 22);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "Show Report";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShowReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 666);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.myPreview);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ShowReport";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Show Report";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ShowInterest_KeyDown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public CrystalDecisions.Windows.Forms.CrystalReportViewer myPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblHeader;
    }
}