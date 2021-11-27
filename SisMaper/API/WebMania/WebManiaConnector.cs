using RestSharp;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.API.WebMania
{
    
    public static class WebManiaConnector
    {
        private static readonly RestClient Client;
        private const string URL = "https://webmaniabr.com/api/1/nfe/emissao/";
        
        private static string ACCESS_TOKEN_SECRET;
        private static string ACCESS_TOKEN;
        private static string CONSUMER_SECRET;
        private static string CONSUMER_KEY;

        static WebManiaConnector()
        {
            Client = new RestClient(URL);
        }

        public static void Init(Configuracoes empresa)
        {
            ACCESS_TOKEN_SECRET = Encrypt.RSADecryption(empresa.ACCESS_TOKEN_SECRET);
            ACCESS_TOKEN = Encrypt.RSADecryption(empresa.ACCESS_TOKEN);
            CONSUMER_SECRET = Encrypt.RSADecryption(empresa.CONSUMER_SECRET);
            CONSUMER_KEY = Encrypt.RSADecryption(empresa.CONSUMER_KEY);
        }
        
        public static RestRequest BuildRequest()
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-access-token-secret", ACCESS_TOKEN_SECRET);
            request.AddHeader("x-access-token", ACCESS_TOKEN);
            request.AddHeader("x-consumer-secret", CONSUMER_SECRET);
            request.AddHeader("x-consumer-key", CONSUMER_KEY);
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
            
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            IRestResponse response = Client.Execute(request);
            return response;
        }
    }
}