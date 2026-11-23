using Relay.Platform.Domain.Apps;
using Relay.Runtime.V1;

namespace Relay.Platform.Infrastructure.Runtime;

internal static class WorkloadPhaseMapping
{
    public static AppPhase ToAppPhase(WorkloadPhase phase) => phase switch
    {
        WorkloadPhase.Running => AppPhase.Running,
        WorkloadPhase.Stopped => AppPhase.Stopped,
        WorkloadPhase.Restarting or WorkloadPhase.Crashed => AppPhase.Crashed,
        _ => AppPhase.Deploying,
    };
}
