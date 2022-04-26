using System.Text.RegularExpressions;

namespace LogViewer.Core;

/// <summary>
/// Analyze the log file and extract useful information
/// </summary>
public static class LogAnalyzer
{
    /// <summary>
    /// Analyze the log file using a custom set of rules
    /// </summary>
    /// <param name="logFile">The log file to analyze</param>
    /// <param name="rules">The custom rules to follow while analyzing the log file</param>
    /// <returns>An analysis of the provided log file</returns>
    public static LogAnalysis Analyze(LogFile logFile, LogAnalyzerRules rules)
    {
        // Extract the version number
        var version = ExtractValue(logFile, rules.VersionIdentifierStart, rules.VersionIdentifierEnd);
            
        // Replace the template values now that we have the version number
        rules.ReplaceTemplateValues(version);

        // Perform the analysis
        return new LogAnalysis
        {
            TimestampStart = ExtractTimestampFirst(logFile.GetContent(), rules.TimestampFormat),
            TimestampEnd = ExtractTimestampLast(logFile.GetContent(), rules.TimestampFormat),
            Version = version,
            Closed = ExtractLog(logFile, rules.AdditionalExitLines, rules.TimestampFormat, rules.ExitIdentifier),
            Restarts = ExtractLog(logFile, rules.AdditionalRestartLines, rules.TimestampFormat, rules.RestartIdentifier),
            Exceptions = ExtractLog(logFile, rules.AdditionalExceptionLines, rules.TimestampFormat, rules.ExceptionIdentifier),
            Crashes = ExtractLog(logFile, rules.AdditionalCrashLines, rules.TimestampFormat, rules.CrashIdentifier),
            Extract = PerformCustomExtraction(logFile, rules),
            Count = PerformCustomCount(logFile, rules)
        };
    }

    /// <summary>
    /// Extract a string value from the log file
    /// </summary>
    /// <param name="logFile">The log file</param>
    /// <param name="identifierStart">The start of the identifier used to extract the value</param>
    /// <param name="identifierEnd">The end of the identifier used to extract the value</param>
    /// <returns>The string value that was extracted from the provided log file</returns>
    private static string ExtractValue(LogFile logFile, string identifierStart, string identifierEnd)
    {
        // Get the start index
        var start = logFile.GetContent().LastIndexOf(identifierStart, StringComparison.Ordinal);
            
        // Check if we have a start index
        if (start == -1)
        {
            // No start index - No value can be extracted
            return "Unknown";
        }

        // Exclude the identifierStart value from the final extracted value
        start += identifierStart.Length;
            
        // Get the end index
        var end = logFile.GetContent().IndexOf(identifierEnd, start, StringComparison.Ordinal);
            
        // Extract the value
        return end == -1
            ? logFile.GetContent()[start..].Trim()
            : logFile.GetContent().Substring(start, end - start).Trim();
    }

    /// <summary>
    /// Count a string in the log file
    /// </summary>
    /// <param name="logFile">The log file</param>
    /// <param name="identifier">The substring that needs to be counted</param>
    /// <returns>The amount of times the substring was detected in the log file</returns>
    private static int Count(LogFile logFile, string identifier) =>
        Regex.Matches(logFile.GetContent(), identifier).Count;

    /// <summary>
    /// Extract the first timestamp from the specified string
    /// </summary>
    /// <param name="content">The string containing the timestamp(s)</param>
    /// <param name="formats">The allowed timestamp formats</param>
    /// <returns>The first timestamp in the specified string</returns>
    private static string ExtractTimestampFirst(string content, IEnumerable<string> formats)
    {
        // Loop through all allowed formats
        foreach (var format in formats)
        {
            // Try to extract the first timestamp
            var timestamp = Regex.Matches(content, format).FirstOrDefault()?.Value ?? string.Empty;
                
            // Check if we have a timestamp
            if (!string.IsNullOrEmpty(timestamp))
            {
                // Return the timestamp
                return timestamp;
            }
        }

        // No timestamp found
        return string.Empty;
    }

