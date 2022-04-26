using System;
using System.Globalization;
using System.Windows.Data;
using LogViewer.Core;

namespace LogViewer.Converters;

/// <summary>
/// Convert a log extract array to an integer
/// </summary>
public class LogExtractsToIntConverter : IValueConverter
{
    /// <summary>
    /// Convert a log extract array to an integer
    /// </summary>
    /// <param name="value">The value produced by the binding source</param>
    /// <param name="targetType">The type of the binding target property</param>
    /// <param name="parameter">The converter parameter to use</param>
    /// <param name="culture">The culture to use in the converter</param>
    /// <returns>A converted value</returns>
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        ((LogExtract[]) value)?.Length ?? 0;

    /// <summary>
    /// Converts a value
    /// </summary>
    /// <param name="value">The value that is produced by the binding target</param>
    /// <param name="targetType">The type to convert to</param>
    /// <param name="parameter">The converter parameter to use</param>
    /// <param name="culture">The culture to use in the converter</param>
    /// <returns>A converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}