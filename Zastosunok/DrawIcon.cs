using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Zastosunok;

public partial class Form1
{
    [DllImport("user32.dll")]
    extern static bool DestroyIcon(IntPtr hIcon);

    private Icon CreateBorderedTextIcon(string text)
    {
        int size = 16;
        Bitmap bmp = new Bitmap(size, size);

        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Narrow border rectangle
            Rectangle rect = new Rectangle(1, 1, size - 3, size - 3);
            using (Brush bgBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(bgBrush, rect);
            }
            using (Pen borderPen = new Pen(Color.Red, 1))
            {
                g.DrawRectangle(borderPen, rect);
            }

            // Draw centered text
            using (Font font = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Pixel))
            using (Brush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(text, font, textBrush, rect, sf);
            }
        }

        IntPtr hIcon = bmp.GetHicon();
        Icon icon = Icon.FromHandle(hIcon);
        Icon clone = (Icon)icon.Clone();

        DestroyIcon(hIcon);
        icon.Dispose();
        bmp.Dispose();

        return clone;
    }
}