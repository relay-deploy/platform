using Relay.Chassis.Primitives;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class RestartApp(AppRuntimeOperation operation, IContainerRuntime runtime)
{
    public Task<Result<App>> Handle(AppId id, CancellationToken cancellationToken) =>
        operation.Run(id, runtime.Restart, cancellationToken);
}
