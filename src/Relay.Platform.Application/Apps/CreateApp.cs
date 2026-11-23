using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Projects;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public sealed class CreateApp(IPlatformDbContext db, TimeProvider time)
{
    public async Task<Result<App>> Handle(CreateAppCommand command, CancellationToken cancellationToken)
    {
        var existing = await db.Apps.ById(command.Id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        if (!await db.Projects.Exists(command.Project, cancellationToken))
        {
            return ProjectErrors.NotFound(command.Project);
        }

        var name = ResourceName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        if (await db.Apps.NameTaken(command.Project, name.Value, cancellationToken))
        {
            return AppErrors.NameTaken(name.Value);
        }

        if (await db.Apps.SubdomainTaken(command.Spec.Exposure, cancellationToken))
        {
            return AppErrors.SubdomainTaken(command.Spec.Exposure!.Subdomain);
        }

        var created = App.Create(command.Id, command.Project, name.Value, command.Spec, time.GetUtcNow());
        db.Apps.Add(created);
        await db.SaveChangesAsync(cancellationToken);
        return created;
    }
}
