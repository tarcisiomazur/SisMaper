using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace SisMaper.Tools
{
    public class MyConverter<T> : IValueConverter
    {
        private Func<T,object> Converter;
        
        public MyConverter(Func<T, object> converter)
        {
            Converter = converter;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = Converter.Invoke((T) value);
            Console.WriteLine(x);
            return x;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
    public class DecimalToRealString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "R$"+((decimal)value).RealFormat();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public static class InterfaceExtension
    {
        
        public static bool IsTrue(this bool? b)
        {
            return b.HasValue && b.Value;
        }

        public static void AddSelector<T>(this ObservableCollection<DataGridColumn> dataGridColumns, string columnName, string element, Func<T,object> fun)
        {
            var dgc = new DataGridTextColumn()
            {
                Header = columnName,
                CanUserSort = true,
                SortMemberPath = element,
                Width = new DataGridLength(1.0, DataGridLengthUnitType.Star),
                Binding = new Binding()
                {
                    Converter = new MyConverter<T>(fun)
                },
            };
            dataGridColumns.Add(dgc);
        }

        public static void AddSelector(this ObservableCollection<DataGridColumn> dataGridColumns, string columnName, string element)
        {
            var dgc = new DataGridTextColumn();
            dgc.Header = columnName;
            dgc.Width = new DataGridLength(1.0, DataGridLengthUnitType.Star);
            dgc.Binding = new Binding(element);
            dataGridColumns.Add(dgc);
        }

        public static string RealFormat(this decimal d)
        {
            return $"{d:N2}";
        }
        public static string DmaFormat(this DateTime d)
        {
            return $"{d:d}";
        }

    }
}