using System.Data;
using Persistence;

namespace SisMaper.Models
{

    [Table(Name = "Parcela", VersionControl = true)]
    public class Parcela: DAO
    {
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Fatura Fatura { get; set; }
        
        [Field(FieldType = SqlDbType.Int)]
        public int Indice { get; set; }

        [Field(FieldType = SqlDbType.Date)]
        public decimal Prazo { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal Valor { get; set; }
        
        [OneToMany(orphanRemoval = true, Cascade = Cascade.ALL)]
        public PList<Pagamento> Pagamentos { get; set; }


    }
}