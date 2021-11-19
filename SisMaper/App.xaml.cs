using System.Windows;

namespace SisMaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        public App()
        {
            SisMaper.Main.Init();
        }
        
    }
}
