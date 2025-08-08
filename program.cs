using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TranscriptAnalytics;

/// <summary>
/// Main program entry point for the transcript analytics CLI application
/// </summary>
class Program
{
    /// <summary>
    /// Main entry point for the application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code</returns>
    static async Task<int> Main(string[] args)
    {
        // Load environment variables from .env file
        EnvironmentConfig.LoadEnvironmentVariables();

        // Create the root command with description
        var rootCommand = new RootCommand("Transcript Analytics CLI - Analyze text files for word frequency and insights");

        // Create command line options
        var filePathArgument = new Argument<string>(
            name: "file-path",
            description: "Path to the transcript text file to analyze");

        var thresholdOption = new Option<int>(
            name: "--min-count-threshold",
            description: "Minimum word count threshold (default: 10)",
            getDefaultValue: () => 10);

        var chartOption = new Option<string>(
            name: "--chart",
            description: "Chart type for HTML output: bar, pie, line, radial, bubble (default: bar)",
            getDefaultValue: () => "bar");

        var outputFileOption = new Option<string?>(
            name: "--output-file",
            description: "Output file path (.txt, .json, .md, .yaml, .htmlsld). If not specified, shows HTML in browser");

        var themeOption = new Option<string>(
            name: "--theme",
            description: $"HTML theme: {string.Join(", ", HtmlReportDirector.GetAvailableThemes())} (default: default)",
            getDefaultValue: () => "default");

        thresholdOption.AddAlias("-t");
        chartOption.AddAlias("-c");
        outputFileOption.AddAlias("-o");
        themeOption.AddAlias("--th");

        // Add arguments and options to the command
        rootCommand.AddArgument(filePathArgument);
        rootCommand.AddOption(thresholdOption);
        rootCommand.AddOption(chartOption);
        rootCommand.AddOption(outputFileOption);
        rootCommand.AddOption(themeOption);

        // Set the handler for the command
        rootCommand.SetHandler(async (string filePath, int minCountThreshold, string chartType, string? outputFile, string theme) =>
        {
            await AnalyzeTranscript(filePath, minCountThreshold, chartType, outputFile, theme);
        }, filePathArgument, thresholdOption, chartOption, outputFileOption, themeOption);

        // Execute the command
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Analyzes a transcript file and displays word frequency and LLM analysis results
    /// </summary>
    /// <param name="pathToScriptTextFile">Path to the transcript text file</param>
    /// <param name="minCountThreshold">Minimum count threshold for word inclusion</param>
    /// <param name="chartType">Type of chart to display (bar, pie, line, radial, bubble)</param>
    /// <param name="outputFile">Optional output file path for saving results</param>
    /// <param name="theme">HTML theme for the report</param>
    private static async Task AnalyzeTranscript(string pathToScriptTextFile, int minCountThreshold = 10, string chartType = "bar", string? outputFile = null, string theme = "default")
    {
        try
        {
            Console.WriteLine($"🔍 Analyzing transcript: {pathToScriptTextFile}");
            Console.WriteLine($"📊 Minimum count threshold: {minCountThreshold}");
            if (!string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine($"📁 Output file: {outputFile}");
                
                // Show theme and chart info for .htmlsld files
                if (outputFile.EndsWith(".htmlsld", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"📊 Chart type: {chartType}");
                    Console.WriteLine($"🎨 Theme: {theme}");
                }
            }
            else
            {
                Console.WriteLine($"📊 Chart type: {chartType}");
                Console.WriteLine($"🎨 Theme: {theme}");
            }
            Console.WriteLine();

            // Read the file content
            string transcriptContent = File.ReadAllText(pathToScriptTextFile);
            
            // Count words using our word counter
            Console.WriteLine("⚙️  Counting words...");
            var wordCounts = WordCounter.CountWords(pathToScriptTextFile, minCountThreshold);

            // Run LLM analysis
            Console.WriteLine();
            Console.WriteLine("🤖 Running AI analysis...");
            var analysis = await LLMAnalyzer.AnalyzeTranscript(transcriptContent, wordCounts);

            // Handle output based on options
            if (!string.IsNullOrEmpty(outputFile))
            {
                // Check for .htmlsld extension (HTML with slider)
                if (outputFile.EndsWith(".htmlsld", StringComparison.OrdinalIgnoreCase))
                {
                    // Generate HTML with slider functionality
                    Console.WriteLine($"🎛️ Generating interactive HTML report with {chartType} chart and {theme} theme...");
                    var director = new HtmlReportDirector();
                    string analysisText = $"{analysis.Summary}\n\nKey Words: {string.Join(", ", analysis.ImportantWords)}\n\nSentiment: {analysis.SentimentAnalysis}";
                    string htmlContent = director.ConstructReportWithSlider(analysisText, wordCounts, chartType, theme);
                    
                    // Save with .html extension
                    string actualOutputFile = outputFile.Replace(".htmlsld", ".html");
                    await File.WriteAllTextAsync(actualOutputFile, htmlContent);
                    Console.WriteLine($"✅ Interactive HTML file saved: {actualOutputFile}");
                    
                    // Open in browser
                    OpenInBrowser(actualOutputFile);
                    
                    // Display usage example
                    Console.WriteLine();
                    Console.WriteLine("💡 Usage example:");
                    Console.WriteLine($"   {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} \"{pathToScriptTextFile}\" -o report.htmlsld --chart radial --theme ocean");
                }
                else if (outputFile.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    // Generate regular HTML file
                    Console.WriteLine($"📄 Generating HTML report with {chartType} chart and {theme} theme...");
                    string analysisText = $"{analysis.Summary}\n\nKey Words: {string.Join(", ", analysis.ImportantWords)}\n\nSentiment: {analysis.SentimentAnalysis}";
                    string htmlContent = HtmlGenerator.GenerateHtmlReport(analysisText, wordCounts, chartType);
                    
                    await File.WriteAllTextAsync(outputFile, htmlContent);
                    Console.WriteLine($"✅ HTML file saved: {outputFile}");
                    
                    // Open in browser
                    OpenInBrowser(outputFile);
                }
                else
                {
                    // Save to regular file formats
                    Console.WriteLine($"💾 Saving results to {outputFile}...");
                    await FileOutput.WriteToFileAsync(analysis, wordCounts, outputFile);
                    Console.WriteLine("✅ File saved successfully!");
                }
                
                // Also display summary in console
                Console.WriteLine();
                Console.WriteLine("📊 SUMMARY:");
                Console.WriteLine($"Total Words: {wordCounts.Values.Sum():N0}");
                Console.WriteLine($"Unique Words: {wordCounts.Count:N0}");
                
            }
            else
            {
                // No output file specified - generate temporary HTML and open in browser
                Console.WriteLine($"🌐 Generating HTML report with {chartType} chart and {theme} theme...");
                string analysisText = $"{analysis.Summary}\n\nKey Words: {string.Join(", ", analysis.ImportantWords)}\n\nSentiment: {analysis.SentimentAnalysis}";
                string htmlContent = HtmlGenerator.GenerateHtmlReport(analysisText, wordCounts, chartType);
                
                // Create temporary file
                string tempFile = Path.Combine(Path.GetTempPath(), $"transcript_analysis_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                await File.WriteAllTextAsync(tempFile, htmlContent);
                
                // Open in browser
                OpenInBrowser(tempFile);
                
                // Display summary in console
                Console.WriteLine();
                Console.WriteLine("📊 SUMMARY:");
                Console.WriteLine($"Total Words: {wordCounts.Values.Sum():N0}");
                Console.WriteLine($"Unique Words: {wordCounts.Count:N0}");
            }

            Console.WriteLine();
            Console.WriteLine("✅ Analysis complete!");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"❌ Error: File not found - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Opens a file in the default browser
    /// </summary>
    /// <param name="filePath">Path to the HTML file to open</param>
    private static void OpenInBrowser(string filePath)
    {
        try
        {
            string fullPath = Path.GetFullPath(filePath);
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", fullPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", fullPath);
            }
            
            Console.WriteLine($"🌐 Opening {Path.GetFileName(filePath)} in your default browser...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Could not open browser automatically: {ex.Message}");
            Console.WriteLine($"📄 Please manually open: {Path.GetFullPath(filePath)}");
        }
    }
}
