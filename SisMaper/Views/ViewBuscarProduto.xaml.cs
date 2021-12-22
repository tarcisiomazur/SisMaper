using System.Windows;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewBuscarProduto
    {
        
        public ViewBuscarProduto()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is BuscarProdutoViewModel newViewModel)
            {
                newViewModel.Cancel += Close;
                newViewModel.Select += Close; 
            }
            if (e.OldValue is BuscarProdutoViewModel oldViewModel)
            {
                oldViewModel.Cancel -= Close;
                oldViewModel.Select -= Close;
            }
        }
    }
}