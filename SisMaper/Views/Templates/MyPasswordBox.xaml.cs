using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SisMaper.Views.Templates;

public partial class MyPasswordBox : UserControl
{
    public static readonly DependencyProperty IsShowProperty =
        DependencyProperty.RegisterAttached("IsShow",
            typeof(bool),
            typeof(MyPasswordBox), new FrameworkPropertyMetadata(false, IsShowPropertyChanged));

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password",
            typeof(string),
            typeof(MyPasswordBox), new FrameworkPropertyMetadata("", PasswordPropertyChanged));

    public static readonly DependencyProperty MaxLengthProperty =
        DependencyProperty.RegisterAttached("MaxLength",
            typeof(int),
            typeof(MyPasswordBox));

    public static readonly DependencyProperty ContentPaddingProperty =
        DependencyProperty.RegisterAttached("ContentPadding",
            typeof(Thickness),
            typeof(MyPasswordBox));

    private bool _isShow;

    public MyPasswordBox()
    {
        InitializeComponent();
        _passwordBox.Password = Password;
        _passwordBox.PasswordChanged += ChangePassword;
        _passwordBox.Visibility = _isShow ? Visibility.Collapsed : Visibility.Visible;

        //_textBox.Text = Password;
        //_textBox.TextChanged += ChangeText;
        _textBox.Visibility = _isShow ? Visibility.Visible : Visibility.Collapsed;

        LostFocus += (_, _) => ToolTipCapsLock.IsOpen = false;
        GotFocus += (_, _) =>
        {
            if (Keyboard.IsKeyToggled(Key.CapsLock)) ToolTipCapsLock.IsOpen = true;
        };
        KeyDown += (s, e) =>
        {
            if (e.Key == Key.CapsLock) ToolTipCapsLock.IsOpen = Keyboard.IsKeyToggled(Key.CapsLock);
        };
    }

    public TextBox TextBox => _textBox;

    public PasswordBox PasswordBox => _passwordBox;

    public bool IsShow
    {
        get => _isShow;
        set => SetValue(IsShowProperty, value);
    }

    public string Password
    {
        get => (string) GetValue(PasswordProperty);
        set => ChangePassword(value);
    }

    [Bindable(true)]
    public int MaxLength
    {
        get => (int) GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    [Bindable(true)] public Thickness ContentPadding { get; set; }

    private static void IsShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = (MyPasswordBox) d;
        if (e.OldValue != e.NewValue && e.NewValue is bool b)
        {
            passwordBox.ChangeShow(b);
        }
    }

    private void ChangeShow(bool value)
    {
        _textBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        _passwordBox.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
        if (value == _isShow)
        {
            return;
        }

        _isShow = value;
        if (_isShow)
        {
            _textBox.Text = Password;
        }
        else
        {
            _passwordBox.Password = Password;
        }
    }

    private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = (MyPasswordBox) d;
        if (e.OldValue != e.NewValue && e.NewValue is not null)
        {
            passwordBox.UpdateViews((string) e.NewValue);
        }
    }

    public new bool Focus()
    {
        return _isShow && _textBox.Focus() || !_isShow && _passwordBox.Focus();
    }

    private void ChangePassword(object sender, RoutedEventArgs e)
    {
        Password = _passwordBox.Password;
    }

    private void UpdateViews(string newPassword)
    {
        if (_passwordBox.Password != newPassword)
        {
            _passwordBox.Password = newPassword;
        }

        if (_textBox.Text != newPassword)
        {
            _textBox.Text = newPassword;
        }
    }

    private void ChangePassword(string password)
    {
        SetValue(PasswordProperty, password);
        if (_passwordBox.Password != password)
        {
            _passwordBox.Password = password;
        }
    }
}