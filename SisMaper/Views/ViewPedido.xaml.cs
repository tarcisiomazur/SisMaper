using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;
using Xceed.Wpf.Toolkit.Core;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class ViewPedido : MetroWindow
    {
        private List<Produto> BoxProdutos;
        public ViewPedido()
        {
            InitializeComponent();
            
            var pedido = DAO.Load<Pedido>(24);

            cmbCliente.Text = pedido.Cliente.Nome;
            
            ValorTotal.Text = $"R$ {pedido.Itens.Sum(item => item.Total).RealFormat()}";
            Console.WriteLine(pedido.Itens.Count);
            Itens.ItemsSource = pedido.Itens;
            BoxProdutos = new List<Produto>();
            cmbProduto.DataContext = BoxProdutos;
        }

        private static TextBox FindTextBox(DependencyObject element)
        {
            foreach (var dpo in element.GetChildObjects(true))
            {
                Console.WriteLine(dpo.GetType());
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
            BoxProdutos.Clear();
            var tce = (TextCompositionEventArgs) e;
            var tb = FindTextBox(cmbProduto);
            if (tb != null)
            {
                var txt = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength);
                Console.WriteLine(txt);
                BoxProdutos.AddRange(new PList<Produto>($"Descricao LIKE '%{txt+tce.Text}%'", 0,100));
            }

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
    }
}
