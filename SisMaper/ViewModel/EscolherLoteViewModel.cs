using System;
using Persistence;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    public class EscolherLoteViewModel: BaseViewModel
    {
        public PList<Lote> Lotes { get; set; }
        public Lote? LoteSelecionado { get; set; }

        public event Action? OnOK;
        public event Action? OnCancel;
        
        public BaseCommand OKCommand => new SimpleCommand(OK, _ => LoteSelecionado is not null);
        
        public BaseCommand CancelarCommand => new SimpleCommand(Cancel);
        
        public EscolherLoteViewModel()
        {
            
        }

        public void Initialize(PList<Lote> lotes)
        {
            Lotes = lotes;
        }
        
        private void OK()
        {
            OnOK?.Invoke();
        }
        private void Cancel()
        {
            OnCancel?.Invoke();
        }
    }
}