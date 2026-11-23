using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class CreateProject(IProjectRepository projects, IUnitOfWork unitOfWork, TimeProvider time)
{
    public async Task<Result<Project>> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var existing = await projects.Find(command.Id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var name = ResourceName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        if (await projects.NameTaken(name.Value, cancellationToken))
        {
            return ProjectErrors.NameTaken(name.Value);
        }

        var project = new Project(command.Id, name.Value, time.GetUtcNow());
        await projects.Add(project, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return project;
    }
}
