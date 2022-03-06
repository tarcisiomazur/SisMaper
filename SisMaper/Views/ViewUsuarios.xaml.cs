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
    /// Interação lógica para ViewUsuarios.xam
    /// </summary>
    public partial class ViewUsuarios : MyUserControl
    {
        public ViewUsuarios()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is UsuariosViewModel newViewModel)
            {
                Show += newViewModel.Initialize;
                Hide += newViewModel.Clear;
                newViewModel.OpenCrudUsuario += OpenCrudUsuario;
            }

            if (e.OldValue is UsuariosViewModel oldViewModel)
            {
                Show -= oldViewModel.Initialize;
                Hide -= oldViewModel.Clear;
                oldViewModel.OpenCrudUsuario -= OpenCrudUsuario;
            }
        }


        private void OpenCrudUsuario(UsuariosViewModel.CrudUsuarioViewModel viewModel)
        {
            new CrudUsuario { DataContext = viewModel, Owner = Window}.ShowDialog();
            OnShow();
        }
    }
}
