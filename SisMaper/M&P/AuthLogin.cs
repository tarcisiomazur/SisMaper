using System;
using System.Linq;
using Persistence;
using SisMaper.Models;

namespace SisMaper.M_P
{
    
    public class AuthLogin
    {
        public static Usuario? Login(Usuario user)
        {
            var functionLogin = new StoredProcedure<Usuario>("efetuarLogin", "login_t", "senha_t");
            var users = functionLogin.Execute(user.Login, user.Senha);
            foreach (var usuario in users)
            {
                Console.WriteLine(usuario);
            }

            return users.FirstOrDefault();
        }
    }
}