using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Pagamento", VersionControl = true)]
    public class Pagamento: DAO
    {
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Usuario Usuario { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorPagamento { get; set; }
        
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Parcela Parcela { get; set; }

    }
}