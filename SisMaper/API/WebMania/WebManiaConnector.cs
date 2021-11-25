using RestSharp;
using SisMaper.Models;

namespace SisMaper.API.WebMania
{
    
    public static class WebManiaConnector
    {
        private static readonly RestClient Client;

        static WebManiaConnector()
        {
            Client = new RestClient("https://webmaniabr.com/api/1/nfe/emissao/");
        }
        
        public static RestRequest BuildRequest()
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-access-token-secret", "SEU_ACCESS_TOKEN_SECRET");
            request.AddHeader("x-access-token", "SEU_ACCESS_TOKEN");
            request.AddHeader("x-consumer-secret", "SEU_CONSUMER_SECRET");
            request.AddHeader("x-consumer-key", "SEU_CONSUMER_KEY");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            return request;
        }
        
        public static IRestResponse? Emit(NotaFiscal notaFiscal, bool IsNfc)
        {
            string json = "";
            if (IsNfc)
            {
                var nfc = new NotaFiscalConsumidor(notaFiscal);
                var result = nfc.BuildJsonDefault();
                if (result != "OK") return null;
                json = nfc.Json;
            }
            else
            {
                var nfc = new NotaFiscalEletronica(notaFiscal);
                var result = nfc.BuildJsonDefault();
                if (result != "OK") return null;
                json = nfc.Json;
            }
            var request = BuildRequest();
            
            request.AddParameter("undefined",json, ParameterType.RequestBody);
            IRestResponse response = Client.Execute(request);
            return response;
        }
    }
}