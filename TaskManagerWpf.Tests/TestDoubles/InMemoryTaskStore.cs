using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.Services;

namespace TaskManagerWpf.Tests.TestDoubles;

public sealed class InMemoryTaskStore : ITaskStore
{
    private readonly object _gate = new();
    private List<TaskItem> _items;

    public InMemoryTaskStore(IEnumerable<TaskItem>? seed = null, string storagePath = "memory://tasks")
    {
        _items = seed?.Select(Clone).ToList() ?? new List<TaskItem>();
        StoragePath = storagePath;
    }

    public string StoragePath { get; }

    public Task<IReadOnlyList<TaskItem>> LoadAsync(CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            IReadOnlyList<TaskItem> snapshot = _items.Select(Clone).ToList();
            return Task.FromResult(snapshot);
        }
    }

    public Task SaveAsync(IReadOnlyList<TaskItem> tasks, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _items = tasks.Select(Clone).ToList();
        }

        return Task.CompletedTask;
    }

    private static TaskItem Clone(TaskItem t) =>
        new()
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt
        };
}

