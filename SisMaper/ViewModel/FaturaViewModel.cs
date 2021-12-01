using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class FaturaViewModel : BaseViewModel, ICloseWindow, IFatura
    {
        public Fatura Fatura { get; set; }

        public PList<Cliente> Clientes { get; private set; }
        //public List<object> Clientes { get; private set; }


        private Cliente _clienteSelecionado;

        public Cliente ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set
            {
                if (ClienteSelecionado is PessoaFisica || ClienteSelecionado is PessoaJuridica) isClienteSelecionadoCorrect = false;

                SetField(ref _clienteSelecionado, value);
                SetCliente(value);
            }
        }





        private bool isClienteSelecionadoCorrect;



        private Parcela _parcelaSelecionada;

        public Parcela ParcelaSelecionada
        {
            get { return _parcelaSelecionada; }
            set { SetField(ref _parcelaSelecionada, value); }
        }

        public bool IsFaturaAberta { get; private set; }

        public SalvarFaturaCommand Salvar { get; private set; }
        public AlterarStatusFaturaCommand AlterarFatura { get; private set; }


        public Action Close { get; set; }


        public Action FaturaChanged { get; set; }
        public Action ClienteChangedToPessoaFisica { get; set; }
        public Action ClienteChangedToPessoaJuridica { get; set; }
        public Action OpenNovaParcela { get; set; }
        public Action OpenEditarParcela { get; set; }

        public Action ChangeCliente { get; set; }

        public NovaParcelaCommand NovaParcela { get; private set; }


        private bool isPessoaFisica;

        public Action<bool> OpenClienteView { get; set; }

        public VerClienteCommand VerCliente { get; private set; }

        public FaturaViewModel(object? faturaSelecionada)
        {
            VerCliente = new VerClienteCommand();

            //ChangeCliente = ClienteChange;

            NovaParcela = new NovaParcelaCommand();

            //Clientes = DAO.FindWhereQuery<PessoaFisica>("Cliente_Id > 0");


            Clientes = DAO.FindWhereQuery<Cliente>("Id > 0");


            ClienteSelecionado = null;

            Fatura = (Fatura)faturaSelecionada;


            Salvar = new SalvarFaturaCommand();
            AlterarFatura = new AlterarStatusFaturaCommand();


            if (Fatura is not null)
            {
                IsFaturaAberta = (Fatura.Status == Fatura.Fatura_Status.Aberta) ? true : false;

                Fatura.Parcelas.Load();
                Fatura.Pedidos.Load();

                foreach(Pedido p in Fatura.Pedidos)
                {
                    p.Cliente?.Load();
                }


                PessoaFisica? c1 = DAO.Load<PessoaFisica>(Fatura.Cliente.Id);

                if(c1 is not null)
                {
                    isPessoaFisica = true;
                }
                else
                {
                    PessoaJuridica? c2 = DAO.Load<PessoaJuridica>(Fatura.Cliente.Id);

                    if(c2 is not null)
                    {
                        isPessoaFisica = false;
                    }
                }


                //Fatura.Cliente.Load();

                //ClienteSelecionado = Fatura.Cliente;
                //ClienteSelecionado = DAO.Load<Cliente>(Fatura.Cliente.Id);

                //ClienteSelecionado = Clientes[0];

            }

        }


        /*
        private void ClienteChange()
        {

            if (Fatura.Cliente.Load())
            {
                foreach (Cliente c in Clientes)
                {
                    if (c.Id == Fatura.Cliente.Id)
                    {
                        ClienteSelecionado = c;
                        //isClienteSelecionadoCorrect = false;
                        //SetCliente(c);
                        return;
                    }
                }
            }
            
        }*/


        private void SetCliente(Cliente c)
        {
            if (!isClienteSelecionadoCorrect)
            {
                PList<PessoaFisica> pessoasFisicas = DAO.FindWhereQuery<PessoaFisica>("Cliente_Id > 0");
                PList<PessoaJuridica> pessoasJuridicas = DAO.FindWhereQuery<PessoaJuridica>("Cliente_Id > 0");

                int countPf = pessoasFisicas.Count;
                int countPj = pessoasJuridicas.Count;

                for (int i = 0; i < Math.Max(countPf, countPj); i++)
                {
                    if (i < countPf && pessoasFisicas[i].Id == c.Id)
                    {
                        isClienteSelecionadoCorrect = true;
                        ClienteSelecionado = pessoasFisicas[i];
                        ClienteChangedToPessoaFisica?.Invoke();
                        return;
                    }

                    if (i < countPj && pessoasJuridicas[i].Id == c.Id)
                    {
                        isClienteSelecionadoCorrect = true;
                        ClienteSelecionado = pessoasJuridicas[i];
                        ClienteChangedToPessoaJuridica?.Invoke();
                        return;
                    }
                }
            }

        }




        public void SalvarFatura()
        {
            Close?.Invoke();
        }


        public void OpenCrudNovaParcela() => OpenNovaParcela?.Invoke();

        public void OpenCrudEditarParcela() => OpenEditarParcela?.Invoke();


        public void AlterarStatusFatura()
        {
            if (Fatura.Status == Fatura.Fatura_Status.Aberta)
            {
                Fatura.Status = Fatura.Fatura_Status.Fechada;
            }

            else if (Fatura.Status == Fatura.Fatura_Status.Fechada)
            {
                Fatura.Status = Fatura.Fatura_Status.Aberta;
            }

            IsFaturaAberta = !IsFaturaAberta;
            FaturaChanged?.Invoke();
        }

        public bool CanChangeStatus()
        {
            if (Fatura.Status == Fatura.Fatura_Status.Aberta)
            {
                //return decimal.Equals(Fatura.ValorPago, Fatura.ValorTotal);
            }


            //teste de usuário -> se for administrador retorna true
            return true;


        }

        public void OpenCliente() => OpenClienteView?.Invoke(isPessoaFisica);
    }


    public class SalvarFaturaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            vm.SalvarFatura();
        }
    }

    public class AlterarStatusFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            return vm.CanChangeStatus();
        }

        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            vm.AlterarStatusFatura();
        }
    }


    public class NovaParcelaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            vm.OpenCrudNovaParcela();
        }
    }

    public class EditarParcelaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            return vm.ParcelaSelecionada is not null;
        }
        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            vm.OpenCrudEditarParcela();
        }
    }


    public class VerClienteCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel) parameter;
            vm.OpenCliente();
        }
    }




    public interface IFatura
    {
        public Action FaturaChanged { get; set; }
        public Action ClienteChangedToPessoaFisica { get; set; }
        public Action ClienteChangedToPessoaJuridica { get; set; }
        public Action OpenNovaParcela { get; set; }
        public Action OpenEditarParcela { get; set; }

        public Action ChangeCliente { get; set; }

        public Action<bool> OpenClienteView { get; set; }
    }
}