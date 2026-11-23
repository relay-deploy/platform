using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed class ListReleases(IReleaseRepository releases)
{
    public Task<Page<Release>> Handle(ListReleasesQuery query, CancellationToken cancellationToken) =>
        releases.ListForApp(query.App, query.Page, cancellationToken);
}
