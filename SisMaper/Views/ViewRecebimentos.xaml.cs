using SisMaper.Models.Views;
using SisMaper.Tools;
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
    /// Interação lógica para ViewRecebimentos.xaml
    /// </summary>
    public partial class ViewRecebimentos : MyUserControl
    {
        private RecebimentosViewModel viewModel => (RecebimentosViewModel) DataContext;

        public ViewRecebimentos()
        {
            DataContextChanged += SetActions;
            InitializeComponent();

        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is RecebimentosViewModel newViewModel)
            {
                Show += newViewModel.Initialize;
                Hide += newViewModel.Clear;
                newViewModel.OpenFatura += OpenFatura;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }
            if(e.OldValue is RecebimentosViewModel oldViewModel)
            {
                Show -= oldViewModel.Initialize;
                Hide -= oldViewModel.Clear;
                oldViewModel.OpenFatura -= OpenFatura;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }

        private void OpenFatura(FaturaViewModel viewModel)
        {
            new ViewFatura { DataContext = viewModel, Owner = Window }.ShowDialog();
            OnShow();
        }

    }
}
