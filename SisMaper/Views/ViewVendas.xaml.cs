using System.Windows;
using SisMaper.Tools;
using SisMaper.ViewModel;
using SisMaper.Views.Templates;

namespace SisMaper.Views
{
    public partial class ViewVendas : MyUserControl
    {
        public ViewVendas()
        {
            InitializeComponent();
            DataContextChanged += SetActions;
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.IsChanged(out VendasViewModel viewModel))
            {
                Show += viewModel.Initialize;
                Hide += viewModel.Clear;
                viewModel.OpenPedido += OpenPedido;
                viewModel.ShowMessage += Helper.MahAppsDefaultMessage;
                viewModel.ShowProgress += Helper.MahAppsDefaultProgress;
            }
        }

        private void OpenPedido(PedidoViewModel viewModel)
        {
            var viewPedido = new ViewPedido {DataContext = viewModel};
            viewPedido.ShowDialog();
            OnShow();
        }
    }
}