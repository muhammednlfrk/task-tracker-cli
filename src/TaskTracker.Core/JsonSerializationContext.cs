using System.Text.Json.Serialization;

namespace TaskTracker.Core;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<TaskEntity>))]
[JsonSerializable(typeof(TaskEntity))]
internal partial class JsonSerializationContext : JsonSerializerContext { }
