using System;
using System.ComponentModel;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;
using Xceed.Wpf.Toolkit.Core;

namespace SisMaper.Views
{
    public partial class ViewConfiguracoes
    {
        public ConfiguracoesViewModel ViewModel => (ConfiguracoesViewModel) DataContext;
        public IDialogCoordinator DialogCoordinator { get; set; }
        public ViewConfiguracoes(Configuracoes empresa)
        {
            InitializeComponent();
            DialogCoordinator = new DialogCoordinator();
            ViewModel.Empresa = empresa;
            Initialize();
            Closed += ReloadEmpresa;
        }

        private void ReloadEmpresa(object? sender, EventArgs e)
        {
            ViewModel.Empresa.Load();
        }

        private void Initialize()
        {
            try
            {
                ConsumerKey.Password = Encrypt.RSADecryption(ViewModel.Empresa.CONSUMER_KEY);
                ConsumerSecret.Password = Encrypt.RSADecryption(ViewModel.Empresa.CONSUMER_SECRET);
                Token.Password = Encrypt.RSADecryption(ViewModel.Empresa.ACCESS_TOKEN);
                TokenSecret.Password = Encrypt.RSADecryption(ViewModel.Empresa.ACCESS_TOKEN_SECRET);
            }
            catch
            {
                DialogCoordinator.ShowModalMessageExternal(DataContext, "Erro de Desencriptação" ,"Erro ao desencriptar os Dados");
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Salvar(object sender, MouseButtonEventArgs e)
        {
            if (!ViewModel.Empresa.CNPJ.IsCnpj())
            {
                DialogCoordinator.ShowModalMessageExternal(DataContext, "Erro ao Salvar Empresa" ,"CNPJ Inválido");
                return;
            }

            try
            {
                ViewModel.Empresa.CONSUMER_KEY = Encrypt.RSAEncryption(ConsumerKey.Password);
                ViewModel.Empresa.CONSUMER_SECRET = Encrypt.RSAEncryption(ConsumerSecret.Password);
                ViewModel.Empresa.ACCESS_TOKEN = Encrypt.RSAEncryption(Token.Password);
                ViewModel.Empresa.ACCESS_TOKEN_SECRET = Encrypt.RSAEncryption(TokenSecret.Password);
                ViewModel.Empresa.Save();
            }
            catch
            {
                DialogCoordinator.ShowModalMessageExternal(DataContext, "Erro de Encriptação" ,"Erro ao Encriptar os Dados");
            }
            Close();
        }
        private void Cancelar(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void AtualizarNCMs(object sender, System.Windows.RoutedEventArgs e)
        {
            BuildNCM.Run();
        }

        private void ImportPem(object sender, MouseButtonEventArgs e)
        {
            Encrypt.ImportKey();
        }

        private void AlterarNaturezas(object sender, MouseButtonEventArgs e)
        {
            new ViewNaturezas().ShowDialog();
        }
        
    }
}