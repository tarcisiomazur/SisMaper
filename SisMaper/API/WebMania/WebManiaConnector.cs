﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using RestSharp;
using SisMaper.API.WebMania.Models;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.API.WebMania
{
    
    public static class WebManiaConnector
    {
        private static RestClient Client;
        public static JsonSerializerOptions Options;
        private const string URL_EMISSAO = "https://webmaniabr.com/api/1/nfe/emissao/";
        private const string URL_CONSULTA = "https://webmaniabr.com/api/1/nfe/consulta/";
        private const string URL_SEFAZ = "https://webmaniabr.com/api/1/nfe/sefaz/";
        
        private static string ACCESS_TOKEN_SECRET;
        private static string ACCESS_TOKEN;
        private static string CONSUMER_SECRET;
        private static string CONSUMER_KEY;

        static WebManiaConnector()
        {
            Client = new RestClient();
            Options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true
            };
        }

        public static void Init(Configuracoes empresa)
        {
            ACCESS_TOKEN_SECRET = Encrypt.RSADecryption(empresa.ACCESS_TOKEN_SECRET);
            ACCESS_TOKEN = Encrypt.RSADecryption(empresa.ACCESS_TOKEN);
            CONSUMER_SECRET = Encrypt.RSADecryption(empresa.CONSUMER_SECRET);
            CONSUMER_KEY = Encrypt.RSADecryption(empresa.CONSUMER_KEY);
        }
        
        public static RestRequest BuildRequest(string URL, Method method)
        {
            var request = new RestRequest(URL, method);
            request.AddHeader("x-access-token-secret", ACCESS_TOKEN_SECRET);
            request.AddHeader("x-access-token", ACCESS_TOKEN);
            request.AddHeader("x-consumer-secret", CONSUMER_SECRET);
            request.AddHeader("x-consumer-key", CONSUMER_KEY);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("cache-control", "no-cache");
            return request;
        }
        
        public static NF_Result? Emitir(string json)
        {
            var request = BuildRequest(URL_EMISSAO,Method.POST);
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            var timer = new Stopwatch();
            timer.Start();
            IRestResponse response = Client.Execute(request);
            timer.Stop();
            LogRequest(request,response,timer.ElapsedMilliseconds);
            try
            {
                return JsonSerializer.Deserialize<NF_Result>(response.Content);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
        
        public static NF_Result? Consultar(string chave)
        {
            var request = BuildRequest(URL_CONSULTA, Method.GET);
            request.AddParameter(chave.Length == 44 ? "chave" : "uuid", chave, ParameterType.QueryString);
            var timer = new Stopwatch();
            timer.Start();
            IRestResponse response = Client.Execute(request);
            timer.Stop();
            LogRequest(request,response,0);
            try
            {
                return JsonSerializer.Deserialize<NF_Result>(response.Content);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
        
        
        private static void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
        {
            var requestToLog = new
            {
                resource = request.Resource,
                parameters = request.Parameters.Select(parameter => new
                {
                    name = parameter.Name,
                    value = parameter.Value,
                    type = parameter.Type.ToString()
                }),
                method = request.Method.ToString(),
                uri = Client.BuildUri(request),
            };
            var responseToLog = new
            {
                statusCode = response.StatusCode,
                content = response.Content,
                headers = response.Headers,
                responseUri = response.ResponseUri,
                errorMessage = response.ErrorMessage,
            };

            Trace.Write(string.Format("Request completed in {0} ms, Request: {1}, Response: {2}",
                durationMs, 
                JsonSerializer.Serialize(requestToLog,Options),
                JsonSerializer.Serialize(responseToLog,Options)));
        }
    }
}