using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.Views;

namespace SisMaper.ViewModel
{
    public class PedidoViewModel : BaseViewModel
    {
        #region Properties

        public Pedido Pedido { get; set; }
        public PList<Cliente> Clientes { get; set; }
        public PList<Natureza> Naturezas { get; set; }
        public PList<Produto> Produtos { get; set; }
        public Item NovoItem { get; set; }
        private PersistenceContext PersistenceContext { get; set; }

        #endregion

        #region UIProperties

        public IEnumerable<Produto> ProdutosAtivos { get; set; }
        public ICollectionView NotasFiscaisView { get; set; }
        public NotaFiscal? NotaFiscalSelecionada { get; set; }
        public bool HasFatura => Pedido.Fatura is not null;
        public bool HasNotaFiscal => Pedido.NotasFiscais.Count > 0;
        public bool FaturaTabItemIsSelected { get; set; }
        public bool NotaFiscalTabItemIsSelected { get; set; }
        public string QuantidadeItem { get; set; } = "";
        private Func<bool> IsPedidoAberto(bool x = true) => () => Pedido.IsOpen() == x;

        #endregion

        #region ICommands

        public FullCmd<TextChangedEventArgs> VerificaQuantidadeCmd => new(VerificaQuantidadeEventHandler);
        public FullCmd<KeyEventArgs> PreviewKeyDownAddItemCmd => new(AdicionarItemEventHandler, IsPedidoAberto());
        public SimpleCommand AdicionarCmd => new(AddItem);
        public SimpleCommand EditarClienteCmd => new(EditCliente, IsPedidoAberto());
        public SimpleCommand AdicionarClienteCmd => new(NewCliente);
        public SimpleCommand AdicionarClienteContextMenuCmd => new(ShowContextMenu, IsPedidoAberto());
        public SimpleCommand EmitirNotaFiscalContextMenuCmd => new(ShowContextMenu, IsPedidoAberto(false));
        public FullCmd<string> EmitirNotaFiscalCmd => new(NewNF);
        public SimpleCommand SalvarCmd => new(SavePedido, IsPedidoAberto());
        public SimpleCommand CancelarCmd => new(CancelPedido, IsPedidoAberto());
        public SimpleCommand AtualizarSituacaoCmd => new(AtualizarSituacaoNF, _ => NotaFiscalSelecionada is not null);
        public SimpleCommand AbrirFaturaCmd => new(AbrirFaturaPedido);
        public SimpleCommand ReceberCmd => new(ReceberPedido, _ => Pedido.IsOpen() && Pedido.Itens.Count > 0);
        public SimpleCommand OpenBuscarProdutoCmd => new(AbrirBuscarProduto, IsPedidoAberto());

        #endregion

        #region Actions

        public event Action? Cancel;
        public event Action? Save;

        #endregion
        
        public PedidoViewModel(long? pedidoId)
        {
            PersistenceContext = new PersistenceContext();
            Naturezas = PersistenceContext.Get<Natureza>("ID>0");
            Clientes = new PList<Cliente>();
            Clientes.AddRange(PersistenceContext.Get<PessoaFisica>("Cliente_ID>0"));
            Clientes.AddRange(PersistenceContext.Get<PessoaJuridica>("Cliente_ID>0"));
            Produtos = PersistenceContext.Get<Produto>("ID>0");
            ProdutosAtivos = Produtos.Where(p => p.Inativo == false);

            Pedido = PersistenceContext.Get<Pedido>(pedidoId);
            if (Pedido.Id == 0)
            {
                Pedido.Usuario = Main.Usuario;
                Pedido.Natureza = Naturezas.FirstOrDefault();
            }

            NotaFiscalSelecionada = Pedido.NotasFiscais.FirstOrDefault(nf => nf.Situacao.BeEmitted());

            NotasFiscaisView = CollectionViewSource.GetDefaultView(Pedido.NotasFiscais);
            NotasFiscaisView.GroupDescriptions.Add(new PropertyGroupDescription("Chave"));

            NovoItem = new Item {Pedido = Pedido, Context = PersistenceContext};
            Pedido.Itens.ListChanged += UpdatePedido;
        }

