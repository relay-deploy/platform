using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Application.EnvVars;

public sealed record DeleteEnvVarCommand(EnvVarScope Scope, string Name);
