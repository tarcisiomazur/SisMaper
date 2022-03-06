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

        public Action<FaturaViewModel> OpenFatura { get; set; }

        public SimpleCommand EditarFaturaCmd => new( () => OpenFatura?.Invoke(new FaturaViewModel(FaturaSelecionada.Id)), () => FaturaSelecionada != null);
        public SimpleCommand ExcluirFaturaCmd => new( ExcluirFatura, () => FaturaSelecionada != null );


        public RecebimentosViewModel()
        {
            StatusList = new List<Fatura.Fatura_Status>()
            {
                Fatura.Fatura_Status.Aberta,
                Fatura.Fatura_Status.Fechada
            };

            PropertyChanged += UpdateFilter;
        }

        public void Initialize(object? sender, EventArgs e)
        {
            Faturas = View.Execute<ListarFatura>();
        }

        public void Clear(object? sender, EventArgs e)
        {
            Faturas = null;
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


        private void ExcluirFatura()
        {
            try
            {
                var fat = DAO.Load<Fatura>(FaturaSelecionada.Id);
                fat.Delete();
                Initialize(null, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                OnShowMessage("Erro", ex.Message + "  inner: " + ex.InnerException);
            }
        }




    }
}
