using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class NaturezaViewModel : BaseViewModel
    {
        #region Properties

        public IList<Natureza> Naturezas { get; private set; }

        #endregion

        #region UIProperties

        public Natureza? NaturezaSelecionada { get; set; }

        #endregion

        #region ICommands

        public SimpleCommand AdicionarCmd => new(AdicionarNatureza);
        public SimpleCommand EditarCmd => new(EditarNatureza, _ => NaturezaSelecionada is not null);
        public SimpleCommand RemoverCmd => new(ExcluirNatureza, _ => NaturezaSelecionada is not null);

        #endregion

        public NaturezaViewModel()
        {
            Naturezas = DAO.All<Natureza>();
            NaturezaSelecionada = Naturezas.FirstOrDefault();
        }

        private void AdicionarNatureza()
        {
            string desc = OnInput("Editar Natureza", "Digite a Descrição");
            if (desc is null) return;
            string classe = OnInput("Editar Natureza", "Digite a Classe de Imposto");
            if (classe is null) return;

            if (Naturezas.Any(n => n.Descricao.Equals(desc)))
            {
                OnShowMessage("Erro", "Natureza ja existente");
                return;
            }

            Natureza c = new Natureza()
            {
                Descricao = desc,
                Classe_de_Imposto = classe
            };
            switch (c)
            {
                case {Classe_de_Imposto: ""}:
                    OnShowMessage("Erro", "Classe de imposto não pode ser vazia!!");
                    return;
                case {Descricao: ""}:
                    OnShowMessage("Erro", "Natureza não pode ser vazia!!");
                    return;
            }

            Naturezas.Add(c);
            if (c.Save())
                OnShowMessage("Natureza", "Natureza adicionada com sucesso");
            else
                OnShowMessage("Erro", "Natureza não pode ser adicionada!!");
        }

        public void EditarNatureza()
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = NaturezaSelecionada.Descricao
            };

            var desc = OnInput("Editar Natureza", "Digite a Descrição", dialogSettings);
            if (desc is null) return;
            dialogSettings.DefaultText = NaturezaSelecionada.Classe_de_Imposto;
            var classe = OnInput("Editar Natureza", "Digite a Classe de Imposto", dialogSettings);
            if (classe is null) return;

            switch (new {Classe_de_Imposto = classe, Descricao = desc})
            {
                case {Classe_de_Imposto: ""}:
                    OnShowMessage("Erro", "Classe de imposto não pode ser vazia!!");
                    return;
                case {Descricao: ""}:
                    OnShowMessage("Erro", "Natureza não pode ser vazia!!");
                    return;
            }

            NaturezaSelecionada.Descricao = desc;
            NaturezaSelecionada.Classe_de_Imposto = classe;
            if (NaturezaSelecionada.Save())
                OnShowMessage("Natureza", "Natureza salva com sucesso");
            else
                OnShowMessage("Erro", "Natureza não pode ser editada!!");
        }

        public void ExcluirNatureza()
        {
            MessageDialogResult afirmacao = OnShowMessage("Confirmação",
                "Excluir Natureza " + NaturezaSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);

            if (!afirmacao.Equals(MessageDialogResult.Affirmative)) return;
            if (NaturezaSelecionada.Delete())
            {
                Naturezas.Remove(NaturezaSelecionada);
                OnShowMessage("Confirmado", "Natureza removida");
            }
            else
            {
                OnShowMessage("Erro", "Natureza não pode ser removida!!");
            }
        }
    }
}