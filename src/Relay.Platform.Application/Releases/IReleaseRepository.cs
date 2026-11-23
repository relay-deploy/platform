using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public interface IReleaseRepository
{
    Task<Release?> Find(ReleaseId id, CancellationToken cancellationToken);

    Task Add(Release release, CancellationToken cancellationToken);

    Task Update(Release release, CancellationToken cancellationToken);

    Task<Page<Release>> ListForApp(AppId app, PageRequest page, CancellationToken cancellationToken);
}
