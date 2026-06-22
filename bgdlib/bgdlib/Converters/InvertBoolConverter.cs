using System.Globalization;
using Microsoft.Maui.Controls;

namespace bgdlib.Converters;

public class InvertBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : value;
}

public class NotEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && !string.IsNullOrEmpty(s);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class IsEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is string s && string.IsNullOrEmpty(s);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToAccentConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is true) ? Color.FromArgb("#E53935") : Color.FromArgb("#242424");
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToAccentInvConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is true) ? Color.FromArgb("#242424") : Color.FromArgb("#E53935");
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// Returns #29E53935 (tinted red) when value == parameter, else #1a1a1a
public class ChipSelectedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool selected = value switch
        {
            string s when parameter is string p => s == p,
            bool b when parameter is string p   => b && p == "True",
            _ => false
        };
        return selected ? Color.FromArgb("#29E53935") : Color.FromArgb("#1a1a1a");
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// Returns #FFFFFF when selected, #888888 when not
public class ChipTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool selected = value switch
        {
            string s when parameter is string p => s == p,
            bool b when parameter is string p   => b && p == "True",
            _ => false
        };
        return selected ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#888888");
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class EngineColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value as string) switch
        {
            "Unity"  => Color.FromArgb("#a6e3a1"),
            "Godot"  => Color.FromArgb("#89b4fa"),
            "Unreal" => Color.FromArgb("#f38ba8"),
            _        => Color.FromArgb("#f9e2af"),
        };
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class EngineLetterConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value as string) switch
        {
            "Unity"  => "U",
            "Godot"  => "G",
            "Unreal" => "UE",
            _        => "?",
        };
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
