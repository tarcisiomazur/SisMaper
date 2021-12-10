using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class ClientesViewModel : BaseViewModel, ICliente
    {
        private Cliente _clienteSelecionado;

        public Cliente ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set { SetField(ref _clienteSelecionado, value); }
        }

        public PList<PessoaFisica> PessoaFisicaList { get; private set; }
        public PList<PessoaJuridica> PessoaJuridicaList { get; private set; }

        public Action OpenNovoCliente { get; set; }
        public Action OpenEditarCliente { get; set; }
        public Action ClienteExcluido { get; set; }


        public IDialogCoordinator DialogCoordinator { get; set; }

        public NovoClienteCommand NovoCliente { get; private set; }
        public EditarClienteCommand EditarCliente { get; private set; }
        public ExcluirClienteCommand ExcluirCliente { get; set; }
        

        public ClientesViewModel()
        {
            PessoaFisicaList = DAO.All<PessoaFisica>();
            PessoaJuridicaList = DAO.All<PessoaJuridica>();
            
            DialogCoordinator = new DialogCoordinator();

            NovoCliente = new NovoClienteCommand();
            EditarCliente = new EditarClienteCommand();
            ExcluirCliente = new ExcluirClienteCommand();

            ClienteSelecionado = null;
        }


        public void OpenNovoClienteCrud()
        {
            OpenNovoCliente?.Invoke();
        }



        public void OpenEditarClienteCrud()
        {
            OpenEditarCliente?.Invoke();
        }

        public void DeletarCliente()
        {
            try
            {
                Cliente clienteToDelete = new Cliente();

                PList<Cliente> clientes = DAO.All<Cliente>();

                foreach(Cliente c in clientes)
                {
                    if(c.Id == ClienteSelecionado.Id)
                    {
                        clienteToDelete = ClienteSelecionado;
                        break;
                    }
                }

                MessageDialogResult confirmacao = DialogCoordinator.ShowModalMessageExternal(this, "Excluir Cliente", "Deseja Excluir cliente selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                if(confirmacao.Equals(MessageDialogResult.Affirmative))
                {
                    ClienteSelecionado.Delete();
                    clienteToDelete.Delete();

                    ClienteExcluido?.Invoke();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }

        }
    }




    public class NovoClienteCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ClientesViewModel vm = (ClientesViewModel)parameter;
            vm.OpenNovoClienteCrud();
        }
    }


    public class EditarClienteCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            ClientesViewModel vm = (ClientesViewModel)parameter;
            return vm.ClienteSelecionado is not null;
        }

        public override void Execute(object parameter)
        {
            ClientesViewModel vm = (ClientesViewModel)parameter;
            vm.OpenEditarClienteCrud();
        }
    }


    public class ExcluirClienteCommand : BaseCommand
    {

        public override bool CanExecute(object parameter)
        {
            ClientesViewModel vm = (ClientesViewModel)parameter;
            return vm.ClienteSelecionado is not null;
        }
        public override void Execute(object parameter)
        {
            ClientesViewModel vm = (ClientesViewModel)parameter;
            vm.DeletarCliente();
        }
    }




    public interface ICliente
    {
        public Action OpenNovoCliente { get; set; }
        public Action OpenEditarCliente { get; set; }
        public Action ClienteExcluido { get; set; }
    }

}

