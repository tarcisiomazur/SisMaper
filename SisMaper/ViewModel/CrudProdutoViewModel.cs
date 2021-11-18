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

namespace SisMaper.ViewModel
{
    
    public class CrudProdutoViewModel : BaseViewModel, ICloseWindow
    {
        private NCM _ncmSelecionado;

        public NCM NCMSelecionado
        {
            get { return _ncmSelecionado; }
            set { SetField(ref _ncmSelecionado, value); }
        }

        public event Action OnSave;

        private Categoria _categoriaSelecionada;

        public Categoria CategoriaSelecionada
        {
            get { return _categoriaSelecionada; }
            set { SetField(ref _categoriaSelecionada, value); }
        }

        private Unidade _unidadeSelecionada;

        public Unidade UnidadeSelecionada
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

        public string TextoCategoria { get; set; }
        public string TextoUnidade { get; set; }
        public Produto Produto { get; set; }

        public ObservableCollection<NCM> NCMs { get; private set; }
        public ObservableCollection<Categoria> Categorias { get; private set; }
        public ObservableCollection<Unidade> Unidades { get; private set; }
        public ObservableCollection<Lote> Lotes { get; set; }

        public PList<NCM> ListaNCM { get; private set; }

        public SalvarCommand Salvar { get; private set; }
        public AdicionarLoteCommand Adicionar { get; private set; }
        public RemoverLoteCommand Remover { get; private set; }

        public IDialogCoordinator DialogCoordinator { get; set; }
        
        public Action Close { get; set; }


        public CrudProdutoViewModel(object produtoSelecionado)
        {

            Salvar = new SalvarCommand();
            Adicionar = new AdicionarLoteCommand();
            Remover = new RemoverLoteCommand();

            Produto = (Produto) produtoSelecionado;

            ListaNCM = DAO.FindWhereQuery<NCM>("Id > 0");
            PList<Categoria> listaCategorias = DAO.FindWhereQuery<Categoria>("Id > 0");
            PList<Unidade> listaUnidades = DAO.FindWhereQuery<Unidade>("Id > 0");
            

            DialogCoordinator = new DialogCoordinator();

            TextoCategoria = "";
            TextoUnidade = "";

            Lotes = new ObservableCollection<Lote>();

            _loteSelecionado = LoteSelecionado = new Lote();
            
            Categorias = new ObservableCollection<Categoria>();
            Unidades = new ObservableCollection<Unidade>();


            foreach(Categoria c in listaCategorias)
            {
                Categorias.Add(c);
            }

            foreach(Unidade u in listaUnidades)
            {
                Unidades.Add(u);
            }


            if (Produto is not null)
            {

                CategoriaSelecionada = Produto.Categoria;
                UnidadeSelecionada = Produto.Unidade;

                if(!Equals(CategoriaSelecionada, null))
                {
                    TextoCategoria = CategoriaSelecionada.Descricao;
                }
                else
                {
                    TextoCategoria = "";
                }



                if (!Equals(UnidadeSelecionada, null))
                {
                    TextoUnidade = UnidadeSelecionada.Descricao;
                }
                else
                {
                    TextoUnidade = "";
                }


                var lotesBanco = DAO.FindWhereQuery<Lote>("Id > 0");


                foreach(Lote l in lotesBanco)
                {
                    //Console.WriteLine(l);
                    if(l.Produto.Id.Equals(Produto.Id))
                    {
                        Console.WriteLine(l);
                        Lotes.Add(l);
                    }
                }
                
            }

            else
            {
                PList<Produto> pl = DAO.FindWhereQuery<Produto>("Id > 0");

                if (pl.Count == 0)
                {
                    Produto=new Produto()
                    {
                        Id = 1
                    };
                    
                }
                else
                {

                    Produto p2 = pl.Last();

                    Produto=new Produto()
                    {
                        Id = p2.Id + 1
                    };
                }
                NCMSelecionado = null;
                CategoriaSelecionada = null;
                UnidadeSelecionada = null;
            }

            /*
            _ncmSelecionado = NCMSelecionado = null;
            _categoriaSelecionada = CategoriaSelecionada = null;
            _unidadeSelecionada = UnidadeSelecionada = null;
            */


            //Produtos.CollectionChanged += OnProdutosCollectionChanged;
            //Produtos.CollectionChanged += new NotifyCollectionChangedEventHandler(OnProdutosCollectionChanged);

            //Produtos[0].PropertyChanged += OnProdutosCollectionChanged;
        }


        private void SalvarCategoriaNoBanco(Categoria c)
        {
            c.Save();
        }

        private void SalvarUnidadeNoBanco(Unidade u)
        {
            u.Save();
        }


        private void CheckCategoria()
        {
            
            if(string.IsNullOrWhiteSpace(TextoCategoria))
            {
                CategoriaSelecionada = null;
                return;
            }

            if(!object.Equals(CategoriaSelecionada, null) && CategoriaSelecionada.Descricao.Equals(TextoCategoria))
            {
                return;
            }

            Categoria c = new Categoria() { Descricao = TextoCategoria };
            Categorias.Add(c);
            CategoriaSelecionada = c;

            SalvarCategoriaNoBanco(c);

            
            
        }

        private void CheckUnidade()
        {
            
            if(string.IsNullOrWhiteSpace(TextoUnidade))
            {
                UnidadeSelecionada = null;
                return;
            }

            if(!object.Equals(UnidadeSelecionada, null) && UnidadeSelecionada.Descricao.Equals(TextoUnidade))
            {
                return;
            }

            Unidade u = new Unidade() { Descricao = TextoUnidade };
            Unidades.Add(u);
            UnidadeSelecionada = u;

            SalvarUnidadeNoBanco(u);

            
            
        }

        public void ExcluirLote()
        {
            Lotes.Remove(LoteSelecionado);
        }

        public void SalvarProduto()
        {
            CheckCategoria();
            CheckUnidade();

            PList<Lote> listaLotesPraAdicionar = new PList<Lote>();

            Produto.Lotes = new PList<Lote>();

            foreach(Lote l in Lotes)
            {
                listaLotesPraAdicionar.Add(l);
                Produto.Lotes.Add(l);
            }

            Produto.Categoria = CategoriaSelecionada;
            Produto.Unidade = UnidadeSelecionada;

            try
            {
                Produto.Save();

                foreach (Lote l in listaLotesPraAdicionar)
                {
                    l.Produto = Produto;
                }

                listaLotesPraAdicionar.Save();

                Close?.Invoke();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }




        public class SalvarCommand : BaseCommand
        {

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

    }



    public interface ICloseWindow
    {
        public Action Close { get; set; }
    }
}
