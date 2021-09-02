using System.Data;
using Persistence;

namespace SisMaper.Models
{

    [Table(Name = "Estado")]
    public class Estado: DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Nome { get; set; }
        
        [OneToMany(Fetch = Fetch.Eager, Cascade = Cascade.SAVE)]
        public PList<Cidade> Cidades { get; set; }

    }
}