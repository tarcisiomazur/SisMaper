using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using MvvmCross.Core.ViewModels;
using Persistence;
using SisMaper.API.WebMania;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.Views;

namespace SisMaper.ViewModel
{
    public class PedidoViewModel : BaseViewModel
    {
        public double De { get; set; } = 28;

        public Pedido? Pedido { get; set; }
        public NotaFiscal? NotaFiscalSelecionada { get; set; }
        public ICollectionView NotasFiscaisView { get; set; }
        public bool HasFatura => Pedido?.Fatura is not null;
        public bool HasNotaFiscal => Pedido?.NotasFiscais?.Count > 0;
        public bool FaturaTabItemIsSelected { get; set; }
        public PList<Cliente> Clientes { get; set; }
        public PList<Natureza> Naturezas { get; set; }
        public IEnumerable<Produto> ProdutosAtivos { get; set; }
        public PList<Produto> Produtos { get; set; }
        public Item NovoItem { get; set; }
        public IMvxCommand VerificaQuantidade => new MvxCommand<TextChangedEventArgs>(VerificaQuantidadeEventHandler);

        public IMvxCommand PreviewKeyDownAddItem => new MvxCommand<KeyEventArgs>(AdicionarItemEventHandler,
            _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);

        public event Action? Cancel;
        public event Action? Save;
        public event Action? PrintWebBrowser;

        public SimpleCommand Adicionar => new(AddItem);

        public SimpleCommand EditarCliente => new(EditCliente,
            o => Pedido?.Cliente is not null && Pedido?.Status == Pedido.Pedido_Status.Aberto);

        public SimpleCommand AdicionarCliente => new(NewCliente);
        public SimpleCommand EmitirNF => new(NewNF);

        public SimpleCommand AdicionarClienteContextMenu =>
            new(ShowContextMenu, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);

        public SimpleCommand EmitirNotaFiscalContextMenu =>
            new(ShowContextMenu, _ => Pedido?.Status == Pedido.Pedido_Status.Fechado);

        public SimpleCommand Salvar => new(SavePedido, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);
        public SimpleCommand Cancelar => new(CancelPedido, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);
        public SimpleCommand AtualizarSituacao => new(AtualizarSituacaoNF, _ => NotaFiscalSelecionada is not null);
        public SimpleCommand Imprimir => new(ImprimirNF, _ => NotaFiscalSelecionada is not null);

        public SimpleCommand Receber => new(ReceberPedido,
            _ => Pedido?.Status == Pedido.Pedido_Status.Aberto && Pedido?.Itens.Count > 0);

        public SimpleCommand OpenBuscarProduto =>
            new(AbrirBuscarProduto, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);

        private IDialogCoordinator DialogCoordinator;

        private PersistenceContext PersistenceContext { get; set; }

        public string StringQuantidade { get; set; }

        public int TabSelecionada { get; set; }

        public PedidoViewModel()
        {
            PersistenceContext = new PersistenceContext();
        }

        public void Initialize(long? pedidoId)
        {
            Naturezas = PersistenceContext.Get<Natureza>("ID>0");
            Clientes = new PList<Cliente>();
            Clientes.AddRange(PersistenceContext.Get<PessoaFisica>("Cliente_ID>0"));
            Clientes.AddRange(PersistenceContext.Get<PessoaJuridica>("Cliente_ID>0"));
            Produtos = PersistenceContext.Get<Produto>("ID>0");
            ProdutosAtivos = Produtos.Where(p => p.Inativo == false);

            if (pedidoId == null)
            {
                Pedido = new Pedido
                {
                    Context = PersistenceContext,
                    Usuario = Main.Usuario,
                    Natureza = Naturezas.FirstOrDefault(),
                };
            }
            else
            {
                Pedido = PersistenceContext.Get<Pedido>(pedidoId);
            }

            NotaFiscalSelecionada = Pedido.NotasFiscais.FirstOrDefault(nf => nf.Situacao.BeEmitted());

            NotasFiscaisView = CollectionViewSource.GetDefaultView(Pedido.NotasFiscais);
            NotasFiscaisView.GroupDescriptions.Add(new PropertyGroupDescription("Chave"));

            DialogCoordinator = new DialogCoordinator();
            NovoItem = new Item() {Pedido = Pedido, Context = PersistenceContext};
            Pedido.Itens.ListChanged += UpdatePedido;
        }

        private void UpdatePedido(object sender, ListChangedEventArgs e)
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
                DialogCoordinator.ShowMessageAsync(this, "Salvar Pedido", "O pedido deve conter um ou mais itens!");
                return;
            }

