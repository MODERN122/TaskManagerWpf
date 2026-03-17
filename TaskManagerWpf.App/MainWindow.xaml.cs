using System.Windows;
using System.Windows.Controls;

namespace TaskManagerWpf.App.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const double ScrollOffsetEpsilon = 0.5;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void ScrollToolbarLeft_Click(object sender, RoutedEventArgs e)
    {
        var step = GetToolbarScrollStep();
        var target = Math.Max(0, ToolbarScrollViewer.HorizontalOffset - step);
        ToolbarScrollViewer.ScrollToHorizontalOffset(target);
    }

    private void ScrollToolbarRight_Click(object sender, RoutedEventArgs e)
    {
        var step = GetToolbarScrollStep();
        var target = ToolbarScrollViewer.HorizontalOffset + step;
        ToolbarScrollViewer.ScrollToHorizontalOffset(target);
    }

    private void ToolbarScrollViewer_Loaded(object sender, RoutedEventArgs e) => UpdateToolbarScrollButtons();
    private void ToolbarScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateToolbarScrollButtons();
    private void ToolbarScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) => UpdateToolbarScrollButtons();

    private void UpdateToolbarScrollButtons()
    {
        if (ToolbarScrollViewer is null || ScrollLeftButton is null || ScrollRightButton is null) return;

        var canScrollLeft = ToolbarScrollViewer.HorizontalOffset > ScrollOffsetEpsilon;
        var canScrollRight = ToolbarScrollViewer.HorizontalOffset < ToolbarScrollViewer.ScrollableWidth - ScrollOffsetEpsilon;

        ScrollLeftButton.IsEnabled = canScrollLeft;
        ScrollRightButton.IsEnabled = canScrollRight;
    }

    private double GetToolbarScrollStep()
    {
        // Scroll by one "page" (visible width) so it adapts to window size.
        var step = ToolbarScrollViewer.ViewportWidth;
        if (double.IsNaN(step) || double.IsInfinity(step) || step <= 0)
            step = ToolbarScrollViewer.ActualWidth;
        if (double.IsNaN(step) || double.IsInfinity(step) || step <= 0)
            step = 0;

        return step;
    }
}