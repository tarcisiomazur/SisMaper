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
    public class ParcelaViewModel : BaseViewModel, ICloseWindow
    {
        public bool isParcelaEditable { get; private set; }

        public Parcela Parcela { get; set; }
        public Fatura Fatura { get; set; }

        
        private decimal _valorMoeda;

        public decimal ValorMoeda 
        {
            get
            {
                return _valorMoeda;
            }
            set 
            {
                _valorMoeda = value;
                SetValorTotal();
            } 
        }

        private decimal _valorCredito;

        public decimal ValorCredito
        {
            get
            {
                return _valorCredito;
            }
            set
            {
                _valorCredito = value;
                SetValorTotal();
            }
        }

        private decimal _valorDebito;

        public decimal ValorDebito
        {
            get
            {
                return _valorDebito;
            }
            set
            {
                _valorDebito = value;
                SetValorTotal();
            }
        }

        private decimal _valorOutro;

        public decimal ValorOutro
        {
            get
            {
                return _valorOutro;
            }
            set
            {
                _valorOutro = value;
                SetValorTotal();
            }
        }


        public SalvarParcelaCommand Salvar { get; private set; }
        public ConfirmarRecebimentoCommand Confirmar { get; private set; }

        public Action Close { get; set; }


        public IDialogCoordinator DialogCoordinator { get; set; }


        public decimal ValorTotal  { get; private set; }


        public PList<Pagamento> Pagamentos { get; private set; }

        public List<Pagamento.TipoPagamento> TiposPagamento { get; private set; }

        private Pagamento.TipoPagamento _tipoSelecionado;
        public Pagamento.TipoPagamento TipoSelecionado
        {
            get { return _tipoSelecionado; }
            set
            {
                foreach(Pagamento p in Pagamentos)
                {
                    if(p.Tipo == value)
                    {
                        Valor = p.ValorPagamento;
                    }
                }
                SetField(ref _tipoSelecionado, value);

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

        public ModificarPagamentoCommand Modificar { get; private set; }

        public decimal TotalPagamentos { get; private set; }

        public ParcelaViewModel(object? parcelaSelecionada, object? faturaSelecionada)
        {
            if(faturaSelecionada is null)
            {
                return;
            }

            Modificar = new ModificarPagamentoCommand();

            isParcelaEditable = true;


            Parcela = (Parcela)parcelaSelecionada;


            //Fatura = (Fatura)faturaSelecionada;

            Fatura = DAO.Load<Fatura>((faturaSelecionada as Fatura).Id);


            FillPagamentos();

            foreach(Parcela p in Fatura.Parcelas)
            {
                ValorRegistrado += p.Valor;
            }


            Salvar = new SalvarParcelaCommand();
            Confirmar = new ConfirmarRecebimentoCommand();

            DialogCoordinator = new DialogCoordinator();


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
                    isParcelaEditable = false;
                }
            }

            //DataVencimento = Parcela.DataVencimento;
            //DataVencimento = DateTime.Today;

            //Console.WriteLine(Main.Usuario.Id);


        }


        private void ChangePagamentoSelecionado(Pagamento.TipoPagamento value)
        {
            if(PagamentoSelecionado.Tipo != value)
            {
                foreach(Pagamento p in Pagamentos)
                {
                    if(p.Tipo == value)
                    {
                        PagamentoSelecionado = p;
                    }
                }
            }
        }
        private void ChangeTipoSelecionado(Pagamento value)
        {
            if(TipoSelecionado != value.Tipo)
            {
                TipoSelecionado = value.Tipo;
            }
        }


        private void FillPagamentos()
        {

            TiposPagamento = new List<Pagamento.TipoPagamento>();
            TiposPagamento.Add(Pagamento.TipoPagamento.Moeda);
            TiposPagamento.Add(Pagamento.TipoPagamento.Credito);
            TiposPagamento.Add(Pagamento.TipoPagamento.Debito);
            TiposPagamento.Add(Pagamento.TipoPagamento.Outro);



            Pagamentos = new PList<Pagamento>();

            Pagamento p_moeda = new Pagamento()
            {
                Usuario = Main.Usuario,
                Tipo = Pagamento.TipoPagamento.Moeda,
                ValorPagamento = 0
            };

            Pagamento p_credito = new Pagamento()
            {
                Usuario = Main.Usuario,
                Tipo = Pagamento.TipoPagamento.Credito,
                ValorPagamento = 0
            };

            Pagamento p_debito = new Pagamento()
            {
                Usuario = Main.Usuario,
                Tipo = Pagamento.TipoPagamento.Debito,
                ValorPagamento = 0
            };

            Pagamento p_outro = new Pagamento()
            {
                Usuario = Main.Usuario,
                Tipo = Pagamento.TipoPagamento.Outro,
                ValorPagamento = 0
            };

            Pagamentos.Add(p_moeda);
            Pagamentos.Add(p_credito);
            Pagamentos.Add(p_debito);
            Pagamentos.Add(p_outro);

            TipoSelecionado = TiposPagamento.First();
            PagamentoSelecionado = Pagamentos.First();

        }

        private void SetValorTotal()
        {
            TotalPagamentos = 0;
            foreach (Pagamento p in Pagamentos)
            {
                TotalPagamentos += p.ValorPagamento;
            }
        }

        public void ChangePagamento()
        {
            switch(TipoSelecionado)
            {
                case Pagamento.TipoPagamento.Moeda:
                    Pagamentos[0].ValorPagamento = Valor;
                    break;

                case Pagamento.TipoPagamento.Credito:
                    Pagamentos[1].ValorPagamento = Valor;
                    break;

                case Pagamento.TipoPagamento.Debito:
                    Pagamentos[2].ValorPagamento = Valor;
                    break;

                case Pagamento.TipoPagamento.Outro:
                    Pagamentos[3].ValorPagamento = Valor;
                    break;

                default:
                    break;
            }

            SetValorTotal();
        }


        private void Check()
        {

            PList<Pagamento> pagamentosParaSalvar = new PList<Pagamento>();

            foreach (Pagamento p in Pagamentos)
            {
                if (p.ValorPagamento > 0)
                {
                    pagamentosParaSalvar.Add(p);
                }
            }

            if (pagamentosParaSalvar.Count == 0)
            {
                throw new InvalidOperationException("Não há pagamentos registrados");
            }

            if (TotalPagamentos != Parcela.Valor)
            {
                throw new InvalidOperationException("O valor do pagamento não pode ser diferente do valor da parcela.");
            }

            Parcela.Pagamentos = pagamentosParaSalvar;
        }

        private void CheckValoresRecebimento()
        {
            if(ValorMoeda < 0 || ValorCredito < 0 || ValorDebito < 0 || ValorOutro < 0)
            {
                throw new InvalidOperationException("Valores de pagamento não podem ser negativos");
            }

            if(ValorMoeda + ValorCredito + ValorDebito + ValorOutro != Parcela.Valor)
            {
                throw new InvalidOperationException("O valor de pagamento não pode ser diferente do valor da parcela");
            }


            //Parcela.Pagamentos = new PList<Pagamento>();
            //pagamentos = new PList<Pagamento>();

            if(ValorMoeda > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    Usuario = Main.Usuario,
                    ValorPagamento = ValorMoeda,
                    Tipo = Pagamento.TipoPagamento.Moeda,
                });
            }

            if (ValorCredito > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    Usuario = Main.Usuario,
                    Tipo = Pagamento.TipoPagamento.Credito,
                    ValorPagamento = ValorCredito,
                });
            }

            if (ValorDebito > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    Usuario = Main.Usuario,
                    Tipo = Pagamento.TipoPagamento.Debito,
                    ValorPagamento = ValorDebito
                });
            }

            if (ValorOutro > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    Usuario = Main.Usuario,
                    Tipo = Pagamento.TipoPagamento.Outro,
                    ValorPagamento = ValorOutro
                });
            }
        }

        private void CheckValorParcela()
        {
            if(Parcela.Valor <= 0)
            {
                throw new InvalidOperationException("Valor da parcela inválido");
            }

            if(Parcela.Valor > Fatura.ValorTotal - ValorRegistrado)
            {
                throw new InvalidOperationException("O valor da parcela não pode exceder o valor restante da fatura: " + (Fatura.ValorTotal - ValorRegistrado));
            }
        }


        public void SalvarParcela()
        {
            try
            {
                CheckValorParcela();

                Parcela.Fatura = Fatura;
                Parcela.Save();

                Close?.Invoke();
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.InnerException is MySqlException e)
                {
                    if(e.Number == 40004)
                    {
                        DialogCoordinator.ShowModalMessageExternal(this, "Erro ao salvar parcela", "ERRO: " + e.Message);
                    }
                }

                else
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro ao salvar parcela", "Erro: " + ex.Message);
                }
            }
            
        }

        public void ConfirmarRecebimento()
        {
            try
            {
                CheckValorParcela();
                //CheckValoresRecebimento();
                Check();


                Parcela.DataPagamento = DateTime.Now;
                Parcela.Status = Parcela.Status_Parcela.Pago;
                Parcela.Fatura = Fatura;
                Parcela.Save();

                Close?.Invoke();

            }
            catch(Exception ex)
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro ao confirmar recebimento", "Erro: " + ex.Message);
            }

            
        }
    }


    public class ModificarPagamentoCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ParcelaViewModel vm = (ParcelaViewModel) parameter;
            vm.ChangePagamento();
        }
    }


    public class SalvarParcelaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ParcelaViewModel vm = (ParcelaViewModel) parameter;
            vm.SalvarParcela();
        }
    }


    public class ConfirmarRecebimentoCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ParcelaViewModel vm = (ParcelaViewModel)parameter;
            vm.ConfirmarRecebimento();
        }
    }

}


