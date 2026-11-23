namespace Relay.Platform.Domain.Releases;

public enum ReleasePhase
{
    Queued,
    Applying,
    Succeeded,
    Failed,
}
