using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using MvvmCross.Core.ViewModels;
using Persistence;
using SisMaper.Models;
using SisMaper.Views;

namespace SisMaper.ViewModel
{
    public class PedidoViewModel: BaseViewModel
    {
        private Pedido _pedido;
        private PList<Cliente> _clientes;
        private PList<Natureza> _naturezas;
        private PList<Produto> _produtos;

        public Pedido Pedido
        {
            get => _pedido;
            set => SetField(ref _pedido, value);
        }

        public PList<Cliente> Clientes
        {
            get => _clientes;
            set => SetField(ref _clientes, value);
        }

        public PList<Natureza> Naturezas 
        {
            get => _naturezas;
            set => SetField(ref _naturezas, value);
        }
        public PList<Produto> Produtos 
        {
            get => _produtos;
            set => SetField(ref _produtos, value);
        }
        
        private Item _novoItem;

        public Item NovoItem
        {
            get => _novoItem;
            set => SetField(ref _novoItem, value);
        }
        
        public IMvxCommand VerificaQuantidade => new MvxCommand<TextChangedEventArgs>(VerificaQuantidadeEventHandler);

        public event Action? OnCancel;
        public event Action? OnSave;
        public SimpleCommand Adicionar => new (AddItem);
        public SimpleCommand EditarCliente => new (EditCliente, o => Pedido.Cliente is not null);
        public SimpleCommand Salvar => new (SavePedido);
        public SimpleCommand Cancelar => new (CancelPedido);

        private IDialogCoordinator DialogCoordinator;
        
        private PersistenceContext PersistenceContext { get; set; }

        public PedidoViewModel()
        {
            PersistenceContext = new PersistenceContext();
        }
        
        public void Initialize(long? pedidoId)
        {
            Naturezas = PersistenceContext.Get<Natureza>("ID>0");
            Clientes = PersistenceContext.Get<Cliente>("ID>0");
            Produtos = PersistenceContext.Get<Produto>("ID>0");
            
            Console.WriteLine("\n\n\n\n\n\n");
            foreach (var pcs in PersistenceContext.Storages)
            {
                Console.WriteLine(pcs.Key.Name);
                foreach (var keyValuePair in pcs.Value.Objects)
                {
                    Console.WriteLine("\t" + keyValuePair.Value);
                }
            }
            
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
            if (NovoItem.Produto is not null && NovoItem.Quantidade > 0)
            {
                NovoItem.Valor = NovoItem.Produto.PrecoVenda;
                Pedido.Itens.Add(NovoItem);
                SumPedido();
                NovoItem = new Item(){Pedido = Pedido, Context = PersistenceContext};
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }

        private void EditCliente()
        {
            if (DAO.Load<PessoaFisica>(Pedido.Cliente.Id) is var pf and not null)
            {
                var crud = new CrudPessoaFisica();
                var vm = (CrudPessoaFisicaViewModel) crud.DataContext;
                vm.PessoaFisica = pf;
                crud.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<Cliente>(vm.PessoaFisica.Id);
            }
            else if (DAO.Load<PessoaJuridica>(Pedido.Cliente.Id) is var pj and not null)
            {
                var crud = new CrudPessoaFisica();
                var vm = (CrudPessoaFisicaViewModel) crud.DataContext;
                vm.PessoaJuridica = pj;
                crud.ShowDialog();
                Pedido.Cliente = PersistenceContext.GetOrRefresh<Cliente>(vm.PessoaJuridica.Id);
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
            if (!string.IsNullOrEmpty(tb.Text) && !double.TryParse(tb.Text, out quantidade) &&
                quantidade is >= 0 and <= 10e10)
            {
                SystemSounds.Beep.Play();
                tb.Text = NovoItem.Quantidade.ToString();
                e.Handled = true;
            }
            else
            {
                Console.WriteLine(quantidade);
                NovoItem.Quantidade = quantidade;
            }
        }
        
    }
}