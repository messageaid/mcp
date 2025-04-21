namespace mcp;

using ModelContextProtocol.Server;

public static class Extensions
{
    public static ILogger GetLogger<T>(this RequestContext<T> context)
    {
        var loggerFactory = context.Services!.GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger(typeof(T).FullName!);
    }
}