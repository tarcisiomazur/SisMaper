using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Cliente", VersionControl = true)]
    public class Cliente : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 128)]
        public string Nome { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 128)]
        public string Endereco { get; set; }

        [ManyToOne(Fetch = Fetch.Eager)]
        public Cidade? Cidade { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal LimiteCredito { get; set; }

    }
}