using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SisMaper.ViewModel.CrudProdutoViewModel;

namespace SisMaper.Views
{
    public partial class CrudProduto : MetroWindow
    {
        private CrudProdutoViewModel cpvm => (CrudProdutoViewModel) DataContext;
        
        public CrudProduto()
        {
            InitializeComponent();
            //Produto = DAO.Load<Produto>(6);
            //grid.DataContext = Produto;
            //Title = "Editar Produto - " + Produto.Descricao;

            cpvm.OnSave += Close;


        }

        private void CrudProdutoLoaded(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is CrudProdutoViewModel vm)
            {
                vm.OnSave += Close;
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







        /* tentativa de deseleção das linhas
        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (sender != null)
            {
                DataGridRow dgr = gridLotes.ItemContainerGenerator.ContainerFromItem(gridLotes.SelectedItem) as DataGridRow;
                //gridLotes.UnselectAllCells();

                if (!dgr.IsMouseOver)
                {
                    //(dgr as DataGridRow).IsSelected = false;
                }
            }
            
        }
        */


        /* 
        private void dataGrid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGrid grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
                {
                    DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                    if (!dgr.IsMouseOver)
                    {
                        (dgr as DataGridRow).IsSelected = false;
                    }
                }
            }
        }
        */



        /*
         * NCM comboBox
         * 
            Grid.Row="0"
            Height="32"
            Margin="158,248,427,0"
            FontSize="16"
            mah:TextBoxHelper.Watermark="NCM"
            DisplayMemberPath="Nome"
            IsEditable="False"
            MaxDropDownHeight="125"
            mah:TextBoxHelper.WatermarkAlignment="Right"
            mah:TextBoxHelper.AutoWatermark="True"
            VerticalAlignment="Top" /> 

         */



        /*
                     <ComboBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Descricao}"/>
                </DataTemplate>
            </ComboBox.ItemTemplate>




                <ComboBox ItemsSource="{Binding Unidades}"
            Grid.Row="0"
            SelectedItem="{Binding UnidadeSelecionada}"
            Height="32"
            Width="165"
            Margin="158,206,427,0"
            FontSize="16"
            IsEditable="True"
            DisplayMemberPath="Descricao"
            MaxDropDownHeight="125"
            VerticalAlignment="Top"
            HorizontalAlignment="Left">


         */






        /*Data COntext
         * 
         *     <mah:MetroWindow.DataContext>
        <m:CrudProdutoViewModel DialogCoordinator="{StaticResource dialogCoordinator}"/>
    </mah:MetroWindow.DataContext>
         
         
         
         */
    }


    public interface IView
    {
        public bool? Open();
    }
}