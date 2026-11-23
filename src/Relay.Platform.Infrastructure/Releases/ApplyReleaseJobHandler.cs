using Relay.Chassis.Postgres.Jobs;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Releases;

public sealed class ApplyReleaseJobHandler(ApplyRelease applyRelease) : IJobHandler<ApplyReleaseJob>
{
    public Task<Result> Handle(ApplyReleaseJob job, JobAttempt attempt, CancellationToken cancellationToken) =>
        applyRelease.Handle(new ApplyReleaseCommand(new ReleaseId(job.ReleaseId), attempt.IsLast), cancellationToken);
}
