using System;
using System.Windows;
using SisMaper.Models;
using Microsoft.Win32;
using SisMaper.M_P;

namespace SisMaper.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        readonly RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SisMaper");

        private Usuario _Usuario;
        public SimpleCommand Login { get; private set; }

        public Usuario Usuario
        {
            get { return _Usuario; }
            set { SetField(ref _Usuario, value); }
        }

        public event Action OnLogin;
        public bool PasswordFocus { get; set; }
        public bool UsuarioFocus { get; set; }

        public LoginViewModel()
        {
            Login = new SimpleCommand(_ => ConfirmLogin());
            Usuario = new Usuario();
            var username = key.GetValue("LastUsername");
            if (username is string usernamestr)
            {
                Usuario.Login = usernamestr;
            }

        }

        public void ConfirmLogin()
        {
            var user = AuthLogin.Login(Usuario);
            
            if (user is {Permissao:>0})
            {
                Usuario = user;
                key.SetValue("LastUsername", Usuario.Login);
                OnLogin.Invoke();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorreto", "Tentativa de Acesso", MessageBoxButton.OK);
            }

            Console.WriteLine("Login Confirmado. Login: " + Usuario.Login + ". Senha: " + Usuario.Senha);
        }
    }
}