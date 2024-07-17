using Core.EventStoreDb.Subscriptions.Checkpoints;
using EventStore.Client;

namespace Core.EventStoreDb.Subscriptions.Batch;

public interface IEventsBatchCheckpointer
{
    Task<ISubscriptionCheckpointRepository.StoreResult> Process(
        ResolvedEvent[] events,
        Checkpoint lastCheckpoint,
        BatchProcessingOptions batchProcessingOptions,
        CancellationToken ct
    );
}

public class EventsBatchCheckpointer(
    ISubscriptionCheckpointRepository checkpointRepository,
    EventsBatchProcessor eventsBatchProcessor
): IEventsBatchCheckpointer
{
    public async Task<ISubscriptionCheckpointRepository.StoreResult> Process(
        ResolvedEvent[] events,
        Checkpoint lastCheckpoint,
        BatchProcessingOptions options,
        CancellationToken ct
    )
    {
        var lastPosition = events.LastOrDefault().OriginalPosition?.CommitPosition;

        if (!lastPosition.HasValue)
            return new ISubscriptionCheckpointRepository.StoreResult.Ignored();

        await eventsBatchProcessor.Handle(events, options, ct)
            .ConfigureAwait(false);

        return await checkpointRepository
            .Store(options.SubscriptionId, lastPosition.Value, lastCheckpoint, ct)
            .ConfigureAwait(false);
    }
}
