using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.IO;



namespace SSS
{
    class CreateReport
    {
        public CreateReport()
        {
           // GenerateUpdateSalesRecordXML();
           // GenerateUpdateSalesEntryXML();
        }

        //public string CreateErrorReports(string[] strReport)
        //{
        //    StreamWriter sw = new StreamWriter("D:\\Report\\Reporting.doc", true);
        //    sw.Write(sw.NewLine);
        //    sw.WriteLine(strReport[0]);
        //    sw.Write(sw.NewLine);
        //    sw.WriteLine(strReport[1]);
        //    sw.Close();

        //    return "Created";
        //}


        //public void GenerateUpdateSalesRecordXML()
        //{
        //    try
        //    {
        //        //XmlDataDocument sourceXML = new XmlDataDocument();
        //        string xmlFile = "E:\\SalesUpdateRecord.xml";
        //        //create a XML file is not exist
        //        System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(xmlFile, null);
        //        //starts a new document
        //        writer.WriteStartDocument();
        //        //write comments
        //        //writer.WriteComment("Commentss : XmlWriter Test Program");
               
        //        writer.Formatting = System.Xml.Formatting.Indented;

        //        writer.WriteStartElement("Sales_Update");
        //        //write some simple elements
        //        writer.WriteElementString("InvoiceNo", "");
        //        writer.WriteElementString("PartName", "");
        //        writer.WriteElementString("Date", "");
        //        writer.WriteElementString("DueDate", "");
        //        writer.WriteElementString("Balance", "");
        //        writer.WriteElementString("Agent", "");
        //        writer.WriteElementString("Commission", "");
        //        writer.WriteElementString("Haste", "");
        //        writer.WriteElementString("Admin", "");
        //        writer.WriteElementString("NetAddLs", "");
        //        writer.WriteElementString("Remark", "");
        //        writer.WriteElementString("Prep_By", "");
        //        writer.WriteElementString("Transport", "");
        //        writer.WriteElementString("Station", "");
        //        writer.WriteElementString("Weight", "");
        //        writer.WriteElementString("Lr_No", "");
        //        writer.WriteElementString("Lr_Date", "");
        //        writer.WriteElementString("Freight", "");
        //        writer.WriteElementString("Fold", "");
        //        writer.WriteElementString("Packing", "");
        //        writer.WriteElementString("Postage", "");
        //        writer.WriteElementString("OtherPer", "");
        //        writer.WriteElementString("Thaili", "");
        //        writer.WriteElementString("Others", "");
        //        writer.WriteElementString("Total_Pcs", "");
        //        writer.WriteElementString("Total_Meter", "");
        //        writer.WriteElementString("Gross_Amt", "");
        //        writer.WriteElementString("Net_Amt", "");
        //        writer.WriteElementString("Final_Amt", "");
        //        writer.WriteEndElement();
                
        //        writer.Close();
        //    }
        //    catch
        //    {
                
        //    }
        //}

        //public void GenerateUpdateSalesEntryXML()
        //{
        //    try
        //    {
        //        //XmlDataDocument sourceXML = new XmlDataDocument();
        //        string xmlFile = "E:\\SalesUpdateEntry.xml";
        //        //create a XML file is not exist
        //        System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(xmlFile, null);
        //        //starts a new document
        //        writer.WriteStartDocument();
        //        //write comments
        //        //writer.WriteComment("Commentss : XmlWriter Test Program");

        //        writer.Formatting = System.Xml.Formatting.Indented;

        //        writer.WriteStartElement("Sales_Update");
        //        //write some simple elements
        //        writer.WriteElementString("InvoiceNo", "");
        //        writer.WriteElementString("SerialNo", "");
        //        writer.WriteElementString("PurchaseParty", "");
        //        writer.WriteElementString("Pieces", "");
        //        writer.WriteElementString("Discount", "");
        //        writer.WriteElementString("DiscountStatus", "");
        //        writer.WriteElementString("Amount", "");
        //        writer.WriteElementString("PurchaseBill", "");
        //        writer.WriteElementString("SalesParty", "");
               
        //        writer.WriteEndElement();

        //        writer.Close();
        //    }
        //    catch
        //    {

        //    }
        //}

