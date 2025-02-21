using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using TaskTracker.CLI.Commands;
using TaskTracker.CLI.Infrastructure;
using TaskTracker.Core.Repository;

namespace TaskTracker.CLI;

public static class Program
{
    public static int Main(string[] args)
    {
        ServiceCollection? registrations = new();
        registrations.AddSingleton<ITaskRepository, JsonTaskRepository>(
            implementationFactory: _ => new JsonTaskRepository(Environment.CurrentDirectory)
        );

        TypeRegistrar registrar = new(registrations);
        CommandApp app = new(registrar);
        app.Configure(config =>
        {
            config.SetApplicationName("ttrack");

            config.AddCommand<AddCommand>(name: "add")
                .WithDescription("Adda a new task")
                .WithExample(["add", "Task 1"]);

            config.AddCommand<ListCommand>(name: "list")
                .WithDescription("List tasks")
                .WithExample(["list"]);

            config.AddCommand<UpdateCommand>(name: "update")
                .WithDescription("Updates a task")
                .WithExample(["update", "1", "Task 1"]);

            config.AddCommand<DeleteCommand>(name: "delete")
                .WithDescription("Deletes a task")
                .WithExample(["delete", "1"]);

            config.AddCommand<MarkCommand>(name: "mark")
                .WithDescription("Changes the status of a task")
                .WithExample(["mark", "1", "done"]);
        });

        return app.Run(args);
    }
}
