using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace TranscriptAnalytics;

/// <summary>
/// Handles file output functionality for different formats
/// </summary>
public static class FileOutput
{
    /// <summary>
    /// Formats analysis data as a string based on the file extension
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <param name="filePath">Output file path to determine format</param>
    /// <returns>Formatted string based on file extension</returns>
    public static string FormatAsString(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts, string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        
        return extension switch
        {
            ".json" => FormatAsJson(analysis, wordCounts),
            ".md" => FormatAsMd(analysis, wordCounts),
            ".yaml" or ".yml" => FormatAsYaml(analysis, wordCounts),
            ".txt" => FormatAsTxt(analysis, wordCounts),
            _ => FormatAsTxt(analysis, wordCounts) // Default to text format
        };
    }

    /// <summary>
    /// Formats the analysis data as JSON
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>JSON formatted string</returns>
    public static string FormatAsJson(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts)
    {
        var output = new
        {
            Analysis = new
            {
                Summary = analysis.Summary,
                ImportantWords = analysis.ImportantWords,
                SentimentAnalysis = analysis.SentimentAnalysis
            },
            WordCounts = wordCounts,
            GeneratedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            TotalWords = wordCounts.Values.Sum(),
            UniqueWords = wordCounts.Count
        };

        return JsonSerializer.Serialize(output, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    /// <summary>
    /// Formats the analysis data as Markdown
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>Markdown formatted string</returns>
    public static string FormatAsMd(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("# Transcript Analysis Report");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"**Total Words:** {wordCounts.Values.Sum():N0}");
        sb.AppendLine($"**Unique Words:** {wordCounts.Count:N0}");
        sb.AppendLine();
        
        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine(analysis.Summary);
        sb.AppendLine();
        
        sb.AppendLine("## Important Words");
        sb.AppendLine();
        foreach (var word in analysis.ImportantWords)
        {
            sb.AppendLine($"- {word}");
        }
        sb.AppendLine();
        
        sb.AppendLine("## Sentiment Analysis");
        sb.AppendLine();
        sb.AppendLine(analysis.SentimentAnalysis);
        sb.AppendLine();
        
        sb.AppendLine("## Word Frequency Analysis");
        sb.AppendLine();
        sb.AppendLine("| Word | Count |");
        sb.AppendLine("|------|-------|");
        
        foreach (var kvp in wordCounts.Take(20))
        {
            sb.AppendLine($"| {kvp.Key} | {kvp.Value} |");
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Formats the analysis data as YAML
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>YAML formatted string</returns>
    public static string FormatAsYaml(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts)
    {
        var output = new
        {
            analysis = new
            {
                summary = analysis.Summary,
                important_words = analysis.ImportantWords,
                sentiment_analysis = analysis.SentimentAnalysis
            },
            word_counts = wordCounts,
            metadata = new
            {
                generated_at = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                total_words = wordCounts.Values.Sum(),
                unique_words = wordCounts.Count
            }
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance)
            .Build();
            
        return serializer.Serialize(output);
    }

    /// <summary>
    /// Formats the analysis data as plain text
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <returns>Plain text formatted string</returns>
    public static string FormatAsTxt(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("TRANSCRIPT ANALYSIS REPORT");
        sb.AppendLine("=========================");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Total Words: {wordCounts.Values.Sum():N0}");
        sb.AppendLine($"Unique Words: {wordCounts.Count:N0}");
        sb.AppendLine();
        
        sb.AppendLine("SUMMARY");
        sb.AppendLine("-------");
        sb.AppendLine(analysis.Summary);
        sb.AppendLine();
        
        sb.AppendLine("IMPORTANT WORDS");
        sb.AppendLine("---------------");
        foreach (var word in analysis.ImportantWords)
        {
            sb.AppendLine($"• {word}");
        }
        sb.AppendLine();
        
        sb.AppendLine("SENTIMENT ANALYSIS");
        sb.AppendLine("------------------");
        sb.AppendLine(analysis.SentimentAnalysis);
        sb.AppendLine();
        
        sb.AppendLine("WORD FREQUENCY ANALYSIS");
        sb.AppendLine("-----------------------");
        foreach (var kvp in wordCounts)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Writes the formatted data to a file
    /// </summary>
    /// <param name="analysis">The transcript analysis results</param>
    /// <param name="wordCounts">Dictionary of word counts</param>
    /// <param name="filePath">Output file path</param>
    public static async Task WriteToFileAsync(TranscriptAnalysis analysis, Dictionary<string, int> wordCounts, string filePath)
    {
        string content = FormatAsString(analysis, wordCounts, filePath);
        
        // Ensure directory exists
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8);
    }
}
