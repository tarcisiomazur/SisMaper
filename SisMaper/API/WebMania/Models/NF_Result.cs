using System.Text.Json;
using System.Text.Json.Serialization;

namespace SisMaper.API.WebMania.Models
{
    public class NF_Result
    {
        [JsonPropertyName("uuid")] public string uuid { get; set; }         
        [JsonPropertyName("status")] public string status { get; set; }         
        [JsonPropertyName("motivo")] public string motivo { get; set; }         
        [JsonPropertyName("nfe")] public string nfe { get; set; }
        [JsonPropertyName("serie")] public string serie { get; set; }
        [JsonPropertyName("recibo")] public string recibo { get; set; }         
        [JsonPropertyName("chave")] public string chave { get; set; }         
        [JsonPropertyName("modelo")] public string modelo { get; set; }         
        [JsonPropertyName("xml")] public string xml { get; set; }         
        [JsonPropertyName("danfe")] public string danfe { get; set; }         
        [JsonPropertyName("danfe_simples")] public string danfe_simples { get; set; }         
        [JsonPropertyName("danfe_etiqueta")] public string danfe_etiqueta { get; set; }
        [JsonPropertyName("log")] public JsonElement log { get; set; }         
    }
}