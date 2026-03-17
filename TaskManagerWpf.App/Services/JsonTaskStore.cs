using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagerWpf.App.Models;

namespace TaskManagerWpf.App.Services;

public sealed class JsonTaskStore : ITaskStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public JsonTaskStore(string? storagePath = null)
    {
        StoragePath = storagePath ?? GetDefaultPath();
    }

    public string StoragePath { get; }

    public async Task<IReadOnlyList<TaskItem>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(StoragePath))
            return Array.Empty<TaskItem>();

        await using var stream = new FileStream(
            StoragePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        var tasks = await JsonSerializer.DeserializeAsync<List<TaskItem>>(stream, SerializerOptions, cancellationToken);
        return tasks ?? new List<TaskItem>();
    }

    public async Task SaveAsync(IReadOnlyList<TaskItem> tasks, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StoragePath)!);

        var tempPath = StoragePath + ".tmp";

        await using (var stream = new FileStream(
            tempPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true))
        {
            await JsonSerializer.SerializeAsync(stream, tasks, SerializerOptions, cancellationToken);
        }

        File.Copy(tempPath, StoragePath, overwrite: true);
        File.Delete(tempPath);
    }

    private static string GetDefaultPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TaskManagerWpf");
        return Path.Combine(folder, "tasks.json");
    }
}

