using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Categoria")]
    public class Categoria : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Descricao { get; set; }

        [OneToMany(Fetch = Fetch.Lazy, ItemsByAccess = 10)]
        public PList<Produto> Produtos { get; set; }

        public Categoria()
        {
            Descricao = "";
            Produtos = new PList<Produto>();
        }

        public override string ToString()
        {
            return Descricao;
        }

    }
}