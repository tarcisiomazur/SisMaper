using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Views
{
    public partial class CrudProduto : MetroWindow
    {
        public static Produto Produto { get; set; }
        
        public CrudProduto()
        {
            InitializeComponent();
            Produto = DAO.Load<Produto>(6);
            grid.DataContext = Produto;
            Title = "Editar Produto - " + Produto.Descricao;
            
        }
    }
}