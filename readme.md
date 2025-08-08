# Transcript Analytics CLI

A powerful CLI application for analyzing text transcripts with AI-powered insights and interactive visualizations.

## Features

- 📊 **Word Frequency Analysis** - Count and filter word occurrences with customizable thresholds
- 🤖 **AI-Powered Analysis** - Generate summaries, extract key terms, and perform sentiment analysis using OpenAI
- 📈 **Interactive Charts** - Visualize data with bar, pie, line, radial, and bubble charts
- 🎨 **Themed HTML Reports** - Beautiful reports with Default, Dark, Ocean, and Forest themes
- 🎛️ **Interactive Filtering** - Dynamic slider controls to filter word frequency in real-time
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

### Themes
```bash
# Default theme (light blue and gray)
dotnet run transcript.txt --theme default

# Dark theme (dark background with bright accents)
dotnet run transcript.txt --theme dark

# Ocean theme (blues and teals)
dotnet run transcript.txt --theme ocean

# Forest theme (greens and earth tones)
dotnet run transcript.txt --theme forest

# Short form
dotnet run transcript.txt --th dark
```

### Chart Types
```bash
# Bar chart (default)
dotnet run transcript.txt --chart bar

# Pie chart
dotnet run transcript.txt --chart pie

# Line chart  
dotnet run transcript.txt --chart line

# Radial chart (polar area visualization)
dotnet run transcript.txt --chart radial

# Bubble chart (size represents frequency)
dotnet run transcript.txt --chart bubble

# Short form
dotnet run transcript.txt -c radial
```

### Interactive HTML with Slider Controls
```bash
# Generate interactive HTML with dynamic filtering
dotnet run transcript.txt --output-file report.htmlsld

# Interactive report with radial chart and ocean theme
dotnet run transcript.txt -o interactive.htmlsld --chart radial --theme ocean

# Interactive bubble chart with dark theme
dotnet run transcript.txt -o analysis.htmlsld --chart bubble --theme dark
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
# Custom threshold with specific chart type and theme
dotnet run transcript.txt -t 8 -c pie --theme dark

# Interactive report with all options
dotnet run transcript.txt -t 5 -o report.htmlsld -c bubble --theme forest

# Static HTML with ocean theme and radial chart
dotnet run transcript.txt --theme ocean --chart radial

# Full command with all options
dotnet run transcript.txt --min-count-threshold 10 --chart bar --theme default --output-file analysis.json
```

## Output Examples

### HTML Report Features
- 📊 Interactive charts using Chart.js (bar, pie, line, radial, bubble)
- 🎨 Four beautiful themes: Default, Dark, Ocean, and Forest
- 🎛️ **Interactive Mode (.htmlsld)**: Real-time filtering with slider controls
  - Adjust minimum word frequency threshold
  - Control maximum number of words displayed
  - Live chart updates as you adjust filters
- 📈 Statistics overview (total words, unique words, key terms)
- 📝 AI-generated summary and sentiment analysis
- 📱 Responsive design for mobile and desktop

### Interactive Features (.htmlsld files)
- **Frequency Slider**: Filter words by minimum occurrence count
- **Word Limit Slider**: Control how many top words to display (5-50)
- **Live Updates**: Charts and word lists update automatically
- **Word List View**: Sortable list showing all filtered words with counts

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
| `--chart` | `-c` | Chart type: bar, pie, line, radial, bubble | bar |
| `--theme` | `--th` | HTML theme: default, dark, ocean, forest | default |
| `--output-file` | `-o` | Export to file (.htmlsld for interactive) | None |

## Examples from Development

These commands were used during development and testing:

```bash
# Original basic command
dotnet run transcript.txt -t 5

# Test different themes
dotnet run transcript.txt --theme default
dotnet run transcript.txt --theme dark
dotnet run transcript.txt --theme ocean
dotnet run transcript.txt --theme forest

# Test new chart types
dotnet run transcript.txt -c radial --theme ocean
dotnet run transcript.txt -c bubble --theme dark

# Test interactive HTML with sliders
dotnet run transcript.txt -o interactive.htmlsld
dotnet run transcript.txt -o report.htmlsld -c radial --theme forest
dotnet run transcript.txt -o analysis.htmlsld -c bubble --theme dark

# Combined theme and chart testing
dotnet run transcript.txt -t 3 -c radial --theme ocean
dotnet run transcript.txt -c bubble --theme forest -o report.htmlsld

# Advanced combinations
dotnet run transcript.txt -t 5 --chart radial --theme dark -o interactive.htmlsld
```

## Theme Preview

- **Default**: Clean light theme with blue accents
- **Dark**: Dark background with bright, vibrant colors  
- **Ocean**: Calming blues and teals inspired by the sea
- **Forest**: Natural greens and earth tones

## Technical Notes

The application uses:
- **.NET 9.0** - Latest .NET runtime
- **System.CommandLine** - Modern CLI argument parsing
- **OpenAI API** - GPT-4 for text analysis
- **Chart.js** - Interactive web charts
- **YamlDotNet** - YAML serialization
- **DotNetEnv** - Environment variable management
