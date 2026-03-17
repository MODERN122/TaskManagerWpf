using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.Converters;

public sealed class PriorityToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TaskPriority p) return Brushes.Gray;

        return p switch
        {
            TaskPriority.Low => new SolidColorBrush(Color.FromRgb(16, 185, 129)),     // emerald
            TaskPriority.Medium => new SolidColorBrush(Color.FromRgb(245, 158, 11)),  // amber
            TaskPriority.High => new SolidColorBrush(Color.FromRgb(239, 68, 68)),     // red
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

