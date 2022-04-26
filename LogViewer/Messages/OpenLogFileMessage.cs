namespace LogViewer.Messages;

/// <summary>
/// Notify the view models that a specific log file should opened
/// </summary>
public class OpenLogFileMessage
{
    /// <summary>
    /// The file path of the log file
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Create a new instance of the OpenLogFileMessage class
    /// </summary>
    /// <param name="filePath">The file path of the log file</param>
    public OpenLogFileMessage(string filePath) => FilePath = filePath;
}