    /// <summary>
    /// Extract the last timestamp from the specified string
    /// </summary>
    /// <param name="content">The string containing the timestamp(s)</param>
    /// <param name="formats">The allowed timestamp formats</param>
    /// <returns>The last timestamp in the specified string</returns>
    private static string ExtractTimestampLast(string content, string[] formats)
    {
        // Loop through all allowed formats
        foreach (var format in formats)
        {
            // Try to extract the last timestamp
            var timestamp = Regex.Matches(content, format).LastOrDefault()?.Value ?? string.Empty;
                
            // Check if we have a timestamp
            if (!string.IsNullOrEmpty(timestamp))
            {
                // Return the timestamp
                return timestamp;
            }
        }

        // No timestamp found
        return string.Empty;
    }

    /// <summary>
    /// Extract an array of strings from the log file
    /// </summary>
    /// <param name="logFile">The log file</param>
    /// <param name="rules">The rules used to identify the string to extract</param>
    /// <returns>The strings that was extracted from the log file</returns>
    private static LogAnalysisExtract[] PerformCustomExtraction(LogFile logFile, LogAnalyzerRules rules) =>
        rules.Extract.Select(extractRule => new LogAnalysisExtract
        {
            Name = extractRule.Name,
            Value = ExtractValue(logFile, extractRule.IdentifierStart, extractRule.IdentifierEnd)
        }).ToArray();

    /// <summary>
    /// Count substrings in the log file
    /// </summary>
    /// <param name="logFile">The log file</param>
    /// <param name="rules">The rules used to identify the substrings to count</param>
    /// <returns>The result from counting substrings in the log file</returns>
    private static LogAnalysisCount[] PerformCustomCount(LogFile logFile, LogAnalyzerRules rules) =>
        rules.Count.Select(countRule => new LogAnalysisCount
        {
            Name = countRule.Name,
            Amount = Count(logFile, countRule.Identifier)
        }).ToArray();

    /// <summary>
    /// Extract part of the log file based on the specified rules
    /// </summary>
    /// <param name="logFile">The log file</param>
    /// <param name="additionalLines">Additional lines to include before the extract</param>
    /// <param name="timestampFormats">The allowed timestamp</param>
    /// <param name="identifier">The identifier used to extract the content</param>
    /// <returns>An array of log extracts</returns>
    private static LogExtract[] ExtractLog(LogFile logFile, int additionalLines, string[] timestampFormats, string identifier)
    {
        // Initialize the required variables
        var entryStarted = false;
        var lines = logFile.GetContent().Split(Environment.NewLine);
        var extracts = new List<LogExtract>();

        // Loop through all lines in the log file
        for (var i = 0; i < lines.Length; i++)
        {
            // Get the line at the current index
            var line = lines[i];
                
            // Check if we're busy extracting the current entry
            if (entryStarted)
            {
                // Extract the timestamp
                var timestamp = ExtractTimestampFirst(line, timestampFormats);
                    
                // Check if we've found a timestamp
                if (string.IsNullOrEmpty(timestamp))
                {
                    // No timestamp - continue adding to the extract
                    extracts.Last().Details = $"{extracts.Last().Details}{Environment.NewLine}{line}";
                    continue;
                }

                // Next entry in the log file - stop extracting content
                var finalTimestamp = ExtractTimestampLast(extracts.Last().Details, timestampFormats);
                if (string.IsNullOrEmpty(finalTimestamp))
                {
                    finalTimestamp = timestamp;
                }

                extracts.Last().Timestamp = finalTimestamp;
                entryStarted = false;
                continue;
            }

            // Check if we've found something that we need to extract
            if (!Regex.Matches(line, identifier).Any())
            {
                continue;
            }

            // Extract the previous n lines
            var firstLine = string.Empty;
            for (var j = Math.Max(i - additionalLines, 0); j < i; j++)
            {
                firstLine += $"{lines[j]}{Environment.NewLine}";
            }

            // Add the current line to the extracts
            entryStarted = true;
            extracts.Add(new LogExtract { Details = firstLine + line });
        }

        // Return all extracts
        return extracts.ToArray();
    }
}