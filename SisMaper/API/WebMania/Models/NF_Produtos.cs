using System.Globalization;
using Newtonsoft.Json;

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
        
        [JsonProperty("ID")] public string ID { get; set; }
        [JsonProperty("codigo")] public string Codigo { get; set; }
        [JsonProperty("nome")] public string Nome { get; set; }
        [JsonProperty("ncm")] public string NCM { get; set; }
        [JsonProperty("cest")] public string CEST { get; set; }
        [JsonProperty("quantidade")] public string Quantidade { get; set; }
        [JsonProperty("unidade")] public string Unidade { get; set; }
        [JsonProperty("origem")] public EnumOrigem Origem { get; set; } = EnumOrigem.Nacional0;
        [JsonProperty("subtotal")] public string Subtotal { get; set; }
        [JsonProperty("total")] public string Total { get; set; }
        [JsonProperty("informacoes_adicionais")] public string Informacoes { get; set; }

        [JsonProperty("classe_imposto")] public string ClasseImposto { get; set; }
        
        

        public NF_Produtos(SisMaper.Models.Item item, SisMaper.Models.Natureza natureza)
        {
            Nome = item.Produto.Descricao;
            Quantidade = item.Quantidade.ToString(CultureInfo.InvariantCulture);
            Unidade = item.Produto.Unidade?.Descricao ?? "";
            ID = item.Produto.Id.ToString();
            NCM = item.Produto.NCM.Id.ToString(@"0000\.00\.00");
            Codigo = item.Produto.CodigoBarras;
            Origem = EnumOrigem.Nacional0;
            Subtotal = item.Valor.ToString(CultureInfo.InvariantCulture);
            Total = (item.Valor * (decimal) item.Quantidade).ToString(CultureInfo.InvariantCulture);
            Informacoes = item.Lote?.Informacoes;
            ClasseImposto = natureza.Classe_de_Imposto;
        }
        
    }
}