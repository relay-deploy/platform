using Relay.Chassis.Postgres.Events;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Apps;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;
using Relay.Runtime.V1;

namespace Relay.Platform.Infrastructure.Runtime;

public sealed class WorkloadEventConsumer(RecordWorkloadObservation recordWorkloadObservation)
    : IEventConsumer<WorkloadCrashed>, IEventConsumer<WorkloadRecovered>, IEventConsumer<WorkloadStopped>
{
    public Task<Result> Handle(WorkloadCrashed message, EventEnvelope envelope, CancellationToken cancellationToken) =>
        Record(message.AppId, message.ReleaseId, AppPhase.Crashed, envelope, cancellationToken);

    public Task<Result> Handle(WorkloadRecovered message, EventEnvelope envelope, CancellationToken cancellationToken) =>
        Record(message.AppId, message.ReleaseId, AppPhase.Running, envelope, cancellationToken);

    public Task<Result> Handle(WorkloadStopped message, EventEnvelope envelope, CancellationToken cancellationToken) =>
        Record(message.AppId, message.ReleaseId, AppPhase.Stopped, envelope, cancellationToken);

    private Task<Result> Record(string appId, string releaseId, AppPhase phase, EventEnvelope envelope, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(appId, out var app) || !Guid.TryParse(releaseId, out var release))
        {
            return Task.FromResult<Result>(Error.Validation("workload_event.invalid_id", $"Workload event {envelope.Id} has an invalid app or release id."));
        }

        var command = new RecordWorkloadObservationCommand(new AppId(app), new ReleaseId(release), phase, envelope.OccurredAt);
        return recordWorkloadObservation.Handle(command, cancellationToken);
    }
}
