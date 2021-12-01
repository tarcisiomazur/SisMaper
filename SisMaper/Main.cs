using System;
using System.Runtime.InteropServices;
using System.Windows;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper
{
    public static class Main
    {
        public static string Version => "Version: " + Application.ResourceAssembly.GetName().Version;
        public static Usuario Usuario { get; set; } = new ();
        public static Configuracoes? Empresa { get; set; }

        private const string DbCfg = "Database.cfg";
        
        internal static MySqlProtocol MySqlProtocol { get; set; }

        [DllImport(@"kernel32.dll")]
        static extern bool AllocConsole();
        
        public static void Init()
        {
            AllocConsole();
            try
            {
                MySqlProtocol = new MySqlProtocol(DbCfg) {ForwardEngineer = false, SkipVerification = true, MonitorIntervalTime = 100};
                Persistence.Persistence.Init(MySqlProtocol);

                //BuildCidadeEstado.Build();

                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);

                //CriaUsuario()
                
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
                Nome = "André",
                Login = "admin",
                Senha = "admin",
                Permissao = Usuario.Tipo_Permissao.Databaser
            };
            u.Save();
        }
    }
}