using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.ViewModels;
using Xunit;

namespace TaskManagerWpf.Tests;

public sealed class TaskEditorViewModelTests
{
    [Fact]
    public void NewTask_TitleIsRequired()
    {
        var draft = new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = new DateTime(2026, 03, 17, 10, 0, 0),
            Title = "",
            Status = TaskItemStatus.Active,
            Priority = TaskPriority.Medium,
        };

        var vm = new TaskEditorViewModel(draft, isNew: true);

        Assert.True(vm.HasErrors);
        var errors = vm.GetErrors(nameof(vm.Title)).Cast<string>().ToList();
        Assert.Contains(errors, e => e.Contains("Title", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NewTask_DueDateCannotBeInPast()
    {
        var draft = new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Title = "Test",
            DueDate = DateTime.Today.AddDays(-1)
        };

        var vm = new TaskEditorViewModel(draft, isNew: true);

        Assert.True(vm.HasErrors);
        var errors = vm.GetErrors(nameof(vm.DueDate)).Cast<string>().ToList();
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void EditTask_DueDateMayBeInPast()
    {
        var draft = new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Title = "Existing task",
            DueDate = DateTime.Today.AddDays(-30)
        };

        var vm = new TaskEditorViewModel(draft, isNew: false);

        var errors = vm.GetErrors(nameof(vm.DueDate)).Cast<string>().ToList();
        Assert.Empty(errors);
    }

    [Fact]
    public void BuildResult_TrimsTitleAndDescription_AndNullsEmptyDescription()
    {
        var draft = new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Title = "  Hello  ",
            Description = "   ",
            Status = TaskItemStatus.Completed,
            Priority = TaskPriority.High,
            DueDate = DateTime.Today.AddDays(1)
        };

        var vm = new TaskEditorViewModel(draft, isNew: true);
        vm.Title = "  Hello  ";
        vm.Description = "   ";

        var result = vm.BuildResult();

        Assert.Equal("Hello", result.Title);
        Assert.Null(result.Description);
    }
}

