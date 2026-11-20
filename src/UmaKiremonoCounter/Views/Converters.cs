using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UmaKiremonoCounter.Views;

/// <summary>true なら Collapsed、false なら Visible。</summary>
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility.Collapsed;
}
