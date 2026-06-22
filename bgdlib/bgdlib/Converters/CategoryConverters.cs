using System.Globalization;
using Microsoft.Maui.Controls;

namespace bgdlib.Converters;

// Converts a category string (e.g. "news", "tutorial") → foreground Color
public class CategoryColorConverter : IValueConverter
{
    private static readonly Dictionary<string, Color> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["tutorial"]   = Color.FromArgb("#1E88E5"),
        ["job"]        = Color.FromArgb("#43A047"),
        ["vacancy"]    = Color.FromArgb("#43A047"),
        ["tool"]       = Color.FromArgb("#8E24AA"),
        ["plugin"]     = Color.FromArgb("#8E24AA"),
        ["news"]       = Color.FromArgb("#E53935"),
        ["postmortem"] = Color.FromArgb("#F9A825"),
        ["docs"]       = Color.FromArgb("#78909C"),
        ["vfx"]        = Color.FromArgb("#E0792B"),
        ["rendering"]  = Color.FromArgb("#E0792B"),
        ["marketing"]  = Color.FromArgb("#E0792B"),
        ["shaders"]    = Color.FromArgb("#17B0B8"),
        ["2d"]         = Color.FromArgb("#17B0B8"),
        ["3d"]         = Color.FromArgb("#2F74E0"),
        ["leveldesign"]= Color.FromArgb("#2F74E0"),
        ["physics"]    = Color.FromArgb("#5B8FE0"),
        ["procgen"]    = Color.FromArgb("#9061F0"),
        ["ui/ux"]      = Color.FromArgb("#9061F0"),
        ["ai"]         = Color.FromArgb("#1FA463"),
        ["release"]    = Color.FromArgb("#1FA463"),
        ["mobile"]     = Color.FromArgb("#4FBE7A"),
        ["animation"]  = Color.FromArgb("#4FBE7A"),
        ["gameplay"]   = Color.FromArgb("#43A047"),
        ["optimize"]   = Color.FromArgb("#F9A825"),
        ["netcode"]    = Color.FromArgb("#E0418E"),
        ["audio"]      = Color.FromArgb("#E0418E"),
        ["multiplayer"]= Color.FromArgb("#E53935"),
        ["jam"]        = Color.FromArgb("#E53935"),
    };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string cat && _map.TryGetValue(cat, out var color))
            return color;
        return Color.FromArgb("#888888");
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// Same map but returns a 20%-alpha background color for badge backgrounds
public class CategoryBgConverter : IValueConverter
{
    private static readonly CategoryColorConverter _fg = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var color = (Color)_fg.Convert(value, targetType, parameter, culture);
        return color.WithAlpha(0.18f);
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// Returns a Material Icons glyph for the category
public class CategoryIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value as string)?.ToLowerInvariant() switch
        {
            "tutorial"   => "",  // school
            "job"        => "",  // work
            "vacancy"    => "",
            "tool"       => "",  // build
            "plugin"     => "",
            "news"       => "",  // newspaper
            "postmortem" => "",  // history_edu
            "docs"       => "",  // description
            "vfx"        => "",  // auto_awesome
            "shaders"    => "",  // gradient
            "2d"         => "",  // crop_square
            "3d"         => "",  // view_in_ar
            "physics"    => "",  // sports_motorsports (placeholder)
            "ai"         => "",  // memory
            "audio"      => "",  // music_note
            "gameplay"   => "",  // sports_esports
            _            => "",  // article (fallback)
        };
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
