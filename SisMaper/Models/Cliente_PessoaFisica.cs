using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Pessoa_Fisica")]
    public class PessoaFisica : Cliente
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 11)]
        public string CPF { get; set; }

    }
}