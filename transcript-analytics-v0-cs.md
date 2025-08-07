# Transcript Analytics v0 Specification

## High-Level Objective

- Create a CLI transcript analytics application

## Mid-Level Objective

- Build a asp.net 9 c# MVP typer CLI application.
- Accept a path to a text file.
- Count the frequency of each word in a file, filter out common words, and limit by count threshold.
- Use an openai chat completion with structured output analyze the transcript and word counts.
- Rich print the frequency of each word to the terminal and the transcript analysis.

## Implementation Notes
- Comment every function.
- When code block is given in low-level tasks, use it without making changes (Task 4).
- Carefully review each low-level task for exact code changes.

## Context

### Beginning context
- `./program.cs`

### Ending context
- `./main.cs`
- `./llm.cs` (new file)
- `./word_counter.cs` (new file)
- `./constants.cs` (new file)

## Low-Level Tasks
> Ordered from start to finish

1. Create common word blacklist.
```
CREATE ./constants.cs: 
    CREATE COMMON_WORDS_BLACKLIST = ['the', 'and', ...add 50 more common words]
```

3. Create our word counter & filter out & limit by count threshold.
```
CREATE ./word_counter.cs:
    CREATE create a function CountWords() that will use the first command argument as the file name to count the instance of each word,
        Remove punctuation from script and make all words lowercase,
        Use the COMMON_WORDS_BLACKLIST to filter out common words,
        Only include words that are greater than the min_count_threshold.
        Sort descending by count.
```

4. Create our LLM function using the code block below.
```
# CREATE ./llm.cs:
    USING openai USING the env key OPENAI_API_KEY, i will provide the key after later.
        CREATE an analysis of the the text that has a summary, bullet points of import words, a sentiment_analysis.  
```

5. Update our main function to use new count and analysis functions.

```
UPDATE ./program.cs:
    CREATE a new typer cli application:
        CREATE function analyze_transcript(path_to_script_text_file, min_count_threshold: int = 10):
            Read file, count words, run analysis, rich print results,
            print words like '<word>: ###' where ### is count 3

            path_to_script_text_file will be commandline argument1,
            min_count_threshold will be commandline argument2
```