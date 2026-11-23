using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Relay.Platform.Infrastructure.Secrets;

public sealed class AesGcmSecretCipher(IOptions<SecretsOptions> options) : ISecretCipher
{
    private const char VersionSeparator = ':';
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;

    private readonly string activeVersion = options.Value.ActiveKeyVersion;
    private readonly Dictionary<string, byte[]> keys = options.Value.Keys.ToDictionary(entry => entry.Key, entry => Convert.FromBase64String(entry.Value));

    public string Encrypt(string plaintext)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plaintext);
        var payload = new byte[NonceSizeBytes + plainBytes.Length + TagSizeBytes];
        var nonce = payload.AsSpan(0, NonceSizeBytes);
        var cipherText = payload.AsSpan(NonceSizeBytes, plainBytes.Length);
        var tag = payload.AsSpan(NonceSizeBytes + plainBytes.Length, TagSizeBytes);
        RandomNumberGenerator.Fill(nonce);
        using var aes = new AesGcm(keys[activeVersion], TagSizeBytes);
        aes.Encrypt(nonce, plainBytes, cipherText, tag);
        return $"{activeVersion}{VersionSeparator}{Convert.ToBase64String(payload)}";
    }

    public string Decrypt(string stored)
    {
        var separator = stored.IndexOf(VersionSeparator, StringComparison.Ordinal);
        if (separator < 0 || !keys.TryGetValue(stored[..separator], out var key))
        {
            throw new CryptographicException("The stored secret names an unknown key version.");
        }

        var payload = Convert.FromBase64String(stored[(separator + 1)..]);
        if (payload.Length < NonceSizeBytes + TagSizeBytes)
        {
            throw new CryptographicException("The stored secret is too short.");
        }

        var cipherLength = payload.Length - NonceSizeBytes - TagSizeBytes;
        var plainBytes = new byte[cipherLength];
        using var aes = new AesGcm(key, TagSizeBytes);
        aes.Decrypt(
            payload.AsSpan(0, NonceSizeBytes),
            payload.AsSpan(NonceSizeBytes, cipherLength),
            payload.AsSpan(NonceSizeBytes + cipherLength, TagSizeBytes),
            plainBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
