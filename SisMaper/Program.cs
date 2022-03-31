using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.Wpf;

namespace SisMaper
{
    public static class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [STAThread]
        public static void Main(string[] args)
        {
            InitChromium(args);
            if (args.Any(arg => arg.Contains("--type="))) return;
#if !DEBUG
            ShowWindow(GetConsoleWindow(), SW_HIDE);
#endif
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static void InitChromium(string[] args)
        {
            Cef.EnableHighDPISupport();
            var exitCode = SelfHost.Main(args);
            if (exitCode >= 0)
            {
                return;
            }

            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WebCache\\Cache"),
                BrowserSubprocessPath = Process.GetCurrentProcess().MainModule.FileName
            };
            settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            Cef.Initialize(settings, false);
        }
    }
}