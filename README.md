# Log Viewer

Log Viewer is a simple tool that allows you to easily analyze log files using a custom set of rules

![Log Viewer 1](https://i.imgur.com/7m2N4EB.png)
![Log Viewer 2](https://i.imgur.com/sdqp5GL.jpeg)

## Rules File
You need to specify a rules file before you can analyze a log file. An example of the rules file can be found [here](https://github.com/hfjooste/LogViewer/blob/main/Sample/rules.json)

## Rules File Structure
| Field                    | Type             | Description |
|--------------------------|------------------|-------------|
| TimestampFormat          | Array\<String\>  | An array of regular expressions used to identify timestamps in the log file |
| VersionIdentifierStart   | String           | The substring found just before the version number in the log file |
| VersionIdentifierEnd     | String           | The substring found directly after the version number in the log file |
| RestartIdentifier        | String           | The substring used to identify restart events in the log file |
| ExceptionIdentifier      | String           | The substring used to identify exceptions in the log file |
| ExitIdentifier           | String           | The substring used to identify normal exit events in the log file |
| CrashIdentifier          | String           | The substring used to identify crashes in the log file |
| AdditionalRestartLines   | Integer          | The amount of additional lines to be added before a restart event extract |
| AdditionalExceptionLines | Integer          | The amount of additional lines to be added before an exception extract |
| AdditionalExitLines      | Integer          | The amount of additional lines to be added before an exit event extract |
| AdditionalCrashLines     | Integer          | The amount of additional lines to be added before a crash extract |
| Extract                  | Array\<Extract\> | An array of custom values that should be extracted from the log file |
| Count                    | Array\<Count\>   | An array of custom substrings that should be counted in the log file |


## Extract Structure
| Field           | Type   | Description |
| ----------------|--------|-------------|
| Name            | String | A user friendly display name associated with this extract |
| IdentifierStart | String | The substring found just before the value that should be extracted |
| IdentifierEnd   | String | The substring found just after the substring that should be extracted |

## Count Structure
| Field      | Type   | Description |
| -----------|--------|-------------|
| Name       | String | A user friendly display name associated with this extract |
| Identifier | String | The substring that should be counted |

## Integration
The functionality from the log viewer can easily be added to another project:
1. Add the `LogViewer.Core` project to your project or include the `LogViewer.Core.dll` file
2. Create a new `LogFile` object:
```c#
var logFile = LogFile.Open(@"C:\Users\Username\Desktop\activity.log");
```
3. Generate a `LogAnalyzerRules` object using a valid rules file:
```c#
var rules = LogAnalyzerRules.Generate(@"C:\Users\Username\Desktop\rules.json");
```
4. Analyze the log file:
```c#
var analysis = LogAnalyzer.Analyze(logFile, rules);
```

This will give you a `LogAnalysis` object containing the following information:
| Field          | Type                        | Description |
| ---------------|-----------------------------|-------------|
| TimestampStart | String                      | The first timestamp detected in the log file |
| TimestampEnd   | String                      | The last timestamp detected in the log file |
| Version        | String                      | The application version extracted from the log file |
| Closed         | Array\<LogExtract\>         | Extracts from when the application was closed |
| Restarts       | Array\<LogExtract\>         | Extracts from when the application was restarted (or opened) |
| Exceptions     | Array\<LogExtract\>         | Extracts for every exceptions that was detected in the log file |
| Crashes        | Array\<LogExtract\>         | Extracts for every crash that was detected in the log file |
| Extract        | Array\<LogAnalysisExtract\> | Custom extracts from the log file |
| Count          | Array\<LogAnalysisCount\>   | Custom count results from the log file |

### Log Extract Structure
| Field     | Type   | Description |
| ----------|--------|-------------|
| Timestamp | String | The timestamp of the extract |
| Details   | String | The actual text that was extracted from the log file |

### Log Analysis Extract Structure
| Field | Type   | Description |
| ------|--------|-------------|
| Name  | String | A user friendly display name |
| Value | String | The value extracted from the log file |

### Log Analysis Count Structure
| Field  | Type    | Description |
| -------|---------|-------------|
| Name   | String  | A user friendly display name |
| Amount | Integer | The amount of occurrences found in the log file |
