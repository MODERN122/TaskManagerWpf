using System.Windows;

namespace TaskManagerWpf.App.Services;

public sealed class MessageBoxMessageService : IMessageService
{
    public void ShowInfo(string message, string? title = null) =>
        MessageBox.Show(message, title ?? "Task Manager", MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string? title = null) =>
        MessageBox.Show(message, title ?? "Task Manager — Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

    public bool Confirm(string message, string? title = null) =>
        MessageBox.Show(message, title ?? "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
}

