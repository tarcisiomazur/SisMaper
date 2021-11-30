using System;
using System.ComponentModel;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using SisMaper.Models;
using SisMaper.Tools;
using Xceed.Wpf.Toolkit.Core;

namespace SisMaper.Views
{
    public partial class ViewConfiguracoes : INotifyPropertyChanged
    {
        public Configuracoes Empresa { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }
        public ViewConfiguracoes(Configuracoes empresa)
        {
            Empresa = empresa;
            InitializeComponent();
            Initialize();
            Closed += ReloadEmpresa;
            DialogCoordinator = new DialogCoordinator();
        }

        private void ReloadEmpresa(object? sender, EventArgs e)
        {
            Empresa.Load();
        }

        private void Initialize()
        {
            ConsumerKey.Password = Encrypt.RSADecryption(Empresa.CONSUMER_KEY);
            ConsumerSecret.Password = Encrypt.RSADecryption(Empresa.CONSUMER_SECRET);
            Token.Password = Encrypt.RSADecryption(Empresa.ACCESS_TOKEN);
            TokenSecret.Password = Encrypt.RSADecryption(Empresa.ACCESS_TOKEN_SECRET);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void Salvar(object sender, MouseButtonEventArgs e)
        {
            if (!Empresa.CNPJ.IsCnpj())
            {
                DialogCoordinator.ShowMessageAsync(DataContext, "Erro ao Salvar Empresa" ,"CNPJ Inválido");
                return;
            }
            Empresa.CONSUMER_KEY = Encrypt.RSAEncryption(ConsumerKey.Password);
            Empresa.CONSUMER_SECRET = Encrypt.RSAEncryption(ConsumerSecret.Password);
            Empresa.ACCESS_TOKEN = Encrypt.RSAEncryption(Token.Password);
            Empresa.ACCESS_TOKEN_SECRET = Encrypt.RSAEncryption(TokenSecret.Password);
            Empresa.Save();
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

        private void ValueRangeTextBox_OnQueryTextFromValue(object? sender, QueryTextFromValueEventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}