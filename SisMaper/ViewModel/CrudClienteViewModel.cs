using MahApps.Metro.Controls.Dialogs;
using Persistence;
using RestSharp;
using SisMaper.API.CnpjWs;
using SisMaper.API.ViaCEP;
using SisMaper.Models;
using SisMaper.Models.Views;
using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

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


        private CepResponse enderecoConfirmado; // informações de endereço depois de usar a api

        private CnpjResponse? cnpjConfirmado;

        public SimpleCommand SalvarPessoaFisicaCmd => new(SalvarPessoaFisica);
        public SimpleCommand SalvarPessoaJuridicaCmd => new(SalvarPessoaJuridica);

        public SimpleCommand ConsultaCpfCmd => new( ConsultaCPF );
        public SimpleCommand ConsultaCnpjCmd => new( ConsultaCNPJ );

        public SimpleCommand PessoaFisicaConsultaCepCmd => new(() => ConsultaCEP(PessoaFisica));
        public SimpleCommand PessoaJuridicaConsultaCepCmd => new(() => ConsultaCEP(PessoaJuridica));


        public Action? ClienteSaved { get; set; }


        public CrudClienteViewModel(ListarClientes? clienteSelecionado)
        {

            cliente = clienteSelecionado;

            Estados = DAO.All<Estado>();

            if (cliente is not null && Estados.Any())
            {
                if (cliente.Tipo == ListarClientes.Pessoa.Fisica)
                {
                    PessoaFisica = DAO.Load<PessoaFisica>(cliente.Id);

                    if (PessoaFisica?.Cidade is not null)
                    {
                        EstadoSelecionado = Estados.Where(e => e.Id == PessoaFisica.Cidade.Estado.Id).FirstOrDefault();
                        CidadeSelecionada = Cidades.Where(c => c.Id == PessoaFisica.Cidade.Id).FirstOrDefault();
                    }

                }
                else
                {
                    PessoaJuridica = DAO.Load<PessoaJuridica>(cliente.Id);
                    if (PessoaJuridica?.Cidade is not null)
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
                if(Cidades is not null && Cidades.Any())
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

        private void CheckCPF(PessoaFisica pf)
        {
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

            if (SearchCliente(PessoaFisica))
            {
                throw new InvalidOperationException("CPF já registrado");
            }   
        }

        private void CheckCNPJ()
        {
            if (PessoaJuridica.CNPJ is null || PessoaJuridica.CNPJ.Equals("__.___.___/____-__"))
            {
                throw new InvalidOperationException("CNPJ não pode ser vazio");
            }

            if (PessoaJuridica.CNPJ.Length == 18)
            {
                PessoaJuridica.CNPJ = PessoaJuridica.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
            }

            if (PessoaJuridica.CNPJ.Contains('_'))
            {
                throw new InvalidOperationException("CNPJ incompleto");
            }

            if (!CheckDigitoVerificadorCNPJ(PessoaJuridica.CNPJ))
            {
                throw new InvalidOperationException("CNPJ Inválido");
            }

            if (SearchCliente(PessoaJuridica))
            {
                throw new InvalidOperationException("CNPJ já registrado");
            }
        }

        private void CheckCEP(ref string? cep)
        {
            if (cep is null || cep.Equals("__.___-___"))
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
         
        private void ConsultaCEP(Cliente cliente)
        {
            enderecoConfirmado = null;
            
            try
            {
                string? cep = cliente.CEP;
                CheckCEP(ref cep);
                cliente.CEP = cep;

                enderecoConfirmado = ViaCepConnector.ConsultarCEP(cep);

                EstadoSelecionado = Estados.Where(e => e.UF == enderecoConfirmado?.UF).FirstOrDefault();
                CidadeSelecionada = Cidades?.Where(c => c.Nome == enderecoConfirmado?.Cidade).FirstOrDefault();
                cliente.Bairro = enderecoConfirmado.Bairro;
                cliente.Endereco = enderecoConfirmado.Endereco;
                if(!string.IsNullOrWhiteSpace(enderecoConfirmado.Numero)) cliente.Numero = enderecoConfirmado.Numero;
            }
            catch (Exception ex)
            {
                OnShowMessage("Erro ao consultar CEP", ex.Message);
            }
        }

        // retorna true se o endereço ta correspondente ao cep ou se o usuário ignorar a confirmação
        private bool ComparaEndereco(Cliente cliente)
        {
            //indica se não tem nenhuma informação de endereço
            bool isEnderecoEmpty = !(CidadeSelecionada is not null ||
                                    !string.IsNullOrWhiteSpace(cliente.Endereco) ||
                                    !string.IsNullOrWhiteSpace(cliente.Bairro) ||
                                    !string.IsNullOrWhiteSpace(cliente.Numero));

            //caso o cep não foi confirmado e tem alguma informação de endereço (pode não ter nenhum cep também)
            //ou se o cep confirmado é diferente do cep atual
            //ou se tem um cep e não foi confirmado (sem outras informações de endereço)
            if ( (!string.IsNullOrWhiteSpace(cliente.CEP) && enderecoConfirmado is null) || 
                    enderecoConfirmado is not null && enderecoConfirmado.CEP != cliente.CEP || 
                        enderecoConfirmado is null && !isEnderecoEmpty)
            {
                MessageDialogResult resultado = OnShowMessage("CEP Não Confirmado", "O CEP não foi confirmado. Continuar?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });
                return resultado == MessageDialogResult.Affirmative; 
            }

            
            //quando o alguma informação não corresponde ao endereço obtido com a consulta do cep
            if(enderecoConfirmado is not null && enderecoConfirmado.CEP == cliente.CEP)
            {
                string? uf = enderecoConfirmado.UF;
                string? cidade = enderecoConfirmado.Cidade;
                string? end = enderecoConfirmado.Endereco;
                string? bairro = enderecoConfirmado.Bairro;

                bool ufDiferente = (EstadoSelecionado?.UF != uf);
                bool cidadeDiferente = (cliente.Cidade?.Nome != cidade);
                bool enderecoDiferente = ( !string.IsNullOrWhiteSpace(end) && end != cliente.Endereco );
                bool bairroDiferente = ( !string.IsNullOrWhiteSpace(bairro) && bairro != cliente.Bairro );

                if(ufDiferente || cidadeDiferente || bairroDiferente || enderecoDiferente)
                {
                    MessageDialogResult resultado = OnShowMessage("Endereço Não Correspondente", "As informações de endereço não correspondem ao CEP. Continuar?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });
                    return resultado == MessageDialogResult.Affirmative;
                }
            }

            return true;
        }


        private void ConsultaCNPJ()
        {
            try
            {
                cnpjConfirmado = null;
                CheckCNPJ();
                cnpjConfirmado = CnpjWs.ConsultarCNPJ(PessoaJuridica.CNPJ);

                PessoaJuridica.Nome = cnpjConfirmado.Nome;
                PessoaJuridica.RazaoSocial = cnpjConfirmado.RazaoSocial;
                PessoaJuridica.InscricaoEstadual = cnpjConfirmado.InscricaoEstadual;

                //Informa caso a situação cadastral não seja ativa (suspensa/inapta/baixada/nula)
                if(cnpjConfirmado.SituacaoCadastral != "Ativa")
                {
                    OnShowMessage("Informação Sobre Situação Cadastral", $"Situação cadastral {cnpjConfirmado.SituacaoCadastral}");
                }

                PessoaJuridica.CEP = cnpjConfirmado.Cep;
                ConsultaCEP(PessoaJuridica);
            }
            catch (Exception ex)
            {
                OnShowMessage("Erro ao consultar CNPJ", ex.Message);
            }
        }


        private bool ComparaCNPJ()
        {
            //indica se não tem nenhuma informação da empresa (fora o endereço)
            bool isInformacoesEmpty = !(!string.IsNullOrWhiteSpace(PessoaJuridica.Nome) || 
                                        !string.IsNullOrWhiteSpace(PessoaJuridica.RazaoSocial) || 
                                        !string.IsNullOrWhiteSpace(PessoaJuridica.InscricaoEstadual) || 
                                        !string.IsNullOrWhiteSpace(PessoaJuridica.CEP));

            if (cnpjConfirmado is null || 
                cnpjConfirmado is not null && cnpjConfirmado.CNPJ != PessoaJuridica.CNPJ || 
                    cnpjConfirmado is null && !isInformacoesEmpty)
            {
                MessageDialogResult resultado = OnShowMessage("CNPJ Não Confirmado", "O CNPJ não foi confirmado. Continuar?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });
                return resultado == MessageDialogResult.Affirmative;
            }


            //quando o alguma informação não corresponde as informações obtidas com a consulta do cnpj
            if (cnpjConfirmado is not null && cnpjConfirmado.CNPJ == PessoaJuridica.CNPJ)
            {
                string? nome = cnpjConfirmado.Nome;
                string? razaoSocial = cnpjConfirmado.RazaoSocial;
                string? inscricaoEstadual = cnpjConfirmado.InscricaoEstadual;
                string? cep = cnpjConfirmado.Cep;

                bool nomeDiferente = !string.IsNullOrWhiteSpace(nome) && nome != PessoaJuridica.Nome;
                bool razaoSocialDiferente = !string.IsNullOrWhiteSpace(razaoSocial) && razaoSocial != PessoaJuridica.RazaoSocial;
                bool inscricaoEstadualDiferente = !string.IsNullOrWhiteSpace(inscricaoEstadual) && inscricaoEstadual != PessoaJuridica.InscricaoEstadual;
                bool cepDiferente = !string.IsNullOrWhiteSpace(cep) && cep != PessoaJuridica.CEP;

                if (nomeDiferente || razaoSocialDiferente || inscricaoEstadualDiferente || cepDiferente)
                {
                    MessageDialogResult resultado = OnShowMessage("Informações Não Correspondentes", "As informações de não correspondem ao CNPJ. Continuar?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });
                    return resultado == MessageDialogResult.Affirmative;
                }
            }

            return true;
        }


        private void ConsultaCPF()
        {
            IWebDriver driver = null;
            try
            {
                CheckCPF(PessoaFisica);

                const string url = "https://cpf.ltsolucoes.com/";

                var request = new RestRequest(url, Method.GET);
                IRestResponse response = new RestClient().Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new Exception("URL utilizada não existe mais");


                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--incognito");
                options.AddArgument("--headless");
                driver = new ChromeDriver(options);

                driver.Navigate().GoToUrl(url);
                driver.FindElement(By.CssSelector("#numero")).SendKeys(PessoaFisica.CPF);
                driver.FindElement(By.CssSelector("#consultar")).Click();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(_ => driver.FindElements(By.CssSelector("#corpo > tbody > tr:nth-child(2) > td > div > p")).Any());
                
                IWebElement e = driver.FindElement(By.CssSelector("#corpo > tbody > tr:nth-child(2) > td > div > p"));
                string[] dados = e.Text.Split('\n');

                PessoaFisica.Nome = dados[1].Split(':')[1].Trim();

                driver.Close();
                driver.Quit();

            }
            catch (Exception ex)
            {
                if (driver is not null)
                {
                    driver.Close();
                    driver.Quit();
                }

                if(ex is WebDriverException)
                {
                    if(ex is WebDriverTimeoutException)
                    {
                        OnShowMessage("Erro ao consultar CPF", "CPF não encontrado");
                        return;
                    }

                    OnShowMessage("Erro ao consultar CPF", "Verifique seu acesso a Internet");
                    return;
                }

                OnShowMessage("Erro ao consultar CPF", ex.Message);
                return;
            }
        }


        // da ruim pra salvar se alguma consulta retornar um valor maior que a Length do atributo field
        // pq mesmo q a textbox tenha maxLength, é só na digitação
        private void SalvarCliente(Cliente clienteParameter)
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

                CheckCPF(pf);

                if (string.IsNullOrWhiteSpace(pf.Nome)) throw new Exception("Nome não pode ser vazio");

                string cep = pf.CEP;
                CheckCEP(ref cep);
                pf.CEP = cep;

                if (!ComparaEndereco(pf)) return;

                string? endereco = pf.Endereco;
                string? bairro = pf.Bairro;
                string? numero = pf.Numero;

                NullText(ref endereco);
                NullText(ref bairro);
                NullText(ref numero);

                pf.Endereco = endereco;
                pf.Bairro = bairro;
                pf.Numero = numero;

                pf.Save();
                ClienteSaved?.Invoke();

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


                CheckCNPJ();

                if (string.IsNullOrWhiteSpace(pj.Nome)) throw new Exception("Nome não pode ser vazio");

                string? cep = pj.CEP;
                CheckCEP(ref cep);
                pj.CEP = cep;


                if (!ComparaCNPJ() || !ComparaEndereco(pj)) return;


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
                ClienteSaved?.Invoke();


            }
        }



        private void SalvarPessoaFisica()
        {

            try
            {
                SalvarCliente(PessoaFisica);
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
            }
            catch (Exception ex)
            {
                OnShowMessage("Erro ao salvar Cliente", "Erro: " + ex.Message);
            }
        }
    }


}

