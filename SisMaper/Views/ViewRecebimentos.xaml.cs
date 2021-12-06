using SisMaper.ViewModel;
using SisMaper.Views.Templates;
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
    public partial class ViewRecebimentos : MyUserControl
    {
        private RecebimentosViewModel viewModel => (RecebimentosViewModel) DataContext;

        public ViewRecebimentos()
        {
            
            InitializeComponent();
            Show += viewModel.Initialize;
            Hide += viewModel.Clear;
            SetActions();
        }


        private void SetActions()
        {
            if(DataContext is IRecebimento vm)
            {
                vm.OpenEditarFatura += (object? fat) =>
                {
                    ViewFatura viewFatura = new ViewFatura() { DataContext = new FaturaViewModel(fat) };
                    viewFatura.Closed += viewModel.Initialize;
                    viewFatura.ShowDialog();
                };

                /*
                vm.FaturaExcluida += () =>
                {
                    DataContext = new RecebimentosViewModel();
                };
                */
            }
        }
    }
}
