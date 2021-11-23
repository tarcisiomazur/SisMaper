using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
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
            ViewModel.Initialize(pedidoId);
            SetActions();
        }

        private void SetActions()
        {
            ViewModel.OnSave += Close;
            ViewModel.OnCancel += Close;
        }


        private void Context_Delete(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
