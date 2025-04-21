namespace mcp.RabbitMqManagement;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Convert rabbitmq date times
/// </summary>
public class RabbitMqDateTimeJsonConverter : JsonConverter<DateTime?>
{
    readonly string[] _formats =
    {
        // 2022-08-25 21:06:33 - CloudAMQP
        "yyyy-MM-dd HH:mm:ss",
        // 2022-08-27 0:02:00 - CloudAMQP
        "yyyy-MM-dd H:mm:ss",
        // 2022-08-25T12:46:31.585+00:00
        "yyyy-MM-ddTHH:mm:ss.fffK"
    };

    /// <inheritdoc />
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        if (val == null)
            return null;

        return DateTime.ParseExact(val, _formats, null);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        var fmt = value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        writer.WriteStringValue(fmt);
    }
}