using Newtonsoft.Json;

namespace SisMaper.API.WebMania.Models;

public class NF_Cliente
{
    [JsonProperty("cpf")] public string CPF { get; set; }

    [JsonProperty("nome_completo")] public string NomeCompleto { get; set; }

    [JsonProperty("cnpj")] public string CNPJ { get; set; }

    [JsonProperty("razao_social")] public string RazaoSocial { get; set; }

    [JsonProperty("ie")] public string InscricaoEstadual { get; set; }

    [JsonProperty("endereco")] public string Endereco { get; set; }

    [JsonProperty("numero")] public string Numero { get; set; }

    [JsonProperty("bairro")] public string Bairro { get; set; }

    [JsonProperty("cidade")] public string Cidade { get; set; }

    [JsonProperty("cep")] public string CEP { get; set; }

    [JsonProperty("uf")] public string UF { get; set; }
}