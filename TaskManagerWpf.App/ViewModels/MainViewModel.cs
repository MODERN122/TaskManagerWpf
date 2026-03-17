using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManagerWpf.App.Models;
using TaskManagerWpf.App.Services;

namespace TaskManagerWpf.App.ViewModels;

public enum StatusFilterMode
{
    All = 0,
    Active = 1,
    Completed = 2
}

public enum SortMode
{
    CreatedAtDesc = 0,
    PriorityDesc = 1,
    DueDateAsc = 2
}

public sealed class Option<T>
{
    public Option(string title, T value)
    {
        Title = title;
        Value = value;
    }

    public string Title { get; }
    public T Value { get; }

    public override string ToString() => Title;
}

public sealed partial class MainViewModel : ObservableObject
{
    private readonly ITaskStore _store;
    private readonly IDialogService _dialogService;
    private readonly IMessageService _messageService;

    private CancellationTokenSource? _saveDebounceCts;

    public MainViewModel(ITaskStore store, IDialogService dialogService, IMessageService messageService)
    {
        _store = store;
        _dialogService = dialogService;
        _messageService = messageService;

        Tasks = new ObservableCollection<TaskItemViewModel>();
        Tasks.CollectionChanged += TasksOnCollectionChanged;

        TasksView = CollectionViewSource.GetDefaultView(Tasks);
        TasksView.Filter = FilterTask;

        StatusFilters = new[]
        {
            new Option<StatusFilterMode>("Все", StatusFilterMode.All),
            new Option<StatusFilterMode>("Активные", StatusFilterMode.Active),
            new Option<StatusFilterMode>("Завершённые", StatusFilterMode.Completed),
        };
        SelectedStatusFilter = StatusFilters[0];

        SortModes = new[]
        {
            new Option<SortMode>("По дате создания (новые сверху)", SortMode.CreatedAtDesc),
            new Option<SortMode>("По приоритету (High → Low)", SortMode.PriorityDesc),
            new Option<SortMode>("По сроку (раньше сверху)", SortMode.DueDateAsc),
        };
        SelectedSortMode = SortModes[0];
        ApplySorting();

        UpdateStatusText();
    }

    public ObservableCollection<TaskItemViewModel> Tasks { get; }
    public ICollectionView TasksView { get; }

    public IReadOnlyList<Option<StatusFilterMode>> StatusFilters { get; }
    public IReadOnlyList<Option<SortMode>> SortModes { get; }

    [ObservableProperty]
    private Option<StatusFilterMode> selectedStatusFilter;

    [ObservableProperty]
    private Option<SortMode> selectedSortMode;

    [ObservableProperty]
    private string searchText = "";

    [ObservableProperty]
    private TaskItemViewModel? selectedTask;

    [ObservableProperty]
    private string statusText = "";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var items = await _store.LoadAsync(cancellationToken);

            Tasks.Clear();
            foreach (var item in items.OrderByDescending(x => x.CreatedAt))
            {
                var vm = new TaskItemViewModel(item);
                HookTask(vm);
                Tasks.Add(vm);
            }

