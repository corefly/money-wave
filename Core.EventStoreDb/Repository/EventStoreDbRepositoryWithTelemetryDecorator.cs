﻿using Core.Aggregates;
using Core.OpenTelemetry;

namespace Core.EventStoreDb.Repository;

public class EventStoreDbRepositoryWithTelemetryDecorator<T>(
    IEventStoreDbRepository<T> inner,
    IActivityScope activityScope)
    : IEventStoreDbRepository<T>
    where T : class, IAggregate
{
    public Task<T?> Find(Guid id, CancellationToken cancellationToken) =>
        inner.Find(id, cancellationToken);

    public Task<ulong> Add(Guid id, T aggregate, CancellationToken cancellationToken = default) =>
        activityScope.Run($"EventStoreDbRepository/{nameof(Add)}",
            (_, ct) => inner.Add(id, aggregate, ct),
            new StartActivityOptions
            {
                Tags =
                {
                    { TelemetryTags.Logic.Entities.EntityType, typeof(T).Name },
                    { TelemetryTags.Logic.Entities.EntityId, id },
                    { TelemetryTags.Logic.Entities.EntityVersion, aggregate.Version }
                }
            },
            cancellationToken
        );

    public Task<ulong> Update(Guid id, T aggregate, ulong? expectedVersion = null, CancellationToken token = default) =>
        activityScope.Run($"EventStoreDbRepository/{nameof(Update)}",
            (_, ct) => inner.Update(id, aggregate, expectedVersion, ct),
            new StartActivityOptions
            {
                Tags =
                {
                    { TelemetryTags.Logic.Entities.EntityType, typeof(T).Name },
                    { TelemetryTags.Logic.Entities.EntityId, id },
                    { TelemetryTags.Logic.Entities.EntityVersion, aggregate.Version }
                }
            },
            token
        );

    public Task<ulong> Delete(Guid id, T aggregate, ulong? expectedVersion = null, CancellationToken token = default) =>
        activityScope.Run($"EventStoreDbRepository/{nameof(Delete)}",
            (_, ct) => inner.Delete(id, aggregate, expectedVersion, ct),
            new StartActivityOptions
            {
                Tags =
                {
                    { TelemetryTags.Logic.Entities.EntityType, typeof(T).Name },
                    { TelemetryTags.Logic.Entities.EntityId, id },
                    { TelemetryTags.Logic.Entities.EntityVersion, aggregate.Version }
                }
            },
            token
        );
}
