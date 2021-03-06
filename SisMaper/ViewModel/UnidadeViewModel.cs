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
        public List<Unidade> Unidades { get; private set; }

        public SimpleCommand AdicionarUnidadeCmd => new(() => AdicionarUnidade());
        public SimpleCommand EditarUnidadeCmd => new(EditarUnidade, () => !string.IsNullOrEmpty(UnidadeSelecionada?.Descricao));
        public SimpleCommand RemoverUnidadeCmd => new(ExcluirUnidade, () => !string.IsNullOrEmpty(UnidadeSelecionada?.Descricao));


        private Unidade _unidadeSelecionada;

        public Unidade UnidadeSelecionada
        {
            get { return _unidadeSelecionada; }
            set { SetField(ref _unidadeSelecionada, value); }
        }

        
        public UnidadeViewModel()
        {
            Unidades = DAO.All<Unidade>().ToList();
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
                OnShowMessage("Erro", "Erro ao salvar unidade: " + ex.Message, MessageDialogStyle.Affirmative);
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
                    
                    if ((ex as MySqlConnector.MySqlConnectorException).ErrorCode == -2147467259)
                    {
                        OnShowMessage("Erro", String.Format("Unidade {0} está vinculada a algum produto, não pode ser excluida", u.Descricao), MessageDialogStyle.Affirmative);
                        return false;
                    }

                    OnShowMessage("Erro", String.Format("Erro ao deletar unidade {0}: " + ex.Message, u.Descricao), MessageDialogStyle.Affirmative);
                    return false;
                }

                OnShowMessage("Erro", String.Format("Erro ao deletar unidade {0}: " + ex.Message, u.Descricao), MessageDialogStyle.Affirmative);
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
                OnShowMessage("Erro", "Erro ao editar unidade: " + ex.Message, MessageDialogStyle.Affirmative);
                return false;
            }
        }



        public void AdicionarUnidade(string defaultText = "")
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = defaultText
            };

            string? uni = OnInput("Adicionar Unidade", "Unidade", dialogSettings);


            foreach (Unidade element in Unidades)
            {
                if (element.Descricao.Equals(uni))
                {
                    OnShowMessage("Erro", "Unidade ja existente");
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
                    RaisePropertyChanged(nameof(Unidades));
                    OnShowMessage("Unidade", "Unidade adicionada com sucesso");
                }
                return;
            }

            else if (string.Equals(uni, string.Empty) || (string.IsNullOrWhiteSpace(uni) && !string.Equals(uni, null)))
            {
                OnShowMessage("Erro", "Unidade não pode ser vazia!!");
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

            string? uni = OnInput("Editar Unidade", "Unidade", dialogSettings);

            foreach (Unidade element in Unidades)
            {
                if (element.Descricao.Equals(uni) && element.Id != UnidadeSelecionada.Id)
                {
                    OnShowMessage("Erro", "Unidade ja existente");
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
                    RaisePropertyChanged(nameof(Unidades));
                    OnShowMessage("Unidade", "Unidade editada com sucesso");
                }
                return;
            }

            else if (string.Equals(uni, string.Empty) || (string.IsNullOrWhiteSpace(uni) && !string.Equals(uni, null)))
            {
                OnShowMessage("Erro", "Unidade não pode ser vazia!!");
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
            MessageDialogResult afirmacao = OnShowMessage("Confirmação", "Excluir unidade " + UnidadeSelecionada.Descricao, MessageDialogStyle.AffirmativeAndNegative);

            if (afirmacao.Equals(MessageDialogResult.Affirmative))
            {
                if (DeletarUnidadeDoBanco(UnidadeSelecionada))
                {
                    Unidades.Remove(UnidadeSelecionada);
                    RaisePropertyChanged(nameof(Unidades));
                    OnShowMessage("Confirmado", "Unidade removida");
                }
            }

        }
    }

}
