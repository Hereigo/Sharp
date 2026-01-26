using System.IO;
using System.Windows;

namespace WebView2Starter
{
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
                MessageBox.Show($"From Web: {message}");

                // Send a message back to the web content
                WebView.ExecuteScriptAsync(
                    "alert('Hello from C#!')"
                );
            };
        }
    }
}
