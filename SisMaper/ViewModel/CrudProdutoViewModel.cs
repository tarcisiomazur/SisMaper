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

        public ObservableCollection<NCM> NCMs { get; private set; }
        public PList<Categoria> Categorias { get; private set; }
        public PList<Unidade> Unidades { get; private set; }
        public ObservableCollection<Lote> Lotes { get; set; }

        public PList<NCM> ListaNCM { get; private set; }

        public SimpleCommand SalvarProdutoCmd => new( SalvarProduto, () => !string.IsNullOrWhiteSpace(Produto?.Descricao) );
        public SimpleCommand AdicionarLoteCmd => new(AdicionarLote);
        public SimpleCommand RemoverLoteCmd => new(ExcluirLote, () => LoteSelecionado != null);
        public SimpleCommand EditarCategoriasCmd => new(EditarCategorias);
        public SimpleCommand EditarUnidadesCmd => new(EditarUnidades);

        public Action? ProdutoSaved { get; set; }
        public Action? OpenEditarCategoria { get; set; }
        public Action? OpenEditarUnidade { get; set; }



        public CrudProdutoViewModel(ListarProdutos? produtoSelecionado)
        {

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
                if(Produto.Categoria is not null) CategoriaSelecionada = Categorias.Where(cat => cat.Id == Produto.Categoria.Id).FirstOrDefault();
                
                if(Produto.Unidade is not null) UnidadeSelecionada = Unidades.Where(uni => uni.Id == Produto.Unidade.Id).FirstOrDefault();
                
                if(Produto.NCM is not null) NCMSelecionado = ListaNCM.Where(n => n.Id == Produto.NCM.Id).FirstOrDefault();

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
                OnShowMessage("Erro ao remover lote", "Erro " + ex.Message);
            }
        }

        public void SalvarProduto()
        {
            try 
            {
                if(Produto.PrecoCusto > Produto.PrecoVenda)
                {
                    OnShowMessage("Preço Inválido", "Margem negativa", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "Ok" });
                    return;
                }
                
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


        private void EditarCategorias()
        {
            OpenEditarCategoria?.Invoke();
            Categorias = DAO.All<Categoria>();
            CategoriaSelecionada = null;
        }

        private void EditarUnidades()
        {
            OpenEditarUnidade?.Invoke();
            Unidades = DAO.All<Unidade>();
            UnidadeSelecionada = null;
        }
        
        private void AdicionarLote()
        {
            Lote l = new Lote() { Descricao = "Descrição", Informacoes = "Informações" };
            Lotes.Add(l);
            LoteSelecionado = l;
        }

    }

}
