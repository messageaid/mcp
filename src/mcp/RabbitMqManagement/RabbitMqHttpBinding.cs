// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
#pragma warning disable CS8618
namespace mcp.RabbitMqManagement;

using System.Diagnostics;
using System.Text.Json.Serialization;

/// <summary>
/// This is a DTO for the RabbitMQManagement API
/// </summary>
[DebuggerDisplay("{DebuggerDisplay()}")]
public class RabbitMqHttpBinding
{
    /// <summary>
    /// vhost
    /// </summary>
    [JsonPropertyName("vhost")]
    public string VHost { get; set; }

    /// <summary>
    /// source
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// destination
    /// </summary>
    public string Destination { get; set; }

    /// <summary>
    /// type
    /// </summary>
    public string DestinationType { get; set; }

    /// <summary>
    /// routing key
    /// </summary>
    public string RoutingKey { get; set; }

    /// <summary>
    /// arguments
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; }

    /// <summary>
    /// Properties Key
    /// </summary>
    public string PropertiesKey { get; set; }

    public string DebuggerDisplay()
    {
        var src = Source;
        if (src == "")
            src = "(direct)";
        return $"{VHost}/{src}->{Destination}";
    }
}