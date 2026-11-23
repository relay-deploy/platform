using Grpc.Core;
using Microsoft.Extensions.Options;
using Relay.Chassis.Grpc;
using Relay.Chassis.Primitives;
using Relay.Common.V1;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;
using Relay.Runtime.V1;
using App = Relay.Platform.Domain.Apps.App;
using ResourceLimits = Relay.Common.V1.Resources;

namespace Relay.Platform.Infrastructure.Runtime;

public sealed class GrpcContainerRuntime(RuntimeService.RuntimeServiceClient client, IOptions<RuntimeOptions> options) : IContainerRuntime
{
    public Task<Result<WorkloadObservation>> Apply(
        Project project,
        App app,
        Release release,
        IReadOnlyList<EnvVar> environment,
        CancellationToken cancellationToken) =>
        Call(async () =>
        {
            using var call = client.ApplyWorkload(new ApplyWorkloadRequest { Workload = ToWorkload(project, app, release, environment) }, cancellationToken: cancellationToken);
            WorkloadStatus? status = null;
            await foreach (var update in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                if (update.UpdateCase == ApplyWorkloadResponse.UpdateOneofCase.Status)
                {
                    status = update.Status;
                }
            }

            return status is null
                ? Error.Unavailable("runtime.no_status", "The runtime finished applying without reporting a status.")
                : Result.Success(ToObservation(status));
        });

    public Task<Result<WorkloadObservation>> Observe(AppId app, CancellationToken cancellationToken) =>
        Call(async () => Result.Success(ToObservation(
            (await client.GetWorkloadAsync(new GetWorkloadRequest { AppId = app.ToString() }, cancellationToken: cancellationToken)).Status)));

    public Task<Result<WorkloadObservation>> Restart(AppId app, CancellationToken cancellationToken) =>
        Call(async () => Result.Success(ToObservation(
            (await client.RestartWorkloadAsync(new RestartWorkloadRequest { AppId = app.ToString() }, cancellationToken: cancellationToken)).Status)));

    public Task<Result<WorkloadObservation>> Stop(AppId app, CancellationToken cancellationToken) =>
        Call(async () => Result.Success(ToObservation(
            (await client.StopWorkloadAsync(new StopWorkloadRequest { AppId = app.ToString() }, cancellationToken: cancellationToken)).Status)));

    public async Task<Result> Remove(AppId app, CancellationToken cancellationToken)
    {
        try
        {
            await client.RemoveWorkloadAsync(new RemoveWorkloadRequest { AppId = app.ToString() }, cancellationToken: cancellationToken);
            return Result.Success();
        }
        catch (RpcException exception)
        {
            return exception.ToError();
        }
    }

    private static async Task<Result<WorkloadObservation>> Call(Func<Task<Result<WorkloadObservation>>> call)
    {
        try
        {
            return await call();
        }
        catch (RpcException exception)
        {
            return exception.ToError();
        }
    }

    private static WorkloadObservation ToObservation(WorkloadStatus status) =>
        new(WorkloadPhaseMapping.ToAppPhase(status.Phase), status.ImageDigest);

    private Workload ToWorkload(Project project, App app, Release release, IReadOnlyList<EnvVar> environment)
    {
        var spec = new WorkloadSpec
        {
            Image = new Image { Reference = release.Image.Value, Digest = release.ImageDigest ?? "" },
            Resources = new ResourceLimits
            {
                MemoryLimitBytes = (ulong)app.Spec.Resources.MemoryLimitBytes,
                CpuLimitMillicores = (uint)app.Spec.Resources.CpuLimitMillicores,
            },
        };
        spec.Environment.AddRange(environment.Select(envVar => new EnvironmentVariable { Name = envVar.Name.Value, Value = envVar.Value }));
        if (app.Spec.Exposure is { } exposure)
        {
            spec.Routes.Add(new Route { Host = $"{exposure.Subdomain}.{options.Value.AppsDomain}", Port = (uint)exposure.Port });
        }

        return new Workload
        {
            AppId = app.Id.ToString(),
            ProjectId = project.Id.ToString(),
            ReleaseId = release.Id.ToString(),
            Name = $"{project.Name}-{app.Name}",
            Spec = spec,
        };
    }
}
