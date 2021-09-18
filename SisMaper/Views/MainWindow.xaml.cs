using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public void RandomPedido()
        {
            var p = new Pedido()
            {
                Cliente = DAO.FindWhereQuery<Cliente>("Id>0")[0],
                Itens = new PList<Item>(),
                Data = DateTime.Now
            };
            var produtos = PList<Produto>.FindWhereQuery("Id>0");
            Random r = new Random(DateTime.Now.Millisecond);

            var x = r.Next() % 10 + 5;
            Console.WriteLine(x);
            for (int i = 0; i < x; i++)
            {
                p.Itens.Add(new Item()
                {
                    Produto = produtos[r.Next()%produtos.Count],
                    Quantidade = r.Next(1,10),
                });
            }

            p.Save();
        }
        public MainWindow()
        {
            try
            {
                new ViewFatura().ShowDialog();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            var pedidos = PList<Pedido>.FindWhereQuery("Id>0");
            foreach (var pedido in pedidos)
            {
                Console.WriteLine(pedido);
            }
            Pedidos.Columns.AddSelector("Status",nameof(Pedido.Status));
            Pedidos.Columns.AddSelector("Id",nameof(Pedido.Id));
            Pedidos.Columns.AddSelector<Pedido>("Data","Data", p=>p.Data.DmaFormat());
            Pedidos.Columns.AddSelector("Cliente","Cliente.Nome");
            Pedidos.Columns.AddSelector<Pedido>("Valor Total", "ValorTotal",p=>p.ValorTotal.RealFormat());
            foreach (var pedido in pedidos)
            {
                pedido.ValorTotal = pedido.Itens.Sum(i => i.Total);
            }
            Pedidos.DataContext = pedidos;
            var c = new Cliente() {Nome = "Tarcísio Mazur Junior"};
            var faturas = new List<Fatura>();
            faturas.Add(new Fatura()
            {
                Id = 1,
                Cliente = c,
                ValorTotal = 500,
                Data = new DateTime(2021,05,15),
                Status = Fatura.Fatura_Status.Aberta,
                ValorPago = 300,
            });
            faturas.Add(new Fatura()
            {
                Id = 2,
                Cliente = c,
                ValorTotal = 150,
                Data = new DateTime(2021,8,7),
                Status = Fatura.Fatura_Status.Aberta,
                ValorPago = 0,
            });
            faturas.Add(new Fatura()
            {
                Id = 3,
                Cliente = c,
                ValorTotal = 870,
                Data = new DateTime(2021,8,15),
                Status = Fatura.Fatura_Status.Aberta,
                ValorPago = 200,
            });
            faturas.Add(new Fatura()
            {
                Id = 4,
                Cliente = c,
                ValorTotal = 165.89m,
                Data = new DateTime(2021,8,11),
                Status = Fatura.Fatura_Status.Fechada,
                ValorPago = 165.89m,
            });
            Faturas.DataContext = faturas;
            
        }

        public void Test1()
        {
            var cidade = DAO.FindWhereQuery<Cidade>("Nome LIKE 'Prude%'")[0];
            var cliente = DAO.FindWhereQuery<Cliente>("Id=7")[0];
            
            var produto = new Produto()
            {
                Categoria = new Categoria() { Descricao = "Rações" },
                Fracionado = true,
                CodigoBarras = "789321147823",
                Descricao = "Ração Special Dog Carne 20kg",
                PrecoCusto = 110.29m,
                PrecoVenda = 180m,
                Unidade = new Unidade() { Descricao = "Saco" },
            };
            try
            {
                Console.WriteLine(produto.Save() ? "Produto Salvo" : "Erro");
                Console.WriteLine("Produto: " + produto);
            }
            catch(PersistenceException pe)
            {
                if (pe.ErrorCode == 40001)
                {
                    
                }
                
            }

            var item = new Item();
            item.Produto = produto;
            item.Quantidade = 3;
            item.Valor = produto.PrecoVenda;
            
            var pedido = new Pedido();
            pedido.Cliente = cliente;
            pedido.Itens = new PList<Item>();
            pedido.Itens.Add(item);
            pedido.ValorTotal = pedido.Itens.Sum(item => item.Total);
            pedido.Data = DateTime.Now;
            
            var parcelas = new PList<Parcela>();
            parcelas.Add(new Parcela()
            {
                Valor = pedido.ValorTotal,
                DataVencimento = new DateTime(2021,10,15),
                Status = Parcela.Status_Parcela.Pendente,
                Indice = 1,
            });
            
            pedido.Fatura = new Fatura()
            {
                Cliente = cliente,
                Parcelas = parcelas,
                ValorTotal = pedido.ValorTotal,
                ValorPago = 0,
            };
            
            Console.WriteLine(pedido.Save() ? "Pedido Salvo" : "Erro");
            Console.WriteLine("Pedido: " + pedido);
            
        }
        
        public void Test()
        {
            #region Carrega Cidades e Estados

            var estados = new PList<Estado>("ID>0");
            foreach (var estado in estados)
            {
                Console.WriteLine(estado.Nome + " " + estado.Cidades.Count);
            }

            var parana = estados.Find(estado => estado.Nome.Like("paraná"));
            var prudentopolis = parana.Cidades.Find(cidade => cidade.Nome.Like("pruden%"));

            #endregion

            #region Cadastra Cliente

            var cliente = new PessoaFisica()
            {
                CPF = "12345678901",
                Nome = "Tarcisio",
                LimiteCredito = 500,
                Endereco = "Av. Visconde de Guarapuava, 347",
                Cidade = prudentopolis
            };

            Console.WriteLine(cliente.Save() ? "Cliente Salvo" : "Erro");
            Console.WriteLine("Cliente: " + cliente);

            #endregion

            #region Cadastro Produto

            var produto = new Produto()
            {
                Categoria = new Categoria() { Descricao = "Rações" },
                Fracionado = true,
                CodigoBarras = "789321147823",
                Descricao = "Ração Special Dog Carne 20kg",
                PrecoCusto = 110.29m,
                PrecoVenda = 180m,
                Unidade = new Unidade() { Descricao = "Saco" },
            };
            Console.WriteLine(produto.Save() ? "Produto Salvo" : "Erro");
            Console.WriteLine("Produto: " + produto);

            #endregion

            #region Cria Pedido

            var item = new Item();
            item.Produto = produto;
            item.Quantidade = 3;

            var pedido = new Pedido();
            pedido.Cliente = cliente;
            pedido.Itens = new PList<Item>();
            pedido.Itens.Add(item);
            pedido.ValorTotal = pedido.Itens.Sum(item => item.Total);
            pedido.Data = DateTime.Now;

            Console.WriteLine(pedido.Save() ? "Pedido Salvo" : "Erro");
            Console.WriteLine("Pedido: " + pedido);

            #endregion

            #region Deleta Tudo

            Console.WriteLine(pedido.Delete() ? "Pedido Excluído" : "Erro");
            Console.WriteLine(cliente.Delete() ? "Cliente Excluído" : "Erro");
            Console.WriteLine(produto.Delete() ? "Produto Excluído" : "Erro");
            Console.WriteLine(produto.Categoria.Delete() ? "Categoria Excluída" : "Erro");
            Console.WriteLine(produto.Unidade.Delete() ? "Unidade Excluída" : "Erro");

            #endregion
        }

        private void ev(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine(e.NewSize.ToString());
        }

        private void NovaVenda(object sender, RoutedEventArgs e)
        {
            var viewPedido = new ViewPedido();
            viewPedido.Owner = this;
            viewPedido.Show();
        }
        private void NovaFatura(object sender, RoutedEventArgs e)
        {
            var viewFatura = new ViewFatura();
            viewFatura.Owner = this;
            viewFatura.Show();
        }
    }
}
