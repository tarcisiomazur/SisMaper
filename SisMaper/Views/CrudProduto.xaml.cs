using MahApps.Metro.Controls;
using SisMaper.ViewModel;
using System;
using System.Windows;


namespace SisMaper.Views
{
    public partial class CrudProduto : MetroWindow
    {
        private CrudProdutoViewModel cpvm => (CrudProdutoViewModel) DataContext;
        
        public CrudProduto(object selectedItem = null)
        {
            InitializeComponent();
            //cpvm.OnSave += Close;
            //Produto = DAO.Load<Produto>(6);
            //grid.DataContext = Produto;
            //Title = "Editar Produto - " + Produto.Descricao;

            Loaded += CrudProdutoLoaded;

        }
        

        private void CrudProdutoLoaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is ICloseWindow vm)
            {
                //vm.OnSave += Close;
                vm.Close += Close;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(cat.SelectedItem);
        }

        private void CancelarButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }







        public Object ProdutoProperty
        {
            get { return (Object)GetValue(ProdutoPropertyProperty); }
            set { SetValue(ProdutoPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProdutoProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProdutoPropertyProperty =
            DependencyProperty.Register("ProdutoProperty", typeof(Object), typeof(CrudProduto), new PropertyMetadata(null));

    }

}