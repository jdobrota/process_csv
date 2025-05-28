using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace main
{
    class CSV_Record
    {
        [Name("InvoiceNumber")]
        public string InvoiceNumber { get; set; }
        [Name("DeliveryNoteNumber")]
        public string DeliveryNoteNumber { get; set; }
        [Name("CompanyCode")]
        public string CompanyCode { get; set; }
        [Name("SalesOrg")]
        public string SalesOrg { get; set; }
        [Name("Division")]
        public string Division { get; set; }
        [Name("SoldToNumber")]
        public string SoldToNumber{ get; set; }
        [Name("SoldToName")]
        public string SoldToName { get; set; }
        [Name("SoldToComMail")]
        public string SoldToComMail { get; set; }
        [Name("SoldToLanguage")]
        public string SoldToLanguage { get; set; }
        [Name("SoldToStatus")]
        public string SoldToStatus { get; set; }
        [Name("InvoiceIssueDate")]
        public string InvoiceIssueDate { get; set; }
        [Name("DocumentType")]
        public string DocumentType { get; set; }
        [Name("ShipToNumber")]
        public string ShipToNumber { get; set; }
        [Name("ShipToName")]
        public string ShipToName { get; set; }
        [Name("DeliveryNoteDate")]
        public string DeliveryNoteDate { get; set; }
        [Name("ShippingPoint")]
        public string ShippingPoint { get; set; }
    }

    class Process_csv
    {
        public static string DivSwitch(string num)
        {
            switch (num)
            {
                case "10": return "CEM";
                case "11": return "AFR";
                case "20": return "MIC";
                case "30": return "AGG";
                case "40": return "RMC";
                case "50": return "PRE";
                case "99": return "CEM";
                default: return "";
            }
        }

        public static string CountrySwitch(string companyCode)
        {
            switch (companyCode)
            {
                case "SK": return "SVK";
                case "HU": return "HUN";
                case "RS": return "SRB";
                case "AT": return "AUT";
                case "CZ": return "SVK";
                case "DE": return "AUT";
                case "PL": return "SVK";
                default: return "";
            }
        }


        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File not found");
                return;
            }

            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8, BadDataFound = null };

            using (var reader = new StreamReader(args[0]))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<CSV_Record>();
                string path = args[0].Remove(args[0].LastIndexOf('\\'));
                path = path.Remove(path.LastIndexOf('\\'));
                foreach (var rec in records)
                {
                    string name = rec.CompanyCode.Substring(0, 2);

                    //PDF z InvoiceNumber
                    if (!File.Exists($"{path}\\{name}_pdf\\{rec.InvoiceNumber}.pdf") && rec.SoldToStatus != String.Empty)
                    {
                        if(!Directory.Exists($"{path}\\{name}_pdf")){ Directory.CreateDirectory($"{path}\\{name}_pdf"); }

                        using (FileStream fs = new FileStream($"{path}\\{name}_pdf\\{rec.InvoiceNumber}.pdf", FileMode.Create, FileAccess.Write))
                        {
                            fs.SetLength(500000);
                            Document doc = new Document(PageSize.A4);
                            PdfWriter.GetInstance(doc, fs);

                            doc.Open();

                            doc.Add(new Paragraph($"{nameof(rec.InvoiceNumber)}:{rec.InvoiceNumber}"));
                            doc.Add(new Paragraph($"\ncreated by Juraj Dobrota @Synergon"));

                            doc.Close();
                        }
                    }

                    //XML z InvoiceNumber
                    if (!File.Exists($"{path}\\{name}_xml\\{name}.xml") && rec.SoldToStatus != String.Empty)
                    {
                        if (!Directory.Exists($"{path}\\{name}_xml")) { Directory.CreateDirectory($"{path}\\{name}_xml"); }

                        using (FileStream fs = new FileStream($"{path}\\{name}_xml\\{rec.InvoiceNumber}.xml", FileMode.Create, FileAccess.Write))
                        {
                            fs.SetLength(500000);
                            string text = $"{nameof(rec.InvoiceNumber)}:{rec.InvoiceNumber}";

                            text += "\ncreated by Juraj Dobrota @Synergon";
                            fs.Write(Encoding.UTF8.GetBytes(text));
                        }
                    }

                    //PDF z DeliveryNotes podla Division a Country
                    if (!File.Exists($"{path}\\DeliveryNotes\\{DivSwitch(rec.Division)}\\{CountrySwitch(rec.CompanyCode.Substring(0, 2))}\\{rec.DeliveryNoteNumber}.pdf"))
                    {
                        if (!Directory.Exists($"{path}\\DeliveryNotes\\{DivSwitch(rec.Division)}\\{CountrySwitch(rec.CompanyCode.Substring(0, 2))}"))
                        {
                            Directory.CreateDirectory($"{path}\\DeliveryNotes\\{DivSwitch(rec.Division)}\\{CountrySwitch(rec.CompanyCode.Substring(0, 2))}");
                        }
                        using (FileStream fs = new FileStream($"{path}\\DeliveryNotes\\{DivSwitch(rec.Division)}\\{CountrySwitch(rec.CompanyCode.Substring(0, 2))}\\{rec.DeliveryNoteNumber}.pdf", FileMode.Create, FileAccess.Write))
                        {
                            fs.SetLength(50000);
                            Document doc = new Document(PageSize.A4);
                            PdfWriter.GetInstance(doc, fs);

                            doc.Open();

                            doc.Add(new Paragraph($"{nameof(rec.DeliveryNoteNumber)}:{rec.DeliveryNoteNumber}"));
                            doc.Add(new Paragraph($"\ncreated by Juraj Dobrota @Synergon"));

                            doc.Close();
                        }
                    }
                }

            }
            Console.WriteLine("Program has ended");
        }
    }
}
