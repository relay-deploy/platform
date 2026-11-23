using Microsoft.EntityFrameworkCore;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed class ListReleases(IPlatformDbContext db)
{
    public Task<Page<Release>> Handle(ListReleasesQuery query, CancellationToken cancellationToken) =>
        db.Releases.AsNoTracking().OfApp(query.App).NewestFirst().ToPage(query.Page, cancellationToken);
}
