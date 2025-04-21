namespace mcp.test;

using mcp.RabbitMqManagement;
using Microsoft.Extensions.Options;
using NUnit.Framework.Constraints;

public class RabbitMqManagementClientTests
{
    [Test]
    public async Task ListQueues()
    {
        var httpClient = new RabbitMqManagementClient(new HttpClient(), new OptionsWrapper<McpOptions>(
            new McpOptions()
            {
                Url = "rabbitmq://guest:guest@localhost:15672/"
            }));

        var factory = new RabbitMQ.Client.ConnectionFactory();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        await using var conn = await factory.CreateConnectionAsync();
        await using var channel = await conn.CreateChannelAsync();
        await channel.QueueDeclareAsync("test", true, false,
            false);
        
        var queues = httpClient.Queues();

        var count = 0;
        await foreach (var q in queues)
        {
            count += 1;
        }
        Assert.That(count, Is.EqualTo(1));
    }
}