using System.Globalization;
using System.Windows.Data;

namespace TaskManagerWpf.App.Converters;

public sealed class NullableDateToStringConverter : IValueConverter
{
    public string Format { get; set; } = "dd.MM.yyyy";
    public string EmptyText { get; set; } = "—";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return EmptyText;

        if (value is DateTime dt) return dt.ToString(Format, culture);

        return EmptyText;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

