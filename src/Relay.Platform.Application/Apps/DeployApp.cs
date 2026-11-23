using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Apps;

public sealed class DeployApp(
    IAppRepository apps,
    IReleaseRepository releases,
    IReleaseScheduler scheduler,
    IUnitOfWork unitOfWork,
    TimeProvider time)
{
    public async Task<Result<Release>> Handle(DeployAppCommand command, CancellationToken cancellationToken)
    {
        var existing = await releases.Find(command.Release, cancellationToken);
        if (existing is not null)
        {
            return existing.AppId == command.App ? existing : ReleaseErrors.BelongsToAnotherApp(command.Release);
        }

        var app = await apps.Find(command.App, cancellationToken);
        if (app is null)
        {
            return AppErrors.NotFound(command.App);
        }

        var release = app.StartRelease(command.Release, time.GetUtcNow());
        if (release.IsFailure)
        {
            return release;
        }

        await releases.Add(release.Value, cancellationToken);
        await apps.Update(app, cancellationToken);
        await scheduler.Schedule(release.Value.Id, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return release;
    }
}
