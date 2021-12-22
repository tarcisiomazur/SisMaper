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
        public static MainWindow? Instance { get; set; }

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
            if (e.NewValue is MainViewModel newViewModel)
            {
                newViewModel.ShowProgress += Helper.MahAppsDefaultProgress;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is MainViewModel oldViewModel)
            {
                oldViewModel.ShowProgress -= Helper.MahAppsDefaultProgress;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }

        private void OpenConfig(object sender, RoutedEventArgs e)
        {
            var vm = new ConfiguracoesViewModel();
            var window = new ViewConfiguracoes {Owner = this, DataContext = vm};
            window.Loaded += (_, _) => vm.Initialize(Main.Empresa);
            window.ShowDialog();
            e.Handled = true;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow = new Login()).Show();
            Close();
        }
    }
}