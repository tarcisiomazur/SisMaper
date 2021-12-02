using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;

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

                //CriaUsuario();

                //BuildNCM.Run();

                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void CriaUsuario()
        {
            Usuario u = new Usuario()
            {
                Login = "admin",
                Senha = Encrypt.ToSha512("admin"),
                Permissao = Usuario.Tipo_Permissao.Databaser
            };
            u.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}