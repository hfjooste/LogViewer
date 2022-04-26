namespace LogViewer.Helpers;

/// <summary>
/// Contains custom data that was extracted from the log file
/// </summary>
public class LogInfo
{
    /// <summary>
    /// User friendly display name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// The value extracted from the log file
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Create a new instance of the LogInfo class
    /// </summary>
    /// <param name="name">User friendly display name</param>
    /// <param name="amount">The amount extracted from the log file</param>
    public LogInfo(string name, int amount) : this(name, amount.ToString()) { }

    /// <summary>
    /// Create a new instance of the LogInfo class
    /// </summary>
    /// <param name="name">User friendly display name</param>
    /// <param name="value">The value extracted from the log file</param>
    public LogInfo(string name, string value)
    {
        Name = name;
        Value = value;
    }
}