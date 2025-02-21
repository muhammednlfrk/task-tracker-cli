using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TaskTracker.CLI.Infrastructure;
using TaskTracker.Core;
using TaskTracker.Core.Repository;

namespace TaskTracker.CLI.Commands;

public class ListCommand(ITaskRepository taskRepository) : AsyncCommand<ListCommandSettings>
{
    private readonly ITaskRepository _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] ListCommandSettings settings)
    {
        List<TaskEntity> tasks = await _taskRepository.ListAsync(settings.Status);

        if (tasks.Count == 0)
        {
            AnsiConsole.MarkupLine("No tasks found :face_screaming_in_fear:");
            return 0;
        }

        Table table = new();
        table.AddColumn("#");
        table.Columns[0].RightAligned();
        table.Columns[0].Padding(1, 1);
        table.Columns[0].Width(settings.ListDetailed == true ? 32 : 10);
        table.AddColumn("STATUS");
        table.Columns[1].LeftAligned();
        table.Columns[1].PadLeft(1);
        table.AddColumn("TASK");
        table.Columns[2].LeftAligned();
        table.Columns[2].PadLeft(1);
        foreach (TaskEntity entity in tasks)
        {
            if (settings.ListDetailed == true)
            {
                table.AddRow(
                entity.Id,
                entity.Status.GetStringValue(),
                entity.Title);
            }
            else
            {
                table.AddRow(
                    entity.Id[..10],
                    entity.Status.GetStringValue(),
                    entity.Title);
            }
        }
        table.Collapse();
        table.Border(TableBorder.AsciiDoubleHead);
        AnsiConsole.Write(table);

        return 0;
    }
}

public class ListCommandSettings : CommandSettings
{
    [Description("Status of the task (optional) [todo | done | in-progress]")]
    [CommandArgument(0, "[STATUS]")]
    [TypeConverter(typeof(TaskStatusEnumConverter))]
    public Core.TaskStatus? Status { get; set; } = null;

    [Description("Lists detailed information")]
    [CommandOption("-d|--detailed")]
    public bool? ListDetailed { get; set; } = false;
}
