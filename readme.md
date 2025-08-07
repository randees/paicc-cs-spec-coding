## Example cmds to execute
dotnet run transcript.txt -t 5

## I also had to use the following prompts to fix issues after the initial spec build

Go through the program and add all the using statements for c# elements like List<> and Task<> and Dictionary<,>

add a using System; to the start of each .cs file

update word_counter.cs add the proper context to the StringSplitOptions namespace it needs to be more specific or add proper using statement at the top of the file.
update llm.cs add the proper context for the Dictionary data structure namespace it needs to be more specific or add proper using statement at the top of the file.
