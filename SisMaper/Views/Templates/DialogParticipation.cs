using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace SisMaper.Views.Templates;

public static class DialogParticipation
{
    public static readonly DependencyProperty MyRegisterProperty = DependencyProperty.RegisterAttached(
        "MyRegister",
        typeof(object),
        typeof(DialogParticipation),
        new PropertyMetadata(default, RegisterPropertyChangedCallback));

    private static void RegisterPropertyChangedCallback(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        switch (dependencyObject)
        {
            case MetroWindow metroWindow:
                metroWindow.Closed += UnRegister;
                break;
            case MyUserControl control:
                control.Unloaded += UnRegister;
                control.Hide += UnRegister;
                control.Show += ReRegister;
                break;
        }

        if (dependencyPropertyChangedEventArgs.NewValue != null)
        {
            MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(dependencyObject,
                dependencyPropertyChangedEventArgs.NewValue);
        }
    }

    private static void UnRegister(object? sender, EventArgs eventArgs)
    {
        if (sender is Control control)
        {
            MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(control, null);
        }
    }

    private static void ReRegister(object? sender, EventArgs eventArgs)
    {
        if (sender is Control control)
        {
            MahApps.Metro.Controls.Dialogs.DialogParticipation.SetRegister(control, GetMyRegister(control));
        }
    }

    public static void SetMyRegister(DependencyObject element, object context)
    {
        element.SetValue(MyRegisterProperty, context);
    }

    public static object GetMyRegister(DependencyObject element)
    {
        return element.GetValue(MyRegisterProperty);
    }
}