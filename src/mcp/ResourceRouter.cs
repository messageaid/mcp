namespace mcp;

using System.Text.Json;
using mcp.Brokers;
using mcp.RabbitMqManagement;
using mcp.Resources;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

public static class ResourceRouter
{
    public static async ValueTask<ReadResourceResult> ReadResource(
        RequestContext<ReadResourceRequestParams> context,
        CancellationToken ct = default)
    {
        var logger = context.GetLogger();
        
        var client = context.Services!.GetRequiredService<RabbitMqManagementClient>();

        var contents = new List<ResourceContents>();

        if (QueueUriTemplate.Parse(context.Params?.Uri) is { } qu)
        {
            logger.LogInformation("Read Resource - Queue - {Uri}", context.Params?.Uri);
            
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

        if (TopicUriTemplate.Parse(context.Params?.Uri) is { } tu)
        {
            logger.LogInformation("Read Resource - Topic - {Uri}", context.Params?.Uri);
            
            var t = await client.GetExchange(tu.Name, ct);
            if (t != null)
            {
                var str = JsonSerializer.Serialize(t);

                contents.Add(new TextResourceContents()
                {
                    MimeType = "application/json",
                    Uri = tu.Uri,
                    Text = str
                });
            }
        }
        
        if (SubscriptionUriTemplate.Parse(context.Params?.Uri) is { } su)
        {
            logger.LogInformation("Read Resource - Binding - {Uri}", context.Params?.Uri);
            
            var parts = RabbitMqSubscriptionParts.DecodeRabbitMqParts(su.Name);
            var ex = await client.GetBinding(parts.Source,
                parts.DestinationType,
                parts.Destination,
                parts.PropertiesKey, ct);
            if (ex != null)
            {
                var str = JsonSerializer.Serialize(ex);

                contents.Add(new TextResourceContents()
                {
                    MimeType = "application/json",
                    Uri = su.Uri,
                    Text = str
                });
            }
        }

        // 404?
        if(contents.Count == 0)
            throw new McpException("Resource not found", -32002);

        return new ReadResourceResult()
        {
            Contents = contents,
        };
    }

    public static async ValueTask<ListResourcesResult> ListResources(
        RequestContext<ListResourcesRequestParams> context,
        CancellationToken ct = default)
    {
        await Task.Yield();
        
        var cursor = PaginationCursor.Extract(context.Params?.Cursor);
        var cache = context.Services!.GetRequiredService<RabbitMqItemCache>();
        
        var resources = new List<Resource>();

        var (items, nextCursor) = cache.NextPage(cursor);
        foreach (var item in items)
        {
            resources.Add(new Resource
            {
                Name = item.Name,
                Uri = item.Uri.ToString()
            });
        }
        
        if (resources.Count < cursor.PerPage)
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
                },
                new ResourceTemplate
                {
                    Name = "A Topic",
                    UriTemplate = TopicUriTemplate.Template,
                    Description = "Get Topic By Name",
                    MimeType = "application/json"
                }
            ]
        };
    }
}