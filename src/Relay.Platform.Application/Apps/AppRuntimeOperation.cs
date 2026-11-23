using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class AppRuntimeOperation(IPlatformDbContext db, TimeProvider time)
{
    public async Task<Result<App>> Run(
        AppId id,
        Func<AppId, CancellationToken, Task<Result<WorkloadObservation>>> operation,
        CancellationToken cancellationToken)
    {
        var app = await db.Apps.ById(id, cancellationToken);
        if (app is null)
        {
            return AppErrors.NotFound(id);
        }

        var active = app.EnsureActive();
        if (active.IsFailure)
        {
            return active.Error;
        }

        var observed = await operation(id, cancellationToken);
        if (observed.IsFailure)
        {
            return observed.Error;
        }

        app.Observe(observed.Value.Phase, time.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);
        return app;
    }
}
