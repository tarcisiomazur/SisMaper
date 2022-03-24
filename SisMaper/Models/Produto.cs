using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Produto")]
public class Produto : DAO
{
    [Field(FieldType = SqlDbType.VarChar, Length = 13)]
    public string CodigoInterno { get; set; }
    
    [Field(FieldType = SqlDbType.VarChar, Length = 13)]
    public string CodigoBarras { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 256)]
    public string Descricao { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
    public decimal PrecoCusto { get; set; }

    [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
    public decimal PrecoVenda { get; set; }

    [Field(FieldType = SqlDbType.Bit, Length = 1)]
    public bool Fracionado { get; set; }

    [Field(FieldType = SqlDbType.Bit, Length = 1)]
    public bool Inativo { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public Categoria? Categoria { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public NCM? NCM { get; set; }

    [ManyToOne(Fetch = Fetch.Eager)] public Unidade? Unidade { get; set; }

    [OneToMany] public PList<Lote>? Lotes { get; set; }

    public double Porcentagem
    {
        get => ((double) PrecoVenda / (double) PrecoCusto - 1) * 100;
        set => PrecoVenda = PrecoCusto * (decimal) (1 + value / 100);
    }
}