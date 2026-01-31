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

            string htmlPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "wwwroot",
                "index.html"
            );

            WebView.Source = new Uri(htmlPath);

            // Handle messages from the web content
            WebView.CoreWebView2.WebMessageReceived += (s, e) =>
            {
                string message = e.TryGetWebMessageAsString();
                // MessageBox.Show($"From Web: {message}");

                // Send a message back to the web content
                // WebView.ExecuteScriptAsync(
                //     "alert('Hello from C#!')"
                // );

                string json = GetData(message);

                WebView.CoreWebView2.ExecuteScriptAsync(
                    $"window.receiveData({json});"
                );
            };
        }

        private string GetData(string folderName)
        {
            string UserProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\";
            string path = Path.Combine(UserProfileFolder, "Downloads", folderName);

            if (!Directory.Exists(path))
            {
                return "[]";
            }

            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            var payload = new List<MyFileInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                payload.Add(new MyFileInfo
                {
                    fileSize = new FileInfo(files[i]).Length,
                    fileName = new FileInfo(files[i]).FullName
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(payload);
        }
    }
}

// https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/wpf