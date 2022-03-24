using System;
using System.Collections.Generic;
using SisMaper.Models;

namespace SisMaper.ViewModel;

public class SelecionarFaturaViewModel : BaseViewModel
{
    public SelecionarFaturaViewModel(List<Fatura> faturas)
    {
        Faturas = faturas;
    }

    #region Actions

    public event Action? Cancel;

    public event Action? Select;

    #endregion

    #region Properties

    public Cliente Cliente { get; set; }

    public Fatura? FaturaSelecionada { get; set; }

    public Fatura? Selecionado { get; set; }

    public List<Fatura> Faturas { get; set; }

    #endregion

    #region ICommands

    public SimpleCommand CancelarCmd => new(_ => Cancel?.Invoke());

    public SimpleCommand SelecionarCmd => new(Selecionar, _ => Selecionado is not null);

    #endregion

    private void Selecionar()
    {
        FaturaSelecionada = Selecionado;
        Select?.Invoke();
    }
}