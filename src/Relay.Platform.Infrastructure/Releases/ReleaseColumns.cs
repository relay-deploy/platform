using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Releases;

internal static class ReleaseColumns
{
    public static string From(ReleasePhase phase) => phase switch
    {
        ReleasePhase.Queued => "queued",
        ReleasePhase.Applying => "applying",
        ReleasePhase.Succeeded => "succeeded",
        ReleasePhase.Failed => "failed",
        _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null),
    };

    public static ReleasePhase ToPhase(string value) => value switch
    {
        "queued" => ReleasePhase.Queued,
        "applying" => ReleasePhase.Applying,
        "succeeded" => ReleasePhase.Succeeded,
        "failed" => ReleasePhase.Failed,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static string From(ReleaseTrigger trigger) => trigger switch
    {
        ReleaseTrigger.Manual => "manual",
        _ => throw new ArgumentOutOfRangeException(nameof(trigger), trigger, null),
    };

    public static ReleaseTrigger ToTrigger(string value) => value switch
    {
        "manual" => ReleaseTrigger.Manual,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}
