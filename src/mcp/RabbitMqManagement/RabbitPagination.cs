namespace mcp.RabbitMqManagement;

/// <summary>
/// pagination parameters for RabbitMQ
/// </summary>
public class RabbitPagination
{
    /// <summary>
    /// ?page=
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// ?page_size=
    /// </summary>
    public int PageSize { get; set; } = 100;

    /// <summary>
    /// ?name=
    /// </summary>
    public string? NameFilter { get; set; }

    /// <summary>
    /// ?use_regex=true
    /// </summary>
    public bool UseRegex { get; set; } = false;

    /// <summary>
    /// default options
    /// </summary>
    public static RabbitPagination Default()
    {
        return new RabbitPagination();
    }

    /// <summary>
    /// build query string
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string?> ToQueryString()
    {
        var dict = new Dictionary<string, string?>();
        dict.Add("page", Page.ToString());
        dict.Add("page_size", PageSize.ToString());

        if (NameFilter != null)
        {
            dict.Add("name", NameFilter);
            dict.Add("use_regex", UseRegex.ToString().ToLower());
        }

        return dict;
    }
}