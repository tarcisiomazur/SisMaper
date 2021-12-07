using System.Windows;
using MahApps.Metro.Controls;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para ViewCategorias.xaml
    /// </summary>
    public partial class ViewNaturezas : MetroWindow
    {
        public ViewNaturezas()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.IsChanged(out NaturezaViewModel viewModel))
            {
                viewModel.ShowMessage += Helper.MahAppsDefaultMessage;
                viewModel.ShowInput += Helper.MahAppsDefaultInput;
            }
        }
    }
}