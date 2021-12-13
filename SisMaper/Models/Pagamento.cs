using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Pagamento")]
public class Pagamento : DAO
{
    public enum EnumTipoPagamento
    {
        Moeda = 0x01,
        Credito = 0x02,
        Debito = 0x03,
        Outro = 0x04
    }

    [ManyToOne(Nullable = Nullable.NotNull)]
    public Usuario Usuario { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
    public decimal ValorPagamento { get; set; }

    [ManyToOne(Nullable = Nullable.NotNull)]
    public Parcela Parcela { get; set; }

    [Field(FieldType = SqlDbType.Bit)] public EnumTipoPagamento TipoPagamento { get; set; }
}