using System;
using System.Data;
using Persistence;
using Nullable = Persistence.Nullable;

namespace SisMaper.Models;

[Table(Name = "Pedido")]
public class Pedido : DAO
{
    public enum Pedido_Status
    {
        Aberto = 0x00,
        Fechado = 0x01,
        Cancelado = 0x02
    }

    [Field(FieldType = SqlDbType.DateTime)]
    public DateTime? Data { get; set; } = DateTime.Now;

    [Field(FieldType = SqlDbType.Bit, Length = 3)]
    public Pedido_Status Status { get; set; } = Pedido_Status.Aberto;

    [ManyToOne(Fetch = Fetch.Eager)] public Cliente? Cliente { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public Fatura? Fatura { get; set; }

    [ManyToOne(Nullable = Nullable.NotNull)]
    public Usuario Usuario { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public Natureza? Natureza { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2, ReadOnly = true)]
    public decimal ValorTotal { get; set; }

    [OneToMany(orphanRemoval = true, Fetch = Fetch.Eager, Cascade = Cascade.ALL)]
    public PList<Item> Itens { get; set; }

    [OneToMany(Fetch = Fetch.Eager)] public PList<NotaFiscal> NotasFiscais { get; set; }
}