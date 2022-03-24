using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.API.ViaCEP
{
    public class CepResponse
    {
        public string? UF;
        public string? Cidade;
        public string? Endereco;
        public string? Bairro;
    }

    public class ViaCepConnector
    {

        public static CepResponse Consultar(string cep)
        {
            var request = new RestRequest($"https://viacep.com.br/ws/{cep}/json", Method.GET);
            IRestResponse response = new RestClient().Execute(request);

            if (!response.IsSuccessful) throw new Exception("Verifique seu acesso a Internet");


            JObject json = JObject.Parse(response.Content);

            if (json.ContainsKey("erro")) throw new Exception("CEP Inválido ou não encontrado");

            CepResponse endereco = new CepResponse()
            {
                UF = json.GetValue("uf")?.ToString(),
                Cidade = json.GetValue("localidade")?.ToString(),
                Endereco = json.GetValue("logradouro")?.ToString(),
                Bairro = json.GetValue("bairro")?.ToString(),
            };

            return endereco;
        }
    }


}
