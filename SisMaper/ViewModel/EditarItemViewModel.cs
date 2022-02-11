using System;
using System.ComponentModel;
using Persistence;
using PropertyChanged;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class EditarItemViewModel : BaseViewModel, IDataErrorInfo
{
    public EditarItemViewModel(Item item)
    {
        item.Produto?.Lotes?.Load();
        ItemChanged = (Item) item.Clone(PersistenceContext);
    }

    #region Actions

    public event Action? Cancel;

    public event Action? Save;

    public event Action<BuscarProdutoViewModel>? OpenBuscarProduto;

    #endregion

    #region Properties

    public bool HasLotes => ItemChanged.Produto?.Lotes?.Count > 0;

    private PersistenceContext PersistenceContext { get; } = new();

    [Description("Test-Property")] public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            if (columnName is nameof(Lote) && HasLotes && ItemChanged.Lote is null)
            {
                return "Selecione um Lote";
            }

            if (columnName is nameof(Quantidade) && !Quantidade.IsNatural() && !ItemChanged.Produto.Fracionado)
            {
                return "A Quantidade deve ser Inteira, pois o Produto não permite venda Fracionada";
            }

            return "";
        }
    }

    public decimal ValorTotal
    {
        get => ItemChanged.Total;
        set => DescontoPorcentagem = 100 * (1 - (double) value / ((double) ItemChanged.Valor * Quantidade));
    }

    [AlsoNotifyFor(nameof(ValorTotal))]
    public double DescontoPorcentagem
    {
        get => ItemChanged.DescontoPorcentagem;
        set => ItemChanged.DescontoPorcentagem = value;
    }

    [AlsoNotifyFor(nameof(ValorTotal))]
    public double Quantidade
    {
        get => ItemChanged.Quantidade;
        set => ItemChanged.Quantidade = value;
    }

    public double ValorUnitario
    {
        get => (1 - DescontoPorcentagem / 100) * (double) ItemChanged.Valor;
        set => DescontoPorcentagem = 100 * (1 - value / (double) ItemChanged.Valor);
    }

    public Item ItemChanged { get; set; }

    public Lote? Lote
    {
        get => ItemChanged.Lote;
        set => ItemChanged.Lote = value;
    }

    #endregion

    #region ICommands

    public SimpleCommand CancelarCmd => new(() => Cancel?.Invoke());

    public SimpleCommand OpenBuscarProdutoCmd => new(AbrirBuscarProduto);

    public SimpleCommand SalvarCmd => new(Salvar);

    #endregion

    private void Salvar()
    {
        Save?.Invoke();
    }

    private void AbrirBuscarProduto()
    {
        var vm = new BuscarProdutoViewModel();
        OpenBuscarProduto?.Invoke(vm);
        var ProdutoSelecionado = vm.ProdutoSelecionado;
        if (ProdutoSelecionado is null) return;
        ItemChanged.Produto = PersistenceContext.Get<Produto>(ProdutoSelecionado.Id);
        ItemChanged.Produto?.Lotes?.Load();
        ItemChanged.Valor = ItemChanged.Produto.PrecoVenda;
        ItemChanged.DescontoPorcentagem = 0;
        Lote = null;
        RaisePropertyChanged(nameof(Lote));
        RaisePropertyChanged(nameof(HasLotes));
    }
}