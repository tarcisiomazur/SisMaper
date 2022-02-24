using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SisMaper.Views;

public partial class ViewHelpBrowser : INotifyPropertyChanged
{
    private string _URL = "";

    public ViewHelpBrowser()
    {
        InitializeComponent();
    }

    public string URL
    {
        get => _URL;
        set
        {
            _URL = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}