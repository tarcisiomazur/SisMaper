using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.ViewModel
{
    public class BuscarProdutoViewModel : BaseViewModel
    {
        public PList<Produto> Produtos { get; set; }
        public IEnumerable<Produto> ProdutosFiltrados { get; set; }
        public Produto? ProdutoSelecionado { get; set; }
        public Categoria? CategoriaSelecionada { get; set; }
        public PList<Categoria> Categorias { get; set; }
        public string? TextoFiltro { get; set; }
        public bool? Inativos { get; set; } = false;

        public PersistenceContext PersistenceContext { get; set; }

        public BuscarProdutoViewModel()
        {
            PropertyChanged += UpdateFilter;
        }

        public void Initialize(PList<Produto> produtos)
        {
            Produtos = produtos;
            PersistenceContext = Produtos.Context;
            Categorias = PersistenceContext.Get<Categoria>("ID>0");
        }

        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(CategoriaSelecionada) or nameof(Produtos) or nameof(TextoFiltro) or nameof(Inativos))
            {
                ProdutosFiltrados = Produtos.Where(p =>
                    (string.IsNullOrEmpty(TextoFiltro) ||
                     !string.IsNullOrEmpty(p.Descricao) && p.Descricao.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase) ||
                     !string.IsNullOrEmpty(p.CodigoBarras) && p.CodigoBarras.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase)) &&
                    (CategoriaSelecionada == null || p.Categoria == CategoriaSelecionada) &&
                    (Inativos is null || Inativos == p.Inativo)
                );
            }
        }
    }
}