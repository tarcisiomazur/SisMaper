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
        }
        if (e.OldValue is EditarItemViewModel oldViewModel) { 
            oldViewModel.Save -= Close;
            oldViewModel.Cancel -= Close;
            oldViewModel.OpenBuscarProduto -= OpenBuscarProduto;
        }
    }
    
    
    
    private void OpenBuscarProduto(BuscarProdutoViewModel viewModel)
    {
        new ViewBuscarProduto {DataContext = viewModel, Owner = this}.ShowDialog();
    }
}