using System.ComponentModel;
using System.Globalization;

namespace TaskTracker.CLI.Infrastructure;

public class TaskStatusEnumConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string str)
        {
            return str switch
            {
                "todo" => Core.TaskStatus.Todo,
                "done" => Core.TaskStatus.Done,
                "in-progress" => Core.TaskStatus.InProgress,
                _ => throw new ArgumentException("Invalid status format")
            };
        }
        else if (value is int)
        {
            return null;
        }
        throw new ArgumentException("Invalid priority format");
    }
}