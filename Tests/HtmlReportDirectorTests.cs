using System;
using System.Collections.Generic;
using Xunit;
using TranscriptAnalytics;

namespace TranscriptAnalytics.Tests;

/// <summary>
/// Tests for HtmlReportDirector functionality
/// </summary>
public class HtmlReportDirectorTests
{
    private readonly Dictionary<string, int> _testWordCounts;
    private readonly string _testAnalysis;
    private readonly HtmlReportDirector _director;

    public HtmlReportDirectorTests()
    {
        _testWordCounts = new Dictionary<string, int>
        {
            { "test", 50 },
            { "word", 30 },
            { "frequency", 20 },
            { "analysis", 15 },
            { "report", 10 }
        };
        _testAnalysis = "This is a test analysis with insights about word frequency patterns.";
        _director = new HtmlReportDirector();
    }

    /// <summary>
    /// Tests basic HTML report construction with default theme
    /// </summary>
    [Fact]
    public void ConstructReport_WithDefaultTheme_ShouldGenerateValidHtml()
    {
        // Act
        string result = _director.ConstructReport(_testAnalysis, _testWordCounts, "bar", "default");

        // Assert
        Assert.Contains("<!DOCTYPE html>", result);
        Assert.Contains("Transcript Analysis Report", result);
        Assert.Contains("Default Theme", result);
        Assert.Contains("test", result); // Should contain test data
        Assert.Contains("Chart.js", result); // Should include Chart.js
    }

    /// <summary>
    /// Tests HTML report with slider functionality
    /// </summary>
    [Fact]
    public void ConstructReportWithSlider_ShouldGenerateInteractiveHtml()
    {
        // Act
        string result = _director.ConstructReportWithSlider(_testAnalysis, _testWordCounts, "bar", "default");

        // Assert
        Assert.Contains("Interactive Transcript Analysis", result);
        Assert.Contains("slider", result);
        Assert.Contains("updateChart()", result);
        Assert.Contains("minFrequency", result);
        Assert.Contains("maxWords", result);
        Assert.Contains("Filter Controls", result);
    }

    /// <summary>
    /// Tests radial chart generation
    /// </summary>
    [Fact]
    public void ConstructReportWithSlider_WithRadialChart_ShouldGenerateRadialVisualization()
    {
        // Act
        string result = _director.ConstructReportWithSlider(_testAnalysis, _testWordCounts, "radial", "default");

        // Assert
        Assert.Contains("polarArea", result);
        Assert.Contains("createRadialBarChart", result);
        Assert.Contains("Word Frequency - Radial View", result);
    }

    /// <summary>
    /// Tests bubble chart generation
    /// </summary>
    [Fact]
    public void ConstructReportWithSlider_WithBubbleChart_ShouldGenerateBubbleVisualization()
    {
        // Act
        string result = _director.ConstructReportWithSlider(_testAnalysis, _testWordCounts, "bubble", "dark");

        // Assert
        Assert.Contains("bubble", result);
        Assert.Contains("createBubbleChart", result);
        Assert.Contains("Word Frequency - Bubble View", result);
        Assert.Contains("Math.sqrt", result); // Bubble size calculation
    }

    /// <summary>
    /// Tests all available themes
    /// </summary>
    [Theory]
    [InlineData("default")]
    [InlineData("dark")]
    [InlineData("ocean")]
    [InlineData("forest")]
    public void ConstructReport_WithDifferentThemes_ShouldApplyCorrectTheme(string theme)
    {
        // Act
        string result = _director.ConstructReport(_testAnalysis, _testWordCounts, "bar", theme);

        // Assert
        Assert.Contains($"{theme.Substring(0, 1).ToUpper()}{theme.Substring(1)} Theme", result);
        Assert.Contains("--primary-color:", result);
        Assert.Contains("--background-color:", result);
    }

    /// <summary>
    /// Tests chart type handling in slider version
    /// </summary>
    [Theory]
    [InlineData("bar", "createStandardChart")]
    [InlineData("pie", "createStandardChart")]
    [InlineData("line", "createStandardChart")]
    [InlineData("radial", "createRadialBarChart")]
    [InlineData("bubble", "createBubbleChart")]
    public void ConstructReportWithSlider_WithDifferentChartTypes_ShouldGenerateCorrectChart(string chartType, string expectedFunction)
    {
        // Act
        string result = _director.ConstructReportWithSlider(_testAnalysis, _testWordCounts, chartType, "default");

        // Assert
        Assert.Contains(expectedFunction, result);
    }

    /// <summary>
    /// Tests invalid theme fallback
    /// </summary>
    [Fact]
    public void ConstructReport_WithInvalidTheme_ShouldFallbackToDefault()
    {
        // Act
        string result = _director.ConstructReport(_testAnalysis, _testWordCounts, "bar", "invalid-theme");

        // Assert
        Assert.Contains("Default Theme", result);
    }

    /// <summary>
    /// Tests word data serialization in slider version
    /// </summary>
    [Fact]
    public void ConstructReportWithSlider_ShouldSerializeWordDataCorrectly()
    {
        // Act
        string result = _director.ConstructReportWithSlider(_testAnalysis, _testWordCounts, "bar", "default");

        // Assert
        Assert.Contains("allWordData", result);
        Assert.Contains("\"word\":", result);
        Assert.Contains("\"count\":", result);
        foreach (var word in _testWordCounts.Keys)
        {
            Assert.Contains($"\"{word}\"", result);
        }
    }

    /// <summary>
    /// Tests available themes retrieval
    /// </summary>
    [Fact]
    public void GetAvailableThemes_ShouldReturnAllThemes()
    {
        // Act
        string[] themes = HtmlReportDirector.GetAvailableThemes();

        // Assert
        Assert.Contains("default", themes);
        Assert.Contains("dark", themes);
        Assert.Contains("ocean", themes);
        Assert.Contains("forest", themes);
        Assert.True(themes.Length >= 4);
    }
}
