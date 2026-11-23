using Microsoft.EntityFrameworkCore;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class ListApps(IPlatformDbContext db)
{
    public Task<Page<App>> Handle(ListAppsQuery query, CancellationToken cancellationToken) =>
        db.Apps.AsNoTracking().Live().InProject(query.Project).NewestFirst().ToPage(query.Page, cancellationToken);
}
