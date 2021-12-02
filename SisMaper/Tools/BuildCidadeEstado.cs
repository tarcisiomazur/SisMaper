using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Tools
{
    public class BuildCidadeEstado
    {
        public class Raw
        {
            public List<Estado> estados { get; set; }

            public class Estado
            {
                public string sigla { get; set; }
                public string nome { get; set; }
                public List<string> cidades { get; set; }
            }
        }

        private const string URL =
            "https://gist.githubusercontent.com/letanure/3012978/raw/78474bd9db11e87de65a9d3c9fc4452458dc8a68/estados-cidades.json";

        public static PList<Estado> Build() // Not Work
        {
            var r = PList<Estado>.FindWhereQuery("ID > 0");
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(URL);
                var reader = new JsonTextReader(new StringReader(json));
                var j = new JsonSerializer().Deserialize<Raw>(reader);
                foreach (var jEstado in j.estados)
                {
                    var oldEstado =
                        r.FirstOrDefault(oldE => oldE.Nome.Equals(jEstado.nome, StringComparison.InvariantCulture)) ?? new Estado();
                    if (oldEstado.Nome != jEstado.nome || oldEstado.UF != jEstado.sigla)
                    {
                        oldEstado.Nome = jEstado.nome;
                        oldEstado.UF = jEstado.sigla;
                        oldEstado.Save();
                    }
                    /*
                    foreach (var c in jEstado.cidades.Select(jCidade => new Cidade {Nome = jCidade, Estado = oldEstado}))
                    {
                        var oldC = oldEstado.Cidades.FirstOrDefault(oldC => oldC.Nome.Equals(c.Nome));
                        if (oldC.Nome != c.Nome)
                        {
                            oldC.Nome = c.Nome;
                            oldC.Save();
                        }
                    }*/
                }

                return r;
            }
        }
    }
}