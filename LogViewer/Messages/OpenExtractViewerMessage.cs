using LogViewer.Core;

namespace LogViewer.Messages;

/// <summary>
/// Notify the view models that the extract viewer should be opened
/// </summary>
public class OpenExtractViewerMessage
{
    /// <summary>
    /// The title displayed by the extract viewer
    /// </summary>
    public string Title { get; }
    
    /// <summary>
    /// The list of extracts to display
    /// </summary>
    public LogExtract[] Extracts { get; }

    /// <summary>
    /// Create a new instance of the OpenExtractViewerMessage class
    /// </summary>
    /// <param name="title">The title displayed by the extract viewer</param>
    /// <param name="extracts">The list of extracts to display</param>
    public OpenExtractViewerMessage(string title, LogExtract[] extracts)
    {
        Title = title;
        Extracts = extracts;
    }
}