using System.Text.Json;
using Corvus.UriTemplates;
using mcp.RabbitMqManagement;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace mcp;

public static class ResourceRouter
{
    public static async ValueTask<ReadResourceResult> ReadResource(
        RequestContext<ReadResourceRequestParams> context,
        CancellationToken ct = default)
    {
        var logger = context.GetLogger();
        logger.LogInformation("Read Resource");
        
        var client = context.Services!.GetRequiredService<RabbitMqManagementClient>();

        var contents = new List<ResourceContents>();

        var qu = QueueUriTemplate.Parse(context.Params?.Uri);
        
        if (qu != null)
        {
            var q = await client.GetQueue(qu.Name, ct);
            if (q != null)
            {
                var str = JsonSerializer.Serialize(q);
                    
                contents.Add(new TextResourceContents()
                {
                    MimeType = "application/json",
                    Uri = qu.Uri,
                    Text = str
                });
            }    
        }
        
        
        // 404?
        return new ReadResourceResult()
        {
            Contents = contents
        };
    }

    public static async ValueTask<ListResourcesResult> ListResources(
        RequestContext<ListResourcesRequestParams> context,
        CancellationToken ct = default)
    {
        var (page, perPage) = PaginationCursor.Extract(context.Params?.Cursor);
        
        var logger = context.GetLogger();
        logger.LogInformation("List Resources");

        var client = context.Services!.GetRequiredService<RabbitMqManagementClient>();

        var resources = new List<Resource>();

        await foreach (var q in client.Queues(page, perPage, ct))
        {
            resources.Add(new Resource
            {
                Name = q.Name,
                Uri = $"rabbitmq://localhost/queues/{q.Name}"
            });
        }

        var nextCursor = PaginationCursor.Encode(page + 1, perPage);
        if (resources.Count < perPage)
            nextCursor = null;
        
        return new ListResourcesResult
        {
            Resources = resources,
            NextCursor = nextCursor,
        };
    }

    public static async ValueTask<ListResourceTemplatesResult> ListResourceTemplates(
        RequestContext<ListResourceTemplatesRequestParams> context,
        CancellationToken ct = default)
    {
        var logger = context.GetLogger();
        logger.LogInformation("List Resource Templates");
        
        await Task.Yield();

        return new ListResourceTemplatesResult
        {
            ResourceTemplates = [
                new ResourceTemplate
                {
                    Name = "A Queue",
                    UriTemplate = QueueUriTemplate.Template,
                    Description = "Get Queue By Name",
                    MimeType = "application/json"
                }
            ]
        };
    }
}