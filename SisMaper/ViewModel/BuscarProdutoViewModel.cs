using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SisMaper.Models.Views;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class BuscarProdutoViewModel : BaseViewModel
{
    public BuscarProdutoViewModel(List<ListarProdutos> produtos)
    {
        PropertyChanged += UpdateFilter;
        ProdutosFiltrados = new ListCollectionView(produtos)
        {
            Filter = Filtro
        };
        Categorias = new SortedSet<string>(produtos.Select(p => p.Categoria));
    }
    

    #region Actions

    public event Action? Cancel;

    public event Action? Select;

    #endregion

    #region Properties

    public bool? Inativos { get; set; } = false;

    public ListCollectionView ProdutosFiltrados { get; set; }

    public ListarProdutos? ProdutoSelecionado { get; set; }

    public ListarProdutos? Selecionado { get; set; }

    public SortedSet<string> Categorias { get; set; }

    public string TextoFiltro { get; set; } = "";

    public string? CategoriaSelecionada { get; set; }

    #endregion

    #region ICommands

    public SimpleCommand CancelarCmd => new(_ => Cancel?.Invoke());

    public SimpleCommand SelecionarCmd => new(Selecionar, _ => Selecionado is not null);

    #endregion

    private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CategoriaSelecionada) or nameof(TextoFiltro) or nameof(
                Inativos))
        {
            ProdutosFiltrados.Refresh();
        }
    }

    private void Selecionar()
    {
        ProdutoSelecionado = Selecionado;
        Select?.Invoke();
    }

    private bool Filtro(object obj)
    {
        return obj is ListarProdutos p &&
               (TextoFiltro.IsContainedIn(p.Descricao) || TextoFiltro.IsContainedIn(p.CodigoBarras) ||
                TextoFiltro.IsContainedIn(p.Id.ToString())) && (Inativos is null || Inativos == p.Inativo) &&
               (CategoriaSelecionada == null || p.Categoria == CategoriaSelecionada);
    }
}