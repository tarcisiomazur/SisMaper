using System;
using System.Windows.Controls;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewVendas : UserControl
    {
        private VendasViewModel ViewModel => (VendasViewModel) DataContext;

        public ViewVendas()
        {
            InitializeComponent();
            ViewModel.OpenPedido += OpenPedido;
        }

        private void OpenPedido(long? pedidoId)
        {
            var viewPedido = new ViewPedido(pedidoId);
            viewPedido.Closed += ViewModel.UpdatePedidos;
            viewPedido.ShowDialog();
        }


    }
}