using System.IO;
using System.Xml;
using Microsoft.Win32;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Tools;

public class Import
{
    public static PList<Produto>? ImportProdutosFromXML()
    {
        var fileChoser = new OpenFileDialog()
        {
            DefaultExt = ".cml",
            Filter = "XML Files (*.xml)|*.xml",
            Title = "Selecione o XML da nota fiscal",
            Multiselect = false
        };

        if (!fileChoser.ShowDialog().IsTrue()) return null;

        var file = File.OpenText(fileChoser.FileName);
        var doc = new XmlDocument();
        doc.Load(file);
        var nfe = doc["nfeProc"]?["NFe"]?["infNFe"];
        if (nfe is null) return null;

        var produtos = new PList<Produto>();
        foreach (XmlNode nfeChildNode in nfe.ChildNodes)
        {
            if (nfeChildNode.Name == "det" && ProcessProduct(nfeChildNode["prod"]) is { } produto)
            {
                produtos.Add(produto);
            }
        }

        return produtos;
    }

    private static Produto? ProcessProduct(XmlNode? xml)
    {
        if (xml is null) return null;
        var produto = new Produto();
        var descricao = xml["xProd"]?.FirstChild?.Value ?? "";
        produto.Descricao = descricao;
        var ncm = xml["NCM"]?.FirstChild?.Value ?? "";
        produto.NCM = new NCM() {Id = ncm.ToLong()};
        var codBarras = xml["cEANTrib"]?.FirstChild?.Value ?? "";
        produto.CodigoBarras = codBarras;
        var valor = xml["vUnCom"]?.FirstChild?.Value ?? "";
        produto.PrecoCusto = valor.ToDecimal(2);
        return produto;
    }
}