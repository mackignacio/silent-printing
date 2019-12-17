using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace silent_printing
{
    class Program
    {
        static int verticalSpacing = 0;
        static void Main(string[] args)
        {
            Print("EPSON TM-T82II Receipt", "Roll Paper 58 x 297 mm", 280, VerticalSpacing(5), args);
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

        private static int VerticalSpacing(int spacing)
        {
            verticalSpacing += spacing;
            return verticalSpacing;
        }

        private static int TextWidht(string text, Font font)
        { 
            return TextRenderer.MeasureText(text, font).Width;
        }

        private static int Center(int widht, int textLength)
        {
            int w = (widht / 2);
            int tl = (textLength / 2);
            return w - tl;
        }
    }
}
