using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "NotaFiscal")]
    public class NotaFiscal : DAO
    {
        [PrimaryKey(FieldType = SqlDbType.VarChar, Length = 32)]
        public string Chave { get; set; }

        [Field(FieldType = SqlDbType.DateTime)]
        public DateTime DataEmissao { get; set;  }

        [Field(FieldType = SqlDbType.Xml)]
        public byte[] XML { get; set; }

        [Field(FieldType = SqlDbType.Int)]
        public int Serie { get; set; }

        [Field(FieldType = SqlDbType.Int)]
        public int Numero { get; set; }

        [Field(FieldType =  SqlDbType.Bit, Length = 3)]
        public byte[] Situacao { get; set; }

        [ManyToOne] public Pedido Pedido { get; set; }

    }
}