using System;
using System.Windows;
using System.Windows.Controls;

namespace SisMaper.Views;

public partial class ViewMetodoPagamento
{
    public enum OptionPagamento
    {
        Null,
        AVista,
        NovaFatura,
        FaturaExistente
    }

    public ViewMetodoPagamento()
    {
        InitializeComponent();
    }

    public OptionPagamento Selecionado { get; set; }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        try
        {
            Close();
        }
        catch { }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            Selecionado = button.Name switch
            {
                "AVista" => OptionPagamento.AVista,
                "NovaFatura" => OptionPagamento.NovaFatura,
                "FaturaExistente" => OptionPagamento.FaturaExistente,
                _ => OptionPagamento.Null
            };
        }

        Close();
    }
}