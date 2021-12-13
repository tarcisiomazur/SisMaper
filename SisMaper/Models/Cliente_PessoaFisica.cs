using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "Pessoa_Fisica")]
public class PessoaFisica : Cliente
{
    [UniqueIndex(FieldType = SqlDbType.VarChar, Length = 11)]
    public string CPF { get; set; }

    public string MaskedCPF => long.TryParse(CPF, out var c) ? c.ToString(@"000\.000\.000\-00") : "";
}