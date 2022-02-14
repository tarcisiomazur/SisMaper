using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace SisMaper.Views.Templates;

public partial class LostConnection
{
    private ProgressDialogController? _progress;

    public LostConnection()
    {
        InitializeComponent();
        Instance = this;
        Reconnecting();
    }

    public static LostConnection? Instance { get; set; }

    private async void Reconnecting()
    {
        Main.Instance.Status = "Reconectando";
        var s = new MetroDialogSettings
        {
            NegativeButtonText = "Sair",
            OwnerCanCloseWithDialog = true
        };
        _progress = await this.ShowProgressAsync("", "", settings: s);
        if (_progress is null) return;
        _progress.SetTitle("Conexão com o Banco de Dados Perdida");
        _progress.SetMessage("Reconectando com o Servidor...");
        _progress.SetIndeterminate();
        await Task.Delay(5000);
        _progress.SetCancelable(true);
        _progress.Canceled += (_, _) => Environment.Exit(0);
    }

    public new async void Close()
    {
        if (_progress is not null)
        {
            await _progress.CloseAsync();
        }

        Instance = null;
        base.Close();
    }
}