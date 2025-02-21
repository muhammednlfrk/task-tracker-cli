using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.Core;

namespace TaskTracker.CLI.Commands;

public class UpdateCommand(ITaskRepository? taskRepository) : AsyncCommand<UpdateCommandSettings>
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] UpdateCommandSettings settings)
    {
        TaskEntity? updatedTask = await _taskRepository.UpdateAsync(settings.Id, settings.Title ?? string.Empty);

        if (updatedTask == null)
        {
            AnsiConsole.MarkupLine($"[red]Unable to find task with ID:[/]{settings.Id}");
            return 1;
        }

        AnsiConsole.MarkupLine($"Task updated successfully");
        AnsiConsole.MarkupLine($"ID: [bold]{updatedTask.Id}[/]");
        AnsiConsole.MarkupLine($"Status: {updatedTask.Status}");
        AnsiConsole.MarkupLine($"Title: {updatedTask.Title}");
        return 0;
    }
}

public class UpdateCommandSettings : CommandSettings
{
    [Description("ID of the task to update")]
    [CommandArgument(0, "[ID]")]
    public int Id { get; set; }

    [Description("New title of the task")]
    [CommandArgument(1, "[Title]")]
    public string? Title { get; set; }

    public override ValidationResult Validate()
    {
        if (Id < 0)
            return ValidationResult.Error("ID must be a positive number");

        if (string.IsNullOrWhiteSpace(Title))
            return ValidationResult.Error("Title must not be empty");

        return ValidationResult.Success();
    }
}
