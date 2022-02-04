using MahApps.Metro.Controls.Dialogs;
using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SisMaper.Models.Views;
using SisMaper.Tools;

namespace SisMaper.ViewModel
{
    public class RecebimentosViewModel : BaseViewModel
    {
        public List<ListarFatura>? Faturas { get; set; }
        public IEnumerable<ListarFatura>? FaturasFiltradas { get; set; }
        public ListarFatura? FaturaSelecionada { get; set; }
        public string TextoFiltro { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Fatura.Fatura_Status? StatusSelecionado { get; set; }
        public List<Fatura.Fatura_Status> StatusList { get; set; }

        public EditarFaturaCommand EditarFatura { get; private set; }
        public ExcluirFaturaCommand DeletarFatura { get; private set; }

        public Action<FaturaViewModel> OpenFatura { get; set; }

        private PersistenceContext persistenceContext;


        public RecebimentosViewModel()
        {
            //Faturas = DAO.FindWhereQuery<Fatura>("Id > 0");
            /*
            foreach(Fatura f in Faturas)
            {
                f?.Cliente?.Load();
            }
            */

            //GetFaturas();
            persistenceContext = new PersistenceContext();
            StatusList = new List<Fatura.Fatura_Status>()
            {
                Fatura.Fatura_Status.Aberta,
                Fatura.Fatura_Status.Fechada
            };

            EditarFatura = new EditarFaturaCommand();
            DeletarFatura = new ExcluirFaturaCommand();
            PropertyChanged += UpdateFilter;
        }

        private void UpdateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Faturas != null && e.PropertyName is nameof(StatusSelecionado) or nameof(Faturas) or nameof(TextoFiltro)
            or nameof(StartDate) or nameof(EndDate))
            {
                FaturasFiltradas = Faturas.Where(p =>
                    (string.IsNullOrEmpty(TextoFiltro) || !string.IsNullOrEmpty(p.Nome) &&
                     p.Nome.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase) ||
                     p.Id.ToString().Equals(TextoFiltro)) &&
                    (StatusSelecionado == null || p.Status == StatusSelecionado) &&
                    (StartDate == null || p.Data.Date >= StartDate) && 
                    (EndDate == null || p.Data.Date <= EndDate)
                );
            }
        }

        public void OpenCrudEditarFatura() => OpenFatura?.Invoke(new FaturaViewModel(FaturaSelecionada.Id));


        public void ExcluirFatura()
        {
            try
            {
                var fat = DAO.Load<Fatura>(FaturaSelecionada.Id);
                fat.Delete();
                Initialize(null, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                OnShowMessage("Erro", ex.Message);
            }
        }


        public void Initialize(object? sender, EventArgs e)
        {
            Faturas = View.Execute<ListarFatura>();
        }

        public void Clear(object? sender, EventArgs e)
        {
            Faturas = null;
        }

    }




    public class EditarFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            return vm.FaturaSelecionada is not null;
            //return true;
        }
        public override void Execute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            vm.OpenCrudEditarFatura();
        }
    }

    public class ExcluirFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            return vm.FaturaSelecionada is not null;
        }
        public override void Execute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            vm.ExcluirFatura();
        }
    }

}
