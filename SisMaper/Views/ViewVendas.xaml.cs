using SisMaper.ViewModel;
using SisMaper.Views.Templates;

namespace SisMaper.Views
{
    public partial class ViewVendas : MyUserControl
    {
        private VendasViewModel ViewModel => (VendasViewModel) DataContext;

        public ViewVendas()
        {
            InitializeComponent();
            Open += ViewModel.Initialize;
            Close += ViewModel.Clear;
            ViewModel.OpenPedido += OpenPedido;
        }

        private void OpenPedido(long? pedidoId)
        {
            var viewPedido = new ViewPedido(pedidoId);
            viewPedido.Closed += ViewModel.Initialize;
            viewPedido.ShowDialog();
        }


    }
}