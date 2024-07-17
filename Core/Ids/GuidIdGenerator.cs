namespace Core.Ids;

public class GuidIdGenerator : IIdGenerator
{
    public Guid New() => Guid.NewGuid();
}
