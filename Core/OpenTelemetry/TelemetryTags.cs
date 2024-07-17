namespace Core.OpenTelemetry;

public static class TelemetryTags
{
    public static class Logic
    {
        public static class  Entities
        {
            public const string Entity = $"{ActivitySourceProvider.DefaultSourceName}.entity";
            public const string EntityType = $"{Entity}.type";
            public const string EntityId = $"{Entity}.id";
            public const string EntityVersion = $"{Entity}.version";
        }

        public const string Stream = $"{ActivitySourceProvider.DefaultSourceName}.stream";
    }

    public static class Commands
    {
        public const string Command = $"{ActivitySourceProvider.DefaultSourceName}.command";
        public const string CommandType = $"{Command}.type";
        public const string CommandsMeter = $"{ActivitySourceProvider.DefaultSourceName}.commands";
        public const string CommandHandling = $"{CommandsMeter}.handling";
        public const string ActiveCommandsNumber = $"{CommandHandling}.active.number";
        public const string TotalCommandsNumber = $"{CommandHandling}.total";
        public const string CommandHandlingDuration = $"{CommandHandling}.duration";
    }

    public static class QueryHandling
    {
        public const string Query = $"{ActivitySourceProvider.DefaultSourceName}.query";
    }

    public static class EventHandling
    {
        public const string Event = $"{ActivitySourceProvider.DefaultSourceName}.event";
    }
}
