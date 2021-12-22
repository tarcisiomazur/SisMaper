using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace SisMaper.Views.Templates;

public static class SaveUIProperty
{
    public static readonly DependencyProperty SaveSizesProperty =
        DependencyProperty.RegisterAttached("SaveSizes",
            typeof(bool),
            typeof(DataGrid),
            new FrameworkPropertyMetadata(false, SaveSizesPropertyChanged));

    public static readonly DependencyProperty SaveSelectedProperty =
        DependencyProperty.RegisterAttached("SaveSelected",
            typeof(bool),
            typeof(ComboBox),
            new FrameworkPropertyMetadata(false, SaveSelectPropertyChanged));

    public static void SetSaveSizes(DependencyObject obj, bool value)
    {
        obj.SetValue(SaveSizesProperty, value);
    }

    public static bool GetSaveSizes(DependencyObject obj)
    {
        return (bool) obj.GetValue(SaveSizesProperty);
    }

    public static void SetSaveSelected(DependencyObject obj, bool value)
    {
        obj.SetValue(SaveSelectedProperty, value);
    }

    public static bool GetSaveSelected(DependencyObject obj)
    {
        return (bool) obj.GetValue(SaveSelectedProperty);
    }

    private static UserControl? GetUserControl(DependencyObject source)
    {
        while (source is not null and not UserControl)
            source = source.GetParentObject();

        return source as UserControl;
    }

    private static void SaveSizesPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dg) return;
        switch ((bool) e.NewValue)
        {
            case true:
                dg.Loaded += ReadWidths;
                dg.Unloaded += SaveWidths;
                break;
            case false:
                dg.Loaded -= ReadWidths;
                dg.Unloaded -= SaveWidths;
                break;
        }
    }

    private static void SaveSelectPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var cb = (ComboBox) d;
        switch ((bool) e.NewValue)
        {
            case true:
                cb.Loaded += ReadSelected;
                cb.Unloaded += SaveSelected;
                break;
            case false:
                cb.Loaded -= ReadWidths;
                cb.Unloaded -= SaveWidths;
                break;
        }
    }

    private static void ReadSelected(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ComboBox cb) return;
        var path = Window.GetWindow(cb)?.ToString() ?? GetUserControl(cb)?.ToString();
        if (path is null) return;
        var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\ComboBoxSelection");
        if (reg == null) return;
        var obj = reg?.GetValue(cb.Name);
        if (obj is int selected)
        {
            cb.SelectedIndex = selected;
        }
    }

    private static void SaveSelected(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ComboBox cb) return;
        var path = Window.GetWindow(cb)?.ToString() ?? GetUserControl(cb)?.ToString();
        if (path is null) return;
        var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\ComboBoxSelection\");

        if (reg != null)
        {
            reg.SetValue(cb.Name, cb.SelectedIndex);
        }
    }

    private static void ReadWidths(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not DataGrid dg) return;
        var path = Window.GetWindow(dg)?.ToString() ?? GetUserControl(dg)?.ToString();
        if (path is null) return;

        var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\DataGridSizes\{dg.Name}");
        if (reg == null) return;
        foreach (var c in dg.Columns)
        {
            if (reg?.GetValue(c.Header.ToString()) is not string value) continue;
            var values = value.Split(';');
            if (values.Length != 3) continue;
            if (int.TryParse(values[0], out var index) && index >= 0 && index < dg.Columns.Count)
            {
                c.DisplayIndex = index;
            }

            if (double.TryParse(values[1], out var width) && width > 0)
            {
                c.Width = new DataGridLength(width, c.Width.UnitType);
            }

            if (!dg.CanUserSortColumns) continue;
            if (int.TryParse(values[2], out var order) && order is 0 or 1)
            {
                c.SortDirection = (ListSortDirection) order;
            }
        }
    }

    private static void SaveWidths(object sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not DataGrid dg) return;
        var path = Window.GetWindow(dg)?.ToString() ?? GetUserControl(dg)?.ToString();
        if (path is null) return;

        var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\DataGridSizes\{dg.Name}");
        if (reg == null) return;

        foreach (var c in dg.Columns)
        {
            var sort = (int?) c.SortDirection ?? 2;
            reg.SetValue(c.Header.ToString(), $"{c.DisplayIndex};{c.Width.DesiredValue};{sort}");
        }
    }
}