using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SisMaper.ViewModel
{
    public class ProdutosViewModel : BaseViewModel, IProdutos
    {

        private Produto? _produtoSelecionado;

        public Produto ProdutoSelecionado
        {
            get { return _produtoSelecionado; }
            set { SetField(ref _produtoSelecionado, value); }
        }

        public PList<Produto> ProdutosList { get; private set; }
        public NovoProdutoCommand NovoProduto { get; private set; }
        public EditarProdutoCommand Editar { get; private set; }
        public ExcluirProdutoCommand Deletar { get; private set; }
        public OpenCategoriaCommand AbrirCategorias { get; private set; }
        public OpenUnidadeCommand AbrirUnidades { get; private set; }
        
        public IDialogCoordinator DialogCoordinator { get; set; }


        public Action OpenNovoProduto { get; set; }
        public Action OpenEditarProduto { get; set; }
        public Action ProdutoExcluido { get; set; }
        public Action OpenCategoria { get; set; }
        public Action OpenUnidade { get; set; }

        public ProdutosViewModel()
        {

            ProdutosList = DAO.FindWhereQuery<Produto>("Id > 0");

            NovoProduto = new NovoProdutoCommand();
            Editar = new EditarProdutoCommand();
            Deletar = new ExcluirProdutoCommand();
            AbrirCategorias = new OpenCategoriaCommand();
            AbrirUnidades = new OpenUnidadeCommand();


            DialogCoordinator = new DialogCoordinator();

            _produtoSelecionado = ProdutoSelecionado = null;

        }


        public void ExcluirProduto()
        {
            try
            {
                PList<Lote> lotesBanco = DAO.FindWhereQuery<Lote>("Id > 0");
                PList<Lote> lotesParaDeletar = new PList<Lote>();

                foreach(Lote l in lotesBanco)
                {
                    if(l.Produto.Id.Equals(ProdutoSelecionado.Id))
                    {
                        lotesParaDeletar.Add(l);
                    }
                }

                MessageDialogResult confirmacao = DialogCoordinator.ShowModalMessageExternal(this, "Excluir Produto", "Deseja Excluir produto selecionado?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Sim", NegativeButtonText = "Não" });

                if (confirmacao.Equals(MessageDialogResult.Affirmative))
                {
                    if (lotesParaDeletar.Count > 0)
                    {
                        lotesParaDeletar.DeleteAll();
                    }

                    ProdutoSelecionado.Delete();
                    ProdutoExcluido?.Invoke();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        internal void OpenCrudProdutos() => OpenNovoProduto?.Invoke();

        internal void OpenEditarCrudProdutos() => OpenEditarProduto?.Invoke();

        public void OpenCategorias() => OpenCategoria?.Invoke();

        public void OpenUnidades() => OpenUnidade?.Invoke();

    }





    public class OpenCategoriaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;
            vm.OpenCategorias();
        }
    }


    public class OpenUnidadeCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;
            vm.OpenUnidades();
        }
    }



    public class NovoProdutoCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;

            vm.OpenCrudProdutos();
        }
    }


    public class EditarProdutoCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;

            return !Equals(vm.ProdutoSelecionado, null);
        }


        public override void Execute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;
            vm.OpenEditarCrudProdutos();
        }
    }


    public class ExcluirProdutoCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;

            return !Equals(vm.ProdutoSelecionado, null);
        }
        public override void Execute(object parameter)
        {
            ProdutosViewModel vm = (ProdutosViewModel)parameter;
            vm.ExcluirProduto();
        }
    }



    public interface IProdutos
    {
        public Action OpenNovoProduto { get; set; }
        public Action OpenEditarProduto { get; set; }
        public Action ProdutoExcluido { get; set; }
        public Action OpenCategoria { get; set; }
        public Action OpenUnidade { get; set; }
    }

}
