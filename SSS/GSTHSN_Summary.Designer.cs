namespace SSS
{
    partial class GSTHSN_Summary
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagePurchase = new System.Windows.Forms.TabPage();
            this.dgrdPurchase = new System.Windows.Forms.DataGridView();
            this.tabPageJournal = new System.Windows.Forms.TabPage();
            this.dgrdJournal = new System.Windows.Forms.DataGridView();
            this.tabPagePurchaseReturn = new System.Windows.Forms.TabPage();
            this.dgrdPurchaseReturn = new System.Windows.Forms.DataGridView();
            this.tabPageSale = new System.Windows.Forms.TabPage();
            this.dgrdSales = new System.Windows.Forms.DataGridView();
            this.tabPageSaleService = new System.Windows.Forms.TabPage();
            this.dgrdSaleService = new System.Windows.Forms.DataGridView();
            this.tabPageSaleReturn = new System.Windows.Forms.TabPage();
            this.dgrdSaleReturn = new System.Windows.Forms.DataGridView();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnShowSummary = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPagePurchase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPurchase)).BeginInit();
            this.tabPageJournal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdJournal)).BeginInit();
            this.tabPagePurchaseReturn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPurchaseReturn)).BeginInit();
            this.tabPageSale.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSales)).BeginInit();
            this.tabPageSaleService.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSaleService)).BeginInit();
            this.tabPageSaleReturn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSaleReturn)).BeginInit();
            this.grpSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(977, 37);
            this.panel1.TabIndex = 106;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12.5F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(402, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "GST HSN SUMMARY";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.tabControl1);
            this.panel3.Controls.Add(this.grpSearch);
            this.panel3.Location = new System.Drawing.Point(12, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(976, 585);
            this.panel3.TabIndex = 107;
            this.panel3.TabStop = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPagePurchase);
            this.tabControl1.Controls.Add(this.tabPageJournal);
            this.tabControl1.Controls.Add(this.tabPagePurchaseReturn);
            this.tabControl1.Controls.Add(this.tabPageSale);
            this.tabControl1.Controls.Add(this.tabPageSaleService);
            this.tabControl1.Controls.Add(this.tabPageSaleReturn);
            this.tabControl1.Location = new System.Drawing.Point(11, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(18, 8);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(953, 519);
            this.tabControl1.TabIndex = 102;
            // 
            // tabPagePurchase
            // 
            this.tabPagePurchase.Controls.Add(this.dgrdPurchase);
            this.tabPagePurchase.Location = new System.Drawing.Point(4, 35);
            this.tabPagePurchase.Name = "tabPagePurchase";
            this.tabPagePurchase.Size = new System.Drawing.Size(945, 480);
            this.tabPagePurchase.TabIndex = 0;
            this.tabPagePurchase.Text = "Purchase";
            this.tabPagePurchase.UseVisualStyleBackColor = true;
            // 
            // dgrdPurchase
            // 
            this.dgrdPurchase.AllowUserToAddRows = false;
            this.dgrdPurchase.AllowUserToDeleteRows = false;
            this.dgrdPurchase.AllowUserToResizeRows = false;
            this.dgrdPurchase.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgrdPurchase.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdPurchase.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrdPurchase.ColumnHeadersHeight = 30;
            this.dgrdPurchase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdPurchase.EnableHeadersVisualStyles = false;
            this.dgrdPurchase.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdPurchase.Location = new System.Drawing.Point(13, 17);
            this.dgrdPurchase.Name = "dgrdPurchase";
            this.dgrdPurchase.ReadOnly = true;
            this.dgrdPurchase.RowHeadersVisible = false;
            this.dgrdPurchase.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdPurchase.RowTemplate.Height = 30;
            this.dgrdPurchase.Size = new System.Drawing.Size(917, 450);
            this.dgrdPurchase.TabIndex = 109;
            // 
            // tabPageJournal
            // 
            this.tabPageJournal.Controls.Add(this.dgrdJournal);
            this.tabPageJournal.Location = new System.Drawing.Point(4, 35);
            this.tabPageJournal.Name = "tabPageJournal";
            this.tabPageJournal.Size = new System.Drawing.Size(945, 480);
            this.tabPageJournal.TabIndex = 1;
            this.tabPageJournal.Text = "Journal";
            this.tabPageJournal.UseVisualStyleBackColor = true;
            // 
            // dgrdJournal
            // 
            this.dgrdJournal.AllowUserToAddRows = false;
            this.dgrdJournal.AllowUserToDeleteRows = false;
            this.dgrdJournal.AllowUserToResizeRows = false;
            this.dgrdJournal.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdJournal.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgrdJournal.ColumnHeadersHeight = 30;
            this.dgrdJournal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdJournal.EnableHeadersVisualStyles = false;
            this.dgrdJournal.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdJournal.Location = new System.Drawing.Point(14, 16);
            this.dgrdJournal.Name = "dgrdJournal";
            this.dgrdJournal.ReadOnly = true;
            this.dgrdJournal.RowHeadersVisible = false;
            this.dgrdJournal.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdJournal.RowTemplate.Height = 30;
            this.dgrdJournal.Size = new System.Drawing.Size(917, 450);
            this.dgrdJournal.TabIndex = 110;
            // 
            // tabPagePurchaseReturn
            // 
            this.tabPagePurchaseReturn.Controls.Add(this.dgrdPurchaseReturn);
            this.tabPagePurchaseReturn.Location = new System.Drawing.Point(4, 35);
            this.tabPagePurchaseReturn.Name = "tabPagePurchaseReturn";
            this.tabPagePurchaseReturn.Size = new System.Drawing.Size(945, 480);
            this.tabPagePurchaseReturn.TabIndex = 2;
            this.tabPagePurchaseReturn.Text = "Purchase Return";
            this.tabPagePurchaseReturn.UseVisualStyleBackColor = true;
            // 
            // dgrdPurchaseReturn
            // 
            this.dgrdPurchaseReturn.AllowUserToAddRows = false;
            this.dgrdPurchaseReturn.AllowUserToDeleteRows = false;
            this.dgrdPurchaseReturn.AllowUserToResizeRows = false;
            this.dgrdPurchaseReturn.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdPurchaseReturn.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrdPurchaseReturn.ColumnHeadersHeight = 30;
            this.dgrdPurchaseReturn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdPurchaseReturn.EnableHeadersVisualStyles = false;
            this.dgrdPurchaseReturn.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdPurchaseReturn.Location = new System.Drawing.Point(14, 16);
            this.dgrdPurchaseReturn.Name = "dgrdPurchaseReturn";
            this.dgrdPurchaseReturn.ReadOnly = true;
            this.dgrdPurchaseReturn.RowHeadersVisible = false;
            this.dgrdPurchaseReturn.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdPurchaseReturn.RowTemplate.Height = 30;
            this.dgrdPurchaseReturn.Size = new System.Drawing.Size(917, 450);
            this.dgrdPurchaseReturn.TabIndex = 111;
            // 
            // tabPageSale
            // 
            this.tabPageSale.Controls.Add(this.dgrdSales);
            this.tabPageSale.Location = new System.Drawing.Point(4, 35);
            this.tabPageSale.Name = "tabPageSale";
            this.tabPageSale.Size = new System.Drawing.Size(945, 480);
            this.tabPageSale.TabIndex = 3;
            this.tabPageSale.Text = "Sales";
            this.tabPageSale.UseVisualStyleBackColor = true;
            // 
            // dgrdSales
            // 
            this.dgrdSales.AllowUserToAddRows = false;
            this.dgrdSales.AllowUserToDeleteRows = false;
            this.dgrdSales.AllowUserToResizeRows = false;
            this.dgrdSales.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSales.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgrdSales.ColumnHeadersHeight = 30;
            this.dgrdSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSales.EnableHeadersVisualStyles = false;
            this.dgrdSales.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSales.Location = new System.Drawing.Point(14, 16);
            this.dgrdSales.Name = "dgrdSales";
            this.dgrdSales.ReadOnly = true;
            this.dgrdSales.RowHeadersVisible = false;
            this.dgrdSales.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSales.RowTemplate.Height = 30;
            this.dgrdSales.Size = new System.Drawing.Size(917, 450);
            this.dgrdSales.TabIndex = 111;
            // 
            // tabPageSaleService
            // 
            this.tabPageSaleService.Controls.Add(this.dgrdSaleService);
            this.tabPageSaleService.Location = new System.Drawing.Point(4, 35);
            this.tabPageSaleService.Name = "tabPageSaleService";
            this.tabPageSaleService.Size = new System.Drawing.Size(945, 480);
            this.tabPageSaleService.TabIndex = 7;
            this.tabPageSaleService.Text = "Sales Service";
            this.tabPageSaleService.UseVisualStyleBackColor = true;
            // 
            // dgrdSaleService
            // 
            this.dgrdSaleService.AllowUserToAddRows = false;
            this.dgrdSaleService.AllowUserToDeleteRows = false;
            this.dgrdSaleService.AllowUserToResizeRows = false;
            this.dgrdSaleService.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSaleService.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgrdSaleService.ColumnHeadersHeight = 30;
            this.dgrdSaleService.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSaleService.EnableHeadersVisualStyles = false;
            this.dgrdSaleService.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSaleService.Location = new System.Drawing.Point(14, 16);
            this.dgrdSaleService.Name = "dgrdSaleService";
            this.dgrdSaleService.ReadOnly = true;
            this.dgrdSaleService.RowHeadersVisible = false;
            this.dgrdSaleService.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSaleService.RowTemplate.Height = 30;
            this.dgrdSaleService.Size = new System.Drawing.Size(917, 450);
            this.dgrdSaleService.TabIndex = 112;
            // 
            // tabPageSaleReturn
            // 
            this.tabPageSaleReturn.Controls.Add(this.dgrdSaleReturn);
            this.tabPageSaleReturn.Location = new System.Drawing.Point(4, 35);
            this.tabPageSaleReturn.Name = "tabPageSaleReturn";
            this.tabPageSaleReturn.Size = new System.Drawing.Size(945, 480);
            this.tabPageSaleReturn.TabIndex = 4;
            this.tabPageSaleReturn.Text = "Sale Return";
            this.tabPageSaleReturn.UseVisualStyleBackColor = true;
            // 
            // dgrdSaleReturn
            // 
            this.dgrdSaleReturn.AllowUserToAddRows = false;
            this.dgrdSaleReturn.AllowUserToDeleteRows = false;
            this.dgrdSaleReturn.AllowUserToResizeRows = false;
            this.dgrdSaleReturn.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrdSaleReturn.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgrdSaleReturn.ColumnHeadersHeight = 30;
            this.dgrdSaleReturn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgrdSaleReturn.EnableHeadersVisualStyles = false;
            this.dgrdSaleReturn.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgrdSaleReturn.Location = new System.Drawing.Point(14, 16);
            this.dgrdSaleReturn.Name = "dgrdSaleReturn";
            this.dgrdSaleReturn.ReadOnly = true;
            this.dgrdSaleReturn.RowHeadersVisible = false;
            this.dgrdSaleReturn.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgrdSaleReturn.RowTemplate.Height = 30;
            this.dgrdSaleReturn.Size = new System.Drawing.Size(917, 450);
            this.dgrdSaleReturn.TabIndex = 111;
            // 
            // grpSearch
            // 
            this.grpSearch.Controls.Add(this.btnExport);
            this.grpSearch.Controls.Add(this.btnShowSummary);
            this.grpSearch.Controls.Add(this.btnClose);
            this.grpSearch.Location = new System.Drawing.Point(11, -3);
            this.grpSearch.Name = "grpSearch";
            this.grpSearch.Size = new System.Drawing.Size(953, 50);
            this.grpSearch.TabIndex = 101;
            this.grpSearch.TabStop = false;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnExport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(400, 12);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(186, 32);
            this.btnExport.TabIndex = 110;
            this.btnExport.Text = "&Export in Excel";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnShowSummary
            // 
            this.btnShowSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnShowSummary.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnShowSummary.ForeColor = System.Drawing.Color.White;
            this.btnShowSummary.Location = new System.Drawing.Point(214, 12);
            this.btnShowSummary.Name = "btnShowSummary";
            this.btnShowSummary.Size = new System.Drawing.Size(186, 32);
            this.btnShowSummary.TabIndex = 109;
            this.btnShowSummary.Text = "&Show HSN Summary";
            this.btnShowSummary.UseVisualStyleBackColor = false;
            this.btnShowSummary.Click += new System.EventHandler(this.btnShowSummary_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(30)))), ((int)(((byte)(12)))));
            this.btnClose.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(586, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(158, 32);
            this.btnClose.TabIndex = 109;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // GSTHSN_Summary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.ClientSize = new System.Drawing.Size(1000, 658);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GSTHSN_Summary";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GST HSN Summary";
            this.Load += new System.EventHandler(this.GSTHSN_Summary_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GSTHSN_Summary_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPagePurchase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPurchase)).EndInit();
            this.tabPageJournal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdJournal)).EndInit();
            this.tabPagePurchaseReturn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdPurchaseReturn)).EndInit();
            this.tabPageSale.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSales)).EndInit();
            this.tabPageSaleService.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSaleService)).EndInit();
            this.tabPageSaleReturn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrdSaleReturn)).EndInit();
            this.grpSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Button btnShowSummary;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagePurchase;
        private System.Windows.Forms.TabPage tabPageJournal;
        private System.Windows.Forms.TabPage tabPagePurchaseReturn;
        private System.Windows.Forms.TabPage tabPageSale;
        private System.Windows.Forms.TabPage tabPageSaleReturn;
        private System.Windows.Forms.DataGridView dgrdPurchase;
        private System.Windows.Forms.DataGridView dgrdJournal;
        private System.Windows.Forms.DataGridView dgrdPurchaseReturn;
        private System.Windows.Forms.DataGridView dgrdSales;
        private System.Windows.Forms.DataGridView dgrdSaleReturn;
        private System.Windows.Forms.TabPage tabPageSaleService;
        private System.Windows.Forms.DataGridView dgrdSaleService;
        private System.Windows.Forms.Button btnExport;
    }
}