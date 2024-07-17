using Core.Events;
using Core.EventStoreDb.Serialization;
using EventStore.Client;

namespace Core.EventStoreDb.Events;

public static class EventEnvelopeExtensions
{
    public static IEventEnvelope? ToEventEnvelope(this ResolvedEvent resolvedEvent)
    {
        var eventData = resolvedEvent.Deserialize();
        var eventMetadata = resolvedEvent.DeserializePropagationContext();

        if (eventData == null)
            return null;

        var metaData = new EventMetadata(
            resolvedEvent.Event.EventId.ToString(),
            resolvedEvent.Event.EventNumber.ToUInt64(),
            resolvedEvent.Event.Position.CommitPosition,
            eventMetadata
        );

        return EventEnvelope.From(eventData, metaData);
    }
}
