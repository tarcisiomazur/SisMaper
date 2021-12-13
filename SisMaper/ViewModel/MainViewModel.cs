using System.Windows.Controls;
using SisMaper.Models;
using SisMaper.Views.Templates;

namespace SisMaper.ViewModel;

public class MainViewModel : BaseViewModel
{
    private TabItem? _selectedItem;

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

    public TabItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value == _selectedItem) return;
            if (_selectedItem?.Content is MyUserControl hide) hide.OnHide();
            _selectedItem = value;
            if (value?.Content is MyUserControl show) show.OnShow();
        }
    }

    #endregion
}