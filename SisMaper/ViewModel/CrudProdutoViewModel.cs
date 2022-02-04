using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using SisMaper.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SisMaper.Models.Views;

namespace SisMaper.ViewModel
{

    public class CrudProdutoViewModel : BaseViewModel
    {

        private NCM? _ncmSelecionado;

        public NCM? NCMSelecionado
        {
            get { return _ncmSelecionado; }
            set { SetField(ref _ncmSelecionado, value); }
        }


        private Categoria? _categoriaSelecionada;

        public Categoria? CategoriaSelecionada
        {
            get { return _categoriaSelecionada; }
            set { SetField(ref _categoriaSelecionada, value); }
        }

        private Unidade? _unidadeSelecionada;

        public Unidade? UnidadeSelecionada
        {
            get { return _unidadeSelecionada; }
            set { SetField(ref _unidadeSelecionada, value); }
        }


        private Lote _loteSelecionado;

        public Lote LoteSelecionado
        {
            get { return _loteSelecionado; }
            set { SetField(ref _loteSelecionado, value); }
        }

        public Produto? Produto { get; set; }

        public double Valor1 { get; set; }

        public ObservableCollection<NCM> NCMs { get; private set; }
        public PList<Categoria> Categorias { get; private set; }
        public PList<Unidade> Unidades { get; private set; }
        public ObservableCollection<Lote> Lotes { get; set; }

        public PList<NCM> ListaNCM { get; private set; }

        public SalvarCommand Salvar { get; private set; }
        public AdicionarLoteCommand Adicionar { get; private set; }
        public RemoverLoteCommand Remover { get; private set; }
        public EditarCategoriaCommand EditarCategorias { get; private set; }
        public EditarUnidadeCommand EditarUnidades { get; private set; }


        public Action? ProdutoSaved { get; set; }
        public Action? OpenEditarCategoria { get; set; }
        public Action? OpenEditarUnidade { get; set; }



        public CrudProdutoViewModel(ListarProdutos? produtoSelecionado)
        {

            Salvar = new SalvarCommand();
            Adicionar = new AdicionarLoteCommand();
            Remover = new RemoverLoteCommand();
            EditarCategorias = new EditarCategoriaCommand();
            EditarUnidades = new EditarUnidadeCommand();

            NCMSelecionado = null;
            CategoriaSelecionada = null;
            UnidadeSelecionada = null;

            if (produtoSelecionado is not null)
            {
                Produto = DAO.Load<Produto>(produtoSelecionado.Id);
            }
            else
            {
                Produto = null;
            }

            ListaNCM = DAO.All<NCM>();
            Categorias = DAO.All<Categoria>();
            Unidades = DAO.All<Unidade>();

            Lotes = new ObservableCollection<Lote>();


            if (Produto is not null)
            {
                if(Produto.Categoria is not null) CategoriaSelecionada = Categorias.Where(cat => cat.Id == Produto.Categoria.Id).First();
                
                if(Produto.Unidade is not null) UnidadeSelecionada = Unidades.Where(uni => uni.Id == Produto.Unidade.Id).First();
                
                if(Produto.NCM is not null) NCMSelecionado = ListaNCM.Where(n => n.Id == Produto.NCM.Id).First();

                var lotesBanco = DAO.All<Lote>();

                foreach (Lote l in lotesBanco)
                {
                    if (l.Produto.Id.Equals(Produto.Id))
                    {
                        Console.WriteLine(l);
                        Lotes.Add(l);
                    }
                }

            }

            else
            {
                PList<Produto> produtos = DAO.All<Produto>();

                if (produtos.Count == 0)
                {
                    Produto = new Produto()
                    {
                        Id = 1
                    };

                }
                else
                {
                    Produto p2 = produtos.Last();

                    Produto = new Produto()
                    {
                        Id = p2.Id + 1
                    };
                }

            }

        }




      

        private void CheckCodigoBarras()
        {
            if(Produto.CodigoBarras is not null)
            {
                if(Produto.CodigoBarras.Length == 0)
                {
                    Produto.CodigoBarras = null;
                    return;
                }

                if(Produto.CodigoBarras.Length < 13)
                {
                    throw new InvalidOperationException("Código de Barras incompleto");
                }
            }
        }
        

        public void ExcluirLote()
        {
            try
            {
                Lote l = LoteSelecionado;
                Lotes.Remove(LoteSelecionado);
                l.Delete();
            }
            catch(Exception ex)
            {
                OnShowMessage("Erro ao remover lote", "Erro" + ex.Message);
            }
        }

        public void SalvarProduto()
        {
            try 
            {
                
                PList<Lote> listaLotesPraAdicionar = new PList<Lote>();

                Produto.Lotes = new PList<Lote>();

                foreach (Lote l in Lotes)
                {
                    listaLotesPraAdicionar.Add(l);
                    Produto.Lotes.Add(l);
                }
                

                Produto.Categoria = CategoriaSelecionada;
                Produto.Unidade = UnidadeSelecionada;
                Produto.NCM = NCMSelecionado;

            
            
                CheckCodigoBarras();

                Produto.Save();

                
                foreach (Lote l in listaLotesPraAdicionar)
                {
                    l.Produto = Produto;
                }

                listaLotesPraAdicionar.Save();
                

                ProdutoSaved?.Invoke();
            }

            catch (Exception ex)
            {
                OnShowMessage("Erro ao salvar produto", "Erro: " + ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings() {AffirmativeButtonText = "Ok" });
            }
        }


        public void EditCategoria()
        {
            OpenEditarCategoria?.Invoke();
            Categorias = DAO.All<Categoria>();
            CategoriaSelecionada = null;
        } 
        public void EditUnidade()
        {
            OpenEditarUnidade?.Invoke();
            Unidades = DAO.All<Unidade>();
            UnidadeSelecionada = null;
        }
        



        public class SalvarCommand : BaseCommand
        {
            public override bool CanExecute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                return !string.IsNullOrWhiteSpace(vm.Produto.Descricao);
            }

            public override void Execute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                vm.SalvarProduto();

            }
        }



        public class AdicionarLoteCommand : BaseCommand
        {
            public override void Execute(object parameter)
            {
                var viewModel = (CrudProdutoViewModel)parameter;

                Lote l = new Lote() { Descricao = "Descrição", Informacoes = "Informações" };

                viewModel.Lotes.Add(l);
                viewModel.LoteSelecionado = l;
            }
        }


        public class RemoverLoteCommand : BaseCommand
        {
            public override bool CanExecute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                return !Equals(vm.LoteSelecionado, null);
            }

            public override void Execute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                vm.ExcluirLote();
            }
        }



        public class EditarCategoriaCommand : BaseCommand
        {
            public override void Execute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                vm.EditCategoria();
            }
        }

        public class EditarUnidadeCommand : BaseCommand
        {
            public override void Execute(object parameter)
            {
                CrudProdutoViewModel vm = (CrudProdutoViewModel)parameter;
                vm.EditUnidade();
            }
        }

    }

}
