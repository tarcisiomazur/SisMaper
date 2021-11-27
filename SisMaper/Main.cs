using System;
using System.Runtime.InteropServices;
using MySqlConnector;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper
{
    public static class Main
    {
        public static Usuario Usuario { get; set; } = new ();
        public static Configuracoes? Empresa { get; set; }

        private const string DbCfg = "Database.cfg";
        [DllImport(@"kernel32.dll")]

        static extern bool AllocConsole();
        
        public static void Init()
        {
            AllocConsole();
            try
            {
                Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { ForwardEngineer = false, SkipVerification = true});
                Empresa = DAO.Load<Configuracoes>(1);
                WebManiaConnector.Init(Empresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}