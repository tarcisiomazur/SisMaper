using System.Windows;
using MahApps.Metro.Controls;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewBuscarProduto : MetroWindow
    {
        
        public ViewBuscarProduto()
        {
            InitializeComponent();
            DataContextChanged += SetActions;
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.IsChanged(out BuscarProdutoViewModel viewModel))
            {
                viewModel.Cancel += Close;
                viewModel.Select += Close;
            }
        }
    }
}