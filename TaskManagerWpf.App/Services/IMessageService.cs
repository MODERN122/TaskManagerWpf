namespace TaskManagerWpf.App.Services;

public interface IMessageService
{
    void ShowInfo(string message, string? title = null);
    void ShowError(string message, string? title = null);
    bool Confirm(string message, string? title = null);
}

