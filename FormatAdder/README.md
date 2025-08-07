# Format Adder Tool

A C# console application that uses GitHub Copilot to automatically add new output format support to the transcript analytics application.

## Prerequisites

1. .NET 8.0 SDK
2. GitHub CLI with Copilot extension:
   ```bash
   gh extension install github/gh-copilot
   ```
3. GitHub Copilot access and authentication:
   ```bash
   gh auth login
   ```

## Building

```bash
cd FormatAdder
dotnet build
```

## Usage

```bash
dotnet run -- <format> --description <description> [--target-path <path>]
```

### Examples

```bash
# Add XML support
dotnet run -- XML --description "Add XML format with hierarchical structure including analysis summary and word frequency data"

# Add CSV support
dotnet run -- CSV --description "Add CSV format for easy import into spreadsheet applications"

# Add PDF support with custom target path
dotnet run -- PDF --description "Add PDF format with formatted report" --target-path "C:\MyProject"
```

## Features

- **Code Analysis**: Automatically analyzes existing codebase structure
- **Copilot Integration**: Uses GitHub Copilot CLI for intelligent code suggestions
- **Safe Operations**: Creates timestamped backups before any changes
- **Interactive**: Requires user confirmation for each change
- **Fallback**: Provides manual guidance when Copilot is unavailable

## What It Does

1. Analyzes the target codebase to understand existing formats
2. Generates appropriate prompts for GitHub Copilot
3. Gets code suggestions for:
   - Adding new case statements to `FileOutput.cs`
   - Creating new format-specific methods
   - Updating help text in `program.cs`
4. Creates backups and opens files for manual editing
5. Saves suggestions as reference files

## Safety Features

- Creates timestamped backups before any modifications
- Requires explicit user confirmation for each change
- Opens files in default editor for manual review
- Saves Copilot suggestions as separate reference files
- Never automatically modifies code without user approval

## Output

The tool generates:
- Backup files with timestamps
- Suggestion files for manual reference
- Console guidance for implementation
- Opens target files for editing
