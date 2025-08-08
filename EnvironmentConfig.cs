using System;
using System.IO;
using DotNetEnv;

namespace TranscriptAnalytics;

/// <summary>
/// Handles environment variable configuration for the application
/// </summary>
public static class EnvironmentConfig
{
    /// <summary>
    /// Loads environment variables from .env file and system environment
    /// </summary>
    public static void LoadEnvironmentVariables()
    {
        try
        {
            // Load from .env file if it exists
            if (File.Exists(".env"))
            {
                Env.Load(".env");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not load .env file: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the OpenAI API key from environment variables
    /// </summary>
    /// <returns>OpenAI API key</returns>
    /// <exception cref="InvalidOperationException">Thrown when API key is not found</exception>
    public static string GetOpenAIApiKey()
    {
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException(
                "OPENAI_API_KEY environment variable is not set. " +
                "Please set it in your .env file or system environment variables.");
        }
        return apiKey;
    }

    /// <summary>
    /// Gets the OpenAI API base URL from environment variables
    /// </summary>
    /// <returns>OpenAI API base URL or default</returns>
    public static string GetOpenAIApiBaseUrl()
    {
        return Environment.GetEnvironmentVariable("OPENAI_API_BASE_URL") ?? "https://api.openai.com/v1";
    }

    /// <summary>
    /// Gets the OpenAI model from environment variables
    /// </summary>
    /// <returns>OpenAI model name or default</returns>
    public static string GetOpenAIModel()
    {
        return Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4";
    }

    /// <summary>
    /// Gets the minimum word count threshold from environment variables
    /// </summary>
    /// <returns>Minimum word count threshold or default</returns>
    public static int GetMinWordCountThreshold()
    {
        string? threshold = Environment.GetEnvironmentVariable("MIN_WORD_COUNT_THRESHOLD");
        return int.TryParse(threshold, out int result) ? result : 10;
    }

    /// <summary>
    /// Gets the maximum transcript length from environment variables
    /// </summary>
    /// <returns>Maximum transcript length or default</returns>
    public static int GetMaxTranscriptLength()
    {
        string? length = Environment.GetEnvironmentVariable("MAX_TRANSCRIPT_LENGTH");
        return int.TryParse(length, out int result) ? result : 3000;
    }
}
