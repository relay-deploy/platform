using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class ListProjects(IProjectRepository projects)
{
    public Task<Page<Project>> Handle(PageRequest page, CancellationToken cancellationToken) =>
        projects.List(page, cancellationToken);
}
