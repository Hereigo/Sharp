namespace Zastosunok;

public partial class Form1 : Form
{
    private NotifyIcon trayIcon;
    private System.Windows.Forms.Timer timer;

    public Form1()
    {
        InitializeComponent();

        // Create tray icon
        trayIcon = new NotifyIcon();
        trayIcon.Icon = this.Icon;
        trayIcon.Visible = true;
        trayIcon.Text = "My Application";

        trayIcon.DoubleClick += TrayIcon_DoubleClick;

        ContextMenuStrip menu = new ContextMenuStrip();
        menu.Items.Add("Show", null, (s, e) => ShowForm());
        menu.Items.Add("Exit", null, (s, e) => ExitApplication());
        trayIcon.ContextMenuStrip = menu;

        //labelSec.Text = "59";

        timer = new System.Windows.Forms.Timer();
        timer.Interval = 1000;
        timer.Tick += Timer_Tick;

        // TEST
        timer.Start();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;  // Cancel closing
            this.Hide();
        }
        else
        {
            base.OnFormClosing(e);
        }
    }

    private void TrayIcon_DoubleClick(object sender, EventArgs e)
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

    private void Timer_Tick(object sender, EventArgs e)
    {
        var sec = int.TryParse(labelSec.Text, out int s) ? s : 0;
        var min = int.TryParse(labelMin.Text, out int m) ? m : 0;
        var hour = int.TryParse(labelHrs.Text, out int h) ? h : 0;

        sec++;
        if (sec >= 60)
        {
            sec = 0;
            min++;
        }
        labelSec.Text = sec.ToString("D2");
        if (min >= 60)
        {
            min = 0;
            hour++;
        }
        labelMin.Text = min.ToString("D2");

    }

    private void buttonStart_Click(object sender, EventArgs e)
    {
        if (!timer.Enabled) { timer.Start(); }
        else { timer.Stop(); }
    }
    private void buttonReset_Click(object sender, EventArgs e)
    {

    }
}

