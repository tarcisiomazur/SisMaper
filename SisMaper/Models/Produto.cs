using System.Data;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "Produto", VersionControl = true)]
    public class Produto : DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 256)]
        public string CodigoBarras { get; set; }

        [Field(FieldType = SqlDbType.VarChar, Length = 256)]
        public string Descricao { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal PrecoCusto { get; set; }

        [Field(FieldType = SqlDbType.Decimal, Length = 10, Precision = 2)]
        public decimal PrecoVenda { get; set; }

        [Field(FieldType = SqlDbType.Bit, Length = 1)]
        public bool Fracionado { get; set; }

        [Field(FieldType = SqlDbType.Bit, Length = 5)]
        public byte[] Status { get; set; }

        [ManyToOne(Cascade = Cascade.SAVE)]
        public Categoria Categoria { get; set; }

        [ManyToOne] public NCM NCM { get; set; }

        [ManyToOne(Cascade = Cascade.SAVE)]
        public Unidade Unidade { get; set; }

        [OneToMany] public PList<Lote> Lotes { get; set; }


    }

}