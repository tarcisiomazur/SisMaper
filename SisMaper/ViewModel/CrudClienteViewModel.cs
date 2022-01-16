using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class CrudClienteViewModel : BaseViewModel, IClienteSave
    {
        private Estado _estadoSelecionado;

        public Estado EstadoSelecionado
        {
            get { return _estadoSelecionado; }
            set
            {    
                SetField(ref _estadoSelecionado, value);
                SetCidades(value);
            }
        }


        private Cidade _cidadeSelecionada;

        public Cidade CidadeSelecionada
        {
            get { return _cidadeSelecionada; }
            set 
            { 
                SetField(ref _cidadeSelecionada, value);
                TextoCidade = value.Nome;
            }

        }

        public PessoaFisica PessoaFisica { get; set; }
        public PessoaJuridica PessoaJuridica { get; set; }

        private Cliente cliente;


        public PList<Estado> Estados { get; private set; }

        private PList<Cidade> _cidades;
        public PList<Cidade> Cidades
        {
            get { return _cidades; }
            set { SetField(ref _cidades, value); }
        }

        public SavePessoaFisicaCommand SavePessoaFisica { get; private set; }

        public SavePessoaJuridicaCommand SavePessoaJuridica { get; private set; }

        public Action SaveCliente { get; set; }


        public string _textoCidade;
        public string TextoCidade
        {
            get { return _textoCidade; }
            set 
            {
                SetField(ref _textoCidade, value);
                SetTextoCidade(value);
            } 
        }

        public Action ClientePessoaFisica { get; set; }
        public Action ClientePessoaJuridica { get; set; }

        public CrudClienteViewModel(object clienteSelecionado)
        {

            cliente = (Cliente)clienteSelecionado;

            Estados = DAO.All<Estado>();

            if (cliente is not null)
            {
                if (cliente.Cidade is not null)
                {
                    
                    EstadoSelecionado = cliente.Cidade.Estado;
                    CidadeSelecionada = cliente.Cidade;

                    foreach (Estado e in Estados)
                    {
                        if (e.Id == CidadeSelecionada.Estado.Id)
                        {
                            EstadoSelecionado = e;
                            foreach (Cidade c in e.Cidades)
                            {
                                if (c.Id == cliente.Cidade.Id)
                                {
                                    CidadeSelecionada = c;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }


                PList<PessoaFisica> pessoasFisicas = DAO.All<PessoaFisica>();
                PList<PessoaJuridica> pessoasJuridicas = DAO.All<PessoaJuridica>();

                int countPf = pessoasFisicas.Count;
                int countPj = pessoasJuridicas.Count;

                for (int i = 0; i < Math.Max(countPf, countPj); i++)
                {
                    if (i < countPf && pessoasFisicas[i].Id == cliente.Id)
                    {
                        cliente = pessoasFisicas[i];
                        break;
                    }

                    if (i < countPj && pessoasJuridicas[i].Id == cliente.Id)
                    {
                        cliente = pessoasJuridicas[i];
                        break;
                    }
                }


            }
            else
            {
                CidadeSelecionada = null;
                EstadoSelecionado = null;
                Cidades = null;
            }


            SavePessoaFisica = new SavePessoaFisicaCommand();
            SavePessoaJuridica = new SavePessoaJuridicaCommand();


            PessoaFisica = new PessoaFisica();
            PessoaJuridica = new PessoaJuridica();

            if (cliente is PessoaFisica pf)
            {
                PessoaFisica = pf;
                //ClientePessoaFisica?.Invoke();
            }

            else if (cliente is PessoaJuridica pj)
            {
                PessoaJuridica = pj;
                //ClientePessoaJuridica?.Invoke();
            }

        }


        private void SetCidades(Estado e)
        {
            if (e is not null)
            {
                Cidades = e.Cidades;
                if(Cidades is not null && Cidades.Count > 0)
                {
                    CidadeSelecionada = Cidades.First();
                    TextoCidade = CidadeSelecionada.Nome;
                }
                
            }

            else
            {
                CidadeSelecionada = null;
                TextoCidade = null;
                Cidades = null;
            }
        }

        private void SetTextoCidade(string cidade)
        {
            if(String.IsNullOrWhiteSpace(cidade))
            {
                EstadoSelecionado = null;
            }
        }


        private bool SearchCliente(Cliente c)
        {
            if (c is PessoaFisica pf)
            {
                PList<PessoaFisica> pessoas = DAO.All<PessoaFisica>();
                foreach (PessoaFisica p in pessoas)
                {
                    if (p.CPF == pf.CPF && p.Id != pf.Id)
                    {
                        return true;
                    }
                }
            }

            else if (c is PessoaJuridica pj)
            {
                PList<PessoaJuridica> pessoas = DAO.All<PessoaJuridica>();
                foreach (PessoaJuridica p in pessoas)
                {
                    if (p.CNPJ == pj.CNPJ && p.Id != pj.Id)
                    {
                        return true;
                    }
                }
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


            String digitos = digito1.ToString() + digito2.ToString();

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
       


        private void SalvarCliente<T>(T clienteParameter) where T : Cliente
        {
            if (cliente is null)
            {

                PList<Cliente> clientes = DAO.All<Cliente>();

                if (clientes.Count == 0)
                {
                    cliente = new Cliente()
                    {
                        Id = 1
                    };
                }

                else
                {
                    Cliente c2 = clientes.Last();

                    cliente = new Cliente()
                    {
                        Id = c2.Id + 1
                    };
                }

                if (clienteParameter is PessoaFisica pf)
                {

                    if (CidadeSelecionada is not null)
                    {
                        pf.Cidade = CidadeSelecionada;
                    }

                    if (pf.CPF is null || pf.CPF.Equals("___.___.___-__"))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF não pode ser vazio");
                    }

                    pf.CPF = pf.CPF.Replace(".", "").Replace("-", "");

                    if (pf.CPF.Contains('_'))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF incompleto");
                    }

                    if(!CheckDigitoVerificadorCPF(pf.CPF))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF Inválido");
                    }

                    if (SearchCliente(pf))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF já registrado");
                    }

                    pf.Id = cliente.Id;
                    pf.Save();

                }

                else if (clienteParameter is PessoaJuridica pj)
                {
                    if (CidadeSelecionada is not null)
                    {
                        pj.Cidade = CidadeSelecionada;
                    }

                    if (pj.CNPJ is null || pj.CNPJ.Equals("__.___.___/____-__"))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ não pode ser vazio");
                    }


                    pj.CNPJ = pj.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");

                    if (pj.CNPJ.Contains('_'))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ incompleto");
                    }

                    if(!CheckDigitoVerificadorCNPJ(pj.CNPJ))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ Inválido");
                    }

                    if (SearchCliente(pj))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ já registrado");
                    }

                    pj.Id = cliente.Id;

                    pj.Save();
                }

            }

            else
            {
                if (clienteParameter is PessoaFisica pf)
                {
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

                    if (pf.CPF.Length == 14)
                    {
                        pf.CPF = pf.CPF.Replace(".", "").Replace("-", "");
                    }

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

                    if (cliente.Id == pf.Id)
                    {
                        string nome = pf.Nome;
                        string endereco = pf.Endereco;

                        NullText(ref nome);
                        NullText(ref endereco);

                        pf.Nome = nome;
                        pf.Endereco = endereco;

                        pf.Save();
                    }

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
                        cliente = null;
                        throw new InvalidOperationException("CNPJ Inválido");
                    }

                    if (SearchCliente(pj))
                    {
                        throw new InvalidOperationException("CNPJ já registrado");
                    }

                    if (cliente.Id == pj.Id)
                    {
                        string nome = pj.Nome;
                        string endereco = pj.Endereco;
                        string razaoSocial = pj.RazaoSocial;

                        NullText(ref nome);
                        NullText(ref endereco);
                        NullText(ref razaoSocial);

                        pj.Nome = nome;
                        pj.Endereco = endereco;
                        pj.RazaoSocial = razaoSocial;

                        pj.Save();
                    }
                }

            }
        }



        public void SalvarPessoaFisica()
        {
            try
            {
                SalvarCliente(PessoaFisica);
                SaveCliente?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }

        }

        public void SalvarPessoaJuridica()
        {
            try
            {
                SalvarCliente(PessoaJuridica);
                SaveCliente?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }




    public class SavePessoaFisicaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            CrudClienteViewModel vm = (CrudClienteViewModel)parameter;

            vm.SalvarPessoaFisica();
        }
    }

    public class SavePessoaJuridicaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            CrudClienteViewModel vm = (CrudClienteViewModel)parameter;

            vm.SalvarPessoaJuridica();
        }
    }




    public interface IClienteSave
    {
        public Action SaveCliente { get; set; }
        public Action ClientePessoaFisica { get; set; }
        public Action ClientePessoaJuridica { get; set; }
    }


}

