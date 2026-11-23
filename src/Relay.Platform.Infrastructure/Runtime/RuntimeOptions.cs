using System.ComponentModel.DataAnnotations;

namespace Relay.Platform.Infrastructure.Runtime;

public sealed class RuntimeOptions
{
    public const string Section = "Relay:Runtime";

    [Required]
    public Uri? Address { get; init; }

    [Required]
    [RegularExpression("^[a-z0-9]([a-z0-9.-]*[a-z0-9])?$")]
    public string AppsDomain { get; init; } = "";
}
