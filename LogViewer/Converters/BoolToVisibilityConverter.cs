﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LogViewer.Converters;

/// <summary>
/// Convert a boolean value to a visibility value
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convert a boolean value to a visibility value
    /// </summary>
    /// <param name="value">The value produced by the binding source</param>
    /// <param name="targetType">The type of the binding target property</param>
    /// <param name="parameter">The converter parameter to use</param>
    /// <param name="culture">The culture to use in the converter</param>
    /// <returns>A converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (bool) value ? Visibility.Visible : Visibility.Collapsed;

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