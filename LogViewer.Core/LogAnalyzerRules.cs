using Newtonsoft.Json;

namespace LogViewer.Core;

/// <summary>
/// A set of rules followed when analyzing the log file
/// </summary>
public class LogAnalyzerRules
{
    /// <summary>
    /// Private constructor. Create an instance by using the static Generate method
    /// </summary>
    private LogAnalyzerRules() { }

    /// <summary>
    /// Generate a new set of rules for the log analyzer
    /// </summary>
    /// <param name="filePath">The path to the rules file</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if a null/empty file path is provided</exception>
    /// <exception cref="FileNotFoundException">Thrown if the rules file doesn't exist</exception>
    /// <exception cref="JsonSerializationException">Thrown if the rules file isn't valid json</exception>
    public static LogAnalyzerRules Generate(string filePath)
    {
        // Check for valid input
        if (string.IsNullOrEmpty(filePath.Trim()))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        // Read the file and convert it to a LogAnalyzerRules object
        var fileContent = File.ReadAllText(filePath);
        var rules = JsonConvert.DeserializeObject<LogAnalyzerRules>(fileContent);
            
        // Ensure the object was created
        if (rules == null)
        {
            throw new JsonSerializationException();
        }            

        // Replace the template values in the rules object
        rules.ReplaceTemplateValues(string.Empty);
        return rules;
    }

    /// <summary>
    /// An array of all the possible timestamp formats used by the log file
    /// </summary>
    public string[] TimestampFormat { get; set; } = Array.Empty<string>();
        
    /// <summary>
    /// The start of the identifier used to extract the version number from the log file
    /// </summary>
    public string VersionIdentifierStart { get; set; } = string.Empty;
        
    /// <summary>
    /// The end of the identifier used to extract the version number from the log file
    /// </summary>
    public string VersionIdentifierEnd { get; set; } = string.Empty;
        
    /// <summary>
    /// The identifier used to extract the restart events from the log file
    /// </summary>
    public string RestartIdentifier { get; set; } = string.Empty;
        
    /// <summary>
    /// The identifier used to extract the exceptions from the log file
    /// </summary>
    public string ExceptionIdentifier { get; set; } = string.Empty;
        
    /// <summary>
    /// The identifier used to extract the exit events from the log file
    /// </summary>
    public string ExitIdentifier { get; set; } = string.Empty;
        
    /// <summary>
    /// The identifier used to extract the crashes from the log file
    /// </summary>
    public string CrashIdentifier { get; set; } = string.Empty;
        
    /// <summary>
    /// The amount of additional lines added to the start of a restart event extract
    /// </summary>
    public int AdditionalRestartLines { get; set; }
        
    /// <summary>
    /// The amount of additional lines added to the start of an exception event extract
    /// </summary>
    public int AdditionalExceptionLines { get; set; }
        
    /// <summary>
    /// The amount of additional lines added to the start of an exit event extract
    /// </summary>
    public int AdditionalExitLines { get; set; }
        
    /// <summary>
    /// The amount of additional lines added to the start of a crash event extract
    /// </summary>
    public int AdditionalCrashLines { get; set; }
        
    /// <summary>
    /// An array of custom values extracted from the log file
    /// </summary>
    public LogAnalyzerExtractRule[] Extract { get; set; } = Array.Empty<LogAnalyzerExtractRule>();
        
    /// <summary>
    /// An array of custom values counted in the log file
    /// </summary>
    public LogAnalyzerCountRule[] Count { get; set; } = Array.Empty<LogAnalyzerCountRule>();

    /// <summary>
    /// Replace the template values in the set of rules
    /// </summary>
    /// <param name="version">The version number extracted from the log file</param>
    public void ReplaceTemplateValues(string version)
    {
        // Replace the template values in the timestamp formats 
        for (var i = 0; i < TimestampFormat.Length; i++)
        {
            TimestampFormat[i] = ReplaceTemplateValues(TimestampFormat[i], version);
        }

        // Replace the template values in the identifiers
        VersionIdentifierStart = ReplaceTemplateValues(VersionIdentifierStart, version);
        VersionIdentifierEnd = ReplaceTemplateValues(VersionIdentifierEnd, version);
        RestartIdentifier = ReplaceTemplateValues(RestartIdentifier, version);
        ExceptionIdentifier = ReplaceTemplateValues(ExceptionIdentifier, version);
        ExitIdentifier = ReplaceTemplateValues(ExitIdentifier, version);
        CrashIdentifier = ReplaceTemplateValues(CrashIdentifier, version);

        // Replace the template values in the extract rules
        foreach (var extractRule in Extract)
        {
            extractRule.IdentifierStart = ReplaceTemplateValues(extractRule.IdentifierStart, version);
            extractRule.IdentifierEnd = ReplaceTemplateValues(extractRule.IdentifierEnd, version);
        }

        // Replace the template values in the count rules
        foreach (var countRule in Count)
        {
            countRule.Identifier = ReplaceTemplateValues(countRule.Identifier, version);
        }
    }

    /// <summary>
    /// Replace the template values in the set of rules
    /// </summary>
    /// <param name="input">The string that contains template values that needs to be replaced</param>
    /// <param name="version">The version number extracted from the log file</param>
    private string ReplaceTemplateValues(string input, string version)
    {
        // Replace identifier template values
        var formattedInput = input.Replace("$VersionIdentifierStart", VersionIdentifierStart)
            .Replace("$VersionIdentifierEnd", VersionIdentifierEnd)
            .Replace("$RestartIdentifier", RestartIdentifier)
            .Replace("$ExceptionIdentifier", ExceptionIdentifier)
            .Replace("$ExitIdentifier", ExitIdentifier)
            .Replace("$CrashIdentifier", CrashIdentifier);

        // Replace the version template value if possible
        return string.IsNullOrEmpty(version)
            ? formattedInput
            : formattedInput.Replace("$Version", version);
    }
}

/// <summary>
/// Custom value extraction rule used by the analyzer
/// </summary>
public class LogAnalyzerExtractRule
{
    /// <summary>
    /// A user friendly display name
    /// </summary>
    public string Name { get; set; } = string.Empty;
        
    /// <summary>
    /// The start of the identifier used to extract the value
    /// </summary>
    public string IdentifierStart { get; set; } = string.Empty;
        
    /// <summary>
    /// The end of the identifier used to extract the value
    /// </summary>
    public string IdentifierEnd { get; set; } = string.Empty;
}

/// <summary>
/// Custom count rule used by the analyzer
/// </summary>
public class LogAnalyzerCountRule
{
    /// <summary>
    /// A user friendly display name
    /// </summary>
    public string Name { get; set; } = string.Empty;
        
    /// <summary>
    /// The identifier used to perform the count operation
    /// </summary>
    public string Identifier { get; set; } = string.Empty;
}