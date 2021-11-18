using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
//using System.Windows;
//using System.Windows.Data;
//using Google.Protobuf.WellKnownTypes;
using SisMaper.Models;

namespace SisMaper.Views
{
    public sealed partial class ViewFatura
    {
        public ObservableCollection<Parcela> ListParcelas = new();
        
        public ViewFatura()
        {
            InitializeComponent();
            Parcelas.DataContext = ListParcelas;
            var d = new DateTime(2021, 06, 15);
            ListParcelas.Add(new Parcela()
            {
                Indice = 1,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pago,
                DataPagamento = d.Add(TimeSpan.FromDays(-1)),
                Valor = 100
            });
            d=d.AddMonths(1);
            ListParcelas.Add(new Parcela()
            {
                Indice = 2,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pago,
                DataPagamento = d.Add(TimeSpan.FromDays(-1)),
                Valor = 200
            });
            d=d.AddMonths(1);
            ListParcelas.Add(new Parcela()
            {
                Indice = 3,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pendente,
                DataPagamento = null,
                Valor = 100
            });
            d=d.AddMonths(1);
            ListParcelas.Add(new Parcela()
            {
                Indice = 4,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pendente,
                DataPagamento = null,
                Valor = 100
            });
            ValorTotal.Text = ListParcelas.Sum(p => p.Valor).ToString("N2");
        }

        private void AdicionarParcela(object sender, MouseButtonEventArgs e)
        {
            ListParcelas.Add(new Parcela()
            {
                Indice = ListParcelas.Count+1,
                DataVencimento = DateTime.Now,
                Status = Parcela.Status_Parcela.Pendente,
                DataPagamento = null,
                Valor = 100
            });
            ListParcelas[0].Valor += 1000;
            ListParcelas.Clear();
        }
    }
}