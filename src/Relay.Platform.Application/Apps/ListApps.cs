using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class ListApps(IAppRepository apps)
{
    public Task<Page<App>> Handle(ListAppsQuery query, CancellationToken cancellationToken) =>
        apps.List(query.Project, query.Page, cancellationToken);
}
