using System.CommandLine;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FormatAdder;

/// <summary>
/// Console application that uses GitHub Copilot to add new output formats to the transcript analytics application
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Add new output format support using GitHub Copilot");

        var formatArgument = new Argument<string>(
            name: "format",
            description: "The format to add (e.g., XML, CSV, PDF)");

        var descriptionOption = new Option<string>(
            name: "--description",
            description: "Description of what the format should contain") { IsRequired = true };

        var targetPathOption = new Option<string>(
            name: "--target-path",
            description: "Path to the target project directory",
            getDefaultValue: () => @"c:\Users\rande\Code\Sandbox\AI Training\paicc-cs-spec-coding");

        descriptionOption.AddAlias("-d");
        targetPathOption.AddAlias("-t");

        rootCommand.AddArgument(formatArgument);
        rootCommand.AddOption(descriptionOption);
        rootCommand.AddOption(targetPathOption);

        rootCommand.SetHandler(async (string format, string description, string targetPath) =>
        {
            var adder = new FormatAdder(targetPath);
            await adder.AddFormatAsync(format, description);
        }, formatArgument, descriptionOption, targetPathOption);

        return await rootCommand.InvokeAsync(args);
    }
}

/// <summary>
/// Handles the process of adding new output formats using GitHub Copilot
/// </summary>
public class FormatAdder
{
    private readonly string _targetPath;
    private readonly CopilotClient _copilot;
    private readonly CodeAnalyzer _analyzer;
    private readonly FileManager _fileManager;

    public FormatAdder(string targetPath)
    {
        _targetPath = targetPath;
        _copilot = new CopilotClient();
        _analyzer = new CodeAnalyzer();
        _fileManager = new FileManager();
    }

    public async Task AddFormatAsync(string format, string description)
    {
        Console.WriteLine($"🚀 Adding {format} format support to Transcript Analytics");
        Console.WriteLine($"📋 Description: {description}");
        Console.WriteLine($"📁 Target path: {_targetPath}");
        Console.WriteLine();

        try
        {
            // Check prerequisites
            if (!await _copilot.IsAvailableAsync())
            {
                Console.WriteLine("❌ GitHub Copilot CLI not available. Please install: gh extension install github/gh-copilot");
                return;
            }

            // Analyze current code structure
            Console.WriteLine("🔍 Analyzing current code structure...");
            var structure = await _analyzer.AnalyzeCodebaseAsync(_targetPath);

            if (structure.ExistingFormats.Contains(format.ToLower()))
            {
                Console.WriteLine($"⚠️  {format} format already exists!");
                if (!ConfirmContinue())
                    return;
            }

            // Generate and apply changes
            await ProcessFileOutputChanges(format, description, structure);
            await ProcessProgramChanges(format, description);

            Console.WriteLine();
            Console.WriteLine("✅ Format addition process complete!");
            Console.WriteLine("📝 Manual review and testing recommended");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    private async Task ProcessFileOutputChanges(string format, string description, CodeStructure structure)
    {
        Console.WriteLine("🤖 Generating FileOutput.cs changes...");
        
        var prompt = GenerateFileOutputPrompt(format, description, structure);
        var suggestion = await _copilot.GetSuggestionAsync(prompt, "FileOutput.cs");
        
        if (suggestion != null)
        {
            var filePath = Path.Combine(_targetPath, "FileOutput.cs");
            await ApplyChangesAsync(filePath, suggestion, format);
        }
    }

    private async Task ProcessProgramChanges(string format, string description)
    {
        Console.WriteLine("🤖 Generating Program.cs changes...");
        
        var prompt = GenerateProgramPrompt(format);
        var suggestion = await _copilot.GetSuggestionAsync(prompt, "program.cs");
        
        if (suggestion != null)
        {
            var filePath = Path.Combine(_targetPath, "program.cs");
            await ApplyChangesAsync(filePath, suggestion, format);
        }
    }

    private string GenerateFileOutputPrompt(string format, string description, CodeStructure structure)
    {
        var existingFormats = string.Join(", ", structure.ExistingFormats);
        
        return $@"
You are modifying a C# transcript analytics application to add support for {format} output format.

Task: {description}

Current existing formats: {existingFormats}

Please modify the FileOutput.cs file to:
1. Add a new case for "".{format.ToLower()}"" in the WriteToFileAsync method switch statement
2. Create a new private static async method Write{format}Async that formats the analysis and word counts data appropriately for {format}
3. The {format} format should include both the LLMAnalysisResult and Dictionary<string, int> wordCounts data
4. Follow the existing pattern used by other format methods
5. Use appropriate {format} formatting libraries if needed (mention required using statements)

Method signature should be:
private static async Task Write{format}Async(LLMAnalysisResult analysis, Dictionary<string, int> wordCounts, string filePath)

Make sure the {format} output is well-structured and includes:
- Analysis summary and insights
- Word frequency data
- Proper {format} formatting and structure
";
    }

    private string GenerateProgramPrompt(string format)
    {
        return $@"
You are updating the Program.cs file to support a new {format} output format.

Please update:
1. The outputFileOption description text to include .{format.ToLower()} as a supported format
2. Any file extension validation to accept .{format.ToLower()} files
3. Update console output messages that list supported formats to include {format}

Keep all existing functionality intact, just add support for the new {format} format.
";
    }

    private async Task ApplyChangesAsync(string filePath, string suggestion, string format)
    {
        Console.WriteLine($"📝 Proposed changes for {Path.GetFileName(filePath)}:");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(suggestion);
        Console.WriteLine(new string('-', 50));

        if (ConfirmApply())
        {
            try
            {
                await _fileManager.CreateBackupAsync(filePath);
                Console.WriteLine($"✅ Backup created for {Path.GetFileName(filePath)}");
                
                Console.WriteLine($"📁 Opening {Path.GetFileName(filePath)} for manual editing...");
                await _fileManager.OpenFileAsync(filePath);
                
                // Save suggestion to a reference file
                var suggestionFile = Path.Combine(Path.GetDirectoryName(filePath)!, $"{format}_suggestions_{Path.GetFileName(filePath)}.txt");
                await File.WriteAllTextAsync(suggestionFile, suggestion);
                Console.WriteLine($"💾 Suggestions saved to: {suggestionFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to process {filePath}: {ex.Message}");
            }
        }
    }

    private static bool ConfirmContinue()
    {
        Console.Write("Continue anyway? (y/n): ");
        var response = Console.ReadLine();
        return response?.ToLower() is "y" or "yes";
    }

    private static bool ConfirmApply()
    {
        Console.Write("Apply these changes? (y/n): ");
        var response = Console.ReadLine();
        return response?.ToLower() is "y" or "yes";
    }
}
