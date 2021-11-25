using System.Windows.Input;
using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewBuscarProduto : MetroWindow
    {
        public BuscarProdutoViewModel ViewModel => (BuscarProdutoViewModel) DataContext;
        
        public ViewBuscarProduto(PList<Produto> produtos)
        {
            InitializeComponent();
            ViewModel.Initialize(produtos);
        }

        private void Selecionar(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void Cancelar(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}