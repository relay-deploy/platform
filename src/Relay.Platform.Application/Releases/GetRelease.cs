using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed class GetRelease(IPlatformDbContext db)
{
    public async Task<Result<Release>> Handle(ReleaseId id, CancellationToken cancellationToken)
    {
        var release = await db.Releases.AsNoTracking().ById(id, cancellationToken);
        return release is null ? ReleaseErrors.NotFound(id) : release;
    }
}
