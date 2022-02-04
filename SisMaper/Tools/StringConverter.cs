using System;

namespace SisMaper.Tools;

public static class StringConverter
{
    public static long ToLong(this string str)
    {
        return long.TryParse(str, out var x) ? x : 0;
    }

    public static int ToInt(this string str)
    {
        return int.TryParse(str, out var x) ? x : 0;
    }

    public static double ToDouble(this string str)
    {
        return double.TryParse(str, out var x) ? x : 0;
    }

    public static decimal ToDecimal(this string str, int precision = 0)
    {
        return decimal.TryParse(str, out var x) ? Math.Round(x, precision) : 0;
    }
}