using System.Collections.Generic;

namespace TranscriptAnalytics;

/// <summary>
/// Interface for building HTML themes
/// </summary>
public interface IThemeBuilder
{
    IThemeBuilder SetPrimaryColor(string color);
    IThemeBuilder SetSecondaryColor(string color);
    IThemeBuilder SetBackgroundColor(string color);
    IThemeBuilder SetTextColor(string color);
    IThemeBuilder SetAccentColor(string color);
    IThemeBuilder SetChartColors(string[] colors);
    string BuildCssStyles();
    string GetThemeName();
}

/// <summary>
/// Default theme builder
/// </summary>
public class DefaultThemeBuilder : IThemeBuilder
{
    private string _primaryColor = "#2563eb";
    private string _secondaryColor = "#64748b";
    private string _backgroundColor = "#ffffff";
    private string _textColor = "#1f2937";
    private string _accentColor = "#f59e0b";
    private string[] _chartColors = { "#3b82f6", "#ef4444", "#10b981", "#f59e0b", "#8b5cf6" };

    public string GetThemeName() => "Default";

    public IThemeBuilder SetPrimaryColor(string color)
    {
        _primaryColor = color;
        return this;
    }

    public IThemeBuilder SetSecondaryColor(string color)
    {
        _secondaryColor = color;
        return this;
    }

    public IThemeBuilder SetBackgroundColor(string color)
    {
        _backgroundColor = color;
        return this;
    }

    public IThemeBuilder SetTextColor(string color)
    {
        _textColor = color;
        return this;
    }

    public IThemeBuilder SetAccentColor(string color)
    {
        _accentColor = color;
        return this;
    }

    public IThemeBuilder SetChartColors(string[] colors)
    {
        _chartColors = colors;
        return this;
    }

    public string BuildCssStyles()
    {
        return $@"
        :root {{
            --primary-color: {_primaryColor};
            --secondary-color: {_secondaryColor};
            --background-color: {_backgroundColor};
            --text-color: {_textColor};
            --accent-color: {_accentColor};
            --chart-colors: {string.Join(", ", _chartColors)};
        }}
        
        body {{
            background-color: var(--background-color);
            color: var(--text-color);
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        }}
        
        .header {{
            background: linear-gradient(135deg, var(--primary-color), var(--accent-color));
            color: white;
            padding: 2rem;
            border-radius: 8px;
            margin-bottom: 2rem;
        }}
        
        .section {{
            background: var(--background-color);
            border: 1px solid #e5e7eb;
            border-radius: 8px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        }}
        
        .chart-colors {{
            --chart-color-1: {_chartColors[0]};
            --chart-color-2: {_chartColors[1]};
            --chart-color-3: {_chartColors[2]};
            --chart-color-4: {_chartColors[3]};
            --chart-color-5: {_chartColors[4]};
        }}";
    }
}

/// <summary>
/// Dark theme builder
/// </summary>
public class DarkThemeBuilder : DefaultThemeBuilder
{
    public DarkThemeBuilder()
    {
        SetBackgroundColor("#1f2937")
            .SetTextColor("#f9fafb")
            .SetPrimaryColor("#3b82f6")
            .SetSecondaryColor("#6b7280")
            .SetAccentColor("#fbbf24")
            .SetChartColors(new[] { "#60a5fa", "#f87171", "#34d399", "#fbbf24", "#a78bfa" });
    }

    public new string GetThemeName() => "Dark";
}

/// <summary>
/// Ocean theme builder
/// </summary>
public class OceanThemeBuilder : DefaultThemeBuilder
{
    public OceanThemeBuilder()
    {
        SetBackgroundColor("#f0f9ff")
            .SetTextColor("#0c4a6e")
            .SetPrimaryColor("#0369a1")
            .SetSecondaryColor("#0891b2")
            .SetAccentColor("#06b6d4")
            .SetChartColors(new[] { "#0ea5e9", "#06b6d4", "#22d3ee", "#67e8f9", "#0891b2" });
    }

    public new string GetThemeName() => "Ocean";
}

/// <summary>
/// Forest theme builder
/// </summary>
public class ForestThemeBuilder : DefaultThemeBuilder
{
    public ForestThemeBuilder()
    {
        SetBackgroundColor("#f0fdf4")
            .SetTextColor("#14532d")
            .SetPrimaryColor("#16a34a")
            .SetSecondaryColor("#15803d")
            .SetAccentColor("#84cc16")
            .SetChartColors(new[] { "#22c55e", "#84cc16", "#65a30d", "#16a34a", "#15803d" });
    }

    public new string GetThemeName() => "Forest";
}
