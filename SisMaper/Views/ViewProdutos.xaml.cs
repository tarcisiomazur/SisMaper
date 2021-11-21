using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewProdutos : UserControl
    {


        //private string _filterText;

        /*
        public string FilterText
        {
            get => _filterText;
            set => _filterText = value;
        }
        */


        public ViewProdutos()
        {
            InitializeComponent();
            SetActions();
        }



        private void SetActions()
        {
            if (this.DataContext is IProdutos vm)
            {
                vm.OpenEditarProduto += () =>
                {
                    new CrudProduto() { DataContext = new CrudProdutoViewModel(ProdutosDataGrid.SelectedItem) }.ShowDialog();
                    DataContext = new ProdutosViewModel();
                    SetActions();
                };

                vm.OpenNovoProduto += () =>
                {
                    new CrudProduto() { DataContext = new CrudProdutoViewModel(null) }.ShowDialog();
                    DataContext = new ProdutosViewModel();
                    SetActions();
                };

                vm.ProdutoExcluido += () =>
                {
                    DataContext = new ProdutosViewModel();
                    SetActions();
                };

                vm.OpenCategoria += () =>
                {
                    new ViewCategorias().ShowDialog();
                };

                vm.OpenUnidade += () =>
                {
                    new ViewUnidades().ShowDialog();
                };
            }

        }


        private void NovoProduto(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Height: " + Height + "    Width: " + Width);
            new CrudProduto().ShowDialog();
        }






        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseCommandProperty =
            DependencyProperty.Register("MouseCommand", typeof(ICommand), typeof(ViewProdutos), new PropertyMetadata(null));

        public ICommand MouseCommand
        {
            get { return (BaseCommand)GetValue(MouseCommandProperty); }
            set { SetValue(MouseCommandProperty, value); }
        }




        // Using a DependencyProperty as the backing store for MouseCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseCommandParameterProperty =
            DependencyProperty.Register("MouseCommandParameter", typeof(Object), typeof(ViewProdutos), new PropertyMetadata(null));

        public Object MouseCommandParameter
        {
            get { return (Object)GetValue(MouseCommandParameterProperty); }
            set { SetValue(MouseCommandParameterProperty, value); }
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MouseCommand?.Execute(null);
        }

        private void Produtos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MouseCommand?.Execute(MouseCommandParameter);

            if (sender != null)
            {
                DataGrid? grid = sender as DataGrid;
                if (grid != null)
                {
                    DataGridRow? dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    if (dgr != null && dgr.IsMouseOver)
                    {
                        new CrudProduto() { DataContext = new CrudProdutoViewModel(grid.SelectedItem) }.ShowDialog();
                    }
                }
            }

        }

        private void Produtos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid? grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null)
                {
                    DataGridRow? dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    if (dgr != null && !dgr.IsMouseOver)
                    {
                        dgr.IsSelected = false;
                    }
                }
            }
        }
    }
}