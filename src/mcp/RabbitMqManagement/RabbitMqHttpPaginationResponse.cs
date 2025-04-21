namespace mcp.RabbitMqManagement;

using System.Diagnostics;

/// <summary>
/// paginated response
/// </summary>
[DebuggerDisplay("{Page}/{PageCount}")]
public class RabbitMqHttpPaginationResponse<T>
{
    /// <summary>
    /// the number post filter
    /// </summary>
    public int FilteredCount { get; set; }

    /// <summary>
    /// total items
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// current page of items
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// current page
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// number of pages
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// size of the page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// total records
    /// </summary>
    public int TotalCount { get; set; }
}