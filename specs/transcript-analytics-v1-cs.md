# Transcript Analytics Updates: Chart and Output File

> Ingest the information from this file, implement the Low-Level Tasks, and generate the code that will satisfy the High and Mid-Level Objectives.

## High-Level Objective

- Add charting and output file functionality to the CLI transcirp analytics application

## Mid-Level Objective

- Change the output from a console app to a webpage so we can graphically represent the data.
- Add a bar and pie chart in a new class which will handle the html output
- Add output file functionality to a new output.cs
- Add two new cli args: '--chart' and '--output-file' which controls what is displayed and where the data comes from

## Implementation Notes

- UPDATE the project with the ability to display web pages which displays the information in a chart
- Chart types include bar, pie, line.
- Create a new file and class for building the web page htlm to be displayed
- Create a new file and class for outputting the data to a file
- Output file options are: .txt, .json, .md, .yaml. Mare sure to use the proper file extension for each type.
- Use the Low-Level tasks for details on implementation.

## Context

### Beginning context

- program.cs
- llm.cs
- readme.md
- TranscriptAnalytics.csproj

### Ending context  

- program.cs
- llm.cs
- readme.md
- TranscriptAnalytics.csproj
- HTML.cs
- FileOutput.cs

## Low-Level Tasks

1. MODIFY the solution
    UPDATE so that it returns an html response
2. CREATE FileOutput.cs
    CREATE a method format_as_string(TranscriptAnalysis, WordCounts) -> string, that outputs the data to a file. the filetype is the --output-file cli argument,
        Example formating function names are:
        formatAsJson(...),
        formatAsMd(...),
        formatAsYaml(...)
3. CREATE HTML.cs
    CREATE a method that takes the data from the Task AnalyzeTranscript formats it for an HTML response and creates a graphical chart based on the cli arg --chart DEFAULT bar graph,
        CREATE seperate methods to make the wordcount graphical,
            createBarChart(WordCounts) -> wordcount as an html bar graph, order desc top to bottom, top quartile green, bottom quartile red, rest blue
            createPieChart(WordCounts) -> wordccount as an html pie graph,
            createLineChart(WordCounts) -> wordcount as html line graph

## Allowances

- If you need to download a library to make generating the html graphs easier you can do so.

## Final instructions

- comment all functions, provide examples of command lines to run in the readme.md
