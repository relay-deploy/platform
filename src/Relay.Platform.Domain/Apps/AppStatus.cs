using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Domain.Apps;

public sealed record AppStatus(AppPhase Phase, ReleaseId? CurrentRelease, DateTimeOffset ObservedAt);
