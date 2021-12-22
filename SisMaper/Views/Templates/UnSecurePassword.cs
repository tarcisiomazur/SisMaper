using System.Windows;
using System.Windows.Controls;

namespace SisMaper.Views.Templates;

public static class UnSecurePassword
{
    public static readonly DependencyProperty UnSecureProperty =
        DependencyProperty.RegisterAttached("UnSecure",
            typeof(bool),
            typeof(PasswordBox),
            new FrameworkPropertyMetadata(false, UnSecurePropertyChanged));

    public static readonly DependencyProperty UnSecurePasswordProperty =
        DependencyProperty.RegisterAttached("UnSecurePassword",
            typeof(string),
            typeof(UnSecurePassword),
            new FrameworkPropertyMetadata(null, UnSecurePasswordPropertyChanged));

    private static void UnSecurePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox pb)
        {
            if ((bool) e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }
    }

    public static void SetUnSecure(DependencyObject obj, bool value)
    {
        obj.SetValue(UnSecureProperty, value);
    }

    public static bool GetUnSecure(DependencyObject obj)
    {
        return (bool) obj.GetValue(UnSecureProperty);
    }

    private static void UnSecurePasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox pb && !pb.Password.Equals(e.NewValue))
        {
            pb.Password = (string) e.NewValue;
        }
    }

    public static void SetUnSecurePassword(DependencyObject obj, string value)
    {
        obj.SetValue(UnSecurePasswordProperty, value);
    }

    public static string GetUnSecurePassword(DependencyObject obj)
    {
        return (string) obj.GetValue(UnSecurePasswordProperty);
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
        {
            SetUnSecurePassword(pb, pb.Password);
        }
    }
}