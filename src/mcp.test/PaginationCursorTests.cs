namespace mcp.test;

public class PaginationCursorTests
{
    [Test]
    public void NullStartingPoint()
    {
        var pc = PaginationCursor.Extract(null);
        
        Assert.That(pc.ResourceType, Is.EqualTo(ResourceTypes.Queue));
        Assert.That(pc.LastName, Is.EqualTo(""));
        Assert.That(pc.PerPage, Is.EqualTo(PaginationCursor.DefaultPerPage));
    }
    
    [Test]
    public void ValidSetUp()
    {
        var encoded = PaginationCursor.Encode(ResourceTypes.Queue, "Abc", 12);
        
        var pc = PaginationCursor.Extract(encoded);
        
        Assert.That(pc.ResourceType, Is.EqualTo(ResourceTypes.Queue));
        Assert.That(pc.LastName, Is.EqualTo("Abc"));
        Assert.That(pc.PerPage, Is.EqualTo(12));
    }
}