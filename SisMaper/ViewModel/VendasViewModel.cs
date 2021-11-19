using System;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class VendasViewModel: BaseViewModel
    {
        private ViewListarPedido? _pedidoSelecionado;

        public ViewListarPedido? PedidoSelecionado
        {
            get { return _pedidoSelecionado; }
            set { SetField(ref _pedidoSelecionado, value); }
        }

        public event Action<Pedido> OpenPedido;
        
        public ObservableCollection<ViewListarPedido> Pedidos { get; set; }

        public SimpleCommand NovoPedido => new (_ => NewVenda());
        public SimpleCommand EditarPedido => new (_ => OpenVenda(), _ => PedidoSelecionado is not null);
        public SimpleCommand ExcluirPedido => new (_ => DeleteVenda(), _ => PedidoSelecionado is not null);
        
        private IDialogCoordinator DialogCoordinator;

        public VendasViewModel()
        {
            Pedidos = new ObservableCollection<ViewListarPedido>(View.Execute<ViewListarPedido>());
            DialogCoordinator = new DialogCoordinator();
        }

        private void NewVenda()
        {
            OpenPedido.Invoke(new Pedido());
        }
        
        private void OpenVenda()
        {
            if (PedidoSelecionado == null) return;
            var pedido = DAO.Load<Pedido>(PedidoSelecionado.Id);
            if (pedido == null) return;
            OpenPedido.Invoke(pedido);
        }

        private void DeleteVenda()
        {
            if (PedidoSelecionado == null) return;
            var pedido = DAO.Load<Pedido>(PedidoSelecionado.Id);
            if (pedido == null) return;
            
            if (pedido.Status == Pedido.Pedido_Status.Fechado &&
                !Main.Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento))
            {
                DialogCoordinator.ShowMessageAsync(this, "Excluir Pedido", "Você não possui permissão para excluir um pedido fechado");
                return;
            }
            var result = DialogCoordinator.ShowModalMessageExternal(this, "Excluir Pedido", "Deseja Excluir o pedido selecionado?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                DialogCoordinator.ShowMessageAsync(this, "Excluir Pedido",
                    pedido.Delete()
                        ? "Pedido Excluído com Sucesso!"
                        : "Pedido não excluído, verifique se o pedido ainda existe ou possui dependências como notas fiscais ou faturas");
            }
        }
        
        
    }
}