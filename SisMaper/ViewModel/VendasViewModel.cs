using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class VendasViewModel : BaseViewModel
{
    public VendasViewModel()
    {
        PedidosFiltrados = new ListCollectionView(Pedidos)
        {
            Filter = Filter
        };
        StatusList = new List<Pedido.Pedido_Status>
        {
            Pedido.Pedido_Status.Aberto,
            Pedido.Pedido_Status.Fechado,
            Pedido.Pedido_Status.Cancelado
        };
        PropertyChanged += UpdateFilter;
    }

    #region Actions

    public event Action<PedidoViewModel>? OpenPedido;

    #endregion

    #region Properties

    public DateTime? EndDate { get; set; }

    public DateTime? StartDate { get; set; }

    public List<ListarPedido> Pedidos { get; set; } = new();

    public List<Pedido.Pedido_Status> StatusList { get; set; }

    public ListarPedido? PedidoSelecionado { get; set; }

    public ListCollectionView PedidosFiltrados { get; set; }

    public Pedido.Pedido_Status? StatusSelecionado { get; set; }

    public string TextoFiltro { get; set; } = "";

    public string? IntervaloSelecionado
    {
        set => ChangeIntervalo(value);
        get => GetIntervalo();
    }

    #endregion

    #region ICommands

    public SimpleCommand EditarPedidoCmd => new(EditarPedido, _ => PedidoSelecionado is not null);

    public SimpleCommand ExcluirPedidoCmd => new(ExcluirPedido, _ => PedidoSelecionado is not null);

    public SimpleCommand NovoPedidoCmd => new(NovoPedido);

    #endregion

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
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

            OnShowMessage("Excluir Pedido",
                "Pedido não excluído, verifique se o pedido ainda existe ou possui dependências como notas fiscais ou faturas");
        }
    }

    public void Initialize(object? sender, EventArgs e)
    {
        Pedidos.Clear();
        Pedidos.AddRange(View.Execute<ListarPedido>());
        PedidosFiltrados.Refresh();
        TextoFiltro = "";
    }

    private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Pedidos) or nameof(TextoFiltro) or
            nameof(StartDate) or nameof(EndDate) or nameof(StatusSelecionado))
        {
            PedidosFiltrados?.Refresh();
        }
    }

    private bool Filter(object obj)
    {
        return obj is ListarPedido p &&
               (TextoFiltro.IsContainedIn(p.Cliente) || TextoFiltro.IsContainedIn(p.Id.ToString())) &&
               (StatusSelecionado == null || p.Status == StatusSelecionado) &&
               !(p.Data.Date < StartDate?.Date || p.Data.Date > EndDate?.Date);
    }

    public void Clear(object? sender, EventArgs e)
    {
        Pedidos.Clear();
    }

    private void ChangeIntervalo(string? value)
    {
        var today = DateTime.Today;
        (EndDate, StartDate) = value switch
        {
            "0" => (today, today),
            "1" => (today, today.AddDays(-(int) DateTime.Today.DayOfWeek)),
            "2" => (today, today.AddDays(1 - DateTime.Today.Day)),
            "3" => (today, today.AddDays(-60)),
            "4" => (default, default),
            _ => (EndDate, StartDate)
        };
    }

    private string? GetIntervalo()
    {
        var today = DateTime.Today;
        if (StartDate is null && EndDate is null)
        {
            return "4";
        }

        if (EndDate != today)
        {
            return null;
        }

        if (StartDate == today)
        {
            return "0";
        }

        if (StartDate == today.AddDays(-(int) DateTime.Today.DayOfWeek))
        {
            return "1";
        }

        if (StartDate == today.AddDays(1 - DateTime.Today.Day))
        {
            return "2";
        }

        if (StartDate == today.AddDays(-60))
        {
            return "3";
        }

        return null;
    }
}