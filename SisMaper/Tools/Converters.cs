using System;
using System.Globalization;
using System.Linq;
using System.Windows;
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

public class NullToVisibility : IValueConverter
{
    public object IfNull { get; set; }

    public object IfNotNull { get; set; }

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is null ? IfNull : IfNotNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
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

public class NotNullOrEmpty : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string {Length: 0} or null ? DependencyProperty.UnsetValue : value;
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

public class ObjectAndConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.FirstOrDefault(value => value != DependencyProperty.UnsetValue);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ObjectAndConverter is a OneWay converter.");
    }
}