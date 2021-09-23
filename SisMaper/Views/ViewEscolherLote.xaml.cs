using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Views
{
    public partial class ViewEscolherLote : MetroWindow
    {
        public ViewEscolherLote()
        {
            InitializeComponent();
            LOTES.DataContext = DAO.FindWhereQuery<Lote>("Produto_Id = 14");
        }
    }
}