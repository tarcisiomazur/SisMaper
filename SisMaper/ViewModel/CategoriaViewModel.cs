using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SisMaper.ViewModel
{
    public class CategoriaViewModel : BaseViewModel
    {
        public List<Categoria> Categorias { get; private set; }

        public SimpleCommand AdicionarCategoriaCmd => new(() => AdicionarCategoria());
        public SimpleCommand EditarCategoriaCmd => new(EditarCategoria, () => !string.IsNullOrEmpty(CategoriaSelecionada?.Descricao));
        public SimpleCommand RemoverCategoriaCmd => new(ExcluirCategoria, () => !string.IsNullOrEmpty(CategoriaSelecionada?.Descricao));


        private Categoria _categoriaSelecionada;

        public Categoria CategoriaSelecionada
        {
            get { return _categoriaSelecionada; }
            set { SetField(ref _categoriaSelecionada, value); }
        }

        

        public CategoriaViewModel()
        {
            Categorias = DAO.All<Categoria>().ToList();
            _categoriaSelecionada = CategoriaSelecionada = new Categoria();
        }



        private bool SalvarCategoriaNoBanco(Categoria c)
        {
            try
            {
                c.Save();
                return true;
            }
            catch(Exception ex)
            {
                OnShowMessage("Erro", "Erro ao salvar categoria: " + ex.Message, MessageDialogStyle.Affirmative);
                return false;
            }
        }

        private bool DeletarCategoriaDoBanco(Categoria c)
        {
            try
            {
                c.Delete();
                return true;
            }
            catch (Exception ex)
            {
                
                if(ex is MySqlConnector.MySqlConnectorException && ex.InnerException is MySqlException)
                {
                    if( (ex as MySqlConnector.MySqlConnectorException).ErrorCode == -2147467259)
                    {
                        OnShowMessage("Erro", String.Format("Categoria {0} está vinculada a algum produto, não pode ser excluida", c.Descricao), MessageDialogStyle.Affirmative);
                        return false;
                    }

                    OnShowMessage("Erro", String.Format("Erro ao deletar categoria {0}: " + ex.Message, c.Descricao), MessageDialogStyle.Affirmative);
                    return false;
                }

                OnShowMessage("Erro", String.Format("Erro ao deletar categoria {0}: " + ex.Message, c.Descricao), MessageDialogStyle.Affirmative);
                return false;
            }
        }

        private bool EditarCategoriaDoBanco(Categoria original, Categoria nova)
        {
            try
            {
                original.Descricao = nova.Descricao;
                original.Save();
                return true;
            }
            catch(Exception ex)
            {
                OnShowMessage("Erro", "Erro ao editar categoria: " + ex.Message, MessageDialogStyle.Affirmative);
                return false;
            }
        }




        public void AdicionarCategoria(string defaultText = "")
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = defaultText
            };

            string? cat = OnInput("Adicionar Categoria", "Categoria", dialogSettings);


            foreach(Categoria element in Categorias)
            {
                if(element.Descricao.Equals(cat))
                {
                    OnShowMessage("Erro", "Categoria ja existente");
                    AdicionarCategoria(cat);
                    return;
                }
            }


            Categoria c = new Categoria()
            {
                Descricao = cat
            };



            if (!string.IsNullOrEmpty(cat) && !string.IsNullOrWhiteSpace(cat))
            {
                if (SalvarCategoriaNoBanco(c))
                {
                    Categorias.Add(c);
                    RaisePropertyChanged(nameof(Categorias));
                    OnShowMessage("Categoria", "Categoria adicionada com sucesso");
                }
                return;
            }

            else if(string.Equals(cat, string.Empty) || (string.IsNullOrWhiteSpace(cat) && !string.Equals(cat, null)))
            {
                OnShowMessage("Erro", "Categoria não pode ser vazia!!");
                AdicionarCategoria("");
                return;
            }
            

            else
            {
                return;
            }

        }

        public void EditarCategoria()
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = CategoriaSelecionada.Descricao
            };

            string? cat = OnInput("Editar Categoria", "Categoria", dialogSettings);

            foreach (Categoria element in Categorias)
            {
                if (element.Descricao.Equals(cat) && element.Id != CategoriaSelecionada.Id)
                {
                    OnShowMessage("Erro", "Categoria já existente");
                    EditarCategoria();
                    return;
                }
            }



            Categoria c = new Categoria()
            {
                Descricao = cat
            };

            if (!string.IsNullOrEmpty(cat) && !string.IsNullOrWhiteSpace(cat))
            {
                
                if (EditarCategoriaDoBanco(Categorias[Categorias.IndexOf(CategoriaSelecionada)], c))
                {
                    Categorias[Categorias.IndexOf(CategoriaSelecionada)] = c;
                    RaisePropertyChanged(nameof(Categorias));
                    OnShowMessage("Categoria", "Categoria editada com sucesso");
                }
                
                return;
            }

            else if (string.Equals(cat, string.Empty) || (string.IsNullOrWhiteSpace(cat) && !string.Equals(cat, null)))
            {
                OnShowMessage("Erro", "Categoria não pode ser vazia!!");
                EditarCategoria();
                return;
            }


            else
            {
                return;
            }
        }


        public void ExcluirCategoria()
        {
            MessageDialogResult afirmacao = OnShowMessage("Confirmação", "Excluir categoria " + CategoriaSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);
            
            if(afirmacao.Equals(MessageDialogResult.Affirmative))
            {
                
                if (DeletarCategoriaDoBanco(CategoriaSelecionada))
                {
                    Categorias.Remove(CategoriaSelecionada);
                    RaisePropertyChanged(nameof(Categorias));
                    OnShowMessage("Confirmado", "Categoria removida");
                }
                
            }
            
        }
    }
}
