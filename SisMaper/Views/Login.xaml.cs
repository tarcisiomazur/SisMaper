using System;
using System.Windows;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views;

/// <summary>
///     Interaction logic for Login.xaml
/// </summary>
public partial class Login
{
    protected override void OnClosed(EventArgs eventArgs)
    {
        if (Application.Current.MainWindow == null)
        {
            Environment.Exit(0);
        }
        base.OnClosed(eventArgs);
    }
    
    public Login()
    {
        DataContextChanged += SetActions;
        InitializeComponent();
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is LoginViewModel newViewModel)
        {
            newViewModel.Login += OpenMainWindow;
            newViewModel.Cancel += Close;
            newViewModel.ShowMessage += Helper.SystemDefaultMessage;
            newViewModel.SetFocus();
        }

        if (e.OldValue is LoginViewModel oldViewModel)
        {
            oldViewModel.Login -= OpenMainWindow;
            oldViewModel.Cancel -= Close;
            oldViewModel.ShowMessage -= Helper.SystemDefaultMessage;
        }
    }

    private void OpenMainWindow()
    {
        (Application.Current.MainWindow = new MainWindow()).Show();
        Close();
    }
}