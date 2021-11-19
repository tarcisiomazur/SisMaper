using System.Windows.Controls;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewVendas : UserControl
    {
        private VendasViewModel? ViewModel => (DataContext as VendasViewModel) ?? null;
        public ViewVendas()
        {
            InitializeComponent();
            ViewModel!.OpenPedido += OpenPedido;
        }

        private void OpenPedido(Pedido pedido)
        {
            new ViewPedido(pedido).ShowDialog();
        }
    }
}