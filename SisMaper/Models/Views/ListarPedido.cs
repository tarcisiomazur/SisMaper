using System;
using Persistence;

namespace SisMaper.Models.Views
{
    [View(ViewName = "ListarPedidos")]
    public class ListarPedido
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }
        public Pedido.Pedido_Status Status { get; set; }
        [Field(FieldName = "Nome")]
        public string Cliente { get; set; }
        public Fatura.Fatura_Status Fatura { get; set; }
        public decimal ValorTotal { get; set; }
        
    }
}