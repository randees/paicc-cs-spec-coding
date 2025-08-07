using System;

namespace TranscriptAnalytics;

/// <summary>
/// Contains constant values used throughout the application
/// </summary>
public static class Constants
{
    /// <summary>
    /// Common words to filter out from word frequency analysis
    /// </summary>
    public static readonly string[] COMMON_WORDS_BLACKLIST = new string[]
    {
        "the", "and", "to", "of", "a", "in", "for", "is", "on", "that", "by", "this", "with", "i", "you", "it", "not", "or", "be", "are", "from", "at", "as", "your", "all", "any", "can", "had", "her", "was", "one", "our", "out", "day", "get", "has", "him", "his", "how", "man", "new", "now", "old", "see", "two", "way", "who", "boy", "did", "its", "let", "put", "say", "she", "too", "use"
    };
}
