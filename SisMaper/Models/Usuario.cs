using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Usuario")]
    public class Usuario: DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Login { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Nome { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 512)]
        public string Senha { get; set; }

        [Field(FieldType = SqlDbType.Binary, Length = 5)]
        public byte[] Permissao { get; set; }
    }

}