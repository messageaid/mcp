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
[DebuggerDisplay("{VHost}/{Name}")]
public class RabbitMqHttpExchange
{
    /// <summary>
    /// name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// vhost
    /// </summary>
    [JsonPropertyName("vhost")]
    public string VHost { get; set; }

    /// <summary>
    /// Arguments
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; }

    /// <summary>
    /// Auto Delete
    /// </summary>
    public bool AutoDelete { get; set; }

    /// <summary>
    /// Durable
    /// </summary>
    public bool Durable { get; set; }

    /// <summary>
    /// Internal
    /// </summary>
    public bool Internal { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public string UserWhoPerformedAction { get; set; }
}