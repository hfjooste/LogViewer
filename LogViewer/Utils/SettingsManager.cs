using System.Collections.Specialized;
using System.Configuration;
using LogViewer.Properties;

namespace LogViewer.Utils;

/// <summary>
/// Load/Save settings values
/// </summary>
public static class SettingsManager
{
    /// <summary>
    /// Initialize the settings manager by loading data from the settings file
    /// </summary>
    public static void Init()
    {
        // Load the settings
        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        Settings.Default.Reload();

        // Check if we should perform an upgrade
        if (!Settings.Default.UpgradeRequired)
        {
            return;
        }
        
        // Upgrade the settings file
        Settings.Default.Upgrade();
        Settings.Default.UpgradeRequired = false;
        Settings.Default.Save();
    }

    /// <summary>
    /// Get/Set the rules file path
    /// </summary>
    public static string RulesPath
    {
        get => Settings.Default.RulesPath;
        set
        {
            Settings.Default.RulesPath = value;
            Settings.Default.Save();
        }
    }

    /// <summary>
    /// Get/Set the list of recent log file paths
    /// </summary>
    public static StringCollection Recents
    {
        get => Settings.Default.Recents ?? new StringCollection();
        private set
        {
            Settings.Default.Recents = value;
            Settings.Default.Save();
        }
    }

    /// <summary>
    /// Add a log file path to the list of recents
    /// </summary>
    /// <param name="entry"></param>
    public static void AddToRecents(string entry)
    {
        // Get the list of recents
        var currentList = Recents;
        
        // Check if the entry is already in the list
        if (currentList.Contains(entry))
        {
            // Remove the old entry
            currentList.Remove(entry);
        }
        
        // Add the entry to the top
        currentList.Insert(0, entry);
        
        // Check if we've exceeded the max amount of entries
        if (currentList.Count <= 5)
        {
            // Still a valid list - Save the changes
            Recents = currentList;
            return;
        }
        
        // Remove the oldest entry
        currentList.RemoveAt(Recents.Count - 1);
        
        // Save the changes
        Recents = currentList;
    }
}