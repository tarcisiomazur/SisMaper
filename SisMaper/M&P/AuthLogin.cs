using System;
using System.Linq;
using Persistence;
using SisMaper.Models;

namespace SisMaper.M_P;

public static class AuthLogin
{
    private static readonly StoredProcedure<Usuario> FunctionLogin;

    static AuthLogin()
    {
        FunctionLogin = new StoredProcedure<Usuario>("efetuarLogin", "login_t", "senha_t");
    }

    public static Usuario? Login(Usuario user)
    {
        try
        {
            var users = FunctionLogin.Execute(user.Login, user.Senha);
            foreach (var usuario in users) Console.WriteLine(usuario);

            return users.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}