using System.Linq;
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
            Produtos.Columns.AddSelector("Código de Barras",nameof(Produto.CodigoBarras));
            Produtos.Columns.AddSelector("Descrição",nameof(Produto.Descricao));
            Produtos.Columns.AddSelector("NCM", "NCM.Id");
            Produtos.Columns.AddSelector("Categoria", "Categoria.Descricao");
            Produtos.Columns.AddSelector<Produto>("Preço de Custo", "PrecoCusto",p=>p.PrecoCusto.RealFormat());
            Produtos.Columns.AddSelector<Produto>("Preço de Venda", "PrecoVenda",p=>p.PrecoVenda.RealFormat());

            Produtos.DataContext = PList<Produto>.FindWhereQuery("Id>0");
            FiltrarCategorias.ItemsSource = PList<Categoria>.FindWhereQuery("Id>0").Select(c => c.Descricao);
        }

    }
}