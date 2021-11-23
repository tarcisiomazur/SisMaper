using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace SisMaper.Views.Templates
{
    public class ColumnDataGridSave
    {

        public static readonly DependencyProperty SaveSizesProperty =
            DependencyProperty.RegisterAttached("SaveSizes",
                typeof(bool),
                typeof(ColumnDataGridSave),
                new FrameworkPropertyMetadata(false, SaveSizesPropertyChanged));

        public static void SetSaveSizes(DependencyObject obj, bool value)
        {
            obj.SetValue(SaveSizesProperty, value);
        }

        public static bool GetSaveSizes(DependencyObject obj)
        {
            return (bool) obj.GetValue(SaveSizesProperty);
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
            var dg = (DataGrid) d;
            if ((bool) e.NewValue)
            {
                dg.Initialized += Initialize;
            }
        }

        private static void Initialize(object? sender, EventArgs e)
        {
            if (sender is not DataGrid dg) return;
            var window = Window.GetWindow(dg);
            if (window == null)
            {
                var uc = GetMyUserControl(dg);
                if (uc is null) return;
                uc.OnOpen += ReadWidths(dg, uc.ToString());
                uc.OnClose += SaveWidths(dg, uc.ToString());
            }
            else
            {
                ReadWidths(dg, window.ToString()).Invoke(null, EventArgs.Empty);
                window.Closed += SaveWidths(dg, window.ToString());
            }
        }

        private static EventHandler ReadWidths(DataGrid dg, string? path)
        {
            return (_, _) =>
            {
                var reg = Registry.CurrentUser.CreateSubKey(@$"SOFTWARE\SisMaper\DataGridSizes\{path}\{dg.Name}");
                if (reg == null) return;
                foreach (var c in dg.Columns)
                {
                    if (reg?.GetValue(c.Header.ToString()) is not string value) continue;
                    var values = value.Split(';');
                    if (values.Length != 2) continue;
                    if (int.TryParse(values[0], out var index) && index >= 0 && index < dg.Columns.Count)
                        c.DisplayIndex = index;
                    if (double.TryParse(values[1], out var val))
                        c.Width = new DataGridLength(val, c.Width.UnitType);
                }
            };
        }

        private static EventHandler SaveWidths(DataGrid dg, string? path)
        {
            return (o, args) =>
            {
                var window = Window.GetWindow(dg);
                if (window == null) return;
                var reg = Registry.CurrentUser.CreateSubKey(
                    @$"SOFTWARE\SisMaper\DataGridSizes\{path}\{dg.Name}");

                if (reg == null) return;
                foreach (var c in dg.Columns)
                {
                    reg.SetValue(c.Header.ToString(), $"{c.DisplayIndex};{c.Width.DesiredValue}");
                }
            };
        }
    }
}