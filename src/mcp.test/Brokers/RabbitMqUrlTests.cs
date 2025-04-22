namespace mcp.test;

public class RabbitMqUrlTests
{
    [Test]
    public void Simple()
    {
        var u = new Uri("rabbitmq://localhost:15672");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("%2f"));
        Assert.That(r.Uri.Port, Is.EqualTo(15672));
        Assert.That(r.Uri.Scheme, Is.EqualTo("http"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.Null);
    }
    
    [Test]
    public void Abc()
    {
        var u = new Uri("rabbitmq://localhost/");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("%2f"));
        Assert.That(r.Uri.Port, Is.EqualTo(80));
        Assert.That(r.Uri.Scheme, Is.EqualTo("http"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.Null);
    }
    
    [Test]
    public void Abc2()
    {
        var u = new Uri("rabbitmq://localhost/mt-demo");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("mt-demo"));
        Assert.That(r.Uri.Port, Is.EqualTo(80));
        Assert.That(r.Uri.Scheme, Is.EqualTo("http"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.Null);
    }
    
    [Test]
    public void Abc22()
    {
        var u = new Uri("rabbitmqs://localhost/mt-demo");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("mt-demo"));
        Assert.That(r.Uri.Port, Is.EqualTo(443));
        Assert.That(r.Uri.Scheme, Is.EqualTo("https"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.Null);
    }
    
    [Test]
    public void Abc24()
    {
        var u = new Uri("rabbitmq://localhost:15672/mt-demo");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("mt-demo"));
        Assert.That(r.Uri.Port, Is.EqualTo(15672));
        Assert.That(r.Uri.Scheme, Is.EqualTo("http"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.Null);
    }
    
    [Test]
    public void Def()
    {
        var u = new Uri("rabbitmq://guest:guest@localhost:15672/mt-demo");
        var r = RabbitMqUrlConverter.Convert(u);
        
        Assert.That(r.VirtualHost, Is.EqualTo("mt-demo"));
        Assert.That(r.Uri.Port, Is.EqualTo(15672));
        Assert.That(r.Uri.Scheme, Is.EqualTo("http"));
        Assert.That(r.Uri.Host, Is.EqualTo("localhost"));
        Assert.That(r.Uri.UserInfo, Is.EqualTo(""));
        
        Assert.That(r.BasicCredentials, Is.EqualTo("guest:guest"));
    }
}