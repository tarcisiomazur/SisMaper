using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.Views
{
    public partial class ViewProdutos : UserControl
    {
        
        
        private string _filterText;

        public string FilterText
        {
            get => _filterText;
            set => _filterText = value;
        }

        public ViewProdutos()
        {
            InitializeComponent();
            Produtos.DataContext = PList<Produto>.FindWhereQuery("Id>0");
            FiltrarCategorias.ItemsSource = PList<Categoria>.FindWhereQuery("Id>0").Select(c => c.Descricao);
        }

        private void NovoProduto(object sender, RoutedEventArgs e)
        {
            new CrudProduto().Show();
        }
    }
}