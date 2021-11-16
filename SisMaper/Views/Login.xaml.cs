using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SisMaper.M_P;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login: MetroWindow
    {
        readonly RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SisMaper");
        public Usuario? Usuario { get; set; }

        public bool CanCloseApplication { get; set; }

        public Login()
        {
            
            InitializeComponent();

            CanCloseApplication = true;

            Closing += CloseApplication;

            var username = key.GetValue("LastUsername");
            if (username is string user)
            {
                tb_usuario.Text = user;
                pb_senha.Focus();
            }
            else
            {
                tb_usuario.Focus();
            }
            
        }

        private void CloseApplication(object sender, CancelEventArgs e)
        {
            if (CanCloseApplication) Environment.Exit(0);
        }

        private void ArrastarTela(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Login(object sender, MouseButtonEventArgs e)
        {
            CanCloseApplication = true;

            if (btn_cancelar.IsPressed)
                Close();
        }

        

        private void Confirm_Login(object sender, MouseButtonEventArgs e)
        {
            if (btn_login.IsPressed)
                Confirm_Login();
        }

        private void Confirm_Login(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirm_Login();
        }

        private void Confirm_Login()
        {
            Usuario = AuthLogin.Login(new Usuario()
            {
                Login = tb_usuario.Text,
                Senha = Encrypt.ToSha512(pb_senha.Password) ?? ""
            });
            
            if (Usuario?.Permissao > 0)
            {
                key.SetValue("LastUsername", tb_usuario.Text);
                //DialogResult = true;
                CanCloseApplication = false;
                Close();
            }
            else
            {
                MessageBox.Show("Usuário ou senha incorreto","Tentativa de Acesso", MessageBoxButton.OK);
            }
        }

        private void ShowPassword(object sender, RoutedEventArgs routedEventArgs)
        {
            pb_senha.IsShow = true;
        }

        private void HidePassword(object sender, RoutedEventArgs routedEventArgs)
        {
            pb_senha.IsShow = false;
        }

    }
}
