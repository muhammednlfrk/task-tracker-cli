using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.CLI.Infrastructure;
using TaskTracker.Core;

namespace TaskTracker.CLI.Commands;

public class MarkCommand(ITaskRepository taskRepository) : AsyncCommand<MarkCommandSettings>
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync(CommandContext context, MarkCommandSettings settings)
    {
        TaskEntity? task = await _taskRepository.MarkAsAsync(settings.Id, settings.Status);
        if (task == null)
        {
            AnsiConsole.MarkupLine($"[red]Unable to find task with ID: {settings.Id}[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"Task [bold]{task.Id}[/] marked as [bold]{task.Status.GetStringValue()}[/]");
        return 0;
    }
}

public class MarkCommandSettings : CommandSettings
{
    [Description("Id of the task to mark")]
    [CommandArgument(0, "[ID]")]
    public int Id { get; set; }

    [Description("Status to mark the task as")]
    [CommandArgument(1, "[status]")]
    [TypeConverter(typeof(TaskStatusEnumConverter))]
    public Core.TaskStatus Status { get; set; }
}
