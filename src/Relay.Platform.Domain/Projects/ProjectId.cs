namespace Relay.Platform.Domain.Projects;

public readonly record struct ProjectId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
