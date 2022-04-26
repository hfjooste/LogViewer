using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using LogViewer.Core;
using LogViewer.Helpers;
using LogViewer.Messages;
using LogViewer.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Win32;

namespace LogViewer.ViewModels;

/// <summary>
/// View Model for the main analysis window
/// </summary>
public class AnalysisViewModel : ObservableObject
{
    /// <summary>
    /// A boolean value indicating if the report is loaded
    /// </summary>
    private bool _reportLoaded;
    
    /// <summary>
    /// The title of the main window
    /// </summary>
    private string _title = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
    
    /// <summary>
    /// The version number extracted from the log file
    /// </summary>
    private string _version;
    
    /// <summary>
    /// The crashes extracted from the log file
    /// </summary>
    private LogExtract[] _crashes;
    
    /// <summary>
    /// The exceptions extracted from the log file
    /// </summary>
    private LogExtract[] _exceptions;
    
    /// <summary>
    /// The closed events extracted from the log file
    /// </summary>
    private LogExtract[] _closedEvents;
    
    /// <summary>
    /// The restart events extracted from the log file
    /// </summary>
    private LogExtract[] _restartEvents;
    
    /// <summary>
    /// A list of custom information extracted from the log file
    /// </summary>
    private ObservableCollection<LogInfo> _logInfo;

    /// <summary>
    /// Create a new instance of the AnalysisViewModel class
    /// </summary>
    public AnalysisViewModel()
    {
        // Initialize the RelayCommands
        OpenCommand = new RelayCommand(OnOpenCommandExecuted);
        RecentsCommand = new RelayCommand(OnRecentsCommandExecuted);
        SettingsCommand = new RelayCommand(OnSettingsCommandExecuted);
        AboutCommand = new RelayCommand(OnAboutCommandExecuted);
        ExitCommand = new RelayCommand(OnExitCommandExecuted);
        ViewCrashesCommand = new RelayCommand(OnViewCrashesCommandExecuted);
        ViewExceptionsCommand = new RelayCommand(OnViewExceptionsCommandExecuted);
        ViewClosedEventsCommand = new RelayCommand(OnViewClosedEventsCommandExecuted);
        ViewRestartEventsCommand = new RelayCommand(OnViewRestartEventsCommandExecuted);
        
        // Register the messenger
        WeakReferenceMessenger.Default.Register<OpenLogFileMessage>(this, OnOpenLogFileMessageReceived);
    }

    /// <summary>
    /// Executed after clicking on the open button
    /// </summary>
    public RelayCommand OpenCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the recents button
    /// </summary>
    public RelayCommand RecentsCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the settings button
    /// </summary>
    public RelayCommand SettingsCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the about button
    /// </summary>
    public RelayCommand AboutCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the exit button
    /// </summary>
    public RelayCommand ExitCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the crashes button
    /// </summary>
    public RelayCommand ViewCrashesCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the exceptions button
    /// </summary>
    public RelayCommand ViewExceptionsCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the closed events button
    /// </summary>
    public RelayCommand ViewClosedEventsCommand { get; }
    
    /// <summary>
    /// Executed after clicking on the restart events button
    /// </summary>
    public RelayCommand ViewRestartEventsCommand { get; }
    
    /// <summary>
    /// Get/Set the boolean value indicating if the report is loaded
    /// </summary>
    public bool ReportLoaded
    {
        get => _reportLoaded;
        set => SetProperty(ref _reportLoaded, value);
    }
    
    /// <summary>
    /// Get/Set the title of the main window
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    /// <summary>
    /// Get/Set the version number extracted from the log file
    /// </summary>
    public string Version
    {
        get => _version;
        set => SetProperty(ref _version, value);
    }

    /// <summary>
    /// Get/Set the crashes extracted from the log file
    /// </summary>
    public LogExtract[] Crashes
    {
        get => _crashes;
        set => SetProperty(ref _crashes, value);
    }
    
    /// <summary>
    /// Get/Set the exceptions extracted from the log file
    /// </summary>
    public LogExtract[] Exceptions
    {
        get => _exceptions;
        set => SetProperty(ref _exceptions, value);
    }
    
    /// <summary>
    /// Get/Set the restart events extracted from the log file
    /// </summary>
    public LogExtract[] RestartEvents
    {
        get => _restartEvents;
        set => SetProperty(ref _restartEvents, value);
    }
    
    /// <summary>
    /// Get/Set the closed events extracted from the log file
    /// </summary>
    public LogExtract[] ClosedEvents
    {
        get => _closedEvents;
        set => SetProperty(ref _closedEvents, value);
    }
    
