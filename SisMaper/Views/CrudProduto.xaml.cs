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
            if (e.Key is >= Key.D0 and <= Key.D9 or >= Key.NumPad0 and <= Key.NumPad9 or Key.Back or Key.Tab)
            {
                return;
            }
            e.Handled = true;
        }
    }

}