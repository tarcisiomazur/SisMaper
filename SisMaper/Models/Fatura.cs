using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Fatura")]
    public class Fatura : DAO
    {
        public enum Fatura_Status
        {
            Aberta,
            Fechada
        }
        
        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorTotal { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorPago { get; set; }
        
        [OneToMany(orphanRemoval = true, Cascade = Cascade.ALL)]
        public PList<Parcela> Parcelas { get; set; }
        
        [OneToMany(Cascade = Cascade.SAVE)]
        public PList<Pedido> Pedidos { get; set; }

        [ManyToOne] public Cliente Cliente { get; set; }
        
        [Field(FieldType = SqlDbType.Bit, Length = 1)]
        public Fatura_Status Status { get; set; }
        
        [Field(FieldType = SqlDbType.DateTime)]
        public DateTime Data { get; set; }
        
    }
}