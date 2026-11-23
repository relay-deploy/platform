using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public interface IReleaseScheduler
{
    void Schedule(ReleaseId release);
}
