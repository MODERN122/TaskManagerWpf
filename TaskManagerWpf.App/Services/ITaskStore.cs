using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.Services;

public interface ITaskStore
{
    Task<IReadOnlyList<TaskItem>> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IReadOnlyList<TaskItem> tasks, CancellationToken cancellationToken = default);
    string StoragePath { get; }
}

