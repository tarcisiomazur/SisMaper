using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SisMaper.Models;
using SisMaper.Views.Templates;

namespace SisMaper.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        public MainViewModel()
        {
            
        }

        private TabItem? _selectedItem;
        
        public TabItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == _selectedItem) return;
                if(value?.Content is MyUserControl open) open.OnOpen?.Invoke(this, EventArgs.Empty);
                if(_selectedItem?.Content is MyUserControl close) close.OnClose?.Invoke(this, EventArgs.Empty);
                _selectedItem = value;
            }
        }
    }


    /* mainView
     * 
         <mah:MetroWindow.DataContext>
        <vm:ProdutosViewModel/>
    </mah:MetroWindow.DataContext>
     */
}
