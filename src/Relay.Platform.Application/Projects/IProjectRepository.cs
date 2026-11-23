using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public interface IProjectRepository
{
    Task<Project?> Find(ProjectId id, CancellationToken cancellationToken);

    Task<bool> NameTaken(ResourceName name, CancellationToken cancellationToken);

    Task Add(Project project, CancellationToken cancellationToken);

    Task<Page<Project>> List(PageRequest page, CancellationToken cancellationToken);
}
