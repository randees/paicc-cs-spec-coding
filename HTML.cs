using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranscriptAnalytics;

/// <summary>
/// Handles HTML generation and charting functionality
/// </summary>
public static class HtmlGenerator
{
    /// <summary>
    /// Generates a complete HTML page with analysis results and charts
    /// </summary>
    /// <param name="analysis">The transcript analysis results as string</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <param name="chartType">Type of chart to display (bar, pie, line)</param>
    /// <returns>Complete HTML page as string</returns>
    public static string GenerateHtmlReport(string analysis, Dictionary<string, int> wordCounts, string chartType = "bar")
    {
        var sb = new StringBuilder();
        
        // HTML Document Structure
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Transcript Analytics Report</title>");
        sb.AppendLine("    <script src=\"https://cdn.jsdelivr.net/npm/chart.js\"></script>");
        sb.AppendLine("    <style>");
        sb.AppendLine(GetCssStyles());
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        
        // Header
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <h1>ðŸ“Š Transcript Analytics Report</h1>");
        sb.AppendLine($"        <p class=\"timestamp\">Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");
        
        // Statistics Summary
        sb.AppendLine("        <div class=\"stats-grid\">");
        sb.AppendLine($"            <div class=\"stat-card\"><h3>{wordCounts.Values.Sum():N0}</h3><p>Total Words</p></div>");
        sb.AppendLine($"            <div class=\"stat-card\"><h3>{wordCounts.Count:N0}</h3><p>Unique Words</p></div>");
        sb.AppendLine($"            <div class=\"stat-card\"><h3>{wordCounts.Take(10).Count()}</h3><p>Top Words</p></div>");
        sb.AppendLine("        </div>");
        
        // Analysis Section
        sb.AppendLine("        <div class=\"section\">");
        sb.AppendLine("            <h2>ðŸ¤– AI Analysis</h2>");
        sb.AppendLine($"            <p>{analysis.Replace("\n", "<br>")}</p>");
        sb.AppendLine("        </div>");
        
        // Chart Section
        sb.AppendLine("        <div class=\"section\">");
        sb.AppendLine("            <h2>ðŸ“ˆ Word Frequency Chart</h2>");
        sb.AppendLine("            <div class=\"chart-container\">");
        sb.AppendLine("                <canvas id=\"wordChart\"></canvas>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");
        
        // Chart Script
        sb.AppendLine("    </div>");
        sb.AppendLine("    <script>");
        sb.AppendLine(GenerateChartScript(wordCounts, chartType));
        sb.AppendLine("    </script>");
        
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        
        return sb.ToString();
    }

    /// <summary>
    /// Creates a bar chart for word counts
    /// </summary>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>HTML string for bar chart</returns>
    public static string CreateBarChart(Dictionary<string, int> wordCounts)
    {
        return GenerateChartScript(wordCounts, "bar");
    }

    /// <summary>
    /// Creates a pie chart for word counts
    /// </summary>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>HTML string for pie chart</returns>
    public static string CreatePieChart(Dictionary<string, int> wordCounts)
    {
        return GenerateChartScript(wordCounts, "pie");
    }

    /// <summary>
    /// Creates a line chart for word counts
    /// </summary>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>HTML string for line chart</returns>
    public static string CreateLineChart(Dictionary<string, int> wordCounts)
    {
        return GenerateChartScript(wordCounts, "line");
    }

