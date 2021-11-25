using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SisMaper.API.WebMania.Models
{
    public class NF_NotaFiscal
    {
        public enum EnumOperacao
        {
            Saida,
            Entrada
        };

        public enum EnumModelo
        {
            NFe,
            NFCe
        };

        public enum EnumFinalidade
        {
            Normal = 1,
            Devolução = 4
        };

        public enum EnumAmbiente
        {
            Producao,
            Homologacao
        };


        [JsonPropertyName("ID")] public string ID { get; set; } = "";

        [JsonPropertyName("operacao")] public EnumOperacao Operacao { get; set; } = EnumOperacao.Saida;

        [JsonPropertyName("natureza_operacao")] public string Natureza { get; set; } = "";

        [JsonPropertyName("modelo")] public EnumModelo Modelo { get; set; } = EnumModelo.NFCe;

        [JsonPropertyName("finalidade")] public EnumFinalidade Finalidade { get; set; } = EnumFinalidade.Normal;

        [JsonPropertyName("ambiente")] public EnumAmbiente Ambiente { get; set; } = EnumAmbiente.Homologacao;

        [JsonPropertyName("cliente")] public NF_Cliente Cliente { get; set; }

        [JsonPropertyName("produtos")] public List<NF_Produtos> Produtos { get; set; }

        [JsonPropertyName("pedido")] public NF_Pedido Pedido { get; set; }

        public NF_NotaFiscal()
        {
        }
        
    }
}