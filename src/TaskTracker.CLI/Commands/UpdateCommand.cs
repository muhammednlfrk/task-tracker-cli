using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.Core;
using TaskTracker.Core.Repository;

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

        AnsiConsole.MarkupLine($"Task updated successfully :person_raising_hand:");
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
    public required string Id { get; set; }

    [Description("New title of the task")]
    [CommandArgument(1, "[TITLE]")]
    public string? Title { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return ValidationResult.Error("ID is required");

        if (string.IsNullOrWhiteSpace(Title))
            return ValidationResult.Error("Title must not be empty");

        return ValidationResult.Success();
    }
}
