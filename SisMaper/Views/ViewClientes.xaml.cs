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
    public partial class ViewClientes : UserControl
    {
        public ViewClientes()
        {
            InitializeComponent();
            SetActions();


        }


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
                    new CrudPessoaFisica() { isSelectedPessoaFisicaTab = (PessoaFisicaTabItem.IsSelected) ? true : false, DataContext = new CrudClienteViewModel(null) }.ShowDialog();
                    DataContext = new ClientesViewModel();
                    SetActions();
                };

                vm.OpenEditarCliente += () =>
                {
                    new CrudPessoaFisica() { isSelectedPessoaFisicaTab = (PessoaFisicaTabItem.IsSelected)? true : false, DataContext = new CrudClienteViewModel( (PessoaFisicaTabItem.IsSelected)? PessoaFisicaDataGrid.SelectedItem : PessoaJuridicaDataGrid.SelectedItem ) }.ShowDialog();
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
