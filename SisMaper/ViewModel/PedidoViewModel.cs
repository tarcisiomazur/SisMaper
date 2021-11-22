using System;
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
    public class PedidoViewModel: BaseViewModel
    {
        

        public Pedido Pedido { get; set; }
        public PList<Cliente> Clientes{ get; set; }
        public PList<Natureza> Naturezas { get; set; }
        public PList<Produto> Produtos { get; set; }
        public Item NovoItem{ get; set; }
        public IMvxCommand VerificaQuantidade => new MvxCommand<TextChangedEventArgs>(VerificaQuantidadeEventHandler);
        public IMvxCommand PreviewKeyDownAddItem => new MvxCommand<KeyEventArgs>(AdicionarItemEventHandler);

        public event Action? OnCancel;
        public event Action? OnSave;
        public SimpleCommand Adicionar => new (AddItem);
        public SimpleCommand EditarCliente => new (EditCliente, o => Pedido?.Cliente is not null);
        public SimpleCommand AdicionarCliente => new (NewCliente);
        public SimpleCommand AdicionarClienteContextMenu => new (NewClienteContextMenu);
        public SimpleCommand MouseLeave => new (MouseLeaveContextMenu);
        public SimpleCommand Salvar => new (SavePedido);
        public SimpleCommand Cancelar => new (CancelPedido);

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
            Clientes = PersistenceContext.Get<Cliente>("ID>0");
            Produtos = PersistenceContext.Get<Produto>("ID>0");
            Pedido = PersistenceContext.Get<Pedido>(pedidoId);
            if (pedidoId == null) Pedido.Usuario = Main.Usuario;

            DialogCoordinator = new DialogCoordinator();
            NovoItem = new Item() {Pedido = Pedido, Context = PersistenceContext};
            Pedido.Itens.ListChanged += UpdatePedido;
        }

        private void UpdatePedido(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType is > ListChangedType.Reset and < ListChangedType.PropertyDescriptorAdded &&
                e.PropertyDescriptor?.Name == nameof(Item.Total))
            {
                SumPedido();
            }
        }
        
        private void AdicionarItemEventHandler(KeyEventArgs obj)
        {
            if(obj.Key == Key.Enter)
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
                OnSave?.Invoke();
            }
            catch (PersistenceException ex)
            {
                if (ex.ErrorCode == SQLException.ErrorCodeVersion)
                {
                    DialogCoordinator.ShowMessageAsync(this, "Salvar Pedido", "O pedido não pode ser salvo pois está desatualizado!");
                    OnCancel?.Invoke();
                }
            }
            
        }
        
        private void CancelPedido()
        {
            OnCancel?.Invoke();
        }

        private void AddItem()
        {
            if (NovoItem.Produto is not null)
            {
                if (NovoItem.Quantidade == 0) NovoItem.Quantidade = 1;
                if (!NovoItem.Produto.Fracionado && !NovoItem.Quantidade.IsNatural())
                {
                    DialogCoordinator.ShowMessageAsync(this, "Adicionar Item", "O item não aceita quantidade fracionada!");
                    return;
                }
                NovoItem.Produto.Lotes.Load();
                if (NovoItem.Produto.Lotes?.Count>0)
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
                StringQuantidade = "";
                NovoItem = new Item(){Pedido = Pedido, Context = PersistenceContext};
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }

        private void NewCliente(object obj)
        {
            new CrudPessoaFisica()
            {
                isSelectedPessoaFisicaTab = obj.Equals("PF"),
                DataContext = new CrudPessoaFisicaViewModel(null)
            }.ShowDialog();

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
                new CrudPessoaFisica() { isSelectedPessoaFisicaTab = true, DataContext = new CrudPessoaFisicaViewModel(pf) }.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<Cliente>(pf.Id);
            }
            else if (DAO.Load<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
            {
                new CrudPessoaFisica() { isSelectedPessoaFisicaTab = false, DataContext = new CrudPessoaFisicaViewModel(pj) }.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<Cliente>(pj.Id);
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
            if (!Regex.IsMatch(tb.Text,@"^(\d*,?\d*)?$") || !string.IsNullOrEmpty(tb.Text) && !double.TryParse(tb.Text, out quantidade) &&
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