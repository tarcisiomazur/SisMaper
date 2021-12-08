using System;
using System.ComponentModel;
using System.Windows;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;

namespace SisMaper
{
    public class Main : INotifyPropertyChanged
    {
        private const string DbCfg = "Database.cfg";
        public static Main Instance { get; set; }
        public static string Version => "Version: " + Application.ResourceAssembly.GetName().Version;
        public string Status { get; set; } = "Desconectado";
        public static Usuario Usuario { get; set; } = new();
        public static Configuracoes? Empresa { get; set; }
        internal static MySqlProtocol MySqlProtocol { get; set; }

        static Main()
        {
            Instance = new Main();
        }

        public static void Init()
        {
            try
            {
                MySqlProtocol = new MySqlProtocol(DbCfg) {ForwardEngineer = false, SkipVerification = true};
                Persistence.Persistence.Init(MySqlProtocol);
                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}