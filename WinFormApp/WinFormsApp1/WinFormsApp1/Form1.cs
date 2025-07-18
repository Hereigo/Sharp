namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Test();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void Test()
        {
            float str = 0;

            str = GetTotalFreeSpace("C:\\");

            CreateTextIcon(str);
        }

        public void CreateTextIcon(float str)
        {
            Font fontToUse = new Font("Microsoft Sans Serif", 15, FontStyle.Bold, GraphicsUnit.Pixel);
            Brush brushToUse = new SolidBrush(Color.Red);
            Bitmap bitmapText = new Bitmap(16, 16);
            Graphics g = System.Drawing.Graphics.FromImage(bitmapText);

            IntPtr hIcon;

            g.Clear(Color.Transparent);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(str.ToString(), fontToUse, brushToUse, -4, -2);
            hIcon = (bitmapText.GetHicon());
            notifyIcon1.Icon = System.Drawing.Icon.FromHandle(hIcon);

            //DestroyIcon(hIcon.ToInt32);
        }

        private float GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace/1024/1024/1024; // in GB
                }
            }
            return -1;
        }
    }
}
