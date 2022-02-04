using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class ClientesViewModel : BaseViewModel
    {
        private ListarClientes _clienteSelecionado;

        public ListarClientes? ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set { SetField(ref _clienteSelecionado, value); }
        }

        //public PList<PessoaFisica>? PessoaFisicaList { get; private set; }
        //public PList<PessoaJuridica>? PessoaJuridicaList { get; private set; }

        public List<ListarClientes>? PessoaFisicaList { get; set; }
        public List<ListarClientes>? PessoaJuridicaList { get; set; }

        public Action<CrudClienteViewModel?>? OpenCrudCliente { get; set; }


        public NovoClienteCommand NovoCliente { get; private set; }
        public EditarClienteCommand EditarCliente { get; private set; }
        public ExcluirClienteCommand ExcluirCliente { get; set; }
        

        public ClientesViewModel()
        { 
            Initialize(null, EventArgs.Empty);


            NovoCliente = new NovoClienteCommand();
            EditarCliente = new EditarClienteCommand();
            ExcluirCliente = new ExcluirClienteCommand();

            ClienteSelecionado = null;
        }


        public void Initialize(object? sender, EventArgs e)
        {
            PessoaFisicaList = View.Execute<ListarClientes>().FindAll(cliente => cliente.Tipo == ListarClientes.Pessoa.Fisica);
            PessoaJuridicaList = View.Execute<ListarClientes>().FindAll(cliente => cliente.Tipo == ListarClientes.Pessoa.Juridica);
        }

        public void Clear(object? sender, EventArgs e)
        {
            PessoaFisicaList = null;
            PessoaJuridicaList = null;
        }



        public void OpenNovoClienteCrud() => OpenCrudCliente?.Invoke(new CrudClienteViewModel(null));

        public void OpenEditarClienteCrud() => OpenCrudCliente?.Invoke(new CrudClienteViewModel(ClienteSelecionado));

        
        
        public void DeletarCliente()
        {
            try
            {

                MessageDialogResult confirmacao = OnShowMessage("Excluir Cliente", "Deseja Excluir cliente selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                if(confirmacao.Equals(MessageDialogResult.Affirmative))
                {
                    if(ClienteSelecionado.Tipo == ListarClientes.Pessoa.Fisica)
                    {
                        PessoaFisica clienteToDelete = DAO.Load<PessoaFisica>(ClienteSelecionado.Id);
                        clienteToDelete.Delete();
                    }
                    else
                    {
                        PessoaJuridica clienteToDelete = DAO.Load<PessoaJuridica>(ClienteSelecionado.Id);
                        clienteToDelete.Delete();
                    }

                    Initialize(null, EventArgs.Empty);
                }

            }
            catch(Exception e)
            {
                OnShowMessage("Erro", "Erro ao excluir cliente: " + e.Message + "  Stack: " + e.StackTrace + "  Inenr: " + e.InnerException?.ToString());
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

}