namespace Relay.Platform.Domain.Apps;

public sealed record AppSpec
{
    public required AppSource Source { get; init; }

    public Exposure? Exposure { get; init; }

    public required Resources Resources { get; init; }
}
