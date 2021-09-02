using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Tools
{
    public class BuildCidadeEstado
    {
        public class Raw
        {
            public List<Estado> estados{ get; set; }
            
            public class Estado
            {
                public string sigla{ get; set; }
                public string nome{ get; set; }
                public List<string> cidades{ get; set; }
            }
        }
        
        private const string URL = "https://gist.githubusercontent.com/letanure/3012978/raw/78474bd9db11e87de65a9d3c9fc4452458dc8a68/estados-cidades.json";
        public static PList<Estado> Build()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(URL);
                var j = JsonSerializer.Deserialize<Raw>(json);
                return j.estados.Select(estado => new Estado
                {
                    Nome = estado.nome,
                    Cidades = estado.cidades.Select(cidade => new Cidade
                    {
                        Nome = cidade,
                    }).ToPList()
                }).ToPList();
            }
        }
    }
}