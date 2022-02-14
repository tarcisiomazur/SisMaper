using System.Windows;
using SisMaper.ViewModel;

namespace SisMaper.Views;

public partial class ViewEditarItem
{
    public ViewEditarItem()
    {
        DataContextChanged += SetActions;
        InitializeComponent();
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is EditarItemViewModel newViewModel)
        {
            newViewModel.Save += Close;
            newViewModel.Cancel += Close;
            newViewModel.OpenBuscarProduto += OpenBuscarProduto;
            newViewModel.OpenEscolherLote += OpenEscolherLote;
        }

        if (e.OldValue is EditarItemViewModel oldViewModel)
        {
            oldViewModel.Save -= Close;
            oldViewModel.Cancel -= Close;
            oldViewModel.OpenBuscarProduto -= OpenBuscarProduto;
            oldViewModel.OpenEscolherLote -= OpenEscolherLote;
        }
    }

    private void OpenBuscarProduto(BuscarProdutoViewModel viewModel)
    {
        new ViewBuscarProduto {DataContext = viewModel, Owner = this}.ShowDialog();
    }

    private void OpenEscolherLote(EscolherLoteViewModel viewModel)
    {
        new ViewEscolherLote {DataContext = viewModel, Owner = this}.ShowDialog();
    }
}