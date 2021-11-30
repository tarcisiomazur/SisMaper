using Persistence;

namespace SisMaper.Models
{
    [View(ViewName = "ListarProdutos")]
    public class ViewListarProdutos
    {
        public long Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Descricao { get; set; }
        public long NCM { get; set; }
        public bool Inativo { get; set; }
        public string Categoria { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}