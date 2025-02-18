using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using Microsoft.UI.Xaml.Controls;
using System.IO;

namespace WebView2TouchDemo
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                var env = await CoreWebView2Environment.CreateAsync();
                await WebView.EnsureCoreWebView2Async(env);

                string htmlPath = Path.Combine(AppContext.BaseDirectory, "test.html");
                WebView.Source = new Uri(htmlPath);
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = Content.XamlRoot,
                    Title = "WebView2 Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            TouchInjector.SimulateTouchInWindow(windowHandle, 500, 500);
            WebView.ReleasePointerCaptures();
        }
    }
}