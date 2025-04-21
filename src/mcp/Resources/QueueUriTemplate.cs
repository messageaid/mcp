namespace mcp;

using Corvus.UriTemplates;
using Corvus.UriTemplates.TavisApi;

public static class QueueUriTemplate
{
    static QueueUriTemplate()
    {
        Parser = UriTemplateParserFactory.CreateParser(Template.ToString());
    }
    
    public static IUriTemplateParser Parser
    {
        get;
    }
    
    public static readonly string Template = "rabbitmq://localhost/queues/{name}";
    public static readonly UriTemplate Template2 = new(Template);

    public static string Create(string name)
    {
        return $"rabbitmq://localhost/queues/{name}";
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