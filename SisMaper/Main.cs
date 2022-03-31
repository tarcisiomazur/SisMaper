using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Views;
using SisMaper.Views.Templates;

namespace SisMaper
{
    public class Main : INotifyPropertyChanged
    {
        private const string DbCfg = "Database.cfg";
        
        private static string Path;
        public static Main Instance { get; set; }
        public static string Version => "Versão: " + Application.ResourceAssembly.GetName().Version;
        public string Status { get; set; } = "Desconectado";
        public static Usuario Usuario { get; set; } = new();
        public static Configuracoes Empresa { get; set; }
        private static MySqlProtocol MySqlProtocol { get; set; }
        private static Dispatcher Dispatcher => Application.Current.Dispatcher;

        static Main()
        {
            Instance = new Main();
        }

        public static void Init()
        {
            try
            {
                Path = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Agromaper\SisMaper").GetValue("Path").ToString();
                var x = Path + "DataBase.sql";
                Console.WriteLine(x);
                string dll = File.ReadAllText(x);
                MySqlProtocol = new MySqlProtocol(Path + DbCfg, dll) {ForwardEngineer = false, SkipVerification = true};
                MySqlProtocol.Connected += Connected;
                MySqlProtocol.Disconnected += Disconnected;
                MySqlProtocol.Reconnecting += Reconnecting;
                Persistence.Persistence.Init(MySqlProtocol);
                Empresa = DAO.Load<Configuracoes>(1);
                if (Empresa is null)
                    (Empresa = new Configuracoes {Id = 1}).Save();
                WebManiaConnector.Init(Empresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Disconnected()
        {
            Instance.Status = "Desconectado";
        }

        private static void Reconnecting()
        {
            Instance.Status = "Reconectando";
            Dispatcher.InvokeAsync(() =>
            {
                if (MainWindow.Instance is not null)
                {
                    new LostConnection {Owner = MainWindow.Instance}.Show();
                }
            });
        }

        private static void Connected()
        {
            Instance.Status = "Conectado";
            Dispatcher.InvokeAsync(() => LostConnection.Instance?.Close());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}