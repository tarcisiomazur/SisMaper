using MahApps.Metro.Controls;
using Persistence;
using SisMaper.Models;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    public partial class ViewEscolherLote : MetroWindow
    {
        public EscolherLoteViewModel ViewModel => (EscolherLoteViewModel) DataContext;
        public ViewEscolherLote(PList<Lote> lotes)
        {
            InitializeComponent();
            SetActions();
            ViewModel.Initialize(lotes);
        }

        private void SetActions()
        {
            ViewModel.OnOK += () =>
            {
                DialogResult = true;
                Close();
            };
            ViewModel.OnCancel += () =>
            {
                DialogResult = false;
                Close();
            };
            
        }
    }
}