namespace TaskTracker.Core.Repository;

public interface ITaskRepository : IDisposable
{
    Task<TaskEntity?> AddAsync(string title);
    Task<TaskEntity?> UpdateAsync(string id, string title);
    Task<bool?> DeleteAsync(string id);
    Task<TaskEntity?> MarkAsAsync(string id, TaskStatus status);
    Task<List<TaskEntity>> ListAsync(TaskStatus? status);
    Task<TaskEntity?> FindAsync(string id);
}
