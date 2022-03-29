using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Models.Views;
using SisMaper.Tools;
using SisMaper.Views;

namespace SisMaper.ViewModel;

public class PedidoViewModel : BaseViewModel, IDataErrorInfo
{
    private ListarProdutos? _produtoSelecionado;

    public PedidoViewModel(long? pedidoId)
    {
        PersistenceContext = new PersistenceContext();
        Naturezas = PersistenceContext.All<Natureza>();
        Pedido = PersistenceContext.Get<Pedido>(pedidoId);
        NotaFiscalSelecionada = Pedido.NotasFiscais.FirstOrDefault(nf => nf.Situacao.BeEmitted());
        NotasFiscaisView = CollectionViewSource.GetDefaultView(Pedido.NotasFiscais);
        NotasFiscaisView.GroupDescriptions.Add(new PropertyGroupDescription("Chave"));

        if (!IsOpen) // Se o pedido está fechado não é preciso carregar Clientes, Categorias, etc
        {
            if (Pedido.Cliente is null)
            {
                return;
            }

            Clientes = new List<ListarClientes>
            {
                new(Pedido.Cliente)
            };
            return;
        }

        Clientes = View.Execute<ListarClientes>();
        Produtos = View.Execute<ListarProdutos>();
        if (Pedido.Id == 0)
        {
            Pedido.Usuario = Main.Usuario;
            Pedido.Natureza = Naturezas.FirstOrDefault();
        }

        ProdutosAtivos = Produtos.Where(p => p.Inativo == false);
        NovoItem = new Item {Pedido = Pedido, Context = PersistenceContext};
        Pedido.Itens.ListChanged += UpdatePedido;
    }

    #region Actions

    public event Action? Cancel;

    public event Action? Save;

    public event Action<Action<ViewMetodoPagamento.OptionPagamento>>? OpenMetodoPagamento;

    public event Action<BuscarProdutoViewModel>? OpenBuscarProduto;

    public event Action<CrudClienteViewModel, bool>? OpenCrudCliente;

    public event Action<EditarItemViewModel>? OpenEditarItem;

    public event Action<EscolherLoteViewModel>? OpenEscolherLote;

    public event Action<FaturaViewModel>? OpenFatura;

    public event Action<SelecionarFaturaViewModel>? OpenSelecionarFatura;

    #endregion

    #region Properties

    public bool HasFatura => Pedido.Fatura is not null;

    public bool HasNotaFiscal => Pedido.NotasFiscais.Count > 0;

    public bool IsOpen => Pedido.Status is Pedido.Pedido_Status.Aberto;

    private List<ListarProdutos> Produtos { get; }

    private PersistenceContext PersistenceContext { get; }

    [Description("Test-Property")] public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            if (columnName is nameof(Data) or nameof(NaturezaSelecionada) && IsOpen && Data is null)
            {
                return "Campo Obrigatório";
            }

