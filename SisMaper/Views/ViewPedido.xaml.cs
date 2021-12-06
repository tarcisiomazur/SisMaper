using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para ViewPedido.xaml
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
            if (e.IsChanged(out PedidoViewModel viewModel))
            {
                viewModel.Save += Close;
                viewModel.Cancel += Close;
                viewModel.OpenFatura += OpenFatura;
                viewModel.OpenBuscarProduto += OpenBuscarProduto;
                viewModel.OpenCrudCliente += OpenCrudCliente;
                viewModel.ShowMessage += Helper.MahAppsDefaultMessage;
                viewModel.ShowProgress += Helper.MahAppsDefaultProgress;
            }
        }

        private void OpenCrudCliente(CrudClienteViewModel viewModel)
        {
            new CrudPessoaFisica {DataContext = viewModel}.ShowDialog();
        }

        private void OpenBuscarProduto(BuscarProdutoViewModel viewModel)
        {
            new ViewBuscarProduto {DataContext = viewModel}.ShowDialog();
        }

        private void OpenFatura(FaturaViewModel viewModel)
        {
            new ViewFatura {DataContext = viewModel}.ShowDialog();
        }

        private void SetZoom(object? sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            if (sender is ChromiumWebBrowser webBrowser)
                webBrowser.SetZoomLevel(2);
        }
    }
}