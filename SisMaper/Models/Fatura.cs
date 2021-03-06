using System;
using System.Data;
using Persistence;
using Nullable = Persistence.Nullable;

namespace SisMaper.Models;

[Table(Name = "Fatura")]
public class Fatura : DAO
{
    public enum Fatura_Status
    {
        Aberta,
        Fechada
    }

    public Fatura()
    {
        Status = Fatura_Status.Aberta;
        Data = DateTime.Today;
    }
    
    [Field(FieldType = SqlDbType.BigInt, Nullable = Nullable.Null)]
    public long? Numero { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
    public decimal ValorTotal { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
    public decimal ValorPago { get; set; }

    [OneToMany(orphanRemoval = true, Cascade = Cascade.ALL, Fetch = Fetch.Eager)]
    public PList<Parcela> Parcelas { get; set; }

    [OneToMany(Cascade = Cascade.SAVE)] public PList<Pedido> Pedidos { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public Cliente Cliente { get; set; }

    [Field(FieldType = SqlDbType.Bit, Length = 1)]
    public Fatura_Status Status { get; set; }

    [Field(FieldType = SqlDbType.DateTime)]
    public DateTime Data { get; set; }
}