using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Domain.Apps;

public sealed class App
{
    private App(AppId id, ProjectId projectId, ResourceName name, DateTimeOffset createdAt, DateTimeOffset? removedAt)
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        CreatedAt = createdAt;
        RemovedAt = removedAt;
        Status = new AppStatus(AppPhase.NotDeployed, null, createdAt);
    }

    public AppId Id { get; }

    public ProjectId ProjectId { get; }

    public ResourceName Name { get; }

    public required AppSpec Spec { get; init; }

    public AppStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? RemovedAt { get; private set; }

    public bool IsRemoved => RemovedAt is not null;

    public static App Create(AppId id, ProjectId projectId, ResourceName name, AppSpec spec, DateTimeOffset now) =>
        new(id, projectId, name, now, null) { Spec = spec };

    public Result EnsureActive() => IsRemoved ? AppErrors.Removed(Id) : Result.Success();

    public Result<Release> StartRelease(ReleaseId releaseId, DateTimeOffset now)
    {
        if (IsRemoved)
        {
            return AppErrors.Removed(Id);
        }

        if (Status.Phase == AppPhase.Deploying)
        {
            return AppErrors.DeployInProgress(Id);
        }

        var image = Spec.Source switch
        {
            ImageSource source => source.Reference,
            _ => throw new InvalidOperationException($"App {Id} has an unsupported source."),
        };
        Status = Status with { Phase = AppPhase.Deploying, ObservedAt = now };
        return Release.Queue(releaseId, Id, image, ReleaseTrigger.Manual, now);
    }

    public void GoLive(ReleaseId release, AppPhase phase, DateTimeOffset now) => Status = new AppStatus(phase, release, now);

    public void Observe(AppPhase phase, DateTimeOffset now) => Status = Status with { Phase = phase, ObservedAt = now };

    public void ObserveWorkload(ReleaseId release, AppPhase phase, DateTimeOffset observedAt)
    {
        if (IsRemoved || Status.Phase == AppPhase.Deploying || Status.CurrentRelease != release || observedAt < Status.ObservedAt)
        {
            return;
        }

        Status = Status with { Phase = phase, ObservedAt = observedAt };
    }

    public void Remove(DateTimeOffset now)
    {
        if (IsRemoved)
        {
            return;
        }

        RemovedAt = now;
        Status = Status with { Phase = AppPhase.Stopped, ObservedAt = now };
    }
}
