# HTML Slider Output and Charts Feature

## High-Level Objective

- Create an interactive HTML output format with dynamic word frequency visualization

## Mid-Level Objective

- Build new HTML output format with slider-based word frequency filtering
- Add radial bar and bubble chart visualizations for word frequencies
- Extend CLI to support new output formats
- Ensure comprehensive test coverage

## Implementation Notes

- Comment every new function
- For CLI commands add usage examples
- Follow existing code patterns and type safety practices
- Add tests for all new functionality

## Context

### Beginning context

Everything but the FormatAdder folder

### Ending context

Everything but the FormatAdder folder

## Low-Level Tasks

> Ordered from start to finish

1. Add new HTML output format with slider
UPDATE HTML.cs
    CREATE formatAsHtmlWithSliderFilter() function:
        Add HTML template with slider control
        Add JavaScript for dynamic filtering
        MIRROR formatAsHtml()

2. Add new chart visualizations
UPDATE HTML.cs
    CREATE createRadialBarChart(word_counts: WordCounts), createBubbleChart(...)

3. Update CLI interface
UPDATE program.cs:
    ADD support for checking .htmlsld extension and calling formatAsHtmlWithSliderFilter()
        Be sure to use .html when saving the file, .htmlsld is just for checking
    ADD support for 'radial' and 'bubble' choices and calling respective chart functions


4. Add comprehensive tests

UPDATE test files:
    ADD formatAsHtmlWithSliderFilter()
    ADD createRadialBarChart()
    ADD createBubbleChart()

