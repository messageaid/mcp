namespace mcp;

using mcp.RabbitMqManagement;
using Microsoft.Extensions.Options;

public class RabbitMqCacheBackgroundJob(
    IServiceProvider provider,
    ILogger<RabbitMqCacheBackgroundJob> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = provider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<RabbitMqManagementClient>();
        var cache = scope.ServiceProvider.GetRequiredService<RabbitMqItemCache>();
        var opt = scope.ServiceProvider.GetRequiredService<IOptions<McpOptions>>();

        logger.LogInformation("BG JOB");
        try
        {
            await foreach (var queue in client.Queues(stoppingToken))
            {
                var u = new Uri(QueueUriTemplate.Create(opt.Value.Hostname(), queue.Name));
                cache.Ensure(ResourceTypes.Queue, queue.Name, u);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Listing Queues");
        }


        try
        {
            await foreach (var exchange in client.Exchanges(stoppingToken))
            {
                var u = new Uri(TopicUriTemplate.Create(opt.Value.Hostname(), exchange.Name));
                cache.Ensure(ResourceTypes.Topic, exchange.Name, u);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Listing Topics");
        }

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

    }
}