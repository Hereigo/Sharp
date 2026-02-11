namespace Zastosunok;

public partial class Form1 : Form
{
    private readonly NotifyIcon trayIcon;
    private readonly System.Windows.Forms.Timer timer;

    public Form1()
    {
        InitializeComponent();

        trayIcon = TrayIconCreate();

        timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += TimerExecuteByInterval;


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

    private void ButtonStart_Click(object sender, EventArgs e)
    {
        if (!timer.Enabled)
        {
            timer.Start();
            buttonStart.Text = "Stop";
        }
        else
        {
            timer.Stop();
            buttonStart.Text = "Start";
        }
    }

    private void ButtonReset_Click(object sender, EventArgs e)
    {
        labelSec.Text = "00";
        labelMin.Text = "00";
        labelHrs.Text = "00";
    }

    private void TimerExecuteByInterval(object? sender, EventArgs e)
    {
        buttonStart.Text = timer.Enabled ? "Stop" : "Start";

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
        labelHrs.Text = hour.ToString("D2");
    }
}