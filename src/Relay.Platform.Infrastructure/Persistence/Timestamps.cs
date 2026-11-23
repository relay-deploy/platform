namespace Relay.Platform.Infrastructure.Persistence;

internal static class Timestamps
{
    public static DateTimeOffset FromColumn(DateTime value) => new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    public static DateTimeOffset? FromColumn(DateTime? value) => value is { } present ? FromColumn(present) : null;
}
