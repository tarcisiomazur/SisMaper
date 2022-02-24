using System;
using Microsoft.Win32;
using SisMaper.M_P;
using SisMaper.Models;

namespace SisMaper.ViewModel;

public class LoginViewModel : BaseViewModel
{
    private readonly RegistryKey _key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SisMaper");

    public LoginViewModel()
    {
        Usuario = new Usuario();
        var username = _key.GetValue("LastUsername");
        if (username is string usernamestr)
        {
            Usuario.Login = usernamestr;
        }
    }

    #region Actions

    public event Action? Cancel;

    public event Action? Login;

    #endregion

    #region Properties

    public bool PasswordFocus { get; set; }

    public bool UsuarioFocus { get; set; }

    public Usuario Usuario { get; set; }

    #endregion

    #region ICommands

    public SimpleCommand OnCancel => new(() => Cancel?.Invoke());

    public SimpleCommand OnLogin => new(ConfirmLogin);

    #endregion

    public void SetFocus()
    {
        if (string.IsNullOrEmpty(Usuario.Login))
        {
            UsuarioFocus = true;
        }
        else
        {
            PasswordFocus = true;
        }
    }

    private void ConfirmLogin()
    {
        var user = AuthLogin.Login(Usuario);

        if (user is {Permissao: > 0})
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