using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace TranscriptAnalytics;

/// <summary>
/// Director class that orchestrates the HTML report building process with themes
/// </summary>
public class HtmlReportDirector
{
    private static readonly Dictionary<string, IThemeBuilder> _availableThemes = new()
    {
        { "default", new DefaultThemeBuilder() },
        { "dark", new DarkThemeBuilder() },
        { "ocean", new OceanThemeBuilder() },
        { "forest", new ForestThemeBuilder() }
    };

    /// <summary>
    /// Gets all available theme names
    /// </summary>
    public static string[] GetAvailableThemes()
    {
        return _availableThemes.Keys.ToArray();
    }

    /// <summary>
    /// Constructs an HTML report with the specified theme
    /// </summary>
    /// <param name="analysis">LLM analysis results</param>
    /// <param name="wordCounts">Word frequency data</param>
    /// <param name="chartType">Type of chart to generate</param>
    /// <param name="themeName">Name of the theme to apply</param>
    /// <returns>Complete HTML report with themed styling</returns>
    public string ConstructReport(string analysis, Dictionary<string, int> wordCounts, string chartType, string themeName = "default")
    {
        // Get the appropriate theme builder
        if (!_availableThemes.TryGetValue(themeName.ToLower(), out var themeBuilder))
        {
            themeBuilder = _availableThemes["default"];
        }

        // Build the themed CSS styles
        string themeStyles = themeBuilder.BuildCssStyles();

        // Generate the complete HTML report with theme
        return GenerateThemedHtmlReport(analysis, wordCounts, chartType, themeStyles, themeBuilder.GetThemeName());
    }

    /// <summary>
    /// Constructs an HTML report with slider filtering capability
    /// </summary>
    /// <param name="analysis">LLM analysis results</param>
    /// <param name="wordCounts">Word frequency data</param>
    /// <param name="chartType">Type of chart to generate (bar, pie, line, radial, bubble)</param>
    /// <param name="themeName">Name of the theme to apply</param>
    /// <returns>Complete HTML report with slider controls and themed styling</returns>
    public string ConstructReportWithSlider(string analysis, Dictionary<string, int> wordCounts, string chartType, string themeName = "default")
    {
        // Get the appropriate theme builder
        if (!_availableThemes.TryGetValue(themeName.ToLower(), out var themeBuilder))
        {
            themeBuilder = _availableThemes["default"];
        }

        // Build the themed CSS styles
        string themeStyles = themeBuilder.BuildCssStyles();

        // Generate the complete HTML report with slider and theme
        return GenerateHtmlWithSliderFilter(analysis, wordCounts, chartType, themeStyles, themeBuilder.GetThemeName());
    }

