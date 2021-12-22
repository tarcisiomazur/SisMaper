using System.Windows;
using System.Windows.Input;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views;

public partial class ViewConfiguracoes
{
    public ViewConfiguracoes()
    {
        DataContextChanged += SetActions;
        InitializeComponent();
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is ConfiguracoesViewModel newViewModel)
        {
            newViewModel.Save += Close;
            newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
        }

        if (e.OldValue is ConfiguracoesViewModel oldViewModel)
        {
            oldViewModel.Save -= Close;
            oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
        }
    }

    private void Cancelar(object sender, MouseButtonEventArgs e)
    {
        Close();
    }

    private void AtualizarNCMs(object sender, RoutedEventArgs e)
    {
        BuildNCM.Run();
    }

    private void ImportPem(object sender, MouseButtonEventArgs e)
    {
        Encrypt.ImportKey();
    }

    private void AlterarNaturezas(object sender, MouseButtonEventArgs e)
    {
        new ViewNaturezas().ShowDialog();
    }
}