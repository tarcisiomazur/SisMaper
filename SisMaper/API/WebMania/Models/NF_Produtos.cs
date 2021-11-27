﻿using System.Text.Json.Serialization;

namespace SisMaper.API.WebMania.Models
{
    public class NF_Produtos
    {
        public enum EnumOrigem
        {
            Nacional0,
            Estrangeira1,
            Estrangeira2,
            Nacional3,
            Nacional4,
            Nacional5,
            Estrangeira6,
            Estrangeira7,
            Nacional8,
        }
        
        [JsonPropertyName("ID")] public string ID { get; set; } = "";
        [JsonPropertyName("codigo")] public string Codigo { get; set; } = "";
        [JsonPropertyName("nome")] public string Nome { get; set; } = "";
        [JsonPropertyName("ncm")] public string NCM { get; set; } = "";
        [JsonPropertyName("cest")] public string CEST { get; set; } = "";
        [JsonPropertyName("quantidade")] public string Quantidade { get; set; } = "";
        [JsonPropertyName("unidade")] public string Unidade { get; set; } = "";
        [JsonPropertyName("origem")] public EnumOrigem Origem { get; set; } = EnumOrigem.Nacional0;
        [JsonPropertyName("subtotal")] public string Subtotal { get; set; } = "";
        [JsonPropertyName("total")] public string Total { get; set; } = "";
        
        
        
    }
}