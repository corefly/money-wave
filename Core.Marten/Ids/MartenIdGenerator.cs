using Core.Ids;
using Marten;
using Marten.Schema.Identity;

namespace Core.Marten.Ids;

public class MartenIdGenerator(IDocumentSession documentSession): IIdGenerator
{
    private readonly IDocumentSession _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));

    public Guid New() => CombGuidIdGeneration.NewGuid();
}
