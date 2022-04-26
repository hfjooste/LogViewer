using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace LogViewer.ViewModels;

/// <summary>
/// Manages all the view models in the application
/// </summary>
public class ViewModelLocator
{
    /// <summary>
    /// Register all the view models
    /// </summary>
    public static void RegisterAll()
    {
        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddTransient<AboutViewModel>()
            .AddTransient<AnalysisViewModel>()
            .AddTransient<ExtractViewModel>()
            .AddTransient<RecentsViewModel>()
            .AddTransient<SettingsViewModel>()
            .BuildServiceProvider());
    }

    /// <summary>
    /// Get a reference to the AboutViewModel
    /// </summary>
    public AboutViewModel AboutViewModel => Ioc.Default.GetService<AboutViewModel>()!;
    
    /// <summary>
    /// Get a reference to the AnalysisViewModel
    /// </summary>
    public AnalysisViewModel AnalysisViewModel => Ioc.Default.GetService<AnalysisViewModel>()!;
    
    /// <summary>
    /// Get a reference to the ExtractViewModel
    /// </summary>
    public ExtractViewModel ExtractViewModel => Ioc.Default.GetService<ExtractViewModel>()!;
    
    /// <summary>
    /// Get a reference to the RecentsViewModel
    /// </summary>
    public RecentsViewModel RecentsViewModel => Ioc.Default.GetService<RecentsViewModel>()!;
    
    /// <summary>
    /// Get a reference to the SettingsViewModel
    /// </summary>
    public SettingsViewModel SettingsViewModel => Ioc.Default.GetService<SettingsViewModel>()!;
}