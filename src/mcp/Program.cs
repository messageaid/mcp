using mcp;
using mcp.RabbitMqManagement;
using ModelContextProtocol.Protocol.Types;
using Serilog;
using Serilog.Events;

// This creates a global logger that can be used during
// bootstrap, but can later be configured using the host
// builder
Log.Logger = new LoggerConfiguration()
    // STDIO ISSUE
    // Configure all logs to go to stderr
    .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose)
    .CreateBootstrapLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog(cfg =>
{
    // STDIO ISSUE
    // Configure all logs to go to stderr
    cfg.WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose);
});

builder.Services.AddOptions<McpOptions>()
    .Configure(o =>
    {
        // TODO: Validate the scheme of the URL
        o.Url = Environment.GetEnvironmentVariable("BROKER_URL");
        
        Log.Information("URL = {Url}", o.Url);
        // If in Docker, help the user by changing "LOCALHOST" to 
        // MacOS / Windows use `host.docker.internal`
        // Linux use `172.17.0.1`
        var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        if (inDocker)
        {
            Log.Information("In Docker");
            if (o.Url is { } url)
            {
                // TODO: Make this a bit smarter
                if (url.Contains("localhost"))
                {
                    o.Url = url.Replace("localhost", "host.docker.internal");
                }
            }
        }
        
        if(Enum.TryParse<McpMode>(Environment.GetEnvironmentVariable("MCP_MODE"), out var e))
        {
            o.Mode = e;
        }
    }).Validate(o => !string.IsNullOrWhiteSpace(o.Url), "BROKER_URL was not supplied")
    .ValidateOnStart();

builder.Services.AddHttpClient<RabbitMqManagementClient>();

builder.Services
    .AddMcpServer(o =>
    {
        o.ServerInfo = new Implementation
        {
            Name = "Message Aid MCP",
            Version = "1.0"
        };
    })
    .WithStdioServerTransport()
    .WithReadResourceHandler(ResourceRouter.ReadResource)
    .WithListResourcesHandler(ResourceRouter.ListResources)
    .WithListResourceTemplatesHandler(ResourceRouter.ListResourceTemplates)
    .WithPromptsFromAssembly()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();