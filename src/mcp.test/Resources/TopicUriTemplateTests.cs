namespace mcp.test.Resources;

using Corvus.UriTemplates;
using mcp.Resources;

public class TopicUriTemplateTests
{
    [Test]
    public void UrlTemplateParsing()
    {
        var validTopic = TopicUriTemplate.Create("localhost", "bill");
        
        var topicParser = TopicUriTemplate.Parser;
        Assert.That(topicParser.IsMatch(validTopic), Is.True);
        
        Assert.That(topicParser.TryGetUriTemplateParameters(validTopic, 2, out var p), Is.True);
        Assert.That(p!.TryGet("name", out var parameter), Is.True);
        var value = parameter.GetValue(validTopic);
        Assert.That(value.ToString(), Is.EqualTo("bill"));
    }
}