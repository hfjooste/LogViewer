using LogViewer.Messages;
using LogViewer.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;

namespace LogViewer.ViewModels;

/// <summary>
/// View Model for the Settings popup
/// </summary>
public class SettingsViewModel : ObservableObject
{
    /// <summary>
    /// Used to show/hide the settings popup
    /// </summary>
    private bool _showPopup;
    
    /// <summary>
    /// The path to the selected rules file
    /// </summary>
    private string _rulesPath = string.Empty;

    /// <summary>
    /// Create a new instance of the SettingsViewModel class
    /// </summary>
    public SettingsViewModel()
    {
        // Initialize the RelayCommands
        ChangePathCommand = new RelayCommand(OnChangePathCommandExecuted);
        CloseCommand = new RelayCommand(OnCloseCommandExecuted);
        
        // Register the messenger
        WeakReferenceMessenger.Default.Register<OpenSettingsPopupMessage>(this, OnOpenSettingsPopupMessageReceived);
    }

    /// <summary>
    /// Executed after clicking on the Change Path (...) button
    /// </summary>
    public RelayCommand ChangePathCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    public RelayCommand CloseCommand { get; }
    
    /// <summary>
    /// Get/Set the value used to show/hide the settings popup
    /// </summary>
    public bool ShowPopup
    {
        get => _showPopup;
        set => SetProperty(ref _showPopup, value);
    }
    
    /// <summary>
    /// Get/Set the path to the selected rules file
    /// </summary>
    public string RulesPath
    {
        get => _rulesPath;
        set => SetProperty(ref _rulesPath, value);
    }

    /// <summary>
    /// Executed after clicking on the Change Path (...) button
    /// </summary>
    private void OnChangePathCommandExecuted()
    {
        // Display the OpenFileDialog
        var filter = "JSON Files (*.json)|*.json|All Files|*";
        var dialog = new OpenFileDialog { Filter = filter, Multiselect = false };
        var result = dialog.ShowDialog();
        
        // Check the selected file
        if (!result.HasValue || !result.Value)
        {
            return;
        }

        // Set the rules file
        RulesPath = dialog.FileName;
        
        // Save changes
        SettingsManager.RulesPath = RulesPath;
    }

    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    private void OnCloseCommandExecuted()
    {
        // Check if a rules file was selected
        if (string.IsNullOrEmpty(RulesPath))
        {
            // Don't allow the user to close settings without selecting a file
            return;
        }
        
        // Close the popup
        ShowPopup = false;
    }
    
    /// <summary>
    /// Executed after receiving an OpenSettingsPopupMessage
    /// </summary>
    /// <param name="recipient">The receiving object</param>
    /// <param name="message">The message</param>
    private void OnOpenSettingsPopupMessageReceived(object recipient, OpenSettingsPopupMessage message)
    {
        // Load the rules file path
        RulesPath = SettingsManager.RulesPath;
        
        // Show the popup
        ShowPopup = true;
    }
}