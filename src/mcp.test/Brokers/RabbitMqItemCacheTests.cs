namespace mcp.test;

using Microsoft.Extensions.Logging.Abstractions;
using Serilog;

public class RabbitMqItemCacheTests
{
    [Test]
    public void Null()
    {
        var l = new NullLogger<RabbitMqItemCache>();
        var cache = new RabbitMqItemCache(l);
        
        var items = cache.NextPage(new PaginationCursor(ResourceTypes.Queue, null, 20));
        
        Assert.That(items.Items.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void BasicOrder_Null()
    {
        var l = new NullLogger<RabbitMqItemCache>();
        var cache = new RabbitMqItemCache(l);
        
        cache.Ensure(ResourceTypes.Queue, "abc", new Uri("https://localhost"));
        cache.Ensure(ResourceTypes.Topic, "abc", new Uri("https://localhost"));

        var items = cache.NextPage(new PaginationCursor(ResourceTypes.Queue, null, 20));
        
        Assert.That(items.Items[0].ResourceType, Is.EqualTo(ResourceTypes.Queue));
        Assert.That(items.Items[0].Name, Is.EqualTo("abc"));
        
        Assert.That(items.Items[1].ResourceType, Is.EqualTo(ResourceTypes.Topic));
        Assert.That(items.Items[1].Name, Is.EqualTo("abc"));
    }
    
    [Test]
    public void BasicOrder_Empty()
    {
        var l = new NullLogger<RabbitMqItemCache>();
        var cache = new RabbitMqItemCache(l);
        
        cache.Ensure(ResourceTypes.Queue, "abc", new Uri("https://localhost"));
        cache.Ensure(ResourceTypes.Topic, "abc", new Uri("https://localhost"));

        var items = cache.NextPage(new PaginationCursor(ResourceTypes.Queue, "", 20));
        
        Assert.That(items.Items[0].ResourceType, Is.EqualTo(ResourceTypes.Queue));
        Assert.That(items.Items[0].Name, Is.EqualTo("abc"));
        
        Assert.That(items.Items[1].ResourceType, Is.EqualTo(ResourceTypes.Topic));
        Assert.That(items.Items[1].Name, Is.EqualTo("abc"));
    }
    
    [Test]
    public void BasicOrder_TopicsOnly()
    {
        var l = new NullLogger<RabbitMqItemCache>();
        var cache = new RabbitMqItemCache(l);
        
        cache.Ensure(ResourceTypes.Queue, "abc", new Uri("https://localhost"));
        cache.Ensure(ResourceTypes.Topic, "abc", new Uri("https://localhost"));
        cache.Ensure(ResourceTypes.Topic, "def", new Uri("https://localhost"));

        var items = cache.NextPage(new PaginationCursor(ResourceTypes.Topic, null, 20));
        
        Assert.That(items.Items[0].ResourceType, Is.EqualTo(ResourceTypes.Topic));
        Assert.That(items.Items[0].Name, Is.EqualTo("abc"));
        
        Assert.That(items.Items[1].ResourceType, Is.EqualTo(ResourceTypes.Topic));
        Assert.That(items.Items[1].Name, Is.EqualTo("def"));
    }
}