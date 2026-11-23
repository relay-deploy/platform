using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public interface IAppRepository
{
    Task<App?> Find(AppId id, CancellationToken cancellationToken);

    Task<bool> NameTaken(ProjectId project, ResourceName name, CancellationToken cancellationToken);

    Task<bool> SubdomainTaken(ResourceName subdomain, CancellationToken cancellationToken);

    Task Add(App app, CancellationToken cancellationToken);

    Task Update(App app, CancellationToken cancellationToken);

    Task<Page<App>> List(ProjectId? project, PageRequest page, CancellationToken cancellationToken);
}