            TasksView.Refresh();
            UpdateStatusText();
        }
        catch (Exception ex)
        {
            _messageService.ShowError(
                $"Не удалось загрузить задачи из JSON.\nФайл: {_store.StoragePath}\n\n{ex.Message}");
            UpdateStatusText();
        }
    }

    partial void OnSelectedTaskChanged(TaskItemViewModel? value)
    {
        EditSelectedTaskCommand.NotifyCanExecuteChanged();
        DeleteSelectedTaskCommand.NotifyCanExecuteChanged();
        ToggleStatusCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedStatusFilterChanged(Option<StatusFilterMode> value)
    {
        TasksView.Refresh();
        UpdateStatusText();
    }

    partial void OnSelectedSortModeChanged(Option<SortMode> value)
    {
        ApplySorting();
        TasksView.Refresh();
        UpdateStatusText();
    }

    partial void OnSearchTextChanged(string value)
    {
        TasksView.Refresh();
        UpdateStatusText();
    }

    private void ApplySorting()
    {
        using (TasksView.DeferRefresh())
        {
            TasksView.SortDescriptions.Clear();
            switch (SelectedSortMode.Value)
            {
                case SortMode.CreatedAtDesc:
                    TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.CreatedAt), ListSortDirection.Descending));
                    break;
                case SortMode.PriorityDesc:
                    TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.Priority), ListSortDirection.Descending));
                    TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.CreatedAt), ListSortDirection.Descending));
                    break;
                case SortMode.DueDateAsc:
                    TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.DueDate), ListSortDirection.Ascending));
                    TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskItemViewModel.CreatedAt), ListSortDirection.Descending));
                    break;
            }
        }
    }

    private bool FilterTask(object obj)
    {
        if (obj is not TaskItemViewModel t) return false;

        if (SelectedStatusFilter.Value == StatusFilterMode.Active && t.Status != TaskItemStatus.Active) return false;
        if (SelectedStatusFilter.Value == StatusFilterMode.Completed && t.Status != TaskItemStatus.Completed) return false;

        var q = (SearchText ?? "").Trim();
        if (q.Length == 0) return true;

        return (t.Title?.Contains(q, StringComparison.CurrentCultureIgnoreCase) ?? false)
               || (t.Description?.Contains(q, StringComparison.CurrentCultureIgnoreCase) ?? false);
    }

    [RelayCommand]
    private async Task AddTaskAsync()
    {
        var draft = new TaskItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Status = TaskItemStatus.Active,
            Priority = TaskPriority.Medium
        };

        var result = await _dialogService.ShowTaskEditorAsync(draft, isNew: true);
        if (result is null) return;

        var vm = new TaskItemViewModel(result);
        HookTask(vm);
        Tasks.Insert(0, vm);
        SelectedTask = vm;

        await SaveSoonAsync();
        UpdateStatusText();
    }

    [RelayCommand(CanExecute = nameof(CanActOnSelected))]
    private async Task EditSelectedTaskAsync()
    {
        if (SelectedTask is null) return;

        var draft = SelectedTask.CloneModel();
        var result = await _dialogService.ShowTaskEditorAsync(draft, isNew: false);
        if (result is null) return;

        SelectedTask.ApplyFrom(result);

        await SaveSoonAsync();
        UpdateStatusText();
    }

    [RelayCommand(CanExecute = nameof(CanActOnSelected))]
    private async Task DeleteSelectedTaskAsync()
    {
        if (SelectedTask is null) return;

        var ok = _messageService.Confirm($"Удалить задачу \"{SelectedTask.Title}\"?", "Удаление");
        if (!ok) return;

        var toRemove = SelectedTask;
        SelectedTask = null;
        Tasks.Remove(toRemove);

        await SaveSoonAsync();
        UpdateStatusText();
    }

    [RelayCommand(CanExecute = nameof(CanActOnSelected))]
    private async Task ToggleStatusAsync()
    {
        if (SelectedTask is null) return;

        SelectedTask.Status = SelectedTask.Status == TaskItemStatus.Active ? TaskItemStatus.Completed : TaskItemStatus.Active;
        TasksView.Refresh();

        await SaveSoonAsync();
        UpdateStatusText();
    }

    private bool CanActOnSelected() => SelectedTask is not null;

    private void TasksOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (TaskItemViewModel vm in e.OldItems)
                UnhookTask(vm);
        }

        if (e.NewItems is not null)
        {
            foreach (TaskItemViewModel vm in e.NewItems)
                HookTask(vm);
        }

        _ = SaveSoonAsync();
        UpdateStatusText();
    }

    private void HookTask(TaskItemViewModel vm) => vm.PropertyChanged += TaskOnPropertyChanged;
    private void UnhookTask(TaskItemViewModel vm) => vm.PropertyChanged -= TaskOnPropertyChanged;

    private void TaskOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TaskItemViewModel.Title)
            or nameof(TaskItemViewModel.Description)
            or nameof(TaskItemViewModel.Status)
            or nameof(TaskItemViewModel.Priority)
            or nameof(TaskItemViewModel.DueDate))
        {
            if (e.PropertyName is nameof(TaskItemViewModel.Title) or nameof(TaskItemViewModel.Description))
                TasksView.Refresh();

            _ = SaveSoonAsync();
            UpdateStatusText();
        }
    }

    private async Task SaveSoonAsync()
    {
        _saveDebounceCts?.Cancel();
        _saveDebounceCts = new CancellationTokenSource();
        var token = _saveDebounceCts.Token;

        try
        {
            await Task.Delay(350, token);
            await SaveNowAsync(token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task SaveNowAsync(CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = Tasks.Select(t => t.CloneModel()).ToList();
            await _store.SaveAsync(snapshot, cancellationToken);
        }
        catch (Exception ex)
        {
            _messageService.ShowError(
                $"Не удалось сохранить задачи в JSON.\nФайл: {_store.StoragePath}\n\n{ex.Message}");
        }
    }

    private void UpdateStatusText()
    {
        var all = Tasks.Count;
        var active = Tasks.Count(t => t.Status == TaskItemStatus.Active);
        var completed = all - active;
        StatusText = $"Всего: {all} | Активные: {active} | Завершённые: {completed} | Файл: {_store.StoragePath}";
    }
}

