using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Domain.Releases;

public sealed class Release
{
    private Release(ReleaseId id, AppId appId, ImageReference image, ReleaseTrigger trigger, ReleasePhase phase, DateTimeOffset createdAt)
    {
        Id = id;
        AppId = appId;
        Image = image;
        Trigger = trigger;
        Phase = phase;
        CreatedAt = createdAt;
    }

    public ReleaseId Id { get; }

    public AppId AppId { get; }

    public ImageReference Image { get; }

    public string? ImageDigest { get; private set; }

    public ReleaseTrigger Trigger { get; }

    public ReleasePhase Phase { get; private set; }

    public ReleaseFailure? Failure { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? FinishedAt { get; private set; }

    public bool IsFinished => Phase is ReleasePhase.Succeeded or ReleasePhase.Failed;

    public static Release Queue(ReleaseId id, AppId appId, ImageReference image, ReleaseTrigger trigger, DateTimeOffset now) =>
        new(id, appId, image, trigger, ReleasePhase.Queued, now);


    public void BeginApplying() => Phase = ReleasePhase.Applying;

    public void Succeed(string imageDigest, DateTimeOffset now)
    {
        ImageDigest = imageDigest;
        Phase = ReleasePhase.Succeeded;
        FinishedAt = now;
    }

    public void Fail(ReleaseFailure failure, DateTimeOffset now)
    {
        Failure = failure;
        Phase = ReleasePhase.Failed;
        FinishedAt = now;
    }
}
