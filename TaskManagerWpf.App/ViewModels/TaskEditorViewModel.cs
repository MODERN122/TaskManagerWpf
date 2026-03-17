using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.ViewModels;

public sealed partial class TaskEditorViewModel : ObservableObject, INotifyDataErrorInfo
{
    private readonly bool _isNew;
    private readonly Dictionary<string, List<string>> _errors = new();

    public TaskEditorViewModel(TaskItem draft, bool isNew)
    {
        _isNew = isNew;

        Title = draft.Title;
        Description = draft.Description;
        Status = draft.Status;
        Priority = draft.Priority;
        DueDate = draft.DueDate;

        Id = draft.Id;
        CreatedAt = draft.CreatedAt;

        ValidateAll();
        SaveCommand.NotifyCanExecuteChanged();
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    [ObservableProperty]
    private bool? dialogResult;

    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private TaskItemStatus status;

    [ObservableProperty]
    private TaskPriority priority = TaskPriority.Medium;

    [ObservableProperty]
    private DateTime? dueDate;

    public IEnumerable<TaskItemStatus> StatusValues => Enum.GetValues<TaskItemStatus>();
    public IEnumerable<TaskPriority> PriorityValues => Enum.GetValues<TaskPriority>();

    public TaskItem BuildResult() =>
        new()
        {
            Id = Id,
            CreatedAt = CreatedAt,
            Title = Title.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            Status = Status,
            Priority = Priority,
            DueDate = DueDate
        };

    partial void OnTitleChanged(string value)
    {
        ValidateTitle();
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnDueDateChanged(DateTime? value)
    {
        ValidateDueDateRule();
        SaveCommand.NotifyCanExecuteChanged();
    }

    private void ValidateAll()
    {
        ValidateTitle();
        ValidateDueDateRule();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        ValidateAll();
        if (HasErrors) return;
        DialogResult = true;
    }

    private bool CanSave() =>
        !HasErrors &&
        !string.IsNullOrWhiteSpace(Title) &&
        (!_isNew || DueDate is null || DueDate.Value.Date >= DateTime.Today);

    [RelayCommand]
    private void Cancel() => DialogResult = false;

    public bool HasErrors => _errors.Count > 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return _errors.Values.SelectMany(x => x).ToList();

        return _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();
    }

    private void SetErrors(string propertyName, params string[] errors)
    {
        if (errors.Length == 0)
        {
            ClearErrors(propertyName);
            return;
        }

        _errors[propertyName] = errors.ToList();
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }

    private void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName))
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }

    private void ValidateTitle()
    {
        if (string.IsNullOrWhiteSpace(Title))
            SetErrors(nameof(Title), "Title обязателен.");
        else
            ClearErrors(nameof(Title));
    }

    private void ValidateDueDateRule()
    {
        if (_isNew && DueDate is DateTime due && due.Date < DateTime.Today)
            SetErrors(nameof(DueDate), "DueDate не может быть в прошлом при создании.");
        else
            ClearErrors(nameof(DueDate));
    }
}

