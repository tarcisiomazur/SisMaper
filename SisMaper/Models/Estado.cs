using System.Data;
using Persistence;

namespace SisMaper.Models
{

    [Table(Name = "Estado")]
    public class Estado: DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Nome { get; set; }
        
        [OneToMany]
        public PList<Cidade> Cidades { get; set; }
        
        [Field(FieldType = SqlDbType.VarChar, Length = 2)]
        public string UF { get; set; }

        public override string ToString()
        {
            return Nome;
        }
    }
}