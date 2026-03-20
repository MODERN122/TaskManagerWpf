using TaskManagerWpf.App.Services;

namespace TaskManagerWpf.Tests.MockServices;

public sealed class RecordingMessageService : IMessageService
{
    public List<(string Message, string? Title)> Infos { get; } = new();
    public List<(string Message, string? Title)> Errors { get; } = new();
    public Queue<bool> ConfirmResults { get; } = new();

    public void ShowInfo(string message, string? title = null) => Infos.Add((message, title));

    public void ShowError(string message, string? title = null) => Errors.Add((message, title));

    public bool Confirm(string message, string? title = null)
    {
        if (ConfirmResults.Count > 0)
            return ConfirmResults.Dequeue();

        return true;
    }
}

