using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Views;
using SisMaper.Views.Templates;

namespace SisMaper.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public string Status { get; set; } = "Desconectado";
        
        public MainViewModel()
        {
            
        }

        private TabItem? _selectedItem;
        
        public TabItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;
                if(value?.Content is MyUserControl open) open.OnOpen();
                if(_selectedItem?.Content is MyUserControl close) close.OnClose();
                _selectedItem = value;
            }
        }
        public void Connected()
        {
            Status = "Conectado";
            Close = true;
        }
        public void Disconnected()
        {
            Status = "Desconectado";
        }
        
        public bool Close;

        public async void Reconnecting()
        {
            Status = "Reconectando";
            Close = false;
            var s = new MetroDialogSettings()
            {
                NegativeButtonText = "Sair"
            };
            var progressAsync = await MainWindow.Instance.ShowProgressAsync("Conexão com o Banco de Dados Perdida",
                "Reconectando com o Servidor...", settings: s);

            progressAsync.SetIndeterminate();
            await Task.Delay(5000);
            progressAsync.SetCancelable(true);
            while (true)
            {
                if (Close)
                {
                    await progressAsync.CloseAsync();
                    return;
                }
                if (progressAsync.IsCanceled)
                {
                    Environment.Exit(0);
                }

                await Task.Delay(100);
            }
        }
    }

}
