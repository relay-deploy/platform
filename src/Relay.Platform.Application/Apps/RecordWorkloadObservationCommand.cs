using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Apps;

public sealed record RecordWorkloadObservationCommand(AppId App, ReleaseId Release, AppPhase Phase, DateTimeOffset ObservedAt);
