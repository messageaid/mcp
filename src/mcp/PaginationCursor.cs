namespace mcp;

using System.Text;

public record PaginationCursor(ResourceTypes ResourceType, string? LastName, int PerPage)
{
    public const int DefaultPerPage = 25;
    
    public static PaginationCursor Extract(string? cursor)
    {
        if (cursor == null) return new(ResourceTypes.Queue, "", DefaultPerPage);
        
        return Decode(cursor);
    }

    public static string Encode(ResourceTypes resourceType, string? lastName, int perPage)
    {
        lastName ??= "";
        
        var str = $"{resourceType}:{lastName}:{perPage}";
        var bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }
    
    static PaginationCursor Decode(string cursor)
    {
        var bytes = Convert.FromBase64String(cursor);
        var str = Encoding.UTF8.GetString(bytes);
        var parts = str.Split(":");

        var t = Enum.Parse<ResourceTypes>(parts[0]);
        var ln = parts[1];
        var pp = int.Parse(parts[2]);
        return new PaginationCursor(t, ln, pp);
    }
}
