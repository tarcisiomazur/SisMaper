using System;
using Persistence;

namespace SisMaper.Models
{
    [View(ViewName = "ListarPedidos")]
    public class ViewListarPedido
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