    /// <summary>
    /// Creates JavaScript for radial bar chart visualization
    /// </summary>
    /// <param name="wordCounts">Word frequency data</param>
    /// <returns>JavaScript code for radial bar chart</returns>
    private string CreateRadialBarChart(Dictionary<string, int> wordCounts)
    {
        var topWords = wordCounts.OrderByDescending(kvp => kvp.Value).Take(20).ToList();
        
        return $@"
        function createRadialBarChart(data) {{
            const ctx = document.getElementById('wordChart').getContext('2d');
            return new Chart(ctx, {{
                type: 'polarArea',
                data: {{
                    labels: data.map(item => item.word),
                    datasets: [{{
                        label: 'Word Frequency',
                        data: data.map(item => item.count),
                        backgroundColor: [
                            'var(--chart-color-1)',
                            'var(--chart-color-2)',
                            'var(--chart-color-3)',
                            'var(--chart-color-4)',
                            'var(--chart-color-5)'
                        ].slice(0, data.length)
                    }}]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {{
                        title: {{
                            display: true,
                            text: 'Word Frequency - Radial View'
                        }},
                        legend: {{
                            position: 'bottom'
                        }}
                    }},
                    scales: {{
                        r: {{
                            beginAtZero: true,
                            ticks: {{
                                color: 'var(--text-color)'
                            }}
                        }}
                    }}
                }}
            }};
        }}";
    }

    /// <summary>
    /// Creates JavaScript for bubble chart visualization
    /// </summary>
    /// <param name="wordCounts">Word frequency data</param>
    /// <returns>JavaScript code for bubble chart</returns>
    private string CreateBubbleChart(Dictionary<string, int> wordCounts)
    {
        var topWords = wordCounts.OrderByDescending(kvp => kvp.Value).Take(20).ToList();
        
        return $@"
        function createBubbleChart(data) {{
            const ctx = document.getElementById('wordChart').getContext('2d');
            const bubbleData = data.map((item, index) => ({{
                x: index + 1,
                y: item.count,
                r: Math.sqrt(item.count) * 2
            }}));
            
            return new Chart(ctx, {{
                type: 'bubble',
                data: {{
                    datasets: [{{
                        label: 'Word Frequency',
                        data: bubbleData,
                        backgroundColor: data.map((_, index) => 
                            `var(--chart-color-${{(index % 5) + 1}})`
                        )
                    }}]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {{
                        title: {{
                            display: true,
                            text: 'Word Frequency - Bubble View'
                        }},
                        tooltip: {{
                            callbacks: {{
                                label: function(context) {{
                                    const word = data[context.dataIndex].word;
                                    const count = context.parsed.y;
                                    return `${{word}}: ${{count}} occurrences`;
                                }}
                            }}
                        }}
                    }},
                    scales: {{
                        x: {{
                            title: {{
                                display: true,
                                text: 'Word Rank'
                            }},
                            ticks: {{
                                color: 'var(--text-color)'
                            }}
                        }},
                        y: {{
                            title: {{
                                display: true,
                                text: 'Frequency'
                            }},
                            ticks: {{
                                color: 'var(--text-color)'
                            }}
                        }}
                    }}
                }}
            }};
        }}";
    }

    /// <summary>
    /// Generates the complete HTML report with themed styling
    /// </summary>
    private string GenerateThemedHtmlReport(string analysis, Dictionary<string, int> wordCounts, string chartType, string themeStyles, string themeName)
    {
        var topWords = wordCounts.OrderByDescending(kvp => kvp.Value).Take(20).ToList();
        
        return $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Transcript Analysis Report - {themeName} Theme</title>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <style>
        {themeStyles}
        
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            padding: 2rem;
        }}
        
        .stats-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }}
        
        .stat-card {{
            background: var(--background-color);
            border: 1px solid var(--secondary-color);
            border-radius: 8px;
            padding: 1rem;
            text-align: center;
        }}
        
        .stat-number {{
            font-size: 2rem;
            font-weight: bold;
            color: var(--primary-color);
        }}
        
        .chart-container {{
            position: relative;
            height: 400px;
            margin: 2rem 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 Transcript Analysis Report</h1>
            <p>Theme: {themeName} | Chart Type: {chartType.ToUpper()}</p>
        </div>
        
        <div class='stats-grid'>
            <div class='stat-card'>
                <div class='stat-number'>{wordCounts.Values.Sum():N0}</div>
                <div>Total Words</div>
            </div>
            <div class='stat-card'>
                <div class='stat-number'>{wordCounts.Count:N0}</div>
                <div>Unique Words</div>
            </div>
            <div class='stat-card'>
                <div class='stat-number'>{topWords.FirstOrDefault().Value}</div>
                <div>Most Frequent</div>
            </div>
        </div>
        
        <div class='section'>
            <h2>🎯 AI Analysis</h2>
            <div>{analysis.Replace("\n", "<br>")}</div>
        </div>
        
        <div class='section chart-colors'>
            <h2>📈 Word Frequency Visualization</h2>
            <div class='chart-container'>
                <canvas id='wordChart'></canvas>
            </div>
        </div>
        
        <script>
            const ctx = document.getElementById('wordChart').getContext('2d');
            const data = {{
                labels: {System.Text.Json.JsonSerializer.Serialize(topWords.Select(w => w.Key).ToArray())},
                datasets: [{{
                    label: 'Word Frequency',
                    data: {System.Text.Json.JsonSerializer.Serialize(topWords.Select(w => w.Value).ToArray())},
                    backgroundColor: [
                        'var(--chart-color-1)',
                        'var(--chart-color-2)',
                        'var(--chart-color-3)',
                        'var(--chart-color-4)',
                        'var(--chart-color-5)'
                    ]
                }}]
            }};
            
            new Chart(ctx, {{
                type: '{chartType}',
                data: data,
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {{
                        title: {{
                            display: true,
                            text: 'Top 20 Most Frequent Words'
                        }}
                    }}
                }}
            }});
        </script>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Generates HTML report with slider filtering capability
    /// </summary>
    /// <param name="analysis">LLM analysis results</param>
    /// <param name="wordCounts">Word frequency data</param>
    /// <param name="chartType">Type of chart to generate</param>
    /// <param name="themeStyles">CSS theme styles</param>
    /// <param name="themeName">Name of the applied theme</param>
    /// <returns>Complete HTML with slider controls</returns>
    private string GenerateHtmlWithSliderFilter(string analysis, Dictionary<string, int> wordCounts, string chartType, string themeStyles, string themeName)
    {
        var allWords = wordCounts.OrderByDescending(kvp => kvp.Value).ToList();
        var maxCount = allWords.FirstOrDefault().Value;
        var minCount = allWords.LastOrDefault().Value;
        
        // Generate chart creation function based on type
        string chartFunction = chartType.ToLower() switch
        {
            "radial" => CreateRadialBarChart(wordCounts),
            "bubble" => CreateBubbleChart(wordCounts),
            "pie" => CreateStandardChart("pie"),
            "line" => CreateStandardChart("line"),
            _ => CreateStandardChart("bar")
        };

        return $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Interactive Transcript Analysis - {themeName} Theme</title>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <style>
        {themeStyles}
        
        .container {{
            max-width: 1400px;
            margin: 0 auto;
            padding: 2rem;
        }}
        
        .controls {{
            background: var(--background-color);
            border: 1px solid var(--secondary-color);
            border-radius: 8px;
            padding: 1.5rem;
            margin-bottom: 2rem;
        }}
        
        .slider-container {{
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 1rem;
        }}
        
        .slider {{
            flex: 1;
            height: 6px;
            border-radius: 3px;
            background: var(--secondary-color);
            outline: none;
            opacity: 0.7;
            transition: opacity 0.2s;
        }}
        
        .slider:hover {{
            opacity: 1;
        }}
        
        .slider::-webkit-slider-thumb {{
            appearance: none;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            background: var(--primary-color);
            cursor: pointer;
        }}
        
        .stats-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }}
        
        .stat-card {{
            background: var(--background-color);
            border: 1px solid var(--secondary-color);
            border-radius: 8px;
            padding: 1rem;
            text-align: center;
        }}
        
        .stat-number {{
            font-size: 2rem;
            font-weight: bold;
            color: var(--primary-color);
        }}
        
        .chart-container {{
            position: relative;
            height: 500px;
            margin: 2rem 0;
        }}
        
        .word-list {{
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
            gap: 0.5rem;
            margin-top: 1rem;
            max-height: 300px;
            overflow-y: auto;
        }}
        
        .word-item {{
            display: flex;
            justify-content: space-between;
            padding: 0.5rem;
            background: var(--background-color);
            border: 1px solid var(--secondary-color);
            border-radius: 4px;
            font-size: 0.9rem;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📊 Interactive Transcript Analysis</h1>
            <p>Theme: {themeName} | Chart Type: {chartType.ToUpper()} | Total Words: {wordCounts.Values.Sum():N0}</p>
        </div>
        
        <div class='controls'>
            <h3>🎛️ Filter Controls</h3>
            <div class='slider-container'>
                <label>Min Frequency:</label>
                <input type='range' id='minFrequency' class='slider' 
                       min='{minCount}' max='{maxCount}' value='{minCount}' 
                       oninput='updateChart()'>
                <span id='minValue'>{minCount}</span>
            </div>
            <div class='slider-container'>
                <label>Max Words:</label>
                <input type='range' id='maxWords' class='slider' 
                       min='5' max='50' value='20' 
                       oninput='updateChart()'>
                <span id='maxWordsValue'>20</span>
            </div>
        </div>
        
        <div class='stats-grid'>
            <div class='stat-card'>
                <div class='stat-number' id='totalWords'>{wordCounts.Values.Sum():N0}</div>
                <div>Total Words</div>
            </div>
            <div class='stat-card'>
                <div class='stat-number' id='uniqueWords'>{wordCounts.Count:N0}</div>
                <div>Unique Words</div>
            </div>
            <div class='stat-card'>
                <div class='stat-number' id='displayedWords'>20</div>
                <div>Displayed Words</div>
            </div>
            <div class='stat-card'>
                <div class='stat-number' id='avgFrequency'>{(wordCounts.Values.Sum() / (double)wordCounts.Count):F1}</div>
                <div>Avg Frequency</div>
            </div>
        </div>
        
        <div class='section'>
            <h2>🎯 AI Analysis</h2>
            <div>{analysis.Replace("\n", "<br>")}</div>
        </div>
        
        <div class='section chart-colors'>
            <h2>📈 Interactive Word Frequency Visualization</h2>
            <div class='chart-container'>
                <canvas id='wordChart'></canvas>
            </div>
            <div class='word-list' id='wordList'></div>
        </div>
        
        <script>
            // Word data for filtering
            const allWordData = {JsonSerializer.Serialize(allWords.Select(w => new { word = w.Key, count = w.Value }).ToArray())};
            let currentChart = null;
            
            {chartFunction}
            
            function updateChart() {{
                const minFreq = parseInt(document.getElementById('minFrequency').value);
                const maxWords = parseInt(document.getElementById('maxWords').value);
                
                // Update display values
                document.getElementById('minValue').textContent = minFreq;
                document.getElementById('maxWordsValue').textContent = maxWords;
                
                // Filter data
                const filteredData = allWordData
                    .filter(item => item.count >= minFreq)
                    .slice(0, maxWords);
                
                // Update stats
                document.getElementById('displayedWords').textContent = filteredData.length;
                
                // Update chart
                if (currentChart) {{
                    currentChart.destroy();
                }}
                
                currentChart = {(chartType.ToLower() == "radial" ? "createRadialBarChart" : chartType.ToLower() == "bubble" ? "createBubbleChart" : "createStandardChart")}(filteredData);
                
                // Update word list
                updateWordList(filteredData);
            }}
            
            function updateWordList(data) {{
                const wordList = document.getElementById('wordList');
                wordList.innerHTML = data.map(item => 
                    `<div class='word-item'>
                        <span>${{item.word}}</span>
                        <strong>${{item.count}}</strong>
                    </div>`
                ).join('');
            }}
            
            // Initialize chart
            updateChart();
        </script>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Creates JavaScript for standard chart types (bar, pie, line)
    /// </summary>
    /// <param name="chartType">Type of standard chart</param>
    /// <returns>JavaScript code for standard chart</returns>
    private string CreateStandardChart(string chartType)
    {
        return $@"
        function createStandardChart(data) {{
            const ctx = document.getElementById('wordChart').getContext('2d');
            return new Chart(ctx, {{
                type: '{chartType}',
                data: {{
                    labels: data.map(item => item.word),
                    datasets: [{{
                        label: 'Word Frequency',
                        data: data.map(item => item.count),
                        backgroundColor: [
                            'var(--chart-color-1)',
                            'var(--chart-color-2)',
                            'var(--chart-color-3)',
                            'var(--chart-color-4)',
                            'var(--chart-color-5)'
                        ]
                    }}]
                }},
                options: {{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {{
                        title: {{
                            display: true,
                            text: `Word Frequency - ${{'{chartType}'.charAt(0).toUpperCase() + '{chartType}'.slice(1)}} Chart`
                        }}
                    }}
                }}
            }};
        }}";
    }
}
