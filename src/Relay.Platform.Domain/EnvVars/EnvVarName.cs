using System.Text.RegularExpressions;
using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.EnvVars;

public sealed partial record EnvVarName
{
    private const int MaxLength = 255;

    private EnvVarName(string value) => Value = value;

    public string Value { get; }

    public static Result<EnvVarName> Create(string value) =>
        value.Length <= MaxLength && Pattern().IsMatch(value)
            ? Result.Success(new EnvVarName(value))
            : EnvVarErrors.InvalidName(value);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*\z")]
    private static partial Regex Pattern();
}
