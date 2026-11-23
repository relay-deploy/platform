namespace Relay.Platform.Domain.EnvVars;

public readonly record struct EnvVarId(Guid Value)
{
    public static EnvVarId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
