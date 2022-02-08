using MahApps.Metro.Controls;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para CrudPessoaFisica.xaml
    /// </summary>
    public partial class CrudCliente : MetroWindow
    {
        public bool IsSelectedPessoaFisicaTab { get; set; }
        public bool IsGridEnabled { get; set; } = true;
        

        public CrudCliente()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
            
            Loaded += CrudPessoaFisica_Loaded;

        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CrudClienteViewModel newViewModel)
            {
                newViewModel.ClienteSaved += Close;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is CrudClienteViewModel oldViewModel)
            {
                oldViewModel.ClienteSaved -= Close;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }

        private void CrudPessoaFisica_Loaded(object sender, RoutedEventArgs e)
        {
            
            if(IsSelectedPessoaFisicaTab)
            {
                PessoaFisicaTabItem.IsSelected = true;
                PessoaJuridicaTabItem.IsEnabled = false;
            }

            else
            {
                PessoaJuridicaTabItem.IsSelected = true;
                PessoaFisicaTabItem.IsEnabled = false;
            }

            grid.IsEnabled = IsGridEnabled;

        }

        private void CancelarButtonLeftClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MaskedTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

            if (sender is MaskedTextBox maskedTextBox)
            {
                int posicaoAtual = maskedTextBox.MaskedTextProvider.FindUnassignedEditPositionFrom(0, true);

                if (maskedTextBox.IsFocused && maskedTextBox.SelectionStart > posicaoAtual && maskedTextBox.SelectionLength == 0)
                {
                    if (posicaoAtual >= 0)
                        maskedTextBox.Select(posicaoAtual, 0);
                }

            }
        }
    }
}
