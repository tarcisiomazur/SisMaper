using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SisMaper.Models;

namespace SisMaper.Tools;

public class MyConverter<T> : IValueConverter
{
    private readonly Func<T, object> Converter;

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
        return "R$" + ((decimal) value).RealFormat();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class NotZero : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return (long) value != 0L ? value : DependencyProperty.UnsetValue;
        }
        catch
        {
            return value;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class ParamIfTrue : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? parameter : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class PedidoStatusToBoolean : IValueConverter
{
    public bool IsEditable { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Pedido.Pedido_Status.Aberto == IsEditable;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool) value == IsEditable ? Pedido.Pedido_Status.Aberto : Pedido.Pedido_Status.Fechado;
    }
}

public class MyRSAConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Encrypt.RSAEncryption(value as string ?? "");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Encrypt.RSAEncryption(value as string ?? "");
    }
}

public class FaturaStatusToBooleanIsEditable : IValueConverter
{
    public bool IsEditable { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Fatura.Fatura_Status.Aberta == IsEditable;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool) value == IsEditable ? Fatura.Fatura_Status.Aberta : Fatura.Fatura_Status.Fechada;
    }
}

public class StringToSha512 : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Encrypt.ToSha512((string) value);
    }
}

public static class InterfaceExtension
{
    public static bool IsChanged<T>(this DependencyPropertyChangedEventArgs e, out T obj)
    {
        if (e.NewValue != e.OldValue && e.NewValue is T value)
        {
            obj = value;
            return true;
        }

        obj = default!;
        return false;
    }

    public static bool IsContainedIn(this string? str, string? value)
    {
        return str == null || value != null && (value.Length == 0 || value.Contains(str, StringComparison.InvariantCultureIgnoreCase));
    }

    public static bool IsOpen(this Pedido pedido)
    {
        return pedido.Status == Pedido.Pedido_Status.Aberto;
    }

    public static bool BeEmitted(this NotaFiscal.EnumSituacao situacao)
    {
        return situacao is NotaFiscal.EnumSituacao.Aprovado
            or NotaFiscal.EnumSituacao.Processamento
            or NotaFiscal.EnumSituacao.Contingencia;
    }

    public static bool IsAprovado(this NotaFiscal.EnumSituacao situacao)
    {
        return situacao is NotaFiscal.EnumSituacao.Aprovado;
    }

    public static bool IsNatural(this double value)
    {
        return value == Math.Floor(value) && !double.IsInfinity(value);
    }

    public static bool IsTrue(this bool? b)
    {
        return b.HasValue && b.Value;
    }

    public static void AddSelector<T>(this ObservableCollection<DataGridColumn> dataGridColumns, string columnName,
        string element, Func<T, object> fun)
    {
        var dgc = new DataGridTextColumn
        {
            Header = columnName,
            CanUserSort = true,
            SortMemberPath = element,
            Width = new DataGridLength(1.0, DataGridLengthUnitType.Star),
            Binding = new Binding
            {
                Converter = new MyConverter<T>(fun)
            }
        };
        dataGridColumns.Add(dgc);
    }

    public static void AddSelector(this ObservableCollection<DataGridColumn> dataGridColumns, string columnName,
        string element)
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