    /// <summary>
    /// Get/Set the list of custom information extracted from the log file
    /// </summary>
    public ObservableCollection<LogInfo> LogInfo
    {
        get => _logInfo;
        set => SetProperty(ref _logInfo, value);
    }
    
    /// <summary>
    /// Validate the settings values and display the settings popup if needed
    /// </summary>
    public void ValidateSettings()
    {
        // Check for a valid rules file
        if (!string.IsNullOrEmpty(SettingsManager.RulesPath))
        {
            return;
        }
        
        // Show the settings popup
        SettingsCommand.Execute(null);
    }

    /// <summary>
    /// Open and process a log file
    /// </summary>
    private void OnOpenCommandExecuted()
    {
        // Show the open file dialog
        var filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files|*";
        var dialog = new OpenFileDialog { Filter = filter, Multiselect = false };
        var result = dialog.ShowDialog();
        
        // Check the selected file
        if (!result.HasValue || !result.Value)
        {
            return;
        }

        // Process the log file
        LoadLogFile(dialog.FileName);
    }
    
    /// <summary>
    /// Show the recents popup
    /// </summary>
    private void OnRecentsCommandExecuted() => WeakReferenceMessenger.Default.Send(new OpenRecentsPopupMessage());
    
    /// <summary>
    /// Show the settings popup
    /// </summary>
    private void OnSettingsCommandExecuted() => WeakReferenceMessenger.Default.Send(new OpenSettingsPopupMessage());
    
    /// <summary>
    /// Show the about popup
    /// </summary>
    private void OnAboutCommandExecuted() => WeakReferenceMessenger.Default.Send(new OpenAboutPopupMessage());
    
    /// <summary>
    /// Exit the application
    /// </summary>
    private void OnExitCommandExecuted() => Application.Current.Shutdown();
    
    /// <summary>
    /// Show the extract viewer with the list of crashes
    /// </summary>
    private void OnViewCrashesCommandExecuted() => 
        WeakReferenceMessenger.Default.Send(new OpenExtractViewerMessage("Crash", _crashes));
    
    /// <summary>
    /// Show the extract viewer with the list of exceptions
    /// </summary>
    private void OnViewExceptionsCommandExecuted() => 
        WeakReferenceMessenger.Default.Send(new OpenExtractViewerMessage("Exception", _exceptions));
    
    /// <summary>
    /// Show the extract viewer with the list of closed events
    /// </summary>
    private void OnViewClosedEventsCommandExecuted() => 
        WeakReferenceMessenger.Default.Send(new OpenExtractViewerMessage("Closed", _closedEvents));
    
    /// <summary>
    /// Show the extract viewer with the list of restart events
    /// </summary>
    private void OnViewRestartEventsCommandExecuted() => 
        WeakReferenceMessenger.Default.Send(new OpenExtractViewerMessage("Restart", _restartEvents));

    /// <summary>
    /// Executed after receiving an OpenLogFileMessage
    /// </summary>
    /// <param name="recipient">The receiving object</param>
    /// <param name="message">The message</param>
    private void OnOpenLogFileMessageReceived(object recipient, OpenLogFileMessage message) =>
        LoadLogFile(message.FilePath);

    /// <summary>
    /// Process the log file
    /// </summary>
    /// <param name="path">The path of the log file</param>
    private void LoadLogFile(string path)
    {
        // Set the title to the log file path
        Title = path;
        
        // Load and process the log file
        var file = LogFile.Open(path);
        var rules = LogAnalyzerRules.Generate(SettingsManager.RulesPath);
        var analysis = LogAnalyzer.Analyze(file, rules);

        // Set the variables
        Version = analysis.Version;
        Crashes = analysis.Crashes;
        Exceptions = analysis.Exceptions;
        RestartEvents = analysis.Restarts;
        ClosedEvents = analysis.Closed;

        // Add the timestamps
        var info = new List<LogInfo>
        {
            new("Date Started", analysis.TimestampStart),
            new("Date Ended", analysis.TimestampEnd)
        };

        // Add the custom extracted values
        info.AddRange(analysis.Extract.Select(extract => new LogInfo(extract.Name, extract.Value)));
        info.AddRange(analysis.Count.Select(count => new LogInfo(count.Name, count.Amount)));

        // Set the list of custom extracted values
        LogInfo = new ObservableCollection<LogInfo>(info);
        
        // Add the log file to the list of recents
        SettingsManager.AddToRecents(path);
        
        // Show the report
        ReportLoaded = true;
    }
}