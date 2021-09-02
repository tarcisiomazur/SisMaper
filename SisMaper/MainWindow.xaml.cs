using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using MySqlConnector;
using Persistence;
using SisMaper.Models;

namespace SisMaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport(@"kernel32.dll")]
        static extern bool AllocConsole();
        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            Persistence.Persistence.Init(new MySqlProtocol());
            Test();
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
    }
}
