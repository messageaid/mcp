using System.ComponentModel;
using mcp.RabbitMqManagement;
using ModelContextProtocol.Server;

[McpServerToolType]
public static class QueueTools
{
    [McpServerTool(Destructive = true), Description("Purges a queue")]
    public static async Task<string> Purge(IMcpServer server, string queue)
    {
        var client = server.Services!.GetRequiredService<RabbitMqManagementClient>();
        await client.PurgeQueue(queue);
        
        return $"purged {queue}";
    }
}