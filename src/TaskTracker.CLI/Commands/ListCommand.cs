using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.CLI.Infrastructure;
using TaskTracker.Core;

namespace TaskTracker.CLI.Commands;

public class ListCommand(ITaskRepository taskRepository) : AsyncCommand<ListCommandSettings>
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] ListCommandSettings settings)
    {
        List<TaskEntity> tasks = await _taskRepository.ListAsync(settings.Status);

        if (tasks.Count == 0)
        {
            AnsiConsole.MarkupLine("No tasks found");
            return 0;
        }

        Table table = new();
        table.AddColumn("#");
        table.Columns[0].RightAligned();
        table.Columns[0].Padding(1, 1);
        table.Columns[0].Width(3);
        table.AddColumn("STATUS");
        table.Columns[1].LeftAligned();
        table.Columns[1].PadLeft(1);
        table.AddColumn("TASK");
        table.Columns[2].LeftAligned();
        table.Columns[2].PadLeft(1);
        foreach (TaskEntity entity in tasks)
            table.AddRow(entity.Id.ToString(), entity.Status.GetStringValue(), entity.Title);
        table.Border(TableBorder.AsciiDoubleHead);
        table.Collapse();
        AnsiConsole.Write(table);

        return 0;
    }
}

public class ListCommandSettings : CommandSettings
{
    [Description("Status of the task (optional) [todo | done | in-progress]")]
    [CommandArgument(0, "[status]")]
    [TypeConverter(typeof(TaskStatusEnumConverter))]
    public Core.TaskStatus? Status { get; set; } = null;
}
