using Relay.Chassis.Postgres.Jobs;

namespace Relay.Platform.Infrastructure.Releases;

public sealed record ApplyReleaseJob(Guid ReleaseId) : IJob
{
    public static string Kind => "platform.release.apply";
}
