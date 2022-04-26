namespace LogViewer.Core;

/// <summary>
/// Contains the entire log file and allow you to read/manipulate the content
/// </summary>
public class LogFile
{
    /// <summary>
    /// The file path to the log file
    /// </summary>
    private readonly string _filePath;
        
    /// <summary>
    /// The content of the log file
    /// </summary>
    private string _content = string.Empty;

    /// <summary>
    /// Private constructor. Use the static Open method to create an instance
    /// </summary>
    /// <param name="filePath">The path to the log file</param>
    private LogFile(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Open a log file
    /// </summary>
    /// <param name="filePath">The path to the log file</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided file path is null/empty</exception>
    /// <exception cref="FileNotFoundException">Throw if the log file doesn't exist</exception>
    public static LogFile Open(string filePath)
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

        // Create the object
        return new LogFile(filePath);
    }

    /// <summary>
    /// Get the content of the log file
    /// </summary>
    /// <returns>The content of the log file</returns>
    public async Task<string> GetContentAsync()
    {
        if (!string.IsNullOrEmpty(_content))
        {
            return _content;
        }
            
        var lines = await File.ReadAllLinesAsync(_filePath);
        _content = string.Join(Environment.NewLine, lines);
        return _content;
    }

    /// <summary>
    /// Get the content of the log file
    /// </summary>
    /// <returns>The content of the log file</returns>
    public string GetContent()
    {
        if (!string.IsNullOrEmpty(_content))
        {
            return _content;
        }
            
        var lines = File.ReadAllLines(_filePath);
        _content = string.Join(Environment.NewLine, lines);
        return _content;
    }
}