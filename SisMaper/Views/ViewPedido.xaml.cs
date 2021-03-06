using System;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views;

/// <summary>
///     Lógica interna para ViewPedido.xaml
/// </summary>
public partial class ViewPedido
{
    public ViewPedido()
    {
        InitializeComponent();
        DataContextChanged += SetActions;
    }

    private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is PedidoViewModel newViewModel)
        {
            newViewModel.Save += Close;
            newViewModel.Cancel += Close;
            newViewModel.OpenFatura += OpenFatura;
            newViewModel.OpenBuscarProduto += OpenBuscarProduto;
            newViewModel.OpenEditarItem += OpenEditarItem;
            newViewModel.OpenCrudCliente += OpenCrudCliente;
            newViewModel.OpenEscolherLote += OpenEscolherLote;
            newViewModel.OpenMetodoPagamento += OpenMetodoPagamento;
            newViewModel.OpenSelecionarFatura += OpenSelecionarFatura;
            newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            newViewModel.ShowProgress += Helper.MahAppsDefaultProgress;
        }

        if (e.OldValue is PedidoViewModel oldViewModel)
        {
            oldViewModel.Save -= Close;
            oldViewModel.Cancel -= Close;
            oldViewModel.OpenFatura -= OpenFatura;
            oldViewModel.OpenBuscarProduto -= OpenBuscarProduto;
            oldViewModel.OpenEditarItem -= OpenEditarItem;
            oldViewModel.OpenCrudCliente -= OpenCrudCliente;
            oldViewModel.OpenEscolherLote -= OpenEscolherLote;
            oldViewModel.OpenMetodoPagamento -= OpenMetodoPagamento;
            oldViewModel.OpenSelecionarFatura -= OpenSelecionarFatura;
            oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            oldViewModel.ShowProgress -= Helper.MahAppsDefaultProgress;
        }
    }

    private void OpenCrudCliente(CrudClienteViewModel viewModel, bool isPessoaFisica)
    {
        new CrudCliente
            {DataContext = viewModel, Owner = this, IsSelectedPessoaFisicaTab = isPessoaFisica}.ShowDialog();
    }

    private void OpenEditarItem(EditarItemViewModel viewModel)
    {
        new ViewEditarItem {DataContext = viewModel, Owner = this}.ShowDialog();
    }

    private void OpenBuscarProduto(BuscarProdutoViewModel viewModel)
    {
        new ViewBuscarProduto {DataContext = viewModel, Owner = this}.ShowDialog();
    }

    private void OpenEscolherLote(EscolherLoteViewModel viewModel)
    {
        new ViewEscolherLote {DataContext = viewModel, Owner = this}.ShowDialog();
    }

    private void OpenFatura(FaturaViewModel viewModel)
    {
        Hide();
        new ViewFatura {DataContext = viewModel, Owner = this}.ShowDialog();
        Show();
    }
    
    private void OpenMetodoPagamento(Action<ViewMetodoPagamento.OptionPagamento> callback)
    {
        var metodopagamento = new ViewMetodoPagamento {Owner = this};
        metodopagamento.Show();
        metodopagamento.Closed += (_, _) =>
        {
            
            Activate();
            Topmost = true;  // important
            Topmost = false; // important
            Focus();         // important
            callback.Invoke(metodopagamento.Selecionado);
        };
    }
    
    private void OpenSelecionarFatura(SelecionarFaturaViewModel viewModel)
    {
        new ViewSelecionarFatura() {DataContext = viewModel, Owner = this}.ShowDialog();
    }

    private void SetZoom(object? sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
    {
        if (sender is ChromiumWebBrowser webBrowser)
        {
            webBrowser.SetZoomLevel(2);
        }
    }
}