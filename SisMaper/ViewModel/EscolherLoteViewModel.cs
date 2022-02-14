using System;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel;

public class EscolherLoteViewModel : BaseViewModel
{
    public EscolherLoteViewModel(PList<Lote> lotes)
    {
        Lotes = lotes;
    }

    #region Actions

    public event Action? OnCancel;

    public event Action? OnOk;

    #endregion

    #region Properties

    public Lote? LoteSelecionado { get; set; }

    public PList<Lote> Lotes { get; set; }

    #endregion

    #region ICommands

    public SimpleCommand CancelarCmd => new(Cancel);

    public SimpleCommand OkCmd => new(Ok, _ => LoteSelecionado is not null);

    #endregion

    private void Ok()
    {
        OnOk?.Invoke();
    }

    private void Cancel()
    {
        LoteSelecionado = null;
        OnCancel?.Invoke();
    }
}