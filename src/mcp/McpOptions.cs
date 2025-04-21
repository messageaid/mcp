namespace mcp;

public class McpOptions
{
    public string? Url { get; set; }
    public McpMode Mode { get; set; } = McpMode.ReadOnly;
}