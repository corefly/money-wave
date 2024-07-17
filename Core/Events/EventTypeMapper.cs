using System.Collections.Concurrent;
using Core.Reflection;

namespace Core.Events;

public class EventTypeMapper
{
    public static readonly EventTypeMapper Instance = new();

    private readonly ConcurrentDictionary<string, Type?> _typeMap = new();
    private readonly ConcurrentDictionary<Type, string> _typeNameMap = new();

    public void AddCustomMap<T>(string eventTypeName) => AddCustomMap(typeof(T), eventTypeName);

    public void AddCustomMap(Type eventType, string eventTypeName)
    {
        _typeNameMap.AddOrUpdate(eventType, eventTypeName, (_, typeName) => typeName);
        _typeMap.AddOrUpdate(eventTypeName, eventType, (_, type) => type);
    }

    public string ToName<TEventType>() => ToName(typeof(TEventType));

    public string ToName(Type eventType) => _typeNameMap.GetOrAdd(eventType, _ =>
    {
        var eventTypeName = eventType.FullName!;

        _typeMap.TryAdd(eventTypeName, eventType);

        return eventTypeName;
    });

    public Type? ToType(string eventTypeName) => _typeMap.GetOrAdd(eventTypeName, _ =>
    {
        var type = TypeProvider.GetFirstMatchingTypeFromCurrentDomainAssembly(eventTypeName);

        if (type == null)
            return null;

        _typeNameMap.TryAdd(type, eventTypeName);

        return type;
    });
}
