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
    /// Lógica interna para ViewParcela.xaml
    /// </summary>
    public partial class ViewParcela : MetroWindow
    {
        public ViewParcela()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
            Loaded += ViewParcelaLoaded;
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is ParcelaViewModel newViewModel)
            {
                newViewModel.ParcelaSaved += Close;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }
            if(e.OldValue is ParcelaViewModel oldViewModel)
            {
                oldViewModel.ParcelaSaved -= Close;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }


        private void ViewParcelaLoaded(object sender, RoutedEventArgs e)
        {
            DataVencimentoDatePicker.DisplayDateStart = DateTime.Today;
        }

        private void CancelarButtonPress(object sender, MouseButtonEventArgs e)
        {
            Close();
        }




    }
}
