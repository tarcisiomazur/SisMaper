using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using SisMaper.M_P;
using SisMaper.Models;
using SisMaper.Tools;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {

        public Login()
        {
            InitializeComponent();

            (DataContext as LoginViewModel)!.OnLogin += () =>
            {
                DialogResult = true;
                Close();
            };
        }

        private void ArrastarTela(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ShowPassword(object sender, RoutedEventArgs routedEventArgs)
        {
            pb_senha.IsShow = true;
        }

        private void HidePassword(object sender, RoutedEventArgs routedEventArgs)
        {
            pb_senha.IsShow = false;
        }

        private void Sair(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}