using System.Timers;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private static System.Timers.Timer? aTimer;
        private static string freeGb = "?";

        public Form1()
        {
            InitializeComponent();
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            
            WriteIconText();
            RunTimer();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void RunTimer()
        {
            aTimer = new System.Timers.Timer(60 * 1000 * 10); // 10 minutes
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            WriteIconText();
        }

        private void WriteIconText()
        {
            freeGb = GetFreeSpaceInGb("C:\\").ToString();

            Font fontToUse = new("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.Red);
            Bitmap bitmapText = new(18, 12);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(freeGb.ToString(), fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            notifyIcon1.Icon = System.Drawing.Icon.FromHandle(hIcon);

            //DestroyIcon(hIcon.ToInt32);
        }

        private static float GetFreeSpaceInGb(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace / 1024 / 1024 / 1024; // in GB
                }
            }
            return -1;
        }
    }
}
