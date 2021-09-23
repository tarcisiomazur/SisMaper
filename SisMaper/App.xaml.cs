using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using MySqlConnector;

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

        public new static App Current => (App) Application.Current; 
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
                Persistence.Persistence.Init(new MySqlProtocol(DbCfg){SkipVerification = true});  //ForwardEngineer = true
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
