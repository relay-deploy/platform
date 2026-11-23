namespace Relay.Platform.Domain.Apps;

public enum AppPhase
{
    NotDeployed,
    Deploying,
    Running,
    Stopped,
    Crashed,
}
