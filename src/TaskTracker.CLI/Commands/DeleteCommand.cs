using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.Core;

namespace TaskTracker.CLI.Commands;

public class DeleteCommand(ITaskRepository taskRepository) : AsyncCommand<DeleteCommandSettings>
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull]DeleteCommandSettings settings)
    {
        bool? isDeleted = await _taskRepository.DeleteAsync(settings.Id);

        if (isDeleted == null || isDeleted == false)
        {
            AnsiConsole.MarkupLine($"[red]Unable to find task with ID:[/]{settings.Id}");
            return 1;
        }

        AnsiConsole.MarkupLine($"Task deleted successfully");
        return 0;
    }
}

public class DeleteCommandSettings : CommandSettings
{
    [Description("Id of the task to delete")]
    [CommandArgument(0, "[ID]")]
    public int Id { get; set; }
}
