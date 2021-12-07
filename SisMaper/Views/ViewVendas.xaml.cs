using System.Windows;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewVendas
    {
        public ViewVendas()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
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
            if (e.OldValue is VendasViewModel oldVm)
            {
                Show -= oldVm.Initialize;
                Hide -= oldVm.Clear;
            }
        }

        private void OpenPedido(PedidoViewModel viewModel)
        {
            var viewPedido = new ViewPedido {DataContext = viewModel, Owner = Window.GetWindow(this)};
            viewPedido.ShowDialog();
            OnShow();
        }
    }
}