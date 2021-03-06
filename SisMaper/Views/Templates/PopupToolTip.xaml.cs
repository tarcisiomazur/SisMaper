using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace SisMaper.Views.Templates;

public static class HelperPopupToolTip
{
    public static readonly DependencyProperty PopupToolTipProperty =
        DependencyProperty.RegisterAttached("PopupToolTip",
            typeof(PopupToolTip),
            typeof(HelperPopupToolTip),
            new FrameworkPropertyMetadata(null));

    public static string GetHelpHeader(DependencyObject element)
    {
        return (element.GetValue(PopupToolTipProperty) as PopupToolTip)?.HelpHeader;
    }

    public static void SetHelpHeader(DependencyObject element, string value)
    {
        var popup = element.GetValue(PopupToolTipProperty) as PopupToolTip;
        if (popup == null)
        {
            popup = CreateToolTip(element);
            element.SetValue(PopupToolTipProperty, popup);
        }

        popup.HelpHeader = value;
    }

    public static string GetHelpContent(DependencyObject element)
    {
        return (element.GetValue(PopupToolTipProperty) as PopupToolTip)?.HelpContent;
    }

    public static void SetHelpContent(DependencyObject element, string value)
    {
        var popup = element.GetValue(PopupToolTipProperty) as PopupToolTip;
        if (popup == null)
        {
            popup = CreateToolTip(element);
            element.SetValue(PopupToolTipProperty, popup);
        }

        popup.HelpContent = value;
    }

    public static string GetHelpLink(DependencyObject element)
    {
        return (element.GetValue(PopupToolTipProperty) as PopupToolTip)?.HelpLink;
    }

    public static void SetHelpLink(DependencyObject element, string value)
    {
        var popup = element.GetValue(PopupToolTipProperty) as PopupToolTip;
        if (popup == null)
        {
            popup = CreateToolTip(element);
            element.SetValue(PopupToolTipProperty, popup);
        }

        popup.HelpLink = value;
    }

    private static PopupToolTip CreateToolTip(DependencyObject d)
    {
        var dp = d;

        if (d is not FrameworkElement fe)
        {
            throw new NotSupportedException($"HelpNotSuported {d.GetType()}");
        }
        
        var newHelpToolTip = new PopupToolTip(fe);

        while (dp is not Panel && dp.GetParentObject() is { } next)
            dp = next;

        if (dp is not Panel p)
        {
            fe.Loaded += AttachOnInitialize;
            return newHelpToolTip;
        }
        

        p.Children.Add(newHelpToolTip);
        return newHelpToolTip;
    }

    private static void AttachOnInitialize(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is FrameworkElement fe) fe.Loaded -= AttachOnInitialize;
        if (sender is not DependencyObject dp) return;
        if (dp.GetValue(PopupToolTipProperty) is not PopupToolTip toolTip) return;
        while (true)
        {
            var next = dp.GetParentObject() ?? VisualTreeHelper.GetParent(dp);

            switch (next)
            {
                case null:
                    throw new NotSupportedException($"HelpNotSuported {sender.GetType()}");
                case Grid p:
                    p.Children.Add(toolTip);
                    return;
                default:
                    dp = next;
                    break;
            }
        }
    }
}

public partial class PopupToolTip : Popup
{
    public static readonly DependencyProperty HelpHeaderProperty =
        DependencyProperty.RegisterAttached("HelpHeader",
            typeof(string),
            typeof(PopupToolTip),
            new FrameworkPropertyMetadata(null, HelpHeaderPropertyChangedCallback));

    public static readonly DependencyProperty HelpContentProperty =
        DependencyProperty.RegisterAttached("HelpContent",
            typeof(string),
            typeof(PopupToolTip),
            new FrameworkPropertyMetadata(null, HelpContentPropertyChangedCallback));

    public static readonly DependencyProperty HelpLinkProperty =
        DependencyProperty.RegisterAttached("HelpLink",
            typeof(string),
            typeof(PopupToolTip),
            new FrameworkPropertyMetadata(null, HelpLinkPropertyChangedCallback));

    public static readonly DependencyProperty IsOpeningProperty =
        DependencyProperty.RegisterAttached("IsOpening",
            typeof(bool),
            typeof(PopupToolTip),
            new FrameworkPropertyMetadata(false, ChangeIsOpen));

    public static readonly DependencyProperty ResetOpeningProperty =
        DependencyProperty.RegisterAttached("ResetOpening",
            typeof(bool),
            typeof(PopupToolTip),
            new FrameworkPropertyMetadata(false));

    private static void ChangeIsOpen(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PopupToolTip popup) return;
        if (e.NewValue is bool b)
        {
            popup.IsOpen = b;
        }
    }

    public PopupToolTip(FrameworkElement fe)
    {
        PlacementTarget = fe;
        fe.MouseMove += ResetOpeningAction;
        fe.PreviewMouseDown += (_, _) => CloseToolTip();
        fe.KeyDown += (_, _) => CloseToolTip();
        fe.GotFocus += (_, _) => CloseToolTip();
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void CloseToolTip()
    {
        SetValue(IsOpeningProperty, false);
        IsOpen = false;
    }

    private void ResetOpeningAction(object sender, MouseEventArgs e)
    {
        if (!IsOpen)
        {
            ResetOpening = true;
            ResetOpening = false;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        if (parentWindow == null) return;
        parentWindow.Deactivated += OnDeactivated;
    }

    private void OnDeactivated(object? sender, EventArgs e)
    {
        SetValue(IsOpeningProperty, false);
        IsOpen = false;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        if (parentWindow == null) return;
        parentWindow.Deactivated -= OnDeactivated;
    }

    public bool IsOpening
    {
        get => IsOpen;
        set => IsOpen = value;
    }

    public bool ResetOpening
    {
        get => (bool) GetValue(ResetOpeningProperty);
        set => SetValue(ResetOpeningProperty, value);
    }

    public string HelpHeader
    {
        get => GetHelpHeader(this);
        set => SetHelpHeader(this, value);
    }

    public string HelpContent
    {
        get => GetHelpContent(this);
        set => SetHelpContent(this, value);
    }

    public string HelpLink
    {
        get => GetHelpLink(this);
        set => SetHelpLink(this, value);
    }

    public static string GetHelpHeader(DependencyObject element)
    {
        return (string) element.GetValue(HelpHeaderProperty);
    }

    public static void SetHelpHeader(DependencyObject element, string value)
    {
        element.SetValue(HelpHeaderProperty, value);
    }

    private static void HelpHeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    public static string GetHelpContent(DependencyObject element)
    {
        return (string) element.GetValue(HelpContentProperty);
    }

    public static void SetHelpContent(DependencyObject element, string value)
    {
        element.SetValue(HelpContentProperty, value);
    }

    private static void HelpContentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

    public static string GetHelpLink(DependencyObject element)
    {
        return (string) element.GetValue(HelpLinkProperty);
    }

    public static void SetHelpLink(DependencyObject element, string value)
    {
        element.SetValue(HelpLinkProperty, value);
    }

    private static void HelpLinkPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
}