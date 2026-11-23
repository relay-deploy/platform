using Microsoft.EntityFrameworkCore;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public static class ReleaseQueries
{
    public static Task<Release?> ById(this IQueryable<Release> releases, ReleaseId id, CancellationToken cancellationToken) =>
        releases.FirstOrDefaultAsync(release => release.Id == id, cancellationToken);

    public static IQueryable<Release> OfApp(this IQueryable<Release> releases, AppId app) =>
        releases.Where(release => release.AppId == app);

    public static IQueryable<Release> NewestFirst(this IQueryable<Release> releases) =>
        releases.OrderByDescending(release => release.Id);
}
