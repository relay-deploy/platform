using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed record CreateProjectCommand(ProjectId Id, string Name);
