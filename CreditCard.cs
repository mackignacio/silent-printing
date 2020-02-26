using silent_printing.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

class CreditCard
{
    Int32 printType = 0;
    private int verticalSpacing = 0;
    private int logoHeight = 0;
    public CreditCard(string printer, string paper, int width, int height, string[] args)
    {
        verticalSpacing = height;

        using (PrintDocument pd = new PrintDocument())
        {
            pd.PrinterSettings.PrinterName = printer;
            pd.DefaultPageSettings.PaperSize = new PaperSize(paper, width, height);
            pd.PrintPage += PrintPage(args, width);
            pd.Print();
        }
    }

    public void Type(string type)
    {
        printType = (type == "EMV") ? 0 : 1;
    }

    private PrintPageEventHandler PrintPage(string[] args, int width)
    {
        return (object sender, PrintPageEventArgs e) =>
        {
            Graphics graphics = e.Graphics;
            Logo(args[0], graphics, new PointF(50, 0));
            Headers(args, width, graphics);
            DateAndTime(args[4], graphics);
            Transaction("SALE",  width, graphics);
            InvoiceDetails(args[5], width, graphics);
            Signature("I agree to pay above total amount according to card issuer agreement.", width, graphics);
            Spacer(width, graphics);
        };
    }
    private void Spacer( int width, Graphics graphics)
    {
        string text = "----------------";
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 8, FontStyle.Regular))
        {
            graphics.DrawString(text, font, Brushes.Black, Position(width, text, font, VerticalSpacing(20)));
        }
    }

    private int VerticalSpacing(int spacing)
    {
        verticalSpacing += spacing;
        return verticalSpacing;
    }

    private PointF Position(int width, string text, Font font, int height, float padding = 0)
    {
        int textWidth = TextWidth(text, font);
        float center = Center(width, textWidth);
        return new PointF(center + padding, height);
    }

    private int TextWidth(string text, Font font)
    {
        return TextRenderer.MeasureText(text, font).Width;
    }

    private float Center(int width, int textLength)
    {
        float margin = (float)(width / 2) - (float)(textLength / 2);
        return margin;
    }

    private Font GetCustomFont(byte[] fontData, float size, FontStyle style)
    {
        using (PrivateFontCollection pfc = new PrivateFontCollection())
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            pfc.AddMemoryFont(fontPtr, fontData.Length);
            Marshal.FreeCoTaskMem(fontPtr);
            return new Font(pfc.Families[0], size, style);
        }
    }

    private string[] LimitTextWidth(string text, int length)
    {
        string strlist = "";
        string excess = "";
        string[] split = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        split.ToList().ConvertAll(str =>
        {
            if (strlist.Length < length) strlist += str + " ";
            else excess += str + " ";
            return str;
        });

        return new string[] { strlist, excess };
    }

    private void Logo(string imgURL, Graphics graphics, PointF point)
    {
        using (WebClient wc = new WebClient())
        {
            byte[] bytes = wc.DownloadData(imgURL);
            MemoryStream ms = new MemoryStream(bytes);
            Image img = Image.FromStream(ms);
            graphics.DrawImage(img, point);
            logoHeight = img.Height;
        }
    }

    private void Headers(string[] args, int width, Graphics graphics)
    {
        using (Font headerFont = GetCustomFont(Resources.HelveticaNeueBd, 12, FontStyle.Bold))
            SetBusinessName(LimitTextWidth(args[1], 20), headerFont, width, graphics);
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 10, FontStyle.Regular))
        {
            SetBusinessAddress(LimitTextWidth(args[2], 35), font, width, graphics);
            graphics.DrawString(args[3], font, Brushes.Black, Position(width, args[3], font, VerticalSpacing(15), 10));
        }
    }

    private void SetBusinessAddress(string[] text, Font font, int width, Graphics graphics)
    {
        graphics.DrawString(text[0], font, Brushes.Black, Position(width, text[0], font, VerticalSpacing(20), 20));
        if (text.Length > 1) graphics.DrawString(text[1], font, Brushes.Black, Position(width, text[1], font, VerticalSpacing(15)));
    }

    private void SetBusinessName(string[] text, Font font, int width, Graphics graphics)
    {
        graphics.DrawString(text[0], font, Brushes.Black, Position(width, text[0], font, VerticalSpacing(logoHeight + 25)));
        if (text.Length > 1) graphics.DrawString(text[1], font, Brushes.Black, Position(width, text[1], font, VerticalSpacing(20)));
    }

    private void SetItems(string[] array, float x, Graphics graphics, byte[] fontData, FontStyle style, int fontSize = 9)
    {
        using (Font font = GetCustomFont(fontData, fontSize, style))
        {
            for (int runs = 0; runs < array.Length; runs++)
                graphics.DrawString(array[runs], font, Brushes.Black, new PointF(x, VerticalSpacing(runs == 0 ? 20 : 12)));
        }
    }

    private void SetItems(string[] array, Func<int, string, int> x, Func<int, int> y, Graphics graphics, byte[] fontData, FontStyle style, int fontSize = 9)
    {
        using (Font font = GetCustomFont(fontData, fontSize, style))
        {
            for (int runs = 0; runs < array.Length; runs++)
                graphics.DrawString(array[runs], font, Brushes.Black, new PointF(x(runs, array[runs]), y(runs)));
        }
    }

    private void SetItems(string[] array, float x, Func<int, int> y, Graphics graphics, byte[] fontData, FontStyle style, int fontSize = 9)
    {
        using (Font font = GetCustomFont(fontData, fontSize, style))
        {
            for (int runs = 0; runs < array.Length; runs++)
                graphics.DrawString(array[runs], font, Brushes.Black, new PointF(x, y(runs)));
        }
    }

    private void DateAndTime(string text, Graphics graphics)
    {
        int resetSpacing = verticalSpacing;
        string[] names = { "DATE :", "TIME :" };
        SetItems(names, 20, graphics, Resources.HelveticaNeueBd, FontStyle.Bold);
        verticalSpacing = resetSpacing;

        string[] split = text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        List<string> list = new List<string>();

        for (int runs = 0; runs < split.Length; runs++)
        {
            list.Add(split[runs]);
        }

        SetItems(list.ToArray(), 70, graphics, Resources.HelveticaNeue, FontStyle.Regular);
    }

    private void InvoiceDetails(string args, int width, Graphics graphics)
    {
        int resetSpacing = VerticalSpacing(10);
        string[] names = {  "ACCT :", "APP NAME :", "AID :", "ARQC :", "ENTRY :", "APPROVAL :" };
        SetItems(names, 20, graphics, Resources.HelveticaNeueBd, FontStyle.Bold);
        verticalSpacing = resetSpacing ;
        string[] split = args.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        SetItems(SplitInvoiceDetails(split), 120, graphics, Resources.HelveticaNeue, FontStyle.Regular);
        string total = split[split.Length - 1];
       
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 12, FontStyle.Bold))
        {
            graphics.DrawString("TOTAL : ", font, Brushes.Black, Position(width, "TOTAL :", font, VerticalSpacing(25), -50));
            graphics.DrawString("$"+total, font, Brushes.Black, Position(width, total, font, VerticalSpacing(0)));
        }     
    }

    private string[] SplitInvoiceDetails(string[] split)
    {
        List<string> list = new List<string>();

        for (int runs = 0; runs < split.Length - 1; runs++)
        {
            list.Add(split[runs]);
        }

        return list.ToArray();
    }

    private void Transaction(string text, int width, Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeueBd, 12, FontStyle.Bold))
            graphics.DrawString(text, font, Brushes.Black, Position(width, text, font, VerticalSpacing(25)));
    }

    private void Signature(string args, int width, Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 9, FontStyle.Regular))
        {
            string[] text = LimitTextWidth(args, 35);
            graphics.DrawString(text[0], font, Brushes.Black, Position(width , text[0], font, VerticalSpacing(25), 15));
            if (text.Length > 1) graphics.DrawString(text[1], font, Brushes.Black, Position(width, text[1], font, VerticalSpacing(10)));
            graphics.DrawLine(new Pen(Color.Black, 1), 20, VerticalSpacing(50), 290, VerticalSpacing(0));
        }

        string sig = "SIGNATURE";
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 9, FontStyle.Bold))
            graphics.DrawString(sig, font, Brushes.Black, Position(width, sig, font, VerticalSpacing(5)));
    }
}
