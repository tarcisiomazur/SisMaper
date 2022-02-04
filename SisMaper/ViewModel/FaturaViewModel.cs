using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;

namespace SisMaper.ViewModel
{
    public class FaturaViewModel : BaseViewModel
    {
        public Fatura Fatura { get; set; }

        public PList<Cliente> Clientes { get; private set; }
        //public List<object> Clientes { get; private set; }


        private ListarClientes _clienteSelecionado;

        public ListarClientes ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set
            {
                //if (ClienteSelecionado is PessoaFisica || ClienteSelecionado is PessoaJuridica) isClienteSelecionadoCorrect = false;

                SetField(ref _clienteSelecionado, value);
                //SetCliente(value);
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


        
        

        public Action ChangeCliente { get; set; }

        public NovaParcelaCommand NovaParcela { get; private set; }
        public EditarParcelaCommand EditarParcela { get; private set; }
        public ExcluirParcelaCommand ExcluirParcela { get; private set; }
        public VerClienteCommand VerCliente { get; private set; }


        public Action<ParcelaViewModel>? OpenCrudParcela { get; set; }
        public Action<CrudClienteViewModel,bool>? OpenViewCliente { get; set; }
        public Action? FaturaChanged { get; set; }
        public Action? FaturaSaved { get; set; }



        public FaturaViewModel(long faturaId)
        {
            VerCliente = new VerClienteCommand();

            //ChangeCliente = ClienteChange;

            NovaParcela = new NovaParcelaCommand();
            EditarParcela = new EditarParcelaCommand();
            ExcluirParcela = new ExcluirParcelaCommand();

            //Clientes = DAO.FindWhereQuery<PessoaFisica>("Cliente_Id > 0");


            Clientes = DAO.All<Cliente>();


            ClienteSelecionado = null;

            Fatura = DAO.Load<Fatura>(faturaId);


            Salvar = new SalvarFaturaCommand();
            AlterarFatura = new AlterarStatusFaturaCommand();


            if (Fatura is not null)
            {
                IsFaturaAberta = (Fatura.Status == Fatura.Fatura_Status.Aberta);

                Fatura.Parcelas.Load();
                Fatura.Pedidos.Load();

                foreach(Pedido p in Fatura.Pedidos)
                {
                    p.Cliente?.Load();
                }

                if (Fatura.Cliente == null) return;

                ClienteSelecionado = View.Execute<ListarClientes>().Find(cliente => cliente.Id == Fatura.Cliente.Id);

                /*
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
                */

            }

        }

        /*
        private void SetCliente(Cliente c)
        {
            if (!isClienteSelecionadoCorrect)
            {
                PList<PessoaFisica> pessoasFisicas = DAO.All<PessoaFisica>();
                PList<PessoaJuridica> pessoasJuridicas = DAO.All<PessoaJuridica>();

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

        */





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
                return decimal.Equals(Fatura.ValorPago, Fatura.ValorTotal);
            }


            //teste de usuário -> se for administrador retorna true
            return true;

        }

        public void OpenCliente() => OpenViewCliente?.Invoke(new CrudClienteViewModel(ClienteSelecionado), ClienteSelecionado.Tipo == ListarClientes.Pessoa.Fisica);
        
        public void OpenCrudNovaParcela() => OpenCrudParcela?.Invoke(new ParcelaViewModel(null, Fatura));
        public void OpenCrudEditarParcela() => OpenCrudParcela?.Invoke(new ParcelaViewModel(ParcelaSelecionada, Fatura));

        public void SalvarFatura() => FaturaSaved?.Invoke();


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

    public class ExcluirParcelaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            return vm.ParcelaSelecionada is not null;
        }
        public override void Execute(object parameter)
        {
            FaturaViewModel vm = (FaturaViewModel)parameter;
            //vm.OpenCrudEditarParcela();
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


}