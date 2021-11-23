using System;
using System.Runtime.InteropServices;
using MySqlConnector;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper
{
    public static class Main
    {
        public static Usuario Usuario { get; set; } = new ();

        private const string DbCfg = "Database.cfg";
        [DllImport(@"kernel32.dll")]

        static extern bool AllocConsole();
        
        
        public static void Init()
        {
            AllocConsole();
            try
            {
                //Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = false, ForwardEngineer = true });  //ForwardEngineer = true
                //Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = true });
                Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = true, ForwardEngineer = false});


                //CriaUsuario();

                //BuildCidadeEstado.Build();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        
        private static void CriaUsuario()
        {
            var user = new Usuario()
            {
                Login = "admin",
                Senha = Encrypt.ToSha512("admin"),
                Permissao = Usuario.Tipo_Permissao.Databaser,
                Nome = "André",
            };
            user.Save();
        }
    }
}