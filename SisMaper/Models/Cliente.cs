using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Cliente")]
    public class Cliente : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 60)]
        public string Nome { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 60)]
        public string Endereco { get; set; }
        [Field(FieldType = SqlDbType.VarChar, Length = 10)]
        public string Numero { get; set; }
        [Field(FieldType = SqlDbType.VarChar, Length = 60)]
        public string Bairro { get; set; }
        [Field(FieldType = SqlDbType.VarChar, Length = 60)]
        public string CEP { get; set; }

        [ManyToOne(Fetch = Fetch.Eager)]
        public Cidade? Cidade { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal LimiteCredito { get; set; }

    }
}