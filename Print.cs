using Newtonsoft.Json;
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
using System.Web;
using System.Windows.Forms;

class Print
{
    private int verticalSpacing = 0;
    private int logoHeight = 0;
    public Print(string printer, string paper, int width, int height, string[] args)
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

    private PrintPageEventHandler PrintPage(string[] args, int width)
    {
        return (object sender, PrintPageEventArgs e) =>
        {
            Graphics graphics = e.Graphics;
            string[] decodedUrl = HttpUtility.UrlDecode(args[0]).Replace("neutronpos:", "").Trim(new char[] { '"' }).Split(new string[] { "\" \"" }, StringSplitOptions.RemoveEmptyEntries);
            Logo(decodedUrl[0], graphics, new PointF(50, 0));
            Headers(decodedUrl, width, graphics);
            InvoiceDetails(decodedUrl[4], graphics);
            ItemDetails(decodedUrl[6], graphics);
            PaymentDetails(decodedUrl[5], graphics);
            graphics.DrawLine(new Pen(Color.Black, 1), 20, VerticalSpacing(30), 290, VerticalSpacing(0));
        };
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

    private void InvoiceDetails(string args, Graphics graphics)
    {
        int resetSpacing = verticalSpacing;
        string[] names = { "Receipt No. :", "Date Issue :", "Cashier :", "Station :", "Customer :" };
        SetItems(names, 20, graphics, Resources.HelveticaNeueBd, FontStyle.Bold);
        verticalSpacing = resetSpacing;
        SetItems(SplitInvoiceDetails(args, 6), 100, graphics, Resources.HelveticaNeue, FontStyle.Regular);
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

    private string[] SplitInvoiceDetails(string text, int length)
    {
        string[] split = text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        List<string> list = new List<string>();

        for (int runs = 0; runs < length; runs++)
        {
            string value = split.Length > runs ? split[runs] : "";
            list.Add(value);
        }

        return list.ToArray();
    }

    private void PaymentDetails(string args, Graphics graphics)
    {
        VerticalSpacing(5);
        SetTitle("PAYMENT DETAILS", graphics);
        int resetSpacing = verticalSpacing;
        string[] names = { "Subtotal :", "Less Discount :", "Total :", "Add Tax :", "Grand Total :", "Amount :", "Change :" };
        SetItems(names, 50, PaymentNameSpacing, graphics, Resources.HelveticaNeue, FontStyle.Regular, 10);
        verticalSpacing = resetSpacing;
        SetItems(SplitInvoiceDetails(args, 7), PaymentNameMargin(150), PaymentNameSpacing, graphics, Resources.HelveticaNeue, FontStyle.Regular, 10);
        verticalSpacing = resetSpacing;
        PaymentSeparatorLines(graphics);
    }

    private void SetTitle(string title, Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeueBd, 14, FontStyle.Regular))
        {
            graphics.DrawString(title, font, Brushes.Black, new PointF(20, VerticalSpacing(15)));
            graphics.DrawLine(new Pen(Color.Black, 1), 20, VerticalSpacing(22), 290, VerticalSpacing(0));
        }
    }

    private void PaymentSeparatorLines(Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeueBd, 10, FontStyle.Bold))
        {
            for (int runs = 0; runs < 3; runs++)
            {
                if (runs == 0)
                    graphics.DrawLine(new Pen(Color.Black, 2), 50, VerticalSpacing(40), 230, VerticalSpacing(0));
                if (runs > 0)
                    graphics.DrawLine(new Pen(Color.Black, 2), 50, VerticalSpacing(45), 230, VerticalSpacing(0));
                if (runs == 2)
                    graphics.DrawLine(new Pen(Color.Black, 2), 50, VerticalSpacing(4), 230, VerticalSpacing(0));
            }
        }
    }

    private Func<int, string, int> PaymentNameMargin(int margin)
    {
        return (int index, string text) =>
        {
            int excess = 9 - text.Length;
            int additional = excess * 8;
            return margin + additional;
        };
    }
    private int PaymentNameSpacing(int index)
    {
        return VerticalSpacing(index == 0 ? 5 : index % 2 == 0 ? 30 : 15);
    }

    private void ItemDetails(string args, Graphics graphics)
    {
        VerticalSpacing(-10);
        SetTitle("ITEM DETAILS", graphics);
        SetItemHeaders(graphics);
        Item[] json = JsonConvert.DeserializeObject<Item[]>(args);
        SetItemsDetailsJSON(json, graphics);
    }

    private void SetItemHeaders(Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeueBd, 8, FontStyle.Regular))
        {
            graphics.DrawString("DESCRIPTION", font, Brushes.Black, new PointF(25, VerticalSpacing(5)));
            graphics.DrawString("QTY", font, Brushes.Black, new PointF(145, VerticalSpacing(0)));
            graphics.DrawString("PRICE", font, Brushes.Black, new PointF(220, VerticalSpacing(0)));
        }
    }

    private void SetItemsDetailsJSON(Item[] json, Graphics graphics)
    {
        for (int runs = 0; runs < json.Length; runs++) SetItemsJSON(json[runs], runs, graphics);
    }

    private void SetItemsJSON(Item json, int index, Graphics graphics)
    {
        using (Font font = GetCustomFont(Resources.HelveticaNeue, 10, FontStyle.Regular))
        {
            graphics.DrawString(json.name, font, Brushes.Black, new PointF(30, VerticalSpacing(15)));
            graphics.DrawString(json.quantity, font, Brushes.Black, new PointF(150, VerticalSpacing(0)));
            graphics.DrawString(json.price, font, Brushes.Black, new PointF(PaymentNameMargin(190)(index, json.price), VerticalSpacing(0)));
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
}



