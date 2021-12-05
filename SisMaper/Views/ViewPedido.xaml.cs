using System;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using SisMaper.ViewModel;

namespace SisMaper.Views
{
    /// <summary>
    /// Lógica interna para ViewPedido.xaml
    /// </summary>
    public partial class ViewPedido : MetroWindow
    {
        public PedidoViewModel ViewModel => (PedidoViewModel) DataContext;

        public ViewPedido(long? pedidoId)
        {
            DataContext = new PedidoViewModel(pedidoId);
            InitializeComponent();
            SetActions();
        }

        private void SetActions()
        {
            ViewModel.Save += Close;
            ViewModel.Cancel += Close;
        }
        
        private void SetZoom(object? sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            if(sender is ChromiumWebBrowser webBrowser)
                webBrowser.SetZoomLevel(2);
        }
        
    }
}