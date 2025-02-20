using System.Text;
using System.Text.Json;

namespace TaskTracker.Core;

public class JsonTaskRepository : ITaskRepository
{
    private static readonly object _saveLock = new();
    private readonly string _jsonFileName = ".tasks.json";
    private readonly Encoding _jsonEncoding = Encoding.UTF8;

    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly string _jsonFilePath;
    private readonly List<TaskEntity> _tasks;
    private int _nextId;

    public JsonTaskRepository(string workingFolder)
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            MaxDepth = 0,
        };
        _jsonFilePath = Path.Combine(workingFolder, _jsonFileName);
        using FileStream jsonFileStream = new FileStream(
                path: _jsonFilePath,
                mode: FileMode.OpenOrCreate,
                access: FileAccess.ReadWrite,
                share: FileShare.None);
        FileAttributes attributes = File.GetAttributes(_jsonFilePath);
        File.SetAttributes(_jsonFilePath, attributes | FileAttributes.Hidden);
        using StreamReader jsonFileReader = new(jsonFileStream, encoding: _jsonEncoding);
        string jsonData = jsonFileReader.ReadToEnd();
        if (string.IsNullOrEmpty(jsonData)) _tasks = [];
        else _tasks = JsonSerializer.Deserialize<List<TaskEntity>>(jsonData, _jsonSerializerOptions) ?? [];
        _nextId = _tasks.Count == 0 ? 1 : _tasks.Max(x => x.Id) + 1;
    }

    #region Private Methods

    private async Task saveChangesAsync()
    {
        byte[] jsonBytes;
        lock (_saveLock)
        {
            string jsonData = JsonSerializer.Serialize(_tasks, _jsonSerializerOptions);
            jsonBytes = _jsonEncoding.GetBytes(jsonData);
        }
        using FileStream jsonFileStream = new FileStream(
                path: _jsonFilePath,
                mode: FileMode.Open,
                access: FileAccess.Write,
                share: FileShare.None);
        await jsonFileStream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
        await jsonFileStream.FlushAsync();
    }

    private TaskEntity? find(int id)
    {
        if (id < 1) return null;
        TaskEntity? result = _tasks.FirstOrDefault(x => x.Id == id && x.Status != TaskStatus.Deleted);
        return result;
    }

    #endregion

    #region ITaskRepository Implementation

    public async Task<TaskEntity?> AddAsync(string title)
    {
        TaskEntity taskToAdd = new TaskEntity
        {
            Id = _nextId,
            Title = title,
            Status = TaskStatus.Todo
        };
        _tasks.Add(taskToAdd);
        _nextId++;
        await saveChangesAsync();
        return taskToAdd;
    }

    public async Task<TaskEntity?> UpdateAsync(int id, string title)
    {
        TaskEntity? taskToUpdate = find(id);
        if (taskToUpdate == null) return null;
        taskToUpdate.Title = title;
        await saveChangesAsync();
        return taskToUpdate;
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        TaskEntity? taskToRemove = find(id);
        if (taskToRemove == null) return null;
        taskToRemove.Status = TaskStatus.Deleted;
        await saveChangesAsync();
        return true;
    }

    public async Task<TaskEntity?> MarkAsAsync(int id, TaskStatus status)
    {
        if (status < TaskStatus.Todo || status > TaskStatus.Deleted) return null;
        TaskEntity? taskToUpdate = find(id);
        if (taskToUpdate == null) return null;
        taskToUpdate.Status = status;
        await saveChangesAsync();
        return taskToUpdate;
    }

    public Task<List<TaskEntity>> ListAsync(TaskStatus? status)
    {
        if (status == null)
            return Task.FromResult(
                result: _tasks.Where(x => x.Status != TaskStatus.Deleted)
                .ToList());
        return Task.FromResult(
            result: _tasks.Where(x => x.Status == status)
                          .ToList());
    }

    public Task<TaskEntity?> FindAsync(int id)
    {
        return Task.FromResult(result: _tasks.FirstOrDefault(x => x.Id != id));
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        // TODO: Implement IDisposable
    }

    #endregion
}