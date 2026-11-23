using Relay.Platform.Domain.Naming;

namespace Relay.Platform.Domain.Projects;

public sealed class Project(ProjectId id, ResourceName name, DateTimeOffset createdAt)
{
    public ProjectId Id { get; } = id;

    public ResourceName Name { get; } = name;

    public DateTimeOffset CreatedAt { get; } = createdAt;
}
