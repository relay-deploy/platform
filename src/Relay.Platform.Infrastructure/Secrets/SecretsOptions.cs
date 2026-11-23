using System.ComponentModel.DataAnnotations;

namespace Relay.Platform.Infrastructure.Secrets;

public sealed class SecretsOptions
{
    public const string Section = "Relay:Secrets";

    [Required]
    public string ActiveKeyVersion { get; init; } = "";

    [Required]
    public Dictionary<string, string> Keys { get; init; } = [];
}
