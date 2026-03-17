using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManagerWpf.App.Converters;

public sealed class OverdueToBrushConverter : IValueConverter
{
    public Brush OverdueBrush { get; set; } = new SolidColorBrush(Color.FromRgb(220, 38, 38)); // red-600
    public Brush NormalBrush { get; set; } = new SolidColorBrush(Color.FromRgb(17, 24, 39));  // gray-900

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b) return OverdueBrush;
        return NormalBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

