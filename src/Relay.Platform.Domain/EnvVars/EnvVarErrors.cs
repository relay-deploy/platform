using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.EnvVars;

public static class EnvVarErrors
{
    public static Error InvalidName(string name) =>
        Error.Validation("env_var.invalid_name", $"'{name}' is not a valid variable name. Use up to 255 letters, digits and underscores, not starting with a digit.");

    public static Error ScopeRequired() =>
        Error.Validation("env_var.scope_required", "The scope must name exactly one app or project.");

    public static Error NotFound(EnvVarName name) =>
        Error.NotFound("env_var.not_found", $"Variable {name} does not exist in this scope.");
}
