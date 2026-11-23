using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Infrastructure.Apps;

internal static class AppPhaseColumn
{
    public static string From(AppPhase phase) => phase switch
    {
        AppPhase.NotDeployed => "not_deployed",
        AppPhase.Deploying => "deploying",
        AppPhase.Running => "running",
        AppPhase.Stopped => "stopped",
        AppPhase.Crashed => "crashed",
        _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null),
    };

    public static AppPhase To(string value) => value switch
    {
        "not_deployed" => AppPhase.NotDeployed,
        "deploying" => AppPhase.Deploying,
        "running" => AppPhase.Running,
        "stopped" => AppPhase.Stopped,
        "crashed" => AppPhase.Crashed,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };
}
