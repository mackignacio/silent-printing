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

class Print
{
    private int verticalSpacing = 0;
    public Print(string printer, string paper, int width, int height, string[] args)
    {
        verticalSpacing = height;

        using (PrintDocument pd = new PrintDocument())
        {
            pd.PrinterSettings.PrinterName = printer;
            pd.DefaultPageSettings.PaperSize = new PaperSize(paper, width, height);
            pd.PrintPage += PrintPage(args);
            pd.Print();
        }
    }

    private PrintPageEventHandler PrintPage(string[] args)
    {
        return (object sender, PrintPageEventArgs e) =>
        {
            Graphics g = e.Graphics;
            Logo(args[0], g, new PointF(50, 0));
        };
    }

    private void Logo(string imgURL, Graphics g, PointF point)
    {
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(imgURL);
            MemoryStream ms = new MemoryStream(bytes);
            Image img = Image.FromStream(ms);
            g.DrawImage(img, point);
        }
    }

    private int VerticalSpacing(int spacing)
    {
        verticalSpacing += spacing;
        return verticalSpacing;
    }

    private int TextWidht(string text, Font font)
    {
        return TextRenderer.MeasureText(text, font).Width;
    }

    private int Center(int widht, int textLength)
    {
        int w = (widht / 2);
        int tl = (textLength / 2);
        return w - tl;
    }
}



