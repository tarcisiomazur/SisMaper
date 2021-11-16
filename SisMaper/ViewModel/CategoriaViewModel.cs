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
using System.Windows.Input;

namespace SisMaper.ViewModel
{
    public class CategoriaViewModel : BaseViewModel
    {
        public ObservableCollection<Categoria> Categorias { get; private set; }

        public AdicionarCategoriaCommand AdicionarCat { get; private set; } = new AdicionarCategoriaCommand();
        public EditarCategoriaCommand EditarCat { get; private set; } = new EditarCategoriaCommand();
        public RemoverCategoriaCommand RemoverCat { get; private set; } = new RemoverCategoriaCommand();


        private Categoria _categoriaSelecionada;

        public Categoria CategoriaSelecionada
        {
            get { return _categoriaSelecionada; }
            set { SetField(ref _categoriaSelecionada, value); }
        }


        public IDialogCoordinator DialogCoordinator { get; set; }

        public CategoriaViewModel()
        {
            PList<Categoria> lista = DAO.FindWhereQuery<Categoria>("Id > 0");

            Categorias = new ObservableCollection<Categoria>();
            DialogCoordinator = new DialogCoordinator();



            foreach(Categoria c in lista)
            {
                Categorias.Add(c);
            }


            _categoriaSelecionada = CategoriaSelecionada = new Categoria();

        }


        private void SalvarCategoriaNoBanco(Categoria c)
        {
            c.Save();
        }

        private void DeletarCategoriaDoBanco(Categoria c)
        {
            c.Delete();
        }

        private void EditarCategoriaDoBanco(Categoria original, Categoria nova)
        {
            original.Descricao = nova.Descricao;
            original.Save();
        }


        public void AdicionarCategoria(string defaultText)
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = defaultText
            };

            string cat = DialogCoordinator.ShowModalInputExternal(this, "Adicionar Categoria", "Categoria", dialogSettings);


            foreach(Categoria element in Categorias)
            {
                if(element.Descricao.Equals(cat))
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Categoria ja existente");
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
                Categorias.Add(c);
                SalvarCategoriaNoBanco(c);
                DialogCoordinator.ShowModalMessageExternal(this, "Categoria", "Categoria adicionada com sucesso");
                return;
            }

            else if(string.Equals(cat, string.Empty) || (string.IsNullOrWhiteSpace(cat) && !string.Equals(cat, null)))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Categoria não pode ser vazia!!");
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

            string cat = DialogCoordinator.ShowModalInputExternal(this, "Editar Categoria", "Categoria", dialogSettings);

            foreach (Categoria element in Categorias)
            {
                if (element.Descricao.Equals(cat))
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Categoria ja existente");
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
                EditarCategoriaDoBanco(Categorias[Categorias.IndexOf(CategoriaSelecionada)], c);
                Categorias[Categorias.IndexOf(CategoriaSelecionada)] = c;
                DialogCoordinator.ShowModalMessageExternal(this, "Categoria", "Categoria editada com sucesso");
                return;
            }

            else if (string.Equals(cat, string.Empty) || (string.IsNullOrWhiteSpace(cat) && !string.Equals(cat, null)))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Categoria não pode ser vazia!!");
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
            MessageDialogResult afirmacao = DialogCoordinator.ShowModalMessageExternal(this, "Confirmação", "Excluir categoria " + CategoriaSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);
            
            if(afirmacao.Equals(MessageDialogResult.Affirmative))
            {
                DeletarCategoriaDoBanco(CategoriaSelecionada);
                Categorias.Remove(CategoriaSelecionada);
                DialogCoordinator.ShowModalMessageExternal(this, "Confirmado", "Categoria removida");
            }
            
        }
    }





    public class AdicionarCategoriaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            CategoriaViewModel vm = (CategoriaViewModel)parameter;
            vm.AdicionarCategoria("");
        }
    }



    public class EditarCategoriaCommand : BaseCommand
    {

        public override bool CanExecute(object parameter)
        {
            CategoriaViewModel vm = (CategoriaViewModel)parameter;
            return !String.IsNullOrEmpty(vm.CategoriaSelecionada?.Descricao);
        }

        public override void Execute(object parameter)
        {
            CategoriaViewModel vm = (CategoriaViewModel)parameter;
            vm.EditarCategoria();
        }
    }



    public class RemoverCategoriaCommand : BaseCommand
    {
        
        public override bool CanExecute(object parameter)
        {
            CategoriaViewModel vm = (CategoriaViewModel)parameter;
            return !String.IsNullOrEmpty(vm.CategoriaSelecionada?.Descricao);
        }

        

        public override void Execute(object parameter)
        {
            CategoriaViewModel vm = (CategoriaViewModel)parameter;
            vm.ExcluirCategoria();

        }
    }




}
