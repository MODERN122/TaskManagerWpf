using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.Services;

namespace TaskManagerWpf.Tests.TestDoubles;

public sealed class StubDialogService : IDialogService
{
    public Func<TaskItem, bool, CancellationToken, Task<TaskItem?>>? OnShowTaskEditorAsync { get; set; }

    public Task<TaskItem?> ShowTaskEditorAsync(TaskItem draft, bool isNew, CancellationToken cancellationToken = default)
    {
        if (OnShowTaskEditorAsync is not null)
            return OnShowTaskEditorAsync(draft, isNew, cancellationToken);

        return Task.FromResult<TaskItem?>(null);
    }
}

