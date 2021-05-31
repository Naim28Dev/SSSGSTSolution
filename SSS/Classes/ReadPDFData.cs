using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace SSS
{
    class ReadPDFData
    {
        public static string GetTextFromPDF(string strPath, int pageCount)
        {
            if (strPath != "")
            {
                var vText = "";
                var pdfReader = new PdfReader(strPath);
                byte[] pageContent = pdfReader.GetPageContent(pageCount);
                byte[] utf8 = Encoding.Convert(Encoding.Default, Encoding.UTF8, pageContent);
                string textFromPage = Encoding.UTF8.GetString(utf8);
                vText = ExtractPdfContent(textFromPage);
                return vText;
            }
            return "";
        }

      
        static string ExtractPdfContent(string rawPdfContent)
        {
            string text = "";
            string[] _lines = rawPdfContent.Split('\n');
            foreach (string strLine in _lines)
            {
                if (strLine.Contains("]TJ"))
                    text += strLine.Replace("(", "").Replace("[","").Replace("]TJ", "").Replace(")", "").Replace(@"\", "") + "\n";
            }
            return text;
        }

        public class RectAndText
        {
            public iTextSharp.text.Rectangle Rect;
            public String Text;
            public RectAndText(iTextSharp.text.Rectangle rect, String text)
            {
                this.Rect = rect;
                this.Text = text;
            }
        }

        //PdfReader reader = new PdfReader("E:\\DL449.pdf");
        //List<String> text = new List<String>();
        //String page;
        //List<String> pageStrings;
        //string[] separators = { "\n", "\r\n" };

        //for (int i = 1; i <= reader.NumberOfPages; i++)
        //{
        //    page = PdfTextExtractor.GetTextFromPage(reader, i,new SimpleTextExtractionStrategy());
        //    pageStrings = new List<string>(page.Split(separators, StringSplitOptions.RemoveEmptyEntries));
        //    text.AddRange(pageStrings);

        //}

        //PdfReader reader = new PdfReader("E:\\DL449.pdf");
        //int PageNum = reader.NumberOfPages;
        //string[] words;
        //string line = "", text;

        //for (int i = 1; i <= PageNum; i++)
        //{
        //    text = PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy());                             
        //}
    }
}
