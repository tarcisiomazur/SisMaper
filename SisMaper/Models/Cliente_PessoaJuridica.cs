using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Pessoa_Juridica")]
    public class PessoaJuridica : Cliente
    {
        [PrimaryKey(FieldType = SqlDbType.VarChar, Length = 14)]
        public string CNPJ { get; set; }
        
    }
}