using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TranscriptAnalytics;

/// <summary>
/// Analysis result structure for structured output
/// </summary>
public class TranscriptAnalysis
{
    [System.Text.Json.Serialization.JsonPropertyName("Summary")]
    public string Summary { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("ImportantWords")]
    public List<string> ImportantWords { get; set; } = new List<string>();

    [System.Text.Json.Serialization.JsonPropertyName("SentimentAnalysis")]
    public string SentimentAnalysis { get; set; } = string.Empty;
}

/// <summary>
/// Handles LLM-based analysis of transcripts using OpenAI
/// </summary>
public static class LLMAnalyzer
{
    /// <summary>
    /// Analyzes transcript text using OpenAI chat completion with structured output
    /// </summary>
    /// <param name="transcriptText">The transcript text to analyze</param>
    /// <param name="wordCounts">Dictionary of word counts from the transcript</param>
    /// <returns>Structured analysis of the transcript</returns>
    public static async Task<TranscriptAnalysis> AnalyzeTranscript(string transcriptText, Dictionary<string, int> wordCounts)
    {
        // Get OpenAI configuration from environment
        string apiKey = EnvironmentConfig.GetOpenAIApiKey();
        string apiBaseUrl = EnvironmentConfig.GetOpenAIApiBaseUrl();
        string model = EnvironmentConfig.GetOpenAIModel();
        int maxLength = EnvironmentConfig.GetMaxTranscriptLength();

        // Create HTTP client for OpenAI API
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Prepare word counts summary for analysis
        var topWords = wordCounts.Take(20).Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList();
        string wordCountsSummary = string.Join(", ", topWords);

        // Create the analysis prompt
        string prompt = $@"Analyze the following transcript and provide a structured analysis:

Transcript: {transcriptText.Substring(0, Math.Min(transcriptText.Length, maxLength))}...

Top word frequencies: {wordCountsSummary}

Please provide:
1. A concise summary of the main topics discussed
2. A list of important words that represent key concepts (bullet points)
3. A sentiment analysis of the overall tone

Format your response as JSON with the following structure:
{{
    ""Summary"": ""Your summary here"",
    ""ImportantWords"": [""word1"", ""word2"", ""word3""],
    ""SentimentAnalysis"": ""Your sentiment analysis here""
}}";

        // Prepare the OpenAI API request
        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You are an expert transcript analyzer. Provide structured analysis in JSON format." },
                new { role = "user", content = prompt }
            },
            temperature = 0.3,
            max_tokens = 1000
        };

        string jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        try
        {
            // Make the API call
            var response = await httpClient.PostAsync($"{apiBaseUrl}/chat/completions", content);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent);
            
            string analysisText = responseJson.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";

            // Parse the JSON response from the LLM
            var analysis = JsonSerializer.Deserialize<TranscriptAnalysis>(analysisText);
            return analysis ?? new TranscriptAnalysis();
        }
        catch (Exception ex)
        {
            // Return a fallback analysis if the API call fails
            return new TranscriptAnalysis
            {
                Summary = $"Analysis failed: {ex.Message}",
                ImportantWords = wordCounts.Take(10).Select(kvp => kvp.Key).ToList(),
                SentimentAnalysis = "Unable to determine sentiment due to API error"
            };
        }
    }
}
