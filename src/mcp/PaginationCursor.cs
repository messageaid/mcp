namespace mcp;

using System.Text;

public static class PaginationCursor
{
    public static string FirstCursor()
    {
        return Encode(1, 20);
    }

    public static string NextCursor(string cursor)
    {
        var (page, perPage) = Decode(cursor);
        return Encode(page + 1, perPage);
    }

    public static (int, int) Extract(string? cursor)
    {
        if (cursor == null) return (1, 20);
        
        return Decode(cursor);
    }

    public static string Encode(int page, int perPage)
    {
        var str = $"{page}:{perPage}";
        var bytes = Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(bytes);
    }
    
    static (int, int) Decode(string cursor)
    {
        var bytes = Convert.FromBase64String(cursor);
        var str = Encoding.UTF8.GetString(bytes);
        var parts = str.Split(":");
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }
}