using System;
using System.Data;
using Persistence;

namespace SisMaper.Models;

[Table(Name = "NotaFiscal")]
public class NotaFiscal : DAO
{
    public enum EnumSituacao
    {
        Null,
        Aprovado,
        Reprovado,
        Cancelado,
        Denegado,
        Processamento,
        Contingencia
    }

    [UniqueIndex(FieldType = SqlDbType.VarChar, Length = 36)]
    public string UUID { get; set; }

    [UniqueIndex(FieldType = SqlDbType.VarChar, Length = 44)]
    public string Chave { get; set; }

    [Field(FieldType = SqlDbType.DateTime)]
    public DateTime DataEmissao { get; set; }

    [Field(FieldType = SqlDbType.Xml)] public byte[] XML { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 256)]
    public string URL_XML { get; set; }

    [Field(FieldType = SqlDbType.VarChar, Length = 256)]
    public string URL_DANFE { get; set; }

    [Field(FieldType = SqlDbType.Int)] public int Serie { get; set; }

    [Field(FieldType = SqlDbType.Int)] public int Numero { get; set; }

    [Field(FieldType = SqlDbType.Bit, Length = 3)]
    public EnumSituacao Situacao { get; set; }

    [ManyToOne] public Pedido Pedido { get; set; }
}