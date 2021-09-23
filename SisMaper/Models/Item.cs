using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Item", VersionControl = true)]
    public class Item : DAO
    {
        [ManyToOne(Nullable = Nullable.NotNull, Fetch = Fetch.Eager)]
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
        public decimal Desconto { get; set; }

        [ManyToOne] public Lote Lote { get; set; }

        public decimal Total
        {
            get => (decimal) Quantidade * Valor - Desconto;
            set => value = value;
        }

        public double DescontoPorcentagem
        {
            get => 100.0 * (double) Desconto * Quantidade / (double) Valor;
            set
            {
                Desconto = new decimal(value * (double) Valor * Quantidade / 100.0);
                Total++;
            }
        }
    }
}