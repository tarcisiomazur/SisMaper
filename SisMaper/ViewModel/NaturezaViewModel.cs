using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.ObjectModel;

namespace SisMaper.ViewModel
{
    public class NaturezaViewModel : BaseViewModel
    {
        public ObservableCollection<Natureza> Naturezas { get; private set; }

        public AdicionarNaturezaCommand AdicionarNat { get; private set; } = new AdicionarNaturezaCommand();
        public EditarNaturezaCommand EditarNat { get; private set; } = new EditarNaturezaCommand();
        public RemoverNaturezaCommand RemoverNat { get; private set; } = new RemoverNaturezaCommand();


        private Natureza _NaturezaSelecionada;

        public Natureza NaturezaSelecionada
        {
            get { return _NaturezaSelecionada; }
            set { SetField(ref _NaturezaSelecionada, value); }
        }


        public IDialogCoordinator DialogCoordinator { get; set; }

        public NaturezaViewModel()
        {
            PList<Natureza> lista = DAO.FindWhereQuery<Natureza>("Id > 0");

            Naturezas = new ObservableCollection<Natureza>();
            DialogCoordinator = new DialogCoordinator();


            foreach (Natureza c in lista)
            {
                Naturezas.Add(c);
            }


            _NaturezaSelecionada = NaturezaSelecionada = new Natureza();
        }


        private void SalvarNaturezaNoBanco(Natureza c)
        {
            c.Save();
        }

        private void DeletarNaturezaDoBanco(Natureza c)
        {
            c.Delete();
        }

        private void EditarNaturezaDoBanco(Natureza original, Natureza nova)
        {
            original.Descricao = nova.Descricao;
            original.Save();
        }


        public void AdicionarNatureza()
        {
            string desc = DialogCoordinator.ShowModalInputExternal(this, "Editar Natureza", "Digite a Descrição");
            if (desc is null) return;
            string classe =
                DialogCoordinator.ShowModalInputExternal(this, "Editar Natureza", "Digite a Classe de Imposto");
            if (classe is null) return;


            foreach (Natureza element in Naturezas)
            {
                if (element.Descricao.Equals(desc))
                {
                    DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Natureza ja existente");
                    return;
                }
            }


            Natureza c = new Natureza()
            {
                Descricao = desc,
                Classe_de_Imposto = classe
            };


            if (!string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(classe))
            {
                Naturezas.Add(c);
                SalvarNaturezaNoBanco(c);
                DialogCoordinator.ShowModalMessageExternal(this, "Natureza", "Natureza adicionada com sucesso");
                return;
            }

            else if (string.Equals(desc, string.Empty) ||
                     (string.IsNullOrWhiteSpace(desc) && !string.Equals(desc, null)))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Natureza não pode ser vazia!!");
                return;
            }


            else
            {
                return;
            }
        }

        public void EditarNatureza()
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = NaturezaSelecionada.Descricao
            };

            string desc =
                DialogCoordinator.ShowModalInputExternal(this, "Editar Natureza", "Digite a Descrição", dialogSettings);
            dialogSettings.DefaultText = NaturezaSelecionada.Classe_de_Imposto;
            string classe = DialogCoordinator.ShowModalInputExternal(this, "Editar Natureza",
                "Digite a Classe de Imposto", dialogSettings);
            
            
            Natureza c = new Natureza()
            {
                Descricao = desc,
                Classe_de_Imposto = classe
            };
            if (!string.IsNullOrEmpty(classe) && !string.IsNullOrEmpty(desc))
            {
                EditarNaturezaDoBanco(Naturezas[Naturezas.IndexOf(NaturezaSelecionada)], c);
                Naturezas[Naturezas.IndexOf(NaturezaSelecionada)] = c;
                DialogCoordinator.ShowModalMessageExternal(this, "Natureza", "Natureza editada com sucesso");
            }
            else if (string.IsNullOrEmpty(classe))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Classe de imposto não pode ser vazia!!");
            }
            else if (string.IsNullOrEmpty(desc))
            {
                DialogCoordinator.ShowModalMessageExternal(this, "Erro", "Descrição não pode ser vazia!!");
            }
        }

        public void ExcluirNatureza()
        {
            MessageDialogResult afirmacao = DialogCoordinator.ShowModalMessageExternal(this, "Confirmação",
                "Excluir Natureza " + NaturezaSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);

            if (afirmacao.Equals(MessageDialogResult.Affirmative))
            {
                DeletarNaturezaDoBanco(NaturezaSelecionada);
                Naturezas.Remove(NaturezaSelecionada);
                DialogCoordinator.ShowModalMessageExternal(this, "Confirmado", "Natureza removida");
            }
        }
    }


    public class AdicionarNaturezaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            NaturezaViewModel vm = (NaturezaViewModel) parameter;
            vm.AdicionarNatureza();
        }
    }


    public class EditarNaturezaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            NaturezaViewModel vm = (NaturezaViewModel) parameter;
            return !String.IsNullOrEmpty(vm.NaturezaSelecionada?.Descricao);
        }

        public override void Execute(object parameter)
        {
            NaturezaViewModel vm = (NaturezaViewModel) parameter;
            vm.EditarNatureza();
        }
    }


    public class RemoverNaturezaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            NaturezaViewModel vm = (NaturezaViewModel) parameter;
            return !String.IsNullOrEmpty(vm.NaturezaSelecionada?.Descricao);
        }


        public override void Execute(object parameter)
        {
            NaturezaViewModel vm = (NaturezaViewModel) parameter;
            vm.ExcluirNatureza();
        }
    }
}