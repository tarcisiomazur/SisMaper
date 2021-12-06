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
        #region Properties

        public PList<Produto> Produtos { get; set; }
        public PList<Categoria> Categorias { get; set; }
        public PersistenceContext PersistenceContext { get; set; }
        public Produto? ProdutoSelecionado { get; set; }

        #endregion

        #region UIProperties

        public IEnumerable<Produto> ProdutosFiltrados { get; set; }
        public Produto? Selecionado { get; set; }
        public Categoria? CategoriaSelecionada { get; set; }
        public string TextoFiltro { get; set; } = "";
        public bool? Inativos { get; set; } = false;

        #endregion


        #region ICommands

        public SimpleCommand CancelarCmd => new(_ => Cancel?.Invoke());
        public SimpleCommand SelecionarCmd => new(Selecionar, _ => Selecionado is not null);

        #endregion

        #region Actions

        public event Action? Cancel;
        public event Action? Select;

        #endregion

        public BuscarProdutoViewModel(PList<Produto> produtos)
        {
            PropertyChanged += UpdateFilter;
            Produtos = produtos;
            PersistenceContext = Produtos.Context;
            Categorias = PersistenceContext.Get<Categoria>("ID>0");
        }

        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(CategoriaSelecionada) or nameof(Produtos) or nameof(TextoFiltro) or nameof(
                Inativos))
            {
                ProdutosFiltrados = Produtos.Where(p =>
                    (TextoFiltro.IsContainedIn(p.Descricao) || TextoFiltro.IsContainedIn(p.CodigoBarras) ||
                     TextoFiltro.IsContainedIn(p.Id.ToString())) && (Inativos is null || Inativos == p.Inativo) &&
                    (CategoriaSelecionada == null || p.Categoria == CategoriaSelecionada)
                );
            }
        }

        private void Selecionar()
        {
            ProdutoSelecionado = Selecionado;
            Select?.Invoke();
        }
    }
}