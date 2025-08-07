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

        thresholdOption.AddAlias("-t");

        // Add arguments and options to the command
        rootCommand.AddArgument(filePathArgument);
        rootCommand.AddOption(thresholdOption);

        // Set the handler for the command
        rootCommand.SetHandler(async (string filePath, int minCountThreshold) =>
        {
            await AnalyzeTranscript(filePath, minCountThreshold);
        }, filePathArgument, thresholdOption);

        // Execute the command
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Analyzes a transcript file and displays word frequency and LLM analysis results
    /// </summary>
    /// <param name="pathToScriptTextFile">Path to the transcript text file</param>
    /// <param name="minCountThreshold">Minimum count threshold for word inclusion</param>
    private static async Task AnalyzeTranscript(string pathToScriptTextFile, int minCountThreshold = 10)
    {
        try
        {
            Console.WriteLine($"🔍 Analyzing transcript: {pathToScriptTextFile}");
            Console.WriteLine($"📊 Minimum count threshold: {minCountThreshold}");
            Console.WriteLine();

            // Read the file content
            string transcriptContent = File.ReadAllText(pathToScriptTextFile);
            
            // Count words using our word counter
            Console.WriteLine("⚙️  Counting words...");
            var wordCounts = WordCounter.CountWords(pathToScriptTextFile, minCountThreshold);

            // Display word frequency results
            Console.WriteLine();
            Console.WriteLine("📈 WORD FREQUENCY ANALYSIS");
            Console.WriteLine("================================");
            
            if (wordCounts.Any())
            {
                foreach (var kvp in wordCounts)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            else
            {
                Console.WriteLine("No words found above the minimum threshold.");
            }

            // Run LLM analysis
            Console.WriteLine();
            Console.WriteLine("🤖 Running AI analysis...");
            var analysis = await LLMAnalyzer.AnalyzeTranscript(transcriptContent, wordCounts);

            // Display LLM analysis results
            Console.WriteLine();
            Console.WriteLine("🧠 AI TRANSCRIPT ANALYSIS");
            Console.WriteLine("==========================");
            
            Console.WriteLine();
            Console.WriteLine("📝 SUMMARY:");
            Console.WriteLine(analysis.Summary);
            
            Console.WriteLine();
            Console.WriteLine("🔑 IMPORTANT WORDS:");
            foreach (var word in analysis.ImportantWords)
            {
                Console.WriteLine($"• {word}");
            }
            
            Console.WriteLine();
            Console.WriteLine("😊 SENTIMENT ANALYSIS:");
            Console.WriteLine(analysis.SentimentAnalysis);

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
