namespace Chaos.CommandInterceptor;

public class CommandDescriptor
{
    public required CommandAttribute Details { get; init; }
    public required Type Type { get; init; }
}