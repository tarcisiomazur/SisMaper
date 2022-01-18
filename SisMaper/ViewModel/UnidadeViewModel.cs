using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.ViewModel
{
    public class UnidadeViewModel : BaseViewModel
    {
        public ObservableCollection<Unidade> Unidades { get; private set; }

        public AdicionarUnidadeCommand AdicionarUni { get; private set; } = new AdicionarUnidadeCommand();
        public EditarUnidadeCommand EditarUni { get; private set; } = new EditarUnidadeCommand();
        public RemoverUnidadeCommand RemoverUni { get; private set; } = new RemoverUnidadeCommand();


        private Unidade _unidadeSelecionada;

        public Unidade UnidadeSelecionada
        {
            get { return _unidadeSelecionada; }
            set { SetField(ref _unidadeSelecionada, value); }
        }


        public IDialogCoordinator DialogCoordinator { get; set; }
        
        public UnidadeViewModel()
        {
            PList<Unidade> listaUnidade = DAO.All<Unidade>();

            Unidades = new ObservableCollection<Unidade>();
            DialogCoordinator = new DialogCoordinator();


            
            foreach(Unidade c in listaUnidade)
            {
                Unidades.Add(c);
            }


            _unidadeSelecionada = UnidadeSelecionada = new Unidade();

        }


        private bool SalvarUnidadeNoBanco(Unidade u)
        {
            try
            {
                u.Save();
                return true;
            }
            catch (Exception ex)
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Erro ao salvar unidade: " + ex.Message, MessageDialogStyle.Affirmative);
                return false;
            }
        }

        private bool DeletarUnidadeDoBanco(Unidade u)
        {
            try
            {
                u.Delete();
                return true;
            }
            catch (Exception ex)
            {

                if (ex is MySqlConnector.MySqlConnectorException && ex.InnerException is MySqlException)
                {
                    //MessageBox.Show((ex as MySqlConnector.MySqlConnectorException).ErrorCode.ToString());
                    if ((ex as MySqlConnector.MySqlConnectorException).ErrorCode == -2147467259)
                    {
                        DialogCoordinator.ShowModalMessageExternal(this, "Erro", String.Format("Unidade {0} está vinculada a algum produto, não pode ser excluida", u.Descricao), MessageDialogStyle.Affirmative);
                        return false;
                    }

                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", String.Format("Erro ao deletar unidade {0}: " + ex.Message, u.Descricao), MessageDialogStyle.Affirmative);
                    return false;
                }

                DialogCoordinator.ShowModalMessageExternal(this, "Erro", String.Format("Erro ao deletar unidade {0}: " + ex.Message, u.Descricao), MessageDialogStyle.Affirmative);
                return false;
            }
        }

        private bool EditarUnidadeDoBanco(Unidade original, Unidade nova)
        {
            try
            {
                original.Descricao = nova.Descricao;
                original.Save();
                return true;
            }
            catch (Exception ex)
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Erro ao editar unidade: " + ex.Message, MessageDialogStyle.Affirmative);
                return false;
            }
        }








        public void AdicionarUnidade(string defaultText)
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = defaultText
            };

            string uni = DialogCoordinator.ShowModalInputExternal(this, "Adicionar Unidade", "Unidade", dialogSettings);


            foreach (Unidade element in Unidades)
            {
                if (element.Descricao.Equals(uni))
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Unidade ja existente");
                    AdicionarUnidade(uni);
                    return;
                }
            }


            Unidade u = new Unidade()
            {
                Descricao = uni
            };



            if (!string.IsNullOrEmpty(uni) && !string.IsNullOrWhiteSpace(uni))
            {
                if (SalvarUnidadeNoBanco(u))
                {
                    Unidades.Add(u);
                    DialogCoordinator.ShowModalMessageExternal(this, "Unidade", "Unidade adicionada com sucesso");
                }
                return;
            }

            else if (string.Equals(uni, string.Empty) || (string.IsNullOrWhiteSpace(uni) && !string.Equals(uni, null)))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Unidade não pode ser vazia!!");
                AdicionarUnidade("");
                return;
            }


            else
            {
                return;
            }

        }

        public void EditarUnidade()
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = UnidadeSelecionada.Descricao
            };

            string uni = DialogCoordinator.ShowModalInputExternal(this, "Editar Unidade", "Unidade", dialogSettings);

            foreach (Unidade element in Unidades)
            {
                if (element.Descricao.Equals(uni))
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Unidade ja existente");
                    EditarUnidade();
                    return;
                }
            }


            Unidade u = new Unidade()
            {
                Descricao = uni
            };

            if (!string.IsNullOrEmpty(uni) && !string.IsNullOrWhiteSpace(uni))
            {
                if (EditarUnidadeDoBanco(Unidades[Unidades.IndexOf(UnidadeSelecionada)], u))
                {
                    Unidades[Unidades.IndexOf(UnidadeSelecionada)] = u;
                    DialogCoordinator.ShowModalMessageExternal(this, "Unidade", "Unidade editada com sucesso");
                }
                return;
            }

            else if (string.Equals(uni, string.Empty) || (string.IsNullOrWhiteSpace(uni) && !string.Equals(uni, null)))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Unidade não pode ser vazia!!");
                EditarUnidade();
                return;
            }


            else
            {
                return;
            }
        }

        public void ExcluirUnidade()
        {
            MessageDialogResult afirmacao = DialogCoordinator.ShowModalMessageExternal(this, "Confirmação", "Excluir unidade " + UnidadeSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);

            if (afirmacao.Equals(MessageDialogResult.Affirmative))
            {
                if (DeletarUnidadeDoBanco(UnidadeSelecionada))
                {
                    Unidades.Remove(UnidadeSelecionada);
                    DialogCoordinator.ShowModalMessageExternal(this, "Confirmado", "Unidade removida");
                }
            }

        }
    }





    public class AdicionarUnidadeCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            UnidadeViewModel vm = (UnidadeViewModel)parameter;
            vm.AdicionarUnidade("");
        }
    }



    public class EditarUnidadeCommand : BaseCommand
    {

        public override bool CanExecute(object parameter)
        {
            var vm = (UnidadeViewModel)parameter;
            return !String.IsNullOrEmpty(vm.UnidadeSelecionada?.Descricao);
        }

        public override void Execute(object parameter)
        {
            UnidadeViewModel vm = (UnidadeViewModel)parameter;
            vm.EditarUnidade();
        }
    }



    public class RemoverUnidadeCommand : BaseCommand
    {

        public override bool CanExecute(object parameter)
        {
            UnidadeViewModel vm = (UnidadeViewModel)parameter;
            return !String.IsNullOrEmpty(vm.UnidadeSelecionada?.Descricao);
        }



        public override void Execute(object parameter)
        {
            UnidadeViewModel vm = (UnidadeViewModel)parameter;
            vm.ExcluirUnidade();

        }
    }
}
