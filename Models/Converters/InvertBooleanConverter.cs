using System.Globalization;
using System;
using System.Windows.Data;

namespace GoCarlos.NET.Models.Converters;

public class InvertBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool?)value ?? true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(value as bool?);
}
