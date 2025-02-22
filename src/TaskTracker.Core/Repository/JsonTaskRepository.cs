using System.Text;
using System.Text.Json;

namespace TaskTracker.Core.Repository;

public class JsonTaskRepository : ITaskRepository
{
    private const string JSON_FILE_NAME = ".tasks.json";
    private const string TEMP_FILE_NAME = ".tasks.temp.json";

    private readonly string _jsonFile;
    private readonly string _tempFile;
    private readonly Encoding _jsonEncoding;
    private List<TaskEntity> _tasks;

    public JsonTaskRepository(string workingFolder)
    {
        _tasks = [];

        _jsonFile = Path.Combine(workingFolder, JSON_FILE_NAME);
        _tempFile = Path.Combine(workingFolder, TEMP_FILE_NAME);
        _jsonEncoding = Encoding.Unicode;

        checkForTempFile();
        openOrCreateJsonFile();
    }

    #region Private Methods

    private void checkForTempFile()
    {
        FileInfo tempFileInfo = new(_tempFile);
        if (tempFileInfo.Exists)
        {
            if (tempFileInfo.Length > 0)
            {
                File.Replace(_tempFile, _jsonFile, null);
            }

            File.Delete(_tempFile);
        }
    }

    private void openOrCreateJsonFile()
    {
        using FileStream jsonFileStream = new FileStream(
            path: _jsonFile,
            mode: FileMode.OpenOrCreate,
            access: FileAccess.Read,
            share: FileShare.Read);

        FileAttributes attributes = File.GetAttributes(_jsonFile);
        File.SetAttributes(_jsonFile, attributes | FileAttributes.Hidden);

        using StreamReader jsonFileReader = new(jsonFileStream, encoding: _jsonEncoding);
        string json = jsonFileReader.ReadToEnd();

        if (string.IsNullOrEmpty(json))
        {
            _tasks = [];
        }
        else
        {
            _tasks = JsonSerializer.Deserialize<List<TaskEntity>>(
                json: json,
                jsonTypeInfo: JsonSerializationContext.Default.ListTaskEntity) ?? [];
        }
    }

    private async Task saveChangesAsync()
    {
        string json = JsonSerializer.Serialize(
            value: _tasks,
            jsonTypeInfo: JsonSerializationContext.Default.ListTaskEntity);
        byte[] bytes = _jsonEncoding.GetBytes(json);

        await File.WriteAllBytesAsync(_tempFile, bytes);
        File.Replace(_tempFile, _jsonFile, null);
    }

    #endregion

    #region ITaskRepository Implementation

    public Task<List<TaskEntity>> ListAsync(TaskStatus? status)
    {
        if (status == null)
        {
            return Task.FromResult(
                result: _tasks.Where(x => x.Status != TaskStatus.Deleted)
                .ToList());
        }

        return Task.FromResult(
            result: _tasks.Where(x => x.Status == status)
                          .ToList());
    }

    public Task<TaskEntity?> FindAsync(string id)
    {
        if (string.IsNullOrEmpty(id)) return Task.FromResult<TaskEntity?>(null);
        TaskEntity? result = _tasks.FirstOrDefault(
            x => (x.Id == id || x.Id[..10] == id) && x.Status != TaskStatus.Deleted);
        return Task.FromResult(result);
    }

    public async Task<TaskEntity?> AddAsync(string title)
    {
        TaskEntity taskToAdd = new()
        {
            Id = Guid.NewGuid().ToString().Replace("-", string.Empty),
            Title = title,
            Status = TaskStatus.Todo
        };
        _tasks.Add(taskToAdd);
        await saveChangesAsync();
        return taskToAdd;
    }

    public async Task<TaskEntity?> UpdateAsync(string id, string title)
    {
        TaskEntity? taskToUpdate = await FindAsync(id);
        if (taskToUpdate == null) return null;
        taskToUpdate.Title = title;
        await saveChangesAsync();
        return taskToUpdate;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        TaskEntity? taskToRemove = await FindAsync(id);
        if (taskToRemove == null) return null;
        taskToRemove.Status = TaskStatus.Deleted;
        await saveChangesAsync();
        return true;
    }

    public async Task<TaskEntity?> MarkAsAsync(string id, TaskStatus status)
    {
        if (status < TaskStatus.Todo || status > TaskStatus.Deleted) return null;
        TaskEntity? taskToUpdate = await FindAsync(id);
        if (taskToUpdate == null) return null;
        taskToUpdate.Status = status;
        await saveChangesAsync();
        return taskToUpdate;
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        File.Delete(_tempFile);
        _tasks.Clear();
        _tasks = null!;
        GC.SuppressFinalize(this);
    }

    #endregion
}