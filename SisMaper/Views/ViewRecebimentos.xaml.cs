using SisMaper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SisMaper.Views
{
    /// <summary>
    /// Interação lógica para ViewRecebimentos.xam
    /// </summary>
    public partial class ViewRecebimentos : UserControl
    {
        public ViewRecebimentos()
        {
            InitializeComponent();
            SetActions();
        }


        private void SetActions()
        {
            if(DataContext is IFaturas vm)
            {
                vm.OpenNovaFatura += () =>
                {
                    new ViewFatura() { DataContext = new FaturaViewModel(null) }.ShowDialog();
                    DataContext = new RecebimentosViewModel();
                    SetActions();
                };

                vm.OpenEditarFatura += () =>
                {
                    new ViewFatura() { DataContext = new FaturaViewModel(FaturasDataGrid.SelectedItem) }.ShowDialog();
                    DataContext = new RecebimentosViewModel();
                    SetActions();
                };

                vm.FaturaExcluida += () =>
                {
                    DataContext = new RecebimentosViewModel();
                    SetActions();
                };
            }
        }
    }
}
