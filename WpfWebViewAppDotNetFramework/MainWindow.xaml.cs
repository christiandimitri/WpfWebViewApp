using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfWebViewApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Block _block;
        private HttpListener _httpListener;
        private const string BaseUrl = "http://localhost:8000/";

        public event PropertyChangedEventHandler PropertyChanged;

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle == null ? "Hello World" : _windowTitle;
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            InitializeValues();
            DataContext = this;
        }

        private void InitializeValues()
        {
            _block = new Block(new Random(100));
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Start the HTTP server
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(BaseUrl);
            _httpListener.Start();

            Task.Run(() => StartListener());

            // Ensure that CoreWebView2 is ready
            await webView.EnsureCoreWebView2Async(null);

            // Register the host object before navigating
            webView.CoreWebView2.AddHostObjectToScript("block", _block);

            // Load the React app URL into the WebView2 control
            webView.CoreWebView2.Navigate(BaseUrl + "index.html");

            // Set up the message received event handler
            webView.CoreWebView2.WebMessageReceived += webView_CoreWebView2WebMessageReceived;
        }

        private async Task StartListener()
        {
            string repoRoot = GetRepoRoot();

            while (_httpListener.IsListening)
            {
                var context = await _httpListener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                var filePath = Path.Combine(repoRoot, request.Url.LocalPath.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    var buffer = File.ReadAllBytes(filePath);
                    response.ContentType = GetMimeType(filePath);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                response.OutputStream.Close();
            }
        }

        private string GetMimeType(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".htm":
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                default:
                    return "application/octet-stream";
            }
        }

        private void webView_CoreWebView2WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string message = e.TryGetWebMessageAsString();

                // Parse the JSON message into an EventData object
                EventData eventData = JsonConvert.DeserializeObject<EventData>(message);

                // Process the eventData object
                MessageBox.Show($"Received message from React:\nType: {eventData.Type}\nValue: {eventData.Value}\nObject: {eventData.Object}", "Message from React", MessageBoxButton.OK, MessageBoxImage.Information);

                // Send a response back to React
                webView.CoreWebView2.PostWebMessageAsString("Hello again from WPF");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GetRepoRoot()
        {
            // Traverse up to the repository root directory assuming the structure of /bin/Debug/[repo]/build
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string repoRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;
            return Path.Combine(repoRoot, "build");
        }
        private void OnClosed(object sender, EventArgs e)
        {
            _httpListener.Stop();
            _httpListener.Close();
            base.OnClosed(e);
        }
        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
