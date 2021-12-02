using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Models;
using SisMaper.Views;
using SisMaper.Views.Templates;

namespace SisMaper.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public bool PAdmin { get; set; }
        public bool PVendas { get; set; }
        public bool PRecebimento { get; set; }
        public bool PCadastro { get; set; }
        public bool PDB { get; set; }

        public MainViewModel()
        {
        }

        public void Initialize()
        {
            var permissoes = Main.Usuario.Permissao;
            PCadastro = permissoes.HasFlag(Usuario.Tipo_Permissao.Cadastros);
            PRecebimento = permissoes.HasFlag(Usuario.Tipo_Permissao.Recebimento);
            PVendas = permissoes.HasFlag(Usuario.Tipo_Permissao.Venda);
            PAdmin = permissoes.HasFlag(Usuario.Tipo_Permissao.Gerenciamento);
            PDB = permissoes.HasFlag(Usuario.Tipo_Permissao.Databaser);
        }
        
        private TabItem? _selectedItem;
        
        public TabItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;
                if(_selectedItem?.Content is MyUserControl close) close.OnClose();
                _selectedItem = value;
                if(value?.Content is MyUserControl open) open.OnOpen();
            }
        }
        public void Connected()
        {
            Main.Instance.Status = "Conectado";
            Close = true;
        }
        public void Disconnected()
        {
            Main.Instance.Status = "Desconectado";
        }
        
        public bool Close;

        public async void Reconnecting()
        {
            Main.Instance.Status = "Reconectando";
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
