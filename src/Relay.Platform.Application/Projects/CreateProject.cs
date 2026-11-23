using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class CreateProject(IPlatformDbContext db, TimeProvider time)
{
    public async Task<Result<Project>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var existing = await db.Projects.ById(command.Id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var name = ResourceName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        if (await db.Projects.NameTaken(name.Value, cancellationToken))
        {
            return ProjectErrors.NameTaken(name.Value);
        }

        var created = new Project(command.Id, name.Value, time.GetUtcNow());
        db.Projects.Add(created);
        await db.SaveChangesAsync(cancellationToken);
        return created;
    }
}
