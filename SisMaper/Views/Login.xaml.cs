﻿using System.Windows;
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
        private string[] themes = new[]
        {
            "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet",
            "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna"
        };

        private int i = 0;
        
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\SisMaper");
        public Usuario? Usuario { get; set; }
        public Login()
        {
            InitializeComponent();
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
        
        private void ArrastarTela(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Close_Login(object sender, MouseButtonEventArgs e)
        {
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
                DialogResult = true;
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
