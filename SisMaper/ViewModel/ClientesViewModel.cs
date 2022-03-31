using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Persistence;
using SisMaper.Models;
using SisMaper.Models.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        public string TextoFiltro { get; set; } = String.Empty;

        private List<ListarClientes>? clientes;
        private IEnumerable<ListarClientes>? clientesFiltrados;
        public IEnumerable<ListarClientes>? PessoaFisicaList { get; set; }
        public IEnumerable<ListarClientes>? PessoaJuridicaList { get; set; }

        public SimpleCommand NovoClienteCmd => new( () => OpenCrudCliente?.Invoke(new CrudClienteViewModel(null)) );
        public SimpleCommand EditarClienteCmd => new( () => OpenCrudCliente?.Invoke(new CrudClienteViewModel(ClienteSelecionado)), () => ClienteSelecionado != null );
        public SimpleCommand ExcluirClienteCmd => new(DeletarCliente, _ => ClienteSelecionado != null);

        public Action<CrudClienteViewModel?>? OpenCrudCliente { get; set; }

        public ClientesViewModel() 
        { 
            PropertyChanged += UpdateFilter;
        }


        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (clientes != null && e.PropertyName is nameof(TextoFiltro))
            {

                clientesFiltrados = clientes.Where(c =>
                    (!string.IsNullOrWhiteSpace(c.Nome) && c.Nome.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.CPF_CNPJ) && c.CPF_CNPJ.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase))
                ).OrderBy(c => c.Nome);

                PessoaFisicaList = clientesFiltrados.Where(cliente => cliente.Tipo == ListarClientes.Pessoa.Fisica);
                PessoaJuridicaList = clientesFiltrados.Where(cliente => cliente.Tipo == ListarClientes.Pessoa.Juridica);
            }
        }


        public void Initialize(object? sender, EventArgs e)
        {
            clientes = View.Execute<ListarClientes>();
            RaisePropertyChanged(nameof(TextoFiltro));
            ClienteSelecionado = null;
        }

        public void Clear(object? sender, EventArgs e)
        {
            clientes = null;
            PessoaFisicaList = null;
            PessoaJuridicaList = null;
            clientesFiltrados = null;
            TextoFiltro = string.Empty;
        }

        
        private void DeletarCliente()
        {
            try
            {
                MessageDialogResult confirmacao = OnShowMessage("Excluir Cliente", "Deseja Excluir cliente selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() {AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                if(confirmacao.Equals(MessageDialogResult.Affirmative))
                {
                    if(ClienteSelecionado?.Tipo == ListarClientes.Pessoa.Fisica)
                    {
                        PessoaFisica clienteToDelete = DAO.Load<PessoaFisica>(ClienteSelecionado.Id);
                        clienteToDelete?.Delete();
                    }
                    else
                    {
                        PessoaJuridica clienteToDelete = DAO.Load<PessoaJuridica>(ClienteSelecionado.Id);
                        clienteToDelete?.Delete();
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