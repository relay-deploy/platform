using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public interface IReleaseScheduler
{
    Task Schedule(ReleaseId release, CancellationToken cancellationToken);
}
