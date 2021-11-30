using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
//using System.Windows;
//using System.Windows.Data;
//using Google.Protobuf.WellKnownTypes;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public sealed partial class ViewFatura
    {
        public ObservableCollection<Parcela> ListParcelas = new();

        private bool isClientePessoaFisica = true;

        public ViewFatura()
        {
            InitializeComponent();


            Loaded += ViewFatura_Loaded;



            /*

            ParcelasDataGrid.DataContext = ListParcelas;
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
            */
        }



        private void ViewFatura_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ParcelasDataGrid.IsEnabled)
            {
                ChangeFaturaButton.Content = "Encerrar Fatura";
            }
            else
            {
                ChangeFaturaButton.Content = "Reabrir Fatura";
            }
            SetActions();
            //SetCliente();

        }


        private void SetCliente()
        {
            if (DataContext is IFatura vm)
            {
                vm.ChangeCliente?.Invoke();
            }
        }


        private void SetActions()
        {
            if (DataContext is ICloseWindow vm)
            {
                vm.Close = () => Close();
            }

            if (DataContext is IFatura vm2)
            {
                vm2.FaturaChanged = () =>
                {
                    ChangeFaturaButton.Content = ((string)ChangeFaturaButton.Content == "Encerrar Fatura") ? "Reabrir Fatura" : "Encerrar Fatura";
                };

                vm2.ClienteChangedToPessoaFisica = () => isClientePessoaFisica = true;
                vm2.ClienteChangedToPessoaJuridica = () => isClientePessoaFisica = false;

                /*
                vm2.OpenNovaParcela = () =>
                {
                    new ViewParcela() { DataContext = new ParcelaViewModel(ParcelasDataGrid.SelectedItem, (DataContext as FaturaViewModel)?.Fatura) }.ShowDialog();
                    DataContext = new FaturaViewModel( (DataContext as FaturaViewModel)?.Fatura );
                    SetActions();
                };
                */
                vm2.OpenNovaParcela = () =>
                {
                    new ViewParcela() { DataContext = new ParcelaViewModel(null, (DataContext as FaturaViewModel).Fatura) }.ShowDialog();

                    DataContext = new FaturaViewModel((DataContext as FaturaViewModel).Fatura);
                    SetActions();
                };

            }
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

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OpenViewCliente(object sender, MouseButtonEventArgs e)
        {

            new CrudPessoaFisica() { DataContext = new CrudPessoaFisicaViewModel( (DataContext as FaturaViewModel).Fatura.Cliente ), isSelectedPessoaFisicaTab = isClientePessoaFisica, IsGridEnabled = false }.ShowDialog();

            /*
            if (ClientesComboBox.SelectedItem is not null)
            {
                new CrudPessoaFisica() { DataContext = new CrudPessoaFisicaViewModel(ClientesComboBox.SelectedItem), isSelectedPessoaFisicaTab = isClientePessoaFisica, IsGridEnabled = false }.ShowDialog();
            }
            */
        }

    }
}