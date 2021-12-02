using MahApps.Metro.Controls;
using SisMaper.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SisMaper.Views
{
    public partial class CrudProduto : MetroWindow
    {
        
        public CrudProduto()
        {
            InitializeComponent();
            Loaded += CrudProdutoLoaded;

        }
        

        private void CrudProdutoLoaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is ICloseWindow vm)
            {
                vm.Close += Close;
            }
        }


        private void CancelarButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CodigoDeBarrasTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (!( (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back))
            {
                e.Handled = true;
            }
            
        }

        private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = !new Regex("[0-9]+").IsMatch(e.Text);
        }
    }

}