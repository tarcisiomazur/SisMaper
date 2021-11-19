using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;
using Xceed.Wpf.Toolkit.Core;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class ViewPedido : MetroWindow
    {
        private List<Produto> BoxProdutos;
        private TextBox TbBuscarProduto;

        private PedidoViewModel? ViewModel => DataContext as PedidoViewModel ?? null;
        
        public ViewPedido(Pedido pedido)
        {
            InitializeComponent();
            ViewModel!.Pedido = pedido;
        }

        private void SetActions()
        {
        }

        private static TextBox FindTextBox(DependencyObject element)
        {
            foreach (var dpo in element.GetChildObjects(true))
            {
                switch (dpo)
                {
                    case TextBox tb:
                        return tb;
                    case UIElement ui:
                        var t = FindTextBox(ui);
                        if (t != null) return t;
                        break;
                }
            }

            return null;
        }
        
        private void FiltrarProdutos(object sender, RoutedEventArgs e)
        {
            var tce = (TextCompositionEventArgs) e;
            TbBuscarProduto ??= FindTextBox(cmbProduto);
            if (TbBuscarProduto == null) return;
            var txt = TbBuscarProduto.Text.Remove(TbBuscarProduto.SelectionStart, TbBuscarProduto.SelectionLength);
            Console.WriteLine(txt);
            BoxProdutos.Clear();
            BoxProdutos.AddRange(new PList<Produto>($"Descricao LIKE '%{txt+tce.Text}%'", 0,100));
        }

        private void FiltrarCliente(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Filtrando Cliente");
        }

        private void VerificaQuantidade(object? sender, QueryValueFromTextEventArgs e)
        {
            Console.WriteLine(e.HasParsingError);
            if (e.Value != null)
            {
                Console.WriteLine(e.Value);
            }
            Console.WriteLine(Quantidade.Text);
        }
        private void VerificaQuantidade2(object? sender, QueryTextFromValueEventArgs e)
        {
            Console.WriteLine(e.Text);
            if (e.Value != null)
            {
                Console.WriteLine(e.Value);
            }
            Console.WriteLine(Quantidade.Text);
        }

        private void AdicionarItem(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                //TODO adiciona item  
                e.Handled = true; 
            }
            Console.WriteLine(e.Handled);
        }

        private void Context_Delete(object sender, RoutedEventArgs e)
        {
            
        }

        private void WindowLote(object sender, MouseButtonEventArgs e)
        {
            new ViewEscolherLote().ShowDialog();
        }
    }
}
