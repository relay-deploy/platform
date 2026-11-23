using Relay.Chassis.Postgres.Jobs;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Releases;

public sealed class JobReleaseScheduler(IJobScheduler jobs) : IReleaseScheduler
{
    public Task Schedule(ReleaseId release, CancellationToken cancellationToken) =>
        jobs.Schedule(new ApplyReleaseJob(release.Value), $"release:{release}", cancellationToken);
}
