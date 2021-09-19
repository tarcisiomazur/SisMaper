using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{

    [Table(Name = "Pedido", VersionControl = true)]
    public class Pedido : DAO
    {
        public enum Pedido_Status
        {
            Aberto = 0x00,
            Fechado = 0x01,
            Cancelado = 0x02
        }
        
        [Field(FieldType = SqlDbType.DateTime)]
        public DateTime Data { get; set; }

        [Field(FieldType = SqlDbType.Bit, Length = 3)]
        public Pedido_Status Status { get; set; }

        [ManyToOne(Fetch = Fetch.Eager)]
        public Cliente Cliente { get; set; }

        [ManyToOne(Cascade = Cascade.SAVE)]
        public Fatura Fatura { get; set; }

        [ManyToOne(Nullable = Persistence.Nullable.NotNull)]
        public Usuario Usuario { get; set; }

        [ManyToOne] public Natureza Natureza { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorTotal { get; set; }
        
        [OneToMany(orphanRemoval = true, Fetch = Fetch.Eager, Cascade = Cascade.ALL)]
        public PList<Item> Itens { get; set; }

        [OneToMany(Fetch = Fetch.Eager)]
        public PList<NotaFiscal> NotasFiscais { get; set; }

    }
}