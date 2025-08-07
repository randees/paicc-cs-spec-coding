using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TranscriptAnalytics;

/// <summary>
/// Handles word counting and filtering functionality
/// </summary>
public static class WordCounter
{
    /// <summary>
    /// Counts words in a text file, filters out common words and applies minimum count threshold
    /// </summary>
    /// <param name="filePath">Path to the text file to analyze</param>
    /// <param name="minCountThreshold">Minimum count threshold for words to be included</param>
    /// <returns>Dictionary of words and their counts, sorted by count descending</returns>
    public static Dictionary<string, int> CountWords(string filePath, int minCountThreshold = 10)
    {
        // Read the file content
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        string content = File.ReadAllText(filePath);
        
        // Remove punctuation and convert to lowercase
        string cleanedContent = Regex.Replace(content, @"[^\w\s]", " ");
        string[] words = cleanedContent.ToLower().Split(new char[] { ' ', '\n', '\r', '\t' }, 
            System.StringSplitOptions.RemoveEmptyEntries);

        // Count word frequencies
        var wordCounts = new Dictionary<string, int>();
        
        foreach (string word in words)
        {
            // Skip if word is in blacklist
            if (Constants.COMMON_WORDS_BLACKLIST.Contains(word))
                continue;
                
            // Skip very short words
            if (word.Length <= 2)
                continue;

            if (wordCounts.ContainsKey(word))
            {
                wordCounts[word]++;
            }
            else
            {
                wordCounts[word] = 1;
            }
        }

        // Filter by minimum count threshold and sort descending
        var filteredWords = wordCounts
            .Where(kvp => kvp.Value >= minCountThreshold)
            .OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return filteredWords;
    }
}
