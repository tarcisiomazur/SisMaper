using System;
using Microsoft.Win32;
using SisMaper.M_P;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        #region Properties

        private readonly RegistryKey _key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SisMaper");
        public Usuario Usuario { get; set; }

        #endregion

        #region UIProperties

        public bool PasswordFocus { get; set; }
        public bool UsuarioFocus { get; set; }

        #endregion

        #region ICommands

        public SimpleCommand OnLogin => new(ConfirmLogin);
        public SimpleCommand OnCancel => new(() => Cancel?.Invoke());

        #endregion

        #region Actions

        public event Action? Login;
        public event Action? Cancel;

        #endregion

        public LoginViewModel()
        {
            Usuario = new Usuario();
            var username = _key.GetValue("LastUsername");
            if (username is string usernamestr)
            {
                Usuario.Login = usernamestr;
            }
        }

        private void ConfirmLogin()
        {
            var user = AuthLogin.Login(Usuario);

            if (user is {Permissao: >0})
            {
                Main.Usuario = user;
                _key.SetValue("LastUsername", Usuario.Login);
                Login?.Invoke();
            }
            else
            {
                OnShowMessage("Usuário ou senha incorreto", "Tentativa de Acesso");
            }

            Console.WriteLine("Login Confirmado. Login: " + Usuario.Login + ". Senha: " + Usuario.Senha);
        }
    }
}