using System.ComponentModel;
using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.ViewModels;
using TaskManagerWpf.Tests.TestDoubles;
using Xunit;

namespace TaskManagerWpf.Tests;

public sealed class MainViewModelQueryTests
{
    private static MainViewModel CreateVm(IEnumerable<TaskItem>? seed = null)
    {
        var store = new InMemoryTaskStore(seed);
        var dialog = new StubDialogService();
        var messages = new RecordingMessageService();
        return new MainViewModel(store, dialog, messages);
    }

    [Fact]
    public void Search_FindsByTitleOrDescription()
    {
        StaTest.Run(() =>
        {
            var vm = CreateVm();

            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "Купить молоко", Description = "магазин", Status = TaskItemStatus.Active }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "Workout", Description = "gym", Status = TaskItemStatus.Active }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "Оплатить счета", Description = "квартира", Status = TaskItemStatus.Completed }));

            vm.SearchText = "квар";

            var results = vm.TasksView.Cast<TaskItemViewModel>().ToList();
            Assert.Single(results);
            Assert.Equal("Оплатить счета", results[0].Title);
        });
    }

    [Fact]
    public void Filter_ByStatus_ActiveOnly()
    {
        StaTest.Run(() =>
        {
            var vm = CreateVm();

            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "A1", Status = TaskItemStatus.Active }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "C1", Status = TaskItemStatus.Completed }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "A2", Status = TaskItemStatus.Active }));

            vm.SelectedStatusFilter = vm.StatusFilters.Single(x => x.Value == StatusFilterMode.Active);

            var results = vm.TasksView.Cast<TaskItemViewModel>().ToList();
            Assert.Equal(2, results.Count);
            Assert.All(results, t => Assert.Equal(TaskItemStatus.Active, t.Status));
        });
    }

    [Fact]
    public void Sorting_ByPriorityDesc_PutsHighFirst()
    {
        StaTest.Run(() =>
        {
            var vm = CreateVm();

            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "Low", Priority = TaskPriority.Low, CreatedAt = new DateTime(2026, 1, 1) }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "High", Priority = TaskPriority.High, CreatedAt = new DateTime(2026, 1, 2) }));
            vm.Tasks.Add(new TaskItemViewModel(new TaskItem { Title = "Medium", Priority = TaskPriority.Medium, CreatedAt = new DateTime(2026, 1, 3) }));

            vm.SelectedSortMode = vm.SortModes.Single(x => x.Value == SortMode.PriorityDesc);

            var results = vm.TasksView.Cast<TaskItemViewModel>().ToList();
            Assert.Equal("High", results[0].Title);
            Assert.True(results[0].Priority >= results[1].Priority);
        });
    }
}

