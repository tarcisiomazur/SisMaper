using System.Windows;
using System.Windows.Input;
//using System.Windows;
//using System.Windows.Data;
//using Google.Protobuf.WellKnownTypes;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public sealed partial class ViewFatura
    {
        public ViewFatura()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FaturaViewModel newViewModel)
            {
                newViewModel.FaturaSaved += Close;
                newViewModel.OpenViewCliente += OpenViewCliente;
                newViewModel.OpenCrudParcela += OpenCrudParcela;
                newViewModel.OpenGerarParcelas += GerarParcelas;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is FaturaViewModel oldViewModel)
            {
                oldViewModel.FaturaSaved -= Close;
                oldViewModel.OpenViewCliente -= OpenViewCliente;
                oldViewModel.OpenCrudParcela -= OpenCrudParcela;
                oldViewModel.OpenGerarParcelas -= GerarParcelas;
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



        private void GerarParcelas(FaturaViewModel viewModel)
        {
            new ViewGerarParcelas() { DataContext = viewModel, Owner = this }.ShowDialog();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

    }
}