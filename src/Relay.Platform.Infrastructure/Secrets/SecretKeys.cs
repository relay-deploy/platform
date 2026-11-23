using System.Security.Cryptography;

namespace Relay.Platform.Infrastructure.Secrets;

internal static class SecretKeys
{
    public const int KeySizeBytes = 32;

    public static bool TryParse(string encoded, out byte[] key)
    {
        key = new byte[KeySizeBytes];
        return Convert.TryFromBase64String(encoded, key, out var written) && written == KeySizeBytes;
    }

    public static byte[] Generate() => RandomNumberGenerator.GetBytes(KeySizeBytes);
}
