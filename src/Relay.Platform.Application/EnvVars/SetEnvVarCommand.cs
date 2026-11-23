using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Application.EnvVars;

public sealed record SetEnvVarCommand(EnvVarScope Scope, string Name, string Value, bool Secret);