        private void UpdatePedido(object? sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType is > ListChangedType.Reset and < ListChangedType.PropertyDescriptorAdded ||
                e.PropertyDescriptor?.Name == nameof(Item.Total))
            {
                SumPedido();
            }
        }

        private void AdicionarItemEventHandler(KeyEventArgs obj)
        {
            if (obj.Key == Key.Enter)
                AddItem();
        }

        private void SumPedido() => Pedido.ValorTotal = Pedido.Itens.Sum(i => i.Total);

        private void SavePedido()
        {
            if (Pedido.Itens.Count == 0)
            {
                ShowMessage("Salvar Pedido", "O pedido deve conter um ou mais itens!");
                return;
            }

            try
            {
                if (!Pedido.Save())
                {
                    ShowMessage("Salvar Pedido", "O pedido não pode ser salvo!");
                }

                Save?.Invoke();
            }
            catch (PersistenceException ex)
            {
                if (ex.ErrorCode == SQLException.ErrorCodeVersion)
                {
                    ShowMessage("Salvar Pedido", "O pedido não pode ser salvo pois está desatualizado!");
                    Cancel?.Invoke();
                }
            }
        }

        private void AbrirFaturaPedido()
        {
            var viewFatura = new ViewFatura {DataContext = new FaturaViewModel(Pedido.Fatura)};
            Save?.Invoke();
            viewFatura.ShowDialog();
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

        private void CancelPedido()
        {
            Cancel?.Invoke();
        }

        private void AbrirBuscarProduto()
        {
            var view = new ViewBuscarProduto(Produtos);
            if (view.ShowDialog().IsTrue() && view.ViewModel.ProdutoSelecionado != null)
            {
                NovoItem.Produto = view.ViewModel.ProdutoSelecionado;
            }
        }

        private void AddItem()
        {
            if (NovoItem.Produto is not null)
            {
                if (NovoItem.Quantidade == 0) NovoItem.Quantidade = 1;
                if (!NovoItem.Produto.Fracionado && !NovoItem.Quantidade.IsNatural())
                {
                    ShowMessage("Adicionar Item", "O item não aceita quantidade fracionada!");
                    return;
                }

                NovoItem.Produto.Lotes.Load();
                if (NovoItem.Produto.Lotes?.Count > 0)
                {
                    var view = new ViewEscolherLote(NovoItem.Produto.Lotes);
                    if (view.ShowDialog().IsTrue())
                    {
                        NovoItem.Lote = view.ViewModel.LoteSelecionado;
                    }
                    else
                    {
                        return;
                    }
                }

                NovoItem.Valor = NovoItem.Produto.PrecoVenda;
                Pedido.Itens.Add(NovoItem);
                SumPedido();
                NovoItem = new Item() {Pedido = Pedido, Context = PersistenceContext};
                QuantidadeItem = "";
            }
            else
            {
                AbrirBuscarProduto();
            }
        }

        private void ReceberPedido()
        {
            var metodopagamento = new ViewMetodoPagamento();
            metodopagamento.Closed += MetodoSelecionado;
            metodopagamento.Show();
        }

        private void MetodoSelecionado(object? sender, EventArgs e)
        {
            if (sender is not ViewMetodoPagamento pgmto ||
                pgmto.Selecionado == ViewMetodoPagamento.OptionPagamento.Null) return;
            if (!Pedido.Save())
            {
                ShowMessage("Salvar Pedido", "O pedido não pode ser fechado!");
                Cancel?.Invoke();
                return;
            }

            switch (pgmto.Selecionado)
            {
                case ViewMetodoPagamento.OptionPagamento.AVista:
                    ProcessarPagamentoAVista();
                    break;
                case ViewMetodoPagamento.OptionPagamento.NovaFatura:
                    ProcessarPagamentoNovaFatura();
                    break;
                case ViewMetodoPagamento.OptionPagamento.FaturaExistente:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessarPagamentoNovaFatura()
        {
            if (Pedido.Cliente == null)
            {
                ShowMessage("Salvar Fatura",
                    "O cliente não pode estar em branco!");
                return;
            }

            Pedido.Fatura = CreateFatura();
            Pedido.Status = Pedido.Pedido_Status.Fechado;
            if (!Pedido.Fatura.Save())
            {
                ShowMessage("Salvar Fatura", "A fatura do pedido não pode ser salva!");
                Cancel?.Invoke();
            }
            else
            {
                SystemSounds.Beep.Play();
                AbrirFaturaPedido();
            }
        }

        private void ProcessarPagamentoAVista()
        {
            Pedido.Fatura = CreateFatura();
            Pedido.Fatura.Parcelas.Add(new Parcela()
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
                    ShowMessage("Receber Pedido", "O pedido foi salvo e recebido!");
                    RaisePropertyChanged(nameof(Pedido));
                    RaisePropertyChanged(nameof(HasFatura));
                    SystemSounds.Beep.Play();
                    return;
                }
            }

            ShowMessage("Salvar Fatura", "A fatura do pedido não pode ser salva!");
            Cancel?.Invoke();
        }

        private Fatura CreateFatura()
        {
            var fatura = new Fatura()
            {
                Cliente = Pedido.Cliente,
                ValorTotal = Pedido.ValorTotal,
                Data = DateTime.Now,
                Context = PersistenceContext,
            };
            fatura.Parcelas = new PList<Parcela>()
            {
                Context = PersistenceContext
            };
            fatura.Pedidos = new PList<Pedido>(new[] {Pedido})
            {
                Context = PersistenceContext
            };
            return fatura;
        }

        private void NewCliente(object obj)
        {
            var isPf = obj.Equals("PF");
            var dc = new CrudPessoaFisicaViewModel(null);
            var crud = new CrudPessoaFisica()
            {
                isSelectedPessoaFisicaTab = isPf,
                DataContext = dc
            }.ShowDialog();
            if (crud.IsTrue())
            {
                var newClient = (Cliente) (isPf ? dc.PessoaFisica : dc.PessoaJuridica);
                Pedido.Cliente = PersistenceContext.Get<Cliente>(newClient.Id);
            }
        }

        private void NewNF(string obj)
        {
            var isNFC = obj.Equals("NFC-e");
            if (Pedido.NotasFiscais.FirstOrDefault(n => n.Situacao.BeEmitted()) is { } _nf)
            {
                ShowMessage("Emitir Nota Fiscal",
                    $"O pedido possui uma Nota Fiscal" +
                    (_nf.Situacao.IsAprovado()
                        ? $" nº{_nf.Serie}-{_nf.Numero} com chave {_nf.Chave}. Aprovada em {_nf.DataEmissao}."
                        : $" com a situação {_nf.Situacao}. Caso deseje reemitir, atualize a Nota Fiscal antes de uma nova emissão!"));
                return;
            }

            var nf = new NotaFiscal();
            nf.Pedido = Pedido;
            if (!nf.Save())
            {
                ShowMessage("Erro", "Ocorreu um ao gerar a numeração da próxima emitir Nota Fiscal");
                return;
            }

            INotaFiscal apiNf;
            if (isNFC)
                apiNf = new NotaFiscalConsumidor(nf);
            else
                apiNf = new NotaFiscalEletronica(nf);

            if (Pedido.Cliente is { } and not PessoaFisica and not PessoaJuridica)
            {
                if (PersistenceContext.Get<PessoaFisica>(Pedido.Cliente.Id) is var pf and not null)
                    Pedido.Cliente = pf;
                else if (PersistenceContext.Get<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
                    Pedido.Cliente = pj;
            }

            var result = apiNf.BuildJsonDefault();
            if (result != "OK")
            {
                ShowMessage("Erro ao validar a Nota Fiscal", $"Erro ao validar a nota fiscal: {result}");
                nf.Delete();
                return;
            }

            EmitindoNotaFiscal(apiNf, nf);
        }

        private async Task EmitindoNotaFiscal(INotaFiscal apiNf, NotaFiscal nf)
        {
            var pdc = await ShowProgressAsync();
            pdc.SetTitle("Emitindo Nota Fiscal");
            pdc.SetMessage("Aguarde. A Nota Fiscal está em processo de emissão.");
            pdc.SetIndeterminate();
            var task = Task.Factory.StartNew(apiNf.Emit);
            pdc.SetCancelableDelayed(true, 3000);
            pdc.SetMessageDelayed("A emissão está demorando mais do que o normal. Verifique a sua Conexão com a Internet", 3000);
            await task.ContinueWith(_ => pdc.TryCloseAsync());

            if (!task.Result)
            {
                ShowMessage("Erro ao emitir Nota Fiscal", "Erro ao enviar a Nota Fiscal para emissão");
                nf.Delete();
                return;
            }

            if (apiNf.NF_Result is {Error: { }})
            {
                ShowMessage("Erro ao emitir Nota Fiscal",
                    $"Erro ao processar a Nota Fiscal: {apiNf.NF_Result.Error}");
                nf.Delete();
                return;
            }

            NFConverter.Merge(apiNf.NF_Result, nf);
            Pedido.NotasFiscais.Add(nf);
            NotaFiscalSelecionada = nf;
            if (nf.Save())
            {
                ShowMessage("Emissão de Nota Fiscal",
                    $"Nota Fiscal enviada para emissão. Situação :{nf.Situacao}");
                RaisePropertyChanged(nameof(HasNotaFiscal));
                NotaFiscalTabItemIsSelected = true;
                return;
            }

            ShowMessage("Erro ao emitir Nota Fiscal",
                $"Um erro ocorreu ao salvar a nota fiscal emitida. Situação: {nf.Situacao}");
        }


        private void ShowContextMenu(object obj)
        {
            if (obj is Button
            {
                ContextMenu:
                {
                }
            } button)
            {
                button.ContextMenu.DataContext = button.DataContext;
                button.ContextMenu.IsOpen = true;
            }
        }

        private void EditCliente()
        {
            if (DAO.Load<PessoaFisica>(Pedido.Cliente.Id) is var pf and not null)
            {
                new CrudPessoaFisica()
                    {isSelectedPessoaFisicaTab = true, DataContext = new CrudPessoaFisicaViewModel(pf)}.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<PessoaFisica>(pf.Id);
            }
            else if (DAO.Load<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
            {
                new CrudPessoaFisica()
                    {isSelectedPessoaFisicaTab = false, DataContext = new CrudPessoaFisicaViewModel(pj)}.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<PessoaJuridica>(pj.Id);
            }
            else
            {
                ShowMessage("Editar Cliente", "Não foi possível editar este cliente");
            }
        }

        private void VerificaQuantidadeEventHandler(TextChangedEventArgs e)
        {
            if (!Regex.IsMatch(QuantidadeItem, @"^(\d*,?\d*)?$") || !string.IsNullOrEmpty(QuantidadeItem) &&
                !double.TryParse(QuantidadeItem, out var quantidade) &&
                quantidade is >= 0 and <= 10e10)
            {
                SystemSounds.Beep.Play();
                QuantidadeItem = NovoItem.Quantidade.ToString();
                e.Handled = true;
            }
            else
            {
                NovoItem.Quantidade = 0;
            }
        }

        private void ShowMessage(string title, string message)
        {
            DialogCoordinator.Instance.ShowModalMessageExternal(this, title, message);
        }
        public async Task<ProgressDialogController> ShowProgressAsync()
        {
            var settings = new MetroDialogSettings()
            {
                NegativeButtonText = "Fechar"
            };
            var pdc = await DialogCoordinator.Instance.ShowProgressAsync(this, null, null, false, settings);
            return pdc;
        }
    }
}