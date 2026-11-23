using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Runtime;

public sealed record WorkloadObservation(AppPhase Phase, string ImageDigest);
