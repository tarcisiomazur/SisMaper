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
        
        public decimal ValorRegistrado { get; private set; }

        public decimal Valor { get; set; } = 0;

        public SimpleCommand SalvarParcelaCmd => new(SalvarParcela);
        public SimpleCommand ConfirmarRecebimentoCmd => new(ConfirmarRecebimento);

        public Action? ParcelaSaved { get; set; }

        public ParcelaViewModel(Parcela? parcelaSelecionada, Fatura faturaSelecionada)
        {
            if(faturaSelecionada is null)
            {
                return;
            }

            IsParcelaEditable = true;


            Parcela = parcelaSelecionada;


            //Fatura = DAO.Load<Fatura>(faturaSelecionada.Id);
            Fatura = faturaSelecionada;


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
                    Fatura = Fatura,
                    Pagamentos = new PList<Pagamento>()
                };
            }

            else
            {
                ValorRegistrado -= Parcela.Valor;

                if(Parcela.Status == Parcela.Status_Parcela.Pago)
                {
                    Parcela.Pagamentos.Load();
                    IsParcelaEditable = false;
                }
                else
                {
                    Valor = Parcela.Valor;
                }
            }

        }
       
        private void CheckValorParcela()
        {
            if(Parcela.Valor > Fatura.ValorTotal - ValorRegistrado)
            {
                throw new InvalidOperationException("O valor da parcela não pode exceder o valor restante da fatura: " + (Fatura.ValorTotal - ValorRegistrado));
            }

            if(Parcela.Valor <= 0)
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
                    if(e.Number == 40004)   //erro da falta de crédito
                    {
                        OnShowMessage("Erro ao salvar parcela", e.Message );
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

                if(Valor < Parcela.Valor)
                {
                    MessageDialogResult confirmacao = OnShowMessage("Recebimento", 
                        "Valor do pagamento menor que o valor da parcela, deseja redistribuir entre as parcelas?",
                        MessageDialogStyle.AffirmativeAndNegative, 
                        new MetroDialogSettings() { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                    if (confirmacao == MessageDialogResult.Affirmative) RedistribuirParcelas();
                    else return;
                }


                Parcela.Pagamentos.Add(new Pagamento()
                {
                    Usuario = Main.Usuario,
                    TipoPagamento = Pagamento.EnumTipoPagamento.Credito,
                    ValorPagamento = Valor
                });

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


        private void RedistribuirParcelas()
        {
            try
            {
                PList<Parcela> parcelasPendentes = Fatura.Parcelas.Where(p => p.Id != Parcela.Id && p.Status == Parcela.Status_Parcela.Pendente).ToList().ToPList();

                decimal valorRestante = Parcela.Valor - Valor;
                Parcela.Valor = Valor;
                
                //cria uma nova parcela
                if (!parcelasPendentes.Any())
                {
                    Fatura.Parcelas.Add(new Parcela()
                    {
                        DataVencimento = Parcela.DataVencimento.AddMonths(1),
                        Indice = Fatura.Parcelas.Count + 1,
                        Valor = valorRestante
                    });
                    return;
                }

                //redistribui o valor nas parcelas
                decimal valorParcelaArredondado = decimal.Round(valorRestante / parcelasPendentes.Count, 2);
                decimal diferenca = (valorRestante / parcelasPendentes.Count - valorParcelaArredondado) * parcelasPendentes.Count;

                for (int i = 0; i < parcelasPendentes.Count; i++)
                {
                    parcelasPendentes[i].Valor += (i + 1 == parcelasPendentes.Count) ? (valorParcelaArredondado + diferenca) : valorParcelaArredondado;
                }
                parcelasPendentes.Save();
            }
            catch
            {
                throw;
            }

        }

    }

}


