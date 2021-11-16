using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private Usuario? _Usuario;
        public AcessarCommand Acessar { get; private set; } = new AcessarCommand();

        public Usuario Usuario
        {
            get { return _Usuario; }
            set { SetField(ref _Usuario, value); }
        }

        public LoginViewModel()
        {
            Usuario = new Usuario();
        }

        public void ConfirmLogin()
        {
            Console.WriteLine("Login Confirmado. Login: " + Usuario.Login + ". Senha: " + Usuario.Senha);
        }
    }



    public class AcessarCommand : BaseCommand
    {
        /*
        public override bool CanExecute(object parameter)
        {
            var viewModel = (LoginViewModel) parameter;
            return viewModel.Usuario.Login != null && viewModel.Usuario.Senha != null;
        }

        */

        public override void Execute(object parameter)
        {
            var viewModel = (LoginViewModel)parameter;
            //viewModel.ConfirmLogin();

            //new CrudProdutoViewModel();
        }
    }

}
