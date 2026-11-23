using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Relay.Platform.Infrastructure.Secrets;

public sealed partial class SecretsOptionsValidator : IValidateOptions<SecretsOptions>
{
    public ValidateOptionsResult Validate(string? name, SecretsOptions options)
    {
        var failures = new List<string>();
        foreach (var (version, encoded) in options.Keys)
        {
            if (!VersionPattern().IsMatch(version))
            {
                failures.Add($"{SecretsOptions.Section}:Keys:{version} is not a valid key version. Use letters and digits only.");
            }

            if (!SecretKeys.TryParse(encoded, out _))
            {
                failures.Add($"{SecretsOptions.Section}:Keys:{version} must be {SecretKeys.KeySizeBytes} bytes, base64 encoded.");
            }
        }

        if (!options.Keys.ContainsKey(options.ActiveKeyVersion))
        {
            failures.Add($"{SecretsOptions.Section}:ActiveKeyVersion '{options.ActiveKeyVersion}' has no matching entry in Keys.");
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    [GeneratedRegex(@"^[A-Za-z0-9]+\z")]
    private static partial Regex VersionPattern();
}
