using System;
using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Pagamento")]
    public class Pagamento : DAO
    {
        [Flags]
        public enum TipoPagamento
        {
            Moeda = 0x01,
            Credito = 0x02,
            Debito = 0x03,
            Outro = 0x04
        }

        [ManyToOne(Nullable = Persistence.Nullable.NotNull)]
        public Usuario Usuario { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal ValorPagamento { get; set; }
        
        [ManyToOne(Nullable = Persistence.Nullable.NotNull)]
        public Parcela Parcela { get; set; }

        [Field(FieldType = SqlDbType.Char, FieldName = "TipoPagamento")]
        public TipoPagamento Tipo { get; set; }


        /*
        public const string TIPO_MOEDA = "Moeda";
        public const string TIPO_CREDITO = "Cr�dito";
        public const string TIPO_DEBITO = "D�bito";
        public const string TIPO_OUTRO = "Outro";

        public string? TipoPagamentoString
        {
            get
            {
                switch(Tipo)
                {
                    case (char)TipoPagamento.Moeda:
                        return TIPO_MOEDA;

                    case (char)TipoPagamento.Credito:
                        return TIPO_CREDITO;

                    case (char)TipoPagamento.Debito:
                        return TIPO_DEBITO;

                    case (char)TipoPagamento.Outro:
                        return TIPO_OUTRO;

                    default:
                        return null;
                }
            }
            private set { }
        }
        */
    }
}