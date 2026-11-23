using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed class GetRelease(IReleaseRepository releases)
{
    public async Task<Result<Release>> Handle(ReleaseId id, CancellationToken cancellationToken)
    {
        var release = await releases.Find(id, cancellationToken);
        return release is null ? ReleaseErrors.NotFound(id) : release;
    }
}
