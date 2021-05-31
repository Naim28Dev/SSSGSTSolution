using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SSS
{
    public partial class CompanyDetails : Form
    {
        DataBaseAccess dba;
        DataTable dtRecord = null;
        public CompanyDetails()
        {
            InitializeComponent();
            dba = new DataBaseAccess();
            BindControls();
        }

        //using statment bind Controls...
        private void BindControls()
        {
            try
            {
                if (dtRecord != null)
                {
                    dtRecord.Rows.Clear();
                }
                dtRecord = dba.GetDataTable("Select * from CompanyDetails");
                if (dtRecord.Rows.Count > 0)
                {
                    DisableAllText();
                    
                    btnAdd.Enabled = false;
                    DataRow drow = dtRecord.Rows[0];
                    txtCompany.Text = Convert.ToString(drow["CompanyName"]);
                    txtFullName.Text = Convert.ToString(drow["FullCompanyName"]);
                    txtAddress.Text = Convert.ToString(drow["Address"]);
                    txtState.Text = Convert.ToString(drow["StateName"]);
                    txtPinCode.Text = Convert.ToString(drow["PinCode"]);
                    txtGSTNo.Text = Convert.ToString(drow["GSTNo"]);
                    txtPANNo.Text = Convert.ToString(drow["PANNo"]);
                    txtHOAddress.Text = Convert.ToString(drow["TINNo"]);
                    txtEmailId.Text = Convert.ToString(drow["EmailID"]);
                    txtSTD.Text = Convert.ToString(drow["STDNo"]);
                    txtPhone.Text = Convert.ToString(drow["PhoneNo"]);
                    txtMobileNo.Text = Convert.ToString(drow["MobileNo"]);
                   // txtSignaturePath.Text = Convert.ToString(drow["SignaturePath"]);
                    rdoEnable.Checked = Convert.ToBoolean(drow["TAXEnabled"]);
                    txtBankName.Text = Convert.ToString(drow["BankName"]);
                    txtAccountNo.Text = Convert.ToString(drow["AccountName"]);
                    txtIFSCCode.Text = Convert.ToString(drow["IFSCCode"]);
                    txtBranchName.Text = Convert.ToString(drow["BranchName"]);
                    txtCINNumber.Text = Convert.ToString(drow["CINNumber"]);
                    txtSACCode.Text = Convert.ToString(drow["SACCode"]);
                    txtWebsite.Text = Convert.ToString(drow["Website"]);

                    if (!rdoEnable.Checked)
                        rdoDisable.Checked = true;

                    byte[] imgdata = new byte[0];
                    MemoryStream ms = null;
                    if (drow["SignatureImage"] != null && Convert.ToString(drow["SignatureImage"])!="")
                    {
                        imgdata = (byte[])drow["SignatureImage"];
                        ms = new MemoryStream(imgdata);
                        picSignature.Image = ByteToImage((byte[])drow["SignatureImage"]);
                    }                 

                    if (drow["HeaderImage"] != null && Convert.ToString(drow["HeaderImage"]) != "")
                    {
                        imgdata = (byte[])drow["HeaderImage"];
                        ms = new MemoryStream(imgdata);
                        picHeaderImage.Image = Image.FromStream(ms);
                    }

                    if (drow["BrandLogo"] != null && Convert.ToString(drow["BrandLogo"]) != "")
                    {
                        imgdata = (byte[])drow["BrandLogo"];
                        ms = new MemoryStream(imgdata);
                        picBrandLogo.Image = Image.FromStream(ms);
                    }
                }
            }
            catch(Exception ex) { ; }
        }

        private void PointHandler(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox txtBox = sender as TextBox;
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
            catch { }
        }


        //using statment clear control value..
        private void ClearRecord()
        {
            txtFullName.Clear();
            txtMobileNo.Clear();
            txtPhone.Clear();
            txtPANNo.Clear();
            txtPinCode.Clear();
            txtHOAddress.Clear();
            txtSTD.Clear();
            txtAddress.Clear();
            txtEmailId.Clear();
            txtCompany.Clear();
            txtState.Clear();
            txtGSTNo.Clear(); 
            txtSignaturePath.Clear();
            txtBankName.Clear();
            txtAccountNo.Clear();
            txtIFSCCode.Clear();
            txtBranchName.Clear();
            txtSACCode.Clear();
            txtCINNumber.Clear();
            txtWebsite.Clear();
            txtBrandLogo.Text = txtHeaderImage.Text = "";
            picBrandLogo.Image = picHeaderImage.Image = picSignature.Image = null;

            rdoEnable.Checked = true;
        }

        private bool checkRequiredFeild()
        {
            if (txtFullName.Text == "")
            {
                MessageBox.Show("Full Name is required!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFullName.Focus();
                return false;
            }
            else if (txtAddress.Text == "")
            {
                MessageBox.Show("Address is required!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtAddress.Focus();
                return false;
            }
            else if (txtPinCode.Text == "")
            {
                MessageBox.Show("Pin Code is required!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPinCode.Focus();
                return false;
            }
            else
            {
                if (rdoEnable.Checked)
                {
                    if (txtSTD.Text == "")
                    {
                        MessageBox.Show("STD Code is required!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtSTD.Focus();
                        return false;
                    }
                    else if (txtPhone.Text == "")
                    {
                        MessageBox.Show("Phone No is required!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtPhone.Focus();
                        return false;
                    }
                    else if (txtEmailId.Text == "")
                    {
                        MessageBox.Show("Email Id is required", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtEmailId.Focus();
                        return false;
                    }
                }
            }
            return true;
        }

        private void EnableAllText()
        {
            foreach (Control ctr in panelText.Controls)
            {
                if (ctr is TextBox)
                {
                    ((TextBox)ctr).ReadOnly = false;
                }
            }
            btnBrowse.Enabled = btnHeaderImageBrowse.Enabled = btnBrandLogo.Enabled = grpBox.Enabled = true;
            if (!MainPage.strUserRole.Contains("ADMIN"))
                txtBankName.ReadOnly = txtAccountNo.ReadOnly = txtBranchName.ReadOnly = txtIFSCCode.ReadOnly = true;
        }

        private void DisableAllText()
        {
            foreach (Control ctr in panelText.Controls)
            {
                if (ctr is TextBox)
                {
                    ((TextBox)ctr).ReadOnly = true;
                }
            }
            btnBrowse.Enabled = btnHeaderImageBrowse.Enabled = btnBrandLogo.Enabled = grpBox.Enabled = false;
        }

        private void SaveCompanyDetails()
        {
            try
            {
                string strCode = "", strPath = GetSignturePath();
                if (rdoEnable.Checked)
                    strCode = "1";
                else
                    strCode = "0";
               string _signatureImage = "NULL", _headerImage = "NULL", _brandLogo = "NULL";
                if (txtHeaderImage.Text != "")
                    _headerImage  = "@HeaderImage";
                if (txtSignaturePath.Text != "")
                    _signatureImage = "@SignatureImage";
                if (txtBrandLogo.Text != "")
                    _brandLogo = "@BrandLogo";

                string strQuery = " INSERT INTO [dbo].[CompanyDetails] ([CompanyName],[FullCompanyName],[Address],[StateName],[PinCode],[GSTNo],[PANNo],[TINNo],[EmailID],[STDNo],[PhoneNo],[MobileNo],[SignaturePath],[Other],[TAXEnabled],[CreatedBy],[UpdateBy],[InsertStatus],[UpdateStatus],[BankName],[AccountName],[IFSCCode],[BranchName],[CINNumber],[SACCode],[HeaderImage],[BrandLogo],[SignatureImage],[Website]) VALUES "
                                + " ('" + txtCompany.Text + "','" + txtFullName.Text + "' ,'" + txtAddress.Text + "','" + txtState.Text + "' ,'" + txtPinCode.Text + "','" + txtGSTNo.Text + "','" + txtPANNo.Text + "','" + txtHOAddress.Text + "','" + txtEmailId.Text + "','" + txtSTD.Text + "','" + txtPhone.Text + "'  ,'" + txtMobileNo.Text + "','" + strPath + "','" + MainPage.strCompanyName + "' ," + strCode + ",'" + MainPage.strLoginName + "','',1,0,'" + txtBankName.Text + "','" + txtAccountNo.Text + "','" + txtIFSCCode.Text + "','" + txtBranchName.Text + "','" + txtCINNumber.Text + "','" + txtSACCode.Text + "'," + _headerImage + "," + _brandLogo + "," + _signatureImage + ",'"+txtWebsite.Text+"')";

                MainPage.OpenConnection();
                SqlCommand cmd = new SqlCommand(strQuery, MainPage.con);
                cmd.CommandTimeout = 1000000;
                if (txtHeaderImage.Text != "")
                    cmd.Parameters.Add("HeaderImage", SqlDbType.Image, 0).Value = ImageToByte(picHeaderImage.Image);
                if (txtSignaturePath.Text != "")
                    cmd.Parameters.Add("SignatureImage", SqlDbType.Image, 0).Value = ImageToByte(picSignature.Image);
                if (txtBrandLogo.Text != "")
                    cmd.Parameters.Add("BrandLogo", SqlDbType.Image, 0).Value = ImageToByte(picBrandLogo.Image);

                int count = cmd.ExecuteNonQuery();
               
                if (count > 0)
                {
                    if (txtHeaderImage.Text != "")
                        MainPage._headerImage = ImageToByte(picHeaderImage.Image);
                    if (txtSignaturePath.Text != "")
                        MainPage._signatureImage = ImageToByte(picSignature.Image);
                    if (txtBrandLogo.Text != "")
                        MainPage._brandLogo = ImageToByte(picBrandLogo.Image);

                    MessageBox.Show("Thanks ! Company details save successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    SaveNewImage();
                    MainPage.strPrintComapanyName = txtCompany.Text;
                    MainPage._bTaxStatus = rdoEnable.Checked;
                    MainPage.strCompanyStateName = txtState.Text;
                    ClearRecord();
                    btnAdd.Text = "&Add";
                    BindControls();
                }
                else
                {
                    MessageBox.Show("Sorry ! Unable to save record ! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
            }
        }
        private void CompanyDetails_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyCode == Keys.Enter && !txtAddress.Focused && !txtHOAddress.Focused)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch { }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Add")
            {
                ClearRecord();
                btnAdd.Text ="&Save";
                btnEdit.Text = "&Edit";
                txtFullName.Focus();            
                EnableAllText();
            }
            else if (btnAdd.Text == "&Save")
            {
                if (checkRequiredFeild())
                {
                    DialogResult result = MessageBox.Show("Do you sure you want to save record?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        SaveCompanyDetails();                       
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (btnEdit.Text == "&Edit")
            {
                btnEdit.Text = "&Update";
                EnableAllText();               
                txtFullName.Focus();
            }

            else if (btnEdit.Text == "&Update")
            {
                if (checkRequiredFeild())
                {
                    DialogResult result = MessageBox.Show("Do you want to update company details ? ", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        UpdateCompanyRecord();      
                    }
                }
            }
        }

        private string GetSignturePath()
        {
            if (txtSignaturePath.Text != "")
            {
                string strExtension = Path.GetExtension(txtSignaturePath.Text), strNewPath = MainPage.strServerPath + @"\Signature", strNewImage = strNewPath +@"\owner" + strExtension;
                return strNewImage;
            }
            else
                return "";
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Image ByteToImage(byte[] img)
        {
            MemoryStream obj = new MemoryStream(img);
            return Image.FromStream(obj);                
        }

        private void UpdateCompanyRecord()
        {
            try
            {
                string strCode = "", strPath = GetSignturePath(), strUpdateQuery = "";
                if (rdoEnable.Checked)
                    strCode = "1";
                else
                    strCode = "0";

                if (txtHeaderImage.Text != "")
                    strUpdateQuery += ",[HeaderImage]=@HeaderImage";
                else if (picHeaderImage.Image == null)
                    strUpdateQuery += ",[HeaderImage]=NULL ";
                if (txtSignaturePath.Text != "")
                    strUpdateQuery += ",[SignatureImage]=@SignatureImage";
                else if (picSignature.Image == null)
                    strUpdateQuery += ",[SignatureImage]=NULL ";
                if (txtBrandLogo.Text != "")
                    strUpdateQuery += ",[BrandLogo]=@BrandLogo ";
                else if(picBrandLogo.Image==null)
                    strUpdateQuery += ",[BrandLogo]=NULL ";

                string strQuery = " Update CompanyDetails SET [CompanyName]='" + txtCompany.Text + "' ,[FullCompanyName]='" + txtFullName.Text + "' ,[Address]='" + txtAddress.Text + "' ,[StateName]='" + txtState.Text + "' ,[PinCode]='" + txtPinCode.Text + "' ,[GSTNo]='" + txtGSTNo.Text + "' ,[PANNo]='" + txtPANNo.Text + "' ,[TINNo]='" + txtHOAddress.Text + "' ,[EmailID]='" + txtEmailId.Text + "' ,[STDNo]='" + txtSTD.Text + "' ,[PhoneNo]='" + txtPhone.Text + "' ,[MobileNo]='" + txtMobileNo.Text + "',"
                                + " [SignaturePath]='" + strPath + "' ,[Other]='" + MainPage.strCompanyName + "',[TAXEnabled]=" + strCode + " ,[BankName]='" + txtBankName.Text + "',[AccountName]='" + txtAccountNo.Text + "',[IFSCCode]='" + txtIFSCCode.Text + "',[BranchName]='" + txtBranchName.Text + "',[UpdateBy]='" + MainPage.strLoginName + "',[UpdateStatus]=1,[CINNumber]='" + txtCINNumber.Text + "',[SACCode]='" + txtSACCode.Text + "',[Website]='"+txtWebsite.Text+"' " + strUpdateQuery;


                MainPage.OpenConnection();
                SqlCommand cmd = new SqlCommand(strQuery, MainPage.con);
                cmd.CommandTimeout = 1000000;
                if (txtHeaderImage.Text != "")
                    cmd.Parameters.Add("HeaderImage", SqlDbType.Image, 0).Value = ImageToByte(picHeaderImage.Image);
                if (txtSignaturePath.Text != "")
                    cmd.Parameters.Add("SignatureImage", SqlDbType.Image, 0).Value = ImageToByte(picSignature.Image);
                if (txtBrandLogo.Text != "")
                    cmd.Parameters.Add("BrandLogo", SqlDbType.Image, 0).Value = ImageToByte(picBrandLogo.Image);

                int count = cmd.ExecuteNonQuery();
               
                if (count > 0)
                {
                    if (txtHeaderImage.Text != "")
                        MainPage._headerImage = ImageToByte(picHeaderImage.Image);
                    if (txtSignaturePath.Text != "")
                        MainPage._signatureImage = ImageToByte(picSignature.Image);
                    if (txtBrandLogo.Text != "")
                        MainPage._brandLogo = ImageToByte(picBrandLogo.Image);

                    MessageBox.Show("Update record successfully !", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    SaveNewImage();
                    btnEdit.Text = "&Edit";
                    MainPage.strPrintComapanyName = txtCompany.Text;
                    MainPage._bTaxStatus = rdoEnable.Checked;
                    MainPage.strCompanyStateName = txtState.Text;
                    BindControls();
                }
            }
            catch(Exception EX) { MessageBox.Show(EX.Message); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFullName.Text != "")
                {
                    BindControls();
                    DialogResult result = MessageBox.Show("Are you sure want to delete customer detail record", "Confrimation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int i = dba.ExecuteMyQuery(" Delete from CompanyDetails ");
                        if (i > 0)
                        {
                            MessageBox.Show("Record has been deleted successfully", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            ClearRecord();
                            //GetNextCompanyRecord();
                        }
                    }
                }
            }
            catch { }
        }
        private void txtPinCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            PointHandler(txtPinCode, e);
        }

        private void txtMobileNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            PointHandler(txtMobileNo, e);
        }

        private void txt1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char pressedKey = e.KeyChar;
            if (pressedKey == Convert.ToChar(8) || pressedKey == Convert.ToChar(43) || pressedKey == Convert.ToChar(45))
                e.Handled = false;
            else
                PointHandler(txtSTD, e);
        }

        private void txtPhone1_KeyPress(object sender, KeyPressEventArgs e)
        {
            PointHandler(txtPhone, e);
        }    

        private void txtPhone2_KeyPress(object sender, KeyPressEventArgs e)
        {
            PointHandler(txtPANNo, e);
        }

        private void txtEamilId_Leave(object sender, EventArgs e)
        {
            //if (txtEamilId.Text != "")
            //{
            //    CheckValidationText objCheckValidation = new CheckValidationText();
            //    objCheckValidation.EmailValid(txtEamilId);
            //}
        }

        private void CompanyDetails_Load(object sender, EventArgs e)
        {
            if (MainPage.strLoginName == "USER")
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        //private void txtSaleTitle_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
        //        {
        //            char objChar = Convert.ToChar(e.KeyCode);
        //            int value = e.KeyValue;
        //            if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
        //            {
        //                SearchData objSearch = new SearchData("SALESREPORTTITLE", "SEARCH SALES REPORT TITLE", e.KeyCode);
        //                objSearch.ShowDialog();
        //                txtSaleTitle.Text = objSearch.strSelectedData;
        //            }
        //            else
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void txtCity_KeyDown(object sender, KeyEventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                char objChar = Convert.ToChar(e.KeyCode);
                int value = e.KeyValue;
                if (e.Modifiers != Keys.Alt && e.Modifiers != Keys.Control && ((value > 47 && value < 58) || (value > 64 && value < 91) || (value > 95 && value < 106) || e.KeyCode == Keys.F2 || value == 32))
                {
                    SearchData objSearch = new SearchData("STATENAME", "SEARCH STATE NAME", e.KeyCode);
                    objSearch.ShowDialog();
                    txtState.Text = objSearch.strSelectedData;
                  
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Image Files (*.jpg,*.jpeg,*.bmp)|*.jpg;*.jpeg;*.bmp";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                {
                    txtSignaturePath.Text = _browser.FileName;
                    picSignature.Image = Image.FromFile(_browser.FileName);
                }
                else
                    picSignature.Image = null;
            }
        }

        private void SaveNewImage()
        {
            try
            {
                string strImagePath = txtSignaturePath.Text;
                if (strImagePath == "")
                {
                    //string strPath = MainPage.strServerPath + @"\Signature";
                    //if (Directory.Exists(strPath))
                    //{
                    //    Directory.Delete(strPath);
                    //}
                }
                else
                {

                    string strExtension = Path.GetExtension(strImagePath), strNewPath = MainPage.strServerPath + @"\Signature", strNewImage = strNewPath + @"\owner" + strExtension;
                    Directory.CreateDirectory(strNewPath);
                    if (strNewImage.ToUpper() != strImagePath.ToUpper())
                    {
                        if (File.Exists(strNewImage) && File.Exists(strImagePath))
                            File.Delete(strNewImage);
                        File.Copy(strImagePath, strNewImage);
                    }
                }
            }
            catch
            {
            }
        }

        private void txtCompany_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateSpace(sender, e);
        }

        private void txtAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            dba.ValidateRichTextBoxSpace(sender, e);
        }

        private void btnHeaderImageBrowse_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Image Files (*.jpg,*.jpeg,*.bmp)|*.jpg;*.jpeg;*.bmp";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                {
                    txtHeaderImage.Text = _browser.FileName;
                    picHeaderImage.Image = Image.FromFile(_browser.FileName);
                }
                else
                    picHeaderImage.Image = null;
            }
        }

        private void btnBrandLogo_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "&Save" || btnEdit.Text == "&Update")
            {
                OpenFileDialog _browser = new OpenFileDialog();
                _browser.Filter = "Image Files (*.jpg,*.jpeg,*.bmp)|*.jpg;*.jpeg;*.bmp";
                _browser.ShowDialog();
                if (_browser.FileName != "")
                {
                    txtBrandLogo.Text = _browser.FileName;
                    picBrandLogo.Image = Image.FromFile(_browser.FileName);
                }
                else
                    picBrandLogo.Image = null;
            }
        }
    }
}
