using System.Threading;
using System.Threading.Tasks;
using SisMaper.API.WebMania.Models;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.API.WebMania
{
    public abstract class INotaFiscal
    {
        public string Json { get; set; }
        public NotaFiscal NotaFiscal { get; set; }
        public NF_Result? NF_Result { get; set; }
        public abstract string BuildJsonDefault();

        public string? BuildCliente(Cliente cliente, NF_NotaFiscal NF_NotaFiscal)
        {
            if (string.IsNullOrEmpty(cliente .Nome)) return "Nome do Cliente Inválido";
            if (cliente.Cidade == null) return "Cidade do Cliente Inválida";
            if (string.IsNullOrEmpty(cliente.Bairro)) return "Bairro do Endereco do Cliente Inválido";
            if (string.IsNullOrEmpty(cliente.Endereco)) return "Endereco do Cliente Inválido";
            if (string.IsNullOrEmpty(cliente.Numero)) return "Numero do Endereço Inválido";
            if (string.IsNullOrEmpty(cliente.CEP)) return "CEP do Endereço Inválido";
            if (!int.TryParse(cliente.CEP, out var CEP)) return "CEP do Endereço Inválido";
            cliente.Cidade.Estado.Load();
            var NF_Cliente = new NF_Cliente();
            NF_Cliente.NomeCompleto = cliente.Nome;
            NF_Cliente.Cidade = cliente.Cidade.Nome;
            NF_Cliente.Endereco = cliente.Endereco;
            NF_Cliente.Bairro = cliente.Bairro;
            NF_Cliente.Numero = cliente.Numero;
            NF_Cliente.CEP = CEP.ToString(@"00000-000");
            NF_Cliente.UF = cliente.Cidade.Estado.UF;
            if (cliente is PessoaFisica pessoaFisica)
            {
                if (string.IsNullOrEmpty(pessoaFisica.CPF)) return "Cliente não possui CPF";
                if (!pessoaFisica.CPF.IsCpf()) return "CPF Inválido";
                NF_Cliente.CPF = pessoaFisica.MaskedCPF;
                NF_NotaFiscal.Cliente = NF_Cliente;
            }
            else if (cliente is PessoaJuridica pessoaJuridica)
            {
                if (string.IsNullOrEmpty(pessoaJuridica.CNPJ)) return "Cliente não possui CNPJ";
                if (string.IsNullOrEmpty(pessoaJuridica.RazaoSocial)) return "Cliente não possui Razão Social";
                if (!pessoaJuridica.CNPJ.IsCnpj()) return "CNPJ incorreto";
                NF_Cliente.CNPJ = pessoaJuridica.MaskedCNPJ;
                NF_Cliente.RazaoSocial = pessoaJuridica.RazaoSocial;
                NF_Cliente.InscricaoEstadual = pessoaJuridica.InscricaoEstadual;
                NF_NotaFiscal.Cliente = NF_Cliente;
            }

            return null;
        }
        
        public bool Emit()
        {
            if (string.IsNullOrEmpty(Json)) return false;
            NF_Result = WebManiaConnector.Emitir(Json);
            return NF_Result != null;
        }
    }
}