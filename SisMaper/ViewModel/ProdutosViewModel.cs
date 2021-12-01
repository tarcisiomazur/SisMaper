using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SisMaper.Tools;

namespace SisMaper.ViewModel
{
    public class ProdutosViewModel : BaseViewModel, IProdutos
    {
        public ViewListarProdutos ProdutoSelecionado { get; set; }
        public List<ViewListarProdutos> Produtos { get; set; }
        public IEnumerable<ViewListarProdutos> ProdutosFiltrados { get; set; }
        
        public Categoria? CategoriaSelecionada { get; set; }
        public PList<Categoria> Categorias { get; set; }
        public string? TextoFiltro { get; set; }
        public bool? Inativos { get; set; } = false;
        
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
            Categorias = DAO.FindWhereQuery<Categoria>("ID > 0");
            NovoProduto = new NovoProdutoCommand();
            Editar = new EditarProdutoCommand();
            Deletar = new ExcluirProdutoCommand();
            AbrirCategorias = new OpenCategoriaCommand();
            AbrirUnidades = new OpenUnidadeCommand();
            
            DialogCoordinator = new DialogCoordinator();
            PropertyChanged += UpdateFilter;
            Produtos = View.Execute<ViewListarProdutos>();

        }

        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Produtos != null && e.PropertyName is nameof(CategoriaSelecionada) or nameof(Produtos) or nameof(TextoFiltro) or nameof(Inativos))
            {
                ProdutosFiltrados = Produtos.Where(p =>
                    (string.IsNullOrEmpty(TextoFiltro) ||
                     !string.IsNullOrEmpty(p.Descricao) && p.Descricao.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase) ||
                     !string.IsNullOrEmpty(p.CodigoBarras) && p.CodigoBarras.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase)) &&
                    (CategoriaSelecionada == null || p.Categoria == CategoriaSelecionada.Descricao) &&
                    (Inativos is null || Inativos == p.Inativo)
                );
            }
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

                    var p = DAO.Load<Produto>(ProdutoSelecionado.Id);
                    if (p?.Delete() ?? false)
                    {
                        ProdutoExcluido?.Invoke();
                    }
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
