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
    /// Lógica interna para CrudUsuario.xaml
    /// </summary>
    public partial class CrudUsuario
    {
        public CrudUsuario()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is UsuariosViewModel.CrudUsuarioViewModel newViewModel)
            {
                newViewModel.UsuarioSaved += Close;
                newViewModel.ShowMessage += Helper.MahAppsDefaultMessage;
            }

            if (e.OldValue is UsuariosViewModel.CrudUsuarioViewModel oldViewModel)
            {
                oldViewModel.UsuarioSaved -= Close;
                oldViewModel.ShowMessage -= Helper.MahAppsDefaultMessage;
            }
        }

        private void CancelarButtonPressed(object sender, MouseButtonEventArgs e) => Close();

        private void PasswordBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Tab) e.Handled = true;
        }
    }
}
