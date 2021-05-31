using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Org.BouncyCastle.Pkcs;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.xml.xmp;
using iTextSharp.text;
using Org.BouncyCastle.Crypto;

namespace SSS
{
    /// <summary>
    /// This class hold the certificate and extract private key needed for e-signature 
    /// </summary>
    class Cert
    {
        #region Attributes

        private string path = "";
        private string password = "";
        private AsymmetricKeyParameter akp;
        private Org.BouncyCastle.X509.X509Certificate[] chain;

        #endregion

        #region Accessors
        public Org.BouncyCastle.X509.X509Certificate[] Chain
        {
          get { return chain; }
        }
        public AsymmetricKeyParameter Akp
        {
          get { return akp; }
        }

        public string Path
        {
            get { return path; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region Helpers

        private void processCert()
        {
                string alias = null;                                                
                Pkcs12Store pk12;

                //First we'll read the certificate file
                pk12 = new Pkcs12Store(new FileStream(this.Path, FileMode.Open, FileAccess.Read), this.password.ToCharArray());

                //then Iterate throught certificate entries to find the private key entry              
            //IEnumerator<string> i = (IEnumerator<string>) pk12.Aliases;

            foreach (string al in pk12.Aliases)
            {
                if (pk12.IsKeyEntry(al) && pk12.GetKey(al).Key.IsPrivate)
                {
                    alias = al;
                    break;
                }
            }

            //while (i.MoveNext())
            //    {
            //        alias = ((string)i.Current);
            //        if (pk12.IsKeyEntry(alias))
            //            break;
            //    }

                this.akp = pk12.GetKey(alias).Key;
                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                this.chain = new Org.BouncyCastle.X509.X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                    chain[k] = ce[k].Certificate;

            }
        #endregion

        #region Constructors
            public Cert()
            { }
            public Cert(string cpath)
            {
                this.path = cpath;
                this.processCert();
            }
            public Cert(string cpath, string cpassword)
            {
                this.path = cpath;
                this.Password = cpassword;
                this.processCert();
            }
        #endregion

    }

    /// <summary>
    /// This is a holder class for PDF metadata
    /// </summary>
    class MetaData
    {
        private Dictionary<String, String> info = new Dictionary<String, String>();

        public Dictionary<String, String> Info
        {
            get { return info; }
            set { info = value; }
        }

        public string Author
        {
            get { return (string)info["Author"]; }
            set { info.Add("Author", value); }
        }
        public string Title
        {
            get { return (string)info["Title"]; }
            set { info.Add("Title", value); }
        }
        public string Subject
        {
            get { return (string)info["Subject"]; }
            set { info.Add("Subject", value); }
        }
        public string Keywords
        {
            get { return (string)info["Keywords"]; }
            set { info.Add("Keywords", value); }
        }
        public string Producer
        {
            get { return (string)info["Producer"]; }
            set { info.Add("Producer", value); }
        }

        public string Creator
        {
            get { return (string)info["Creator"]; }
            set { info.Add("Creator", value); }
        }

        public Dictionary<String, String> getMetaData()
        {
            return this.info;
        }
        public byte[] getStreamedMetaData()
        {
            MemoryStream os = new System.IO.MemoryStream();           
            XmpWriter xmp = new XmpWriter(os, this.Info);            
            xmp.Close();            
            return os.ToArray();
        }

    }
    
    /// <summary>
    /// this is the most important class
    /// it uses iTextSharp library to sign a PDF document
    /// </summary>
    class PDFSigner
    {
        private string inputPDF = "";
        private string outputPDF = "";
        private Cert myCert;
        private MetaData metadata;
        public string SRC = "";
        public string DEST = "";
        public string Sign_File = "";

        public bool SetSign(string strSRC,string strDest,string strSign)
        {
            Cert myCert = null;
            SRC = strSRC;
            DEST = strDest;
            Sign_File = strSign;

            try
            {
                myCert = new Cert(Sign_File, "123");
            }
            catch(Exception ex)
            {
                throw ex;
                return false;
            }

            //Adding Meta Datas
            MetaData MyMD = new MetaData();
            //MyMD.Author = authorBox.Text;
            //MyMD.Title = titleBox.Text;
            //MyMD.Subject = subjectBox.Text;
            //MyMD.Keywords = kwBox.Text;
            //MyMD.Creator = creatorBox.Text;
            //MyMD.Producer = prodBox.Text;
          
            this.inputPDF = SRC;
            this.outputPDF = DEST;
            this.myCert = myCert;
            this.metadata = MyMD;

           Sign("", "", "IN", true);
            return true;
        }

        public void Sign(string SigReason, string SigContact, string SigLocation, bool visible)
        {
            PdfReader reader = new PdfReader(this.inputPDF);
            //Activate MultiSignatures
            PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0', null, true);
            //To disable Multi signatures uncomment this line : every new signature will invalidate older ones !
            //PdfStamper st = PdfStamper.CreateSignature(reader, new FileStream(this.outputPDF, FileMode.Create, FileAccess.Write), '\0'); 

            st.MoreInfo = this.metadata.getMetaData();
            st.XmpMetadata = this.metadata.getStreamedMetaData();
            PdfSignatureAppearance sap = st.SignatureAppearance;
            
            sap.SetCrypto(this.myCert.Akp, this.myCert.Chain, null, PdfSignatureAppearance.WINCER_SIGNED);
            //sap.Reason = SigReason;
            sap.Contact = SigContact;
            sap.Location = SigLocation;
            int _count= reader.NumberOfPages;
            
            if (visible)
                sap.SetVisibleSignature(new iTextSharp.text.Rectangle(430, 55, 570, 95), _count, null);
            
            st.Close();           
        }
    }

    class ReadDataFromPDF
    {
        //public string GetTextFromPDF()
        //{
        //    StringBuilder text = new StringBuilder();
        //    PdfReader objReader = new PdfReader("D:\\RentReceiptFormat.pdf");
        //    //using ()
        //    {
        //        for (int i = 1; i <= objReader.NumberOfPages; i++)
        //        {
        //            text.Append(PdfTextExtractor.GetTextFromPage(objReader, i));
        //        }
        //    }

        //    return text.ToString();
        //}
    }

}




