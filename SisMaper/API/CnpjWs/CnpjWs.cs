using Newtonsoft.Json.Linq;
using RestSharp;
using SisMaper.API.ViaCEP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.API.CnpjWs
{
  
    public class CnpjResponse
    {
        public string? CNPJ;
        public string? SituacaoCadastral;
        public string? Nome;
        public string? RazaoSocial;
        public string? InscricaoEstadual;
        public string? Cep;

    }



    public class CnpjWs
    {

        public static CnpjResponse ConsultarCNPJ(string cnpj)
        {
            var request = new RestRequest($"https://publica.cnpj.ws/cnpj/{cnpj}", Method.GET);
            IRestResponse response = new RestClient().Execute(request);

            if (!response.IsSuccessful)
            {
           
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    throw new Exception("Foram realizadas três consultas consecutivas, espere 1 minuto para realizar outra consulta");

                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    throw new Exception("CNPJ inválido");

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception("CNPJ não encontrado");

                else
                    throw new Exception("Não foi possível consultar o CNPJ. Verifique seu acesso a Internet");
            }

                      

            JObject json = JObject.Parse(response.Content);

            JToken? inscricoesEstaduais = json.SelectToken("estabelecimento.inscricoes_estaduais");
            string? inscricao_estadual = null;


            if (inscricoesEstaduais is not null && inscricoesEstaduais.Any())
            {
                foreach(JToken? token in inscricoesEstaduais.Children())
                {
                    if ((bool?)token.SelectToken("ativo") == true && string.Equals( token.SelectToken("estado")?.ToString(), json.SelectToken("estabelecimento.estado")?.ToString() ))
                    {
                        inscricao_estadual = token.SelectToken("inscricao_estadual")?.ToString();
                        break;
                    }
                }
            }

            CnpjResponse dados = new()
            {
                SituacaoCadastral = json.SelectToken("estabelecimento.situacao_cadastral")?.ToString(),
                Nome = json.SelectToken("estabelecimento.nome_fantasia")?.ToString(),
                RazaoSocial = json.SelectToken("razao_social")?.ToString(),
                InscricaoEstadual = inscricao_estadual,
                Cep = json.SelectToken("estabelecimento.cep")?.ToString().Replace(".", "").Replace("-", ""),
                CNPJ = cnpj
            };

            return dados;

        }
    }

}

