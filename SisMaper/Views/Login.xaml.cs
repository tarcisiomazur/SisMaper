using System.Windows;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        public Login()
        {
            DataContextChanged += SetActions;
            InitializeComponent();
        }

        private void SetActions(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.IsChanged(out LoginViewModel viewModel))
            {
                viewModel.Login += () =>
                {
                    (Application.Current.MainWindow = new MainWindow()).Show();
                    Close();
                };
                viewModel.Cancel += Close;
                viewModel.ShowMessage += Helper.SystemDefaultMessage;
            }
        }
    }
}