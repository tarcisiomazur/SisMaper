using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SisMaper.API.WebMania.Models;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.API.WebMania
{
    public class NotaFiscalEletronica : INotaFiscal
    {

        public NotaFiscalEletronica(NotaFiscal notaFiscal)
        {
            NotaFiscal = notaFiscal;
        }

        public bool Validate()
        {
            return true;
        }

        public override string BuildJsonDefault()
        {
            NF_NotaFiscal NF_NotaFiscal;

            #region NotaFiscal

            if (NotaFiscal == null) return "NotaFiscal Nula";
            if (NotaFiscal.Id == 0) return "ID Inválido";
            NF_NotaFiscal = new NF_NotaFiscal()
            {
                ID = NotaFiscal.Id.ToString(),
                Ambiente = NF_NotaFiscal.EnumAmbiente.Homologacao,
                Modelo = NF_NotaFiscal.EnumModelo.NFe,
                Operacao = NF_NotaFiscal.EnumOperacao.Saida,
                Finalidade = NF_NotaFiscal.EnumFinalidade.Normal,
                Pedido = new NF_Pedido(),
                Produtos = new List<NF_Produtos>()
            };

            #endregion

            #region Pedido

            var pedido = NotaFiscal.Pedido;

            if (pedido == null) return "Pedido Nulo";
            if (pedido.Itens == null || pedido.Itens.Count == 0) return "Pedido sem Itens";

            NF_NotaFiscal.Natureza = pedido.Natureza?.Descricao ?? "Venda Normal";
            NF_NotaFiscal.Pedido.desconto =
                pedido.Itens.Sum(item => item.Desconto).ToString(CultureInfo.InvariantCulture);
            NF_NotaFiscal.Pedido.frete = "0.00";
            NF_NotaFiscal.Pedido.Modalidade_frete = NF_Pedido.EnumFrete.Sem;
            NF_NotaFiscal.Pedido.Presenca = NF_Pedido.EnumPresenca.Presencial;

            #endregion

            #region Cliente

            if (pedido.Cliente == null)
            {
                return "Cliente deve ser Identificado";
            }
            if (BuildCliente(pedido.Cliente, NF_NotaFiscal) is { } error)
                return error;
            #endregion


            #region Produto

            foreach (var item in pedido.Itens)
            {
                if (item.Produto == null) return "Produto Inválido";
                if (item.Produto.NCM == null) return "Produto sem NCM";

                NF_NotaFiscal.Produtos.Add(new NF_Produtos(item, pedido.Natureza));
            }

            #endregion

            Json = JsonConvert.SerializeObject(NF_NotaFiscal, WebManiaConnector.Settings);
            return "OK";
        }

        public override async Task<bool> Emit()
        {
            if (string.IsNullOrEmpty(Json)) return false;
            NF_Result = WebManiaConnector.Emitir(Json);
            return NF_Result != null;
        }
    }
}