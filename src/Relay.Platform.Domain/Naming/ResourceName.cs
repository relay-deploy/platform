using System.Text.RegularExpressions;
using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.Naming;

public sealed partial record ResourceName
{
    private ResourceName(string value) => Value = value;

    public string Value { get; }

    public static Result<ResourceName> Create(string value) =>
        Pattern().IsMatch(value)
            ? Result.Success(new ResourceName(value))
            : Error.Validation("name.invalid", $"'{value}' is not a valid name. Use 1-63 lowercase letters, digits and hyphens, starting with a letter.");

    public override string ToString() => Value;

    [GeneratedRegex("^[a-z]([a-z0-9-]{0,61}[a-z0-9])?$")]
    private static partial Regex Pattern();
}
