using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace SisMaper.Views.Templates
{
    public static class SaveUIProperty
    {

        public static readonly DependencyProperty SaveSizesProperty =
            DependencyProperty.RegisterAttached("SaveSizes",
                typeof(bool),
                typeof(SaveUIProperty),
                new FrameworkPropertyMetadata(false, SaveSizesPropertyChanged));
        
        public static readonly DependencyProperty SaveSelectedProperty =
            DependencyProperty.RegisterAttached("SaveSelected",
                typeof(bool),
                typeof(SaveUIProperty),
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

        private static MyUserControl? GetMyUserControl(DependencyObject source)
        {
            while (source is not null and not MyUserControl)
            {
                source = source.GetParentObject();
            }

            return source as MyUserControl;
        }

        private static void SaveSizesPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dg && (bool) e.NewValue)
            {
                dg.Initialized += Initialize;
            }
        }
        
        private static void SaveSelectPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var dg = (ComboBox) d;
            if ((bool) e.NewValue)
            {
                dg.Initialized += Initialize;
            }
        }

        private static void Initialize(object? sender, EventArgs e)
        {
            if (sender is not DependencyObject dep) return;
            var window = Window.GetWindow(dep);
            if (window == null)
            {
                var uc = GetMyUserControl(dep);
                if (uc is null) return;
                switch (sender)
                {
                    case DataGrid dg:
                        uc.Loaded += ReadWidths(dg, uc.ToString());
                        uc.Unloaded += SaveWidths(dg, uc.ToString());
                        break;
                    case ComboBox cb:
                        uc.Loaded += ReadSelected(cb, uc.ToString());
                        uc.Unloaded += SaveSelected(cb, uc.ToString());
                        break;
                }
            }
            else
            {
                switch (sender)
                {
                    case DataGrid dg:
                        ReadWidths(dg, window.ToString()).Invoke(null, null);
                        window.Unloaded += SaveWidths(dg, window.ToString());
                        break;
                    case ComboBox cb:
                        ReadSelected(cb, window.ToString()).Invoke(null, null);
                        window.Unloaded += SaveSelected(cb, window.ToString());
                        break;
                }
            }
        }

        private static RoutedEventHandler ReadSelected(ComboBox cb, string? path)
        {
            return (_, _) =>
            {
                var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\ComboBoxSelection");
                if (reg == null) return;
                var obj = reg?.GetValue(cb.Name);
                if (obj is int selected)
                {
                    cb.SelectedIndex = selected;
                }
            };
        }

        private static RoutedEventHandler SaveSelected(ComboBox cb, string? path)
        {
            return (_, _) =>
            {
                var window = Window.GetWindow(cb);
                if (window == null) return;
                var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\ComboBoxSelection\");

                if (reg != null)
                    reg.SetValue(cb.Name, cb.SelectedIndex);
            };
        }
        private static RoutedEventHandler ReadWidths(DataGrid dg, string? path)
        {
            return (_, _) =>
            {
                var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\{path}\DataGridSizes\{dg.Name}");
                if (reg == null) return;
                foreach (var c in dg.Columns)
                {
                    if (reg?.GetValue(c.Header.ToString()) is not string value) continue;
                    var values = value.Split(';');
                    if (values.Length != 3) continue;
                    if (int.TryParse(values[0], out var index) && index >= 0 && index < dg.Columns.Count)
                        c.DisplayIndex = index;
                    if (double.TryParse(values[1], out var width) && width > 0)
                        c.Width = new DataGridLength(width, c.Width.UnitType);
                    if(!dg.CanUserSortColumns) continue;
                    if (int.TryParse(values[2], out var order) && order is 0 or 1)
                        c.SortDirection = (ListSortDirection) order;
                }
            };
        }

        private static RoutedEventHandler SaveWidths(DataGrid dg, string? path)
        {
            return (o, args) =>
            {
                var window = Window.GetWindow(dg);
                if (window == null) return;
                var reg = Registry.CurrentUser.CreateSubKey(
                    @$"SOFTWARE\SisMaper\{path}\DataGridSizes\{dg.Name}");

                if (reg == null) return;
                foreach (var c in dg.Columns)
                {
                    var sort = (int?) c.SortDirection ?? 2;
                    reg.SetValue(c.Header.ToString(), $"{c.DisplayIndex};{c.Width.DesiredValue};{sort}");
                }
            };
        }
    }
}