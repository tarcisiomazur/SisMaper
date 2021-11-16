using System;
using System.Runtime.InteropServices;
using System.Windows;
using MySqlConnector;
using SisMaper.Models;  /**/
using SisMaper.Tools;   /**/

namespace SisMaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DbCfg = "Database.cfg";
        [DllImport(@"kernel32.dll")]

        static extern bool AllocConsole();

        public new static App Current => (App)Application.Current;
        public App()
        {
            AllocConsole();
            Init();
            //new Thread(Init).Start();
        }
        

        public void Init()
        {
            try
            {
                //Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = true });  //ForwardEngineer = true
                Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = true });
                //Persistence.Persistence.Init(new MySqlProtocol(DbCfg) { SkipVerification = false, ForwardEngineer = true});

                //CriaUsuario();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void CriaUsuario()
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
