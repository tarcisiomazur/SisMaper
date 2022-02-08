using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class ParcelaViewModel : BaseViewModel
    {
        public bool IsParcelaEditable { get; private set; }

        public Parcela? Parcela { get; set; }
        public Fatura Fatura { get; set; }
        


        public decimal ValorTotal  { get; private set; }

        public PList<Pagamento> Pagamentos { get; private set; }

        public List<Pagamento.EnumTipoPagamento> TiposPagamento { get; private set; }

        private Pagamento.EnumTipoPagamento _tipoPagamentoSelecionado;
        public Pagamento.EnumTipoPagamento TipoPagamentoSelecionado
        {
            get { return _tipoPagamentoSelecionado; }
            set
            {
                foreach(Pagamento p in Pagamentos)
                {
                    if(p.TipoPagamento == value)
                    {
                        Valor = p.ValorPagamento;
                    }
                }
                SetField(ref _tipoPagamentoSelecionado, value);

                if (PagamentoSelecionado is not null)
                    ChangePagamentoSelecionado(value);

                
            }
        }

        private Pagamento _pagamentoSelecionado;
        public Pagamento PagamentoSelecionado
        {
            get { return _pagamentoSelecionado; }
            set
            {
                SetField(ref _pagamentoSelecionado, value);
                ChangeTipoSelecionado(value);
            }
        }


        public decimal ValorRegistrado { get; private set; }

        public decimal Valor { get; set; }

        public SimpleCommand ModificarPagamentoCmd => new(ChangePagamento);
        public SimpleCommand SalvarParcelaCmd => new(SalvarParcela);
        public SimpleCommand ConfirmarRecebimentoCmd => new(ConfirmarRecebimento);

        public decimal TotalPagamentos { get; private set; }

        public Action? ParcelaSaved { get; set; }

        public ParcelaViewModel(Parcela? parcelaSelecionada, Fatura faturaSelecionada)
        {
            if(faturaSelecionada is null)
            {
                return;
            }

            IsParcelaEditable = true;


            Parcela = parcelaSelecionada;


            Fatura = DAO.Load<Fatura>((faturaSelecionada as Fatura).Id);


            FillPagamentos();

            foreach(Parcela p in Fatura.Parcelas)
            {
                ValorRegistrado += p.Valor;
            }



            if(Parcela is null)
            {
                Parcela = new Parcela()
                {
                    Indice = Fatura.Parcelas.Count + 1,
                    DataVencimento = DateTime.Today.AddMonths(1),
                    Status = Parcela.Status_Parcela.Pendente,
                };
            }

            else
            {
                ValorRegistrado -= Parcela.Valor;

                if(Parcela.Status == Parcela.Status_Parcela.Pago)
                {
                    Parcela.Pagamentos.Load();
                    for(int i = 0;i < Parcela.Pagamentos.Count;i++)
                    {
                        if(Parcela.Pagamentos[i].ValorPagamento > 0)
                        {
                            Pagamentos[i].ValorPagamento = Parcela.Pagamentos[i].ValorPagamento;
                        }
                    }
                    SetValorTotal();
                    IsParcelaEditable = false;
                }
            }

        }


        private void ChangePagamentoSelecionado(Pagamento.EnumTipoPagamento value)
        {
            if(PagamentoSelecionado.TipoPagamento != value)
            {
                foreach(Pagamento p in Pagamentos)
                {
                    if(p.TipoPagamento == value)
                    {
                        PagamentoSelecionado = p;
                    }
                }
            }
        }
        private void ChangeTipoSelecionado(Pagamento value)
        {
            if(TipoPagamentoSelecionado != value.TipoPagamento)
            {
                TipoPagamentoSelecionado = value.TipoPagamento;
            }
        }


        private void FillPagamentos()
        {

            TiposPagamento = new List<Pagamento.EnumTipoPagamento>();
            TiposPagamento.Add(Pagamento.EnumTipoPagamento.Moeda);
            TiposPagamento.Add(Pagamento.EnumTipoPagamento.Credito);
            TiposPagamento.Add(Pagamento.EnumTipoPagamento.Debito);
            TiposPagamento.Add(Pagamento.EnumTipoPagamento.Outro);



            Pagamentos = new PList<Pagamento>();

            Pagamento p_moeda = new Pagamento()
            {
                Usuario = Main.Usuario,
                TipoPagamento = Pagamento.EnumTipoPagamento.Moeda,
                ValorPagamento = 0
            };

            Pagamento p_credito = new Pagamento()
            {
                Usuario = Main.Usuario,
                TipoPagamento = Pagamento.EnumTipoPagamento.Credito,
                ValorPagamento = 0
            };

            Pagamento p_debito = new Pagamento()
            {
                Usuario = Main.Usuario,
                TipoPagamento = Pagamento.EnumTipoPagamento.Debito,
                ValorPagamento = 0
            };

            Pagamento p_outro = new Pagamento()
            {
                Usuario = Main.Usuario,
                TipoPagamento = Pagamento.EnumTipoPagamento.Outro,
                ValorPagamento = 0
            };

            Pagamentos.Add(p_moeda);
            Pagamentos.Add(p_credito);
            Pagamentos.Add(p_debito);
            Pagamentos.Add(p_outro);

        }

        private void SetValorTotal()
        {
            TotalPagamentos = 0;
            foreach (Pagamento p in Pagamentos)
            {
                TotalPagamentos += p.ValorPagamento;
            }
        }

        private void ChangePagamento()
        {
            switch(TipoPagamentoSelecionado)
            {
                case Pagamento.EnumTipoPagamento.Moeda:
                    Pagamentos[0].ValorPagamento = Valor;
                    break;

                case Pagamento.EnumTipoPagamento.Credito:
                    Pagamentos[1].ValorPagamento = Valor;
                    break;

                case Pagamento.EnumTipoPagamento.Debito:
                    Pagamentos[2].ValorPagamento = Valor;
                    break;

                case Pagamento.EnumTipoPagamento.Outro:
                    Pagamentos[3].ValorPagamento = Valor;
                    break;

                default:
                    break;
            }

            SetValorTotal();
        }

        private void CheckPagamentos()
        {

            PList<Pagamento> pagamentosParaSalvar = new PList<Pagamento>();
            
            foreach (Pagamento p in Pagamentos)
            {
                if (p.ValorPagamento > 0)
                {
                    pagamentosParaSalvar.Add(p);
                }
            }
            

            if (!pagamentosParaSalvar.Any())
            {
                throw new InvalidOperationException("Não há pagamentos registrados");
            }

            if (TotalPagamentos != Parcela.Valor)
            {
                throw new InvalidOperationException("O valor do pagamento não pode ser diferente do valor da parcela.");
            }

            Parcela.Pagamentos = pagamentosParaSalvar;
        }

        private void CheckValorParcela()
        {
            if(Parcela.Valor > Fatura.ValorTotal - ValorRegistrado)
            {
                throw new InvalidOperationException("O valor da parcela não pode exceder o valor restante da fatura: " + (Fatura.ValorTotal - ValorRegistrado));
            }

            if(Parcela.Valor == 0)
            {
                throw new InvalidOperationException("O valor da parcela deve ser maior que zero");
            }
        }

        private void SalvarParcela()
        {
            try
            {
                CheckValorParcela();

                Parcela.Fatura = Fatura;
                Parcela.Save();

                ParcelaSaved?.Invoke();
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.InnerException is MySqlException e)
                {
                    if(e.Number == 40004)
                    {
                        OnShowMessage("Erro ao salvar parcela", "ERRO: " + e.Message );
                    }
                }

                else
                {
                    OnShowMessage("Erro ao salvar parcela", ex.Message);
                }
            }
            
        }

        private void ConfirmarRecebimento()
        {
            try
            {
                CheckValorParcela();
                CheckPagamentos();


                Parcela.DataPagamento = DateTime.Now;
                Parcela.Status = Parcela.Status_Parcela.Pago;
                Parcela.Fatura = Fatura;
                Parcela.Save();

                ParcelaSaved?.Invoke();

            }
            catch(Exception ex)
            {
                OnShowMessage("Erro ao confirmar recebimento", ex.Message);
            }

            
        }
    }

}


