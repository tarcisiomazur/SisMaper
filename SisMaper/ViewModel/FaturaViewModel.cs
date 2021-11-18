using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SisMaper.Models;

namespace SisMaper.ViewModel
{
    
    public class FaturaViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public ObservableCollection<Parcela> ListParcelas { get; set; }

        public FaturaViewModel()
        {
            ListParcelas = new ObservableCollection<Parcela>();
        }
        
    }
}