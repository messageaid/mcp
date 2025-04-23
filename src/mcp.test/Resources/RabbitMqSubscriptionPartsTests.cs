namespace mcp.test.Resources;

using mcp.RabbitMqManagement;
using mcp.Resources;

public class RabbitMqSubscriptionPartsTests
{
    [Test]
    public void EncodeDecode()
    {
        var b = new RabbitMqHttpBinding
        {
            Source = "src",
            DestinationType = "queue",
            Destination = "dest",
            RoutingKey = ""
        };
        
        var str = RabbitMqSubscriptionParts.EncodeRabbitMqParts(b);

        var bits = RabbitMqSubscriptionParts.DecodeRabbitMqParts(str);
        
        Assert.That(bits.Source, Is.EqualTo("src"));
        Assert.That(bits.DestinationType, Is.EqualTo("queue"));
        Assert.That(bits.Destination, Is.EqualTo("dest"));
        Assert.That(bits.PropertiesKey, Is.EqualTo(""));
    }
    
    [Test]
    public void EncodeDecode_WithKey()
    {
        var b = new RabbitMqHttpBinding
        {
            Source = "src",
            DestinationType = "queue",
            Destination = "dest",
            PropertiesKey = "key"
        };
        
        var str = RabbitMqSubscriptionParts.EncodeRabbitMqParts(b);

        var bits = RabbitMqSubscriptionParts.DecodeRabbitMqParts(str);
        
        Assert.That(bits.Source, Is.EqualTo("src"));
        Assert.That(bits.DestinationType, Is.EqualTo("queue"));
        Assert.That(bits.Destination, Is.EqualTo("dest"));
        Assert.That(bits.PropertiesKey, Is.EqualTo("key"));
    }
}