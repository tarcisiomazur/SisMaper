using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{

    [Table(Name = "Pedido", VersionControl = true)]
    public class Pedido : DAO
    {
        [Field(FieldType = SqlDbType.DateTime)]
        public DateTime Data { get; set; }

        [Field(FieldType = SqlDbType.Binary, Length = 3)]
        public byte[] Status { get; set; }

        [ManyToOne(Cascade = Cascade.SAVE)]
        public Cliente Cliente { get; set; }

        [ManyToOne] public Fatura Fatura { get; set; }

        [ManyToOne] public Usuario Usuario { get; set; }

        [ManyToOne] public Natureza Natureza { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorTotal { get; set; }
        
        [OneToMany(orphanRemoval = true, Fetch = Fetch.Eager, Cascade = Cascade.ALL)]
        public PList<Item> Itens { get; set; }

        [OneToMany(Fetch = Fetch.Eager)]
        public PList<NotaFiscal> NotasFiscais { get; set; }

    }
}