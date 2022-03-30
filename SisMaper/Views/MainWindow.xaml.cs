using System;
using System.Windows;
using System.Windows.Controls;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    protected override void OnClosed(EventArgs eventArgs)
    {
        if (Application.Current.MainWindow == null)
        {
            Environment.Exit(0);
        }
        base.OnClosed(eventArgs);
    }
    
    public MainWindow()
    {
        Instance = this;
        DataContextChanged += SetActions;
        InitializeComponent();
        Initialize();
    }

    public static MainWindow? Instance { get; set; }

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
        window.ShowDialog();
        e.Handled = true;
    }

    private void OpenUser(object sender, RoutedEventArgs e)
    {
        var vm = new UsuariosViewModel();
        new CrudUsuario { Owner = this, DataContext = new UsuariosViewModel.CrudUsuarioViewModel(false, Main.Usuario.Id, vm.Usuarios) }.ShowDialog();
        e.Handled = true;
    }

    private void Logout(object sender, RoutedEventArgs e)
    {
        Instance = null;
        (Application.Current.MainWindow = new Login()).Show();
        Close();
    }


}