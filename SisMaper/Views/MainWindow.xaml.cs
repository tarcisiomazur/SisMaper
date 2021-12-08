using System.Windows;
using System.Windows.Controls;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            Instance = this;
            DataContextChanged += SetActions;
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            foreach (TabItem tabItem in TabCtrl.Items)
            {
                if (tabItem.Visibility != Visibility.Visible) continue;
                TabCtrl.SelectedItem = tabItem;
                return;
            }
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MainViewModel newVm)
            {
                newVm.ShowProgress += Helper.MahAppsDefaultProgress;
                newVm.ShowMessage += Helper.MahAppsDefaultMessage;
                Main.MySqlProtocol.Connected += newVm.Connected;
                Main.MySqlProtocol.Disconnected += newVm.Disconnected;
                Main.MySqlProtocol.Reconnecting += newVm.Reconnecting;
            }

            if (e.OldValue is MainViewModel oldVm)
            {
                Main.MySqlProtocol.Connected -= oldVm.Connected;
                Main.MySqlProtocol.Disconnected -= oldVm.Disconnected;
                Main.MySqlProtocol.Reconnecting -= oldVm.Reconnecting;
            }

            Main.Instance.Status = Main.MySqlProtocol.IsConnected ? "Conectado" : "Desconectado";
        }

        private void OpenConfig(object sender, RoutedEventArgs e)
        {
            new ViewConfiguracoes(Main.Empresa).ShowDialog();
            e.Handled = true;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow = new Login()).Show();
            Close();
        }
    }
}