            try
            {
                if (!Pedido.Save())
                {
                    DialogCoordinator.ShowMessageAsync(this, "Salvar Pedido", "O pedido não pode ser salvo!");
                }

                Save?.Invoke();
            }
            catch (PersistenceException ex)
            {
                if (ex.ErrorCode == SQLException.ErrorCodeVersion)
                {
                    DialogCoordinator.ShowMessageAsync(this, "Salvar Pedido",
                        "O pedido não pode ser salvo pois está desatualizado!");
                    Cancel?.Invoke();
                }
            }
        }

        private void ImprimirNF()
        {
            PrintWebBrowser?.Invoke();
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
                    DialogCoordinator.ShowMessageAsync(this, "Adicionar Item",
                        "O item não aceita quantidade fracionada!");
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
                StringQuantidade = "";
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
                DialogCoordinator.ShowMessageAsync(this, "Salvar Pedido", "O pedido não pode ser fechado!");
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
            if (Pedido?.Cliente == null)
            {
                DialogCoordinator.ShowMessageAsync(this, "Salvar Fatura",
                    "O cliente não pode ser nulo!");
                Cancel?.Invoke();
                return;
            }

            Pedido.Fatura = CreateFatura();
            Pedido.Status = Pedido.Pedido_Status.Fechado;
            if (!Pedido.Fatura.Save())
            {
                DialogCoordinator.ShowMessageAsync(this, "Salvar Fatura",
                    "A fatura do pedido não pode ser salva!");
                Cancel?.Invoke();
            }
            else
            {
                RaisePropertyChanged(nameof(Pedido));
                RaisePropertyChanged(nameof(HasFatura));
                SystemSounds.Beep.Play();
                FaturaTabItemIsSelected = true;
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
                        Tipo = Pagamento.TipoPagamento.Moeda,
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
                    DialogCoordinator.ShowMessageAsync(this, "Receber Pedido", "O pedido foi salvo e recebido!");
                    RaisePropertyChanged(nameof(Pedido));
                    RaisePropertyChanged(nameof(HasFatura));
                    SystemSounds.Beep.Play();
                    return;
                }
            }

            DialogCoordinator.ShowMessageAsync(this, "Salvar Fatura",
                "A fatura do pedido não pode ser salva!");
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

        private void NewNF(object obj)
        {
            var isNFC = obj.Equals("NFC-e");
            if (Pedido.NotasFiscais.FirstOrDefault(n => n.Situacao.BeEmitted()) is { } _nf)
            {
                DialogCoordinator.ShowMessageAsync(this, "Emitir Nota Fiscal",
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
                DialogCoordinator.ShowMessageAsync(this, "Erro",
                    "Ocorreu um ao gerar a numeração da próxima emitir Nota Fiscal");
                return;
            }

            INotaFiscal apiNf;
            if (isNFC)
                apiNf = new NotaFiscalConsumidor(nf);
            else
                apiNf = new NotaFiscalEletronica(nf);
            
            if (Pedido.Cliente != null){
                if (PersistenceContext.Get<PessoaFisica>(Pedido.Cliente.Id) is var pf and not null)
                    Pedido.Cliente = pf;
                else if (PersistenceContext.Get<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
                    Pedido.Cliente = pj;
            }
            
            var result = apiNf.BuildJsonDefault();
            if (result != "OK")
            {
                DialogCoordinator.ShowMessageAsync(this, "Erro ao validar a Nota Fiscal",
                    $"Erro ao validar a nota fiscal: {result}");
                return;
            }

            EmitindoNotaFiscal(apiNf, nf);

        }

        private async Task EmitindoNotaFiscal(INotaFiscal apiNf, NotaFiscal nf)
        {
            var task = apiNf.Emit();
            var s = new MetroDialogSettings()
            {
                NegativeButtonText = "Fechar"
            };
            var progressAsync = await DialogCoordinator.ShowProgressAsync(this,$"Emitindo Nota Fiscal",
                "Aguarde. A Nota Fiscal está em processo de emissão.", settings:s);
            
            progressAsync.SetIndeterminate();
            
            var t = new Timer(10000){AutoReset = false};
            t.Start();
            while (true)
            {
                if (t is {Enabled: false})
                {
                    progressAsync.SetCancelable(true);
                    progressAsync.SetMessage("A emissão está demorando mais do que o normal. Verifique a sua Conexão com a Internet");
                    t = null;
                }
                if (task.IsCompleted)
                {
                    await progressAsync.CloseAsync();
                    break;
                }
                if (progressAsync.IsCanceled)
                {
                    break;
                }

                await Task.Delay(100);
            }

            await task;
            if (!task.Result)
            {
                DialogCoordinator.ShowMessageAsync(this, "Erro ao emitir Nota Fiscal",
                    $"Erro ao enviar a Nota Fiscal para emissão");
                nf.Delete();
                return;
            }
            if (apiNf.NF_Result is {Error: { }})
            {
                    DialogCoordinator.ShowMessageAsync(this, "Erro ao emitir Nota Fiscal",
                        $"Erro ao processar a Nota Fiscal: {apiNf.NF_Result.Error}");
                nf.Delete();
                return;
            }

            NFConverter.Merge(apiNf.NF_Result, nf);
            Pedido.NotasFiscais.Add(nf);
            NotaFiscalSelecionada = nf;
            if (nf.Save())
            {
                DialogCoordinator.ShowMessageAsync(this, "Emissão de Nota Fiscal",
                    $"Nota Fiscal enviada para emissão. Situação :{nf.Situacao}");
                RaisePropertyChanged(nameof(HasNotaFiscal));
                TabSelecionada = 2;
                return;
            }

            DialogCoordinator.ShowMessageAsync(this, "Erro ao emitir Nota Fiscal",
                $"Um erro ocorreu ao salvar a nota fiscal emitida. Situação :{nf.Situacao}");
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
                DialogCoordinator.ShowMessageAsync(this, "Editar Cliente", "Não foi possível editar este cliente");
            }
        }

        private void VerificaQuantidadeEventHandler(TextChangedEventArgs e)
        {
            TextBox tb = (TextBox) e.Source;
            double quantidade = 0;
            if (!Regex.IsMatch(tb.Text, @"^(\d*,?\d*)?$") || !string.IsNullOrEmpty(tb.Text) &&
                !double.TryParse(tb.Text, out quantidade) &&
                quantidade is >= 0 and <= 10e10)
            {
                SystemSounds.Beep.Play();
                tb.Text = NovoItem.Quantidade.ToString();
                e.Handled = true;
            }
            else
            {
                NovoItem.Quantidade = quantidade;
            }
        }
    }
}