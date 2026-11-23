namespace Relay.Platform.Domain.Releases;

public readonly record struct ReleaseId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
