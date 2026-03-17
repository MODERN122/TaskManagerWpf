using System.IO;
using System.Text;
using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.Services;
using Xunit;

namespace TaskManagerWpf.Tests;

public sealed class JsonTaskStoreSeedTests
{
    [Fact]
    public async Task Seed_AllPrioritiesAndStatuses_SavesAndLoads()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "TaskManagerWpf.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        var jsonPath = Path.Combine(tempDir, "tasks.seed.json");

        try
        {
            var store = new JsonTaskStore(jsonPath);

            var tasks = new List<TaskItem>();
            var now = new DateTime(2026, 03, 17, 12, 0, 0);

            var i = 0;
            foreach (var status in Enum.GetValues<TaskItemStatus>())
            foreach (var priority in Enum.GetValues<TaskPriority>())
            {
                i++;
                tasks.Add(new TaskItem
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = now.AddMinutes(-i),
                    Title = $"Seed {status} {priority}",
                    Description = $"Заполнение базы: {status}/{priority}",
                    Status = status,
                    Priority = priority,
                    DueDate = now.Date.AddDays(priority == TaskPriority.High ? 1 : 3)
                });
            }

            await store.SaveAsync(tasks);

            Assert.True(File.Exists(jsonPath));
            var text = await File.ReadAllTextAsync(jsonPath, Encoding.UTF8);
            Assert.Contains("Заполнение базы", text);
            Assert.DoesNotContain("\\u04", text, StringComparison.OrdinalIgnoreCase); // кириллица не должна быть экранирована

            var loaded = await store.LoadAsync();
            Assert.Equal(tasks.Count, loaded.Count);
            Assert.Equal(Enum.GetValues<TaskItemStatus>().Length, loaded.Select(t => t.Status).Distinct().Count());
            Assert.Equal(Enum.GetValues<TaskPriority>().Length, loaded.Select(t => t.Priority).Distinct().Count());
        }
        finally
        {
            try { Directory.Delete(tempDir, recursive: true); } catch { }
        }
    }
}