        public void SetValueOnSalesRecord(string[] record)
        {
            //XElement element = new XElement("Sales_Update",
            //    new XElement("InvoiceNo", record[0]),
            //    new XElement("PartyName", record[1]),
            //    new XElement("Date", record[2]),
            //    new XElement("DueDate", record[3]),
            //    new XElement("Balance", record[4]),
            //    new XElement("Agent", record[5]),
            //    new XElement("Commission", record[6]),
            //    new XElement("Haste", record[7]),
            //    new XElement("Admin", record[8]),

            //    new XElement("NewAddLs", record[9]),
            //    new XElement("Remarks", record[10]),
            //    new XElement("Prep_By", record[11]),
            //    new XElement("Transport", record[12]),
            //    new XElement("Station", record[13]),
            //    new XElement("Weight", record[14]),
            //    new XElement("Lr_No", record[15]),
            //    new XElement("Lr_Date", record[16]),
            //    new XElement("Freight", record[17]),
            //    new XElement("Fold", record[18]),
            //    new XElement("Packing", record[19]),
            //    new XElement("Postage", record[20]),
            //    new XElement("OtherPer", record[21]),
            //    new XElement("Thaili", record[22]),
            //    new XElement("Others", record[23]),
            //    new XElement("Total_Pcs", record[24]),
            //    new XElement("Total_Meter", record[25]),
            //    new XElement("Gross_Amt", record[26]),
            //    new XElement("Net_Amt", record[27]),
            //    new XElement("Final_Amt", record[28]));

            //element.Save("E:\\SalesUpdateRecord.xml");
               
        }
        
        public void SetValueOnSalesEntry(string[] record)
        {
            //XElement xml = new XElement("Sales_Update",
            //    new XElement("InvoiceNo", record[0]),
            //    new XElement("SerialNo", record[1]),
            //    new XElement("PurchaseParty", record[2]),
            //    new XElement("Pieces", record[3]),
            //    new XElement("Discount", record[4]),
            //    new XElement("DiscountStatus", record[5]),
            //    new XElement("Amount", record[6]),
            //    new XElement("PurchaseBill", record[7]),
            //    new XElement("SalesParty", record[9]));
            //xml.Save("E:\\SalesUpdateEntry.xml");
            StreamWriter sw = new StreamWriter("E:\\Report.doc");
            StringWriter sting = new StringWriter();
            
            sw.Write(record[0] + "    " + record[1] + "    " + record[2] + "    " + record[3] + "    " + record[4] + "    " + record[5] + "    " + record[6] + "    " + record[7] + "    "+record[8]+"    "+record[9]+"    ");
            sw.Close();
        }

        //#region Create Backup

        //public void BackupDatabase(String databaseName, String userName, String password, String serverName, String destinationPath)
        //{
        //    Backup sqlBackup = new Backup();

        //    sqlBackup.Action = BackupActionType.Database;
        //    sqlBackup.BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString();
        //    sqlBackup.BackupSetName = "Archive";

        //    sqlBackup.Database = databaseName;

        //    BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath, DeviceType.File);
        //    ServerConnection connection = new ServerConnection(@".\SQLEXPRESS");
        //    Server sqlServer = new Server(connection);

        //    Database db = sqlServer.Databases[@"C:\MY DB\RMS.MDF"];

        //    sqlBackup.Initialize = true;
        //    sqlBackup.Checksum = true;
        //    sqlBackup.ContinueAfterError = true;

        //    sqlBackup.Devices.Add(deviceItem);
        //    sqlBackup.Incremental = false;

        //    sqlBackup.ExpirationDate = DateTime.Now.AddDays(365);
        //    sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

        //    sqlBackup.FormatMedia = false;

        //    sqlBackup.SqlBackup(sqlServer);
        //}

        //public void RestoreDatabase(String databaseName, String filePath, String serverName, String userName, String password, String dataFilePath, String logFilePath)
        //{
        //    Restore sqlRestore = new Restore();

        //    BackupDeviceItem deviceItem = new BackupDeviceItem(filePath, DeviceType.File);
        //    sqlRestore.Devices.Add(deviceItem);
        //    sqlRestore.Database = databaseName;

        //    ServerConnection connection = new ServerConnection(serverName, userName, password);
        //    Server sqlServer = new Server(connection);

        //    Database db = sqlServer.Databases[databaseName];
        //    sqlRestore.Action = RestoreActionType.Database;
        //    String dataFileLocation = dataFilePath + databaseName + ".mdf";
        //    String logFileLocation = logFilePath + databaseName + "_Log.ldf";
        //    db = sqlServer.Databases[databaseName];

        //    RelocateFile rf = new RelocateFile(databaseName, dataFileLocation);

        //    sqlRestore.RelocateFiles.Add(new RelocateFile(databaseName, dataFileLocation));
        //    sqlRestore.RelocateFiles.Add(new RelocateFile(databaseName + "_log", logFileLocation));
        //    sqlRestore.ReplaceDatabase = true;
        //   // sqlRestore.Complete += new ServerMessageEventHandler(sqlRestore_Complete);
        //  //  sqlRestore.PercentCompleteNotification = 10;
        //   // sqlRestore.PercentComplete +=
        //     //  new PercentCompleteEventHandler(sqlRestore_PercentComplete);

        //    sqlRestore.SqlRestore(sqlServer);
        //    db = sqlServer.Databases[databaseName];
        //    db.SetOnline();
        //    sqlServer.Refresh();
        //}


        

        //#endregion
    }

}
