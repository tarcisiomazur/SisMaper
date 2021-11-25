using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using MvvmCross.Core.ViewModels;
using Persistence;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.Views;

namespace SisMaper.ViewModel
{
    public class PedidoViewModel : BaseViewModel
    {
        public Pedido? Pedido { get; set; }
        public bool HasFatura => Pedido?.Fatura is not null;
        public bool HasNotaFiscal => Pedido?.NotasFiscais?.Count > 0;
        public bool FaturaTabItemIsSelected { get; set; }
        public PList<Cliente> Clientes { get; set; }
        public PList<Natureza> Naturezas { get; set; }
        public IEnumerable<Produto> ProdutosAtivos { get; set; }
        public PList<Produto> Produtos { get; set; }
        public Item NovoItem { get; set; }
        public IMvxCommand VerificaQuantidade => new MvxCommand<TextChangedEventArgs>(VerificaQuantidadeEventHandler);
        public IMvxCommand PreviewKeyDownAddItem => new MvxCommand<KeyEventArgs>(AdicionarItemEventHandler);

        public event Action? Cancel;
        public event Action? Save;
        public SimpleCommand Adicionar => new(AddItem);

        public SimpleCommand EditarCliente => new(EditCliente,
            o => Pedido?.Cliente is not null && Pedido?.Status == Pedido.Pedido_Status.Aberto);

        public SimpleCommand AdicionarCliente => new(NewCliente);

        public SimpleCommand AdicionarClienteContextMenu =>
            new(NewClienteContextMenu, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);

        public SimpleCommand MouseLeave => new(MouseLeaveContextMenu);
        public SimpleCommand Salvar => new(SavePedido, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);
        public SimpleCommand Cancelar => new(CancelPedido, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);
        public SimpleCommand Receber => new(ReceberPedido, _ => Pedido?.Status == Pedido.Pedido_Status.Aberto);
        public SimpleCommand OpenBuscarProduto => new(AbrirBuscarProduto);

        private IDialogCoordinator DialogCoordinator;

        private PersistenceContext PersistenceContext { get; set; }

        public string StringQuantidade { get; set; }

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
            Pedido = PersistenceContext.Get<Pedido>(pedidoId);
            if (pedidoId == null) Pedido.Usuario = Main.Usuario;

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
                        Context = PersistenceContext
                    }
                })
            });
            Pedido.Status = Pedido.Pedido_Status.Fechado;
            if (!Pedido.Fatura.Save())
            {
                DialogCoordinator.ShowMessageAsync(this, "Salvar Fatura",
                    "A fatura do pedido não pode ser salva!");
                Cancel?.Invoke();
                return;
            }

            Pedido.Fatura.Status = Fatura.Fatura_Status.Fechada;
            Pedido.Fatura.Save();
            DialogCoordinator.ShowMessageAsync(this, "Receber Pedido", "O pedido foi salvo e recebido!");
            RaisePropertyChanged(nameof(Pedido));
            RaisePropertyChanged(nameof(HasFatura));
            SystemSounds.Beep.Play();
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

        private void MouseLeaveContextMenu(object obj)
        {
            ((Menu) obj).Visibility = Visibility.Hidden;
        }

        private void NewClienteContextMenu(object obj)
        {
            if (obj is Menu menu)
            {
                menu.Visibility = Visibility.Visible;
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