namespace mcp.RabbitMqManagement;

using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

public class RabbitMqManagementClient
{
    readonly HttpClient _http;
    readonly string _urlVirtualHost;
    readonly string _virtualHost;
    
    public RabbitMqManagementClient(HttpClient http, IOptions<McpOptions> options)
    {
        _http = http;

        var ru = RabbitMqUrlConverter.Convert(options.Value.Url!);
        var url = new Uri(options.Value.Url!);
        _http.BaseAddress = ru.Uri;
        _http.Timeout = TimeSpan.FromSeconds(10);
        
        _urlVirtualHost = ru.VirtualHost;
        _virtualHost = ru.VirtualHost;
        if (_virtualHost == "%2f")
            _virtualHost = "/";

        
        var ui = url.UserInfo ?? "guest:guest";
        var byteArray = Encoding.ASCII.GetBytes(ui);
        var b64 = Convert.ToBase64String(byteArray);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", b64);
    }

    /// <summary>
    /// get queues
    /// </summary>
    public IAsyncEnumerable<RabbitMqHttpQueue> Queues(CancellationToken ct = default)
    {
        return Queues(RabbitPagination.Default(), ct);
    }
    
    /// <summary>
    /// get queues
    /// </summary>
    public IAsyncEnumerable<RabbitMqHttpQueue> Queues(int page, int perPage, CancellationToken ct = default)
    {
        return Queues(new RabbitPagination
        {
            Page = page,
            PageSize = perPage,
        }, ct);
    }
    
    /// <summary>
    /// get queues
    /// </summary>
    public async IAsyncEnumerable<RabbitMqHttpQueue> Queues(RabbitPagination pagination, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var page = await _http.SimpleGet<RabbitMqHttpPaginationResponse<RabbitMqHttpQueue>>($"/api/queues/{_urlVirtualHost}", pagination, ct: ct);

        if (page == null)
            yield break;

        while (page.Page <= page.PageCount)
        {
            var queues = page.Items;

            // TODO: A concept of "SYSTEM" or "DEFAULT" queues (that are filtered out by default)
            foreach (var queue in queues.Where(element => !element.VHost.StartsWith("amq.")))
            {
                if (queue.VHost != _virtualHost)
                    continue;

                // 2025-02-27 .. RabbitMQ's list API is missing data points such as "Idle Since"
                var fullQueue = await GetQueue(queue.Name, ct);
                if (fullQueue != null)
                    yield return fullQueue;
                else
                    yield return queue;
            }

            pagination.Page += 1;

            if (page.Page >= page.PageCount)
                yield break;

            page = await _http.SimpleGet<RabbitMqHttpPaginationResponse<RabbitMqHttpQueue>>($"/api/queues/{_urlVirtualHost}", pagination, ct: ct);
            if (page == null)
                yield break;
        }
    }
    
    /// <summary>
    /// Get a queue
    /// </summary>
    public async Task<RabbitMqHttpQueue?> GetQueue(string name, CancellationToken ct = default)
    {
        return await _http.SimpleNullableGet<RabbitMqHttpQueue>($"/api/queues/{_urlVirtualHost}/{name}", ct);
    }

    public Task PurgeQueue(string name)
    {
        return PurgeQueue(_urlVirtualHost, name);
    }

    public async Task PurgeQueue(string vhost, string name)
    {
        // /api/queues/vhost/name/contents
        await _http.SimpleDelete($"/api/queues/{vhost}/{name}/contents");
    }
}

public class BoxCarCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var sb = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (i == 0)
            {
                sb.Append(char.ToLowerInvariant(c));
                continue;
            }

            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
                sb.Append(c);
        }

        return sb.ToString();
    }
}