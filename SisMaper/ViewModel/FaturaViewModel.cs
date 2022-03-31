using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
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

        private ListarClientes? clienteSelecionado;
        public bool FaturaHasCliente => clienteSelecionado != null;

        private Parcela _parcelaSelecionada;

        public Parcela ParcelaSelecionada
        {
            get { return _parcelaSelecionada; }
            set { SetField(ref _parcelaSelecionada, value); }
        }

        public Pedido? PedidoSelecionado { get; set; }

        public bool IsFaturaAberta { get; private set; }

        public decimal ValorTotalParcelas { get; private set; }

        public int NumeroParcelas { get; set; } = 1;
        private PList<Parcela> parcelas = new();

        public int[] DiasPagamento { get; private set; } = { 10, 15, 20 };
        public int DiaPagamentoSelecionado { get; set; }


        public SimpleCommand SalvarFaturaCmd => new(() => FaturaSaved?.Invoke());
        public SimpleCommand VerClienteCmd => new(() => OpenViewCliente?.Invoke(new CrudClienteViewModel(clienteSelecionado), clienteSelecionado.Tipo == ListarClientes.Pessoa.Fisica));
        public SimpleCommand NovaParcelaCmd => new(() => OpenCrudParcela?.Invoke(new ParcelaViewModel(null, Fatura)));
        public SimpleCommand EditarParcelaCmd => new( () => OpenCrudParcela?.Invoke(new ParcelaViewModel(ParcelaSelecionada, Fatura)), () => ParcelaSelecionada is not null );
        public SimpleCommand ExcluirParcelaCmd => new(ExcluirParcela, () => ParcelaSelecionada is not null);
        public SimpleCommand GerarParcelasCmd => new(GerarParcelas);

        public SimpleCommand RemoverPedidoCmd => new(RemoverPedido, _ => PedidoSelecionado is not null);

        public SimpleCommand ConfirmarGerarParcelasCmd => new(ConfirmarGerarParcelas);


        public Action<ParcelaViewModel>? OpenCrudParcela { get; set; }
        public Action<CrudClienteViewModel,bool>? OpenViewCliente { get; set; }
        public Action? FaturaSaved { get; set; }
        public Action ChangeCliente { get; set; }
        public Action? ParcelasGeradas { get; set; }
        public Action<FaturaViewModel>? OpenGerarParcelas { get; set; }

        private long faturaId;


        public FaturaViewModel(long faturaId)
        {

            Clientes = DAO.All<Cliente>();


            clienteSelecionado = null;

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

                clienteSelecionado = View.Execute<ListarClientes>().Find(cliente => cliente.Id == Fatura.Cliente.Id);

            }

            DiaPagamentoSelecionado = DiasPagamento[0];

        }


        public void ResetFatura()
        {
            Fatura = DAO.Load<Fatura>(faturaId);
            Fatura?.Pedidos.Load();
            Fatura?.Parcelas.Load();

            ValorTotalParcelas = 0;
            foreach(Parcela p in Fatura.Parcelas)
            {
                ValorTotalParcelas += p.Valor;
            }

            if(Fatura.ValorPago == Fatura.ValorTotal)
            {
                Fatura.Status = Fatura.Fatura_Status.Fechada;
                IsFaturaAberta = false;
                try { Fatura.Save(); }
                catch { }
            }
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



        private void RemoverPedido()
        {
            MessageDialogResult confirmacao = OnShowMessage("Excluir Produto", "Deseja remover o pedido selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

            if (confirmacao == MessageDialogResult.Negative) return;

            if(Fatura.Pedidos.Count == 1)
            {
                OnShowMessage("Remover Pedido", "O pedido selecionado é o único na fatura. Não pode ser removido");
                return;
            }

            foreach (Parcela p in Fatura.Parcelas)
            {
                if(p.Status == Parcela.Status_Parcela.Pago)
                {
                    OnShowMessage("Remover Pedido", "A fatura já possui parcelas pagas. O pedido não pode ser removido");
                    return;
                }
            }


            try
            {
                Pedido pedidoSelecionado = PedidoSelecionado;
                if (Fatura.Pedidos.Remove(PedidoSelecionado))
                {
                    Fatura.ValorTotal -= pedidoSelecionado.ValorTotal;

                    pedidoSelecionado.Fatura = new Fatura()
                    {
                        Cliente = pedidoSelecionado.Cliente,
                        Data = DateTime.Now,
                        Parcelas = new PList<Parcela>(),
                        Pedidos = new PList<Pedido>(new[] {pedidoSelecionado})
                    };


                    if (pedidoSelecionado.Fatura.Save())
                    {
                        pedidoSelecionado.Save();
                        Fatura.Save();
                        OnShowMessage("Remover Pedido", "Pedido Removido e Refaturado");
                    }
                }
            }
            catch(Exception ex)
            {
                OnShowMessage("Remover Pedido", ex.Message);
            }

            
        }


    }

}