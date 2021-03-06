using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewProdutos
    {

        public ViewProdutos()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
            
        }  
        
        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ProdutosViewModel newViewModel)
            {
                Show += newViewModel.Initialize;
                Hide += newViewModel.Clear;
                newViewModel.OpenCrudProduto += OpenCrudProduto;
                newViewModel.OpenCategoria += OpenCategorias;
                newViewModel.OpenUnidade += OpenUnidades;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;

            }
            if (e.OldValue is ProdutosViewModel oldViewModel)
            {
                Show -= oldViewModel.Initialize;
                Hide -= oldViewModel.Clear;
                oldViewModel.OpenCrudProduto -= OpenCrudProduto;
                oldViewModel.OpenCategoria -= OpenCategorias;
                oldViewModel.OpenUnidade -= OpenUnidades;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;

            }
        }

        private void OpenCrudProduto(CrudProdutoViewModel viewModel)
        {
            new CrudProduto() { DataContext = viewModel, Owner = Window }.ShowDialog();
            OnShow();
        }


        private void OpenCategorias()
        {
            new ViewCategorias() { Owner = Window }.ShowDialog();
            OnShow();
        }


        private void OpenUnidades()
        {
            new ViewUnidades() { Owner = Window }.ShowDialog();
            OnShow();
        }

        private void DataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var firstCol = dataGrid.Columns.First();
            firstCol.SortDirection = ListSortDirection.Ascending;
            dataGrid.Items.SortDescriptions.Add(new SortDescription(firstCol.SortMemberPath, ListSortDirection.Ascending));
        }
    }
}