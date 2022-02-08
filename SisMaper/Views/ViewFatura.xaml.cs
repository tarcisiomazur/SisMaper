using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
//using System.Windows;
//using System.Windows.Data;
//using Google.Protobuf.WellKnownTypes;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public sealed partial class ViewFatura
    {
        public ObservableCollection<Parcela> ListParcelas = new();

        private bool isClientePessoaFisica = true;

        public ViewFatura()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
            Loaded += ViewFatura_Loaded;

        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FaturaViewModel newViewModel)
            {
                newViewModel.FaturaSaved += Close;
                newViewModel.OpenViewCliente += OpenViewCliente;
                newViewModel.OpenCrudParcela += OpenCrudParcela;
                newViewModel.FaturaChanged += ChangeFatura;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is FaturaViewModel oldViewModel)
            {
                oldViewModel.FaturaSaved -= Close;
                oldViewModel.OpenViewCliente -= OpenViewCliente;
                oldViewModel.OpenCrudParcela -= OpenCrudParcela;
                oldViewModel.FaturaChanged -= ChangeFatura;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }



        private void OpenViewCliente(CrudClienteViewModel viewModel, bool isPessoaFisica)
        {
            new CrudCliente { DataContext = viewModel, IsSelectedPessoaFisicaTab = isPessoaFisica, IsGridEnabled = false, Owner = this }.ShowDialog();
        }


        private void OpenCrudParcela(ParcelaViewModel viewModel)
        {
            new ViewParcela { DataContext = viewModel, Owner = this }.ShowDialog();
            ((FaturaViewModel)DataContext).ResetFatura();
        }


        private void ChangeFatura()
        {
            ChangeFaturaButton.Content = ((string)ChangeFaturaButton.Content == "Encerrar Fatura") ? "Reabrir Fatura" : "Encerrar Fatura";
        }



        private void ViewFatura_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParcelasDataGrid.IsEnabled)
            {
                ChangeFaturaButton.Content = "Encerrar Fatura";
            }
            else
            {
                ChangeFaturaButton.Content = "Reabrir Fatura";
            }

        }


        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Close();
        }


    }
}