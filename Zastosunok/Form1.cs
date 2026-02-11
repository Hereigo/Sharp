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

    private void buttonStart_Click(object sender, EventArgs e)
    {
        if (!timer.Enabled) { timer.Start(); }
        else { timer.Stop(); }
    }
    private void buttonReset_Click(object sender, EventArgs e)
    {

    }

    private void TimerExecuteByInterval(object? sender, EventArgs e)
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
}

