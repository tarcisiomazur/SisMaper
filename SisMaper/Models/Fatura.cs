using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Fatura")]
    public class Fatura : DAO
    {
        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorTotal { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorPago { get; set; }
        
        [OneToMany(orphanRemoval = true, Cascade = Cascade.ALL)]
        public PList<Parcela> Parcelas { get; set; }
        
    }
}