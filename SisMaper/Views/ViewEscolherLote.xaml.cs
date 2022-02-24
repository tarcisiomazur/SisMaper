using System.Windows;
using MahApps.Metro.Controls;
using SisMaper.ViewModel;

namespace SisMaper.Views;

public partial class ViewEscolherLote : MetroWindow
{
    public ViewEscolherLote()
    {
        InitializeComponent();
        DataContextChanged += SetActions;
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is EscolherLoteViewModel newViewModel)
        {
            newViewModel.OnOk += Close;
            newViewModel.OnCancel += Close;
        }

        if (e.OldValue is EscolherLoteViewModel oldViewModel)
        {
            oldViewModel.OnOk -= Close;
            oldViewModel.OnCancel -= Close;
        }
    }
}