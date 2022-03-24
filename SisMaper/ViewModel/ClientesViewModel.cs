using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
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
        private ListarClientes? _clienteSelecionado;

        public ListarClientes? ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set { SetField(ref _clienteSelecionado, value); }
        }

        public List<ListarClientes>? PessoaFisicaList { get; set; }
        public List<ListarClientes>? PessoaJuridicaList { get; set; }

        public SimpleCommand NovoClienteCmd => new( () => OpenCrudCliente?.Invoke(new CrudClienteViewModel(null)) );
        public SimpleCommand EditarClienteCmd => new( () => OpenCrudCliente?.Invoke(new CrudClienteViewModel(ClienteSelecionado)), () => ClienteSelecionado != null );
        public SimpleCommand ExcluirClienteCmd => new( DeletarCliente, () => ClienteSelecionado != null);

        public Action<CrudClienteViewModel?>? OpenCrudCliente { get; set; }

        public ClientesViewModel() { }


        public void Initialize(object? sender, EventArgs e)
        {
            List<ListarClientes>? listaClientes = View.Execute<ListarClientes>();
            PessoaFisicaList = listaClientes.FindAll(cliente => cliente.Tipo == ListarClientes.Pessoa.Fisica);
            PessoaJuridicaList = listaClientes.FindAll(cliente => cliente.Tipo == ListarClientes.Pessoa.Juridica);
            ClienteSelecionado = null;
        }

        public void Clear(object? sender, EventArgs e)
        {
            PessoaFisicaList = null;
            PessoaJuridicaList = null;
        }

        
        private void DeletarCliente()
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
            catch (Exception ex)
            {
                if (ex.InnerException is not null && ex.InnerException is MySqlException)
                {
                    if (ex.InnerException.Message.StartsWith("Cannot delete or update a parent row")) OnShowMessage("Erro ao excluir cliente", "O cliente está vinculado em algum pedido");
                }
            }

        }
    }


}