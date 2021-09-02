using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Unidade")]
    public class Unidade: DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Descricao { get; set; }

    }
}