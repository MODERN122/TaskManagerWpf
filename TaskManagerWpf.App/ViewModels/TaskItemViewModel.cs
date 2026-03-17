using CommunityToolkit.Mvvm.ComponentModel;
using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.ViewModels;

public sealed partial class TaskItemViewModel : ObservableObject
{
    public TaskItemViewModel(TaskItem model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public TaskItem Model { get; }

    public Guid Id
    {
        get => Model.Id;
        set
        {
            if (Model.Id == value) return;
            Model.Id = value;
            OnPropertyChanged();
        }
    }

    public string Title
    {
        get => Model.Title;
        set
        {
            if (Model.Title == value) return;
            Model.Title = value;
            OnPropertyChanged();
        }
    }

    public string? Description
    {
        get => Model.Description;
        set
        {
            if (Model.Description == value) return;
            Model.Description = value;
            OnPropertyChanged();
        }
    }

    public TaskItemStatus Status
    {
        get => Model.Status;
        set
        {
            if (Model.Status == value) return;
            Model.Status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsOverdue));
            OnPropertyChanged(nameof(OverdueLabel));
        }
    }

    public TaskPriority Priority
    {
        get => Model.Priority;
        set
        {
            if (Model.Priority == value) return;
            Model.Priority = value;
            OnPropertyChanged();
        }
    }

    public DateTime? DueDate
    {
        get => Model.DueDate;
        set
        {
            if (Model.DueDate == value) return;
            Model.DueDate = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsOverdue));
            OnPropertyChanged(nameof(OverdueLabel));
        }
    }

    public DateTime CreatedAt
    {
        get => Model.CreatedAt;
        set
        {
            if (Model.CreatedAt == value) return;
            Model.CreatedAt = value;
            OnPropertyChanged();
        }
    }

    public bool IsOverdue =>
        Status == TaskItemStatus.Active &&
        DueDate is DateTime due &&
        due.Date < DateTime.Today;

    public string OverdueLabel => IsOverdue ? "Просрочено" : "";

    public TaskItem CloneModel() =>
        new()
        {
            Id = Id,
            Title = Title,
            Description = Description,
            Status = Status,
            Priority = Priority,
            DueDate = DueDate,
            CreatedAt = CreatedAt
        };

    public void ApplyFrom(TaskItem source)
    {
        Id = source.Id;
        Title = source.Title;
        Description = source.Description;
        Status = source.Status;
        Priority = source.Priority;
        DueDate = source.DueDate;
        CreatedAt = source.CreatedAt;
    }
}

