using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DMPS.Client.Presentation.Infrastructure.Converters
{
    /// <summary>
    /// Converts a boolean value to a System.Windows.Visibility value.
    /// This is a standard converter used in MVVM to control the visibility of UI elements based on a ViewModel property.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the conversion should be inverted.
        /// If true, a true boolean value will result in Visibility.Collapsed.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Converts a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the binding target property (should be Visibility).</param>
        /// <param name="parameter">An optional parameter. If "Invert" is passed, it inverts the logic for this specific conversion.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Visibility.Visible if the value is true; otherwise, Visibility.Collapsed. The logic is inverted if IsInverted is true or the parameter is "Invert".</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                return Visibility.Collapsed;
            }

            bool invert = IsInverted;
            if (parameter is string paramString && string.Equals(paramString, "Invert", StringComparison.OrdinalIgnoreCase))
            {
                invert = !invert;
            }

            if (invert)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a Visibility value back to a boolean value.
        /// This method is not implemented and will throw an exception if used.
        /// </summary>
        /// <param name="value">The visibility value to convert.</param>
        /// <param name="targetType">The type to convert to (should be boolean).</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A boolean value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BooleanToVisibilityConverter can only be used for one-way bindings.");
        }
    }
}