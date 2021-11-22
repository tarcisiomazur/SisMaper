using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Usuario")]
    public class Usuario: DAO
    {
        [Flags]
        public enum Tipo_Permissao
        {
            Venda = 0x01,
            Recebimento = 0x02,
            Cadastros = 0x04,
            Gerenciamento = 0x08,
            Databaser = 0x10,
            All = Venda | Recebimento | Cadastros | Gerenciamento | Databaser,
        }

        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Login { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Nome { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 512)]
        public string Senha { get; set; } = "";

        [Field(FieldType = SqlDbType.Bit, Length = 5)]
        public Tipo_Permissao Permissao { get; set; }
    }

}