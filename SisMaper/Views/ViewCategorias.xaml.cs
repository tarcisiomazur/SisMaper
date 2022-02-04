using MahApps.Metro.Controls;
using SisMaper.Tools;
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
using System.Windows.Shapes;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para ViewCategorias.xaml
    /// </summary>
    public partial class ViewCategorias : MetroWindow
    {
        public ViewCategorias()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CategoriaViewModel newViewModel)
            {
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
                newViewModel.ShowInput += Helper.MahAppsDefaultInput;
            }

            if(e.OldValue is CategoriaViewModel oldViewModel)
            {
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
                oldViewModel.ShowInput -= Helper.MahAppsDefaultInput;
            }
        }
    }
}
