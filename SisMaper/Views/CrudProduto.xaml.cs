using MahApps.Metro.Controls;
using SisMaper.ViewModel;
using System;
using System.Windows;


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

    }

}