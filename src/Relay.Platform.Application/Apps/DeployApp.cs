using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Apps;

public sealed class DeployApp(IPlatformDbContext db, IReleaseScheduler scheduler, TimeProvider time)
{
    public async Task<Result<Release>> Handle(DeployAppCommand command, CancellationToken cancellationToken)
    {
        var existing = await db.Releases.AsNoTracking().ById(command.Release, cancellationToken);
        if (existing is not null)
        {
            return existing.AppId == command.App ? existing : ReleaseErrors.BelongsToAnotherApp(command.Release);
        }

        var app = await db.Apps.ById(command.App, cancellationToken);
        if (app is null)
        {
            return AppErrors.NotFound(command.App);
        }

        var release = app.StartRelease(command.Release, time.GetUtcNow());
        if (release.IsFailure)
        {
            return release;
        }

        db.Releases.Add(release.Value);
        scheduler.Schedule(release.Value.Id);
        await db.SaveChangesAsync(cancellationToken);
        return release;
    }
}
