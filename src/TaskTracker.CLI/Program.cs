using TaskTracker.Core;

namespace TaskTracker.CLI;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            help(args);
            return 0;
        }

        // TODO: Check the data integrity of the file.

        switch (args[0])
        {
            case "-h":
            case "--help":
                help(args);
                return 0;
            case "add":
                return add(args);
            case "update":
                return update(args);
            case "delete":
                return delete(args);
            case "list":
                return list(args);
            case "mark-in-progress":
                return markAs(args, Core.TaskStatus.InProgress);
            case "mark-done":
                return markAs(args, Core.TaskStatus.Done);
            case "mark-todo":
                return markAs(args, Core.TaskStatus.Todo);
            default:
                Console.WriteLine($"Invalid command {args[0]}");
                help(args);
                return 1;
        }
    }

    #region Private Methods

    private static void help(string[] args)
    {
        // TODO: Add help functionality.
    }

    private static int add(string[] args)
    {
        if (args.Length != 2)
        {
            printInvalidArgs(args);
            return 1;
        }
        string title = args[1];
        using ITaskRepository taskRepository = new JsonTaskRepository(Environment.CurrentDirectory);
        Task<TaskEntity?> addTask = taskRepository.AddAsync(title);
        addTask.Wait();
        if (addTask.Result == null)
        {
            Console.WriteLine("Unable to add to the list");
            return 1;
        }
        Console.WriteLine($"Task added successfully (ID: {addTask.Result.Id})");
        return 0;
    }

    private static int update(string[] args)
    {
        int id = -1;
        if (args.Length != 3 || !int.TryParse(args[1], out id))
        {
            printInvalidArgs(args);
            return 1;
        }
        string title = args[2];
        using ITaskRepository taskRepository = new JsonTaskRepository(workingFolder: Environment.CurrentDirectory);
        Task<TaskEntity?> updateTask = taskRepository.UpdateAsync(id, title);
        updateTask.Wait();
        if (updateTask.Result == null)
        {
            Console.WriteLine($"Unable to find task with ID:{id}");
            return 1;
        }
        Console.WriteLine("Task updated successfully");
        Console.WriteLine($"ID: {id}, TITLE: {title}");
        return 0;
    }

    private static int delete(string[] args)
    {
        int id = -1;
        if (args.Length != 2 || !int.TryParse(args[1], out id))
        {
            printInvalidArgs(args);
            return 1;
        }
        using ITaskRepository taskRepository = new JsonTaskRepository(workingFolder: Environment.CurrentDirectory);
        Task<bool?> deleteTask = taskRepository.DeleteAsync(id);
        deleteTask.Wait();
        if (deleteTask.Result == null || deleteTask.Result == false)
        {
            Console.WriteLine($"Unable to find task with ID:{id}");
            return 1;
        }
        Console.WriteLine($"Task deleted successfully ID: {id}");
        return 0;
    }

    private static int list(string[] args)
    {
        if (args.Length < 1)
        {
            printInvalidArgs(args);
            return 1;
        }
        Core.TaskStatus? status;
        if (args.Length == 1) status = null;
        else status = args[1] switch
        {
            "todo" => (Core.TaskStatus?)Core.TaskStatus.Todo,
            "done" => (Core.TaskStatus?)Core.TaskStatus.Done,
            "in-progress" => (Core.TaskStatus?)Core.TaskStatus.InProgress,
            null or "" or _ => null,
        };
        ITaskRepository taskRepository = new JsonTaskRepository(Environment.CurrentDirectory);
        if (args.Length != 1 && int.TryParse(args[1], out int id))
        {
            Task<TaskEntity?> findEntityTask = taskRepository.FindAsync(id);
            if (findEntityTask.Result == null)
            {
                Console.WriteLine($"Unable to find task with ID: {id}");
                return 1;
            }
            TaskEntity entity = findEntityTask.Result;
            Console.WriteLine($"ID: {entity.Id}\nSTATUS: {entity.Status.GetStringValue()}\nTITLE: {entity.Title}");
            return 0;
        }
        else
        {
            Task<List<TaskEntity>> listTask = taskRepository.ListAsync(status);
            if (listTask.Result.Count == 0)
            {
                Console.WriteLine("No task on the list.");
                return 0;
            }
            Console.WriteLine("|-----|-------------|----------------------------------|");
            string format = "| {0,3} | {1,-11} | {2,-32} |";
            Console.WriteLine(string.Format(format, "ID", "STATUS", "TASK"));
            Console.WriteLine("|-----|-------------|----------------------------------|");
            foreach (TaskEntity item in listTask.Result)
            {
                string title = item.Title.Length > 32 ? item.Title[..29] + "..." : item.Title;
                string statusStr = item.Status.GetStringValue();

                Console.WriteLine(string.Format(format, item.Id, statusStr, title));
            }
            Console.WriteLine("|-----|-------------|----------------------------------|");
            return 0;
        }
    }

    private static int markAs(string[] args, Core.TaskStatus status)
    {
        int id = -1;
        if (args.Length != 2 || !int.TryParse(args[1], out id))
        {
            printInvalidArgs(args);
            return 1;
        }
        ITaskRepository taskRepository = new JsonTaskRepository(Environment.CurrentDirectory);
        Task<TaskEntity?> markAsTask = taskRepository.MarkAsAsync(id, status);
        if (markAsTask.Result == null)
        {
            Console.WriteLine($"Unable to find task with ID: {id}");
            return 1;
        }
        Console.WriteLine($"ID: {id} succssfully marked as {status.GetStringValue()}");
        return 0;
    }

    private static void printInvalidArgs(string[] args)
    {
        string m = string.Join(",", args.Skip(1));
        Console.WriteLine($"Invalid argument(s) {m}");
    }

    #endregion
}
