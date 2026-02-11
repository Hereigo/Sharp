namespace Zastosunok;

public partial class Form1
{
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

        // menu.Items.Add("Show", null, (s, e) => ShowForm());

        menu.Items.Add("Exit", null, (s, e) => ExitApplication());
        trayIcon.ContextMenuStrip = menu;

        return trayIcon;
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        ShowForm();
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
}
