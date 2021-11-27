using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class VendasViewModel : BaseViewModel
    {
        public ViewListarPedido? PedidoSelecionado { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string TextoFiltro { get; set; }
        public Pedido.Pedido_Status? StatusSelecionado { get; set; }
        public List<Pedido.Pedido_Status> StatusList { get; set; }
        public event Action<long?>? OpenPedido;
        public IEnumerable<ViewListarPedido> PedidosFiltrados { get; set; }
        public ObservableCollection<ViewListarPedido>? Pedidos { get; set; }
        public SimpleCommand NovoPedido => new(NewVenda);
        public SimpleCommand EditarPedido => new(_ => OpenVenda(), _ => PedidoSelecionado is not null);
        public SimpleCommand ExcluirPedido => new(_ => DeleteVenda(), _ => PedidoSelecionado is not null);

        private IDialogCoordinator DialogCoordinator;

        public VendasViewModel()
        {
            StatusList = new List<Pedido.Pedido_Status>()
            {
                Pedido.Pedido_Status.Aberto,
                Pedido.Pedido_Status.Fechado,
                Pedido.Pedido_Status.Cancelado,
            };
            PropertyChanged += UpdateFilter;
            DialogCoordinator = new DialogCoordinator();
        }

        private void NewVenda()
        {
            OpenPedido?.Invoke(null);
        }

        private void OpenVenda()
        {
            if (PedidoSelecionado == null) return;
            OpenPedido?.Invoke(PedidoSelecionado.Id);
        }

        private void DeleteVenda()
        {
            if (PedidoSelecionado == null) return;
            var pedido = DAO.Load<Pedido>(PedidoSelecionado.Id);
            if (pedido == null) return;

            if (pedido.Status == Pedido.Pedido_Status.Fechado &&
                !Main.Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento))
            {
                DialogCoordinator.ShowMessageAsync(this, "Excluir Pedido",
                    "Você não possui permissão para excluir um pedido fechado");
                return;
            }

            var result = DialogCoordinator.ShowModalMessageExternal(this, "Excluir Pedido",
                "Deseja Excluir o pedido selecionado?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                try
                {
                    if (pedido.Delete())
                    {
                        UpdatePedidos(null, EventArgs.Empty);
                        DialogCoordinator.ShowMessageAsync(this, "Excluir Pedido", "Pedido Excluído com Sucesso!");
                        return;
                    }
                }
                catch
                {
                    //
                }

                DialogCoordinator.ShowMessageAsync(this, "Excluir Pedido",
                    "Pedido não excluído, verifique se o pedido ainda existe ou possui dependências como notas fiscais ou faturas");
            }
        }

        public void UpdatePedidos(object? sender, EventArgs e)
        {
            Pedidos = new ObservableCollection<ViewListarPedido>(View.Execute<ViewListarPedido>());
        }


        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Pedidos != null && e.PropertyName is nameof(Pedidos) or nameof(TextoFiltro) or
                nameof(StartDate) or nameof(EndDate) or nameof(StatusSelecionado))
            {
                PedidosFiltrados = Pedidos!.Where(p =>
                    (string.IsNullOrEmpty(TextoFiltro) || !string.IsNullOrEmpty(p.Cliente) &&
                     p.Cliente.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase) ||
                     p.Id.ToString().Equals(TextoFiltro)) &&
                    (StatusSelecionado == null || p.Status == StatusSelecionado) &&
                    p.Data > StartDate && p.Data < EndDate
                );
            }
        }
    }
}