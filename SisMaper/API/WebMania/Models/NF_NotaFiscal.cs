using System.Collections.Generic;
using Newtonsoft.Json;

namespace SisMaper.API.WebMania.Models
{
    public class NF_NotaFiscal
    {
        public enum EnumOperacao
        {
            Saida = 1,
            Entrada = 0
        };

        public enum EnumModelo
        {
            NFe = 1,
            NFCe = 2
        };

        public enum EnumFinalidade
        {
            Normal = 1,
            Devolução = 4
        };

        public enum EnumAmbiente
        {
            Producao = 1,
            Homologacao = 2
        };


        [JsonProperty("ID")] public string ID { get; set; } 

        [JsonProperty("operacao")] public EnumOperacao Operacao { get; set; } = EnumOperacao.Saida;

        [JsonProperty("natureza_operacao")] public string Natureza { get; set; } 

        [JsonProperty("modelo")] public EnumModelo Modelo { get; set; } = EnumModelo.NFCe;

        [JsonProperty("finalidade")] public EnumFinalidade Finalidade { get; set; } = EnumFinalidade.Normal;

        [JsonProperty("ambiente")] public EnumAmbiente Ambiente { get; set; } = EnumAmbiente.Homologacao;

        [JsonProperty("cliente", NullValueHandling = NullValueHandling.Include)] public object Cliente { get; set; }

        [JsonProperty("produtos")] public List<NF_Produtos> Produtos { get; set; }

        [JsonProperty("pedido")] public NF_Pedido Pedido { get; set; }

        public NF_NotaFiscal()
        {
        }
        
    }
}