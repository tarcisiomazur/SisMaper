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

            /*
            if(clienteSelecionado is null)
            {
                PList<Cliente> clientes = DAO.FindWhereQuery<Cliente>("Id > 0");

                if(clientes.Count == 0)
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

                //PessoaFisica = (PessoaFisica)cliente;
                //PessoaJuridica = (PessoaJuridica)cliente;


            }
            */

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


        private void SetCliente<T>(T clienteParameter) where T : Cliente
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

                    cliente.Save();

                    pf.Id = cliente.Id;

                    pf.Save();
                }

                else if(clienteParameter is PessoaJuridica pj)
                {
                    if (CidadeSelecionada is not null)
                    {
                        pj.Cidade = CidadeSelecionada;
                    }

                    cliente.Nome = pj.Nome;
                    cliente.Cidade = pj.Cidade;
                    cliente.Endereco = pj.Endereco;

                    cliente.Save();

                    pj.Id = cliente.Id;

                    pj.Save();
                }


            }

            else
            {
                clienteParameter.Save();
            }
        }

        public void SalvarPessoaFisica()
        {

            try
            {
                SetCliente(PessoaFisica);
                SaveCliente?.Invoke();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }


            /*
            try
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


                    if (CidadeSelecionada is not null)
                    {
                        PessoaFisica.Cidade = CidadeSelecionada;
                    }



                    cliente.Nome = PessoaFisica.Nome;
                    cliente.Cidade = PessoaFisica.Cidade;
                    cliente.Endereco = PessoaFisica.Endereco;

                    cliente.Save();

                    PessoaFisica.Id = cliente.Id;

                    PessoaFisica.Save();


                }

                else
                {
                    PessoaFisica.Save();
                }

                SaveCliente?.Invoke();
            }

            catch(Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }

            */
        }

        public void SalvarPessoaJuridica()
        {
            try
            {
                SetCliente(PessoaJuridica);
                SaveCliente?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }

            /*
            try
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


                    if (CidadeSelecionada is not null)
                    {
                        PessoaJuridica.Cidade = CidadeSelecionada;
                    }


                    cliente.Nome = PessoaJuridica.Nome;
                    cliente.Cidade = PessoaJuridica.Cidade;
                    cliente.Endereco = PessoaJuridica.Endereco;

                    cliente.Save();

                    PessoaJuridica.Id = cliente.Id;

                    PessoaJuridica.Save();
                }

                else
                {
                    PessoaJuridica.Save();
                }

                SaveCliente?.Invoke();
            }

            catch(Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            */
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
