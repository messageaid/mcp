namespace mcp.Resources;

using System.Text;
using mcp.RabbitMqManagement;

public record RabbitMqSubscriptionParts(string Source, string DestinationType, string Destination, string PropertiesKey)
{
    public static string BindingName(RabbitMqHttpBinding binding)
    {
        return $"{binding.Source}:{binding.DestinationType}:{binding.Destination}:{binding.PropertiesKey}";
    }
    
    public static string EncodeRabbitMqParts(RabbitMqHttpBinding binding)
    {
        // TODO: properties key / arguments
        
        // source is ALWAYS an Exchange
        var str = $"{binding.Source}:{binding.DestinationType}:{binding.Destination}:{binding.PropertiesKey}";
        var bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }

    public static RabbitMqSubscriptionParts DecodeRabbitMqParts(string input)
    {
        var bytes = Convert.FromBase64String(input);
        var str = Encoding.UTF8.GetString(bytes);
        var parts = str.Split(':');

        var source = parts[0];
        var destinationType = parts[1];
        var destination = parts[2];
        var propertiesKey = parts[3];
        
        return new RabbitMqSubscriptionParts(source, destinationType, destination, propertiesKey);
    }
};