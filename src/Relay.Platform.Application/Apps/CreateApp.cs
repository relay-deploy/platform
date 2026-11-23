using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Projects;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public sealed class CreateApp(IAppRepository apps, IProjectRepository projects, IUnitOfWork unitOfWork, TimeProvider time)
{
    public async Task<Result<App>> Handle(CreateAppCommand command, CancellationToken cancellationToken)
    {
        var existing = await apps.Find(command.Id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        if (await projects.Find(command.Project, cancellationToken) is null)
        {
            return ProjectErrors.NotFound(command.Project);
        }

        var name = ResourceName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        if (await apps.NameTaken(command.Project, name.Value, cancellationToken))
        {
            return AppErrors.NameTaken(name.Value);
        }

        if (command.Spec.Exposure is { } exposure && await apps.SubdomainTaken(exposure.Subdomain, cancellationToken))
        {
            return AppErrors.SubdomainTaken(exposure.Subdomain);
        }

        var app = App.Create(command.Id, command.Project, name.Value, command.Spec, time.GetUtcNow());
        await apps.Add(app, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return app;
    }
}
