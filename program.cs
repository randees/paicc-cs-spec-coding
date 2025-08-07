using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
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
            description: "Chart type for HTML output: bar, pie, or line (default: bar)",
            getDefaultValue: () => "bar");

        var outputFileOption = new Option<string?>(
            name: "--output-file",
            description: "Output file path (.txt, .json, .md, .yaml). If not specified, shows HTML in browser");

        thresholdOption.AddAlias("-t");
        chartOption.AddAlias("-c");
        outputFileOption.AddAlias("-o");

        // Add arguments and options to the command
        rootCommand.AddArgument(filePathArgument);
        rootCommand.AddOption(thresholdOption);
        rootCommand.AddOption(chartOption);
        rootCommand.AddOption(outputFileOption);

        // Set the handler for the command
        rootCommand.SetHandler(async (string filePath, int minCountThreshold, string chartType, string? outputFile) =>
        {
            await AnalyzeTranscript(filePath, minCountThreshold, chartType, outputFile);
        }, filePathArgument, thresholdOption, chartOption, outputFileOption);

        // Execute the command
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Analyzes a transcript file and displays word frequency and LLM analysis results
    /// </summary>
    /// <param name="pathToScriptTextFile">Path to the transcript text file</param>
    /// <param name="minCountThreshold">Minimum count threshold for word inclusion</param>
    /// <param name="chartType">Type of chart to display (bar, pie, line)</param>
    /// <param name="outputFile">Optional output file path for saving results</param>
    private static async Task AnalyzeTranscript(string pathToScriptTextFile, int minCountThreshold = 10, string chartType = "bar", string? outputFile = null)
    {
        try
        {
            Console.WriteLine($"🔍 Analyzing transcript: {pathToScriptTextFile}");
            Console.WriteLine($"📊 Minimum count threshold: {minCountThreshold}");
            if (!string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine($"📁 Output file: {outputFile}");
            }
            else
            {
                Console.WriteLine($"📊 Chart type: {chartType}");
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
                // Save to file
                Console.WriteLine($"💾 Saving results to {outputFile}...");
                await FileOutput.WriteToFileAsync(analysis, wordCounts, outputFile);
                Console.WriteLine("✅ File saved successfully!");
                
                // Also display summary in console
                Console.WriteLine();
                Console.WriteLine("📊 SUMMARY:");
                Console.WriteLine($"Total Words: {wordCounts.Values.Sum():N0}");
                Console.WriteLine($"Unique Words: {wordCounts.Count:N0}");
                Console.WriteLine($"Analysis saved to: {outputFile}");
            }
            else
            {
                // Generate and save HTML report, then open in browser
                Console.WriteLine($"🌐 Generating HTML report with {chartType} chart...");
                string htmlContent = HtmlGenerator.GenerateHtmlReport(analysis, wordCounts, chartType);
                
                // Save to temporary HTML file
                string tempFile = Path.Combine(Path.GetTempPath(), "transcript_analysis_report.html");
                await File.WriteAllTextAsync(tempFile, htmlContent);
                
                Console.WriteLine($"📄 HTML report generated: {tempFile}");
                
                // Try to open in default browser
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempFile,
                        UseShellExecute = true
                    });
                    Console.WriteLine("🌍 Opening report in your default browser...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️  Could not open browser automatically: {ex.Message}");
                    Console.WriteLine($"📁 Please open this file manually: {tempFile}");
                }
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
}
