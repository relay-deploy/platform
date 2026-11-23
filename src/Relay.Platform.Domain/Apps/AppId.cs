namespace Relay.Platform.Domain.Apps;

public readonly record struct AppId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
