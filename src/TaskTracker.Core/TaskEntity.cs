namespace TaskTracker.Core;

public sealed class TaskEntity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public TaskStatus Status { get; set; }
}

public enum TaskStatus : int
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Deleted = 3
}
