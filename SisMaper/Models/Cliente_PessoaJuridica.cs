using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Pessoa_Juridica")]
public class PessoaJuridica : Cliente
{
    [UniqueIndex(FieldType = SqlDbType.VarChar, Length = 14)]
    public string CNPJ { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 256)]
    public string RazaoSocial { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 10)]
    public string InscricaoEstadual { get; set; }
}