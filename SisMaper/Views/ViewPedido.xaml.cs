using System;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class ViewPedido : MetroWindow
    {
        private TextBox TbBuscarProduto;

        public PedidoViewModel ViewModel => (PedidoViewModel) DataContext;
        
        public ViewPedido(long? pedidoId)
        {
            InitializeComponent();
            WebBrowser.Navigated += OnNavigate;
            ViewModel.Initialize(pedidoId);
            SetActions();
        }

        private void OnNavigate(object sender, NavigationEventArgs e)
        {
            var content = e.Content;
        }

        private void SetActions()
        {
            ViewModel.Save += Close;
            ViewModel.Cancel += Close;
        }


        private void Context_Delete(object sender, RoutedEventArgs e)
        {
        }

        private void EmitirNF(object sender, RoutedEventArgs e)
        {
            var nf = new NotaFiscalEletronica(new NotaFiscal()
            {
                Pedido = ViewModel.Pedido,
                Id = 99,
            });
            var result = nf.BuildJsonDefault();
            Console.WriteLine(nf.Json);
            MessageBox.Show(nf.Json, result);
            
        }

        private void Imprimir(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
        }
        
    }
}
