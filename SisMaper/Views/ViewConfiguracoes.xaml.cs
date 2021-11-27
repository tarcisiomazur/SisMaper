using System.ComponentModel;
using System.Windows.Input;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.Views
{
    public partial class ViewConfiguracoes : INotifyPropertyChanged
    {
        public Configuracoes Empresa { get; set; }
        public ViewConfiguracoes(Configuracoes empresa)
        {
            Empresa = empresa;
            InitializeComponent();
            Initialize();
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
            Empresa.CONSUMER_KEY = Encrypt.RSAEncryption(ConsumerKey.Password);
            Empresa.CONSUMER_SECRET = Encrypt.RSAEncryption(ConsumerSecret.Password);
            Empresa.ACCESS_TOKEN = Encrypt.RSAEncryption(Token.Password);
            Empresa.ACCESS_TOKEN_SECRET = Encrypt.RSAEncryption(TokenSecret.Password);
            Empresa.Save();
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
    }
}