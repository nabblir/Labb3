using System;
using System.Globalization;
using System.Windows.Data;
/*
 * This converter checks if a value is null and converts it to a boolean.
 * If the value is null, it returns false; otherwise, it returns true.
 * This is useful for toggling visibility or enabling/disabling controls based on the presence of a value.
 * 
 * Not my own code, found on:
 * https://stackoverflow.com
 */
namespace Labb3.Converter
    {
    class NullToBoolConverter : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            return value != null;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            throw new NotImplementedException();
            }
        }
    }