namespace mcp.test.Resources;

using Corvus.UriTemplates;

public class QueueUriTemplateTests
{
    [Test]
    public void UrlTemplateParsing()
    {
        var validQueue = QueueUriTemplate.Create("localhost", "bill");
        
        var queueParser = QueueUriTemplate.Parser;
        Assert.That(queueParser.IsMatch(validQueue), Is.True);
        
        Assert.That(queueParser.TryGetUriTemplateParameters(validQueue, 2, out var p), Is.True);
        Assert.That(p.TryGet("name", out var parameter), Is.True);
        var value = parameter.GetValue(validQueue);
        Assert.That(value.ToString(), Is.EqualTo("bill"));
    }
}