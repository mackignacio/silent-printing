using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace silent_printing
{
    class Program
    {
        static int currentSpacing = 0;
        static void Main(string[] args)
        {
            Print("EPSON TM-T82II Receipt", "Roll Paper 58 x 297 mm", 280, AddSpacing(5), args);
        }

        private static void Print(string printer, string paper, int width, int heigth, string[] args)
        {
            using (PrintDocument pd = new PrintDocument())
            {
                pd.PrinterSettings.PrinterName = printer;
                pd.DefaultPageSettings.PaperSize = new PaperSize(paper, width, heigth);
                pd.PrintPage += PrintPage(args);
                pd.Print();
            }
        }

        private static PrintPageEventHandler PrintPage(string[] args)
        {
            return (object sender, PrintPageEventArgs e) =>
            {
                Graphics g = e.Graphics;
                Logo(args[0], g, new PointF(50, 0));
            };
        }

        private static void Logo(string imgURL, Graphics g, PointF point)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] bytes = wc.DownloadData(imgURL);
                MemoryStream ms = new MemoryStream(bytes);
                Image img = Image.FromStream(ms);
                g.DrawImage(img, point);
            }
        }

        private static int AddSpacing(int spacing)
        {
            currentSpacing += spacing;
            return currentSpacing;
        }
    }
}
