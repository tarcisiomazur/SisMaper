using MahApps.Metro.Controls;
using SisMaper.Tools;
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
            DataContextChanged += SetActions;
            InitializeComponent();

        }


        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CrudProdutoViewModel newViewModel)
            {
                newViewModel.ProdutoSaved += Close;
                newViewModel.OpenEditarCategoria += OpenEditarCategoria;
                newViewModel.OpenEditarUnidade += OpenEditarUnidade;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is CrudProdutoViewModel oldViewModel)
            {
                oldViewModel.ProdutoSaved -= Close;
                oldViewModel.OpenEditarCategoria -= OpenEditarCategoria;
                oldViewModel.OpenEditarUnidade -= OpenEditarUnidade;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }



        private void OpenEditarCategoria()
        {
            new ViewCategorias { Owner = this }.ShowDialog();
        }

        private void OpenEditarUnidade()
        {
            new ViewUnidades { Owner = this }.ShowDialog();
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
    }

}