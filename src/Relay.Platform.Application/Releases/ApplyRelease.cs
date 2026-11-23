using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Apps;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Projects;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed class ApplyRelease(IPlatformDbContext db, IContainerRuntime runtime, TimeProvider time)
{
    public async Task<Result> Handle(ApplyReleaseCommand command, CancellationToken cancellationToken)
    {
        var release = await db.Releases.ById(command.Release, cancellationToken);
        if (release is null)
        {
            return ReleaseErrors.NotFound(command.Release);
        }

        if (release.IsFinished)
        {
            return Result.Success();
        }

        var app = await db.Apps.ById(release.AppId, cancellationToken);
        var project = app is null ? null : await db.Projects.AsNoTracking().ById(app.ProjectId, cancellationToken);
        if (app is null || project is null || app.IsRemoved)
        {
            release.Fail(new ReleaseFailure("app.removed", "The app was removed before the release was applied."), time.GetUtcNow());
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        release.BeginApplying();
        await db.SaveChangesAsync(cancellationToken);

        var applied = await runtime.Apply(project, app, release, cancellationToken);
        if (applied.IsSuccess)
        {
            var now = time.GetUtcNow();
            release.Succeed(applied.Value.ImageDigest, now);
            app.GoLive(release.Id, applied.Value.Phase, now);
            await db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        if (applied.Error.Kind == ErrorKind.Unavailable && !command.IsLastAttempt)
        {
            return applied.Error;
        }

        return await Fail(release, app, applied.Error, cancellationToken);
    }

    private static AppPhase PhaseWithoutWorkload(App app) =>
        app.Status.CurrentRelease is null ? AppPhase.NotDeployed : AppPhase.Crashed;

    private async Task<Result> Fail(Release release, App app, Error error, CancellationToken cancellationToken)
    {
        var observed = await runtime.Observe(app.Id, cancellationToken);
        var now = time.GetUtcNow();
        release.Fail(new ReleaseFailure(error.Code, error.Message), now);
        app.Observe(observed.IsSuccess ? observed.Value.Phase : PhaseWithoutWorkload(app), now);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
