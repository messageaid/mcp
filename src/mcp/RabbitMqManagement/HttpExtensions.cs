namespace mcp.RabbitMqManagement;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

public static class HttpExtensions
{
    static readonly JsonSerializerOptions _options;

    static HttpExtensions()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new RabbitMqDateTimeJsonConverter());
        _options.PropertyNamingPolicy = new BoxCarCaseNamingPolicy();    
    }
    
    /// <summary>
    /// A simple get
    /// </summary>
    public static async Task<T?> SimpleGet<T>(this HttpClient http, string path, RabbitPagination pagination, CancellationToken ct = default)
        where T : class
    {
        var uri = QueryHelpers.AddQueryString(path, pagination.ToQueryString());

        var msg = new HttpRequestMessage(HttpMethod.Get, uri);


        var response = await http.SendAsync(msg, ct);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new HttpRequestException($"Unauthorized: {path}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new HttpRequestException($"Couldn't find {path}");


        await using var stream = await response.Content.ReadAsStreamAsync(ct);

        try
        {
            var payload = JsonSerializer.Deserialize<T>(stream, _options);
            return payload;
        }
        catch (JsonException)
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var str = await reader.ReadToEndAsync(ct);
            
            throw;
        }
    }
    
    /// <summary>
    /// A simple get
    /// </summary>
    public static Task<T?> SimpleNullableGet<T>(this HttpClient http, string path, CancellationToken ct = default)
        where T : class
    {
        return http.SimpleNullableGet<T>(path, RabbitPagination.Default(), ct);
    }
    
    /// <summary>
    /// A simple get
    /// </summary>
    public static async Task<T?> SimpleNullableGet<T>(this HttpClient http, string path, RabbitPagination pagination, CancellationToken ct = default)
        where T : class
    {
        var uri = QueryHelpers.AddQueryString(path, pagination.ToQueryString());

        var msg = new HttpRequestMessage(HttpMethod.Get, uri);


        var response = await http.SendAsync(msg, ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);
        var payload = JsonSerializer.Deserialize<T>(stream, _options);
        return payload;
    }
    
    /// <summary>
    /// delete
    /// </summary>
    public static async Task SimpleDelete(this HttpClient client, string path)
    {
        using var response = await client.DeleteAsync(path);
        // using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
        // using var jsonReader = new JsonTextReader(reader);
        // var serializer = new JsonSerializer();
        // var payload = serializer.Deserialize<TOutput>(jsonReader);
        // return payload;
    }
}