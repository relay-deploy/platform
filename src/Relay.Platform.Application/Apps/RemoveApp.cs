using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class RemoveApp(IAppRepository apps, IContainerRuntime runtime, IUnitOfWork unitOfWork, TimeProvider time)
{
    public async Task<Result> Handle(AppId id, CancellationToken cancellationToken)
    {
        var app = await apps.Find(id, cancellationToken);
        if (app is null)
        {
            return AppErrors.NotFound(id);
        }

        if (app.IsRemoved)
        {
            return Result.Success();
        }

        var removed = await runtime.Remove(id, cancellationToken);
        if (removed.IsFailure && removed.Error.Kind != ErrorKind.NotFound)
        {
            return removed;
        }

        app.Remove(time.GetUtcNow());
        await apps.Update(app, cancellationToken);
        await unitOfWork.Commit(cancellationToken);
        return Result.Success();
    }
}
