namespace mcp.Brokers;

using mcp.RabbitMqManagement;
using mcp.Resources;
using Microsoft.Extensions.Options;

public class RabbitMqCacheBackgroundJob(
    IServiceProvider provider,
    ILogger<RabbitMqCacheBackgroundJob> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = provider.CreateScope();
        // TODO: Eventually support multiple HOSTs (aka brokers)
        var client = scope.ServiceProvider.GetRequiredService<RabbitMqManagementClient>();
        var cache = scope.ServiceProvider.GetRequiredService<RabbitMqItemCache>();
        var opt = scope.ServiceProvider.GetRequiredService<IOptions<McpOptions>>();
        
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
        
        try
        {
            await foreach (var binding in client.Bindings(stoppingToken))
            {
                var name = RabbitMqSubscriptionParts.BindingName(binding);
                var key = RabbitMqSubscriptionParts.EncodeRabbitMqParts(binding);
                var u = new Uri(SubscriptionUriTemplate.Create(opt.Value.Hostname(), key));
                cache.Ensure(ResourceTypes.Subscription, name, u);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error Listing Topics");
        }

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

    }
}