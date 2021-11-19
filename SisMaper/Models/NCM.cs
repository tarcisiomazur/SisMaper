using System;
using System.Data;
using System.Globalization;
using Persistence;

namespace SisMaper.Models
{
    [Table(Name = "NCM")]
    public class NCM: DAO
    {
        [Field(FieldType = SqlDbType.VarChar, Length = 45)]
        public string Descricao { get; set; }
        
        public override string ToString()
        {
            return Id.ToString(@"####\.##\.##") + " - " + Descricao;
        }
        
    }
}