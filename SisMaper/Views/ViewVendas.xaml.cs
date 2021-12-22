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
            if (e.NewValue is VendasViewModel newViewModel)
            {
                Show += newViewModel.Initialize;
                Hide += newViewModel.Clear;
                newViewModel.OpenPedido += OpenPedido;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
                newViewModel.ShowProgress += Helper.MahAppsDefaultProgress;
            }
            if (e.OldValue is VendasViewModel oldViewModel)
            {
                Show -= oldViewModel.Initialize;
                Hide -= oldViewModel.Clear;
                oldViewModel.OpenPedido -= OpenPedido;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
                oldViewModel.ShowProgress -= Helper.MahAppsDefaultProgress;
            }
        }

        private void OpenPedido(PedidoViewModel viewModel)
        {
            new ViewPedido {DataContext = viewModel, Owner = Window}.ShowDialog();
            OnShow();
        }
    }
}