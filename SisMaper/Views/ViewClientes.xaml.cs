using SisMaper.Tools;
using SisMaper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SisMaper.Views
{
    /// <summary>
    /// Interação lógica para ViewClientes.xaml
    /// </summary>
    public partial class ViewClientes
    {
        public ViewClientes()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }


        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ClientesViewModel newViewModel)
            {
                Show += newViewModel.Initialize;
                Hide += newViewModel.Clear;
                newViewModel.OpenCrudCliente += OpenCrudCliente;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;

            }
            if (e.OldValue is ClientesViewModel oldViewModel)
            {
                Show -= oldViewModel.Initialize;
                Hide -= oldViewModel.Clear;
                oldViewModel.OpenCrudCliente -= OpenCrudCliente;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;

            }
        }



        private void OpenCrudCliente(CrudClienteViewModel? viewModel)
        {
            new CrudCliente() { IsSelectedPessoaFisicaTab = (PessoaFisicaTabItem.IsSelected)? true : false, DataContext = viewModel, Owner = Window }.ShowDialog();
            OnShow();
        }


        /*
        private void SetActions()
        {
            if(PessoaFisicaTabItem.IsSelected)
            {
                PessoaJuridicaDataGrid.SelectedItem = null;
            }

            if (PessoaJuridicaTabItem.IsSelected)
            {
                PessoaFisicaDataGrid.SelectedItem = null;
            }


            if (DataContext is ICliente vm)
            {
                vm.OpenNovoCliente += () =>
                {
                    new CrudCliente() { isSelectedPessoaFisicaTab = (PessoaFisicaTabItem.IsSelected) ? true : false, DataContext = new CrudClienteViewModel(null) }.ShowDialog();
                    DataContext = new ClientesViewModel();
                    SetActions();
                };

                vm.OpenEditarCliente += () =>
                {
                    new CrudCliente() { isSelectedPessoaFisicaTab = (PessoaFisicaTabItem.IsSelected)? true : false, DataContext = new CrudClienteViewModel( (PessoaFisicaTabItem.IsSelected)? PessoaFisicaDataGrid.SelectedItem : PessoaJuridicaDataGrid.SelectedItem ) }.ShowDialog();
                    DataContext = new ClientesViewModel();
                    SetActions();
                };

                vm.ClienteExcluido += () =>
                {
                    DataContext = new ClientesViewModel();
                    SetActions();
                };
            }
        }

        */

        private void PessoaFisicaTabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PessoaJuridicaDataGrid.SelectedItem = null;
        }

        private void PessoaJuridicaTabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PessoaFisicaDataGrid.SelectedItem = null;
        }
    }
}
