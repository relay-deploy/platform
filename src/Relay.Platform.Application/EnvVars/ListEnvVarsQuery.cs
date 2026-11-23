using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Application.EnvVars;

public sealed record ListEnvVarsQuery(EnvVarScope Scope, PageRequest Page);
