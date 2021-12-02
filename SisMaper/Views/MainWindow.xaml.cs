using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Instance;
        public MainViewModel ViewModel => (MainViewModel) DataContext;

        public MainWindow()
        {
            Instance = this;

            MakeLogin();

            try
            {
                InitializeComponent();
                Initialize();
                SetActions();
            }
            catch (Exception ex)
            {
                Close();
            }
            
        }

        private void Initialize()
        {
            ViewModel.Initialize();
            if(TabCtrl.SelectedItem is TabItem{Visibility: Visibility.Collapsed})
            {
                foreach (TabItem tabItem in TabCtrl.Items)
                {
                    if (tabItem.Visibility == Visibility.Visible)
                    {
                        TabCtrl.SelectedItem = tabItem;
                        return;
                    }
                }
            }
            TabCtrl.SelectedIndex = -1;
        }

        private void MakeLogin()
        {
            try
            {
                var login = new Login();
                if (!login.ShowDialog().IsTrue())
                {
                    Close();
                }

                Main.Usuario = login.ViewModel.Usuario;
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        private void SetActions()
        {
            Main.MySqlProtocol.Connected += () => Dispatcher.Invoke(() => ViewModel.Connected());
            Main.MySqlProtocol.Disconnected += () => Dispatcher.Invoke(() => ViewModel.Disconnected());
            Main.MySqlProtocol.Reconnecting += () => Dispatcher.Invoke(() => ViewModel.Reconnecting());
            Main.Instance.Status = Main.MySqlProtocol.IsConnected ? "Conectado" : "Desconectado";
        }

        private void OpenConfig(object sender, RoutedEventArgs e)
        {
            new ViewConfiguracoes(Main.Empresa).ShowDialog();
            e.Handled = true;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            Hide();
            MakeLogin();
            Initialize();
            Show();
        }
    }
}