
using System.Windows;
using LogViewer.Utils;
using LogViewer.ViewModels;

namespace LogViewer;

public partial class App : Application
{
    /// <summary>
    /// Raises the Startup event
    /// </summary>
    /// <param name="e">A StartupEventArgs that contains the event data</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        // Handle the startup event
        base.OnStartup(e);
        
        // Register all the view models
        ViewModelLocator.RegisterAll();
        
        // Initialize the settings manager
        SettingsManager.Init();
    }
}