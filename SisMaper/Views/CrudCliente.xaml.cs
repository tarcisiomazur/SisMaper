using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para CrudPessoaFisica.xaml
    /// </summary>
    public partial class CrudPessoaFisica : MetroWindow
    {
        public bool isSelectedPessoaFisicaTab { get; set; }
        public bool IsGridEnabled { get; set; }
        

        public CrudPessoaFisica()
        {
            IsGridEnabled = true;

            InitializeComponent();
            
            Loaded += CrudPessoaFisica_Loaded;
        }





        private void CrudPessoaFisica_Loaded(object sender, RoutedEventArgs e)
        {
            

            if(isSelectedPessoaFisicaTab)
            {
                PessoaFisicaTabItem.IsSelected = true;
                PessoaJuridicaTabItem.IsEnabled = false;
            }

            else
            {
                PessoaJuridicaTabItem.IsSelected = true;
                PessoaFisicaTabItem.IsEnabled = false;
            }

            if(DataContext is IClienteSave vm)
            {
                vm.SaveCliente += () => Close();
            }

            grid.IsEnabled = IsGridEnabled;

        }

        private void CancelarButtonLeftClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void CPFMaskedTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CPFMaskedTextBox.Focus();
            CPFMaskedTextBox.Select(0, 0);
            e.Handled = true;
        }

        private void CNPJMaskedTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CNPJMaskedTextBox.Focus();
            CNPJMaskedTextBox.Select(0, 0);
            e.Handled = true;
        }
    }
}
