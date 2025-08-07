using System.Text.RegularExpressions;

namespace FormatAdder;

/// <summary>
/// Analyzes the existing codebase to understand current structure and patterns
/// </summary>
public class CodeAnalyzer
{
    public async Task<CodeStructure> AnalyzeCodebaseAsync(string projectPath)
    {
        var structure = new CodeStructure();

        // Analyze FileOutput.cs if it exists
        var fileOutputPath = Path.Combine(projectPath, "FileOutput.cs");
        if (File.Exists(fileOutputPath))
        {
            var content = await File.ReadAllTextAsync(fileOutputPath);
            structure.ExistingFormats.AddRange(ExtractFormats(content));
            structure.HasFileOutput = true;
        }

        // Analyze Program.cs
        var programPath = Path.Combine(projectPath, "program.cs");
        if (File.Exists(programPath))
        {
            structure.HasProgram = true;
        }

        return structure;
    }

    private List<string> ExtractFormats(string content)
    {
        var formats = new List<string>();
        
        // Look for case statements like: case ".txt":
        var casePattern = @"case\s+""\.(\w+)""\s*:";
        var matches = Regex.Matches(content, casePattern, RegexOptions.IgnoreCase);
        
        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                formats.Add(match.Groups[1].Value.ToLower());
            }
        }

        return formats;
    }
}

/// <summary>
/// Represents the structure of the analyzed codebase
/// </summary>
public class CodeStructure
{
    public List<string> ExistingFormats { get; set; } = new();
    public bool HasFileOutput { get; set; }
    public bool HasProgram { get; set; }
    public List<string> RequiredChanges { get; set; } = new();
}
