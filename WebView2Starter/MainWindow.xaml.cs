using System.IO;
using System.Windows;

namespace WebView2Starter
{
    public class MyFileInfo
    {
        public long fileSize { get; set; }
        public string fileName { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await WebView.EnsureCoreWebView2Async();

            string UserProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";
            string path = Path.Combine(UserProfileFolder, "aaa\\tax");
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                Console.WriteLine($"{fi.Length} - {fi.FullName}");
            }

            var payload = new List<MyFileInfo>();

            for (int i = 0; i < 5; i++)
            {
                payload.Add(new MyFileInfo
                {
                    fileSize = new FileInfo(files[i]).Length,
                    fileName = new FileInfo(files[i]).FullName
                });
            }

            string json = System.Text.Json.JsonSerializer.Serialize(payload);

            string htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "index.html"
            );

            WebView.Source = new Uri(htmlPath);

            // Handle messages from the web content
            WebView.CoreWebView2.WebMessageReceived += (s, e) =>
            {
                // string message = e.TryGetWebMessageAsString();
                // MessageBox.Show($"From Web: {message}");

                // Send a message back to the web content
                // WebView.ExecuteScriptAsync(
                //     "alert('Hello from C#!')"
                // );

                WebView.CoreWebView2.ExecuteScriptAsync(
                    $"window.receiveData({json});"
                );
            };
        }
    }
}
