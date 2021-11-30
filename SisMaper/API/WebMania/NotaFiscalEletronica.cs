using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
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
                Cliente = new NF_Cliente(),
                Produtos = new List<NF_Produtos>()
            };

            #endregion

            #region Pedido

            var pedido = NotaFiscal.Pedido;

            if (pedido == null) return "Pedido Nulo";
            if (pedido.Itens == null || pedido.Itens.Count == 0) return "Pedido sem Itens";

            NF_NotaFiscal.Natureza = pedido.Natureza?.Descricao ?? "";
            NF_NotaFiscal.Pedido.desconto =
                pedido.Itens.Sum(item => item.Desconto).ToString(CultureInfo.InvariantCulture);
            NF_NotaFiscal.Pedido.frete = "0.00";
            NF_NotaFiscal.Pedido.Modalidade_frete = NF_Pedido.EnumFrete.Sem;
            NF_NotaFiscal.Pedido.Presenca = NF_Pedido.EnumPresenca.Presencial;

            #endregion

            #region Cliente

            var cliente = pedido.Cliente;
            if (cliente != null)
            {
                if (string.IsNullOrEmpty(pedido.Cliente.Nome)) return "Nome do Cliente Inválido";
                if (cliente is PessoaFisica pessoaFisica)
                {
                    if (pessoaFisica.Cidade == null) return "Cliente não possui cidade";
                    if (string.IsNullOrEmpty(pessoaFisica.Endereco)) return "Cliente não possui endereço";
                    if (string.IsNullOrEmpty(pessoaFisica.CPF)) return "Cliente não possui CPF";
                    if (!pessoaFisica.CPF.IsCpf()) return "CPF incorreto";

                    NF_NotaFiscal.Cliente.NomeCompleto = pessoaFisica.Nome;
                    NF_NotaFiscal.Cliente.Cidade = pessoaFisica.Cidade.Nome;
                    NF_NotaFiscal.Cliente.Endereco = pessoaFisica.Endereco;
                    NF_NotaFiscal.Cliente.CPF = pessoaFisica.MaskedCPF;
                }
                else if (cliente is PessoaJuridica pessoaJuridica)
                {
                    if (pessoaJuridica.Cidade == null) return "Cliente não possui cidade";
                    if (string.IsNullOrEmpty(pessoaJuridica.Endereco)) return "Cliente não possui endereço";
                    if (string.IsNullOrEmpty(pessoaJuridica.RazaoSocial)) return "Cliente não possui razão social";
                    if (string.IsNullOrEmpty(pessoaJuridica.CNPJ)) return "Cliente não possui CNPJ";
                    if (!pessoaJuridica.CNPJ.IsCpf()) return "CPF incorreto";

                    NF_NotaFiscal.Cliente.NomeCompleto = pessoaJuridica.Nome;
                    NF_NotaFiscal.Cliente.Cidade = pessoaJuridica.Cidade.Nome;
                    NF_NotaFiscal.Cliente.Endereco = pessoaJuridica.Endereco;
                    NF_NotaFiscal.Cliente.CNPJ = pessoaJuridica.MaskedCNPJ;
                    NF_NotaFiscal.Cliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                    NF_NotaFiscal.Cliente.InscricaoEstadual = pessoaJuridica.InscricaoEstadual;
                }
            }

            #endregion


            #region Produto

            foreach (var item in pedido.Itens)
            {
                if (item.Produto == null) return "Produto Inválido";
                if (item.Produto.NCM == null) return "Produto sem NCM";

                NF_NotaFiscal.Produtos.Add(new NF_Produtos(item));
            }

            #endregion

            var op = new JsonSerializerOptions();
            op.WriteIndented = true;
            op.IgnoreNullValues = true;
            Json = JsonSerializer.Serialize(NF_NotaFiscal, new JsonSerializerOptions(op));
            return "OK";
        }

        public override bool Emit()
        {
            if (Json != null)
            {
                WebManiaConnector.Emitir(Json);
                return true;
            }

            return true;
        }
    }
}