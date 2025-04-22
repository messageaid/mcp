namespace mcp;

public static class RabbitMqUrlConverter
{
    public static RabbitMqUrl Convert(string input)
    {
        return Convert(new Uri(input));
    }

    public static RabbitMqUrl Convert(Uri url)
    {
        var scheme = url.Scheme == "rabbitmq" ? "http" : "https";

        var port = scheme switch
        {
            "http" => url.Port == -1 ? 80 : url.Port,
            "https" => url.Port == -1 ? 443 : url.Port,
            _ => 15673
        };
        
        
        
        var urb = new UriBuilder
        {
            Scheme = url.Scheme == "rabbitmq" ? "http" : "https",
            Host = url.Host,
            Port = port,
        };

        string? basicCredentials = null;
        if (url.UserInfo != "")
        {
            basicCredentials = url.UserInfo;
        }
        
        var vhost = url.PathAndQuery == "/" ? "%2f" : url.PathAndQuery[1..];

        return new RabbitMqUrl(urb.Uri, vhost, basicCredentials);
    }
}

public record RabbitMqUrl(Uri Uri, string VirtualHost, string? BasicCredentials);