            return "";
        }
    }

    public bool FaturaTabItemIsSelected { get; set; }

    public bool NotaFiscalTabItemIsSelected { get; set; }

    public DateTime? Data
    {
        get => Pedido.Data;
        set => Pedido.Data = value;
    }

    public ICollectionView NotasFiscaisView { get; set; }

    public IEnumerable<ListarProdutos> ProdutosAtivos { get; set; }

    public Item ItemSelecionado { get; set; }

    private Item NovoItem { get; set; }

    public List<ListarClientes>? Clientes { get; set; }

    public ListarClientes? ClienteSelecionado
    {
        get => Clientes?.FirstOrDefault(c => c.Id == Pedido.Cliente?.Id);
        set => SetCliente(value);
    }

    public ListarProdutos? ProdutoSelecionado
    {
        set => SetProduto(value);
        get => _produtoSelecionado;
    }

    public Natureza? NaturezaSelecionada
    {
        get => Pedido.Natureza;
        set => Pedido.Natureza = value;
    }

    public NotaFiscal? NotaFiscalSelecionada { get; set; }

    public Pedido Pedido { get; set; }

    public PList<Natureza> Naturezas { get; set; }

    public string QuantidadeItem { get; set; } = "";

    public string TextoFiltro { get; set; } = "";

    #endregion

    #region ICommands

    public FullCmd<string> EmitirNotaFiscalCmd => new(NewNF);

    public FullCmd<TextChangedEventArgs> VerificaQuantidadeCmd => new(VerificaQuantidadeEventHandler);

    public SimpleCommand AbrirFaturaCmd => new(AbrirFaturaPedido);

    public SimpleCommand AddItemCmd => new(AddItem, IsPedidoAberto());

    public SimpleCommand AdicionarClienteCmd => new(AdicionarCliente);

    public SimpleCommand AdicionarClienteContextMenuCmd => new(ShowContextMenu, IsPedidoAberto());

    public SimpleCommand AtualizarSituacaoCmd => new(AtualizarSituacaoNF, _ => NotaFiscalSelecionada is not null);

    public SimpleCommand CancelarCmd => new(() => Cancel?.Invoke(), IsPedidoAberto());

    public SimpleCommand EditarClienteCmd => new(EditarCliente, IsPedidoAberto());

    public SimpleCommand EditarItemCmd => new(EditarItem);

    public SimpleCommand EmitirNotaFiscalContextMenuCmd => new(ShowContextMenu, IsPedidoAberto(false));

    public SimpleCommand OpenBuscarProdutoCmd => new(AbrirBuscarProduto, IsPedidoAberto());

    public SimpleCommand ReceberCmd => new(ReceberPedido, _ => Pedido.IsOpen() && Pedido.Itens.Count > 0);

    public SimpleCommand RemoverItemCmd => new(RemoverItem);

    public SimpleCommand SalvarCmd => new(SavePedido, IsPedidoAberto());

    #endregion

    private Func<bool> IsPedidoAberto(bool x = true)
    {
        return () => Pedido.IsOpen() == x;
    }

    private void UpdatePedido(object? sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType is > ListChangedType.Reset and < ListChangedType.PropertyDescriptorAdded ||
            e.PropertyDescriptor?.Name == nameof(Item.Total))
        {
            SumPedido();
        }
    }

    private void SumPedido()
    {
        Pedido.ValorTotal = Pedido.Itens.Sum(i => i.Total);
    }

    private void SavePedido()
    {
        if (Pedido.Itens.Count == 0)
        {
            OnShowMessage("Salvar Pedido", "O pedido deve conter um ou mais itens!");
            return;
        }

        try
        {
            if (!Pedido.Save())
            {
                OnShowMessage("Salvar Pedido", "O pedido não pode ser salvo!");
            }

            Save?.Invoke();
        }
        catch (PersistenceException ex)
        {
            if (ex.ErrorCode == SQLException.ErrorCodeVersion)
            {
                OnShowMessage("Salvar Pedido", "O pedido não pode ser salvo pois está desatualizado!");
                Cancel?.Invoke();
            }
            else
            {
                OnShowMessage("Fatal Error", ex.Message);
            }
        }
    }

    private void AbrirFaturaPedido()
    {
        OpenFatura?.Invoke(new FaturaViewModel(Pedido.Fatura.Id));
    }

    private void AtualizarSituacaoNF()
    {
        var key = NotaFiscalSelecionada.Chave;
        if (key is not {Length: 44})
        {
            key = NotaFiscalSelecionada.UUID;
            if (key is not {Length: 36})
            {
                return;
            }
        }

        var result = WebManiaConnector.Consultar(key);
        NFConverter.Merge(result, NotaFiscalSelecionada);
        NotaFiscalSelecionada.Save();
    }

    private void AbrirBuscarProduto()
    {
        var vm = new BuscarProdutoViewModel(Produtos)
        {
            TextoFiltro = TextoFiltro
        };
        OpenBuscarProduto?.Invoke(vm);
        ProdutoSelecionado = vm.ProdutoSelecionado;
        if (ProdutoSelecionado is null) return;
        NovoItem.Produto = PersistenceContext.Get<Produto>(ProdutoSelecionado.Id);
        if (NovoItem.Produto != null && NovoItem.Quantidade > 0)
        {
            AddItem();
        }
    }

    private void AddItem()
    {
        if (NovoItem.Produto is null)
        {
            ListarProdutos? prod = null;
            if (long.TryParse(TextoFiltro, out var num))
            {
                prod = ProdutosAtivos.FirstOrDefault(p => p.Id == num);
            }

            if (prod is null && TextoFiltro.Length > 3)
            {
                prod = ProdutosAtivos.FirstOrDefault(p => p.CodigoBarras == TextoFiltro);
            }

            if (prod is not null)
            {
                AbrirBuscarProduto();
                return;
            }

            NovoItem.Produto = PersistenceContext.Get<Produto>(prod.Id);
        }

        if (NovoItem.Quantidade == 0) NovoItem.Quantidade = 1;
        if (!NovoItem.Produto.Fracionado && !NovoItem.Quantidade.IsNatural())
        {
            OnShowMessage("Adicionar Item", "O item não aceita quantidade fracionada!");
            return;
        }

        NovoItem.Produto.Lotes.Load();
        if (NovoItem.Produto.Lotes?.Count > 0)
        {
            var vm = new EscolherLoteViewModel(NovoItem.Produto.Lotes);
            OpenEscolherLote?.Invoke(vm);
            if (vm.LoteSelecionado != null)
            {
                NovoItem.Lote = vm.LoteSelecionado;
            }
            else
            {
                return;
            }
        }

        NovoItem.Valor = NovoItem.Produto.PrecoVenda;
        Pedido.Itens.Add(NovoItem);
        SumPedido();
        NovoItem = new Item {Pedido = Pedido, Context = PersistenceContext};
        QuantidadeItem = "";
        TextoFiltro = "";
        ProdutoSelecionado = null;
    }

    private void ReceberPedido()
    {
        OpenMetodoPagamento?.Invoke(MetodoSelecionado);
    }

    private void MetodoSelecionado(ViewMetodoPagamento.OptionPagamento pgmto)
    {
        if (pgmto == ViewMetodoPagamento.OptionPagamento.Null)
        {
            return;
        }

        if (!Pedido.Save())
        {
            OnShowMessage("Salvar Pedido", "O pedido não pode ser fechado!");
            Cancel?.Invoke();
            return;
        }

        switch (pgmto)
        {
            case ViewMetodoPagamento.OptionPagamento.AVista:
                ProcessarPagamentoAVista();
                break;
            case ViewMetodoPagamento.OptionPagamento.NovaFatura:
                ProcessarPagamentoNovaFatura();
                break;
            case ViewMetodoPagamento.OptionPagamento.FaturaExistente:
                ProcessarPagamentoFaturaExistente();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Pedido.Load();
        RaisePropertyChanged(nameof(IsOpen));
    }

    private void ProcessarPagamentoFaturaExistente()
    {
        if (Pedido.Cliente is null)
        {
            OnShowMessage("Cliente não informado", "Um cliente deve ser informado no pedido");
            return;
        }

        if (!Pedido.Cliente.Faturas.Load())
        {
            OnShowMessage("Erro ao Buscar Faturas",
                $"Não foi possível buscar faturas para o cliente {Pedido.Cliente.Nome}.");
            return;
        }

        var faturas = Pedido.Cliente.Faturas
            .Where(f => f is {Status: Fatura.Fatura_Status.Aberta, Parcelas.Count: 0}).ToList();

        if (faturas.Count == 0)
        {
            OnShowMessage("Fatura não encontrada", "Nenhuma fatura aberta foi encontrada.");
            return;
        }

        var vm = new SelecionarFaturaViewModel(faturas);
        OpenSelecionarFatura?.Invoke(vm);
        if (vm.FaturaSelecionada is null)
        {
            return;
        }

        Pedido.Fatura = vm.FaturaSelecionada;
        Pedido.Fatura.Pedidos.Add(Pedido);
        Pedido.Status = Pedido.Pedido_Status.Fechado;
        if (!Pedido.Fatura.Save())
        {
            OnShowMessage("Salvar Fatura", "A fatura do pedido não pode ser salva!");
            Cancel?.Invoke();
        }

        {
            Pedido.Fatura.Load();
            SystemSounds.Beep.Play();
            AbrirFaturaPedido();
        }
    }

    private void ProcessarPagamentoNovaFatura()
    {
        if (Pedido.Cliente == null)
        {
            OnShowMessage("Cliente não informado", "Um cliente deve ser informado no pedido");
            return;
        }

        Pedido.Fatura = CreateFatura();
        Pedido.Status = Pedido.Pedido_Status.Fechado;
        if (!Pedido.Fatura.Save())
        {
            OnShowMessage("Salvar Fatura", "A fatura do pedido não pode ser salva!");
            Cancel?.Invoke();
        }
        else
        {
            Pedido.Fatura.Load();
            SystemSounds.Beep.Play();
            AbrirFaturaPedido();
        }
    }

    private void ProcessarPagamentoAVista()
    {
        Pedido.Fatura = CreateFatura();
        Pedido.Fatura.Parcelas.Add(new Parcela
        {
            Indice = 0,
            Status = Parcela.Status_Parcela.Pago,
            Valor = Pedido.ValorTotal,
            DataVencimento = DateTime.Now,
            DataPagamento = DateTime.Now,
            Context = PersistenceContext,
            Pagamentos = new PList<Pagamento>(new[]
            {
                new Pagamento
                {
                    ValorPagamento = Pedido.ValorTotal,
                    Usuario = Main.Usuario,
                    TipoPagamento = Pagamento.EnumTipoPagamento.Moeda,
                    Context = PersistenceContext
                }
            })
        });
        Pedido.Status = Pedido.Pedido_Status.Fechado;
        if (Pedido.Fatura.Save())
        {
            Pedido.Fatura.Status = Fatura.Fatura_Status.Fechada;
            if (Pedido.Fatura.Save())
            {
                Pedido.Fatura.Load();
                OnShowMessage("Receber Pedido", "O pedido foi salvo e recebido!");
                RaisePropertyChanged(nameof(Pedido));
                RaisePropertyChanged(nameof(HasFatura));
                SystemSounds.Beep.Play();
                return;
            }
        }

        OnShowMessage("Salvar Fatura", "A fatura do pedido não pode ser salva!");
        Cancel?.Invoke();
    }

    private Fatura CreateFatura()
    {
        var fatura = new Fatura
        {
            Cliente = Pedido.Cliente,
            Data = DateTime.Now,
            Context = PersistenceContext
        };
        fatura.Parcelas = new PList<Parcela>
        {
            Context = PersistenceContext
        };
        fatura.Pedidos = new PList<Pedido>(new[] {Pedido})
        {
            Context = PersistenceContext
        };
        return fatura;
    }

    private void NewNF(string obj)
    {
        var isNFC = obj.Equals("NFC-e");
        if (Pedido.NotasFiscais.FirstOrDefault(n => n.Situacao.BeEmitted()) is { } _nf)
        {
            OnShowMessage("Emitir Nota Fiscal",
                "O pedido possui uma Nota Fiscal" +
                (_nf.Situacao.IsAprovado()
                    ? $" nº{_nf.Serie}-{_nf.Numero} com chave {_nf.Chave}. Aprovada em {_nf.DataEmissao}."
                    : $" com a situação {_nf.Situacao}. Caso deseje reemitir, atualize a Nota Fiscal antes de uma nova emissão!"));
            return;
        }

        var nf = new NotaFiscal();
        nf.Pedido = Pedido;
        if (!nf.Save())
        {
            OnShowMessage("Erro", "Ocorreu um ao gerar a numeração da próxima emitir Nota Fiscal");
            return;
        }

        INotaFiscal apiNf;
        if (isNFC)
        {
            apiNf = new NotaFiscalConsumidor(nf);
        }
        else
        {
            apiNf = new NotaFiscalEletronica(nf);
        }

        if (Pedido.Cliente is { } and not PessoaFisica and not PessoaJuridica)
        {
            if (PersistenceContext.TryGet<PessoaFisica>(Pedido.Cliente.Id) is var pf and not null)
            {
                Pedido.Cliente = pf;
            }
            else if (PersistenceContext.TryGet<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
            {
                Pedido.Cliente = pj;
            }
        }

        var result = apiNf.BuildJsonDefault();
        if (result != "OK")
        {
            OnShowMessage("Erro ao validar a Nota Fiscal", $"Erro ao validar a nota fiscal: {result}");
            nf.Delete();
            return;
        }

        EmitindoNotaFiscal(apiNf, nf);
    }

    private async void EmitindoNotaFiscal(INotaFiscal apiNf, NotaFiscal nf)
    {
        var pdc = await OnShowProgressAsync(new MetroDialogSettings {NegativeButtonText = "Fechar"});
        pdc?.SetTitle("Emitindo Nota Fiscal");
        pdc?.SetMessage("Aguarde. A Nota Fiscal está em processo de emissão.");
        pdc?.SetIndeterminate();

        var task = Task.Factory.StartNew(apiNf.Emit);
        pdc?.SetCancelableDelayed(true, 5000);
        pdc?.SetMessageDelayed(
            "A emissão está demorando mais do que o normal. Verifique a sua Conexão com a Internet", 5000);
        await task.ContinueWith(_ => pdc?.CloseAsync());
        if (!task.Result || apiNf.NF_Result is null)
        {
            OnShowMessage("Erro ao emitir Nota Fiscal", "Erro ao enviar a Nota Fiscal para emissão");
            nf.Delete();
            return;
        }

        if (apiNf.NF_Result is {Error: { }})
        {
            OnShowMessage("Erro ao emitir Nota Fiscal",
                $"Erro ao processar a Nota Fiscal: {apiNf.NF_Result.Error}");
            nf.Delete();
            return;
        }

        NFConverter.Merge(apiNf.NF_Result, nf);
        Pedido.NotasFiscais.Add(nf);
        NotaFiscalSelecionada = nf;
        if (nf.Save())
        {
            OnShowMessage("Emissão de Nota Fiscal",
                $"Nota Fiscal enviada para emissão. Situação :{nf.Situacao}");
            RaisePropertyChanged(nameof(HasNotaFiscal));
            NotaFiscalTabItemIsSelected = true;
            return;
        }

        OnShowMessage("Erro ao emitir Nota Fiscal",
            $"Um erro ocorreu ao salvar a nota fiscal emitida. Situação: {nf.Situacao}");
    }

    private void ShowContextMenu(object obj)
    {
        if (obj is Button {ContextMenu: { }} button)
        {
            button.ContextMenu.DataContext = button.DataContext;
            button.ContextMenu.IsOpen = true;
        }
    }

    private void AdicionarCliente(object obj)
    {
        var isPf = obj.Equals("PF");
        var vm = new CrudClienteViewModel(null);
        OpenCrudCliente?.Invoke(vm, isPf);

        if ((Cliente) (isPf ? vm.PessoaFisica : vm.PessoaJuridica) is {Id: > 0} cliente)
        {
            Pedido.Cliente = PersistenceContext.GetOrRefresh<Cliente>(cliente.Id);
            UpdateSelectionCliente();
        }
    }

    private void EditarCliente()
    {
        if (Pedido.Cliente is null) return;
        switch (Pedido.Cliente)
        {
            case PessoaFisica pf:
                OpenCrudCliente?.Invoke(
                    new CrudClienteViewModel(Clientes.FirstOrDefault(cliente => cliente.Id == pf.Id)), true);
                Pedido.Cliente = PersistenceContext.GetOrRefresh<PessoaFisica>(pf.Id);
                break;
            case PessoaJuridica pj:
                OpenCrudCliente?.Invoke(
                    new CrudClienteViewModel(Clientes.FirstOrDefault(cliente => cliente.Id == pj.Id)), false);
                Pedido.Cliente = PersistenceContext.GetOrRefresh<PessoaJuridica>(pj.Id);
                break;
            default:
                OnShowMessage("Editar Cliente", "Não foi possível editar este cliente");
                break;
        }

        UpdateSelectionCliente();
    }

    private void UpdateSelectionCliente()
    {
        Clientes = View.Execute<ListarClientes>();
        RaisePropertyChanged(nameof(ClienteSelecionado));
    }

    private void SetCliente(ListarClientes? cliente)
    {
        Pedido.Cliente = cliente?.Tipo switch
        {
            ListarClientes.Pessoa.Fisica => PersistenceContext.Get<PessoaFisica>(cliente.Id),
            ListarClientes.Pessoa.Juridica => PersistenceContext.Get<PessoaJuridica>(cliente.Id),
            _ => Pedido.Cliente
        };
    }

    private void SetProduto(ListarProdutos? produto)
    {
        _produtoSelecionado = produto;
        NovoItem.Produto = produto is not null ? PersistenceContext.Get<Produto>(produto.Id) : null;
    }

    private void VerificaQuantidadeEventHandler(TextChangedEventArgs e)
    {
        var quantidade = NovoItem.Quantidade;
        if (!Regex.IsMatch(QuantidadeItem, @"^(\d*,?\d*)?$") || !string.IsNullOrEmpty(QuantidadeItem) &&
            !double.TryParse(QuantidadeItem, out quantidade) &&
            quantidade is >= 0 and <= 10e10)
        {
            SystemSounds.Beep.Play();
            QuantidadeItem = NovoItem.Quantidade.ToString();
            e.Handled = true;
        }
        else
        {
            NovoItem.Quantidade = quantidade;
        }
    }

    public void EditarItem()
    {
        if (ItemSelecionado is null) return;
        var vm = new EditarItemViewModel(ItemSelecionado);
        OpenEditarItem?.Invoke(vm);
    }

    public void RemoverItem()
    {
        if (ItemSelecionado is null) return;
        Pedido.Itens.Remove(ItemSelecionado);
    }
}