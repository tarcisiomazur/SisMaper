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
        public SimpleCommand Login => new (ConfirmLogin);
        public SimpleCommand Cancel => new (OnCancel);
        public Usuario Usuario { get; set; }

        public event Action? OnLogin;
        public event Action? OnCancel;
        public bool PasswordFocus { get; set; }
        public bool UsuarioFocus { get; set; }

        public LoginViewModel()
        {
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
                OnLogin?.Invoke();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorreto", "Tentativa de Acesso", MessageBoxButton.OK);
            }

            Console.WriteLine("Login Confirmado. Login: " + Usuario.Login + ". Senha: " + Usuario.Senha);
        }
        
    }
}