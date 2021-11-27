using System;
using System.Data;
using Persistence;
using Nullable = Persistence.Nullable;

namespace SisMaper.Models
{

    [Table(Name = "Parcela", VersionControl = true)]
    public class Parcela: DAO
    {
        [Flags]
        public enum Status_Parcela
        {
            Pendente = 0x00,
            Pago = 0x01
        }
        
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Fatura Fatura { get; set; }
        
        [Field(FieldType = SqlDbType.Bit, Length = 1)]
        public Status_Parcela Status { get; set; }

        [Field(FieldType = SqlDbType.Int)]
        public int Indice { get; set; }

        [Field(FieldType = SqlDbType.Date)]
        public DateTime DataVencimento { get; set; }

        [Field(FieldType = SqlDbType.DateTime)]
        public DateTime? DataPagamento { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal Valor { get; set; }
        
        [OneToMany(orphanRemoval = true, Cascade = Cascade.ALL)]
        public PList<Pagamento> Pagamentos { get; set; }
    }
}