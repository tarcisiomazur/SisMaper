using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using SisMaper.Tools;

namespace SisMaper.ViewModel
{
    public class VendasViewModel : BaseViewModel
    {
        #region Properties

        public List<ListarPedido>? Pedidos { get; set; }
        public List<Pedido.Pedido_Status> StatusList { get; set; }

        #endregion

        #region UIProperties

        public ListarPedido? PedidoSelecionado { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TextoFiltro { get; set; } = "";
        public Pedido.Pedido_Status? StatusSelecionado { get; set; }
        public IEnumerable<ListarPedido>? PedidosFiltrados { get; set; }

        #endregion

        #region ICommands

        public SimpleCommand NovoPedidoCmd => new(NovoPedido);
        public SimpleCommand EditarPedidoCmd => new(EditarPedido, _ => PedidoSelecionado is not null);
        public SimpleCommand ExcluirPedidoCmd => new(ExcluirPedido, _ => PedidoSelecionado is not null);

        #endregion

        #region Actions

        public event Action<PedidoViewModel>? OpenPedido;

        #endregion

        public VendasViewModel()
        {
            StatusList = new List<Pedido.Pedido_Status>()
            {
                Pedido.Pedido_Status.Aberto,
                Pedido.Pedido_Status.Fechado,
                Pedido.Pedido_Status.Cancelado,
            };
            PropertyChanged += UpdateFilter;
        }

        private void NovoPedido()
        {
            OpenPedido?.Invoke(new PedidoViewModel(null));
        }

        private void EditarPedido()
        {
            if (PedidoSelecionado == null) return;
            var vm = new PedidoViewModel(PedidoSelecionado.Id);
            OpenPedido?.Invoke(vm);
        }

        private void ExcluirPedido()
        {
            if (PedidoSelecionado == null) return;
            var pedido = DAO.Load<Pedido>(PedidoSelecionado.Id);
            if (pedido == null) return;

            if (pedido.Status == Pedido.Pedido_Status.Fechado &&
                !Main.Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento))
            {
                OnShowMessage("Excluir Pedido", "Você não possui permissão para excluir um pedido fechado");
                return;
            }

            var result = OnShowMessage("Excluir Pedido", "Deseja Excluir o pedido selecionado?",
                MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                try
                {
                    if (pedido.Delete())
                    {
                        Initialize(null, EventArgs.Empty);
                        OnShowMessage("Excluir Pedido", "Pedido Excluído com Sucesso!");
                        return;
                    }
                }
                catch
                {
                    //
                }

                OnShowMessage("Excluir Pedido",
                    "Pedido não excluído, verifique se o pedido ainda existe ou possui dependências como notas fiscais ou faturas");
            }
        }

        public void Initialize(object? sender, EventArgs e)
        {
            Pedidos = View.Execute<ListarPedido>();
            StartDate = DateTime.Today.AddMonths(-1);
            EndDate = DateTime.Today;
            TextoFiltro = "";
        }


        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Pedidos != null && e.PropertyName is nameof(Pedidos) or nameof(TextoFiltro) or
                nameof(StartDate) or nameof(EndDate) or nameof(StatusSelecionado))
            {
                PedidosFiltrados = Pedidos.Where(p =>
                    (TextoFiltro.IsContainedIn(p.Cliente) || TextoFiltro.IsContainedIn(p.Id.ToString())) &&
                    (StatusSelecionado == null || p.Status == StatusSelecionado) &&
                    p.Data.Date >= StartDate.Date && p.Data.Date <= EndDate.Date
                );
            }
        }

        public void Clear(object? sender, EventArgs e)
        {
            Pedidos = null;
        }
    }
}