using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public decimal ValorTotal  { get; set; }

        public ParcelaViewModel(object? parcelaSelecionada, object? faturaSelecionada)
        {
            if(faturaSelecionada is null)
            {
                return;
            }

            isParcelaEditable = true;


            Parcela = (Parcela)parcelaSelecionada;


            //Fatura = (Fatura)faturaSelecionada;

            Fatura = DAO.Load<Fatura>((faturaSelecionada as Fatura).Id);


            Salvar = new SalvarParcelaCommand();
            Confirmar = new ConfirmarRecebimentoCommand();

            DialogCoordinator = new DialogCoordinator();


            if(Parcela is null)
            {
                Parcela = new Parcela()
                {
                    Indice = Fatura.Parcelas.Count + 1,
                    DataVencimento = DateTime.Today.AddMonths(1),
                    Status = Parcela.Status_Parcela.Pendente
                };

            }

            else
            {
                if(Parcela.Status == Parcela.Status_Parcela.Pago)
                {
                    isParcelaEditable = false;
                }
            }


        }


        private void SetValorTotal()
        {
            ValorTotal = ValorMoeda + ValorCredito + ValorDebito + ValorOutro;
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


            Parcela.Pagamentos = new PList<Pagamento>();

            if(ValorMoeda > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    //Usuario = Main.Usuario

                    Usuario = DAO.Load<Usuario>(13),

                    ValorPagamento = ValorMoeda,
                    Tipo = (char) Pagamento.TipoPagamento.Moeda,
                });
            }

            if (ValorCredito > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    //Usuario = Main.Usuario

                    Usuario = DAO.Load<Usuario>(13),

                    Tipo = (char) Pagamento.TipoPagamento.Credito,
                    ValorPagamento = ValorCredito,
                });
            }

            if (ValorDebito > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    //Usuario = Main.Usuario

                    Usuario = DAO.Load<Usuario>(13),

                    Tipo = (char) Pagamento.TipoPagamento.Debito,
                    ValorPagamento = ValorDebito
                });
            }

            if (ValorOutro > 0)
            {
                Parcela.Pagamentos.Add(new Pagamento()
                {
                    //Usuario = Main.Usuario

                    Usuario = DAO.Load<Usuario>(13),

                    Tipo = (char) Pagamento.TipoPagamento.Outro,
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

            if(Parcela.Valor > Fatura.ValorTotal)
            {
                throw new InvalidOperationException("O valor da parcela não pode exceder o valor da fatura");
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
            catch(Exception ex)
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro ao salvar parcela", "Erro: " + ex.Message + "      " + ex.InnerException);
            }
            
        }

        public void ConfirmarRecebimento()
        {
            try
            {
                CheckValorParcela();
                CheckValoresRecebimento();


                Parcela.DataPagamento = DateTime.Now;
                Parcela.Status = Parcela.Status_Parcela.Pago;
                Parcela.Fatura = Fatura;
                Parcela.Save();

                Close?.Invoke();

            }
            catch(Exception ex)
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro ao confirmar recebimento", "Erro: " + ex.Message);
                //DialogCoordinator.ShowModalMessageExternal(this, "Erro ao confirmar recebimento", ex.ToString());
            }

            
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


