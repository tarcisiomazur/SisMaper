using System;
using System.Data;
using Persistence;
using PropertyChanged;
using Nullable = Persistence.Nullable;

namespace SisMaper.Models
{
    [Table(Name = "Item", VersionControl = true)]
    public class Item : DAO
    {
        [ManyToOne(Nullable = Nullable.NotNull, Fetch = Fetch.Eager)]
        public Produto? Produto { get; set; }
        
        [AlsoNotifyFor(nameof(Total))]
        [ManyToOne(Nullable = Nullable.NotNull)]
        public Pedido? Pedido { get; set; }

        [Field(FieldType = SqlDbType.Real, Length = 20, Precision = 10)]
        public double Quantidade { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal Valor { get; set; }

        [Field(FieldType = SqlDbType.Int)]
        public int CFOP { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal Desconto
        {
            get => new (Math.Round(DescontoPorcentagem * (double) Valor * Quantidade /100.0, 2));
            set => DescontoPorcentagem = 100.0 * (double) value / ((double) Valor * Quantidade);
        }

        [ManyToOne(Fetch = Fetch.Eager)]
        public Lote? Lote { get; set; }

        public decimal Total => decimal.Round((decimal) Quantidade * Valor - Desconto, 2);
        
        public double DescontoPorcentagem { get; set; }
    }
}