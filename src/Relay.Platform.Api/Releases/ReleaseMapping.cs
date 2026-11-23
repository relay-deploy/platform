using Google.Protobuf.WellKnownTypes;
using Relay.Common.V1;
using Relay.Platform.Domain.Releases;
using ReleaseMessage = Relay.Platform.V1.Release;
using ReleasePhaseMessage = Relay.Platform.V1.ReleasePhase;
using ReleaseTriggerMessage = Relay.Platform.V1.ReleaseTrigger;

namespace Relay.Platform.Api.Releases;

internal static class ReleaseMapping
{
    public static ReleaseMessage ToMessage(this Release release)
    {
        var message = new ReleaseMessage
        {
            Id = release.Id.ToString(),
            AppId = release.AppId.ToString(),
            Image = new Image { Reference = release.Image.Value, Digest = release.ImageDigest ?? "" },
            Trigger = release.Trigger.ToMessage(),
            Phase = release.Phase.ToMessage(),
            CreatedAt = Timestamp.FromDateTimeOffset(release.CreatedAt),
        };
        if (release.Failure is { } failure)
        {
            message.Failure = new Failure { Code = failure.Code, Message = failure.Message };
        }

        if (release.FinishedAt is { } finishedAt)
        {
            message.FinishedAt = Timestamp.FromDateTimeOffset(finishedAt);
        }

        return message;
    }

    private static ReleasePhaseMessage ToMessage(this ReleasePhase phase) => phase switch
    {
        ReleasePhase.Queued => ReleasePhaseMessage.Queued,
        ReleasePhase.Applying => ReleasePhaseMessage.Applying,
        ReleasePhase.Succeeded => ReleasePhaseMessage.Succeeded,
        ReleasePhase.Failed => ReleasePhaseMessage.Failed,
        _ => ReleasePhaseMessage.Unspecified,
    };

    private static ReleaseTriggerMessage ToMessage(this ReleaseTrigger trigger) => trigger switch
    {
        ReleaseTrigger.Manual => ReleaseTriggerMessage.Manual,
        _ => ReleaseTriggerMessage.Unspecified,
    };
}
