
namespace silent_printing
{
    class Program
    {
        static void Main(string[] args)
        {
            string argBody = System.Web.HttpUtility
                 .UrlDecode(args[0])
                 .Replace("neutronpos:", "");
            string command = argBody.Substring(0, 1);
            string[] options = argBody.Substring(1)
                .Trim(new char[] { '"' })
                .Split(new string[] { "\" \"" }, System.StringSplitOptions.RemoveEmptyEntries);

            CashDrawer cashDrawer = new CashDrawer();

            if (command == "0" || command == "2")
            {
                cashDrawer.Open();
            }

            string printerName = ""; 

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printer.Contains("EPSON");

                if (printer.Contains("EPSON") || printer.Contains("Receipt"))
                {
                    printerName = printer;
                }

            }

            if (command == "1" || command == "2")
            {
                new Print(printerName, "Roll Paper 58 x 297 mm", 297, 0, options);
            }

            if (command == "3" || command == "4")
            {
                CreditCard card = new CreditCard(printerName, "Roll Paper 58 x 297 mm", 297, 0 );
                card.Type(command == "3" ? "EMV": "MGS");
                card.Print(options);
            }

        }

    }
}
