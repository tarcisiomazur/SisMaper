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

        //alturas
        //usuario sem admin - 620 com expander / 450 sem expander
        //usuario com admin - 500 novo usuario e editar usuario sem expander / 570 com expander 

        const double notAdminWithExpanderHeight = 620;
        const double notAdminWithoutExpanderHeight = 450;

        const double adminWithoutExpanderHeight = 500;
        const double adminWithExpanderHeight = 570;

        public CrudUsuario()
        {
            DataContextChanged += SetActions;
            Loaded += CrudUsuario_Loaded;
            
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





        private void CrudUsuario_Loaded(object sender, RoutedEventArgs e)
        {
            if (!PermissoesBorder.IsEnabled)
                Height = notAdminWithoutExpanderHeight;

            else
                Height = adminWithoutExpanderHeight;
        }

        private void SenhaExpander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!SenhaExpander.IsExpanded)
            {
                if (!PermissoesBorder.IsEnabled)
                    Height = notAdminWithoutExpanderHeight;

                else
                    Height = adminWithoutExpanderHeight;
            }
            else
            {
                if (!PermissoesBorder.IsEnabled)
                    Height = notAdminWithExpanderHeight;

                else
                    Height = adminWithExpanderHeight;
            }
        }

    }
}
