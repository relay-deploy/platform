using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class GetProject(IProjectRepository projects)
{
    public async Task<Result<Project>> Handle(ProjectId id, CancellationToken cancellationToken)
    {
        var project = await projects.Find(id, cancellationToken);
        return project is null ? ProjectErrors.NotFound(id) : project;
    }
}
