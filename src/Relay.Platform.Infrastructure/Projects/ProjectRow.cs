using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Infrastructure.Persistence;

namespace Relay.Platform.Infrastructure.Projects;

internal sealed class ProjectRow
{
    public Guid Id { get; init; }

    public string Name { get; init; } = "";

    public DateTime CreatedAt { get; init; }

    public Project ToDomain() => new(new ProjectId(Id), ResourceName.Create(Name).Value, Timestamps.FromColumn(CreatedAt));
}
