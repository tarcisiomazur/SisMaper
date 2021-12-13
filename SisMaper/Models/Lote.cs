using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Lote")]
public class Lote : DAO
{
    [Field(FieldType = SqlDbType.VarChar, Length = 45)]
    public string Descricao { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 256)]
    public string Informacoes { get; set; }

    [ManyToOne(Nullable = Nullable.NotNull)]
    public Produto Produto { get; set; }
}