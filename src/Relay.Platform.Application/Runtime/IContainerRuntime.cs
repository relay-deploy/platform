using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Runtime;

public interface IContainerRuntime
{
    Task<Result<WorkloadObservation>> Apply(Project project, App app, Release release, IReadOnlyList<EnvVar> environment, CancellationToken cancellationToken);

    Task<Result<WorkloadObservation>> Observe(AppId app, CancellationToken cancellationToken);

    Task<Result<WorkloadObservation>> Restart(AppId app, CancellationToken cancellationToken);

    Task<Result<WorkloadObservation>> Stop(AppId app, CancellationToken cancellationToken);

    Task<Result> Remove(AppId app, CancellationToken cancellationToken);
}
