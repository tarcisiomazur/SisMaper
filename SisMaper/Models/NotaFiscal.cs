using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "NotaFiscal")]
    public class NotaFiscal : DAO
    {
        public enum EnumSituacao
        {
            Aprovado,
            Reprovado,
            Cancelado,
            Denegado,
            Processamento,
            Contingencia,
        }
        
        [PrimaryKey(FieldType = SqlDbType.VarChar, Length = 36)]
        public string UUID { get; set; }
        
        [UniqueIndex(FieldType = SqlDbType.VarChar, Length = 32)]
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
        public EnumSituacao Situacao { get; set; }

        [ManyToOne] public Pedido Pedido { get; set; }

    }
}