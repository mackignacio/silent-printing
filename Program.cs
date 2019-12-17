using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace silent_printing
{
    class Program
    {
        static int currentSpacing = 0;
        static void Main(string[] args)
        {
            Print("EPSON TM-T82II Receipt", "Roll Paper 58 x 297 mm", 280, AddSpacing(5));
        }

        private static void Print(string printer, string paper, int width, int heigth)
        {
            using (PrintDocument pd = new PrintDocument())
            {
                pd.PrinterSettings.PrinterName = printer;
                pd.DefaultPageSettings.PaperSize = new PaperSize(paper, width, heigth);
                pd.PrintPage += Pd_PrintPage;
                pd.Print();
            }
        }

        private static void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
        }

        private static int AddSpacing(int spacing)
        {
            currentSpacing += spacing;
            return currentSpacing;
        }
    }
}
