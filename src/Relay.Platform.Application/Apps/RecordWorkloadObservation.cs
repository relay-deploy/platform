using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;

namespace Relay.Platform.Application.Apps;

public sealed class RecordWorkloadObservation(IPlatformDbContext db)
{
    public async Task<Result> Handle(RecordWorkloadObservationCommand command, CancellationToken cancellationToken)
    {
        var app = await db.Apps.ById(command.App, cancellationToken);
        if (app is null)
        {
            return Result.Success();
        }

        app.ObserveWorkload(command.Release, command.Phase, command.ObservedAt);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
