# Transcript Analytics CLI

A powerful CLI application for analyzing text transcripts with AI-powered insights and interactive visualizations.

## Features

- 📊 **Word Frequency Analysis** - Count and filter word occurrences with customizable thresholds
- 🤖 **AI-Powered Analysis** - Generate summaries, extract key terms, and perform sentiment analysis using OpenAI
- 📈 **Interactive Charts** - Visualize data with bar charts, pie charts, and line graphs
- 💾 **Multiple Output Formats** - Export results to JSON, Markdown, YAML, or plain text
- 🌐 **HTML Reports** - Beautiful web-based reports that open automatically in your browser

## Setup

1. **Install Dependencies**
   ```bash
   dotnet restore
   ```

2. **Configure Environment Variables**
   - Copy `.env.example` to `.env`
   - Add your OpenAI API key:
     ```
     OPENAI_API_KEY=sk-your-openai-api-key-here
     ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

## Usage

### Basic Analysis (HTML Report)
```bash
# Generate an HTML report with bar chart (default)
dotnet run transcript.txt

# Specify minimum word count threshold
dotnet run transcript.txt --min-count-threshold 5
dotnet run transcript.txt -t 5
```

### Chart Types
```bash
# Bar chart (default)
dotnet run transcript.txt --chart bar

# Pie chart
dotnet run transcript.txt --chart pie

# Line chart  
dotnet run transcript.txt --chart line

# Short form
dotnet run transcript.txt -c pie
```

### File Output
```bash
# Export to JSON
dotnet run transcript.txt --output-file results.json

# Export to Markdown
dotnet run transcript.txt --output-file report.md

# Export to YAML
dotnet run transcript.txt --output-file data.yaml

# Export to plain text
dotnet run transcript.txt --output-file analysis.txt

# Short form
dotnet run transcript.txt -o results.json
```

### Combined Options
```bash
# Custom threshold with specific chart type
dotnet run transcript.txt -t 8 -c pie

# Analysis with file output and custom threshold
dotnet run transcript.txt -t 3 -o detailed_report.md

# Full command with all options
dotnet run transcript.txt --min-count-threshold 10 --chart bar --output-file analysis.json
```

## Output Examples

### HTML Report Features
- 📊 Interactive charts using Chart.js
- 📈 Statistics overview (total words, unique words, key terms)
- 📝 AI-generated summary and sentiment analysis
- 🎨 Color-coded word frequency (green = top quartile, red = bottom quartile, blue = middle)
- 📱 Responsive design for mobile and desktop

### File Export Formats

**JSON Output** - Structured data perfect for further processing
**Markdown Output** - Human-readable reports with tables
**YAML Output** - Configuration-friendly format
**Text Output** - Simple, clean text format

## Configuration Options

Set these in your `.env` file:

```bash
# Required
OPENAI_API_KEY=sk-your-api-key-here

# Optional customization
OPENAI_MODEL=gpt-4
MIN_WORD_COUNT_THRESHOLD=10
MAX_TRANSCRIPT_LENGTH=3000
```

## Command Line Arguments

| Argument | Short | Description | Default |
|----------|-------|-------------|---------|
| `file-path` | - | Path to transcript file | Required |
| `--min-count-threshold` | `-t` | Minimum word count to include | 10 |
| `--chart` | `-c` | Chart type: bar, pie, line | bar |
| `--output-file` | `-o` | Export to file instead of HTML | None |

## Examples from Development

These commands were used during development and testing:

```bash
# Original basic command
dotnet run transcript.txt -t 5

# Test different chart types
dotnet run transcript.txt -c bar
dotnet run transcript.txt -c pie  
dotnet run transcript.txt -c line

# Test file outputs
dotnet run transcript.txt -o test.json
dotnet run transcript.txt -o report.md
dotnet run transcript.txt -o data.yaml
dotnet run transcript.txt -o summary.txt

# Combined testing
dotnet run transcript.txt -t 3 -c pie -o results.json
```

## Technical Notes

The application uses:
- **.NET 9.0** - Latest .NET runtime
- **System.CommandLine** - Modern CLI argument parsing
- **OpenAI API** - GPT-4 for text analysis
- **Chart.js** - Interactive web charts
- **YamlDotNet** - YAML serialization
- **DotNetEnv** - Environment variable management
