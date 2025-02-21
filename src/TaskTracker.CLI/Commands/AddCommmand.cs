using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.Core;
using TaskTracker.Core.Repository;

namespace TaskTracker.CLI.Commands;

public class AddCommand(ITaskRepository taskRepository) : AsyncCommand<AddCommandSettings>()
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] AddCommandSettings settings)
    {
        TaskEntity? addedTask = await _taskRepository.AddAsync(settings.Title);
        if (addedTask == null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Unable to add task");
            return 1;
        }
        AnsiConsole.MarkupLine($"Task added successfully :call_me_hand: ID: {addedTask.Id}");
        return 0;
    }
}

public class AddCommandSettings : CommandSettings
{
    [Description("Title of the task")]
    [CommandArgument(0, "[title]")]
    public required string Title { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(Title))
            return ValidationResult.Error("Title is required");
            
        if (Title.Length < 3)
            return ValidationResult.Error("Task title must be at least 3 characters long!");

        return ValidationResult.Success();
    }
}