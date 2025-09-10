using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DMPS.Client.Presentation.Infrastructure.Converters
{
    /// <summary>
    /// Converts a boolean value to a System.Windows.Visibility value.
    /// By default, true is Visible and false is Collapsed.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the conversion should be inverted.
        /// If true, a true boolean value will be converted to Collapsed and false to Visible.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// Gets or sets the visibility value to use for the 'false' state. Defaults to Collapsed.
        /// Can be set to Hidden if space should be reserved.
        /// </summary>
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// Converts a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">An optional parameter. If not null, the conversion logic is inverted.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A Visibility value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;

            // Allow the parameter to override the IsInverted property for more flexibility in XAML
            bool inverted = IsInverted;
            if (parameter != null)
            {
                if (parameter is bool pBool)
                {
                    inverted = pBool;
                }
                else if (bool.TryParse(parameter.ToString(), out bool pBoolParsed))
                {
                    inverted = pBoolParsed;
                }
            }

            if (inverted)
            {
                return boolValue ? FalseVisibility : Visibility.Visible;
            }

            return boolValue ? Visibility.Visible : FalseVisibility;
        }

        /// <summary>
        /// Converts a Visibility value back to a boolean value.
        /// </summary>
        /// <param name="value">The visibility value to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">An optional parameter. If not null, the conversion logic is inverted.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A boolean value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility visibility)
            {
                return DependencyProperty.UnsetValue;
            }

            bool inverted = IsInverted;
            if (parameter is bool pBool)
            {
                inverted = pBool;
            }

            bool result = visibility == Visibility.Visible;

            return inverted ? !result : result;
        }
    }
}