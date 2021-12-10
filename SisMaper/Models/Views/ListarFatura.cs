using System;
using Persistence;

namespace SisMaper.Models.Views
{
    [View(ViewName = "ListarFaturas")]
    public class ListarFatura
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public Fatura.Fatura_Status Status { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPago { get; set; }

    }
}