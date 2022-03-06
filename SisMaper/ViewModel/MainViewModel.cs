using System;
using System.Windows;
using System.Windows.Controls;
using SisMaper.Models;
using SisMaper.Views.Templates;

namespace SisMaper.ViewModel;

public class MainViewModel : BaseViewModel
{
    private TabItem? _selectedItem;

    private TabItem? _cadastroSelecionado;

    public MainViewModel()
    {
        var permissoes = Main.Usuario.Permissao;
        PCadastro = permissoes.HasFlag(Usuario.Tipo_Permissao.Cadastros);
        PRecebimento = permissoes.HasFlag(Usuario.Tipo_Permissao.Recebimento);
        PVendas = permissoes.HasFlag(Usuario.Tipo_Permissao.Venda);
        PAdmin = permissoes.HasFlag(Usuario.Tipo_Permissao.Gerenciamento);
        PDB = permissoes.HasFlag(Usuario.Tipo_Permissao.Databaser);
    }

    #region Properties

    public bool PAdmin { get; set; }

    public bool PCadastro { get; set; }

    public bool PDB { get; set; }

    public bool PRecebimento { get; set; }

    public bool PVendas { get; set; }

    public bool NoAdmin => !PAdmin;

    public TabItem? SelectedItem
    {
        get => _selectedItem;
        set
        {

            //quando está fora da tab de cadastro e vai para o cadastro
            if(_selectedItem?.Content is not TabControl && value?.Content is TabControl tb)
            {
                if(tb.SelectedIndex != -1) CadastroSelecionado = tb.SelectedItem as TabItem;
            }

            // quando está na tab de cadastro e vai para outra tab
            if (_selectedItem?.Content is TabControl && value?.Content is not TabControl && CadastroSelecionado?.Content is MyUserControl control)
            {
                control.OnHide();
            }
            
            if (value == _selectedItem) return;
            if (_selectedItem?.Content is MyUserControl hide) hide.OnHide();
            _selectedItem = value;
            if (value?.Content is MyUserControl show) show.OnShow();


        }
    }


    public TabItem? CadastroSelecionado
    {
        get => _cadastroSelecionado;
        set
        {
            if (value == _cadastroSelecionado && value?.Content is MyUserControl control)
            {
                control.OnShow();
                return;
            }
            if (_cadastroSelecionado?.Content is MyUserControl hide) hide.OnHide();
            _cadastroSelecionado = value;
            if (value?.Content is MyUserControl show) show.OnShow();
        }
    }

    #endregion
}