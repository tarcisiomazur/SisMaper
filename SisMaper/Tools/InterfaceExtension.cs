using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SisMaper.Models;

namespace SisMaper.Tools;

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
        return str == null || value != null &&
            (value.Length == 0 || value.Contains(str, StringComparison.InvariantCultureIgnoreCase));
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