using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Natureza")]
public class Natureza : DAO
{
    [Field(FieldType = SqlDbType.VarChar, Length = 45)]
    public string Descricao { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 45)]
    public string Classe_de_Imposto { get; set; }
}