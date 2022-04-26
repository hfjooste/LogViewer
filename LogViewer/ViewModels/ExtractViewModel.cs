using LogViewer.Core;
using LogViewer.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace LogViewer.ViewModels;

/// <summary>
/// View Model for the Extract popup
/// </summary>
public class ExtractViewModel : ObservableObject
{
    /// <summary>
    /// Used to show/hide the extract popup
    /// </summary>
    private bool _showPopup;
    
    /// <summary>
    /// The title of the extract popup
    /// </summary>
    private string _title;
    
    /// <summary>
    /// The timestamp of the current extract
    /// </summary>
    private string _timestamp;
    
    /// <summary>
    /// The text of the current extract
    /// </summary>
    private string _extractText;
    
    /// <summary>
    /// A boolean value indicating if the user can change the current extract
    /// </summary>
    private bool _canChangeExtract;

    /// <summary>
    /// The index of the current extract
    /// </summary>
    private int _index;
    
    /// <summary>
    /// The title of the extract type
    /// </summary>
    private string _extractTitle;
    
    /// <summary>
    /// The array of extracts that can be displayed
    /// </summary>
    private LogExtract[] _extracts;

    /// <summary>
    /// Create a new instance of the ExtractViewModel class
    /// </summary>
    public ExtractViewModel()
    {
        // Initialize the RelayCommands
        PreviousCommand = new RelayCommand(OnPreviousCommandExecuted);
        NextCommand = new RelayCommand(OnNextCommandExecuted);
        CloseCommand = new RelayCommand(OnCloseCommandExecuted);
        
        // Register the messenger
        WeakReferenceMessenger.Default.Register<OpenExtractViewerMessage>(this, OnOpenExtractViewerMessageReceived);
    }

    /// <summary>
    /// Executed after clicking on the previous extract button
    /// </summary>
    public RelayCommand PreviousCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the next extract button
    /// </summary>
    public RelayCommand NextCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the close button
    /// </summary>
    public RelayCommand CloseCommand { get; }
    
    /// <summary>
    /// Get/Set the value used to show/hide the extract popup
    /// </summary>
    public bool ShowPopup
    {
        get => _showPopup;
        set => SetProperty(ref _showPopup, value);
    }

    /// <summary>
    /// Get/Set the title of the extract popup
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    /// <summary>
    /// Get/Set the timestamp of the current extract
    /// </summary>
    public string Timestamp
    {
        get => _timestamp;
        set => SetProperty(ref _timestamp, value);
    }
    
    /// <summary>
    /// Get/Set the text of the current extract
    /// </summary>
    public string ExtractText
    {
        get => _extractText;
        set => SetProperty(ref _extractText, value);
    }

    /// <summary>
    /// Get/Set the boolean value indicating if the user can change the current extract
    /// </summary>
    public bool CanChangeExtract
    {
        get => _canChangeExtract;
        set => SetProperty(ref _canChangeExtract, value);
    }

    /// <summary>
    /// Executed after clicking on the previous extract button
    /// </summary>
    private void OnPreviousCommandExecuted()
    {
        // Update the index
        _index--;
        
        // Check if we should rather load the last extract
        if (_index < 0)
        {
            // Update the index
            _index = _extracts.Length - 1;
        }

        // Load the extract
        LoadExtract();
    }

    /// <summary>
    /// Executed after clicking on the next extract button
    /// </summary>
    private void OnNextCommandExecuted()
    {
        // Update the index
        _index++;
        
        // Check if we should rather load the first extract
        if (_index >= _extracts.Length)
        {
            // Update the index
            _index = 0;
        }

        // Load the extract
        LoadExtract();
    }
    
    /// <summary>
    /// Executed after clicking on the close button
    /// </summary>
    private void OnCloseCommandExecuted() => ShowPopup = false;
    
    /// <summary>
    /// Executed after receiving an OpenExtractViewerMessage
    /// </summary>
    /// <param name="recipient">The receiving object</param>
    /// <param name="message">The message</param>
    private void OnOpenExtractViewerMessageReceived(object recipient, OpenExtractViewerMessage message)
    {
        // Check for a valid list of extracts
        if (message.Extracts.Length == 0)
        {
            return;
        }
        
        // Reset the index
        _index = 0;
        
        // Update the variables
        _extractTitle = message.Title;
        _extracts = message.Extracts;
        CanChangeExtract = _extracts.Length > 1;
        
        // Load the extract
        LoadExtract();
        ShowPopup = true;
    }

    /// <summary>
    /// Load the extract at the current index
    /// </summary>
    private void LoadExtract()
    {
        // Set the values using the extract at the current index
        Title = $"{_extractTitle} {_index + 1} of {_extracts.Length}";
        Timestamp = _extracts[_index].Timestamp;
        ExtractText = _extracts[_index].Details;
    }
}