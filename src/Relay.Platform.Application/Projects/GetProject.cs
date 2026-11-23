using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class GetProject(IPlatformDbContext db)
{
    public async Task<Result<Project>> Handle(ProjectId id, CancellationToken cancellationToken)
    {
        var project = await db.Projects.AsNoTracking().ById(id, cancellationToken);
        return project is null ? ProjectErrors.NotFound(id) : project;
    }
}
