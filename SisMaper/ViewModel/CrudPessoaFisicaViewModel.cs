using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class CrudPessoaFisicaViewModel : BaseViewModel, IClienteSave
    {
        private Estado _estadoSelecionado;

        public Estado EstadoSelecionado
        {
            get { return _estadoSelecionado; }
            set { SetField(ref _estadoSelecionado, value); }
        }


        private Cidade _cidadeSelecionada;

        public Cidade CidadeSelecionada
        {
            get { return _cidadeSelecionada; }
            set { SetField(ref _cidadeSelecionada, value); }
        }

        public string _textoCPF;
        public string TextoCPF
        {
            get { return _textoCPF; }
            set { SetField(ref _textoCPF, value); }
        }

        public PessoaFisica PessoaFisica { get; set; }
        public PessoaJuridica PessoaJuridica { get; set; }

        private Cliente cliente;


        public PList<Estado> Estados { get; private set; }
        public PList<Cidade> Cidades { get; private set; }

        public SavePessoaFisicaCommand SavePessoaFisica { get; private set; }

        public SavePessoaJuridicaCommand SavePessoaJuridica { get; private set; }

        public Action SaveCliente { get; set; }


        public CrudPessoaFisicaViewModel(object clienteSelecionado)
        {

            cliente = (Cliente)clienteSelecionado;

            SavePessoaFisica = new SavePessoaFisicaCommand();
            SavePessoaJuridica = new SavePessoaJuridicaCommand();


            PessoaFisica = new PessoaFisica();
            PessoaJuridica = new PessoaJuridica();

            if (clienteSelecionado is PessoaFisica pf)
            {
                PessoaFisica = pf;
            }

            else if (clienteSelecionado is PessoaJuridica pj)
            {
                PessoaJuridica = pj;
            }

        }


        private bool SearchCliente(Cliente c)
        {
            

            if(c is PessoaFisica pf)
            {
                PList<PessoaFisica> pessoas = DAO.FindWhereQuery<PessoaFisica>("Cliente_Id > 0");
                foreach(PessoaFisica p in pessoas)
                {
                    if(p.CPF == pf.CPF)
                    {
                        return true;
                    }
                }
            }

            else if (c is PessoaJuridica pj)
            {
                PList<PessoaJuridica> pessoas = DAO.FindWhereQuery<PessoaJuridica>("Cliente_Id > 0");
                foreach (PessoaJuridica p in pessoas)
                {
                    if (p.CNPJ == pj.CNPJ)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private void SalvarCliente<T>(T clienteParameter) where T : Cliente
        {
            if (cliente is null)
            {

                PList<Cliente> clientes = DAO.FindWhereQuery<Cliente>("Id > 0");

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



                    cliente.Nome = pf.Nome;
                    cliente.Cidade = pf.Cidade;
                    cliente.Endereco = pf.Endereco;

                    if (pf.CPF is null || pf.CPF.Equals("___.___.___-__"))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF não pode ser vazio");
                    }

                    pf.CPF = pf.CPF.Remove(3, 1);
                    pf.CPF = pf.CPF.Remove(6, 1);
                    pf.CPF = pf.CPF.Remove(9, 1);

                    if (pf.CPF.Contains('_'))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF incompleto");
                    }

                    if(SearchCliente(pf))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CPF já registrado");
                    }

                    cliente.Save();

                    pf.Id = cliente.Id;
                    pf.Save();

                }

                else if (clienteParameter is PessoaJuridica pj)
                {
                    if (CidadeSelecionada is not null)
                    {
                        pj.Cidade = CidadeSelecionada;
                    }

                    cliente.Nome = pj.Nome;
                    cliente.Cidade = pj.Cidade;
                    cliente.Endereco = pj.Endereco;

                    if (pj.CNPJ is null || pj.CNPJ.Equals("__.___.___/____-__"))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ não pode ser vazio");
                    }

                    pj.CNPJ = pj.CNPJ.Remove(2, 1);
                    pj.CNPJ = pj.CNPJ.Remove(5, 1);
                    pj.CNPJ = pj.CNPJ.Remove(8, 1);
                    pj.CNPJ = pj.CNPJ.Remove(12, 1);

                    if (pj.CNPJ.Contains('_'))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ incompleto");
                    }


                    if(SearchCliente(pj))
                    {
                        cliente = null;
                        throw new InvalidOperationException("CNPJ já registrado");
                    }

                    cliente.Save();

                    pj.Id = cliente.Id;

                    pj.Save();
                }


            }

            else
            {
                if (clienteParameter is PessoaFisica pf)
                {

                    if (pf.CPF is null || pf.CPF.Equals("___.___.___-__"))
                    {
                        throw new InvalidOperationException("CPF não pode ser vazio");
                    }

                    if (pf.CPF.Length == 14)
                    {
                        pf.CPF = pf.CPF.Remove(3, 1);
                        pf.CPF = pf.CPF.Remove(6, 1);
                        pf.CPF = pf.CPF.Remove(9, 1);
                    }

                    if (pf.CPF.Contains('_'))
                    {
                        throw new InvalidOperationException("CPF incompleto");
                    }

                    if (SearchCliente(pf))
                    {
                        throw new InvalidOperationException("CPF já registrado");
                    }

                    if (cliente.Id == pf.Id)
                    {
                        pf.Save();
                    }

                }

                else if(clienteParameter is PessoaJuridica pj)
                {
                    if (pj.CNPJ is null || pj.CNPJ.Equals("__.___.___/____-__"))
                    {
                        throw new InvalidOperationException("CNPJ não pode ser vazio");
                    }

                    if (pj.CNPJ.Length == 18)
                    {
                        pj.CNPJ = pj.CNPJ.Remove(2, 1);
                        pj.CNPJ = pj.CNPJ.Remove(5, 1);
                        pj.CNPJ = pj.CNPJ.Remove(8, 1);
                        pj.CNPJ = pj.CNPJ.Remove(12, 1);
                    }

                    if (pj.CNPJ.Contains('_'))
                    {
                        throw new InvalidOperationException("CNPJ incompleto");
                    }

                    if (SearchCliente(pj))
                    {
                        throw new InvalidOperationException("CNPJ já registrado");
                    }

                    if (cliente.Id == pj.Id)
                    {
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
                MessageBox.Show("Erro: " + ex.Message + " Stack: " + ex.StackTrace);
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
            CrudPessoaFisicaViewModel vm = (CrudPessoaFisicaViewModel)parameter;

            vm.SalvarPessoaFisica();
        }
    }

    public class SavePessoaJuridicaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            CrudPessoaFisicaViewModel vm = (CrudPessoaFisicaViewModel)parameter;

            vm.SalvarPessoaJuridica();
        }
    }




    public interface IClienteSave
    {
        public Action SaveCliente { get; set; }
    }
}

