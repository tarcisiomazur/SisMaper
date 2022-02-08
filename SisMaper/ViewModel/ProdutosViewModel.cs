using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SisMaper.Models.Views;
using MySql.Data.MySqlClient;

namespace SisMaper.ViewModel
{
    public class ProdutosViewModel : BaseViewModel
    {
        public ListarProdutos ProdutoSelecionado { get; set; }
        public List<ListarProdutos>? Produtos { get; set; }
        public IEnumerable<ListarProdutos> ProdutosFiltrados { get; set; }
        
        public Categoria? CategoriaSelecionada { get; set; }
        public PList<Categoria> Categorias { get; set; }
        public string? TextoFiltro { get; set; }
        public bool? Inativos { get; set; } = false;

        public SimpleCommand NovoProdutoCmd => new( () => OpenCrudProduto?.Invoke(new CrudProdutoViewModel(null)) );
        public SimpleCommand OpenCategoriasCmd => new( () => OpenCategoria?.Invoke() );
        public SimpleCommand OpenUnidadesCmd => new( () => OpenUnidade?.Invoke() );
        public SimpleCommand EditarProdutoCmd => new( () => OpenCrudProduto?.Invoke(new CrudProdutoViewModel(ProdutoSelecionado)), () => ProdutoSelecionado != null);
        public SimpleCommand DeletarProdutoCmd => new( ExcluirProduto, () => ProdutoSelecionado != null );

        public Action<CrudProdutoViewModel>? OpenCrudProduto { get; set; }
        public Action? OpenCategoria { get; set; }
        public Action? OpenUnidade { get; set; }


        public ProdutosViewModel()
        {
            Produtos = View.Execute<ListarProdutos>();       
            PropertyChanged += UpdateFilter;
            ProdutosFiltrados = Produtos;

        }

        public void Initialize(object? sender, EventArgs e)
        {
            Produtos = View.Execute<ListarProdutos>();
        }

        public void Clear(object? sender, EventArgs e)
        {
            Produtos = null;
        }

        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Produtos != null && e.PropertyName is nameof(CategoriaSelecionada) or nameof(Produtos) or nameof(TextoFiltro) or nameof(Inativos))
            {
                ProdutosFiltrados = Produtos.Where(p =>
                    (string.IsNullOrEmpty(TextoFiltro) ||
                     !string.IsNullOrEmpty(p.Descricao) && p.Descricao.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase) ||
                     !string.IsNullOrEmpty(p.CodigoBarras) && p.CodigoBarras.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase)) &&
                    (CategoriaSelecionada == null || p.Categoria == CategoriaSelecionada.Descricao) &&
                    (Inativos is null || Inativos == p.Inativo)
                );
            }
        }

        
        private void ExcluirProduto()
        {
            try
            {
                PList<Lote> lotesBanco = DAO.All<Lote>();
                PList<Lote> lotesParaDeletar = new PList<Lote>();

                foreach (Lote l in lotesBanco)
                {
                    if (l.Produto.Id.Equals(ProdutoSelecionado.Id))
                    {
                        lotesParaDeletar.Add(l);
                    }
                }


                MessageDialogResult confirmacao = OnShowMessage("Excluir Produto", "Deseja Excluir produto selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                if (confirmacao.Equals(MessageDialogResult.Affirmative))
                {
                    if (lotesParaDeletar.Count > 0)
                    {
                        lotesParaDeletar.DeleteAll();
                    }

                    var p = DAO.Load<Produto>(ProdutoSelecionado.Id);
                    if (p?.Delete() ?? false)
                    {
                        Initialize(null, EventArgs.Empty);
                        OnShowMessage("Excluir Produto", "Produto excluido com sucesso");
                    }
                }
                return;

            }
            catch (Exception ex) 
            {  
                if(ex.InnerException is not null && ex.InnerException is MySqlException)
                {
                    if (ex.InnerException.Message.StartsWith("Cannot delete or update a parent row")) OnShowMessage("Erro ao excluir produto", "O produto está vinculado em alguma venda");
                }
            }



        }
    }


}
