using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Item", VersionControl = true)]
    public class Item : DAO
    {
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Produto Produto { get; set; }

        [ManyToOne(Nullable = Nullable.NotNull)]
        public Pedido Pedido { get; set; }

        [Field(FieldType = SqlDbType.Real, Length = 20, Precision = 10)]
        public double Quantidade { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal Valor { get; set; }

        [Field(FieldType = SqlDbType.Int)]
        public int CFOP { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public double Desconto { get; set; }

        [ManyToOne] public Lote Lote { get; set; }
        
        public decimal Total => (decimal)Quantidade * Valor;
    }
}