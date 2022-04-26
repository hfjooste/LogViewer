using System.Diagnostics;
using System.Reflection;
using LogViewer.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace LogViewer.ViewModels;

/// <summary>
/// View Model for the About popup
/// </summary>
public class AboutViewModel : ObservableObject
{
    /// <summary>
    /// Used to show/hide the about popup
    /// </summary>
    private bool _showPopup;
    
    /// <summary>
    /// The current application version
    /// </summary>
    private string _version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? string.Empty;

    /// <summary>
    /// Create a new instance of the AboutViewModel class
    /// </summary>
    public AboutViewModel()
    {
        // Initialize the RelayCommands
        SourceCodeCommand = new RelayCommand(OnSourceCodeCommandExecuted);
        CloseCommand = new RelayCommand(OnCloseCommandExecuted);
        
        // Register the messenger
        WeakReferenceMessenger.Default.Register<OpenAboutPopupMessage>(this, OnOpenAboutPopupMessageReceived);
    }

    /// <summary>
    /// Executed after clicking on the Source Code button
    /// </summary>
    public RelayCommand SourceCodeCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    public RelayCommand CloseCommand { get; }
    
    /// <summary>
    /// Get/Set the value used to show/hide the about popup
    /// </summary>
    public bool ShowPopup
    {
        get => _showPopup;
        set => SetProperty(ref _showPopup, value);
    }
    
    /// <summary>
    /// Get/Set the current application version
    /// </summary>
    public string Version
    {
        get => _version;
        set => SetProperty(ref _version, value);
    }

    /// <summary>
    /// Executed after clicking on the Source Code button
    /// </summary>
    private void OnSourceCodeCommandExecuted()
    {
        // Open the GitHub project link
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/hfjooste/LogViewer",
            UseShellExecute = true
        });
    }
    
    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    private void OnCloseCommandExecuted() => ShowPopup = false;

    /// <summary>
    /// Executed after receiving an OpenAboutPopupMessage
    /// </summary>
    /// <param name="recipient">The receiving object</param>
    /// <param name="message">The message</param>
    private void OnOpenAboutPopupMessageReceived(object recipient, OpenAboutPopupMessage message) => ShowPopup = true;
}