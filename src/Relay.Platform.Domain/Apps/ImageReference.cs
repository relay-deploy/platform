using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.Apps;

public sealed record ImageReference
{
    private const int MaxLength = 512;

    private ImageReference(string value) => Value = value;

    public string Value { get; }

    public static Result<ImageReference> Create(string value) =>
        value.Length is > 0 and <= MaxLength && !value.Any(char.IsWhiteSpace)
            ? Result.Success(new ImageReference(value))
            : Error.Validation("image.invalid_reference", $"'{value}' is not a valid image reference.");

    public override string ToString() => Value;
}
