using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.Services;

public interface IDialogService
{
    Task<TaskItem?> ShowTaskEditorAsync(TaskItem draft, bool isNew, CancellationToken cancellationToken = default);
}

