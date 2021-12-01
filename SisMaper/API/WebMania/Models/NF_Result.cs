using Newtonsoft.Json;

namespace SisMaper.API.WebMania.Models
{
    public class NF_Result
    {
        [JsonProperty("uuid")] public string uuid { get; set; }         
        [JsonProperty("status")] public string status { get; set; }         
        [JsonProperty("motivo")] public string motivo { get; set; }         
        [JsonProperty("nfe")] public string nfe { get; set; }
        [JsonProperty("serie")] public string serie { get; set; }
        [JsonProperty("recibo")] public string recibo { get; set; }         
        [JsonProperty("chave")] public string chave { get; set; }         
        [JsonProperty("modelo")] public string modelo { get; set; }         
        [JsonProperty("xml")] public string xml { get; set; }         
        [JsonProperty("danfe")] public string danfe { get; set; }         
        [JsonProperty("danfe_simples")] public string danfe_simples { get; set; }         
        [JsonProperty("danfe_etiqueta")] public string danfe_etiqueta { get; set; }
        [JsonProperty("log")] public Log log { get; set; }         
    }

    public struct Log
    {
        [JsonProperty("aProt")]
        public AProt[] aProt { get; set; }
    }

    public struct AProt
    {
        [JsonProperty("dhRecbto")]
        public string dhRecbto { get; set; }
    }
}