using System.Windows;
using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.ViewModels;
using TaskManagerWpf.App.Views;

namespace TaskManagerWpf.App.Services;

public sealed class DialogService : IDialogService
{
    public Task<TaskItem?> ShowTaskEditorAsync(TaskItem draft, bool isNew, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var vm = new TaskEditorViewModel(draft, isNew);
        var window = new TaskEditorWindow
        {
            DataContext = vm,
            Owner = Application.Current?.MainWindow,
            Title = isNew ? "Новая задача" : "Редактирование задачи"
        };

        var result = window.ShowDialog();
        if (result == true)
            return Task.FromResult<TaskItem?>(vm.BuildResult());

        return Task.FromResult<TaskItem?>(null);
    }
}

