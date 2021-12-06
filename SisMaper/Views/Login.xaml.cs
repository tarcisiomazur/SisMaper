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
        public LoginViewModel ViewModel => (LoginViewModel) DataContext;
        public Login()
        {
            InitializeComponent();
            SetActions();
        }

        private void SetActions()
        {
            ViewModel.Login += () =>
            {
                DialogResult = true;
                Close();
            };
            ViewModel.Cancel += Close;
        }
    }
}