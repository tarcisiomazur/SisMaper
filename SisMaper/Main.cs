using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.Wpf;
using MySqlConnector;
using Persistence;
using PropertyChanged;
using SisMaper.API.WebMania;
using SisMaper.Models;

namespace SisMaper
{
    public class Main: INotifyPropertyChanged
    {
        public static Main Instance { get; set; }
        public static string Version => "Version: " + Application.ResourceAssembly.GetName().Version;
        public string Status { get; set; } = "Desconectado";
        public static Usuario Usuario { get; set; } = new ();
        public static Configuracoes? Empresa { get; set; }

        private const string DbCfg = "Database.cfg";
        
        internal static MySqlProtocol MySqlProtocol { get; set; }

        [DllImport(@"kernel32.dll")]
        static extern bool AllocConsole();

        static Main()
        {
            Instance = new Main();
        }
        
        public static void Init()
        {
            AllocConsole();
            try
            {
                MySqlProtocol = new MySqlProtocol(DbCfg) {ForwardEngineer = false, SkipVerification = true};
                Persistence.Persistence.Init(MySqlProtocol);
                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);
                InitChromium();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void InitChromium()
        {
            Cef.EnableHighDPISupport();
            var exitCode = SelfHost.Main(Array.Empty<string>());
            if (exitCode >= 0)
            {
                return;
            }

            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebCache\\Cache"),
                BrowserSubprocessPath = Process.GetCurrentProcess().MainModule.FileName
            };
            settings.CefCommandLineArgs.Add("enable-media-stream");
            settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
            settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");
            Cef.Initialize(settings, false);
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}