    /// <summary>
    /// Generates Chart.js script for the specified chart type
    /// </summary>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <param name="chartType">Type of chart (bar, pie, line)</param>
    /// <returns>JavaScript code for the chart</returns>
    private static string GenerateChartScript(Dictionary<string, int> wordCounts, string chartType)
    {
        var topWords = wordCounts.Take(20).ToList();
        var labels = topWords.Select(kvp => $"'{kvp.Key}'").ToList();
        var data = topWords.Select(kvp => kvp.Value).ToList();
        
        // Calculate quartiles for color coding
        var sortedCounts = data.OrderByDescending(x => x).ToList();
        var quartileSize = sortedCounts.Count / 4;
        var topQuartileThreshold = quartileSize > 0 ? sortedCounts[quartileSize - 1] : sortedCounts.LastOrDefault();
        var bottomQuartileThreshold = quartileSize > 0 ? sortedCounts[sortedCounts.Count - quartileSize] : sortedCounts.FirstOrDefault();
        
        // Generate colors based on quartiles
        var colors = data.Select(count =>
        {
            if (count >= topQuartileThreshold && quartileSize > 0) return "'#22c55e'"; // Green
            if (count <= bottomQuartileThreshold && quartileSize > 0) return "'#ef4444'"; // Red
            return "'#3b82f6'"; // Blue
        }).ToList();

        var sb = new StringBuilder();
        
        sb.AppendLine("const ctx = document.getElementById('wordChart').getContext('2d');");
        sb.AppendLine("const wordChart = new Chart(ctx, {");
        sb.AppendLine($"    type: '{chartType}',");
        sb.AppendLine("    data: {");
        sb.AppendLine($"        labels: [{string.Join(", ", labels)}],");
        sb.AppendLine("        datasets: [{");
        sb.AppendLine("            label: 'Word Count',");
        sb.AppendLine($"            data: [{string.Join(", ", data)}],");
        
        if (chartType == "pie")
        {
            sb.AppendLine($"            backgroundColor: [{string.Join(", ", colors)}],");
            sb.AppendLine($"            borderColor: [{string.Join(", ", colors.Select(c => c))}],");
        }
        else
        {
            sb.AppendLine($"            backgroundColor: [{string.Join(", ", colors)}],");
            sb.AppendLine($"            borderColor: [{string.Join(", ", colors)}],");
        }
        
        sb.AppendLine("            borderWidth: 1");
        sb.AppendLine("        }]");
        sb.AppendLine("    },");
        sb.AppendLine("    options: {");
        sb.AppendLine("        responsive: true,");
        sb.AppendLine("        maintainAspectRatio: false,");
        
        if (chartType != "pie")
        {
            sb.AppendLine("        scales: {");
            if (chartType == "bar")
            {
                sb.AppendLine("            y: {");
                sb.AppendLine("                beginAtZero: true,");
                sb.AppendLine("                title: { display: true, text: 'Count' }");
                sb.AppendLine("            },");
                sb.AppendLine("            x: {");
                sb.AppendLine("                title: { display: true, text: 'Words' }");
                sb.AppendLine("            }");
            }
            else if (chartType == "line")
            {
                sb.AppendLine("            y: {");
                sb.AppendLine("                beginAtZero: true,");
                sb.AppendLine("                title: { display: true, text: 'Count' }");
                sb.AppendLine("            },");
                sb.AppendLine("            x: {");
                sb.AppendLine("                title: { display: true, text: 'Word Rank' }");
                sb.AppendLine("            }");
            }
            sb.AppendLine("        },");
        }
        
        sb.AppendLine("        plugins: {");
        sb.AppendLine("            title: {");
        sb.AppendLine("                display: true,");
        sb.AppendLine($"                text: 'Top {topWords.Count} Words - {chartType.ToUpper()} Chart'");
        sb.AppendLine("            },");
        sb.AppendLine("            legend: {");
        sb.AppendLine($"                display: {(chartType == "pie" ? "true" : "false")}");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("});");
        
        return sb.ToString();
    }

    /// <summary>
    /// Gets the CSS styles for the HTML report
    /// </summary>
    /// <returns>CSS styles as string</returns>
    private static string GetCssStyles()
    {
        return @"
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f8fafc;
            color: #1e293b;
            line-height: 1.6;
        }
        
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            padding: 30px;
        }
        
        h1 {
            color: #0f172a;
            margin-bottom: 10px;
            font-size: 2.5rem;
            font-weight: 700;
        }
        
        h2 {
            color: #334155;
            margin-top: 30px;
            margin-bottom: 15px;
            font-size: 1.5rem;
            font-weight: 600;
            border-bottom: 2px solid #e2e8f0;
            padding-bottom: 10px;
        }
        
        .timestamp {
            color: #64748b;
            font-size: 0.9rem;
            margin-bottom: 30px;
        }
        
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 8px;
            text-align: center;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }
        
        .stat-card h3 {
            margin: 0;
            font-size: 2rem;
            font-weight: 700;
        }
        
        .stat-card p {
            margin: 5px 0 0 0;
            opacity: 0.9;
            font-size: 0.9rem;
        }
        
        .section {
            margin-bottom: 30px;
        }
        
        .word-tags {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin-top: 15px;
        }
        
        .word-tag {
            background: #e0e7ff;
            color: #3730a3;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 500;
        }
        
        .chart-container {
            position: relative;
            height: 500px;
            margin-top: 20px;
            background: #f8fafc;
            border-radius: 8px;
            padding: 20px;
        }
        
        p {
            color: #475569;
            margin: 10px 0;
        }
        
        @media (max-width: 768px) {
            .container {
                margin: 10px;
                padding: 20px;
            }
            
            h1 {
                font-size: 2rem;
            }
            
            .stats-grid {
                grid-template-columns: 1fr;
            }
            
            .chart-container {
                height: 400px;
                padding: 15px;
            }
        }";
    }
}
