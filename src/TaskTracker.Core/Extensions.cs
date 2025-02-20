namespace TaskTracker.Core;

public static class Extensions
{
    public static string GetStringValue(this Core.TaskStatus status) => status switch
    {
        TaskStatus.Todo => "todo",
        TaskStatus.InProgress => "in-progress",
        TaskStatus.Done => "done",
        _ => "-",
    };
}
