using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Domain.Releases;

public sealed class Release
{
    private Release(
        ReleaseId id,
        AppId appId,
        ImageReference image,
        string? imageDigest,
        ReleaseTrigger trigger,
        ReleasePhase phase,
        ReleaseFailure? failure,
        DateTimeOffset createdAt,
        DateTimeOffset? finishedAt)
    {
        Id = id;
        AppId = appId;
        Image = image;
        ImageDigest = imageDigest;
        Trigger = trigger;
        Phase = phase;
        Failure = failure;
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
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
        new(id, appId, image, null, trigger, ReleasePhase.Queued, null, now, null);

    public static Release Restore(
        ReleaseId id,
        AppId appId,
        ImageReference image,
        string? imageDigest,
        ReleaseTrigger trigger,
        ReleasePhase phase,
        ReleaseFailure? failure,
        DateTimeOffset createdAt,
        DateTimeOffset? finishedAt) =>
        new(id, appId, image, imageDigest, trigger, phase, failure, createdAt, finishedAt);

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
