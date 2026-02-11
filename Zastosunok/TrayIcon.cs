namespace Zastosunok;

public partial class Form1
{
    private static string freeGb = "?";

    private NotifyIcon TrayIconCreate()
    {
        // Create tray icon
        var trayIcon = new NotifyIcon
        {
            Icon = this.Icon,
            Visible = true,
            Text = "Zastosunok"
        };

        trayIcon.DoubleClick += TrayIcon_DoubleClick;

        ContextMenuStrip menu = new();
        menu.Items.Add("Show", null, (s, e) => ShowForm());
        menu.Items.Add("Exit", null, (s, e) => ExitApplication());
        trayIcon.ContextMenuStrip = menu;

        return trayIcon;
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        WriteIconText();
    }

    private void ShowForm()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
    }

    private void ExitApplication()
    {
        trayIcon.Visible = false; // Always set before exiting!!!
        trayIcon.Dispose();
        Application.Exit();
    }

    private void WriteIconText()
    {
        freeGb = GetFreeSpaceInGb("C:\\").ToString();

        if (freeGb != "?" && freeGb != "-1")
        {


            trayIcon.Icon = CreateBorderedTextIcon(freeGb);



            //if (freeGb.Length == 2) freeGb = "." + freeGb;
            //else if (freeGb.Length == 1) freeGb = ".  " + freeGb;

            //Font fontToUse = new("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            //Brush brushToUse = new SolidBrush(switcher ? Color.Red : Color.HotPink);

            //// switcher = !switcher;

            //IntPtr hIcon;
            //Bitmap bitmapImageText = new(18, 12); // width, height

            //using Graphics g = Graphics.FromImage(bitmapImageText);
            //g.Clear(Color.Transparent);
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            //g.DrawString(freeGb.ToString(), fontToUse, brushToUse, -4, -2);
            //hIcon = (bitmapImageText.GetHicon());
            //trayIcon.Icon = Icon.FromHandle(hIcon);

            ////DestroyIcon(hIcon.ToInt32);
        }
    }
}
