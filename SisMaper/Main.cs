using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.Views;

namespace SisMaper
{
    public static class Main
    {
        public static string Version => "Versão: 1.1.0";
        public static Usuario Usuario { get; set; } = new ();
        public static Configuracoes? Empresa { get; set; }
        public static string Status { get; set; }

        private const string DbCfg = "Database.cfg";
        
        internal static MySqlProtocol MySqlProtocol { get; set; }

        [DllImport(@"kernel32.dll")]
        static extern bool AllocConsole();
        
        public static void Init()
        {
            AllocConsole();
            try
            {
                MySqlProtocol = new MySqlProtocol(DbCfg) {ForwardEngineer = false, SkipVerification = false, MonitorIntervalTime = 1};
                Persistence.Persistence.Init(MySqlProtocol);
                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);

                //CriaUsuario()
                //BuildCidadeEstado.Build();
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