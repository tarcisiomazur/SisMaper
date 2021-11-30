using System;
using System.Net;
using SisMaper.API.WebMania.Models;
using SisMaper.Models;

namespace SisMaper.Tools
{
    public class NFConverter
    {
        public static void Merge(NF_Result result, NotaFiscal notaFiscal)
        {
            notaFiscal.UUID = result.uuid;
            notaFiscal.Chave = result.chave;
            notaFiscal.URL_XML = result.xml;
            notaFiscal.URL_DANFE = result.danfe_simples;
            try
            {
                var value = result.log.GetProperty("aProt");
                var value1 = value[0].GetProperty("dhRecbto");
                notaFiscal.DataEmissao = DateTime.Parse(value1.GetString());
            }
            catch
            {
                
            }
            notaFiscal.Situacao = result.status switch
            {
                "aprovado" => NotaFiscal.EnumSituacao.Aprovado,
                "cancelado" => NotaFiscal.EnumSituacao.Cancelado,
                "processamento" => NotaFiscal.EnumSituacao.Processamento,
                "contingencia" => NotaFiscal.EnumSituacao.Contingencia,
                "denegado" => NotaFiscal.EnumSituacao.Denegado,
                "reprovado" => NotaFiscal.EnumSituacao.Reprovado,
                _ => notaFiscal.Situacao
            };
            if (int.TryParse(result.nfe, out var numero))
            {
                notaFiscal.Numero = numero;
            }
            if (int.TryParse(result.serie, out var serie))
            {
                notaFiscal.Serie = serie;
            }
            using (var client = new WebClient())
            {
                var file = client.DownloadData(notaFiscal.URL_XML);
                if (file != null)
                {
                    notaFiscal.XML = file;
                }
            }
            
        }
    }
}