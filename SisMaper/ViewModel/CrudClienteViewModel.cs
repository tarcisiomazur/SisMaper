using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class CrudClienteViewModel : BaseViewModel
    {
        private Estado? _estadoSelecionado;

        public Estado? EstadoSelecionado
        {
            get { return _estadoSelecionado; }
            set
            {
                SetField(ref _estadoSelecionado, value);
                SetCidades(value);

            }
        }


        private Cidade? _cidadeSelecionada;

        public Cidade? CidadeSelecionada
        {
            get { return _cidadeSelecionada; }
            set 
            { 
                SetField(ref _cidadeSelecionada, value);
            }

        }

        public PessoaFisica PessoaFisica { get; set; } = new();
        public PessoaJuridica PessoaJuridica { get; set; } = new();

        private ListarClientes? cliente;


        public PList<Estado> Estados { get; private set; }

        private PList<Cidade>? _cidades;
        public PList<Cidade>? Cidades
        {
            get { return _cidades; }
            set { SetField(ref _cidades, value); }
        }

        
        public SimpleCommand SalvarPessoaFisicaCmd => new(SalvarPessoaFisica);
        public SimpleCommand SalvarPessoaJuridicaCmd => new(SalvarPessoaJuridica);

        public Action? ClienteSaved { get; set; }


        public CrudClienteViewModel(ListarClientes? clienteSelecionado)
        {

            cliente = clienteSelecionado;

            Estados = DAO.All<Estado>();

            //SavePessoaFisica = new SavePessoaFisicaCommand();
            //SavePessoaJuridica = new SavePessoaJuridicaCommand();

            if (cliente is not null)
            {
                if (cliente.Tipo == ListarClientes.Pessoa.Fisica)
                {
                    PessoaFisica = DAO.Load<PessoaFisica>(cliente.Id);

                    if (PessoaFisica.Cidade is not null)
                    {
                        EstadoSelecionado = Estados.Where(e => e.Id == PessoaFisica.Cidade.Estado.Id).FirstOrDefault();
                        CidadeSelecionada = Cidades.Where(c => c.Id == PessoaFisica.Cidade.Id).FirstOrDefault();
                    }

                }
                else
                {
                    PessoaJuridica = DAO.Load<PessoaJuridica>(cliente.Id);
                    if (PessoaJuridica.Cidade is not null)
                    {
                        EstadoSelecionado = Estados.Where(e => e.Id == PessoaJuridica.Cidade.Estado.Id).FirstOrDefault();
                        CidadeSelecionada = Cidades.Where(c => c.Id == PessoaJuridica.Cidade.Id).FirstOrDefault();
                    }
                }

                return;
                
            }

            CidadeSelecionada = null;
            EstadoSelecionado = null;
            Cidades = null;

        }


        private void SetCidades(Estado e)
        {

            if (e is not null)
            {
                Cidades = e.Cidades;
                if(Cidades is not null && Cidades.Count > 0)
                {
                    CidadeSelecionada = Cidades.First();
                }
                
            }

            else
            {
                CidadeSelecionada = null;
                Cidades = null;
            }
        }



        private bool SearchCliente(Cliente c)
        {
            if (c is PessoaFisica pf)
            {           
                PList<PessoaFisica> pessoas = DAO.All<PessoaFisica>();
                return pessoas.Where(p => (p.CPF == pf.CPF && p.Id != pf.Id)).Any();
            }

            else if (c is PessoaJuridica pj)
            {
                PList<PessoaJuridica> pessoas = DAO.All<PessoaJuridica>();
                return pessoas.Where(p => (p.CNPJ == pj.CNPJ && p.Id != pj.Id)).Any();
            }
            
            return false;
        }


        private void NullText(ref string? texto)
        {
            if(string.IsNullOrWhiteSpace(texto))
            {
                texto = null;
            }
        }


        private bool CheckDigitoVerificadorCPF(string cpf)
        {
            //verifica se todos os elementos são iguais
            if (cpf.All(ch => ch == cpf[0]))
                return false;

            int somaDigito1 = 0, somaDigito2 = 0, multiplicador = 11;

            for(int i = 0;i < 10;i++)
            {
                somaDigito2 += Convert.ToInt16(cpf[i].ToString()) * multiplicador;

                if(multiplicador < 11)
                    somaDigito1 += Convert.ToInt16(cpf[i-1].ToString()) * multiplicador;

                multiplicador--;
            }

            int digito1 = 11 - (somaDigito1 % 11);
            int digito2 = 11 - (somaDigito2 % 11);

            if (digito1 > 9)
                digito1 = 0;


            if (digito2 > 9)
                digito2 = 0;


            string digitos = digito1.ToString() + digito2.ToString();

            return (cpf.EndsWith(digitos));
        }


        private bool CheckDigitoVerificadorCNPJ(string cnpj)
        {
            //verifica se todos os elementos são iguais
            if (cnpj.All(ch => ch == cnpj[0]))
                return false;

            int[] multiplicadores = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int somaDigito1 = 0, somaDigito2 = 0;

            for(int i = 0;i < multiplicadores.Length;i++)
            {

                Console.WriteLine(cnpj[i].ToString());

                somaDigito2 += Convert.ToInt16(cnpj[i].ToString()) * multiplicadores[i];

                if(i > 0)
                    somaDigito1 += Convert.ToInt16(cnpj[i-1].ToString()) * multiplicadores[i];
            }

            int digito1 = 11 - (somaDigito1 % 11);
            int digito2 = 11 - (somaDigito2 % 11);

            if (digito1 > 9)
                digito1 = 0;

            if (digito2 > 9)
                digito2 = 0;

            string digitos = digito1.ToString() + digito2.ToString();

            return (cnpj.EndsWith(digitos));

        }
       

        private void CheckCEP(ref string? cep)
        {
            if (cep is null || cep.Equals("00.000-000"))
            {
                cep = null;
                return;
            }

            cep = cep.Replace(".", "").Replace("-", "");

            if (cep.Contains('_'))
            {
                throw new InvalidOperationException("CEP Incompleto");
            }
        }



        private void SalvarCliente(Cliente clienteParameter)
        {
            //Console.WriteLine($"CEPP === {CepString}");

            if (clienteParameter is PessoaFisica pf)
            {
                //Console.WriteLine($"CPFFF ===   {pf.CPF}");

                Console.WriteLine($"CEPP === {pf.CEP}");

                if(CidadeSelecionada is not null)
                {
                    pf.Cidade = CidadeSelecionada;
                }
                else
                {
                    pf.Cidade = null;
                }


                if (pf.CPF is null || pf.CPF.Equals("___.___.___-__"))
                {
                    throw new InvalidOperationException("CPF não pode ser vazio");
                }


                pf.CPF = pf.CPF.Replace(".", "").Replace("-", "");

                if (pf.CPF.Contains('_'))
                {
                    throw new InvalidOperationException("CPF incompleto");
                }

                if (!CheckDigitoVerificadorCPF(pf.CPF))
                {
                    throw new InvalidOperationException("CPF Inválido");
                }

                if (SearchCliente(pf))
                {
                    throw new InvalidOperationException("CPF já registrado");
                }

                string cep = pf.CEP;
                CheckCEP(ref cep);
                pf.CEP = cep;



                string? nome = pf.Nome;
                string? endereco = pf.Endereco;
                string? bairro = pf.Bairro;
                string? numero = pf.Numero;

                NullText(ref nome);
                NullText(ref endereco);
                NullText(ref bairro);
                NullText(ref numero);

                pf.Nome = nome;
                pf.Endereco = endereco;
                pf.Bairro = bairro;
                pf.Numero = numero;

                pf.Save();


            }

            else if (clienteParameter is PessoaJuridica pj)
            {
                if(CidadeSelecionada is not null)
                {
                    pj.Cidade = CidadeSelecionada;
                }
                else
                {
                    pj.Cidade = null;
                }


                if (pj.CNPJ is null || pj.CNPJ.Equals("__.___.___/____-__"))
                {
                    throw new InvalidOperationException("CNPJ não pode ser vazio");
                }

                if (pj.CNPJ.Length == 18)
                {
                    pj.CNPJ = pj.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                }

                if (pj.CNPJ.Contains('_'))
                {
                    throw new InvalidOperationException("CNPJ incompleto");
                }

                if (!CheckDigitoVerificadorCNPJ(pj.CNPJ))
                {
                    throw new InvalidOperationException("CNPJ Inválido");
                }

                if (SearchCliente(pj))
                {
                    throw new InvalidOperationException("CNPJ já registrado");
                }

                string cep = pj.CEP;
                CheckCEP(ref cep);
                pj.CEP = cep;


                string? nome = pj.Nome;
                string? endereco = pj.Endereco;
                string? razaoSocial = pj.RazaoSocial;
                string? numero = pj.Numero;
                string? bairro = pj.Bairro;

                NullText(ref nome);
                NullText(ref endereco);
                NullText(ref razaoSocial);
                NullText(ref numero);
                NullText(ref bairro);

                pj.Nome = nome;
                pj.Endereco = endereco;
                pj.RazaoSocial = razaoSocial;
                pj.Numero = numero;
                pj.Bairro = bairro;

                pj.Save();
                    
            }

            
        }



        private void SalvarPessoaFisica()
        {
            try
            {
                SalvarCliente(PessoaFisica);
                ClienteSaved?.Invoke();
            }
            catch (Exception ex)
            {
                OnShowMessage("Erro ao salvar Cliente", "Erro: " + ex.Message);
            }

        }

        private void SalvarPessoaJuridica()
        {
            try
            {
                SalvarCliente(PessoaJuridica);
                ClienteSaved?.Invoke();
            }
            catch (Exception ex)
            {
                OnShowMessage("Erro ao salvar Cliente", "Erro: " + ex.Message);
            }
        }
    }


}

