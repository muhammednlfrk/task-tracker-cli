using System.Threading.Tasks;

namespace TaskTracker.Core.Repository;

public interface ITaskRepository : IDisposable
{
    Task<TaskEntity?> AddAsync(string title);
    Task<TaskEntity?> UpdateAsync(int id, string title);
    Task<bool?> DeleteAsync(int id);
    Task<TaskEntity?> MarkAsAsync(int id, TaskStatus status);
    Task<List<TaskEntity>> ListAsync(TaskStatus? status);
    Task<TaskEntity?> FindAsync(int id);
}
