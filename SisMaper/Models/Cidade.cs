using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Cidade")]
    public class Cidade : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 256)]
        public string Nome { get; set; }

        [ManyToOne(Nullable = Nullable.NotNull)]
        public Estado Estado { get; set; }

        public override string ToString()
        {
            return Nome;
        }

    }
}