using System.Windows;

namespace SisMaper.Views.Templates;

public static class FocusExtension
{
    public static readonly DependencyProperty IsFocusedProperty =
        DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(FocusExtension),
            new FrameworkPropertyMetadata(false, OnIsFocusedPropertyChanged));

    public static bool GetIsFocused(DependencyObject obj)
    {
        return (bool) obj.GetValue(IsFocusedProperty);
    }

    public static void SetIsFocused(DependencyObject obj, bool value)
    {
        obj.SetValue(IsFocusedProperty, value);
    }

    private static void OnIsFocusedPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not true) return;
        switch (d)
        {
            case MyPasswordBox mpb:
                mpb.Focus();
                break;
            case UIElement uie:
                uie.Focus();
                break;
        }
    }
}