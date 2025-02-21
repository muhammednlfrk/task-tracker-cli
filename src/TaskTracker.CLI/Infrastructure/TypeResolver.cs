using Spectre.Console.Cli;

namespace TaskTracker.CLI.Infrastructure;

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public object? Resolve(Type? type)
    {
        if (type == null) return null;
        return _provider.GetService(type);
    }
}