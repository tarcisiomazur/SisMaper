using System.Windows;
using SisMaper.ViewModel;

namespace SisMaper.Views;

public partial class ViewSelecionarFatura
{
    public ViewSelecionarFatura()
    {
        DataContextChanged += SetActions;
        InitializeComponent();
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is SelecionarFaturaViewModel newViewModel)
        {
            newViewModel.Cancel += Close;
            newViewModel.Select += Close;
        }

        if (e.OldValue is SelecionarFaturaViewModel oldViewModel)
        {
            oldViewModel.Cancel -= Close;
            oldViewModel.Select -= Close;
        }
    }
}