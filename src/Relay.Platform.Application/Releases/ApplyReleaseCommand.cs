using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public sealed record ApplyReleaseCommand(ReleaseId Release, bool IsLastAttempt);
