using System.Text.Json.Serialization;

namespace SisMaper.API.WebMania.Models
{
    public class NF_Cliente
    {

        [JsonPropertyName("cpf")] public string CPF { get; set; }

        [JsonPropertyName("nome_completo")] public string NomeCompleto { get; set; } = "";

        [JsonPropertyName("cnpj")] public string CNPJ { get; set; }

        [JsonPropertyName("razao_social")] public string RazaoSocial { get; set; }

        [JsonPropertyName("ie")] public string InscricaoEstadual { get; set; }

        [JsonPropertyName("endereco")] public string Endereco { get; set; } = "";

        [JsonPropertyName("numero")] public string Numero { get; set; } = "";

        [JsonPropertyName("bairro")] public string Bairro { get; set; } = "";

        [JsonPropertyName("cidade")] public string Cidade { get; set; } = "";

        [JsonPropertyName("cep")] public string CEP { get; set; } = "";

        [JsonPropertyName("uf")] public string UF { get; set; } = "";

        public NF_Cliente()
        {
            
        }
    }
}