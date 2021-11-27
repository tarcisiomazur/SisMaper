using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Configuracoes")]
    public class Configuracoes : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 14)]
        public string CNPJ { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 128)]
        public string Razao_Social { get; set; }

        [Field(FieldType = SqlDbType.Xml)]
        public byte[] CertificadoA1 { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 512)]
        public string Senha_Certificado { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 128)]
        public string Endereco { get; set; }

        [ManyToOne] public Cidade Cidade { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 344)]
        public string ACCESS_TOKEN_SECRET { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 344)]
        public string ACCESS_TOKEN { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 344)]
        public string CONSUMER_SECRET { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 344)]
        public string CONSUMER_KEY { get; set; }
    }
}