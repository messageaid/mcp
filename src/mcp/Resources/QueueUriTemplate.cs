namespace mcp;

using Corvus.UriTemplates;
using Corvus.UriTemplates.TavisApi;

public static class QueueUriTemplate
{
    static QueueUriTemplate()
    {
        Parser = UriTemplateParserFactory.CreateParser(Template);
    }
    
    public static IUriTemplateParser Parser
    {
        get;
    }
    
    public static readonly string Template = "rabbitmq://{hostname}/queues/{name}";
    public static readonly UriTemplate Template2 = new(Template);

    public static bool IsMatch(string? input) => Parser.IsMatch(input);

    public static string Create(string hostname, string name)
    {
        return $"rabbitmq://{hostname}/queues/{name}";
    }

    public static QueueUri? Parse(string? input)
    {
        if (input == null) return null;
        if (!Parser.IsMatch(input)) return null;

        var dict = Template2.GetParameters(new Uri(input));
        if (dict == null) return null;

        if (!dict.TryGetValue("name", out var value)) return null;
        
        var str = value.ToString()!;
        return new QueueUri(str, input);
    }
}

public record QueueUri(string Name, string Uri);