using Persistence;

namespace SisMaper.Models
{
    [View(ViewName = "ListarProdutos")]
    public class ViewListarProdutos
    {
        public long Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Descricao { get; set; }
        public string Unidade { get; set; }
        public string NCM { get; set; }

        public string NCM_String
        {
            get
            {
                if (NCM is not null)
                {
                    return NCM.Split(' ')[0];
                }
                return "-";
            }
            private set { }
        }

        public bool Inativo { get; set; }
        public string Categoria { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
    }
}