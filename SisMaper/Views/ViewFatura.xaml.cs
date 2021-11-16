using System;
using System.Collections.Generic;
using System.Linq;
//using System.Windows;
//using System.Windows.Data;
//using Google.Protobuf.WellKnownTypes;
using MahApps.Metro.Controls;
using SisMaper.Models;

namespace SisMaper.Views
{
    public partial class ViewFatura : MetroWindow
    {
        public ViewFatura()
        {
            InitializeComponent();
            var x = new List<Parcela>();
            var d = new DateTime(2021, 06, 15);
            x.Add(new Parcela()
            {
                Indice = 1,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pago,
                DataPagamento = d.Add(TimeSpan.FromDays(-1)),
                Valor = 100
            });
            d=d.AddMonths(1);
            x.Add(new Parcela()
            {
                Indice = 2,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pago,
                DataPagamento = d.Add(TimeSpan.FromDays(-1)),
                Valor = 200
            });
            d=d.AddMonths(1);
            x.Add(new Parcela()
            {
                Indice = 3,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pendente,
                DataPagamento = null,
                Valor = 100
            });
            d=d.AddMonths(1);
            x.Add(new Parcela()
            {
                Indice = 4,
                DataVencimento = d,
                Status = Parcela.Status_Parcela.Pendente,
                DataPagamento = null,
                Valor = 100
            });
            ValorTotal.Text = x.Sum(p => p.Valor).ToString("N2");
            Parcelas.DataContext = x;
            
        }
    }
}