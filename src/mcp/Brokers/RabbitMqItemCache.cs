namespace mcp;

public class RabbitMqItemCache(ILogger<RabbitMqItemCache> logger)
{
    readonly SortedList<RabbitMqItem, RabbitMqItem> _items = new();

    public void Ensure(ResourceTypes resourceType, string name, Uri uri)
    {
        logger.LogInformation("Storing {T}/{N}", resourceType, name);
        var v = new RabbitMqItem(resourceType, name, uri);
        _items.Add(v, v);    
    }

    public PagedSet NextPage(PaginationCursor cursor)
    {
        if (_items.Count == 0)
            return new PagedSet(new List<RabbitMqItem>(), PaginationCursor.Encode(ResourceTypes.Queue, null, 20));
        var query = _items.AsEnumerable();
            

        if (cursor.ResourceType == ResourceTypes.Topic)
        {
            query = query.Where(x => x.Key.ResourceType == ResourceTypes.Topic);
        }
        
        if (!string.IsNullOrEmpty(cursor.LastName))
        {
            query = query
                // starting name
                .SkipWhile(x => string.Compare(x.Key.Name, cursor.LastName, StringComparison.Ordinal) > 0);
        }

        var items = query.Take(cursor.PerPage)
            .Select(x => x.Value)
            .ToList();

        var last = items.Last();

        return new PagedSet(
            items,
            PaginationCursor.Encode(last.ResourceType, last.Name, cursor.PerPage)
        );
    }
}

public record PagedSet(List<RabbitMqItem> Items, string Cursor);

public record RabbitMqItem(ResourceTypes ResourceType, string Name, Uri Uri)
    : IComparable<RabbitMqItem>
{
    public string ToUri()
    {
        if (ResourceType == ResourceTypes.Queue)
            return QueueUriTemplate.Create(Uri.Host, Name);

        return TopicUriTemplate.Create(Uri.Host, Name);
    }

    public int CompareTo(RabbitMqItem? other)
    {
        if (other == null) return 1;
        
        // First compare by ResourceType (Queue = 0, Topic = 1)
        int typeComparison = ResourceType.CompareTo(other.ResourceType);
        if (typeComparison != 0)
            return typeComparison;
        
        // Then compare by Name (ordinal string comparison)
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}