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
    /// Lógica interna para ViewParcela.xaml
    /// </summary>
    public partial class ViewParcela : MetroWindow
    {
        public ViewParcela()
        {
            InitializeComponent();
            Loaded += ViewParcelaLoaded;
        }


        private void CancelarButton(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ViewParcelaLoaded(object sender, RoutedEventArgs e)
        {
            DataVencimentoDatePicker.DisplayDateStart = DateTime.Today;

            if(DataContext is ICloseWindow vm)
            {
                vm.Close = () => Close();
            }

        }

        /*
        private void MoedaTextBox(object sender, RoutedEventArgs e)
        {
            if(tx.Text.Length == 0)
            {
                tx.Text = "0";
            }
        }
        */
    }
}
