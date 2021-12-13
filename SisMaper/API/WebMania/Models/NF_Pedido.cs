using Newtonsoft.Json;

namespace SisMaper.API.WebMania.Models;

public class NF_Pedido
{
    public enum EnumFrete
    {
        CIF,
        FOB,
        Terceiros,
        Proprio,
        ProprioRemetente,
        ProprioDestinatario,
        Sem = 9
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

    [JsonProperty("presenca")] public EnumPresenca Presenca { get; set; } = EnumPresenca.Presencial;

    [JsonProperty("modalidade_frete")] public EnumFrete Modalidade_frete { get; set; } = EnumFrete.ProprioDestinatario;

    [JsonProperty("frete")] public string frete { get; set; }

    [JsonProperty("desconto")] public string desconto { get; set; }
}