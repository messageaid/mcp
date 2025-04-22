using System.Text.Json;
using Corvus.UriTemplates;
using mcp.RabbitMqManagement;
using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;

namespace mcp;

using ModelContextProtocol;

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

        if (QueueUriTemplate.Parse(context.Params?.Uri) is { } qu)
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

        if (TopicUriTemplate.Parse(context.Params?.Uri) is { } tu)
        {
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
        var logger = context.GetLogger();
        logger.LogInformation("ListResources");
        
        var cursor = PaginationCursor.Extract(context.Params?.Cursor);
        logger.LogInformation("GotCursor");
        
        var cache = context.Services!.GetRequiredService<RabbitMqItemCache>();
        logger.LogInformation("GotCache");
        
        var resources = new List<Resource>();

        var (items, nextCursor) = cache.NextPage(cursor);
        foreach (var item in items)
        {
            resources.Add(new Resource
            {
                Name = item.Name,
                Uri = item.ToUri()
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