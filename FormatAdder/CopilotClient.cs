using System.Diagnostics;
using System.Text;

namespace FormatAdder;

/// <summary>
/// Client for interacting with GitHub Copilot CLI
/// </summary>
public class CopilotClient
{
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var result = await RunCommandAsync("gh", "copilot --version");
            return result.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetSuggestionAsync(string prompt, string fileName)
    {
        try
        {
            // Use gh copilot suggest to get code suggestions
            var escapedPrompt = prompt.Replace("\"", "\\\"");
            var command = $"copilot suggest --type shell \"Provide code changes for {fileName}: {escapedPrompt}\"";
            
            var result = await RunCommandAsync("gh", command);
            
            if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(result.Output))
            {
                return result.Output;
            }
            
            Console.WriteLine($"⚠️  Copilot suggestion failed for {fileName}");
            return GenerateFallbackSuggestion(prompt, fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Error getting Copilot suggestion: {ex.Message}");
            return GenerateFallbackSuggestion(prompt, fileName);
        }
    }

    private string GenerateFallbackSuggestion(string prompt, string fileName)
    {
        return $@"
// Fallback suggestion for {fileName}
// Manual implementation required based on this prompt:
/*
{prompt}
*/

// TODO: Implement the following:
// 1. Add new case statement for the format
// 2. Create new format method
// 3. Follow existing patterns in the codebase
// 4. Add appropriate error handling
";
    }

    private async Task<CommandResult> RunCommandAsync(string command, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) error.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        return new CommandResult
        {
            ExitCode = process.ExitCode,
            Output = output.ToString(),
            Error = error.ToString()
        };
    }

    private record CommandResult
    {
        public int ExitCode { get; init; }
        public string Output { get; init; } = string.Empty;
        public string Error { get; init; } = string.Empty;
    }
}
