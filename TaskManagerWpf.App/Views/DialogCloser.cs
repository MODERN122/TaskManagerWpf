using System.Windows;

namespace TaskManagerWpf.App.Views;

public static class DialogCloser
{
    public static readonly DependencyProperty DialogResultProperty =
        DependencyProperty.RegisterAttached(
            "DialogResult",
            typeof(bool?),
            typeof(DialogCloser),
            new PropertyMetadata(null, OnDialogResultChanged));

    public static bool? GetDialogResult(Window target) => (bool?)target.GetValue(DialogResultProperty);
    public static void SetDialogResult(Window target, bool? value) => target.SetValue(DialogResultProperty, value);

    private static void OnDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window) return;
        if (e.NewValue is not bool result) return;

        window.DialogResult = result;
        window.Close();
    }
}

