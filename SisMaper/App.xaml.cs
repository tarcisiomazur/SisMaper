using System.Windows;
using ControlzEx.Theming;

namespace SisMaper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {

        public App()
        {
            SisMaper.Main.Init();
            Startup += (_, _) =>
            {
                ThemeManager.Current.ChangeTheme(this, "Light.Teal");
            };
        }

    }
}
