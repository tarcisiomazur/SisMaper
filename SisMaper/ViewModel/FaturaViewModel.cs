using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using SisMaper.Views;

namespace SisMaper.ViewModel
{
    public class FaturaViewModel : BaseViewModel
    {
        public Fatura Fatura { get; set; }

        public PList<Cliente> Clientes { get; private set; }

        private ListarClientes? _clienteSelecionado;

        public ListarClientes? ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set
            {
                SetField(ref _clienteSelecionado, value);
            }
        }



        private Parcela _parcelaSelecionada;

        public Parcela ParcelaSelecionada
        {
            get { return _parcelaSelecionada; }
            set { SetField(ref _parcelaSelecionada, value); }
        }

        public bool IsFaturaAberta { get; private set; }



        public int NumeroParcelas { get; set; } = 1;
        private PList<Parcela> parcelas = new();

        public int[] DiasPagamento { get; private set; } = { 10, 15, 20 };
        public int DiaPagamentoSelecionado { get; set; }


        public SimpleCommand SalvarFaturaCmd => new(() => FaturaSaved?.Invoke());
        public SimpleCommand VerClienteCmd => new(() => OpenViewCliente?.Invoke(new CrudClienteViewModel(ClienteSelecionado), ClienteSelecionado.Tipo == ListarClientes.Pessoa.Fisica));
        public SimpleCommand NovaParcelaCmd => new(() => OpenCrudParcela?.Invoke(new ParcelaViewModel(null, Fatura)));
        public SimpleCommand EditarParcelaCmd => new( () => OpenCrudParcela?.Invoke(new ParcelaViewModel(ParcelaSelecionada, Fatura)), () => ParcelaSelecionada is not null );
        public SimpleCommand AlterarStatusFaturaCmd => new(AlterarStatusFatura, CanChangeStatus);
        public SimpleCommand ExcluirParcelaCmd => new(ExcluirParcela, () => ParcelaSelecionada is not null);
        public SimpleCommand GerarParcelasCmd => new(GerarParcelas);

        public SimpleCommand ConfirmarGerarParcelasCmd => new(ConfirmarGerarParcelas);


        public Action<ParcelaViewModel>? OpenCrudParcela { get; set; }
        public Action<CrudClienteViewModel,bool>? OpenViewCliente { get; set; }
        public Action? FaturaChanged { get; set; }
        public Action? FaturaSaved { get; set; }
        public Action ChangeCliente { get; set; }
        public Action? ParcelasGeradas { get; set; }
        public Action<FaturaViewModel>? OpenGerarParcelas { get; set; }

        private long faturaId;



        public FaturaViewModel(long faturaId)
        {

            Clientes = DAO.All<Cliente>();


            ClienteSelecionado = null;

            this.faturaId = faturaId;

            ResetFatura();

            

            if (Fatura is not null)
            {
                IsFaturaAberta = (Fatura.Status == Fatura.Fatura_Status.Aberta);

                foreach(Pedido p in Fatura.Pedidos)
                {
                    p.Cliente?.Load();
                }

                if (Fatura.Cliente == null) return;

                ClienteSelecionado = View.Execute<ListarClientes>().Find(cliente => cliente.Id == Fatura.Cliente.Id);

            }

            DiaPagamentoSelecionado = DiasPagamento[0];

        }


        public void ResetFatura()
        {
            Fatura = DAO.Load<Fatura>(faturaId);
            Fatura?.Pedidos.Load();
            Fatura?.Parcelas.Load();
        }


        private void AlterarStatusFatura()
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

        private bool CanChangeStatus()
        {
            if (Fatura.Status == Fatura.Fatura_Status.Aberta)
            {
                return decimal.Equals(Fatura.ValorPago, Fatura.ValorTotal);
            }


            //teste de usuário -> se for administrador retorna true
            return true;

        }

        private void ExcluirParcela()
        {
            MessageDialogResult confirmacao = OnShowMessage("Excluir Parcela", "Deseja Excluir parcela selecionada?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

            if(confirmacao == MessageDialogResult.Affirmative)
            {
                if(ParcelaSelecionada.Status == Parcela.Status_Parcela.Pago)
                {
                    OnShowMessage("Erro", "A parcela foi paga, não pode ser excluida");
                    return;
                }

                try
                {
                    ParcelaSelecionada.Delete();
                    ResetFatura();
                }
                catch(Exception ex)
                {
                    OnShowMessage("Erro", ex.Message);
                }
            }
        }

        private void GerarParcelas()
        {
            //if(Fatura.Parcelas?.get)

            foreach(Parcela p in Fatura.Parcelas)
            {
                if(p.Status == Parcela.Status_Parcela.Pago)
                {
                    OnShowMessage("Gerar Parcelas", "Para gerar as parcelas, a fatura não pode ter nenhuma parcela paga");
                    return;
                }
            }

            OpenGerarParcelas?.Invoke(this);
            

            if(parcelas.Count > 0)
            {
                foreach(Parcela p in parcelas)
                {
                    p.Fatura = Fatura;
                    p.Save();
                }
                ResetFatura();
            }

        }

        private void ConfirmarGerarParcelas()
        {
            if (Fatura.Parcelas.DeleteAll())
            {
                parcelas = CalculaParcelas(Fatura.ValorTotal, NumeroParcelas, DiaPagamentoSelecionado);
                ParcelasGeradas?.Invoke();
            }
        }


        private PList<Parcela> CalculaParcelas(decimal valorTotal, int qtdParcelas, int diaPagamento)
        {
            decimal valorParcelaArredondado = decimal.Round(valorTotal / qtdParcelas, 2);
            decimal diferenca = (valorTotal / qtdParcelas - valorParcelaArredondado) * qtdParcelas;

            PList<Parcela> parcelas = new();

            DateTime dataVencimento = new DateTime(DateTime.Today.Year, DateTime.Today.Month, diaPagamento);

            for (int i = 0; i < qtdParcelas; i++)
            {
                //se for a ultima parcela, pega a diferença
                decimal valorParcela = (i + 1 == qtdParcelas)? (valorParcelaArredondado + diferenca) : valorParcelaArredondado;

                valorParcela = decimal.Round(valorParcela, 2);
                

                parcelas.Add( new Parcela() { Indice = i+1, DataVencimento = dataVencimento, Valor = valorParcela} );

                dataVencimento = dataVencimento.AddMonths(1);

            }

            return parcelas;

        }


    }

}