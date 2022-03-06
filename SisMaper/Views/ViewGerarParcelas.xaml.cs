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
    /// Lógica interna para ViewGerarParcelas.xaml
    /// </summary>
    public partial class ViewGerarParcelas : Window
    {

        public ViewGerarParcelas()
        {
            InitializeComponent();

            DataContextChanged += SetActions;
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is FaturaViewModel viewModel) viewModel.ParcelasGeradas += Close;
        }

        private void NumeroParcelasTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void CancelarButtonPressed(object sender, MouseButtonEventArgs e) => Close();
    }
}
