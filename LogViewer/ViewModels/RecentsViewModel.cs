using System.Collections.ObjectModel;
using System.Linq;
using LogViewer.Messages;
using LogViewer.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace LogViewer.ViewModels;

/// <summary>
/// View Model for the Recents popup
/// </summary>
public class RecentsViewModel : ObservableObject
{
    /// <summary>
    /// Used to show/hide the recents popup
    /// </summary>
    private bool _showPopup;
    
    /// <summary>
    /// The list of recent log file paths
    /// </summary>
    private ObservableCollection<string> _recents;
    
    /// <summary>
    /// The selected index of the log file paths list
    /// </summary>
    private int _selectedIndex;

    /// <summary>
    /// Create a new instance of the RecentsViewModel class
    /// </summary>
    public RecentsViewModel()
    {
        // Initialize the RelayCommands
        CloseCommand = new RelayCommand(OnCloseCommandExecuted);
        
        // Register the messenger
        WeakReferenceMessenger.Default.Register<OpenRecentsPopupMessage>(this, OnOpenRecentsPopupMessageReceived);
    }
    
    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    public RelayCommand CloseCommand { get; }
    
    /// <summary>
    /// Get/Set the value used to show/hide the recents popup
    /// </summary>
    public bool ShowPopup
    {
        get => _showPopup;
        set => SetProperty(ref _showPopup, value);
    }

    /// <summary>
    /// Get/Set the list of recent log file paths
    /// </summary>
    public ObservableCollection<string> Recents
    {
        get => _recents;
        set => SetProperty(ref _recents, value);
    }

    /// <summary>
    /// Get/Set the selected index of the log file paths list
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            SetProperty(ref _selectedIndex, value);
            OnSelectedIndexChanged();
        }
    }

    /// <summary>
    /// Executed after clicking on the Close button
    /// </summary>
    private void OnCloseCommandExecuted() => ShowPopup = false;
    
    /// <summary>
    /// Executed after receiving an OpenRecentsPopupMessage
    /// </summary>
    /// <param name="recipient">The receiving object</param>
    /// <param name="message">The message</param>
    private void OnOpenRecentsPopupMessageReceived(object recipient, OpenRecentsPopupMessage message)
    {
        // Set the variables
        Recents = new ObservableCollection<string>(SettingsManager.Recents.Cast<string>().ToList());
        SelectedIndex = -1;
        
        // Show the popup
        ShowPopup = true;
    }

    /// <summary>
    /// Executed after changing the selected index of the log file paths list
    /// </summary>
    private void OnSelectedIndexChanged()
    {
        // Check for a valid index
        if (_selectedIndex < 0)
        {
            return;
        }
        
        // Close the popup
        CloseCommand.Execute(null);
        
        // Load the log file at the selected index
        WeakReferenceMessenger.Default.Send(new OpenLogFileMessage(Recents[_selectedIndex]));
    }
}