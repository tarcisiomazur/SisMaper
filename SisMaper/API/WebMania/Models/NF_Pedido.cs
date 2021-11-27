using System.Text.Json.Serialization;

namespace SisMaper.API.WebMania.Models
{
    public class NF_Pedido
    {
        public NF_Pedido()
        {

        }

        public enum EnumPresenca
        {
            NA,
            Presencial,
            Internet,
            TeleAtendimento,
            Entrega,
            PresencialFora,
            Outros
        }

        public enum EnumFrete
        {
            CIF,
            FOB,
            Terceiros,
            Proprio,
            ProprioRemetente,
            ProprioDestinatario,
            Sem = 9,
        }

        [JsonPropertyName("presenca")] public EnumPresenca Presenca { get; set; } = EnumPresenca.Presencial;

        [JsonPropertyName("modalidade_frete")]
        public EnumFrete Modalidade_frete { get; set; } = EnumFrete.ProprioDestinatario;

        [JsonPropertyName("frete")] public string frete { get; set; } = "";
        
        [JsonPropertyName("desconto")] public string desconto { get; set; } = "";
    }
}
