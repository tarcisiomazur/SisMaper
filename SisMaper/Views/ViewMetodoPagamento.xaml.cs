using System;
using System.Windows;
using System.Windows.Controls;

namespace SisMaper.Views
{
    public partial class ViewMetodoPagamento
    {
        public ViewMetodoPagamento()
        {
            InitializeComponent();
        }
        
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            try
            {
                Close();
            }
            catch
            {
                
            }
        }
        public OptionPagamento Selecionado { get; set; } 
        public enum OptionPagamento
        {
            Null,
            AVista,
            NovaFatura,
            FaturaExistente,
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button)
            {
                Selecionado = button.Name switch
                {
                    "AVista" => OptionPagamento.AVista,
                    "NovaFatura" => OptionPagamento.NovaFatura,
                    "FaturaExistente" => OptionPagamento.FaturaExistente,
                    _ => OptionPagamento.Null
                };
            }
            Close();
        }
    }
}